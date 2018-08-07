using ContentManagement.Common.WebToolkit;
using ContentManagement.Entities;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ContentManagement.ViewModels.Areas.Manage
{
    public class ImageLinkViewModel
    {
        public long Id { get; set; }
        [Required(ErrorMessage = "لطفاً پرتال را انتخاب نمایید.")]
        public int PortalId { get; set; }
        [Required(ErrorMessage = "لطفاً زبان را انتخاب نمایید.")]
        public ContentManagement.Entities.Language Language { get; set; } = ContentManagement.Entities.Language.FA;
        [Required(ErrorMessage = "عنوان لینک تصویری را وارد نمایید.")]
        public string Title { get; set; }
        [Required(ErrorMessage = "توضیحات لینک تصویری را وارد نمایید.")]
        public string Description { get; set; }
        [Required(ErrorMessage = "آدرس لینک تصویری را وارد نمایید.")]
        public string Url { get; set; }
        [UploadFileExtensions(".png,.jpg,.jpeg,.gif", ErrorMessage = "لطفاً یک تصویر با پسوندهای .png,.jpg,.jpeg,.gif آپلود نمائید.")]
        public IFormFile Image { get; set; }
        public string Imagename { get; set; }
        public bool IsBlankUrlTarget { get; set; }
        public int? Priority { get; set; }
    }
}
