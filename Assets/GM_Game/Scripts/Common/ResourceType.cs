namespace Common
{
    /**
     * Title:资源类型
     * Desciption:短路径加载时指定资源种类，用于拼路径前缀与缓存 Key。
     **/
    public enum ResourceType
    {
        Prefab = 0,
        Texture = 1,
        Sprite = 2,
        Audio = 3,
        Material = 4,
        Scriptable = 5,
        TextAsset = 6,
    }
}
