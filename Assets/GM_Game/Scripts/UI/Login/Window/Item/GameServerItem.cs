using Common;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Login
{
    /**
 	* Title:
 	* Desciption:
 	**/
    public class GameServerItem : MonoBehaviour
    {
        [SerializeField, Header("运行状态图片")] private Image _imgRunState;
        [SerializeField, Header("服务器名称")] private TMP_Text _textServerName;
        [SerializeField, Header("按钮组件")] private UGUIBtn _itemBtn;
        public Action<GameServer> OnServerItemSingleClicked;
        public Action<GameServer> OnServerItemDoubleClicked;
        private GameServer _gameServer;

        private void Awake()
        {
            _itemBtn.AddSingleClick(OnItemBtnClick);
            _itemBtn.AddDoubleClick(OnItemBtnDoubleClick);
        }

        internal void RefreshUI(GameServer gameServer)
        {
            _gameServer = gameServer;
            Color color = Color.white;
            if (gameServer.RunState == 1)
            {
                color = Color.red;
            }
            else if (gameServer.RunState == 2)
            {
                color = Color.yellow;
            }
            else if (gameServer.RunState == 3)
            {
                color = Color.green;
            }

            _imgRunState.color = color;

            string str = "";
            if (gameServer.IsNew == 1)
            {
                // TODO 这里应该也需要配表
                str = "(新服)";
            }

            _textServerName.SetText(gameServer.ServerName + str);
        }

        private void OnDestroy()
        {
            _itemBtn.RemoveSingleClick(OnItemBtnClick);
            _itemBtn.RemoveDoubleClick(OnItemBtnDoubleClick);
        }

        private void OnItemBtnClick()
        {
            OnServerItemSingleClicked?.Invoke(_gameServer);
        }

        private void OnItemBtnDoubleClick()
        {
            OnServerItemDoubleClicked?.Invoke(_gameServer);
        }

    }
}
