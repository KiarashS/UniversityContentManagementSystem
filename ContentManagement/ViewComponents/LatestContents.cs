using ContentManagement.Services.Contracts;
using ContentManagement.ViewModels.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ContentManagement.Common.GuardToolkit;
using System.Threading.Tasks;
using ContentManagement.Services;
using ContentManagement.ViewModels;
using System.Linq;

namespace ContentManagement.ViewComponents
{
    public class LatestContents : ViewComponent
    {
        private readonly IContentService _contentService;
        private readonly IRequestService _requestService;
        private readonly IOptionsSnapshot<SiteSettings> _siteSettings;

        public LatestContents(IContentService contentService, IRequestService requestService, IOptionsSnapshot<SiteSettings> siteSettings)
        {
            _contentService = contentService;
            _contentService.CheckArgumentIsNull(nameof(contentService));

            _requestService = requestService;
            _requestService.CheckArgumentIsNull(nameof(requestService));

            _siteSettings = siteSettings;
            _siteSettings.CheckArgumentIsNull(nameof(siteSettings));
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var currentLanguage = _requestService.CurrentLanguage().Language;
            int size;
            var vm = new LatestContentsTabsViewModel();

            vm.ContentsVisibilityViewModel = await _contentService.CheckContentsVisibility(_requestService.PortalKey(), currentLanguage);

            ViewData["IsOtherContents"] = false;
            if (vm.IsExistEducation) // just for tab initialize, in order
            {
                size = _siteSettings.Value.PagesSize.EducationsTabSize;
                vm.ContentsViewModel = await _contentService.GetContentsAsync(_requestService.PortalKey(), currentLanguage, Entities.ContentType.Education, 0, size).ConfigureAwait(false);
                ViewData["ContentsQuery"] = "?t=education";
            }
            else if (vm.IsExistForm)
            {
                size = _siteSettings.Value.PagesSize.FormsTabSize;
                vm.ContentsViewModel = await _contentService.GetContentsAsync(_requestService.PortalKey(), currentLanguage, Entities.ContentType.Form, 0, size).ConfigureAwait(false);
                ViewData["ContentsQuery"] = "?t=form";
            }
            else if (vm.IsExistEducationalCalendar)
            {
                size = _siteSettings.Value.PagesSize.EducationalCalendarsTabSize;
                vm.ContentsViewModel = await _contentService.GetContentsAsync(_requestService.PortalKey(), currentLanguage, Entities.ContentType.EducationalCalendar, 0, size).ConfigureAwait(false);
                ViewData["ContentsQuery"] = "?t=educationalcalendar";
            }
            else if (vm.IsExistStudentAndCultural)
            {
                size = _siteSettings.Value.PagesSize.StudentAndCulturalsTabSize;
                vm.ContentsViewModel = await _contentService.GetContentsAsync(_requestService.PortalKey(), currentLanguage, Entities.ContentType.StudentAndCultural, 0, size).ConfigureAwait(false);
                ViewData["ContentsQuery"] = "?t=studentandcultural";
            }
            else if (vm.IsExistPoliticalAndIdeological)
            {
                size = _siteSettings.Value.PagesSize.PoliticalAndIdeologicalsTabSize;
                vm.ContentsViewModel = await _contentService.GetContentsAsync(_requestService.PortalKey(), currentLanguage, Entities.ContentType.PoliticalAndIdeological, 0, size).ConfigureAwait(false);
                ViewData["ContentsQuery"] = "?t=politicalandideological";
            }
            else if (vm.IsExistOtherContents)
            {
                size = _siteSettings.Value.PagesSize.OtherContentsTabSize;
                vm.ContentsViewModel = await _contentService.GetOtherContentsAsync(_requestService.PortalKey(), null, currentLanguage, 0, size).ConfigureAwait(false);
                ViewData["ContentsQuery"] = "?othercontents=true";
                ViewData["IsOtherContents"] = true;
            }

            return View(vm);
        }
    }
}
