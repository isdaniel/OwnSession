using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleSession
{
    /// <summary>
    /// SessionHanlder 的摘要描述
    /// </summary>
    public class SessionHanlder : IHttpHandler
    {
        //請求上下文
        private ApplicationContext app;

        public SessionHanlder()

        {
            app = new ApplicationContext(HttpContext.Current);
        }

        public void ProcessRequest(HttpContext context)
        {
            if (null == app.Session["Time"])
            {
                app.Session["Time"] = $"Hello {DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss")}";
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