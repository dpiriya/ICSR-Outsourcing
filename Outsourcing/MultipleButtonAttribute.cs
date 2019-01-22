using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Outsourcing
{
    [AttributeUsage(AttributeTargets.Method,AllowMultiple=false,Inherited=true)]
    public class MultipleButtonAttribute:ActionNameSelectorAttribute
    {
        public string Name { get; set; }
        public string Argument { get; set; }

        public override bool IsValidName(ControllerContext controllerContext, string actionName, System.Reflection.MethodInfo methodInfo)
        {
            var isValidName = false;
            var keyValue = string.Format("{0}:{1}", Name, Argument);
            var value = controllerContext.Controller.ValueProvider.GetValue(keyValue);
            if (value != null)
            {
                controllerContext.Controller.ControllerContext.RouteData.Values[Name] = Argument;
                isValidName = true;
            }
            return isValidName;
        }
        
    }
    public class DenyByControllerAttribute:AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {            
            var controller = httpContext.Request.RequestContext.RouteData.GetRequiredString("controller");
            var action = httpContext.Request.RequestContext.RouteData.GetRequiredString("action");
            var denyUser = string.Format("Deny{0}:{1}", controller, action);
            if (httpContext.User.Identity.Name == "tnm")
                return false;
            else
            return !base.AuthorizeCore(httpContext);
        }
    }
}