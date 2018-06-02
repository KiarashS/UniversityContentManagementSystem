using System.Linq;
using System.Collections.Generic;
using ContentManagement.DataLayer.Context;
using ContentManagement.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using ContentManagement.Services.Contracts;
using ContentManagement.Common.GuardToolkit;
using ContentManagement.ViewModels.Areas.Manage;
using EFSecondLevelCache.Core;

namespace ContentManagement.Services
{
    public class PortalService : IPortalService
    {
        private readonly IUnitOfWork _uow;
        private readonly DbSet<Portal> _portal;

        public PortalService(IUnitOfWork uow)
        {
            _uow = uow;
            _uow.CheckArgumentIsNull(nameof(uow));

            _portal = _uow.Set<Portal>();
        }

        public async Task AddOrUpdatePortalAsync(PortalViewModel portal)
        {
            if (portal.Id == 0) // Add
            {
                var newPortal = new Portal
                {
                    PortalKey = portal.PortalKey?.Trim(),
                    TitleFa = portal.TitleFa?.Trim(),
                    DescriptionFa = portal.DescriptionFa?.Trim(),
                    TitleEn = portal.TitleEn?.Trim(),
                    DescriptionEn = portal.DescriptionEn?.Trim(),
                    ShowInMainPortal = portal.ShowInMainPortal
                };

                _portal.Add(newPortal);
                await _uow.SaveChangesAsync().ConfigureAwait(false);
                return;
            }
            
            var currentPortal = await FindPortalByIdAsync(portal.Id).ConfigureAwait(false);

            if (currentPortal.PortalKey != null) // Do not manipulate the main portal
            {
                currentPortal.PortalKey = portal.PortalKey?.Trim();
                currentPortal.ShowInMainPortal = portal.ShowInMainPortal;
            }
            currentPortal.TitleFa = portal.TitleFa?.Trim();
            currentPortal.DescriptionFa = portal.DescriptionFa?.Trim();
            currentPortal.TitleEn = portal.TitleEn?.Trim();
            currentPortal.DescriptionEn = portal.DescriptionEn?.Trim();

            await _uow.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<Portal> FindPortalByKeyAsync(string portalKey)
        {
            var portal = await _portal.FirstOrDefaultAsync(p => p.PortalKey == portalKey.Trim()).ConfigureAwait(false);
            return portal;
        }

        public async Task<Portal> FindPortalByIdAsync(int portalId)
        {
            var portal = await _portal.FirstOrDefaultAsync(p => p.Id == portalId).ConfigureAwait(false);
            return portal;
        }

        public async Task<bool> ValidatePortalKeyAsync(string portalKey)
        {
            var isExist = await _portal.Where(p => p.PortalKey == portalKey.Trim()).Cacheable().AnyAsync().ConfigureAwait(false);
            return isExist;
        }

        public async Task<IList<PortalViewModel>> GetAllPortalsAsync()
        {
            var portals = await _portal.Cacheable().Select(p => new { p.Id, p.PortalKey, p.TitleFa, p.DescriptionFa, p.ShowInMainPortal }).ToListAsync().ConfigureAwait(false);
            var portalsViewModel = new List<PortalViewModel>();

            foreach (var item in portals)
            {
                portalsViewModel.Add(new PortalViewModel
                {
                    Id = item.Id,
                    PortalKey = item.PortalKey,
                    TitleFa = item.TitleFa,
                    DescriptionFa = item.DescriptionFa,
                    ShowInMainPortal = item.ShowInMainPortal
                });
            }

            return portalsViewModel;
        }

        public async Task DeletePortalAsync(int id)
        {
            var portal = new Portal { Id = id };

            _uow.Entry(portal).State = EntityState.Deleted;

            await _uow.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<long> PortalsCountAsync()
        {
            var portal = await _portal.LongCountAsync().ConfigureAwait(false);
            return portal;
        }

        public async Task<string> GetPortalTitleAsync(string portalKey, Language language)
        {
            IQueryable<string> query;

            switch (language)
            {
                case Language.FA:
                default:
                    query = _portal.Where(x => x.PortalKey == portalKey).Select(x => x.TitleFa);
                    break;
                case Language.EN:
                    query = _portal.Where(x => x.PortalKey == portalKey).Select(x => x.TitleEn);
                    break;
            }

            var title = await query.Cacheable().FirstOrDefaultAsync();

            return title;
        }
    }
}
