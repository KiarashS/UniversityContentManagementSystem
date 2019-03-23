using ContentManagement.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ContentManagement.ViewModels
{
    public class ContactEmailViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        public string Direction { get; set; }
        public string SenderIpAddress { get; set; }
        public string Portal { get; set; }
        public string PortalLanguage { get; set; }
        public string SubmitDateTime { get; set; }
    }
}
