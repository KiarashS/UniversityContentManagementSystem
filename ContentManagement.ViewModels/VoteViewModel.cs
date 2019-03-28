using ContentManagement.Common.ReflectionToolkit;
using ContentManagement.Common.WebToolkit;
using ContentManagement.Common.WebToolkit.Attributes;
using ContentManagement.Entities;
using DNTPersianUtils.Core;
using System;
using System.Collections.Generic;

namespace ContentManagement.ViewModels
{
    public class VoteViewModel
    {
        public long Id { get; set; }
        public Entities.Language Language { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTimeOffset PublishDate { get; set; }
        public DateTimeOffset? ExpireDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsMultiChoice { get; set; }
        public bool IsVisibleResults { get; set; }
        public long TotalVoteCount { get; set; }
        public IList<VoteItemViewModel> VoteItems { get; set; }
        public IList<VoteResultViewModel> VoteResults { get; set; }


        public bool HasDescription
        {
            get
            {
                return !string.IsNullOrEmpty(Description);
            }
        }

        public bool IsNew
        {
            get
            {
                var todayDate = DateTimeOffset.UtcNow;
                return (todayDate - PublishDate).Days < 3;
            }
        }

        public string GetPublishDate
        {
            get
            {
                if (Language == Entities.Language.EN)
                {
                    return PublishDate.ToString("MM/dd/yyyy"); //ToString("dddd, d MMMM yyyy, h:m:s tt")
                }
                else
                {
                    return PublishDate.ToFriendlyPersianDateTextify().ToPersianNumbers(); //ToLongPersianDateTimeString()
                }
            }
        }

        public string GetExpireDate
        {
            get
            {
                if (Language == Entities.Language.EN)
                {
                    return ExpireDate?.ToString("MM/dd/yyyy"); //ToString("dddd, d MMMM yyyy, h:m:s tt")
                }
                else
                {
                    return ExpireDate?.ToFriendlyPersianDateTextify().ToPersianNumbers(); //ToLongPersianDateTimeString()
                }
            }
        }

        public string GetId
        {
            get
            {
                if (Language == Entities.Language.EN)
                {
                    return Id.ToString();
                }
                else
                {
                    return Id.ToString().ToPersianNumbers();
                }
            }
        }

        public string GetTotalVoteCount
        {
            get
            {
                if (Language == Entities.Language.EN)
                {
                    return TotalVoteCount.ToString();
                }
                else
                {
                    return TotalVoteCount.ToString().ToPersianNumbers();
                }
            }
        }

        public string GetDescription
        {
            get
            {
                if (!string.IsNullOrEmpty(Description))
                {
                    return Description.TruncateAtWord(300);
                }

                return string.Empty;
            }
        }

        public string GetItemInputType
        {
            get
            {
                if (IsMultiChoice)
                {
                    return "checkbox";
                }
                return "radio";
            }
        }
    }
}
