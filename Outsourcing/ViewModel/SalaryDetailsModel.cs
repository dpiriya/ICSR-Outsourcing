using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Outsourcing.ViewModel
{
    public class SalaryDetailsModel
    {
        public SalaryDetailsView salaryDetailsView { get; set; }

        public SalaryDetailsPartView SalaryDetailsPartView { get; set; }

        public SalaryDetailsNewView SalaryDetailsNewView { get; set; }
        public string type { get; set; }
        public string EmployeeName { get; set; }
        public bool PH { get; set; }
        public string desig { get; set; }
    }
}