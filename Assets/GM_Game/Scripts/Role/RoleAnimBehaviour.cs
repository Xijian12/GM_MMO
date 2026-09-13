using Common;
using Controller;
using UnityEngine;
using Manager;

namespace Role
{
    /**
 	* Title:
 	* Desciption:
 	**/
    public class RoleAnimBehaviour : MonoBehaviour
    {
        private RoleCtrlBase _roleCtrl;
        [SerializeField] private Transform _effectPos;

        private void Awake()
        {
            _roleCtrl = GetComponent<RoleCtrlBase>();
        }

        private void AnimEnd()
        {
            _roleCtrl.ChangeState(RoleState.Idle);
        }

        private async void PlayEffect(string tag)
        {
            switch (tag)
            {
                case "atk01":
                    // TODO: ResourceMgr.SpawnPrefabAsync / LoadAssetAsync 挂接攻击特效
                    GameObject attackFx01 = await ResourceMgr.Instance.LoadAssetAsync<GameObject>(ResourceType.Effect, "Attack/Attack01/Effect_Attack01");
                    attackFx01.transform.SetParent(_effectPos);
                    attackFx01.transform.localPosition = new Vector3(-0.18f, -0.58f, -0.58f);
                    attackFx01.transform.localEulerAngles = new Vector3(40.97f, 83.41f, 216.81f);
                    attackFx01.transform.localScale = Vector3.one;
                    attackFx01.SetActive(true);
                    break;
                case "atk02":
                    // TODO: ResourceMgr.SpawnPrefabAsync / LoadAssetAsync 挂接攻击特效
                    GameObject attackFx02 = await ResourceMgr.Instance.LoadAssetAsync<GameObject>(ResourceType.Effect, "Attack/Attack01/Effect_Attack01");
                    attackFx02.transform.SetParent(_effectPos);
                    attackFx02.transform.localPosition = new Vector3(0.09f, -0.2f, -0.17f);
                    attackFx02.transform.localEulerAngles = new Vector3(270f, 35.54f, 0);
                    attackFx02.transform.localScale = Vector3.one;
                    attackFx02.SetActive(true);
                    break;
                case "atk03_1":
                    // TODO: ResourceMgr.SpawnPrefabAsync / LoadAssetAsync 挂接攻击特效
                    GameObject attackFx03_1 = await ResourceMgr.Instance.LoadAssetAsync<GameObject>(ResourceType.Effect, "Attack/Attack01/Effect_Attack01");
                    attackFx03_1.transform.SetParent(_effectPos);
                    attackFx03_1.transform.localPosition = new Vector3(-0.33f, -0.5f, -0.45f);
                    attackFx03_1.transform.localEulerAngles = new Vector3(51.62f, 262.38f, 99.87f);
                    attackFx03_1.transform.localScale = Vector3.one;
                    attackFx03_1.SetActive(true);
                    break;
                case "atk03_2":
                    // TODO: ResourceMgr.SpawnPrefabAsync / LoadAssetAsync 挂接攻击特效
                    GameObject attackFx03_2 = await ResourceMgr.Instance.LoadAssetAsync<GameObject>(ResourceType.Effect, "Attack/Attack01/Effect_Attack01");
                    attackFx03_2.transform.SetParent(_effectPos);
                    attackFx03_2.transform.localPosition = new Vector3(0.92f, 0.36f, 0.07f);
                    attackFx03_2.transform.localEulerAngles = new Vector3(298.69f, 254.45f, 98.72f);
                    attackFx03_2.transform.localScale = Vector3.one;
                    attackFx03_2.SetActive(true);
                    break;
            }
        }
    }
}
