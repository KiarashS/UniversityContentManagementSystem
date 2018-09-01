using ContentManagement.Services.Contracts;
using ContentManagement.ViewModels.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using ContentManagement.Common.GuardToolkit;
using System.Threading.Tasks;
using ContentManagement.Services;
using ContentManagement.ViewModels;
using DNTPersianUtils.Core;
using System.Collections.Generic;

namespace ContentManagement.ViewComponents
{
    public class HeaderLogo : ViewComponent
    {
        private readonly IPortalService _portalService;
        private readonly IRequestService _requestService;

        public HeaderLogo(IPortalService portalService, IRequestService requestService)
        {
            _portalService = portalService;
            _portalService.CheckArgumentIsNull(nameof(portalService));

            _requestService = requestService;
            _requestService.CheckArgumentIsNull(nameof(requestService));
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var currentLanguage = _requestService.CurrentLanguage().Language;
            var title = await _portalService.GetPortalTitleAsync(_requestService.PortalKey(), currentLanguage);
            var islamicDay = DateTimeOffset.UtcNow.ToIslamicDay();

            var vm = new HeaderLogoViewModel
            {
                PortalTitle = title,
                TodayDate = getTodayDate(currentLanguage),
                TodayIslamicDate = $"سال {islamicDay.Year.ToPersianNumbers()} قمری، {islamicDay.Day.ToPersianNumbers()} {getIslamicMonthName(islamicDay.Month)}"
            };

            return View(vm);
        }

        private string getTodayDate(Entities.Language language)
        {
            if (language == Entities.Language.EN)
            {
                return DateTimeOffset.UtcNow.ToString("dddd, d MMMM yyyy");
            }
            else
            {
                return DateTimeOffset.UtcNow.ToPersianDateTextify();
            }
        }

        private string getIslamicMonthName(int islamicMonthNumber)
        {
            var islamicMonthNames = new Dictionary<int, string>
            {
                { 1, "محرم" },
                { 2, "صفر" },
                { 3, "ربیع‌الاول" },
                { 4, "ربیع‌الثانی" },
                { 5, "جمادی‌الاول" },
                { 6, "جمادی‌الثانی" },
                { 7, "رجب" },
                { 8, "شعبان" },
                { 9, "رمضان" },
                { 10, "شوال" },
                { 11, "ذیقعده" },
                { 12, "ذیحجه" }
            };

            return islamicMonthNames[islamicMonthNumber];
        }
    }
}
