using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContentManagement.Common.GuardToolkit;
using ContentManagement.DataLayer.Context;
using ContentManagement.Entities;
using ContentManagement.Services.Contracts;
using ContentManagement.ViewModels;
using ContentManagement.ViewModels.Areas.Manage;
using EFSecondLevelCache.Core;
using Microsoft.EntityFrameworkCore;

namespace ContentManagement.Services
{
    public class VoteService : IVoteService
    {
        private readonly IUnitOfWork _uow;
        private readonly DbSet<Vote> _votes;

        public VoteService(IUnitOfWork uow)
        {
            _uow = uow;
            _uow.CheckArgumentIsNull(nameof(_uow));

            _votes = _uow.Set<Vote>();
        }

        public async Task AddOrUpdateVoteAsync(ContentManagement.ViewModels.Areas.Manage.VoteViewModel vote)
        {
            if (vote.Id == 0) // Add
            {
                var newVote = new Vote
                {
                    Title = vote.Title?.Trim(),
                    Description = vote.Description?.Trim(),
                    IsMultiChoice = vote.IsMultiChoice,
                    IsVisibleResults = vote.IsVisibleResults,
                    IsActive = vote.IsActive,
                    PortalId = vote.PortalId,
                    Language = vote.Language,
                    ExpireDate = vote.ExpireDate?.ToUniversalTime()
                };

                _votes.Add(newVote);
                await _uow.SaveChangesAsync().ConfigureAwait(false);
                return;
            }

            var currentVote = await FindVoteAsync(vote.Id).ConfigureAwait(false);

            currentVote.Title = vote.Title?.Trim();
            currentVote.Description = vote.Description?.Trim();
            currentVote.IsMultiChoice = vote.IsMultiChoice;
            currentVote.IsVisibleResults = vote.IsVisibleResults;
            currentVote.IsActive = vote.IsActive;
            currentVote.PortalId = vote.PortalId;
            currentVote.Language = vote.Language;
            currentVote.ExpireDate = vote.ExpireDate?.ToUniversalTime();

            await _uow.SaveChangesAsync().ConfigureAwait(false);
        }

        public Task<Vote> FindVoteAsync(long id)
        {
            return _votes.FindAsync(id);
        }

        public async Task<IList<ContentManagement.ViewModels.Areas.Manage.VoteViewModel>> GetPagedVotesAsync(int? portalId, Language language, string searchTerm = null, int start = 0, int length = 20)
        {
            var query = _votes.Where(x => x.Language == language).AsQueryable();

            if (portalId.HasValue)
            {
                query = query.Where(x => x.PortalId == portalId);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.Trim();
                query = query.Where(x => x.Title.Contains(searchTerm) || x.Description.Contains(searchTerm));
            }

            var votes = await query
                                    .OrderByDescending(x => x.PublishDate)
                                    .Skip(start)
                                    .Take(length)
                                    .Select(x => new { x.Id, x.Title, x.Description, x.PublishDate, x.ExpireDate, x.IsMultiChoice, x.IsActive, x.IsVisibleResults, x.PortalId, x.Language, x.Portal.PortalKey })
                                    .Cacheable()
                                    .ToListAsync();

            return votes.Select(x => new ContentManagement.ViewModels.Areas.Manage.VoteViewModel { Id = x.Id, Title = x.Title, Description = x.Description, PortalId = x.PortalId, PublishDate = x.PublishDate, ExpireDate = x.ExpireDate, IsActive = x.IsActive, IsMultiChoice = x.IsMultiChoice, IsVisibleResults = x.IsVisibleResults, Language = x.Language, PortalKey = x.PortalKey }).ToList();
        }

        public async Task<long> VotesCountAsync()
        {
            var count = await _votes.LongCountAsync().ConfigureAwait(false);
            return count;
        }

        public async Task<long> VotesPagedCountAsync(int? portalId, Language language, string searchTerm = null)
        {
            var query = _votes.Where(x => x.Language == language).AsQueryable();

            if (portalId.HasValue)
            {
                query = query.Where(x => x.PortalId == portalId);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.Trim();
                query = query.Where(x => x.Title.Contains(searchTerm) || x.Description.Contains(searchTerm));
            }

            var count = await query.Cacheable().LongCountAsync().ConfigureAwait(false);
            return count;
        }

        public async Task DeleteVoteAsync(long id)
        {
            var vote = new Vote { Id = id };

            _uow.Entry(vote).State = EntityState.Deleted;

            await _uow.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<ViewModels.VoteViewModel> GetVoteDetails(string portalKey, Language language = Language.FA, long voteId = 0)
        {
            var vote = await _votes.Where(x => x.Id == voteId && x.Portal.PortalKey == portalKey && x.Language == language).Cacheable().SingleOrDefaultAsync().ConfigureAwait(false);

            if (vote == null)
            {
                return null;
            }

            return new ViewModels.VoteViewModel
            {
                Id = vote.Id,
                Language = vote.Language,
                Description = vote.Description,
                ExpireDate = vote.ExpireDate,
                IsActive = vote.IsActive,
                IsMultiChoice = vote.IsMultiChoice,
                IsVisibleResults = vote.IsVisibleResults,
                PublishDate = vote.PublishDate,
                Title = vote.Title
            };
        }

        public async Task AddVoteResults(long vid, long[] voteItemsId)
        {
            var voteResults = new List<VoteResult>();
            foreach (var item in voteItemsId)
            {
                voteResults.Add(new VoteResult { VoteId = vid, VoteItemId = item });
            }

            _uow.AddRange(voteResults);
            await _uow.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
