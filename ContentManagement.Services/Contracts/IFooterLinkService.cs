using ContentManagement.Entities;
using ContentManagement.ViewModels.Areas.Manage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ContentManagement.Services.Contracts
{
    public interface IFooterLinkService
    {
        Task AddOrUpdateLinkAsync(long sectionId, FooterLinkViewModel link);
        Task<FooterLink> FindLinkByIdAsync(long sectionId, long linkId);
        Task<IList<FooterLinkViewModel>> GetPagedLinksAsync(long sectionId);
        Task<long> LinksCountAsync(long sectionId);
        Task<long> LinksPagedCountAsync(long sectionId, string searchTerm = null);
        Task DeleteLinkAsync(long sectionId, long id);
        Task<IList<ViewModels.FooterLinkViewModel>> GetLinksOfSectionsAsync(IList<long> sectionsId, int maxLinksSize = 6);
    }
}
