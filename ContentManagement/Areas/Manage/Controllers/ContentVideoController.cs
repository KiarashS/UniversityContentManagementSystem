using ContentManagement.Common.GuardToolkit;
using ContentManagement.Common.WebToolkit.Attributes;
using ContentManagement.Services;
using ContentManagement.Services.Contracts;
using ContentManagement.ViewModels.Areas.Manage;
using ContentManagement.ViewModels.Settings;
using DataTables.AspNet.AspNetCore;
using DataTables.AspNet.Core;
using DNTCommon.Web.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Filters;
using SixLabors.ImageSharp.Processing.Transforms;
using System;
using System.Collections.Generic;
using ContentManagement.Common.ReflectionToolkit;
using System.Linq;
using System.Threading.Tasks;
using ContentManagement.Entities;
using ContentManagement.Common.WebToolkit;
using System.IO;

namespace ContentManagement.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Policy = CustomRoles.User)]
    public partial class ContentVideoController : Controller
    {
        private readonly IContentVideoService _contentVideoService;
        private readonly IContentService _contentService;
        private readonly IOptionsSnapshot<SiteSettings> _siteSettings;
        private readonly IHostingEnvironment _env;

        public ContentVideoController(IContentVideoService contentVideoService, IContentService contentService, IOptionsSnapshot<SiteSettings> siteSettings, IHostingEnvironment env)
        {
            _contentVideoService = contentVideoService;
            _contentVideoService.CheckArgumentIsNull(nameof(contentVideoService));

            _contentService = contentService;
            _contentService.CheckArgumentIsNull(nameof(contentService));

            _siteSettings = siteSettings;
            _siteSettings.CheckArgumentIsNull(nameof(siteSettings));

            _env = env;
            _env.CheckArgumentIsNull(nameof(env));
        }

        public async virtual Task<IActionResult> Index(long cid)
        {
            var positionsList = new List<SelectListItem>();
            var positionValues = Enum.GetValues(typeof(ContentVideoPosition)).Cast<ContentVideoPosition>();
            var selectedPosition = await _contentService.GetVideosPosition(cid);

            foreach (var item in positionValues)
            {
                var text = item.GetAttributeOfType<ContentGalleryPossitionTitleInAdminAttribute>().Description;
                positionsList.Add(new SelectListItem { Text = text, Value = ((int)item).ToString(), Selected = (selectedPosition == item) });
            }

            ViewBag.VideoPositions = positionsList;

            return View();
        }

        public virtual async Task<IActionResult> List(IDataTablesRequest request, long cid)
        {
            var videos = await _contentVideoService.GetPagedVideosAsync(cid).ConfigureAwait(false);
            var videosCount = await _contentVideoService.ContentVideosCountAsync(cid);
            var videosPagedCount = videosCount;

            var response = DataTablesResponse.Create(request, (int)videosCount, (int)videosPagedCount, videos);
            return new DataTablesJsonResult(response, true);
        }

        public virtual IActionResult Add(long cid)
        {
            return View(new ContentVideoViewModel());
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public virtual async Task<IActionResult> Add(ContentVideoViewModel videoViewModel, long cid)
        {
            if (!ModelState.IsValid)
            {
                return View(videoViewModel);
            }
            else if (videoViewModel.Video == null || videoViewModel.Video.Length <= 0)
            {
                ModelState.AddModelError("", "لطفاً یک ویدئو معتبر انتخاب نمائید.");
                return View(videoViewModel);
            }

            var webRoot = _env.WebRootPath;
            videoViewModel.Videoname = System.IO.Path.GetFileName(videoViewModel.Video.FileName);
            var file = System.IO.Path.Combine(webRoot, Infrastructure.Constants.ContentVideosRootPath, videoViewModel.Videoname);

            if (System.IO.File.Exists(file))
            {
                videoViewModel.Videoname = $"{System.IO.Path.GetFileNameWithoutExtension(file)}{DateTime.Now.Ticks}{System.IO.Path.GetExtension(file)}";
                file = System.IO.Path.Combine(webRoot, Infrastructure.Constants.ContentVideosRootPath, videoViewModel.Videoname);
            }

            if (videoViewModel.Video.Length > 0)
            {
                using (var stream = new FileStream(file, FileMode.Create))
                {
                    await _contentVideoService.AddOrUpdateVideoAsync(cid, videoViewModel).ConfigureAwait(false);
                    await videoViewModel.Video.CopyToAsync(stream);
                }
            }
            
            TempData["IsOk"] = true;
            return RedirectToAction("index", "contentvideo", new { area = "manage", cid });
        }

        public async virtual Task<IActionResult> Update(long cid, long id)
        {
            var video = await _contentVideoService.FindVideoByIdAsync(cid, id);

            if (video == null)
            {
                return RedirectToAction("index", "contentvideo", new { area = "manage", cid });
            }

            var videoViewModel = new ContentVideoViewModel
            {
                Id = video.Id,
                Videoname = video.Videoname,
                Width = video.Width,
                Height = video.Height,
                EnableControls = video.EnableControls,
                EnableAutoplay = video.EnableAutoplay,
                Priority = video.Priority,
                Caption = video.Caption
            };

            return View(videoViewModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public virtual async Task<IActionResult> Update(long cid, long id, ContentVideoViewModel videoViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(videoViewModel);
            }

            videoViewModel.Id = id;
            var currentVideoname = await _contentVideoService.GetVideonameAsync(cid, videoViewModel.Id);
            if (videoViewModel.Video != null)
            {
                if (videoViewModel.Video == null || videoViewModel.Video.Length <= 0)
                {
                    ModelState.AddModelError("", "لطفاً یک ویدئو معتبر انتخاب نمائید.");
                    return View(videoViewModel);
                }

                var webRoot = _env.WebRootPath;
                videoViewModel.Videoname = System.IO.Path.GetFileName(videoViewModel.Video.FileName);
                var file = System.IO.Path.Combine(webRoot, Infrastructure.Constants.ContentVideosRootPath, videoViewModel.Videoname);

                if (System.IO.File.Exists(file))
                {
                    videoViewModel.Videoname = $"{System.IO.Path.GetFileNameWithoutExtension(file)}{DateTime.Now.Ticks}{System.IO.Path.GetExtension(file)}";
                    file = System.IO.Path.Combine(webRoot, Infrastructure.Constants.ContentVideosRootPath, videoViewModel.Videoname);
                }
                else if (string.Equals(currentVideoname, videoViewModel.Videoname, StringComparison.InvariantCultureIgnoreCase))
                {
                    videoViewModel.Videoname = $"{System.IO.Path.GetFileNameWithoutExtension(file)}{DateTime.Now.Ticks}{System.IO.Path.GetExtension(file)}";
                    file = System.IO.Path.Combine(webRoot, Infrastructure.Constants.ContentVideosRootPath, videoViewModel.Videoname);
                }

                if (videoViewModel.Video.Length > 0)
                {
                    using (var stream = new FileStream(file, FileMode.Create))
                    {
                        await _contentVideoService.AddOrUpdateVideoAsync(cid, videoViewModel).ConfigureAwait(false);
                        await videoViewModel.Video.CopyToAsync(stream);
                    }
                }

                var previousFile = System.IO.Path.Combine(webRoot, Infrastructure.Constants.ContentVideosRootPath, currentVideoname);
                if (System.IO.File.Exists(previousFile))
                {
                    System.IO.File.Delete(previousFile);
                }
            }
            else
            {
                videoViewModel.Videoname = currentVideoname;
                await _contentVideoService.AddOrUpdateVideoAsync(cid, videoViewModel).ConfigureAwait(false);
            }

            TempData["IsOk"] = true;
            return RedirectToAction("index", "contentvideo", new { area = "manage", cid });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(long cid, long id)
        {
            var videoname = await _contentVideoService.GetVideonameAsync(cid, id);
            var webRoot = _env.WebRootPath;
            var file = System.IO.Path.Combine(webRoot, Infrastructure.Constants.ContentVideosRootPath, videoname);

            if (System.IO.File.Exists(file))
            {
                System.IO.File.Delete(file);
            }

            await _contentVideoService.DeleteVideoAsync(cid, id);

            return Content("ویدئو با موفقیت حذف شد.", "text/html", System.Text.Encoding.UTF8);
        }

        [HttpPost]
        public virtual async Task<IActionResult> UpdatePosition(long cid, ContentVideoPosition position)
        {
            await _contentService.UpdateVideoPosition(cid, position);
            if (position == ContentVideoPosition.None)
            {
                return Content("ویدئو با موفقیت غیر فعال شد.", "text/html", System.Text.Encoding.UTF8);
            }
            else
            {
                return Content("موقعیت ویدئو با موفقیت بروز شد.", "text/html", System.Text.Encoding.UTF8);
            }
        }
    }
}