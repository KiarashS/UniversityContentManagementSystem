using ContentManagement.Common.ReflectionToolkit;
using ContentManagement.Common.WebToolkit;
using ContentManagement.Common.WebToolkit.Attributes;
using ContentManagement.Entities;
using DNTPersianUtils.Core;
using System;

namespace ContentManagement.ViewModels
{
    public class SubPortalsContentsViewModel
    {
        public long Id { get; set; }
        public string PortalKey { get; set; }
        public string PortalTitle { get; set; }
        public Entities.Language Language { get; set; }
        public ContentType ContentType { get; set; }
        public string Title { get; set; }
        public DateTimeOffset PublishDate { get; set; }
        public bool IsFavorite { get; set; }
        public bool HasGallery { get; set; }


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
                    return ContentType.GetAttributeOfType<ContentTypeTextEnAttribute>().Description;
                }
                else
                {
                    return ContentType.GetAttributeOfType<ContentTypeTextFaAttribute>().Description;
                }
            }
        }

        public string GetTitle
        {
            get
            {
                if (!string.IsNullOrEmpty(Title))
                {
                    return Title.TruncateAtWord(55);
                }

                return string.Empty;
            }
        }
    }
}
