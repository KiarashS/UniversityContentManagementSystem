using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContentManagement.Common.GuardToolkit;
using ContentManagement.Services;
using ContentManagement.Services.Contracts;
using ContentManagement.Services.Seo;
using ContentManagement.ViewModels.Settings;
using DNTBreadCrumb.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace ContentManagement.Controllers
{
    public class VoteController : Controller
    {
        private readonly IVoteService _voteService;
        private readonly IVoteItemService _voteItemService;
        private readonly IVoteResultService _voteResultService;
        private readonly IOptionsSnapshot<SiteSettings> _siteSettings;
        private readonly IRequestService _requestService;
        private readonly IStringLocalizer<SharedResource> _sharedLocalizer;
        private readonly SeoService _seoService;
        private readonly IHostingEnvironment _hostingEnvironment;

        public VoteController(IVoteService voteService, IVoteItemService voteItemService, IVoteResultService voteResultService, IOptionsSnapshot<SiteSettings> siteSettings, IRequestService requestService, IStringLocalizer<SharedResource> sharedLocalizer, SeoService seoService, IHostingEnvironment hostingEnvironment)
        {
            _voteService = voteService;
            _voteService.CheckArgumentIsNull(nameof(voteService));

            _voteItemService = voteItemService;
            _voteItemService.CheckArgumentIsNull(nameof(voteItemService));

            _voteResultService = voteResultService;
            _voteResultService.CheckArgumentIsNull(nameof(voteResultService));

            _voteService = voteService;
            _voteService.CheckArgumentIsNull(nameof(voteService));

            _siteSettings = siteSettings;
            _siteSettings.CheckArgumentIsNull(nameof(siteSettings));

            _requestService = requestService;
            _requestService.CheckArgumentIsNull(nameof(requestService));

            _sharedLocalizer = sharedLocalizer;
            _sharedLocalizer.CheckArgumentIsNull(nameof(sharedLocalizer));

            _seoService = seoService;
            _seoService.CheckArgumentIsNull(nameof(seoService));

            _hostingEnvironment = hostingEnvironment;
            _hostingEnvironment.CheckArgumentIsNull(nameof(hostingEnvironment));
        }


        public async Task<IActionResult> Index(long id)
        {
            if (id <= 0)
            {
                return RedirectToAction("index", "error", new { id = 404 });
            }

            var portalKey = _requestService.PortalKey();
            var language = _requestService.CurrentLanguage().Language;
            var vote = await _voteService.GetVoteDetails(portalKey, language, id);
            if (vote == null)
            {
                return RedirectToAction("index", "error", new { id = 404 });
            }

            vote.VoteItems = await _voteItemService.GetVoteItemsAsync(id);
            vote.VoteResults = await _voteItemService.GetVoteResultsAsync(id);
            vote.TotalVoteCount = vote.VoteResults.Sum(x => x.VoteCount);
            if (vote.VoteItems.Count == 0)
            {
                return RedirectToAction("index", "error", new { id = 404 });
            }

            _seoService.Title = vote.Title;
            if (vote.HasDescription)
            {
                _seoService.MetaDescription = vote.GetDescription;
            }

            return View(vote);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApplyVote(long vid, long[] voteItem)
        {
            var vote = await _voteService.FindVoteAsync(vid).ConfigureAwait(false);
            if (vote == null || !vote.IsActive || (vote.ExpireDate != null && vote.ExpireDate < DateTimeOffset.UtcNow))
            {
                throw new InvalidOperationException("User cannot participate in vote because: 1.Not exist vote 2.Disabled vote 3.Expired vote");
            }

            await _voteService.AddVoteResults(vid, voteItem).ConfigureAwait(false);

            return Json(new { IsOk = "true" });
        }
    }
}
