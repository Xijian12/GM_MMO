using DG.Tweening;
using TMPro;
using UnityEngine;
/**
 * Title:
 * Desciption:
 **/
public class InputStyle01 : MonoBehaviour
{
    [SerializeField, Header("占位符")] private TMP_Text _textPlaceholder;
    [SerializeField, Header("占位符上移高度")] private float _textMoveHeight = 20f;
    [SerializeField,Header("动画持续时间")] private float _animDuration = 0.1f;
    private float _textPosY;

    private void Start()
    {
        _textPosY = _textPlaceholder.rectTransform.anchoredPosition.y;
        TMP_InputField ipt = GetComponent<TMP_InputField>();

        // 默认情况下，如果输入框不为空，则占位符向上移动_textMoveHeight
        if (!string.IsNullOrEmpty(ipt.text))
        {
            //DOTWEEN
            _textPlaceholder.rectTransform.DOAnchorPosY(_textPosY + _textMoveHeight, _animDuration);
        }

        // 当输入框选中时，占位符向上移动_textMoveHeight
        ipt.onSelect.AddListener((string str) =>
        {
            // 这里的str和ipt.text是等价的
            if (string.IsNullOrEmpty(ipt.text))
            {
                _textPlaceholder.rectTransform.DOAnchorPosY(_textPosY + _textMoveHeight, _animDuration);
            }
        });

        // 当不被选中时，回到默认位置
        ipt.onDeselect.AddListener((string str) =>
        {
            if (string.IsNullOrEmpty(ipt.text))
            {
                _textPlaceholder.rectTransform.DOAnchorPosY(_textPosY, _animDuration);
            }
        });
    }
}
