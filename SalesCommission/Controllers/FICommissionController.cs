using System;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Linq;
using SalesCommission.Models;
using SalesCommission.Business;

namespace SalesCommission.Controllers
{
    public class FICommissionController : Controller
    {
        // GET: FICommission
        public ActionResult Index()
        {
            var FICommissionModel = new FICommissionModel();

            if (DateTime.Now < new DateTime(DateTime.Now.Year, DateTime.Now.Month, 6))
            {
                var previousMonth = DateTime.Now.AddMonths(-1);
                FICommissionModel.MonthId = previousMonth.Month;
                FICommissionModel.YearId = previousMonth.Year;
            }
            else
            {
                FICommissionModel.MonthId = DateTime.Now.Month;
                FICommissionModel.YearId = DateTime.Now.Year;
            }

            //FICommissionModel = SqlQueries.GetFIManagerDealsByDate(FICommissionModel);

            //var objectivesStandardsModel = new ObjectivesStandardsModel();
            //objectivesStandardsModel.MonthId = FICommissionModel.MonthId;
            //objectivesStandardsModel.YearId = FICommissionModel.YearId;
            //objectivesStandardsModel = SqlQueries.GetObjectivesAndStandardsByDate(objectivesStandardsModel);

            //FICommissionModel.ObjectivesAndStandards = objectivesStandardsModel.ObjectivesAndStandards;

            return View(FICommissionModel);
        }

        [HttpPost]
        public ActionResult Index(FICommissionModel FICommissionModel)
        {

            FICommissionModel = SqlQueries.GetFIManagerDealsByDate(FICommissionModel);

            var objectivesStandardsModel = new ObjectivesStandardsModel();
            objectivesStandardsModel.MonthId = FICommissionModel.MonthId;
            objectivesStandardsModel.YearId = FICommissionModel.YearId;
            objectivesStandardsModel = SqlQueries.GetObjectivesAndStandardsByDate(objectivesStandardsModel);

            FICommissionModel.ObjectivesAndStandards = objectivesStandardsModel.ObjectivesAndStandards;

            FICommissionModel.FIManagerDealDetails = MapFIManagerDeals(FICommissionModel);

            return View(FICommissionModel);
        }

        public List<FIManagerDealDetails> MapFIManagerDeals(FICommissionModel FICommissionModel)
        {
            var FIManagerDealMappings = new List<FIManagerDealDetails>();
            var otherAftermarketDealDetails = new List<AftermarketDealDetail>(FICommissionModel.AftermarketDealDetails);

            //otherAftermarketDealDetails = FICommissionModel.AftermarketDealDetails;

            var FIManagers = FICommissionModel.AftermarketDealDetails.Select(x => x.FandIManager).Distinct().ToList();

            var otherManagerDealDetails = new FIManagerDealDetails();
            otherManagerDealDetails.FIManagerName = "Other Managers";
            otherManagerDealDetails.FIManagerLastName = "ZZZZ";
            otherManagerDealDetails.FIManagerAssociateNumber = "000";
            otherManagerDealDetails.FIManagerHireDate = DateTime.Now;
            otherManagerDealDetails.FIManagerLocation = FICommissionModel.StoreId;
            otherManagerDealDetails.FIManagerSSN = FICommissionModel.StoreId + "000";
            otherManagerDealDetails.FIDepartmentCode = FICommissionModel.StoreId;
            otherManagerDealDetails.FIDepartmentDescription = FICommissionModel.StoreId;


            foreach (var manager in FIManagers)
            {
                var managerDealDetails = new FIManagerDealDetails();

                var FILastName = "";
                if (manager != null)
                {
                    if (manager.LastIndexOf(' ') > 0)
                    {
                        FILastName = manager.Substring(manager.LastIndexOf(' ')).Trim();
                    }
                    else
                    {
                        FILastName = manager;
                    }
                }
                var managerInformation = FICommissionModel.FIManagers.Find(x => x.AssociateLastName.ToLower().Contains(FILastName.ToLower()));
                var managerDeals = FICommissionModel.AftermarketDealDetails.FindAll(x => x.FandIManager == manager);

                if (managerInformation != null)
                {
                    otherAftermarketDealDetails.RemoveAll(x => x.FandIManager == manager);

                    managerDealDetails.FIManagerName = manager;
                    managerDealDetails.FIManagerLastName = FILastName;
                    managerDealDetails.FIManagerAssociateNumber = managerInformation.AssociateNumber;
                    managerDealDetails.FIManagerHireDate = managerInformation.AssociateHireDate;
                    managerDealDetails.FIManagerLocation = managerInformation.AssociateLocation;
                    managerDealDetails.FIManagerSSN = managerInformation.AssociateSSN;
                    managerDealDetails.FIDepartmentCode = managerInformation.AssociateDepartment;
                    managerDealDetails.FIDepartmentDescription = managerInformation.AssociateDepartmentDescription;

                    var aftermarketDealDetails = new List<AftermarketDealDetail>();
                    aftermarketDealDetails = managerDeals;

                    managerDealDetails.AftermarketDealDetails = aftermarketDealDetails;

                    FIManagerDealMappings.Add(managerDealDetails);
                }
            }


            otherManagerDealDetails.AftermarketDealDetails = otherAftermarketDealDetails;
            FIManagerDealMappings.Add(otherManagerDealDetails);

            var orderedFIManagerDealMappings = FIManagerDealMappings.OrderBy(x => x.FIManagerLastName).ToList();

            return orderedFIManagerDealMappings;

        }

        public ActionResult Payscales()
        {
            var payscaleModel = new FIPayscaleModel();

            payscaleModel.MonthId = DateTime.Now.Month;
            payscaleModel.YearId = DateTime.Now.Year;
            payscaleModel.FIPayscaleSelectList = SqlQueries.GetFIPayscaleSelectList().OrderBy(x=> x.Text).ToList();

            return View(payscaleModel);
        }

        [HttpPost]
        public ActionResult Payscales(FIPayscaleModel FIPayscaleModel)
        {
            var payscaleModel = new FIPayscaleModel();

            payscaleModel.MonthId = FIPayscaleModel.MonthId;
            payscaleModel.YearId = FIPayscaleModel.YearId;
            payscaleModel.PayscaleId = FIPayscaleModel.PayscaleId;
            payscaleModel.FIPayscaleSelectList = SqlQueries.GetFIPayscaleSelectList().OrderBy(x => x.Text).ToList();

            payscaleModel.FIPayscales = SqlQueries.GetFIPayscaleByIDAndDate(FIPayscaleModel.YearId, FIPayscaleModel.MonthId, FIPayscaleModel.PayscaleId);

            return View(payscaleModel);
        }

        public ActionResult Associate(string id, string monthId, string yearId)
        {

            //SetUserInformation();
            var associateCommissionModel = new FIAssociateCommissionModel();

            associateCommissionModel.AssociateId = id;
            associateCommissionModel.MonthId = Int32.Parse(monthId);
            associateCommissionModel.YearId = Int32.Parse(yearId);

            if (id != null && id != "")
            {
                associateCommissionModel.AssociateInformation = SqlQueries.GetFIAssociateInformationByDate(associateCommissionModel.AssociateId);

                // BUTCHERY AND HACKERY UNTIL WE CAN GET THE ACTUAL EMPLOYEE NUMBER FROM REYNOLDS
                var FICommissionModel = new FICommissionModel();
                FICommissionModel.MonthId = associateCommissionModel.MonthId;
                FICommissionModel.YearId = associateCommissionModel.YearId;
                FICommissionModel.StoreId = associateCommissionModel.AssociateInformation.AssociateLocation;

                FICommissionModel = SqlQueries.GetFIManagerDealsByDate(FICommissionModel);

                FICommissionModel.FIManagerDealDetails = MapFIManagerDeals(FICommissionModel);

                //END THE BUTCHERY AND HACKERY

                var FIManagerDetails = FICommissionModel.FIManagerDealDetails.Find(x => x.FIManagerAssociateNumber.Trim() == id);
                if (FIManagerDetails != null)
                { 
                    associateCommissionModel.AftermarketDealDetails = FIManagerDetails.AftermarketDealDetails;
                }

                var payscaleId = "";

                var payscaleList = SqlQueries.GetFIPayscaleSelectList();
                payscaleId = payscaleList.Find(x => x.Value.Contains(associateCommissionModel.AssociateInformation.AssociateLocation)).Value;

                associateCommissionModel.FIPayscales = SqlQueries.GetFIPayscaleByIDAndDate(associateCommissionModel.YearId, associateCommissionModel.MonthId, payscaleId);
            }

            ViewBag.IsCommissionAdmin = Session["IsCommissionAdmin"];

            return View(associateCommissionModel);
        }
        public ActionResult ScoreCard(string id, string monthId, string yearId)
        {
            return View();
        }

        public ActionResult ScoreCardDashboard()
        {
            return View();
        }
    }
}