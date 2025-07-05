using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;


/// <summary>
/// 服务器启动类
/// </summary>

namespace GodDecayServer
{
    public class MainProgram
    {
        //服务器IP
        private static string m_ServerIP = "192.168.35.23";
        //服务器端口
        private static int m_ServerPort = 1011;
        //连接对象[服务器的接收对象]
        private static Socket m_ServerSocket;

        //主程序
        static void Main(string[] args)
        {
            //实例化Scoket
            m_ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //向操作系统申请一个可用的IP和端口用于通讯
            m_ServerSocket.Bind(new IPEndPoint(IPAddress.Parse(m_ServerIP), m_ServerPort));

            //设置最大监听连接请求数[单次最高能接收多少请求]
            m_ServerSocket.Listen(3000);

            //日志打印
            Console.WriteLine(String.Format("服务器{0}启动监听", m_ServerSocket.LocalEndPoint.ToString()));

            //在服务器就检测数据库是否可以连接
            if (SqlConnection.Instance.ConnectDatabase())
            {
                Console.WriteLine("日志：数据库连接正常");
            }
            else 
            {
                Console.WriteLine("日志：数据库连接失败");
            }

            //启动线程
            Thread mThread = new Thread(ListenClientCallBack);
            mThread.Start();
        }

        //监听客户端连接
        private static void ListenClientCallBack()
        {
            while (true)
            {
                //接收客户端的请求
                Socket clientSocket = m_ServerSocket.Accept();

                Console.WriteLine("客户端监听{0}成功", clientSocket.RemoteEndPoint.ToString());

                //一个角色就是一个客户端对象{这里里面把观察者接口实现}
                ServerCharacter role = new ServerCharacter();
                role.m_IP = clientSocket.RemoteEndPoint.ToString();
                //在构造中会开启一个线程对象每个角色[客户端进行服务器端的处理]
                ClientSocket client = new ClientSocket(clientSocket, role);
                //然后加入到角色管理类中
                CharacterManager.Instance.AllRoles.Add(role);
            }
        }
    }
}
