using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using maintenanceApplication.Models.Maintenance;
using maintenanceApplication.Models.Maintenance.lookups;
using maintenanceApplication.Models.Maintenance.notifications;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

namespace maintenanceApplication.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        //public string userImagePath { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationRole  : IdentityRole
    {
        public ApplicationRole() : base() { }
        public ApplicationRole(string roleName) : base(roleName) { }

    } 

    public class ApplicationRoleManager : RoleManager<ApplicationRole>
    {
        public ApplicationRoleManager(IRoleStore<ApplicationRole, string> roleStore) : base(roleStore) { }
        public static ApplicationRoleManager Create (IdentityFactoryOptions<ApplicationRoleManager> options , IOwinContext context)
        {
            var applicationRoleManager = new ApplicationRoleManager(new RoleStore<ApplicationRole>(context.Get<ApplicationDbContext>())); 
            return applicationRoleManager; 
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<MaintenanceModel> maintenance { get; set; }
        public DbSet<MaintenancePriorityModel> priority { get; set; }
        public DbSet<MaintenanceStatusModel> status { get; set; }
        public DbSet<TechnicalReportModel> technicalReport { get; set; }
        public DbSet<MaintenanceCustomerCityModel> customerCity { get; set; }
        public DbSet<MaintenanceCommentsModel> maintenceComments { get; set; }

        public DbSet<notificationLinksModel> notificationLinks { get; set; }
        public DbSet<notificationsModel> notifications { get; set; }
        

        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        //public System.Data.Entity.DbSet<maintenanceApplication.Models.RoleViewModel> RoleViewModels { get; set; }
    }
}