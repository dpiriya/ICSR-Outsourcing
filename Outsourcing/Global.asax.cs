using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Globalization;
using System.Threading;

namespace Outsourcing
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //ModelBinders.Binders.Add(typeof(DateTime?), new MyDateTimeModelBinder());


            AuthConfig.RegisterAuth();
        }
      
    //protected void Application_BeginRequest(object sender, EventArgs e)
    //{
    //    System.Globalization.CultureInfo ci =
    //    System.Threading.Thread.CurrentThread.CurrentCulture.Clone() as System.Globalization.CultureInfo;
    //    ci.DateTimeFormat.LongTimePattern = "dd/MM/yyyy";
    //    ci.DateTimeFormat.FullDateTimePattern = "dd/MM/yyyy";
    //    System.Threading.Thread.CurrentThread.CurrentCulture = ci;
    //}
    //protected void Application_BeginRequest(object sender, EventArgs e)
    //{
    //    CultureInfo newCulture = (CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
    //    newCulture.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
    //    newCulture.DateTimeFormat.DateSeparator = "/";
    //    Thread.CurrentThread.CurrentCulture = newCulture;
    //}

}

    //public class MyDateTimeModelBinder : DefaultModelBinder
    //{
    //    public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
    //    {
    //        var displayFormat = bindingContext.ModelMetadata.DisplayFormatString;
    //        var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

    //        if (!string.IsNullOrEmpty(displayFormat) && value != null)
    //        {
    //            DateTime date;
    //            displayFormat = displayFormat.Replace("{0:", string.Empty).Replace("}", string.Empty);
    //            if (DateTime.TryParseExact(value.AttemptedValue, displayFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
    //            {
    //                return date;
    //            }
    //            else
    //            {
    //                bindingContext.ModelState.AddModelError(bindingContext.ModelName, string.Format("{0} is an invalid Date format", value.AttemptedValue));
    //            }
    //        }
    //        return base.BindModel(controllerContext, bindingContext);
    //    }
    //}
}