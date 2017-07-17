using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Outsourcing.ViewModel
{
    public class JoiningReportView
    {
        [Display(Name = "Meeting ID")]
        public string MeetingID { get; set; }

        [Display(Name = "Candidate Name")]
        public string CandidateName { get; set; }

        [Display(Name = "Designation Name")]
        public string DesignationName { get; set; }

        [Required]
        [Display(Name = "Joining Date")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> JoiningDate { get; set; }
        [Required]
        [Display(Name = "Joining Report Approved By")]
        public string JoiningReportApprovedBy { get; set; }
        [Required]
        [Display(Name = "Joining Report Date")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> JoiningReportDate { get; set; }
        [Required]
        [Display(Name = "Approval Channel")]
        public string ApprovalChannel { get; set; }
        public IEnumerable<SelectListItem> ApprovalChannels { get; set; }
        public List<SelectListItem> ApprovalChannelList()
        {
            List<SelectListItem> cList = new List<SelectListItem>();
            cList = new[]
            {
                new SelectListItem {Value="Letter", Text="Letter"},
                new SelectListItem {Value="Email", Text="Email"}
            }.ToList();
            return cList;
        }
    }
}