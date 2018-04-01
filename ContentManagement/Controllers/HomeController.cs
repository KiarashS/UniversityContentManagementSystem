using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ContentManagement.Models;
using Microsoft.AspNetCore.Hosting;
using StackExchange.Exceptional;
using ContentManagement.Services.Contracts;
using Microsoft.Extensions.Options;
using ContentManagement.ViewModels.Settings;
using System.Threading;
using Microsoft.AspNetCore.Localization;
using ContentManagement.Infrastructure;
using ContentManagement.Common.WebToolkit;

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
            var userr = _userService.FindUserAsync(1).Result;
            var lang = _requestService.CurrentLanguage();
            var subDomain = _requestService.CurrentSubDomain();
            var isSubPortal = _requestService.IsSubPortal();

            return View();
        }

        [ResponseCache(Duration = 3600)]
        public virtual IActionResult About()
        {
            // Creates a model and passes it on to the view.
            Employee[] model =
            {
                new Employee { Name = "Alfred", Title = "Manager" },
                new Employee { Name = "Sarah", Title = "Accountant" }
            };

            return View(model);
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
        public async virtual Task Exceptions() => await ExceptionalMiddleware.HandleRequestAsync(HttpContext);
    }
}
