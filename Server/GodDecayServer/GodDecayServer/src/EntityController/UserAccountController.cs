using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

/// <summary>
/// 账户控制类
/// 
/// 用户登录，用户注册
/// </summary>

namespace GodDecayServer
{
    public class UserAccountController
    {
        public bool UserAccountLogin(UserAccount user) 
        {
            //查询指定用户，找到返回true反之false
            MySqlCommand cmd = null;
            MySqlDataReader reader = null;
            UserAccount userAccount = null;
            try
            {
                if (SqlConnection.Instance.m_Connection != null)
                {
                    SqlConnection.Instance.m_Connection.Open();
                    StringBuilder sql = new StringBuilder();
                    sql.Append("select * from useraccount where ");
                    sql.Append("username='");
                    sql.Append(user.UserName);
                    sql.Append("' and  userpassword='");
                    sql.Append(user.UserPassword);
                    sql.Append("'");
                    cmd = new MySqlCommand(sql.ToString(), SqlConnection.Instance.m_Connection);
                    reader = cmd.ExecuteReader();
                    while (reader.Read()) 
                    {
                        userAccount = new UserAccount();
                        userAccount.UserName = reader.GetString(1);
                        userAccount.UserPassword = reader.GetString(2);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally 
            {
                if (reader != null) 
                    reader.Close();
                SqlConnection.Instance.m_Connection.Close();
            }
            if (userAccount != null) 
            {
                if ((user.UserName.CompareTo(userAccount.UserName) == 0) && (user.UserPassword.CompareTo(userAccount.UserPassword) == 0))
                {
                    return true;
                }
            }

            return false;
        }

        public int UserAccountRegister(UserAccount user) 
        {
            MySqlCommand cmd = null;
            MySqlDataReader reader = null;
            UserAccount userAccount = null;
            int flag = 0;
            try
            {
                if (SqlConnection.Instance.m_Connection != null)
                {
                    SqlConnection.Instance.m_Connection.Open();
                    StringBuilder sql = new StringBuilder();
                    sql.Append("insert into useraccount(userid ,username, userpassword) ");
                    sql.Append("values('");
                    sql.Append(user.UserId);
                    sql.Append("','");
                    sql.Append(user.UserName);
                    sql.Append("','");
                    sql.Append(user.UserPassword);
                    sql.Append("');");
                    cmd = new MySqlCommand(sql.ToString(), SqlConnection.Instance.m_Connection);
                    flag = cmd.ExecuteNonQuery();//只要不等于0就是插入成功
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                SqlConnection.Instance.m_Connection.Close();
            }
            Console.WriteLine("已经插入 " + flag + " 个数据");
            return flag;
        }
    }
}
