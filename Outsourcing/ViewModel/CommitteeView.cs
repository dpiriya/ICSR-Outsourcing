using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
namespace Outsourcing.ViewModel
{
    public class CommitteeView
    {
        [Required]
        [Display(Name = "Meeting Date")]
        [DataType(DataType.Date),DisplayFormat(DataFormatString  = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> MeetingDate { get; set; }

        [Required]
        [Display(Name = "Authority")]
        public string Authority { get; set; }
    }
}