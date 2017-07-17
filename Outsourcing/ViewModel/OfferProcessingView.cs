using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Outsourcing.Models;

namespace Outsourcing.ViewModel
{
    public class OfferProcessingView
    {
        [Display(Name = "Meeting ID")]
        public string MeetingID { get; set; }

        [Display(Name = "Candidate Name")]
        public string CandidateName { get; set; }

        [Display(Name = "Designation Name")]
        public string DesignationName { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Gross Salary")]
        public Nullable<decimal> GrossSalary { get; set; }

        [Display(Name = "Duration in Months")]
        public Nullable<int> DurationInMonth { get; set; }

        [Display(Name = "From Date")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> FromDate { get; set; }

        [Display(Name = "To Date")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> ToDate { get; set; }

        [Display(Name = "Offer Request Date")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime OfferRequestDate { get; set; }

        [Display(Name = "Offer Receive Date")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable <DateTime> OfferReceivedDate { get; set; }

        [Display(Name = "Joining Date")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> JoiningDate { get; set; }

        [Display(Name = "Offer Status")]
        public string OfferStatus { get; set; }   
    }
}