using maintenanceApplication.Models;
using maintenanceApplication.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Web.Configuration;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using maintenanceApplication.Models.Maintenance;
using maintenanceApplication.Models.Maintenance.lookups;
using System.Data.Entity.Migrations;
using System.Threading.Tasks;
using System.Runtime.Caching;

namespace maintenanceApplication.Controllers
{
    //[Authorize]
    //[AllowAnonymous]
    //[OutputCache (Duration =50 , Location =System.Web.UI.OutputCacheLocation.Server , VaryByParam ="*")]

    public class MaintenanceController : Controller
    {
        private ApplicationDbContext _context;
        
        public MaintenanceController()
        {
            _context = new ApplicationDbContext();
            var allComments = _context.maintenceComments.ToList();
            ViewData["commentList"] = allComments;
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        public ActionResult MAintenaceHome()    
        {
            return View("MaintenanceHome"); 
        }

        //Get All New Requests 
        public ActionResult GetNewMaintenanceRequests()
        {
            var getStatusId = _context.status.SingleOrDefault(x => x.StatusName == "New Request"); 
            var maintenance_requests = _context.maintenance.Where(x => x.MaintenanceStatusModelId == getStatusId.Id && x.isDeleted == false).OrderByDescending(x => x.Id).Include(x => x.priority).Include(x => x.status).ToList();
            ViewBag.noLayout = 1;
            if (User.IsInRole("SuperAdmin"))
            {
                return View("~/Views/Maintenance/SuperAdmin/GetMaintenanceRequests.cshtml", maintenance_requests);
            }
            else if (User.IsInRole("Admin"))
            {
                return View("~/Views/Maintenance/Admin/GetMaintenanceRequests.cshtml", maintenance_requests);
            }else if (User.IsInRole("Supervisor"))
            {
                return View("~/Views/Maintenance/Supervisor/GetMaintenanceRequests.cshtml", maintenance_requests);
            }else if (User.IsInRole("Technical"))
            {
                return View("~/Views/Maintenance/Technical/GetMaintenanceRequests.cshtml", maintenance_requests);
            }
            else
            {
                return View("~/Views/Maintenance/Guset/GetMaintenanceRequests.cshtml", maintenance_requests);
            }

        }


        public JsonResult GetAllMaintenanceRequests()
        {
            var maintenance_requests = _context.maintenance.Where(x=>x.isDeleted == false);
            return Json(maintenance_requests); 

        }


        //Approve Under checked Request
        public ActionResult Checked_Done(MaintenanceModel maintenance)
        {
            var maintenanceApproved = _context.maintenance.SingleOrDefault(x => x.Id == maintenance.Id);
            var maintenanceStatus = _context.status.SingleOrDefault(x => x.StatusName == "Approved");
            maintenanceApproved.MaintenanceStatusModelId = maintenanceStatus.Id;
            var Refunded_viewModel = new MaintenanceCreteViewModel
            {
                maintenance = maintenanceApproved,
            };

            maintenanceApproved.TechnicalReport = maintenance.TechnicalReport; 
            maintenanceApproved.price = maintenance.price; 
            maintenanceApproved.isAccessoriesAvailable = maintenance.isAccessoriesAvailable;
            TryUpdateModel(maintenanceApproved); 
            _context.SaveChanges();
           
            return RedirectToAction("GetNewMaintenanceRequests");
                
        }
        public ActionResult StartMaintenance(int id , MaintenanceModel maintenance)
        {
            var maintenanceApproved = _context.maintenance.SingleOrDefault(x => x.Id == id);
            var maintenanceStatus = _context.status.SingleOrDefault(x => x.StatusName == "Under Reparing");
            maintenanceApproved.MaintenanceStatusModelId = maintenanceStatus.Id;
            maintenanceApproved.StartReparingDate = DateTime.UtcNow;
            maintenanceApproved.TechnicalReport = maintenance.TechnicalReport;
            var Refunded_viewModel = new MaintenanceCreteViewModel
            {
                maintenance = maintenanceApproved,
            };
            ViewData["mainId"] = maintenanceApproved.Id;
            _context.maintenance.AddOrUpdate(maintenanceApproved);
            _context.SaveChanges();
            return RedirectToAction("GetNewMaintenanceRequests");

        }

        public ActionResult Error()
        {
            return View("Error"); 
        }

        //Get Approved Requets 
        public async Task<ActionResult> GetApprovedRequests()
        {
            var getStatuId = _context.status.FirstOrDefault(x => x.StatusName == "Approved");
            var approvedMaintenanceRequests = _context.maintenance.Where(x => x.MaintenanceStatusModelId == getStatuId.Id).Include(x => x.priority).Include(x => x.status).OrderByDescending(x => x.Customer_Name).ToList();
            await Task.Delay(0);
            return View("GetAllApprovedRequests", approvedMaintenanceRequests);
        }

        //Get Under Reparing Requests 
        public async Task<ActionResult> GetUnderReparingRequests()
        {
            var getStatuId = _context.status.FirstOrDefault(x => x.StatusName == "Under Reparing");
            var underReparingRequests = _context.maintenance.Where(x => x.MaintenanceStatusModelId == getStatuId.Id).Include(x => x.priority).Include(x => x.status).OrderByDescending(x => x.Customer_Name).ToList();
            await Task.Delay(0);
            return View("GetAllUnderReparingRequests", underReparingRequests);
        }


        //Get Under Checking Requets 
        public ActionResult GeCheckingStartedRequests()
        {
            var getStatuId = _context.status.FirstOrDefault(x => x.StatusName == "Under Checking");
            var checkingMaintenanceRequests = _context.maintenance.Where(x => x.MaintenanceStatusModelId == getStatuId.Id).Include(x => x.priority).Include(x => x.status).OrderByDescending(x => x.Id).ToList();
            return View("GetCheckingStartedRequestsView", checkingMaintenanceRequests); 
        }

        //Under Checking Requets Actions 
        public ActionResult GetCheckingStartedRequestsActions(int id)
        {
            var maintenanceUnderCheck = _context.maintenance.SingleOrDefault(x => x.Id == id);
            if (maintenanceUnderCheck == null)
                return HttpNotFound();
            var update_viewModel = new MaintenanceCreteViewModel
            {
                maintenance = maintenanceUnderCheck,
            };
            ViewData["mainId"] = maintenanceUnderCheck.Id;
            return View("CheckingStartedViewActions", update_viewModel);
        }

        [HttpPost]
        public ActionResult saveAsDraft(FormCollection form ,  int id)
        {
            var maintenanceStatus = _context.status.SingleOrDefault(x => x.StatusName == "Under Checking"); 
            var maintenance = _context.maintenance.SingleOrDefault(x => x.Id == id);
                maintenance.MaintenanceStatusModelId = maintenanceStatus.Id;

            maintenance.Recommendations = form["maintenance.Recommendations"].ToString();
            maintenance.TechnicalReport = form["maintenance.TechnicalReport"].ToString();
            //maintenance.Recommendations = maintenanceModel.Recommendations; 

            _context.maintenance.AddOrUpdate(maintenance);
            
            TryUpdateModel(maintenance);
            _context.SaveChanges();
            return RedirectToAction("GetNewMaintenanceRequests");
        }

        [HttpPost]
        public ActionResult Unrepairable(int id)
        {
            var maintenanceStart = _context.status.SingleOrDefault(x => x.StatusName == "To be Delivered");
            var maintenance = _context.maintenance.SingleOrDefault(x => x.Id == id);
            maintenance.MaintenanceStatusModelId = maintenanceStart.Id;
            maintenance.deliverReason = "Unrepairable";
            maintenance.Deliver_Date = DateTime.UtcNow;
            _context.maintenance.AddOrUpdate(maintenance);
            _context.SaveChanges();
            return RedirectToAction("GetNewMaintenanceRequests");
        }

        //Edit Maintenance Requests 
        [HttpGet]
        public ActionResult EditMaintenance(int id)
        {
            bool result = User.IsInRole("admin");
            var maintenanceEdit = _context.maintenance.Include(x=>x.user).SingleOrDefault(x => x.Id == id);
            if (maintenanceEdit == null)
                return HttpNotFound();
            var update_viewModel = new MaintenanceCreteViewModel
            {
                maintenance = maintenanceEdit,
                maintenancepriority = _context.priority.ToList(),
                maintenancestatus = _context.status.ToList() , 
                CustomerCity = _context.customerCity.ToList() , 
                //users = _context.user
            };
            ViewData["mainId"] = maintenanceEdit.Id;

            if (User.IsInRole("SuperAdmin"))
            {
                return View("~/Views/Maintenance/SuperAdmin/Maintenance_ViewData.cshtml", update_viewModel);
            }
            else if (User.IsInRole("Admin"))
            {
                return View("~/Views/Maintenance/Admin/Maintenance_ViewData.cshtml", update_viewModel);
            }
            else if (User.IsInRole("Supervisor"))
            {
                return View("~/Views/Maintenance/Supervisor/Maintenance_ViewData.cshtml", update_viewModel);
            }
            else if (User.IsInRole("Technical"))
            {
                return View("~/Views/Maintenance/Technical/Maintenance_ViewData.cshtml", update_viewModel);
            }
            else
            {
                return View("~/Views/Maintenance/Guset/Maintenance_ViewData.cshtml", update_viewModel);
            }

        }

        [HttpPost]
        public ActionResult EditMaintenance(MaintenanceModel maintenance)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var maintenanceView = new MaintenanceCreteViewModel
                    {
                        maintenance = maintenance,
                        maintenancepriority = _context.priority.ToList(),
                        maintenancestatus = _context.status.ToList()
                    };

                    return View("Maintenance_ViewData", maintenanceView);
                }
                else
                {
                    var maintenanceObj = _context.maintenance.Single(x => x.Id == maintenance.Id);
                    maintenanceObj.MaintenanceStatusModelId = maintenance.MaintenanceStatusModelId;

                    maintenanceObj.CustomerCity = maintenance.CustomerCity;
                    maintenanceObj.Customer_Name = maintenance.Customer_Name;
                    maintenanceObj.Customer_Phone_1 = maintenance.Customer_Phone_1;
                    maintenanceObj.Customer_Phone_2 = maintenance.Customer_Phone_2;
                    maintenanceObj.Customer_Adress = maintenance.Customer_Adress;
                    maintenanceObj.Device_SerialNumber = maintenance.Device_SerialNumber;
                    maintenanceObj.Device_Model = maintenance.Device_Model;
                    maintenanceObj.price = maintenance.price;
                    maintenanceObj.Recommendations = maintenance.Recommendations;
                    maintenanceObj.TechnicalReport = maintenance.TechnicalReport;
                    maintenanceObj.isAccessoriesAvailable = maintenance.isAccessoriesAvailable;
                    maintenanceObj.MaintenanceCustomerCityModelId = maintenance.MaintenanceCustomerCityModelId;

                    maintenanceObj.Device_Model = maintenance.Device_Model;
                    maintenanceObj.Device_SerialNumber = maintenance.Device_SerialNumber;
                    maintenanceObj.Accrssories = maintenance.Accrssories;
                    maintenanceObj.ClientRemarks = maintenance.ClientRemarks;
                    maintenanceObj.MaintenancePriorityModelId = maintenance.MaintenancePriorityModelId;

                    TryUpdateModel(maintenanceObj);
                    _context.SaveChanges();

                    return RedirectToAction("GetNewMaintenanceRequests");
                }

            }
            catch(Exception ex)
            {
                TempData["ex"] = ex;
                return RedirectToAction("Error");
            }
        }

        public JsonResult DeleteMaintenance(int mainId)
        {
            bool result = false;
            try
            {
                var maintenance = _context.maintenance.SingleOrDefault(x => x.Id == mainId);
                if (maintenance == null)
                    return new JsonResult();
                maintenance.isDeleted = true;
                _context.maintenance.AddOrUpdate(maintenance);
                _context.SaveChanges();
                result = true;
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                result = false;
                return Json(result, JsonRequestBehavior.AllowGet);
            }

        }

        //Edit Maintenance Requests 
        public ActionResult EditUnderCheckMaintenance(int id)
        {
            var maintenanceEdit = _context.maintenance.SingleOrDefault(x => x.Id == id);
            if (maintenanceEdit == null)
                return HttpNotFound();
            var update_viewModel = new MaintenanceCreteViewModel
            {
                maintenance = maintenanceEdit,
                maintenancepriority = _context.priority.ToList(),
                maintenancestatus = _context.status.ToList(),
                CustomerCity = _context.customerCity.ToList()
            };
            ViewData["mainId"] = maintenanceEdit.Id;
            return View("GetUnderCheckingRequestsView", update_viewModel);
        }

        // Client Accept Maintenance Requests Actions 
        public ActionResult ClientApproveMaintenanceRequest(int id)
        {
            var maintenanceEdit = _context.maintenance.Include(x=>x.user).SingleOrDefault(x => x.Id == id);
            if (maintenanceEdit == null)
                return HttpNotFound();
            var update_viewModel = new MaintenanceCreteViewModel
            {
                maintenance = maintenanceEdit,
                maintenancepriority = _context.priority.ToList(),
                maintenancestatus = _context.status.ToList(),
                CustomerCity = _context.customerCity.ToList()
            };
            ViewData["mainId"] = maintenanceEdit.Id;

            if (User.IsInRole("SuperAdmin"))
            {
                return View("~/Views/Maintenance/SuperAdmin/ClientApproveMaintenanceRequest.cshtml", update_viewModel);
            }
            else if (User.IsInRole("Admin"))
            {
                return View("~/Views/Maintenance/Admin/ClientApproveMaintenanceRequest.cshtml", update_viewModel);
            }
            else if (User.IsInRole("Supervisor"))
            {
                return View("~/Views/Maintenance/Supervisor/ClientApproveMaintenanceRequest.cshtml", update_viewModel);
            }
            else if (User.IsInRole("Technical"))
            {
                return View("~/Views/Maintenance/Technical/ClientApproveMaintenanceRequest.cshtml", update_viewModel);
            }
            else
            {
                return View("~/Views/Maintenance/Guset/ClientApproveMaintenanceRequest.cshtml", update_viewModel);
            }
        }

        //Under Reparing Acotions 
        public ActionResult UnderReparingActions(int id)
        {
            var maintenanceEdit = _context.maintenance.Include(x=>x.user).SingleOrDefault(x => x.Id == id);
            if (maintenanceEdit == null)
                return HttpNotFound();
            var update_viewModel = new MaintenanceCreteViewModel
            {
                maintenance = maintenanceEdit,
                maintenancepriority = _context.priority.ToList(),
                maintenancestatus = _context.status.ToList(),
                CustomerCity = _context.customerCity.ToList()
            };
            ViewData["mainId"] = maintenanceEdit.Id;

            if (User.IsInRole("SuperAdmin"))
            {
                return View("~/Views/Maintenance/SuperAdmin/UnderReparingActionsView.cshtml", update_viewModel);
            }
            else if (User.IsInRole("Admin"))
            {
                return View("~/Views/Maintenance/Admin/UnderReparingActionsView.cshtml", update_viewModel);
            }
            else if (User.IsInRole("Supervisor"))
            {
                return View("~/Views/Maintenance/Supervisor/UnderReparingActionsView.cshtml", update_viewModel);
            }
            else if (User.IsInRole("Technical"))
            {
                return View("~/Views/Maintenance/Technical/UnderReparingActionsView.cshtml", update_viewModel);
            }
            else
            {
                return View("~/Views/Maintenance/Guset/UnderReparingActionsView.cshtml", update_viewModel);
            }

        }

        [HttpPost]
        public ActionResult RepairCompleted(int id)
        {
            var maintenanceStart = _context.status.SingleOrDefault(x => x.StatusName == "Quality Check");
            var maintenance = _context.maintenance.SingleOrDefault(x => x.Id == id);
            maintenance.Actual_Repair_Date = DateTime.UtcNow;
            maintenance.MaintenanceStatusModelId = maintenanceStart.Id;
            _context.maintenance.AddOrUpdate(maintenance);
            _context.SaveChanges();
            return RedirectToAction("GetNewMaintenanceRequests");
        }

        [HttpPost]
        public ActionResult SaveDraft(int id)
        {
            var maintenanceStart = _context.status.SingleOrDefault(x => x.StatusName == "Under Reparing");
            var maintenance = _context.maintenance.SingleOrDefault(x => x.Id == id);
            maintenance.MaintenanceStatusModelId = maintenanceStart.Id;
            _context.maintenance.AddOrUpdate(maintenance);
            _context.SaveChanges();
            return RedirectToAction("GetNewMaintenanceRequests");
        }

        //Get Returned Requests
        public ActionResult GetAllReturnedRequets()
        {
            var UnderApprovalMaintenance = _context.maintenance.Where(x => x.status.StatusName == "Returned").Include(x => x.priority).Include(x => x.status).OrderByDescending(x => x.Customer_Name).ToList();
            return View("GetAllReturnedRequets", UnderApprovalMaintenance);
        }

        //Returned Request Details
        public ActionResult GetReturnedRequets(int id)
        {
            var UnderApprovalMaintenance = _context.maintenance.Where(x =>x.Id == id).Include(x => x.priority).Include(x => x.status).Include(x=>x.CustomerCity).SingleOrDefault();
            return View(UnderApprovalMaintenance);  
        }

        //Refund Maintenance Request
        [HttpPost]
        public ActionResult ReturnMaintenanceRequest(int id)
        {
            var maintenanceRefunded = _context.maintenance.SingleOrDefault(x => x.Id == id);
            var maintenanceStatus = _context.status.SingleOrDefault(x => x.StatusName == "Delivered");
            maintenanceRefunded.MaintenanceStatusModelId = maintenanceStatus.Id;
            maintenanceRefunded.deliverReason = "Returned";
            maintenanceRefunded.Deliver_Date = DateTime.UtcNow;
            var Refunded_viewModel = new MaintenanceCreteViewModel
            {
                maintenance = maintenanceRefunded,
            };

            _context.maintenance.AddOrUpdate(maintenanceRefunded);
            _context.SaveChanges();

            return RedirectToAction("GetNewMaintenanceRequests");
            //return RedirectToAction("GetAllReturnedRequets"); 
        }

        //Start Checking New Request
        public ActionResult Start_Checking(int id)
        {
            var maintenanceChecked = _context.maintenance.SingleOrDefault(x => x.Id == id);
            var maintenanceStatus = _context.status.SingleOrDefault(x => x.StatusName == "Under Checking");
            maintenanceChecked.MaintenanceStatusModelId = maintenanceStatus.Id;
            maintenanceChecked.StartCheckingDate = DateTime.UtcNow;
            var Refunded_viewModel = new MaintenanceCreteViewModel
            {
                maintenance = maintenanceChecked,
            };
            _context.maintenance.AddOrUpdate(maintenanceChecked);
            _context.SaveChanges();
            return RedirectToAction("GetNewMaintenanceRequests");
        }

        //Maintenance Checked Completed
        public ActionResult CheckCompleted (MaintenanceModel maintenance , int id)
        {
            
            var maintenanceObj = _context.maintenance.Single(x => x.Id == id);
            maintenanceObj.TechnicalReport = maintenance.TechnicalReport;
            maintenanceObj.Recommendations = maintenance.Recommendations;
            TryUpdateModel(maintenanceObj);

            var maintenanceChecked = _context.maintenance.SingleOrDefault(x => x.Id == id);
            var maintenanceStatus = _context.status.SingleOrDefault(x => x.StatusName == "Under Approval");
            maintenanceChecked.MaintenanceStatusModelId = maintenanceStatus.Id;
            maintenanceChecked.CheckCompleted_Date = DateTime.UtcNow;

            ViewData["mainId"] = id;
            _context.maintenance.AddOrUpdate(maintenanceChecked);
            _context.SaveChanges();
            return RedirectToAction("GetNewMaintenanceRequests");
        }

        //GoToUnderApproval Change Request Status
        public ActionResult GoToUnderApproval(int id)
        {
            var maintenanceChecked = _context.maintenance.SingleOrDefault(x => x.Id == id);
            var maintenanceStatus = _context.status.SingleOrDefault(x => x.StatusName == "Under Approval");

            maintenanceChecked.MaintenanceStatusModelId = maintenanceStatus.Id;

            var Refunded_viewModel = new MaintenanceCreteViewModel
            {
                maintenance = maintenanceChecked,
            };

            _context.maintenance.AddOrUpdate(maintenanceChecked);
            _context.SaveChanges();
            return RedirectToAction("GetNewMaintenanceRequests");
        }

        //Get All Under Approval
        public async Task<ActionResult> GetAllUnderApprovalRequests()
        {
            var getStatusId = _context.status.SingleOrDefault(x => x.StatusName == "Under Approval");
            var maintenance_requests = _context.maintenance.Where(x => x.MaintenanceStatusModelId == getStatusId.Id).OrderByDescending(x => x.Id).Include(x => x.priority).Include(x => x.status).ToList();
            await Task.Delay(0);
            return View("GetUnderApprovalView", maintenance_requests);
        }

        //Client Accept Maintenance Requests Actions 
        public ActionResult Maintennace_UnderApproval_Action(int id)
        {
            var maintenanceEdit = _context.maintenance.Include(x=>x.CustomerCity).Include(x=>x.user).SingleOrDefault(x => x.Id == id);
            if (maintenanceEdit == null)
                return HttpNotFound();
            var update_viewModel = new MaintenanceCreteViewModel
            {
                maintenance = maintenanceEdit,
                maintenancestatus = _context.status.ToList()
            };
            ViewData["mainId"] = maintenanceEdit.Id;

            if (User.IsInRole("SuperAdmin"))
            {
                return View("~/Views/Maintenance/SuperAdmin/MaintennaceUnderApprovalActions.cshtml", update_viewModel);
            }
            else if (User.IsInRole("Admin"))
            {
                return View("~/Views/Maintenance/Admin/MaintennaceUnderApprovalActions.cshtml", update_viewModel);
            }
            else if (User.IsInRole("Supervisor"))
            {
                return View("~/Views/Maintenance/Supervisor/MaintennaceUnderApprovalActions.cshtml", update_viewModel);
            }
            else if (User.IsInRole("Technical"))
            {
                return View("~/Views/Maintenance/Technical/MaintennaceUnderApprovalActions.cshtml", update_viewModel);
            }
            else
            {
                return View("~/Views/Maintenance/Guset/MaintennaceUnderApprovalActions.cshtml", update_viewModel);
            }
        }

        //ApproveMaintencneRequest
        public ActionResult Maintennace_Approve_Request_Action(int id)
        {
            @ViewData["mainId"] = id;
            var maintenanceChecked = _context.maintenance.SingleOrDefault(x => x.Id == id);
            var maintenanceStatus = _context.status.SingleOrDefault(x => x.StatusName == "Approved");
            maintenanceChecked.MaintenanceStatusModelId = maintenanceStatus.Id;
            maintenanceChecked.Approved_Date = DateTime.UtcNow;
            var Refunded_viewModel = new MaintenanceCreteViewModel
            {
                maintenance = maintenanceChecked,
            };
            _context.maintenance.AddOrUpdate(maintenanceChecked);
            _context.SaveChanges();
            return RedirectToAction("GetNewMaintenanceRequests");
        }

        //Reject MAintenance Request
        public ActionResult Maintennace_Rject_Request_Action(int id)
        {
            var maintenanceRjected = _context.maintenance.SingleOrDefault(x => x.Id == id);
            var maintenanceStatus = _context.status.SingleOrDefault(x => x.StatusName == "To be Delivered");
            maintenanceRjected.deliverReason = "Rejected"; 

            maintenanceRjected.MaintenanceStatusModelId = maintenanceStatus.Id;

            var Refunded_viewModel = new MaintenanceCreteViewModel
            {
                maintenance = maintenanceRjected,
            };

            _context.maintenance.AddOrUpdate(maintenanceRjected);
            _context.SaveChanges();
            return RedirectToAction("GetNewMaintenanceRequests");
        }

        //Get Maintetnce Request Details
        public ActionResult getMaintenanceDetails(int id)
        {
            MaintenanceModel maintenanceRequest = _context.maintenance.Include(x => x.priority).Include(x => x.status).SingleOrDefault(x => x.Id == id);
            if (maintenanceRequest == null)
            {
                return HttpNotFound();
            }
            return View("getMaintenanceDetailsById", maintenanceRequest);
        }

        //Get Maintetnce Under delivered Requests
        public ActionResult getMaintenanceUnderDeliverRequests()
        {

            var getStatusId = _context.status.SingleOrDefault(x => x.StatusName == "New Request");
            var maintenance_requests = _context.maintenance.Where(x => x.MaintenanceStatusModelId == getStatusId.Id).Include(x => x.priority).Include(x => x.status).OrderByDescending(x => x.Id).ToList();
            return View("GetMaintenanceRequests", maintenance_requests);
        }

        //Get Maintetnce Under Checking Requests
        public async Task<ActionResult> getMaintenanceCheckingStartedRequests()
        {
            var getStatusId = _context.status.SingleOrDefault(x => x.StatusName == "Under Checking");
            var maintenance_requests = _context.maintenance.Where(x => x.MaintenanceStatusModelId == getStatusId.Id).Include(x => x.priority).Include(x => x.status).OrderByDescending(x => x.Id).ToList();
            await Task.Delay(0);
            return  View("GetCheckingStartedRequests", maintenance_requests);
        }

        //Get Maintetnce Under Checking Requests Actions
        public ActionResult getMaintenanceCheckingStartedRequestsActions(int id)
            {
            var maintenanceUnderChecking = _context.maintenance.Include(x=>x.CustomerCity).Include(x=>x.user).SingleOrDefault(x => x.Id == id);
            if (maintenanceUnderChecking == null)
                return HttpNotFound();
            var underChecking_viewModel = new MaintenanceCreteViewModel
            {
                maintenance = maintenanceUnderChecking,
                maintenancestatus = _context.status.ToList()
            };
            ViewData["mainId"] = maintenanceUnderChecking.Id;

            if (User.IsInRole("SuperAdmin"))
            {
                return View("~/Views/Maintenance/SuperAdmin/Maintennace_CheckingStarted_ActionView.cshtml", underChecking_viewModel);
            }
            else if (User.IsInRole("Admin"))
            {
                return View("~/Views/Maintenance/Admin/Maintennace_CheckingStarted_ActionView.cshtml", underChecking_viewModel);
            }
            else if (User.IsInRole("Supervisor"))
            {
                return View("~/Views/Maintenance/Supervisor/Maintennace_CheckingStarted_ActionView.cshtml", underChecking_viewModel);
            }
            else if (User.IsInRole("Technical"))
            {
                return View("~/Views/Maintenance/Technical/Maintennace_CheckingStarted_ActionView.cshtml", underChecking_viewModel);
            }
            else
            {
                return View("~/Views/Maintenance/Guset/Maintennace_CheckingStarted_ActionView.cshtml", underChecking_viewModel);
            }

        }

        [HttpPost]
        public ActionResult updateUnderChecking(MaintenanceModel maintenance)
        {
            try
            {
                    var maintenanceObj = _context.maintenance.Single(x => x.Id == maintenance.Id);
                    maintenanceObj.MaintenanceStatusModelId = maintenance.MaintenanceStatusModelId;
                    maintenanceObj.TechnicalReport = maintenance.TechnicalReport;
                    maintenanceObj.Recommendations = maintenance.Recommendations;

                    TryUpdateModel(maintenanceObj);
                    _context.SaveChanges();
                    return RedirectToAction("GetNewMaintenanceRequests");
                

            }
            catch (Exception ex)
            {
                TempData["ex"] = ex;
                return RedirectToAction("Error");
            }
        }

        [HttpPost]
        public ActionResult updateUnderApproval(MaintenanceModel maintenance)
        {
            try
            {
                    var maintenanceObj = _context.maintenance.Single(x => x.Id == maintenance.Id);
                    maintenanceObj.MaintenanceStatusModelId = maintenance.MaintenanceStatusModelId;

                    TryUpdateModel(maintenanceObj);
                    _context.SaveChanges();
                    return RedirectToAction("GetNewMaintenanceRequests");
            }
            catch (Exception ex)
            {
                TempData["ex"] = ex;
                return RedirectToAction("Error");
            }
        }

        [HttpPost]
        public ActionResult updateApproved(MaintenanceModel maintenance)
        {
            try
            {
                var maintenanceObj = _context.maintenance.Single(x => x.Id == maintenance.Id);
                maintenanceObj.MaintenanceStatusModelId = maintenance.MaintenanceStatusModelId;

                TryUpdateModel(maintenanceObj);
                _context.SaveChanges();
                return RedirectToAction("GetNewMaintenanceRequests");
            }
            catch (Exception ex)
            {
                TempData["ex"] = ex;
                return RedirectToAction("Error");
            }
        }

        [HttpPost]
        public ActionResult updateUnderReparing(MaintenanceModel maintenance)
        {
            try
            {
                var maintenanceObj = _context.maintenance.Single(x => x.Id == maintenance.Id);
                maintenanceObj.MaintenanceStatusModelId = maintenance.MaintenanceStatusModelId;
                TryUpdateModel(maintenanceObj);
                _context.SaveChanges();
                return RedirectToAction("GetNewMaintenanceRequests");
            }
            catch (Exception ex)
            {
                TempData["ex"] = ex;
                return RedirectToAction("Error");
            }
        }

        public ActionResult updateQualityChecked(MaintenanceModel maintenance)
        {
            try
            {
                var maintenanceObj = _context.maintenance.Single(x => x.Id == maintenance.Id);
                maintenanceObj.MaintenanceStatusModelId = maintenance.MaintenanceStatusModelId;

                TryUpdateModel(maintenanceObj);
                _context.SaveChanges();
                return RedirectToAction("GetNewMaintenanceRequests");
            }
            catch (Exception ex)
            {
                TempData["ex"] = ex;
                return RedirectToAction("Error");
            }
        }

        public ActionResult updateToBeDelivred(MaintenanceModel maintenance)
        {
            try
            {
                var maintenanceObj = _context.maintenance.Single(x => x.Id == maintenance.Id);
                maintenanceObj.MaintenanceStatusModelId = maintenance.MaintenanceStatusModelId;
                TryUpdateModel(maintenanceObj);
                _context.SaveChanges();
                return RedirectToAction("GetNewMaintenanceRequests");
            }
            catch (Exception ex)
            {
                TempData["ex"] = ex;
                return RedirectToAction("Error");
            }
        }

        public ActionResult updateDelivred(MaintenanceModel maintenance)
        {
            try
            {
                var maintenanceObj = _context.maintenance.Single(x => x.Id == maintenance.Id);
                maintenanceObj.MaintenanceStatusModelId = maintenance.MaintenanceStatusModelId;

                TryUpdateModel(maintenanceObj);
                _context.SaveChanges();
                return RedirectToAction("GetNewMaintenanceRequests");
            }
            catch (Exception ex)
            {
                TempData["ex"] = ex;
                return RedirectToAction("Error");
            }
        }

        //Get Maintetnce  delivered Requests
        public async Task<ActionResult> getdeliveredRequests()
        {
            var getStatusId = _context.status.SingleOrDefault(x => x.StatusName == "Delivered");
            var maintenance_requests = _context.maintenance.Where(x => x.MaintenanceStatusModelId == getStatusId.Id).Include(x => x.priority).Include(x => x.status).OrderByDescending(x => x.Id).ToList();
            await Task.Delay(0);
            return View("GetAllDelivredMaintenanceRequestView", maintenance_requests);
        }

        // Client Accept Maintenance Requests Actions 
        public ActionResult Maintennace_Delivred_Action(int id)
        {
            var maintenanceDelivred = _context.maintenance.Include(x=>x.CustomerCity).Include(x=>x.user).SingleOrDefault(x => x.Id == id);
            if (maintenanceDelivred == null)
                return HttpNotFound();
            var update_viewModel = new MaintenanceCreteViewModel
            {
                maintenance = maintenanceDelivred,
                maintenancestatus = _context.status.ToList()
            };
            ViewData["mainId"] = maintenanceDelivred.Id;


            if (User.IsInRole("SuperAdmin"))
            {
                return View("~/Views/Maintenance/SuperAdmin/Maintennace_Delivred_ActionView.cshtml", update_viewModel);
            }
            else if (User.IsInRole("Admin"))
            {
                return View("~/Views/Maintenance/Admin/Maintennace_Delivred_ActionView.cshtml", update_viewModel);
            }
            else if (User.IsInRole("Supervisor"))
            {
                return View("~/Views/Maintenance/Supervisor/Maintennace_Delivred_ActionView.cshtml", update_viewModel);
            }
            else if (User.IsInRole("Technical"))
            {
                return View("~/Views/Maintenance/Technical/Maintennace_Delivred_ActionView.cshtml", update_viewModel);
            }
            else
            {
                return View("~/Views/Maintenance/Guset/Maintennace_Delivred_ActionView.cshtml", update_viewModel);
            }

        }

        [HttpPost]
        public ActionResult Maintennace_Delivred_Action_Done(int id)
        {
            var maintenanceChecked = _context.maintenance.SingleOrDefault(x => x.Id == id);
            var maintenanceStatus = _context.status.SingleOrDefault(x => x.StatusName == "Delivered");
            maintenanceChecked.MaintenanceStatusModelId = maintenanceStatus.Id;
            var Refunded_viewModel = new MaintenanceCreteViewModel
            {
                maintenance = maintenanceChecked,
            };
            _context.maintenance.AddOrUpdate(maintenanceChecked);
            _context.SaveChanges();
            return RedirectToAction("GetNewMaintenanceRequests");
        }

        //Create New Maintencne Request (Get)
        [HttpGet]
        public ActionResult CreateNewMaintenanceRequest()
        {
            MaintenanceModel maintenance = new MaintenanceModel(); 
            var maintenancePriorityList = _context.priority.ToList();
            var maintenanceStatusList = _context.status.ToList();
            var CustomerCity = _context.customerCity.ToList();

            var maintenanceview_Model = new MaintenanceCreteViewModel()
            {
                maintenancepriority = maintenancePriorityList,
                maintenancestatus = maintenanceStatusList,
                CustomerCity = CustomerCity , 
            };

            ViewBag.isCreate = true; 
            return View("MaintenanceForm", maintenanceview_Model);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult CreateNewMaintenanceRequest(MaintenanceModel maintenance)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("MaintenanceForm", new MaintenanceCreteViewModel());
                }

                MaintenanceStatusModel maintenanceStatus = new MaintenanceStatusModel();
                    MaintenancePriorityModel priority = new MaintenancePriorityModel();

                    priority = _context.priority.SingleOrDefault(x => x.PriorityName == "Normal");
                    maintenanceStatus = _context.status.SingleOrDefault(x => x.StatusName == "New Request");

                maintenance.CreationDate = DateTime.UtcNow;
                maintenance.MaintenanceStatusModelId = maintenanceStatus.Id;
                if (maintenance.MaintenancePriorityModelId == 0 || maintenance.MaintenancePriorityModelId == null)
                    maintenance.MaintenancePriorityModelId = priority.Id;
                if (maintenance.TechnicalReport == "" || maintenance.TechnicalReport == null)
                    maintenance.TechnicalReport = "No Technical Report Yet";
                if (maintenance.Recommendations == "" || maintenance.Recommendations == null)
                    maintenance.Recommendations = "No Recommendations Yet";
                maintenance.userName = User.Identity.Name;
                _context.maintenance.Add(maintenance);
                _context.SaveChanges();

                int mainId = maintenance.Id;
                var maintennaceCheck = _context.maintenance.SingleOrDefault(x => x.Id == mainId);
                string new_DeviceNumber = maintennaceCheck.Device_SerialNumber;

                var maintenanceCheckSerial = _context.maintenance.Where(x => x.Device_SerialNumber == new_DeviceNumber).ToList();

                if (maintenanceCheckSerial.Count > 1)
                {
                    maintennaceCheck.isRepeated = true;
                }

                _context.maintenance.AddOrUpdate(maintennaceCheck);
                _context.SaveChanges();

            }
            catch (Exception ex)
            {
                return View("Error");
            }
            return RedirectToAction("GetNewMaintenanceRequests", "Maintenance");
        }

        // Maintenance_Technical_Report 
        [HttpGet]
        public ActionResult Maintenance_Technical_Report()
        {
            var getStatusId = _context.status.SingleOrDefault(x => x.StatusName == "Maintenance Started");
            var maintenance_requests = _context.maintenance.Where(x => x.MaintenanceStatusModelId == getStatusId.Id).Include(x => x.priority).Include(x => x.status).OrderByDescending(x => x.Id).ToList();

            //return RedirectToAction("GetNewMaintenanceRequests", "Maintenance");

            return View("GetAllTechnicalReportMaintenanceRequests", maintenance_requests);
        }

        // Client Accept Maintenance Requests Actions 
        public ActionResult Maintennace_TechnicalReport_Action(int id)
        {
            var maintenanceEdit = _context.maintenance.SingleOrDefault(x => x.Id == id);
            if (maintenanceEdit == null)
                return HttpNotFound();
            var update_viewModel = new MaintenanceCreteViewModel
            {
                maintenance = maintenanceEdit,
            };
            ViewData["mainId"] = maintenanceEdit.Id;
            return View("MaintenanceTechnicalReportActionsView", update_viewModel);
        }

        [HttpPost]
        public ActionResult Maintenance_Technical_Report_Done(int id)
        {
            var maintenanceChecked = _context.maintenance.SingleOrDefault(x => x.Id == id);

            var maintenanceStatus = _context.status.SingleOrDefault(x => x.StatusName == "Quality Check");

            maintenanceChecked.MaintenanceStatusModelId = maintenanceStatus.Id;

            var Refunded_viewModel = new MaintenanceCreteViewModel
            {
                maintenance = maintenanceChecked,
            };

            _context.maintenance.AddOrUpdate(maintenanceChecked);
            _context.SaveChanges();
            return RedirectToAction("GetNewMaintenanceRequests");

        }

        // Maintenance_Quality_Check
        [HttpGet]
        public async Task<ActionResult> GetAllMaintenance_Quality_Check()
        {
            var getStatusId = _context.status.SingleOrDefault(x => x.StatusName == "Quality Check");
            var maintenance_requests = _context.maintenance.Where(x => x.MaintenanceStatusModelId == getStatusId.Id).Include(x => x.priority).Include(x => x.status).OrderByDescending(x => x.Id).ToList();
            await Task.Delay(0);
            return View("GetAllQualityMaintenanceRequestsView", maintenance_requests);
        }

        // Maintenance_Quality_Check
        [HttpGet]
        public ActionResult Maintenance_Quality_CheckActions(int id)
        {
            var maintenanceEdit = _context.maintenance.Include(x=>x.CustomerCity).Include(x=>x.user).SingleOrDefault(x => x.Id == id);
            if (maintenanceEdit == null)
                return HttpNotFound();
            var update_viewModel = new MaintenanceCreteViewModel
            {
                maintenance = maintenanceEdit,
                maintenancestatus = _context.status.ToList()
            };
            ViewData["mainId"] = maintenanceEdit.Id;

            if (User.IsInRole("SuperAdmin"))
            {
                return View("~/Views/Maintenance/SuperAdmin/MainteanceQualityCheck_ActionsView.cshtml", update_viewModel);
            }
            else if (User.IsInRole("Admin"))
            {
                return View("~/Views/Maintenance/Admin/MainteanceQualityCheck_ActionsView.cshtml", update_viewModel);
            }
            else if (User.IsInRole("Supervisor"))
            {
                return View("~/Views/Maintenance/Supervisor/MainteanceQualityCheck_ActionsView.cshtml", update_viewModel);
            }
            else if (User.IsInRole("Technical"))
            {
                return View("~/Views/Maintenance/Technical/MainteanceQualityCheck_ActionsView.cshtml", update_viewModel);
            }
            else
            {
                return View("~/Views/Maintenance/Guset/MainteanceQualityCheck_ActionsView.cshtml", update_viewModel);
            }

        }


        [HttpPost]
        public ActionResult ApproveMaintenanceQuality(int id)
        {
            var maintenanceChecked = _context.maintenance.SingleOrDefault(x => x.Id == id);
            var maintenanceStatus = _context.status.SingleOrDefault(x => x.StatusName == "To be Delivered");

            maintenanceChecked.MaintenanceStatusModelId = maintenanceStatus.Id;
            maintenanceChecked.QualityApprovedDate = DateTime.UtcNow;

            var Refunded_viewModel = new MaintenanceCreteViewModel
            {
                maintenance = maintenanceChecked,
            };
            _context.maintenance.AddOrUpdate(maintenanceChecked);
            _context.SaveChanges();
            //return RedirectToAction("GetAllMaintenance_Quality_Check");
            return RedirectToAction("GetNewMaintenanceRequests");
        }

        [HttpPost]
        public ActionResult DeliverMaintenanceRequest(int id)
        {
            var maintenanceChecked = _context.maintenance.SingleOrDefault(x => x.Id == id);
            var maintenanceStatus = _context.status.SingleOrDefault(x => x.StatusName == "Delivered");
            maintenanceChecked.MaintenanceStatusModelId = maintenanceStatus.Id;
            maintenanceChecked.Deliver_Date = DateTime.UtcNow;

            var Refunded_viewModel = new MaintenanceCreteViewModel
            {
                maintenance = maintenanceChecked,
            };

            _context.maintenance.AddOrUpdate(maintenanceChecked);
            _context.SaveChanges();
            //return RedirectToAction("GetAllMaintenance_Quality_Check");
            return RedirectToAction("GetNewMaintenanceRequests");
        }

        [HttpPost]
        public ActionResult MaintenanceRejectQuality(int id)
        {
            var maintenanceChecked = _context.maintenance.SingleOrDefault(x => x.Id == id);
            var maintenanceStatus = _context.status.SingleOrDefault(x => x.StatusName == "Under Reparing");
            maintenanceChecked.MaintenanceStatusModelId = maintenanceStatus.Id;

            var Refunded_viewModel = new MaintenanceCreteViewModel
            {
                maintenance = maintenanceChecked,
            };

            _context.maintenance.AddOrUpdate(maintenanceChecked);
            _context.SaveChanges();
            return RedirectToAction("GetNewMaintenanceRequests");
        }

        public async Task<ActionResult> GetAllToBedeliveredRequests()
        {
            var getMaintenanceStatus = _context.status.SingleOrDefault(x => x.StatusName == "To be Delivered");
            var getAlldeliveredRequests = _context.maintenance.Include(x=>x.status).Include(x=>x.priority).Where(x => x.MaintenanceStatusModelId == getMaintenanceStatus.Id).ToList();
            await Task.Delay(0);
            return View("GetAllToBeDelivredMaintenanceRequestsView", getAlldeliveredRequests);
        }
      
        public ActionResult MaintenanceToBeDeliveredActions(int id)
        {
            var maintenanceEdit = _context.maintenance.Include(x=>x.user).Include(x=>x.CustomerCity).SingleOrDefault(x => x.Id == id);
            if (maintenanceEdit == null)
                return HttpNotFound();
            var update_viewModel = new MaintenanceCreteViewModel
            {
                maintenance = maintenanceEdit,
                maintenancestatus = _context.status.ToList()
            };
            ViewData["mainId"] = maintenanceEdit.Id;


            if (User.IsInRole("SuperAdmin"))
            {
                return View("~/Views/Maintenance/SuperAdmin/MaintenanceToBeDelivredActionView.cshtml", update_viewModel);
            }
            else if (User.IsInRole("Admin"))
            {
                return View("~/Views/Maintenance/Admin/MaintenanceToBeDelivredActionView.cshtml", update_viewModel);
            }
            else if (User.IsInRole("Supervisor"))
            {
                return View("~/Views/Maintenance/Supervisor/MaintenanceToBeDelivredActionView.cshtml", update_viewModel);
            }
            else if (User.IsInRole("Technical"))
            {
                return View("~/Views/Maintenance/Technical/MaintenanceToBeDelivredActionView.cshtml", update_viewModel);
            }
            else
            {
                return View("~/Views/Maintenance/Guset/MaintenanceToBeDelivredActionView.cshtml", update_viewModel);
            }

        }

        public ActionResult MaintenanceToBeDeliveredDone(int id)
        {
            var maintenanceChecked = _context.maintenance.SingleOrDefault(x => x.Id == id);

            var maintenanceStatus = _context.status.SingleOrDefault(x => x.StatusName == "Delivered");

            maintenanceChecked.MaintenanceStatusModelId = maintenanceStatus.Id;

            var Refunded_viewModel = new MaintenanceCreteViewModel
            {
                maintenance = maintenanceChecked,
            };

            _context.maintenance.AddOrUpdate(maintenanceChecked);
            _context.SaveChanges();
            return RedirectToAction("GetNewMaintenanceRequests");

        }
        public ActionResult GetAlldeliveredRequests()
        {
            var getMaintenanceStatus = _context.status.SingleOrDefault(x => x.StatusName == "Delivered");
            var getAlldeliveredRequests = _context.maintenance.Where(x => x.MaintenanceStatusModelId == getMaintenanceStatus.Id).Include(x=>x.priority).ToList();
            return View("GetAllDeliveredMaintenanceRequestsView", getAlldeliveredRequests);
        }

        [HttpGet]
        public ActionResult GetAllMAintenanceComments(int id)
        {
            try
            {
                var allComments = _context.maintenceComments.Where(x => x.MaintenanceModelId == id).ToList();
                ViewData["count"] = allComments.Count();
                return View("GetAllMAintenanceComments", allComments);
            }
            catch
            {
                return View(); 
            }
          
        }

        [HttpGet]
        public ActionResult Add_Maintenance_Comment(int id )
        {
            MaintenanceCommentsModel Maintenance_Comment = new MaintenanceCommentsModel(); 
            var maintenanceComment = new MaintenanceCommentsModel
            {
                comment = Maintenance_Comment.comment, 
                user_Id = Maintenance_Comment.user_Id
            };
            @ViewData["mainId"] = id;

            return View("AddMaintenanceComment", maintenanceComment);
        }
        [HttpPost]
        public ActionResult Add_Maintenance_Comment(int id , MaintenanceCommentsModel maintenanceComment )
        {
            if(maintenanceComment.comment != null && maintenanceComment.comment != "")
            {
                MaintenanceCommentsModel addMaintenanceComment = new MaintenanceCommentsModel()
                {
                    comment = maintenanceComment.comment,
                    MaintenanceModelId = maintenanceComment.MaintenanceModelId = id,
                    comment_DateTme = DateTime.UtcNow,
            };

                _context.maintenceComments.Add(addMaintenanceComment);
                _context.SaveChanges();

                var maintenanceComment_user = _context.maintenceComments.SingleOrDefault(x => x.Id == addMaintenanceComment.Id);
                maintenanceComment_user.user_Id = User.Identity.Name;
                _context.maintenceComments.AddOrUpdate(maintenanceComment_user);

                //UpdateModel(maintenanceComment_user);
                _context.SaveChanges();

            }
            string url = this.Request.UrlReferrer.AbsolutePath;
            return Redirect(url); 

        }

        public ActionResult Returned_ToUnderApproval(int id)
        {
            var maintenaceStatus = _context.status.SingleOrDefault(x => x.StatusName == "Under Checking");
            var maintenance = _context.maintenance.SingleOrDefault(x => x.Id == id);
            maintenance.MaintenanceStatusModelId = maintenaceStatus.Id;
            _context.maintenance.AddOrUpdate(maintenance);
            _context.SaveChanges();

            return RedirectToAction("GetNewMaintenanceRequests");

        }


    }
}