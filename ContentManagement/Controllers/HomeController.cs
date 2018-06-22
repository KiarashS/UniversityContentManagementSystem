using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using StackExchange.Exceptional;
using ContentManagement.Services.Contracts;
using Microsoft.Extensions.Options;
using ContentManagement.ViewModels.Settings;
using ContentManagement.Services;
using ContentManagement.Services.Seo;
using Microsoft.AspNetCore.Authorization;
using DNTBreadCrumb.Core;
using System.IO;

namespace ContentManagement.Controllers
{
    /// <summary>
    /// A controller intercepts the incoming browser request and returns
    /// an HTML view (.cshtml file) or any other type of data.
    /// </summary>
    public partial class HomeController : Controller
    {
        private readonly IHostingEnvironment _environment;
        private readonly IUsersService _userService;
        protected readonly IOptionsSnapshot<SiteSettings> _siteSettings;
        protected readonly IRequestService _requestService;

        public HomeController(IHostingEnvironment environment, IUsersService userService, IOptionsSnapshot<SiteSettings> siteSettings, IRequestService requestService)
        {
            _environment = environment;
            _userService = userService;
            _siteSettings = siteSettings;
            _requestService = requestService;
        }

        //[ResponseCache(Duration = 3600)]
        public virtual IActionResult Index()
        {
            // The view being returned is calculated based on the name of the
            // controller (Home) and the name of the action method (Index).
            // So in this case, the view returned is /Views/Home/Index.cshtml.
            var lang = _requestService.CurrentLanguage();
            var subDomain = _requestService.CurrentSubDomain();
            var isSubPortal = _requestService.IsSubPortal();
            var seoService = (SeoService)HttpContext.RequestServices.GetService(typeof(SeoService));
            ViewBag.Title = "کیارش سلیمان زاده";
            return View();
        }

        [ResponseCache(Duration = 3600)]
        public virtual IActionResult About()
        {
            return View();
        }

        //public virtual IActionResult RedirectToDefaultLanguage()
        //{
        //    var lang = CurrentLanguage;
        //    //if (lang == "et")
        //    //{
        //    //    lang = "ee";
        //    //}
        //    return RedirectToAction(MVC.Home.ActionNames.Index, new { lang = lang });
        //}

        //[Authorize]
        //[BasicAuthentication("kiarash", "admin@110", BasicRealm = "manage")]
        [Authorize(Policy = CustomRoles.Admin)]
        public async virtual Task Exceptions() => await ExceptionalMiddleware.HandleRequestAsync(HttpContext);

        [HttpGet("/.well-known/acme-challenge/{id}")]
        public IActionResult LetsEncrypt(string id, [FromServices] IHostingEnvironment env)
        {
            id = Path.GetFileName(id); // security cleaning
            var file = Path.Combine(env.ContentRootPath, ".well-known", "acme-challenge", id);
            return PhysicalFile(file, "text/plain");
        }
    }
}
