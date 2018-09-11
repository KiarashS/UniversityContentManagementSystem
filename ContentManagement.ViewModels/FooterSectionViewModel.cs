using ContentManagement.Common.ReflectionToolkit;
using ContentManagement.Common.WebToolkit;
using ContentManagement.Common.WebToolkit.Attributes;
using ContentManagement.Entities;
using DNTPersianUtils.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ContentManagement.ViewModels
{
    public class FooterSectionViewModel
    {
        public FooterSectionViewModel()
        {
            Links = new List<FooterLinkViewModel>();
        }

        public long Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public IList<FooterLinkViewModel> Links { get; set; }
    }
}
