using ContentManagement.Common.GuardToolkit;
using ContentManagement.Services;
using ContentManagement.Services.Contracts;
using ContentManagement.ViewModels.Areas.Manage;
using ContentManagement.ViewModels.Settings;
using DataTables.AspNet.AspNetCore;
using DataTables.AspNet.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ContentManagement.Common.ReflectionToolkit;
using System;
using System.Threading.Tasks;
using Ganss.XSS;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using ContentManagement.Common.WebToolkit.Attributes;

namespace ContentManagement.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Policy = CustomRoles.Admin)]
    public partial class LogController : Controller
    {
        private readonly IActivityLogService _logService;
        private readonly IOptionsSnapshot<SiteSettings> _siteSettings;

        public LogController(IActivityLogService logService, IOptionsSnapshot<SiteSettings> siteSettings)
        {
            _logService = logService;
            _logService.CheckArgumentIsNull(nameof(logService));

            _siteSettings = siteSettings;
            _siteSettings.CheckArgumentIsNull(nameof(siteSettings));
        }

        public virtual IActionResult Index()
        {
            return View();
        }

        public virtual async Task<IActionResult> List(IDataTablesRequest request)
        {
            var logs = await _logService.GetActivityLogsAsync(request.Search.Value, request.Start, request.Length).ConfigureAwait(false);
            var logsCount = await _logService.TotalLogsCountAsync();
            var logsPagedCount = await _logService.TotalFilteredLogsCountAsync(request.Search.Value);

            var response = DataTablesResponse.Create(request, (int)logsCount, (int)logsPagedCount, logs);
            return new DataTablesJsonResult(response, true);
        }
    }
}