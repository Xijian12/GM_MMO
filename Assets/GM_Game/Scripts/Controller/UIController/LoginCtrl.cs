using Common;
using Google.Protobuf;
using Manager;
using System;
using UI;
using UnityEngine;

namespace UI.Login
{
    /**
     * Title:登录控制器
     * Desciption:对登录视图进行管理（显示、隐藏、数据操作）
     **/
    public class LoginCtrl : CtrlBase
    {
        private readonly LoginView _loginView;

        public LoginCtrl(UIBase view) : base(view)
        {
            _loginView = view as LoginView;
            _loginView.InitView();

            RegistCommand();
        }

        private void RegistCommand()
        {
            // 监听注册协议码事件
            SocketDispatcher.Instance.AddEventHandler(NetDefine.CMD_RegistCode, OnRegistHandle);

            // 监听登录协议码事件
            SocketDispatcher.Instance.AddEventHandler(NetDefine.CMD_LoginCode, OnLoginHandle);
        }

        /// <summary>
        /// 处理客户端返回回来的登录结果
        /// </summary>
        /// <param name="data"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnLoginHandle(ByteString data)
        {
            LoginRet loginRet = LoginRet.Parser.ParseFrom(data);

            if (loginRet != null && loginRet.CmdCode == CmdCode.Succeed)
            {
                Debug.Log("登录成功...");
                TipsMgr.Instance.ShowSystemTips("登录成功...");
                ShowWindow(WindowType.GameServerWindow);
            }
            else
            {
                Debug.Log("登录失败，" + loginRet.ToString());
                TipsMgr.Instance.ShowSystemTips("登录失败...");
            }
        }

        /// <summary>
        /// 处理服务端返回回来的注册结果
        /// </summary>
        /// <param name="data"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnRegistHandle(ByteString data)
        {
            RegistRet registRet = RegistRet.Parser.ParseFrom(data);

            if (registRet != null && registRet.CmdCode == CmdCode.Succeed)
            {
                Debug.Log("注册成功...");
                TipsMgr.Instance.ShowSystemTips("注册成功，请登录...");
                ShowWindow(WindowType.LoginWindow);
            }
            else
            {
                Debug.Log("注册失败，" + registRet.ToString());
                TipsMgr.Instance.ShowSystemTips("注册失败...");
            }
        }
    }
}
