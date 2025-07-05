using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 观察者，它负责事件的监听和派发
/// 以及添加/删除监听对象等功能
/// 
/// </summary>
namespace GodDecayServer 
{
    public class EventDispatcher
    {
        #region 单例
        private static EventDispatcher instance;
        public static EventDispatcher Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EventDispatcher();
                }
                return instance;
            }
        }

        #endregion

        //委托原型[告诉服务器中的角色集合我们实际需要去通知那个具体的角色]
        public delegate void OnActionHandler(ServerCharacter character, byte[] buffer);
        //委托字典
        private Dictionary<ushort, List<OnActionHandler>> eventDic = new Dictionary<ushort, List<OnActionHandler>>();

        //添加监听
        public void AddEventListener(ushort protoCode, OnActionHandler handler)
        {
            if (eventDic.ContainsKey(protoCode))
            {
                eventDic[protoCode].Add(handler);
            }
            else
            {
                List<OnActionHandler> list = new List<OnActionHandler>();
                list.Add(handler);
                eventDic[protoCode] = list;
            }
        }

        //移除监听
        public void RemoveEventListener(ushort protoCode, OnActionHandler handler)
        {
            if (eventDic.ContainsKey(protoCode))
            {
                List<OnActionHandler> list = eventDic[protoCode];
                list.Remove(handler);
                if (list.Count == 0)
                {
                    eventDic.Remove(protoCode);
                }
            }
        }

        //派发事件[广播通知]

        public void Dispatch(ushort protoID, ServerCharacter character, byte[] buffer)
        {
            if (eventDic.ContainsKey(protoID))
            {
                List<OnActionHandler> list = eventDic[protoID];
                if (list != null && list.Count > 0)
                {
                    for (int i = 0; i < list.Count; ++i)
                    {
                        list[i](character, buffer);
                    }
                }
            }
        }
    }
}
