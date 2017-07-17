using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using Outsourcing.Models;

namespace Outsourcing.ViewModel
{
    public class VariousReportView
    {
        [Required(ErrorMessage ="Select Appointment Status")]
        [Display(Name ="Appointment Status")]
        public string AppointmentStatus { get; set; }
        public IEnumerable<SelectListItem> AppointmentStatuskal { get; set; }

        [Display(Name = "Employee Number")]
        public string EmployeeNo { get; set; }
            
        [Display(Name = "Employee Name")]
        public string EmployeeName { get; set; }

        [Display(Name = "Department")]
        public string Department { get; set; }
        public IEnumerable<SelectListItem> Departments { get; set; }

        [Display(Name = "Project Type")]
        public string ProjectType { get; set; }
        public IEnumerable<SelectListItem>ProjectTypes { get; set; }

        [Display(Name = "Project Number")]
        public string ProjectNumber { get; set; }
        public IEnumerable<SelectListItem> ProjectNumbers { get; set; }

        [Display(Name = "Coordinator Name")]
        public string CoorName { get; set; }
        public IEnumerable<SelectListItem> CoorNames { get; set; }

        [Display(Name = "Designation")]
        public string Designation { get; set; }
        public IEnumerable<SelectListItem> Designations { get; set; }

        [Display(Name = "Required Document Format")]
        public string DocumentFormat { get; set; }
        public IEnumerable<SelectListItem> DocumentFormats { get; set; }


        public List<SelectListItem> AppointmentStatusList()
        {
            List<SelectListItem> al = new List<SelectListItem>() {
                new SelectListItem { Text = "Ongoing", Value = "Ongoing"},
                new SelectListItem { Text = "Project Relieved", Value = "ProjectRelieved"},
                new SelectListItem { Text = "Agency Relieved", Value = "AgencyRelieved"}
            };
            return al;
        }
        public List<SelectListItem> DepartmentList()
        {
            using (RecruitEntities recruit = new RecruitEntities())
            {
                List<string> apl = recruit.AppointmentProjects.GroupBy(em => em.DepartmentCode).Select(g=> g.Key).ToList();
                List<SelectListItem> dl = (from p in recruit.Department() where apl.Contains(p.DepartmentCode) orderby p.DepartmentCode select new SelectListItem { Text = p.DepartmentCode + " - " + p.DepartmentName, Value = p.DepartmentCode }).ToList();
                return dl;
            }
        }
        public List<SelectListItem> ProjectTypeList()
        {
            using (RecruitEntities recruit = new RecruitEntities())
            {
                List<string> apl = recruit.AppointmentProjects.GroupBy(em => em.ProjectType).Select(g => g.Key).ToList();
                List<SelectListItem> ptl = recruit.ListItemMasters.Where(l => l.ListName == "ProjectType" && apl.Contains(l.ListItemValue)).OrderBy(l=>l.ListItemValue).Select(l => new SelectListItem { Text = l.ListItemValue + " - " + l.ListItemText, Value = l.ListItemValue }).ToList();
                return ptl;
            }
        }
        public List<SelectListItem> ProjectNumberList()
        {
            using (RecruitEntities recruit = new RecruitEntities())
            {
                List<SelectListItem> pl = recruit.AppointmentProjects.GroupBy(em => em.ProjectNo).OrderBy(g => g.Key).Select(g => new SelectListItem { Text = g.Key, Value = g.Key }).ToList();
                return pl;
            }
        }
        public List<SelectListItem> CoorNameList()
        {
            using (RecruitEntities recruit = new RecruitEntities())
            {
                //modified by priya
                var apl = recruit.AppointmentProjects.GroupBy(em =>new { em.PICode, em.PIName }).OrderBy(g => g.Key).Select(g => g.Key).ToList();
                return apl.Select(a => new SelectListItem() { Text = a.PICode + "-" + a.PIName, Value = a.PICode }).ToList();
                //original
                //List<string> apl = recruit.AppointmentProjects.GroupBy(em => em.PICode).OrderBy(g=>g.Key).Select(g => g.Key).ToList();
                //List<SelectListItem> cl = new List<SelectListItem>();
                //modified by priya
                //apl.ForEach(delegate (string piCode)
                //{
                //    cl.Add(recruit.Coordinator(piCode).Select(em => new SelectListItem { Text = em.CoorCode + " - " + em.CoorName, Value = em.CoorCode }).FirstOrDefault());
                //});

                //return null; 
            }
        }
        public List<SelectListItem> DesignationList()
        {
            using (RecruitEntities recruit = new RecruitEntities())
            {
                List<string> apl = recruit.AppointmentProjects.GroupBy(em => em.DesignationCode).OrderBy(g => g.Key).Select(g => g.Key).ToList();
                List<SelectListItem> dl = recruit.OutSourcingDesignations.Where(em => apl.Contains(em.DesignationCode)).Select(em => new SelectListItem { Text = em.DesignationCode + " - " + em.DesignationName, Value = em.DesignationCode }).ToList();
                return dl;
            }
        }
        public List<SelectListItem> DocumentFormatList()
        {
            List<SelectListItem> al = new List<SelectListItem>() {
                new SelectListItem { Text = "PDF", Value = "PDF"},
                new SelectListItem { Text = "Excel (xls)", Value = "XLS"},
                new SelectListItem { Text = "Excel (xlsx)", Value = "XLSX"},
                new SelectListItem { Text = "Word (doc)", Value = "DOC"},
                new SelectListItem { Text = "Word (docx)", Value = "DOCX"}
            };
            return al;
        }
    }
}