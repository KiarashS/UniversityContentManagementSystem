using ContentManagement.Entities;
using ContentManagement.ViewModels.Areas.Manage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ContentManagement.Services.Contracts
{
    public interface IFooterSectionService
    {
        Task AddOrUpdateSectionAsync(FooterSectionViewModel section);
        Task<FooterSection> FindSectionByIdAsync(long sectionId);
        Task<IList<FooterSectionViewModel>> GetPagedSectionsAsync(int portalId, Language language = Language.FA, string searchTerm = null, int start = 0, int length = 10);
        Task<long> SectionsCountAsync();
        Task<long> SectionsPagedCountAsync(int portalId, Language language = Language.FA, string searchTerm = null);
        Task DeleteSectionAsync(long id);
        Task<IList<ViewModels.FooterSectionViewModel>> GetSectionAndLinksAsync(string portalKey, Language language, int maxSectionSize = 4, int maxLinkSize = 6);
    }
}
