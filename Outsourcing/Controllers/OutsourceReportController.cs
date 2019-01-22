using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Outsourcing.BusinessModel;
using System.Data.Linq;
using Outsourcing.ViewModel;

using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using CrystalDecisions.CrystalReports.Engine;
using Outsourcing.Datasets;
using System.IO;
using System.Text;
using System.Transactions;
using System.Globalization;
using CrystalDecisions.Shared;
using DataLayer.Repository;

namespace Outsourcing.Controllers
{
    public class OutsourceReportController : Controller
    {
        //
        // GET: /OutsourceReport/
        [HttpGet]
        public ActionResult Reports()
        {
            return View();
        }

        [HttpGet]
        public ActionResult VariousReport()
        {
            VariousReportView vrv = new VariousReportView();
            vrv.AppointmentStatuskal = vrv.AppointmentStatusList();
            vrv.Departments = vrv.DepartmentList();
            vrv.ProjectTypes = vrv.ProjectTypeList();
            vrv.ProjectNumbers = vrv.ProjectNumberList();
            vrv.CoorNames = vrv.CoorNameList();
            vrv.Designations = vrv.DesignationList();
            vrv.DocumentFormats = vrv.DocumentFormatList();
            return View(vrv);
        }

        [HttpPost]
        public ActionResult VariousReport(FormCollection fc)
        {
            try
            {
                string appointmentStatus = Convert.ToString(fc["AppointmentStatus"]);
                string employeeNo = Convert.ToString(fc["EmployeeNo"]);
                string employeeName = Convert.ToString(fc["EmployeeName"]);

                string documentFormat = Convert.ToString(fc["DocumentFormat"]);
                string tmpQ = "";
                if (appointmentStatus == "Ongoing")
                {
                    tmpQ = "P.ProjectRelieveDate is null";
                }
                else if (appointmentStatus == "ProjectRelieved")
                {
                    tmpQ = "P.ProjectRelieveDate is not null";
                }
                else if (appointmentStatus == "AgencyRelieved")
                {
                    tmpQ = "M.RelieveDate is not null";
                }
                if (employeeNo != null && employeeNo != "") tmpQ = "M.EmployeeID like '" + employeeNo + "'";
                if (employeeName != null && employeeName != "") tmpQ = "M.CandidateID in (select CandidateID from AppointmentMaster where EmployeeName like '%" + employeeName + "%'";
                string tmpQ1 = "";
                if ((employeeNo == null || employeeNo == "") && (employeeName == null || employeeName == ""))
                {
                    string department = Convert.ToString(fc["Department"]);
                    if (department != null && department != "") tmpQ1 = tmpQ1 + " and P.DepartmentCode='" + department + "'";
                    string projectType = Convert.ToString(fc["ProjectType"]);
                    if (projectType != null && department != null && projectType != "" && department != "") tmpQ1 = tmpQ1 + " and P.DepartmentCode='" + department + "' and P.ProjectType = '" + projectType + "'";
                    else if (projectType != null && department == null) tmpQ1 = tmpQ1 + " and P.ProjectType = '" + projectType + "'";
                    string projectNumber = Convert.ToString(fc["ProjectNumber"]);
                    if (projectNumber != null && projectNumber != "") tmpQ1 = " and P.ProjectNo like '%" + projectNumber + "%'";
                    string coorName = Convert.ToString(fc["CoorName"]);
                    if (coorName != null && coorName != "") tmpQ1 = " and P.PICode like '%" + coorName + "%'";
                    string designation = Convert.ToString(fc["Designation"]);
                    if (designation != null && designation != "") tmpQ1 = " and P.DesignationCode like '%" + designation + "%'";
                    tmpQ = tmpQ + tmpQ1;
                }
                using (RecruitEntities recruit = new RecruitEntities())
                {
                    // var tt = (from M in recruit.AppointmentMasters join P in recruit.AppointmentProjects on M.EmployeeID equals P.EmployeeID where ( "P.ProjectRelieveDate == @0","12/12/2016")  select new { M.EmployeeID, M.EmployeeName, P.ProjectNo, P.DepartmentCode, P.DesignationCode, P.AppointmentDate, P.ToDate, P.ProjectRelieveDate }).ToList();
                    using (SqlConnection con = new SqlConnection())
                    {
                        con.ConnectionString = ConfigurationManager.ConnectionStrings["Recruit"].ConnectionString;
                        string sql = "select M.EmployeeID,M.EmployeeName,P.ProjectNo,P.DepartmentCode,P.DesignationCode,(SELECT TOP 1 BasicSalary FROM AppointmentDetails where OrderType in ('Appointment','Enhancement','Extension') and EmployeeID=M.EmployeeID and MeetingID=P.MeetingID ORDER BY OrderID DESC) as BasicSalary, (SELECT TOP 1 GrossSalary FROM AppointmentDetails where OrderType in ('Appointment','Enhancement','Extension') and EmployeeID=M.EmployeeID and MeetingID=P.MeetingID ORDER BY OrderID DESC) as GrossSalary,P.AppointmentDate,P.ToDate,P.ProjectRelieveDate as RelieveDate from AppointmentMaster M  inner join AppointmentProject P on M.EmployeeID=P.EmployeeID and " + tmpQ;
                        SqlDataAdapter sda = new SqlDataAdapter(sql, con);
                        ReportsDS ds = new ReportsDS();
                        sda.Fill(ds.Tables["DtVarious"]);
                        if (documentFormat == "PDF")
                        {
                            ReportClass rptVarious = new ReportClass();
                            rptVarious.FileName = Server.MapPath("~/Reports/VariousRpt.rpt");
                            rptVarious.Load();
                            rptVarious.SetDataSource(ds.Tables["DtVarious"]);
                            Stream fileStream = rptVarious.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                            return File(fileStream, "application/pdf");
                        }
                        else if (documentFormat == "XLS" || documentFormat == "XLSX")
                        {
                            GenerateExcel ge = new GenerateExcel();
                            ge.ExportToExcel(ds.Tables["DtVarious"], "VariousReport");
                            return null;
                            //  Stream fileStream = rptVarious.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
                            // return File(fileStream, "application/vnd.ms-excel","VariousReportExcel.xls");

                        }
                        /*else if (documentFormat == "XLSX")
                        {
                            Stream fileStream = rptVarious.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
                            return File(fileStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "VariousReportExcel.xlsx");
                        }
                        else if (documentFormat == "DOCX")
                        {
                            Stream fileStream = rptVarious.ExportToStream(CrystalDecisions.Shared.ExportFormatType.WordForWindows);
                            return File(fileStream, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "VariousReportWord.docx");
                        }*/
                        else
                        {
                            GenerateExcel ge = new GenerateExcel();
                            ge.ExportToWord(ds.Tables["DtVarious"], "VariousReport");
                            return null;
                            //Stream fileStream = rptVarious.ExportToStream(CrystalDecisions.Shared.ExportFormatType.WordForWindows);
                            //return File(fileStream, "application/msword", "VariousReportWord.doc");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                VariousReportView vrv = new VariousReportView();
                vrv.AppointmentStatuskal = vrv.AppointmentStatusList();
                vrv.Departments = vrv.DepartmentList();
                vrv.ProjectTypes = vrv.ProjectTypeList();
                vrv.ProjectNumbers = vrv.ProjectNumberList();
                vrv.CoorNames = vrv.CoorNameList();
                vrv.Designations = vrv.DesignationList();
                vrv.DocumentFormats = vrv.DocumentFormatList();
                return View(vrv);
            }
        }

        [HttpGet]
        public ActionResult MedicalCard()
        {
            using (RecruitEntities recruit = new RecruitEntities())
            {
                List<string> el = (from m in recruit.AppointmentMasters where m.RelieveDate == null select m.EmployeeID).ToList();
                ViewData["EmployeeList"] = el;
                ViewData["Authorities"] = Authorities.AuthorityList();
                return View();
            }
        }

        [HttpPost]
        public ActionResult MedicalCard(FormCollection fc)
        {
            string EmpID = Convert.ToString(fc["ddlEmpID"]);
            if (string.IsNullOrEmpty(EmpID))
            {
                throw new Exception("Select Employee ID");
            }
            string Authority = Convert.ToString(fc["ddlAuthority"]);
            if (string.IsNullOrEmpty(Authority))
            {
                throw new Exception("Select Signing Authority");
            }
            try
            {
                using (SqlConnection con = new SqlConnection())
                {
                    con.ConnectionString = ConfigurationManager.ConnectionStrings["Recruit"].ConnectionString;
                    string sql = "SELECT M.EmployeeID,M.EmployeeName,M.DesignationName,M.DOB,(select case when Gender='M' then 'Male' else 'Female' end from OutsourcingEmployeeDetails where CandidateID=M.CandidateID) as Gender, M.AppointmentDate,M.ToDate,M.CommunicationAddress, M.MobileNumber,(select Top 1 ProjectNo from AppointmentProject where EmployeeID=M.EmployeeID order by AppointmentDate desc) as ProjectNo,(select Top 1 PIName from AppointmentProject where EmployeeID=M.EmployeeID order by AppointmentDate desc) as CoorName   FROM AppointmentMaster M  where M.EmployeeID like '" + EmpID + "'";
                    SqlDataAdapter sda = new SqlDataAdapter(sql, con);
                    MedicalDS ds = new MedicalDS();
                    sda.Fill(ds.Tables["dtMedicalCard"]);
                    ReportClass rptMedicalCard = new ReportClass();
                    string authorityDesign = Convert.ToString(Authorities.AuthorityList().Where(em => em.AuthorityID == Authority).Select(em => em.AuthorityDesignation).Single());
                    rptMedicalCard.FileName = Server.MapPath("~/Reports/MedicalCard.rpt");
                    rptMedicalCard.Load();
                    ((CrystalDecisions.CrystalReports.Engine.TextObject)rptMedicalCard.ReportDefinition.Sections["Section3"].ReportObjects["txtDesignation"]).Text = authorityDesign;
                    rptMedicalCard.SetDataSource(ds.Tables["dtMedicalCard"]);
                    Stream fileStream = rptMedicalCard.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    return File(fileStream, "application/pdf");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                using (RecruitEntities recruit = new RecruitEntities())
                {
                    List<string> el = (from m in recruit.AppointmentMasters where m.RelieveDate == null select m.EmployeeID).ToList();
                    ViewData["EmployeeList"] = el;
                    ViewData["Authorities"] = Authorities.AuthorityList();
                    return View();
                }
            }
        }

        [HttpGet]
        public ActionResult MonthWiseReport()
        {
            MonthWiseReportView mrv = new MonthWiseReportView();
            mrv.OrderTypes = mrv.OrderTypeList();
            mrv.DocumentFormats = mrv.DocumentFormatList();
            mrv.ReportBased = mrv.ReportBasedList();
            return View(mrv);
        }

        public ActionResult MonthWiseReport(MonthWiseReportView mrv)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (RecruitEntities recruit = new RecruitEntities())
                    {
                        using (SqlConnection con = new SqlConnection())
                        {
                            con.ConnectionString = ConfigurationManager.ConnectionStrings["Recruit"].ConnectionString;
                            string tmpQry;
                            if (mrv.OrderType == "Appointment")
                            {
                                string tmpTitle;
                                if (mrv.ReportBasedOn == "EntryDate")
                                {
                                    tmpQry = "D.CreatedOn between convert(date,'" + mrv.FromDate + "',103) and convert(date,'" + mrv.ToDate + "',103)";
                                    tmpTitle = "Entry Date From " + Convert.ToDateTime(mrv.FromDate).ToShortDateString() + " To " + Convert.ToDateTime(mrv.ToDate).ToShortDateString();
                                }
                                else
                                {
                                    tmpQry = "D.FromDate between convert(date,'" + mrv.FromDate + "',103) and convert(date,'" + mrv.ToDate + "',103)";
                                    tmpTitle = "Appointment Date From " + Convert.ToDateTime(mrv.FromDate).ToShortDateString() + " To " + Convert.ToDateTime(mrv.ToDate).ToShortDateString();
                                }
                                string sql = "select ROW_NUMBER() over (order by D.EmployeeID), D.EmployeeID,D.EmployeeName,case when (p.PartTime=0) then 'Full' else 'Part' end as Part_Full,P.DesignationName,D.ProjectNo,D.CommitmentNo,P.DepartmentCode, D.FromDate AS AppointmentDate ,D.ToDate,D.BasicSalary,sa.HRA,sa.Bonus,sa.SpecialAllowance,D.GrossSalary,am.BankName,am.BankAccountNo,am.BranchName,am.IFSC_Code,case when (sa.EmployeePF>1) then 'Yes' else 'No' end as PF_Eligibility,case when (sa.EmployeeESIC>1) then 'Yes' else 'No' end as ESIC_Eligibilty, D.CreatedOn as EntryDate from AppointmentDetails D  inner join AppointmentProject P on D.EmployeeID=P.EmployeeID and D.MeetingID =P.MeetingID AND D.OrderType ='Appointment' inner join SalaryDetails sa on sa.EmployeeID=D.EmployeeID and sa.OrderID=D.OrderID inner join myview v on v.employeeid=sa.EmployeeID inner join AppointmentMaster am on am.EmployeeID=P.EmployeeID and " + tmpQry;
                                //string sql = "select D.CreatedOn as EntryDate, D.EmployeeID,D.EmployeeName,D.ProjectNo,P.DepartmentCode,P.DesignationCode,D.BasicSalary, D.GrossSalary, D.FromDate AS AppointmentDate ,D.ToDate from AppointmentDetails D  inner join AppointmentProject P on D.EmployeeID=P.EmployeeID and D.MeetingID =P.MeetingID AND D.OrderType ='Appointment' and " + tmpQry;
                                SqlDataAdapter sda = new SqlDataAdapter(sql, con);
                                ReportsDS ds = new ReportsDS();
                                sda.Fill(ds.Tables["DtAppointmentList"]);
                                if (mrv.DocumentFormat == "PDF")
                                {
                                    ReportClass rptClsAppointment = new ReportClass();
                                    rptClsAppointment.FileName = Server.MapPath("~/Reports/AppointmentList.rpt");
                                    rptClsAppointment.Load();
                                    rptClsAppointment.SetDataSource(ds.Tables["DtAppointmentList"]);
                                    Stream fileStream = rptClsAppointment.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                                    return File(fileStream, "application/pdf");
                                }
                                else if (mrv.DocumentFormat == "XLS" || mrv.DocumentFormat == "XLSX")
                                {
                                    GenerateExcel ge = new GenerateExcel();
                                    ge.ExportToExcel(ds.Tables["DtAppointmentList"], "AppointmentList");
                                    return null;
                                }
                                else
                                {
                                    GenerateExcel ge = new GenerateExcel();
                                    ge.ExportToWord(ds.Tables["DtAppointmentList"], "AppointmentList");
                                    return null;
                                }
                            }
                            else if (mrv.OrderType == "Enhancement")
                            {
                                string tmpTitle;
                                if (mrv.ReportBasedOn == "EntryDate")
                                {
                                    tmpQry = "D.CreatedOn between convert(date,'" + mrv.FromDate + "',103) and convert(date,'" + mrv.ToDate + "',103)";
                                    tmpTitle = "Entry Date From " + Convert.ToDateTime(mrv.FromDate).ToShortDateString() + " To " + Convert.ToDateTime(mrv.ToDate).ToShortDateString();
                                }
                                else
                                {
                                    tmpQry = "D.FromDate between convert(date,'" + mrv.FromDate + "',103) and convert(date,'" + mrv.ToDate + "',103)";
                                    tmpTitle = "Effective Date From " + Convert.ToDateTime(mrv.FromDate).ToShortDateString() + " To " + Convert.ToDateTime(mrv.ToDate).ToShortDateString();
                                }
                                string sql = "select ROW_NUMBER() over (order by D.EmployeeID), D.EmployeeID,D.EmployeeName,P.DesignationName,P.AppointmentDate,P.ToDate,D.FromDate as IncrementDt,( select top 1 BasicSalary from (select Top 2 BasicSalary,OrderID from AppointmentDetails where EmployeeID=d.EmployeeID and MeetingID=d.MeetingID order by OrderID desc) as Pre_BasicSalary order by OrderID),(D.GrossSalary-(select top 1 GrossSalary from AppointmentDetails where EmployeeID=d.EmployeeID and MeetingID=d.MeetingID and OrderID < D.OrderID order by OrderID desc)) as IncrementAmount,D.BasicSalary,sa.HRA,sa.Bonus,sa.SpecialAllowance,sa.GrossSalary,case when (sa.EmployeePF>1) then 'Yes' else 'No' end,case when (sa.EmployeeESIC>1) then 'Yes' else 'No' end,D.CreatedOn as EntryDate from AppointmentDetails D  inner join AppointmentProject P on D.EmployeeID=P.EmployeeID and D.MeetingID =P.MeetingID AND D.OrderType ='Enhancement' inner join SalaryDetails sa on sa.EmployeeID=D.EmployeeID and sa.OrderID=D.OrderID inner join myview v on sa.EmployeeID=v.employeeid and sa.OrderID=v.orderid and " + tmpQry;
                                //string sql = "select D.CreatedOn as EntryDate,D.EmployeeID,D.EmployeeName,D.ProjectNo,P.DepartmentCode,P.DesignationCode,(D.GrossSalary-(select top 1 GrossSalary from AppointmentDetails where EmployeeID=d.EmployeeID and MeetingID=d.MeetingID and OrderID < D.OrderID order by OrderID desc)) as IncrementAmount,D.GrossSalary,P.AppointmentDate AS AppointmentDate,D.FromDate as EffectFromDate,D.ToDate from AppointmentDetails D  inner join AppointmentProject P on D.EmployeeID=P.EmployeeID and D.MeetingID =P.MeetingID AND D.OrderType ='Enhancement' and " + tmpQry;
                                SqlDataAdapter sda = new SqlDataAdapter(sql, con);
                                ReportsDS ds = new ReportsDS();
                                sda.Fill(ds.Tables["DtEnhancementList"]);
                                if (mrv.DocumentFormat == "PDF")
                                {
                                    ReportClass rptClsEnhance = new ReportClass();
                                    rptClsEnhance.FileName = Server.MapPath("~/Reports/EnhancementList.rpt");
                                    rptClsEnhance.Load();
                                    rptClsEnhance.SetDataSource(ds.Tables["DtEnhancementList"]);
                                    ((CrystalDecisions.CrystalReports.Engine.TextObject)rptClsEnhance.ReportDefinition.Sections["Section1"].ReportObjects["txtTitle"]).Text = "Enhancement List based on " + tmpTitle;
                                    Stream fileStream = rptClsEnhance.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                                    return File(fileStream, "application/pdf");
                                }
                                else if (mrv.DocumentFormat == "XLS" || mrv.DocumentFormat == "XLSX")
                                {
                                    GenerateExcel ge = new GenerateExcel();
                                    ge.ExportToExcel(ds.Tables["DtEnhancementList"], "EnhancementList");
                                    return null;
                                }
                                else
                                {
                                    GenerateExcel ge = new GenerateExcel();
                                    ge.ExportToWord(ds.Tables["DtEnhancementList"], "EnhancementList");
                                    return null;
                                }
                            }
                            else if (mrv.OrderType == "Extension")
                            {
                                string tmpTitle;
                                if (mrv.ReportBasedOn == "EntryDate")
                                {
                                    tmpQry = "D.CreatedOn between convert(date,'" + mrv.FromDate + "',103) and convert(date,'" + mrv.ToDate + "',103)";
                                    tmpTitle = "Entry Date From " + Convert.ToDateTime(mrv.FromDate).ToShortDateString() + " To " + Convert.ToDateTime(mrv.ToDate).ToShortDateString();
                                }
                                else
                                {
                                    tmpQry = "D.FromDate between convert(date,'" + mrv.FromDate + "',103) and convert(date,'" + mrv.ToDate + "',103)";
                                    tmpTitle = "Extension Date From " + Convert.ToDateTime(mrv.FromDate).ToShortDateString() + " To " + Convert.ToDateTime(mrv.ToDate).ToShortDateString();
                                }
                                string sql = "select D.CreatedOn as EntryDate,D.EmployeeID,D.EmployeeName,D.ProjectNo,P.DepartmentCode,P.DesignationCode,(D.GrossSalary-(select top 1 GrossSalary from AppointmentDetails where EmployeeID=d.EmployeeID and MeetingID=d.MeetingID and OrderID < D.OrderID order by OrderID desc)) as IncrementAmount,D.GrossSalary,P.AppointmentDate AS AppointmentDate,D.FromDate as ExtensionFromDate,D.ToDate from AppointmentDetails D  inner join AppointmentProject P on D.EmployeeID=P.EmployeeID and D.MeetingID =P.MeetingID AND D.OrderType ='Extension' and " + tmpQry;
                                SqlDataAdapter sda = new SqlDataAdapter(sql, con);
                                ReportsDS ds = new ReportsDS();
                                sda.Fill(ds.Tables["DtExtensionList"]);
                                if (mrv.DocumentFormat == "PDF")
                                {
                                    ReportClass rptClsExtension = new ReportClass();
                                    rptClsExtension.FileName = Server.MapPath("~/Reports/ExtensionList.rpt");
                                    rptClsExtension.Load();
                                    rptClsExtension.SetDataSource(ds.Tables["DtExtensionList"]);
                                    ((CrystalDecisions.CrystalReports.Engine.TextObject)rptClsExtension.ReportDefinition.Sections["Section1"].ReportObjects["txtTitle"]).Text = "Extension List based on " + tmpTitle;
                                    Stream fileStream = rptClsExtension.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                                    return File(fileStream, "application/pdf");
                                }
                                else if (mrv.DocumentFormat == "XLS" || mrv.DocumentFormat == "XLSX")
                                {
                                    GenerateExcel ge = new GenerateExcel();
                                    ge.ExportToExcel(ds.Tables["DtExtensionList"], "ExtensionList");
                                    return null;
                                }
                                else
                                {
                                    GenerateExcel ge = new GenerateExcel();
                                    ge.ExportToWord(ds.Tables["DtExtensionList"], "ExtensionList");
                                    return null;
                                }
                            }
                            else if (mrv.OrderType == "ProjectRelieve")
                            {
                                string tmpTitle;
                                if (mrv.ReportBasedOn == "EntryDate")
                                {
                                    tmpQry = "P.RelievedOn between convert(date,'" + mrv.FromDate + "',103) and convert(date,'" + mrv.ToDate + "',103)";
                                    tmpTitle = "Entry Date From " + Convert.ToDateTime(mrv.FromDate).ToShortDateString() + " To " + Convert.ToDateTime(mrv.ToDate).ToShortDateString();
                                }
                                else
                                {
                                    tmpQry = "P.ProjectRelieveDate between convert(date,'" + mrv.FromDate + "',103) and convert(date,'" + mrv.ToDate + "',103)";
                                    tmpTitle = "Relieve Date From " + Convert.ToDateTime(mrv.FromDate).ToShortDateString() + " To " + Convert.ToDateTime(mrv.ToDate).ToShortDateString();
                                }
                                string sql = "select P.RelievedOn as EntryDate,P.EmployeeID,P.EmployeeName,P.ProjectNo,P.DepartmentCode,P.DesignationCode,(SELECT TOP 1 BasicSalary FROM AppointmentDetails where OrderType in ('Appointment','Enhancement','Extension') and EmployeeID = P.EmployeeID and MeetingID = P.MeetingID ORDER BY FromDate DESC) as BasicSalary, (SELECT TOP 1 GrossSalary FROM AppointmentDetails where OrderType in ('Appointment','Enhancement','Extension') and EmployeeID = P.EmployeeID and MeetingID = P.MeetingID ORDER BY FromDate DESC) as GrossSalary, P.AppointmentDate,P.ToDate,P.ProjectRelieveDate as RelieveDate from AppointmentProject P inner join AppointmentMaster M on P.EmployeeID=M.EmployeeID and M.RelieveDate is null and " + tmpQry;
                                SqlDataAdapter sda = new SqlDataAdapter(sql, con);
                                ReportsDS ds = new ReportsDS();
                                sda.Fill(ds.Tables["DtRelieveList"]);
                                if (mrv.DocumentFormat == "PDF")
                                {
                                    ReportClass rptClsRelieve = new ReportClass();
                                    rptClsRelieve.FileName = Server.MapPath("~/Reports/RelieveList.rpt");
                                    rptClsRelieve.Load();
                                    rptClsRelieve.SetDataSource(ds.Tables["DtRelieveList"]);
                                    ((CrystalDecisions.CrystalReports.Engine.TextObject)rptClsRelieve.ReportDefinition.Sections["Section1"].ReportObjects["txtTitle"]).Text = "Project Relieve List based on " + tmpTitle;
                                    Stream fileStream = rptClsRelieve.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                                    return File(fileStream, "application/pdf");
                                }
                                else if (mrv.DocumentFormat == "XLS" || mrv.DocumentFormat == "XLSX")
                                {
                                    GenerateExcel ge = new GenerateExcel();
                                    ge.ExportToExcel(ds.Tables["DtRelieveList"], "ProjectRelieveList");
                                    return null;
                                }
                                else
                                {
                                    GenerateExcel ge = new GenerateExcel();
                                    ge.ExportToWord(ds.Tables["DtRelieveList"], "ProjectRelieveList");
                                    return null;
                                }
                            }
                            else if (mrv.OrderType == "Relieve")
                            {
                                string tmpTitle;
                                if (mrv.ReportBasedOn == "EntryDate")
                                {
                                    tmpQry = "P.RelievedOn between convert(date,'" + mrv.FromDate + "',103) and convert(date,'" + mrv.ToDate + "',103)";
                                    tmpTitle = "Entry Date From " + Convert.ToDateTime(mrv.FromDate).ToShortDateString() + " To " + Convert.ToDateTime(mrv.ToDate).ToShortDateString();
                                }
                                else
                                {
                                    tmpQry = "M.RelieveDate between convert(date,'" + mrv.FromDate + "',103) and convert(date,'" + mrv.ToDate + "',103)";
                                    tmpTitle = "Relieve Date From " + Convert.ToDateTime(mrv.FromDate).ToShortDateString() + " To " + Convert.ToDateTime(mrv.ToDate).ToShortDateString();
                                }
                                string sql = "select P.RelievedOn as EntryDate,P.EmployeeID,P.EmployeeName,P.ProjectNo,P.DepartmentCode,P.DesignationCode,(SELECT TOP 1 BasicSalary FROM AppointmentDetails where OrderType in ('Appointment','Enhancement','Extension') and EmployeeID = P.EmployeeID and MeetingID = P.MeetingID ORDER BY FromDate DESC) as BasicSalary, (SELECT TOP 1 GrossSalary FROM AppointmentDetails where OrderType in ('Appointment','Enhancement','Extension') and EmployeeID = P.EmployeeID and MeetingID = P.MeetingID ORDER BY FromDate DESC) as GrossSalary,M.AppointmentDate,M.ToDate,M.RelieveDate as RelieveDate from AppointmentProject P inner join AppointmentMaster M on P.EmployeeID=M.EmployeeID and P.ProjectRelieveDate = M.RelieveDate and " + tmpQry;
                                SqlDataAdapter sda = new SqlDataAdapter(sql, con);
                                ReportsDS ds = new ReportsDS();
                                sda.Fill(ds.Tables["DtRelieveList"]);
                                if (mrv.DocumentFormat == "PDF")
                                {
                                    ReportClass rptClsRelieve = new ReportClass();
                                    rptClsRelieve.FileName = Server.MapPath("~/Reports/RelieveList.rpt");
                                    rptClsRelieve.Load();
                                    rptClsRelieve.SetDataSource(ds.Tables["DtRelieveList"]);
                                    ((CrystalDecisions.CrystalReports.Engine.TextObject)rptClsRelieve.ReportDefinition.Sections["Section1"].ReportObjects["txtTitle"]).Text = "Relieve List based on " + tmpTitle;
                                    Stream fileStream = rptClsRelieve.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                                    return File(fileStream, "application/pdf");
                                }
                                else if (mrv.DocumentFormat == "XLS" || mrv.DocumentFormat == "XLSX")
                                {
                                    GenerateExcel ge = new GenerateExcel();
                                    ge.ExportToExcel(ds.Tables["DtRelieveList"], "RelieveList");
                                    return null;
                                }
                                else
                                {
                                    GenerateExcel ge = new GenerateExcel();
                                    ge.ExportToWord(ds.Tables["DtRelieveList"], "RelieveList");
                                    return null;
                                }
                            }
                            //else if(mrv.OrderType== "StopPayment")
                            //{
                            //    string tmpTitle;
                            //    if (mrv.ReportBasedOn == "EntryDate")
                            //    {
                            //        tmpQry = "d.CreatedOn between convert(date,'" + mrv.FromDate + "',103) and convert(date,'" + mrv.ToDate + "',103)";
                            //        tmpTitle = "Entry Date From " + Convert.ToDateTime(mrv.FromDate).ToShortDateString() + " To " + Convert.ToDateTime(mrv.ToDate).ToShortDateString();
                            //    }
                            //    else
                            //    {
                            //        tmpQry = "D.FromDate between convert(date,'" + mrv.FromDate + "',103) and convert(date,'" + mrv.ToDate + "',103)";
                            //        tmpTitle = "StopPayment Date From " + Convert.ToDateTime(mrv.FromDate).ToShortDateString() + " To " + Convert.ToDateTime(mrv.ToDate).ToShortDateString();
                            //    }
                            //    string sql ="select D.CreatedOn as EntryDate,P.EmployeeID,P.EmployeeName,P.ProjectNo,P.DepartmentCode,P.DesignationCode,(SELECT TOP 1 BasicSalary FROM AppointmentDetails where OrderType in ('StopPayment') and EmployeeID = P.EmployeeID and MeetingID = P.MeetingID ORDER BY FromDate DESC) as BasicSalary, (SELECT TOP 1 GrossSalary FROM AppointmentDetails where OrderType in ('StopPayment') and EmployeeID = P.EmployeeID and MeetingID = P.MeetingID ORDER BY FromDate DESC) as GrossSalary,M.AppointmentDate,M.ToDate,M.RelieveDate as RelieveDate from AppointmentProject P inner join AppointmentMaster M on P.EmployeeID=M.EmployeeID and P.ProjectRelieveDate = M.RelieveDate and " + tmpQry;
                            //    SqlDataAdapter sda = new SqlDataAdapter(sql, con);
                            //    ReportsDS ds = new ReportsDS();
                            //    sda.Fill(ds.Tables["DtRelieveList"]);
                            //    if (mrv.DocumentFormat == "PDF")
                            //    {
                            //        ReportClass rptClsRelieve = new ReportClass();
                            //        rptClsRelieve.FileName = Server.MapPath("~/Reports/RelieveList.rpt");
                            //        rptClsRelieve.Load();
                            //        rptClsRelieve.SetDataSource(ds.Tables["DtRelieveList"]);
                            //        ((CrystalDecisions.CrystalReports.Engine.TextObject)rptClsRelieve.ReportDefinition.Sections["Section1"].ReportObjects["txtTitle"]).Text = "Relieve List based on " + tmpTitle;
                            //        Stream fileStream = rptClsRelieve.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                            //        return File(fileStream, "application/pdf");
                            //    }
                            //    else if (mrv.DocumentFormat == "XLS" || mrv.DocumentFormat == "XLSX")
                            //    {
                            //        GenerateExcel ge = new GenerateExcel();
                            //        ge.ExportToExcel(ds.Tables["DtRelieveList"], "RelieveList");
                            //        return null;
                            //    }
                            //    else
                            //    {
                            //        GenerateExcel ge = new GenerateExcel();
                            //        ge.ExportToWord(ds.Tables["DtRelieveList"], "RelieveList");
                            //        return null;
                            //    }
                            //}
                            else
                            {
                                return null;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    mrv.OrderTypes = mrv.OrderTypeList();
                    mrv.ReportBased = mrv.ReportBasedList();
                    mrv.DocumentFormats = mrv.DocumentFormatList();
                    return View(mrv);
                }
            }
            else
            {
                mrv.OrderTypes = mrv.OrderTypeList();
                mrv.DocumentFormats = mrv.DocumentFormatList();
                return View(mrv);
            }
        }
        [HttpGet]
        public ActionResult OrderRequest(string OrderType, string Command)
        {
            using (RecruitEntities recruit = new RecruitEntities())
            {
                List<OrderRequestView> orv;
                if (OrderType == "Appointment" || OrderType == "Extension" || OrderType == "Enhancement")
                {
                    orv = (from M in recruit.AppointmentMasters
                           join P in recruit.AppointmentProjects on M.EmployeeID equals P.EmployeeID
                           join D in recruit.AppointmentDetails on new { a = P.EmployeeID, b = P.MeetingID } equals new { a = D.EmployeeID, b = D.MeetingID }
                           join O in recruit.OrderRequestDetails on new { a = D.EmployeeID, b = D.OrderID } equals new { a = O.EmployeeID, b = O.OrderID }
                           where O.OrderType == OrderType && O.RequestID == null && O.OrderRequestDate == null
                           select new OrderRequestView
                           {
                               EmployeeID = M.EmployeeID,
                               CreatedOn = D.CreatedOn,
                               EmployeeName = P.EmployeeName,
                               DesignationName = P.DesignationName,
                               ProjectNo = P.ProjectNo,
                               PIName = P.PIName,
                               DepartmentCode = P.DepartmentCode,
                               FromDate = D.FromDate,
                               ToDate = D.ToDate,
                               BasicSalary = D.BasicSalary,
                               OrderID = O.OrderID
                           }).ToList();
                }
                else if (OrderType == "Relieve")
                {
                    orv = (from P in recruit.AppointmentProjects
                           join D in recruit.AppointmentDetails on new {a=P.EmployeeID,b=P.MeetingID} equals new {a=D.EmployeeID,b=D.MeetingID}
                           join V in recruit.myview on new { a = D.EmployeeID, b = D.OrderID } equals new { a = V.employeeid, b = (int)V.orderid }
                           join O in recruit.OrderRequestDetails on D.EmployeeID equals O.EmployeeID where O.OrderID == 100 && O.RequestID == null && O.OrderRequestDate == null
                           select new OrderRequestView
                           {
                               EmployeeID = D.EmployeeID,
                               CreatedOn = D.CreatedOn,
                               EmployeeName = D.EmployeeName,
                               DesignationName = P.DesignationName,
                               ProjectNo = P.ProjectNo,
                               PIName = P.PIName,
                               DepartmentCode = P.DepartmentCode,
                               FromDate = D.FromDate,
                               ToDate = D.ToDate,
                               BasicSalary = D.BasicSalary
                           }).ToList();
                
                    //orv = (from M in recruit.AppointmentMasters
                    //       join P in recruit.AppointmentProjects on M.EmployeeID equals P.EmployeeID
                    //       join D in recruit.AppointmentDetails on new { a = P.EmployeeID, b = P.MeetingID } equals new { a = D.EmployeeID, b = D.MeetingID }
                    //       join O in recruit.OrderRequestDetails on new { a = D.EmployeeID, b = D.OrderID } equals new { a = O.EmployeeID, b = O.OrderID }
                    //       where O.OrderType == "Relieve" && O.RequestID == null && O.OrderRequestDate == null
                    //       select new OrderRequestView
                    //       {
                    //           EmployeeID = M.EmployeeID,
                    //           CreatedOn = D.CreatedOn,
                    //           EmployeeName = P.EmployeeName,
                    //           DesignationName = P.DesignationName,
                    //           ProjectNo = P.ProjectNo,
                    //           PIName = P.PIName,
                    //           DepartmentCode = P.DepartmentCode,
                    //           FromDate = D.FromDate,
                    //           ToDate = D.ToDate,
                    //           BasicSalary = D.BasicSalary
                    //       }).ToList();
                }
                else
                {
                    orv = new List<ViewModel.OrderRequestView>();
                }
                List<SelectListItem> ot = recruit.ListItemMasters.Where(em => em.ListName == "OrderRequest").Select(em => new SelectListItem { Text = em.ListItemText, Value = em.ListItemValue }).ToList();
                ViewBag.OrderTypeList = ot;
                ViewBag.OrderType = OrderType;
                return View(orv);
            }
        }

        public ActionResult OrderRequest(IEnumerable<OrderRequestView> orv, string OrderType)
        {
            try
            {
                if (orv.Where(em => em.isOrdered == true).Count() == 0)
                {
                    throw new Exception(" You did not select any one");
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    int maxValue;
                    using (RecruitEntities recruit = new RecruitEntities())
                    {
                        if (recruit.OrderRequestDetails.Count() == 0) maxValue = 0;
                        else
                            maxValue = Convert.ToInt32(recruit.OrderRequestDetails.Max(em => em.RequestID));
                        maxValue = maxValue + 1;
                        using (TransactionScope transaction = new TransactionScope())
                        {
                            recruit.Database.Connection.Open();
                            foreach (OrderRequestView mid in orv)
                            {
                                if (mid.isOrdered == true)
                                {
                                    recruit.Configuration.ValidateOnSaveEnabled = false;
                                    OrderRequestDetail or = new OrderRequestDetail
                                    {
                                        EmployeeID = mid.EmployeeID,
                                        OrderID = mid.OrderID,
                                        RequestID = Convert.ToInt32(maxValue),
                                        OrderRequestDate = DateTime.Today,
                                        UpdatedBy = Session["UserName"].ToString(),
                                        UpdatedOn = DateTime.Today
                                    };
                                    recruit.OrderRequestDetails.Attach(or);
                                    //recruit.Entry(or).State = EntityState.Modified;
                                    recruit.Entry(or).Property(em => em.RequestID).IsModified = true;
                                    recruit.Entry(or).Property(em => em.OrderRequestDate).IsModified = true;
                                    recruit.Entry(or).Property(em => em.UpdatedBy).IsModified = true;
                                    recruit.Entry(or).Property(em => em.UpdatedOn).IsModified = true;
                                    recruit.SaveChanges();
                                }
                            }
                            transaction.Complete();
                        }
                    }
                    using (SqlConnection con = new SqlConnection())
                    {
                        if (OrderType == "Appointment")
                        {
                            con.ConnectionString = ConfigurationManager.ConnectionStrings["Recruit"].ConnectionString;
                            string sql = "select ROW_NUMBER() over (order by P.EmployeeID) as SerialNumber, P.EmployeeID,P.EmployeeName,P.DesignationName,S.BasicSalary,S.HRA,S.Bonus,S.SpecialAllowance,S.GrossSalary,D.FromDate,D.ToDate,M.BankAccountNo,S.EmployeePF,S.EmployeeESIC,p.DepartmentCode,D.CommitmentNo from AppointmentMaster M  inner join AppointmentProject P on M.EmployeeID= P.EmployeeID " +
                            "inner join AppointmentDetails D on P.EmployeeID = D.EmployeeID and P.MeetingID = D.MeetingID inner join SalaryDetails S on D.EmployeeID = S.EmployeeID and D.OrderID = S.OrderID  inner join OrderRequestDetails O on O.EmployeeID = D.EmployeeID and O.OrderID = D.OrderID and O.OrderType = 'Appointment' and O.RequestID =" + maxValue;
                            SqlDataAdapter sda = new SqlDataAdapter(sql, con);
                            ReportsDS ds = new ReportsDS();
                            sda.Fill(ds.Tables["AppointmentOrderRequest"]);
                            ReportClass rptOrderReq = new ReportClass();
                            rptOrderReq.FileName = Server.MapPath("~/Reports/AppointmentRequest.rpt");
                            rptOrderReq.Load();
                            rptOrderReq.SetDataSource(ds.Tables["AppointmentOrderRequest"]);
                            rptOrderReq.SetParameterValue("type", OrderType);
                            Stream fileStream = rptOrderReq.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                            return File(fileStream, "application/pdf");
                        }
                        else if (OrderType == "Extension")
                        {
                            con.ConnectionString = ConfigurationManager.ConnectionStrings["Recruit"].ConnectionString;
                            string sql = "select ROW_NUMBER() over (order by P.EmployeeID) as SerialNumber, P.EmployeeID,P.EmployeeName,P.DesignationName,S.BasicSalary,S.HRA,S.Bonus,S.SpecialAllowance,S.GrossSalary,D.FromDate,D.ToDate,M.BankAccountNo,S.EmployeePF,S.EmployeeESIC,p.DepartmentCode,D.CommitmentNo from AppointmentMaster M  inner join AppointmentProject P on M.EmployeeID= P.EmployeeID " +
                            "inner join AppointmentDetails D on P.EmployeeID = D.EmployeeID and P.MeetingID = D.MeetingID inner join SalaryDetails S on D.EmployeeID = S.EmployeeID and D.OrderID = S.OrderID  inner join OrderRequestDetails O on O.EmployeeID = D.EmployeeID and O.OrderID = D.OrderID and O.OrderType = 'Extension' and O.RequestID =" + maxValue;
                            SqlDataAdapter sda = new SqlDataAdapter(sql, con);
                            ReportsDS ds = new ReportsDS();
                            sda.Fill(ds.Tables["AppointmentOrderRequest"]);
                            ReportClass rptOrderReq = new ReportClass();
                            rptOrderReq.FileName = Server.MapPath("~/Reports/AppointmentRequest.rpt");
                            rptOrderReq.Load();
                            rptOrderReq.SetDataSource(ds.Tables["AppointmentOrderRequest"]);
                            rptOrderReq.SetParameterValue("type", OrderType);
                            Stream fileStream = rptOrderReq.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                            return File(fileStream, "application/pdf");
                        }
                        else if (OrderType == "Enhancement")
                        {
                            con.ConnectionString = ConfigurationManager.ConnectionStrings["Recruit"].ConnectionString;
                            string sql = "select ROW_NUMBER() over (order by P.EmployeeID) as SerialNumber, P.EmployeeID,P.EmployeeName,P.DesignationName,S.BasicSalary,S.HRA,S.Bonus,S.SpecialAllowance,S.GrossSalary,D.FromDate,D.ToDate,M.BankAccountNo,S.EmployeePF,S.EmployeeESIC,p.DepartmentCode,D.CommitmentNo from AppointmentMaster M  inner join AppointmentProject P on M.EmployeeID= P.EmployeeID " +
                            "inner join AppointmentDetails D on P.EmployeeID = D.EmployeeID and P.MeetingID = D.MeetingID inner join SalaryDetails S on D.EmployeeID = S.EmployeeID and D.OrderID = S.OrderID  inner join OrderRequestDetails O on O.EmployeeID = D.EmployeeID and O.OrderID = D.OrderID and O.OrderType = 'Extension' and O.RequestID =" + maxValue;
                            SqlDataAdapter sda = new SqlDataAdapter(sql, con);
                            ReportsDS ds = new ReportsDS();
                            sda.Fill(ds.Tables["AppointmentOrderRequest"]);
                            ReportClass rptOrderReq = new ReportClass();
                            rptOrderReq.FileName = Server.MapPath("~/Reports/AppointmentRequest.rpt");
                            rptOrderReq.Load();
                            rptOrderReq.SetDataSource(ds.Tables["AppointmentOrderRequest"]);
                            rptOrderReq.SetParameterValue("type", OrderType);
                            Stream fileStream = rptOrderReq.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                            return File(fileStream, "application/pdf");
                        }
                        else
                        {
                            con.ConnectionString = ConfigurationManager.ConnectionStrings["Recruit"].ConnectionString;
                            string sql = "select ROW_NUMBER() over (order by P.EmployeeID) as SerialNumber, P.EmployeeID,P.EmployeeName,P.DesignationName,S.BasicSalary,S.HRA,S.Bonus,S.SpecialAllowance,S.GrossSalary,D.FromDate,D.ToDate,M.BankAccountNo,S.EmployeePF,S.EmployeeESIC,p.DepartmentCode,D.CommitmentNo from AppointmentMaster M  inner join AppointmentProject P on M.EmployeeID= P.EmployeeID " +
                            "inner join AppointmentDetails D on P.EmployeeID = D.EmployeeID and P.MeetingID = D.MeetingID inner join SalaryDetails S on D.EmployeeID = S.EmployeeID and D.OrderID = S.OrderID  inner join OrderRequestDetails O on O.EmployeeID = D.EmployeeID and O.OrderID = D.OrderID and O.OrderType = 'Extension' and O.RequestID =" + maxValue;
                            SqlDataAdapter sda = new SqlDataAdapter(sql, con);
                            ReportsDS ds = new ReportsDS();
                            sda.Fill(ds.Tables["AppointmentOrderRequest"]);
                            ReportClass rptOrderReq = new ReportClass();
                            rptOrderReq.FileName = Server.MapPath("~/Reports/AppointmentRequest.rpt");
                            rptOrderReq.Load();
                            rptOrderReq.SetDataSource(ds.Tables["AppointmentOrderRequest"]);
                            rptOrderReq.SetParameterValue("type", OrderType);
                            Stream fileStream = rptOrderReq.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                            return File(fileStream, "application/pdf");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                using (RecruitEntities recruit = new RecruitEntities())
                {
                    List<SelectListItem> ot = recruit.ListItemMasters.Where(em => em.ListName == "OrderRequest").Select(em => new SelectListItem { Text = em.ListItemText, Value = em.ListItemValue }).ToList();
                    ViewBag.OrderTypeList = ot;
                }
                return View();
            }
        }
        public ActionResult OrderReport(string OrderType, int RequestID)
        {
            using (RecruitEntities recruit = new RecruitEntities())
            {
                List<OrderRequestView> orv = new List<OrderRequestView>();
                if (OrderType == "Appointment" || OrderType == "Extension" || OrderType == "Enhancement")
                {
                    orv = (from M in recruit.AppointmentMasters
                           join P in recruit.AppointmentProjects on M.EmployeeID equals P.EmployeeID
                           join D in recruit.AppointmentDetails on new { a = P.EmployeeID, b = P.MeetingID } equals new { a = D.EmployeeID, b = D.MeetingID }
                           join O in recruit.OrderRequestDetails on new { a = D.EmployeeID, b = D.OrderID } equals new { a = O.EmployeeID, b = O.OrderID }
                           where O.OrderType == OrderType && O.RequestID == RequestID
                           select new OrderRequestView
                           {
                               EmployeeID = M.EmployeeID,
                               CreatedOn = D.CreatedOn,
                               EmployeeName = P.EmployeeName,
                               DesignationName = P.DesignationName,
                               ProjectNo = P.ProjectNo,
                               PIName = P.PIName,
                               DepartmentCode = P.DepartmentCode,
                               FromDate = D.FromDate,
                               ToDate = D.ToDate,
                               BasicSalary = D.BasicSalary,
                               OrderID = O.OrderID
                           }).ToList();
                }

                List<SelectListItem> ot = recruit.ListItemMasters.Where(em => em.ListName == "OrderRequest").Select(em => new SelectListItem { Text = em.ListItemText, Value = em.ListItemValue }).ToList();
                ViewBag.OrderTypeList = ot;
                ViewBag.OrderType = OrderType;
                return View(orv);
            }
        }
        public PartialViewResult _OrderRequestList()
        {

            return PartialView();
        }
        [HttpGet]
        public ActionResult Reminder()
        {
            ReminderView mrv = new ReminderView();
            ////mrv.OrderTypes = mrv.OrderTypeList();
            mrv.DocumentFormats = mrv.DocumentFormatList();

            ////mrv.ReportBased = mrv.ReportBasedList();
            return View(mrv);
        }
        [HttpPost]
        public ActionResult Reminder(ReminderView mrv)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (RecruitEntities recruit = new RecruitEntities())
                    {
                        using (SqlConnection con = new SqlConnection())
                        {
                            con.ConnectionString = ConfigurationManager.ConnectionStrings["Recruit"].ConnectionString;
                            SqlDataAdapter sda = new SqlDataAdapter("select P.EmployeeID,P.EmployeeName,P.ProjectNo,P.DepartmentCode,P.DesignationCode,m.BasicSalary, m.AppointmentDate,m.ToDate from AppointmentProject P inner join AppointmentMaster m on m.EmployeeID=p.EmployeeID and m.MeetingID=p.MeetingID and m.RelieveDate is null and m.ToDate between convert(date, '" + mrv.FromDate + "', 103) and convert(date, '" + mrv.ToDate + "', 103) ", con);
                            ReportsDS ds = new ReportsDS();
                            sda.Fill(ds.Tables["Reminder"]);
                            //Outsourcing.Reports.ToDateReminder rpt = new Outsourcing.Reports.ToDateReminder();
                            ReportClass rpt = new ReportClass();
                            rpt.FileName = Server.MapPath("~/Reports/ToDateReminder.rpt");
                            rpt.Load();
                            rpt.SetDataSource(ds.Tables["Reminder"]);
                            if (mrv.DocumentFormat == "PDF")
                            {
                                Stream fileStream = rpt.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                                return File(fileStream, "application/pdf");
                            }
                            else if (mrv.DocumentFormat == "XLS" || mrv.DocumentFormat == "XLSX")
                            {
                                //Stream fileStream = rpt.ExportToStream(CrystalDecisions.Shared.ExportFormatType.ExcelRecord);
                                //return File(fileStream, "application/xls");
                                GenerateExcel ge = new GenerateExcel();
                                ge.ExportToExcel(ds.Tables["Reminder"], "Reminder");
                                return null;
                            }
                            else
                            {
                                Stream fileStream = rpt.ExportToStream(CrystalDecisions.Shared.ExportFormatType.WordForWindows);
                                return File(fileStream, "application/doc");
                            }


                        }
                    }
                }
                catch (Exception ex)
                {
                    mrv.DocumentFormats = mrv.DocumentFormatList();
                    ModelState.AddModelError("", ex.Message);
                    return View(mrv);
                }
            }
            else
                return null;
        }



        [HttpGet]
        public ActionResult MasterReport()
        {
            using (RecruitEntities recruit = new RecruitEntities())
            {
                ViewModel.MasterReport mp = new ViewModel.MasterReport();
                mp.am = (from t in typeof(AppointmentMasterView).GetProperties() select new SelectListItem { Text = t.Name, Value = t.Name }).ToList();
                mp.ad = (from t in typeof(AppointmentDetailsView).GetProperties() select new SelectListItem { Text = t.Name, Value = t.Name }).ToList();
                mp.ap = (from t in typeof(AppointmentProjectView).GetProperties() select new SelectListItem { Text = t.Name, Value = t.Name }).ToList();
                mp.oe = (from t in typeof(OutsourcingEmployeeDetailsView).GetProperties() select new SelectListItem { Text = t.Name, Value = t.Name }).ToList();
                mp.om = (from t in typeof(OutsourcingMeetingView).GetProperties() select new SelectListItem { Text = t.Name, Value = t.Name }).ToList();
                mp.Sal = (from t in typeof(SalaryDetailsView).GetProperties() select new SelectListItem { Text = t.Name, Value = t.Name }).ToList();
                mp.DBList = new List<SelectListItem>() {
                new SelectListItem { Text = "AppointmentMaster", Value = "AppointmentMaster"},
                new SelectListItem { Text = "AppointmentProject", Value = "AppointmentProject"},
                new SelectListItem { Text = "SalaryDetails", Value = "SalaryDetails"},
                new SelectListItem { Text = "AppointmentDetails", Value = "AppointmentDetails"},
                new SelectListItem {Text= "OutsourcingMeeting",Value="OutsourcingMeeting" },
                new SelectListItem { Text = "OutsourcingEmployeeDetails", Value = "OutsourcingEmployeeDetails"}
            };

                return View(mp);
            }
        }
        [HttpPost]
        public ActionResult MasterReport(MasterReport mas)
        {
            using (RecruitEntities recruit = new RecruitEntities())
            {

                var SelectedDblist = mas.DBList1[0];
                using (SqlConnection con = new SqlConnection())
                {
                    con.ConnectionString = ConfigurationManager.ConnectionStrings["Recruit"].ConnectionString;
                    string sql = "select * from " + SelectedDblist + "";
                    SqlDataAdapter sda = new SqlDataAdapter(sql, con);
                    DataTable dt = new DataTable("Master");
                    sda.Fill(dt);

                    //    for (int i = 1; i < mas.DBList1.Count; i++)
                    //    {
                    //        SelectedDblist += " inner join " + mas.DBList1[i] + " on " + mas.DBList1[i] + ".EmployeeID= " + mas.DBList1[i-1] + ".EmployeeID or " + mas.DBList1[i] + ".MeetingID= " + mas.DBList1[i-1] + ".MeetingID";
                    //      }

                    //    if (mas.DBList1.Count>0)
                    //    {
                    //        var SelectedAppProject = (mas.ap1 != null) ? String.Join(",", mas.ap1.Select(r => string.Concat("AppointmentProject.", r)).ToList()) : null;
                    //        var SelectedAppMaster = (mas.am1 != null) ? String.Join(",", mas.am1.Select(r => string.Concat("AppointmentMaster.", r)).ToList()) : null;
                    //        var SelectedAppDetails = (mas.ad1 != null) ? String.Join(",", mas.ad1.Select(r => string.Concat("AppointmentDetails.", r)).ToList()) : null;
                    //        var SelectedOutMeeting = (mas.om1 != null) ? String.Join(",", mas.om1.Select(r => string.Concat("OutsourcingMeeting", r)).ToList()) : null;
                    //        var SelectedOutEmployee = (mas.oe1 != null) ? String.Join(",", mas.oe1.Select(r => string.Concat("OutsourcingEmployeeDetails", r)).ToList()) : null;
                    //        var SelectedSalaryDetails = (mas.sal1 != null) ? String.Join(",", mas.sal1.Select(r => string.Concat("SalaryDetails", r)).ToList()) : null;
                    //        var SelectedValues = "";
                    //        if (SelectedAppProject != null)
                    //            SelectedValues = String.Join(",",SelectedAppProject, SelectedValues);
                    //        if (SelectedAppMaster != null)
                    //            SelectedValues = String.Join(",",SelectedAppMaster, SelectedValues);
                    //        if (SelectedAppDetails != null)
                    //            SelectedValues = String.Join(",", SelectedAppDetails, SelectedValues);
                    //        if (SelectedOutMeeting != null)
                    //            SelectedValues = String.Join(",", SelectedOutMeeting, SelectedValues);
                    //        if (SelectedOutEmployee != null)
                    //            SelectedValues = String.Join(",", SelectedOutEmployee, SelectedValues);
                    //        if (SelectedSalaryDetails != null)
                    //            SelectedValues = String.Join(",", SelectedSalaryDetails, SelectedValues);
                    //        //, SelectedOutEmployee, SelectedOutMeeting,SelectedSalaryDetails);
                    //        SelectedValues = SelectedValues.TrimEnd(',');
                    //        //For columns
                    //        var ListValue = SelectedValues.Split(',').ToList();
                    //        using (SqlConnection con = new SqlConnection())
                    //        {
                    //            con.ConnectionString = ConfigurationManager.ConnectionStrings["Recruit"].ConnectionString;
                    //            string sql = "select " + SelectedValues + " from " + SelectedDblist + "";
                    //            SqlDataAdapter sda = new SqlDataAdapter(sql, con);
                    //            DataTable dt = new DataTable("Master");
                    //            sda.Fill(dt);
                    //            ReportClass rpt = new ReportClass();

                    //            //for (int i = 0; i < ListValue.Count(); i++)
                    //            //{
                    //            //    DataColumn dc = new DataColumn(ListValue[i]);
                    //            //    dt.Columns.Add(dc);
                    //            //}

                    //            //for (int j = 0; j < dt.Columns.Count; j++)
                    //            //{
                    //            //for (int i = 0; i < dt.Columns.Count; i++)
                    //            //{
                    //            //    DataRow dr = dt.NewRow();
                    //            //    dr[i] = dt.Rows[i].ToString();
                    //            //    dt.Rows.Add(dr);
                    //            //}
                    //            //return Json(dt);

                    //            //}
                    //            //ReportsDS ds = new ReportsDS();
                    //            // sda.Fill(dt);
                    //            // ViewBag.ds = dt;
                    //            // ReportClass rptOrderReq = new ReportClass();
                    //            // rptOrderReq.FileName = Server.MapPath("~/Reports/AppointmentRequest.rpt");
                    //            // rptOrderReq.Load();
                    //            // rptOrderReq.SetDataSource(dt);
                    //            //// rptOrderReq.SetParameterValue("type", OrderType);
                    //            // Stream fileStream = rptOrderReq.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    //            // return File(fileStream, "application/pdf");
                    //        }

                    //    }
                    //   else
                    //    {
                    //        return View();
                    //    }

                }
                return View();
            }

        }
        //public PartialViewResult _MasterReport(MasterReport mas)
        //{
        //    using (RecruitEntities recruit = new RecruitEntities())
        //    {
        //        using (SqlConnection con = new SqlConnection())
        //        {

        //            con.ConnectionString = ConfigurationManager.ConnectionStrings["Recruit"].ConnectionString;
        //            //string sql = "select '"+mas.ap1+"' from '"+mas.+"'";
        //            //SqlDataAdapter sda = new SqlDataAdapter(sql, con);

        //            return PartialView();
        //        }
        //    }
        //}

        public ActionResult MasterTables()
        {
            return View();
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MasterTables(string Command)
        {
            using (RecruitEntities recruit = new RecruitEntities())
            {
                using (SqlConnection con1 = new SqlConnection())
                {
                    con1.ConnectionString = ConfigurationManager.ConnectionStrings["Recruit"].ConnectionString;
                    string sql="";
                    if (Command == "Format 1")
                    {
                       sql = "select  am.EmployeeID,am.EmployeeName,am.CandidateID,am.dob,am.DesignationCode,am.DesignationName,om.Qualification,om.DepartmentCode,am.AppointmentDate,am.ToDate,am.RelieveDate,am.PermanentAddress,am.CommunicationAddress,am.MobileNumber,am.EmailID,om.GrossSalary,am.BankName,am.BranchName,am.BankAccountNo,am.IFSC_Code,am.OutSourcingCompany,oe.Gender,oe.CasteCategory,oe.MaritalStatus,oe.ph,oe.FatherName,oe.MotherName,oe.HusbandName,oe.PAN,oe.Aadhar,oe.PhoneNumber,oe.EmergencyContactNo,ap.ProjectRelieveDate,am.Remarks from AppointmentMaster am inner join OutsourcingEmployeeDetails oe on am.CandidateID = oe.CandidateID inner join OutsourcingMeeting om on om.MeetingID = am.MeetingID inner join AppointmentProject ap on ap.EmployeeID=am.EmployeeID where  am.RelieveDate is null and ap.ProjectRelieveDate is null";
                    }
                    if(Command=="Format 2")
                    {
                       sql = "select  am.EmployeeID,am.EmployeeName,am.CandidateID,sa.GrossSalary,sa.BasicSalary,SA.HRA,sa.Bonus,sa.SpecialAllowance,sa.EmployeePF,sa.EmployeeESIC,sa.ProfessionalTax,sa.EmployerPF,sa.EmployerESIC,sa.Insurance,sa.NetSalary,sa.AgencyFee,sa.ServiceTax,sa.TotalSalary,om.ProjectNo,om.DesignationName,am.AppointmentDate,am.ToDate,am.BankAccountNo,am.BankName,am.BranchName,am.IFSC_Code,case when (om.PartTime=0) then 'Full' else 'Part' end,case when (om.ProjectNo='IC0910ICS039ICOHDEAN') then 'ICSR' else 'NON-ICSR' end,am.Remarks from AppointmentMaster am inner join SalaryDetails sa on am.EmployeeID=sa.EmployeeID inner join myview v on v.employeeid=sa.EmployeeID and v.orderid =sa.OrderID inner join OutsourcingMeeting om on om.MeetingID=am.MeetingID where am.RelieveDate is null";
                    }
                    SqlDataAdapter sda = new SqlDataAdapter(sql, con1);
                    ReportsDS ds = new ReportsDS();
                    if (Command == "Format 1")
                    {
                        sda.Fill(ds.Tables["Master"]);
                    }
                    else if(Command == "Format 2")
                    { sda.Fill(ds.Tables["Master1"]); }
                       
                    GenerateExcel ge = new GenerateExcel();
                    if (Command == "Format 1")
                    {
                        ge.ExportToExcel(ds.Tables["Master"], "Master");
                    }
                    else if (Command == "Format 2")
                    { ge.ExportToExcel(ds.Tables["Master1"], "Master1"); }
                    
                    return null;
                }
            }            
        }
        public ActionResult Payroll()
        {
            return View();
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Payroll(string Download)
        {
            using (RecruitEntities recruit = new RecruitEntities())
            {
                using (SqlConnection con1 = new SqlConnection())
                {
                    con1.ConnectionString = ConfigurationManager.ConnectionStrings["Recruit"].ConnectionString;
                    //string sql="select a"
                }
            }
            return View();
        }
    }  
}

