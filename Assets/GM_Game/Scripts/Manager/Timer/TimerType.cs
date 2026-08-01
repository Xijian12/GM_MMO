namespace Manager
{
    /**
     * Title:定时器时间类型
     * Desciption:GameTime 受 timeScale 影响；RealTime 不受暂停影响。
     **/
    public enum TimerType
    {
        /// <summary>
        /// 游戏时间（Time.deltaTime，受 timeScale 影响）
        /// </summary>
        GameTime = 0,

        /// <summary>
        /// 真实时间（Time.unscaledDeltaTime，不受暂停影响）
        /// </summary>
        RealTime = 1
    }
}
