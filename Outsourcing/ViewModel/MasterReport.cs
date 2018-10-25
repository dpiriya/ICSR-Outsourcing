using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Outsourcing.ViewModel
{
    public class MasterReport
    {
       //public IEnumerable<AppointmentMasterView> am;
       // AppointmentProjectView ap;
       // OutsourcingMeetingView om;
       // OutsourcingEmployeeDetailsView oe;

       public List<SelectListItem> ap { get; set; }
        public List<string> ap1 { get; set; }
        public List<SelectListItem> am { get; set; }
        public  List<string> am1 { get; set; }
        public List<SelectListItem> ad { get; set; }
        public List<string> ad1 { get; set; }
        public List<SelectListItem> om { get; set; }
        public List<string> om1 { get; set; }
        public List<SelectListItem> oe { get; set;}
        public List<string> oe1 { get; set; }
        public List<SelectListItem> Sal { get; set; }
        public List<string> sal1 { get; set; }
        public List<SelectListItem> DBList { get; set; }       
        public List<string> DBList1 { get; set; }


    }
}