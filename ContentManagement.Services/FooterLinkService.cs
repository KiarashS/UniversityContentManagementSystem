namespace ContentManagement.Services
{
    using ContentManagement.Common.GuardToolkit;
    using ContentManagement.DataLayer.Context;
    using ContentManagement.Entities;
    using ContentManagement.Services.Contracts;
    using ContentManagement.ViewModels.Areas.Manage;
    using EFSecondLevelCache.Core;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class FooterLinkService : IFooterLinkService
    {
        private readonly IUnitOfWork _uow;
        private readonly DbSet<FooterLink> _link;

        public FooterLinkService(IUnitOfWork uow)
        {
            _uow = uow;
            _uow.CheckArgumentIsNull(nameof(uow));

            _link = _uow.Set<FooterLink>();
        }

        public async Task AddOrUpdateLinkAsync(long sectionId, FooterLinkViewModel link)
        {
            if (link.Id == 0) // Add
            {
                var newLink = new FooterLink
                {
                    FooterSectionId = sectionId,
                    Text = link.Text?.Trim(),
                    Url = link.Url?.Trim(),
                    IsBlankUrlTarget = link.IsBlankUrlTarget,
                    Priority = link.Priority,
                };

                _link.Add(newLink);
                await _uow.SaveChangesAsync().ConfigureAwait(false);
                return;
            }
            var currentLink = await FindLinkByIdAsync(sectionId, link.Id).ConfigureAwait(false);

            currentLink.Text = link.Text?.Trim();
            currentLink.Url = link.Url?.Trim();
            currentLink.IsBlankUrlTarget = link.IsBlankUrlTarget;
            currentLink.Priority = link.Priority;

            await _uow.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task DeleteLinkAsync(long sectionId, long id)
        {
            var link = new FooterLink { Id = id, FooterSectionId = sectionId };

            _uow.Entry(link).State = EntityState.Deleted;

            await _uow.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<FooterLink> FindLinkByIdAsync(long sectionId, long linkId)
        {
            var link = await _link.FirstOrDefaultAsync(x => x.Id == linkId && x.FooterSectionId == sectionId);
            return link;
        }

        public async Task<IList<ViewModels.FooterLinkViewModel>> GetLinksOfSectionsAsync(IList<long> sectionsId, int maxLinksSize = 6)
        {
            var links = await _link
                        .Where(x => sectionsId.Contains(x.FooterSectionId))
                        .OrderByDescending(x => x.Priority)
                        .ThenByDescending(x => x.Id)
                        .GroupBy(x => x.FooterSectionId)
                        .SelectMany(s => s.Take(maxLinksSize))
                        .Cacheable()
                        .ToListAsync();

            return links.Select(x => new ViewModels.FooterLinkViewModel { Id = x.Id, FooterSectionId = x.FooterSectionId, Text = x.Text, Url = x.Url, IsBlankUrlTarget = x.IsBlankUrlTarget, Priority = x.Priority }).ToList();
        }

        public async Task<IList<FooterLinkViewModel>> GetPagedLinksAsync(long sectionId)
        {
            var links = await _link
                                    .Where(x => x.FooterSectionId == sectionId)
                                    .OrderByDescending(x => x.Priority)
                                    .ThenByDescending(x => x.Id)
                                    .Select(x => new { x.Id, x.Text, x.Url, x.IsBlankUrlTarget, x.Priority })
                                    .Cacheable()
                                    .ToListAsync();

            return links.Select(x => new FooterLinkViewModel { Id = x.Id, Text = x.Text, Url = x.Url, IsBlankUrlTarget = x.IsBlankUrlTarget, Priority = x.Priority }).ToList();
        }

        public async Task<long> LinksCountAsync(long sectionId)
        {
            var link = await _link.LongCountAsync(x => x.FooterSectionId == sectionId).ConfigureAwait(false);
            return link;
        }

        public async Task<long> LinksPagedCountAsync(long sectionId, string searchTerm = null)
        {
            var query = _link.Where(x => x.FooterSectionId == sectionId).AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.Trim();
                query = query.Where(x => x.Text.Contains(searchTerm) || x.Url.Contains(searchTerm));
            }

            var count = await query.LongCountAsync().ConfigureAwait(false);
            return count;
        }
    }
}
