using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ContentManagement.Controllers
{
    public partial class LanguageController : Controller
    {
        public virtual IActionResult Fa()
        {
            //var locOptions = (IOptions<RequestLocalizationOptions>)HttpContext.RequestServices.GetService(typeof(IOptions<RequestLocalizationOptions>));
            //var provider = locOptions.Value.RequestCultureProviders.SingleOrDefault(x => x.GetType() == typeof(CookieRequestCultureProvider));
            //var cookieName = ((CookieRequestCultureProvider)provider).CookieName;

            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(new CultureInfo("en-US"), new CultureInfo("fa-IR"))),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return RedirectToAction("index", "home");
        }

        public virtual IActionResult En()
        {
            //var locOptions = (IOptions<RequestLocalizationOptions>)HttpContext.RequestServices.GetService(typeof(IOptions<RequestLocalizationOptions>));
            //var provider = locOptions.Value.RequestCultureProviders.SingleOrDefault(x => x.GetType() == typeof(CookieRequestCultureProvider));
            //var cookieName = ((CookieRequestCultureProvider)provider).CookieName;

            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(new CultureInfo("en-US"), new CultureInfo("en-US"))),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return RedirectToAction("index", "home");
        }
    }
}