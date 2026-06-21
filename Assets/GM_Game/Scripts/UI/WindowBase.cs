using UnityEngine;

namespace UI
{

    /**
     * Title:
     * Desciption:
     **/
    public class WindowBase : MonoBehaviour
    {
        public virtual void InitView()
        {
            // 初始化窗口
        }

        public virtual void Show()
        {
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}