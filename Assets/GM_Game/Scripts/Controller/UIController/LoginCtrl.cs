using Common;
using GM;
using Google.Protobuf;
using Manager;
using System;
using UI;
using UI.Login.Data;
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

            // 监听请求服务器列表协议码事件
            SocketDispatcher.Instance.AddEventHandler(NetDefine.CMD_GetServerListCode, OnGetServerListHandle);

            // 监听请求登录游戏服务器协议码事件
            SocketDispatcher.Instance.AddEventHandler(NetDefine.CMD_LoginGameServerCode, OnLoginGameServerHandle);

            /*--- 注册点击事件--- */
            // 注册登录游戏服务器事件
            _loginView.RegisterGameServerBtnClicked(OnGameServerBtnClicked);
            _loginView.RegisterGotoServerListBtnClicked(OnGotoServerListBtnClicked);

            _loginView.RegisterLoginSubmit(OnLoginSubmit);
            _loginView.RegisterRegistSubmit(OnRegistSubmit);
        }

        /// <summary>
        /// 登录表单提交
        /// </summary>
        private void OnLoginSubmit(LoginFormData form)
        {
            LoginReq loginReq = new LoginReq()
            {
                UserName = form.UserName,
                Password = form.Password,
            };

            NetSocketMgr.Client.SendData(NetDefine.CMD_LoginCode, loginReq.ToByteString());
        }

        /// <summary>
        /// 注册表单提交
        /// </summary>
        private void OnRegistSubmit(RegistFormData form)
        {
            RegistReq registReq = new RegistReq()
            {
                UserName = form.UserName,
                PhoneNum = form.PhoneNum,
                Password = form.Password,
            };

            NetSocketMgr.Client.SendData(NetDefine.CMD_RegistCode, registReq.ToByteString());
        }

        /// <summary>
        /// 点击跳转服务器列表事件
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void OnGotoServerListBtnClicked()
        {
            GetServerListReq getServerListReq = new GetServerListReq()
            {
                ServerId = 0,
            };
            // 向服务端发送获取服务器列表请求
            NetSocketMgr.Client.SendData(NetDefine.CMD_GetServerListCode, getServerListReq.ToByteString());
        }

        /// <summary>
        /// 点击服务器登录事件
        /// </summary>
        /// <param name="server"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnGameServerBtnClicked(GameServer server)
        {
            LoginGameServerReq req = new LoginGameServerReq()
            {
                AccountId = Global.Instance.LoginInfo.AccountId,
                GameServerId = server.ServerId
            };

            // 服务器请求登录服务器
            NetSocketMgr.Client.SendData(NetDefine.CMD_LoginGameServerCode, req.ToByteString());
        }

        /// <summary>
        /// 处理服务端（登录服务器）返回回来的请求登录游戏服务器结果
        /// </summary>
        /// <param name="data"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnLoginGameServerHandle(ByteString data)
        {
            LoginGameServerRet ret = LoginGameServerRet.Parser.ParseFrom(data);

            if (ret != null && ret.CmdCode == CmdCode.Succeed)
            {
                Debug.Log("请求登录游戏服务器成功...");

                SceneMgr.Instance.LoadCreateRoleScene(() =>
                {
                    UIRoot.Instance.LoginViewCtrl.HideView();
                    // 1、是否已有角色，是则
                    // 跳转角色选择界面
                    if (ret.CreateRoleInfo != null)
                    {
                        UIRoot.Instance.CreateRoleViewCtrl.ShowWindow(WindowType.SelectRoleWindow, ret.CreateRoleInfo);
                    }
                    // 2、否则跳转创建角色界面
                    else
                    {
                        UIRoot.Instance.CreateRoleViewCtrl.ShowWindow(WindowType.CreateRoleWindow);
                    }
                });
                TipsMgr.Instance.ShowSystemTips("登录服务器成功...");
            }
            else
            {
                Debug.Log("请求登录游戏服务器失败，" + ret.ToString());
                TipsMgr.Instance.ShowSystemTips("登录服务器失败...");
            }
        }

        /// <summary>
        /// 处理服务端（登录服务器）返回回来的获取服务列表结果
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
                Debug.Log("登录成功..." + ret.ToString());
                TipsMgr.Instance.ShowSystemTips("登录成功...");

                Global.Instance.LoginInfo = ret;
                ShowWindow(WindowType.GameServerWindow, ret.GameServer);
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

        public override void Dispose()
        {
            _loginView.UnRegisterGameServerBtnClicked(OnGameServerBtnClicked);
            _loginView.UnRegisterGotoServerListBtnClicked(OnGotoServerListBtnClicked);
            _loginView.UnregisterLoginSubmit(OnLoginSubmit);
            _loginView.UnregisterRegistSubmit(OnRegistSubmit);

            SocketDispatcher.Instance.RemoveEventHandler(NetDefine.CMD_RegistCode);
            SocketDispatcher.Instance.RemoveEventHandler(NetDefine.CMD_LoginCode);
            SocketDispatcher.Instance.RemoveEventHandler(NetDefine.CMD_GetServerListCode);
            SocketDispatcher.Instance.RemoveEventHandler(NetDefine.CMD_LoginGameServerCode);

            base.Dispose();
        }
    }
}
