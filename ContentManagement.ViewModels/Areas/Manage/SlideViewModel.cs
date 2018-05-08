using ContentManagement.Common.WebToolkit;
using ContentManagement.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ContentManagement.ViewModels.Areas.Manage
{
    public class SlideViewModel
    {
        public long Id { get; set; }
        [Required(ErrorMessage = "لطفاً پرتال را انتخاب نمایید.")]
        public int PortalId { get; set; }
        [Required(ErrorMessage = "لطفاً زبان را انتخاب نمایید.")]
        public Language Language { get; set; } = Language.FA;
        [Required(ErrorMessage = "عنوان اسلاید را وارد نمایید.")]
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public DateTimeOffset PublishDate { get; set; }
        public string Url { get; set; }
        //[Required(ErrorMessage = "تصویر اسلاید را انتخاب نمایید.")]
        [UploadFileExtensions(".png,.jpg,.jpeg,.gif", ErrorMessage = "لطفاً یک تصویر با پسوندهای .png,.jpg,.jpeg,.gif آپلود نمائید.")]
        public IFormFile Image { get; set; }
        public string Filename { get; set; }
        public bool IsBlankUrlTarget { get; set; }
        public int? Priority { get; set; }
    }
}
