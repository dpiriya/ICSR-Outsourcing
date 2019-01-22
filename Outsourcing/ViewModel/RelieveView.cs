using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using DataLayer.Repository;

namespace Outsourcing.ViewModel
{
    public class RelieveView
    {
        [Display(Name = "Employee ID")]
        public string EmployeeID { get; set; }

        [Required]
        [Display(Name = "Employee Name")]
        public string EmployeeName { get; set; }

        [Required]
        [Display(Name = "Meeting ID")]
        public string MeetingID { get; set; }

        [Required]
        [Display(Name = "Project Type")]
        public string ProjectType { get; set; }

        [Required]
        [Display(Name = "Project Number")]
        public string ProjectNo { get; set; }

        [Display(Name = "Appointment From Date")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> AppointmentFromDate { get; set; }

        [Display(Name = "Appointment To Date")]
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> AppointmentToDate { get; set; }

        [Required]
        [Display(Name = "Relieve Date from Project")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> RelieveDate { get; set; }     
   
        [Display(Name = "Remarks")]
        public string Remarks { get; set; }        
        
        [Display(Name = "Relieved On")]
        public Nullable<System.DateTime> RelievedOn { get; set; }
        
        [Display(Name = "Relieved By")]
        public string RelievedBy { get; set; }
        
        public string Command { get; set; }

        [Display(Name = "Extension From Date")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> ExtensionFromDate { get; set; }

        [Display(Name = "Extension To Date")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> ExtensionToDate { get; set; }
                
        public static implicit operator RelieveView(AppointmentProject ap)
        {
            return new RelieveView
            {
                EmployeeID = ap.EmployeeID,
                EmployeeName = ap.EmployeeName,
                MeetingID = ap.MeetingID,
                ProjectType = ap.ProjectType,
                ProjectNo = ap.ProjectNo,
                RelieveDate = ap.ProjectRelieveDate,
                Remarks = ap.Remarks
            };
        }

        public static implicit operator AppointmentProject(RelieveView rv)
        {
            return new AppointmentProject
            {
                EmployeeID = rv.EmployeeID,
                EmployeeName = rv.EmployeeName,
                MeetingID = rv.MeetingID,
                ProjectType = rv.ProjectType,
                ProjectNo = rv.ProjectNo,
                ProjectRelieveDate = Convert.ToDateTime(rv.RelieveDate),
                Remarks = rv.Remarks
            };
        }

    }
}