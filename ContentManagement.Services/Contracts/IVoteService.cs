using ContentManagement.Entities;
using ContentManagement.ViewModels.Areas.Manage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ContentManagement.Services.Contracts
{
    public interface IVoteService
    {
        Task<Vote> FindVoteAsync(long id);
        Task<IList<VoteViewModel>> GetPagedVotesAsync(int? portalId, Language language = Language.FA, string searchTerm = null, int start = 0, int length = 10);
        Task<long> VotesCountAsync();
        Task<long> VotesPagedCountAsync(int? portalId, Language language = Language.FA, string searchTerm = null);
        Task AddOrUpdateVoteAsync(VoteViewModel vote);
        Task DeleteVoteAsync(long id);
        Task<ContentManagement.ViewModels.VoteViewModel> GetVoteDetails(string portalKey, Language language = Language.FA, long voteId = 0);
        Task AddVoteResults(long vid, long[] voteItemsId);
    }
}
