using System;
using System.Threading;
using Common;
using Cysharp.Threading.Tasks;
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
        [SerializeField, Header("提示文本")] private TMP_Text _textMsg;
        [SerializeField, Header("颜色曲线")] private AnimationCurve _colorCurve;
        [SerializeField, Header("移动曲线")] private AnimationCurve _moveCurve;

        private CancellationTokenSource _lifeCts;

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
            CancelLifeTask();
            transform.DOKill();
        }

        /// <summary>
        /// 刷新UI。
        /// </summary>
        /// <param name="msg">提示文本</param>
        public void RefreshUI(string msg)
        {
            CancelLifeTask();
            _lifeCts = new CancellationTokenSource();

            _textMsg.SetText(msg);
            _textMsg.DOColor(Color.red, 2f)
                .SetEase(_colorCurve);

            RectTransform rectTransform = transform as RectTransform;
            rectTransform.DOAnchorPosY(rectTransform.anchoredPosition.y + Random.Range(200, 260), 2f)
                .SetEase(_moveCurve);

            DespawnAfterDelayAsync(_lifeCts.Token).Forget();
        }

        /// <summary>
        /// 延迟归还池。
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        private async UniTaskVoid DespawnAfterDelayAsync(CancellationToken cancellationToken)
        {
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(3f), cancellationToken: cancellationToken);
                GameObjectPoolMgr.Instance.Despawn(gameObject);
            }
            catch (OperationCanceledException)
            {
            }
        }

        /// <summary>
        /// 取消生命周期任务。
        /// </summary>
        private void CancelLifeTask()
        {
            if (_lifeCts == null)
            {
                return;
            }

            _lifeCts.Cancel();
            _lifeCts.Dispose();
            _lifeCts = null;
        }
    }
}
