using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;
namespace Outsourcing.ViewModel
{
    public class Authority
    {
        public string AuthorityID { get; set;}
        public string AuthorityName { get; set; }
        public string AuthorityDesignation { get; set;}
               
    }
    public class Authorities 
    {
        
        public static List<Authority> AuthorityList()
        {
            List<Authority> aList  = new List<Authority>();
            aList.Add( new Authority() { AuthorityID= "AR",AuthorityName="",AuthorityDesignation="Assistant Registrar "});
            aList.Add(new Authority() { AuthorityID = "Senior Manager – HR", AuthorityName = "", AuthorityDesignation = "Senior Manager – HR" });
            return aList;
        }
    } 

}