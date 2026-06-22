using Common;
using Cysharp.Threading.Tasks;
using UI.Tips;
using UnityEngine;

namespace Manager
{
    /**
 	* Title:管理所有的Tips
 	* Desciption:
 	**/
    public class TipsMgr : Singleton<TipsMgr>
    {
        private const string SystemTipsPath = "UIPrefabs/SystemTips";

        public void ShowSystemTips(string msg)
        {
            ShowSystemTipsAsync(msg).Forget();
        }

        /// <summary>
        /// 显示系统提示
        /// </summary>
        /// <param name="msg">提示信息</param>
        /// <returns></returns>
        private async UniTaskVoid ShowSystemTipsAsync(string msg)
        {
            // TODO 后续使用 UIMgr 注入 Canvas，替换 GameObject.Find
            Transform parent = GameObject.Find("Canvas")?.transform;
            if (parent == null)
            {
                Debug.LogError("[TipsMgr] 未找到 Canvas。");
                return;
            }

            GameObject go = await GameObjectPoolMgr.Instance.SpawnAsync(SystemTipsPath, parent);
            if (go == null)
            {
                return;
            }

            go.transform.localPosition = new Vector3(0, 160, 0);
            go.transform.localScale = Vector3.one;

            if (go.TryGetComponent<SystemTips>(out SystemTips systemTips))
            {
                systemTips.RefreshUI(msg);
            }
        }
    }
}
