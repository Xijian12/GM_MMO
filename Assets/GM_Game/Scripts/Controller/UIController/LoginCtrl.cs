using Common;
using GM;
using Google.Protobuf;
using Manager;
using System;
using UI;
using UnityEngine;
using YooAsset;

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

            // 监听请求服务器列表协议码事件
            SocketDispatcher.Instance.AddEventHandler(NetDefine.CMD_GetServerListCode, OnGetServerListHandle);
        }

        /// <summary>
        /// 处理服务端（登录服务器）返回回来的登录结果
        /// </summary>
        /// <param name="data"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnGetServerListHandle(ByteString data)
        {
            GetServerListRet ret = GetServerListRet.Parser.ParseFrom(data);

            if (ret != null && ret.CmdCode == CmdCode.Succeed)
            {
                Debug.Log("获取服务列表成功...");
                TipsMgr.Instance.ShowSystemTips("请选择服务器...");
                ShowWindow(WindowType.ServerListWindow, ret);
            }
            else
            {
                Debug.Log("获取服务列表失败，" + ret.ToString());
                TipsMgr.Instance.ShowSystemTips("服务列表获取失败...");
            }
        }

        /// <summary>
        /// 处理服务端（登录服务器）返回回来的登录结果
        /// </summary>
        /// <param name="data"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnLoginHandle(ByteString data)
        {
            LoginRet ret = LoginRet.Parser.ParseFrom(data);

            if (ret != null && ret.CmdCode == CmdCode.Succeed)
            {
                Debug.Log("登录成功...");
                TipsMgr.Instance.ShowSystemTips("登录成功...");
                ShowWindow(WindowType.GameServerWindow);
            }
            else
            {
                Debug.Log("登录失败，" + ret.ToString());
                TipsMgr.Instance.ShowSystemTips("登录失败...");
            }
        }

        /// <summary>
        /// 处理服务端（登录服务器）返回回来的注册结果
        /// </summary>
        /// <param name="data"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnRegistHandle(ByteString data)
        {
            RegistRet ret = RegistRet.Parser.ParseFrom(data);

            if (ret != null && ret.CmdCode == CmdCode.Succeed)
            {
                Debug.Log("注册成功...");
                TipsMgr.Instance.ShowSystemTips("注册成功，请登录...");
                ShowWindow(WindowType.LoginWindow);
            }
            else
            {
                Debug.Log("注册失败，" + ret.ToString());
                TipsMgr.Instance.ShowSystemTips("注册失败...");
            }
        }
    }
}
