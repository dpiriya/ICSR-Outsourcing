using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Expressions;
using System.Web.Mvc.Html;
using Outsourcing.CustomDataAnnotations;
namespace Outsourcing.CustomHelper
{
    public static class BooleanOptionHelper
    {
        public static IList<SelectListItem> OptionsForBoolean <TModel,TProperty>(this HtmlHelper<TModel>htmlHelper, Expression<Func<TModel, TProperty>> expression)
{
    var metaData = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
    object trueTitle;
    metaData.AdditionalValues.TryGetValue(BooleanDisplayValuesAttribute.TrueTitleAdditionalValueName, out trueTitle);
    trueTitle = trueTitle ?? "Yes";
 
    object falseTitle;
    metaData.AdditionalValues.TryGetValue(BooleanDisplayValuesAttribute.FalseTitleAdditionalValueName, out falseTitle);
    falseTitle = falseTitle ?? "No";
 
    var options = new[]
                        {
                            new SelectListItem {Text = (string) falseTitle, Value = Boolean.FalseString},
                            new SelectListItem {Text = (string) trueTitle, Value = Boolean.TrueString},
                        };
    return options;
}

    }
}