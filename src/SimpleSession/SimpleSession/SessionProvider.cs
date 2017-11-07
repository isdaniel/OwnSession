using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleSession
{
    public class MyApplication
    {
        private readonly string SessionID = "MySessionID";

        public HttpRequest Request { get; private set; }
        public HttpResponse Respone { get; private set; }

        public MyApplication(HttpContext context)
        {
            Respone = context.Response;
            Request = context.Request;
        }

        private static SessionPool _container = new SessionPool();

        private static SessionPool SessionPool
        {
            get
            {
                return _container;
            }
        }

        public SessionObject Session
        {
            get
            {
                return GetSessionObj();
            }
        }

        /// <summary>
        /// 取得Session對象
        /// </summary>
        /// <returns></returns>
        private SessionObject GetSessionObj()
        {
            Guid sessionGuid;
            var cookieSessionID = HttpContext.Current.Request.Cookies[SessionID];
            if (cookieSessionID == null)
            {
                sessionGuid = Guid.NewGuid();
                HttpCookie cookie = new HttpCookie(SessionID, sessionGuid.ToString())
                { Expires = DateTime.Now.AddDays(60) };
                Respone.Cookies.Add(cookie);
            }
            else
            {
                sessionGuid = Guid.Parse(cookieSessionID.Value);
            }
            return _container[sessionGuid];
        }
    }

    /// <summary>
    /// 存放所有Session池子
    /// </summary>
    public class SessionPool
    {
        private Dictionary<Guid, SessionObject> _SessionContain = new Dictionary<Guid, SessionObject>();

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
        private CacheDictionary cache = new CacheDictionary();

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