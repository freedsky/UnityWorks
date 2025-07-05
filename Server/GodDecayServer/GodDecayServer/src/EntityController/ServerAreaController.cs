using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// 
/// </summary>

namespace GodDecayServer
{
    public class ServerAreaController
    {
        //基本上就一个方法，查询所有
        public List<ServerArea> GetServerAll() 
        {
            MySqlCommand cmd = null;
            MySqlDataReader reader = null;
            UserAccount userAccount = null;
            List<ServerArea> servers = new List<ServerArea>();
            try
            {
                SqlConnection.Instance.m_Connection.Open();
                StringBuilder sql = new StringBuilder();
                sql.Append("select * from serverarea");
                cmd = new MySqlCommand(sql.ToString(), SqlConnection.Instance.m_Connection);
                reader = cmd.ExecuteReader();
                while (reader.Read()) 
                {
                    int id = int.Parse(reader.GetString(0));
                    string name = reader.GetString(1);
                    string description = reader.GetString(2);
                    int number = int.Parse(reader.GetString(3));
                    string ip = reader.GetString(4);
                    ServerArea area = new ServerArea(id, name, description, number, ip);
                    servers.Add(area);
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
            
            return servers;
        }

        //实现观察者接口
        public void ServerAreaRenturn(ServerCharacter character, byte[] buffer) 
        {
            List<ServerArea> servers = GetServerAll();

            using (MMO_MemoryStream ms = new MMO_MemoryStream()) 
            {
                //第一个是协议编号
                ms.WriteUShort(2);
                //服务器数量
                ms.WriteInt((int)servers.Count);
                for (int i = 0; i < servers.Count; ++i) 
                {
                    ms.WriteInt(servers[i].ServerId);
                    ms.WriteString(servers[i].ServerName);
                    ms.WriteString(servers[i].ServerDescribe);
                    ms.WriteInt(servers[i].ServerNumber);
                    ms.WriteString(servers[i].ServerIp);
                }
                character.Client_Socket.SendMsg(ms.ToArray());
            }
        }

        public void ServerAreaRenturnResult(ServerCharacter character, byte[] buffer) 
        {
            ServerArea area;
            using (MMO_MemoryStream ms = new MMO_MemoryStream(buffer))
            {
                int id = ms.ReadInt();
                string name = ms.ReadUTF8String();
                string desc = ms.ReadUTF8String();
                int num = ms.ReadInt();
                string ip = ms.ReadUTF8String();
                area = new ServerArea(id, name, desc, num, ip);
            }
            Console.WriteLine("IP地址为 " + character.m_IP + " 的客户端选择的服务器信息为\n" + area.ToString());
        }
    }
}
