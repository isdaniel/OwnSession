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

        public void ProcessRequest(HttpContext context)
        {
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