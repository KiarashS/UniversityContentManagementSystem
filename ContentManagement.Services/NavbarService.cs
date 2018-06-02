namespace ContentManagement.Services
{
    using ContentManagement.Common.GuardToolkit;
    using ContentManagement.DataLayer.Context;
    using ContentManagement.Entities;
    using ContentManagement.Services.Contracts;
    using ContentManagement.ViewModels;
    using ContentManagement.ViewModels.Areas.Manage;
    using EFSecondLevelCache.Core;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="NavbarService" />
    /// </summary>
    public class NavbarService : INavbarService
    {
        /// <summary>
        /// Defines the _uow
        /// </summary>
        private readonly IUnitOfWork _uow;

        /// <summary>
        /// Defines the _navbar
        /// </summary>
        private readonly DbSet<Navbar> _navbar;

        /// <summary>
        /// Initializes a new instance of the <see cref="NavbarService"/> class.
        /// </summary>
        /// <param name="uow">The <see cref="IUnitOfWork"/></param>
        public NavbarService(IUnitOfWork uow)
        {
            _uow = uow;
            _uow.CheckArgumentIsNull(nameof(uow));

            _navbar = _uow.Set<Navbar>();
        }

        /// <summary>
        /// The AddOrUpdateNavbarAsync
        /// </summary>
        /// <param name="navbar">The <see cref="NavbarViewModel"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public async Task AddOrUpdateNavbarAsync(NavbarViewModel navbar)
        {
            if (navbar.Id == 0) // Add
            {
                var newNavbar = new Navbar
                {
                    Text = navbar.Text?.Trim(),
                    Url = navbar.Url?.Trim(),
                    Icon = navbar.Icon,
                    IsBlankUrlTarget = navbar.IsBlankUrlTarget,
                    Priority = navbar.Priority,
                    ParentId = navbar.ParentId,
                    Language = navbar.Language,
                    PortalId = navbar.PortalId
                };

                _navbar.Add(newNavbar);
                await _uow.SaveChangesAsync().ConfigureAwait(false);
                return;
            }

            var currentNavbar = await _navbar.Where(x => x.Id == navbar.Id).Include(x => x.Childrens).SingleOrDefaultAsync();

            if (currentNavbar.PortalId != navbar.PortalId || currentNavbar.Language != navbar.Language)
            {
                var navbarChildrens = currentNavbar.Childrens;

                foreach (var item in navbarChildrens)
                {
                    if (item.Childrens.Count > 0)
                    {
                        item.ParentId = navbar.ParentId;
                    }
                    else
                    {
                        item.PortalId = navbar.PortalId;
                        item.Language = navbar.Language;
                    }
                }
            }

            currentNavbar.Text = navbar.Text?.Trim();
            currentNavbar.Url = navbar.Url?.Trim();
            currentNavbar.Icon = navbar.Icon;
            currentNavbar.IsBlankUrlTarget = navbar.IsBlankUrlTarget;
            currentNavbar.Priority = navbar.Priority;
            currentNavbar.ParentId = navbar.ParentId;
            currentNavbar.PortalId = navbar.PortalId;
            currentNavbar.Language = navbar.Language;

            await _uow.SaveChangesAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// The DeleteNavbarAsync
        /// </summary>
        /// <param name="id">The <see cref="long"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public async Task DeleteNavbarAsync(long id)
        {
            var navbar = await _navbar.Where(x => x.Id == id).Include(x => x.Childrens).SingleOrDefaultAsync();
            var navbarChildrens = navbar.Childrens;

            if (navbar?.ParentId != null && navbarChildrens.Count > 0)
            {
                foreach (var item in navbarChildrens)
                {
                    item.ParentId = navbar.ParentId;
                }
            }
            else if (navbar?.ParentId == null  && navbarChildrens.Count > 0)
            {
                foreach (var item in navbarChildrens)
                {
                    item.ParentId = null;
                }
            }

            _navbar.Remove(navbar);
            await _uow.SaveChangesAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// The FindNavbarByIdAsync
        /// </summary>
        /// <param name="navbarId">The <see cref="long"/></param>
        /// <returns>The <see cref="Task{Navbar}"/></returns>
        public async Task<Navbar> FindNavbarByIdAsync(long navbarId)
        {
            var navbar = await _navbar.FirstOrDefaultAsync(x => x.Id == navbarId);
            return navbar;
        }

        public async Task<IList<Navbar>> GetHeaderNavbarsAsync(string portalKey, Language language)
        {
            var navbars = await _navbar
                          .Where(x => x.Portal.PortalKey == portalKey && x.Language == language)
                          .OrderByDescending(x => x.Priority)
                          .ThenByDescending(x => x.Id)
                          //.Cacheable()
                          .ToListAsync(); // fills the childs list too

            navbars = navbars
                    .Where(x => x.Parent == null) // for TreeViewHelper
                    .ToList();

            return navbars;
        }

        /// <summary>
        /// The GetPagedNavbarsAsync
        /// </summary>
        /// <param name="portalId">The <see cref="int"/></param>
        /// <param name="language">The <see cref="Language"/></param>
        /// <param name="searchTerm">The <see cref="string"/></param>
        /// <param name="start">The <see cref="int"/></param>
        /// <param name="length">The <see cref="int"/></param>
        /// <returns>The <see cref="Task{IList{NavbarViewModel}}"/></returns>
        public async Task<IList<NavbarViewModel>> GetPagedNavbarsAsync(int portalId, Language language = Language.FA, string searchTerm = null, int start = 0, int length = 10)
        {
            var query = _navbar.Where(x => x.PortalId == portalId && x.Language == language).AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.Trim();
                query = query.Where(x => x.Text.Contains(searchTerm) || x.Url.Contains(searchTerm));
            }

            var navbars = await query
                                    .OrderByDescending(x => x.Priority)
                                    .ThenByDescending(x => x.Id)
                                    .Skip(start)
                                    .Take(length)
                                    .Cacheable()
                                    .Select(x => new { x.Id, x.Text, x.Url, x.Icon, x.IsBlankUrlTarget, x.Priority, x.ParentId })
                                    .ToListAsync();

            return navbars.Select(x => new NavbarViewModel { Id = x.Id, Text = x.Text, Icon = x.Icon, IsBlankUrlTarget = x.IsBlankUrlTarget, Url = x.Url, Priority = x.Priority, ParentId = x.ParentId }).ToList();
        }

        /// <summary>
        /// The GetParentOfNavbarAsync
        /// </summary>
        /// <param name="parentId">The <see cref="long"/></param>
        /// <returns>The <see cref="Task{Navbar}"/></returns>
        public async Task<Navbar> GetParentOfNavbarAsync(long parentId)
        {
            var navbar = await _navbar.SingleOrDefaultAsync(x => x.Id == parentId).ConfigureAwait(false);
            return navbar;
        }

        /// <summary>
        /// The NavbarCount
        /// </summary>
        /// <returns>The <see cref="Task{long}"/></returns>
        public async Task<long> NavbarCountAsync()
        {
            var count = await _navbar.LongCountAsync().ConfigureAwait(false);
            return count;
        }

        /// <summary>
        /// The NavbarPagedCountAsync
        /// </summary>
        /// <param name="portalId">The <see cref="int"/></param>
        /// <param name="language">The <see cref="Language"/></param>
        /// <param name="searchTerm">The <see cref="string"/></param>
        /// <returns>The <see cref="Task{long}"/></returns>
        public async Task<long> NavbarPagedCountAsync(int portalId, Language language = Language.FA, string searchTerm = null)
        {
            var query = _navbar.Where(x => x.PortalId == portalId && x.Language == language).AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.Trim();
                query = query.Where(x => x.Text.Contains(searchTerm) || x.Url.Contains(searchTerm));
            }

            var count = await query.LongCountAsync().ConfigureAwait(false);
            return count;
        }
    }
}
