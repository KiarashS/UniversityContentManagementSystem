using ContentManagement.Services;
using ContentManagement.Services.Contracts;
using ContentManagement.Services.Seo;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using ContentManagement.Common.GuardToolkit;
using System.Threading.Tasks;
using DNTCommon.Web.Core;

namespace ContentManagement.Infrastructure
{
    public class SetSeoMetaValuesAttribute : ActionFilterAttribute
    {
        private readonly IPortalService _portalService;
        private readonly IRequestService _requestService;
        private readonly SeoService _seoService;

        public SetSeoMetaValuesAttribute(IPortalService portalService, IRequestService requestService, SeoService seoService)
        {
            _portalService = portalService;
            _portalService.CheckArgumentIsNull(nameof(portalService));

            _requestService = requestService;
            _requestService.CheckArgumentIsNull(nameof(requestService));

            _seoService = seoService;
            _seoService.CheckArgumentIsNull(nameof(seoService));
        }


        public async override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            context.CheckArgumentIsNull(nameof(context));

            if (context.HttpContext.Request.Method == "GET" && !context.HttpContext.Request.IsAjaxRequest())
            {
                var portalKey = _requestService.PortalKey();
                var language = _requestService.CurrentLanguage().Language;
                var info = await _portalService.GetPortalSeoInfo(portalKey, language);

                if (info != null)
                {
                    _seoService.BaseTitle = info.Title;
                    _seoService.MetaDescription = info.Description;
                }
            }

            await next();
        }
    }
}
