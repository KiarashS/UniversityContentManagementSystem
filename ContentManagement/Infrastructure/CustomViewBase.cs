﻿using ContentManagement.Services.Seo;
using ContentManagement.ViewModels.Settings;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.Extensions.Options;
using ContentManagement.Services;
using System.Threading.Tasks;

namespace ContentManagement.Infrastructure
{
    public class CustomViewBase<TModel> : RazorPage<TModel>
    {
        [RazorInject]

        [RazorInject]
        public SeoService SeoService { get; set; }

        [RazorInject]
        public IOptionsSnapshot<SiteSettings> SiteSettings { get; set; }

        [RazorInject]

        //[RazorInject]
        //public IHtmlLocalizerFactory MyHtmlLocalizerFactory { get; set; }

        //public IHtmlLocalizer MySharedLocalizer => MyHtmlLocalizerFactory.Create(
        //            baseName: "SharedResource" /*مشخصات*/,
        //            location: "Core1RtmTestResources.ExternalResources" /*نام اسمبلی ثالث*/);

        //public RequestLanguage CurrentLanguage()


#pragma warning disable 1998
    }
}