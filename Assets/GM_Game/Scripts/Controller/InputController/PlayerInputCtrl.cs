using UnityEngine;

namespace Controller.InputController
{
	/**
 	* Title:玩家输入控制器
 	* Desciption:玩家输入控制器，用于管理玩家输入
 	**/
	public class PlayerInputCtrl : MonoBehaviour
	{
		private PlayerInput _input; // 玩家输入
		
		public Vector2 Movement => _input.Player.Movement.ReadValue<Vector2>();
		public bool Jump => _input.Player.Jump.WasPressedThisFrame();

		private void Awake()
		{
			_input = new PlayerInput();
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
