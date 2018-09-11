using ContentManagement.Common.GuardToolkit;
using ContentManagement.Services;
using ContentManagement.Services.Contracts;
using ContentManagement.ViewModels.Areas.Manage;
using ContentManagement.ViewModels.Settings;
using DataTables.AspNet.AspNetCore;
using DataTables.AspNet.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;


namespace ContentManagement.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Policy = CustomRoles.Admin)]
    public partial class FooterSectionController : Controller
    {
        private readonly IFooterSectionService _sectionService;
        private readonly IOptionsSnapshot<SiteSettings> _siteSettings;

        public FooterSectionController(IFooterSectionService sectionService, IOptionsSnapshot<SiteSettings> siteSettings)
        {
            _sectionService = sectionService;
            _sectionService.CheckArgumentIsNull(nameof(sectionService));

            _siteSettings = siteSettings;
            _siteSettings.CheckArgumentIsNull(nameof(siteSettings));
        }

        public virtual IActionResult Index()
        {
            return View();
        }

        public virtual async Task<IActionResult> List(IDataTablesRequest request, int portalId, Entities.Language language)
        {
            var sections = await _sectionService.GetPagedSectionsAsync(portalId, language, request.Search.Value, request.Start, request.Length).ConfigureAwait(false);
            var sectionsCount = await _sectionService.SectionsCountAsync();
            var sectionsPagedCount = await _sectionService.SectionsPagedCountAsync(portalId, language, request.Search.Value);

            var response = DataTablesResponse.Create(request, (int)sectionsCount, (int)sectionsPagedCount, sections);
            return new DataTablesJsonResult(response, true);
        }

        public virtual IActionResult Add()
        {
            var sectionViewModel = new FooterSectionViewModel();
            sectionViewModel.IsEnable = true;
            return View(sectionViewModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public virtual async Task<IActionResult> Add(FooterSectionViewModel section)
        {
            if (!ModelState.IsValid)
            {
                return View(section);
            }

            await _sectionService.AddOrUpdateSectionAsync(section).ConfigureAwait(false);

            TempData["IsOk"] = true;
            return RedirectToAction("index", "footersection", "manage");
        }

        public async virtual Task<IActionResult> Update(long id)
        {
            var section = await _sectionService.FindSectionByIdAsync(id);

            if (section == null)
            {
                return RedirectToAction("index", "footersection", "manage");
            }

            var sectionViewModel = new FooterSectionViewModel
            {
                Id = section.Id,
                Title = section.Title,
                Url = section.Url,
                IsEnable = section.IsEnable,
                Priority = section.Priority,
                PortalId = section.PortalId,
                Language = section.Language
            };

            return View(sectionViewModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public virtual async Task<IActionResult> Update(FooterSectionViewModel section)
        {
            if (!ModelState.IsValid)
            {
                return View(section);
            }

            await _sectionService.AddOrUpdateSectionAsync(section).ConfigureAwait(false);

            TempData["IsOk"] = true;
            return RedirectToAction("index", "footersection", "manage");
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(long id)
        {
            await _sectionService.DeleteSectionAsync(id);

            return Content("بخش مورد نظر در فوتر با موفقیت حذف شد.", "text/html", System.Text.Encoding.UTF8);
        }
    }
}