using maintenanceApplication.Models;
using maintenanceApplication.Models.Maintenance.lookups;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

[assembly: OwinStartupAttribute(typeof(maintenanceApplication.Startup))]
namespace maintenanceApplication
{
    public partial class Startup
    {
        ApplicationDbContext context = new ApplicationDbContext();

        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            CreateUSerAndRoles();
            IntialData();
        }

        private  void IntialData()
        {
            ////check if status have data
            var statusData = context.status.ToList();
            if (statusData.Count < 1)
            {
                //Adding Status
                List<string> statusList = new List<string>();
                statusList.Add("New Request");
                statusList.Add("Under Checking");
                statusList.Add("Under Approval");
                statusList.Add("Approved");
                statusList.Add("Under Reparing");
                statusList.Add("Quality Check");
                statusList.Add("To be Delivered");
                statusList.Add("Delivered");
              
                var status = new MaintenanceStatusModel();
                foreach (string statusItem in statusList)
                {
                    status.StatusName = statusItem;
                    context.status.Add(status);
                    context.SaveChanges();
                }
            }
            //Adding Priority
            var prorityData = context.priority.ToList();
            if (prorityData.Count < 1)
            {
                List<string> priorityList = new List<string>();
                priorityList.Add("Normal");
                priorityList.Add("Urgent");

                var priority = new MaintenancePriorityModel();
                foreach (string priorityItem in priorityList)
                {
                    priority.PriorityName = priorityItem;
                    context.priority.Add(priority);
                    context.SaveChanges();
                }
            }
        }

        private void CreateUSerAndRoles()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            if (!roleManager.RoleExists("SuperAdmin"))
            {
                //Create All Needed Roles On Applcation Starts 
                //var SuperAdmin = new IdentityRole("SuperAdmin");
                //var Anymoumus = new IdentityRole("Anymoumus");
                //var Technical = new IdentityRole("Technical");
                //var Supervisor = new IdentityRole("Supervisor");
                //var Admin = new IdentityRole("Admin");

                //roleManager.Create(SuperAdmin); 
                //roleManager.Create(Anymoumus); 
                //roleManager.Create(Technical); 
                //roleManager.Create(Supervisor); 
                //roleManager.Create(Admin); 


                //Create Default user 
                var user = new ApplicationUser();
                user.UserName = "superadmin";
                user.Email = "super@admin.com";
                string pwd = "admin@2020";

                var newUser = userManager.Create(user, pwd);
                if (newUser.Succeeded)
                {
                    userManager.AddToRole(user.Id, "SuperAdmin");
                }
            }
        }
    }
}
