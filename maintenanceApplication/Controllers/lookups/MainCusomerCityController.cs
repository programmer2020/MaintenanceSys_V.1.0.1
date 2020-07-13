using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using maintenanceApplication.Models;
using maintenanceApplication.Models.Maintenance.lookups;

namespace maintenanceApplication.Controllers.lookups
{
    public class MainCusomerCityController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: MainCusomerCity
        public ActionResult Index()
        {
            return View(db.customerCity.ToList());
        }

        // GET: MainCusomerCity/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MaintenanceCustomerCityModel maintenanceCustomerCityModel = db.customerCity.Find(id);
            if (maintenanceCustomerCityModel == null)
            {
                return HttpNotFound();
            }
            return View(maintenanceCustomerCityModel);
        }

        // GET: MainCusomerCity/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MainCusomerCity/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CustomerCity")] MaintenanceCustomerCityModel maintenanceCustomerCityModel)
        {
            if (ModelState.IsValid)
            {
                db.customerCity.Add(maintenanceCustomerCityModel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(maintenanceCustomerCityModel);
        }

        // GET: MainCusomerCity/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MaintenanceCustomerCityModel maintenanceCustomerCityModel = db.customerCity.Find(id);
            if (maintenanceCustomerCityModel == null)
            {
                return HttpNotFound();
            }
            return View(maintenanceCustomerCityModel);
        }

        // POST: MainCusomerCity/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CustomerCity")] MaintenanceCustomerCityModel maintenanceCustomerCityModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(maintenanceCustomerCityModel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(maintenanceCustomerCityModel);
        }

        // GET: MainCusomerCity/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MaintenanceCustomerCityModel maintenanceCustomerCityModel = db.customerCity.Find(id);
            if (maintenanceCustomerCityModel == null)
            {
                return HttpNotFound();
            }
            return View(maintenanceCustomerCityModel);
        }

        // POST: MainCusomerCity/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MaintenanceCustomerCityModel maintenanceCustomerCityModel = db.customerCity.Find(id);
            db.customerCity.Remove(maintenanceCustomerCityModel);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        public ActionResult GoToAdminSettingHome()
        {
            return View("adminSeetings"); 
        }
    }
}
