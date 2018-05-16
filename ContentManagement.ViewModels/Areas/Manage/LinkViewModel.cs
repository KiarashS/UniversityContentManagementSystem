using ContentManagement.Common.ReflectionToolkit;
using ContentManagement.Common.WebToolkit;
using ContentManagement.Common.WebToolkit.Attributes;
using ContentManagement.Entities;
using DNTPersianUtils.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ContentManagement.ViewModels.Areas.Manage
{
    public class LinkViewModel
    {
        //public LinkViewModel()
        //{
        //    LinkTypes = new List<SelectListItem>();
        //}

        public long Id { get; set; }
        [Required(ErrorMessage = "لطفاً پرتال را انتخاب نمایید.")]
        public int PortalId { get; set; }
        [Required(ErrorMessage = "لطفاً زبان را انتخاب نمایید.")]
        public ContentManagement.Entities.Language Language { get; set; } = ContentManagement.Entities.Language.FA;
        [Required(ErrorMessage = "متن لینک را وارد نمایید.")]
        public string Text { get; set; }
        [Required(ErrorMessage = "آدرس لینک را وارد نمایید.")]
        public string Url { get; set; }
        [Required(ErrorMessage = "نوع لینک را انتخاب نمایید.")]
        public LinkType LinkType { get; set; }
        public string Icon { get; set; }
        public string IconColor { get; set; }
        public bool IsBlankUrlTarget { get; set; }
        public int? Priority { get; set; }


        public IList<SelectListItem> LinkTypes
        {
            get
            {
                var linkTypesList = new List<SelectListItem>() { new SelectListItem { Text = "", Selected = (Id == 0) } };
                var linkValues = Enum.GetValues(typeof(ContentManagement.Entities.LinkType)).Cast<ContentManagement.Entities.LinkType>();
                
                foreach (var item in linkValues)
                {
                    var text = item.GetAttributeOfType<LinkTypeTextInAdminAttribute>().Description;
                    linkTypesList.Add(new SelectListItem { Text = text, Value = ((int)item).ToString(), Selected = (Id != 0 && item == LinkType) });
                }

                return linkTypesList;
            }
            set
            {
                LinkTypes = value;
            }
        }
    }
}
