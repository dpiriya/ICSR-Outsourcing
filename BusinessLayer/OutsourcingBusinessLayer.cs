using DataLayer.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class OutsourcingBusinessLayer
    {
         public List<SelectListItem> Getdesig()
        {
            using (RecruitEntities recruit = new RecruitEntities())
            {
                var desig=recruit.OutSourcingDesignations.Select(em => new SelectListItem { Text = em.DesignationName, Value = em.DesignationCode }).ToList();
                return desig;
            }
        }
        public List<SelectListItem> GetCID()
        {
            using(RecruitEntities recruit=new RecruitEntities())
            {
                var CID = recruit.OutsourcingEmployeeDetails.Select(m =>new SelectListItem { Text = m.CandidateID, Value = m.CandidateID }).ToList();
                return CID;
            }
        }
        
        }
}
