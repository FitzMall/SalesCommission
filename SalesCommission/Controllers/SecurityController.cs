using SalesCommission.Models;
using SalesCommission.Business;
using System.Web.Mvc;
using System.Configuration;
using System.Collections.Specialized;
using System.Web;
using System;

namespace SalesCommission.Controllers
{
    public class SecurityController : Controller
    {

        public ActionResult Import()
        {
            var importModel = new ImportModel();
            return View(importModel);
        }

        [HttpPost]
        public ActionResult Import(ImportModel importModel)
        {
            SqlQueries.ExportSalesToCommission();
            return View(importModel);
        }

        public ActionResult Login(string id)
        {
            id = id.ToUpper();

            var userId = "";
            var userName = "";
            var associateId = "";
            var locationId = "";

            switch (id)
            {
                case "FOC":
                    userName = "ashert";
                    userName = "Thomas Asher";
                    associateId = "1135";
                    locationId = "FOC";
                    break;
                case "FMM":
                    userName = "Tolerj";
                    userName = "Joshua Toler";
                    associateId = "1075";
                    locationId = "FMM";
                    break;
                case "LFT":
                    userName = "Vahmed";
                    userName = "Vaqar Ahmed";
                    associateId = "1646";
                    locationId = "LFT";
                    break;
                case "LFO":
                    userName = "Drew";
                    userName = "Andrew Huon";
                    associateId = "1090";
                    locationId = "LFM";
                    break;
                case "FTO":
                    userName = "Gallaghera";
                    userName = "Alexa Gallagher";
                    associateId = "78138";
                    locationId = "FTN";
                    break;
                case "CJE":
                    userName = "Bowersw";
                    userName = "Wilhelm Bowers";
                    associateId = "3972";
                    locationId = "CJE";
                    break;
                case "FCG":
                    userName = "Fullera";
                    userName = "Andrew Fuller";
                    associateId = "150713";
                    locationId = "FAM";
                    break;
                case "FLP":
                    userName = "Ridgella";
                    userName = "Amii Ridgell";
                    associateId = "10193";
                    locationId = "FLP";
                    break;
                case "WMC":
                    userName = "Contrerasc";
                    userName = "Carlos Contreras";
                    associateId = "63614";
                    locationId = "WDC";
                    break;
                case "FBN":
                    userName = "Camachoj";
                    userName = "Jacob Camacho";
                    associateId = "9103";
                    locationId = "CDO";
                    break;
                case "FBS":
                    userName = "paynel";
                    userName = "Lawrence Payne";
                    associateId = "4619";
                    locationId = "FBS";
                    break;
                case "CDO":
                    userName = "Hardya";
                    userName = "Arthur Hardy";
                    associateId = "4068";
                    locationId = "CDO";
                    break;                    
                default:
                    userName = "Anonymous";
                    userName = "Anonymous User";
                    associateId = "007";
                    locationId = "JJF";
                    break;
            }

            Session.Add("AssociateId", associateId);
            Session.Add("LocationId", locationId);
            Session.Add("UserId", userId);
            Session.Add("UserName", userName);
            Session.Add("IsAdmin", false);
            Session.Add("IsCommissionAdmin", false);
            Session.Add("IsAssociateAdmin", false);
            Session.Add("CanSeeReporting", false);


            return RedirectToAction("Index", "Home");
        }

        // GET: Security
        public ActionResult Index()
        {

            var securityModel = new SecurityModel();
            securityModel.JJFUsers = SqlQueries.GetJJFUsers();

            return View(securityModel);
        }

        [HttpPost]
        public ActionResult Index(SecurityModel securityModel)
        {

            if (Request.Form != null && Request.Form["Save"] != null)
            {
                //Save the security settings
                var indexCount = Int32.Parse(Request.Form["indexValue"]);

                for (int i = 1; i < indexCount; i++)
                {
                    var userPermissions = new UserPersmissions();

                    userPermissions.UserId = securityModel.UserId;
                    userPermissions.Location = Request.Form["locationId-" + i];
                    userPermissions.SecurityKey = Request.Form["keyId-" + i];

                    if (Request.Form["admin-" + i] == "on")
                    {
                        userPermissions.IsAdmin = true;
                    }
                    else
                    { 
                        userPermissions.IsAdmin = false;
                    }

                    if (Request.Form["showroom-" + i] == "on")
                    {
                        userPermissions.CanShowroomValidateDeal = true;
                    }
                    else
                    {
                        userPermissions.CanShowroomValidateDeal = false;
                    }

                    if (Request.Form["showroom-update-" + i] == "on")
                    {
                        userPermissions.CanShowroomUpdateDeal = true;
                    }
                    else
                    {
                        userPermissions.CanShowroomUpdateDeal = false;
                    }

                    if (Request.Form["office-" + i] == "on")
                    {
                        userPermissions.CanOfficeValidateDeal = true;
                    }
                    else
                    {
                        userPermissions.CanOfficeValidateDeal = false;
                    }

                    if (Request.Form["delete-" + i] == "on")
                    {
                        userPermissions.CanDeleteDeal = true;
                    }
                    else
                    {
                        userPermissions.CanDeleteDeal = false;
                    }

                    if (Request.Form["objectives-" + i] == "on")
                    {
                        userPermissions.CanSetObjectivesStandards = true;
                    }
                    else
                    {
                        userPermissions.CanSetObjectivesStandards = false;
                    }

                    if (Request.Form["reports-" + i] == "on")
                    {
                        userPermissions.CanCreateReports = true;
                    }
                    else
                    {
                        userPermissions.CanCreateReports = false;
                    }

                    if (Request.Form["commission-" + i] == "on")
                    {
                        userPermissions.IsCommissionAdmin = true;
                    }
                    else
                    {
                        userPermissions.IsCommissionAdmin = false;
                    }

                    if (Request.Form["associates-" + i] == "on")
                    {
                        userPermissions.IsAssociateAdmin = true;
                    }
                    else
                    {
                        userPermissions.IsAssociateAdmin = false;
                    }
                    var success = SqlQueries.SaveUserPermissions(userPermissions);
                }
            }


            securityModel.JJFUsers = SqlQueries.GetJJFUsers();
            securityModel.UserPermissions = SqlQueries.GetUserPermissions(securityModel.UserId);

            return View(securityModel);
        }

        
    }
}