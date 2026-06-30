using System.Collections.Generic;
using Common;
using UnityEngine;

namespace UI
{
    /**
     * Title:UI 基类
     * Desciption:所有UI都会继承这个UI基类
     **/
    public class UIBase : MonoBehaviour
    {
        protected Dictionary<WindowType, WindowBase> windowDict;

        /// <summary>
        /// 初始化模块视图：注册子窗口并调用各 Window.InitView()
        /// </summary>
        public virtual void InitView()
        {
            windowDict = new Dictionary<WindowType, WindowBase>();
            // 注册窗口
            RegisterWindows();

            // 初始化窗口
            foreach (var window in windowDict.Values)
            {
                window.InitView();
            }

            OnWindowsInited();
        }

        /// <summary>
        /// 子类重写：将 Window 注册到 windowDict
        /// </summary>
        protected virtual void RegisterWindows()
        {
        }

        /// <summary>
        /// 子类重写：所有 Window 初始化完成后的逻辑（如默认显示某个窗口）
        /// </summary>
        protected virtual void OnWindowsInited()
        {
        }

        public virtual void Show()
        {
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 根据WindowType返回对应的窗口
        /// </summary>
        /// <param name="windowType">窗口类型</param>
        /// <returns>对应的窗口</returns>
        public WindowBase GetWindow(WindowType windowType)
        {
            return windowDict[windowType];
        }

        /// <summary>
        /// 根据WindowType显示对应的窗口
        /// </summary>
        /// <param name="windowType">窗口类型</param>
        public void ShowWindow(WindowType windowType, object obj = null)
        {
            if (windowDict == null || windowDict.Count == 0)
            {
                return;
            }
            // 先隐藏所有窗口
            foreach (var item in windowDict)
            {
                if (item.Key == windowType)
                {
                    item.Value.Show(obj);
                }
                else
                {
                    item.Value.Hide();
                }
            }
        }
    }
}
