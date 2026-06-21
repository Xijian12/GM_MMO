using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;
using UniRx;

namespace UI.Tips
{
    /**
 	* Title:系统提示
 	* Desciption:
 	**/
    public class SystemTips : MonoBehaviour
    {
        [SerializeField, Header("提示文本")] private TMP_Text _textMsg;
        [SerializeField, Header("颜色曲线")] private AnimationCurve _colorCurve;
        [SerializeField, Header("移动曲线")] private AnimationCurve _moveCurve;

        public void RefreshUI(string msg)
        {
            _textMsg.SetText(msg);
            _textMsg.DOColor(Color.red, 2)
            .SetEase(_colorCurve);

            RectTransform rectTransform = transform as RectTransform;
            rectTransform.DOAnchorPosY(rectTransform.anchoredPosition.y + Random.Range(200, 260), 2)
            .SetEase(_moveCurve);

            // 定时销毁当前对象
            Observable.Timer(TimeSpan.FromSeconds(3)).Subscribe(_ =>
            {
                Destroy(gameObject);
            });
        }
    }
}
