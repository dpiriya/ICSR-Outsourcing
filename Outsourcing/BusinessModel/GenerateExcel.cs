using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Text;

namespace Outsourcing.BusinessModel
{
    public class GenerateExcel
    {
        public void ExportToExcel(DataTable dtSource, string fileName)
        {
            //System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            StringBuilder sbDocBody = new StringBuilder(); ;
            try
            {
                sbDocBody.Append("<style>");
                sbDocBody.Append(".Header {font-weight:bold;font-family:Verdana; font-size:12px;}");
                sbDocBody.Append(".Content {font-family:Verdana; font-size:12px;text-align:left}");
                sbDocBody.Append("</style>");
                sbDocBody.Append("<table align=\"center\" border=\"1\" cellpadding=1 cellspacing=0 >");
                sbDocBody.Append("<tr><td>");
                sbDocBody.Append("<table border=\"1\" cellpadding=1 cellspacing=2 >");
                if (dtSource.Rows.Count > 0)
                {
                    sbDocBody.Append("<tr><td>");
                    sbDocBody.Append("<table border=\"1\" cellpadding=\"0\" cellspacing=\"2\"><tr>");
                    // Add Column Headers
                    for (int i = 0; i < dtSource.Columns.Count; i++)
                    {
                        sbDocBody.Append("<td class=\"Header\" >" + dtSource.Columns[i].ToString().Replace(".", "<br>") + "</td>");
                    }
                    sbDocBody.Append("</tr>");
                    // Add Data Rows
                    for (int i = 0; i < dtSource.Rows.Count; i++)
                    {
                        sbDocBody.Append("<tr>");
                        for (int j = 0; j < dtSource.Columns.Count; j++)
                        {
                            string dt = dtSource.Columns[j].DataType.ToString();

                            if (dt == "System.DateTime")
                            {
                                if (dtSource.Rows[i][j] != null && dtSource.Rows[i][j].ToString() != "")
                                {
                                    sbDocBody.Append("<td class=\"Content\">" + Convert.ToDateTime(dtSource.Rows[i][j]).ToShortDateString().ToString() + "</td>");
                                }
                                else
                                {
                                    sbDocBody.Append("<td class=\"Content\">" + dtSource.Rows[i][j].ToString() + "</td>");
                                }
                            }
                            else
                            {
                                sbDocBody.Append("<td class=\"Content\">" + dtSource.Rows[i][j].ToString() + "</td>");
                            }
                        }
                        sbDocBody.Append("</tr>");

                    }
                    sbDocBody.Append("</table>");
                    sbDocBody.Append("</td></tr></table>");
                    sbDocBody.Append("</td></tr></table>");
                }
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.Buffer = true;
                //HttpContext.Current.Response.AppendHeader("Content-Type", "application/octet-stream");
                HttpContext.Current.Response.AppendHeader("Content-Type", "application/vnd.ms-excel");
                DateTime fName = DateTime.Now;
                string tFile = fileName + fName.Year.ToString() + fName.Month.ToString() + fName.Day.ToString();
                HttpContext.Current.Response.AppendHeader("Content-disposition", "attachment; filename=" + tFile + ".xls");
                HttpContext.Current.Response.Write(sbDocBody.ToString());
                HttpContext.Current.Response.End();
            }
            catch (Exception ex)
            {
                string errorStr = ex.Message;
                //Response.Write("<script type='text/javascript'>alert('" + ex.Message + "')</script>");
            }
        }

        public void ExportToWord(DataTable dtSource, string fileName)
        {
            //System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            StringBuilder sbDocBody = new StringBuilder(); ;
            try
            {
                sbDocBody.Append("<style>");
                sbDocBody.Append(".Header {font-weight:bold;font-family:Verdana; font-size:12px;}");
                sbDocBody.Append(".Content {font-family:Verdana; font-size:12px;text-align:left}");
                sbDocBody.Append("</style>");
                sbDocBody.Append("<table align=\"center\" border=\"0\" cellpadding=1 cellspacing=0 >");
                sbDocBody.Append("<tr><td>");
                sbDocBody.Append("<table border=\"0\" cellpadding=1 cellspacing=2 >");
                if (dtSource.Rows.Count > 0)
                {
                    sbDocBody.Append("<tr><td>");
                    sbDocBody.Append("<table border=\"1\" cellpadding=\"0\" cellspacing=\"2\"><tr>");
                    // Add Column Headers
                    for (int i = 0; i < dtSource.Columns.Count; i++)
                    {
                        sbDocBody.Append("<td class=\"Header\" >" + dtSource.Columns[i].ToString().Replace(".", "<br>") + "</td>");
                    }
                    sbDocBody.Append("</tr>");
                    // Add Data Rows
                    for (int i = 0; i < dtSource.Rows.Count; i++)
                    {
                        sbDocBody.Append("<tr>");
                        for (int j = 0; j < dtSource.Columns.Count; j++)
                        {
                            string dt = dtSource.Columns[j].DataType.ToString();

                            if (dt == "System.DateTime")
                            {
                                if (dtSource.Rows[i][j] != null && dtSource.Rows[i][j].ToString() != "")
                                {
                                    sbDocBody.Append("<td class=\"Content\">" + Convert.ToDateTime(dtSource.Rows[i][j]).ToShortDateString().ToString() + "</td>");
                                }
                                else
                                {
                                    sbDocBody.Append("<td class=\"Content\">" + dtSource.Rows[i][j].ToString() + "</td>");
                                }
                            }
                            else
                            {
                                sbDocBody.Append("<td class=\"Content\">" + dtSource.Rows[i][j].ToString() + "</td>");
                            }
                        }
                        sbDocBody.Append("</tr>");

                    }
                    sbDocBody.Append("</table>");
                    sbDocBody.Append("</td></tr></table>");
                    sbDocBody.Append("</td></tr></table>");
                }
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.Buffer = true;
                //HttpContext.Current.Response.AppendHeader("Content-Type", "application/octet-stream");
                HttpContext.Current.Response.AppendHeader("Content-Type", "application/msword");
                DateTime fName = DateTime.Now;
                string tFile = fileName + fName.Year.ToString() + fName.Month.ToString() + fName.Day.ToString();
                HttpContext.Current.Response.AppendHeader("Content-disposition", "attachment; filename=" + tFile + ".doc");
                HttpContext.Current.Response.Write(sbDocBody.ToString());
                HttpContext.Current.Response.End();
            }
            catch (Exception ex)
            {
                string errorStr = ex.Message;
                //Response.Write("<script type='text/javascript'>alert('" + ex.Message + "')</script>");
            }
        }
    }
   
}