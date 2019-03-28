using ContentManagement.Common.GuardToolkit;
using ContentManagement.Services;
using ContentManagement.Services.Contracts;
using ContentManagement.ViewModels.Areas.Manage;
using ContentManagement.ViewModels.Settings;
using DataTables.AspNet.AspNetCore;
using DataTables.AspNet.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace ContentManagement.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Policy = CustomRoles.User)]
    public partial class VoteItemController : Controller
    {
        private readonly IVoteItemService _voteItemService;
        private readonly IOptionsSnapshot<SiteSettings> _siteSettings;

        public VoteItemController(IVoteItemService voteItemService, IOptionsSnapshot<SiteSettings> siteSettings)
        {
            _voteItemService = voteItemService;
            _voteItemService.CheckArgumentIsNull(nameof(voteItemService));

            _siteSettings = siteSettings;
            _siteSettings.CheckArgumentIsNull(nameof(siteSettings));
        }

        public virtual IActionResult Index(long vid)
        {
            return View();
        }

        public virtual async Task<IActionResult> List(IDataTablesRequest request, long vid)
        {
            var items = await _voteItemService.GetItemsAsync(vid).ConfigureAwait(false);
            var itemsCount = items.Count;
            var itemsPagedCount = itemsCount;

            var response = DataTablesResponse.Create(request, (int)itemsCount, (int)itemsPagedCount, items);
            return new DataTablesJsonResult(response, true);
        }

        public virtual IActionResult Add(long vid)
        {
            var voteItemViewModel = new VoteItemViewModel();
            return View(voteItemViewModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public virtual async Task<IActionResult> Add(long vid, VoteItemViewModel voteItem)
        {
            if (!ModelState.IsValid)
            {
                return View(voteItem);
            }

            await _voteItemService.AddOrUpdateItemAsync(vid, voteItem).ConfigureAwait(false);

            TempData["IsOk"] = true;
            return RedirectToAction("index", "voteitem", new { area = "manage", vid });
        }

        public async virtual Task<IActionResult> Update(long vid, long id)
        {
            var item = await _voteItemService.FindItemByIdAsync(vid, id);

            if (item == null)
            {
                return RedirectToAction("index", "voteitem", new { area = "manage", vid });
            }

            var voteItemViewModel = new VoteItemViewModel
            {
                Id = item.Id,
                ItemTitle = item.ItemTitle,
                Priority = item.Priority,
            };

            return View(voteItemViewModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public virtual async Task<IActionResult> Update(long vid, VoteItemViewModel voteItem)
        {
            if (!ModelState.IsValid)
            {
                return View(voteItem);
            }

            await _voteItemService.AddOrUpdateItemAsync(vid, voteItem).ConfigureAwait(false);

            TempData["IsOk"] = true;
            return RedirectToAction("index", "voteitem", new { area = "manage", vid });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(long vid, long id)
        {
            await _voteItemService.DeleteItemAsync(vid, id);

            return Content("گزینه با موفقیت حذف شد.", "text/html", System.Text.Encoding.UTF8);
        }
    }
}