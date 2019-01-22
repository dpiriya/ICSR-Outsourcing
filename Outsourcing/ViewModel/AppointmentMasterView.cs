using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

using DataLayer.Repository;

namespace Outsourcing.ViewModel
{
    public class AppointmentMasterView
    {
        [Display(Name = "Employee ID")]
        public string EmployeeID { get; set; }

        [Required]   
        [Display(Name = "Employee Name")]
        public string EmployeeName { get; set; }

        [Required]
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public System.DateTime DOB { get; set; }

        [Display(Name = "Meeting ID")]
        public string MeetingID { get; set; }
        
        [Required]
        [Display(Name = "Candidate ID")]        
        public string CandidateID { get; set; }
       
        [Display(Name = "Designation Code")]                        
        public string DesignationCode { get; set; }

        [Display(Name = "Designation Name")] 
        public string DesignationName { get; set; }

        [Display(Name = "Appointment Date")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> AppointmentDate { get; set; }

        [Display(Name = "To Date")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> ToDate { get; set; }

        [Display(Name = "Relieve Date")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> RelieveDate { get; set; }

        [Display(Name = "Basic Salary")] 
        public decimal BasicSalary { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Permanent Address")] 
        public string PermanentAddress { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Communication Address")] 
        public string CommunicationAddress { get; set; }

        [Required]
        [Display(Name = "Mobile Number")] 
        public string MobileNumber { get; set; }

        [Display(Name = "Email ID")]         
        public string EmailID { get; set; }

        [Display(Name = "Bank Name")]         
        public string BankName { get; set; }
        
        [Display(Name = "Branch Name")]         
        public string BranchName { get; set; }

        [Display(Name = "Bank Account No")] 
        public string BankAccountNo { get; set; }

        [Display(Name = "IFSC Code")] 
        public string IFSC_Code { get; set; }

        [Required]
        [Display(Name = "OutSourcing Company")] 
        public string OutSourcingCompany { get; set; }
        public string Command { get; set; }
        [Display(Name = "Created On")]
        public Nullable<System.DateTime> CreatedOn { get; set; }
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }
        [Display(Name = "Updated On")]
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        [Display(Name = "Updated By")]
        public string UpdatedBy { get; set; }

        public static implicit operator AppointmentMasterView (AppointmentMaster am)
        {
            return new AppointmentMasterView
            {
                EmployeeID = am.EmployeeID,
                EmployeeName = am.EmployeeName,
                DOB = am.DOB,
                MeetingID = am.MeetingID,
                CandidateID = am.CandidateID,
                DesignationCode = am.DesignationCode,
                DesignationName = am.DesignationName,
                AppointmentDate = am.AppointmentDate,
                ToDate = am.ToDate,
                RelieveDate = am.RelieveDate,
                BasicSalary = am.BasicSalary,
                PermanentAddress = am.PermanentAddress,
                CommunicationAddress = am.CommunicationAddress,
                MobileNumber = am.MobileNumber,
                EmailID = am.EmailID,
                BankName = am.BankName,
                BranchName = am.BranchName,
                BankAccountNo = am.BankAccountNo,
                IFSC_Code = am.IFSC_Code,
                OutSourcingCompany = am.OutSourcingCompany
            };
         }

        public static implicit operator AppointmentMaster(AppointmentMasterView amv)
        {
            return new AppointmentMaster {
                EmployeeID = amv.EmployeeID,
                EmployeeName = amv.EmployeeName,
                DOB = amv.DOB,
                MeetingID = amv.MeetingID,
                CandidateID = amv.CandidateID,
                DesignationCode = amv.DesignationCode,
                DesignationName = amv.DesignationName,
                AppointmentDate = Convert.ToDateTime(amv.AppointmentDate),
                ToDate =Convert.ToDateTime(amv.ToDate),
                RelieveDate = amv.RelieveDate,
                BasicSalary = amv.BasicSalary,
                PermanentAddress = amv.PermanentAddress,
                CommunicationAddress = amv.CommunicationAddress,
                MobileNumber = amv.MobileNumber,
                EmailID = amv.EmailID,
                BankName = amv.BankName,
                BranchName = amv.BranchName,
                BankAccountNo = amv.BankAccountNo,
                IFSC_Code = amv.IFSC_Code,
                OutSourcingCompany = amv.OutSourcingCompany
            };
         }
    }
}