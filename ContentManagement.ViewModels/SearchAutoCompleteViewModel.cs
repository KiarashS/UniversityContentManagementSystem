using ContentManagement.Common.ReflectionToolkit;
using ContentManagement.Common.WebToolkit.Attributes;
using ContentManagement.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentManagement.ViewModels
{
    public class SearchAutoCompleteResult
    {
        public SearchAutoCompleteResult()
        {
            Data = new SearchAutoCompleteItems();
        }

        public bool Status { get; set; } = true;
        public string Error { get; set; }
        public int ResultsCount { get; set; }
        public SearchAutoCompleteItems Data { get; set; }
    }

    public class SearchAutoCompleteItems
    {
        public SearchAutoCompleteItems()
        {
            Results = new List<SearchAutoCompleteItem>();
        }

        public IList<SearchAutoCompleteItem> Results { get; set; }
    }

    public class SearchAutoCompleteItem
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string ContentType { get; set; }
        public string Imagename { get; set; }
        public string Link { get; set; }
        public bool IsLastItem { get; set; }
        public bool IsArchive { get; set; }
    }

    public class SearchAutoCompleteViewModel
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public ContentType ContentType { get; set; }
        public string Imagename { get; set; }
        public bool IsArchive { get; set; } 
        public Language Language { get; set; }


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
