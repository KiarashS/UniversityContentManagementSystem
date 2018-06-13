﻿using ContentManagement.Common.ReflectionToolkit;
using ContentManagement.Common.WebToolkit;
using ContentManagement.Common.WebToolkit.Attributes;
using ContentManagement.Entities;
using DNTPersianUtils.Core;
using System;

namespace ContentManagement.ViewModels
{
    public class ContentsViewModel
    {
        public long Id { get; set; }
        public Entities.Language Language { get; set; }
        public ContentType ContentType { get; set; }
        public string Title { get; set; }
        public string Summary { private get; set; }
        public string RawText { private get; set; }
        public string Imagename { get; set; }
        public DateTimeOffset PublishDate { get; set; }


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
                    return PublishDate.ToShortPersianDateString().ToPersianNumbers(); //ToLongPersianDateTimeString()
                }
            }
        }

        public string TypeOfContent
        {
            get
            {
                if (Language == Entities.Language.EN)
                {
                    return ContentType.GetAttributeOfType<ContentTypeTextEnAttribute>().Description;
                }
                else
                {
                    return ContentType.GetAttributeOfType<ContentTypeTextFaAttribute>().Description;
                }
            }
        }

        public string GetSummary
        {
            get
            {
                if (!string.IsNullOrEmpty(Summary))
                {
                    return Summary.TruncateAtWord(64);
                }
                else if (!string.IsNullOrEmpty(RawText))
                {
                    return RawText.TruncateAtWord(64);
                }

                return string.Empty;
            }
        }
    }
}
