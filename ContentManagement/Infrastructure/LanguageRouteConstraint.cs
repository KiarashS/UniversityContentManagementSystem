using ContentManagement.ViewModels.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using System.Linq;

namespace ContentManagement.Infrastructure
{
    public class LanguageRouteConstraint : IRouteConstraint
    {
        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (!values.ContainsKey("lang"))
            {
                return false;
            }

            var siteSettings = (IOptionsSnapshot<SiteSettings>)httpContext.RequestServices.GetService(typeof(IOptionsSnapshot<SiteSettings>));
            var lang = values["lang"].ToString().ToLowerInvariant();
            var supportedLanguages = siteSettings.Value.Localization.SupportedLanguages;

            return supportedLanguages.Any(l => l.Equals(lang, System.StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
