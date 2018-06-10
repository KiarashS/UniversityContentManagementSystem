using ContentManagement.Common.ReflectionToolkit;
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
        

        public string GetPublishDate
        {
            get
            {
                if (Language == Entities.Language.EN)
                {
                    return PublishDate.ToString("dddd, d MMMM yyyy, h:m:s tt");
                }
                else
                {
                    return PublishDate.ToLongPersianDateTimeString();
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
    }
}
