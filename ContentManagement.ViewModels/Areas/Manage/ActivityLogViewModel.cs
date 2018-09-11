using ContentManagement.Common.ReflectionToolkit;
using ContentManagement.Entities;
using DNTPersianUtils.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ContentManagement.ViewModels.Areas.Manage
{
    public class ActivityLogViewModel
    {
        public long Id { get; set; }
        public string SourceAddress { get; set; }
        public string ActionBy { get; set; }
        public string ActionType { get; set; }
        public string Message { get; set; }
        public string Portal { get; set; }
        public string Language { get; set; }
        public ActionLevel ActionLevel { get; set; } = ActionLevel.Medium;
        public DateTimeOffset ActionDate { get; set; }
        public string Url { get; set; }

        public string ActionDateText
        {
            get
            {
                return ActionDate.ToLongPersianDateTimeString();
            }
        }

        public string ActionLevelText
        {
            get
            {
                return ActionLevel.GetAttributeOfType<DescriptionAttribute>().Description;
            }
        }
    }
}
