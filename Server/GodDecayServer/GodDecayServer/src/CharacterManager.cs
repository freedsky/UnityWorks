using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// 角色集合管理类
/// </summary>

namespace GodDecayServer
{
    public class CharacterManager
    {
        //客户端用户集合列表
        private List<ServerCharacter> m_AllRole;
        public List<ServerCharacter> AllRoles { get { return m_AllRole; } }

        private CharacterManager() 
        {
            m_AllRole = new List<ServerCharacter>();
        }

        #region 单例
        //对象锁
        private static object lock_object = new object();
        private static CharacterManager instance;

        public static CharacterManager Instance 
        {
            get 
            {
                if (instance == null) 
                {
                    lock (lock_object) 
                    {
                        if (instance == null) 
                        {
                            instance = new CharacterManager();
                        }
                    }
                }
                return instance;
            }
        }
        #endregion
    }
}
