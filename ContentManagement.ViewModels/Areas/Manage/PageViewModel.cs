using ContentManagement.Common.WebToolkit;
using ContentManagement.Entities;
using DNTPersianUtils.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ContentManagement.ViewModels.Areas.Manage
{
    public class PageViewModel
    {
        public long Id { get; set; }
        [Required(ErrorMessage = "لطفاً پرتال را انتخاب نمایید.")]
        public int PortalId { get; set; }
        [Required(ErrorMessage = "لطفاً زبان را انتخاب نمایید.")]
        public ContentManagement.Entities.Language Language { get; set; } = ContentManagement.Entities.Language.FA;
        [Required(ErrorMessage = "عنوان صفحه را وارد نمایید.")]
        public string Title { get; set; }
        [Required(ErrorMessage = "متن صفحه را وارد نمایید.")]
        public string Text { get; set; }
        public DateTimeOffset PublishDate { get; set; } = DateTimeOffset.UtcNow;
        [Required(ErrorMessage = "شناسه یکتای صفحه را وارد نمایید.")]
        [Remote("checkslug", "page", "manage", AdditionalFields = "InitialSlug", HttpMethod = "post", ErrorMessage = "صفحه ای با این شناسه یکتا در سیستم موجود می باشد.")]
        [MaxLength(200, ErrorMessage = "حداکثر طول شناسه یکتا {1} کاراکتر می باشد.")]
        public string Slug { get; set; }
        [UploadFileExtensions(".png,.jpg,.jpeg,.gif", ErrorMessage = "لطفاً یک تصویر با پسوندهای .png,.jpg,.jpeg,.gif آپلود نمائید.")]
        public IFormFile Image { get; set; }
        public string Imagename { get; set; }
        public bool EnableImage { get; set; }
        public int ViewCount { get; set; }
        public bool IsActive { get; set; } = true;
        public string PortalKey { get; set; }
        public string RawText { get; set; }
        public string PageLink { get; set; }

        public string JPublishDate
        {
            get
            {
                return PublishDate.ToLongPersianDateTimeString();
            }
        }
    }
}
