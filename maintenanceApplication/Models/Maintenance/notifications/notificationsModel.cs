using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace maintenanceApplication.Models.Maintenance.notifications
{
    public class notificationsModel
    {
        public int Id { get; set; }
        public int notificationCount { get; set; }
        [NotMapped]
        public int User_Id { get; set; }
        public ApplicationUser user { get; set; }

        public int MaintenanceModelId { get; set; }
        public MaintenanceModel maintennace { get; set; }

        public int notificationLinksId { get; set; }
        public notificationLinksModel notificationLinks { get; set; }

    }
}