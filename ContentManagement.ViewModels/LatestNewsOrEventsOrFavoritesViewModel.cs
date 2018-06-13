using ContentManagement.Common.ReflectionToolkit;
using ContentManagement.Common.WebToolkit.Attributes;
using ContentManagement.Entities;
using DNTPersianUtils.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ContentManagement.ViewModels
{
    public class LatestNewsOrEventsOrFavoritesViewModel
    {
        public LatestNewsOrEventsOrFavoritesViewModel()
        {
            NewsOrEventsOrFavoritesViewModel = new List<ContentsViewModel>();
            AnnouncementsViewModel = new List<ContentsViewModel>();
        }

        public IList<ContentsViewModel> NewsOrEventsOrFavoritesViewModel { get; set; }
        public IList<ContentsViewModel> AnnouncementsViewModel { get; set; }
        public bool IsExistAnnouncement { get; set; }
        public bool IsExistFavorite { get; set; }
        public bool IsExistNews { get; set; }
        public bool IsExistEvent { get; set; }


        public bool JustNewsAndEventAndFavorite
        {
            get
            {
                return (IsExistNews || IsExistEvent || IsExistFavorite) && !IsExistAnnouncement;
            }
        }

        public bool JustNewsAndEventAndFavoriteAndAnnouncement
        {
            get
            {
                return (IsExistNews || IsExistEvent || IsExistFavorite) && IsExistAnnouncement;
            }
        }

        public bool JustAnnouncement
        {
            get
            {
                return !IsExistNews && !IsExistEvent && !IsExistFavorite && IsExistAnnouncement;
            }
        }

        public bool AnyNewsAndEventAndFavoriteAndAnnouncement
        {
            get
            {
                return IsExistNews || IsExistEvent || IsExistFavorite || IsExistAnnouncement;
            }
        }

        public bool AnyNewsAndEventAndFavorite
        {
            get
            {
                return IsExistNews || IsExistEvent || IsExistFavorite;
            }
        }
    }
}
