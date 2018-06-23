using ContentManagement.Common.ReflectionToolkit;
using ContentManagement.Common.WebToolkit;
using ContentManagement.Common.WebToolkit.Attributes;
using ContentManagement.Entities;
using DNTPersianUtils.Core;
using System;

namespace ContentManagement.ViewModels
{
    public class ContentViewModel
    {
        public long Id { get; set; }
        public Entities.Language Language { get; set; }
        public ContentType ContentType { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Text { get; set; }
        public string RawText { get; set; }
        public string Imagename { get; set; }
        public DateTimeOffset PublishDate { get; set; }
        public int ViewCount { get; set; }
        public bool IsFavorite { get; set; }


        public bool HasImage
        {
            get
            {
                return !string.IsNullOrEmpty(Imagename);
            }
        }

        public bool HasSummary
        {
            get
            {
                return !string.IsNullOrEmpty(Summary);
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

        public string GetViewCount
        {
            get
            {
                if (Language == Entities.Language.EN)
                {
                    return ViewCount.ToString();
                }
                else
                {
                    return ViewCount.ToString().ToPersianNumbers();
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
                    return Summary.TruncateAtWord(300);
                }
                else if (!string.IsNullOrEmpty(RawText))
                {
                    return RawText.TruncateAtWord(300);
                }

                return string.Empty;
            }
        }
    }
}
