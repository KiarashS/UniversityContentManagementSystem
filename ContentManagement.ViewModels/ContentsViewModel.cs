using ContentManagement.Common.ReflectionToolkit;
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
        public ContentType? ContentType { get; set; }
        public ContentType DisplayContentType { get; set; }
        public string Title { get; set; }
        public string Summary { private get; set; }
        public string RawText { private get; set; }
        public string Imagename { get; set; }
        public DateTimeOffset PublishDate { get; set; }
        public string Link { get; set; }
        public bool IsFavorite { get; set; }
        public bool IsArchive { get; set; }
        public bool HasGallery { get; set; }
        public bool HasVideo { get; set; }
        public bool HasAudio { get; set; }
        public int? Priority { get; set; }


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
                    return DisplayContentType.GetAttributeOfType<ContentTypeTextEnAttribute>().Description;
                }
                else
                {
                    return DisplayContentType.GetAttributeOfType<ContentTypeTextFaAttribute>().Description;
                }
            }
        }

        public string GetTitle
        {
            get
            {
                if (!string.IsNullOrEmpty(Title) && DisplayContentType == Entities.ContentType.Announcement)
                {
                    return Title.TruncateAtWord(50);
                }
                else if (!string.IsNullOrEmpty(Title))
                {
                    return Title.TruncateAtWord(64);
                }

                return string.Empty;
            }
        }

        public string GetSummary
        {
            get
            {
                if (!string.IsNullOrEmpty(Summary))
                {
                    return Summary.TruncateAtWord(94);
                }
                else if (!string.IsNullOrEmpty(RawText))
                {
                    return RawText.TruncateAtWord(94);
                }

                return string.Empty;
            }
        }

        public bool IsPinned
        {
            get
            {
                return Priority.HasValue;
            }
        }
    }
}
