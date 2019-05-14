using ContentManagement.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ContentManagement.ViewModels
{
    public class VisaApplicationViewModel
    {
        [Required(ErrorMessage = "Please enter name")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Please enter surname")]
        public string Surname { get; set; }
        public string Forename { get; set; }
        [Required(ErrorMessage = "Please enter father name")]
        public string Fatherame { get; set; }
        [Required(ErrorMessage = "Please enter your email address")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Please enter birth place")]
        public string BirthPlace { get; set; }
        [Required(ErrorMessage = "Please enter birth date")]
        public string BirthDate { get; set; }
        [Required(ErrorMessage = "Please enter passport type")]
        public string PassportType { get; set; }
        [Required(ErrorMessage = "Please enter passport number")]
        public string PassportNumber { get; set; }
        [Required(ErrorMessage = "Please enter passport issue place")]
        public string PassportIssuePlace { get; set; }
        [Required(ErrorMessage = "Please enter passport issue date")]
        public string PassportIssueDate { get; set; }
        [Required(ErrorMessage = "Please enter passport expire date")]
        public string PassportExpireDate { get; set; }
        [Required(ErrorMessage = "Please enter present nationality")]
        public string PresentNationality { get; set; }
        public string FormerNationality { get; set; }
        [Required(ErrorMessage = "Please select marital status")]
        public string MaritalStatus { get; set; }
        public string MarriedDescription { get; set; }
        [Required(ErrorMessage = "Please enter occupation")]
        public string Occupation { get; set; }
        public string EmployerAddressPhone { get; set; }
        [Required(ErrorMessage = "Please enter your home address and phone")]
        public string PermanentAddressPhone { get; set; }
        public string IranAddressPhone { get; set; }
        public string TripedCountries { get; set; }
        [Required(ErrorMessage = "Please select visa type")]
        public string VisaType { get; set; }
        public string PurposeOfTravelingIran { get; set; }
        public string LengthOfStayInIran { get; set; }
        public string EntryExitBorder { get; set; }
        public string DateOfArrivalInIran { get; set; }
        public string MeettingIndivisualsOrganization { get; set; }
        public string ExpenseCover { get; set; }
        public string MoneyAmount { get; set; }
        public string PreviousTripToIran { get; set; }
        public string PreviousTripToIranDescription { get; set; }
        public string VisaApplicationRejection { get; set; }
        public string VisaApplicationRejectionDate { get; set; }
        public string VisaApplicationForNextCountry { get; set; }
        public string PersonsAlongWithYou { get; set; }

        public string SenderIpAddress { get; set; }
        public DateTimeOffset SubmitDateTime { get; set; } = DateTimeOffset.UtcNow;
    }

    //public enum MaritalStatus
    //{
    //    Single = 1,
    //    Married = 2
    //}

    //public enum VisaType
    //{
    //    Transit = 1,
    //    TouristOrPilgrimage = 2,
    //    Entry = 3,
    //    Other = 4
    //}

    //public enum PreviousTripToIran
    //{
    //    No = 1,
    //    Yes = 2
    //}

    //public enum VisaApplicationForNextCountry
    //{
    //    No = 1,
    //    Yes = 2
    //}

    //public enum VisaApplicationRejection
    //{
    //    No = 1,
    //    Yes = 2
    //}
}
