using System;

/**
 * Title:控制器基类
 * Desciption:
 **/
public class CtrlBase : IDisposable
{
    public CtrlBase(UIBase view)
    {

    }

    public virtual void ShowView()
    {
    }

    public virtual void HideView()
    {
    }

    public virtual void Dispose()
    {

    }
}
