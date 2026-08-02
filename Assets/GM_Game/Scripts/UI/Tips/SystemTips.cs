using Common;
using DG.Tweening;
using Manager;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UI.Tips
{
    /**
 	* Title:系统提示
 	* Desciption:
 	**/
    public class SystemTips : MonoBehaviour, IPoolable
    {
        private const float LifeSeconds = 3f;

        [SerializeField, Header("提示文本")] private TMP_Text _textMsg;
        [SerializeField, Header("颜色曲线")] private AnimationCurve _colorCurve;
        [SerializeField, Header("移动曲线")] private AnimationCurve _moveCurve;

        private TimerHandle _lifeTimer = TimerHandle.Invalid;

        /// <summary>
        /// 从池中取出并激活后调用。
        /// </summary>
        public void OnSpawn()
        {
        }

        /// <summary>
        /// 归还池前调用，用于清理动画、事件、计时等。
        /// </summary>
        public void OnDespawn()
        {
            CancelLifeTimer();
            transform.DOKill();
        }

        /// <summary>
        /// 刷新UI。
        /// </summary>
        /// <param name="msg">提示文本</param>
        public void RefreshUI(string msg)
        {
            CancelLifeTimer();

            _textMsg.SetText(msg);
            _textMsg.DOColor(Color.red, 2f)
                .SetEase(_colorCurve);

            RectTransform rectTransform = transform as RectTransform;
            rectTransform.DOAnchorPosY(rectTransform.anchoredPosition.y + Random.Range(200, 260), 2f)
                .SetEase(_moveCurve);

            // Tips 用真实时间，游戏暂停时仍会按时归还
            _lifeTimer = TimerMgr.Instance.Delay(
                LifeSeconds,
                OnLifeTimerFinished,
                TimerType.RealTime,
                this,
                nameof(SystemTips));
        }

        private void OnLifeTimerFinished()
        {
            _lifeTimer = TimerHandle.Invalid;
            GameObjectPoolMgr.Instance.Despawn(gameObject);
        }

        /// <summary>
        /// 取消生命周期定时器。
        /// </summary>
        private void CancelLifeTimer()
        {
            if (!_lifeTimer.IsValid)
            {
                return;
            }

            _lifeTimer.Cancel();
            _lifeTimer = TimerHandle.Invalid;
        }
    }
}
