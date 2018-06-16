using ContentManagement.Entities;
using ContentManagement.ViewModels.Areas.Manage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ContentManagement.Services.Contracts
{
    public interface IPageService
    {
        Task AddOrUpdatePageAsync(PageViewModel page);
        Task<Page> FindPageByIdAsync(long pageId);
        Task<IList<PageViewModel>> GetPagedPagesAsync(int portalId, Language language = Language.FA, string searchTerm = null, int start = 0, int length = 10);
        Task<long> PagesCountAsync();
        Task<long> PagesPagedCountAsync(int portalId, Language language = Language.FA, string searchTerm = null);
        Task DeletePageAsync(long id);
        Task<string> GetPageImagenameAsync(long id);
        Task<bool> ValidatePageSlugAsync(string slug);
        Task<ViewModels.PageViewModel> GetPageDetails(string portalKey, Language language, string slug);
    }
}
