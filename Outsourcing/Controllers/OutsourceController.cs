using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Outsourcing.ViewModel;
using System.Data;
using System.Web.Security;
using System.Data.SqlClient;
using System.Configuration;
using Outsourcing.Datasets;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using System.Text;
using PagedList;
using System.Transactions;
using Outsourcing.BusinessModel;
using CrystalDecisions.Shared;
using DataLayer.Repository;
using System.Data.Entity;
using BusinessLayer;

namespace Outsourcing.Controllers
{
    [DenyByController(Users ="tnm")]
    public class OutsourceController : Controller
    {
        //
        // GET: /Outsource/
        OutsourcingBusinessLayer bl = new OutsourcingBusinessLayer();


        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        #region Designation

        [HttpGet]
        public ActionResult DesignationList()
        {
            using (var RecruitEntity = new DataLayer.Repository.RecruitEntities())
            {
                List<DesignationView> desigView = RecruitEntity.OutSourcingDesignations.Select(em => new DesignationView { DesignationCode = em.DesignationCode, DesignationName = em.DesignationName, LowerAgeLimit = em.LowerAgeLimit, UpperAgeLimit = em.UpperAgeLimit, Qualifications = em.Qualifications, Experience = em.Experience }).ToList();
                return View(desigView);
            }
        }

        [HttpGet]
        public ActionResult DesignationInsert()
        {
            DesignationView DesigInsert = new DesignationView();
            return View(DesigInsert);
        }

        [HttpPost]
        public ActionResult DesignationInsert(DesignationView dv)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    OutSourcingDesignation Desig = dv;
                    Desig.CreatedBy = Membership.GetUser().UserName.ToString();
                    Desig.CreatedOn = DateTime.Today;
                    using (RecruitEntities Recruit = new RecruitEntities())
                    {
                        Recruit.OutSourcingDesignations.Add(Desig);
                        Recruit.SaveChanges();
                    }
                    ViewData["result"] = "True";
                    ModelState.Clear();
                    return View();
                }
                else
                {
                    ModelState.AddModelError("", "Validation Failed");
                    return View();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }

        [HttpPost]
        [MultipleButton(Name = "action", Argument = "DesignationEdit")]
        public ActionResult DesignationEdit(string id)
        {
            using (RecruitEntities Recruit = new RecruitEntities())
            {
                OutSourcingDesignation osd = new OutSourcingDesignation();
                osd = Recruit.OutSourcingDesignations.Where(em => em.DesignationCode == id).First();
                DesignationView dv = osd;
                return View(dv);
            }
        }

        [HttpPost]
        public ActionResult DesignationEdit(DesignationView dv)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    OutSourcingDesignation Desig = dv;
                    Desig.UpdatedBy = Membership.GetUser().UserName.ToString();
                    Desig.UpdatedOn = DateTime.Today;
                    using (RecruitEntities Recruit = new RecruitEntities())
                    {
                        Recruit.Entry(Desig).State = EntityState.Modified;
                        Recruit.SaveChanges();
                    }
                    return RedirectToAction("DesignationList");
                }
                else
                {
                    ModelState.AddModelError("", "Validation Failed");
                    return View();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }

        [HttpPost]
        [MultipleButton(Name = "action", Argument = "DesignationDelete")]
        public ActionResult DesignationDelete(string id)
        {
            using (RecruitEntities Recruit = new RecruitEntities())
            {
                OutSourcingDesignation osd = Recruit.OutSourcingDesignations.Where(em => em.DesignationCode == id).First();
                Recruit.OutSourcingDesignations.Remove(osd);
                Recruit.SaveChanges();
                return RedirectToAction("DesignationList");
            }
        }
        [HttpPost]
        [MultipleButton(Name = "action", Argument = "DesignationDetails")]
        public ActionResult DesignationDetails(string id)
        {
            using (RecruitEntities RecruitEntity = new RecruitEntities())
            {
                OutSourcingDesignation desig = RecruitEntity.OutSourcingDesignations.Where(em => em.DesignationCode == id).First();
                DesignationView dv = desig;
                dv.CreatedOn = desig.CreatedOn;
                dv.CreatedBy = desig.CreatedBy;
                dv.UpdatedOn = desig.UpdatedOn;
                dv.UpdatedBy = desig.UpdatedBy;
                return View(dv);
            }
        }
        #endregion

        #region Outsourcing
        [HttpGet]
        public ActionResult OutsourcingModule()
        {
            return View();
        }
        #endregion

        #region salarycalcu
        public ActionResult SalaryCalculation()
        {
            List<SelectListItem> desig = bl.Getdesig();
            ViewData["Designation"] = desig;
            List<SelectListItem> CID = bl.GetCID();
            ViewData["CID"] = CID;
            return View();
        }
        [HttpPost]
        [MultipleButton(Name = "action", Argument = "SalaryCalculation")]
        public ActionResult SalaryCalculation(decimal RecommendSalary,string Eemp,string CID,string Name, string Design,string type, bool PH)
        {
            List<SelectListItem> desig = bl.Getdesig();
            ViewData["Designation"] = desig;
             List<SelectListItem> CaID = bl.GetCID();
            ViewData["CID"] = CaID;
            SalaryDetailsModel sdv;
            try
            {                SalaryCalculation sc = new SalaryCalculation();               
                if (Eemp=="PartTime" || Eemp == "New Employee")
                {                    
                    sc.salaryNew(RecommendSalary,Eemp,Name,PH,Design,out sdv);
                    
                }                
                else
                {
                    sc.salaryOld(RecommendSalary,CID,Name,PH,Design,Eemp, out sdv);
                    
                }
                return View(sdv);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
           
        }

        #endregion
        #region CandidateBank
        [HttpGet]
        public ActionResult EmployeeList(string SearchName, string dob, int? Page)
        {
            using (RecruitEntities RecruitEntity = new RecruitEntities())
            {
                List<OutsourcingEmployeeDetailsView> emp;
                DateTime sdob;
                string SearchDOB = null;
                if (!string.IsNullOrEmpty(dob))

                    if (dob.Length == 8)
                    {
                        SearchDOB = dob.Substring(0, 2) + "/" + dob.Substring(2, 2) + "/" + dob.Substring(4, 4);
                    }
                bool isDate = DateTime.TryParse(Convert.ToString(SearchDOB), out sdob);
                if (isDate && !string.IsNullOrEmpty(SearchName))
                    emp = RecruitEntity.OutsourcingEmployeeDetails.Where(em => em.CandidateName.Contains(SearchName) && em.DOB.Equals(sdob)).Select(em => new OutsourcingEmployeeDetailsView { CandidateID = em.CandidateID, CandidateName = em.CandidateName, DOB = em.DOB, FatherName = em.FatherName, PermanentAddress = em.PermanentAddress, MobileNumber = em.MobileNumber, EmailID = em.EmailID }).ToList();
                else if (string.IsNullOrEmpty(SearchName) && isDate)
                    emp = RecruitEntity.OutsourcingEmployeeDetails.Where(em => em.DOB.Equals(sdob)).Select(em => new OutsourcingEmployeeDetailsView { CandidateID = em.CandidateID, CandidateName = em.CandidateName, DOB = em.DOB, FatherName = em.FatherName, PermanentAddress = em.PermanentAddress, MobileNumber = em.MobileNumber, EmailID = em.EmailID }).ToList();
                else if (!string.IsNullOrEmpty(SearchName) && isDate == false)
                    emp = RecruitEntity.OutsourcingEmployeeDetails.Where(em => em.CandidateName.Contains(SearchName)).Select(em => new OutsourcingEmployeeDetailsView { CandidateID = em.CandidateID, CandidateName = em.CandidateName, DOB = em.DOB, FatherName = em.FatherName, PermanentAddress = em.PermanentAddress, MobileNumber = em.MobileNumber, EmailID = em.EmailID }).ToList();
                else
                    emp = RecruitEntity.OutsourcingEmployeeDetails.Select(em => new OutsourcingEmployeeDetailsView { CandidateID = em.CandidateID, CandidateName = em.CandidateName, DOB = em.DOB, FatherName = em.FatherName, PermanentAddress = em.PermanentAddress, MobileNumber = em.MobileNumber, EmailID = em.EmailID }).OrderByDescending(em => em.CandidateID).Take(10).ToList();
                ViewData["Status"] = TempData["Status"];
                ViewData["CandidateID"] = TempData["CandidateID"];
                return View(emp.ToPagedList(Page ?? 1, 10));
            }
        }

        [HttpGet]
        public ActionResult NewEmployee()
        {
            //CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
            OutsourcingEmployeeDetailsView ev = new OutsourcingEmployeeDetailsView();
            ev.Genders = ev.GenderList();
            ev.CasteCategories = ev.CasteList();
            ev.MStatus = ev.M_Status();
            ev.BankNameList = ev.bankList();
            ev.Command = "Insert";
            ViewBag.Title = "New Candidate";
            return View(ev);
        }
        [HttpPost]
        public ActionResult NewEmployee(OutsourcingEmployeeDetailsView emp)
        {
            //CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
            if (ModelState.IsValid)
            {
                try
                {
                    OutsourcingEmployeeDetail ose = emp;
                    if (emp.Command == "Insert")
                    {
                        using (RecruitEntities Recruit = new RecruitEntities())
                        {
                            List<string> nameList = (from m in Recruit.OutsourcingEmployeeDetails select m.CandidateName).ToList();
                            List<string> foundNameList = FuzzySearch.Search(ose.CandidateName.ToUpper(), nameList, 0.50);
                            List<string> foundNameList1 = FuzzySearch.Search(ose.CandidateName.ToUpper(), nameList, 0.70);
                            List<DateTime> dobList = (from m in Recruit.OutsourcingEmployeeDetails select m.DOB).ToList();
                            List<string> dobStrList = dobList.ConvertAll<string>(delegate (DateTime i) { return i.ToShortDateString(); });
                            List<string> foundDobList = FuzzySearch.Search(ose.DOB.ToShortDateString(), dobStrList, 0.80);
                            //List<string> foundDobList1 = FuzzySearch.Search(ose.DOB.ToShortDateString(), dobStrList, 0.70);
                            dobList = foundDobList.ConvertAll<DateTime>(delegate (string x) { return Convert.ToDateTime(x); });
                            // List<DateTime> dobList1 = foundDobList1.ConvertAll<DateTime>(delegate(string x) { return Convert.ToDateTime(x); });
                            List<OutsourcingEmployeeDetail> tmpOE = (from m in Recruit.OutsourcingEmployeeDetails where ((foundNameList.Contains(m.CandidateName) && m.DOB == ose.DOB) || (foundNameList1.Contains(m.CandidateName) && dobList.Contains(m.DOB)) || (m.PAN == ose.PAN && ose.PAN!=null) || m.MobileNumber == ose.MobileNumber ||( m.EmailID == ose.EmailID && ose.EmailID != null )|| (m.PF_UAN == ose.PF_UAN && ose.PF_UAN != null) || (m.ESIC_NO == ose.ESIC_NO && ose.ESIC_NO != null) || (m.BankAccountNo == ose.BankAccountNo && ose.BankAccountNo != null)) select m).ToList();
                            //List<OutsourcingEmployeeDetail> tmpOE = (from m in Recruit.OutsourcingEmployeeDetails where ((foundNameList.Contains(m.CandidateName) && dobList.Contains(m.DOB)) || (foundNameList1.Contains(m.CandidateName) && dobList.Contains(m.DOB)) || (m.PAN == ose.PAN && ose.PAN != null) || m.MobileNumber == ose.MobileNumber || (m.EmailID == ose.EmailID && ose.EmailID != null) || (m.PF_UAN == ose.PF_UAN && ose.PF_UAN != null) || (m.ESIC_NO == ose.ESIC_NO && ose.ESIC_NO != null) || (m.BankAccountNo == ose.BankAccountNo && ose.BankAccountNo != null)) select m).ToList();
                            if (tmpOE.Any())
                            {
                                string exMsg = "";
                                foreach (OutsourcingEmployeeDetail tt in tmpOE)
                                {
                                    if (exMsg == "") exMsg = tt.CandidateID + " -- " + tt.CandidateName; else exMsg = exMsg + ",\r\n" + tt.CandidateID + " -- " + tt.CandidateName;
                                }
                                throw new Exception("This entry already exists, Verify the following Candidate id's " + exMsg + " or  Contact Administrator");
                            }
                            int MaxVal;
                            if (Recruit.OutsourcingEmployeeDetails.Count() == 0) MaxVal = 0;
                            else
                                MaxVal = Convert.ToInt32(Recruit.OutsourcingEmployeeDetails.Max(em => em.CandidateID).Substring(3, 4));
                            MaxVal = MaxVal + 1;
                            string strMaxVal = Convert.ToString(MaxVal);
                            if (strMaxVal.Length == 1) strMaxVal = "CID000" + strMaxVal;
                            else if (strMaxVal.Length == 2) strMaxVal = "CID00" + strMaxVal;
                            else if (strMaxVal.Length == 3) strMaxVal = "CID0" + strMaxVal;
                            else strMaxVal = "CID" + strMaxVal;
                            ose.CandidateID = strMaxVal;                            
                            ose.CreatedBy = Membership.GetUser().UserName.ToString();
                            ose.CreatedOn = DateTime.Today;
                            ose.CandidateName = ose.CandidateName.ToUpper().Trim();
                            Recruit.OutsourcingEmployeeDetails.Add(ose);
                            Recruit.SaveChanges();
                            ViewData["CandidateID"] = ose.CandidateID;
                        }
                        ViewData["result"] = "True";
                        ModelState.Clear();
                        OutsourcingEmployeeDetailsView empTemp = new OutsourcingEmployeeDetailsView();
                        empTemp.Genders = empTemp.GenderList();
                        empTemp.CasteCategories = empTemp.CasteList();
                        empTemp.BankNameList = empTemp.bankList();
                        empTemp.Command = "Insert";
                        ViewBag.Title = "New Candidate";
                        return View(empTemp);
                    }
                    else if (emp.Command == "Update")
                    {
                        ose.UpdatedBy = Session["UserName"].ToString();
                        ose.UpdatedOn = DateTime.Today;
                        using (RecruitEntities Recruit = new RecruitEntities())
                        {
                            List<string> nameList = (from m in Recruit.OutsourcingEmployeeDetails where m.CandidateID != ose.CandidateID select m.CandidateName).ToList();
                            List<string> foundNameList = FuzzySearch.Search(ose.CandidateName, nameList, 0.50);
                            List<string> foundNameList1 = FuzzySearch.Search(ose.CandidateName, nameList, 0.70);
                            List<DateTime> dobList = (from m in Recruit.OutsourcingEmployeeDetails where m.CandidateID != ose.CandidateID select m.DOB).ToList();
                            List<string> dobStrList = dobList.ConvertAll<string>(delegate (DateTime i) { return i.ToShortDateString(); });
                            List<string> foundDobList = FuzzySearch.Search(ose.DOB.ToShortDateString(), dobStrList, 0.80);
                            //List<string> foundDobList1 = FuzzySearch.Search(ose.DOB.ToShortDateString(), dobStrList, 0.70);
                            dobList = foundDobList.ConvertAll<DateTime>(delegate (string x) { return Convert.ToDateTime(x); });
                            //List<DateTime> dobList1 = foundDobList1.ConvertAll<DateTime>(delegate(string x) { return Convert.ToDateTime(x); });                                                                    
                            List<OutsourcingEmployeeDetail> tmpOE = (from m in Recruit.OutsourcingEmployeeDetails where (m.CandidateID != ose.CandidateID && ((foundNameList.Contains(m.CandidateName) && m.DOB == ose.DOB) || (foundNameList1.Contains(m.CandidateName) && dobList.Contains(m.DOB)) || (m.PAN == ose.PAN && ose.PAN != null) || (m.MobileNumber == ose.MobileNumber && ose.MobileNumber!=null) || (m.EmailID == ose.EmailID && ose.EmailID!=null) || (m.PF_UAN == ose.PF_UAN && ose.PF_UAN!=null) || (m.ESIC_NO == ose.ESIC_NO && ose.ESIC_NO!=null) || (m.BankAccountNo == ose.BankAccountNo && ose.BankAccountNo!=null))) select m).ToList();
                            if (tmpOE.Any())
                            {
                                string exMsg = "";
                                foreach (OutsourcingEmployeeDetail tt in tmpOE)
                                {
                                    if (exMsg == "") exMsg = tt.CandidateID + " -- " + tt.CandidateName; else exMsg = exMsg + ",\r\n" + tt.CandidateID + " -- " + tt.CandidateName;
                                }
                                throw new Exception("This entry already exists, Verify the following Candidate id's " + exMsg + " or  Contact Administrator");
                            }

                            Recruit.Entry(ose).State = EntityState.Modified;
                            Recruit.Entry(ose).Property(em => em.CreatedBy).IsModified = false;
                            Recruit.Entry(ose).Property(em => em.CreatedOn).IsModified = false;
                            Recruit.SaveChanges();
                            // to update in appointment Master

                            AppointmentMaster am = (from m in Recruit.AppointmentMasters where (m.CandidateID == ose.CandidateID) orderby m.MeetingID descending select m).FirstOrDefault();
                            if (am != null)
                            {
                                if (String.IsNullOrEmpty(am.BankAccountNo) && String.IsNullOrEmpty(am.BankName) && String.IsNullOrEmpty(am.BranchName) && String.IsNullOrEmpty(am.IFSC_Code))
                                {
                                    am.BankAccountNo = ose.BankAccountNo;
                                    am.IFSC_Code = ose.IFSC_Code;
                                    am.BankName = ose.BankName;
                                    am.BranchName = ose.BranchName;
                                    am.DOB = ose.DOB;
                                    am.PermanentAddress = ose.PermanentAddress;
                                    am.CommunicationAddress = ose.CommunicationAddress;
                                    am.MobileNumber = ose.MobileNumber;
                                    am.EmailID = ose.EmailID;
                                }
                                else if (am.BankAccountNo == ose.BankAccountNo)
                                {
                                    am.DOB = ose.DOB;
                                    am.PermanentAddress = ose.PermanentAddress;
                                    am.CommunicationAddress = ose.CommunicationAddress;
                                    am.MobileNumber = ose.MobileNumber;
                                    am.EmailID = ose.EmailID;
                                }
                                else
                                {
                                    throw new Exception("Bank details are not updated in Appointment Master, Because they are already exists");
                                }
                                Recruit.SaveChanges();
                            }

                        }
                        TempData["Status"] = "Update";
                        TempData["CandidateID"] = ose.CandidateID;
                        ModelState.Clear();
                        return RedirectToAction("EmployeeList");
                    }
                    else
                    {
                        OutsourcingEmployeeDetailsView empTemp = new OutsourcingEmployeeDetailsView();
                        emp.Genders = empTemp.GenderList();
                        emp.CasteCategories = empTemp.CasteList();
                        emp.MStatus = empTemp.M_Status();
                        emp.BankNameList = empTemp.bankList();
                        return View(emp);
                    }
                }

                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    OutsourcingEmployeeDetailsView empTemp = new OutsourcingEmployeeDetailsView();
                    emp.Genders = empTemp.GenderList();
                    emp.MStatus = empTemp.M_Status();
                    emp.CasteCategories = empTemp.CasteList();
                    emp.BankNameList = empTemp.bankList();
                    return View(emp);
                }
            }
            else
            {
                ModelState.AddModelError("", "Validation Failed");
                OutsourcingEmployeeDetailsView empTemp = new OutsourcingEmployeeDetailsView();
                emp.MStatus = empTemp.M_Status();
                emp.Genders = empTemp.GenderList();
                emp.CasteCategories = empTemp.CasteList();
                emp.BankNameList = empTemp.bankList();
                return View(emp);
            }


        }

        [HttpPost]
        [MultipleButton(Name = "action", Argument = "NewEmployee")]
        public ActionResult NewEmployee(string id)
        {
            using (RecruitEntities Recruit = new RecruitEntities())
            {
                OutsourcingEmployeeDetail osc = new OutsourcingEmployeeDetail();
                osc = Recruit.OutsourcingEmployeeDetails.Single(em => em.CandidateID == id);
                OutsourcingEmployeeDetailsView oscv = osc;
                oscv.Genders = oscv.GenderList();
                oscv.MStatus = oscv.M_Status();
                oscv.CasteCategories = oscv.CasteList();
                oscv.BankNameList = oscv.bankList();
                oscv.Command = "Update";
                ViewBag.Title = "Update Employee";
                return View(oscv);
            }
        }

        [HttpPost]
        [MultipleButton(Name = "action", Argument = "CandidateDetails")]
        public ActionResult CandidateDetails(string id)
        {
            using (RecruitEntities Recruit = new RecruitEntities())
            {
                OutsourcingEmployeeDetail osc = new OutsourcingEmployeeDetail();
                osc = Recruit.OutsourcingEmployeeDetails.Single(em => em.CandidateID == id);
                OutsourcingEmployeeDetailsView oscv = osc;
                oscv.Genders = oscv.GenderList();
                oscv.MStatus = oscv.M_Status();
                oscv.CasteCategories = oscv.CasteList();
                oscv.BankNameList = oscv.bankList();
                oscv.CreatedOn = osc.CreatedOn;
                oscv.CreatedBy = osc.CreatedBy;
                oscv.UpdatedOn = osc.UpdatedOn;
                oscv.UpdatedBy = osc.UpdatedBy;
                return View(oscv);
            }
        }
        [HttpPost]
        public ActionResult getEmployee(string id)
        {
            using (RecruitEntities RecruitEntity = new RecruitEntities())
            {
                var EmpDetails = RecruitEntity.OutsourcingEmployeeDetails.Where(em => em.CandidateID == id).Select(em => new { EmployeeName = em.CandidateName, DOB = em.DOB });
                var output = "fail";
                var empName = "";
                var birth = "";
                if (EmpDetails.Any())
                {
                    output = "success";
                    empName = EmpDetails.Select(em => em.EmployeeName).First();
                    birth = EmpDetails.Select(em => em.DOB).First().ToShortDateString();
                }
                var ReturnVal = new { result = output, EmployeeName = empName, DOB = birth };
                return Json(ReturnVal, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult getPI(string id)
        {
            using (RecruitEntities RecruitEntity = new RecruitEntities())
            {
                var CoorName = RecruitEntity.Coordinator(id).Select(em => em.CoorName).First().ToString();
                var output = "fail";
                if (!string.IsNullOrEmpty(CoorName)) output = "success";
                var ReturnVal = new { result = output, PIName = CoorName };
                return Json(ReturnVal, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult getProjectDetails(string projectNo, string projectType, string dept)
            {
            using (RecruitEntities RecruitEntity = new RecruitEntities())
            {
                var pTypes = RecruitEntity.ListItemMasters.Where(em => em.ListItemValue == projectType && em.ListName == "ProjectType").Select(em => em.ListGroup).First().ToString();
                var ReturnVal = new { result = "fail", ProjectTitle = "", SponsoredAgency = "", CloseDate = "" };
                if (pTypes == "1 Sponsored")
                {
                    var ProjDetail = RecruitEntity.sponsoredProjectDetail(dept, projectNo).Select(em => new { projectTitle = em.TITLE, SponsoredBy = em.SPON, ClosureDate = em.ClosureDate }).ToList();
                    if (ProjDetail.Any())
                    {
                        DateTime cDate;
                        string dateStr = "";
                        bool checkDate = DateTime.TryParse(Convert.ToString(ProjDetail[0].ClosureDate), out cDate);
                        if (checkDate) dateStr = cDate.ToString("dd/MM/yyyy");
                        ReturnVal = new { result = "success", ProjectTitle = ProjDetail[0].projectTitle, SponsoredAgency = ProjDetail[0].SponsoredBy, CloseDate = dateStr };
                    }

                }
                else if (pTypes == "2 Consultancy")
                {
                    if (dept == "ICS" && projectNo == "IC0910ICS039ICOHDEAN")
                    {
                        ReturnVal = new { result = "success", ProjectTitle = "Various Projects and Programmes", SponsoredAgency = "ICOH", CloseDate = "" };
                    }
                    else
                    {
                        var ProjDetail = RecruitEntity.ConsultancyProjectDetail(dept, projectType, projectNo).Select(em => new { projectTitle = em.TITLE, SponsoredBy = em.AGENCY, ClosureDate = em.ClosureDate }).ToList();
                        if (ProjDetail.Any())
                        {
                            DateTime cDate;
                            string dateStr = "";
                            bool checkDate = DateTime.TryParse(Convert.ToString(ProjDetail[0].ClosureDate), out cDate);
                            if (checkDate) dateStr = cDate.ToString("dd/MM/yyyy");
                            ReturnVal = new { result = "success", ProjectTitle = ProjDetail[0].projectTitle, SponsoredAgency = ProjDetail[0].SponsoredBy, CloseDate = dateStr };
                        }
                    }
                }
                return Json(ReturnVal, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #region minutes
        [HttpGet]
        public ActionResult MinutesList()
        {
            using (RecruitEntities RecruitEntity = new RecruitEntities())
            {
                List<OutsourcingMeetingView> Meet = RecruitEntity.OutsourcingMeetings.Where(em => em.StatusOfRequest == null || em.StatusOfRequest == "").Select(em => new OutsourcingMeetingView { MeetingID = em.MeetingID, MeetingDate = em.MeetingDate, CandidateID = em.CandidateID, CandidateName = em.CandidateName, DOB = em.DOB, DesignationCode = em.DesignationCode, ProjectNo = em.ProjectNo }).ToList();
                ViewData["Status"] = TempData["Status"];
                ViewData["MeetID"] = TempData["MeetID"];
                return View(Meet);
            }
        }

        [HttpPost]
        [MultipleButton(Name = "action", Argument = "NewMinutesFromEmpList")]
        public ActionResult NewMinutesFromEmpList(string id)
        {
            using (RecruitEntities recruit = new RecruitEntities())
            {
                var EmpDetails = recruit.OutsourcingEmployeeDetails.Where(em => em.CandidateID == id).Select(em => new { CandidateID = em.CandidateID, EmployeeName = em.CandidateName, DOB = em.DOB });
                if (EmpDetails.Any())
                {
                    TempData["CandidateID"] = EmpDetails.Select(em => em.CandidateID).First();
                    TempData["CandidateName"] = EmpDetails.Select(em => em.EmployeeName).First();
                    TempData["DOB"] = EmpDetails.Select(em => em.DOB).First().ToShortDateString();
                    return RedirectToAction("NewMinutes", "Outsource");
                }
                return View();
            }
        }

        [HttpGet]
        public ActionResult NewMinutes()
        {
            OutsourcingMeetingView mv = new OutsourcingMeetingView();
            mv.Designations = mv.DesignationList();
            mv.ProjectTypes = mv.ProjectTypeList();
            mv.Sections = mv.SectionList();
            mv.IITMExperiences = mv.IITMExperienceList();
            mv.Departments = mv.DepartmentList();
            mv.DurationTypes = mv.DurationTypeList();
            mv.OutSourcingCompanies = mv.OutSourcingCompanyList();
            DateTime today = DateTime.Today;
            int days = ((int)DayOfWeek.Tuesday - (int)today.DayOfWeek + 7) % 7;
            mv.MeetingDate = today.AddDays(days);
            if (!string.IsNullOrEmpty(Convert.ToString(TempData["CandidateID"])))
            {
                mv.CandidateID = Convert.ToString(TempData["CandidateID"]);
                mv.CandidateName = Convert.ToString(TempData["CandidateName"]);
                mv.DOB = Convert.ToDateTime(TempData["DOB"]);
                //to add experience 
                using (RecruitEntities recruit = new RecruitEntities()) {
                    decimal mos=0;
                    List<AppointmentMasterView> list = (from om in recruit.OutsourcingMeetings join am in recruit.AppointmentMasters on new { a = om.CandidateID, b = om.MeetingID } equals new { a = am.CandidateID, b = am.MeetingID } where om.CandidateID == mv.CandidateID select new AppointmentMasterView {  AppointmentDate = am.AppointmentDate, CandidateID = om.CandidateID,ToDate = am.ToDate,RelieveDate = am.RelieveDate}).ToList();
                    List<OutsourcingMeeting> meet = recruit.OutsourcingMeetings.Where(m => m.CandidateID == mv.CandidateID).ToList();


                    for (int i=0;i<list.Count;i++)
                    {
                        DateTime fromdt = (DateTime)list[i].AppointmentDate;
                        DateTime todt =(DateTime) list[i].ToDate;
                        DateTime? relievedt = list[i].RelieveDate;
                        DateTime dt = relievedt != null ? (DateTime)relievedt: todt;
                        var timespan = ((dt.Year - fromdt.Year) * 12) + fromdt.Month - dt.Month;
                        mos += timespan;
                    }
                    mv.IIT_Experience = mos/12;
                    //string iitexp = list.Sum(m => m.IIT_Experience).ToString();
                    //var yrs = iitexp.Split('.');
                    //int yr = Convert.ToInt16(yrs[1]);
                    //int mons = Convert.ToInt32(yrs[0]);
                    //if (Convert.ToInt16(yrs[1])>=12)
                    //{
                        
                    //    yr =yr + (mons-12);
                    //    mons = (mons - 12)/100;
                    //    mv.IIT_Experience = yr + mons;
                    //}
                    //else
                    //{ mv.IIT_Experience = list.Sum(m => m.IIT_Experience); }
                    mv.IITMExperience = meet.Sum(m => m.IIT_Experience)==0 ? false : true;
                   // mv.IITMExperience = (list.Sum(m => m.IITMExperience ? 1 : 0) > 1) ? true : false ;
                    mv.NONIIT_Experience = meet.Max(m => m.NONIIT_Experience);
                    mv.Total_Experience = mv.IIT_Experience + mv.NONIIT_Experience;
                        }
            }
            mv.Command = "Insert";
            ViewBag.Title = "New Minutes";
            return View(mv);
        }
        [HttpPost]
        public ActionResult NewMinutes(OutsourcingMeetingView meet)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (meet.DurationType == "DurationInMonths")
                    {
                        int checkResult;
                        bool checkMonth = int.TryParse(Convert.ToString(meet.DurationInMonth), out checkResult);
                        if (checkMonth == false) throw new Exception("Enter Duration in Month");
                    }
                    else if (meet.DurationType == "FormToDate")
                    {
                        DateTime checkFdResult;
                        DateTime checkTdResult;
                        bool checkFDate = DateTime.TryParse(Convert.ToString(meet.FromDate), out checkFdResult);
                        bool checkTDate = DateTime.TryParse(Convert.ToString(meet.ToDate), out checkTdResult);
                        if (checkFDate == false || checkTDate == false) throw new Exception("Please Verify From Date and To Date");
                    }
                    else if (meet.DurationType == "ValidUpToDate")
                    {
                        DateTime checkTdResult;
                        bool checkTDate = DateTime.TryParse(Convert.ToString(meet.ToDate), out checkTdResult);
                        if (checkTDate == false) throw new Exception("Please Verify To Date");
                    }
                    OutsourcingMeeting osm = meet;
                    if (meet.Command == "Insert")
                    {
                        //try
                        //{
                        using (RecruitEntities Recruit = new RecruitEntities())
                        {
                            List<OutsourcingMeeting> tmpOM = (from m in Recruit.OutsourcingMeetings where (m.CandidateID == osm.CandidateID && (m.StatusOfRequest != "Relieved" && m.StatusOfRequest != "ShortClosure") && (m.FromDate == osm.FromDate || m.ToDate == osm.ToDate || m.RequestFromDate == osm.RequestFromDate || m.RequestToDate == osm.RequestToDate)) select m).ToList();
                            if (tmpOM.Any())
                            {
                                string exMsg = "";
                                foreach (OutsourcingMeeting tt in tmpOM)
                                {
                                    if (exMsg == "") exMsg = tt.MeetingID; else exMsg = exMsg + ", " + tt.MeetingID;
                                }
                                throw new Exception("This entry already exists, Verify the following meeting id's " + exMsg + " or  Contact Administrator");
                            }
                            int MaxVal;
                            if (Recruit.OutsourcingMeetings.Count() == 0) MaxVal = 0;
                            else
                                MaxVal = Convert.ToInt32(Recruit.OutsourcingMeetings.Max(em => em.MeetingID).Substring(1, 4));
                            MaxVal = MaxVal + 1;
                            string strMaxVal = Convert.ToString(MaxVal);
                            if (strMaxVal.Length == 1) strMaxVal = "M000" + strMaxVal;
                            else if (strMaxVal.Length == 2) strMaxVal = "M00" + strMaxVal;
                            else if (strMaxVal.Length == 3) strMaxVal = "M0" + strMaxVal;
                            else strMaxVal = "M" + strMaxVal;
                            osm.MeetingID = strMaxVal;
                            osm.CreatedBy = Membership.GetUser().UserName.ToString();
                            osm.CreatedOn = DateTime.Today;                            
                            Recruit.OutsourcingMeetings.Add(osm);
                            Recruit.SaveChanges();
                            ViewData["MeetID"] = osm.MeetingID;
                        }
                        //}
                        //catch (DbEntityValidationException dbEx)
                        //{
                        //    foreach (var validationErrors in dbEx.EntityValidationErrors)
                        //    {
                        //        foreach (var validationError in validationErrors.ValidationErrors)
                        //        {
                        //            Trace.TraceInformation("Property: {0} Error: {1}",
                        //                                    validationError.PropertyName,
                        //                                    validationError.ErrorMessage);
                        //        }
                        //    }
                        //}

                        ViewData["result"] = "True";
                        ModelState.Clear();
                        OutsourcingMeetingView mv = new OutsourcingMeetingView();
                        mv.Designations = mv.DesignationList();
                        mv.ProjectTypes = mv.ProjectTypeList();
                        mv.Sections = mv.SectionList();
                        mv.IITMExperiences = mv.IITMExperienceList();
                        mv.Departments = mv.DepartmentList();
                        mv.DurationTypes = mv.DurationTypeList();
                        mv.OutSourcingCompanies = mv.OutSourcingCompanyList();
                        mv.Command = "Insert";
                        ViewBag.Title = "New Minutes";
                        return View(mv);
                    }
                    else if (meet.Command == "Update")
                    {
                        osm.UpdatedBy = Session["UserName"].ToString();
                        osm.UpdatedOn = DateTime.Today;
                        using (RecruitEntities Recruit = new RecruitEntities())
                        {
                            List<OutsourcingMeeting> tmpOM = (from m in Recruit.OutsourcingMeetings where  (m.CandidateID == osm.CandidateID && (m.StatusOfRequest != "Relieved" && m.StatusOfRequest != "ShortClosure") && m.MeetingID != osm.MeetingID && (m.FromDate == osm.FromDate || m.ToDate == osm.ToDate || m.DurationInMonth == osm.DurationInMonth || m.RequestFromDate == osm.RequestFromDate || m.RequestToDate == osm.RequestToDate)) select m).ToList();
                            if (tmpOM.Any())
                            {
                                string exMsg = "";
                                foreach (OutsourcingMeeting tt in tmpOM)
                                {
                                    if (exMsg == "") exMsg = tt.MeetingID; else exMsg = exMsg + ", " + tt.MeetingID;
                                }
                                throw new Exception("This entry already exists, Verify the following meeting id's " + exMsg + " or  Contact Administrator");
                            }
                            Recruit.Entry(osm).State = EntityState.Modified;
                            Recruit.Entry(osm).Property(em => em.CreatedBy).IsModified = false;
                            Recruit.Entry(osm).Property(em => em.CreatedOn).IsModified = false;
                            Recruit.SaveChanges();
                        }
                        TempData["Status"] = "Update";
                        TempData["MeetID"] = osm.MeetingID;
                        ModelState.Clear();
                        return RedirectToAction("MinutesList");
                    }
                    else
                    {
                        OutsourcingMeetingView mv = new OutsourcingMeetingView();
                        meet.Designations = mv.DesignationList();
                        //meet.ProjectTypes = mv.ProjectTypeList();
                        meet.Sections = mv.SectionList();
                        meet.IITMExperiences = mv.IITMExperienceList();
                        meet.Departments = mv.DepartmentList();
                        meet.DurationTypes = mv.DurationTypeList();
                        meet.OutSourcingCompanies = mv.OutSourcingCompanyList();
                        return View(meet);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Validation Failed");
                    OutsourcingMeetingView mv = new OutsourcingMeetingView();
                    meet.Designations = mv.DesignationList();
                    meet.ProjectTypes = mv.ProjectTypeList();
                    meet.Sections = mv.SectionList();
                    meet.IITMExperiences = mv.IITMExperienceList();
                    meet.Departments = mv.DepartmentList();
                    meet.DurationTypes = mv.DurationTypeList();
                    meet.OutSourcingCompanies = mv.OutSourcingCompanyList();
                    return View(meet);
                }
            }

            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                OutsourcingMeetingView mv = new OutsourcingMeetingView();
                meet.Designations = mv.DesignationList();
                meet.ProjectTypes = mv.ProjectTypeList();
                meet.Sections = mv.SectionList();
                meet.IITMExperiences = mv.IITMExperienceList();
                meet.Departments = mv.DepartmentList();
                meet.DurationTypes = mv.DurationTypeList();
                meet.OutSourcingCompanies = mv.OutSourcingCompanyList();
                return View(meet);
            }
        }

        [HttpPost]
        [MultipleButton(Name = "action", Argument = "NewMinutes")]
        public ActionResult NewMinutes(string id)
        {
            using (RecruitEntities Recruit = new RecruitEntities())
            {
                OutsourcingMeeting osm = new OutsourcingMeeting();
                osm = Recruit.OutsourcingMeetings.Where(em => em.MeetingID == id).First();
                OutsourcingMeetingView osmv = osm;
                osmv.Designations = osmv.DesignationList();
                osmv.ProjectTypes = osmv.ProjectTypeList();
                osmv.Sections = osmv.SectionList();
                osmv.IITMExperiences = osmv.IITMExperienceList();
                osmv.Departments = osmv.DepartmentList();
                osmv.DurationTypes = osmv.DurationTypeList();
                int checkResult;
                bool checkMonth = int.TryParse(Convert.ToString(osmv.DurationInMonth), out checkResult);
                DateTime checkResult1;
                bool checkFDate = DateTime.TryParse(Convert.ToString(osmv.FromDate), out checkResult1);
                if (checkMonth) osmv.DurationType = "DurationInMonths";
                else if (checkFDate) osmv.DurationType = "FormToDate";
                else osmv.DurationType = "ValidUpToDate";
                osmv.OutSourcingCompanies = osmv.OutSourcingCompanyList();
                osmv.Command = "Update";
                ViewBag.Title = "Update Minutes";
                return View(osmv);
            }
        }

        [HttpPost]
        [MultipleButton(Name = "action", Argument = "MinutesDetails")]
        public ActionResult MinutesDetails(string id)
        {
            using (RecruitEntities RecruitEntity = new RecruitEntities())
            {
                OutsourcingMeeting meet = RecruitEntity.OutsourcingMeetings.Where(em => em.MeetingID == id).First();
                OutsourcingMeetingView mv = meet;
                mv.ProjectTypes = mv.ProjectTypeList();
                mv.OutSourcingCompanies = mv.OutSourcingCompanyList();
                mv.CreatedOn = meet.CreatedOn;
                mv.CreatedBy = meet.CreatedBy;
                mv.UpdatedOn = meet.UpdatedOn;
                mv.UpdatedBy = meet.UpdatedBy;
                return View(mv);
            }
        }

        [HttpPost]
        [MultipleButton(Name = "action", Argument = "MinutesDelete")]
        public ActionResult MinutesDelete(string id)
        {
            using (RecruitEntities Recruit = new RecruitEntities())
            {
                OutsourcingMeeting osm = Recruit.OutsourcingMeetings.Where(em => em.MeetingID == id).First();
                Recruit.OutsourcingMeetings.Remove(osm);
                Recruit.SaveChanges();
                TempData["Status"] = "Delete";
                TempData["MeetID"] = osm.MeetingID;
                return RedirectToAction("MinutesList");
            }
        }

        [HttpPost]
        [MultipleButton(Name = "action", Argument = "MinutesSummary")]
        public ActionResult MinutesSummary(string id)
        {
            using (SqlConnection con = new SqlConnection())
            {
                con.ConnectionString = ConfigurationManager.ConnectionStrings["Recruit"].ConnectionString;
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "OutsourcingSummarySheet";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = con;
                SqlParameter sp = new SqlParameter();
                sp.ParameterName = "@MeetID";
                sp.SourceColumn = "MeetingID";
                sp.SqlDbType = SqlDbType.VarChar;
                sp.Size = 10;
                sp.Value = id;
                sp.Direction = ParameterDirection.Input;
                cmd.Parameters.Add(sp);
                //string sql = "select MeetingID,ProjectNo,ProjectTitle,SponsoredAgency,PICode,PIName,ProjectCloseDate,DepartmentCode,DepartmentName," +
                //" CASE WHEN CommitmentNo IS Not NULL and CommitmentNo <>'' THEN 'Funds Available' else 'Verify Funds Availablility' end as FundsPosition,"+ 
                //" CandidateName,DesignationName,Qualification,Experience,DOB from OutsourcingMeeting where MeetingID= '" + id + "'";
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                OutsourcingMeetingDS ds = new OutsourcingMeetingDS();
                sda.Fill(ds.Tables["MinutesSummary"]);
                ReportClass rptSummary = new ReportClass();
                rptSummary.FileName = Server.MapPath("~/Reports/SummarySheetOutSource.rpt");
                rptSummary.Load();
                rptSummary.SetDataSource(ds.Tables["MinutesSummary"]);
                Stream fileStream = rptSummary.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                return File(fileStream, "application/pdf");

                /*ReportDocument rdoc = new ReportDocument();
                rdoc.Load(Server.MapPath("/Reports/SummarySheetOutSource.rpt"));
                BinaryReader summaryStream = new BinaryReader(rdoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat));
                Response.ClearContent();
                Response.ClearHeaders();
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment; filename=test.pdf");
                Response.AddHeader("content-length", summaryStream.BaseStream.Length.ToString());
                Response.OutputStream.Write(summaryStream.ReadBytes(Convert.ToInt32(summaryStream.BaseStream.Length)));
                return File(Response.OutputStream, "application/pdf");*/
            }
        }
        #endregion
        #region offerrequest
        [HttpGet]
        public ActionResult OfferRequestList()
        {
            using (RecruitEntities RecruitEntity = new RecruitEntities())
            {
                var am = (from m in RecruitEntity.AppointmentMasters where m.RelieveDate == null select m.CandidateID);
                //List<OfferRequestView> rq = RecruitEntity.OutsourcingMeetings.Where(em => em.StatusOfRequest == null || em.StatusOfRequest.Equals(string.Empty)).Select(em => new OfferRequestView { MeetingID = em.MeetingID, MeetingDate = em.MeetingDate, CandidateName = em.CandidateName, DesignationName = em.DesignationName, GrossSalary = em.GrossSalary, DurationInMonth = em.DurationInMonth, FromDate = em.FromDate, ToDate = em.ToDate }).ToList();
                List<OfferRequestView> rq = (from m in RecruitEntity.OutsourcingMeetings where ((m.StatusOfRequest == null || m.StatusOfRequest.Equals(string.Empty)) && !am.Contains(m.CandidateID)) select new OfferRequestView { MeetingID = m.MeetingID, MeetingDate = m.MeetingDate, CandidateName = m.CandidateName, DesignationName = m.DesignationName, GrossSalary = m.GrossSalary, DurationInMonth = m.DurationInMonth, FromDate = m.FromDate, ToDate = m.ToDate }).ToList();
                return View(rq);
            }
        }

        [HttpPost]
        public ActionResult OfferRequestList(IEnumerable<OfferRequestView> orv)
        {
            try
            {
                if (orv.Where(em => em.isOffered == true).Count() == 0)
                {
                    throw new Exception(" You did not select any one");
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    int maxValue;
                    using (RecruitEntities RecruitEntity = new RecruitEntities())
                    {
                        if (RecruitEntity.OutsourcingOffers.Count() == 0) maxValue = 0;
                        else
                            maxValue = RecruitEntity.OutsourcingOffers.Max(em => em.RequestID);
                        maxValue = maxValue + 1;
                        using (TransactionScope transaction = new TransactionScope())
                        {
                            RecruitEntity.Database.Connection.Open();
                            foreach (OfferRequestView mid in orv)
                            {
                                if (mid.isOffered == true)
                                {
                                    OutsourcingOffer oo = new OutsourcingOffer();
                                    oo.MeetingID = mid.MeetingID;
                                    oo.RequestID = Convert.ToInt32(maxValue);
                                    oo.OfferRequestDate = DateTime.Today;
                                    oo.CreatedBy = Session["UserName"].ToString();
                                    oo.CreatedOn = DateTime.Today;
                                    RecruitEntity.OutsourcingOffers.Add(oo);
                                    RecruitEntity.SaveChanges();
                                    RecruitEntity.Configuration.ValidateOnSaveEnabled = false;
                                    var om = new OutsourcingMeeting { MeetingID = mid.MeetingID, StatusOfRequest = "Offer" };
                                    RecruitEntity.OutsourcingMeetings.Attach(om);
                                    RecruitEntity.Entry(om).Property(em => em.StatusOfRequest).IsModified = true;
                                    RecruitEntity.SaveChanges();
                                }
                            }
                            transaction.Complete();
                        }
                    }
                    using (SqlConnection con = new SqlConnection())
                    {
                        con.ConnectionString = ConfigurationManager.ConnectionStrings["Recruit"].ConnectionString;
                        string sql = "select  ROW_NUMBER() over (order by M.MeetingID) as SerialNumber, M.MeetingID,M.CandidateName,M.DesignationName,M.DepartmentName,M.GrossSalary, " +
                                    "case when M.DurationInMonth is not null and M.DurationInMonth=12 then 'One year from the date of joining' " +
                                    "when M.DurationInMonth is not null and M.DurationInMonth = 1 then cast(M.DurationInMonth as varchar(2)) +' Month from the date of joining' " +
                                    "when M.DurationInMonth is not null and M.DurationInMonth<>'' then cast(M.DurationInMonth as varchar(2)) +' Months from the date of joining' " +
                                    "when M.fromDate is not null and M.fromDate<>'' then convert(varchar,M.FromDate,103) + ' to ' + convert(varchar,M.todate,103) " +
                                    "else 'offer up to '+ convert(varchar,M.ToDate,103) end as Duration from outsourcingMeeting M inner join OutsourcingOffer O on M.MeetingID=O.MeetingID and O.RequestID =" + maxValue;
                        SqlDataAdapter sda = new SqlDataAdapter(sql, con);
                        OutsourcingMeetingDS ds = new OutsourcingMeetingDS();
                        sda.Fill(ds.Tables["OfferRequest"]);
                        ReportClass rptOfferReq = new ReportClass();
                        rptOfferReq.FileName = Server.MapPath("~/Reports/OutSourcingOffer.rpt");
                        rptOfferReq.Load();
                        rptOfferReq.SetDataSource(ds.Tables["OfferRequest"]);
                        Stream fileStream = rptOfferReq.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                        return File(fileStream, "application/pdf");
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(orv);
            }
        }
        #endregion
        #region commitee report        
        [HttpGet]
        public ActionResult CommitteeReport()
        {
            ViewData["Authorities"] = Authorities.AuthorityList();
            return View();
        }

        [HttpPost]
        public ActionResult CommitteeReport(FormCollection fc)
        {
            try
            {
                DateTime meetingDate = Convert.ToDateTime(fc["MeetingDate"]);
                string authority = Convert.ToString(fc["Authority"]);
                using (SqlConnection con = new SqlConnection())
                {
                    con.ConnectionString = ConfigurationManager.ConnectionStrings["Recruit"].ConnectionString;
                    string sql = "select  ROW_NUMBER() over (order by MeetingID) as SerialNumber, MeetingID, MeetingDate, CandidateName,DesignationName,DepartmentName,ProjectNo,GrossSalary, " +
                                "case when DurationInMonth is not null and DurationInMonth=12 then 'One year from the date of joining' " +
                                "when DurationInMonth is not null and DurationInMonth = 1 then cast(DurationInMonth as varchar(2)) +' Month from the date of joining' " +
                                "when DurationInMonth is not null and DurationInMonth<>'' then cast(DurationInMonth as varchar(2)) +' Months from the date of joining' " +
                                "when fromDate is not null and fromDate<>'' then convert(varchar,FromDate,103) + ' To ' + convert(varchar,todate,103) " +
                                "else 'Up to '+ convert(varchar,ToDate,103) end as Duration from outsourcingMeeting where MeetingDate = convert(datetime,'" + meetingDate + "',103)";
                    SqlDataAdapter sda = new SqlDataAdapter(sql, con);
                    OutsourcingMeetingDS ds = new OutsourcingMeetingDS();
                    sda.Fill(ds.Tables["CommitteeApproval"]);
                    ReportClass rptDeanApproval = new ReportClass();
                    authority = authority + " (PR)";
                    rptDeanApproval.FileName = Server.MapPath("~/Reports/MinutesApproval.rpt");
                    rptDeanApproval.Load();
                    ((CrystalDecisions.CrystalReports.Engine.TextObject)rptDeanApproval.ReportDefinition.Sections["Section4"].ReportObjects["txtAuthority"]).Text = authority;
                    rptDeanApproval.SetDataSource(ds.Tables["CommitteeApproval"]);
                    Stream fileStream = rptDeanApproval.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    return File(fileStream, "application/pdf");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewData["Authorities"] = Authorities.AuthorityList();
                return View();
            }
        }
        #endregion

        [HttpGet]
        public ActionResult OfferProcessing()
        {
            using (RecruitEntities recruit = new RecruitEntities())
            {
                List<OfferProcessingView> rov = (from m in recruit.OutsourcingMeetings join f in recruit.OutsourcingOffers on m.MeetingID equals f.MeetingID where ((f.OfferStatus == "" || f.OfferStatus == null) && f.OfferReceivedDate == null) || (f.OfferStatus == "Appointment" && f.OfferReceivedDate == null) || ((f.OfferStatus == "" || f.OfferStatus == null) && f.OfferReceivedDate != null) select new OfferProcessingView { MeetingID = f.MeetingID, CandidateName = m.CandidateName, DesignationName = m.DesignationName, GrossSalary = m.GrossSalary, DurationInMonth = m.DurationInMonth, FromDate = m.FromDate, ToDate = m.ToDate, OfferRequestDate = f.OfferRequestDate, OfferReceivedDate = f.OfferReceivedDate, JoiningDate = f.JoiningDate, OfferStatus = f.OfferStatus }).ToList();
                return View(rov);
            }
        }
        #region receiving offer
        [HttpPost]
        [MultipleButton(Name = "action", Argument = "ReceivingOffer")]
        public ActionResult ReceivingOffer(string EmployeeID, string MeetingID, int OrderID, string cmd)
        {
            using (RecruitEntities recruit = new RecruitEntities())
            {
                OfferProcessingView rov = (from m in recruit.OutsourcingMeetings join f in recruit.OutsourcingOffers on m.MeetingID equals f.MeetingID where f.MeetingID == MeetingID select new OfferProcessingView { MeetingID = f.MeetingID, CandidateName = m.CandidateName, DesignationName = m.DesignationName, GrossSalary = m.GrossSalary, DurationInMonth = m.DurationInMonth, FromDate = m.FromDate, ToDate = m.ToDate, OfferRequestDate = f.OfferRequestDate, OfferReceivedDate = f.OfferReceivedDate }).Single();
                return View(rov);
            }
        }

        [HttpPost]
        public ActionResult ReceivingOffer(OfferProcessingView opv)
        {
            try
            {
                using (RecruitEntities recruit = new RecruitEntities())
                {
                    var om = new OutsourcingOffer { MeetingID = opv.MeetingID, OfferReceivedDate = opv.OfferReceivedDate, UpdatedBy = Session["UserName"].ToString(), UpdatedOn = DateTime.Today };
                    recruit.Configuration.ValidateOnSaveEnabled = false;
                    recruit.OutsourcingOffers.Attach(om);
                    recruit.Entry(om).Property(em => em.OfferReceivedDate).IsModified = true;
                    recruit.Entry(om).Property(em => em.UpdatedBy).IsModified = true;
                    recruit.Entry(om).Property(em => em.UpdatedOn).IsModified = true;
                    recruit.SaveChanges();
                    ViewData["Result"] = "true";
                    return View(opv);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewData["Result"] = "";
                return View(opv);
            }

        }
        #endregion
        #region Joining report
        [HttpPost]
        [MultipleButton(Name = "action", Argument = "JoiningReport")]
        public ActionResult JoiningReport(string EmployeeID, string MeetingID, int OrderID, string cmd)
        {
            using (RecruitEntities recruit = new RecruitEntities())
            {
                JoiningReportView jrv = (from m in recruit.OutsourcingMeetings join f in recruit.OutsourcingOffers on m.MeetingID equals f.MeetingID where f.MeetingID == MeetingID select new JoiningReportView { MeetingID = f.MeetingID, CandidateName = m.CandidateName, DesignationName = m.DesignationName }).Single();
                jrv.ApprovalChannels = jrv.ApprovalChannelList();
                return View(jrv);
            }
        }

        [HttpPost]
        public ActionResult JoiningReport(JoiningReportView jrv)
        {
            using (RecruitEntities recruit = new RecruitEntities())
            {
                var om = new OutsourcingOffer { MeetingID = jrv.MeetingID, JoiningDate = jrv.JoiningDate, JoiningReportApprovedBy = jrv.JoiningReportApprovedBy, JoiningReportDate = jrv.JoiningReportDate, ApprovalChannel = jrv.ApprovalChannel, UpdatedBy = Session["UserName"].ToString(), UpdatedOn = DateTime.Today };
                recruit.Configuration.ValidateOnSaveEnabled = false;
                recruit.OutsourcingOffers.Attach(om);
                recruit.Entry(om).Property(em => em.JoiningDate).IsModified = true;
                recruit.Entry(om).Property(em => em.JoiningReportApprovedBy).IsModified = true;
                recruit.Entry(om).Property(em => em.JoiningReportDate).IsModified = true;
                recruit.Entry(om).Property(em => em.ApprovalChannel).IsModified = true;
                recruit.Entry(om).Property(em => em.UpdatedBy).IsModified = true;
                recruit.Entry(om).Property(em => em.UpdatedOn).IsModified = true;
                recruit.SaveChanges();
                ViewData["Result"] = "true";
                jrv.ApprovalChannels = jrv.ApprovalChannelList();
                return View(jrv);
            }
        }
        #endregion
        #region officeorder
        [HttpPost]
        [MultipleButton(Name = "action", Argument = "Officeorder")]
        public ActionResult Officeorder(string EmployeeID, string MeetingID, int OrderID, string cmd)
        {
            using (RecruitEntities recruit = new RecruitEntities())
            {

                using (SqlConnection con1 = new SqlConnection())
                {
                    con1.ConnectionString = ConfigurationManager.ConnectionStrings["Recruit"].ConnectionString;
                    SqlCommand cmd1 = new SqlCommand("select top 1 am.EmployeeID,am.EmployeeName,om.DOB,om.DesignationName,om.ProjectType,om.ProjectNo,om.ProjectTitle,om.DepartmentName,om.PIName,om.FromDate,om.ToDate,sd.TotalSalary as CostToProject,om.CommitmentNo from OutsourcingMeeting as om inner join AppointmentProject as am  on om.MeetingID=am.MeetingID  inner join SalaryDetails as sd on sd.EmployeeID = am.EmployeeID where am.EmployeeID = '" + EmployeeID + "' and om.MeetingID='" + MeetingID + "' order by OrderID desc", con1);
                    //SqlCommand cmd1 = new SqlCommand("select am.EmployeeID,am.EmployeeName,om.DOB,om.DesignationName,om.ProjectType,om.ProjectNo,om.ProjectTitle,om.DepartmentName,om.PIName,om.FromDate,om.ToDate,om.CostToProject,om.CommitmentNo from OutsourcingMeeting as om inner join AppointmentMaster as am on om.CandidateID=am.CandidateID where om.MeetingID = '" + MeetingID + "'", con1);
                    //IEnumerable<DataRow> print = (from report in recruit.OutsourcingMeetings.AsEnumerable() where report.MeetingID == MeetingID select report).SingleOrDefault() as IEnumerable<DataRow>;
                    // var print = (from report in recruit.OutsourcingMeetings where report.MeetingID == MeetingID select report).SingleOrDefault();
                    SqlDataAdapter adp = new SqlDataAdapter(cmd1);
                    OutsourcingMeetingDS ds = new OutsourcingMeetingDS();

                    //ds.Tables["officeorder"].add(print);
                    //ds.Tables["officeorder"].Equals(print.CopyToDataTable<DataRow>());
                    adp.Fill(ds.Tables["officeorder"]);
                    //ReportDocument rd = new ReportDocument();
                    //rd.Load(Path.Combine(Server.MapPath("~/Reports/officeorder.rpt")));
                    //rd.SetDataSource(recruit.OutsourcingMeetings.Select(m => new {
                    //    EmployeeID = m.CandidateID
                    //}).FirstOrDefault());
                    //Response.Buffer = false;
                    //Response.ClearContent();
                    //Response.ClearHeaders();
                    //Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    //stream.Seek(0, SeekOrigin.Begin);
                    //return File(stream, "application/pdf", "ListProducts.pdf");
                    Outsourcing.Reports.officeorder rpt = new Outsourcing.Reports.officeorder();
                    rpt.Load();
                    rpt.SetDataSource(ds);
                    rpt.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, true, EmployeeID + "Office Order");
                    Stream fileStream = rpt.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    return File(fileStream, "application/pdf");

                }
            }
            return View();
        }
        #endregion
        #region cancel order
        [HttpPost]
        public ActionResult CancelOffer(OfferCancelView ocv)
        {
            using (RecruitEntities recruit = new RecruitEntities())
            {
                var om = new OutsourcingOffer { MeetingID = ocv.MeetingID, OfferStatus = ocv.OfferStatus, Remarks = ocv.Remarks, UpdatedBy = Session["UserName"].ToString(), UpdatedOn = DateTime.Today };
                recruit.Configuration.ValidateOnSaveEnabled = false;
                recruit.OutsourcingOffers.Attach(om);
                recruit.Entry(om).Property(em => em.OfferStatus).IsModified = true;
                recruit.Entry(om).Property(em => em.Remarks).IsModified = true;
                recruit.Entry(om).Property(em => em.UpdatedBy).IsModified = true;
                recruit.Entry(om).Property(em => em.UpdatedOn).IsModified = true;
                recruit.SaveChanges();
                ViewData["Result"] = "true";
                return View(ocv);
            }
        }
        #endregion
        #region new appointment
        [HttpPost]
        [MultipleButton(Name = "action", Argument = "NewAppointment")]
        public ActionResult NewAppointment(string EmployeeID, string MeetingID, int OrderID, string cmd)
        {
            if (cmd == "New" && MeetingID != "New")
            {
                using (RecruitEntities recruit = new RecruitEntities())
                {
                    AppointmentMasterView amv = (from ed in recruit.OutsourcingEmployeeDetails
                                                 join me in recruit.OutsourcingMeetings on ed.CandidateID equals me.CandidateID
                                                 where me.MeetingID == MeetingID
                                                 select new AppointmentMasterView
                                                 {                                                    
                                                     EmployeeName = ed.CandidateName,
                                                     CandidateID = ed.CandidateID,
                                                     DOB = ed.DOB,
                                                     PermanentAddress = ed.PermanentAddress,
                                                     CommunicationAddress = ed.CommunicationAddress,
                                                     MobileNumber = ed.MobileNumber,
                                                     EmailID = ed.EmailID,
                                                     BankName = ed.BankName,
                                                     BranchName = ed.BranchName,
                                                     BankAccountNo = ed.BankAccountNo,
                                                     IFSC_Code = ed.IFSC_Code,
                                                     OutSourcingCompany = me.OutSourcingCompany,
                                                 }).Single();

                    AppointmentProjectView apv = (from ed in recruit.OutsourcingEmployeeDetails
                                                  join me in recruit.OutsourcingMeetings on ed.CandidateID equals me.CandidateID
                                                  where me.MeetingID == MeetingID
                                                  select new AppointmentProjectView
                                                  {                                                     
                                                      EmployeeName = ed.CandidateName,
                                                      MeetingID = me.MeetingID,
                                                      DesignationCode = me.DesignationCode,
                                                      DesignationName = me.DesignationName,
                                                      DepartmentCode = me.DepartmentCode,
                                                      PICode = me.PICode,
                                                      PIName = me.PIName,
                                                      ProjectType = me.ProjectType,
                                                      ProjectNo = me.ProjectNo,
                                                      PartTime  =me.PartTime
                                                  }).Single();
                    AppointmentDetailsView adv = (from me in recruit.OutsourcingMeetings
                                                  where me.MeetingID == MeetingID
                                                  select new AppointmentDetailsView
                                                  {
                                                      EmployeeName = amv.EmployeeName,
                                                      MeetingID = me.MeetingID,
                                                      OrderType = "Appointment",
                                                      ProjectNo = me.ProjectNo,
                                                      GrossSalary = me.GrossSalary,
                                                      CostToProject = me.CostToProject
                                                  }).Single();
                    OutsourcingEmployeeDetailsView edv = (from ed in recruit.OutsourcingEmployeeDetails
                                                          join me in recruit.OutsourcingMeetings on ed.CandidateID equals me.CandidateID
                                                          where me.MeetingID == MeetingID
                                                          select new OutsourcingEmployeeDetailsView
                                                          {
                                                              PH = ed.PH
                                                          }).SingleOrDefault();
                    SalaryDetailsView sdv;
                    SalaryCalculation sc = new SalaryCalculation();
                    sc.TAMsalary(Convert.ToDecimal(adv.GrossSalary), amv.DesignationCode, edv.PH, out sdv);
                    NewAppointmentView nav = new NewAppointmentView();
                    nav.appointmentMasterView = amv;
                    nav.appointmentProjectView = apv;
                    nav.appointmentDetailsView = adv;
                    nav.salaryDetailsView = sdv;
                    nav.PH = edv.PH;
                    nav.Command = "Insert";
                    ViewBag.MinimumDate = null;
                    List<SelectListItem> pf = new List<SelectListItem> { new SelectListItem { Text = "With PF", Value = "With PF" }, new SelectListItem { Text = "Without PF", Value = "Without PF" } };
                    ViewData["PFEligible"] = pf;
                    ViewBag.Title = "New Appointment Order";
                    return View(nav);
                }
            }
            else if (cmd == "New" && MeetingID == "New")
            {
                using (RecruitEntities recruit = new RecruitEntities())
                {
                    var tmpAp = (from m in recruit.AppointmentProjects where m.EmployeeID == EmployeeID orderby m.AppointmentDate descending select new { m.MeetingID, m.ToDate, m.ProjectRelieveDate }).Take(1).Single();
                    string OldmeetID = tmpAp.MeetingID.ToString();
                    DateTime tmpMinDate;
                    if (tmpAp.ProjectRelieveDate != null)
                    {
                        ViewBag.MinimumDate = Convert.ToDateTime(tmpAp.ProjectRelieveDate).AddDays(1);
                        tmpMinDate = Convert.ToDateTime(tmpAp.ProjectRelieveDate).AddDays(1);
                    }
                    else
                    {
                        ViewBag.MinimumDate = Convert.ToDateTime(tmpAp.ToDate).AddDays(1);
                        tmpMinDate = Convert.ToDateTime(tmpAp.ToDate).AddDays(1);
                    }
                    string cId = (from m in recruit.AppointmentMasters where m.EmployeeID == EmployeeID select m.CandidateID).First().ToString();
                    var count = Convert.ToInt32(recruit.OutsourcingMeetings.Count(em => em.CandidateID == cId && (em.StatusOfRequest == null || em.StatusOfRequest == "")));
                    if (count >= 1) MeetingID = Convert.ToString((from m in recruit.OutsourcingMeetings where m.CandidateID == cId && m.StatusOfRequest == null orderby m.MeetingID descending select m.MeetingID).Take(1).First());
                    else MeetingID = null;
                    if (MeetingID == null)
                    {
                        TempData["IDverify"] = "There is no new Meeting ID";
                        return RedirectToAction("AppointmentDetails", "Outsource", new { id = EmployeeID });
                    }
                    else if (Convert.ToInt32(MeetingID.Substring(1)) < Convert.ToInt32(OldmeetID.Substring(1)))
                    {
                        TempData["IDverify"] = "There is no new Meeting ID";
                        return RedirectToAction("AppointmentDetails", "Outsource", new { id = EmployeeID });
                    }
                    AppointmentMasterView amv = (from ed in recruit.AppointmentMasters
                                                 where ed.CandidateID == cId && ed.EmployeeID == EmployeeID
                                                 select new AppointmentMasterView
                                                 {
                                                     
                                                     EmployeeID = ed.EmployeeID,
                                                     EmployeeName = ed.EmployeeName,
                                                     CandidateID = ed.CandidateID,
                                                     DOB = ed.DOB,
                                                     PermanentAddress = ed.PermanentAddress,
                                                     CommunicationAddress = ed.CommunicationAddress,
                                                     MobileNumber = ed.MobileNumber,
                                                     EmailID = ed.EmailID,
                                                     BankName = ed.BankName,
                                                     BranchName = ed.BranchName,
                                                     BankAccountNo = ed.BankAccountNo,
                                                     IFSC_Code = ed.IFSC_Code,
                                                     OutSourcingCompany = ed.OutSourcingCompany,
                                                 }).Single();
                    AppointmentProjectView apv = (from me in recruit.OutsourcingMeetings
                                                  where me.MeetingID == MeetingID
                                                  select new AppointmentProjectView
                                                  {
                                                      EmployeeName = amv.EmployeeName,
                                                      MeetingID = me.MeetingID,
                                                      DesignationCode = me.DesignationCode,
                                                      DesignationName = me.DesignationName,
                                                      DepartmentCode = me.DepartmentCode,
                                                      PICode = me.PICode,
                                                      PIName = me.PIName,
                                                      ProjectType = me.ProjectType,
                                                      ProjectNo = me.ProjectNo,
                                                      PartTime=me.PartTime
                                                  }).Single();
                    AppointmentDetailsView adv = (from me in recruit.OutsourcingMeetings
                                                  where me.MeetingID == MeetingID
                                                  select new AppointmentDetailsView
                                                  {
                                                      EmployeeName = amv.EmployeeName,
                                                      MeetingID = me.MeetingID,
                                                      OrderType = "Appointment",
                                                      ProjectNo = me.ProjectNo,
                                                      FromDate = tmpMinDate,
                                                      GrossSalary = me.GrossSalary,
                                                      CostToProject = me.CostToProject
                                                  }).Single();
                    OutsourcingEmployeeDetailsView edv = (from ed in recruit.OutsourcingEmployeeDetails
                                                          join me in recruit.OutsourcingMeetings on ed.CandidateID equals me.CandidateID
                                                          where me.MeetingID == MeetingID
                                                          select new OutsourcingEmployeeDetailsView
                                                          {
                                                              PH = ed.PH
                                                          }).SingleOrDefault();
                    SalaryDetailsView sdv;
                    SalaryCalculation sc = new SalaryCalculation();
                    sc.TAMsalary(Convert.ToDecimal(adv.GrossSalary), amv.DesignationCode, edv.PH, out sdv);
                    NewAppointmentView nav = new NewAppointmentView();
                    nav.appointmentMasterView = amv;
                    nav.appointmentProjectView = apv;
                    nav.appointmentDetailsView = adv;
                    nav.PH = edv.PH;
                    nav.salaryDetailsView = sdv;
                    nav.Command = "ReAppointment";
                    List<SelectListItem> pf = new List<SelectListItem> { new SelectListItem { Text = "With PF", Value = "With PF" }, new SelectListItem { Text = "Without PF", Value = "Without PF" } };
                    ViewData["PFEligible"] = pf;
                    ViewBag.Title = "Reappointment Order";
                    return View(nav);
                }
            }
            else if (cmd == "Update")
            {
                using (RecruitEntities recruit = new RecruitEntities())
                {
                    AppointmentMaster am = (from m in recruit.AppointmentMasters where (m.EmployeeID == EmployeeID) select m).Single();
                    AppointmentMasterView amv = am;
                    AppointmentProject ap = (from m in recruit.AppointmentProjects where (m.EmployeeID == EmployeeID && m.MeetingID == MeetingID) select m).Single();
                    AppointmentProjectView apv = ap;
                    AppointmentDetail ad = (from m in recruit.AppointmentDetails where (m.EmployeeID == EmployeeID && m.OrderType == "Appointment" && m.MeetingID == MeetingID) select m).Single();
                    AppointmentDetailsView adv = ad;
                    if (ad.OrderID == 1)
                        ViewBag.MinimumDate = null;
                    else
                    {
                        var Tmpap = Convert.ToDateTime((from m in recruit.AppointmentProjects where (m.EmployeeID == EmployeeID && m.ProjectRelieveDate != null) orderby m.AppointmentDate descending select m.ProjectRelieveDate).Take(1).First());
                        ViewBag.MinimumDate = Convert.ToDateTime(Tmpap).AddDays(1);
                    }
                    SalaryDetail sd = (from m in recruit.SalaryDetails where (m.EmployeeID == EmployeeID && m.OrderID == OrderID) select m).Single();
                    SalaryDetailsView sdv = sd;
                    sdv.TotalDeduction = sdv.EmployeePF + sdv.EmployeeESIC + sdv.ProfessionalTax;
                    sdv.GrossTotalwithAgencyFee = sdv.GrossTotal + sdv.AgencyFee;
                    NewAppointmentView nav = new NewAppointmentView();
                    nav.appointmentMasterView = amv;
                    nav.appointmentProjectView = apv;
                    nav.appointmentDetailsView = adv;
                    nav.salaryDetailsView = sdv;

                    nav.Command = "Update";
                    ViewBag.Title = "Update Appointment Order";
                    ViewData["EmployeeID"] = nav.appointmentDetailsView.EmployeeID;
                    List<SelectListItem> pf = new List<SelectListItem> { new SelectListItem { Text = "With PF", Value = "With PF" }, new SelectListItem { Text = "Without PF", Value = "Without PF" } };
                    ViewData["PFEligible"] = pf;
                    return View(nav);
                }
            }
            else
            {
                AppointmentMasterView amv = new AppointmentMasterView();
                AppointmentProjectView apv = new AppointmentProjectView();
                AppointmentDetailsView adv = new AppointmentDetailsView();
                OutsourcingEmployeeDetailsView edv = new OutsourcingEmployeeDetailsView();
                SalaryDetailsView sdv = new SalaryDetailsView();
                NewAppointmentView nav = new NewAppointmentView();
                nav.appointmentMasterView = amv;
                nav.appointmentProjectView = apv;
                nav.appointmentDetailsView = adv;
                // nav.OutsourcingEmployeeDetails = edv;
                nav.salaryDetailsView = sdv;
                ViewBag.MinimumDate = null;
                List<SelectListItem> pf = new List<SelectListItem> { new SelectListItem { Text = "With PF", Value = "With PF" }, new SelectListItem { Text = "Without PF", Value = "Without PF" } };
                ViewData["PFEligible"] = pf;
                return View(nav);
            }
        }

        [HttpPost]
        public ActionResult NewAppointment(NewAppointmentView nav)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (nav.Command == "Insert")
                    {
                        using (RecruitEntities RecruitEntity = new RecruitEntities())
                        {
                            int maxValue;
                            int subLen = 3;
                            if (RecruitEntity.AppointmentMasters.Count() == 0) maxValue = 0;
                            else
                            {
                                string Cnt = RecruitEntity.AppointmentMasters.Count().ToString();
                                if (Cnt.Length > 3) subLen = Cnt.Length;
                                maxValue = Convert.ToInt32(RecruitEntity.AppointmentMasters.Max(em => em.EmployeeID).Substring(3, subLen));
                            }
                            maxValue = maxValue + 1;
                            string strMaxVal = Convert.ToString(maxValue);
                            if (strMaxVal.Length == 1) strMaxVal = "TIC00" + strMaxVal;
                            else if (strMaxVal.Length == 2) strMaxVal = "TIC0" + strMaxVal;
                            else strMaxVal = "TIC" + strMaxVal;
                            nav.appointmentMasterView.EmployeeID = strMaxVal;
                            nav.appointmentMasterView.MeetingID = nav.appointmentProjectView.MeetingID;
                            nav.appointmentMasterView.DesignationCode = nav.appointmentProjectView.DesignationCode;
                            nav.appointmentMasterView.DesignationName = nav.appointmentProjectView.DesignationName;
                            nav.appointmentMasterView.AppointmentDate = Convert.ToDateTime(nav.appointmentDetailsView.FromDate);
                            nav.appointmentMasterView.ToDate = nav.appointmentDetailsView.ToDate;
                            nav.appointmentMasterView.BasicSalary = nav.salaryDetailsView.BasicSalary;
                            nav.appointmentProjectView.EmployeeID = strMaxVal;
                            nav.appointmentProjectView.EmployeeName = nav.appointmentMasterView.EmployeeName;
                            nav.appointmentProjectView.AppointmentDate = Convert.ToDateTime(nav.appointmentDetailsView.FromDate);
                            nav.appointmentProjectView.ToDate = nav.appointmentDetailsView.ToDate;
                            int OrderID = 1;
                            nav.appointmentDetailsView.EmployeeID = strMaxVal;
                            nav.appointmentDetailsView.OrderID = OrderID;
                            nav.appointmentDetailsView.BasicSalary = nav.salaryDetailsView.BasicSalary;
                            nav.appointmentDetailsView.GrossSalary = nav.salaryDetailsView.GrossSalary;
                            nav.appointmentDetailsView.CostToProject = nav.salaryDetailsView.TotalSalary;
                            nav.salaryDetailsView.EmployeeID = strMaxVal;
                            nav.salaryDetailsView.OrderID = OrderID;
                            nav.salaryDetailsView.OrderType = "Appointment";

                            using (TransactionScope transaction = new TransactionScope())
                            {
                                RecruitEntity.Database.Connection.Open();
                                AppointmentMaster am = nav.appointmentMasterView;
                                am.CreatedOn = DateTime.Today;
                                am.CreatedBy = Session["UserName"].ToString();
                                RecruitEntity.AppointmentMasters.Add(am);
                                AppointmentProject ap = nav.appointmentProjectView;
                                RecruitEntity.AppointmentProjects.Add(ap);
                                AppointmentDetail ad = nav.appointmentDetailsView;
                                ad.CreatedOn = DateTime.Today;
                                ad.CreatedBy = Session["UserName"].ToString();
                                RecruitEntity.AppointmentDetails.Add(ad);
                                SalaryDetail sd = nav.salaryDetailsView;
                                RecruitEntity.SalaryDetails.Add(sd);
                                OrderRequestDetail ord = new OrderRequestDetail();
                                ord.EmployeeID = strMaxVal;
                                ord.EmployeeName = nav.appointmentMasterView.EmployeeName;
                                ord.OrderID = OrderID;
                                ord.OrderType = "Appointment";
                                ord.CreatedOn = DateTime.Today;
                                ord.CreatedBy = Session["UserName"].ToString();
                                RecruitEntity.OrderRequestDetails.Add(ord);
                                var om = new OutsourcingOffer { MeetingID = am.MeetingID, OfferStatus = "Appointment", UpdatedBy = Session["UserName"].ToString(), UpdatedOn = DateTime.Today };
                                RecruitEntity.Configuration.ValidateOnSaveEnabled = false;
                                RecruitEntity.OutsourcingOffers.Attach(om);
                                RecruitEntity.Entry(om).Property(em => em.OfferStatus).IsModified = true;
                                RecruitEntity.Entry(om).Property(em => em.UpdatedBy).IsModified = true;
                                RecruitEntity.Entry(om).Property(em => em.UpdatedOn).IsModified = true;
                                RecruitEntity.SaveChanges();
                                transaction.Complete();
                            }
                            ViewData["Result"] = "True";
                            ViewData["status"] = "Insert";
                            ViewData["EmployeeID"] = nav.appointmentMasterView.EmployeeID;
                            List<SelectListItem> pf = new List<SelectListItem> { new SelectListItem { Text = "With PF", Value = "With PF" }, new SelectListItem { Text = "Without PF", Value = "Without PF" } };
                            ViewBag.MinimumDate = null;
                            ViewData["PFEligible"] = pf;
                            return View(nav);
                        }
                    }
                    else if (nav.Command == "ReAppointment")
                    {
                        using (RecruitEntities RecruitEntity = new RecruitEntities())
                        {
                            var Tmpap = Convert.ToDateTime((from m in RecruitEntity.AppointmentProjects where (m.EmployeeID == nav.appointmentMasterView.EmployeeID && m.ProjectRelieveDate != null) orderby m.AppointmentDate descending select m.ProjectRelieveDate).Take(1).First());
                            if (Convert.ToDateTime(Tmpap) >= Convert.ToDateTime(nav.appointmentDetailsView.FromDate))
                            {
                                ModelState.AddModelError("", "From Date should be greater than Previous appointment");
                                List<SelectListItem> pf = new List<SelectListItem> { new SelectListItem { Text = "With PF", Value = "With PF" }, new SelectListItem { Text = "Without PF", Value = "Without PF" } };
                                ViewData["PFEligible"] = pf;
                                ViewBag.MinimumDate = Convert.ToDateTime(Tmpap);
                                return View(nav);
                            }

                            nav.appointmentProjectView.EmployeeID = nav.appointmentMasterView.EmployeeID;
                            nav.appointmentProjectView.EmployeeName = nav.appointmentMasterView.EmployeeName;
                            nav.appointmentProjectView.AppointmentDate = Convert.ToDateTime(nav.appointmentDetailsView.FromDate);
                            nav.appointmentProjectView.ToDate = nav.appointmentDetailsView.ToDate;
                            int orderID = (from m in RecruitEntity.AppointmentDetails where m.EmployeeID == nav.appointmentMasterView.EmployeeID select m.OrderID).Max();
                            decimal bp = Convert.ToDecimal((from m in RecruitEntity.AppointmentDetails where m.EmployeeID == nav.appointmentMasterView.EmployeeID && m.OrderID == orderID select m.BasicSalary).First());
                            if (bp > nav.salaryDetailsView.BasicSalary)
                            {
                                ModelState.AddModelError("", "Basic Salary should be greater than or equal to Previous appointment");
                                List<SelectListItem> pf = new List<SelectListItem> { new SelectListItem { Text = "With PF", Value = "With PF" }, new SelectListItem { Text = "Without PF", Value = "Without PF" } };
                                ViewData["PFEligible"] = pf;
                                ViewBag.MinimumDate = Convert.ToDateTime(Tmpap);
                                return View(nav);
                            }
                            orderID = orderID + 1;
                            nav.appointmentDetailsView.EmployeeID = nav.appointmentMasterView.EmployeeID;
                            nav.appointmentDetailsView.OrderID = orderID;
                            nav.appointmentDetailsView.BasicSalary = nav.salaryDetailsView.BasicSalary;
                            nav.appointmentDetailsView.GrossSalary = nav.salaryDetailsView.GrossSalary;
                            nav.appointmentDetailsView.CostToProject = nav.salaryDetailsView.TotalSalary;
                            nav.salaryDetailsView.EmployeeID = nav.appointmentMasterView.EmployeeID;
                            nav.salaryDetailsView.OrderID = orderID;
                            nav.salaryDetailsView.OrderType = "Appointment";
                            AppointmentProject tmpAP = (from m in RecruitEntity.AppointmentProjects where m.EmployeeID == nav.appointmentMasterView.EmployeeID orderby m.AppointmentDate descending select m).Take(1).Single();
                            string orderType = "";
                            if (nav.appointmentProjectView.DesignationCode == tmpAP.DesignationCode && nav.appointmentProjectView.ToDate > tmpAP.ToDate)
                                orderType = "Extension";
                            else if (nav.appointmentProjectView.DesignationCode != tmpAP.DesignationCode && nav.appointmentProjectView.ToDate > tmpAP.ToDate)
                                orderType = "DesigChangeWithExtension";
                            else if (nav.appointmentProjectView.DesignationCode != tmpAP.DesignationCode && nav.appointmentProjectView.ToDate == tmpAP.ToDate)
                                orderType = "DesigChangeWithEnhancement";
                            else if (nav.appointmentProjectView.DesignationCode == tmpAP.DesignationCode && nav.appointmentProjectView.ToDate == tmpAP.ToDate && nav.salaryDetailsView.BasicSalary != bp)
                                orderType = "Enhancement";
                            else if (nav.appointmentProjectView.DesignationCode == tmpAP.DesignationCode && nav.appointmentProjectView.ToDate == tmpAP.ToDate)
                                orderType = "";
                            using (TransactionScope transaction = new TransactionScope())
                            {
                                RecruitEntity.Database.Connection.Open();
                                AppointmentProject ap = nav.appointmentProjectView;
                                RecruitEntity.AppointmentProjects.Add(ap);
                                AppointmentDetail ad = nav.appointmentDetailsView;
                                ad.CreatedOn = DateTime.Today;
                                ad.CreatedBy = Session["UserName"].ToString();
                                RecruitEntity.AppointmentDetails.Add(ad);
                                SalaryDetail sd = nav.salaryDetailsView;
                                RecruitEntity.SalaryDetails.Add(sd);
                                if (orderType != "")
                                {
                                    OrderRequestDetail ord = new OrderRequestDetail();
                                    ord.EmployeeID = nav.appointmentMasterView.EmployeeID;
                                    ord.EmployeeName = nav.appointmentMasterView.EmployeeName;
                                    ord.OrderID = orderID;
                                    ord.OrderType = orderType;
                                    ord.CreatedOn = DateTime.Today;
                                    ord.CreatedBy = Session["UserName"].ToString();
                                    RecruitEntity.OrderRequestDetails.Add(ord);
                                }
                                var am = new AppointmentMaster
                                {
                                    MeetingID=nav.appointmentProjectView.MeetingID,//added to update meeting id in master
                                    EmployeeID = nav.appointmentMasterView.EmployeeID,
                                    DesignationCode = nav.appointmentProjectView.DesignationCode,
                                    DesignationName = nav.appointmentProjectView.DesignationName,
                                    ToDate = Convert.ToDateTime(nav.appointmentDetailsView.ToDate),
                                    RelieveDate = null,
                                    BasicSalary = nav.salaryDetailsView.BasicSalary
                                };
                                RecruitEntity.Configuration.ValidateOnSaveEnabled = false;
                                RecruitEntity.AppointmentMasters.Attach(am);


                                RecruitEntity.Entry(am).Property(em => em.MeetingID).IsModified = true;
                                RecruitEntity.Entry(am).Property(em => em.DesignationCode).IsModified = true;
                                RecruitEntity.Entry(am).Property(em => em.DesignationName).IsModified = true;
                                RecruitEntity.Entry(am).Property(em => em.ToDate).IsModified = true;
                                RecruitEntity.Entry(am).Property(em => em.RelieveDate).IsModified = true;
                                RecruitEntity.Entry(am).Property(em => em.BasicSalary).IsModified = true;
                                var om = new OutsourcingMeeting { MeetingID = ap.MeetingID, StatusOfRequest = "Appointment" };
                                RecruitEntity.Configuration.ValidateOnSaveEnabled = false;
                                RecruitEntity.OutsourcingMeetings.Attach(om);
                                RecruitEntity.Entry(om).Property(em => em.StatusOfRequest).IsModified = true;
                                RecruitEntity.SaveChanges();
                                transaction.Complete();
                            }
                            ViewData["Result"] = "True";
                            ViewData["status"] = "Insert";
                            ViewData["EmployeeID"] = nav.appointmentMasterView.EmployeeID;
                            ViewBag.MinimumDate = null;
                            List<SelectListItem> pf1 = new List<SelectListItem> { new SelectListItem { Text = "With PF", Value = "With PF" }, new SelectListItem { Text = "Without PF", Value = "Without PF" } };
                            ViewData["PFEligible"] = pf1;
                            return View(nav);
                        }
                    }
                    else if (nav.Command == "Update")
                    {
                        using (RecruitEntities recruit = new RecruitEntities())
                        {

                            nav.appointmentMasterView.MeetingID = nav.appointmentProjectView.MeetingID;
                            nav.appointmentMasterView.DesignationCode = nav.appointmentProjectView.DesignationCode;
                            nav.appointmentMasterView.DesignationName = nav.appointmentProjectView.DesignationName;
                            if (nav.appointmentDetailsView.OrderID == 1)
                            {
                                nav.appointmentMasterView.AppointmentDate = Convert.ToDateTime(nav.appointmentDetailsView.FromDate);
                            }
                            else
                            {
                                // nav.appointmentMasterView.AppointmentDate = Convert.ToDateTime(recruit.AppointmentMasters.Where(em => em.EmployeeID == nav.appointmentMasterView.EmployeeID).Select(em => em.AppointmentDate));
                                var Tmpap = Convert.ToDateTime((from m in recruit.AppointmentProjects where (m.EmployeeID == nav.appointmentMasterView.EmployeeID && m.ProjectRelieveDate != null) orderby m.AppointmentDate descending select m.ProjectRelieveDate).Take(1).First());
                                if (Convert.ToDateTime(Tmpap) >= Convert.ToDateTime(nav.appointmentDetailsView.FromDate))
                                {
                                    ModelState.AddModelError("", "From Date should be greater than Previous appointment");
                                    List<SelectListItem> pf = new List<SelectListItem> { new SelectListItem { Text = "With PF", Value = "With PF" }, new SelectListItem { Text = "Without PF", Value = "Without PF" } };
                                    ViewData["PFEligible"] = pf;
                                    ViewBag.MinimumDate = Convert.ToDateTime(Tmpap);
                                    return View(nav);
                                }
                                int oID = nav.appointmentDetailsView.OrderID - 1;
                                decimal bp = Convert.ToDecimal((from m in recruit.AppointmentDetails where m.EmployeeID == nav.appointmentMasterView.EmployeeID && m.OrderID == oID select m.BasicSalary).First());
                                if (bp > nav.salaryDetailsView.BasicSalary)
                                {
                                    ModelState.AddModelError("", "Basic Salary should be greater than or equal to Previous appointment");
                                    List<SelectListItem> pf = new List<SelectListItem> { new SelectListItem { Text = "With PF", Value = "With PF" }, new SelectListItem { Text = "Without PF", Value = "Without PF" } };
                                    ViewData["PFEligible"] = pf;
                                    ViewBag.MinimumDate = Convert.ToDateTime(Tmpap);
                                    return View(nav);
                                }
                            }
                            nav.appointmentMasterView.ToDate = nav.appointmentDetailsView.ToDate;
                            nav.appointmentMasterView.BasicSalary = nav.salaryDetailsView.BasicSalary;
                            nav.appointmentProjectView.EmployeeID = nav.appointmentMasterView.EmployeeID;
                            nav.appointmentProjectView.EmployeeName = nav.appointmentMasterView.EmployeeName;
                            nav.appointmentProjectView.AppointmentDate = Convert.ToDateTime(nav.appointmentDetailsView.FromDate);
                            nav.appointmentProjectView.ToDate = nav.appointmentDetailsView.ToDate;
                            nav.appointmentDetailsView.EmployeeID = nav.appointmentMasterView.EmployeeID;
                            nav.appointmentDetailsView.BasicSalary = nav.salaryDetailsView.BasicSalary;
                            nav.appointmentDetailsView.GrossSalary = nav.salaryDetailsView.GrossSalary;
                            nav.appointmentDetailsView.CostToProject = nav.salaryDetailsView.TotalSalary;
                            nav.salaryDetailsView.EmployeeID = nav.appointmentMasterView.EmployeeID;
                            nav.salaryDetailsView.OrderID = nav.appointmentDetailsView.OrderID;
                            nav.salaryDetailsView.OrderType = "Appointment";
                            using (TransactionScope transaction = new TransactionScope())
                            {
                                recruit.Database.Connection.Open();
                                AppointmentMaster am = nav.appointmentMasterView;
                                am.UpdatedOn = DateTime.Today;
                                am.UpdatedBy = Session["UserName"].ToString();
                                recruit.Entry(am).State = EntityState.Modified;
                                if (nav.appointmentDetailsView.OrderID != 1) recruit.Entry(am).Property(em => em.AppointmentDate).IsModified = false;
                                recruit.Entry(am).Property(em => em.CreatedBy).IsModified = false;
                                recruit.Entry(am).Property(em => em.CreatedOn).IsModified = false;
                                AppointmentProject ap = nav.appointmentProjectView;
                                recruit.Entry(ap).State = EntityState.Modified;
                                AppointmentDetail ad = nav.appointmentDetailsView;
                                ad.UpdatedOn = DateTime.Today;
                                ad.UpdatedBy = Session["UserName"].ToString();
                                recruit.Entry(ad).State = EntityState.Modified;
                                recruit.Entry(ad).Property(em => em.CreatedBy).IsModified = false;
                                recruit.Entry(ad).Property(em => em.CreatedOn).IsModified = false;
                                SalaryDetail sd = nav.salaryDetailsView;
                                recruit.Entry(sd).State = EntityState.Modified;
                                recruit.SaveChanges();
                                transaction.Complete();
                            }
                            ViewData["Result"] = "True";
                            ViewData["status"] = "Update";
                            ViewData["EmployeeID"] = nav.appointmentMasterView.EmployeeID;
                            List<SelectListItem> pf1 = new List<SelectListItem> { new SelectListItem { Text = "With PF", Value = "With PF" }, new SelectListItem { Text = "Without PF", Value = "Without PF" } };
                            ViewData["PFEligible"] = pf1;
                            ViewBag.MinimumDate = null;
                            return View(nav);
                        }
                    }
                    else
                    {
                        List<SelectListItem> pf = new List<SelectListItem> { new SelectListItem { Text = "With PF", Value = "With PF" }, new SelectListItem { Text = "Without PF", Value = "Without PF" } };
                        ViewData["PFEligible"] = pf;
                        ViewBag.MinimumDate = null;
                        return View(nav);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Validation Error");
                    List<SelectListItem> pf = new List<SelectListItem> { new SelectListItem { Text = "With PF", Value = "With PF" }, new SelectListItem { Text = "Without PF", Value = "Without PF" } };
                    ViewData["PFEligible"] = pf;
                    ViewBag.MinimumDate = null;
                    return View(nav);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                List<SelectListItem> pf = new List<SelectListItem> { new SelectListItem { Text = "With PF", Value = "With PF" }, new SelectListItem { Text = "Without PF", Value = "Without PF" } };
                ViewData["PFEligible"] = pf;
                ViewBag.MinimumDate = null;
                return View(nav);
            }
        }


        #endregion
        [HttpPost]
        public ActionResult SalaryCalculationAppointment(string gross, string Design, bool PH, string pfEligible)
        {
            try
            {
                SalaryDetailsView tmpSdv;
                SalaryCalculation sc = new SalaryCalculation();
                decimal RecommendSalary = Convert.ToDecimal(gross);
                var output = "fail";
                //if (RecommendSalary > 15000 && pfEligible == "Without PF")
                if (pfEligible == "Without PF") // changed by priya on request from recruitment
                {
                    sc.TAMsalaryWithoutPF(RecommendSalary, Design, PH, out tmpSdv);
                }
                else
                {
                    sc.TAMsalary(RecommendSalary, Design, PH, out tmpSdv);
                }
                output = "success";
                var ReturnVal = new
                {
                    result = output,
                    BasicSalary = tmpSdv.BasicSalary,
                    Bonus = tmpSdv.Bonus,
                    HRA = tmpSdv.HRA,
                    SpecialAllowance = tmpSdv.SpecialAllowance,
                    GrossSalary = tmpSdv.GrossSalary,
                    EmployeePF = tmpSdv.EmployeePF,
                    EmployeeESIC = tmpSdv.EmployeeESIC,
                    ProfessionalTax = tmpSdv.ProfessionalTax,
                    TotalDeduction = tmpSdv.TotalDeduction,
                    NetSalary = tmpSdv.NetSalary,
                    EmployerPF = tmpSdv.EmployerPF,
                    EmployerESIC = tmpSdv.EmployerESIC,
                    Insurance = tmpSdv.Insurance,
                    TotalContribution = tmpSdv.TotalContribution,
                    GrossTotal = tmpSdv.GrossTotal,
                    AgencyFee = tmpSdv.AgencyFee,
                    GrossTotalwithAgencyFee = tmpSdv.GrossTotalwithAgencyFee,
                    ServiceTax = tmpSdv.ServiceTax,
                    TotalSalary = tmpSdv.TotalSalary
                };
                return Json(ReturnVal, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var output = "fail";
                var ReturnVal = new
                {
                    result = output
                };
                return Json(ReturnVal, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public ActionResult TAMSalaryCalculation()
        {
            using (RecruitEntities recruit = new RecruitEntities())
            {
                List<SelectListItem> desig = recruit.OutSourcingDesignations.Select(em => new SelectListItem { Text = em.DesignationName, Value = em.DesignationCode }).ToList();
                List<SelectListItem> pf = new List<SelectListItem> { new SelectListItem { Text = "With PF", Value = "With PF" }, new SelectListItem { Text = "Without PF", Value = "Without PF" } };
                ViewData["Designation"] = desig;
                ViewData["PFEligible"] = pf;
                return View();
            }
        }

        [HttpPost]
        [MultipleButton(Name = "action", Argument = "TAMSalaryCalculation")]
        public ActionResult TAMSalaryCalculation(decimal RecommendSalary, string Design, string PFEligible, bool PH,bool PaySplit)
        {
            using (RecruitEntities recruit = new RecruitEntities())
            {
                List<SelectListItem> desig = recruit.OutSourcingDesignations.Select(em => new SelectListItem { Text = em.DesignationName, Value = em.DesignationCode }).ToList();
                List<SelectListItem> pf = new List<SelectListItem> { new SelectListItem { Text = "With PF", Value = "With PF" }, new SelectListItem { Text = "Without PF", Value = "Without PF" } };
                ViewData["Designation"] = desig;
                ViewData["PFEligible"] = pf;
            }
            try
            {
                SalaryDetailsView sdv;
                SalaryCalculation sc = new SalaryCalculation();
                //if (RecommendSalary > 15000 && PFEligible == "Without PF")
                if (PFEligible == "Without PF")
                {
                    sc.TAMsalaryWithoutPF(Convert.ToDecimal(RecommendSalary), Design, PH, out sdv);
                }
                else
                {
                    sc.TAMsalary(Convert.ToDecimal(RecommendSalary), Design, PH, out sdv);
                }
                return View(sdv);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }
        [HttpPost]
        [MultipleButton(Name = "action", Argument = "rptsalary")]
        public ActionResult rptsalary(SalaryDetailsModel sal)
        {   
           
            OutsourcingMeetingDS ds = new OutsourcingMeetingDS();
            if (sal.type == "Existing Employee")
            {
                Outsourcing.Reports.salary rpt = new Outsourcing.Reports.salary();
                rpt.Load();
                DataRow dr = ds.Tables["SalaryDetails"].NewRow();
                dr[0] = sal.salaryDetailsView.BasicSalary;
                dr[1] = sal.salaryDetailsView.HRA;
                dr[2] = sal.salaryDetailsView.Bonus;
                dr[3] = sal.salaryDetailsView.SpecialAllowance;
                dr[4] = sal.salaryDetailsView.GrossSalary;
                dr[5] = sal.salaryDetailsView.EmployeePF;
                dr[6] = sal.salaryDetailsView.EmployeeESIC;
                dr[7] = sal.salaryDetailsView.ProfessionalTax;
                dr[8] = sal.salaryDetailsView.NetSalary;
                dr[9] = sal.salaryDetailsView.EmployerPF;
                dr[10] = sal.salaryDetailsView.EmployerESIC;
                dr[11] = sal.salaryDetailsView.Insurance;
                dr[12] = sal.salaryDetailsView.TotalContribution;
                dr[13] = sal.salaryDetailsView.GrossTotal;
                dr[14] = sal.salaryDetailsView.AgencyFee;
                dr[15] = sal.salaryDetailsView.ServiceTax;
                dr[16] = sal.salaryDetailsView.TotalSalary;
                dr[17] = sal.EmployeeName;
                dr[18] = sal.desig;
                //TextObject t = rpt.ReportDefinition.Sections(0).ReportObjects("TXTNAME");
                //ds.SalaryDetails.Rows.Add(frm[0].ToString());
                ds.Tables["SalaryDetails"].Rows.Add(dr);
                rpt.SetDataSource(ds);
                //rpt.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, true, "Salary");
                Stream fileStream = rpt.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                return File(fileStream, "application/pdf");
            }


            else if (sal.type == "New Employee")
            {
                ReportDocument rpt;
                if (sal.SalaryDetailsNewView.RecommendedSalary <= 15000)
                {
                    rpt = new Outsourcing.Reports.salaryNew();
                    rpt.Load(Server.MapPath(@"~/Reports/salaryNew"));
                }
                else if ((sal.SalaryDetailsNewView.RecommendedSalary > 15000 && sal.SalaryDetailsNewView.RecommendedSalary <= 21000 && sal.PH == false) || (sal.PH == true && sal.SalaryDetailsNewView.RecommendedSalary <= 25000))
                {
                    rpt = new Outsourcing.Reports.salaryNewESI();
                    rpt.Load(Server.MapPath(@"~/Reports/salaryNewESI"));
                }
                else
                {
                    rpt = new Outsourcing.Reports.salaryNewNoPFESIC();
                    rpt.Load(Server.MapPath(@"~/Reports/salaryNewNoPFESIC"));
                }
                    DataRow dr = ds.Tables["SalaryDetailsNew"].NewRow();
                    dr[0] = sal.SalaryDetailsNewView.RecommendedSalary;
                    dr[1] = sal.SalaryDetailsNewView.EmployeePF;
                    dr[2] = sal.SalaryDetailsNewView.EmployeeESIC;
                    dr[5] = sal.SalaryDetailsNewView.EmployerPF;
                    dr[6] = sal.SalaryDetailsNewView.EmployerESIC;
                    dr[3] = sal.SalaryDetailsNewView.ProfessionalTax;
                    dr[4] = sal.SalaryDetailsNewView.NetSalary;
                    dr[7] = sal.SalaryDetailsNewView.Insurance;
                    dr[8] = sal.SalaryDetailsNewView.TotalContribution;
                    dr[9] = sal.SalaryDetailsNewView.GrossTotal;
                    dr[10] = sal.SalaryDetailsNewView.AgencyFee;
                    dr[11] = sal.SalaryDetailsNewView.GST;
                    dr[12] = sal.SalaryDetailsNewView.TotalSalary;
                    dr[13] = sal.EmployeeName;
                dr[14] = sal.desig;
                ds.Tables["SalaryDetailsNew"].Rows.Add(dr);
                    rpt.SetDataSource(ds);
                    //rpt.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, true, "Salary");
                    Stream fileStream = rpt.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    return File(fileStream, "application/pdf");
               
            }
            else
            {
                Outsourcing.Reports.salaryNewPart rpt = new Outsourcing.Reports.salaryNewPart();
                rpt.Load();
                DataRow dr = ds.Tables["SalaryDetailsNew"].NewRow();
                dr[0] = sal.SalaryDetailsNewView.RecommendedSalary;
                dr[1] = sal.SalaryDetailsNewView.EmployeePF;
                dr[2] = sal.SalaryDetailsNewView.EmployeeESIC;
                dr[5] = sal.SalaryDetailsNewView.EmployerPF;
                dr[6] = sal.SalaryDetailsNewView.EmployerESIC;
                dr[3] = sal.SalaryDetailsNewView.ProfessionalTax;
                dr[4] = sal.SalaryDetailsNewView.NetSalary;
                dr[7] = sal.SalaryDetailsNewView.Insurance;
                dr[8] = sal.SalaryDetailsNewView.TotalContribution;
                dr[9] = sal.SalaryDetailsNewView.GrossTotal;
                dr[10] = sal.SalaryDetailsNewView.AgencyFee;
                dr[11] = sal.SalaryDetailsNewView.GST;
                dr[12] = sal.SalaryDetailsNewView.TotalSalary;
                dr[13] = sal.EmployeeName;
                dr[14] = sal.desig;
                ds.Tables["SalaryDetailsNew"].Rows.Add(dr);
                rpt.SetDataSource(ds);
                //rpt.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, true, "Salary");
                Stream fileStream = rpt.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                return File(fileStream, "application/pdf");
            }
        }
        #region appointment list
        [HttpGet]
        public ActionResult AppointmentList(string SearchName, string SearchID, int? Page)
        {
            using (RecruitEntities recruit = new RecruitEntities())
            {
                List<AppointmentList> appList;
                DateTime sdob;
                string SearchDOB = null;
                //if (!string.IsNullOrEmpty(SearchID))
                //if (dob.Length == 8)
                //{
                //    SearchDOB = dob.Substring(0, 2) + "/" + dob.Substring(2, 2) + "/" + dob.Substring(4, 4);
                //}
                //bool isDate = DateTime.TryParse(Convert.ToString(SearchDOB), out sdob);
                if (!string.IsNullOrEmpty(SearchID) && !string.IsNullOrEmpty(SearchName))
                    //appList = (from m in recruit.AppointmentMasters where (m.EmployeeName.Contains(SearchName) && m.DOB.Equals(sdob)) select new AppointmentList { EmployeeID = m.EmployeeID, EmployeeName = m.EmployeeName, DOB = m.DOB, DesignationCode = m.DesignationCode, AppointmentDate = m.AppointmentDate, ToDate = m.ToDate, RelieveDate = m.RelieveDate, BasicSalary = m.BasicSalary }).ToList();
                    appList = (from m in recruit.AppointmentMasters where (m.EmployeeName.Contains(SearchName) && m.EmployeeID.Contains(SearchID)) select new AppointmentList { EmployeeID = m.EmployeeID, EmployeeName = m.EmployeeName, DOB = m.DOB, DesignationCode = m.DesignationCode, AppointmentDate = m.AppointmentDate, ToDate = m.ToDate, RelieveDate = m.RelieveDate, BasicSalary = m.BasicSalary }).ToList();
                else if (string.IsNullOrEmpty(SearchName) && !string.IsNullOrEmpty(SearchID))
                    // appList = (from m in recruit.AppointmentMasters where m.DOB.Equals(sdob) select new AppointmentList { EmployeeID = m.EmployeeID, EmployeeName = m.EmployeeName, DOB = m.DOB, DesignationCode = m.DesignationCode, AppointmentDate = m.AppointmentDate, ToDate = m.ToDate, RelieveDate = m.RelieveDate, BasicSalary = m.BasicSalary }).ToList();
                    appList = (from m in recruit.AppointmentMasters where m.EmployeeID.Contains(SearchID) select new AppointmentList { EmployeeID = m.EmployeeID, EmployeeName = m.EmployeeName, DOB = m.DOB, DesignationCode = m.DesignationCode, AppointmentDate = m.AppointmentDate, ToDate = m.ToDate, RelieveDate = m.RelieveDate, BasicSalary = m.BasicSalary }).ToList();
                else if (!string.IsNullOrEmpty(SearchName) && string.IsNullOrEmpty(SearchID))
                    appList = (from m in recruit.AppointmentMasters where m.EmployeeName.Contains(SearchName) select new AppointmentList { EmployeeID = m.EmployeeID, EmployeeName = m.EmployeeName, DOB = m.DOB, DesignationCode = m.DesignationCode, AppointmentDate = m.AppointmentDate, ToDate = m.ToDate, RelieveDate = m.RelieveDate, BasicSalary = m.BasicSalary }).ToList();
                else
                    appList = (from m in recruit.AppointmentMasters orderby m.EmployeeID descending select new AppointmentList { EmployeeID = m.EmployeeID, EmployeeName = m.EmployeeName, DOB = m.DOB, DesignationCode = m.DesignationCode, AppointmentDate = m.AppointmentDate, ToDate = m.ToDate, RelieveDate = m.RelieveDate, BasicSalary = m.BasicSalary }).Take(10).ToList();
                return View(appList.ToPagedList(Page ?? 1, 10));
            }
        }
        #endregion
        #region appointmentdetails
        public ActionResult AppointmentDetails(string id)
        {
            using (RecruitEntities recruit = new RecruitEntities())
            {
                AppointmentMaster am = (from m in recruit.AppointmentMasters where m.EmployeeID == id select m).Single();
                AppointmentMasterView amv = am;
                ViewData["AppointmentMaster"] = amv;
                AppointmentProject ap = (from m in recruit.AppointmentProjects where m.EmployeeID == id orderby m.AppointmentDate descending select m).Take(1).Single();
                ViewData["appointmentProjects"] = ap;
                List<AppointmentDetailsView> adv = (from d in recruit.AppointmentDetails where d.EmployeeID == id orderby d.OrderID descending select new AppointmentDetailsView { EmployeeName = d.EmployeeName, MeetingID = d.MeetingID, OrderID = d.OrderID, OrderType = d.OrderType, ProjectNo = d.ProjectNo, FromDate = d.FromDate, ToDate = d.ToDate, GrossSalary = d.GrossSalary, CostToProject = d.CostToProject }).ToList();
                string tmp = Convert.ToString(TempData["IDverify"]);
                if (tmp != null && tmp != "") ModelState.AddModelError("Error", tmp);
                return View(adv);
            }
        }
        #endregion

        [HttpPost]
        [MultipleButton(Name = "action", Argument = "AppointmentMasterEdit")]
        public ActionResult AppointmentMasterEdit(string EmployeeID)
        {
            return View();
        }

        [HttpPost]
        [MultipleButton(Name = "action", Argument = "AppointmentMasterDetails")]
        public ActionResult AppointmentMasterDetails(string EmployeeID)
        {
            using (RecruitEntities recruit = new RecruitEntities())
            {
                AppointmentMaster am = (from m in recruit.AppointmentMasters where m.EmployeeID == EmployeeID select m).Single();
                AppointmentMasterView amv = am;
                amv.CreatedBy = am.CreatedBy;
                amv.CreatedOn = am.CreatedOn;
                amv.UpdatedBy = am.UpdatedBy;
                amv.UpdatedOn = am.UpdatedOn;
                return View(amv);
            }
        }

        [HttpPost]
        [MultipleButton(Name = "action", Argument = "AppointmentDetailsEdit")]
        public ActionResult AppointmentDetailsEdit(string EmployeeID, string MeetingID, int OrderID)
        {
            using (RecruitEntities recruit = new RecruitEntities())
            {
                AppointmentDetail ad = (from m in recruit.AppointmentDetails where (m.EmployeeID == EmployeeID && m.MeetingID == MeetingID && m.OrderID == OrderID) select m).Single();
                AppointmentDetailsView adv = ad;
                SalaryDetailsView sdv = new SalaryDetailsView();
                if (ad.OrderType == "Appointment" || ad.OrderType == "Extension" || ad.OrderType == "Enhancement")
                {
                    SalaryDetail sd = (from m in recruit.SalaryDetails where (m.EmployeeID == EmployeeID && m.OrderID == OrderID) select m).Single();
                    sdv = sd;
                    sdv.TotalDeduction = sdv.EmployeePF + sdv.EmployeeESIC + sdv.ProfessionalTax;
                    sdv.GrossTotalwithAgencyFee = sdv.GrossTotal + sdv.AgencyFee;
                }

                if (ad.OrderType == "Appointment")
                {
                    return View();
                }
                else if (ad.OrderType == "Extension")
                {
                    ExtensionOrderView eov = new ExtensionOrderView();
                    eov.appointmentDetailsView = adv;
                    eov.salaryDetailsView = sdv;
                    return RedirectToAction("ExtensionOrder", "Outsource", eov);
                }
                else if (ad.OrderType == "Enhancement")
                {
                    EnhancementOrderView eov = new EnhancementOrderView();
                    eov.appointmentDetailsView = adv;
                    eov.salaryDetailsView = sdv;
                    return RedirectToAction("EnhancementOrder", "Outsource", new { EmployeeID = adv.EmployeeID });
                }
                else
                {
                    return View();
                }
            }
        }

        [HttpPost]
        [MultipleButton(Name = "action", Argument = "AppointmentDetailsDetails")]
        public ActionResult AppointmentDetailsDetails(string EmployeeID, string MeetingID, int OrderID, string Command)
        {
            using (RecruitEntities recruit = new RecruitEntities())
            {
                //int oID = Convert.ToInt32(OrderID);
                AppointmentDetail ad = (from m in recruit.AppointmentDetails where (m.EmployeeID == EmployeeID && m.MeetingID == MeetingID && m.OrderID == OrderID) select m).Single();
                AppointmentDetailsView adv = ad;
                adv.CreatedBy = ad.CreatedBy;
                adv.CreatedOn = ad.CreatedOn;
                adv.UpdatedBy = ad.UpdatedBy;
                adv.UpdatedOn = ad.UpdatedOn;
                SalaryDetail sd = (from m in recruit.SalaryDetails where (m.EmployeeID == EmployeeID && m.OrderID == OrderID) select m).Single();
                SalaryDetailsView sdv = sd;
                sdv.TotalDeduction = sdv.EmployeePF + sdv.EmployeeESIC + sdv.ProfessionalTax;
                sdv.GrossTotalwithAgencyFee = sdv.GrossTotal + sdv.AgencyFee;
                ExtensionOrderView eov = new ExtensionOrderView();
                eov.appointmentDetailsView = adv;
                eov.salaryDetailsView = sdv;
                return View(eov);
            }
        }
        //[HttpPost]
        //[MultipleButton(Name = "action", Argument = "AppointmentDetailsDelete")]
        //public ActionResult AppointmentDetailsDelete(string EmployeeID, string MeetingID, int OrderID, string Command)
        //{
        //    using (TransactionScope trans = new TransactionScope())
        //    {
        //        //TempData[""]
        //        using (RecruitEntities recruit = new RecruitEntities())
        //        {
        //            AppointmentDetail ad = (from m in recruit.AppointmentDetails where (m.EmployeeID == EmployeeID && m.MeetingID == MeetingID && m.OrderID == OrderID) select m).FirstOrDefault();
        //            recruit.AppointmentDetails.Remove(ad);

        //        }
        //    }
        //    return View();

            
        //}
        #region extension order
        [HttpPost]
        [MultipleButton(Name = "action", Argument = "ExtensionOrder")]
        public ActionResult ExtensionOrder(string EmployeeID, string MeetingID, int OrderID, string cmd)
        {
            if (cmd == "New")
            {
                using (RecruitEntities recruit = new RecruitEntities())
                {
                    AppointmentDetail ad = (from m in recruit.AppointmentDetails where m.EmployeeID == EmployeeID && (m.OrderType == "Appointment" || m.OrderType == "Extension") orderby m.OrderID descending select m).Take(1).Single();
                    AppointmentDetailsView adv = ad;
                    var tmpAd1 = (from m in recruit.AppointmentDetails where m.EmployeeID == EmployeeID && m.OrderID >= adv.OrderID && (m.OrderType == "Appointment" || m.OrderType == "Extension" || m.OrderType == "Enhancement") orderby m.OrderID descending select new { OrderID = m.OrderID, BasicSalary = m.BasicSalary, GrossSalary = m.GrossSalary }).Take(1).Single();
                    adv.OrderType = "Extension";
                    adv.FromDate = Convert.ToDateTime(adv.ToDate).AddDays(1);
                    adv.ToDate = null;
                    adv.BasicSalary = tmpAd1.BasicSalary;
                    adv.Remarks = "";
                    SalaryDetail sd = (from m in recruit.SalaryDetails where m.EmployeeID == EmployeeID && m.OrderID == tmpAd1.OrderID select m).Single();
                    SalaryDetailsView sdv = sd;
                    sdv.TotalDeduction = sdv.EmployeePF + sdv.EmployeeESIC + sdv.ProfessionalTax;
                    sdv.GrossTotalwithAgencyFee = sdv.GrossTotal + sdv.AgencyFee;
                    ExtensionOrderView eov = new ExtensionOrderView();
                    eov.appointmentDetailsView = adv;
                    eov.salaryDetailsView = sdv;
                    eov.Command = "Insert";
                    ViewBag.Title = "New Extension Order";
                    ViewData["EmployeeID"] = eov.appointmentDetailsView.EmployeeID;
                    return View(eov);
                }
            }
            else if (cmd == "Update")
            {
                using (RecruitEntities recruit = new RecruitEntities())
                {
                    AppointmentDetail ad = (from m in recruit.AppointmentDetails where (m.EmployeeID == EmployeeID && m.MeetingID == MeetingID && m.OrderID == OrderID) select m).Single();
                    AppointmentDetailsView adv = ad;
                    SalaryDetailsView sdv = new SalaryDetailsView();
                    SalaryDetail sd = (from m in recruit.SalaryDetails where (m.EmployeeID == EmployeeID && m.OrderID == OrderID) select m).Single();
                    sdv = sd;
                    sdv.TotalDeduction = sdv.EmployeePF + sdv.EmployeeESIC + sdv.ProfessionalTax;
                    sdv.GrossTotalwithAgencyFee = sdv.GrossTotal + sdv.AgencyFee;
                    ExtensionOrderView eov = new ExtensionOrderView();
                    eov.appointmentDetailsView = adv;
                    eov.salaryDetailsView = sdv;
                    eov.Command = "Update";
                    ViewBag.Title = "Update Extension Order";
                    ViewData["EmployeeID"] = eov.appointmentDetailsView.EmployeeID;
                    return View(eov);
                }
            }
            else
            {
                AppointmentDetailsView adv = new AppointmentDetailsView();
                SalaryDetailsView sdv = new SalaryDetailsView();
                EnhancementOrderView eov = new EnhancementOrderView();
                eov.appointmentDetailsView = adv;
                eov.salaryDetailsView = sdv;
                return View(eov);
            }
        }

        [HttpPost]
        public ActionResult ExtensionOrder(ExtensionOrderView eov)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (eov.Command == "Insert")
                    {
                        using (RecruitEntities recruit = new RecruitEntities())
                        {

                            int orderID = (from m in recruit.AppointmentDetails where m.EmployeeID == eov.appointmentDetailsView.EmployeeID select m.OrderID).Max();
                            orderID = orderID + 1;
                            eov.appointmentDetailsView.OrderID = orderID;
                            eov.appointmentDetailsView.BasicSalary = eov.salaryDetailsView.BasicSalary;
                            eov.appointmentDetailsView.GrossSalary = eov.salaryDetailsView.GrossSalary;
                            eov.appointmentDetailsView.CostToProject = eov.salaryDetailsView.TotalSalary;
                            eov.salaryDetailsView.EmployeeID = eov.appointmentDetailsView.EmployeeID;
                            eov.salaryDetailsView.OrderID = orderID;
                            eov.salaryDetailsView.OrderType = eov.appointmentDetailsView.OrderType;
                            using (TransactionScope transaction = new TransactionScope())
                            {
                                recruit.Database.Connection.Open();
                                AppointmentDetail ad = eov.appointmentDetailsView;
                                ad.CreatedOn = DateTime.Today;
                                ad.CreatedBy = Session["UserName"].ToString();
                                recruit.AppointmentDetails.Add(ad);
                                SalaryDetail sd = eov.salaryDetailsView;
                                recruit.SalaryDetails.Add(sd);
                                OrderRequestDetail ord = new OrderRequestDetail();
                                ord.EmployeeID = eov.appointmentDetailsView.EmployeeID;
                                ord.EmployeeName = eov.appointmentDetailsView.EmployeeName;
                                ord.OrderID = eov.appointmentDetailsView.OrderID;
                                ord.OrderType = eov.appointmentDetailsView.OrderType;
                                ord.CreatedOn = DateTime.Today;
                                ord.CreatedBy = Session["UserName"].ToString();
                                recruit.OrderRequestDetails.Add(ord);
                                var ap = new AppointmentProject { EmployeeID = eov.appointmentDetailsView.EmployeeID, MeetingID = eov.appointmentDetailsView.MeetingID, ToDate = Convert.ToDateTime(eov.appointmentDetailsView.ToDate) };
                                recruit.Configuration.ValidateOnSaveEnabled = false;
                                recruit.AppointmentProjects.Attach(ap);
                                recruit.Entry(ap).Property(em => em.ToDate).IsModified = true;
                                var mv = new AppointmentMaster { EmployeeID = eov.appointmentDetailsView.EmployeeID, BasicSalary = eov.salaryDetailsView.BasicSalary, ToDate = Convert.ToDateTime(eov.appointmentDetailsView.ToDate), UpdatedBy = Session["UserName"].ToString(), UpdatedOn = DateTime.Today };
                                recruit.Configuration.ValidateOnSaveEnabled = false;
                                recruit.AppointmentMasters.Attach(mv);
                                recruit.Entry(mv).Property(em => em.BasicSalary).IsModified = true;
                                recruit.Entry(mv).Property(em => em.ToDate).IsModified = true;
                                recruit.Entry(mv).Property(em => em.UpdatedBy).IsModified = true;
                                recruit.Entry(mv).Property(em => em.UpdatedOn).IsModified = true;
                                recruit.SaveChanges();
                                transaction.Complete();
                                ViewData["EmployeeID"] = eov.appointmentDetailsView.EmployeeID;
                            }



                            ViewData["Result"] = "True";
                            ViewData["status"] = "Update";
                            return View(eov);
                        }
                    }
                    else if (eov.Command == "Update")
                    {
                        using (RecruitEntities recruit = new RecruitEntities())
                        {
                            eov.appointmentDetailsView.BasicSalary = eov.salaryDetailsView.BasicSalary;
                            eov.appointmentDetailsView.GrossSalary = eov.salaryDetailsView.GrossSalary;
                            eov.appointmentDetailsView.CostToProject = eov.salaryDetailsView.TotalSalary;
                            eov.salaryDetailsView.EmployeeID = eov.appointmentDetailsView.EmployeeID;
                            eov.salaryDetailsView.OrderID = eov.appointmentDetailsView.OrderID;
                            eov.salaryDetailsView.OrderType = eov.appointmentDetailsView.OrderType;
                            using (TransactionScope transaction = new TransactionScope())
                            {
                                recruit.Database.Connection.Open();
                                AppointmentDetail ad = eov.appointmentDetailsView;
                                ad.UpdatedOn = DateTime.Today;
                                ad.UpdatedBy = Session["UserName"].ToString();
                                recruit.Entry(ad).State = EntityState.Modified;
                                recruit.Entry(ad).Property(em => em.CreatedBy).IsModified = false;
                                recruit.Entry(ad).Property(em => em.CreatedOn).IsModified = false;
                                SalaryDetail sd = eov.salaryDetailsView;
                                recruit.Entry(sd).State = EntityState.Modified;
                                var ap = new AppointmentProject { EmployeeID = eov.appointmentDetailsView.EmployeeID, MeetingID = eov.appointmentDetailsView.MeetingID, ToDate = Convert.ToDateTime(eov.appointmentDetailsView.ToDate) };
                                recruit.Configuration.ValidateOnSaveEnabled = false;
                                recruit.AppointmentProjects.Attach(ap);
                                recruit.Entry(ap).Property(em => em.ToDate).IsModified = true;
                                var mv = new AppointmentMaster { EmployeeID = eov.appointmentDetailsView.EmployeeID, BasicSalary = eov.salaryDetailsView.BasicSalary, ToDate = Convert.ToDateTime(eov.appointmentDetailsView.ToDate), UpdatedBy = Session["UserName"].ToString(), UpdatedOn = DateTime.Today };
                                recruit.Configuration.ValidateOnSaveEnabled = false;
                                recruit.AppointmentMasters.Attach(mv);
                                recruit.Entry(mv).Property(em => em.BasicSalary).IsModified = true;
                                recruit.Entry(mv).Property(em => em.ToDate).IsModified = true;
                                recruit.Entry(mv).Property(em => em.UpdatedBy).IsModified = true;
                                recruit.Entry(mv).Property(em => em.UpdatedOn).IsModified = true;
                                recruit.SaveChanges();
                                transaction.Complete();
                                ViewData["EmployeeID"] = eov.appointmentDetailsView.EmployeeID;
                            }

                            ViewData["Result"] = "True";
                            ViewData["status"] = "Update";
                            return View(eov);
                        }
                    }
                    else
                    {
                        ViewData["EmployeeID"] = eov.appointmentDetailsView.EmployeeID;
                        return View(eov);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Validation Error");
                    ViewData["EmployeeID"] = eov.appointmentDetailsView.EmployeeID;
                    return View(eov);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewData["EmployeeID"] = eov.appointmentDetailsView.EmployeeID;
                return View(eov);
            }
        }
        [HttpPost]
        [MultipleButton(Name = "action", Argument = "rptext")]
        public ActionResult rptextension(string EmployeeID, int OrderID)
        {
            using (RecruitEntities recruit = new RecruitEntities())
            {
                using (SqlConnection con1 = new SqlConnection())
                {
                    con1.ConnectionString = ConfigurationManager.ConnectionStrings["Recruit"].ConnectionString;
                    string sql = "select ad.EmployeeID,ad.EmployeeName,DOB,om.DesignationName,om.ProjectType,ad.ProjectNo,om.ProjectTitle,om.DepartmentName,PIName,ad.FromDate,ad.ToDate,ad.CostToProject,ad.CommitmentNo from OutsourcingMeeting as om inner join AppointmentDetails as ad on ad.MeetingID=om.MeetingID and ad.EmployeeID='" + EmployeeID + "' and ad.OrderID='" + OrderID + "'";
                    SqlDataAdapter sda = new SqlDataAdapter(sql, con1);
                    OutsourcingMeetingDS ds = new OutsourcingMeetingDS();
                    sda.Fill(ds.Tables["officeorder"]);
                    Outsourcing.Reports.Extension rpt = new Outsourcing.Reports.Extension();
                    rpt.Load();
                    rpt.SetDataSource(ds);
                    rpt.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, true, EmployeeID + " Extension");
                    Stream fileStream = rpt.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    return File(fileStream, "application/pdf");

                }
            }
        }
        #endregion
        #region enhancement order
        [HttpPost]
        [MultipleButton(Name = "action", Argument = "EnhancementOrder")]
        public ActionResult EnhancementOrder(string EmployeeID, string MeetingID, int OrderID, string cmd)
        {
            if (cmd == "New")
            {
                using (RecruitEntities recruit = new RecruitEntities())
                {
                    EnhancementOrderView eov = new EnhancementOrderView();
                    AppointmentDetail ad = (from m in recruit.AppointmentDetails where m.EmployeeID == EmployeeID && (m.OrderType == "Appointment" || m.OrderType == "Extension") orderby m.OrderID descending select m).Take(1).Single();
                    AppointmentDetailsView adv = ad;
                    var tmpAd1 = (from m in recruit.AppointmentDetails where m.EmployeeID == EmployeeID && m.OrderID >= adv.OrderID && (m.OrderType == "Appointment" || m.OrderType == "Extension" || m.OrderType == "Enhancement") orderby m.OrderID descending select new { OrderID = m.OrderID, BasicSalary = m.BasicSalary, GrossSalary = m.GrossSalary }).Take(1).Single();
                    adv.OrderType = "Enhancement";
                    eov.PreviousFromDate = (from m in recruit.AppointmentDetails where m.EmployeeID == EmployeeID && (m.OrderType == "Appointment") orderby m.OrderID descending select m.FromDate).FirstOrDefault();
                    adv.FromDate = null;
                    adv.BasicSalary = tmpAd1.BasicSalary;
                    adv.Remarks = "";
                    SalaryDetail sd = (from m in recruit.SalaryDetails where m.EmployeeID == EmployeeID && m.OrderID == tmpAd1.OrderID select m).Single();
                    SalaryDetailsView sdv = sd;
                    sdv.TotalDeduction = sdv.EmployeePF + sdv.EmployeeESIC + sdv.ProfessionalTax;
                    sdv.GrossTotalwithAgencyFee = sdv.GrossTotal + sdv.AgencyFee;
                    eov.appointmentDetailsView = adv;
                    eov.salaryDetailsView = sdv;
                    eov.Command = "Insert";
                    ViewBag.Title = "New Enhancement Order";
                    ViewData["EmployeeID"] = eov.appointmentDetailsView.EmployeeID;
                    return View(eov);
                }
            }
            else if (cmd == "Update")
            {
                using (RecruitEntities recruit = new RecruitEntities())
                {
                    AppointmentDetail ad = (from m in recruit.AppointmentDetails where (m.EmployeeID == EmployeeID && m.MeetingID == MeetingID && m.OrderID == OrderID) select m).Single();
                    AppointmentDetailsView adv = ad;
                    SalaryDetailsView sdv = new SalaryDetailsView();
                    SalaryDetail sd = (from m in recruit.SalaryDetails where (m.EmployeeID == EmployeeID && m.OrderID == OrderID) select m).Single();
                    sdv = sd;
                    sdv.TotalDeduction = sdv.EmployeePF + sdv.EmployeeESIC + sdv.ProfessionalTax;
                    sdv.GrossTotalwithAgencyFee = sdv.GrossTotal + sdv.AgencyFee;
                    EnhancementOrderView eov = new EnhancementOrderView();
                    eov.appointmentDetailsView = adv;
                    eov.salaryDetailsView = sdv;
                    eov.PreviousFromDate = (from m in recruit.AppointmentDetails where m.EmployeeID == EmployeeID && (m.OrderType == "Appointment") orderby m.OrderID descending select m.FromDate).FirstOrDefault();
                    eov.Command = "Update";
                    ViewBag.Title = "Update Enhancement Order";
                    ViewData["EmployeeID"] = eov.appointmentDetailsView.EmployeeID;
                    return View(eov);
                }
            }
            else
            {
                AppointmentDetailsView adv = new AppointmentDetailsView();
                SalaryDetailsView sdv = new SalaryDetailsView();
                EnhancementOrderView eov = new EnhancementOrderView();
                eov.appointmentDetailsView = adv;
                eov.salaryDetailsView = sdv;
                return View(eov);
            }
        }

        [HttpPost]
        public ActionResult EnhancementOrder(EnhancementOrderView eov)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (eov.Command == "Insert")
                    {
                        using (RecruitEntities recruit = new RecruitEntities())
                        {
                            int orderID = (from m in recruit.AppointmentDetails where m.EmployeeID == eov.appointmentDetailsView.EmployeeID select m.OrderID).Max();
                            orderID = orderID + 1;
                            eov.appointmentDetailsView.OrderID = orderID;
                            eov.appointmentDetailsView.BasicSalary = eov.salaryDetailsView.BasicSalary;
                            eov.appointmentDetailsView.GrossSalary = eov.salaryDetailsView.GrossSalary;
                            eov.appointmentDetailsView.CostToProject = eov.salaryDetailsView.TotalSalary;
                            eov.salaryDetailsView.EmployeeID = eov.appointmentDetailsView.EmployeeID;
                            eov.salaryDetailsView.OrderID = orderID;
                            eov.salaryDetailsView.OrderType = eov.appointmentDetailsView.OrderType;
                            using (TransactionScope transaction = new TransactionScope())
                            {
                                recruit.Database.Connection.Open();
                                AppointmentDetail ad = eov.appointmentDetailsView;
                                ad.CreatedOn = DateTime.Today;
                                ad.CreatedBy = Session["UserName"].ToString();
                                recruit.AppointmentDetails.Add(ad);
                                SalaryDetail sd = eov.salaryDetailsView;
                                recruit.SalaryDetails.Add(sd);
                                OrderRequestDetail ord = new OrderRequestDetail();
                                ord.EmployeeID = eov.appointmentDetailsView.EmployeeID;
                                ord.EmployeeName = eov.appointmentDetailsView.EmployeeName;
                                ord.OrderID = eov.appointmentDetailsView.OrderID;
                                ord.OrderType = eov.appointmentDetailsView.OrderType;
                                ord.CreatedOn = DateTime.Today;
                                ord.CreatedBy = Session["UserName"].ToString();
                                recruit.OrderRequestDetails.Add(ord);
                                var mv = new AppointmentMaster { EmployeeID = eov.appointmentDetailsView.EmployeeID, BasicSalary = eov.salaryDetailsView.BasicSalary, UpdatedBy = Session["UserName"].ToString(), UpdatedOn = DateTime.Today };
                                recruit.Configuration.ValidateOnSaveEnabled = false;
                                recruit.AppointmentMasters.Attach(mv);
                                recruit.Entry(mv).Property(em => em.BasicSalary).IsModified = true;
                                recruit.Entry(mv).Property(em => em.UpdatedBy).IsModified = true;
                                recruit.Entry(mv).Property(em => em.UpdatedOn).IsModified = true;
                                recruit.SaveChanges();
                                transaction.Complete();
                                ViewData["EmployeeID"] = eov.appointmentDetailsView.EmployeeID;
                            }
                            ViewData["Result"] = "True";
                            ViewData["status"] = "Insert";
                            return View(eov);
                        }
                    }
                    else if (eov.Command == "Update")
                    {
                        using (RecruitEntities recruit = new RecruitEntities())
                        {
                            eov.appointmentDetailsView.BasicSalary = eov.salaryDetailsView.BasicSalary;
                            eov.appointmentDetailsView.GrossSalary = eov.salaryDetailsView.GrossSalary;
                            eov.appointmentDetailsView.CostToProject = eov.salaryDetailsView.TotalSalary;                            
                            eov.salaryDetailsView.EmployeeID = eov.appointmentDetailsView.EmployeeID;
                            eov.salaryDetailsView.OrderID = eov.appointmentDetailsView.OrderID;
                            eov.salaryDetailsView.OrderType = eov.appointmentDetailsView.OrderType;
                            using (TransactionScope transaction = new TransactionScope())
                            {
                                recruit.Database.Connection.Open();
                                AppointmentDetail ad = eov.appointmentDetailsView;
                                ad.UpdatedOn = DateTime.Today;
                                ad.UpdatedBy = Session["UserName"].ToString();                              
                                recruit.Entry(ad).State = EntityState.Modified;
                                recruit.Entry(ad).Property(em => em.FromDate).IsModified = true; //added privileage to chamge from date as per request from recruitment
                                recruit.Entry(ad).Property(em => em.CreatedBy).IsModified = false;
                                recruit.Entry(ad).Property(em => em.CreatedOn).IsModified = false;
                                recruit.SaveChanges();
                                SalaryDetail sd = eov.salaryDetailsView;
                                recruit.Entry(sd).State = EntityState.Modified;
                                recruit.SaveChanges();
                                var mv = new AppointmentMaster { EmployeeID = eov.appointmentDetailsView.EmployeeID, BasicSalary = eov.salaryDetailsView.BasicSalary, UpdatedBy = Session["UserName"].ToString(), UpdatedOn = DateTime.Today };
                                recruit.Configuration.ValidateOnSaveEnabled = false;
                                recruit.AppointmentMasters.Attach(mv);
                                recruit.Entry(mv).Property(em => em.BasicSalary).IsModified = true;
                                recruit.Entry(mv).Property(em => em.UpdatedBy).IsModified = true;
                                recruit.Entry(mv).Property(em => em.UpdatedOn).IsModified = true;
                                recruit.SaveChanges();
                                transaction.Complete();
                                ViewData["EmployeeID"] = eov.appointmentDetailsView.EmployeeID;
                            }
                            ViewData["Result"] = "True";
                            ViewData["status"] = "Update";
                            return View(eov);
                        }
                    }
                    else
                    {
                        ViewData["EmployeeID"] = eov.appointmentDetailsView.EmployeeID;
                        return View(eov);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Validation Error");
                    return View(eov);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(eov);
            }
        }
        [HttpPost]
        [MultipleButton(Name = "action", Argument = "rptenhance")]
        public ActionResult rptenhance(string EmployeeID, int OrderID)
        {
            using (RecruitEntities recruit = new RecruitEntities())
            {
                using (SqlConnection con1 = new SqlConnection())
                {
                    con1.ConnectionString = ConfigurationManager.ConnectionStrings["Recruit"].ConnectionString;
                    string sql = "select ad.EmployeeID,ad.EmployeeName,DOB,om.DesignationName,om.ProjectType,ad.ProjectNo,om.ProjectTitle,om.DepartmentName,PIName,ad.FromDate,ad.ToDate,ad.CostToProject,ad.CommitmentNo from OutsourcingMeeting as om inner join AppointmentDetails as ad on ad.MeetingID=om.MeetingID and ad.EmployeeID='" + EmployeeID + "' and ad.OrderID='" + OrderID + "'";
                    SqlDataAdapter sda = new SqlDataAdapter(sql, con1);
                    OutsourcingMeetingDS ds = new OutsourcingMeetingDS();
                    sda.Fill(ds.Tables["officeorder"]);
                    Outsourcing.Reports.Enhancement rpt = new Outsourcing.Reports.Enhancement();
                    rpt.Load();
                    rpt.SetDataSource(ds);
                    rpt.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, true, EmployeeID + " Enhancement");
                    Stream fileStream = rpt.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    return File(fileStream, "application/pdf");

                }
            }
        }
        #endregion
        #region stoppayment  

        [HttpPost]
        [MultipleButton(Name = "action", Argument = "stoppayment")]
        public ActionResult stoppayment(string EmployeeID, string MeetingID, int OrderID, string cmd)
        {

            using (RecruitEntities recruit = new RecruitEntities())
            {
                if (cmd == "New")
                {
                    var view = (from ad in recruit.AppointmentDetails
                                where ad.EmployeeID == EmployeeID && ad.MeetingID == MeetingID
                                select new stoppaymentview
                                {
                                    EmployeeID = ad.EmployeeID,
                                    EmployeeName = ad.EmployeeName,
                                    MeetingID = ad.MeetingID,
                                    OrderID = ad.OrderID,
                                    OrderType = "StopPayment",
                                    Command = "Insert"
                                }).OrderByDescending(h => h.OrderID).FirstOrDefault();
                    return View(view);
                }
                else if (cmd == "Update")
                {
                    var view = (from ad in recruit.AppointmentDetails
                                where ad.EmployeeID == EmployeeID && ad.OrderID == OrderID
                                select new stoppaymentview
                                {
                                    EmployeeID = ad.EmployeeID,
                                    EmployeeName = ad.EmployeeName,
                                    MeetingID = ad.MeetingID,
                                    OrderID = ad.OrderID,
                                    OrderType = ad.OrderType,
                                    Command = cmd,
                                    FromDate = ad.FromDate,
                                    Remarks = ad.Remarks
                                }).FirstOrDefault();
                    return View(view);


                }
            }

            return View();

        }

        [HttpPost]
        public ActionResult stoppayment(stoppaymentview eov)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    using (RecruitEntities recruit = new RecruitEntities())
                    {
                        if (eov.Command == "Insert")
                        {

                            var dbdata = (recruit.AppointmentDetails.OrderByDescending(o => o.OrderID).FirstOrDefault(ad => ad.EmployeeID == eov.EmployeeID && ad.MeetingID == eov.MeetingID));
                            if (dbdata != null)
                            {
                                using (TransactionScope transaction = new TransactionScope())
                                {
                                    // var dbNewData = new AppointmentDetail();
                                    recruit.Database.Connection.Open();
                                    AppointmentDetail ad = new AppointmentDetail()
                                    {
                                        EmployeeID = dbdata.EmployeeID,
                                        EmployeeName = dbdata.EmployeeName,
                                        MeetingID = dbdata.MeetingID,
                                        OrderType = eov.OrderType,
                                        OrderID = dbdata.OrderID + 1,
                                        ProjectNo = dbdata.ProjectNo,
                                        FromDate = eov.FromDate ?? DateTime.Now,
                                        ToDate = dbdata.ToDate,
                                        BasicSalary = dbdata.BasicSalary,
                                        GrossSalary = dbdata.GrossSalary,
                                        CostToProject = dbdata.CostToProject,
                                        CommitmentNo = dbdata.CommitmentNo,
                                        Remarks = eov.Remarks,
                                        CreatedOn = DateTime.Today,
                                        CreatedBy = Session["UserName"].ToString()
                                    };


                                    recruit.AppointmentDetails.Add(ad);
                                    recruit.SaveChanges();
                                    OrderRequestDetail ord = new OrderRequestDetail();
                                    ord.EmployeeID = dbdata.EmployeeID;
                                    ord.EmployeeName = dbdata.EmployeeName;
                                    ord.OrderID = dbdata.OrderID + 1;
                                    ord.OrderType = eov.OrderType;
                                    ord.CreatedOn = DateTime.Today;
                                    ord.CreatedBy = Session["UserName"].ToString();
                                    recruit.OrderRequestDetails.Add(ord);
                                    var sddata = (recruit.SalaryDetails.OrderByDescending(o => o.OrderID).FirstOrDefault(a => a.EmployeeID == eov.EmployeeID));
                                    SalaryDetail sd = new SalaryDetail()
                                    {
                                        EmployeeID = sddata.EmployeeID,
                                        OrderID = sddata.OrderID + 1,
                                        OrderType = eov.OrderType,
                                        BasicSalary = sddata.BasicSalary,
                                        HRA = sddata.HRA,
                                        Bonus = sddata.Bonus,
                                        SpecialAllowance = sddata.SpecialAllowance,
                                        GrossSalary = sddata.GrossSalary,
                                        EmployeePF = sddata.EmployeePF,
                                        EmployeeESIC = sddata.EmployeeESIC,
                                        ProfessionalTax = sddata.ProfessionalTax,
                                        NetSalary = sddata.NetSalary,
                                        EmployerPF = sddata.EmployerPF,
                                        EmployerESIC = sddata.EmployerESIC,
                                        Insurance = sddata.Insurance,
                                        TotalContribution = sddata.TotalContribution,
                                        GrossTotal = sddata.GrossTotal,
                                        AgencyFee = sddata.AgencyFee,
                                        ServiceTax = sddata.ServiceTax,
                                        TotalSalary = sddata.TotalSalary

                                    };
                                    recruit.SalaryDetails.Add(sd);
                                    recruit.SaveChanges();
                                    var l = recruit.AppointmentProjects.Where(m => m.EmployeeID.Equals(eov.EmployeeID)).FirstOrDefault();
                                    l.Remarks = eov.OrderType;
                                    recruit.SaveChanges();
                                    var lq = recruit.AppointmentMasters.Where(m => m.EmployeeID.Equals(eov.EmployeeID)).FirstOrDefault();
                                    lq.Remarks = eov.OrderType;
                                    lq.UpdatedBy = Session["UserName"].ToString();
                                    lq.UpdatedOn = DateTime.Now;

                                    //var mv = new AppointmentMaster { EmployeeID = dbdata.EmployeeID, Remarks = eov.Remarks, UpdatedBy = Session["UserName"].ToString(), UpdatedOn = DateTime.Today };
                                    //recruit.Configuration.ValidateOnSaveEnabled = false;
                                    // recruit.AppointmentMasters.Attach(mv);
                                    recruit.SaveChanges();
                                    transaction.Complete();
                                    //ReportDocument rpt = new ReportDocument();
                                    //rpt.Load(Path.Combine(Server.MapPath("~/Reports/stoppaymentrpt.rpt")));
                                    //rpt.SetDataSource(l);
                                    //Stream stream = rpt.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                                    //stream.Seek(0, SeekOrigin.Begin);
                                    //return File(stream, "application/pdf", eov.EmployeeID + ".pdf");
                                    ViewData["Result"] = "True";
                                    ViewData["status"] = "Insert";
                                    ViewData["EmployeeID"] = eov.EmployeeID;
                                }
                            }
                        }//end of insert
                        else if (eov.Command == "Update")
                        {

                            var dbdata = (recruit.AppointmentDetails.OrderByDescending(o => o.OrderID).FirstOrDefault(ad => ad.EmployeeID == eov.EmployeeID && ad.MeetingID == eov.MeetingID));
                            if (dbdata != null)
                            {
                                using (TransactionScope transaction = new TransactionScope())
                                {
                                    recruit.Database.Connection.Open();
                                    dbdata.Remarks = eov.Remarks;
                                    dbdata.UpdatedOn = DateTime.Today;
                                    dbdata.UpdatedBy = Session["UserName"].ToString();
                                    recruit.SaveChanges();
                                    transaction.Complete();
                                    ViewData["Result"] = "True";
                                    ViewData["status"] = "Insert";
                                    ViewData["EmployeeID"] = eov.EmployeeID;
                                }
                            }
                            return View(eov);
                        }
                        else
                        {
                            ModelState.AddModelError("", "Error");
                            return View(eov);
                        }

                    }
                }
                else
                {
                    ModelState.AddModelError("", "Validation Error");
                    return View(eov);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(eov);
            }
            return View(eov);
        }
        [HttpPost]
        [MultipleButton(Name = "action", Argument = "stoppaymentrpt")]
        public ActionResult rptstoppayment(string EmployeeID, int OrderID)
        {
            using (RecruitEntities recruit = new RecruitEntities())
            {
                using (SqlConnection con1 = new SqlConnection())
                {
                    con1.ConnectionString = ConfigurationManager.ConnectionStrings["Recruit"].ConnectionString;
                    string sql = "select am.EmployeeID,am.EmployeeName,om.DOB,om.DesignationName,om.ProjectType,om.ProjectNo,om.ProjectTitle,om.DepartmentName,om.PIName,om.FromDate,om.ToDate,om.CostToProject,om.CommitmentNo from OutsourcingMeeting as om inner join AppointmentDetails as am on om.MeetingID=am.MeetingID where EmployeeID='" + EmployeeID + "' and OrderID='" + OrderID + "'";
                    //"select am.EmployeeID,am.EmployeeName,om.DOB,om.DesignationName,om.ProjectType,om.ProjectNo,om.ProjectTitle,om.DepartmentName,om.PIName,om.FromDate,om.ToDate,om.CostToProject,om.CommitmentNo from OutsourcingMeeting as om inner join AppointmentMaster as am on om.CandidateID=am.CandidateID where om.MeetingID = '" + MeetingID + "')";
                    SqlDataAdapter sda = new SqlDataAdapter(sql, con1);
                    OutsourcingMeetingDS ds = new OutsourcingMeetingDS();
                    sda.Fill(ds.Tables["officeorder"]);
                    Outsourcing.Reports.stoppaymentrpt rpt = new Outsourcing.Reports.stoppaymentrpt();
                    rpt.Load();
                    rpt.SetDataSource(ds);
                    rpt.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, true, EmployeeID + " Stop Payment");
                    Stream fileStream = rpt.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    return File(fileStream, "application/pdf");

                }
            }
        }
        #endregion
        #region rejoin
        [HttpPost]
        [MultipleButton(Name = "action", Argument = "rejoin")]
        public ActionResult rejoin(string EmployeeID, string MeetingID, int OrderID, string cmd)
        {
            using (RecruitEntities recruit = new RecruitEntities())
            {
                if (cmd == "New")
                {
                    var view = (from ad in recruit.AppointmentDetails
                                where ad.EmployeeID == EmployeeID && ad.MeetingID == MeetingID
                                select new stoppaymentview
                                {
                                    EmployeeID = ad.EmployeeID,
                                    EmployeeName = ad.EmployeeName,
                                    MeetingID = ad.MeetingID,
                                    OrderID = ad.OrderID,
                                    OrderType = "Rejoin",
                                    Command = "Insert"
                                }).OrderByDescending(h => h.OrderID).FirstOrDefault();
                    return View(view);
                }
                else if (cmd == "Update")
                {
                    var view = (from ad in recruit.AppointmentDetails
                                where ad.EmployeeID == EmployeeID && ad.OrderID == OrderID
                                select new stoppaymentview
                                {
                                    EmployeeID = ad.EmployeeID,
                                    EmployeeName = ad.EmployeeName,
                                    MeetingID = ad.MeetingID,
                                    OrderID = ad.OrderID,
                                    OrderType = ad.OrderType,
                                    Command = cmd,
                                    FromDate = ad.FromDate,
                                    Remarks = ad.Remarks
                                }).FirstOrDefault();
                    return View(view);
                }
            }
            return View();
        }

        [HttpPost]
        public ActionResult rejoin(stoppaymentview eov)
        {

            if (ModelState.IsValid)
            {

                using (RecruitEntities recruit = new RecruitEntities())
                {
                    if (eov.Command == "Insert")
                    {

                        var dbdata = (recruit.AppointmentDetails.OrderByDescending(o => o.OrderID).FirstOrDefault(ad => ad.EmployeeID == eov.EmployeeID && ad.MeetingID == eov.MeetingID));
                        if (dbdata != null)
                        {
                            using (TransactionScope transaction = new TransactionScope())
                            {
                                // var dbNewData = new AppointmentDetail();
                                recruit.Database.Connection.Open();
                                AppointmentDetail ad = new AppointmentDetail()
                                {
                                    EmployeeID = dbdata.EmployeeID,
                                    EmployeeName = dbdata.EmployeeName,
                                    MeetingID = dbdata.MeetingID,
                                    OrderType = eov.OrderType,
                                    OrderID = dbdata.OrderID + 1,
                                    ProjectNo = dbdata.ProjectNo,
                                    FromDate = eov.FromDate ?? DateTime.Now,
                                    ToDate = dbdata.ToDate,
                                    BasicSalary = dbdata.BasicSalary,
                                    GrossSalary = dbdata.GrossSalary,
                                    CostToProject = dbdata.CostToProject,
                                    CommitmentNo = dbdata.CommitmentNo,
                                    Remarks = eov.Remarks,
                                    CreatedOn = DateTime.Today,
                                    CreatedBy = Session["UserName"].ToString()
                                };


                                recruit.AppointmentDetails.Add(ad);
                                recruit.SaveChanges();
                                OrderRequestDetail ord = new OrderRequestDetail();
                                ord.EmployeeID = dbdata.EmployeeID;
                                ord.EmployeeName = dbdata.EmployeeName;
                                ord.OrderID = dbdata.OrderID + 1;
                                ord.OrderType = eov.OrderType;
                                ord.CreatedOn = DateTime.Today;
                                ord.CreatedBy = Session["UserName"].ToString();
                                recruit.OrderRequestDetails.Add(ord);
                                recruit.SaveChanges();
                                var sddata = (recruit.SalaryDetails.OrderByDescending(o => o.OrderID).FirstOrDefault(a => a.EmployeeID == eov.EmployeeID));
                                SalaryDetail sd = new SalaryDetail()
                                {
                                    EmployeeID = sddata.EmployeeID,
                                    OrderID = sddata.OrderID + 1,
                                    OrderType = eov.OrderType,
                                    BasicSalary = sddata.BasicSalary,
                                    HRA = sddata.HRA,
                                    Bonus = sddata.Bonus,
                                    SpecialAllowance = sddata.SpecialAllowance,
                                    GrossSalary = sddata.GrossSalary,
                                    EmployeePF = sddata.EmployeePF,
                                    EmployeeESIC = sddata.EmployeeESIC,
                                    ProfessionalTax = sddata.ProfessionalTax,
                                    NetSalary = sddata.NetSalary,
                                    EmployerPF = sddata.EmployerPF,
                                    EmployerESIC = sddata.EmployerESIC,
                                    Insurance = sddata.Insurance,
                                    TotalContribution = sddata.TotalContribution,
                                    GrossTotal = sddata.GrossTotal,
                                    AgencyFee = sddata.AgencyFee,
                                    ServiceTax = sddata.ServiceTax,
                                    TotalSalary = sddata.TotalSalary
                                };
                                recruit.SalaryDetails.Add(sd);
                                recruit.SaveChanges();
                                var l = recruit.AppointmentProjects.Where(m => m.EmployeeID.Equals(eov.EmployeeID)).FirstOrDefault();
                                l.Remarks = eov.OrderType;
                                recruit.SaveChanges();
                                var lq = recruit.AppointmentMasters.Where(m => m.EmployeeID.Equals(eov.EmployeeID)).FirstOrDefault();
                                lq.Remarks = eov.OrderType;
                                lq.UpdatedBy = Session["UserName"].ToString();
                                lq.UpdatedOn = DateTime.Now;
                                //var mv = new AppointmentMaster { EmployeeID = dbdata.EmployeeID, Remarks = eov.Remarks, UpdatedBy = Session["UserName"].ToString(), UpdatedOn = DateTime.Today };
                                //recruit.Configuration.ValidateOnSaveEnabled = false;
                                // recruit.AppointmentMasters.Attach(mv);
                                recruit.SaveChanges();
                                transaction.Complete();
                                ViewData["Result"] = "True";
                                ViewData["status"] = "Insert";
                                ViewData["EmployeeID"] = eov.EmployeeID;
                            }
                        }
                    }//end of insert
                    else if (eov.Command == "Update")
                    {

                        var dbdata = (recruit.AppointmentDetails.OrderByDescending(o => o.OrderID).FirstOrDefault(ad => ad.EmployeeID == eov.EmployeeID && ad.MeetingID == eov.MeetingID));
                        if (dbdata != null)
                        {
                            using (TransactionScope transaction = new TransactionScope())
                            {
                                recruit.Database.Connection.Open();
                                dbdata.Remarks = eov.Remarks;
                                dbdata.UpdatedOn = DateTime.Today;
                                dbdata.UpdatedBy = Session["UserName"].ToString();
                                recruit.SaveChanges();
                                transaction.Complete();
                                ViewData["Result"] = "True";
                                ViewData["status"] = "Insert";
                                ViewData["EmployeeID"] = eov.EmployeeID;
                            }
                        }
                    }
                    return View(eov);
                }
            }
            return View();
        }
        [HttpPost]
        [MultipleButton(Name = "action", Argument = "rptrejoin")]
        public ActionResult rejoinrpt(string EmployeeID, int OrderID)
        {
            using (RecruitEntities recruit = new RecruitEntities())
            {
                using (SqlConnection con1 = new SqlConnection())
                {
                    con1.ConnectionString = ConfigurationManager.ConnectionStrings["Recruit"].ConnectionString;
                    string sql = "select am.EmployeeID,am.EmployeeName,om.DOB,om.DesignationName,om.ProjectType,om.ProjectNo,om.ProjectTitle,om.DepartmentName,om.PIName,om.FromDate,om.ToDate,om.CostToProject,om.CommitmentNo from OutsourcingMeeting as om inner join AppointmentDetails as am on om.MeetingID=am.MeetingID where EmployeeID='" + EmployeeID + "' and OrderID='" + OrderID + "'";
                    //"select am.EmployeeID,am.EmployeeName,om.DOB,om.DesignationName,om.ProjectType,om.ProjectNo,om.ProjectTitle,om.DepartmentName,om.PIName,om.FromDate,om.ToDate,om.CostToProject,om.CommitmentNo from OutsourcingMeeting as om inner join AppointmentMaster as am on om.CandidateID=am.CandidateID where om.MeetingID = '" + MeetingID + "')";
                    SqlDataAdapter sda = new SqlDataAdapter(sql, con1);
                    OutsourcingMeetingDS ds = new OutsourcingMeetingDS();
                    sda.Fill(ds.Tables["officeorder"]);
                    Outsourcing.Reports.rejoin rpt = new Outsourcing.Reports.rejoin();
                    rpt.Load();
                    rpt.SetDataSource(ds);
                    rpt.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, true, EmployeeID + " Rejoin");
                    Stream fileStream = rpt.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    return File(fileStream, "application/pdf");

                }
            }
        }

        #endregion
        #region project relieve
        [HttpPost]
        [MultipleButton(Name = "action", Argument = "ProjectRelieveOrder")]
        public ActionResult ProjectRelieveOrder(string EmployeeID, string MeetingID, int OrderID, string cmd)
        {
            if (cmd == "New")
            {
                using (RecruitEntities recruit = new RecruitEntities())
                {
                    AppointmentProject ap = (from m in recruit.AppointmentProjects where m.EmployeeID == EmployeeID && m.ProjectRelieveDate == null orderby m.AppointmentDate descending select m).Take(1).Single();
                    RelieveView rv = ap;
                    rv.AppointmentFromDate = ap.AppointmentDate;
                    rv.AppointmentToDate = ap.ToDate;
                    rv.RelieveDate = null;
                    rv.Command = "Insert";
                    AppointmentDetail ad = (from m in recruit.AppointmentDetails where m.EmployeeID == EmployeeID && (m.OrderType == "Appointment" || m.OrderType == "Extension") orderby m.OrderID descending select m).Take(1).Single();
                    rv.ExtensionFromDate = ad.FromDate;
                    rv.ExtensionToDate = ad.ToDate;
                    return View(rv);
                }
            }
            else if (cmd == "Update")
            {
                using (RecruitEntities recruit = new RecruitEntities())
                {
                    AppointmentProject ap = (from m in recruit.AppointmentProjects where (m.EmployeeID == EmployeeID && m.MeetingID == MeetingID) select m).Single();
                    RelieveView rv = ap;
                    rv.AppointmentFromDate = ap.AppointmentDate;
                    rv.AppointmentToDate = ap.ToDate;
                    rv.Command = "Update";
                    AppointmentDetail ad = (from m in recruit.AppointmentDetails where m.EmployeeID == EmployeeID && (m.OrderType == "Appointment" || m.OrderType == "Extension") && m.MeetingID == MeetingID orderby m.OrderID descending select m).Take(1).Single();
                    rv.ExtensionFromDate = ad.FromDate;
                    rv.ExtensionToDate = ad.ToDate;
                    return View(rv);
                }
            }
            else
            {
                RelieveView rv = new RelieveView();
                return View(rv);
            }
        }
        [HttpPost]
        public ActionResult ProjectRelieveOrder(RelieveView rv)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (rv.Command == "Insert")
                    {
                        using (RecruitEntities recruit = new RecruitEntities())
                        {
                            var ap = new AppointmentProject { EmployeeID = rv.EmployeeID, MeetingID = rv.MeetingID, ProjectRelieveDate = rv.RelieveDate, RelievedBy = Session["UserName"].ToString(), RelievedOn = DateTime.Today, Remarks = rv.Remarks };
                            recruit.Configuration.ValidateOnSaveEnabled = false;
                            recruit.AppointmentProjects.Attach(ap);
                            recruit.Entry(ap).Property(em => em.ProjectRelieveDate).IsModified = true;
                            recruit.Entry(ap).Property(em => em.RelievedBy).IsModified = true;
                            recruit.Entry(ap).Property(em => em.RelievedOn).IsModified = true;
                            recruit.Entry(ap).Property(em => em.Remarks).IsModified = true;
                            var om = new OutsourcingMeeting {MeetingID=rv.MeetingID, StatusOfRequest = "ShortClosure" };
                            recruit.Configuration.ValidateOnSaveEnabled = false;
                            recruit.OutsourcingMeetings.Attach(om);
                            recruit.Entry(om).Property(em => em.StatusOfRequest).IsModified = true;
                            recruit.SaveChanges();
                        }
                        ViewData["Result"] = "True";
                        ViewData["status"] = "Insert";
                        return View(rv);
                    }
                    else if (rv.Command == "Update")
                    {
                        using (RecruitEntities recruit = new RecruitEntities())
                        {
                            var ap = new AppointmentProject { EmployeeID = rv.EmployeeID, MeetingID = rv.MeetingID, ProjectRelieveDate = rv.RelieveDate, RelievedBy = Session["UserName"].ToString(), RelievedOn = DateTime.Today, Remarks = rv.Remarks };
                            recruit.Configuration.ValidateOnSaveEnabled = false;
                            recruit.AppointmentProjects.Attach(ap);
                            recruit.Entry(ap).Property(em => em.ProjectRelieveDate).IsModified = true;
                            recruit.Entry(ap).Property(em => em.RelievedBy).IsModified = true;
                            recruit.Entry(ap).Property(em => em.RelievedOn).IsModified = true;
                            recruit.Entry(ap).Property(em => em.Remarks).IsModified = true;
                            recruit.SaveChanges();
                        }
                        ViewData["Result"] = "True";
                        ViewData["status"] = "Update";
                        return View(rv);

                        /*RelieveProject rp = new RelieveProject();
                        string results = rp.updateRelieveProject(rv.EmployeeID, rv.MeetingID, rv.OrderID, Convert.ToDateTime(rv.RelieveDate), DateTime.Today, Session["UserName"].ToString());
                        if (results == "true")
                        {
                            using (RecruitEntities recruit = new RecruitEntities())
                            {
                                AppointmentDetail ad = (from m in recruit.AppointmentDetails where (m.EmployeeID == rv.EmployeeID && m.MeetingID == rv.MeetingID && m.OrderID == rv.OrderID) select m).Single();
                                RelieveView re = ad;
                                re.AppointmentFromDate = rv.AppointmentFromDate;
                                re.AppointmentToDate = rv.AppointmentToDate;
                                ViewData["Result"] = "True";
                                ViewData["status"] = "Update";
                                return View(re);
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "Validation Error");
                            return View(rv);
                        }*/
                    }
                    else
                    {
                        return View(rv);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Validation Error");
                    return View(rv);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(rv);
            }
        }
        #endregion
        #region relieve
        [HttpPost]
        [MultipleButton(Name = "action", Argument = "RelieveOrder")]
        public ActionResult RelieveOrder(string EmployeeID, string MeetingID, int OrderID, string cmd)
        {
            if (cmd == "New")
            {
                using (RecruitEntities recruit = new RecruitEntities())
                {
                    AppointmentProject ap = (from m in recruit.AppointmentProjects where m.EmployeeID == EmployeeID && m.MeetingID == MeetingID orderby m.AppointmentDate descending select m).Take(1).Single();
                    RelieveView rv = ap;
                    AppointmentMaster am = (from m in recruit.AppointmentMasters where m.EmployeeID == EmployeeID select m).Single();
                    rv.AppointmentFromDate = am.AppointmentDate;
                    rv.AppointmentToDate = am.ToDate;
                    rv.RelieveDate = null;
                    rv.Command = "Insert";
                    AppointmentDetail ad = (from m in recruit.AppointmentDetails where m.EmployeeID == EmployeeID && (m.OrderType == "Appointment" || m.OrderType == "Extension") orderby m.OrderID descending select m).Take(1).Single();
                    rv.ExtensionFromDate = ad.FromDate;
                    rv.ExtensionToDate = ad.ToDate;
                    return View(rv);
                }
            }
            else if (cmd == "Update")
            {
                using (RecruitEntities recruit = new RecruitEntities())
                {
                    AppointmentProject ap = (from m in recruit.AppointmentProjects where (m.EmployeeID == EmployeeID && m.MeetingID == MeetingID) select m).Single();
                    RelieveView rv = ap;
                    AppointmentMaster am = (from m in recruit.AppointmentMasters where m.EmployeeID == EmployeeID select m).Single();
                    rv.AppointmentFromDate = am.AppointmentDate;
                    rv.AppointmentToDate = am.ToDate;
                    rv.Command = "Update";
                    AppointmentDetail ad = (from m in recruit.AppointmentDetails where m.EmployeeID == EmployeeID && (m.OrderType == "Appointment" || m.OrderType == "Extension") && m.MeetingID == MeetingID orderby m.OrderID descending select m).Take(1).Single();
                    rv.ExtensionFromDate = ad.FromDate;
                    rv.ExtensionToDate = ad.ToDate;
                    return View(rv);
                }
            }
            else
            {
                RelieveView rv = new RelieveView();
                return View(rv);
            }
        }

        [HttpPost]
        public ActionResult RelieveOrder(RelieveView rv)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (rv.Command == "Insert")
                    {
                        using (RecruitEntities recruit = new RecruitEntities())
                        {
                            using (TransactionScope transaction = new TransactionScope())
                            {
                                recruit.Database.Connection.Open();
                                var ap = new AppointmentProject { EmployeeID = rv.EmployeeID, MeetingID = rv.MeetingID, ProjectRelieveDate = rv.RelieveDate, RelievedBy = Session["UserName"].ToString(), RelievedOn = DateTime.Today, Remarks = rv.Remarks };
                                recruit.Configuration.ValidateOnSaveEnabled = false;
                                recruit.AppointmentProjects.Attach(ap);
                                recruit.Entry(ap).Property(em => em.ProjectRelieveDate).IsModified = true;
                                recruit.Entry(ap).Property(em => em.RelievedBy).IsModified = true;
                                recruit.Entry(ap).Property(em => em.RelievedOn).IsModified = true;
                                recruit.Entry(ap).Property(em => em.Remarks).IsModified = true;
                                OrderRequestDetail ord = new OrderRequestDetail();
                                ord.EmployeeID = rv.EmployeeID;
                                ord.EmployeeName = rv.EmployeeName;
                                ord.OrderID = 100;
                                ord.OrderType = "Relieve";
                                ord.CreatedOn = DateTime.Today;
                                ord.CreatedBy = Session["UserName"].ToString();
                                recruit.OrderRequestDetails.Add(ord);
                                var mv = new AppointmentMaster { EmployeeID = rv.EmployeeID, RelieveDate = rv.RelieveDate, UpdatedBy = Session["UserName"].ToString(), UpdatedOn = DateTime.Today };
                                recruit.Configuration.ValidateOnSaveEnabled = false;
                                recruit.AppointmentMasters.Attach(mv);
                                recruit.Entry(mv).Property(em => em.RelieveDate).IsModified = true;
                                recruit.Entry(mv).Property(em => em.UpdatedBy).IsModified = true;
                                recruit.Entry(mv).Property(em => em.UpdatedOn).IsModified = true;
                                var om = new OutsourcingMeeting {MeetingID=rv.MeetingID, StatusOfRequest = "Relieved" };
                                recruit.Configuration.ValidateOnSaveEnabled = false;
                                recruit.OutsourcingMeetings.Attach(om);
                                recruit.Entry(om).Property(em => em.StatusOfRequest).IsModified = true;
                                recruit.SaveChanges();
                                transaction.Complete();
                            }
                            ViewData["Result"] = "True";
                            ViewData["status"] = "Insert";
                            return View(rv);
                        }
                    }
                    else if (rv.Command == "Update")
                    {
                        using (RecruitEntities recruit = new RecruitEntities())
                        {
                            using (TransactionScope transaction = new TransactionScope())
                            {
                                recruit.Database.Connection.Open();
                                var ap = new AppointmentProject { EmployeeID = rv.EmployeeID, MeetingID = rv.MeetingID, ProjectRelieveDate = rv.RelieveDate, RelievedBy = Session["UserName"].ToString(), RelievedOn = DateTime.Today, Remarks = rv.Remarks };
                                recruit.Configuration.ValidateOnSaveEnabled = false;
                                recruit.AppointmentProjects.Attach(ap);
                                recruit.Entry(ap).Property(em => em.ProjectRelieveDate).IsModified = true;
                                recruit.Entry(ap).Property(em => em.RelievedBy).IsModified = true;
                                recruit.Entry(ap).Property(em => em.RelievedOn).IsModified = true;
                                recruit.Entry(ap).Property(em => em.Remarks).IsModified = true;
                                var mv = new AppointmentMaster { EmployeeID = rv.EmployeeID, RelieveDate = rv.RelieveDate, UpdatedBy = Session["UserName"].ToString(), UpdatedOn = DateTime.Today };
                                recruit.Configuration.ValidateOnSaveEnabled = false;
                                recruit.AppointmentMasters.Attach(mv);
                                recruit.Entry(mv).Property(em => em.RelieveDate).IsModified = true;
                                recruit.Entry(mv).Property(em => em.UpdatedBy).IsModified = true;
                                recruit.Entry(mv).Property(em => em.UpdatedOn).IsModified = true;
                                recruit.SaveChanges();
                                transaction.Complete();
                            }
                            ViewData["Result"] = "True";
                            ViewData["status"] = "Insert";
                            return View(rv);
                        }
                        /*RelieveProject rp = new RelieveProject();
                        string results = rp.updateRelieveOrder(rv.EmployeeID, rv.MeetingID, rv.OrderID, Convert.ToDateTime(rv.RelieveDate), DateTime.Today, Session["UserName"].ToString());
                        if (results == "true")
                        {
                            using (RecruitEntities recruit = new RecruitEntities())
                            {
                                AppointmentDetail ad = (from m in recruit.AppointmentDetails where (m.EmployeeID == rv.EmployeeID && m.MeetingID == rv.MeetingID && m.OrderID == rv.OrderID) select m).Single();
                                RelieveView re = ad;
                                re.AppointmentFromDate = rv.AppointmentFromDate;
                                re.AppointmentToDate = rv.AppointmentToDate;
                                ViewData["Result"] = "True";
                                ViewData["status"] = "Update";
                                return View(re);
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "Validation Error");
                            return View(rv);
                        }*/
                    }
                    else
                    {
                        return View(rv);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Validation Error");
                    return View(rv);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(rv);
            }
        }
        [HttpPost]
        [MultipleButton(Name = "action", Argument = "rptrelieve")]
        public ActionResult rptrelieve(string EmployeeID, int OrderID)
        {
            using (RecruitEntities recruit = new RecruitEntities())
            {
                using (SqlConnection con1 = new SqlConnection())
                {
                    con1.ConnectionString = ConfigurationManager.ConnectionStrings["Recruit"].ConnectionString;
                    string sql = "select ap.ProjectNo,ap.EmployeeID,ap.EmployeeName,ap.DesignationName,ap.ProjectType,am.ProjectTitle,am.DepartmentName,am.CommitmentNo,oe.Gender,RelieveDate  from OutsourcingEmployeeDetails as oe inner join OutsourcingMeeting as am on oe.CandidateID=am.CandidateID inner join appointmentProject as ap on ap.EmployeeName=am.CandidateName and ap.MeetingID=am.MeetingID inner join AppointmentMaster am1 on am1.EmployeeID=ap.EmployeeID where  ap.EmployeeID='" + EmployeeID + "'";
                    //"select am.EmployeeID,am.EmployeeName,om.DOB,om.DesignationName,om.ProjectType,om.ProjectNo,om.ProjectTitle,om.DepartmentName,om.PIName,om.FromDate,om.ToDate,om.CostToProject,om.CommitmentNo from OutsourcingMeeting as om inner join AppointmentMaster as am on om.CandidateID=am.CandidateID where om.MeetingID = '" + MeetingID + "')";
                    SqlDataAdapter sda = new SqlDataAdapter(sql, con1);
                    OutsourcingMeetingDS ds = new OutsourcingMeetingDS();
                    sda.Fill(ds.Tables["relieve"]);
                    Outsourcing.Reports.relieve rpt = new Outsourcing.Reports.relieve();
                    rpt.Load();
                    rpt.SetDataSource(ds);
                    rpt.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, true, EmployeeID + " Relieve Agency");
                    Stream fileStream = rpt.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    return File(fileStream, "application/pdf");

                }
            }
        }
        #endregion
        [HttpPost]
        public ActionResult SalaryCalculationUpdate(string id, string basicSalary, string increment, int orderID)  //
        {
            try
            {
                using (RecruitEntities recruit = new RecruitEntities())
                {
                    SalaryDetail sd = (from m in recruit.SalaryDetails where m.EmployeeID == id && m.OrderID == orderID select m).Single();
                    SalaryDetailsView sdv = sd;
                    var output = "fail";
                    SalaryDetailsView tmpSdv = new SalaryDetailsView();
                    output = "success";
                    SalaryCalculation sc = new SalaryCalculation();
                    decimal basic = Convert.ToDecimal(basicSalary) + Convert.ToDecimal(increment);
                    sc.TAMsalaryBasic(basic, sdv, out tmpSdv);
                    //SalaryDetailsView sv;
                    var ReturnVal = new
                    {
                        result = output,
                        BasicSalary = tmpSdv.BasicSalary,
                        Bonus = tmpSdv.Bonus,
                        HRA = tmpSdv.HRA,
                        SpecialAllowance = tmpSdv.SpecialAllowance,
                        GrossSalary = tmpSdv.GrossSalary,
                        EmployeePF = tmpSdv.EmployeePF,
                        EmployeeESIC = tmpSdv.EmployeeESIC,
                        ProfessionalTax = tmpSdv.ProfessionalTax,
                        TotalDeduction = tmpSdv.TotalDeduction,
                        NetSalary = tmpSdv.NetSalary,
                        EmployerPF = tmpSdv.EmployerPF,
                        EmployerESIC = tmpSdv.EmployerESIC,
                        Insurance = tmpSdv.Insurance,
                        TotalContribution = tmpSdv.TotalContribution,
                        GrossTotal = tmpSdv.GrossTotal,
                        AgencyFee = tmpSdv.AgencyFee,
                        GrossTotalwithAgencyFee = tmpSdv.GrossTotalwithAgencyFee,
                        ServiceTax = tmpSdv.ServiceTax,
                        TotalSalary = tmpSdv.TotalSalary
                    };
                    return Json(ReturnVal, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                var output = "fail";
                var ReturnVal = new
                {
                    result = output
                };
                return Json(ReturnVal, JsonRequestBehavior.AllowGet);
            }
            //var m = new ExtensionOrderView();
            //m.salaryDetailsView = eov.salaryDetailsView;
            //return Json(m.salaryDetailsView);
        }

        public ActionResult OrderProcessing()
        {
            return View();
        }
    }
}
