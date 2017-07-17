using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Outsourcing.Models;
using System.ComponentModel.DataAnnotations.Schema;
using Outsourcing.CustomDataAnnotations;

namespace Outsourcing.ViewModel 
{
    public class OutsourcingMeetingView :IValidatableObject
    {
        
        [Display(Name="Meeting ID")]
        public string MeetingID { get; set; }

        [Required]
        [Display(Name = "Meeting Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> MeetingDate { get; set; }

        [Required]
        [Display(Name = "Candidate ID")]
        public string CandidateID { get; set; }

        [Required]
        [Display(Name = "Candidate Name")]
        public string CandidateName { get; set; }

        [Required]
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DOB { get; set; }

        [Required]
        [Display(Name = "Designation Code")]                
        public string DesignationCode { get; set; }

        [Required]
        [Display(Name = "Designation Name")]                        
        public string DesignationName { get; set; }

        public IEnumerable<SelectListItem> Designations { get; set; }
        
        [Required]
        [Display(Name = "Qualification")]                                
        public string Qualification { get; set; }

        [Required]
        [Display(Name = "Experience in Years")]       
        public Nullable<decimal> Experience { get; set; }

        [Required]
        [Display(Name = "IITM Experience")]    
        [BooleanDisplayValuesAsYesNo]
        public Nullable<bool> IITMExperience { get; set; }
        public IEnumerable<SelectListItem> IITMExperiences { get; set; }

        [Required]
        [Display(Name = "Project Type")]                       
        public string ProjectType { get; set; }
        public IEnumerable<SelectListItem> ProjectTypes { get; set; }

        [Required]
        [Display(Name = "Project Number")]                               
        public string ProjectNo { get; set; }

        [Required]
        [Display(Name = "Project Title")]                                       
        public string ProjectTitle { get; set; }

        [Required]
        [Display(Name = "Department Code")] 
        public string DepartmentCode { get; set; }

        [Required]
        [Display(Name = "Department Name")]         
        public string DepartmentName { get; set; }
        public IEnumerable<SelectListItem> Departments { get; set; }
        
        [Required]
        [Display(Name = "Sponsored Agency")]      
        public string SponsoredAgency { get; set; }

        [Display(Name = "Project Closure Date")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> ProjectCloseDate { get; set; }

        [Required]
        [Display(Name = "Coordinator Code")]              
        public string PICode { get; set; }

        [Required]
        [Display(Name = "Coordinator Name")]                      
        public string PIName { get; set; }

        [Required]
        [Display(Name = "OutSourcing Company")]  
        public string OutSourcingCompany { get; set; }
        public IEnumerable<SelectListItem> OutSourcingCompanies { get; set; }
        
        [Required]
        [Display(Name = "Requested From Date")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> RequestFromDate { get; set; }
        
        [Required]
        [Display(Name = "Requested To Date")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> RequestToDate { get; set; }

        [Required]
        [Display(Name = "Duration Recommended")]
        public string DurationType { get; set; }
        public IEnumerable<SelectListItem> DurationTypes { get; set; }

        [Display(Name = "Duration in Months")]
        public Nullable<int> DurationInMonth { get; set; }

        [Display(Name = "From Date")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> FromDate { get; set; }

        [Display(Name = "To Date")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]        
        public Nullable<System.DateTime> ToDate { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Display(Name = "Gross Salary")]   
        public Nullable<decimal> GrossSalary { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Display(Name = "Cost To Project")] 
        public Nullable<decimal> CostToProject { get; set; }

        [Display(Name = "Commitment No")]         
        public string CommitmentNo { get; set; }

        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)] 
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string CreatedBy { get; set; }

        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)] 
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public string Command { get; set; }
        
        IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            if (ToDate < FromDate)
            {
                yield return new ValidationResult("To date must be greater than from date");
            }
        }
        public List<SelectListItem> DesignationList()
        {
            using (RecruitEntities Recruit= new RecruitEntities ())
            {
                List<SelectListItem> dList = Recruit.OutSourcingDesignations.Select(em => new SelectListItem {Value=em.DesignationCode, Text= em.DesignationCode + " -- "+ em.DesignationName }).ToList();
                return dList;
            }
        }
        public List<SelectListItem> ProjectTypeList()
        {
            using (RecruitEntities Recruit = new RecruitEntities())
            {
                List<SelectListItem> ptList = Recruit.ListItemMasters.OrderBy(em=>em.ListGroup).Select(em => new SelectListItem { Value = em.ListItemValue, Text = em.ListItemValue + " -- " + em.ListItemText }).ToList();
                return ptList;
            }
        }
        public List<SelectListItem> IITMExperienceList()
        {
            List<SelectListItem> iList = new List<SelectListItem>();
            iList = new[]
            {
                new SelectListItem {Value="True", Text="Yes"},
                new SelectListItem {Value="False", Text="No"}
            }.ToList();
            return iList;
        }
        public List<SelectListItem> OutSourcingCompanyList()
        {
            List<SelectListItem> osList = new List<SelectListItem>();
            osList = new[]
            {
                new SelectListItem {Value="T & M", Text="T & M Services Consulting Pvt. Ltd."}              
            }.ToList();
            return osList;
        }
        public List<SelectListItem> DepartmentList()
        {
            using (RecruitEntities Recruit = new RecruitEntities())
            {
                List<SelectListItem> dList = Recruit.Department().Select(em => new SelectListItem { Value = em.DepartmentCode, Text = em.DepartmentCode+"--"+em.DepartmentName }).ToList();
                return dList;
            }
        }

        public List<SelectListItem> DurationTypeList()
        {
            List<SelectListItem> dList = new List<SelectListItem>();
            dList = new[]
            {
                new SelectListItem {Value="DurationInMonths", Text="No. of Months"},
                new SelectListItem {Value="FormToDate", Text="From Date / To Date"},
                new SelectListItem {Value="ValidUpToDate", Text="Valid up to Date"}
            }.ToList();
            return dList;
        }
        public static implicit operator OutsourcingMeeting(OutsourcingMeetingView mv)
        {
            return new OutsourcingMeeting
            {
                MeetingID= mv.MeetingID,
                MeetingDate=Convert.ToDateTime(mv.MeetingDate),
                CandidateID=mv.CandidateID,
                CandidateName=mv.CandidateName,  
                DOB = Convert.ToDateTime(mv.DOB),  
                DesignationCode=mv.DesignationCode,
                DesignationName=mv.DesignationName,
                Qualification=mv.Qualification,
                Experience=Convert.ToDecimal(mv.Experience),
                IITMExperience=Convert.ToBoolean(mv.IITMExperience),
                ProjectType=mv.ProjectType,
                ProjectNo =mv.ProjectNo,
                ProjectTitle=mv.ProjectTitle,
                DepartmentCode=mv.DepartmentCode,  
                DepartmentName=mv.DepartmentName,  
                SponsoredAgency=mv.SponsoredAgency,  
                PICode=mv.PICode,  
                PIName=mv.PIName,  
                ProjectCloseDate=mv.ProjectCloseDate,
                RequestFromDate=Convert.ToDateTime(mv.RequestFromDate),  
                RequestToDate=Convert.ToDateTime(mv.RequestToDate),
                DurationInMonth= mv.DurationInMonth,
                FromDate=mv.FromDate,
                ToDate=mv.ToDate, 
                GrossSalary=Convert.ToDecimal(mv.GrossSalary),
                CostToProject=Convert.ToDecimal(mv.CostToProject),  
                CommitmentNo=mv.CommitmentNo,  
                OutSourcingCompany=mv.OutSourcingCompany                 
            };
        }

        public static implicit operator OutsourcingMeetingView(OutsourcingMeeting mv)
        {
            return new OutsourcingMeetingView
            {
                MeetingID = mv.MeetingID,
                MeetingDate = mv.MeetingDate,
                CandidateID = mv.CandidateID,
                CandidateName = mv.CandidateName,
                DOB = mv.DOB,
                DesignationCode = mv.DesignationCode,
                DesignationName = mv.DesignationName,
                Qualification = mv.Qualification,
                Experience = mv.Experience,
                IITMExperience = mv.IITMExperience,
                ProjectType = mv.ProjectType,
                ProjectNo = mv.ProjectNo,
                ProjectTitle = mv.ProjectTitle,
                DepartmentCode = mv.DepartmentCode,
                DepartmentName = mv.DepartmentName,
                SponsoredAgency = mv.SponsoredAgency,
                PICode = mv.PICode,
                PIName = mv.PIName,
                ProjectCloseDate = mv.ProjectCloseDate,
                RequestFromDate = mv.RequestFromDate,
                RequestToDate = mv.RequestToDate,
                DurationInMonth=mv.DurationInMonth,
                FromDate = mv.FromDate,
                ToDate = mv.ToDate,
                GrossSalary = mv.GrossSalary,
                CostToProject = mv.CostToProject,
                CommitmentNo = mv.CommitmentNo,
                OutSourcingCompany = mv.OutSourcingCompany
            };
        }

    }
}