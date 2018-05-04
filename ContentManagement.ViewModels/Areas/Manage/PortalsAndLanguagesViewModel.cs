﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentManagement.ViewModels.Areas.Manage
{
    public class PortalsAndLanguagesViewModel
    {
        public PortalsAndLanguagesViewModel()
        {
            Portals = new List<SelectListItem>();
            Languages = new List<SelectListItem>();
        }

        public int PortalId { get; set; }
        public int Language { get; set; }
        public List<SelectListItem> Portals { set; get; }
        public List<SelectListItem> Languages { set; get; }
    }
}
