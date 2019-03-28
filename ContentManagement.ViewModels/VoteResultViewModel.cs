using ContentManagement.Common.ReflectionToolkit;
using ContentManagement.Common.WebToolkit;
using ContentManagement.Common.WebToolkit.Attributes;
using ContentManagement.Entities;
using DNTPersianUtils.Core;
using System;

namespace ContentManagement.ViewModels
{
    public class VoteResultViewModel
    {
        public long VoteItemId { get; set; }
        public int VoteCount { get; set; }
        public string ItemTitle { get; set; }


        public double GetVotePercentage(long totalVoteCount)
        {
            return ((double)VoteCount/totalVoteCount) * 100;
        }

        public double GetRoundedVotePercentage(double votePercentage)
        {
            return Math.Truncate(votePercentage * 10) / 10;
        }

        public string GetItemColor(long totalVoteCount)
        {
            var votePercentage = GetVotePercentage(totalVoteCount);
            if (votePercentage <= 25.0)
            {
                return "danger";
            }
            else if (votePercentage > 25.0 && votePercentage <= 50.0)
            {
                return "warning";
            }
            else if (votePercentage > 50.0 && votePercentage <= 75.0)
            {
                return "info";
            }
            else
            {
                return "success";
            }
        }
    }
}
