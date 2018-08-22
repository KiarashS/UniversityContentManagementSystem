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
        [Required(ErrorMessage = "نام پرتال را وارد نمایید.")]
        [Remote("checkportalkey", "portal", "manage", AdditionalFields ="InitialPortalKey", HttpMethod = "post", ErrorMessage ="پرتالی با این نام در سیستم موجود می باشد.")]
        [MaxLength(400, ErrorMessage = "حداکثر طول نام پرتال {1} کاراکتر می باشد.")]
        public string PortalKey { get; set; }
        [Required(ErrorMessage = "عنوان فارسی پرتال (هدر) را وارد نمایید.")]
        [MaxLength(200, ErrorMessage = "حداکثر طول عنوان پرتال {1} کاراکتر می باشد.")]
        public string TitleFa { get; set; }
        [Required(ErrorMessage = "عنوان فارسی پرتال (اچ تی ام ال) را وارد نمایید.")]
        public string HtmlTitleFa { get; set; }
        [Required(ErrorMessage = "توضیحات فارسی پرتال را وارد نمایید.")]
        [MaxLength(500, ErrorMessage = "حداکثر طول توضیحات پرتال {1} کاراکتر می باشد.")]
        public string DescriptionFa { get; set; }
        [Required(ErrorMessage = "عنوان انگلیسی پرتال (هدر) را وارد نمایید.")]
        [MaxLength(200, ErrorMessage = "حداکثر طول عنوان پرتال {1} کاراکتر می باشد.")]
        public string TitleEn { get; set; }
        [Required(ErrorMessage = "عنوان انگلیسی پرتال (اچ تی ام ال) را وارد نمایید.")]
        public string HtmlTitleEn { get; set; }
        [Required(ErrorMessage = "توضیحات انگلیسی پرتال را وارد نمایید.")]
        [MaxLength(500, ErrorMessage = "حداکثر طول توضیحات پرتال {1} کاراکتر می باشد.")]
        public string DescriptionEn { get; set; }
        public bool ShowInMainPortal { get; set; }

        public string PortalLink { get; set; }
    }
}
