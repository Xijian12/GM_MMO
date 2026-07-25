using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Controller.InputController
{
    /**
 	* Title:玩家输入控制器
 	* Desciption:玩家输入控制器，用于管理玩家输入
 	**/
    public class PlayerInputCtrl : MonoBehaviour
    {
        private PlayerInput _input; // 玩家输入

        public Action<bool> ShiftKeyIsPressEvenet;

        public Vector2 Movement => _input.Player.Movement.ReadValue<Vector2>();
        public bool Jump => _input.Player.Jump.WasPressedThisFrame();

        private void Awake()
        {
            _input = new PlayerInput();

            RegisterInputEvent();
        }

        /// <summary>
        /// 注册输入事件
        /// </summary>
        private void RegisterInputEvent()
        {
            // shift键按下时的回调
            _input.Player.Shift.started += (InputAction.CallbackContext ctx) =>
            {
                ShiftKeyIsPressEvenet?.Invoke(true);
            };

            // shift键抬起时的回调
            _input.Player.Shift.canceled += (InputAction.CallbackContext ctx) =>
            {
                ShiftKeyIsPressEvenet?.Invoke(false);

            };
        }

        private void OnEnable()
        {
            _input.asset.Enable();
        }


        private void OnDisable()
        {
            _input.asset.Disable();
        }
    }
}
