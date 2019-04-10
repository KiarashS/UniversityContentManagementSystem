using ContentManagement.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ContentManagement.ViewModels
{
    public class PtsdViewModel
    {
        [Required(ErrorMessage = "Please enter firstname")]
        public string Firstname { get; set; }
        [Required(ErrorMessage = "Please enter surname")]
        public string Surname { get; set; }
        public string Affiliation { get; set; }
        public string MilitaryRank { get; set; }
        [Required(ErrorMessage = "Please enter major")]
        public string Major { get; set; }
        [Required(ErrorMessage = "Please enter academic degree")]
        public string AcademicDegree { get; set; }
        [Required(ErrorMessage = "Please enter your email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Please enter your phone number")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Please enter mobile number")]
        public string MobileNumber { get; set; }
        [Required(ErrorMessage = "Please enter your fax number")]
        public string Fax { get; set; }
        [Required(ErrorMessage = "Please enter your address")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Please enter postal code")]
        public string PostalCode { get; set; }

        public string SenderIpAddress { get; set; }
        public DateTimeOffset SubmitDateTime { get; set; } = DateTimeOffset.UtcNow;
    }
}
