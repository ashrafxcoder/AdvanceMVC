using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AdvanceASPNET.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class WwwRequirementAttribute : FilterAttribute, IAuthorizationFilter
    {
        public virtual void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
                throw new ArgumentNullException("filterContext");

            //don't apply filter to child methods
            if (filterContext.IsChildAction)
                return;

            // only redirect for GET requests, 
            // otherwise the browser might not propagate the verb and request body correctly.
            if (!String.Equals(filterContext.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
                return;

            //ignore this rule for localhost
            if (filterContext.HttpContext.Request.IsLocal)
                return;

            var seoSetting = SeoSettings.WithWww;

            string url = filterContext.HttpContext.Request.Url.ToString();
            bool currentConnectionSecured = filterContext.HttpContext.Request.IsSecureConnection;

            switch (seoSetting)
            {
                case SeoSettings.WithWww:
                    {
                        
                        if (currentConnectionSecured)
                        {
                            bool startsWith3W = url.StartsWith("https://www.", StringComparison.OrdinalIgnoreCase);
                            if (!startsWith3W)
                            {
                                url = url.Replace("https://", "https://www.");

                                //301 (permanent) redirection
                                filterContext.Result = new RedirectResult(url, true);
                            }
                        }
                        else
                        {
                            bool startsWith3W = url.StartsWith("http://www.", StringComparison.OrdinalIgnoreCase);
                            if (!startsWith3W)
                            {
                                url = url.Replace("http://", "http://www.");

                                //301 (permanent) redirection
                                filterContext.Result = new RedirectResult(url, true);
                            }
                        }
                    }
                    break;
                case SeoSettings.WithoutWww:
                    {
                        
                        if (currentConnectionSecured)
                        {
                            bool startsWith3W = url.StartsWith("https://www.", StringComparison.OrdinalIgnoreCase);
                            if (startsWith3W)
                            {
                                url = url.Replace("https://www.", "https://");

                                //301 (permanent) redirection
                                filterContext.Result = new RedirectResult(url, true);
                            }
                        }
                        else
                        {
                            bool startsWith3W = url.StartsWith("http://www.", StringComparison.OrdinalIgnoreCase);
                            if (startsWith3W)
                            {
                                url = url.Replace("http://www.", "http://");

                                //301 (permanent) redirection
                                filterContext.Result = new RedirectResult(url, true);
                            }
                        }
                    }
                    break;
                case SeoSettings.NoMatter:
                    {
                        //do nothing
                    }
                    break;
                default:
                    throw new Exception("Not supported WwwRequirement parameter");
            }
        }
    }


    public enum SeoSettings
    {
        WithWww,
        WithoutWww,
        NoMatter
    }
}