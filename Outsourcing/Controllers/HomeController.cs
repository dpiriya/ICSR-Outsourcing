using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Outsourcing.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (!string.IsNullOrEmpty(Convert.ToString(Session["UserName"])))
            {
                ViewBag.Message = "Recruitment - Home";
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public ActionResult Masters()
        {
            ViewBag.Message = "Reference Tables";
            return View();
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


    }
}
