using UnityEngine;

/**
 * Title:UI 基类
 * Desciption:所有UI都会继承这个UI基类
 **/
public class UIBase : MonoBehaviour
{
    public virtual void InitView()
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
}
