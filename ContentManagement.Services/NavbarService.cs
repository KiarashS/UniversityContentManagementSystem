//using System.Linq;
//using System.Collections.Generic;
//using ContentManagement.DataLayer.Context;
//using ContentManagement.Entities;
//using Microsoft.EntityFrameworkCore;
//using System.Threading.Tasks;
//using ContentManagement.Services.Contracts;
//using ContentManagement.Common.GuardToolkit;
//using ContentManagement.ViewModels.Areas.Manage;
//using EFSecondLevelCache.Core;

//namespace ContentManagement.Services
//{
//    public class NavbarService : INavbarService
//    {
//        private readonly IUnitOfWork _uow;
//        private readonly DbSet<Navbar> _portal;

//        public NavbarService(IUnitOfWork uow)
//        {
//            _uow = uow;
//            _uow.CheckArgumentIsNull(nameof(uow));

//            _portal = _uow.Set<Navbar>();
//        }

//        public async Task AddOrUpdatePortalAsync(PortalViewModel portal)
//        {
//            if (portal.Id == 0) // Add
//            {
//                var newPortal = new Portal
//                {
//                    PortalKey = portal.PortalKey,
//                    TitleFa = portal.TitleFa,
//                    DescriptionFa = portal.DescriptionFa,
//                    TitleEn = portal.TitleEn,
//                    DescriptionEn = portal.DescriptionEn
//                };

//                _portal.Add(newPortal);
//                await _uow.SaveChangesAsync().ConfigureAwait(false);
//                return;
//            }
            
//            var currentPortal = await FindPortalByIdAsync(portal.Id).ConfigureAwait(false);

//            currentPortal.PortalKey = portal.PortalKey;
//            currentPortal.TitleFa = portal.TitleFa;
//            currentPortal.DescriptionFa = portal.DescriptionFa;
//            currentPortal.TitleEn = portal.TitleEn;
//            currentPortal.DescriptionEn = portal.DescriptionEn;

//            await _uow.SaveChangesAsync().ConfigureAwait(false);
//        }

//        public Task<Portal> FindPortalByKeyAsync(string portalKey)
//        {
//            return _portal.FirstOrDefaultAsync(p => p.PortalKey == portalKey.Trim());
//        }

//        public Task<Portal> FindPortalByIdAsync(int portalId)
//        {
//            return _portal.FirstOrDefaultAsync(p => p.Id == portalId);
//        }

//        public Task<bool> ValidatePortalKeyAsync(string portalKey)
//        {
//            return _portal.AnyAsync(p => p.PortalKey == portalKey.Trim());
//        }

//        public async Task<IList<PortalViewModel>> GetAllPortalsAsync()
//        {
//            var portals = await _portal.Cacheable().Select(p => new { p.Id, p.PortalKey, p.TitleFa, p.DescriptionFa }).ToListAsync().ConfigureAwait(false);
//            var portalsViewModel = new List<PortalViewModel>();

//            foreach (var item in portals)
//            {
//                portalsViewModel.Add(new PortalViewModel
//                {
//                    Id = item.Id,
//                    PortalKey = item.PortalKey,
//                    TitleFa = item.TitleFa,
//                    DescriptionFa = item.DescriptionFa
//                });
//            }

//            return portalsViewModel;
//        }

//        public async Task DeletePortalAsync(int id)
//        {
//            var portal = new Portal { Id = id };

//            _uow.Entry(portal).State = EntityState.Deleted;

//            await _uow.SaveChangesAsync().ConfigureAwait(false);
//        }
//    }
//}
