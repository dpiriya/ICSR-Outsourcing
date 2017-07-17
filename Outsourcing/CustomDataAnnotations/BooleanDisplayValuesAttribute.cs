using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Outsourcing.CustomDataAnnotations
{
    [AttributeUsage(AttributeTargets.Property,
                Inherited = false, AllowMultiple = false)]
    public class BooleanDisplayValuesAttribute : Attribute, IMetadataAware
    {
        public const string TrueTitleAdditionalValueName = "BooleanTrueValueTitle";
        public const string FalseTitleAdditionalValueName = "BooleanFalseValueTitle";
 
        private readonly string _trueValueTitle;
        private readonly string _falseValueTitle;
 
    public BooleanDisplayValuesAttribute(string trueValueTitle,string falseValueTitle)
    {
        _trueValueTitle = trueValueTitle;
        _falseValueTitle = falseValueTitle;
    }
 
    public void OnMetadataCreated (ModelMetadata metadata)
    {
        metadata.AdditionalValues[TrueTitleAdditionalValueName] = _trueValueTitle;
        metadata.AdditionalValues[FalseTitleAdditionalValueName] = _falseValueTitle;
    }

    }
    public class BooleanDisplayValuesAsShowHideAttribute : BooleanDisplayValuesAttribute
    {
        public BooleanDisplayValuesAsShowHideAttribute() : base("Show", "Hide")
        {
        }
    }
    public class BooleanDisplayValuesAsEnableDisableAttribute : BooleanDisplayValuesAttribute
    {
        public BooleanDisplayValuesAsEnableDisableAttribute() : base("Enable", "Disable")
        {
        }
    }

    public class BooleanDisplayValuesAsYesNoAttribute : BooleanDisplayValuesAttribute
    {
        public BooleanDisplayValuesAsYesNoAttribute() : base("Yes", "No")
        {
        }
    }
}