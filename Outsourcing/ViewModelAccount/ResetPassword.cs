using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using WebMatrix.Data;

namespace Outsourcing.ViewModelAccount
{
    public class ResetPassword
    {
        [Required(ErrorMessage ="Select User Name")]
        [Display(Name = "Select User")]
        public string SelectedUser { get; set; }

        [Required(ErrorMessage = "Enter New Password")]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Enter Confirm New Password")]
        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("NewPassword")]
        [Display(Name = "Confirm New Password")]
        public string ConfirmNewPassword { get; set; }
        public IEnumerable<SelectListItem> Users { get; set; }

        public static SelectList UserList()
        {
            using (var db = Database.Open("ApplicationServices"))
            {
                var Ul = db.Query("select * from UserProfile");
                return new SelectList(Ul, "UserName", "UserName");
            }
        }
    }
}