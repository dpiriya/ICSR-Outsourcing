using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Outsourcing.ViewModel
{
    public class NewAppointmentView
    {
        public AppointmentMasterView appointmentMasterView {get; set;}
        public AppointmentProjectView appointmentProjectView {get; set;}
        public AppointmentDetailsView appointmentDetailsView {get; set;}
        public bool PH { get; set;}
        public SalaryDetailsView salaryDetailsView { get; set;}
        public string Command { get; set; }
    }
}