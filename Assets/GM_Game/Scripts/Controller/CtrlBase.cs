using System;
using Common;

namespace UI
{
    /**
     * Title:控制器基类
     * Desciption:
     **/
    public class CtrlBase : IDisposable
    {
        protected UIBase _view;

        public CtrlBase(UIBase view)
        {
            _view = view;
        }

        public virtual void ShowView()
        {
            _view.Show();
        }

        public virtual void HideView()
        {
            _view.Hide();
        }

        public virtual void Dispose()
        {

        }

        public virtual void ShowWindow(WindowType windowType, object obj = null)
        {
            if (!_view.gameObject.activeSelf)
            {
                _view.Show();
            }
            _view.ShowWindow(windowType, obj);
        }
    }
}
