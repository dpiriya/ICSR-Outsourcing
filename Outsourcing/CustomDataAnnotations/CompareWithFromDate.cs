using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Outsourcing.CustomDataAnnotations
{
    public class CompareWithFromDate:ValidationAttribute
    {
        private ValidationType _ValidationType;
        private DateTime? _fromDate;
        private DateTime _toDatae;
        private string _defaultErrorMessage;
        private string _propertyNameToCompare;
        public CompareWithFromDate()
        { }
        public override bool IsValid(object value)
        {
            return base.IsValid(value);
        }

        public enum ValidationType
        {
            RangeValidation, 
            Compare
        }
        public CompareWithFromDate(ValidationType validationType, string message, string compareWith = "", string fromDate="")
        {
            _ValidationType = validationType;
            switch (validationType)
            { 
                case ValidationType.Compare:
                    {
                        _propertyNameToCompare=compareWith;
                        _defaultErrorMessage=message;
                        break;

                    }
                case ValidationType.RangeValidation:
                    {
                        _fromDate = DateTime.Today;
                        _toDatae = DateTime.Today.AddMonths(15);
                        _defaultErrorMessage = message;
                        break;
                    }
            }
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            switch (_ValidationType)
            {
                case ValidationType.Compare:
                    {
                        var baseProperyInfo = validationContext.ObjectType.GetProperty(_propertyNameToCompare);
                        var startDate = (DateTime)baseProperyInfo.GetValue(validationContext.ObjectInstance, null);
                            if(value!=null)
                            {
                                DateTime thisDate=(DateTime)value;
                                Type classType=typeof(Outsourcing.ViewModel.OutsourcingMeetingView);
                                PropertyInfo methodInfo=classType.GetProperty(_propertyNameToCompare);
                                DisplayAttribute displayAttr=(DisplayAttribute)Attribute.GetCustomAttribute(methodInfo,typeof(DisplayAttribute));
                                if(thisDate<=startDate)
                                {
                                    string message=string.Format(_defaultErrorMessage,validationContext.DisplayName,displayAttr.Name);
                                    return new ValidationResult(message);
                                }
                            }
                        break;
                    }
            }
            return null;
        }
    }
}