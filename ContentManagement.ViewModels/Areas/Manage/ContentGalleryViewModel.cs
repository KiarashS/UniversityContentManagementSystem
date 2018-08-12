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
    public class ContentGalleryViewModel
    {
        public long Id { get; set; }
        public long ContentId { get; set; }
        public string Caption { get; set; }
        //[Required(ErrorMessage = "تصویر اسلاید را انتخاب نمایید.")]
        [UploadFileExtensions(".png,.jpg,.jpeg,.gif", ErrorMessage = "لطفاً یک تصویر با پسوندهای .png,.jpg,.jpeg,.gif آپلود نمائید.")]
        public IFormFile Image { get; set; }
        public string Imagename { get; set; }
        public int? Priority { get; set; }
    }
}
