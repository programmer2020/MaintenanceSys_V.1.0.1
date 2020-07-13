using maintenanceApplication.Models.Maintenance.lookups;
using maintenanceApplication.Models.validations;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace maintenanceApplication.Models.Maintenance
{
    public class MaintenanceModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(250)]
        [Display(Name = "Client Name")]
        //ResourceType = typeof(Resource))
        public string Customer_Name { get; set; }

        [Required]
        [MaxLength(250)]
        [Display(Name = "Customer Phone 1")]
        public string Customer_Phone_1 { get; set; }

        [Required]
        [MaxLength(250)]
        [Display(Name = "Customer Phone 2")]
        public string Customer_Phone_2 { get; set; }

        [Required]
        [MaxLength(250)]
        [Display(Name = "Customer Adress")]
        public string Customer_Adress { get; set; }

        [Required]
        [MaxLength(250)]
        [Display(Name = "Device Serial Number")]
        [RegularExpression(@"(\S)+", ErrorMessage = "White space is not allowed")]
        //[customeValidation]
        public string Device_SerialNumber { get; set; }

        [Required]
        [MaxLength(250)]
        [Display(Name = "Device Model")]
        public string Device_Model { get; set; }

        [MaxLength(250)]
        [Display(Name = "Accrssories")]
        [Required]
        public string Accrssories { get; set; }

        [Required]
        [Display(Name = "Creation Date")]
        public DateTime CreationDate { get; set; }

        [Required]
        [MaxLength(500)]
        [Display(Name = "Client Remarks")]
        public string ClientRemarks {get;set;}

        [MaxLength(500)]
        [Display(Name = "Recommendations")]
        [Required]
        [DefaultValue("No Recommendations Yet")]
        public string Recommendations { get; set; }

        [Display(Name = "Technical Report")]
        [MaxLength(500)]
        [Required]
        [DefaultValue("No Technical Report Yet")]
        public string TechnicalReport { get; set; }

        [Display(Name = "Check Completed Date")]
        public DateTime? CheckCompleted_Date { get; set; }

        [Display(Name = "Completed Repair Date")]
        public DateTime? Actual_Repair_Date { get; set; }

        [Display(Name = "Deliver Date")]
        public DateTime? Deliver_Date { get; set; }

        [Display(Name = "Approved Date")]
        public DateTime? Approved_Date { get; set; }

        [Display(Name = "Start Checking Date")]
        public DateTime? StartCheckingDate { get; set; }

        [Display(Name = "Start Reparing Date")]
        public DateTime? StartReparingDate { get; set; }

        [Display(Name = "Quality Reject Date")]
        public DateTime? QualityRejectDate { get; set; }

        [Display(Name = "Quality Approved Date")]
        public DateTime? QualityApprovedDate { get; set; }

        [Display (Name = "Price")]
        [DefaultValue(0)]
        [Required]
        public double price { get; set; }

        [Display(Name = "Is Accessories Available")]
        public bool isAccessoriesAvailable { get; set; }

        [Required]
        public int MaintenanceCustomerCityModelId { get; set; }
        public MaintenanceCustomerCityModel CustomerCity { get; set; }

        //[Required]
        public IdentityUser user { get; set; }
        //[Key, ForeignKey("Users")]
        [NotMapped]
        public string user_Id { get; set; }

        [Required]
        public string userName { get; set; }

        public MaintenancePriorityModel priority {get;set;}
        [Display(Name = "Maintenance Priority")]
        public int? MaintenancePriorityModelId {get;set;}
        [Display(Name = "Maintenance Status")]
        public MaintenanceStatusModel status {get;set;}
        [Required]
        public int MaintenanceStatusModelId {get;set;}

        [Display(Name = "Deliver Reason")]
        public string deliverReason { get; set; }

        public bool isDeleted { get; set; }

        [Display(Name = "Is Repeated")]
        public bool isRepeated { get; set; }



    }
}