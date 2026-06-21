using DG.Tweening;
using TMPro;
using UnityEngine;

namespace UI.Style
{
    /**
     * Title:
     * Desciption:
     **/
    public class InputStyle01 : MonoBehaviour
    {
        [SerializeField, Header("占位符")] private TMP_Text _textPlaceholder;
        [SerializeField, Header("占位符上移高度")] private float _textMoveHeight = 20f;
        [SerializeField, Header("动画持续时间")] private float _animDuration = 0.1f;
        private float _textPosY;

        // 开始事件
        private void Start()
        {
            _textPosY = _textPlaceholder.rectTransform.anchoredPosition.y;
            TMP_InputField ipt = GetComponent<TMP_InputField>();

            if (!string.IsNullOrEmpty(ipt.text))
            {
                _textPlaceholder.rectTransform.DOAnchorPosY(_textPosY + _textMoveHeight, _animDuration);
            }

            // 输入框选中事件
            ipt.onSelect.AddListener((string str) =>
            {
                if (string.IsNullOrEmpty(ipt.text))
                {
                    _textPlaceholder.rectTransform.DOAnchorPosY(_textPosY + _textMoveHeight, _animDuration);
                }
            });

            // 输入框取消选中事件
            ipt.onDeselect.AddListener((string str) =>
            {
                if (string.IsNullOrEmpty(ipt.text))
                {
                    _textPlaceholder.rectTransform.DOAnchorPosY(_textPosY, _animDuration);
                }
            });
        }
    }
}
