using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Outsourcing.Models;
using Outsourcing.ViewModel;

namespace Outsourcing.BusinessModel
{
    public class SalaryCalculation
    {
        public void TAMsalary(decimal GValue, string Designation,bool PH, out SalaryDetailsView sdv)
        {
            using( RecruitEntities recruit= new RecruitEntities ())
            {
                var payStrut = recruit.PayStructures.Where(em => em.EffectiveDate <= DateTime.Today).Select(em => new { HeadName=em.HeadName,HeadValue= em.HeadValue,Unit=em.Unit }).ToList();
                SalaryDetailsView sd= new SalaryDetailsView ();
                decimal BasicPercent = Convert.ToDecimal(payStrut.Where(em => em.HeadName == "BasicSalary").Select(em => em.HeadValue).First());
                decimal BasicSalary = Math.Round( GValue * BasicPercent / 100);
                decimal HRA;
                if (Designation == "OA") HRA = 1500; else HRA = 2000;
                decimal BonusPercent = Convert.ToDecimal(payStrut.Where(em => em.HeadName == "Bonus").Select(em => em.HeadValue).First());
                decimal Bonus = Math.Round(GValue * BonusPercent / 100);
                decimal SpecialAllowance = GValue - ( Math.Round(BasicSalary) + HRA + Bonus);
                sd.BasicSalary = BasicSalary;
                sd.HRA = HRA;
                sd.Bonus = Bonus;
                sd.SpecialAllowance = SpecialAllowance;
                if (BasicSalary + HRA + Bonus + SpecialAllowance == GValue) sd.GrossSalary = GValue;
                decimal EmployeePFPercent = Convert.ToDecimal(payStrut.Where(em => em.HeadName == "EmployeePF").Select(em => em.HeadValue).First());
                decimal EmployeePF = Math.Round(Math.Round(BasicSalary * EmployeePFPercent / 100,2));
                decimal EmployeeESIC;
                //if (GValue <= 15000)
               
                if (GValue <= 21000 || (GValue <= 25000 && PH == true))
                {
                    decimal EmployeeESICPercent = Convert.ToDecimal(payStrut.Where(em => em.HeadName == "EmployeeESIC").Select(em => em.HeadValue).First());
                    EmployeeESIC = Math.Ceiling(GValue * EmployeeESICPercent / 100);
                }               
                else
                {
                    EmployeeESIC = 0;
                }
                var pTax = (from p in recruit.ProfessionalTaxes where p.EffectiveDate <= DateTime.Today group p by new 
                    {PayRangeFrom=p.PayRangeFrom, PayRangeTo=p.PayRangeTo } into g 
                    select new {PayRangeFrom=g.Key.PayRangeFrom, PayRangeTo=g.Key.PayRangeTo, EffectiveDate= g.Max(q=>q.EffectiveDate)}).ToList();
                decimal ProfessionalTax=0;
                if (pTax.Any())
                {
                    for (int i = 0; i < pTax.Count; i++)
                    {
                        if(pTax[i].PayRangeFrom <= GValue && pTax[i].PayRangeTo >= GValue )
                        {
                            DateTime eDate = pTax[i].EffectiveDate;
                            decimal pFrom = pTax[i].PayRangeFrom;
                            decimal pTo = pTax[i].PayRangeTo;
                            ProfessionalTax = Convert.ToDecimal((from p in recruit.ProfessionalTaxes where p.EffectiveDate == eDate && p.PayRangeFrom == pFrom && p.PayRangeTo == pTo select p.ProfessionalTax1).First());
                            break;
                        }
                    }
                    //ProfessionalTax = Convert.ToDecimal(pTax.Select(em => em.PTax).First());
                }
                decimal TotalDeduction = EmployeePF + EmployeeESIC + ProfessionalTax;
                decimal NetSalary = GValue - TotalDeduction;
                sd.EmployeePF = EmployeePF;
                sd.EmployeeESIC = EmployeeESIC;
                sd.ProfessionalTax = ProfessionalTax;
                sd.TotalDeduction = TotalDeduction;
                sd.NetSalary = NetSalary;
                decimal EmployerPFPercent = Convert.ToDecimal(payStrut.Where(em => em.HeadName == "EmployerPF").Select(em => em.HeadValue).First());
                decimal EmployerPF = Math.Round(Math.Round(BasicSalary * EmployerPFPercent / 100,2));
                decimal EmployerESIC;
                //if (GValue <= 15000)
                if (GValue <= 21000 || (GValue <= 25000 && PH == true))
                {
                    decimal EmployerESICPercent = Convert.ToDecimal(payStrut.Where(em => em.HeadName == "EmployerESIC").Select(em => em.HeadValue).First());
                    EmployerESIC = Math.Ceiling(GValue * EmployerESICPercent / 100);
                }
                else
                {
                    EmployerESIC = 0;
                }
                decimal Insurance = 200;
                decimal Contribution = EmployerPF + EmployerESIC + Insurance;
                decimal GrossTotal = GValue + Contribution;
                sd.EmployerPF = EmployerPF;
                sd.EmployerESIC = EmployerESIC;
                sd.Insurance = Insurance;
                sd.TotalContribution = Contribution;
                sd.GrossTotal = GrossTotal;
                decimal AgencyFeePercent = Convert.ToDecimal(payStrut.Where(em => em.HeadName == "AgencyFee").Select(em => em.HeadValue).First());
                decimal AgencyFee = GrossTotal * AgencyFeePercent / 100;
                decimal GrossTotalWithAgencyFee = GrossTotal + AgencyFee;
                decimal ServiceTaxPercent = Convert.ToDecimal(payStrut.Where(em => em.HeadName == "ServiceTax").Select(em => em.HeadValue).First());
                decimal ServiceTax = GrossTotalWithAgencyFee * ServiceTaxPercent / 100;
                decimal TotalSalary = GrossTotalWithAgencyFee + ServiceTax;
                sd.AgencyFee=Math.Round(AgencyFee);
                sd.GrossTotalwithAgencyFee = Math.Round(GrossTotalWithAgencyFee);
                sd.ServiceTax = Math.Round(ServiceTax);
                sd.TotalSalary = Math.Round(TotalSalary);
                sdv = sd;
            }
        }

        public void TAMsalaryWithoutPF(decimal GValue, string Designation,bool PH, out SalaryDetailsView sdv)
        {
            using (RecruitEntities recruit = new RecruitEntities())
            {
                var payStrut = recruit.PayStructures.Where(em => em.EffectiveDate <= DateTime.Today).Select(em => new { HeadName = em.HeadName, HeadValue = em.HeadValue, Unit = em.Unit }).ToList();
                SalaryDetailsView sd = new SalaryDetailsView();
                decimal BasicPercent = Convert.ToDecimal(payStrut.Where(em => em.HeadName == "BasicSalary").Select(em => em.HeadValue).First());
                decimal BasicSalary = Math.Round(GValue * BasicPercent / 100);
                decimal HRA;
                if (Designation == "OA") HRA = 1500; else HRA = 2000;
                decimal BonusPercent = Convert.ToDecimal(payStrut.Where(em => em.HeadName == "Bonus").Select(em => em.HeadValue).First());
                decimal Bonus = Math.Round(GValue * BonusPercent / 100);
                decimal SpecialAllowance = GValue - (Math.Round(BasicSalary) + HRA + Bonus);
                //decimal BasicSalary = GValue;
                //decimal HRA=0;
                //decimal Bonus = 0;
                //decimal SpecialAllowance = 0;
                sd.BasicSalary = BasicSalary;
                sd.HRA = HRA;
                sd.Bonus = Bonus;
                sd.SpecialAllowance = SpecialAllowance;
                if (BasicSalary + HRA + Bonus + SpecialAllowance == GValue) sd.GrossSalary = GValue;
                decimal EmployeePF = 0;
                decimal EmployeeESIC;
                
                if (GValue <= 21000 || (GValue <= 25000 && PH == true))
                {
                    decimal EmployeeESICPercent = Convert.ToDecimal(payStrut.Where(em => em.HeadName == "EmployeeESIC").Select(em => em.HeadValue).First());
                    EmployeeESIC = Math.Ceiling(GValue * EmployeeESICPercent / 100);
                }
                else
                {
                    EmployeeESIC = 0;
                }
                
                var pTax = (from p in recruit.ProfessionalTaxes
                            where p.EffectiveDate <= DateTime.Today
                            group p by new { PayRangeFrom = p.PayRangeFrom, PayRangeTo = p.PayRangeTo } into g
                            select new { PayRangeFrom = g.Key.PayRangeFrom, PayRangeTo = g.Key.PayRangeTo, EffectiveDate = g.Max(q => q.EffectiveDate) }).ToList();
                decimal ProfessionalTax = 0;
                if (pTax.Any())
                {
                    for (int i = 0; i < pTax.Count; i++)
                    {
                        if (pTax[i].PayRangeFrom <= GValue && pTax[i].PayRangeTo >= GValue)
                        {
                            DateTime eDate = pTax[i].EffectiveDate;
                            decimal pFrom = pTax[i].PayRangeFrom;
                            decimal pTo = pTax[i].PayRangeTo;
                            ProfessionalTax = Convert.ToDecimal((from p in recruit.ProfessionalTaxes where p.EffectiveDate == eDate && p.PayRangeFrom == pFrom && p.PayRangeTo == pTo select p.ProfessionalTax1).First());
                            break;
                        }
                    }
                    //ProfessionalTax = Convert.ToDecimal(pTax.Select(em => em.PTax).First());
                }
                decimal TotalDeduction = EmployeePF + EmployeeESIC + ProfessionalTax;
                decimal NetSalary = GValue - TotalDeduction;
                sd.EmployeePF = EmployeePF;
                sd.EmployeeESIC = EmployeeESIC;
                sd.ProfessionalTax = ProfessionalTax;
                sd.TotalDeduction = TotalDeduction;
                sd.NetSalary = NetSalary;
                decimal EmployerPF = 0;
                decimal EmployerESIC;
                if (GValue <= 21000 || (GValue <= 25000 && PH == true))
                {
                    decimal EmployerESICPercent = Convert.ToDecimal(payStrut.Where(em => em.HeadName == "EmployerESIC").Select(em => em.HeadValue).First());
                    EmployerESIC = Math.Ceiling(GValue * EmployerESICPercent / 100);
                }
                else
                {
                    EmployerESIC = 0;
                }
                decimal Insurance = 200;
                decimal Contribution = EmployerPF + EmployerESIC + Insurance;
                decimal GrossTotal = GValue + Contribution;
                sd.EmployerPF = EmployerPF;
                sd.EmployerESIC = EmployerESIC;
                sd.Insurance = Insurance;
                sd.TotalContribution = Contribution;
                sd.GrossTotal = GrossTotal;
                decimal AgencyFeePercent = Convert.ToDecimal(payStrut.Where(em => em.HeadName == "AgencyFee").Select(em => em.HeadValue).First());
                decimal AgencyFee = GrossTotal * AgencyFeePercent / 100;
                decimal GrossTotalWithAgencyFee = GrossTotal + AgencyFee;
                decimal ServiceTaxPercent = Convert.ToDecimal(payStrut.Where(em => em.HeadName == "ServiceTax").Select(em => em.HeadValue).First());
                decimal ServiceTax = GrossTotalWithAgencyFee * ServiceTaxPercent / 100;
                decimal TotalSalary = GrossTotalWithAgencyFee + ServiceTax;
                sd.AgencyFee = Math.Round(AgencyFee);
                sd.GrossTotalwithAgencyFee = Math.Round(GrossTotalWithAgencyFee);
                sd.ServiceTax = Math.Round(ServiceTax);
                sd.TotalSalary = Math.Round(TotalSalary);
                sdv = sd;
            }
        }
        public void TAMsalaryBasic(decimal Basic, SalaryDetailsView sdv,out SalaryDetailsView outSD)
        {
            using (RecruitEntities recruit = new RecruitEntities())
            {
                var payStrut = recruit.PayStructures.Where(em => em.EffectiveDate <= DateTime.Today).Select(em => new { HeadName = em.HeadName, HeadValue = em.HeadValue, Unit = em.Unit }).ToList();
                SalaryDetailsView sd = new SalaryDetailsView();
                sd.BasicSalary = Basic;
                sd.HRA = sdv.HRA;
                sd.Bonus = sdv.Bonus;
                sd.SpecialAllowance = sdv.SpecialAllowance;
                sd.GrossSalary = sd.BasicSalary + sd.HRA + sd.Bonus + sd.SpecialAllowance;
                decimal EmployeePF = 0;
                decimal empPF;
                Boolean pfStatus = decimal.TryParse(Convert.ToString(sdv.EmployeePF), out empPF);
                if (sdv.EmployeePF > 0 && pfStatus)
                {
                    decimal EmployeePFPercent = Convert.ToDecimal(payStrut.Where(em => em.HeadName == "EmployeePF").Select(em => em.HeadValue).First());
                    EmployeePF = Math.Round(Math.Round(sd.BasicSalary * EmployeePFPercent / 100, 2));
                }                
                decimal EmployeeESIC=0;
                //if (sd.GrossSalary <= 15000)
                if (sd.GrossSalary <= 21000)
                {
                    decimal EmployeeESICPercent = Convert.ToDecimal(payStrut.Where(em => em.HeadName == "EmployeeESIC").Select(em => em.HeadValue).First());
                    EmployeeESIC = Math.Ceiling(sd.GrossSalary * EmployeeESICPercent / 100);
                }
                var pTax = (from p in recruit.ProfessionalTaxes
                            where p.EffectiveDate <= DateTime.Today
                            group p by new { PayRangeFrom = p.PayRangeFrom, PayRangeTo = p.PayRangeTo } into g
                            select new { PayRangeFrom = g.Key.PayRangeFrom, PayRangeTo = g.Key.PayRangeTo, EffectiveDate = g.Max(q => q.EffectiveDate) }).ToList();
                decimal ProfessionalTax = 0;
                if (pTax.Any())
                {
                    for (int i = 0; i < pTax.Count; i++)
                    {
                        if (pTax[i].PayRangeFrom <= sd.GrossSalary && pTax[i].PayRangeTo >= sd.GrossSalary)
                        {
                            DateTime eDate = pTax[i].EffectiveDate;
                            decimal pFrom = pTax[i].PayRangeFrom;
                            decimal pTo = pTax[i].PayRangeTo;
                            ProfessionalTax = Convert.ToDecimal((from p in recruit.ProfessionalTaxes where p.EffectiveDate == eDate && p.PayRangeFrom == pFrom && p.PayRangeTo == pTo select p.ProfessionalTax1).First());
                            break;
                        }
                    }
                    //ProfessionalTax = Convert.ToDecimal(pTax.Select(em => em.PTax).First());
                }
                decimal TotalDeduction = EmployeePF + EmployeeESIC + ProfessionalTax;
                decimal NetSalary = sd.GrossSalary - TotalDeduction;
                sd.EmployeePF = EmployeePF;
                sd.EmployeeESIC = EmployeeESIC;
                sd.ProfessionalTax = ProfessionalTax;
                sd.TotalDeduction = TotalDeduction;
                sd.NetSalary = NetSalary;
                decimal EmployerPF=0;
                if (sdv.EmployerPF > 0 && pfStatus)
                {
                    decimal EmployerPFPercent = Convert.ToDecimal(payStrut.Where(em => em.HeadName == "EmployerPF").Select(em => em.HeadValue).First());
                    EmployerPF = Math.Round(Math.Round( sd.BasicSalary * EmployerPFPercent / 100, 2));
                }
                decimal EmployerESIC=0;
                //if (sd.GrossSalary <= 15000)
                if (sd.GrossSalary <= 21000)
                {
                    decimal EmployerESICPercent = Convert.ToDecimal(payStrut.Where(em => em.HeadName == "EmployerESIC").Select(em => em.HeadValue).First());
                    EmployerESIC = Math.Ceiling(sd.GrossSalary * EmployerESICPercent / 100);
                }
                decimal Insurance = 200;
                decimal Contribution = EmployerPF + EmployerESIC + Insurance;
                decimal GrossTotal = sd.GrossSalary + Contribution;
                sd.EmployerPF = EmployerPF;
                sd.EmployerESIC = EmployerESIC;
                sd.Insurance = Insurance;
                sd.TotalContribution = Contribution;
                sd.GrossTotal = GrossTotal;
                decimal AgencyFeePercent = Convert.ToDecimal(payStrut.Where(em => em.HeadName == "AgencyFee").Select(em => em.HeadValue).First());
                decimal AgencyFee = GrossTotal * AgencyFeePercent / 100;
                decimal GrossTotalWithAgencyFee = GrossTotal + AgencyFee;
                decimal ServiceTaxPercent = Convert.ToDecimal(payStrut.Where(em => em.HeadName == "ServiceTax").Select(em => em.HeadValue).First());
                decimal ServiceTax = GrossTotalWithAgencyFee * ServiceTaxPercent / 100;
                decimal TotalSalary = GrossTotalWithAgencyFee + ServiceTax;
                sd.AgencyFee = Math.Round(AgencyFee);
                sd.GrossTotalwithAgencyFee = Math.Round(GrossTotalWithAgencyFee);
                sd.ServiceTax = Math.Round(ServiceTax);
                sd.TotalSalary = Math.Round(TotalSalary);
                outSD = sd;
            }
        }
    }
}