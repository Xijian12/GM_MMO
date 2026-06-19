using TMPro;
using UnityEngine;
using Common;

namespace UI.Login
{

    /**
     * Title:注册窗口
     * Desciption:
     **/
    public class RegistWindow : UIBase
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

        public void OnRegistBtnClicked()
        {
            //1、判断输入框是否为空
            if (string.IsNullOrEmpty(_iptAcct.text))
            {
                Debug.Log("账号为空...");
                return;
            }
            if (string.IsNullOrEmpty(_iptMobile.text))
            {
                Debug.Log("手机号码为空...");
                return;
            }
            if (string.IsNullOrEmpty(_iptVerify.text))
            {
                Debug.Log("验证码为空...");
                return;
            }
            if (string.IsNullOrEmpty(_iptPasd.text))
            {
                Debug.Log("密码为空...");
                return;
            }
            if (string.IsNullOrEmpty(_iptSurePasd.text))
            {
                Debug.Log("确认密码为空...");
                return;
            }
            //2、验证账号，手机号码，密码的合法性

            //3、判断密码和确认密码是否一致
            if (_iptPasd.text.Equals(_iptSurePasd.text))
            {
                Debug.Log("两次输入的密码不一致");
                return;
            }

            //4、开始注册
            //TODO
            Debug.Log("注册成功...");

            Hide();
        }

        public void OnVerifyCodeBtnClicked()
        {
            Debug.Log("获取验证码成功...");
        }

        public void OnBackLoginBtnClicked()
        {
            // 跳转到登录窗口
            UIRoot.Instance.LoginCtrl.ShowWindow(WindowType.LoginWindow);
        }

        private void OnDestroy()
        {
            _btnRegist.RemoveClick(OnRegistBtnClicked);
            _btnGetVerify.RemoveClick(OnVerifyCodeBtnClicked);
            _btnBackLogin.RemoveClick(OnBackLoginBtnClicked);
        }
    }
}