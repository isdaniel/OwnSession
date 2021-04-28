using System;
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
        private readonly string _mySessionID = "CustomerSessionID";

        public HttpRequest Request { get; private set; }
        public HttpResponse Respone { get; private set; }

        public ApplicationContext(HttpContext context)
        {
            Respone = context.Response;
            Request = context.Request;
        }

        private static readonly SessionPool _container = new SessionPool();

        public SessionObject Session => GetSessionObj();

        /// <summary>
        /// 從SessionPool中取得Session對象
        /// </summary>
        /// <returns></returns>
        private SessionObject GetSessionObj()
        {
            Guid sessionGuid;
            HttpCookie cookieSessionId = Request.Cookies[_mySessionID];
            //如果沒有MySessionID的cookie，做一個新的
            if (cookieSessionId == null)
            {
                sessionGuid = Guid.NewGuid();
                HttpCookie cookie = new HttpCookie(_mySessionID, sessionGuid.ToString())
                {
                    Expires = DateTime.Now.AddDays(60)
                };
                Respone.Cookies.Add(cookie);
            }
            else
            {
                sessionGuid = Guid.Parse(cookieSessionId.Value);
            }
            return _container[sessionGuid];
        }
    }
}