using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Outsourcing.ViewModel
{
    public class EnhancementOrderView
    {
        public AppointmentDetailsView appointmentDetailsView { get; set; }
        public SalaryDetailsView salaryDetailsView { get; set; }
        public Nullable<System.DateTime> PreviousFromDate { get; set; }
        public string Command { get; set; }
    }
}