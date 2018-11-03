using ContentManagement.Entities;
using ContentManagement.ViewModels;
using ContentManagement.ViewModels.Areas.Manage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ContentManagement.Services.Contracts
{
    public interface IPortalService
    {
        Task<bool> ValidatePortalKeyAsync(string portalKey);
        Task<int> AddOrUpdatePortalAsync(PortalViewModel portal);
        Task<Portal> FindPortalByKeyAsync(string portalKey);
        Task<Portal> FindPortalByIdAsync(int portalId);
        Task<IList<PortalViewModel>> GetAllPortalsAsync();
        Task DeletePortalAsync(int id);
        Task<long> PortalsCountAsync();
        Task<string> GetPortalTitleAsync(string portalKey, Language language);
        Task<SeoViewModel> GetPortalSeoInfo(string portalKey, Language language);
        Task<IList<PortalKeyViewModel>> GetPortalsKeyAsync();
    }
}
