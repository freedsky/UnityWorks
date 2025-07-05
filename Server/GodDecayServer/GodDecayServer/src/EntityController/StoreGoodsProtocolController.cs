using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// 商店协议类
/// </summary>

namespace GodDecayServer
{
    public class StoreGoodsProtocolController
    {
        private ushort m_ProtoID = 4;
        //0表示没有任何操作1就表示查询，2表示修改
        private ushort m_HandleType = 0;

        public List<StoreGoods> m_StoreGoodses;

        public StoreGoodsProtocolController() 
        {
            m_StoreGoodses = null;
            EventDispatcher.Instance.AddEventListener(m_ProtoID, StoreGoodsRenturn);
        }
        public void Distroy() 
        {
            EventDispatcher.Instance.RemoveEventListener(m_ProtoID, StoreGoodsRenturn);
        }

        private void StoreGoodsRenturn(ServerCharacter character, byte[] buffer) 
        {
            //先解析数据
            using (MMO_MemoryStream ms = new MMO_MemoryStream(buffer)) 
            {
                //先对类型进行解析
                m_HandleType = ms.ReadUShort();
                if (m_HandleType == 1)
                {
                    m_StoreGoodses = FindStoreAll();
                }
                else if (m_HandleType == 2)
                {
                    int id = ms.ReadInt();
                    m_StoreGoodses = UpdataAndFindAll(id);    
                }
                else 
                {
                    Console.WriteLine("日志：没有找到相对应操作类型");
                }
            }

            //开始返回数据
            //检查返回的数据是否有效
            if (m_StoreGoodses != null)
            {
                //把list结果写入到数据流中并返回给客户端
                using (MMO_MemoryStream ms = new MMO_MemoryStream()) 
                {
                    ms.WriteUShort(4);
                    ms.WriteInt(m_StoreGoodses.Count);
                    for (int i = 0; i < m_StoreGoodses.Count; ++i) 
                    {
                        ms.WriteInt(m_StoreGoodses[i].GoodsId);
                        ms.WriteInt((int)m_StoreGoodses[i].Type);
                        ms.WriteString(m_StoreGoodses[i].GoodsName);
                        ms.WriteInt(m_StoreGoodses[i].GoodsNumber);
                    }
                    character.Client_Socket.SendMsg(ms.ToArray());
                }
            }
            else 
            {
                Console.WriteLine(String.Format("日志：数据操作失败，请Ip为：{0} 的客户端重新请求", character.m_IP));
            }
        }

        private List<StoreGoods> FindStoreAll() 
        {
            //直接返回
            List<StoreGoods> result = new List<StoreGoods>();
            MySqlCommand cmd = null;
            MySqlDataReader reader = null;
            UserAccount userAccount = null;

            try
            {
                SqlConnection.Instance.m_Connection.Open();
                StringBuilder sql = new StringBuilder();
                sql.Append("select * from store");
                cmd = new MySqlCommand(sql.ToString(), SqlConnection.Instance.m_Connection);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int id = int.Parse(reader.GetString(0));
                    GoodsType type = (GoodsType)int.Parse(reader.GetString(1));
                    string name = reader.GetString(2);
                    int number = int.Parse(reader.GetString(3));
                    StoreGoods goods = new StoreGoods(id, type, name, number);
                    result.Add(goods);
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

            return result;
        }

        private List<StoreGoods> UpdataAndFindAll(int goodsId) 
        {
            //先修改再返回
            List<StoreGoods> result = new List<StoreGoods>();
            MySqlCommand cmd = null;
            MySqlDataReader reader = null;
            UserAccount userAccount = null;
            int flag = 0;
            try
            {
                SqlConnection.Instance.m_Connection.Open();
                StringBuilder sql = new StringBuilder();
                sql.Append("update store set ");
                sql.Append("number = ");
                sql.Append("number - 1");
                sql.Append(" where id = ");
                sql.Append(goodsId);
                cmd = new MySqlCommand(sql.ToString(), SqlConnection.Instance.m_Connection);
                flag = cmd.ExecuteNonQuery();//只要flag不返回0就表示修改成功

                if (flag != 0)
                {
                    StringBuilder sql1 = new StringBuilder();
                    sql1.Append("select * from store");
                    cmd = new MySqlCommand(sql1.ToString(), SqlConnection.Instance.m_Connection);
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        int id = int.Parse(reader.GetString(0));
                        GoodsType type = (GoodsType)int.Parse(reader.GetString(1));
                        string name = reader.GetString(2);
                        int number = int.Parse(reader.GetString(3));
                        StoreGoods g = new StoreGoods(id, type, name, number);
                        result.Add(g);
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
            Console.WriteLine("日志：物品购买成功，服务器数据库已经更新");
            return result;
        }
    }
}
