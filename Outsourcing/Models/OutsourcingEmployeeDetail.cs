//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Outsourcing.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class OutsourcingEmployeeDetail
    {
        public OutsourcingEmployeeDetail()
        {
            this.OutsourcingMeetings = new HashSet<OutsourcingMeeting>();
            this.AppointmentMasters = new HashSet<AppointmentMaster>();
        }
    
        public string CandidateID { get; set; }
        public string CandidateName { get; set; }
        public System.DateTime DOB { get; set; }
        public string Gender { get; set; }
        public string CasteCategory { get; set; }
        public string FatherName { get; set; }
        public string MotherName { get; set; }
        public string HusbandName { get; set; }
        public string PermanentAddress { get; set; }
        public string CommunicationAddress { get; set; }
        public string PAN { get; set; }
        public string PhoneNumber { get; set; }
        public string MobileNumber { get; set; }
        public string EmailID { get; set; }
        public string EmergencyContactNo { get; set; }
        public string PF_UAN { get; set; }
        public string ESIC_NO { get; set; }
        public string BankName { get; set; }
        public string BranchName { get; set; }
        public string BankAccountNo { get; set; }
        public string IFSC_Code { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
    
        public virtual ICollection<OutsourcingMeeting> OutsourcingMeetings { get; set; }
        public virtual ICollection<AppointmentMaster> AppointmentMasters { get; set; }
    }
}
