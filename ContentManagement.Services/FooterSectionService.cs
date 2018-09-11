namespace ContentManagement.Services
{
    using ContentManagement.Common.GuardToolkit;
    using ContentManagement.DataLayer.Context;
    using ContentManagement.Entities;
    using ContentManagement.Services.Contracts;
    using ContentManagement.ViewModels.Areas.Manage;
    using EFSecondLevelCache.Core;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class FooterSectionService : IFooterSectionService
    {
        private readonly IUnitOfWork _uow;
        private readonly DbSet<FooterSection> _footerSection;
        private readonly IFooterLinkService _linkService;

        public FooterSectionService(IUnitOfWork uow, IFooterLinkService linkService)
        {
            _uow = uow;
            _uow.CheckArgumentIsNull(nameof(uow));

            _linkService = linkService;
            _linkService.CheckArgumentIsNull(nameof(linkService));

            _footerSection = _uow.Set<FooterSection>();
        }

        public async Task AddOrUpdateSectionAsync(FooterSectionViewModel section)
        {
            if (section.Id == 0) // Add
            {
                var newSection = new FooterSection
                {
                    Title = section.Title?.Trim(),
                    Url = section.Url?.Trim(),
                    IsEnable = section.IsEnable,
                    Priority = section.Priority,
                    Language = section.Language,
                    PortalId = section.PortalId
                };

                _footerSection.Add(newSection);
                await _uow.SaveChangesAsync().ConfigureAwait(false);
                return;
            }
            var currentSection = await FindSectionByIdAsync(section.Id).ConfigureAwait(false);

            currentSection.Title = section.Title?.Trim();
            currentSection.Url = section.Url?.Trim();
            currentSection.IsEnable = section.IsEnable;
            currentSection.Priority = section.Priority;
            currentSection.Language = section.Language;
            currentSection.PortalId = section.PortalId;

            await _uow.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task DeleteSectionAsync(long id)
        {
            var section = new FooterSection { Id = id };

            _uow.Entry(section).State = EntityState.Deleted;

            await _uow.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<FooterSection> FindSectionByIdAsync(long sectionId)
        {
            var section = await _footerSection.FirstOrDefaultAsync(x => x.Id == sectionId);
            return section;
        }

        public async Task<IList<FooterSectionViewModel>> GetPagedSectionsAsync(int portalId, Language language = Language.FA, string searchTerm = null, int start = 0, int length = 10)
        {
            var query = _footerSection.Where(x => x.PortalId == portalId && x.Language == language).AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.Trim();
                query = query.Where(x => x.Title.Contains(searchTerm) || x.Url.Contains(searchTerm));
            }

            var footerSections = await query
                                    .OrderByDescending(x => x.Priority)
                                    .ThenByDescending(x => x.Id)
                                    .Skip(start)
                                    .Take(length)
                                    .Cacheable()
                                    .Select(x => new { x.Id, x.Title, x.Url, x.IsEnable, x.Priority })
                                    .ToListAsync();

            return footerSections.Select(x => new FooterSectionViewModel { Id = x.Id, Title = x.Title, Url = x.Url, IsEnable = x.IsEnable, Priority = x.Priority }).ToList();
        }

        public async Task<IList<ViewModels.FooterSectionViewModel>> GetSectionAndLinksAsync(string portalKey, Language language, int maxSectionSize = 4, int maxLinkSize = 6)
        {
            var sections = await
                _footerSection
                .Where(x => x.Portal.PortalKey == portalKey && x.Language == language && x.IsEnable)
                //.Select(x => new { Section = x, SectionLinks = x.Links.OrderByDescending(l => l.Priority).ThenByDescending(l => l.Id).Take(maxLinkSize) })
                .OrderByDescending(x => x.Priority)
                .ThenByDescending(x => x.Id)
                .Take(maxSectionSize)
                .Cacheable()
                .ToListAsync();

            var sectionViewModel = new List<ViewModels.FooterSectionViewModel>();
            IList<ViewModels.FooterLinkViewModel> linksOfSections = new List<ViewModels.FooterLinkViewModel>();

            if (sections.Any())
            {
                var sectionsId = sections.Select(x => x.Id).ToList();
                linksOfSections = await _linkService.GetLinksOfSectionsAsync(sectionsId, maxLinkSize).ConfigureAwait(false);
            }

            foreach (var section in sections)
            {
                var linksViewModel = new List<ViewModels.FooterLinkViewModel>();
                var currentSectionLinks = linksOfSections.Where(x => x.FooterSectionId == section.Id).OrderByDescending(x => x.Priority).ThenByDescending(x => x.Id).ToList();
                foreach (var link in currentSectionLinks)
                {
                    linksViewModel.Add(new ViewModels.FooterLinkViewModel
                    {
                        Id = link.Id,
                        Text = link.Text,
                        Url = link.Url,
                        IsBlankUrlTarget = link.IsBlankUrlTarget
                    });
                }

                sectionViewModel.Add(new ViewModels.FooterSectionViewModel
                {
                    Id = section.Id,
                    Title = section.Title,
                    Url = section.Url,
                    Links = linksViewModel
                });
            }

            return sectionViewModel;
        }

        public async Task<long> SectionsCountAsync()
        {
            var section = await _footerSection.LongCountAsync().ConfigureAwait(false);
            return section;
        }

        public async Task<long> SectionsPagedCountAsync(int portalId, Language language = Language.FA, string searchTerm = null)
        {
            var query = _footerSection.Where(x => x.PortalId == portalId && x.Language == language).AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.Trim();
                query = query.Where(x => x.Title.Contains(searchTerm) || x.Url.Contains(searchTerm));
            }

            var count = await query.LongCountAsync().ConfigureAwait(false);
            return count;
        }
    }
}
