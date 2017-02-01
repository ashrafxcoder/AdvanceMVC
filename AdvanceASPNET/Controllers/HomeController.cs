using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AdvanceASPNET.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            Session["time"] = DateTime.Now;
            return View();
        }

        [CustomFilter]
        [AlreadyVisitedFilter]
        public ActionResult About()
        {
            ViewBag.Message = HttpContext.Items["Hello"];//"Your application description page.";
            
            var cookie = HttpContext.Items["visitStatus"];
            if (cookie != null)
            {
                ViewBag.Cookie = cookie;
            }

            //Response.AddHeader("Refresh", Session.Timeout + ";URL=Home/About");
            //Response.Headers.Add("Refresh", Session.Timeout + ";URL=Home/About");
            //Response.Headers.Add("Refresh", "5;" + "URL=http://localhost:54686/Home/About");

            //Response.Headers.Remove("X-AspNet-Version");
            //Response.Headers.Remove("X-AspNetMvc-Version");
            //Response.Headers.Remove("X-Powered-By");

            if (HttpContext.Cache["ASPDOTNET"] == null)
            {
                var ones = Enumerable.Repeat("One", 10000);
                HttpContext.Cache["ASPDOTNET"] = ones;
            }
            
            return View();
        }


        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }

    public class CustomFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.HttpContext.Items["Hello"] = "Hello from HttpContext.Items' Dictionary";
        }
    }


    public class AlreadyVisitedFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var ctx = filterContext.HttpContext;
            HttpCookieCollection cookies = ctx.Request.Cookies;

            HttpCookie userStatusCookie = cookies.Get("visitStatus");
            if (userStatusCookie != null)
            {
                ctx.Items["visitStatus"] = userStatusCookie.Value;
            }

            base.OnActionExecuting(filterContext);
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.HttpContext.Items["visitStatus"] == null)
            {
                HttpCookie cookie = new HttpCookie("visitStatus");
                cookie.Expires = DateTime.MaxValue;
                cookie.Value = filterContext.HttpContext.User.Identity.Name;
                filterContext.HttpContext.Response.Cookies.Add(cookie);
            }
            base.OnActionExecuted(filterContext);
        }
    }
}