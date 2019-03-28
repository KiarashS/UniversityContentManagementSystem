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
    public class VoteViewModel
    {
        public long Id { get; set; }
        [Required(ErrorMessage = "لطفاً پرتال را انتخاب نمایید.")]
        public int PortalId { get; set; }
        [Required(ErrorMessage = "لطفاً زبان را انتخاب نمایید.")]
        public ContentManagement.Entities.Language Language { get; set; } = ContentManagement.Entities.Language.FA;
        [Required(ErrorMessage = "لطفاً عنوان نظرسنجی را وارد نمایید.")]
        public string Title { get; set; }
        public string Description { get; set; }
        //[Required(ErrorMessage = "لطفاً حداقل یک گزینه برای نظرسنجی وارد نمایید.")]
        //public string Items { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsMultiChoice { get; set; }
        public bool IsVisibleResults { get; set; } = true;
        public DateTimeOffset PublishDate { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? ExpireDate { get; set; }
        public string PortalKey { get; set; }
        public string VoteLink { get; set; }
        public string ExpireDateText { get; set; }


        public string ExpireText
        {
            get
            {
                if (!ExpireDate.HasValue)
                {
                    return string.Empty;
                }

                return ExpireDate?.ToLongPersianDateTimeString();
            }
        }

        public string PublishDateText
        {
            get
            {
                return PublishDate.ToLongPersianDateTimeString();
            }
        }
    }
}
