using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Outsourcing.Models;
namespace Outsourcing.ViewModel
{
    public class DesignationView
    {
        [Display(Name = "Designation Code")]
        [Required (ErrorMessage="Designation Code Required")]    
        [StringLength(3,MinimumLength=2)]
        [RegularExpression(@"[A-Z]*",ErrorMessage="Please enter upper case characters")]        
        
        public string DesignationCode { get; set; }
        
        [Display(Name = "Designation Name")]
        [Required(ErrorMessage = "Designation Name Required")]        
        public string DesignationName { get; set; }

        [Display(Name = "Lower Age Limit")]
        [Range(18,70,ErrorMessage="Age must be between 18 and 70")]
        public Nullable<short> LowerAgeLimit { get; set; }
        
        [Display(Name = "Upper Age Limit")]
        [Range(18, 90, ErrorMessage = "Age must be between 18 and 70")]
        public Nullable<short> UpperAgeLimit { get; set; }

        [Display(Name = "Qualifications")]
        public string Qualifications { get; set; }

        [Display(Name = "Experience")]
        public string Experience { get; set; }
        [Display(Name = "Created On")]
        public Nullable<System.DateTime> CreatedOn { get; set; }
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }
        [Display(Name = "Updated On")]
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        [Display(Name = "Updated By")]
        public string UpdatedBy { get; set; }

        public static implicit operator OutSourcingDesignation(DesignationView dv)
        {
            return new OutSourcingDesignation
            {
                DesignationCode = dv.DesignationCode,
                DesignationName = dv.DesignationName,
                LowerAgeLimit = dv.LowerAgeLimit,
                UpperAgeLimit = dv.UpperAgeLimit,
                Qualifications = dv.Qualifications,
                Experience = dv.Experience,
            };
        }
        public static implicit operator DesignationView (OutSourcingDesignation om)
        {
            return new DesignationView
            {
                DesignationCode = om.DesignationCode,
                DesignationName = om.DesignationName,
                LowerAgeLimit = om.LowerAgeLimit,
                UpperAgeLimit = om.UpperAgeLimit,
                Qualifications = om.Qualifications,
                Experience = om.Experience,
            };
        }
    }
}