using UnityEngine;
using TMPro;

namespace GM
{
    /**
 	* Title: TMP 字形像素级点击检测
 	* Desciption: 基于 SDF 图集采样，精确判断鼠标是否点到字符笔画（如 O 的环）上
 	**/
    [RequireComponent(typeof(TMP_Text))]
    public class TMPClickOutline : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private Camera _eventCamera;
        [Range(0f, 1f)]
        [SerializeField] private float _sdfThreshold = 0.5f;

        // SDF 图集（需 CPU 可读：动态字体默认可读，静态字体须在 FontAsset 勾选 Read/Write）
        private Texture2D _atlas;

        private void Awake()
        {
            if (_text == null)
            {
                _text = GetComponent<TMP_Text>();
            }

            _atlas = _text.font.atlasTexture;
        }

        private void Update()
        {
            if (!Input.GetMouseButtonDown(0))
            {
                return;
            }

            Vector2 screenPos = Input.mousePosition;

            // 1) 找到鼠标命中的字符（按字符四边形 quad 判定）
            int charIndex = TMP_TextUtilities.FindIntersectingCharacter(_text, screenPos, _eventCamera, true);
            if (charIndex == -1)
            {
                return;
            }

            // 2) 把屏幕坐标转成 Text 本地坐标
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _text.rectTransform, screenPos, _eventCamera, out Vector2 local))
            {
                return;
            }

            TMP_CharacterInfo c = _text.textInfo.characterInfo[charIndex];

            // 3) 计算鼠标在该字符 quad 内的归一化坐标 (0~1)
            float nx = Mathf.InverseLerp(c.bottomLeft.x, c.topRight.x, local.x);
            float ny = Mathf.InverseLerp(c.bottomLeft.y, c.topRight.y, local.y);

            // 4) 映射到图集 UV
            float u = Mathf.Lerp(c.vertex_BL.uv.x, c.vertex_TR.uv.x, nx);
            float v = Mathf.Lerp(c.vertex_BL.uv.y, c.vertex_TR.uv.y, ny);

            // 5) 采样 SDF 值，判断是否在笔画上
            float sdf = _atlas.GetPixelBilinear(u, v).a;
            if (sdf >= _sdfThreshold)
            {
                Debug.Log($"点到了字符 '{c.character}' 的笔画上");
            }
        }
    }
}
