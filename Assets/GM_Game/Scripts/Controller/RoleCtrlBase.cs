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
	}
}
