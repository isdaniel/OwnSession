using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleSession
{
    /// <summary>
    /// 請求上下文
    /// </summary>
    public class ApplicationContext
    {
        /// <summary>
        /// 存在Cookie中的SessionID
        /// </summary>
        private readonly string MySessionID = "MySessionID";

        public HttpRequest Request { get; private set; }
        public HttpResponse Respone { get; private set; }

        public ApplicationContext(HttpContext context)
        {
            Respone = context.Response;
            Request = context.Request;
        }

        private static SessionPool _container = new SessionPool();

        public SessionObject Session
        {
            get
            {
                return GetSessionObj();
            }
        }

        /// <summary>
        /// 從SessionPool中取得Session對象
        /// </summary>
        /// <returns></returns>
        private SessionObject GetSessionObj()
        {
            Guid sessionGuid;
            HttpCookie CookieSessionID = Request.Cookies[MySessionID];
            //如果沒有MySessionID的cookie，做一個新的
            if (CookieSessionID == null)
            {
                sessionGuid = Guid.NewGuid();
                HttpCookie cookie = new HttpCookie(MySessionID, sessionGuid.ToString())
                {
                    Expires = DateTime.Now.AddDays(60)
                };
                Respone.Cookies.Add(cookie);
            }
            else
            {
                sessionGuid = Guid.Parse(CookieSessionID.Value);
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