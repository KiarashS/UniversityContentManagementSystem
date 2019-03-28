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
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Transforms;
using System;
using ContentManagement.Common.WebToolkit;
using System.Threading.Tasks;
using Ganss.XSS;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using DNTPersianUtils.Core;

namespace ContentManagement.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Policy = CustomRoles.User)]
    public partial class VoteController : Controller
    {
        private readonly IVoteService _voteService;
        private readonly IOptionsSnapshot<SiteSettings> _siteSettings;
        private readonly IVoteItemService _voteItemService;

        public VoteController(IVoteService voteService, IOptionsSnapshot<SiteSettings> siteSettings, IVoteItemService voteItemService)
        {
            _voteService = voteService;
            _voteService.CheckArgumentIsNull(nameof(voteService));

            _voteItemService = voteItemService;
            _voteItemService.CheckArgumentIsNull(nameof(voteItemService));

            _siteSettings = siteSettings;
            _siteSettings.CheckArgumentIsNull(nameof(siteSettings));
        }

        public virtual IActionResult Index()
        {
            return View();
        }

        public virtual async Task<IActionResult> List(IDataTablesRequest request, int? portalId, Entities.Language language)
        {
            var votes = await _voteService.GetPagedVotesAsync(portalId, language, request.Search.Value, request.Start, request.Length).ConfigureAwait(false);
            var votesCount = await _voteService.VotesCountAsync();
            var votesPagedCount = await _voteService.VotesPagedCountAsync(portalId, language, request.Search.Value);
            var baseOfCurrentDomain = _siteSettings.Value.DomainName;

            foreach (var item in votes)
            {
                var pageHost = $"{item.PortalKey ?? "www"}.{baseOfCurrentDomain}";
                item.VoteLink = Url.RouteUrl("voteRoute", new { id = item.Id.ToString() }, Request.Scheme, pageHost);
            }

            var response = DataTablesResponse.Create(request, (int)votesCount, (int)votesPagedCount, votes);
            return new DataTablesJsonResult(response, true);
        }

        public virtual IActionResult Add()
        {
            return View(new VoteViewModel { IsActive = true, IsVisibleResults = true });
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public virtual async Task<IActionResult> Add(VoteViewModel vote)
        {
            if (!ModelState.IsValid)
            {
                return View(vote);
            }

            vote.ExpireDate = vote.ExpireDateText.ToGregorianDateTimeOffset()?.AddHours(-1).ToUniversalTime();
            await _voteService.AddOrUpdateVoteAsync(vote).ConfigureAwait(false);

            TempData["IsOk"] = true;
            return RedirectToAction("index", "vote", "manage");
        }

        public async virtual Task<IActionResult> Update(long id)
        {
            var vote = await _voteService.FindVoteAsync(id);

            if (vote == null)
            {
                return RedirectToAction("index", "vote", "manage");
            }

            var voteViewModel = new VoteViewModel
            {
                Id = vote.Id,
                Title = vote.Title,
                Description = vote.Description,
                PortalId = vote.PortalId,
                IsActive = vote.IsActive,
                IsMultiChoice = vote.IsMultiChoice,
                IsVisibleResults = vote.IsVisibleResults,
                Language = vote.Language,
                ExpireDate = vote.ExpireDate?.ToIranTimeZoneDateTimeOffset(),
                ExpireDateText = vote.ExpireDate != null ? vote.ExpireDate?.ToIranTimeZoneDateTimeOffset().ToShortPersianDateTimeString() : string.Empty
            };

            return View(voteViewModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public virtual async Task<IActionResult> Update(VoteViewModel vote)
        {
            if (!ModelState.IsValid)
            {
                return View(vote);
            }

            vote.ExpireDate = vote.ExpireDateText.ToGregorianDateTimeOffset()?.AddHours(-1).ToUniversalTime();
            await _voteService.AddOrUpdateVoteAsync(vote).ConfigureAwait(false);

            TempData["IsOk"] = true;
            return RedirectToAction("index", "vote", "manage");
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(long id)
        {
            await _voteItemService.DeleteItemsAsync(id);
            await _voteService.DeleteVoteAsync(id);

            return Content("نظرسنجی با موفقیت حذف شد.", "text/html", System.Text.Encoding.UTF8);
        }
    }
}