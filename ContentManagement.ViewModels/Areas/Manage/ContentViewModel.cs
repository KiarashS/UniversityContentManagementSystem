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
using System.Text;

namespace ContentManagement.ViewModels.Areas.Manage
{
    public class ContentViewModel
    {
        public ContentViewModel(): this(null) {}

        public ContentViewModel(ContentType? contentType = null)
        {
            if (contentType.HasValue)
            {
                ContentType = contentType.Value;
            }
        }

        public long Id { get; set; }
        [Required(ErrorMessage = "لطفاً پرتال را انتخاب نمایید.")]
        public int PortalId { get; set; }
        [Required(ErrorMessage = "لطفاً زبان را انتخاب نمایید.")]
        public ContentManagement.Entities.Language Language { get; set; } = ContentManagement.Entities.Language.FA;
        [Required(ErrorMessage = "عنوان متن را وارد نمایید.")]
        public string Title { get; set; }
        [Required(ErrorMessage = "متن را وارد نمایید.")]
        public string Text { get; set; }
        public string RawText { get; set; }
        public string Summary { get; set; }
        [Required(ErrorMessage = "نوع متن را انتخاب نمایید.")]
        public ContentType ContentType { get; set; }
        public DateTimeOffset PublishDate { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? ArchiveDate { get; set; }
        [UploadFileExtensions(".png,.jpg,.jpeg,.gif", ErrorMessage = "لطفاً یک تصویر با پسوندهای .png,.jpg,.jpeg,.gif آپلود نمائید.")]
        public IFormFile Image { get; set; }
        public string Imagename { get; set; }
        [Required(ErrorMessage = "کلمات کلیدی را وارد نمایید.")]
        public string Keywords { get; set; }
        public bool EnableImage { get; set; }
        public int ViewCount { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsFavorite { get; set; } = false;
        public bool IsArchive { get; set; } = false;
        public int? Priority { get; set; }
        public string PortalKey { get; set; }
        public string ContentLink { get; set; }


        public string JPublishDate
        {
            get
            {
                return PublishDate.ToLongPersianDateTimeString();
            }
        }

        public IList<SelectListItem> ContentTypes
        {
            get
            {
                var contentTypesList = new List<SelectListItem>() { new SelectListItem { Text = "", Selected = (Id == 0) } };
                var contentValues = Enum.GetValues(typeof(ContentManagement.Entities.ContentType)).Cast<ContentManagement.Entities.ContentType>();

                foreach (var item in contentValues)
                {
                    var text = item.GetAttributeOfType<ContentTypeTextInAdminAttribute>().Description;
                    contentTypesList.Add(new SelectListItem { Text = text, Value = ((int)item).ToString(), Selected = (Id != 0 && item == ContentType) });
                }

                return contentTypesList;
            }
            set
            {
                ContentTypes = value;
            }
        }
    }
}
