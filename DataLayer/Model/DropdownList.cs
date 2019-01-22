using DataLayer.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DataLayer.Model
{
    public class DropdownList
    {
        public string CandidateID { get; set; }

        public IEnumerable<SelectListItem> CandidateIDs { get; set; }
        public List<SelectListItem> CandidateIDList()
        {
            using (RecruitEntities rec = new RecruitEntities())
            {
                List<SelectListItem> clist = rec.OutsourcingEmployeeDetails.Select(m => new SelectListItem { Value = m.CandidateID, Text = m.CandidateID }).ToList();
                return clist;
            }
        }
        public string Designation { get; set; }
        public IEnumerable<SelectListItem> Designations { get; set; }
        public List<SelectListItem> DesignationList()
        {
            using (RecruitEntities rec = new RecruitEntities())
            {
                List<SelectListItem> dlist= rec.OutSourcingDesignations.Select(em => new SelectListItem { Text = em.DesignationName, Value = em.DesignationCode }).ToList();
                return dlist;
            }
        }
    }
}
