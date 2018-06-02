using ContentManagement.ViewModels.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ContentManagement.Services;

namespace ContentManagement.Controllers
{
    public abstract class BaseController : Controller
    {
        protected readonly IOptionsSnapshot<SiteSettings> _siteSettings;
        protected readonly IRequestService _requestService;

        protected BaseController()
        {
        }

        protected BaseController(IOptionsSnapshot<SiteSettings> siteSettings, IRequestService requestService)
        {
            _siteSettings = siteSettings;
            _requestService = requestService;
        }


        //private string CurrentLanguage
        //{
        //    get
        //    {
        //        var siteSettings = _siteSettings.Value;
        //        var _currentLanguage = siteSettings.Localization.DefaultLanguage.Humanize(LetterCasing.LowerCase);

        //        if (RouteData.Values.ContainsKey("culture"))
        //        {
        //            _currentLanguage = RouteData.Values["culture"].ToString().ToLowerInvariant();
        //            //if (_currentLanguage == "ee")
        //            //{
        //            //    _currentLanguage = "et";
        //            //}
        //            return _currentLanguage;
        //        }

        //        if (!string.IsNullOrEmpty(_currentLanguage))
        //        {
        //            return _currentLanguage;
        //        }

        //        if (string.IsNullOrEmpty(_currentLanguage))
        //        {
        //            var feature = HttpContext.Features.Get<IRequestCultureFeature>();
        //            _currentLanguage = feature.RequestCulture.UICulture.TwoLetterISOLanguageName.ToLowerInvariant();
        //        }

        //        return _currentLanguage;
        //    }
        //}
    }
}