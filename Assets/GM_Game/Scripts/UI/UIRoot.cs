using UI.CreateRole;
using UI.Login;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace UI
{
    /**
     * Title:UI和控制器的管理类
     * Desciption:管理所有的UI和控制器
     **/
    public class UIRoot : MonoBehaviour
    {
        public static UIRoot Instance { get; private set; }

        [SerializeField, Header("登录视图")] private LoginView _loginView;
        public LoginCtrl LoginViewCtrl;

        [SerializeField, Header("点击特效")] private ParticleSystem _clickFX;
        [SerializeField, Header("UI画布")] private Canvas _uiCanvas;

        [SerializeField, Header("创建角色视图")] private CreateRoleView _createRoleView;
        public CreateRoleCtrl CreateRoleViewCtrl;

        private void Awake()
        {
            Instance = this;

            DontDestroyOnLoad(this);

            InitCtrl();
        }

        private void Start()
        {
            // 获取UI画布
            _uiCanvas = GetComponentInChildren<Canvas>();
        }

        private void Update()
        {
            // 判断鼠标左键是否被按下
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                // 判断鼠标是否在UI上
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    _clickFX.transform.localPosition = ScreenPointToUIPoint(Mouse.current.position.ReadValue());
                    _clickFX.Play();
                }
            }
        }

        /// <summary>
        /// 初始化控制器
        /// </summary>
        private void InitCtrl()
        {
            // 初始化登录控制器
            if (_loginView != null)
            {
                LoginViewCtrl = new LoginCtrl(_loginView);
            }

            // 初始化创建角色控制器
            if (_createRoleView != null)
            {
                CreateRoleViewCtrl = new CreateRoleCtrl(_createRoleView);
            }
        }

        /// <summary>
        /// 将屏幕坐标转换为UI坐标
        /// </summary>
        public Vector2 ScreenPointToUIPoint(Vector2 screenPoint)
        {
            // 将屏幕坐标转换为UI坐标
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_uiCanvas.transform as RectTransform, screenPoint, _uiCanvas.worldCamera, out var localPoint))
            {
                return localPoint;
            }
            return Vector2.zero;
        }
    }
}
