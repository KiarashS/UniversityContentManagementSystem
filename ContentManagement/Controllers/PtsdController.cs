using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ContentManagement.Services.Contracts;
using Microsoft.Extensions.Options;
using ContentManagement.ViewModels.Settings;
using ContentManagement.Services;
using ContentManagement.Common.GuardToolkit;
using System.Collections.Generic;
using Microsoft.Extensions.Localization;
using DNTCommon.Web.Core;
using ContentManagement.ViewModels;
using System;
using ContentManagement.Services.Seo;

namespace ContentManagement.Controllers
{
    public partial class PtsdController : Controller
    {
        private readonly IOptionsSnapshot<SiteSettings> _siteSettings;
        private readonly IRequestService _requestService;
        private readonly IPortalService _portalService;
        private readonly IWebMailService _webMailService;
        private readonly SeoService _seoService;
        private readonly IStringLocalizer<SharedResource> _sharedLocalizer;

        public PtsdController(IOptionsSnapshot<SiteSettings> siteSettings,
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
            _seoService.Title = "PTSD International Workshop";
            return View(new PtsdViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async virtual Task<IActionResult> Index(PtsdViewModel vm)
        {
            if (ModelState.ErrorCount > 0)
            {
                return View(vm);
            }

            var smtp = _siteSettings.Value.Smtp;
            var adminEmail = _siteSettings.Value.PtsdEmail;
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

            vm.SenderIpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            vm.SubmitDateTime = DateTimeOffset.UtcNow;

            await _webMailService.SendEmailAsync(
                   smtpConfig: smtpConfig,
                   emails: new List<MailAddress>
                   {
                        new MailAddress { ToName = "PTSD", ToAddress = adminEmail },
                   },
                   replyTos: new List<MailAddress>
                   {
                        new MailAddress { ToName = vm.Firstname, ToAddress = vm.Email },
                   },
                   subject: $"PTSD Registration | دانشگاه علوم پزشکی آجا | Ajaums",
                   viewNameOrPath: "~/Views/EmailTemplates/_Ptsd.cshtml",
                   viewModel: vm
               );

            ViewBag.IsSuccess = true;
            ViewBag.AlertMessage = "Your information has been successfully submitted.";
            ModelState.Clear(); // clear form values
            return View(new PtsdViewModel());
        }

        public virtual IActionResult VisaApplication()
        {
            _seoService.Title = "PTSD International Workshop - Visa Application";
            return View(new VisaApplicationViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async virtual Task<IActionResult> VisaApplication(VisaApplicationViewModel vm)
        {
            if (ModelState.ErrorCount > 0)
            {
                return View(vm);
            }

            var smtp = _siteSettings.Value.Smtp;
            var adminEmail = _siteSettings.Value.PtsdEmail;
            var smtpConfig = new DNTCommon.Web.Core.SmtpConfig
            {
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

            vm.SenderIpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            vm.SubmitDateTime = DateTimeOffset.UtcNow;

            await _webMailService.SendEmailAsync(
                   smtpConfig: smtpConfig,
                   emails: new List<MailAddress>
                   {
                        new MailAddress { ToName = "PTSD - Visa Application", ToAddress = adminEmail },
                   },
                   replyTos: new List<MailAddress>
                   {
                        new MailAddress { ToName = vm.Name, ToAddress = vm.Email },
                   },
                   subject: $"PTSD Visa Application | دانشگاه علوم پزشکی آجا | Ajaums",
                   viewNameOrPath: "~/Views/EmailTemplates/_VisaApplication.cshtml",
                   viewModel: vm
               );

            ViewBag.IsSuccess = true;
            ViewBag.AlertMessage = "Your visa application has been successfully submitted.";
            ModelState.Clear(); // clear form values
            return View(new VisaApplicationViewModel());
        }
    }
}
