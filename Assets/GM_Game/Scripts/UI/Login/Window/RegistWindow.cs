using Common;
using Google.Protobuf;
using Manager;
using TMPro;
using UI;
using UnityEngine;

namespace UI.Login
{
    /**
     * Title:注册窗口
     * Desciption:
     **/
    public class RegistWindow : WindowBase
    {
        [SerializeField, Header("账号输入框")] private TMP_InputField _iptAcct;
        [SerializeField, Header("手机号码输入框")] private TMP_InputField _iptMobile;
        [SerializeField, Header("验证码输入框")] private TMP_InputField _iptVerify;
        [SerializeField, Header("密码输入框")] private TMP_InputField _iptPasd;
        [SerializeField, Header("确认密码输入框")] private TMP_InputField _iptSurePasd;
        [SerializeField, Header("注册按钮")] private UGUIBtn _btnRegist;
        [SerializeField, Header("获取验证码按钮")] private UGUIBtn _btnGetVerify;
        [SerializeField, Header("返回登录按钮")] private UGUIBtn _btnBackLogin;

        public override void InitView()
        {
            _btnRegist.AddClick(OnRegistBtnClicked);
            _btnGetVerify.AddClick(OnVerifyCodeBtnClicked);
            _btnBackLogin.AddClick(OnBackLoginBtnClicked);
        }

        /// <summary>
        /// 注册按钮点击事件
        /// </summary>
        public void OnRegistBtnClicked()
        {
            //1、判断输入框是否为空
            if (string.IsNullOrEmpty(_iptAcct.text))
            {
                TipsMgr.Instance.ShowSystemTips("账号不能为空...");
                Debug.Log("账号为空...");
                return;
            }
            if (string.IsNullOrEmpty(_iptMobile.text))
            {
                TipsMgr.Instance.ShowSystemTips("手机号码不能为空...");
                Debug.Log("手机号码为空...");
                return;
            }
            if (string.IsNullOrEmpty(_iptVerify.text))
            {
                TipsMgr.Instance.ShowSystemTips("验证码不能为空...");
                Debug.Log("验证码为空...");
                return;
            }
            if (string.IsNullOrEmpty(_iptPasd.text))
            {
                TipsMgr.Instance.ShowSystemTips("密码不能为空...");
                Debug.Log("密码为空...");
                return;
            }
            if (string.IsNullOrEmpty(_iptSurePasd.text))
            {
                TipsMgr.Instance.ShowSystemTips("确认密码不能为空...");
                Debug.Log("确认密码为空...");
                return;
            }
            //2、验证账号，手机号码，密码的合法性

            //3、判断密码和确认密码是否一致
            if (!_iptPasd.text.Equals(_iptSurePasd.text))
            {
                TipsMgr.Instance.ShowSystemTips("两次输入的密码不一致...");
                Debug.Log("两次输入的密码不一致");
                return;
            }

            //4、开始注册
            //TODO
            Debug.Log("注册成功...");
            TipsMgr.Instance.ShowSystemTips("注册成功...");

            RegistReq registReq = new RegistReq()
            {
                UserName = _iptAcct.text,
                PhoneNum = _iptMobile.text,
                Password = _iptPasd.text,
            };
            // 调用客户端对象向服务端发送注册信息
            NetSocketMgr.Client.SendData(NetDefine.CMD_RegistCode, registReq.ToByteString());

            Hide();
        }

        /// <summary>
        /// 获取验证码按钮点击事件
        /// </summary>
        public void OnVerifyCodeBtnClicked()
        {
            Debug.Log("获取验证码成功...");
            TipsMgr.Instance.ShowSystemTips("获取验证码成功...");
        }

        /// <summary>
        /// 返回登录按钮点击事件
        /// </summary>
        public void OnBackLoginBtnClicked()
        {
            UIRoot.Instance.LoginViewCtrl.ShowWindow(WindowType.LoginWindow);
        }

        private void OnDestroy()
        {
            _btnRegist.RemoveClick(OnRegistBtnClicked);
            _btnGetVerify.RemoveClick(OnVerifyCodeBtnClicked);
            _btnBackLogin.RemoveClick(OnBackLoginBtnClicked);
        }
    }
}
