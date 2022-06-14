using System;

namespace UnfrozenTestWork
{
    public abstract class SingletonBase<T> where T : class
    {
        private static readonly object _locker = new object();
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_locker)
                    {
                        if (_instance == null)
                        {
                            _instance = Activator.CreateInstance<T>();
                        }
                    }
                }
                return _instance;
            }
        }
    }
}