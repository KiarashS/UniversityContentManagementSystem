using ContentManagement.Services;
using ContentManagement.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ContentManagement.Common.GuardToolkit;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;

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

            var identity = (ClaimsIdentity)User.Identity;
            IEnumerable<Claim> claims = identity.Claims;
            var roles = claims.Where(x => x.Type == ClaimTypes.Role).ToList();

            if (roles.Any(x => x.Value.ToLowerInvariant() == "admin"))
            {
                ViewBag.PortalsCount = await _portalService.PortalsCountAsync();
                ViewBag.ContentsCount = await _contentService.ContentsCountAsync();
                ViewBag.PagesCount = await _pageService.PagesCountAsync();
                ViewBag.LinksCount = await _linkService.LinksCountAsync();
            }
            else
            {
                var targetPortalKey = claims.FirstOrDefault(c => c.Type == "PortalKey");

                ViewBag.ContentsCount = await _contentService.ContentsCountAsync(targetPortalKey?.Value);
                ViewBag.PagesCount = await _pageService.PagesCountAsync(targetPortalKey?.Value);
                ViewBag.LinksCount = await _linkService.LinksCountAsync(targetPortalKey?.Value);
            }

            return View();
        }
    }
}
