using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using Outsourcing.Models;
using Outsourcing.CustomDataAnnotations;

namespace Outsourcing.ViewModel
{
    public class OutsourcingEmployeeDetailsView
    {
        [Display(Name = "Candidate ID")]
        public string CandidateID { get; set; }

        [Required]
        [Display(Name = "Candidate Name")]
        public string CandidateName { get; set; }

        [Required]
        [Display(Name = "Date of Birth")]
        //[DataType(DataType.Date),DisplayFormat(DataFormatString="{0:dd/MM/yyyy}",ApplyFormatInEditMode=true)]
        public Nullable<System.DateTime> DOB { get; set; }

        [Display(Name = "Age")]
        public int Age { get; set; }

        
        [Display(Name ="Physically Challenged")]
     
        public bool PH { get; set; }
        
        [Display(Name ="Marital Status")]
        public string MaritalStatus { get; set; }
        public IEnumerable<SelectListItem> MStatus { get; set; }

        [Required]
        [Display(Name = "Gender")]
        public string Gender { get; set; }
        public IEnumerable <SelectListItem> Genders { get; set; }
        
        [Required]
        [Display(Name = "Caste Category")]
        public string CasteCategory { get; set; }
        public IEnumerable<SelectListItem> CasteCategories { get; set; }

        [Required]
        [Display(Name = "Father Name")]
        public string FatherName { get; set; }

        [Display(Name = "Mother Name")]
        public string MotherName { get; set; }

        [Display(Name = "Husband Name")]
        public string HusbandName { get; set; }

        [Required]
        [Display(Name = "Permanent Address")]
        public string PermanentAddress { get; set; }

        [Required]
        [Display(Name = "Communication Address")]
        public string CommunicationAddress { get; set; }

        [RegularExpression("[A-Z]{5}[0-9]{4}[A-Z]{1}$", ErrorMessage = "Length should be 10. eg.AAAPL1234M")]
        [Display(Name = "Permanent Account Number (PAN)")]
        public string PAN { get; set; }

        [RegularExpression("[0-9]{12}",ErrorMessage ="Should contains 12 digits")]
        [Display(Name = "Aadhar Number")]
        public string Aadhar { get; set; }

        [RegularExpression("[0-9]{3,6}-[0-9]{6,8}$", ErrorMessage = "eg. 044-12345678, 04287-223458  ")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Required]
        [RegularExpression("^((\\+91-?)|0)?[0-9]{10}$",ErrorMessage="No. should be 10 digits eg. +91...,+91-....,0.... or 10 digits only")]
        [Display(Name = "Mobile Number")]        
        public string MobileNumber { get; set; }

        [Display(Name = "Email ID")]
        [DataType(DataType.EmailAddress)]
        public string EmailID { get; set; }

        [Display(Name = "Emergency Contact Number")]
        public string EmergencyContactNo { get; set; }

        [RegularExpression("^[0-9]{12}$")]
        [Display(Name = "Provident Fund (Universal Account No [UAN]) No")]
        public string PF_UAN { get; set; }

        [Display(Name = "Employee's State Insurance Corporation (ESIC) No")]
        public string ESIC_NO { get; set; }

        [Display(Name = "Bank Name")]
        public string BankName { get; set; }
        public IEnumerable<SelectListItem> BankNameList { get; set; }
        
        [Display(Name = "Branch Name")]
        public string BranchName { get; set; }

        [RegularExpression("^[0-9]{10,20}$", ErrorMessage = "Length should not exceed 20")]
        [Display(Name = "Bank Account No")]
        public string BankAccountNo { get; set; }

        [System.ComponentModel.DataAnnotations.Compare("BankAccountNo",ErrorMessage="The confirm Bank Account Number does not match with Bank Account Number")]
        [RegularExpression("^[0-9]{10,20}$",ErrorMessage= "Length should not exceed 20")]
        [Display(Name = "Re-Enter Bank Account No")]
        public string ConfirmBankAccountNo { get; set; }

        [RegularExpression("[A-Z]{4}[0]{1}[A-Z0-9]{6}$", ErrorMessage = "Length should be 11. eg. HDFC0123456")]
        [Display(Name = "IFSC Code")]
        public string IFSC_Code { get; set; }
        public string Command { get; set; }
        [Display(Name = "Created On")]
        public Nullable<System.DateTime> CreatedOn { get; set; }
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }
        [Display(Name = "Updated On")]
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        [Display(Name = "Updated By")]
        public string UpdatedBy { get; set; }

        public static implicit operator OutsourcingEmployeeDetail (OutsourcingEmployeeDetailsView ev)
        {
            return new OutsourcingEmployeeDetail
            {
                CandidateID = ev.CandidateID,
                CandidateName = ev.CandidateName,
                DOB= Convert.ToDateTime(ev.DOB),
                Gender =ev.Gender,
                CasteCategory =ev.CasteCategory, 
                PH=Convert.ToBoolean(ev.PH),
                MaritalStatus=ev.MaritalStatus,
                FatherName =ev.FatherName, 
                MotherName =ev.MotherName, 
                HusbandName =ev.HusbandName, 
                PermanentAddress =ev.PermanentAddress,
                CommunicationAddress =ev.CommunicationAddress,
                PAN =ev.PAN, 
                Aadhar=ev.Aadhar,
                PhoneNumber =ev.PhoneNumber, 
                MobileNumber =ev.MobileNumber, 
                EmailID =ev.EmailID, 
                EmergencyContactNo =ev.EmergencyContactNo,
                PF_UAN=ev.PF_UAN,
                ESIC_NO=ev.ESIC_NO,
                BankName=ev.BankName,
                BranchName=ev.BranchName,
                BankAccountNo=ev.BankAccountNo,
                IFSC_Code=ev.IFSC_Code
            };
        }
        
        public static implicit operator OutsourcingEmployeeDetailsView(OutsourcingEmployeeDetail om)
        {
            return new OutsourcingEmployeeDetailsView
            {
                CandidateID = om.CandidateID,
                CandidateName = om.CandidateName,
                DOB = om.DOB,
                Gender = om.Gender,
                CasteCategory = om.CasteCategory,
                PH=om.PH,
                MaritalStatus=om.MaritalStatus,
                FatherName = om.FatherName,
                MotherName = om.MotherName,
                HusbandName = om.HusbandName,
                PermanentAddress = om.PermanentAddress,
                CommunicationAddress = om.CommunicationAddress,
                PAN = om.PAN,
                Aadhar=om.Aadhar,
                PhoneNumber = om.PhoneNumber,
                MobileNumber = om.MobileNumber,
                EmailID = om.EmailID,
                EmergencyContactNo = om.EmergencyContactNo,
                PF_UAN=om.PF_UAN,
                ESIC_NO=om.ESIC_NO,
                BankName=om.BankName,
                BranchName=om.BranchName,
                BankAccountNo=om.BankAccountNo,
                IFSC_Code=om.IFSC_Code
            };
        }
        public List<SelectListItem> GenderList()
        {
            List<SelectListItem> gList = new List<SelectListItem>();
            gList = new[]
            {
                new SelectListItem {Value="M", Text="Male"},
                new SelectListItem {Value="F", Text="Female"}
            }.ToList();
            return gList;
        }
       
        public List<SelectListItem> M_Status()
        {
            List<SelectListItem> mList = new List<SelectListItem>();
            mList = new[]
            {
                new SelectListItem {Value="Married",Text="Married" },
                new SelectListItem {Value="Unmarried",Text="Unmarried" },
                new SelectListItem {Value="Divorced",Text="Divorced" },
                new SelectListItem {Value="Widowed",Text="Widowed" }
            }.ToList();
            return mList;
        }

        public List<SelectListItem> CasteList()
        {
            List<SelectListItem> cList = new List<SelectListItem>();
            cList = new[]
            {
                new SelectListItem {Value="ST", Text="ST"},
                new SelectListItem {Value="SC", Text="SC"},
                new SelectListItem {Value="OBC", Text="OBC"},
                new SelectListItem {Value="OC", Text="Other Caste"}
            }.ToList();
            return cList;
        }

        public List<SelectListItem> bankList()
        {
            using (RecruitEntities recruit = new RecruitEntities())
            {
                List<SelectListItem> blist = recruit.tbl_mst_BankName.Select(m => new SelectListItem { Text = m.BankName, Value = m.BankName }).ToList();
                return blist;
            }
        }
    }         
}