using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdvanceASPNET.Infrastructure
{
    public class RemoveHeadersModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.PostRequestHandlerExecute += (o, e) => PostRequestHandlerExecute(context);
        }

        private void PostRequestHandlerExecute(HttpApplication app)
        {
            app.Response.Headers.Remove("X-AspNet-Version");
            app.Response.Headers.Remove("X-AspNetMvc-Version");
            app.Response.Headers.Remove("X-Powered-By");
            app.Response.Headers.Remove("Server");
            app.Response.Headers.Remove("X-SourceFiles");

        }


        public void Dispose()
        {
            
        }
    }
}