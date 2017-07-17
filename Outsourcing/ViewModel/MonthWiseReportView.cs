using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Outsourcing.ViewModel
{
    public class MonthWiseReportView
    {
        [Required]
        [Display(Name = "From Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> FromDate { get; set; }

        [Required]
        [Display(Name = "To Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> ToDate { get; set; }

        [Required]
        [Display(Name = "Type of Order")]
        public string OrderType { get; set; }
        public IEnumerable<SelectListItem> OrderTypes { get; set; }

        [Required]
        [Display(Name = "Report Based On")]
        public string ReportBasedOn { get; set; }
        public IEnumerable<SelectListItem> ReportBased { get; set; }

        [Display(Name = "Required Document Format")]
        public string DocumentFormat { get; set; }
        public IEnumerable<SelectListItem> DocumentFormats { get; set; }

        public List<SelectListItem> OrderTypeList()
        {
            List<SelectListItem> ol = new List<SelectListItem>() {
                new SelectListItem { Text = "Appointment Order", Value = "Appointment"},
                new SelectListItem { Text = "Enhancement Order", Value = "Enhancement"},
                new SelectListItem { Text = "Extension Order", Value = "Extension"},
                new SelectListItem { Text = "Project Relieve Order", Value = "ProjectRelieve"},
                new SelectListItem { Text = "Relieve Order", Value = "Relieve"}                
            };
            return ol;
        }
        public List<SelectListItem> ReportBasedList()
        {
            List<SelectListItem> rl = new List<SelectListItem>() {
                new SelectListItem { Text = "Entry Date", Value = "EntryDate"},
                new SelectListItem { Text = "Start Date or From Date", Value = "StartDate"}                
            };
            return rl;
        }
        public List<SelectListItem> DocumentFormatList()
        {
            List<SelectListItem> al = new List<SelectListItem>() {
                new SelectListItem { Text = "PDF", Value = "PDF"},
                new SelectListItem { Text = "Excel (xls)", Value = "XLS"},
                new SelectListItem { Text = "Excel (xlsx)", Value = "XLSX"},
                new SelectListItem { Text = "Word (doc)", Value = "DOC"},
                new SelectListItem { Text = "Word (docx)", Value = "DOCX"}
            };
            return al;
        }
    }
}