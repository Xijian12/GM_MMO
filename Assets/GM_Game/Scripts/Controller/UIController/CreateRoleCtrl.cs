using Common;
using Google.Protobuf;
using Manager;
using UI;
using UnityEngine;

namespace UI.CreateRole
{
    /**
     * Title:创建角色控制器
     * Desciption:
     **/
    public class CreateRoleCtrl : CtrlBase
    {
        private readonly CreateRoleView _createRoleView;

        public CreateRoleCtrl(UIBase view) : base(view)
        {
            _createRoleView = view as CreateRoleView;
            _createRoleView.InitView();

            RegistCommand();
        }

        private void RegistCommand()
        {
            // 监听请求登录游戏服务器协议码事件
            SocketDispatcher.Instance.AddEventHandler(NetDefine.CMD_CreateRoleCode, OnCreateRoleHandle);
        }

        /// <summary>
        /// 处理服务端（登录服务器）返回回来的请求创建角色结果
        /// </summary>
        /// <param name="data"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnCreateRoleHandle(ByteString data)
        {
            CreateRoleRet ret = CreateRoleRet.Parser.ParseFrom(data);
            if (ret != null && ret.CmdCode == CmdCode.Succeed)
            {
                Debug.Log("创建角色成功：" + ret.ToString());
                ShowWindow(WindowType.SelectRoleWindow, ret);
            }
            else
            {
                Debug.Log("请求创建角色失败，" + ret.ToString());
                TipsMgr.Instance.ShowSystemTips("请求创建角色失败...");
            }

        }
    }
}
