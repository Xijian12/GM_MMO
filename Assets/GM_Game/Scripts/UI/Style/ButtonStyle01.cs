using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

/**
 * Title:按钮的特效和动效
 * Desciption:
 **/
public class ButtonStyle01 : MonoBehaviour,
 IPointerEnterHandler,
 IPointerExitHandler,
 IPointerUpHandler,
 IPointerDownHandler
{
    private float _btnOriginScale = 1f;
    [SerializeField, Header("按钮默认缩放")] private float _btnDefaultScale = 1f;
    [SerializeField, Header("按钮按下缩放")] private float _btnDownScale = 0.8f;
    [SerializeField, Header("按钮按下缩放时间")] private float _btnDownScaleTime = 0.1f;
    [SerializeField, Header("UI特效对象")] private GameObject _effectGo;

    private void Awake()
    {
        _btnOriginScale = transform.localScale.x;
    }
    
    // 鼠标按下时事件
    public void OnPointerDown(PointerEventData eventData)
    {
        transform.DOScale(_btnOriginScale * _btnDownScale,_btnDownScaleTime);
    }

    // 鼠标抬起时事件
    public void OnPointerUp(PointerEventData eventData)
    {
        transform.DOScale(_btnOriginScale * _btnDefaultScale,_btnDownScaleTime);
    }

    // 鼠标进入时事件
    public void OnPointerEnter(PointerEventData eventData)
    {
        _effectGo.Show();
    }

    // 鼠标离开时事件
    public void OnPointerExit(PointerEventData eventData)
    {
        _effectGo.Hide();
    }
}
