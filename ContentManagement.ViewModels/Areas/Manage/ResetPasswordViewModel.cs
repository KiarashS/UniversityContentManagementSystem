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
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "لطفاً کلمه عبور را وارد نمایید.")]
        [MinLength(8, ErrorMessage = "طول کلمه عبور حداقل {1} کاراکتر می باشد.")]
        [Remote("issafepassword", "users", "manage", HttpMethod = "post", ErrorMessage = "کلمه عبور وارد شده امن نمی باشد! کلمه عبور دیگری وارد نمائید.")]
        public string Password { get; set; }
    }
}
