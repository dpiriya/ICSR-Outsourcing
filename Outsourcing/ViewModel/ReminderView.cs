using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Outsourcing.ViewModel
{
    public class ReminderView
    {
        [Required]
        [Display(Name = "From Date")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> FromDate { get; set; }

        [Required]
        [Display(Name = "To Date")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> ToDate { get; set; }

        public string DocumentFormat { get; set; }
        public IEnumerable<SelectListItem> DocumentFormats { get; set; }

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