using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// �۲��ߣ��������¼��ļ������ɷ�
/// �Լ����/ɾ����������ȹ���
/// 
/// </summary>
namespace GodDecayServer 
{
    public class EventDispatcher
    {
        #region ����
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

        //ί��ԭ��[���߷������еĽ�ɫ��������ʵ����Ҫȥ֪ͨ�Ǹ�����Ľ�ɫ]
        public delegate void OnActionHandler(ServerCharacter character, byte[] buffer);
        //ί���ֵ�
        private Dictionary<ushort, List<OnActionHandler>> eventDic = new Dictionary<ushort, List<OnActionHandler>>();

        //��Ӽ���
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

        //�Ƴ�����
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

        //�ɷ��¼�[�㲥֪ͨ]

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
