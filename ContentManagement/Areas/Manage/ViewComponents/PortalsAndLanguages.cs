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
using ContentManagement.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Collections.Generic;

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
            _siteSettings.CheckArgumentIsNull(nameof(siteSettings));
        }

        public async Task<IViewComponentResult> InvokeAsync(int? portalId, Entities.Language language = Entities.Language.FA)
        {
            portalId = portalId ?? _siteSettings.Value.MainPortal.MainPortalId;
            var vm = new PortalsAndLanguagesViewModel();
            var portals = await _portalService.GetAllPortalsAsync();
            var newPortals = new List<PortalViewModel>();
            var identity = (ClaimsIdentity)User.Identity;
            IEnumerable<Claim> claims = identity.Claims;
            var roles = claims.Where(x => x.Type == ClaimTypes.Role).ToList();

            if (!roles.Any(x=> x.Value.ToLowerInvariant() == "admin"))
            {
                var portalKey = claims.SingleOrDefault(x => x.Type.ToLowerInvariant() == "portalkey").Value;
                foreach (var item in portals)
                {
                    if (portalKey.Equals(item.PortalKey, StringComparison.InvariantCultureIgnoreCase))
                    {
                        newPortals.Add(item);
                        //vm.PortalId = item.Id;
                    }
                }
            }

            if (newPortals.Any())
            {
                portals = newPortals;
            }
            //else
            //{
            //    vm.Portals.Add(new SelectListItem { Text = "همه پرتال ها", Selected = true });
            //}

            for (int i = 0; i < portals.Count; i++)
            {
                var text = portals[i].TitleFa;
                if(string.IsNullOrEmpty(portals[i].PortalKey))
                {
                    text = $"{text}-(اصلی)";
                }

                if (portalId.HasValue && portalId > 0)
                {
                    if (portalId.Value == portals[i].Id)
                    {
                        vm.PortalId = portalId.Value;
                    }
                    vm.Portals.Add(new SelectListItem { Text = text, Value = portals[i].Id.ToString(), Selected = (portalId.Value == portals[i].Id) });
                }
                else
                {
                    if (string.IsNullOrEmpty(portals[i].PortalKey))
                    {
                        vm.PortalId = portals[i].Id;
                    }
                    vm.Portals.Add(new SelectListItem { Text = text, Value = portals[i].Id.ToString(), Selected = string.IsNullOrEmpty(portals[i].PortalKey) });
                }
            }

            //if (newPortals.Any())
            //{
            //    var newPortalItems= new List<SelectListItem>();
            //    foreach (var item in vm.Portals)
            //    {
            //        newPortalItems.Add(new SelectListItem { Text = item.Text, Value = item.Value, Selected = true });
            //    }

            //    vm.Portals = newPortalItems;
            //}

            var languagesValues = Enum.GetValues(typeof(ContentManagement.Entities.Language)).Cast<ContentManagement.Entities.Language>();
            foreach (var item in languagesValues)
            {
                var text = item.GetAttributeOfType<LanguageTextInAdminAttribute>().Description;
                var defaultLanguage = _siteSettings.Value.Localization.DefaultLanguage;

                if (defaultLanguage != language)
                {
                    if (language == item)
                    {
                        vm.Language = (int)language;
                    }
                    vm.Languages.Add(new SelectListItem { Text = text, Value = ((int)item).ToString(), Selected = language == item});
                }
                else
                {
                    if (defaultLanguage == item)
                    {
                        vm.Language = (int)defaultLanguage;
                    }
                    vm.Languages.Add(new SelectListItem { Text = text, Value = ((int)item).ToString(), Selected = defaultLanguage == item });
                }
            }

            return View(vm);
        }
    }
}
