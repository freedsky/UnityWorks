using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GodDecayServer
{
    public class SqlConnection
    {
        #region 单例
        private static SqlConnection instance;
        public static SqlConnection Instance 
        {
            get 
            {
                if (instance == null) 
                {
                    //反之有默认的[正式写的时候就不能这么硬编码了]
                    instance = new SqlConnection();
                }
                return instance;
            }
        }
        #endregion

        private MySqlConnectionStringBuilder m_BuilderString;

        public MySqlConnection m_Connection;

        public SqlConnection() 
        {
            m_BuilderString = new MySqlConnectionStringBuilder();
            m_BuilderString.Server = "localhost";
            m_BuilderString.Database = "goddecay";
            m_BuilderString.UserID = "root";
            m_BuilderString.Password = "748962451";
            m_BuilderString.Pooling = true;
            m_BuilderString.CharacterSet = "utf8";
        }
        public SqlConnection(string name, string password, string server, string databaseName, string charset) 
        {
            m_BuilderString = new MySqlConnectionStringBuilder();
            m_BuilderString.Server = server;
            m_BuilderString.Database = databaseName;
            m_BuilderString.UserID = name;
            m_BuilderString.Password = password;
            m_BuilderString.Pooling = true;
            m_BuilderString.CharacterSet = charset;
        }

        public bool ConnectDatabase() 
        {
            m_Connection = new MySqlConnection(m_BuilderString.ConnectionString);
            
            return m_Connection == null ? false : true;
        }
    }
}
