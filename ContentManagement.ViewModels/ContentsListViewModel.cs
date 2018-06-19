using ContentManagement.Common.ReflectionToolkit;
using ContentManagement.Common.WebToolkit.Attributes;
using ContentManagement.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentManagement.ViewModels
{
    public class ContentsListViewModel
    {
        public ContentsListViewModel()
        {
            ContentsViewModel = new List<ContentsViewModel>();
        }

        public IList<ContentsViewModel> ContentsViewModel { get; set; }
        public int Page { get; set; }
        public long TotalCount { get; set; }
        public int Start { get; set; }
        public int PageSize { get; set; }
        public ContentType? ContentType { get; set; }
        public bool OtherContents { get; set; }
        public bool Favorite { get; set; }
        public Language Language { get; set; }

        public IList<SelectListItem> ContentTypeItems
        {
            get
            {
                var contentTypeItems = new List<SelectListItem>();

                foreach (ContentType type in Enum.GetValues(typeof(ContentType)))
                {
                    if (Language == Entities.Language.EN)
                    {
                        contentTypeItems.Add(new SelectListItem {
                            Text = type.GetAttributeOfType<ContentTypeTextEnAttribute>().Description,
                            Value = type.ToString().ToLowerInvariant(),
                            Selected = ContentType.HasValue && ContentType.Value == type
                        });
                    }
                    else
                    {
                        contentTypeItems.Add(new SelectListItem
                        {
                            Text = type.GetAttributeOfType<ContentTypeTextFaAttribute>().Description,
                            Value = type.ToString().ToLowerInvariant(),
                            Selected = ContentType.HasValue && ContentType.Value == type
                        });
                    }
                }


                return contentTypeItems;
            }
        }
    }
}
