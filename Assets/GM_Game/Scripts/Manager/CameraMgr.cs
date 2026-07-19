using System;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Manager
{
    /**
 	* Title:
 	* Desciption:
 	**/
    public class CameraMgr : MonoBehaviour
    {
        private CinemachineFreeLook _cinemachineFreeLook;
        private Mouse _mouse;

        [SerializeField, Header("轨道相机高度和半径的缩放时间")] private float _delayTime = 0.5f;
        [SerializeField, Header("轨道相机Y轴旋转速度")] private float _yAxisSpeed = 0.1f;
        [SerializeField, Header("轨道相机X轴旋转速度")] private float _xAxisSpeed = 10f;

        private void Awake()
        {
            _cinemachineFreeLook = GetComponent<CinemachineFreeLook>();
            _mouse = Mouse.current;

            SetOrbitCamera(10f); // 设置轨道相机的高度和半径
        }


        private void Update()
        {
            // 摄像机缩放功能
            // 1、获取鼠标滚轮的滚动值
            float scrollValue = _mouse.scroll.y.ReadValue();
            if (scrollValue != 0)
            {
                // 2、根据滚动值缩放轨道相机
                float newHeight = _cinemachineFreeLook.m_Orbits[0].m_Height - scrollValue * Time.deltaTime;
                SetOrbitCamera(newHeight);
            }

            // 摄像机旋转功能
            CameraRotation();
        }

        /// <summary>
        /// 摄像机旋转功能
        /// </summary>
        private void CameraRotation()
        {
            // 1、获取鼠标移动的值
            float mouseX = _mouse.delta.x.ReadValue();
            float mouseY = _mouse.delta.y.ReadValue();
            // 2、根据鼠标移动的值旋转轨道相机
            _cinemachineFreeLook.m_XAxis.Value += mouseX * Time.deltaTime * _xAxisSpeed;
            _cinemachineFreeLook.m_YAxis.Value -= mouseY * Time.deltaTime * _yAxisSpeed;
        }

        /// <summary>
        /// 设置轨道相机的高度和半径
        /// </summary>
        /// <param name="offset">轨道相机的高度和半径的偏移量</param>
        private void SetOrbitCamera(float offset)
        {
            // 限制轨道相机的高度和半径
            offset = Mathf.Clamp(offset, 5f, 15f);
            // Top Rig
            DOTween.To(() => _cinemachineFreeLook.m_Orbits[0].m_Height,
            x => _cinemachineFreeLook.m_Orbits[0].m_Height = x,
            offset, _delayTime);
            DOTween.To(() => _cinemachineFreeLook.m_Orbits[0].m_Radius,
            x => _cinemachineFreeLook.m_Orbits[0].m_Radius = x,
            offset * 0.25f, _delayTime);

            // Middle Rig
            DOTween.To(() => _cinemachineFreeLook.m_Orbits[1].m_Height,
            x => _cinemachineFreeLook.m_Orbits[1].m_Height = x,
            offset * 0.5f, _delayTime);
            DOTween.To(() => _cinemachineFreeLook.m_Orbits[1].m_Radius,
            x => _cinemachineFreeLook.m_Orbits[1].m_Radius = x,
            offset * 0.75f, _delayTime);

            // Bottom Rig
            DOTween.To(() => _cinemachineFreeLook.m_Orbits[2].m_Radius,
            x => _cinemachineFreeLook.m_Orbits[2].m_Radius = x,
            offset * 0.15f, _delayTime);
        }
    }
}
