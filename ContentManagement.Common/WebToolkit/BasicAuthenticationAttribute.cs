using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Text;

namespace ContentManagement.Common.WebToolkit
{
    public class BasicAuthenticationAttribute : ActionFilterAttribute
    {
        public string BasicRealm { get; set; }
        protected string Username { get; set; }
        protected string Password { get; set; }

        public BasicAuthenticationAttribute(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var req = filterContext.HttpContext.Request;
            var auth = req.Headers["Authorization"].ToString();
            if (!string.IsNullOrEmpty(auth))
            {
                var cred = Encoding.ASCII.GetString(Convert.FromBase64String(auth.Substring(6))).Split(':');
                var user = new { Name = cred[0], Pass = cred[1] };
                if (user.Name == Username && user.Pass == Password) return;
            }
            var res = filterContext.HttpContext.Response;
            //res.StatusCode = 401;
            res.Headers.Add("WWW-Authenticate", string.Format("Basic realm=\"{0}\"", BasicRealm ?? "KiaCMS"));
            filterContext.Result = new UnauthorizedResult();
        }
    }


}