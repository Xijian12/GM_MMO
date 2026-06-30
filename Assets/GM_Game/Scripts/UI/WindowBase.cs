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

        public virtual void Show(object obj)
        {
            gameObject.SetActive(true);

            if (obj != null)
            {
                RefreshUI(obj);
            }
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }

        public virtual void RefreshUI(object obj)
        {

        }
    }
}