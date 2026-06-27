using Google.Protobuf;
using System.Collections.Generic;
using UnityEngine;

namespace Common
{
    /**
 	* Title:
 	* Desciption:
 	**/

    public delegate void OnActionHandler(ByteString data);
    public class SocketDispatcher : Singleton<SocketDispatcher>
    {
        private readonly Dictionary<int, OnActionHandler> _actionDic = new Dictionary<int, OnActionHandler>();


        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="protoCode"></param>
        /// <param name="handler"></param>
        public void AddEventHandler(int protoCode, OnActionHandler handler)
        {
            if (!_actionDic.ContainsKey(protoCode) && handler != null)
            {
                _actionDic.Add(protoCode, handler);
            }
        }

        /// <summary>
        /// 移除事件
        /// </summary>
        /// <param name="protoCode"></param>
        public void RemoveEventHandler(int protoCode)
        {
            if (_actionDic.ContainsKey(protoCode))
            {
                _actionDic.Remove(protoCode);
            }
        }

        public void DispatcherEvent(int protoCode, ByteString data)
        {
            if (_actionDic.ContainsKey(protoCode))
            {
                _actionDic[protoCode]?.Invoke(data);
            }
        }
    }
}
