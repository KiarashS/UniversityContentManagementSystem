namespace ContentManagement.Services
{
    using ContentManagement.Common.GuardToolkit;
    using ContentManagement.DataLayer.Context;
    using ContentManagement.Entities;
    using ContentManagement.Services.Contracts;
    using ContentManagement.ViewModels;
    using ContentManagement.ViewModels.Areas.Manage;
    using DNTCommon.Web.Core;
    using EFSecondLevelCache.Core;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class ContentService : IContentService
    {
        private readonly IUnitOfWork _uow;
        private readonly DbSet<Content> _content;

        public ContentService(IUnitOfWork uow)
        {
            _uow = uow;
            _uow.CheckArgumentIsNull(nameof(uow));

            _content = _uow.Set<Content>();
        }

        public async Task AddOrUpdateContentAsync(ContentManagement.ViewModels.Areas.Manage.ContentViewModel content)
        {
            if (content.Id == 0) // Add
            {
                var newContent = new Content
                {
                    Title = content.Title?.Trim(),
                    Text = content.Text,
                    RawText = content.RawText,
                    Summary = content.Summary?.Trim(),
                    Keywords = content.Keywords?.Trim(),
                    Imagename = content.Imagename,
                    IsActive = content.IsActive,
                    IsFavorite = content.IsFavorite,
                    ContentType = content.ContentType,
                    Priority = content.Priority,
                    Language = content.Language,
                    PortalId = content.PortalId
                };

                _content.Add(newContent);
                await _uow.SaveChangesAsync().ConfigureAwait(false);
                return;
            }

            var currentContent = await FindContentByIdAsync(content.Id).ConfigureAwait(false);
            currentContent.Title = content.Title?.Trim();
            currentContent.Text = content.Text;
            currentContent.RawText = content.RawText;
            currentContent.Summary = content.Summary?.Trim();
            currentContent.Keywords = content.Keywords?.Trim();
            currentContent.Imagename = content.Imagename;
            currentContent.IsActive = content.IsActive;
            currentContent.IsFavorite = content.IsFavorite;
            currentContent.ContentType = content.ContentType;
            currentContent.Priority = content.Priority;
            currentContent.Language = content.Language;
            currentContent.PortalId = content.PortalId;

            await _uow.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task DeleteContentAsync(long id)
        {
            var content = new Content { Id = id };

            _uow.Entry(content).State = EntityState.Deleted;

            await _uow.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<Content> FindContentByIdAsync(long contentId)
        {
            var content = await _content.FirstOrDefaultAsync(x => x.Id == contentId);
            return content;
        }

        public async Task<IList<ContentManagement.ViewModels.Areas.Manage.ContentViewModel>> GetPagedContentsAsync(int portalId, ContentType? contentType, Language language = Language.FA, string searchTerm = null, int start = 0, int length = 10)
        {
            if (!string.IsNullOrEmpty(searchTerm) && searchTerm.StartsWith("@id", StringComparison.InvariantCultureIgnoreCase))
            {
                long id;
                if (long.TryParse(searchTerm.ToLowerInvariant().Replace("@id", string.Empty).Trim(), out id))
                {
                    var content = await _content
                                        .Where(x => x.Id == id)
                                        .Select(x => new { x.Id, x.Title, x.Text, x.Summary, x.Language, x.ContentType, x.Imagename, x.PublishDate, x.IsActive, x.IsFavorite, x.Priority, x.ViewCount, x.Portal.PortalKey })
                                        .SingleOrDefaultAsync();

                    if (content == null)
                    {
                        return new List<ContentManagement.ViewModels.Areas.Manage.ContentViewModel>();
                    }

                    return new List<ContentManagement.ViewModels.Areas.Manage.ContentViewModel> {
                        new ViewModels.Areas.Manage.ContentViewModel {
                            Id = content.Id,
                            PortalKey = content.PortalKey,
                            ContentType = content.ContentType,
                            Imagename = content.Imagename,
                            IsActive = content.IsActive,
                            IsFavorite = content.IsFavorite,
                            Priority = content.Priority,
                            PublishDate = content.PublishDate,
                            Text = content.Text,
                            Title = content.Title,
                            Summary = content.Summary,
                            ViewCount = content.ViewCount,
                            Language = content.Language
                        }
                    };
                }
            }

            var query = _content.Where(x => x.PortalId == portalId && x.Language == language).AsQueryable();

            if (contentType.HasValue)
            {
                query = query.Where(x => x.ContentType == contentType);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.Trim();
                query = query.Where(x => x.Title.Contains(searchTerm) || x.RawText.Contains(searchTerm));
            }

            var contents = await query
                                    .OrderByDescending(x => x.Priority)
                                    .ThenByDescending(x => x.PublishDate)
                                    .Skip(start)
                                    .Take(length)
                                    .Cacheable()
                                    .Select(x => new { x.Id, x.Title, x.Text, x.Summary, x.Imagename, x.PublishDate, x.IsActive, x.IsFavorite, x.Priority, x.ViewCount, x.Portal.PortalKey })
                                    .ToListAsync();

            return contents.Select(x => new ContentManagement.ViewModels.Areas.Manage.ContentViewModel(null) { Id = x.Id, Title = x.Title, Text = x.Text, Summary = x.Summary, Imagename = x.Imagename, PublishDate = x.PublishDate, IsActive = x.IsActive, IsFavorite = x.IsFavorite, ViewCount = x.ViewCount, Priority = x.Priority, PortalKey = x.PortalKey }).ToList();
        }

        public async Task<long> ContentsCountAsync()
        {
            var content = await _content.LongCountAsync().ConfigureAwait(false);
            return content;
        }

        public async Task<long> ContentsCountAsync(string portalKey)
        {
            var content = await _content.LongCountAsync(x => x.Portal.PortalKey == portalKey).ConfigureAwait(false);
            return content;
        }

        public async Task<long> ContentsPagedCountAsync(int portalId, ContentType? contentType, Language language = Language.FA, string searchTerm = null)
        {
            if (!string.IsNullOrEmpty(searchTerm) && searchTerm.StartsWith("@id", StringComparison.InvariantCultureIgnoreCase))
            {
                long id;
                if (long.TryParse(searchTerm.ToLowerInvariant().Replace("@id", string.Empty).Trim(), out id))
                {
                    return await _content.LongCountAsync(x => x.Id == id).ConfigureAwait(false);
                }
            }

            var query = _content.Where(x => x.PortalId == portalId && x.Language == language).AsQueryable();

            if (contentType.HasValue)
            {
                query = query.Where(x => x.ContentType == contentType);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.Trim();
                query = query.Where(x => x.Title.Contains(searchTerm) || x.RawText.Contains(searchTerm));
            }

            var count = await query.LongCountAsync().ConfigureAwait(false);
            return count;
        }

        public Task<string> GetContentImagenameAsync(long id)
        {
            var imagename = _content.Where(x => x.Id == id).Select(x => x.Imagename).SingleOrDefaultAsync();
            return imagename;
        }

        public async Task<IList<SubPortalsContentsViewModel>> GetSubPortalsContentsAsync(Language language = Language.FA, int maxSize = 30)
        {
            var subPortalsContentsViewModel = new List<SubPortalsContentsViewModel>();
            var subPortalsContents = await _content
                                        .Where(x => x.IsActive && x.Language == language && x.Portal.PortalKey != null && x.Portal.ShowInMainPortal)
                                        .OrderByDescending(x => x.Priority)
                                        .ThenByDescending(x => x.PublishDate).AsQueryable()
                                        .Select(x => new { x.Id, x.Title, x.ContentType, x.PublishDate, x.IsFavorite, x.Portal.PortalKey, HasGallery = (x.GalleryPosition != ContentGalleryPosition.None && x.ContentGalleries.Count > 0), PortalTitle = (language == Language.EN ? x.Portal.TitleEn : x.Portal.TitleFa) })
                                        .Cacheable()
                                        .ToListAsync();


            foreach (var item in subPortalsContents)
            {
                subPortalsContentsViewModel.Add(new SubPortalsContentsViewModel {
                    Id = item.Id,
                    PortalKey = item.PortalKey,
                    PortalTitle = item.PortalTitle,
                    Language = language,
                    ContentType = item.ContentType,
                    IsFavorite = item.IsFavorite,
                    Title = item.Title,
                    PublishDate = item.PublishDate,
                    HasGallery = item.HasGallery
                });
            }

            return subPortalsContentsViewModel;
        }

        public async Task<IList<ContentsViewModel>> GetContentsAsync(string portalKey, Language language = Language.FA, ContentType contentType = ContentType.News, int start = 0, int length = 10)
        {
            var contents = await _content.Where(x => x.Portal.PortalKey == portalKey && x.Language == language && x.ContentType == contentType && x.IsActive)
                                    .OrderByDescending(x => x.Priority)
                                    .ThenByDescending(x => x.PublishDate)
                                    .Skip(start)
                                    .Take(length)
                                    .Select(x => new { x.Id, x.Title, x.RawText, x.Summary, x.Imagename, x.IsFavorite, x.PublishDate, x.Priority, HasGallery = (x.GalleryPosition != ContentGalleryPosition.None && x.ContentGalleries.Count > 0) })
                                    .Cacheable()
                                    .ToListAsync();

            return contents.Select(x => new ContentsViewModel { Id = x.Id, Title = x.Title, RawText = x.RawText, HasGallery = x.HasGallery, Summary = x.Summary, Imagename = x.Imagename, IsFavorite = x.IsFavorite, PublishDate = x.PublishDate, Priority = x.Priority, Language = language, ContentType = contentType}).ToList();
        }

        public async Task<long> ContentsCountAsync(string portalKey, Language language = Language.FA, ContentType contentType = ContentType.News)
        {
            var count = await _content.Where(x => x.Portal.PortalKey == portalKey && x.Language == language && x.ContentType == contentType && x.IsActive)
                                    .Cacheable()
                                    .LongCountAsync()
                                    .ConfigureAwait(false);

            return count;
        }

        public async Task<IList<ContentsViewModel>> GetFavoritesAsync(string portalKey, ContentType? contentType, Language language = Language.FA, int start = 0, int length = 10)
        {
            var query = _content.Where(x => x.Portal.PortalKey == portalKey && x.Language == language && x.IsFavorite && x.IsActive).AsQueryable();
                                    

            if (contentType.HasValue)
            {
                query = query.Where(x => x.ContentType == contentType.Value);
            }

            var contents = await query
                                    .OrderByDescending(x => x.Priority)
                                    .ThenByDescending(x => x.PublishDate)
                                    .Skip(start)
                                    .Take(length)
                                    .Select(x => new { x.Id, x.Title, x.RawText, x.Summary, x.Imagename, x.IsFavorite, x.PublishDate, x.Priority, x.ContentType, HasGallery = (x.GalleryPosition != ContentGalleryPosition.None && x.ContentGalleries.Count > 0) })
                                    .Cacheable()
                                    .ToListAsync();

            return contents.Select(x => new ContentsViewModel { Id = x.Id, Title = x.Title, HasGallery = x.HasGallery, RawText = x.RawText, Summary = x.Summary, Imagename = x.Imagename, IsFavorite = x.IsFavorite, PublishDate = x.PublishDate, Priority = x.Priority, Language = language, ContentType = x.ContentType }).ToList();
        }

        public async Task<long> FavoritesCountAsync(string portalKey, ContentType? contentType, Language language = Language.FA)
        {
            var query = _content.Where(x => x.Portal.PortalKey == portalKey && x.Language == language && x.IsFavorite && x.IsActive).AsQueryable();

            if (contentType.HasValue)
            {
                query = query.Where(x => x.ContentType == contentType.Value);
            }

            var count = await query
                                .Cacheable()
                                .LongCountAsync()
                                .ConfigureAwait(false);

            return count;
        }

        public async Task<bool> IsExistContent(string portalKey, Language language = Language.FA, ContentType contentType = ContentType.News)
        {
            var isExist = await _content.Where(x => x.Portal.PortalKey == portalKey && x.Language == language && x.ContentType == contentType && x.IsActive).Cacheable().AnyAsync().ConfigureAwait(false);

            return isExist;
        }

        public async Task<bool> IsExistFavorite(string portalKey, ContentType? contentType, Language language = Language.FA)
        {
            var query = _content.Where(x => x.Portal.PortalKey == portalKey && x.Language == language && x.IsFavorite && x.IsActive).AsQueryable();

            if (contentType.HasValue)
            {
                query = query.Where(x => x.ContentType == contentType.Value);
            }

            var isExist = await query
                                    .Cacheable()
                                    .AnyAsync()
                                    .ConfigureAwait(false);

            return isExist;
        }

        public async Task<IList<ViewModels.ContentVisibilityViewModel>> CheckContentsVisibility(string portalKey, Language language)
        {
            var vm = new List<ViewModels.ContentVisibilityViewModel>();
            var contents = await _content
                .Where(x => x.Portal.PortalKey == portalKey && x.Language == language && x.IsActive)
                .GroupBy(x => x.ContentType)
                .Select(g => new { ContentType = g.Key, IsVisible = g.Any() })
                .Cacheable()
                .ToListAsync();

            // fill not existence enums with false IsVisible
            foreach (ContentType item in Enum.GetValues(typeof(ContentType)))
            {
                if (contents.Any(l => l.ContentType == item && l.IsVisible))
                {
                    vm.Add(new ViewModels.ContentVisibilityViewModel
                    {
                        ContentType = item,
                        IsVisible = true
                    });

                    continue;
                }

                vm.Add(new ViewModels.ContentVisibilityViewModel
                {
                    ContentType = item,
                    IsVisible = false
                });
            }

            foreach (var item in vm.ToList())
            {
                if (item.ContentType == ContentType.News ||
                    item.ContentType == ContentType.UpcomingEvent ||
                    item.ContentType == ContentType.Announcement)
                {
                    vm.Remove(item);
                }
            }

            return vm;
        }

        public async Task<IList<ContentsViewModel>> GetOtherContentsAsync(string portalKey, ContentType? contentType, Language language = Language.FA, int start = 0, int length = 10)
        {
            var query = _content.Where(x => x.Portal.PortalKey == portalKey && x.Language == language && x.IsActive).AsQueryable();

            if (contentType.HasValue)
            {
                query = query.Where(x => x.ContentType == contentType.Value);
            }
            else
            {
                query = query.Where(x =>
                                        x.ContentType == ContentType.Congress ||
                                        x.ContentType == ContentType.Regulation ||
                                        x.ContentType == ContentType.Appointment ||
                                        x.ContentType == ContentType.Research ||
                                        x.ContentType == ContentType.Journal ||
                                        x.ContentType == ContentType.Recall ||
                                        x.ContentType == ContentType.ResearchAndTechnology ||
                                        x.ContentType == ContentType.Financial ||
                                        x.ContentType == ContentType.VirtualLearning
                );
            }

            var contents = await query
                                    .OrderByDescending(x => x.Priority)
                                    .ThenByDescending(x => x.PublishDate)
                                    .Skip(start)
                                    .Take(length)
                                    .Select(x => new { x.Id, x.Title, x.RawText, x.Summary, x.Imagename, x.IsFavorite, x.PublishDate, x.Priority, x.ContentType, HasGallery = (x.GalleryPosition != ContentGalleryPosition.None && x.ContentGalleries.Count > 0) })
                                    .Cacheable()
                                    .ToListAsync();

            return contents.Select(x => new ContentsViewModel { Id = x.Id, Title = x.Title, RawText = x.RawText, Summary = x.Summary, Imagename = x.Imagename, IsFavorite = x.IsFavorite, PublishDate = x.PublishDate, Priority = x.Priority, Language = language, ContentType = x.ContentType, HasGallery = x.HasGallery }).ToList();
        }

        public async Task<long> OtherContentsCountAsync(string portalKey, ContentType? contentType, Language language = Language.FA)
        {
            var query = _content.Where(x => x.Portal.PortalKey == portalKey && x.Language == language && x.IsActive).AsQueryable();

            if (contentType.HasValue)
            {
                query = query.Where(x => x.ContentType == contentType.Value);
            }
            else
            {
                query = query.Where(x =>
                                        x.ContentType == ContentType.Congress ||
                                        x.ContentType == ContentType.Regulation ||
                                        x.ContentType == ContentType.Appointment ||
                                        x.ContentType == ContentType.Research ||
                                        x.ContentType == ContentType.Journal ||
                                        x.ContentType == ContentType.Recall ||
                                        x.ContentType == ContentType.ResearchAndTechnology ||
                                        x.ContentType == ContentType.Financial ||
                                        x.ContentType == ContentType.VirtualLearning
                );
            }

            var count = await query
                                .Cacheable()
                                .LongCountAsync()
                                .ConfigureAwait(false);

            return count;
        }

        public async Task<ViewModels.ContentViewModel> GetContentDetails(string portalKey, Language language, long id)
        {
            var content = await _content
                                        .Where(x => x.Id == id && x.Portal.PortalKey == portalKey && x.Language == language && x.IsActive)
                                        //.Select(x => new { x.Id, x.Title, x.Text, x.RawText, x.Summary, x.Imagename, x.PublishDate, x.ContentType, x.IsFavorite, x.ViewCount })
                                        //.Cacheable() // remove `Cacheable` because this prevent updating ViewCount
                                        .SingleOrDefaultAsync()
                                        .ConfigureAwait(false);

            if (content == null)
            {
                return null;
            }

            content.ViewCount++;
            await _uow.SaveChangesAsync().ConfigureAwait(false);

            return new ViewModels.ContentViewModel
            {
                Id = content.Id,
                Title = content.Title,
                Text = content.Text,
                RawText = content.RawText,
                Summary = content.Summary,
                Keywords = content.Keywords,
                ContentType = content.ContentType,
                Imagename = content.Imagename,
                Language = language,
                PublishDate = content.PublishDate,
                IsFavorite = content.IsFavorite,
                ViewCount = content.ViewCount,
                GalleryPosition = content.GalleryPosition
            };
        }

        public async Task<string> GetTitle(string portalKey, Language language, long id)
        {
            var title = await _content
                                        .Where(x => x.Id == id && x.Portal.PortalKey == portalKey && x.Language == language && x.IsActive)
                                        .Select(x => x.Title)
                                        .Cacheable()
                                        .SingleOrDefaultAsync()
                                        .ConfigureAwait(false);

            return title;
        }

        public async Task<IList<SearchAutoCompleteViewModel>> GetSearchAutoCompleteAsync(string portalKey, Language language, string searchQuery, int size = 15)
        {
            if (string.IsNullOrEmpty(searchQuery))
            {
                return null;
            }

            searchQuery = searchQuery.Trim().ToLowerInvariant();
            var contents = await _content
                                    .Where(x => x.Portal.PortalKey == portalKey && x.Language == language && x.IsActive && (x.Title.Contains(searchQuery) || x.RawText.Contains(searchQuery)))
                                    .OrderByDescending(x => x.PublishDate)
                                    .Skip(0)
                                    .Take(size)
                                    .Select(x => new { x.Id, x.Title, x.RawText, x.ContentType, x.Imagename })
                                    .Cacheable()
                                    .ToListAsync();

            return contents.Select(x => new SearchAutoCompleteViewModel { Id = x.Id, Title = x.Title, Text = x.RawText, ContentType = x.ContentType, Imagename = x.Imagename, Language = language }).ToList();
        }

        public async Task<IList<ContentsViewModel>> GetSearchResultsAsync(string portalKey, Language language, string searchQuery, int start = 0, int size = 15)
        {
            searchQuery = searchQuery.Trim().ToLowerInvariant();
            var contents = await _content
                                    .Where(x => x.Portal.PortalKey == portalKey && x.Language == language && x.IsActive && (x.Title.Contains(searchQuery) || x.RawText.Contains(searchQuery)))
                                    .OrderByDescending(x => x.PublishDate)
                                    .Skip(start)
                                    .Take(size)
                                    .Select(x => new { x.Id, x.Title, x.Summary, x.RawText, x.ContentType, x.Imagename, x.IsFavorite, x.PublishDate, x.Priority, HasGallery = (x.GalleryPosition != ContentGalleryPosition.None && x.ContentGalleries.Count > 0) })
                                    .Cacheable()
                                    .ToListAsync();

            return contents.Select(x => new ContentsViewModel { Id = x.Id, Title = x.Title, Summary = x.Summary, RawText = x.RawText, ContentType = x.ContentType, Imagename = x.Imagename, IsFavorite = x.IsFavorite, PublishDate = x.PublishDate, Priority = x.Priority, Language = language, HasGallery = x.HasGallery }).ToList();
        }

        public async Task<long> GetSearchResultsCountAsync(string portalKey, Language language, string searchQuery)
        {
            var count = await _content
                                    .Where(x => x.Portal.PortalKey == portalKey && x.Language == language && x.IsActive && (x.Title.Contains(searchQuery) || x.RawText.Contains(searchQuery)))
                                    .Cacheable()
                                    .LongCountAsync()
                                    .ConfigureAwait(false);

            return count;
        }

        public async Task<ContentGalleryPosition> GetGalleryPosition(long contentId)
        {
            var position = await _content.Where(x => x.Id == contentId).Select(x => x.GalleryPosition).Cacheable().SingleOrDefaultAsync().ConfigureAwait(false);
            return position;
        }

        public async Task UpdateGalleryPosition(long contentId, ContentGalleryPosition newPosition)
        {
            var content = await FindContentByIdAsync(contentId).ConfigureAwait(false);
            content.GalleryPosition = newPosition;

            await _uow.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<bool> HasGallery(long contentId)
        {
            var hasGallery = await _content.AnyAsync(x => x.Id == contentId && x.GalleryPosition != ContentGalleryPosition.None && x.ContentGalleries.Count > 0).ConfigureAwait(false);
            return hasGallery;
        }

        public async Task<IList<RssViewModel>> GetRssResult(string portalKey, ContentType? contentType, int size = 20)
        {
            var items = new List<RssViewModel>();
            var query = _content.Where(x => x.Portal.PortalKey == portalKey).AsQueryable();

            if (contentType.HasValue)
            {
                query = query.Where(x => x.ContentType == contentType.Value);
            }

            var contents = await query
                .OrderByDescending(x => x.PublishDate)
                .Take(size)
                .Select(x => new { x.Id, x.Title, x.Text, x.ContentType, x.PublishDate, x.Language })
                .Cacheable()
                .ToListAsync()
                .ConfigureAwait(false);

            foreach (var item in contents)
            {
                items.Add(new RssViewModel
                {
                    Id = item.Id,
                    Title = item.Title,
                    Categories = new List<string> { item.ContentType.ToString() },
                    Content = item.Text,
                    PublishDate = item.PublishDate,
                    Language = item.Language
                });
            }

            //foreach (var item in contents)
            //{
            //    items.Add(new FeedItem {
            //        Title = item.Title,
            //        AuthorName = $"{item.TitleFa} | {item.TitleEn}",
            //        Categories = new List<string> { item.ContentType.ToString() },
            //        Content = item.Text,
            //        Url = Url.Action(nameof(Article), typeof(FeedResultController).ControllerName(), new { id = 1 }, this.HttpContext.Request.Scheme),
            //        PublishDate = item.PublishDate,
            //        LastUpdatedTime = item.PublishDate
            //    });
            //}

            return items;
        }

        public async Task<bool> IsExistContent(string portalKey, Language language = Language.FA)
        {
            var isExist = await _content.Where(x => x.Portal.PortalKey == portalKey && x.Language == language && x.IsActive).Cacheable().AnyAsync().ConfigureAwait(false);
            return isExist;
        }

        public async Task<IList<ContentsViewModel>> GetMostViewedContentsAsync(string portalKey, Language language = Language.FA, int size = 10)
        {
            var contents = await _content.Where(x => x.Portal.PortalKey == portalKey && x.Language == language && x.IsActive)
                        .OrderByDescending(x => x.ViewCount)
                        .ThenByDescending(x => x.PublishDate)
                        .Take(size)
                        .Select(x => new { x.Id, x.Title, x.RawText, x.Summary, x.Imagename, x.IsFavorite, x.PublishDate, x.Priority, x.ContentType, HasGallery = (x.GalleryPosition != ContentGalleryPosition.None && x.ContentGalleries.Count > 0) })
                        .Cacheable()
                        .ToListAsync();

            return contents.Select(x => new ContentsViewModel { Id = x.Id, Title = x.Title, RawText = x.RawText, HasGallery = x.HasGallery, Summary = x.Summary, Imagename = x.Imagename, IsFavorite = x.IsFavorite, PublishDate = x.PublishDate, Priority = x.Priority, Language = language, ContentType = x.ContentType }).ToList();
        }
    }
}
