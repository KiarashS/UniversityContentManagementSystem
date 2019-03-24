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
    public class UserViewModel
    {
        public long Id { get; set; }
        public int? PortalId { get; set; }
        [Required(ErrorMessage = "لطفاً نام کاربری را وارد نمایید.")]
        [Remote("checkusername", "users", "manage", AdditionalFields = "InitialUsername", HttpMethod = "post", ErrorMessage = "کاربری با این نام کاربری در سیستم موجود می باشد.")]
        public string Username { get; set; }
        [Required(ErrorMessage = "لطفاً ایمیل را وارد نمایید.")]
        [Remote("checkemail", "users", "manage", AdditionalFields = "InitialEmail", HttpMethod = "post", ErrorMessage = "کاربری با این ایمیل در سیستم موجود می باشد.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "لطفاً نام را وارد نمایید.")]
        public string DisplayName { get; set; }
        public DateTimeOffset? LastLogIn { get; set; }
        public string LastIp { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }
        [Required(ErrorMessage = "لطفاً کلمه عبور را وارد نمایید.")]
        [MinLength(8, ErrorMessage = "طول کلمه عبور حداقل {1} کاراکتر می باشد.")]
        public string Password { get; set; }


        public string LastLogInDate
        {
            get
            {
                if (!LastLogIn.HasValue)
                {
                    return string.Empty;
                }

                return LastLogIn?.ToLongPersianDateTimeString();
            }
        }
    }
}
