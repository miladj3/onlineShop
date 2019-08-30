using onlineShop.DTOs;
using onlineShop.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace onlineShop.Validation
{
    public class RequiredForDeliveryMethodType : ValidationAttribute
    {
        public DeliveryMethodType _deliveryMethodType { get; set; }

        public RequiredForDeliveryMethodType(DeliveryMethodType deliveryMethodType)
        {
            _deliveryMethodType = deliveryMethodType;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            OrderDetailsDTO model = (OrderDetailsDTO)validationContext.ObjectInstance;

            var selectedMethod = model.DeliveryMethodType;

            var propName = validationContext.DisplayName;
            var fieldValue = (string)value;

            if (selectedMethod == _deliveryMethodType)
            {
                if (String.IsNullOrEmpty(fieldValue))
                {
                    return new ValidationResult(ErrorMessage = propName + " is required for selected delivery method.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
