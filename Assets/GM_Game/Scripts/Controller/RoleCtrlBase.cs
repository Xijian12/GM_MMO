using System;
using Common;
using UnityEngine;

namespace Controller
{
    /**
 	* Title:角色控制器基类
 	* Desciption:角色控制器基类，用于管理角色的动画和移动
 	**/
    public class RoleCtrlBase : MonoBehaviour
    {

        protected Animator _animator;
        protected CharacterController _characterController;
        protected float _verticalSpeed; // y轴方向的移动速度
        protected float _verticalHeigth;    // y轴方向移动的高度

        protected RoleState _roleState;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _characterController = GetComponent<CharacterController>();
            OnAwake();
        }
        private void Start()
        {
            OnStart();
        }

        private void Update()
        {
            // 检测是否在地面
            IsGround();

            OnUpdate();
        }

        private void OnDestroy()
        {
            OnDespawn();
        }


        private void IsGround()
        {
            if (_verticalHeigth > transform.localPosition.y)
            {
                _verticalSpeed = 20;
            }
            else if (_verticalHeigth < transform.localPosition.y)
            {
                _verticalHeigth = 0;
                _verticalSpeed = -20;

            }
            else
            {
                _verticalSpeed = -100;
            }

            _characterController.Move(transform.up * Time.deltaTime * _verticalSpeed);

            // 检测是否在地面
            if (CheckShareGround())
            {
                // 这里需要先判断一下当前正在播放的动画是不是循坏跳跃动画
                if (_animator.GetCurrentAnimatorStateInfo(0).IsTag("Jump_Loop"))
                {
                    _animator.SetInteger("Action", 23);
                }
                _verticalHeigth = transform.localPosition.y;
            }
        }

        /// <summary>
        /// 检测是否在地面
        /// </summary>
        /// <returns></returns>

        protected bool CheckShareGround()
        {
            // 检测当前位置半径范围内所有碰撞体，如果有碰撞则返回true
            return Physics.CheckSphere(transform.position, 0.1f, 1 << LayerMask.NameToLayer("Geometry"));
        }

        /// <summary>
        /// 初始化
        /// </summary>
        protected virtual void OnAwake()
        {

        }
        /// <summary>
        /// 开始
        /// </summary>
        protected virtual void OnStart()
        {

        }

        /// <summary>
        /// 更新
        /// </summary>
        protected virtual void OnUpdate()
        {

        }


        protected virtual void OnDespawn() { }


        public void ChangeState(RoleState state)
        {
            _roleState = state;
            switch (_roleState)
            {
                case RoleState.None:
                    _animator.SetInteger("Action", 0);
                    break;
                case RoleState.Idle:
                    _animator.SetInteger("Action", 1);
                    break;
                case RoleState.Run:
                    _animator.SetInteger("Action", 2);
                    break;
                case RoleState.FastRun:
                    _animator.SetInteger("Action", 3);
                    break;
                case RoleState.Jump:
                    _animator.SetInteger("Action", 21);
                    break;
                default:
                    break;
            }
        }

    }
}
