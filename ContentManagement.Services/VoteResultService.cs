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
    public class VoteResultService : IVoteResultService
    {
        private readonly IUnitOfWork _uow;
        private readonly DbSet<VoteResult> _voteResults;

        public VoteResultService(IUnitOfWork uow)
        {
            _uow = uow;
            _uow.CheckArgumentIsNull(nameof(_uow));

            _voteResults = _uow.Set<VoteResult>();
        }

        //public async Task<IList<VoteResultViewModel>> GetVoteResultsAsync(long voteId)
        //{
        //    var results = await _voteResults
        //        .Where(x => x.VoteItem.VoteId == voteId)
        //        .GroupBy(x => x.VoteItemId)
        //        .Select(x => new { VoteItemId = x.Key, VoteCount = x.Count() })
        //        .Cacheable()
        //        .ToListAsync()
        //        .ConfigureAwait(false);

        //    results = results.OrderByDescending(x => x.VoteCount).ThenBy(x => x.VoteItemId).ToList();

        //    var voteResultViewModel = new List<VoteResultViewModel>();
        //    foreach (var item in results)
        //    {
        //        voteResultViewModel.Add(new VoteResultViewModel
        //        {
        //            VoteItemId = item.VoteItemId,
        //            VoteCount = item.VoteCount
        //        });
        //    }

        //    return voteResultViewModel;
        //}

        public async Task<long> VoteResultsCountAsync(long voteId)
        {
            var count = await _voteResults.Where(x => x.VoteId == voteId).Cacheable().LongCountAsync().ConfigureAwait(false);
            return count;
        }
    }
}
