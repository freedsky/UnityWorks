using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// 服务器区域
/// 
/// </summary>

namespace GodDecayServer
{
    public class ServerArea
    {
        private int serverId;
        private string serverName;
        private string serverDescribe;
        private int serverNumber;
        private string serverIp;

        public ServerArea() 
        {
            
        }
        public ServerArea(int id, string name, string describe, int number, string ip) 
        {
            serverId = id;
            serverName = name;
            serverDescribe = describe;
            serverNumber = number;
            serverIp = ip;
        }

        public int ServerId { get => serverId; set => serverId = value; }
        public string ServerName { get => serverName; set => serverName = value; }
        public string ServerDescribe { get => serverDescribe; set => serverDescribe = value; }
        public int ServerNumber { get => serverNumber; set => serverNumber = value; }
        public string ServerIp { get => serverIp; set => serverIp = value; }

        public override string ToString()
        {
            return string.Format("服务器id = {0} ， 服务器名称 = {1} , 服务器描述 = {2} ， 服务器人数 = {3} , 服务器IP地址 = {4}", serverId, serverName, serverDescribe, serverNumber, serverIp);
        }
    }
}
