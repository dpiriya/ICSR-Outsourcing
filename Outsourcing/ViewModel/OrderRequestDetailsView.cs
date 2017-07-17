using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Outsourcing.ViewModel
{
    public class OrderRequestDetailsView
    {
        [Display(Name = "Employee ID")]
        public string EmployeeID { get; set; }

        [Display(Name = "Employee Name")]
        public string EmployeeName { get; set; }

        [Display(Name = "Order ID")]
        public int OrderID { get; set; }

        [Display(Name = "Order Type")]
        public string OrderType { get; set; }

        [Display(Name = "Request ID")]
        public Nullable<int> RequestID { get; set; }

        [Display(Name = "Order Request Date")]
        public Nullable<System.DateTime> OrderRequestDate { get; set; }

        [Display(Name = "Order Receive Date")]
        public Nullable<System.DateTime> OrderReceiveDate { get; set; }
    }
}