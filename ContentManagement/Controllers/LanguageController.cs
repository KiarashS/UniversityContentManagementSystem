using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace ContentManagement.Controllers
{
    public partial class LanguageController : Controller
    {
        public virtual IActionResult Fa()
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(new CultureInfo("en-US"), new CultureInfo("fa-IR"))),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return RedirectToAction(MVC.Home.ActionNames.Index, MVC.Home.Name);
        }

        public virtual IActionResult En()
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(new CultureInfo("en-US"), new CultureInfo("en-US"))),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return RedirectToAction(MVC.Home.ActionNames.Index, MVC.Home.Name);
        }
    }
}