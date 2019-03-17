using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ContentManagement.Services.Contracts;
using Microsoft.Extensions.Options;
using ContentManagement.ViewModels.Settings;
using ContentManagement.Services;
using ContentManagement.Entities;
using ContentManagement.Common.GuardToolkit;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using ContentManagement.Common.WebToolkit.Attributes;
using ContentManagement.Common.ReflectionToolkit;
using Microsoft.Extensions.Localization;
using DNTCommon.Web.Core;
using ContentManagement.Services.Seo;
using System.Net;
using DNTBreadCrumb.Core;
using ContentManagement.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Transforms;
using System.IO;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace ContentManagement.Controllers
{
    public partial class ContentController : Controller
    {
        private readonly IContentService _contentService;
        private readonly IOptionsSnapshot<SiteSettings> _siteSettings;
        private readonly IRequestService _requestService;
        private readonly IStringLocalizer<SharedResource> _sharedLocalizer;
        private readonly IUrlUtilityService _urlUtilityService;
        private readonly SeoService _seoService;
        private readonly IHostingEnvironment _hostingEnvironment;

        public ContentController(IContentService contentService, IOptionsSnapshot<SiteSettings> siteSettings, IRequestService requestService, IStringLocalizer<SharedResource> sharedLocalizer, SeoService seoService, IUrlUtilityService urlUtilityService, IHostingEnvironment hostingEnvironment)
        {
            _contentService = contentService;
            _contentService.CheckArgumentIsNull(nameof(contentService));

            _siteSettings = siteSettings;
            _siteSettings.CheckArgumentIsNull(nameof(siteSettings));

            _requestService = requestService;
            _requestService.CheckArgumentIsNull(nameof(requestService));

            _sharedLocalizer = sharedLocalizer;
            _sharedLocalizer.CheckArgumentIsNull(nameof(sharedLocalizer));

            _seoService = seoService;
            _seoService.CheckArgumentIsNull(nameof(seoService));

            _urlUtilityService = urlUtilityService;
            _urlUtilityService.CheckArgumentIsNull(nameof(urlUtilityService));

            _hostingEnvironment = hostingEnvironment;
            _hostingEnvironment.CheckArgumentIsNull(nameof(hostingEnvironment));
        }

        public async virtual Task<IActionResult> Index(int? page, ContentType? t, bool otherContents = false, bool favorite = false)
        {
            if (!page.HasValue || page < 1)
            {
                page = 1;
            }

            var vm = new ContentsListViewModel();
            var portalKey = _requestService.PortalKey();
            var language = _requestService.CurrentLanguage().Language;
            vm.Page = page.Value;
            vm.PageSize = _siteSettings.Value.PagesSize.ContentsSize;
            //vm.Start = (vm.Page - 1) * vm.PageSize;
            vm.Start = (vm.PageSize * vm.Page) - vm.PageSize;
            vm.ContentType = t;
            vm.OtherContents = otherContents;
            vm.Favorite = favorite;
            vm.Language = language;

            if (favorite)
            {
                vm.ContentsViewModel = await _contentService.GetFavoritesAsync(portalKey, t, language, vm.Start, vm.PageSize);
                vm.TotalCount = await _contentService.FavoritesCountAsync(portalKey, t, language);

                _seoService.Title = _sharedLocalizer["Favorite Contents"];
                if (t.HasValue)
                {
                    _seoService.Title = _sharedLocalizer["Favorite Contents"] + " " + (language == Language.EN ? t.Value.GetAttributeOfType<ContentTypeTextEnAttribute>().Description : t.Value.GetAttributeOfType<ContentTypeTextFaAttribute>().Description);
                }
            }
            else if (otherContents)
            {
                vm.ContentsViewModel = await _contentService.GetOtherContentsAsync(portalKey, t, language, vm.Start, vm.PageSize);
                vm.TotalCount = await _contentService.OtherContentsCountAsync(portalKey, t, language);

                _seoService.Title = _sharedLocalizer["Other Contents"];
                if (t.HasValue)
                {
                    _seoService.Title = _sharedLocalizer["Other Contents"] + " " + (language == Language.EN ? t.Value.GetAttributeOfType<ContentTypeTextEnAttribute>().Description : t.Value.GetAttributeOfType<ContentTypeTextFaAttribute>().Description);
                }
            }
            else if (t.HasValue)
            {
                vm.ContentsViewModel = await _contentService.GetContentsAsync(portalKey, language, t.Value, vm.Start, vm.PageSize);
                vm.TotalCount = await _contentService.ContentsCountAsync(portalKey, language, t.Value);
                _seoService.Title = _sharedLocalizer["Contents"] + " " + (language == Language.EN ? t.Value.GetAttributeOfType<ContentTypeTextEnAttribute>().Description : t.Value.GetAttributeOfType<ContentTypeTextFaAttribute>().Description);
            }
            else
            {
                vm.ContentsViewModel = await _contentService.GetContentsAsync(portalKey, language, ContentType.News, vm.Start, vm.PageSize);
                vm.TotalCount = await _contentService.ContentsCountAsync(portalKey, language, ContentType.News);
                t = ContentType.News;
                vm.ContentType = t;
                _seoService.Title = _sharedLocalizer["Contents"] + " " + (language == Language.EN ? t.Value.GetAttributeOfType<ContentTypeTextEnAttribute>().Description : t.Value.GetAttributeOfType<ContentTypeTextFaAttribute>().Description);
            }

            foreach (var item in vm.ContentsViewModel)
            {
                item.Link = _urlUtilityService.GenerateUrl(portalKey, item.Id, item.Title, Url, scheme: Request.Scheme);
            }

            this.AddBreadCrumb(new BreadCrumb
            {
                Title = _sharedLocalizer["Contents"],
                Order = 1,
                GlyphIcon = "fas fa-list"
            });

            return View(vm);
        }

        //[ResponseCache(Duration = 3600)]
        [HttpPost]
        [AjaxOnly]
        public async virtual Task<IActionResult> FetchNews(ContentType t = ContentType.News, bool favorite = false)
        {
            var currentLanguage = _requestService.CurrentLanguage().Language;
            var currentPortalKey = _requestService.PortalKey();
            int size;
            IList<ContentManagement.ViewModels.ContentsViewModel> vm;
            
            if (favorite)
            {
                ViewData["NewsQuery"] = "?favorite=true";
                ViewData["IsFavorite"] = true;
                size = _siteSettings.Value.PagesSize.FavoritesTabSize;
                vm = await _contentService.GetFavoritesAsync(currentPortalKey, null, currentLanguage, 0, size).ConfigureAwait(false);
                t = ContentType.UpcomingEvent;
            }
            else if ( t == ContentType.UpcomingEvent)
            {
                ViewData["NewsQuery"] = "?t=upcomingevent";
                ViewData["IsFavorite"] = false;
                size = _siteSettings.Value.PagesSize.UpcomingEventsTabSize;
                vm = await _contentService.GetContentsAsync(currentPortalKey, currentLanguage, ContentType.UpcomingEvent, 0, size).ConfigureAwait(false);
                t = ContentType.UpcomingEvent;
            }
            else
            {
                ViewData["NewsQuery"] = "?t=news";
                ViewData["IsFavorite"] = false;
                size = _siteSettings.Value.PagesSize.NewsTabSize;
                vm = await _contentService.GetContentsAsync(currentPortalKey, currentLanguage, ContentType.News, 0, size).ConfigureAwait(false);
            }

            return PartialView(t == ContentType.News ? "_News" : "_OtherContents", vm);
        }

        //[ResponseCache(Duration = 3600)]
        [HttpPost]
        [AjaxOnly]
        public async virtual Task<IActionResult> FetchContents(ContentType? t, bool otherContents = false)
        {
            var currentLanguage = _requestService.CurrentLanguage().Language;
            var currentPortalKey = _requestService.PortalKey();
            int size;
            IList<ContentManagement.ViewModels.ContentsViewModel> vm;

            ViewData["IsOtherContents"] = false;

            if (otherContents)
            {
                ViewData["IsOtherContents"] = true;
                size = _siteSettings.Value.PagesSize.OtherContentsTabSize;

                if (t.HasValue)
                {
                    vm = await _contentService.GetOtherContentsAsync(currentPortalKey, t.Value, currentLanguage, 0, size).ConfigureAwait(false);
                    ViewData["ContentsQuery"] = "?othercontents=true&t=" + t.Value.ToString().ToLowerInvariant();
                }
                else
                {
                    vm = await _contentService.GetOtherContentsAsync(currentPortalKey, null, currentLanguage, 0, size).ConfigureAwait(false);
                    ViewData["ContentsQuery"] = "?othercontents=true";
                }

                var contentsVisibilityViewModel = await _contentService.CheckContentsVisibility(_requestService.PortalKey(), currentLanguage);
                var otherContentsItems = new List<SelectListItem>();

                foreach (var item in contentsVisibilityViewModel.Where(x => x.IsVisible).ToList())
                {
                    if (item.ContentType != ContentType.Education &&
                        item.ContentType != ContentType.Form &&
                        item.ContentType != ContentType.EducationalCalendar &&
                        item.ContentType != ContentType.StudentAndCultural &&
                        item.ContentType != ContentType.PoliticalAndIdeological)
                    {
                        var text = "";
                        var value = item.ContentType.ToString().ToLowerInvariant();

                        if (currentLanguage == Entities.Language.EN)
                        {
                            text = item.ContentType.GetAttributeOfType<ContentTypeTextEnAttribute>().Description;
                        }
                        else
                        {
                            text = item.ContentType.GetAttributeOfType<ContentTypeTextFaAttribute>().Description;
                        }

                        otherContentsItems.Add(new SelectListItem
                        {
                            Text = text,
                            Value = value,
                            Selected = (t.HasValue && t == item.ContentType)
                        });
                    }
                }

                ViewData["ContentTypes"] = otherContentsItems;
            }
            else if (t == ContentType.Form)
            {
                ViewData["ContentsQuery"] = "?t=form";
                size = _siteSettings.Value.PagesSize.FormsTabSize;
                vm = await _contentService.GetContentsAsync(currentPortalKey, currentLanguage, ContentType.Form, 0, size).ConfigureAwait(false);
            }
            else if (t == ContentType.EducationalCalendar)
            {
                ViewData["ContentsQuery"] = "?t=educationalcalendar";
                size = _siteSettings.Value.PagesSize.EducationalCalendarsTabSize;
                vm = await _contentService.GetContentsAsync(currentPortalKey, currentLanguage, ContentType.EducationalCalendar, 0, size).ConfigureAwait(false);
            }
            else if (t == ContentType.StudentAndCultural)
            {
                ViewData["ContentsQuery"] = "?t=studentandcultural";
                size = _siteSettings.Value.PagesSize.StudentAndCulturalsTabSize;
                vm = await _contentService.GetContentsAsync(currentPortalKey, currentLanguage, ContentType.StudentAndCultural, 0, size).ConfigureAwait(false);
            }
            else if (t == ContentType.PoliticalAndIdeological)
            {
                ViewData["ContentsQuery"] = "?t=politicalandideological";
                size = _siteSettings.Value.PagesSize.PoliticalAndIdeologicalsTabSize;
                vm = await _contentService.GetContentsAsync(currentPortalKey, currentLanguage, ContentType.PoliticalAndIdeological, 0, size).ConfigureAwait(false);
            }
            else
            {
                ViewData["ContentsQuery"] = "?t=education";
                size = _siteSettings.Value.PagesSize.EducationsTabSize;
                vm = await _contentService.GetContentsAsync(currentPortalKey, currentLanguage, ContentType.Education, 0, size).ConfigureAwait(false);
            }

            return PartialView("_Contents", vm);
        }

        public async virtual Task<IActionResult> Details(long id, string title)
        {
            if (id == 0)
            {
                return RedirectToAction("index", "error", new { id = 404 });
            }

            var portalKey = _requestService.PortalKey();
            var language = _requestService.CurrentLanguage().Language;
            var contentTitle = await _contentService.GetTitle(portalKey, language, id).ConfigureAwait(false);
            var rawTitle = contentTitle;

            if (string.IsNullOrEmpty(contentTitle))
            {
                return RedirectToAction("index", "error", new { id = 404 });
            }

            contentTitle = ContentManagement.Common.WebToolkit.Slug.SeoFriendlyTitle(contentTitle);
            if (string.IsNullOrEmpty(title) || !string.Equals(title, contentTitle))
            {
                return RedirectToActionPermanent("details", "content", new { id, title = WebUtility.UrlDecode(contentTitle) });
            }

            //await _contentService.UpdateViewCount(portalKey, language, id).ConfigureAwait(false);
            var contentDetails = await _contentService.GetContentDetails(portalKey, language, id).ConfigureAwait(false);

            if (contentDetails == null)
            {
                return RedirectToAction("index", "error", new { id = 404 });
            }

            contentDetails.HasGallery = await _contentService.HasGallery(id).ConfigureAwait(false);

            _seoService.Title = contentDetails.Title;
            _seoService.MetaDescription = contentDetails.GetSummary;
            _seoService.MetaKeywords = contentDetails.Keywords;

            this.AddBreadCrumb(new BreadCrumb
            {
                Title = _sharedLocalizer["Contents"],
                Url = Url.Action("Index", "Content", values: new { area = "" }),
                Order = 1,
                GlyphIcon = "fas fa-list"
            });
            this.AddBreadCrumb(new BreadCrumb
            {
                Title = rawTitle,
                Order = 2,
                GlyphIcon = "far fa-newspaper"
            });

            return View(contentDetails);
        }

        public virtual IActionResult GetImage(string name)
        {
            var imageName = System.IO.Path.GetFileName(name);
            var imagePath = System.IO.Path.Combine(_hostingEnvironment.WebRootPath, Infrastructure.Constants.ContentsRootPath, imageName);
            new FileExtensionContentTypeProvider().TryGetContentType(imageName, out string contentType);

            return PhysicalFile(imagePath, contentType ?? "application/octet-stream");
        }

        public virtual IActionResult GetGalleryImage(string name, int? w, int? h)
        {
            var imageName = System.IO.Path.GetFileName(name);
            var imagePath = System.IO.Path.Combine(_hostingEnvironment.WebRootPath, Infrastructure.Constants.ContentGalleriesRootPath, imageName);
            new FileExtensionContentTypeProvider().TryGetContentType(imageName, out string contentType);

            if (!w.HasValue && !h.HasValue)
            {
                return PhysicalFile(imagePath, contentType ?? "application/octet-stream");
            }
            else
            {
                w = w ?? 100;
                h = h ?? 50;
                var outputStream = new MemoryStream();

                using (Image<Rgba32> image = SixLabors.ImageSharp.Image.Load(imagePath))
                {
                    image.Mutate(x => x
                        .Resize(new ResizeOptions
                        {
                            Size = new SixLabors.Primitives.Size(w.Value, h.Value),
                            Mode = ResizeMode.Stretch
                        }));

                    image.Save(outputStream, GetImageFormat(System.IO.Path.GetExtension(imageName).ToLowerInvariant()));
                }

                return new FileContentResult(outputStream.ToArray(), contentType ?? "application/octet-stream");
            }
        }

        private IImageEncoder GetImageFormat(string extension)
        {
            if (extension == ".bmp")
            {
                return new BmpEncoder();
            }
            else if (extension == ".gif")
            {
                return new GifEncoder();
            }
            else if (extension == ".jpg" || extension == ".jpeg")
            {
                return new JpegEncoder();
            }
            else
            {
                return new PngEncoder();
            }
        }
    }
}
