using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleSession
{

    public class SessionProvider
    {
        private SessionProvider() { }
        /// <summary>
        /// 
        /// </summary>
        static SessionPool _container = new SessionPool();
        public static SessionPool Session { get { return _container; } }
    }

    /// <summary>
    /// 存放所有Session池子
    /// </summary>
    public class SessionPool
    {
        Dictionary<Guid, SessionObject> _SessionContain = new Dictionary<Guid, SessionObject>();
        public SessionObject this[Guid index]
        {
            get
            {
                SessionObject obj;
                if (_SessionContain.TryGetValue(index, out obj))
                {
                    return obj;
                }
                else
                {
                    obj = new SessionObject();
                    _SessionContain.Add(index, obj);
                }
                return obj;
            }
        }
    }

    /// <summary>
    /// Session物件
    /// </summary>
    public class SessionObject
    {
        CacheDictionary cache = new CacheDictionary();

        public object this[string index]
        {
            get
            {
                return GetObj(index);
            }
            set
            {
                SetCache(index, value);
            }
        }

        private void SetCache(string key, object value)
        {
            cache.Set(key, () => value);
        }

        private object GetObj(string key)
        {
            return cache.GetOrDefault(key, () => default(object));
        }

    }
}