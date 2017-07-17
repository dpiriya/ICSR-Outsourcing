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
    
    public partial class OutSourcingDesignation
    {
        public OutSourcingDesignation()
        {
            this.OutsourcingMeetings = new HashSet<OutsourcingMeeting>();
            this.AppointmentMasters = new HashSet<AppointmentMaster>();
            this.AppointmentProjects = new HashSet<AppointmentProject>();
        }
    
        public string DesignationCode { get; set; }
        public string DesignationName { get; set; }
        public Nullable<short> LowerAgeLimit { get; set; }
        public Nullable<short> UpperAgeLimit { get; set; }
        public string Qualifications { get; set; }
        public string Experience { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
    
        public virtual ICollection<OutsourcingMeeting> OutsourcingMeetings { get; set; }
        public virtual ICollection<AppointmentMaster> AppointmentMasters { get; set; }
        public virtual ICollection<AppointmentProject> AppointmentProjects { get; set; }
    }
}
