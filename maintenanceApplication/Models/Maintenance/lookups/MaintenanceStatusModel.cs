using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace maintenanceApplication.Models.Maintenance.lookups
{
    public class MaintenanceStatusModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(250)]
        [Display(Name = "Status Name")]
        public string StatusName { get; set; }
    }
}