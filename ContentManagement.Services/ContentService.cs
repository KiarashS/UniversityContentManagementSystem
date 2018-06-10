namespace ContentManagement.Services
{
    using ContentManagement.Common.GuardToolkit;
    using ContentManagement.Common.WebToolkit;
    using ContentManagement.DataLayer.Context;
    using ContentManagement.Entities;
    using ContentManagement.Services.Contracts;
    using ContentManagement.ViewModels;
    using ContentManagement.ViewModels.Areas.Manage;
    using EFSecondLevelCache.Core;
    using Microsoft.EntityFrameworkCore;
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

        public async Task AddOrUpdateContentAsync(ContentViewModel content)
        {
            if (content.Id == 0) // Add
            {
                var newContent = new Content
                {
                    Title = content.Title?.Trim(),
                    Text = content.Text,
                    RawText = content.RawText,
                    Summary = content.Summary?.Trim(),
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

        public async Task<IList<ContentViewModel>> GetPagedContentsAsync(int portalId, ContentType? contentType, Language language = Language.FA, string searchTerm = null, int start = 0, int length = 10)
        {
            var query = _content.Where(x => x.PortalId == portalId && x.Language == language).AsQueryable();

            if (contentType.HasValue)
            {
                query = query.Where(x => x.ContentType == contentType);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.Trim();
                query = query.Where(x => x.Text.Contains(searchTerm) || x.RawText.Contains(searchTerm));
            }

            var links = await query
                                    .OrderByDescending(x => x.Priority)
                                    .ThenByDescending(x => x.PublishDate)
                                    .Skip(start)
                                    .Take(length)
                                    .Cacheable()
                                    .Select(x => new { x.Id, x.Title, x.Text, x.Summary, x.Imagename, x.PublishDate, x.IsActive, x.IsFavorite, x.Priority, x.ViewCount, x.Portal.PortalKey })
                                    .ToListAsync();

            return links.Select(x => new ContentViewModel(null) { Id = x.Id, Title = x.Title, Text = x.Text, Summary = x.Summary, Imagename = x.Imagename, PublishDate = x.PublishDate, IsActive = x.IsActive, IsFavorite = x.IsFavorite, ViewCount = x.ViewCount, Priority = x.Priority, PortalKey = x.PortalKey }).ToList();
        }

        public async Task<long> ContentsCountAsync()
        {
            var content = await _content.LongCountAsync().ConfigureAwait(false);
            return content;
        }

        public async Task<long> ContentsPagedCountAsync(int portalId, ContentType? contentType, Language language = Language.FA, string searchTerm = null)
        {
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
                                        .Where(x => x.Language == language && x.Portal.PortalKey != null && x.Portal.ShowInMainPortal)
                                        .OrderByDescending(x => x.Priority)
                                        .ThenByDescending(x => x.PublishDate).AsQueryable()
                                        .Select(x => new { x.Id, x.Title, x.ContentType, x.PublishDate, x.Portal.PortalKey, PortalTitle = (language == Language.EN ? x.Portal.TitleEn : x.Portal.TitleFa) })
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
                    Title = item.Title,
                    PublishDate = item.PublishDate
                });
            }

            return subPortalsContentsViewModel;
        }
    }
}
