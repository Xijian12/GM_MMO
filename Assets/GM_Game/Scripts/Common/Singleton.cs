using UnityEngine;

namespace Common
{
	/**
 	* Title:单例的基类
 	* Desciption:
 	**/
	public class Singleton<T> where T : new()
	{
        private static T instance;
        private static readonly object instanceLock = new object();

        public static T Instance
        {
            get
            {
            	if(instance == null)
                {
                    lock (instanceLock)
                    {
						if(instance == null)
						{
							instance = new T();
						}
                    }
                }
                return instance;
            }
        }
    }
}
