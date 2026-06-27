using UnityEngine;

namespace Manager
{
    /**
     * Title:池化对象标记
     * Desciption:由对象池自动挂载，记录资源路径，支持 Despawn(GameObject) 无需再传 path。
     **/
    public class PooledObject : MonoBehaviour
    {
        // 序列化资源路径
        [SerializeField, HideInInspector] private string _path;

        public string Path
        {
            get => _path;
            set => _path = value;
        }
    }
}
