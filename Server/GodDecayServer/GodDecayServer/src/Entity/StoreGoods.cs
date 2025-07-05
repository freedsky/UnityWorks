using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// 商店实体类
/// </summary>

namespace GodDecayServer
{
    public enum GoodsType 
    {
        Weapon,
        Item,
        Gift
    }

    public class StoreGoods
    {
        private int goodsId;
        //在于数据库传输时转为int
        private GoodsType type;
        private string goodsName;
        private int goodsNumber;

        public int GoodsId { get => goodsId; set => goodsId = value; }
        public GoodsType Type { get => type; set => type = value; }
        public string GoodsName { get => goodsName; set => goodsName = value; }
        public int GoodsNumber { get => goodsNumber; set => goodsNumber = value; }

        public StoreGoods(){}
        public StoreGoods(int id, GoodsType type, string name, int number) 
        {
            goodsId = id;
            this.type = type;
            goodsName = name;
            goodsNumber = number;
        }

        public override string ToString()
        {
            return string.Format("商品ID={0} 的基本信息：商品种类={1}， 商品名称={2}， 商品数量={3}", goodsId, type, goodsName, goodsNumber);
        }
    }
}
