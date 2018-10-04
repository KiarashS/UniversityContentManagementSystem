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

    public class LinkService : ILinkService
    {
        private readonly IUnitOfWork _uow;
        private readonly DbSet<Link> _link;

        public LinkService(IUnitOfWork uow)
        {
            _uow = uow;
            _uow.CheckArgumentIsNull(nameof(uow));

            _link = _uow.Set<Link>();
        }

        public async Task AddOrUpdateLinkAsync(LinkViewModel link)
        {
            if (link.Id == 0) // Add
            {
                var newLink = new Link
                {
                    Text = link.Text?.Trim(),
                    Url = link.Url?.Trim(),
                    Icon = link.Icon,
                    IconColor = link.IconColor,
                    IsBlankUrlTarget = link.IsBlankUrlTarget,
                    LinkType = link.LinkType,
                    Priority = link.Priority,
                    Language = link.Language,
                    PortalId = link.PortalId
                };

                _link.Add(newLink);
                await _uow.SaveChangesAsync().ConfigureAwait(false);
                return;
            }
            var currentLink = await FindLinkByIdAsync(link.Id).ConfigureAwait(false);

            currentLink.Text = link.Text?.Trim();
            currentLink.Url = link.Url?.Trim();
            currentLink.Icon = link.Icon;
            currentLink.IconColor = link.IconColor;
            currentLink.IsBlankUrlTarget = link.IsBlankUrlTarget;
            currentLink.LinkType = link.LinkType;
            currentLink.Priority = link.Priority;
            currentLink.Language = link.Language;
            currentLink.PortalId = link.PortalId;

            await _uow.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task DeleteLinkAsync(long id)
        {
            var link = new Link { Id = id };

            _uow.Entry(link).State = EntityState.Deleted;

            await _uow.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<Link> FindLinkByIdAsync(long linkId)
        {
            var link = await _link.FirstOrDefaultAsync(x => x.Id == linkId);
            return link;
        }

        public async Task<IList<LinkViewModel>> GetPagedLinksAsync(int portalId, LinkType? linkType, Language language = Language.FA, string searchTerm = null, int start = 0, int length = 10)
        {
            var query = _link.Where(x => x.PortalId == portalId && x.Language == language).AsQueryable();

            if (linkType.HasValue)
            {
                query = query.Where(x => x.LinkType == linkType);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.Trim();
                query = query.Where(x => x.Text.Contains(searchTerm) || x.Url.Contains(searchTerm));
            }

            var links = await query
                                    .OrderByDescending(x => x.Priority)
                                    .ThenByDescending(x => x.Id)
                                    .Skip(start)
                                    .Take(length)
                                    .Cacheable()
                                    .Select(x => new { x.Id, x.Text, x.Url, x.Icon, x.IconColor, x.IsBlankUrlTarget, x.LinkType, x.Priority })
                                    .ToListAsync();

            return links.Select(x => new LinkViewModel { Id = x.Id, Text = x.Text, Url = x.Url, Icon = x.Icon, IconColor = x.IconColor, IsBlankUrlTarget = x.IsBlankUrlTarget, LinkType = x.LinkType, Priority = x.Priority }).ToList();
        }

        public async Task<IList<ViewModels.LinkViewModel>> GetLinksAsync(string portalKey, Language language, LinkType linkType, int maxSize = 6)
        {
            var links = await 
                _link
                .Where(x => x.Portal.PortalKey == portalKey && x.Language == language && x.LinkType == linkType)
                .Select(x => new { x.Id, x.Url, x.Text, x.Icon, x.IconColor, x.LinkType, x.IsBlankUrlTarget, x.Priority })
                .OrderByDescending(x => x.Priority)
                .ThenByDescending(x => x.Id)
                .Take(maxSize)
                .Cacheable()
                .ToListAsync();

            var linkViewModel = new List<ViewModels.LinkViewModel>();

            foreach (var item in links)
            {
                linkViewModel.Add(new ViewModels.LinkViewModel
                {
                    Id = item.Id,
                    Url = item.Url,
                    Text = item.Text,
                    Icon = item.Icon,
                    IconColor = item.IconColor,
                    LinkType = item.LinkType,
                    IsBlankUrlTarget = item.IsBlankUrlTarget,
                    Priority = item.Priority
                });
            }

            return linkViewModel;
        }

        public async Task<long> LinksCountAsync()
        {
            var link = await _link.LongCountAsync().ConfigureAwait(false);
            return link;
        }

        public async Task<long> LinksPagedCountAsync(int portalId, LinkType? linkType, Language language = Language.FA, string searchTerm = null)
        {
            var query = _link.Where(x => x.PortalId == portalId && x.Language == language).AsQueryable();

            if (linkType.HasValue)
            {
                query = query.Where(x => x.LinkType == linkType);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.Trim();
                query = query.Where(x => x.Text.Contains(searchTerm) || x.Url.Contains(searchTerm));
            }

            var count = await query.LongCountAsync().ConfigureAwait(false);
            return count;
        }

        public async Task<IList<ViewModels.LinkVisibilityViewModel>> CheckLinksVisibility(string portalKey, Language language)
        {
            var vm = new List<ViewModels.LinkVisibilityViewModel>();
            var links = await _link
                .Where(x => x.Portal.PortalKey == portalKey && x.Language == language)
                //.Where(predicate)
                .GroupBy(x => x.LinkType)
                .Select(g => new { LinkType = g.Key, IsVisible = g.Any() })
                .Cacheable()
                .ToListAsync();

            // fill not existence enums with false IsVisible
            foreach (LinkType item in Enum.GetValues(typeof(LinkType)))
            {
                if (links.Any(l => l.LinkType == item && l.IsVisible))
                {
                    vm.Add(new ViewModels.LinkVisibilityViewModel {
                        LinkType = item,
                        IsVisible = true
                    });

                    continue;
                }

                vm.Add(new ViewModels.LinkVisibilityViewModel
                {
                    LinkType = item,
                    IsVisible = false
                });
            }

            return vm;
        }

        public async Task<bool> HasLink(string portalKey, Language language, LinkType linkType)
        {
            var hasLink = await _link.AnyAsync(x => x.Portal.PortalKey == portalKey && x.Language == language && x.LinkType == linkType).ConfigureAwait(false);
            return hasLink;
        }
    }
}
