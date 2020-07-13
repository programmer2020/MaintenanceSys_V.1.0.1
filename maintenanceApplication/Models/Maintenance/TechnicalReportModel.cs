using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace maintenanceApplication.Models.Maintenance
{
    public class TechnicalReportModel
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Report Details")]
        public string TechnicalReportReport { get; set; }

        public DateTime PlannedRepairDate { get; set; }

        public DateTime ActualRepairDate { get; set; }


        public MaintenanceModel maintenance { get; set; }
        public int MaintenanceModelId { get; set;}



    }
}