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

namespace ContentManagement.ViewModels.Areas.Manage
{
    public class FooterSectionViewModel
    {
        //public LinkViewModel()
        //{
        //    LinkTypes = new List<SelectListItem>();
        //}

        public long Id { get; set; }
        [Required(ErrorMessage = "لطفاً پرتال را انتخاب نمایید.")]
        public int PortalId { get; set; }
        [Required(ErrorMessage = "لطفاً زبان را انتخاب نمایید.")]
        public ContentManagement.Entities.Language Language { get; set; } = ContentManagement.Entities.Language.FA;
        [Required(ErrorMessage = "عنوان بخش را وارد نمایید.")]
        public string Title { get; set; }
        public string Url { get; set; }
        public bool IsEnable { get; set; }
        public int? Priority { get; set; }
    }
}
