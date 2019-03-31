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
using Z.EntityFramework.Plus;

namespace ContentManagement.Services
{
    public class VoteItemService : IVoteItemService
    {
        private readonly IUnitOfWork _uow;
        private readonly DbSet<VoteItem> _voteItems;

        public VoteItemService(IUnitOfWork uow)
        {
            _uow = uow;
            _uow.CheckArgumentIsNull(nameof(_uow));

            _voteItems = _uow.Set<VoteItem>();
        }

        public async Task<IList<ViewModels.Areas.Manage.VoteItemViewModel>> GetItemsAsync(long voteId)
        {
            var votes = await _voteItems
                                    .Where(x => x.VoteId == voteId)
                                    .OrderByDescending(x => x.Priority)
                                    .ThenBy(x => x.Id)
                                    .Select(x => new { x.Id, x.ItemTitle, x.Priority })
                                    .Cacheable()
                                    .ToListAsync()
                                    .ConfigureAwait(false);

            return votes.Select(x => new ViewModels.Areas.Manage.VoteItemViewModel { Id = x.Id, ItemTitle = x.ItemTitle, Priority = x.Priority }).ToList();
        }

        public async Task<IList<VoteResultViewModel>> GetVoteResultsAsync(long voteId)
        {
            var results = await _voteItems
                .Where(x => x.VoteId == voteId)
                .Select(x => new { VoteItemId = x.Id, VoteCount = x.VoteItemResults.Count })
                //.Cacheable()
                .ToListAsync()
                .ConfigureAwait(false);

            results = results.OrderByDescending(x => x.VoteCount).ThenBy(x => x.VoteItemId).ToList();

            var voteResultViewModel = new List<VoteResultViewModel>();
            foreach (var item in results)
            {
                voteResultViewModel.Add(new VoteResultViewModel
                {
                    VoteItemId = item.VoteItemId,
                    VoteCount = item.VoteCount
                });
            }

            return voteResultViewModel;
        }

        public async Task AddOrUpdateItemAsync(long voteId, ViewModels.Areas.Manage.VoteItemViewModel voteItem)
        {
            if (voteItem.Id == 0) // Add
            {
                var newVoteItem = new VoteItem
                {
                    VoteId = voteId,
                    ItemTitle = voteItem.ItemTitle?.Trim(),
                    Priority = voteItem.Priority,
                };

                _voteItems.Add(newVoteItem);
                await _uow.SaveChangesAsync().ConfigureAwait(false);
                return;
            }

            var currentItem = await FindItemByIdAsync(voteId, voteItem.Id).ConfigureAwait(false);

            currentItem.ItemTitle = voteItem.ItemTitle?.Trim();
            currentItem.Priority = voteItem.Priority;

            await _uow.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task DeleteItemAsync(long voteId, long id)
        {
            var item = new VoteItem { Id = id, VoteId = voteId };

            _uow.Entry(item).State = EntityState.Deleted;

            await _uow.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task DeleteItemsAsync(long voteId)
        {
            await _voteItems.Where(x => x.VoteId == voteId).DeleteAsync().ConfigureAwait(false);
            await _uow.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<VoteItem> FindItemByIdAsync(long voteId, long voteItemId)
        {
            var item = await _voteItems.Where(x => x.Id == voteItemId && x.VoteId == voteId).FirstOrDefaultAsync().ConfigureAwait(false);
            return item;
        }

        public async Task<IList<ViewModels.VoteItemViewModel>> GetVoteItemsAsync(long voteId)
        {
            var items = await _voteItems
                .Where(x => x.VoteId == voteId)
                .OrderByDescending(x => x.Priority)
                .ThenBy(x => x.Id)
                .Cacheable()
                .ToListAsync()
                .ConfigureAwait(false);

            var voteItemViewModel = new List<ViewModels.VoteItemViewModel>();
            foreach (var item in items)
            {
                voteItemViewModel.Add(new ViewModels.VoteItemViewModel
                {
                    Id = item.Id,
                    ItemTitle = item.ItemTitle
                });
            }

            return voteItemViewModel;
        }
    }
}
