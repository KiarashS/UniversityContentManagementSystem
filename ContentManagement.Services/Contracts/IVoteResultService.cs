using ContentManagement.Entities;
using ContentManagement.ViewModels.Areas.Manage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ContentManagement.Services.Contracts
{
    public interface IVoteResultService
    {
        Task<long> VoteResultsCountAsync(long voteId);
        //Task<IList<ViewModels.VoteResultViewModel>> GetVoteResultsAsync(long voteId);
    }
}
