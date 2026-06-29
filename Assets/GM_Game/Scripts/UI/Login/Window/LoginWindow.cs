using Common;
using Google.Protobuf;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Login
{
    /**
     * Title:登录窗口
     * Desciption:
     **/
    public class LoginWindow : WindowBase
    {
        [SerializeField, Header("账号输入框")] private TMP_InputField _iptAcct;
        [SerializeField, Header("密码输入框")] private TMP_InputField _iptPasd;
        [SerializeField, Header("记住账号Toggle")] private Toggle _toRemeberAcct;
        [SerializeField, Header("用户协议Toggle")] private Toggle _toAgreement;

        [SerializeField, Header("登录按钮")] private UGUIBtn _btnLogin;
        [SerializeField, Header("跳转注册按钮")] private UGUIBtn _btnGotoRegist;

        private void Awake()
        {
            // 1、如果PlayerPrefs中存在Agreement，则设置到用户协议Toggle
            if (PlayerPrefs.HasKey("Agreement"))
            {
                int agreement = PlayerPrefs.GetInt("Agreement");
                _toAgreement.isOn = agreement == 1;
            }

            // 2、如果PlayerPrefs中存在PlayerAccount，则设置到账号输入框
            if (PlayerPrefs.HasKey("PlayerAccount"))
            {
                _toRemeberAcct.isOn = true;
                _iptAcct.text = PlayerPrefs.GetString("PlayerAccount");
            }
        }

        public override void InitView()
        {
            _btnLogin.AddClick(OnLoginBtnClicked);
            _btnGotoRegist.AddClick(OnGotoRegistBtnClicked);
        }

        /// <summary>
        /// 跳转注册按钮点击事件
        /// </summary>
        public void OnGotoRegistBtnClicked()
        {
            UIRoot.Instance.LoginViewCtrl.ShowWindow(WindowType.RegistWindow);
        }

        /// <summary>
        /// 登录按钮点击事件
        /// </summary>
        public void OnLoginBtnClicked()
        {
            // TODO 这里的所有文本都需要加入到文案表中
            // 1、判断输入框是否为空
            if (string.IsNullOrEmpty(_iptAcct.text))
            {
                TipsMgr.Instance.ShowSystemTips("账号不能空...");
                Debug.Log("账号为空...");
                return;
            }
            if (string.IsNullOrEmpty(_iptPasd.text))
            {
                TipsMgr.Instance.ShowSystemTips("密码不能为空...");
                Debug.Log("密码为空...");
                return;
            }
            // 2、判断用户协议是否勾选，是则保存到PlayerPrefs
            if (!_toAgreement.isOn)
            {
                TipsMgr.Instance.ShowSystemTips("请阅读并勾选用户协议...");
                Debug.Log("用户协议未勾选...");
                return;
            }

            PlayerPrefs.SetInt("Agreement", 1);

            // 3、判断是否记住账号，是则保存到PlayerPrefs
            if (_toRemeberAcct.isOn)
            {
                PlayerPrefs.SetString("PlayerAccount", _iptAcct.text);
            }
            else
            {
                PlayerPrefs.DeleteKey("PlayerAccount");
            }

            // 4、保存到PlayerPrefs
            PlayerPrefs.Save();

            LoginReq loginReq = new LoginReq()
            {
                UserName = _iptAcct.text,
                Password = _iptPasd.text,
            };

            // 5、向服务器发送登录请求
            NetSocketMgr.Client.SendData(NetDefine.CMD_LoginCode, loginReq.ToByteString());
        }

        private void OnDestroy()
        {
            _btnLogin.RemoveClick(OnLoginBtnClicked);
            _btnGotoRegist.RemoveClick(OnGotoRegistBtnClicked);
        }
    }
}
