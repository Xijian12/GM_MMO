using Common;
using Google.Protobuf;
using System;
using System.Threading;
using UnityEngine;

namespace Manager
{
    /**
 	* Title:网络模块的管理类
 	* Desciption:
 	**/
    public class NetSocketMgr : Singleton<NetSocketMgr>
    {
        // 客户端网络通信
        private static NetClient _client;

        public static NetClient Client { get => _client; }

        // 线程调度中心
        private SynchronizationContext _synchronizationContext;

        public void Init()
        {
            _synchronizationContext = SynchronizationContext.Current;
            ConnectServer(NetDefine.IPHost, NetDefine.LoginServerPort);
        }

        public void ConnectServer(string host, int port)
        {
            // 连接之前调用一下断开连接，防止重复连接
            DisConnect();

            _client = new NetClient(host, port, ClientType.Unity);

            // 当前收到的数据，仍在子线程
            _client.OnReceiveMsg += OnReceiveMsgHandle;

            _client.StartConnect();
        }

        /// <summary>
        /// 收到服务端发来的数据
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="string"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnReceiveMsgHandle(int protoCode, ByteString data)
        {
            // 需要切换到主线程处理数据
            _synchronizationContext.Post(_ =>
            {
                SocketDispatcher.Instance.DispatcherEvent(protoCode, data);
            }, null);
        }

        public void DisConnect()
        {
            if (_client != null)
            {
                _client.isNeedReconnect = false;
                _client.DisConnect();

                _client = null;
            }
        }
    }
}
