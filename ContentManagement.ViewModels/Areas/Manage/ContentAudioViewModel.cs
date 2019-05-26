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
    public class ContentAudioViewModel
    {
        public long Id { get; set; }
        public long ContentId { get; set; }
        public string Caption { get; set; }
        [UploadFileExtensions(".mp3,.wav,.ogg", ErrorMessage = "لطفاً یک فایل صوتی با پسوندهای .mp3,.wav,.ogg آپلود نمائید.")]
        public IFormFile Audio { get; set; }
        public string Audioname { get; set; }
        public bool EnableControls { get; set; } = true;
        public bool EnableAutoplay { get; set; }
        public int? Priority { get; set; }
    }
}
