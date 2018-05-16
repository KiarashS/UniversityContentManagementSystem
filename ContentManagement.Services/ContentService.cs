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

        //public async Task AddOrUpdateLinkAsync(LinkViewModel link)
        //{
        //    if (link.Id == 0) // Add
        //    {
        //        var newLink = new Link
        //        {
        //            Text = link.Text,
        //            Url = link.Url,
        //            Icon = link.Icon,
        //            IconColor = link.IconColor,
        //            IsBlankUrlTarget = link.IsBlankUrlTarget,
        //            LinkType = link.LinkType,
        //            Priority = link.Priority,
        //            Language = link.Language,
        //            PortalId = link.PortalId
        //        };

        //        _content.Add(newLink);
        //        await _uow.SaveChangesAsync().ConfigureAwait(false);
        //        return;
        //    }
        //    var currentLink = await FindLinkByIdAsync(link.Id).ConfigureAwait(false);

        //    currentLink.Text = link.Text;
        //    currentLink.Url = link.Url;
        //    currentLink.Icon = link.Icon;
        //    currentLink.IconColor = link.IconColor;
        //    currentLink.IsBlankUrlTarget = link.IsBlankUrlTarget;
        //    currentLink.LinkType = link.LinkType;
        //    currentLink.Priority = link.Priority;
        //    currentLink.Language = link.Language;
        //    currentLink.PortalId = link.PortalId;

        //    await _uow.SaveChangesAsync().ConfigureAwait(false);
        //}

        //public async Task DeleteLinkAsync(long id)
        //{
        //    var link = new Link { Id = id };

        //    _uow.Entry(link).State = EntityState.Deleted;

        //    await _uow.SaveChangesAsync().ConfigureAwait(false);
        //}

        //public async Task<Link> FindLinkByIdAsync(long linkId)
        //{
        //    var link = await _content.FirstOrDefaultAsync(x => x.Id == linkId);
        //    return link;
        //}

        //public async Task<IList<LinkViewModel>> GetPagedLinksAsync(int portalId, LinkType? linkType, Language language = Language.FA, string searchTerm = null, int start = 0, int length = 10)
        //{
        //    var query = _content.Where(x => x.PortalId == portalId && x.Language == language).AsQueryable();

        //    if (linkType.HasValue)
        //    {
        //        query = query.Where(x => x.LinkType == linkType);
        //    }

        //    if (!string.IsNullOrEmpty(searchTerm))
        //    {
        //        searchTerm = searchTerm.Trim();
        //        query = query.Where(x => x.Text.Contains(searchTerm) || x.Url.Contains(searchTerm));
        //    }

        //    var links = await query
        //                            .OrderByDescending(x => x.Priority)
        //                            .ThenByDescending(x => x.Id)
        //                            .Skip(start)
        //                            .Take(length)
        //                            .Cacheable()
        //                            .Select(x => new { x.Id, x.Text, x.Url, x.Icon, x.IconColor, x.IsBlankUrlTarget, x.LinkType, x.Priority })
        //                            .ToListAsync();

        //    return links.Select(x => new LinkViewModel { Id = x.Id, Text = x.Text, Url = x.Url, Icon = x.Icon, IconColor = x.IconColor, IsBlankUrlTarget = x.IsBlankUrlTarget, LinkType = x.LinkType, Priority = x.Priority }).ToList();
        //}

        public async Task<long> ContentsCountAsync()
        {
            var content = await _content.LongCountAsync().ConfigureAwait(false);
            return content;
        }

        //public async Task<long> LinksPagedCountAsync(int portalId, LinkType? linkType, Language language = Language.FA, string searchTerm = null)
        //{
        //    var query = _content.Where(x => x.PortalId == portalId && x.Language == language).AsQueryable();

        //    if (linkType.HasValue)
        //    {
        //        query = query.Where(x => x.LinkType == linkType);
        //    }

        //    if (!string.IsNullOrEmpty(searchTerm))
        //    {
        //        searchTerm = searchTerm.Trim();
        //        query = query.Where(x => x.Text.Contains(searchTerm) || x.Url.Contains(searchTerm));
        //    }

        //    var count = await query.LongCountAsync().ConfigureAwait(false);
        //    return count;
        //}
    }
}
