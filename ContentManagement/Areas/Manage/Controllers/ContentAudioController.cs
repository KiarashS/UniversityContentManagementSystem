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
    public partial class ContentAudioController : Controller
    {
        private readonly IContentAudioService _contentAudioService;
        private readonly IContentService _contentService;
        private readonly IOptionsSnapshot<SiteSettings> _siteSettings;
        private readonly IHostingEnvironment _env;

        public ContentAudioController(IContentAudioService contentAudioService, IContentService contentService, IOptionsSnapshot<SiteSettings> siteSettings, IHostingEnvironment env)
        {
            _contentAudioService = contentAudioService;
            _contentAudioService.CheckArgumentIsNull(nameof(contentAudioService));

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
            var positionValues = Enum.GetValues(typeof(ContentAudioPosition)).Cast<ContentAudioPosition>();
            var selectedPosition = await _contentService.GetAudiosPosition(cid);

            foreach (var item in positionValues)
            {
                var text = item.GetAttributeOfType<ContentGalleryPossitionTitleInAdminAttribute>().Description;
                positionsList.Add(new SelectListItem { Text = text, Value = ((int)item).ToString(), Selected = (selectedPosition == item) });
            }

            ViewBag.AudioPositions = positionsList;

            return View();
        }

        public virtual async Task<IActionResult> List(IDataTablesRequest request, long cid)
        {
            var audios = await _contentAudioService.GetPagedAudiosAsync(cid).ConfigureAwait(false);
            var audiosCount = await _contentAudioService.ContentAudiosCountAsync(cid);
            var audiosPagedCount = audiosCount;

            var response = DataTablesResponse.Create(request, (int)audiosCount, (int)audiosPagedCount, audios);
            return new DataTablesJsonResult(response, true);
        }

        public virtual IActionResult Add(long cid)
        {
            return View(new ContentAudioViewModel());
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public virtual async Task<IActionResult> Add(ContentAudioViewModel audioViewModel, long cid)
        {
            if (!ModelState.IsValid)
            {
                return View(audioViewModel);
            }
            else if (audioViewModel.Audio == null || audioViewModel.Audio.Length <= 0)
            {
                ModelState.AddModelError("", "لطفاً یک صوت معتبر انتخاب نمائید.");
                return View(audioViewModel);
            }

            var webRoot = _env.WebRootPath;
            audioViewModel.Audioname = System.IO.Path.GetFileName(audioViewModel.Audio.FileName);
            var file = System.IO.Path.Combine(webRoot, Infrastructure.Constants.ContentAudiosRootPath, audioViewModel.Audioname);

            if (System.IO.File.Exists(file))
            {
                audioViewModel.Audioname = $"{System.IO.Path.GetFileNameWithoutExtension(file)}{DateTime.Now.Ticks}{System.IO.Path.GetExtension(file)}";
                file = System.IO.Path.Combine(webRoot, Infrastructure.Constants.ContentAudiosRootPath, audioViewModel.Audioname);
            }

            if (audioViewModel.Audio.Length > 0)
            {
                using (var stream = new FileStream(file, FileMode.Create))
                {
                    await _contentAudioService.AddOrUpdateAudioAsync(cid, audioViewModel).ConfigureAwait(false);
                    await audioViewModel.Audio.CopyToAsync(stream);
                }
            }

            TempData["IsOk"] = true;
            return RedirectToAction("index", "contentaudio", new { area = "manage", cid });
        }

        public async virtual Task<IActionResult> Update(long cid, long id)
        {
            var audio = await _contentAudioService.FindAudioByIdAsync(cid, id);

            if (audio == null)
            {
                return RedirectToAction("index", "contentaudio", new { area = "manage", cid });
            }

            var audioViewModel = new ContentAudioViewModel
            {
                Id = audio.Id,
                Audioname = audio.Audioname,
                EnableControls = audio.EnableControls,
                EnableAutoplay = audio.EnableAutoplay,
                Priority = audio.Priority,
                Caption = audio.Caption
            };

            return View(audioViewModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public virtual async Task<IActionResult> Update(long cid, long id, ContentAudioViewModel audioViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(audioViewModel);
            }

            audioViewModel.Id = id;
            var currentAudioname = await _contentAudioService.GetAudionameAsync(cid, audioViewModel.Id);
            if (audioViewModel.Audio != null)
            {
                if (audioViewModel.Audio == null || audioViewModel.Audio.Length <= 0)
                {
                    ModelState.AddModelError("", "لطفاً یک صوت معتبر انتخاب نمائید.");
                    return View(audioViewModel);
                }

                var webRoot = _env.WebRootPath;
                audioViewModel.Audioname = System.IO.Path.GetFileName(audioViewModel.Audio.FileName);
                var file = System.IO.Path.Combine(webRoot, Infrastructure.Constants.ContentAudiosRootPath, audioViewModel.Audioname);

                if (System.IO.File.Exists(file))
                {
                    audioViewModel.Audioname = $"{System.IO.Path.GetFileNameWithoutExtension(file)}{DateTime.Now.Ticks}{System.IO.Path.GetExtension(file)}";
                    file = System.IO.Path.Combine(webRoot, Infrastructure.Constants.ContentAudiosRootPath, audioViewModel.Audioname);
                }
                else if (string.Equals(currentAudioname, audioViewModel.Audioname, StringComparison.InvariantCultureIgnoreCase))
                {
                    audioViewModel.Audioname = $"{System.IO.Path.GetFileNameWithoutExtension(file)}{DateTime.Now.Ticks}{System.IO.Path.GetExtension(file)}";
                    file = System.IO.Path.Combine(webRoot, Infrastructure.Constants.ContentAudiosRootPath, audioViewModel.Audioname);
                }

                if (audioViewModel.Audio.Length > 0)
                {
                    using (var stream = new FileStream(file, FileMode.Create))
                    {
                        await _contentAudioService.AddOrUpdateAudioAsync(cid, audioViewModel).ConfigureAwait(false);
                        await audioViewModel.Audio.CopyToAsync(stream);
                    }
                }

                var previousFile = System.IO.Path.Combine(webRoot, Infrastructure.Constants.ContentAudiosRootPath, currentAudioname);
                if (System.IO.File.Exists(previousFile))
                {
                    System.IO.File.Delete(previousFile);
                }
            }
            else
            {
                audioViewModel.Audioname = currentAudioname;
                await _contentAudioService.AddOrUpdateAudioAsync(cid, audioViewModel).ConfigureAwait(false);
            }

            TempData["IsOk"] = true;
            return RedirectToAction("index", "contentaudio", new { area = "manage", cid });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(long cid, long id)
        {
            var audioname = await _contentAudioService.GetAudionameAsync(cid, id);
            var webRoot = _env.WebRootPath;
            var file = System.IO.Path.Combine(webRoot, Infrastructure.Constants.ContentAudiosRootPath, audioname);

            if (System.IO.File.Exists(file))
            {
                System.IO.File.Delete(file);
            }

            await _contentAudioService.DeleteAudioAsync(cid, id);

            return Content("صوت با موفقیت حذف شد.", "text/html", System.Text.Encoding.UTF8);
        }

        [HttpPost]
        public virtual async Task<IActionResult> UpdatePosition(long cid, ContentAudioPosition position)
        {
            await _contentService.UpdateAudioPosition(cid, position);
            if (position == ContentAudioPosition.None)
            {
                return Content("صوت با موفقیت غیر فعال شد.", "text/html", System.Text.Encoding.UTF8);
            }
            else
            {
                return Content("موقعیت صوت با موفقیت بروز شد.", "text/html", System.Text.Encoding.UTF8);
            }
        }
    }
}