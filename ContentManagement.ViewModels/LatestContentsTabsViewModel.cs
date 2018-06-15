using ContentManagement.Common.ReflectionToolkit;
using ContentManagement.Common.WebToolkit.Attributes;
using ContentManagement.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace ContentManagement.ViewModels
{
    public class LatestContentsTabsViewModel
    {
        public LatestContentsTabsViewModel()
        {
            ContentsViewModel = new List<ContentsViewModel>();
            ContentsVisibilityViewModel = new List<ContentVisibilityViewModel>();
        }

        public IList<ContentsViewModel> ContentsViewModel { get; set; }
        public IList<ContentVisibilityViewModel> ContentsVisibilityViewModel { get; set; }

        public IList<SelectListItem> OtherContentsItems
        {
            get
            {
                var otherContentsItems = new List<SelectListItem>();
                var firstContent = ContentsViewModel.FirstOrDefault();

                if (firstContent == null)
                {
                    return otherContentsItems;
                }

                var language = firstContent.Language;

                foreach (var item in ContentsVisibilityViewModel.Where(x => x.IsVisible).ToList())
                {
                    if (item.ContentType != ContentType.Education &&
                        item.ContentType != ContentType.Form &&
                        item.ContentType != ContentType.EducationalCalendar &&
                        item.ContentType != ContentType.StudentAndCultural &&
                        item.ContentType != ContentType.PoliticalAndIdeological)
                    {
                        var text = "";
                        var value = item.ContentType.ToString().ToLowerInvariant();

                        if (language == Entities.Language.EN)
                        {
                            text = item.ContentType.GetAttributeOfType<ContentTypeTextEnAttribute>().Description;
                        }
                        else
                        {
                            text = item.ContentType.GetAttributeOfType<ContentTypeTextFaAttribute>().Description;
                        }

                        otherContentsItems.Add(new SelectListItem {
                            Text = text,
                            Value = value
                        });
                    }
                }

                return otherContentsItems;
            }
        }

        //public IList<ContentVisibilityViewModel> OtherContentsVisibility
        //{
        //    get
        //    {
        //        var otherContentsVisibility = new List<ContentVisibilityViewModel>();

        //        foreach (var item in ContentsVisibilityViewModel.ToList())
        //        {
        //            if(item.ContentType != ContentType.Education ||
        //                item.ContentType != ContentType.Form || 
        //                item.ContentType != ContentType.EducationalCalendar ||
        //                item.ContentType != ContentType.StudentAndCultural ||
        //                item.ContentType != ContentType.PoliticalAndIdeological)
        //            {
        //                otherContentsVisibility.Add(item);
        //            }
        //        }

        //        return otherContentsVisibility;
        //    }
        //}

        public bool IsExistForm
        {
            get
            {
                return ContentsVisibilityViewModel.Any(x => x.ContentType == ContentType.Form && x.IsVisible);
            }
        }

        public bool IsExistEducationalCalendar
        {
            get
            {
                return ContentsVisibilityViewModel.Any(x => x.ContentType == ContentType.EducationalCalendar && x.IsVisible);
            }
        }

        public bool IsExistEducation
        {
            get
            {
                return ContentsVisibilityViewModel.Any(x => x.ContentType == ContentType.Education && x.IsVisible);
            }
        }

        public bool IsExistStudentAndCultural
        {
            get
            {
                return ContentsVisibilityViewModel.Any(x => x.ContentType == ContentType.StudentAndCultural && x.IsVisible);
            }
        }

        public bool IsExistPoliticalAndIdeological
        {
            get
            {
                return ContentsVisibilityViewModel.Any(x => x.ContentType == ContentType.PoliticalAndIdeological && x.IsVisible);
            }
        }

        public bool IsExistOtherContents
        {
            get
            {
                return ContentsVisibilityViewModel.Any(x => x.IsVisible &&
                                                        (
                                                            x.ContentType == ContentType.Congress ||
                                                            x.ContentType == ContentType.Regulation ||
                                                            x.ContentType == ContentType.Appointment ||
                                                            x.ContentType == ContentType.Research ||
                                                            x.ContentType == ContentType.Journal ||
                                                            x.ContentType == ContentType.Recall ||
                                                            x.ContentType == ContentType.ResearchAndTechnology ||
                                                            x.ContentType == ContentType.Financial ||
                                                            x.ContentType == ContentType.VirtualLearning
                                                        )
                                                        );
            }
        }

        public bool IsActiveOtherContents
        {
            get
            {
                return !IsExistEducation &&
                        !IsExistForm &&
                        !IsExistEducationalCalendar && 
                        !IsExistStudentAndCultural && 
                        !IsExistPoliticalAndIdeological &&
                        ContentsVisibilityViewModel.Any(x => x.IsVisible && 
                                                            (
                                                                x.ContentType == ContentType.Congress || 
                                                                x.ContentType == ContentType.Regulation ||
                                                                x.ContentType == ContentType.Appointment ||
                                                                x.ContentType == ContentType.Research ||
                                                                x.ContentType == ContentType.Journal ||
                                                                x.ContentType == ContentType.Recall ||
                                                                x.ContentType == ContentType.ResearchAndTechnology ||
                                                                x.ContentType == ContentType.Financial ||
                                                                x.ContentType == ContentType.VirtualLearning
                                                            )
                                                          );
            }
        }
    }
}
