using ContentManagement.Services;
using ContentManagement.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ContentManagement.Common.GuardToolkit;
using System.Threading.Tasks;

namespace ContentManagement.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Policy = CustomRoles.User)]
    public partial class HomeController : Controller
    {
        private readonly IPageService _pageService;
        private readonly IContentService _contentService;
        private readonly IPortalService _portalService;
        private readonly ILinkService _linkService;

        public HomeController(IPageService pageService, IContentService contentService, IPortalService portalService, ILinkService linkService)
        {
            _pageService = pageService;
            _pageService.CheckArgumentIsNull(nameof(pageService));

            _contentService = contentService;
            _contentService.CheckArgumentIsNull(nameof(contentService));

            _portalService = portalService;
            _portalService.CheckArgumentIsNull(nameof(portalService));

            _linkService = linkService;
            _linkService.CheckArgumentIsNull(nameof(linkService));
        }

        public async virtual Task<IActionResult> Index()
        {
            ViewBag.Title = "مدیریت پرتال";

            ViewBag.PortalsCount = await _portalService.PortalsCountAsync();
            ViewBag.ContentsCount = await _contentService.ContentsCountAsync();
            ViewBag.PagesCount = await _pageService.PagesCountAsync();
            ViewBag.LinksCount = await _linkService.LinksCountAsync();

            return View();
        }
    }
}
