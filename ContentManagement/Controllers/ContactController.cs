using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ContentManagement.Services.Contracts;
using Microsoft.Extensions.Options;
using ContentManagement.ViewModels.Settings;
using ContentManagement.Services;
using ContentManagement.Entities;
using ContentManagement.Common.GuardToolkit;
using System.Collections.Generic;
using Microsoft.Extensions.Localization;
using DNTCommon.Web.Core;
using Microsoft.AspNetCore.Hosting;
using ContentManagement.ViewModels;
using System;
using DNTPersianUtils.Core;
using ContentManagement.Services.Seo;

namespace ContentManagement.Controllers
{
    public partial class ContactController : Controller
    {
        private readonly IOptionsSnapshot<SiteSettings> _siteSettings;
        private readonly IRequestService _requestService;
        private readonly IPortalService _portalService;
        private readonly IWebMailService _webMailService;
        private readonly SeoService _seoService;
        private readonly IStringLocalizer<SharedResource> _sharedLocalizer;

        public ContactController(IOptionsSnapshot<SiteSettings> siteSettings,
            IRequestService requestService,
            IPortalService portalService,
            IUrlUtilityService urlUtilityService,
            IWebMailService webMailService,
            SeoService seoService,
            IStringLocalizer<SharedResource> sharedLocalizer)
        {
            _siteSettings = siteSettings;
            _siteSettings.CheckArgumentIsNull(nameof(siteSettings));

            _requestService = requestService;
            _requestService.CheckArgumentIsNull(nameof(requestService));

            _portalService = portalService;
            _portalService.CheckArgumentIsNull(nameof(portalService));

            _webMailService = webMailService;
            _webMailService.CheckArgumentIsNull(nameof(webMailService));

            _seoService = seoService;
            _seoService.CheckArgumentIsNull(nameof(seoService));

            _sharedLocalizer = sharedLocalizer;
            _sharedLocalizer.CheckArgumentIsNull(nameof(sharedLocalizer));
        }

        public virtual IActionResult Index()
        {
            _seoService.Title = _sharedLocalizer["Contact Us"];
            return View(new ContactViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async virtual Task<IActionResult> Index(ContactViewModel msg)
        {
            if (string.IsNullOrEmpty(msg.Name))
            {
                ModelState.AddModelError("Name",
                    _requestService.CurrentLanguage().Language == Entities.Language.FA ? "لطفاً نام خود را وارد نمائید." : "Please enter your name.");
            }
            if (string.IsNullOrEmpty(msg.Email))
            {
                ModelState.AddModelError("Email",
                    _requestService.CurrentLanguage().Language == Entities.Language.FA ? "لطفاً ایمیل خود را وارد نمائید." : "Please enter your email address.");
            }
            if (string.IsNullOrEmpty(msg.Message))
            {
                ModelState.AddModelError("Message",
                    _requestService.CurrentLanguage().Language == Entities.Language.FA ? "لطفاً پیام را وارد نمائید." : "Please enter the message.");
            }

            if(ModelState.ErrorCount > 0)
            {
                return View(msg);
            }

            var smtp = _siteSettings.Value.Smtp;
            var bccEmails = _siteSettings.Value.SuperAdminsEmail;
            var portalAdminEmail = await _portalService.GetAdminEmailAsync(_requestService.PortalKey());
            var portalName = string.IsNullOrEmpty(_requestService.PortalKey()) ? "پرتال اصلی" : _requestService.PortalKey();
            var smtpConfig = new DNTCommon.Web.Core.SmtpConfig {
                FromAddress = smtp.FromAddress,
                FromName = smtp.FromName,
                LocalDomain = smtp.LocalDomain,
                Password = smtp.Password,
                PickupFolder = smtp.PickupFolder,
                Port = smtp.Port,
                Server = smtp.Server,
                UsePickupFolder = smtp.UsePickupFolder,
                Username = smtp.Username
            };

            var blindCarbonCopies = new List<MailAddress>();
            foreach (var item in bccEmails)
            {
                blindCarbonCopies.Add(new MailAddress { ToAddress = item });
            }

            await _webMailService.SendEmailAsync(
                   smtpConfig: smtpConfig,
                   emails: new List<MailAddress>
                   {
                        new MailAddress { ToName = portalName, ToAddress = portalAdminEmail },
                   },
                   blindCarpbonCopies: blindCarbonCopies,
                   replyTos: new List<MailAddress>
                   {
                        new MailAddress { ToName = msg.Name, ToAddress = msg.Email },
                   },
                   subject: $"تماس با ما | دانشگاه علوم پزشکی آجا | Ajaums",
                   viewNameOrPath: "~/Views/EmailTemplates/_Contact.cshtml",
                   viewModel: new ContactEmailViewModel
                   {
                       Name = msg.Name,
                       Email = msg.Email,
                       Message = msg.Message,
                       Direction = _requestService.IsRtl() ? "rtl" : "ltr",
                       SenderIpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                       Portal = portalName,
                       PortalLanguage = _requestService.CurrentLanguage().Language == Entities.Language.FA ? "فارسی" : "انگلیسی",
                       SubmitDateTime = DateTimeOffset.UtcNow.ToIranTimeZoneDateTimeOffset().ToLongPersianDateTimeString()
                   }
               );

            ViewBag.IsSuccess = true;
            ViewBag.AlertMessage = _requestService.CurrentLanguage().Language == Entities.Language.FA ? "پیام شما با موفقیت ارسال شد." : "Your message has been successfully sent.";
            ModelState.Clear(); // clear form values
            return View(new ContactViewModel());
        }
    }
}
