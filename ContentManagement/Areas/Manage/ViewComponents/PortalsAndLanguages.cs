using ContentManagement.Services.Contracts;
using ContentManagement.ViewModels.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using ContentManagement.Common.GuardToolkit;
using System.Threading.Tasks;
using ContentManagement.ViewModels.Areas.Manage;
using System.Linq;
using ContentManagement.Common.ReflectionToolkit;
using ContentManagement.Common.WebToolkit.Attributes;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ContentManagement.Areas.Manage.ViewComponents
{
    public class PortalsAndLanguages : ViewComponent
    {
        private readonly IPortalService _portalService;
        private readonly IOptionsSnapshot<SiteSettings> _siteSettings;

        public PortalsAndLanguages(IPortalService portalService, IOptionsSnapshot<SiteSettings> siteSettings)
        {
            _portalService = portalService;
            _portalService.CheckArgumentIsNull(nameof(portalService));

            _siteSettings = siteSettings;
            _portalService.CheckArgumentIsNull(nameof(siteSettings));
        }

        public async Task<IViewComponentResult> InvokeAsync(int? portalId, Entities.Language language = Entities.Language.FA)
        {
            portalId = portalId ?? _siteSettings.Value.MainPortal.MainPortalId;
            var vm = new PortalsAndLanguagesViewModel();
            
            var portals = await _portalService.GetAllPortalsAsync();
            for (int i = 0; i < portals.Count; i++)
            {
                var text = portals[i].TitleFa;
                if(string.IsNullOrEmpty(portals[i].PortalKey))
                {
                    text = $"{text}-(اصلی)";
                }

                if (portalId.HasValue && portalId > 0)
                {
                    vm.Portals.Add(new SelectListItem { Text = text, Value = portals[i].Id.ToString(), Selected = (portals[i].Id == portalId) });
                }
                else
                {
                    vm.Portals.Add(new SelectListItem { Text = text, Value = portals[i].Id.ToString(), Selected = string.IsNullOrEmpty(portals[i].PortalKey) });
                }
            }

            var languagesValues = Enum.GetValues(typeof(ContentManagement.Entities.Language)).Cast<ContentManagement.Entities.Language>();
            foreach (var item in languagesValues)
            {
                var text = item.GetAttributeOfType<LanguageTextInAdminAttribute>().Description;
                var defaultLanguage = _siteSettings.Value.Localization.DefaultLanguage;

                if (defaultLanguage != language)
                {
                    vm.Languages.Add(new SelectListItem { Text = text, Value = ((int)item).ToString(), Selected = item == language });
                }
                else
                {
                    vm.Languages.Add(new SelectListItem { Text = text, Value = ((int)item).ToString(), Selected = item == defaultLanguage });
                }
            }

            return View(vm);
        }
    }
}
