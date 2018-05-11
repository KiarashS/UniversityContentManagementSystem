using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ContentManagement.ViewModels.Areas.Manage
{
    public class PortalViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "نام پرتال را به وارد نمایید.")]
        [Remote("checkportalkey", "portal", "manage", AdditionalFields ="InitialPortalKey", HttpMethod = "post", ErrorMessage ="پرتالی با این نام در سیستم موجود می باشد.")]
        [MaxLength(400, ErrorMessage = "حداکثر طول نام پرتال {1} کاراکتر می باشد.")]
        public string PortalKey { get; set; }
        [Required(ErrorMessage = "عنوان فارسی پرتال را به وارد نمایید.")]
        [MaxLength(200, ErrorMessage = "حداکثر طول عنوان پرتال {1} کاراکتر می باشد.")]
        public string TitleFa { get; set; }
        [Required(ErrorMessage = "توضیحات فارسی پرتال را به وارد نمایید.")]
        [MaxLength(500, ErrorMessage = "حداکثر طول توضیحات پرتال {1} کاراکتر می باشد.")]
        public string DescriptionFa { get; set; }
        [Required(ErrorMessage = "عنوان انگلیسی پرتال را به وارد نمایید.")]
        [MaxLength(200, ErrorMessage = "حداکثر طول عنوان پرتال {1} کاراکتر می باشد.")]
        public string TitleEn { get; set; }
        [Required(ErrorMessage = "توضیحات انگلیسی پرتال را به وارد نمایید.")]
        [MaxLength(500, ErrorMessage = "حداکثر طول توضیحات پرتال {1} کاراکتر می باشد.")]
        public string DescriptionEn { get; set; }
        public bool ShowInMainPortal { get; set; }
    }
}
