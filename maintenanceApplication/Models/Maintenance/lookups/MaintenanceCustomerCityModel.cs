using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace maintenanceApplication.Models.Maintenance.lookups
{
    public class MaintenanceCustomerCityModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(250)]
        [Display(Name = "Customer Country")]
        public string CustomerCity { get; set; }

    }



}



