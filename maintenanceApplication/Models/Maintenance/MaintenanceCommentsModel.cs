using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace maintenanceApplication.Models.Maintenance
{
    public class MaintenanceCommentsModel
    {
        [Key]
        public int Id { get; set; }
        public string comment { get; set; }
        public DateTime comment_DateTme { get; set; }
        public string  userImagePath { get; set; }

        //[NotMapped]
        public string user_Id { get; set; }
        //public ApplicationUser user { get; set; }

        [Required]
        public int MaintenanceModelId { get; set; }
        public MaintenanceModel maintenance { get; set; }

    }
}