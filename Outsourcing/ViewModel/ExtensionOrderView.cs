﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataLayer.Repository;

namespace Outsourcing.ViewModel
{
    public class ExtensionOrderView
    {
        public AppointmentDetailsView appointmentDetailsView { get; set; }
        public SalaryDetailsView salaryDetailsView { get; set; }
        public string Command { get; set; }
    }
}