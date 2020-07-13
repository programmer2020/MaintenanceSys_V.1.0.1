using maintenanceApplication.Models.Maintenance;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace maintenanceApplication.Models.validations
{
    public class customeValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var maintenanceDeviceSerialNumber = (MaintenanceModel)validationContext.ObjectInstance;
            if(maintenanceDeviceSerialNumber.Device_SerialNumber == null)
            {
                return new ValidationResult("Device Serial Nubmber Required");
            }

            if (maintenanceDeviceSerialNumber.Device_SerialNumber.Length < 2)
            {
               
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult("Device Serial Nubmber Should Be Without Spaces");

            }


        }
    }
}