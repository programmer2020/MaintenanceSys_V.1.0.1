﻿using maintenanceApplication.Models;
using maintenanceApplication.Models.Maintenance;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace maintenanceApplication.Controllers.Reports
{
    public class ReportController : Controller
    {
        private ApplicationDbContext _context;

        public ReportController()
        {
            _context = new ApplicationDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        [HttpGet]
        public ActionResult ReparingCheqView()
        {

            return View("ReparingCheq");
        }


        [HttpGet]
        public ActionResult ReparingCheq()
        {

            return View("PrintDeliverReport", new MaintenanceModel());
        }

        [HttpPost]
        public ActionResult ReparingCheq(int id)
        {
            try
            {
                if(id != 0)
                {
                    var maintenanceSatus = _context.status.Where(x => x.StatusName == "Delivered").FirstOrDefault();
                    int statusId = maintenanceSatus.Id;
                    var maintenance_requests = _context.maintenance.Where(x => x.Id == id).Where(x => x.MaintenanceStatusModelId == statusId).Include(x => x.priority).Include(x => x.status).Include(x => x.CustomerCity).FirstOrDefault();
                    
                    if(maintenance_requests != null)
                    {
                        return View("PrintDeliverReport", maintenance_requests);
                    }
                    else
                    {
                        return View("PrintDeliverReport" , new MaintenanceModel());
                    }
                }
                else
                {
                    return View("PrintDeliverReport", new MaintenanceModel());
                }

            }
            catch
            {
                return View("PrintDeliverReport", new MaintenanceModel());
            }

        }

    }
}