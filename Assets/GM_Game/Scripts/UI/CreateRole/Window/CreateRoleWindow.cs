using Common;
using GM;
using Google.Protobuf;
using Manager;
using TMPro;
using UI;
using UnityEngine;

namespace UI.CreateRole
{
    /**
     * Title:创建角色窗口
     * Desciption:目前只有一个角色，所以创建的角色职业默认时剑修
     **/
    public class CreateRoleWindow : WindowBase
    {
        [SerializeField, Header("角色昵称输入框")] private TMP_InputField _iptNickName;

        [SerializeField, Header("创建角色按钮")] private UGUIBtn _btnCreateRole;

        public override void InitView()
        {
            base.InitView();
            _btnCreateRole.AddSingleClick(OnCreateRoleBtnClick);
        }

        public void OnCreateRoleBtnClick()
        {
            // 判断输入框是否为空
            if (string.IsNullOrEmpty(_iptNickName.text))
            {
                TipsMgr.Instance.ShowSystemTips("请输入昵称...");
            }
            // TODO 验证昵称是否合法

            // 服务器验证，是否创建成功
            CreateRoleReq createRoleReq = new CreateRoleReq()
            {
                AccountId = Global.Instance.LoginInfo.AccountId,
                GameServerId = Global.Instance.LoginInfo.GameServer.ServerId,
                Nickname = _iptNickName.text,
                // TODO 这里的职业类型考虑后续拓展，需要配表
                JobId = 1,
            };
            NetSocketMgr.Client.SendData(NetDefine.CMD_CreateRoleCode, createRoleReq.ToByteString());
        }

        public void OnDestroy()
        {
            _btnCreateRole.RemoveSingleClick(OnCreateRoleBtnClick);
        }
    }
}
