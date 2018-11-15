namespace ContentManagement.Services
{
    using ContentManagement.Common.GuardToolkit;
    using ContentManagement.Common.WebToolkit;
    using ContentManagement.DataLayer.Context;
    using ContentManagement.Entities;
    using ContentManagement.Services.Contracts;
    using ContentManagement.ViewModels.Areas.Manage;
    using EFSecondLevelCache.Core;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class PageService : IPageService
    {
        private readonly IUnitOfWork _uow;
        private readonly DbSet<Page> _page;

        public PageService(IUnitOfWork uow)
        {
            _uow = uow;
            _uow.CheckArgumentIsNull(nameof(uow));

            _page = _uow.Set<Page>();
        }

        public async Task AddOrUpdatePageAsync(PageViewModel page)
        {
            if (page.Id == 0) // Add
            {
                var newPage = new Page
                {
                    Title = page.Title?.Trim(),
                    Text = page.Text,
                    RawText = page.RawText,
                    Slug = page.Slug.GenerateSlug(),
                    Keywords = page.Keywords?.Trim(),
                    IsActive = page.IsActive,
                    Imagename = page.Imagename,
                    Language = page.Language,
                    PortalId = page.PortalId
                };

                _page.Add(newPage);
                await _uow.SaveChangesAsync().ConfigureAwait(false);
                return;
            }
            var currentPage = await FindPageByIdAsync(page.Id).ConfigureAwait(false);

            currentPage.Title = page.Title?.Trim();
            currentPage.Text = page.Text;
            currentPage.RawText = page.RawText;
            currentPage.Slug = page.Slug.GenerateSlug();
            currentPage.Keywords = page.Keywords?.Trim();
            currentPage.IsActive = page.IsActive;
            currentPage.Imagename = page.Imagename;
            currentPage.Language = page.Language;
            currentPage.PortalId = page.PortalId;

            await _uow.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task DeletePageAsync(long id)
        {
            var page = new Page { Id = id };

            _uow.Entry(page).State = EntityState.Deleted;

            await _uow.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<Page> FindPageByIdAsync(long pageId)
        {
            var page = await _page.FirstOrDefaultAsync(x => x.Id == pageId);
            return page;
        }

        public async Task<ViewModels.PageViewModel> GetPageDetails(string portalKey, Language language, string slug)
        {
            slug = slug.Trim();
            var page = await _page
                                .Where(x => x.IsActive && x.Slug == slug && x.Portal.PortalKey == portalKey && x.Language == language)
                                //.Cacheable() // remove `Cacheable` because this prevent updating ViewCount
                                .SingleOrDefaultAsync();

            if (page == null)
            {
                return null;
            }

            page.ViewCount++;
            await _uow.SaveChangesAsync().ConfigureAwait(false);

            return new ViewModels.PageViewModel
            {
                Id = page.Id,
                Slug = page.Slug,
                Title = page.Title,
                Text = page.Text,
                Keywords = page.Keywords,
                RawText = page.RawText,
                Imagename = page.Imagename,
                Language = page.Language,
                PublishDate = page.PublishDate,
                ViewCount = page.ViewCount
            };
        }

        public async Task<IList<PageViewModel>> GetPagedPagesAsync(int portalId, Language language = Language.FA, string searchTerm = null, int start = 0, int length = 10)
        {
            var query = _page.Where(x => x.PortalId == portalId && x.Language == language).AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.Trim();
                query = query.Where(x => x.Title.Contains(searchTerm) || x.RawText.Contains(searchTerm));
            }

            var pages = await query
                                    .OrderByDescending(x => x.PublishDate)
                                    .Skip(start)
                                    .Take(length)
                                    .Cacheable()
                                    .Select(x => new { x.Id, x.Title, x.Slug, x.Imagename, x.PublishDate, x.IsActive, x.ViewCount, x.Portal.PortalKey })
                                    .ToListAsync();

            return pages.Select(x => new PageViewModel { Id = x.Id, Title = x.Title, Slug = x.Slug, Imagename = x.Imagename, PublishDate = x.PublishDate, IsActive = x.IsActive, ViewCount = x.ViewCount, PortalKey = x.PortalKey }).ToList();
        }

        public Task<string> GetPageImagenameAsync(long id)
        {
            var imagename = _page.Where(x => x.Id == id).Select(x => x.Imagename).SingleOrDefaultAsync();
            return imagename;
        }

        public async Task<long> PagesCountAsync()
        {
            var count = await _page.LongCountAsync().ConfigureAwait(false);
            return count;
        }

        public async Task<long> PagesCountAsync(string portalKey)
        {
            var count = await _page.LongCountAsync(x => x.Portal.PortalKey == portalKey).ConfigureAwait(false);
            return count;
        }

        public async Task<long> PagesPagedCountAsync(int portalId, Language language = Language.FA, string searchTerm = null)
        {
            var query = _page.Where(x => x.PortalId == portalId && x.Language == language).AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.Trim();
                query = query.Where(x => x.Title.Contains(searchTerm) || x.RawText.Contains(searchTerm));
            }

            var count = await query.LongCountAsync().ConfigureAwait(false);
            return count;
        }

        public Task<bool> ValidatePageSlugAsync(string slug)
        {
            return _page.AnyAsync(p => p.Slug == slug.Trim());
        }
    }
}
