using ContentManagement.Entities;
using ContentManagement.ViewModels.Areas.Manage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ContentManagement.Services.Contracts
{
    public interface INavbarService
    {
        Task AddOrUpdateNavbarAsync(NavbarViewModel navbar);
        Task<Navbar> FindNavbarByIdAsync(long navbarId);
        Task<IList<NavbarViewModel>> GetPagedNavbarsAsync(int portalId, Language language = Language.FA, string searchTerm = null, int start = 0, int length = 10);
        Task<long> NavbarCountAsync();
        Task<long> NavbarPagedCountAsync(int portalId, Language language = Language.FA, string searchTerm = null);
        Task DeleteNavbarAsync(long id);
        Task<Navbar> GetParentOfNavbarAsync(long parentId);
    }
}
