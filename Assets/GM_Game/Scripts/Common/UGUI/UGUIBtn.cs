using System;
using Unity.VisualScripting;
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

        private event Action ClickAction;

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(OnButtonClick);
        }

        public void SetInteractable(bool value)
        {
            button.interactable = value;
        }

        public void SetActive(bool value)
        {
            gameObject.SetActive(value);
        }

        public void SetText(string text)
        {
            var tmp = GetComponentInChildren<TMPro.TMP_Text>();
            if (tmp != null)
                tmp.text = text;
        }

        private void OnButtonClick()
        {
            ClickAction?.Invoke();
        }

        public void AddClick(Action action)
        {
            ClickAction += action;
        }

        public void RemoveClick(Action action)
        {
            ClickAction -= action;
        }

        private void OnDestroy()
        {
            button.onClick.RemoveListener(OnButtonClick);
            ClickAction = null;
        }
    }
}
