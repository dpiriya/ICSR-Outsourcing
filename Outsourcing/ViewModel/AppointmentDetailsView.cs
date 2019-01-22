using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

using DataLayer.Repository;

namespace Outsourcing.ViewModel
{
    public class AppointmentDetailsView
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
        [Display(Name = "Project Number")]
        public string ProjectNo { get; set; }

        [Required]
        [Display(Name = "From Date")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> FromDate { get; set; }

        [Required]
        [Display(Name = "To Date")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> ToDate { get; set; }

        [Display(Name = "Basic Salary")]
        public decimal BasicSalary { get; set; }

        [Display(Name = "Gross Salary")]
        public decimal GrossSalary { get; set; }

        [Display(Name = "Cost To Project")]
        public decimal CostToProject { get; set; }

        [Display(Name = "Commitment Number")]
        public string CommitmentNo { get; set; }

        [Display(Name = "Remarks")]
        public string Remarks { get; set; }
        public string Command { get; set; }
        [Display(Name = "Created On")]
        public Nullable<System.DateTime> CreatedOn { get; set; }
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }
        [Display(Name = "Updated On")]
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        [Display(Name = "Updated By")]
        public string UpdatedBy { get; set; }

        public static implicit operator AppointmentDetailsView (AppointmentDetail ad)
        {
            return new AppointmentDetailsView
            {
                EmployeeID= ad.EmployeeID,
                EmployeeName=ad.EmployeeName,
                MeetingID=ad.MeetingID,
                OrderID=ad.OrderID,
                OrderType=ad.OrderType,
                ProjectNo=ad.ProjectNo,
                FromDate=Convert.ToDateTime(ad.FromDate),
                ToDate=Convert.ToDateTime(ad.ToDate),
                BasicSalary=ad.BasicSalary,
                GrossSalary=ad.GrossSalary,
                CostToProject=ad.CostToProject,
                CommitmentNo=ad.CommitmentNo,
                Remarks=ad.Remarks
            };
        }

        public static implicit operator AppointmentDetail (AppointmentDetailsView adv)
        {
            return new AppointmentDetail
            {
                EmployeeID = adv.EmployeeID,
                EmployeeName = adv.EmployeeName,
                MeetingID = adv.MeetingID,
                OrderID = adv.OrderID,
                OrderType = adv.OrderType,
                ProjectNo = adv.ProjectNo,
                FromDate = Convert.ToDateTime(adv.FromDate),
                ToDate = Convert.ToDateTime(adv.ToDate),
                BasicSalary = adv.BasicSalary,
                GrossSalary = adv.GrossSalary,
                CostToProject = adv.CostToProject,
                CommitmentNo = adv.CommitmentNo,
                Remarks = adv.Remarks
            };
        }
    }
}