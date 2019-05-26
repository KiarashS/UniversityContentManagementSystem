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
    public class ContentVideoViewModel
    {
        public long Id { get; set; }
        public long ContentId { get; set; }
        public string Caption { get; set; }
        [UploadFileExtensions(".mp4,.webm,.ogg", ErrorMessage = "لطفاً یک ویدئو با پسوندهای .mp4,.webm,.ogg آپلود نمائید.")]
        public IFormFile Video { get; set; }
        public string Videoname { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public bool EnableControls { get; set; } = true;
        public bool EnableAutoplay { get; set; }
        public int? Priority { get; set; }
    }
}
