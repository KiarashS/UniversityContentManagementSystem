using ContentManagement.Common.GuardToolkit;
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
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Filters;
using SixLabors.ImageSharp.Processing.Transforms;
using System;
using System.Drawing;
using System.Threading.Tasks;

namespace ContentManagement.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Policy = CustomRoles.Admin)]
    public partial class SlideController : Controller
    {
        private readonly ISlideService _slideService;
        private readonly IOptionsSnapshot<SiteSettings> _siteSettings;
        private readonly IHostingEnvironment _env;

        public SlideController(ISlideService slideService, IOptionsSnapshot<SiteSettings> siteSettings, IHostingEnvironment env)
        {
            _slideService = slideService;
            _slideService.CheckArgumentIsNull(nameof(slideService));

            _siteSettings = siteSettings;
            _siteSettings.CheckArgumentIsNull(nameof(siteSettings));

            _env = env;
            _env.CheckArgumentIsNull(nameof(env));
        }

        public virtual IActionResult Index()
        {
            return View();
        }

        public virtual async Task<IActionResult> List(IDataTablesRequest request, int portalId, Entities.Language language)
        {
            var slides = await _slideService.GetPagedSlidesAsync(portalId, language, request.Search.Value, request.Start, request.Length).ConfigureAwait(false);
            var slidesCount = await _slideService.SlideCountAsync();
            var slidesPagedCount = await _slideService.SlidePagedCountAsync(portalId, language, request.Search.Value);

            var response = DataTablesResponse.Create(request, (int)slidesCount, (int)slidesPagedCount, slides);
            return new DataTablesJsonResult(response, true);
        }

        public virtual IActionResult Add()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public virtual async Task<IActionResult> Add(SlideViewModel slide)
        {
            if (!ModelState.IsValid)
            {
                return View(slide);
            }
            else if (!slide.Image.IsImageFile())
            {
                ModelState.AddModelError("", "لطفاً یک تصویر معتبر انتخاب نمائید.");
                return View(slide);
            }

            var webRoot = _env.WebRootPath;
            using (Image<Rgba32> image = SixLabors.ImageSharp.Image.Load(slide.Image.OpenReadStream()))
            {
                image.Mutate(x => x
                        .Resize(Infrastructure.Constants.SlideWidthSize, Infrastructure.Constants.SlideHeightSize));

                slide.Filename = slide.Image.FileName;
                var file = System.IO.Path.Combine(webRoot, Infrastructure.Constants.SlidesRootPath, slide.Filename);

                if (System.IO.File.Exists(file))
                {
                    slide.Filename = $"{System.IO.Path.GetFileNameWithoutExtension(file)}{DateTime.Now.Ticks}{System.IO.Path.GetExtension(file)}";
                    file = System.IO.Path.Combine(webRoot, Infrastructure.Constants.SlidesRootPath, slide.Filename);
                }

                image.Save(file); // Automatic encoder selected based on extension.
                await _slideService.AddOrUpdateSlideAsync(slide).ConfigureAwait(false);
            }

            TempData["IsOk"] = true;
            return RedirectToAction("index", "slide", "manage");
        }

        public async virtual Task<IActionResult> Update(long id)
        {
            var slide = await _slideService.FindSlideByIdAsync(id);

            if (slide == null)
            {
                return RedirectToAction("index", "slide", "manage");
            }

            var slideViewModel = new SlideViewModel
            {
                Id = slide.Id,
                Title = slide.Title,
                SubTitle = slide.SubTitle,
                Url = slide.Url,
                IsBlankUrlTarget = slide.IsBlankUrlTarget,
                Priority = slide.Priority,
                PortalId = slide.PortalId,
                Language = slide.Language
            };

            return View(slideViewModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public virtual async Task<IActionResult> Update(SlideViewModel slide)
        {
            if (!ModelState.IsValid)
            {
                return View(slide);
            }

            var currentFilename = await _slideService.GetSlideFilenameAsync(slide.Id);
            if (slide.Image != null)
            {
                if (!slide.Image.IsImageFile())
                {
                    ModelState.AddModelError("", "لطفاً یک تصویر معتبر انتخاب نمائید.");
                    return View(slide);
                }

                var webRoot = _env.WebRootPath;
                using (Image<Rgba32> image = SixLabors.ImageSharp.Image.Load(slide.Image.OpenReadStream()))
                {
                    image.Mutate(x => x
                            .Resize(Infrastructure.Constants.SlideWidthSize, Infrastructure.Constants.SlideHeightSize));

                    slide.Filename = slide.Image.FileName;
                    var file = System.IO.Path.Combine(webRoot, Infrastructure.Constants.SlidesRootPath, slide.Filename);

                    if (System.IO.File.Exists(file))
                    {
                        slide.Filename = $"{System.IO.Path.GetFileNameWithoutExtension(file)}{DateTime.Now.Ticks}{System.IO.Path.GetExtension(file)}";
                        file = System.IO.Path.Combine(webRoot, Infrastructure.Constants.SlidesRootPath, slide.Filename);
                    }

                    image.Save(file); // Automatic encoder selected based on extension.
                    await _slideService.AddOrUpdateSlideAsync(slide).ConfigureAwait(false);

                    var previousFile = System.IO.Path.Combine(webRoot, Infrastructure.Constants.SlidesRootPath, currentFilename);
                    if (System.IO.File.Exists(previousFile))
                    {
                        System.IO.File.Delete(previousFile);
                    }
                }
            }
            else
            {
                slide.Filename = currentFilename;
                await _slideService.AddOrUpdateSlideAsync(slide).ConfigureAwait(false);
            }

            TempData["IsOk"] = true;
            return RedirectToAction("index", "slide", "manage");
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(long id)
        {
            var filename = await _slideService.GetSlideFilenameAsync(id);
            var webRoot = _env.WebRootPath;
            var file = System.IO.Path.Combine(webRoot, Infrastructure.Constants.SlidesRootPath, filename);

            if (System.IO.File.Exists(file))
            {
                System.IO.File.Delete(file);
            }

            await _slideService.DeleteSlideAsync(id);

            return Content("اسلاید با موفقیت حذف شد.", "text/html", System.Text.Encoding.UTF8);
        }
    }
}