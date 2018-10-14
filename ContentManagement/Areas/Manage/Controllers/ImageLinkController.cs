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
using System.Threading.Tasks;

namespace ContentManagement.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Policy = CustomRoles.User)]
    public partial class ImageLinkController : Controller
    {
        private readonly IImageLinkService _imageLinkService;
        private readonly IOptionsSnapshot<SiteSettings> _siteSettings;
        private readonly IHostingEnvironment _env;

        public ImageLinkController(IImageLinkService imageLinkService, IOptionsSnapshot<SiteSettings> siteSettings, IHostingEnvironment env)
        {
            _imageLinkService = imageLinkService;
            _imageLinkService.CheckArgumentIsNull(nameof(imageLinkService));

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
            var imageLinks = await _imageLinkService.GetPagedImageLinksAsync(portalId, language, request.Search.Value, request.Start, request.Length).ConfigureAwait(false);
            var imageLinksCount = await _imageLinkService.ImageLinksCountAsync();
            var imageLinksPagedCount = await _imageLinkService.ImageLinksPagedCountAsync(portalId, language, request.Search.Value);

            var response = DataTablesResponse.Create(request, (int)imageLinksCount, (int)imageLinksPagedCount, imageLinks);
            return new DataTablesJsonResult(response, true);
        }

        public virtual IActionResult Add()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public virtual async Task<IActionResult> Add(ImageLinkViewModel imageLink)
        {
            if (!ModelState.IsValid)
            {
                return View(imageLink);
            }
            else if (!imageLink.Image.IsImageFile())
            {
                ModelState.AddModelError("", "لطفاً یک تصویر معتبر انتخاب نمائید.");
                return View(imageLink);
            }

            var webRoot = _env.WebRootPath;
            using (Image<Rgba32> image = SixLabors.ImageSharp.Image.Load(imageLink.Image.OpenReadStream()))
            {
                image.Mutate(x => x
                        .Resize(new ResizeOptions
                        {
                            Size = new SixLabors.Primitives.Size(
                                Infrastructure.Constants.LinkImageWidthSize, Infrastructure.Constants.LinkImageHeightSize),
                            Mode = ResizeMode.Stretch
                        }));

                imageLink.Imagename = System.IO.Path.GetFileName(imageLink.Image.FileName);
                var file = System.IO.Path.Combine(webRoot, Infrastructure.Constants.ImageLinksRootPath, imageLink.Imagename);

                if (System.IO.File.Exists(file))
                {
                    imageLink.Imagename = $"{System.IO.Path.GetFileNameWithoutExtension(file)}{DateTime.Now.Ticks}{System.IO.Path.GetExtension(file)}";
                    file = System.IO.Path.Combine(webRoot, Infrastructure.Constants.ImageLinksRootPath, imageLink.Imagename);
                }

                await _imageLinkService.AddOrUpdateImageLinkAsync(imageLink).ConfigureAwait(false);
                image.Save(file); // Automatic encoder selected based on extension.
            }

            TempData["IsOk"] = true;
            return RedirectToAction("index", "imagelink", "manage");
        }

        public async virtual Task<IActionResult> Update(long id)
        {
            var imageLink = await _imageLinkService.FindImageLinkByIdAsync(id);

            if (imageLink == null)
            {
                return RedirectToAction("index", "imagelink", "manage");
            }

            var imagelinkViewModel = new ImageLinkViewModel
            {
                Id = imageLink.Id,
                Title = imageLink.Title,
                Description = imageLink.Description,
                Imagename = imageLink.Imagename,
                Url = imageLink.Url,
                IsBlankUrlTarget = imageLink.IsBlankUrlTarget,
                Priority = imageLink.Priority,
                PortalId = imageLink.PortalId,
                Language = imageLink.Language
            };

            return View(imagelinkViewModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public virtual async Task<IActionResult> Update(ImageLinkViewModel imageLink)
        {
            if (!ModelState.IsValid)
            {
                return View(imageLink);
            }

            var currentImagename = await _imageLinkService.GetImageLinkImagenameAsync(imageLink.Id);
            if (imageLink.Image != null)
            {
                if (!imageLink.Image.IsImageFile())
                {
                    ModelState.AddModelError("", "لطفاً یک تصویر معتبر انتخاب نمائید.");
                    return View(imageLink);
                }

                var webRoot = _env.WebRootPath;
                using (Image<Rgba32> image = SixLabors.ImageSharp.Image.Load(imageLink.Image.OpenReadStream()))
                {
                    image.Mutate(x => x
                            .Resize(new ResizeOptions
                            {
                                Size = new SixLabors.Primitives.Size(
                                Infrastructure.Constants.LinkImageWidthSize, Infrastructure.Constants.LinkImageHeightSize),
                                Mode = ResizeMode.Stretch
                            }));

                    imageLink.Imagename = System.IO.Path.GetFileName(imageLink.Image.FileName);
                    var file = System.IO.Path.Combine(webRoot, Infrastructure.Constants.ImageLinksRootPath, imageLink.Imagename);

                    if (System.IO.File.Exists(file))
                    {
                        imageLink.Imagename = $"{System.IO.Path.GetFileNameWithoutExtension(file)}{DateTime.Now.Ticks}{System.IO.Path.GetExtension(file)}";
                        file = System.IO.Path.Combine(webRoot, Infrastructure.Constants.ImageLinksRootPath, imageLink.Imagename);
                    }
                    else if (string.Equals(currentImagename, imageLink.Imagename, StringComparison.InvariantCultureIgnoreCase))
                    {
                        imageLink.Imagename = $"{System.IO.Path.GetFileNameWithoutExtension(file)}{DateTime.Now.Ticks}{System.IO.Path.GetExtension(file)}";
                        file = System.IO.Path.Combine(webRoot, Infrastructure.Constants.ImageLinksRootPath, imageLink.Imagename);
                    }

                    await _imageLinkService.AddOrUpdateImageLinkAsync(imageLink).ConfigureAwait(false);
                    image.Save(file); // Automatic encoder selected based on extension.

                    var previousFile = System.IO.Path.Combine(webRoot, Infrastructure.Constants.ImageLinksRootPath, currentImagename);
                    if (System.IO.File.Exists(previousFile))
                    {
                        System.IO.File.Delete(previousFile);
                    }
                }
            }
            else
            {
                imageLink.Imagename = currentImagename;
                await _imageLinkService.AddOrUpdateImageLinkAsync(imageLink).ConfigureAwait(false);
            }

            TempData["IsOk"] = true;
            return RedirectToAction("index", "imagelink", "manage");
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(long id)
        {
            var imagename = await _imageLinkService.GetImageLinkImagenameAsync(id);
            var webRoot = _env.WebRootPath;
            var file = System.IO.Path.Combine(webRoot, Infrastructure.Constants.ImageLinksRootPath, imagename);

            if (System.IO.File.Exists(file))
            {
                System.IO.File.Delete(file);
            }

            await _imageLinkService.DeleteImageLinkAsync(id);

            return Content("لینک تصویری با موفقیت حذف شد.", "text/html", System.Text.Encoding.UTF8);
        }
    }
}