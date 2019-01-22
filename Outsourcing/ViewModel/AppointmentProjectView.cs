using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

using DataLayer.Repository;

namespace Outsourcing.ViewModel
{
    public class AppointmentProjectView
    {
        [Display(Name = "Employee ID")]
        public string EmployeeID { get; set; }

        [Display(Name = "Employee Name")]
        public string EmployeeName { get; set; }

        [Required]
        [Display(Name = "Meeting ID")]
        public string MeetingID { get; set; }

        [Required]
        [Display(Name = "Designation Code")]
        public string DesignationCode { get; set; }

        [Required]
        [Display(Name = "Designation Name")]
        public string DesignationName { get; set; }

        [Required]
        [Display(Name = "Department Code")]
        public string DepartmentCode { get; set; }

        [Required]
        [Display(Name = "Project Type")]        
        public string ProjectType { get; set; }

        [Required]
        [Display(Name = "Project Number")]
        public string ProjectNo { get; set; }

        [Required]
        [Display(Name = "Coordinator Code")]
        public string PICode { get; set; }

        [Required]
        [Display(Name = "Coordinator Name")]
        public string PIName { get; set; }

        [Display(Name = "Appointment Date")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> AppointmentDate { get; set; }

        [Display(Name = "To Date")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> ToDate { get; set; }

        [Display(Name = "Project Relieve Date")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> ProjectRelieveDate { get; set; }

        [Display(Name = "Remarks")]
        public string Remarks { get; set; }
        public Nullable<System.DateTime> RelievedOn { get; set; }
        public string RelievedBy { get; set; }
        public bool PartTime { get; set; }

        public static implicit operator AppointmentProjectView(AppointmentProject ap)
        {
            return new AppointmentProjectView
            {
                EmployeeID = ap.EmployeeID,
                EmployeeName = ap.EmployeeName,
                MeetingID = ap.MeetingID,
                DesignationCode = ap.DesignationCode,
                DesignationName = ap.DesignationName,
                DepartmentCode=ap.DepartmentCode,
                PICode=ap.PICode,
                PIName=ap.PIName,
                ProjectType=ap.ProjectType,
                ProjectNo=ap.ProjectNo,
                AppointmentDate = ap.AppointmentDate,
                ToDate = ap.ToDate,
                PartTime=ap.PartTime
            };
        }

        public static implicit operator AppointmentProject(AppointmentProjectView apv)
        {
            return new AppointmentProject
            {
                EmployeeID = apv.EmployeeID,
                EmployeeName = apv.EmployeeName,
                MeetingID = apv.MeetingID,
                DesignationCode = apv.DesignationCode,
                DesignationName = apv.DesignationName,
                DepartmentCode = apv.DepartmentCode,
                PICode = apv.PICode,
                PIName = apv.PIName,
                ProjectType = apv.ProjectType,
                ProjectNo = apv.ProjectNo,                
                AppointmentDate = Convert.ToDateTime(apv.AppointmentDate),
                ToDate = Convert.ToDateTime(apv.ToDate),
                PartTime=apv.PartTime
            };
        }
    }
}