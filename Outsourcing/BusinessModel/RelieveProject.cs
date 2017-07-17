using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;


namespace Outsourcing.BusinessModel
{
    public class RelieveProject
    {
        SqlConnection con = new SqlConnection();
        public string updateRelieveProject(string EmployeeID, string MeetingID, int OrderID, DateTime RelieveDate, DateTime UpdatedOn, string UpdatedBy)
        {
            SqlTransaction trans;            
            con.ConnectionString = ConfigurationManager.ConnectionStrings["Recruit"].ConnectionString;
            con.Open();
            trans= con.BeginTransaction();            
            try
            {
                string sql = "update AppointmentDetails set FromDate=@fromDate,UpdatedOn=@updatedOn,UpdatedBy=@updatedBy where EmployeeID=@employeeID and MeetingID=@meetingID and OrderID=@orderID";
                SqlCommand cmd= new SqlCommand ();
                cmd.CommandText=sql;
                cmd.CommandType= CommandType.Text;
                cmd.Transaction=trans;
                cmd.Connection=con;
                SqlParameter sp1= new SqlParameter ();
                sp1.SourceColumn="EmployeeID";
                sp1.ParameterName="@employeeID";
                sp1.Value=EmployeeID;
                sp1.SqlDbType=SqlDbType.VarChar;
                sp1.Direction=ParameterDirection.Input;
                SqlParameter sp2= new SqlParameter ();
                sp2.SourceColumn="MeetingID";
                sp2.ParameterName="@meetingID";
                sp2.Value=MeetingID;
                sp2.SqlDbType=SqlDbType.VarChar;
                sp2.Direction=ParameterDirection.Input;
                SqlParameter sp3= new SqlParameter ();
                sp3.SourceColumn="OrderID";
                sp3.ParameterName="@orderID";
                sp3.Value=OrderID;
                sp3.SqlDbType=SqlDbType.Int;
                sp3.Direction=ParameterDirection.Input;
                SqlParameter sp4= new SqlParameter ();
                sp4.SourceColumn="FromDate";
                sp4.ParameterName="@fromDate";
                sp4.Value = RelieveDate;
                sp4.SqlDbType=SqlDbType.DateTime;
                sp4.Direction=ParameterDirection.Input;
                SqlParameter sp5= new SqlParameter ();
                sp5.SourceColumn="UpdatedOn";
                sp5.ParameterName="@updatedOn";
                sp5.Value=UpdatedOn;
                sp5.SqlDbType=SqlDbType.DateTime;
                sp5.Direction=ParameterDirection.Input;
                SqlParameter sp6= new SqlParameter ();
                sp6.SourceColumn="UpdatedBy";
                sp6.ParameterName="@updatedBy";
                sp6.Value=UpdatedBy;
                sp6.SqlDbType=SqlDbType.VarChar;
                sp6.Direction=ParameterDirection.Input;
                cmd.Parameters.Add(sp1);
                cmd.Parameters.Add(sp2);
                cmd.Parameters.Add(sp3);
                cmd.Parameters.Add(sp4);
                cmd.Parameters.Add(sp5);
                cmd.Parameters.Add(sp6);
                cmd.ExecuteNonQuery();
                string sql2 = "update AppointmentMaster set ToDate=@RelieveDate,UpdatedOn=@updatedOn,UpdatedBy=@updatedBy where EmployeeID=@employeeID";
                SqlCommand cmd2 = new SqlCommand();
                cmd2.CommandText = sql2;
                cmd2.CommandType = CommandType.Text;
                cmd2.Transaction = trans;
                cmd2.Connection = con;
                SqlParameter sm1 = new SqlParameter();
                sm1.SourceColumn = "EmployeeID";
                sm1.ParameterName = "@employeeID";
                sm1.Value = EmployeeID;
                sm1.SqlDbType = SqlDbType.VarChar;
                sm1.Direction = ParameterDirection.Input;
                SqlParameter sm2 = new SqlParameter();
                sm2.SourceColumn = "ToDate";
                sm2.ParameterName = "@RelieveDate";
                sm2.Value = RelieveDate;
                sm2.SqlDbType = SqlDbType.DateTime;
                sm2.Direction = ParameterDirection.Input;
                SqlParameter sm3 = new SqlParameter();
                sm3.SourceColumn = "UpdatedOn";
                sm3.ParameterName = "@updatedOn";
                sm3.Value = UpdatedOn;
                sm3.SqlDbType = SqlDbType.DateTime;
                sm3.Direction = ParameterDirection.Input;
                SqlParameter sm4 = new SqlParameter();
                sm4.SourceColumn = "UpdatedBy";
                sm4.ParameterName = "@updatedBy";
                sm4.Value = UpdatedBy;
                sm4.SqlDbType = SqlDbType.VarChar;
                sm4.Direction = ParameterDirection.Input;
                cmd2.Parameters.Add(sm1);
                cmd2.Parameters.Add(sm2);
                cmd2.Parameters.Add(sm3);
                cmd2.Parameters.Add(sm4);
                cmd2.ExecuteNonQuery();
                trans.Commit();
                con.Close();
                return "true";
            }
            catch(Exception ex)
            {
                trans.Rollback();
                con.Close();
                return "false";
            }
        }
        public string updateRelieveOrder(string EmployeeID, string MeetingID, int OrderID, DateTime RelieveDate, DateTime UpdatedOn, string UpdatedBy)
        {
            SqlTransaction trans;
            con.ConnectionString = ConfigurationManager.ConnectionStrings["Recruit"].ConnectionString;
            con.Open();
            trans = con.BeginTransaction();
            try
            {
                string sql = "update AppointmentDetails set FromDate=@fromDate,UpdatedOn=@updatedOn,UpdatedBy=@updatedBy where EmployeeID=@employeeID and MeetingID=@meetingID and OrderID=@orderID";
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;
                cmd.Transaction = trans;
                cmd.Connection = con;
                SqlParameter sp1 = new SqlParameter();
                sp1.SourceColumn = "EmployeeID";
                sp1.ParameterName = "@employeeID";
                sp1.Value = EmployeeID;
                sp1.SqlDbType = SqlDbType.VarChar;
                sp1.Direction = ParameterDirection.Input;
                SqlParameter sp2 = new SqlParameter();
                sp2.SourceColumn = "MeetingID";
                sp2.ParameterName = "@meetingID";
                sp2.Value = MeetingID;
                sp2.SqlDbType = SqlDbType.VarChar;
                sp2.Direction = ParameterDirection.Input;
                SqlParameter sp3 = new SqlParameter();
                sp3.SourceColumn = "OrderID";
                sp3.ParameterName = "@orderID";
                sp3.Value = OrderID;
                sp3.SqlDbType = SqlDbType.Int;
                sp3.Direction = ParameterDirection.Input;
                SqlParameter sp4 = new SqlParameter();
                sp4.SourceColumn = "FromDate";
                sp4.ParameterName = "@fromDate";
                sp4.Value = RelieveDate;
                sp4.SqlDbType = SqlDbType.DateTime;
                sp4.Direction = ParameterDirection.Input;
                SqlParameter sp5 = new SqlParameter();
                sp5.SourceColumn = "UpdatedOn";
                sp5.ParameterName = "@updatedOn";
                sp5.Value = UpdatedOn;
                sp5.SqlDbType = SqlDbType.DateTime;
                sp5.Direction = ParameterDirection.Input;
                SqlParameter sp6 = new SqlParameter();
                sp6.SourceColumn = "UpdatedBy";
                sp6.ParameterName = "@updatedBy";
                sp6.Value = UpdatedBy;
                sp6.SqlDbType = SqlDbType.VarChar;
                sp6.Direction = ParameterDirection.Input;
                cmd.Parameters.Add(sp1);
                cmd.Parameters.Add(sp2);
                cmd.Parameters.Add(sp3);
                cmd.Parameters.Add(sp4);
                cmd.Parameters.Add(sp5);
                cmd.Parameters.Add(sp6);
                cmd.ExecuteNonQuery();
                string sql2 = "update AppointmentMaster set RelieveDate=@RelieveDate,UpdatedOn=@updatedOn,UpdatedBy=@updatedBy where EmployeeID=@employeeID";
                SqlCommand cmd2 = new SqlCommand();
                cmd2.CommandText = sql2;
                cmd2.CommandType = CommandType.Text;
                cmd2.Transaction = trans;
                cmd2.Connection = con;
                SqlParameter sm1 = new SqlParameter();
                sm1.SourceColumn = "EmployeeID";
                sm1.ParameterName = "@employeeID";
                sm1.Value = EmployeeID;
                sm1.SqlDbType = SqlDbType.VarChar;
                sm1.Direction = ParameterDirection.Input;
                SqlParameter sm2 = new SqlParameter();
                sm2.SourceColumn = "RelieveDate";
                sm2.ParameterName = "@RelieveDate";
                sm2.Value = RelieveDate;
                sm2.SqlDbType = SqlDbType.DateTime;
                sm2.Direction = ParameterDirection.Input;
                SqlParameter sm3 = new SqlParameter();
                sm3.SourceColumn = "UpdatedOn";
                sm3.ParameterName = "@updatedOn";
                sm3.Value = UpdatedOn;
                sm3.SqlDbType = SqlDbType.DateTime;
                sm3.Direction = ParameterDirection.Input;
                SqlParameter sm4 = new SqlParameter();
                sm4.SourceColumn = "UpdatedBy";
                sm4.ParameterName = "@updatedBy";
                sm4.Value = UpdatedBy;
                sm4.SqlDbType = SqlDbType.VarChar;
                sm4.Direction = ParameterDirection.Input;
                cmd2.Parameters.Add(sm1);
                cmd2.Parameters.Add(sm2);
                cmd2.Parameters.Add(sm3);
                cmd2.Parameters.Add(sm4);
                cmd2.ExecuteNonQuery();
                trans.Commit();
                con.Close();
                return "true";
            }
            catch (Exception ex)
            {
                trans.Rollback();
                con.Close();
                return "false";
            }
        }
    }
}