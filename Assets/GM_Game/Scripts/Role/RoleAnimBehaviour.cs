using Common;
using Controller;
using UnityEngine;

namespace Role
{
    /**
 	* Title:
 	* Desciption:
 	**/
    public class RoleAnimBehaviour : MonoBehaviour
    {
        private RoleCtrlBase _roleCtrl;

        private void Awake()
        {
            _roleCtrl = GetComponent<RoleCtrlBase>();
        }

        private void AnimEnd()
        {
            _roleCtrl.ChangeState(RoleState.Idle);
        }

        private void PlayEffect(string tag)
        {
            switch (tag)
            {
                case "atk01":
                    // TODO: ResourceMgr.SpawnPrefabAsync / LoadAssetAsync 挂接攻击特效
                    break;
            }
        }
    }
}
