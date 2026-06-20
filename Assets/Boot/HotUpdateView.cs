using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/**
 * Title:
 * Desciption:
 **/
public class HotUpdateView : MonoBehaviour
{
    [SerializeField] private TMP_Text _textTips;
    [SerializeField] private Slider _slider;
    [SerializeField] private TMP_Text _textPgrs;
    [SerializeField] private Image _imgPoint;

    private float _sliderWidth;

    private void Start()
    {
        RectTransform sliderTrans = _slider.transform as RectTransform;
        _sliderWidth = sliderTrans.rect.width;
        
    }

    /// <summary>
    /// 刷新UI
    /// </summary>
    /// <param name="prgs">进度</param>
    /// <param name="prgsText">进度文本</param>
    /// <param name="prgsText"></param>
    public void RefreshUI(float prgs, string prgsText)
    {
        _slider.value = prgs;
        _textPgrs.SetText(prgsText);

        _imgPoint.rectTransform.anchoredPosition = new Vector3(_sliderWidth * prgs, 0, 0);
    }
}
