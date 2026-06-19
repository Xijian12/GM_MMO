using UnityEngine;

/**
 * Title:GameObject扩展类
 * Desciption:
 **/
public static class GameObjectUtils
{
    public static void Show(this GameObject go)
    {
        if(go == null) return;
        go.SetActive(true);
    }

    public static void Hide(this GameObject go)
    {
        if(go == null) return;
        go.SetActive(false);
    }
}
