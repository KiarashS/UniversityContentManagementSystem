using ContentManagement.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ContentManagement.ViewModels.Areas.Manage
{
    public class NavbarViewModel
    {
        public long Id { get; set; }
        public long? ParentId { get; set; }
        [Required(ErrorMessage = "لطفاً پرتال را انتخاب نمایید.")]
        public int PortalId { get; set; }
        [Required(ErrorMessage = "لطفاً زبان را انتخاب نمایید.")]
        public Language Language { get; set; } = Language.FA;
        [Required(ErrorMessage = "نام منو را وارد نمایید.")]
        public string Text { get; set; }
        public string Url { get; set; }
        public string Icon { get; set; }
        public bool IsBlankUrlTarget { get; set; }
        public int? Priority { get; set; }
    }
}
