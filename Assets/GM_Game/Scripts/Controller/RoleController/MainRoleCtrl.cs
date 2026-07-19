using Controller.InputController;
using UnityEngine;

namespace Controller.RoleController
{
    /**
 	* Title:主角控制器
 	* Desciption:主角控制器，用于管理主角的动画和移动
 	**/
    public class MainRoleCtrl : RoleCtrlBase
    {
        private PlayerInputCtrl _playerInputCtrl;
        private readonly float _moveSpeed = 10f;

        private readonly float _rotateSpeed = 1000f;

        protected override void OnAwake()
        {
            _playerInputCtrl = GetComponent<PlayerInputCtrl>();
        }

        private void Update()
        {
            // 角色移动键是否按下
            if (_playerInputCtrl.Movement != Vector2.zero)
            {
                // 计算角色移动目标偏移量
                Vector3 targetPos = new Vector3(_playerInputCtrl.Movement.x, 0, _playerInputCtrl.Movement.y);
                targetPos = targetPos * Time.deltaTime * _moveSpeed;

                // 角色开始移动
                _animator.SetFloat("Movement", 2);

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
    }
}
