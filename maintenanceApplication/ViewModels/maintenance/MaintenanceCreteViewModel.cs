using maintenanceApplication.Models;
using maintenanceApplication.Models.Maintenance;
using maintenanceApplication.Models.Maintenance.lookups;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace maintenanceApplication.ViewModels
{
    public class MaintenanceCreteViewModel
    {
        public MaintenanceModel maintenance { get; set; }
        public MaintenanceCommentsModel maintenanceComments { get; set; }
        public IEnumerable<MaintenancePriorityModel> maintenancepriority { get; set; }
        public IEnumerable<MaintenanceStatusModel> maintenancestatus { get; set; }
        public IEnumerable<IdentityUser> users { get; set; }
        public IEnumerable<MaintenanceCustomerCityModel> CustomerCity { get; set; }

 
    }
}