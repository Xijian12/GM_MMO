namespace Manager
{
    /**
     * Title:定时器句柄
     * Desciption:用于取消/查询任务；Id + Version 防止复用后误取消。
     **/
    public readonly struct TimerHandle
    {
        public readonly int Id;
        public readonly int Version;

        public static readonly TimerHandle Invalid = new TimerHandle(0, 0); // 

        public TimerHandle(int id, int version)
        {
            Id = id;
            Version = version;
        }

        public bool IsValid => Id > 0 && TimerMgr.Instance.IsHandleValid(this);

        public void Cancel()
        {
            if (Id <= 0)
            {
                return;
            }

            TimerMgr.Instance.Cancel(this);
        }

        public void Pause()
        {
            if (Id <= 0)
            {
                return;
            }

            TimerMgr.Instance.Pause(this);
        }

        public void Resume()
        {
            if (Id <= 0)
            {
                return;
            }

            TimerMgr.Instance.Resume(this);
        }
    }
}
