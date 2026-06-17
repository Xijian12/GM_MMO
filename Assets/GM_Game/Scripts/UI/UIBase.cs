using UnityEngine;

/**
 * Title:UI 基类
 * Desciption:适用于所有UI的Base类
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
