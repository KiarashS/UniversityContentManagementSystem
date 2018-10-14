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

namespace ContentManagement.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Policy = CustomRoles.User)]
    public partial class ContentGalleryController : Controller
    {
        private readonly IContentGalleryService _contentGalleryService;
        private readonly IContentService _contentService;
        private readonly IOptionsSnapshot<SiteSettings> _siteSettings;
        private readonly IHostingEnvironment _env;

        public ContentGalleryController(IContentGalleryService contentGalleryService, IContentService contentService, IOptionsSnapshot<SiteSettings> siteSettings, IHostingEnvironment env)
        {
            _contentGalleryService = contentGalleryService;
            _contentGalleryService.CheckArgumentIsNull(nameof(contentGalleryService));

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
            var positionValues = Enum.GetValues(typeof(ContentManagement.Entities.ContentGalleryPosition)).Cast<ContentManagement.Entities.ContentGalleryPosition>();
            var selectedPosition = await _contentService.GetGalleryPosition(cid);

            foreach (var item in positionValues)
            {
                var text = item.GetAttributeOfType<ContentGalleryPossitionTitleInAdminAttribute>().Description;
                positionsList.Add(new SelectListItem { Text = text, Value = ((int)item).ToString(), Selected = (selectedPosition == item) });
            }

            ViewBag.GalleryPositions = positionsList;

            return View();
        }

        public virtual async Task<IActionResult> List(IDataTablesRequest request, long cid)
        {
            var galleries = await _contentGalleryService.GetPagedGalleriesAsync(cid).ConfigureAwait(false);
            var galleriesCount = await _contentGalleryService.GalleryCountAsync(cid);
            var gallerisPagedCount = galleriesCount;

            var response = DataTablesResponse.Create(request, (int)galleriesCount, (int)gallerisPagedCount, galleries);
            return new DataTablesJsonResult(response, true);
        }

        public virtual IActionResult Add(long cid)
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public virtual async Task<IActionResult> Add(ContentGalleryViewModel gallery, long cid)
        {
            if (!ModelState.IsValid)
            {
                return View(gallery);
            }
            else if (!gallery.Image.IsImageFile())
            {
                ModelState.AddModelError("", "لطفاً یک تصویر معتبر انتخاب نمائید.");
                return View(gallery);
            }

            var webRoot = _env.WebRootPath;
            using (Image<Rgba32> image = SixLabors.ImageSharp.Image.Load(gallery.Image.OpenReadStream()))
            {
                if (image.Width > Infrastructure.Constants.ContentGalleryWidthSize && image.Height <= Infrastructure.Constants.ContentGalleryHeightSize)
                {
                    image.Mutate(x => x
                        .Resize(new ResizeOptions
                        {
                            Size = new SixLabors.Primitives.Size(
                            Infrastructure.Constants.ContentGalleryWidthSize),
                            Mode = ResizeMode.Max
                        }));
                }
                else if (image.Width <= Infrastructure.Constants.ContentGalleryWidthSize && image.Height > Infrastructure.Constants.ContentGalleryHeightSize)
                {
                    image.Mutate(x => x
                       .Resize(new ResizeOptions
                       {
                           Size = new SixLabors.Primitives.Size(
                           Infrastructure.Constants.ContentGalleryHeightSize),
                           Mode = ResizeMode.Max
                       }));
                }
                else if (image.Width > Infrastructure.Constants.ContentGalleryWidthSize && image.Height > Infrastructure.Constants.ContentGalleryHeightSize)
                {
                    image.Mutate(x => x
                       .Resize(new ResizeOptions
                       {
                           Size = new SixLabors.Primitives.Size(
                           Infrastructure.Constants.ContentGalleryWidthSize, Infrastructure.Constants.ContentGalleryHeightSize),
                           Mode = ResizeMode.Max
                       }));
                }

                gallery.Imagename = System.IO.Path.GetFileName(gallery.Image.FileName);
                var file = System.IO.Path.Combine(webRoot, Infrastructure.Constants.ContentGalleriesRootPath, gallery.Imagename);

                if (System.IO.File.Exists(file))
                {
                    gallery.Imagename = $"{System.IO.Path.GetFileNameWithoutExtension(file)}{DateTime.Now.Ticks}{System.IO.Path.GetExtension(file)}";
                    file = System.IO.Path.Combine(webRoot, Infrastructure.Constants.ContentGalleriesRootPath, gallery.Imagename);
                }

                await _contentGalleryService.AddOrUpdateGalleryAsync(cid, gallery).ConfigureAwait(false);
                image.Save(file); // Automatic encoder selected based on extension.
            }

            TempData["IsOk"] = true;
            return RedirectToAction("index", "contentgallery", new { area = "manage", cid });
        }

        public async virtual Task<IActionResult> Update(long cid, long id)
        {
            var gallery = await _contentGalleryService.FindGalleryByIdAsync(cid, id);

            if (gallery == null)
            {
                return RedirectToAction("index", "contentgallery", new { area = "manage", cid });
            }

            var galleryViewModel = new ContentGalleryViewModel
            {
                Id = gallery.Id,
                Imagename = gallery.Imagename,
                Priority = gallery.Priority,
                Caption = gallery.Caption
            };

            return View(galleryViewModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public virtual async Task<IActionResult> Update(long cid, ContentGalleryViewModel gallery)
        {
            if (!ModelState.IsValid)
            {
                return View(gallery);
            }

            var currentImagename = await _contentGalleryService.GetGalleryImagenameAsync(cid, gallery.Id);
            if (gallery.Image != null)
            {
                if (!gallery.Image.IsImageFile())
                {
                    ModelState.AddModelError("", "لطفاً یک تصویر معتبر انتخاب نمائید.");
                    return View(gallery);
                }

                var webRoot = _env.WebRootPath;
                using (Image<Rgba32> image = SixLabors.ImageSharp.Image.Load(gallery.Image.OpenReadStream()))
                {
                    if (image.Width > Infrastructure.Constants.ContentGalleryWidthSize && image.Height <= Infrastructure.Constants.ContentGalleryHeightSize)
                    {
                        image.Mutate(x => x
                            .Resize(new ResizeOptions
                            {
                                Size = new SixLabors.Primitives.Size(
                                Infrastructure.Constants.ContentGalleryWidthSize),
                                Mode = ResizeMode.Max
                            }));
                    }
                    else if (image.Width <= Infrastructure.Constants.ContentGalleryWidthSize && image.Height > Infrastructure.Constants.ContentGalleryHeightSize)
                    {
                        image.Mutate(x => x
                           .Resize(new ResizeOptions
                           {
                               Size = new SixLabors.Primitives.Size(
                               Infrastructure.Constants.ContentGalleryHeightSize),
                               Mode = ResizeMode.Max
                           }));
                    }
                    else if (image.Width > Infrastructure.Constants.ContentGalleryWidthSize && image.Height > Infrastructure.Constants.ContentGalleryHeightSize)
                    {
                        image.Mutate(x => x
                           .Resize(new ResizeOptions
                           {
                               Size = new SixLabors.Primitives.Size(
                               Infrastructure.Constants.ContentGalleryWidthSize, Infrastructure.Constants.ContentGalleryHeightSize),
                               Mode = ResizeMode.Max
                           }));
                    }

                    gallery.Imagename = System.IO.Path.GetFileName(gallery.Image.FileName);
                    var file = System.IO.Path.Combine(webRoot, Infrastructure.Constants.ContentGalleriesRootPath, gallery.Imagename);

                    if (System.IO.File.Exists(file))
                    {
                        gallery.Imagename = $"{System.IO.Path.GetFileNameWithoutExtension(file)}{DateTime.Now.Ticks}{System.IO.Path.GetExtension(file)}";
                        file = System.IO.Path.Combine(webRoot, Infrastructure.Constants.ContentGalleriesRootPath, gallery.Imagename);
                    }
                    else if (string.Equals(currentImagename, gallery.Imagename, StringComparison.InvariantCultureIgnoreCase))
                    {
                        gallery.Imagename = $"{System.IO.Path.GetFileNameWithoutExtension(file)}{DateTime.Now.Ticks}{System.IO.Path.GetExtension(file)}";
                        file = System.IO.Path.Combine(webRoot, Infrastructure.Constants.ContentGalleriesRootPath, gallery.Imagename);
                    }

                    await _contentGalleryService.AddOrUpdateGalleryAsync(cid, gallery).ConfigureAwait(false);
                    image.Save(file); // Automatic encoder selected based on extension.

                    var previousFile = System.IO.Path.Combine(webRoot, Infrastructure.Constants.ContentGalleriesRootPath, currentImagename);
                    if (System.IO.File.Exists(previousFile))
                    {
                        System.IO.File.Delete(previousFile);
                    }
                }
            }
            else
            {
                gallery.Imagename = currentImagename;
                await _contentGalleryService.AddOrUpdateGalleryAsync(cid, gallery).ConfigureAwait(false);
            }

            TempData["IsOk"] = true;
            return RedirectToAction("index", "contentgallery", new { area = "manage", cid });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(long cid, long id)
        {
            var imagename = await _contentGalleryService.GetGalleryImagenameAsync(cid, id);
            var webRoot = _env.WebRootPath;
            var file = System.IO.Path.Combine(webRoot, Infrastructure.Constants.ContentGalleriesRootPath, imagename);

            if (System.IO.File.Exists(file))
            {
                System.IO.File.Delete(file);
            }

            await _contentGalleryService.DeleteGalleryAsync(cid, id);

            return Content("گالری با موفقیت حذف شد.", "text/html", System.Text.Encoding.UTF8);
        }

        [HttpPost]
        public virtual async Task<IActionResult> UpdatePosition(long cid, ContentGalleryPosition position)
        {
            await _contentService.UpdateGalleryPosition(cid, position);
            if (position == ContentGalleryPosition.None)
            {
                return Content("گالری با موفقیت غیر فعال شد.", "text/html", System.Text.Encoding.UTF8);
            }
            else
            {
                return Content("موقعیت گالری با موفقیت بروز شد.", "text/html", System.Text.Encoding.UTF8);
            }
        }
    }
}