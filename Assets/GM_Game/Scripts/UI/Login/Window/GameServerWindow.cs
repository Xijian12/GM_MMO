using Common;
using GM;
using Google.Protobuf;
using Manager;
using System;
using TMPro;
using UI;
using UnityEngine;

namespace UI.Login
{
    /**
     * Title:登录服务器窗口
     * Desciption:
     **/
    public class GameServerWindow : WindowBase
    {
        [SerializeField, Header("跳转服务器列表按钮")] private UGUIBtn _btnGotoServerList;
        [SerializeField, Header("登录服务器按钮")] private UGUIBtn _btnLoginServer;
        [SerializeField, Header("服务器名称")] private TMP_Text _textServerName;
        [SerializeField, Header("服务器运行状态名称")] private TMP_Text _textRunStateName;
        private GameServer _gameServer;

        // 使用Action将UI层和Model层分离
        public event Action<GameServer> GameServerBtnClickAction;
        public event Action GotoServerListBtnClickAction;

        public override void InitView()
        {
            _btnGotoServerList.AddSingleClick(OnGotoServerListBtnClicked);
            _btnLoginServer.AddSingleClick(OnGameServerBtnClicked);
        }

        /// <summary>
        /// 刷新UI
        /// 运行状态 1、爆满；2、拥挤；3、正常；4、维修
        /// </summary>
        /// <param name="obj"></param>
        public override void RefreshUI(object obj)
        {
            if (obj is not GameServer)
            {
                return;
            }
            _gameServer = obj as GameServer;

            // TODO 这里考虑配表
            Color color = Color.white;
            _textRunStateName.SetText("维修");

            if (_gameServer.RunState == 1)
            {
                _textRunStateName.SetText("爆满");
                color = Color.red;
            }
            else if (_gameServer.RunState == 2)
            {
                _textRunStateName.SetText("拥挤");
                color = Color.yellow;
            }
            else if (_gameServer.RunState == 3)
            {
                _textRunStateName.SetText("正常");
                color = Color.green;
            }

            _textRunStateName.color = color;

            string str = "";
            if (_gameServer.IsNew == 1)
            {
                // TODO 这里应该也需要配表
                str = "(新服)";
            }

            _textServerName.SetText(_gameServer.ServerName + str);
        }


        /// <summary>
        /// 跳转服务器列表按钮点击事件
        /// </summary>
        public void OnGotoServerListBtnClicked()
        {
            // 回调跳转服务器列表按钮点击事件
            GotoServerListBtnClickAction?.Invoke();
        }

        /// <summary>
        /// 选择服务器按钮点击事件
        /// </summary>
        public void OnGameServerBtnClicked()
        {
            // UI只负责调用Action，Action的具体内容写在Ctrl里面
            GameServerBtnClickAction?.Invoke(_gameServer);
        }

        public void OnDestroy()
        {
            _btnGotoServerList.RemoveSingleClick(OnGotoServerListBtnClicked);
            _btnLoginServer.RemoveSingleClick(OnGameServerBtnClicked);
        }
    }
}
