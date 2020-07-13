using maintenanceApplication.Models;
using maintenanceApplication.Models.Maintenance;
using maintenanceApplication.Models.Maintenance.lookups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace maintenanceApplication.ViewModels
{
    public class MaintenancesingleViewModel
    {
        public MaintenanceModel maintenance;
        public MaintenancePriorityModel priority;
        public MaintenanceStatusModel status; 

    }
}