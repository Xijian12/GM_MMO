using Common;
using Controller.InputController;
using Manager;
using System;
using UnityEngine;

namespace Controller.RoleController
{
    /**
 	* Title:主角控制器
 	* Desciption:主角控制器，用于管理主角的动画和移动
 	**/
    public class MainRoleCtrl : RoleCtrlBase
    {
        private readonly float StartJumpSencond = 0.26f;
        private PlayerInputCtrl _playerInputCtrl;
        private float _moveSpeed = 10f;    // 角色的移动速度

        private readonly float _rotateSpeed = 1000f;

        private TimerHandle _lifeTimer = TimerHandle.Invalid;


        protected override void OnAwake()
        {
            _playerInputCtrl = GetComponent<PlayerInputCtrl>();

            _playerInputCtrl.ShiftKeyIsPressEvenet += ShiftKeyIsPress;
            _playerInputCtrl.JumpingEvenet += Jumping;
        }

        /// <summary>
        /// 跳跃键是否按下，如果按下就可以跳跃
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void Jumping()
        {
            if (_roleState == RoleState.Jump)
            {
                return;
            }
            if (_lifeTimer.IsValid)
            {
                _lifeTimer.Cancel();
            }

            _lifeTimer = TimerMgr.Instance.Delay(
                StartJumpSencond,
                () => { _verticalHeigth += 8; },
                TimerType.GameTime,
                this,
                nameof(Jumping));

            ChangeState(RoleState.Jump);
        }

        /// <summary>
        /// Shift键是否按下，如果按下，角色就可以快速跑
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void ShiftKeyIsPress(bool isPress)
        {
            if (isPress)
            {
                _moveSpeed = 18;
            }
            else
            {
                _moveSpeed = 10;
            }
        }

        protected override void OnUpdate()
        {
            PlayerMovement();
        }

        /// <summary>
        /// 角色移动
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void PlayerMovement()
        {
            // 角色移动键是否按下
            if (_playerInputCtrl.Movement != Vector2.zero)
            {
                // 计算角色移动目标偏移量
                Vector3 targetPos = new Vector3(_playerInputCtrl.Movement.x, 0, _playerInputCtrl.Movement.y);
                targetPos = targetPos * Time.deltaTime * _moveSpeed;

                // 角色开始移动
                if (_moveSpeed == 10)
                {
                    _animator.SetFloat("Movement", 2);
                }
                else if (_moveSpeed == 18)
                {
                    _animator.SetFloat("Movement", 3);
                }

                // 从本地坐标系转换为世界坐标系
                targetPos = Camera.main.transform.TransformDirection(targetPos);
                targetPos.y = 0;

                // 角色朝向
                transform.rotation = Quaternion.RotateTowards(transform.rotation,
                Quaternion.LookRotation(targetPos),
                _rotateSpeed * Time.deltaTime);

                // 角色移动
                _characterController.Move(targetPos);
            }
            else
            {
                // 角色停止移动
                _animator.SetFloat("Movement", 0);
            }
        }

        /// <summary>
        /// 对象销毁时释放
        /// </summary>
        protected override void OnDespawn()
        {
            base.OnDespawn();
            CancelLifeTimer();
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
