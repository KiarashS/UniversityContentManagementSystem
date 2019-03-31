using ContentManagement.Entities;
using ContentManagement.ViewModels.Areas.Manage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ContentManagement.Services.Contracts
{
    public interface IVoteItemService
    {
        Task<IList<VoteItemViewModel>> GetItemsAsync(long vid);
        Task AddOrUpdateItemAsync(long voteId, VoteItemViewModel voteItem);
        Task DeleteItemAsync(long voteId, long id);
        Task DeleteItemsAsync(long voteId);
        Task<VoteItem> FindItemByIdAsync(long voteId, long voteItemId);
        Task<IList<ViewModels.VoteItemViewModel>> GetVoteItemsAsync(long voteId);
        Task<IList<ViewModels.VoteResultViewModel>> GetVoteResultsAsync(long voteId);
    }
}
