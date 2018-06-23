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
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using ContentManagement.Common.ReflectionToolkit;
using ContentManagement.Common.WebToolkit.Attributes;
using System.Net;

namespace ContentManagement.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Policy = CustomRoles.Admin)]
    public partial class ContentController : Controller
    {
        private readonly IContentService _contentService;
        private readonly IOptionsSnapshot<SiteSettings> _siteSettings;
        private readonly IHostingEnvironment _env;
        private readonly IHtmlSanitizer _htmlSanitizer;
        private readonly IActionContextAccessor _actionContextAccessor;

        public ContentController(IContentService contentService, IOptionsSnapshot<SiteSettings> siteSettings, IHostingEnvironment env, IHtmlSanitizer htmlSanitizer, IActionContextAccessor actionContextAccessor)
        {
            _contentService = contentService;
            _contentService.CheckArgumentIsNull(nameof(contentService));

            _siteSettings = siteSettings;
            _siteSettings.CheckArgumentIsNull(nameof(siteSettings));

            _env = env;
            _env.CheckArgumentIsNull(nameof(env));

            _htmlSanitizer = htmlSanitizer;
            _htmlSanitizer.CheckArgumentIsNull(nameof(htmlSanitizer));

            _actionContextAccessor = actionContextAccessor;
            _actionContextAccessor.CheckArgumentIsNull(nameof(actionContextAccessor));
        }

        public virtual IActionResult Index(ContentManagement.Entities.ContentType? t)
        {
            var contentTypesList = new List<SelectListItem>() { new SelectListItem { Text = "", Selected = (t == null) } };
            var contentValues = Enum.GetValues(typeof(ContentManagement.Entities.ContentType)).Cast<ContentManagement.Entities.ContentType>();

            foreach (var item in contentValues)
            {
                var text = item.GetAttributeOfType<ContentTypeTextInAdminAttribute>().Description;
                contentTypesList.Add(new SelectListItem { Text = text, Value = ((int)item).ToString(), Selected = (t == item) });
            }

            ViewBag.ContentTypes = contentTypesList;
            return View();
        }

        public virtual async Task<IActionResult> List(IDataTablesRequest request, int portalId, Entities.Language language, ContentManagement.Entities.ContentType? contentType)
        {
            var contents = await _contentService.GetPagedContentsAsync(portalId, contentType, language, request.Search.Value, request.Start, request.Length).ConfigureAwait(false);
            var contentsCount = await _contentService.ContentsCountAsync();
            var contentsPagedCount = await _contentService.ContentsPagedCountAsync(portalId, contentType, language, request.Search.Value);

            foreach (var item in contents)
            {
                var baseOfCurrentDomain = _siteSettings.Value.DomainName;
                var pageHost = $"{item.PortalKey ?? "www"}.{baseOfCurrentDomain}";
                item.ContentLink = Url.RouteUrl("default", new { controller = "content", action = "details", id = item.Id, title = WebUtility.UrlDecode(item.Title)}, Request.Scheme, pageHost);
            }

            var response = DataTablesResponse.Create(request, (int)contentsCount, (int)contentsPagedCount, contents);
            return new DataTablesJsonResult(response, true);
        }

        public virtual IActionResult Add(ContentManagement.Entities.ContentType? t)
        {
            var vm = new ContentViewModel(t);
            return View(vm);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public virtual async Task<IActionResult> Add(ContentViewModel content)
        {
            if (!content.Text.IsValidRequiredHtml())
            {
                ModelState.AddModelError("", "لطفاً متن مطلب را وارد نمائید.");
                return View(content);
            }
            else if (!ModelState.IsValid)
            {
                return View(content);
            }
            else if (content.Image != null && !content.Image.IsImageFile())
            {
                ModelState.AddModelError("", "لطفاً یک تصویر معتبر انتخاب نمائید.");
                return View(content);
            }

            var baseOfCurrentDomain = _siteSettings.Value.DomainName;
            content.Text = _htmlSanitizer.Sanitize(content.Text);
            content.Text = content.Text.NofollowExternalLinks(baseOfCurrentDomain);
            content.RawText = content.Text.CleanAllTagsExceptContent();

            if (content.Image != null)
            {
                var webRoot = _env.WebRootPath;
                using (Image<Rgba32> image = SixLabors.ImageSharp.Image.Load(content.Image.OpenReadStream()))
                {
                    image.Mutate(x => x
                            .Resize(new ResizeOptions { Size = new SixLabors.Primitives.Size(
                                Infrastructure.Constants.ContentImageWidthSize, Infrastructure.Constants.ContentImageHeightSize),
                                Mode = ResizeMode.Max }));

                    content.Imagename = System.IO.Path.GetFileName(content.Image.FileName);
                    var file = System.IO.Path.Combine(webRoot, Infrastructure.Constants.ContentsRootPath, content.Imagename);

                    if (System.IO.File.Exists(file))
                    {
                        content.Imagename = $"{System.IO.Path.GetFileNameWithoutExtension(file)}{DateTime.Now.Ticks}{System.IO.Path.GetExtension(file)}";
                        file = System.IO.Path.Combine(webRoot, Infrastructure.Constants.ContentsRootPath, content.Imagename);
                    }

                    await _contentService.AddOrUpdateContentAsync(content).ConfigureAwait(false);
                    image.Save(file); // Automatic encoder selected based on extension.
                }
            }
            else
            {
                await _contentService.AddOrUpdateContentAsync(content).ConfigureAwait(false);
            }

            TempData["IsOk"] = true;
            return RedirectToAction("index", "content", "manage");
        }

        public async virtual Task<IActionResult> Update(long id)
        {
            var content = await _contentService.FindContentByIdAsync(id);

            if (content == null)
            {
                return RedirectToAction("index", "content", "manage");
            }

            var contentViewModel = new ContentViewModel (content.ContentType)
            {
                Id = content.Id,
                Title = content.Title,
                Text = content.Text,
                RawText = content.RawText,
                Summary = content.Summary,
                Imagename = content.Imagename,
                IsActive = content.IsActive,
                IsFavorite = content.IsFavorite,
                ContentType = content.ContentType,
                Priority = content.Priority,
                Language = content.Language,
                PortalId = content.PortalId
            };

            return View(contentViewModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public virtual async Task<IActionResult> Update(ContentViewModel content)
        {
            if (!content.Text.IsValidRequiredHtml())
            {
                ModelState.AddModelError("", "لطفاً متن مطلب را وارد نمائید.");
                return View(content);
            }
            else if (!ModelState.IsValid)
            {
                return View(content);
            }

            var baseOfCurrentDomain = _siteSettings.Value.DomainName;
            var currentImagename = await _contentService.GetContentImagenameAsync(content.Id);
            if (content.Image != null)
            {
                if (!content.Image.IsImageFile())
                {
                    ModelState.AddModelError("", "لطفاً یک تصویر معتبر انتخاب نمائید.");
                    return View(content);
                }

                var webRoot = _env.WebRootPath;
                using (Image<Rgba32> image = SixLabors.ImageSharp.Image.Load(content.Image.OpenReadStream()))
                {
                    image.Mutate(x => x
                            .Resize(new ResizeOptions { Size = new SixLabors.Primitives.Size(
                                Infrastructure.Constants.ContentImageWidthSize, Infrastructure.Constants.ContentImageHeightSize),
                                Mode = ResizeMode.Max }));

                    content.Imagename = System.IO.Path.GetFileName(content.Image.FileName);
                    var file = System.IO.Path.Combine(webRoot, Infrastructure.Constants.ContentsRootPath, content.Imagename);

                    if (System.IO.File.Exists(file))
                    {
                        content.Imagename = $"{System.IO.Path.GetFileNameWithoutExtension(file)}{DateTime.Now.Ticks}{System.IO.Path.GetExtension(file)}";
                        file = System.IO.Path.Combine(webRoot, Infrastructure.Constants.ContentsRootPath, content.Imagename);
                    }

                    content.Text = _htmlSanitizer.Sanitize(content.Text);
                    content.Text = content.Text.NofollowExternalLinks(baseOfCurrentDomain);
                    content.RawText = content.Text.CleanAllTagsExceptContent();
                    await _contentService.AddOrUpdateContentAsync(content).ConfigureAwait(false);
                    image.Save(file); // Automatic encoder selected based on extension.

                    if (!string.IsNullOrEmpty(currentImagename))
                    {
                        var previousFile = System.IO.Path.Combine(webRoot, Infrastructure.Constants.ContentsRootPath, currentImagename);
                        if (System.IO.File.Exists(previousFile))
                        {
                            System.IO.File.Delete(previousFile);
                        }
                    }
                }
            }
            else
            {
                content.Imagename = currentImagename;
                content.Text = _htmlSanitizer.Sanitize(content.Text);
                content.Text = content.Text.NofollowExternalLinks(baseOfCurrentDomain);
                content.RawText = content.Text.CleanAllTagsExceptContent();
                await _contentService.AddOrUpdateContentAsync(content).ConfigureAwait(false);
            }

            TempData["IsOk"] = true;
            return RedirectToAction("index", "content", "manage");
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(long id)
        {
            var imagename = await _contentService.GetContentImagenameAsync(id);

            if(!string.IsNullOrEmpty(imagename))
            {
                var webRoot = _env.WebRootPath;
                var file = System.IO.Path.Combine(webRoot, Infrastructure.Constants.ContentsRootPath, imagename);

                if (System.IO.File.Exists(file))
                {
                    System.IO.File.Delete(file);
                }
            }
            
            await _contentService.DeleteContentAsync(id);

            return Content("مطلب با موفقیت حذف شد.", "text/html", System.Text.Encoding.UTF8);
        }
    }
}