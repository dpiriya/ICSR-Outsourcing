using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using DataLayer.Repository;

namespace Outsourcing.ViewModel
{
    public class SalaryDetailsView
    {
        [Display(Name = "Employee ID")]
        public string EmployeeID { get; set; }
        //[Display(Name = "Employee Name")]
        //public string EmployeeName { get; set; }

        [Display(Name = "Order ID")]
        public int OrderID { get; set; }

        [Display(Name = "Order Type")]
        public string OrderType { get; set; }

        [Required]
        [Display(Name = "Basic Salary")]
        public decimal BasicSalary { get; set; }

        [Required]
        [Display(Name = "House Rent Allowance")]
        public decimal HRA { get; set; }

        [Required]
        [Display(Name = "Bonus")]
        public decimal Bonus { get; set; }

        [Required]
        [Display(Name = "Special Allowance")]
        public decimal SpecialAllowance { get; set; }

        [Required]
        [Display(Name = "Gross Salary")]
        public decimal GrossSalary { get; set; }

        [Required]
        [Display(Name = "Employee PF")]
        public decimal EmployeePF { get; set; }

        [Required]
        [Display(Name = "Employee ESIC")]
        public decimal EmployeeESIC { get; set; }

        [Required]
        [Display(Name = "Professional Tax")]
        public decimal ProfessionalTax { get; set; }

        [Required]
        [Display(Name = "Total Deduction")]
        public decimal TotalDeduction { get; set; }
        
        [Required]
        [Display(Name = "Net Salary")]
        public decimal NetSalary { get; set; }

        [Required]
        [Display(Name = "Employer PF")]
        public decimal EmployerPF { get; set; }

        [Required]
        [Display(Name = "Employer ESIC")]
        public decimal EmployerESIC { get; set; }

        [Required]
        [Display(Name = "Insurance")]
        public decimal Insurance { get; set; }

        [Required]
        [Display(Name = "Total Contribution")]
        public decimal TotalContribution { get; set; }

        [Required]
        [Display(Name = "Cost To Company (CTC) Per Month")]
        public decimal GrossTotal { get; set; }

        [Required]
        [Display(Name = "Agency Fee")]
        public decimal AgencyFee { get; set; }

        [Required]
        [Display(Name = "CTC with Agency Fee")]
        public decimal GrossTotalwithAgencyFee { get; set; }

        [Required]
        [Display(Name = "Service Tax")]
        public decimal ServiceTax { get; set; }

        [Required]
        [Display(Name = "Cost To Project(CTP) Per Month")]
        public decimal TotalSalary { get; set; }
        //public bool PH { get; set; }
        //public string type { get; set; }
        //public string desig { get; set; }

        public static implicit operator SalaryDetailsView(SalaryDetail sd)
        {
            return new SalaryDetailsView { 
                EmployeeID=sd.EmployeeID,
                OrderID=sd.OrderID,
                OrderType=sd.OrderType,
                BasicSalary=sd.BasicSalary,
                HRA=sd.HRA,
                Bonus=sd.Bonus,
                SpecialAllowance=sd.SpecialAllowance,
                GrossSalary=sd.GrossSalary,
                EmployeePF=sd.EmployeePF,
                EmployeeESIC=sd.EmployerESIC,
                ProfessionalTax=sd.ProfessionalTax,
                NetSalary=sd.NetSalary,
                EmployerPF=sd.EmployerPF,
                EmployerESIC=sd.EmployerESIC,
                Insurance=sd.Insurance,
                TotalContribution=sd.TotalContribution,
                GrossTotal=sd.GrossTotal,
                AgencyFee=sd.AgencyFee,
                ServiceTax=sd.ServiceTax,
                TotalSalary=sd.TotalSalary
            };
        }
        public static implicit operator SalaryDetail (SalaryDetailsView sdv)
        {
            return new SalaryDetail
            {
                EmployeeID = sdv.EmployeeID,
                OrderID = sdv.OrderID,
                OrderType = sdv.OrderType,
                BasicSalary = sdv.BasicSalary,
                HRA = sdv.HRA,
                Bonus = sdv.Bonus,
                SpecialAllowance = sdv.SpecialAllowance,
                GrossSalary = sdv.GrossSalary,
                EmployeePF = sdv.EmployeePF,
                EmployeeESIC = sdv.EmployerESIC,
                ProfessionalTax = sdv.ProfessionalTax,
                NetSalary = sdv.NetSalary,
                EmployerPF = sdv.EmployerPF,
                EmployerESIC = sdv.EmployerESIC,
                Insurance = sdv.Insurance,
                TotalContribution = sdv.TotalContribution,
                GrossTotal = sdv.GrossTotal,
                AgencyFee = sdv.AgencyFee,
                ServiceTax = sdv.ServiceTax,
                TotalSalary = sdv.TotalSalary
            };
        }

    }
}