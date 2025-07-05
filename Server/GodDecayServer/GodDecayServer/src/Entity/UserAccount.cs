using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// 账户类
/// </summary>

namespace GodDecayServer
{
    public class UserAccount
    {
        private int userId;
        private string userName;
        private string userPassword;

        public int UserId { get => userId; set => userId = value; }
        public string UserName { get => userName; set => userName = value; }
        public string UserPassword { get => userPassword; set => userPassword = value; }

        public UserAccount() { }
        public UserAccount(int id, string name, string password)
        {
            userId = id;
            userName = name;
            userPassword = password;
        }

        public override string ToString()
        {
            return "用户id = " + userId + " 用户名 = " + userName + " 密码 = " + userPassword;
        }
    }
}
