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
    public class LogoViewModel
    {
        [UploadFileExtensions(".png", ErrorMessage = "لطفاً یک تصویر با پسوند .png آپلود نمائید.")]
        public IFormFile HeaderLogo { get; set; }
        [UploadFileExtensions(".png", ErrorMessage = "لطفاً یک تصویر با پسوند .png آپلود نمائید.")]
        public IFormFile Logo { get; set; }
        [UploadFileExtensions(".png", ErrorMessage = "لطفاً یک تصویر با پسوند .png آپلود نمائید.")]
        public IFormFile FooterLogo { get; set; }
        [UploadFileExtensions(".png", ErrorMessage = "لطفاً یک تصویر با پسوند .png آپلود نمائید.")]
        public IFormFile Favicon { get; set; }
    }
}
