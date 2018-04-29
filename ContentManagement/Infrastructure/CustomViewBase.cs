using ContentManagement.Infrastructure.Seo;
using ContentManagement.ViewModels.Settings;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContentManagement.Infrastructure
{
    public class CustomViewBase<TModel> : RazorPage<TModel>
    {
        [RazorInject]        public IRequestService RequestService { get; set; }

        [RazorInject]
        public SeoService Seo { get; private set; }

        [RazorInject]
        public IOptionsSnapshot<SiteSettings> SiteSettings { get; set; }

        //[RazorInject]
        //public IHtmlLocalizerFactory MyHtmlLocalizerFactory { get; set; }

        //public IHtmlLocalizer MySharedLocalizer => MyHtmlLocalizerFactory.Create(
        //            baseName: "SharedResource" /*مشخصات*/,
        //            location: "Core1RtmTestResources.ExternalResources" /*نام اسمبلی ثالث*/);

        public RequestLanguage CurrentLanguage()        {            return RequestService.CurrentLanguage();        }


#pragma warning disable 1998        public override async Task ExecuteAsync()        {        }#pragma warning restore 1998
    }
}
