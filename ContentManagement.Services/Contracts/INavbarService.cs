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
        Task<bool> ValidatePortalKeyAsync(string portalKey);
        Task AddOrUpdatePortalAsync(PortalViewModel portal);
        Task<Portal> FindPortalByKeyAsync(string portalKey);
        Task<Portal> FindPortalByIdAsync(int portalId);
        Task<IList<PortalViewModel>> GetAllPortalsAsync();
        Task DeletePortalAsync(int id);
    }
}
