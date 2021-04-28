using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleSession
{
    /// <summary>
    /// SessionHandler 的摘要描述
    /// </summary>
    public class SessionHandler : IHttpHandler
    {
        //請求上下文
        private readonly ApplicationContext app;

        public SessionHandler()

        {
            app = new ApplicationContext(HttpContext.Current);
        }

        public void ProcessRequest(HttpContext context)
        {
            if (null == app.Session["Time"])
            {
                app.Session["Time"] = $"Hello {DateTime.Now:yyyy-MM-dd hh-mm-ss}";
            }
            context.Response.Write(app.Session["Time"]);
            context.Response.ContentType = "text/plain";
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}