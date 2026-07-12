using Common;
using Manager;
using System;
using TMPro;
using UI.CreateRole.Data;
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

        private const int DefaultJobId = 1;

        public Action<CreateRoleFormData> OnCreateRoleSubmit;

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
                return;
            }
            // TODO 验证昵称是否合法

            OnCreateRoleSubmit?.Invoke(new CreateRoleFormData(_iptNickName.text, DefaultJobId));
        }

        public void OnDestroy()
        {
            _btnCreateRole.RemoveSingleClick(OnCreateRoleBtnClick);
        }
    }
}
