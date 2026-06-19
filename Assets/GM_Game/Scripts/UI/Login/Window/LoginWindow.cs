using Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Login
{
    /**
     * Title:登录窗口
     * Desciption:
     **/
    public class LoginWindow : UIBase
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

        public void OnGotoRegistBtnClicked()
        {
            // 跳转到注册窗口
            UIRoot.Instance.LoginCtrl.ShowWindow(WindowType.RegistWindow);
        }


        public void OnLoginBtnClicked()
        {
            // 1、判断输入框是否为空
            if (string.IsNullOrEmpty(_iptAcct.text))
            {
                Debug.Log("账号为空");
                return;
            }
            if (string.IsNullOrEmpty(_iptPasd.text))
            {
                Debug.Log("密码为空");
                return;
            }
            // 2、判断用户协议是否勾选，是则保存到PlayerPrefs
            if (!_toAgreement.isOn)
            {
                Debug.Log("用户协议未勾选");
                return;
            }
            else
            {
                PlayerPrefs.SetInt("Agreement", 1);
            }

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

            // 5、服务器验证，验证成功则跳转到主界面
            Debug.Log("登录成功");
        }

        private void OnDestroy()
        {
            _btnLogin.RemoveClick(OnLoginBtnClicked);
            _btnGotoRegist.RemoveClick(OnGotoRegistBtnClicked);
        }
    }
}