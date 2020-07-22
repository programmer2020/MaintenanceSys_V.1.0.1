using maintenanceApplication.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace maintenanceApplication.Controllers.Admin
{
    [Authorize(Roles = "SuperAdmin")]
    public class AdminController : Controller
    {
        ApplicationDbContext context = new ApplicationDbContext();

        [Authorize (Roles = "SuperAdmin")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult CreateUser()
        {
            return View(); 
        }

        [HttpPost]
        public ActionResult CreateUser(FormCollection form)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            string UserName = form["UserName"];
            string Email = form["Email"];
            string Password = form["Password"]; 

            var user = new ApplicationUser();
            user.UserName = UserName;
            user.Email = Email;

            var newUser = userManager.Create(user, Password);

            return View("index");
        }

        [HttpGet]
        public ActionResult CreteRole()
        {
            return View(); 

        }

        [HttpPost]
        public ActionResult CreteRole(FormCollection form)
        {
            string RoleName = form["RoleName"];

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            if (!roleManager.RoleExists(RoleName))
            {
                var role = new IdentityRole(RoleName);
                roleManager.Create(role);
            }

            return View("index");
        }

        [HttpGet]
        public ActionResult AssignRole()
        {
            ViewBag.Roles = context.Roles.Select(x => new SelectListItem { Value = x.Name, Text = x.Name }).ToList(); 
            return View(); 
        }

        [HttpPost]
        public ActionResult AssignRole(FormCollection form)
        {

            ViewBag.Roles = context.Roles.Select(x => new SelectListItem { Value = x.Name, Text = x.Name }).ToList();
            string UserName = form["UserName"];
            string RoleName = form["RoleName"];

            ApplicationUser user = context.Users.Where(x=>x.UserName.Equals(UserName , StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault(); 
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            userManager.AddToRole(user.Id, RoleName);

            return View("index");
        }
    }
}