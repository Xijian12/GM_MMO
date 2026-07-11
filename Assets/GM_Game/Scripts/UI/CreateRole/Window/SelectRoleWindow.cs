using Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CreateRole
{

    /**
     * Title:选择角色窗口
     * Desciption:
     **/
    public class SelectRoleWindow : WindowBase
    {
        [SerializeField, Header("头像图片")] private Image _imgHead;
        [SerializeField, Header("昵称文本")] private TMP_Text _textNickName;
        [SerializeField, Header("职业等级文本")] private TMP_Text _textJobLevel;
        [SerializeField, Header("开始游戏按钮")] private UGUIBtn _btnStartGame;

        /// <summary>
        /// 初始化窗口
        /// </summary>
        public override void InitView()
        {
            _btnStartGame.AddSingleClick(OnStartGameBtnClicked);
        }

        public override void RefreshUI(object obj)
        {
            CreateRoleRet createRoleRet = obj as CreateRoleRet;
            if (createRoleRet != null)
            {
                _textNickName.SetText(createRoleRet.Nickname);
                // TODO 这里需要读表
                string jobStr = "";

                switch (createRoleRet.JobId)
                {
                    case 1:
                        jobStr = "剑修";
                        break;
                    default:
                        break;
                }
                _textJobLevel.SetText($"职业:{jobStr} Lv:{createRoleRet.Level}");
            }
        }

        /// <summary>
        /// 开始游戏按钮点击事件
        /// </summary>
        private void OnStartGameBtnClicked()
        {
            // TODO 进入主场景，开始真正的游戏
            // SceneManager.LoadScene("Scene_Main");
        }

        private void OnDestroy()
        {
            _btnStartGame.RemoveSingleClick(OnStartGameBtnClicked);
        }
    }
}