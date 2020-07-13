using maintenanceApplication.Models.Maintenance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace maintenanceApplication.ViewModels
{
    public class MaintenanceTechnicalReportViewModel
    {
        public IEnumerable<MaintenanceModel> maintenanceList { get; set; }
        public MaintenanceModel maintenance { get; set; }
    }
}