using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Outsourcing.ViewModel
{
    public class OfferCancelView
    {
        [Display(Name = "Meeting ID")]
        public string MeetingID { get; set; }

        [Display(Name = "Candidate Name")]
        public string CandidateName { get; set; }

        [Display(Name = "Designation Name")]
        public string DesignationName { get; set; }

        [Required]
        [Display(Name = "Offer Status")]
        public string OfferStatus { get; set; }

        [Required]
        [Display(Name = "Remarks")]        
        public string Remarks { get; set; }        
    }
}