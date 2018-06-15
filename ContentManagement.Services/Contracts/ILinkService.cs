using ContentManagement.Entities;
using ContentManagement.ViewModels.Areas.Manage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ContentManagement.Services.Contracts
{
    public interface ILinkService
    {
        Task AddOrUpdateLinkAsync(LinkViewModel link);
        Task<Link> FindLinkByIdAsync(long linkId);
        Task<IList<LinkViewModel>> GetPagedLinksAsync(int portalId, LinkType? linkType, Language language = Language.FA, string searchTerm = null, int start = 0, int length = 10);
        Task<long> LinksCountAsync();
        Task<long> LinksPagedCountAsync(int portalId, LinkType? linkType, Language language = Language.FA, string searchTerm = null);
        Task DeleteLinkAsync(long id);
        Task<IList<ViewModels.LinkViewModel>> GetLinksAsync(string portalKey, Language language, LinkType linkType, int maxSize = 6);
        Task<IList<ViewModels.LinkVisibilityViewModel>> CheckLinksVisibility(string portalKey, Language language);
    }
}
