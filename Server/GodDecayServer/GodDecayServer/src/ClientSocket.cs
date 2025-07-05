using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// 客户端连接对象,负责和客户端进行通讯
/// 
/// 这里才是处理和服务器受发相关业务的类
/// </summary>

namespace GodDecayServer
{
    public class ClientSocket
    {
        private Socket m_Socket;
        private ServerCharacter m_SocketRole;

        //用于数据转换
        private MMO_MemoryStream m_ReceiveMS;

        //接收消息线程
        private Thread m_ReveiveThread;
        //用于接收的缓冲
        private byte[] m_ReceiveBuffer = new byte[1024];

        //====================================================

        //发消息的队列
        private Queue<byte[]> m_SendQueue = new Queue<byte[]>();
        //检查队列的委托
        private Action m_CheckSendQueue;
        //定义传递的消息字节长度是否可以被压缩的界限
        private int m_CompressLen = 200;

        //=========================协议【应该加锁】
        private ServerAreaController m_AreaController = null;
        private StoreGoodsProtocolController m_StoreGoodsProtocolController = null;

        public ClientSocket(Socket socket, ServerCharacter role) 
        {
            m_Socket = socket;
            m_SocketRole = role;
            m_SocketRole.Client_Socket = this;
            m_ReceiveMS = new MMO_MemoryStream();

            m_ReveiveThread = new Thread(ReceiveMsg);
            m_ReveiveThread.Start();

            m_CheckSendQueue = OnCheckSendQueueCallBack;

        }


        #region 接收客户端消息
        //服务器接收客户端发来的消息
        private void ReceiveMsg() 
        {
            //异步接收数据
            m_Socket.BeginReceive(m_ReceiveBuffer, 0, m_ReceiveBuffer.Length, SocketFlags.None, ReceiveCallBack, m_Socket);
        }

        //接收数据回调
        private void ReceiveCallBack(IAsyncResult ar) 
        {
            try 
            {
                int len = m_Socket.EndReceive(ar);
                /*
                 * 这里做一个说明
                 * 开启连接时len传进来的长度不为0因为建立的是TCP连接，包头等信息保证传输的数据是大于0的
                 * 而当没有建立连接时，len长度自然为0，因为没有信息的传入
                 */
                if (len > 0)
                {
                    //说明有数据

                    //把接收到的数据写入数据尾部
                    m_ReceiveMS.Position = m_ReceiveMS.Length;
                    //指定数据长度然后写入数据流
                    m_ReceiveMS.Write(m_ReceiveBuffer, 0, len);
                    //因为我们采用ushort进行接收，而该类型的字节长度为2,说明有包传输过来
                    if (m_ReceiveMS.Length > 2) 
                    {
                        while (true) 
                        {
                            //先把指针先移动到开头
                            m_ReceiveMS.Position = 0;
                            //读取包体长度
                            int currMsgLen = m_ReceiveMS.ReadUShort();
                            //包的总长
                            int currFullMsgLen = 2 + currMsgLen;

                            //如果当前接收到的包长等于接收到包头中包含的长度说明刚好能接收到一个完整的消息，那就进行拆包操作
                            if (m_ReceiveMS.Length >= currFullMsgLen)
                            {
                                //定义一个用于接收消息的缓冲内存
                                byte[] buffers = new byte[currMsgLen];
                                //跳过包头，接收包体消息
                                m_ReceiveMS.Position = 2;
                                //把信息接收到定义的缓冲中
                                m_ReceiveMS.Read(buffers, 0, currMsgLen);

                                //开始解包
                                byte[] bufferNew = new byte[buffers.Length - 3];
                                bool isCompress = false;
                                ushort crc = 0;
                                using (MMO_MemoryStream ms = new MMO_MemoryStream(buffers))
                                {
                                    isCompress = ms.ReadBool();
                                    crc = ms.ReadUShort();
                                    ms.Read(bufferNew, 0, bufferNew.Length);
                                }
                                //用于校验数据是否完整
                                int newCrc = Crc16.CalculateCrc16(bufferNew);
                                if (newCrc == crc)
                                {
                                    //先异或
                                    bufferNew = SecurityUtil.Xor(bufferNew);
                                    //在看是否要解压
                                    if (isCompress)
                                    {
                                        bufferNew = ZlibHelper.DeCompressBytes(bufferNew);
                                    }
                                    //解压过后就可以正常读取数据包了
                                    //接收协议数据
                                    ushort protoCode = 0;
                                    byte[] protoContent = new byte[bufferNew.Length - 2];
                                    using (MMO_MemoryStream ms = new MMO_MemoryStream(bufferNew))
                                    {
                                        protoCode = ms.ReadUShort();
                                        if (protoCode == 0)
                                            m_SocketRole.m_CharacterType = CharacterType.LandR;
                                        else
                                            m_SocketRole.m_CharacterType = CharacterType.GamePlayer;

                                        ms.Read(protoContent, 0, protoContent.Length);
                                        //添加移除应该是协议类做的事情，但登录注册属于特殊情况，所以这里做if特殊处理

                                        //先把该协议添加进观察者
                                        if (protoCode == 0)
                                            EventDispatcher.Instance.AddEventListener(protoCode, m_SocketRole.LoginAccount);
                                        else if (protoCode == 1)
                                            EventDispatcher.Instance.AddEventListener(protoCode, m_SocketRole.RegisterAccount);
                                        else if (protoCode == 2)
                                        {
                                            m_AreaController = new ServerAreaController();
                                            EventDispatcher.Instance.AddEventListener(protoCode, m_AreaController.ServerAreaRenturn);
                                        }
                                        else if (protoCode == 3)
                                        {
                                            m_AreaController = new ServerAreaController();
                                            EventDispatcher.Instance.AddEventListener(protoCode, m_AreaController.ServerAreaRenturnResult);
                                        }
                                        else if (protoCode == 4) 
                                        {
                                            m_StoreGoodsProtocolController = new StoreGoodsProtocolController();
                                        }

                                        //观察者监听[这里有问题]
                                        EventDispatcher.Instance.Dispatch(protoCode, m_SocketRole, protoContent);

                                        //处理完毕就移除掉
                                        if (protoCode == 0)
                                            EventDispatcher.Instance.RemoveEventListener(protoCode, m_SocketRole.LoginAccount);
                                        else if (protoCode == 1)
                                            EventDispatcher.Instance.RemoveEventListener(protoCode, m_SocketRole.RegisterAccount);
                                        else if (protoCode == 2)
                                        {
                                            EventDispatcher.Instance.RemoveEventListener(protoCode, m_AreaController.ServerAreaRenturn);
                                            m_AreaController = null;
                                        }
                                        else if (protoCode == 3)
                                        {
                                            EventDispatcher.Instance.RemoveEventListener(protoCode, m_AreaController.ServerAreaRenturnResult);
                                            m_AreaController = null;
                                        }
                                        else if (protoCode == 4) 
                                        {
                                            m_StoreGoodsProtocolController.Distroy();
                                            m_StoreGoodsProtocolController = null;
                                        }
                                    }
                                }
                                else
                                {
                                    //校验失败，数据不完整
                                    break;
                                }

                                //========================如果当前包的长度超过当前数应该的长度就要做剩余字节处理=============================
                                int remainLen = (int)m_ReceiveMS.Length - currFullMsgLen;
                                if (remainLen > 0)
                                {
                                    m_ReceiveMS.Position = currFullMsgLen;
                                    byte[] remainBuffer = new byte[remainLen];
                                    m_ReceiveMS.Read(remainBuffer, 0, remainLen);

                                    //读完就清空这次传输的数据，为下一次接收数据做准备
                                    m_ReceiveMS.Position = 0;
                                    m_ReceiveMS.SetLength(0);
                                    //把剩余字节重新写入到数据流中
                                    m_ReceiveMS.Write(remainBuffer, 0, remainBuffer.Length);
                                    remainBuffer = null;
                                }
                                else 
                                {
                                    //没有字节剩余的情况
                                    m_ReceiveMS.Position = 0;
                                    m_ReceiveMS.SetLength(0);

                                    break;
                                }
                            }
                            else 
                            {
                                //说明当前包的长度并未是一个完整包
                                break;
                            }
                        }
                    }
                    //在此去接收包
                    ReceiveMsg();   
                }
                else
                {
                    //没有数据
                    Console.WriteLine("客户端{0}断开连接", m_Socket.RemoteEndPoint.ToString());

                    //断开连接后自然不用把该连接的roel的继续存储，删除就行
                    CharacterManager.Instance.AllRoles.Remove(m_SocketRole);
                }
            } 
            catch 
            {
                //没有数据
                Console.WriteLine("客户端{0}断开连接", m_Socket.RemoteEndPoint.ToString());

                //断开连接后自然不用把该连接的roel的继续存储，删除就行
                CharacterManager.Instance.AllRoles.Remove(m_SocketRole);
            }
        }
        #endregion

        //==================================

        #region 向客户端发送消息
        //发送消息
        public void SendMsg(byte[] data)
        {
            //封装数据包
            byte[] sendBuffer = MakeData(data);

            //因为不可能只有一个客户端发送数据，所以保证数据发送的有序性要加锁
            lock (m_SendQueue)
            {
                m_SendQueue.Enqueue(sendBuffer);

                //启动委托回调
                /*
                 * Bug:NETAPI版本实现的异步委托方式发生了改变，所有要更改异步委托的方式
                 * 
                 */
                Task.Run(() => m_CheckSendQueue.Invoke());

            }
        }

        #region 检测队列和封装数据包
        //发送的包是封装好的数据
        private byte[] MakeData(byte[] data)
        {
            byte[] resolutBuffer = null;
            //检测是否可被压缩
            bool isCompress = data.Length > m_CompressLen ? true : false;
            if (isCompress)
                data = ZlibHelper.CompressBytes(data);
            //异或
            data = SecurityUtil.Xor(data);
            //Crc校验
            ushort crc = Crc16.CalculateCrc16(data);

            using (MMO_MemoryStream ms = new MMO_MemoryStream())
            {
                ms.WriteUShort((ushort)(data.Length + 3));
                ms.WriteBool(isCompress);
                ms.WriteUShort(crc);
                ms.Write(data, 0, data.Length);

                resolutBuffer = ms.ToArray();
            }
            return resolutBuffer;
        }

        //检查队列回调
        private void OnCheckSendQueueCallBack()
        {
            lock (m_SendQueue)
            {
                //队列有数据就向服务器发送消息
                if (m_SendQueue.Count > 0)
                {
                    Send(m_SendQueue.Dequeue());
                }
            }
        }
        #endregion

        #region Socket发送消息
        //实际数据发送
        private void Send(byte[] buffer)
        {
            m_Socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, SendCallBack, m_Socket);
        }

        //发送数据回调
        private void SendCallBack(IAsyncResult ar)
        {
            //当发送数据结束时
            m_Socket.EndSend(ar);
            //然后继续检测队列是否有数据
            OnCheckSendQueueCallBack();
        }
        #endregion


        #endregion
    }
}
