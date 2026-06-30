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

        internal void RefreshUI(GameServer gameServer)
        {
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
                str = "(新服)";
            }

            _textServerName.SetText(gameServer.ServerName + str);
        }
    }
}
