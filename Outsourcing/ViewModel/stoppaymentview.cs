using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Outsourcing.ViewModel
{
    public class stoppaymentview
    {
        [Display(Name = "Employee ID")]
        public string EmployeeID { get; set; }

        [Required]
        [Display(Name = "Employee Name")]
        public string EmployeeName { get; set; }

        [Required]
        [Display(Name = "Meeting ID")]
        public string MeetingID { get; set; }

        [Display(Name = "Order ID")]
        public int OrderID { get; set; }

        [Required]
        [Display(Name = "Order Type")]
        public string OrderType { get; set; }

        [Required]
        [Display(Name = "From Date")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> FromDate { get; set; }
        [Display(Name = "Remarks")]
        public string Remarks { get; set; }
        public string Command { get; set; }
        
    }
}