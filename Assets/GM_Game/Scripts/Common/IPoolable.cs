namespace Common
{
    /**
     * Title:对象池实例生命周期接口
     * Desciption:可选实现，用于在取出/归还时重置状态；不能替代显式 Despawn 调用。
     **/
    public interface IPoolable
    {
        /// <summary>
        /// 从池中取出并激活后调用。
        /// </summary>
        void OnSpawn();

        /// <summary>
        /// 归还池前调用，用于清理动画、事件、计时等。
        /// </summary>
        void OnDespawn();
    }
}
