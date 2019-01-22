using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

using DataLayer.Repository;

namespace Outsourcing.ViewModel
{
    public class OfferRequestView
    {
        public bool? isOffered { get; set; }

        [Display(Name = "Meeting ID")]
        public string MeetingID { get; set; }

        [Required]
        [Display(Name = "Meeting Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> MeetingDate { get; set; }

        [Required]
        [Display(Name = "Candidate Name")]
        public string CandidateName { get; set; }

        [Required]
        [Display(Name = "Designation Name")]
        public string DesignationName { get; set; }

        [Required]
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

        public static implicit operator OfferRequestView(OutsourcingMeeting mv)
        {
            return new OfferRequestView
            {
                MeetingID = mv.MeetingID,
                MeetingDate = mv.MeetingDate,
                CandidateName = mv.CandidateName,
                DesignationName = mv.DesignationName,
                DurationInMonth = mv.DurationInMonth,
                FromDate = mv.FromDate,
                ToDate = mv.ToDate,
                GrossSalary = mv.GrossSalary                
            };
        }

    }
}