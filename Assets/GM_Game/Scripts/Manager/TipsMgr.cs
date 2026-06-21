using Common;
using GM;
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
		public void ShowSystemTips(string msg)
		{
			ResourceMgr.Instance.LoadAssetAsync<GameObject>("UIPrefabs/SystemTips", (go) =>
            {
            	if(go == null)
            	{
            		return;
                }
                // TODO 后续使用UIMgr进行Canvas获取
                go.transform.SetParent(GameObject.Find("Canvas").transform);
				go.transform.localPosition = new Vector3(0, 160, 0);
                go.transform.localScale = Vector3.one;

                if (go.TryGetComponent<SystemTips>(out var systemTips))
                {
                	systemTips.RefreshUI(msg);
                }
            });
		}
	}
}
