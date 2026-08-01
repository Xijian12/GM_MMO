using UnityEngine;

namespace Manager
{
    /**
     * Title:定时器驱动器
     * Desciption:挂在 Global 下，每帧驱动 TimerMgr.Tick，禁止业务直接依赖。
     **/
    internal sealed class TimerDriver : MonoBehaviour
    {
        private void Update()
        {
            TimerMgr.Instance.Tick(Time.deltaTime, Time.unscaledDeltaTime);
        }
    }
}
