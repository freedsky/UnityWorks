using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// 角色类，用于服务器存储和操作的对象数据结构
/// 
/// 这里实现观察者接口[初始化]
/// 
/// 角色类型[业务类型]
/// 
/// 这里规定0协议编号为LandR内容协议，其它数就为GamePlayer的协议内容
/// </summary>

namespace GodDecayServer
{
    /// <summary>
    /// 两个大类型，一个专门处理登录注册服务
    /// 只要登录进去剩下的业务类型就是由实际游戏角色去选择协议去服务
    /// </summary>
    public enum CharacterType 
    {
        LandR,
        GamePlayer
    }

    public class ServerCharacter
    {
        public CharacterType m_CharacterType;
        public ClientSocket Client_Socket { get; set; }
        public string m_IP;

        public ServerCharacter() 
        {

        }
        public ServerCharacter(CharacterType type) 
        {
            m_CharacterType = type;
        }

        //处理登录
        public void LoginAccount(ServerCharacter character, byte[] buffer) 
        {
            //处理登录时与服务器的ServerCharacter没有关联所以这里可以不管
            //处理byte数据
            using (MMO_MemoryStream ms = new MMO_MemoryStream(buffer)) 
            {
                int id = ms.ReadInt();
                string name = ms.ReadUTF8String();
                string pass = ms.ReadUTF8String();
                UserAccount userAccount = new UserAccount(id, name, pass);
                UserAccountController userAccountController = new UserAccountController();
                //不管是成功还是失败都要向客户端返回结果
                if (userAccountController.UserAccountLogin(userAccount))
                {
                    using (MMO_MemoryStream ms2 = new MMO_MemoryStream()) 
                    {
                        ms2.WriteUShort(0);
                        ms2.WriteBool(true);
                        character.Client_Socket.SendMsg(ms2.ToArray());
                    }

                    
                }
                else 
                {
                    using (MMO_MemoryStream ms2 = new MMO_MemoryStream())
                    {
                        ms2.WriteUShort(0);
                        ms2.WriteBool(false);
                        character.Client_Socket.SendMsg(ms2.ToArray());
                    }
                }
            }
        }

        //注册处理
        public void RegisterAccount(ServerCharacter character, byte[] buffer)
        {
            using (MMO_MemoryStream ms = new MMO_MemoryStream(buffer))
            {
                int id = ms.ReadInt();
                string name = ms.ReadUTF8String();
                string pass = ms.ReadUTF8String();
                UserAccount userAccount = new UserAccount(id, name, pass);
                UserAccountController userAccountController = new UserAccountController();
                int flag = userAccountController.UserAccountRegister(userAccount);
                //这里直接返回数值即可
                using (MMO_MemoryStream ms2 = new MMO_MemoryStream())
                {
                    ms2.WriteUShort(1);
                    ms2.WriteInt(flag);
                    character.Client_Socket.SendMsg(ms2.ToArray());
                }
            }
        }
    }
}
