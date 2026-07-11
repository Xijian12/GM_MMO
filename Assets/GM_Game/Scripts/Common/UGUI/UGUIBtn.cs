using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Common
{

    /**
     * Title:UGUIBtn
     * Desciption:用于封装原有的Button组件事件
     **/
    [RequireComponent(typeof(Button))]
    public class UGUIBtn : MonoBehaviour
    {
        private Button button;
        private TMP_Text _cachedText;

        private event Action SingleClickAction;
        private event Action DoubleClickAction;

        private float _lastClickTime;
        // 单击事件的取消令牌
        private CancellationTokenSource _singleClickCts;

        private const float DoubleClickThreshold = 0.3f;

        // 是否有双击事件
        private bool HasDoubleClickListener => DoubleClickAction != null;

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(OnButtonClick);
            _cachedText = GetComponentInChildren<TMP_Text>(true);
        }

        /// <summary>
        /// 设置按钮是否可交互
        /// </summary>
        public void SetInteractable(bool value)
        {
            button.interactable = value;
        }

        /// <summary>
        /// 设置按钮是否可见
        /// </summary>
        public void SetActive(bool value)
        {
            gameObject.SetActive(value);
        }

        /// <summary>
        /// 设置按钮文本
        /// </summary>
        public void SetText(string text)
        {
            if (_cachedText != null)
            {
                _cachedText.text = text;
            }
        }

        /// <summary>
        /// 按钮点击：无双击监听时立即单击；有双击监听时延迟触发单击，间隔内第二次点击视为双击。
        /// </summary>
        private void OnButtonClick()
        {
            // 没有双击事件时，立即触发单击事件
            if (!HasDoubleClickListener)
            {
                SingleClickAction?.Invoke();
                return;
            }

            float now = Time.unscaledTime;
            if (_lastClickTime > 0f && now - _lastClickTime < DoubleClickThreshold)
            {
                // 双击：仅在此处取消第一次点击挂起的延迟单击
                CancelPendingSingleClick();
                _lastClickTime = 0f;
                DoubleClickAction?.Invoke();
                return;
            }

            _lastClickTime = now;
            _singleClickCts = new CancellationTokenSource();
            FireSingleClickAfterDelayAsync(_singleClickCts.Token).Forget();
        }

        private async UniTaskVoid FireSingleClickAfterDelayAsync(CancellationToken cancellationToken)
        {
            try
            {
                await UniTask.Delay(
                    TimeSpan.FromSeconds(DoubleClickThreshold),
                    ignoreTimeScale: true,
                    cancellationToken: cancellationToken);
                _lastClickTime = 0f;
                SingleClickAction?.Invoke();
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                // 延迟单击正常结束或被取消后，释放令牌（避免重复创建 CTS 时泄漏）
                if (_singleClickCts != null && _singleClickCts.Token == cancellationToken)
                {
                    _singleClickCts.Dispose();
                    _singleClickCts = null;
                }
            }
        }

        /// <summary>
        /// 取消待触发的单击事件
        /// </summary>
        private void CancelPendingSingleClick()
        {
            if (_singleClickCts == null)
            {
                return;
            }

            _singleClickCts.Cancel();   // 取消单击事件
            _singleClickCts.Dispose(); // 释放单击事件的取消令牌
            _singleClickCts = null;
        }

        /// <summary>
        /// 添加单击事件
        /// </summary>
        public void AddSingleClick(Action action)
        {
            SingleClickAction += action;
        }

        /// <summary>
        /// 添加双击事件
        /// </summary>
        public void AddDoubleClick(Action action)
        {
            DoubleClickAction += action;
        }

        /// <summary>
        /// 移除单击事件
        /// </summary>
        public void RemoveSingleClick(Action action)
        {
            SingleClickAction -= action;
        }

        /// <summary>
        /// 移除双击事件
        /// </summary>
        public void RemoveDoubleClick(Action action)
        {
            DoubleClickAction -= action;
        }

        /// <summary>
        /// 移除全部点击事件，并取消待触发的延迟单击
        /// </summary>
        public void RemoveAllClick()
        {
            CancelPendingSingleClick();
            SingleClickAction = null;
            DoubleClickAction = null;
            _lastClickTime = 0f;
        }

        private void OnDestroy()
        {
            CancelPendingSingleClick();
            if (button != null)
            {
                button.onClick.RemoveListener(OnButtonClick);
            }

            SingleClickAction = null;
            DoubleClickAction = null;
        }
    }
}
