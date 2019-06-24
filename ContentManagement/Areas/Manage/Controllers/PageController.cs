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

namespace ContentManagement.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Policy = CustomRoles.User)]
    public partial class PageController : Controller
    {
        private readonly IPageService _pageService;
        private readonly IOptionsSnapshot<SiteSettings> _siteSettings;
        private readonly IHostingEnvironment _env;
        private readonly IHtmlSanitizer _htmlSanitizer;
        private readonly IActionContextAccessor _actionContextAccessor;

        public PageController(IPageService pageService, IOptionsSnapshot<SiteSettings> siteSettings, IHostingEnvironment env, IHtmlSanitizer htmlSanitizer, IActionContextAccessor actionContextAccessor)
        {
            _pageService = pageService;
            _pageService.CheckArgumentIsNull(nameof(pageService));

            _siteSettings = siteSettings;
            _siteSettings.CheckArgumentIsNull(nameof(siteSettings));

            _env = env;
            _env.CheckArgumentIsNull(nameof(env));

            _htmlSanitizer = htmlSanitizer;
            _htmlSanitizer.CheckArgumentIsNull(nameof(htmlSanitizer));

            _actionContextAccessor = actionContextAccessor;
            _actionContextAccessor.CheckArgumentIsNull(nameof(actionContextAccessor));
        }

        public virtual IActionResult Index()
        {
            return View();
        }

        public virtual async Task<IActionResult> List(IDataTablesRequest request, int portalId, Entities.Language language)
        {
            var pages = await _pageService.GetPagedPagesAsync(portalId, language, request.Search.Value, request.Start, request.Length).ConfigureAwait(false);
            var pagesCount = await _pageService.PagesCountAsync();
            var pagesPagedCount = await _pageService.PagesPagedCountAsync(portalId, language, request.Search.Value);

            foreach (var item in pages)
            {
                var baseOfCurrentDomain = _siteSettings.Value.DomainName;
                var pageHost = $"{item.PortalKey ?? "www"}.{baseOfCurrentDomain}";
                item.PageLink = Url.RouteUrl("pageRoute", new { slug = item.Slug, action = "" }, Request.Scheme, pageHost);
            }

            var response = DataTablesResponse.Create(request, (int)pagesCount, (int)pagesPagedCount, pages);
            return new DataTablesJsonResult(response, true);
        }

        public virtual IActionResult Add()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public virtual async Task<IActionResult> Add(PageViewModel page)
        {
            if (!page.Text.IsValidRequiredHtml())
            {
                ModelState.AddModelError("", "لطفاً متن صفحه را وارد نمائید.");
                return View(page);
            }
            else if (!ModelState.IsValid)
            {
                return View(page);
            }
            else if (string.IsNullOrEmpty(page.Slug.GenerateSlug()))
            {
                ModelState.AddModelError("", "لطفاً یک شناسه یکتا(انگلیسی) وارد نمائید.");
                return View(page);
            }
            else if (page.EnableImage && page.Image == null)
            {
                ModelState.AddModelError("", "لطفاً تصویر را انتخاب نمائید.");
                return View(page);
            }
            else if (page.EnableImage && page.Image != null && !page.Image.IsImageFile())
            {
                ModelState.AddModelError("", "لطفاً یک تصویر معتبر انتخاب نمائید.");
                return View(page);
            }

            var baseOfCurrentDomain = _siteSettings.Value.DomainName;
            page.Text = _htmlSanitizer.Sanitize(page.Text);
            page.Text = page.Text.NofollowExternalLinks(baseOfCurrentDomain);
            page.RawText = page.Text.CleanAllTagsExceptContent();

            if (page.EnableImage && page.Image != null)
            {
                var webRoot = _env.WebRootPath;
                using (Image<Rgba32> image = SixLabors.ImageSharp.Image.Load(page.Image.OpenReadStream()))
                {
                    if (image.Width > Infrastructure.Constants.PageImageWidthSize && image.Height <= Infrastructure.Constants.PageImageHeightSize)
                    {
                        image.Mutate(x => x
                            .Resize(new ResizeOptions
                            {
                                Size = new SixLabors.Primitives.Size(
                                Infrastructure.Constants.PageImageWidthSize),
                                Mode = ResizeMode.Max
                            }));
                    }
                    else if (image.Width <= Infrastructure.Constants.PageImageWidthSize && image.Height > Infrastructure.Constants.PageImageHeightSize)
                    {
                        image.Mutate(x => x
                           .Resize(new ResizeOptions
                           {
                               Size = new SixLabors.Primitives.Size(
                               Infrastructure.Constants.PageImageHeightSize),
                               Mode = ResizeMode.Max
                           }));
                    }
                    else if (image.Width > Infrastructure.Constants.PageImageWidthSize && image.Height > Infrastructure.Constants.PageImageHeightSize)
                    {
                        image.Mutate(x => x
                           .Resize(new ResizeOptions
                           {
                               Size = new SixLabors.Primitives.Size(
                               Infrastructure.Constants.PageImageWidthSize, Infrastructure.Constants.PageImageHeightSize),
                               Mode = ResizeMode.Max
                           }));
                    }

                    page.Imagename = System.IO.Path.GetFileName(page.Image.FileName);
                    var file = System.IO.Path.Combine(webRoot, Infrastructure.Constants.PagesRootPath, page.Imagename);

                    if (System.IO.File.Exists(file))
                    {
                        page.Imagename = $"{System.IO.Path.GetFileNameWithoutExtension(file)}{DateTime.Now.Ticks}{System.IO.Path.GetExtension(file)}";
                        file = System.IO.Path.Combine(webRoot, Infrastructure.Constants.PagesRootPath, page.Imagename);
                    }

                    await _pageService.AddOrUpdatePageAsync(page).ConfigureAwait(false);
                    image.Save(file); // Automatic encoder selected based on extension.
                }
            }
            else
            {
                await _pageService.AddOrUpdatePageAsync(page).ConfigureAwait(false);
            }

            TempData["IsOk"] = true;
            return RedirectToAction("index", "page", "manage");
        }

        public async virtual Task<IActionResult> Update(long id)
        {
            var page = await _pageService.FindPageByIdAsync(id);

            if (page == null)
            {
                return RedirectToAction("index", "page", "manage");
            }

            var pageViewModel = new PageViewModel
            {
                Id = page.Id,
                Title = page.Title,
                Text = page.Text,
                Slug = page.Slug,
                Keywords = page.Keywords,
                Imagename = page.Imagename,
                EnableImage = !string.IsNullOrEmpty(page.Imagename),
                IsActive = page.IsActive,
                PortalId = page.PortalId,
                Language = page.Language
            };

            return View(pageViewModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public virtual async Task<IActionResult> Update(PageViewModel page)
        {
            if (!page.Text.IsValidRequiredHtml())
            {
                ModelState.AddModelError("", "لطفاً متن صفحه را وارد نمائید.");
                return View(page);
            }
            else if (!ModelState.IsValid)
            {
                return View(page);
            }
            else if (string.IsNullOrEmpty(page.Slug.GenerateSlug()))
            {
                ModelState.AddModelError("", "لطفاً یک شناسه یکتا(انگلیسی) وارد نمائید.");
                return View(page);
            }

            var webRoot = _env.WebRootPath;
            var baseOfCurrentDomain = _siteSettings.Value.DomainName;
            var currentImagename = await _pageService.GetPageImagenameAsync(page.Id);
            if (page.EnableImage && page.Image != null)
            {
                if (!page.Image.IsImageFile())
                {
                    ModelState.AddModelError("", "لطفاً یک تصویر معتبر انتخاب نمائید.");
                    return View(page);
                }

                using (Image<Rgba32> image = SixLabors.ImageSharp.Image.Load(page.Image.OpenReadStream()))
                {
                    if (image.Width > Infrastructure.Constants.PageImageWidthSize && image.Height <= Infrastructure.Constants.PageImageHeightSize)
                    {
                        image.Mutate(x => x
                            .Resize(new ResizeOptions
                            {
                                Size = new SixLabors.Primitives.Size(
                                Infrastructure.Constants.PageImageWidthSize),
                                Mode = ResizeMode.Max
                            }));
                    }
                    else if (image.Width <= Infrastructure.Constants.PageImageWidthSize && image.Height > Infrastructure.Constants.PageImageHeightSize)
                    {
                        image.Mutate(x => x
                           .Resize(new ResizeOptions
                           {
                               Size = new SixLabors.Primitives.Size(
                               Infrastructure.Constants.PageImageHeightSize),
                               Mode = ResizeMode.Max
                           }));
                    }
                    else if (image.Width > Infrastructure.Constants.PageImageWidthSize && image.Height > Infrastructure.Constants.PageImageHeightSize)
                    {
                        image.Mutate(x => x
                           .Resize(new ResizeOptions
                           {
                               Size = new SixLabors.Primitives.Size(
                               Infrastructure.Constants.PageImageWidthSize, Infrastructure.Constants.PageImageHeightSize),
                               Mode = ResizeMode.Max
                           }));
                    }

                    page.Imagename = System.IO.Path.GetFileName(page.Image.FileName);
                    var file = System.IO.Path.Combine(webRoot, Infrastructure.Constants.PagesRootPath, page.Imagename);

                    if (System.IO.File.Exists(file))
                    {
                        page.Imagename = $"{System.IO.Path.GetFileNameWithoutExtension(file)}{DateTime.Now.Ticks}{System.IO.Path.GetExtension(file)}";
                        file = System.IO.Path.Combine(webRoot, Infrastructure.Constants.PagesRootPath, page.Imagename);
                    }
                    else if (string.Equals(currentImagename, page.Imagename, StringComparison.InvariantCultureIgnoreCase))
                    {
                        page.Imagename = $"{System.IO.Path.GetFileNameWithoutExtension(file)}{DateTime.Now.Ticks}{System.IO.Path.GetExtension(file)}";
                        file = System.IO.Path.Combine(webRoot, Infrastructure.Constants.PagesRootPath, page.Imagename);
                    }

                    page.Text = _htmlSanitizer.Sanitize(page.Text);
                    page.Text = page.Text.NofollowExternalLinks(baseOfCurrentDomain);
                    page.RawText = page.Text.CleanAllTagsExceptContent();
                    await _pageService.AddOrUpdatePageAsync(page).ConfigureAwait(false);
                    image.Save(file); // Automatic encoder selected based on extension.

                    if (!string.IsNullOrEmpty(currentImagename))
                    {
                        var previousFile = System.IO.Path.Combine(webRoot, Infrastructure.Constants.PagesRootPath, currentImagename);
                        if (System.IO.File.Exists(previousFile))
                        {
                            System.IO.File.Delete(previousFile);
                        }
                    }
                }
            }
            else if (!page.EnableImage) // Remove Image
            {
                if (!string.IsNullOrEmpty(currentImagename))
                {
                    var previousFile = System.IO.Path.Combine(webRoot, Infrastructure.Constants.PagesRootPath, currentImagename);
                    if (System.IO.File.Exists(previousFile))
                    {
                        System.IO.File.Delete(previousFile);
                    }
                }

                page.Imagename = null;
                page.Text = _htmlSanitizer.Sanitize(page.Text);
                page.Text = page.Text.NofollowExternalLinks(baseOfCurrentDomain);
                page.RawText = page.Text.CleanAllTagsExceptContent();
                await _pageService.AddOrUpdatePageAsync(page).ConfigureAwait(false);
            }
            else
            {
                page.Imagename = currentImagename;
                page.Text = _htmlSanitizer.Sanitize(page.Text);
                page.Text = page.Text.NofollowExternalLinks(baseOfCurrentDomain);
                page.RawText = page.Text.CleanAllTagsExceptContent();
                await _pageService.AddOrUpdatePageAsync(page).ConfigureAwait(false);
            }

            TempData["IsOk"] = true;
            return RedirectToAction("index", "page", "manage");
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(long id)
        {
            var imagename = await _pageService.GetPageImagenameAsync(id);

            if(!string.IsNullOrEmpty(imagename))
            {
                var webRoot = _env.WebRootPath;
                var file = System.IO.Path.Combine(webRoot, Infrastructure.Constants.PagesRootPath, imagename);

                if (System.IO.File.Exists(file))
                {
                    System.IO.File.Delete(file);
                }
            }
            
            await _pageService.DeletePageAsync(id);

            return Content("صفحه با موفقیت حذف شد.", "text/html", System.Text.Encoding.UTF8);
        }

        [HttpPost]
        public virtual async Task<IActionResult> CheckSlug(string slug, string initialSlug)
        {
            slug = slug.GenerateSlug();
            if (!string.IsNullOrEmpty(initialSlug) && slug.Trim() == initialSlug.Trim())
            {
                return Json(true);
            }

            if (string.IsNullOrEmpty(slug))
            {
                return Json(false);
            }

            return Json(!await _pageService.ValidatePageSlugAsync(slug));
        }
    }
}