using UI.Login;
using Unity.VisualScripting;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.InputSystem;

/**
 * Title:UI和控制器的管理类
 * Desciption:管理所有的UI和控制器
 **/
public class UIRoot : MonoBehaviour
{
    public static UIRoot Instance { get; private set; }
    public LoginCtrl LoginCtrl;

    [SerializeField, Header("登录视图")] private LoginView _loginView;
    [SerializeField, Header("点击特效")] private ParticleSystem _clickFX;
    [SerializeField, Header("UI画布")] private Canvas _uiCanvas;

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
                // 播放点击特效
                // !! 需要把LoginView下的背景图的射线检测开启，不然无法点击 !!
                _clickFX.transform.localPosition = ScreenPointToUIPoint(Mouse.current.position.ReadValue());
                _clickFX.Play();
                // TODO 播放点击音效
                // AudioManager.Instance.PlaySound("Click");
            }
        }
    }
    private void InitCtrl()
    {
        if (_loginView == null) return;

        LoginCtrl = new LoginCtrl(_loginView);
    }

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
