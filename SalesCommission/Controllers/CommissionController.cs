using System;
using System.Web.Mvc;
using SalesCommission.Models;
using SalesCommission.Business;
using System.Configuration;
using System.Web;
using System.Collections.Specialized;
using System.Linq;
using System.Collections.Generic;

namespace SalesCommission.Controllers
{
    public class CommissionController : Controller
    {
        // GET: Commission
        public ActionResult Index()
        {
            var commissionModel = new CommissionModel();

            if (DateTime.Now < new DateTime(DateTime.Now.Year, DateTime.Now.Month, 6))
            {
                var previousMonth = DateTime.Now.AddMonths(-1);
                commissionModel.MonthId = previousMonth.Month;
                commissionModel.YearId = previousMonth.Year;
            }
            else
            {
                commissionModel.MonthId = DateTime.Now.Month;
                commissionModel.YearId = DateTime.Now.Year;
            }

            ViewBag.IsCommissionAdmin = Session["IsCommissionAdmin"];

            return View(commissionModel);
        }

        [HttpPost]
        public ActionResult Index(CommissionModel commissionModel)
        {
            commissionModel.Associates = SqlQueries.GetAssociatesByStoreAndDate(commissionModel.StoreId, commissionModel.YearId, commissionModel.MonthId);
            commissionModel.ManufacturerSpiffs = SqlQueries.GetManufacturerSpiffs(commissionModel.YearId, commissionModel.MonthId);
            commissionModel.DealApprovals = SqlQueries.GetDealApprovalsByDate(commissionModel.YearId, commissionModel.MonthId);

            commissionModel.MoneyDue = SqlQueries.GetAllMoneyDue();
            commissionModel.TitleDue = SqlQueries.GetAllTitlesDue();

            ViewBag.IsCommissionAdmin = Session["IsCommissionAdmin"];
            
            if (commissionModel.YearId < 2019)
            {
                return View(commissionModel);
            }
            else
            {
                return View("NewIndex", commissionModel);
            }            
        }

        public ActionResult ScorecardDashboard()
        {
            var commissionModel = new CommissionModel();
            
            if (DateTime.Now < new DateTime(DateTime.Now.Year, DateTime.Now.Month, 6))
            {
                var previousMonth = DateTime.Now.AddMonths(-1);
                commissionModel.MonthId = previousMonth.Month;
                commissionModel.YearId = previousMonth.Year;
            }
            else
            {
                commissionModel.MonthId = DateTime.Now.Month;
                commissionModel.YearId = DateTime.Now.Year;
            }

            ViewBag.IsCommissionAdmin = Session["IsCommissionAdmin"];

            return View(commissionModel);
        }

        [HttpPost]
        public ActionResult ScorecardDashboard(CommissionModel commissionModel)
        {
            //commissionModel.Associates = SqlQueries.GetAssociateScorecardsByStoreAndDate(commissionModel.StoreId, commissionModel.YearId, commissionModel.MonthId);
            //commissionModel.DealApprovals = SqlQueries.GetDealApprovalsByDate(commissionModel.YearId, commissionModel.MonthId);

            //ViewBag.IsCommissionAdmin = Session["IsCommissionAdmin"];

            commissionModel.Associates = SqlQueries.GetAssociateScorecardsByStoreAndDate(commissionModel.StoreId, commissionModel.YearId, commissionModel.MonthId);

            var dealMonth = new DateTime(commissionModel.YearId, commissionModel.MonthId, 1);

            commissionModel.AssociateLeads = SqlMapperUtil.StoredProcWithParams<AssociateLead>("sp_CommissionGetAssociateLeadsByDate", new { StartDate = dealMonth, EndDate = dealMonth.AddMonths(1) }, "ReynoldsData");
            commissionModel.DealApprovals = SqlQueries.GetDealApprovalsByDate(commissionModel.YearId, commissionModel.MonthId);

            ViewBag.IsCommissionAdmin = Session["IsCommissionAdmin"];

            if (commissionModel.YearId < 2019)
            {
                return View(commissionModel);
            }
            else
            {
                return View("NewScorecardDashboard", commissionModel);
            }            
        }


        public ActionResult ManufacturerSpiff()
        {
            var manufacturerSpiffModel = new ManufacturerSpiffModel();

            manufacturerSpiffModel.MonthId = DateTime.Now.Month;
            manufacturerSpiffModel.YearId = DateTime.Now.Year;

            manufacturerSpiffModel.ManufacturerSpiffs = SqlQueries.GetManufacturerSpiffs(manufacturerSpiffModel.YearId, manufacturerSpiffModel.MonthId);

            return View(manufacturerSpiffModel);
        }
        [HttpPost]
        public ActionResult ManufacturerSpiff(ManufacturerSpiffModel manufacturerSpiffModel)
        {

            if (Request.Form["MonthId"] != null)
            {
                //The Submit button was clicked...
                manufacturerSpiffModel.ManufacturerSpiffs = SqlQueries.GetManufacturerSpiffs(manufacturerSpiffModel.YearId, manufacturerSpiffModel.MonthId);

                if (manufacturerSpiffModel.ManufacturerSpiffs.Count == 0)
                {
                    //Now, let's import from the previous month...
                    manufacturerSpiffModel = SqlQueries.UpdateManufacturerSpiffsFromPrevious(manufacturerSpiffModel);
                }
            }
            else if (Request.Form["hdn-MonthId"] != null)
            {
                //var dealershipInputsModel = new DealershipModel();

                manufacturerSpiffModel.MonthId = (Request.Form["hdn-MonthId"] != "") ? Int32.Parse(Request.Form["hdn-MonthId"]) : 0;
                manufacturerSpiffModel.YearId = (Request.Form["hdn-YearId"] != "") ? Int32.Parse(Request.Form["hdn-YearId"]) : 0;

                var inputIndex = (Request.Form["hdn-InputIndex"] != "") ? Int32.Parse(Request.Form["hdn-InputIndex"]) : 0;

                for (int i = 1; i < inputIndex; i++)
                {
                    var manufacturerSpiff = new ManufacturerSpiff();

                    manufacturerSpiff.SpiffKey = Convert.ToInt16(Request.Form["hdn-InputKey-" + i]);
                    manufacturerSpiff.MonthYear = manufacturerSpiffModel.MonthId.ToString() + "/" + manufacturerSpiffModel.YearId.ToString();

                    manufacturerSpiff.Manufacturer = Request.Form["Manufacturer-" + i].ToString();
                    manufacturerSpiff.SpiffPaid = Request.Form["SpiffPaid-" + i].ToString();

                    var success = SqlQueries.SaveManufacturerSpiffs(manufacturerSpiff);

                }

                manufacturerSpiffModel.ManufacturerSpiffs = SqlQueries.GetManufacturerSpiffs(manufacturerSpiffModel.YearId, manufacturerSpiffModel.MonthId);

            }

            return View(manufacturerSpiffModel);
        }


        public ActionResult Associate(string id, string monthId, string yearId)
        {

            if(Int32.Parse(yearId) > 2018)
            {
                return  RedirectToAction("NewAssociate", new { id = id, monthid = monthId, yearid = yearId });
            }

            //SetUserInformation();
            var associateCommissionModel = new AssociateCommissionModel();

            associateCommissionModel.AssociateId = id;
            associateCommissionModel.MonthId = Int32.Parse(monthId);
            associateCommissionModel.YearId = Int32.Parse(yearId);

            if (id != null && id != "")
            {
                associateCommissionModel.AssociateInformation = SqlQueries.GetAssociateInformationByDate(associateCommissionModel.AssociateId, associateCommissionModel.YearId, associateCommissionModel.MonthId);
                associateCommissionModel.ManufacturerSpiffs = SqlQueries.GetManufacturerSpiffs(associateCommissionModel.YearId, associateCommissionModel.MonthId);
                associateCommissionModel.DealApprovals = SqlQueries.GetDealApprovalsByDate(associateCommissionModel.YearId, associateCommissionModel.MonthId);
            }

            ViewBag.IsCommissionAdmin = Session["IsCommissionAdmin"];

            return View(associateCommissionModel);
        }

        public ActionResult AllAssociates(string id, string monthId, string yearId)
        {
            var commissionModel = new CommissionModel();
            commissionModel.StoreId = id;
            commissionModel.MonthId = Int32.Parse(monthId);
            commissionModel.YearId = Int32.Parse(yearId);

            //SetUserInformation();
            commissionModel.Associates = SqlQueries.GetAssociatesByStoreAndDate(commissionModel.StoreId, commissionModel.YearId, commissionModel.MonthId);
            commissionModel.ManufacturerSpiffs = SqlQueries.GetManufacturerSpiffs(commissionModel.YearId, commissionModel.MonthId);
            return View(commissionModel);
        }

        public ActionResult Scorecard(string id, string monthId, string yearId)
        {

            if (Int32.Parse(yearId) > 2018)
            {
                return RedirectToAction("NewScorecard", new { id = id, monthid = monthId, yearid = yearId });
            }

            //SetUserInformation();
            var associateCommissionModel = new AssociateCommissionModel();

            associateCommissionModel.AssociateId = id;
            associateCommissionModel.MonthId = Int32.Parse(monthId);
            associateCommissionModel.YearId = Int32.Parse(yearId);

            if (id != null && id != "")
            {
                associateCommissionModel.AssociateInformation = SqlQueries.GetAssociateScoreCardByDate(associateCommissionModel.AssociateId, associateCommissionModel.YearId, associateCommissionModel.MonthId);
                associateCommissionModel.ManufacturerSpiffs = SqlQueries.GetManufacturerSpiffs(associateCommissionModel.YearId, associateCommissionModel.MonthId);
                associateCommissionModel.AssociateScorecardHistory = SqlQueries.GetAssociateScoreCardHistoryByDate(associateCommissionModel.AssociateInformation.AssociateSSN, associateCommissionModel.YearId, associateCommissionModel.MonthId);

                if (associateCommissionModel.AssociateScorecardHistory != null && associateCommissionModel.AssociateScorecardHistory.Count > 0)
                {
                    associateCommissionModel.CurrentScorecard = associateCommissionModel.AssociateScorecardHistory[0];
                }
                else
                {
                    associateCommissionModel.CurrentScorecard = new AssociateScoreCard();
                }

                var previousScorecards = new List<AssociateScoreCard>();

                for(int index = 1; index < 4; index++)
                {
                    var previousMonth = index * -1;
                    var currentDate = new DateTime(associateCommissionModel.YearId, associateCommissionModel.MonthId, 1);

                    var reportDate = currentDate.AddMonths(previousMonth);

                    var previousScorecard = SqlQueries.GetAssociateScoreCardHistoryByDate(associateCommissionModel.AssociateInformation.AssociateSSN, reportDate.Year, reportDate.Month );

                    if (previousScorecard != null && previousScorecard.Count > 0)
                    {
                        previousScorecards.Add(previousScorecard[0]);
                    }
                    else
                    {
                        previousScorecards.Add(new AssociateScoreCard());
                    }

                }

                associateCommissionModel.PreviousAssociateScorecards = previousScorecards;

                var objectivesStandardsModel = new ObjectivesStandardsModel();
                objectivesStandardsModel.YearId = associateCommissionModel.YearId;
                objectivesStandardsModel.MonthId = associateCommissionModel.MonthId;

                objectivesStandardsModel = SqlQueries.GetObjectivesAndStandardsByDate(objectivesStandardsModel);

                associateCommissionModel.StoreObjectivesStandards = objectivesStandardsModel.ObjectivesAndStandards.FindAll(x => x.StoreId == associateCommissionModel.AssociateInformation.AssociateLocation);
            }

            associateCommissionModel.AssociateList = SqlQueries.GetAssociateListForScorecard(associateCommissionModel.AssociateInformation.AssociateLocation, associateCommissionModel.YearId, associateCommissionModel.MonthId);

            ViewBag.IsCommissionAdmin = Session["IsCommissionAdmin"];

            return View(associateCommissionModel);
        }

        [HttpPost]
        public ActionResult Scorecard(AssociateCommissionModel associateCommissionModel)
        {
            //SetUserInformation();            
            
            var associateId = "";
            var monthId = 0;
            var yearId = 0;

            if (Request.Form["hdn-MonthId"] != null && Request.Form["hdn-MonthId"].Contains(","))
            {
                var cleanedMonthId = Request.Form["hdn-MonthId"].Substring(Request.Form["hdn-MonthId"].IndexOf(',') + 1);
                monthId = (cleanedMonthId != "") ? Int32.Parse(cleanedMonthId) : 0;
            }
            else
            {
                monthId = (Request.Form["hdn-MonthId"] != null && Request.Form["hdn-MonthId"] != "") ? Int32.Parse(Request.Form["hdn-MonthId"]) : 0;
            }

            if (Request.Form["hdn-YearId"] != null && Request.Form["hdn-YearId"].Contains(","))
            {
                var cleanedYearId = Request.Form["hdn-YearId"].Substring(Request.Form["hdn-YearId"].IndexOf(',') + 1);
                yearId = (cleanedYearId != "") ? Int32.Parse(cleanedYearId) : 0;
            }
            else
            {
                yearId = (Request.Form["hdn-YearId"] != null && Request.Form["hdn-YearId"] != "") ? Int32.Parse(Request.Form["hdn-YearId"]) : 0;
            }

            associateId = (Request.Form["associate-id"] != null && Request.Form["associate-id"] != "") ? Request.Form["associate-id"].ToString() : "";

            // NOW GET ALL THE INFORMATION FROM THE REQUEST...CHECK TO SEE IF SAVE OR FINALIZE WAS PUSHED
            if (Request.Form["SaveComments"] != null || Request.Form["FinalizeScorecard"] != null || Request.Form["ApproveScorecard"] != null)
            {
                var associateScoreCard = new AssociateScoreCard();

                associateScoreCard.AssociateSSN = (Request.Form["associate-SSN"] != null && Request.Form["associate-SSN"] != "") ? Request.Form["associate-SSN"].ToString() : "";
                associateScoreCard.MonthYear = monthId.ToString() + "/" + yearId.ToString();
                associateScoreCard.Rolling3MonthComments = (Request.Form["comments-rollingunits"] != null && Request.Form["comments-rollingunits"] != "") ? Request.Form["comments-rollingunits"].ToString() : ""; ;
                associateScoreCard.Rolling3MonthActual = 0;
                associateScoreCard.Rolling3MonthCount = 0;
                associateScoreCard.DeliveriesComments = (Request.Form["comments-units"] != null && Request.Form["comments-units"] != "") ? Request.Form["comments-units"].ToString() : ""; ;
                associateScoreCard.DeliveriesActual = 0;
                associateScoreCard.DeliveriesCount = 0;
                associateScoreCard.BPPComments = (Request.Form["comments-bpp"] != null && Request.Form["comments-bpp"] != "") ? Request.Form["comments-bpp"].ToString() : ""; ;
                associateScoreCard.BPPActual = 0;
                associateScoreCard.BPPCount = 0;
                associateScoreCard.VSCComments = (Request.Form["comments-service"] != null && Request.Form["comments-service"] != "") ? Request.Form["comments-service"].ToString() : ""; ;
                associateScoreCard.VSCActual = 0;
                associateScoreCard.VSCCount = 0;
                associateScoreCard.TradeComments = (Request.Form["comments-trade"] != null && Request.Form["comments-trade"] != "") ? Request.Form["comments-trade"].ToString() : ""; ;
                associateScoreCard.TradeActual = 0;
                associateScoreCard.TradeCount = 0;
                associateScoreCard.FinanceComments = (Request.Form["comments-finance"] != null && Request.Form["comments-finance"] != "") ? Request.Form["comments-finance"].ToString() : ""; ;
                associateScoreCard.FinanceActual = 0;
                associateScoreCard.FinanceCount = 0;
                associateScoreCard.LeaseComments = (Request.Form["comments-lease"] != null && Request.Form["comments-lease"] != "") ? Request.Form["comments-lease"].ToString() : ""; ;
                associateScoreCard.LeaseActual = 0;
                associateScoreCard.LeaseCount = 0;
                associateScoreCard.AftermarketComments = (Request.Form["comments-aftermarket"] != null && Request.Form["comments-aftermarket"] != "") ? Request.Form["comments-aftermarket"].ToString() : ""; ;
                associateScoreCard.AftermarketActual = 0;
                associateScoreCard.AftermarketCount = 0;
                associateScoreCard.SurveyComments = (Request.Form["comments-ad-survey"] != null && Request.Form["comments-ad-survey"] != "") ? Request.Form["comments-ad-survey"].ToString() : ""; ;
                associateScoreCard.SurveyActual = 0;
                associateScoreCard.SurveyCount = 0;
                associateScoreCard.SatisfactionActual = (Request.Form["satisfaction-actual"] != null && Request.Form["satisfaction-actual"] != "") ? Request.Form["satisfaction-actual"].ToString() : ""; 
                associateScoreCard.SatisfactionObjective = (Request.Form["satisfaction-objective"] != null && Request.Form["satisfaction-objective"] != "") ? Request.Form["satisfaction-objective"].ToString() : ""; 
                associateScoreCard.SatisfactionComments = (Request.Form["comments-satisfaction"] != null && Request.Form["comments-satisfaction"] != "") ? Request.Form["comments-satisfaction"].ToString() : ""; 
                associateScoreCard.TrainingComments = (Request.Form["comments-training"] != null && Request.Form["comments-training"] != "") ? Request.Form["comments-training"].ToString() : ""; 
                associateScoreCard.FandIProcessComments = (Request.Form["comments-fandi"] != null && Request.Form["comments-fandi"] != "") ? Request.Form["comments-fandi"].ToString() : ""; 
                associateScoreCard.FitzwayProcessComments = (Request.Form["comments-fitzway"] != null && Request.Form["comments-fitzway"] != "") ? Request.Form["comments-fitzway"].ToString() : "";
                associateScoreCard.OverrideComments = (Request.Form["comments-override"] != null && Request.Form["comments-override"] != "") ? Request.Form["comments-override"].ToString() : "";
                associateScoreCard.CalculatedLevel = (Request.Form["calculated-level"] != null && Request.Form["calculated-level"] != "") ? Request.Form["calculated-level"].ToString() : ""; 
                associateScoreCard.ApprovedLevel = (Request.Form["certification-level"] != null && Request.Form["certification-level"] != "") ? Request.Form["certification-level"].ToString() : ""; 
                associateScoreCard.UpdateDate = DateTime.Now;
                associateScoreCard.UpdateUser = Session["UserName"].ToString();

                associateScoreCard.SatisfactionActual3Month = (Request.Form["satisfaction-actual-3month"] != null && Request.Form["satisfaction-actual-3month"] != "") ? Request.Form["satisfaction-actual-3month"].ToString() : "";
                associateScoreCard.SatisfactionObjective3Month = (Request.Form["satisfaction-objective-3month"] != null && Request.Form["satisfaction-objective-3month"] != "") ? Request.Form["satisfaction-objective-3month"].ToString() : "";

                if(Request.Form["yesnoManufactuer"] != null)
                {
                    if(Request.Form["yesnoManufactuer"].ToString().ToLower() == "yes")
                    {
                        associateScoreCard.TrainingYN = "1";
                    }
                    else
                    {
                        associateScoreCard.TrainingYN = "0";
                    }
                }
                else
                {
                    associateScoreCard.TrainingYN = "0";
                }

                if (Request.Form["yesnoFI"] != null)
                {
                    if (Request.Form["yesnoFI"].ToString().ToLower() == "yes")
                    {
                        associateScoreCard.FandIProcessYN = "1";
                    }
                    else
                    {
                        associateScoreCard.FandIProcessYN = "0";
                    }
                }
                else
                {
                    associateScoreCard.FandIProcessYN = "0";
                }

                if (Request.Form["yesnoFitzway"] != null)
                {
                    if (Request.Form["yesnoFitzway"].ToString().ToLower() == "yes")
                    {
                        associateScoreCard.FitzwayProcessYN = "1";
                    }
                    else
                    {
                        associateScoreCard.FitzwayProcessYN = "0";
                    }
                }
                else
                {
                    associateScoreCard.FitzwayProcessYN = "0";
                }

                if (Request.Form["chk-MeetsSSI"] != null)
                {
                    if (Request.Form["chk-MeetsSSI"].ToString().ToLower() == "on")
                    {
                        associateScoreCard.MeetsSSIObjective = "1";
                    }
                    else
                    {
                        associateScoreCard.MeetsSSIObjective = "0";
                    }
                }
                else
                {
                    associateScoreCard.MeetsSSIObjective = "0";
                }

                if (Request.Form["FinalizeScorecard"] != null)
                {
                    associateScoreCard.FinalizeDate = DateTime.Now;
                    associateScoreCard.FinalizeUser = Session["UserName"].ToString();

                    // NEED TO UPDATE THE ASSOCIATES CERTIFICATION LEVEL
                    var certificationLevel = "";

                    if(associateScoreCard.ApprovedLevel == "standard")
                    {
                        certificationLevel = "STD";
                    }
                    else if (associateScoreCard.ApprovedLevel == "certified")
                    {
                        certificationLevel = "CERT";
                    }
                    else if (associateScoreCard.ApprovedLevel == "elite")
                    {
                        certificationLevel = "CERTELIT";
                    }

                    var updateLevel = SqlQueries.UpdateAssociatLevel(associateScoreCard.AssociateSSN, associateScoreCard.MonthYear, certificationLevel);
                }
                else
                {
                    associateScoreCard.FinalizeDate = new DateTime(1900,1,1);
                    associateScoreCard.FinalizeUser = "";
                }

                if (Request.Form["ApproveScorecard"] != null)
                {
                    associateScoreCard.ApprovalDate = DateTime.Now;
                    associateScoreCard.ApprovalUser = Session["UserName"].ToString();

                    // NEED TO UPDATE THE ASSOCIATES CERTIFICATION LEVEL
                    var certificationLevel = "";

                    if (associateScoreCard.ApprovedLevel == "standard")
                    {
                        certificationLevel = "STD";
                    }
                    else if (associateScoreCard.ApprovedLevel == "certified")
                    {
                        certificationLevel = "CERT";
                    }
                    else if (associateScoreCard.ApprovedLevel == "elite")
                    {
                        certificationLevel = "CERTELIT";
                    }

                    var updateLevel = SqlQueries.UpdateAssociatLevel(associateScoreCard.AssociateSSN, associateScoreCard.MonthYear, certificationLevel);
                }
                else
                {
                    associateScoreCard.ApprovalDate = new DateTime(1900, 1, 1);
                    associateScoreCard.ApprovalUser = "";
                }

                var done = SqlQueries.SaveAssociateScoreCardHistory(associateScoreCard);
            }

            if (Request.Form["SaveGoals"] != null)
            {

                var associateGoal = new Goal();

                associateGoal.AssociateSSN = (Request.Form["associate-SSN"] != null && Request.Form["associate-SSN"] != "") ? Request.Form["associate-SSN"].ToString() : "";
                associateGoal.MonthYear = monthId + "/" + yearId;
                associateGoal.UpdateUser = Session["UserName"].ToString();
                associateGoal.UpdateDate = DateTime.Now;

                associateGoal.DealCount = (Request.Form["dealCount"] != null && Request.Form["dealCount"] != "") ? Int32.Parse(Request.Form["dealCount"]) : 0;
                associateGoal.NewCount = (Request.Form["newCount"] != null && Request.Form["newCount"] != "") ? Int32.Parse(Request.Form["newCount"]) : 0;
                associateGoal.UsedCount = (Request.Form["usedCount"] != null && Request.Form["usedCount"] != "") ? Int32.Parse(Request.Form["usedCount"]) : 0;
                associateGoal.BPPCount = (Request.Form["bppCount"] != null && Request.Form["bppCount"] != "") ? Int32.Parse(Request.Form["bppCount"]) : 0;
                associateGoal.TradeCount = (Request.Form["tradeCount"] != null && Request.Form["tradeCount"] != "") ? Int32.Parse(Request.Form["tradeCount"]) : 0;
                associateGoal.FinanceCount = (Request.Form["financeCount"] != null && Request.Form["financeCount"] != "") ? Int32.Parse(Request.Form["financeCount"]) : 0;
                associateGoal.ServiceCount = (Request.Form["serviceCount"] != null && Request.Form["serviceCount"] != "") ? Int32.Parse(Request.Form["serviceCount"]) : 0;
                associateGoal.GAPCount = (Request.Form["gapCount"] != null && Request.Form["gapCount"] != "") ? Int32.Parse(Request.Form["gapCount"]) : 0;
                associateGoal.ZurichCount = (Request.Form["zurichCount"] != null && Request.Form["zurichCount"] != "") ? Int32.Parse(Request.Form["zurichCount"]) : 0;
                associateGoal.AftermarketCount = (Request.Form["aftermarketCount"] != null && Request.Form["aftermarketCount"] != "") ? Int32.Parse(Request.Form["aftermarketCount"]) : 0;
                associateGoal.SpiffCount = (Request.Form["spiffCount"] != null && Request.Form["spiffCount"] != "") ? Int32.Parse(Request.Form["spiffCount"]) : 0;

                var success = SqlQueries.SaveAssociateGoals(associateGoal);

            }

            associateCommissionModel.AssociateId = associateId;
            associateCommissionModel.MonthId = monthId;
            associateCommissionModel.YearId = yearId;

            if (associateId != null && associateId != "")
            {
                associateCommissionModel.AssociateInformation = SqlQueries.GetAssociateScoreCardByDate(associateCommissionModel.AssociateId, associateCommissionModel.YearId, associateCommissionModel.MonthId);
                associateCommissionModel.ManufacturerSpiffs = SqlQueries.GetManufacturerSpiffs(associateCommissionModel.YearId, associateCommissionModel.MonthId);
                associateCommissionModel.AssociateScorecardHistory = SqlQueries.GetAssociateScoreCardHistoryByDate(associateCommissionModel.AssociateInformation.AssociateSSN, associateCommissionModel.YearId, associateCommissionModel.MonthId);
                
                if(associateCommissionModel.AssociateScorecardHistory != null && associateCommissionModel.AssociateScorecardHistory.Count > 0)
                {
                    associateCommissionModel.CurrentScorecard = associateCommissionModel.AssociateScorecardHistory[0];
                }
                else
                {
                    associateCommissionModel.CurrentScorecard = new AssociateScoreCard();
                }

                var previousScorecards = new List<AssociateScoreCard>();

                for (int index = 1; index < 4; index++)
                {
                    var previousMonth = index * -1;
                    var currentDate = new DateTime(associateCommissionModel.YearId, associateCommissionModel.MonthId, 1);

                    var reportDate = currentDate.AddMonths(previousMonth);

                    var previousScorecard = SqlQueries.GetAssociateScoreCardHistoryByDate(associateCommissionModel.AssociateInformation.AssociateSSN, reportDate.Year, reportDate.Month);

                    if (previousScorecard != null && previousScorecard.Count > 0)
                    {
                        previousScorecards.Add(previousScorecard[0]);
                    }
                    else
                    {
                        previousScorecards.Add(new AssociateScoreCard());
                    }

                }

                associateCommissionModel.PreviousAssociateScorecards = previousScorecards;


            }

            associateCommissionModel.AssociateList = SqlQueries.GetAssociateListForScorecard(associateCommissionModel.AssociateInformation.AssociateLocation, associateCommissionModel.YearId, associateCommissionModel.MonthId);

            ViewBag.IsCommissionAdmin = Session["IsCommissionAdmin"];

            return View(associateCommissionModel);
        }

        public ActionResult BonusAndDraws(string id, string monthId, string yearId)
        {
            //SetUserInformation();
            var associateCommissionModel = new AssociateCommissionModel();

            associateCommissionModel.AssociateId = id;
            associateCommissionModel.MonthId = Int32.Parse(monthId);
            associateCommissionModel.YearId = Int32.Parse(yearId);

            if (id != null && id != "")
            {
                associateCommissionModel.AssociateInformation = SqlQueries.GetAssociateInformationDrawsAndBonus(associateCommissionModel.AssociateId, associateCommissionModel.YearId, associateCommissionModel.MonthId);
            }

            ViewBag.IsCommissionAdmin = Session["IsCommissionAdmin"];

            return View(associateCommissionModel);
        }

        [HttpPost]
        public ActionResult BonusAndDraws()
        {

            var monthId = 0;
            var yearId = 0;
            var locationId = "";
            var associateSSN = "";

            if (Request.Form["hdn-MonthId"] != null && Request.Form["hdn-MonthId"].Contains(","))
            {
                var cleanedMonthId = Request.Form["hdn-MonthId"].Substring(Request.Form["hdn-MonthId"].IndexOf(',') + 1);
                monthId = (cleanedMonthId != "") ? Int32.Parse(cleanedMonthId) : 0;
            }
            else
            {
                monthId = (Request.Form["hdn-MonthId"] != null && Request.Form["hdn-MonthId"] != "") ? Int32.Parse(Request.Form["hdn-MonthId"]) : 0;
            }

            if (Request.Form["hdn-YearId"] != null && Request.Form["hdn-YearId"].Contains(","))
            {
                var cleanedYearId = Request.Form["hdn-YearId"].Substring(Request.Form["hdn-YearId"].IndexOf(',') + 1);
                yearId = (cleanedYearId != "") ? Int32.Parse(cleanedYearId) : 0;
            }
            else
            {
                yearId = (Request.Form["hdn-YearId"] != null && Request.Form["hdn-YearId"] != "") ? Int32.Parse(Request.Form["hdn-YearId"]) : 0;
            }

            if (Request.Form["hdn-locationId"] != null && Request.Form["hdn-locationId"].Contains(","))
            {
                var cleanedLocationId = Request.Form["hdn-locationId"].Substring(Request.Form["hdn-locationId"].IndexOf(',') + 1);
                locationId = (cleanedLocationId != "") ? cleanedLocationId.ToString() : "";
            }
            else
            {
                locationId = (Request.Form["hdn-locationId"] != null && Request.Form["hdn-locationId"] != "") ? Request.Form["hdn-locationId"].ToString() : "";
            }

            if (Request.Form["hdn-SSN"] != null && Request.Form["hdn-SSN"].Contains(","))
            {
                var cleanedassociateSSN = Request.Form["hdn-SSN"].Substring(Request.Form["hdn-SSN"].IndexOf(',') + 1);
                associateSSN = (cleanedassociateSSN != "") ? cleanedassociateSSN.ToString() : "";
            }
            else
            {
                associateSSN = (Request.Form["hdn-SSN"] != null && Request.Form["hdn-SSN"] != "") ? Request.Form["hdn-SSN"].ToString() : "";
            }

            // FIND ALL THE OLD BONUS AND DRAWS, THEN GET THE NEW ONES AND ADD THEM...

            var drawIds = new List<string>();
            var bonusIds = new List<string>();

            foreach (var key in Request.Form.AllKeys)
            {
                if(key.Contains("-draw-id"))
                {
                    drawIds.Add(key.Replace("-draw-id", ""));
                }

                if (key.Contains("-bonus-id"))
                {
                    bonusIds.Add(key.Replace("-bonus-id", ""));
                }
            }

            foreach(var id in bonusIds)
            {
                var bonusDate = (Request.Form[id + "-bonus-date"] != null && Request.Form[id + "-bonus-date"] != "") ? Request.Form[id + "-bonus-date"].ToString() : "";
                var bonusAmount = (Request.Form[id + "-bonus-amount"] != null && Request.Form[id + "-bonus-amount"] != "") ? Decimal.Parse(Request.Form[id + "-bonus-amount"]) : 0;
                var bonusComments = (Request.Form[id + "-bonus-comments"] != null && Request.Form[id + "-bonus-comments"] != "") ? Request.Form[id + "-bonus-comments"].ToString() : "";
                var bDelete = Request.Form[id + "-bonus-delete"];

                var associateBonus = new Bonus();

                associateBonus.Id = Int32.Parse(id);
                associateBonus.AssociateSSN = associateSSN;
                associateBonus.MonthYear = monthId + "/" + yearId;

                associateBonus.CreateDate = DateTime.Now;
                associateBonus.CreateUser = Session["UserName"].ToString();

                associateBonus.BonusDate = bonusDate;
                associateBonus.BonusAmount = bonusAmount;
                associateBonus.BonusComments = bonusComments;

                if (bDelete != null && bDelete.ToLower() == "on")
                {
                    var success = SqlQueries.DeleteAssociateBonus(associateBonus);
                }
                else
                {
                    var success = SqlQueries.UpdateAssociateBonus(associateBonus);
                }

            }

            foreach (var id in drawIds)
            {
                var drawDate = (Request.Form[id + "-draw-date"] != null ? Convert.ToDateTime(Request.Form[id + "-draw-date"]) : new DateTime(1900, 1, 1));
                var drawAmount = (Request.Form[id + "-draw-amount"] != null && Request.Form[id + "-draw-amount"] != "") ? Decimal.Parse(Request.Form[id + "-draw-amount"]) : 0;
                var bDelete = Request.Form[id + "-draw-delete"];

                var associateDraw = new Draw();

                associateDraw.Id = Int32.Parse(id);
                associateDraw.AssociateSSN = associateSSN;
                associateDraw.MonthYear = monthId + "/" + yearId;

                associateDraw.CreateDate = DateTime.Now;
                associateDraw.CreateUser = Session["UserName"].ToString();

                associateDraw.DrawHours = 0;
                associateDraw.DrawDate = drawDate;
                associateDraw.DrawAmount = drawAmount;
                associateDraw.DrawGuarBegin = "";
                associateDraw.DrawGuarEnd = "";

                if (bDelete != null && bDelete.ToLower() == "on")
                {
                    var success = SqlQueries.DeleteAssociateDraws(associateDraw);
                }
                else
                {
                    var success = SqlQueries.UpdateAssociateDraws(associateDraw);
                }
            }

            if (Request.Form["new-bonus-amount"] != null && Request.Form["new-bonus-amount"] != "")
            {
             
                var associateBonus = new Bonus();

                associateBonus.AssociateSSN = associateSSN;
                associateBonus.MonthYear = monthId + "/" + yearId;

                associateBonus.CreateDate = DateTime.Now;
                associateBonus.CreateUser = Session["UserName"].ToString();

                associateBonus.BonusDate = (Request.Form["new-bonus-date"] != null && Request.Form["new-bonus-date"] != "") ? Request.Form["new-bonus-date"].ToString() : "";
                associateBonus.BonusAmount = (Request.Form["new-bonus-amount"] != null && Request.Form["new-bonus-amount"] != "") ? Decimal.Parse(Request.Form["new-bonus-amount"]) : 0;
                associateBonus.BonusComments = (Request.Form["new-bonus-comments"] != null && Request.Form["new-bonus-comments"] != "") ? Request.Form["new-bonus-comments"].ToString() : "";


                var success = SqlQueries.SaveAssociateBonus(associateBonus);

            }

            if (Request.Form["new-bonus-amount2"] != null && Request.Form["new-bonus-amount2"] != "")
            {

                var associateBonus = new Bonus();

                associateBonus.AssociateSSN = associateSSN;
                associateBonus.MonthYear = monthId + "/" + yearId;

                associateBonus.CreateDate = DateTime.Now;
                associateBonus.CreateUser = Session["UserName"].ToString();

                associateBonus.BonusDate = (Request.Form["new-bonus-date2"] != null && Request.Form["new-bonus-date2"] != "") ? Request.Form["new-bonus-date2"].ToString() : "";
                associateBonus.BonusAmount = (Request.Form["new-bonus-amount2"] != null && Request.Form["new-bonus-amount2"] != "") ? Decimal.Parse(Request.Form["new-bonus-amount2"]) : 0;
                associateBonus.BonusComments = (Request.Form["new-bonus-comments2"] != null && Request.Form["new-bonus-comments2"] != "") ? Request.Form["new-bonus-comments2"].ToString() : "";


                var success = SqlQueries.SaveAssociateBonus(associateBonus);

            }

            if (Request.Form["new-bonus-amount3"] != null && Request.Form["new-bonus-amount3"] != "")
            {

                var associateBonus = new Bonus();

                associateBonus.AssociateSSN = associateSSN;
                associateBonus.MonthYear = monthId + "/" + yearId;

                associateBonus.CreateDate = DateTime.Now;
                associateBonus.CreateUser = Session["UserName"].ToString();

                associateBonus.BonusDate = (Request.Form["new-bonus-date3"] != null && Request.Form["new-bonus-date3"] != "") ? Request.Form["new-bonus-date3"].ToString() : "";
                associateBonus.BonusAmount = (Request.Form["new-bonus-amount3"] != null && Request.Form["new-bonus-amount3"] != "") ? Decimal.Parse(Request.Form["new-bonus-amount3"]) : 0;
                associateBonus.BonusComments = (Request.Form["new-bonus-comments3"] != null && Request.Form["new-bonus-comments3"] != "") ? Request.Form["new-bonus-comments3"].ToString() : "";


                var success = SqlQueries.SaveAssociateBonus(associateBonus);

            }

            if (Request.Form["new-draw-amount"] != null && Request.Form["new-draw-amount"] != "")
            {
             
                var associateDraw = new Draw();

                associateDraw.AssociateSSN = associateSSN;
                associateDraw.MonthYear = monthId + "/" + yearId;

                associateDraw.CreateDate = DateTime.Now;
                associateDraw.CreateUser = Session["UserName"].ToString();

                associateDraw.DrawHours = 0;
                associateDraw.DrawDate = (Request.Form["new-draw-date"] != null ? Convert.ToDateTime(Request.Form["new-draw-date"]) : new DateTime(1900,1,1));
                associateDraw.DrawAmount = (Request.Form["new-draw-amount"] != null && Request.Form["new-draw-amount"] != "") ? Decimal.Parse(Request.Form["new-draw-amount"]) : 0;
                associateDraw.DrawGuarBegin = "";
                associateDraw.DrawGuarEnd = "";
                


                var success = SqlQueries.SaveAssociateDraws(associateDraw);

            }

            if (Request.Form["new-draw-amount2"] != null && Request.Form["new-draw-amount2"] != "")
            {

                var associateDraw = new Draw();

                associateDraw.AssociateSSN = associateSSN;
                associateDraw.MonthYear = monthId + "/" + yearId;

                associateDraw.CreateDate = DateTime.Now;
                associateDraw.CreateUser = Session["UserName"].ToString();

                associateDraw.DrawHours = 0;
                associateDraw.DrawDate = (Request.Form["new-draw-date2"] != null ? Convert.ToDateTime(Request.Form["new-draw-date2"]) : new DateTime(1900, 1, 1));
                associateDraw.DrawAmount = (Request.Form["new-draw-amount2"] != null && Request.Form["new-draw-amount2"] != "") ? Decimal.Parse(Request.Form["new-draw-amount2"]) : 0;
                associateDraw.DrawGuarBegin = "";
                associateDraw.DrawGuarEnd = "";



                var success = SqlQueries.SaveAssociateDraws(associateDraw);

            }
            if (Request.Form["new-draw-amount3"] != null && Request.Form["new-draw-amount3"] != "")
            {

                var associateDraw = new Draw();

                associateDraw.AssociateSSN = associateSSN;
                associateDraw.MonthYear = monthId + "/" + yearId;

                associateDraw.CreateDate = DateTime.Now;
                associateDraw.CreateUser = Session["UserName"].ToString();

                associateDraw.DrawHours = 0;
                associateDraw.DrawDate = (Request.Form["new-draw-date3"] != null ? Convert.ToDateTime(Request.Form["new-draw-date3"]) : new DateTime(1900, 1, 1));
                associateDraw.DrawAmount = (Request.Form["new-draw-amount3"] != null && Request.Form["new-draw-amount3"] != "") ? Decimal.Parse(Request.Form["new-draw-amount3"]) : 0;
                associateDraw.DrawGuarBegin = "";
                associateDraw.DrawGuarEnd = "";



                var success = SqlQueries.SaveAssociateDraws(associateDraw);

            }


            return new EmptyResult();
        }

        public ActionResult Associates()
        {
            SetUserInformation();
            var associateModel = new AssociateModel();

            if (DateTime.Now < new DateTime(DateTime.Now.Year, DateTime.Now.Month, 6))
            {
                var previousMonth = DateTime.Now.AddMonths(-1);
                associateModel.MonthId = previousMonth.Month;
                associateModel.YearId = previousMonth.Year;
            }
            else
            {
                associateModel.MonthId = DateTime.Now.Month;
                associateModel.YearId = DateTime.Now.Year;
            }

            return View(associateModel);
        }

        [HttpPost]
        public ActionResult Associates(AssociateModel associateModel)
        {
            var monthId = 0;
            var yearId = 0;
            var locationId = "";
            var associateSSN = "";
            var associateSSI = "";
            var associateLevel = "";

            if (Request.Form["hdn-MonthId"] != null && Request.Form["hdn-MonthId"].Contains(","))
            {
                var cleanedMonthId = Request.Form["hdn-MonthId"].Substring(Request.Form["hdn-MonthId"].IndexOf(',') + 1);
                monthId = (cleanedMonthId != "") ? Int32.Parse(cleanedMonthId) : 0;
            }
            else
            {
                monthId = (Request.Form["hdn-MonthId"] != null && Request.Form["hdn-MonthId"] != "") ? Int32.Parse(Request.Form["hdn-MonthId"]) : 0;
            }

            if (Request.Form["hdn-YearId"] != null && Request.Form["hdn-YearId"].Contains(","))
            {
                var cleanedYearId = Request.Form["hdn-YearId"].Substring(Request.Form["hdn-YearId"].IndexOf(',') + 1);
                yearId = (cleanedYearId != "") ? Int32.Parse(cleanedYearId) : 0;
            }
            else
            {
                yearId = (Request.Form["hdn-YearId"] != null && Request.Form["hdn-YearId"] != "") ? Int32.Parse(Request.Form["hdn-YearId"]) : 0;
            }

            if (Request.Form["hdn-locationId"] != null && Request.Form["hdn-locationId"].Contains(","))
            {
                var cleanedLocationId = Request.Form["hdn-locationId"].Substring(Request.Form["hdn-locationId"].IndexOf(',') + 1);
                locationId = (cleanedLocationId != "") ? cleanedLocationId.ToString() : "";
            }
            else
            {
                locationId = (Request.Form["hdn-locationId"] != null && Request.Form["hdn-locationId"] != "") ? Request.Form["hdn-locationId"].ToString() : "";
            }

            if (Request.Form["hdn-SSN"] != null && Request.Form["hdn-SSN"].Contains(","))
            {
                var cleanedassociateSSN = Request.Form["hdn-SSN"].Substring(Request.Form["hdn-SSN"].IndexOf(',') + 1);
                associateSSN = (cleanedassociateSSN != "") ? cleanedassociateSSN.ToString() : "";
            }
            else
            {
                associateSSN = (Request.Form["hdn-SSN"] != null && Request.Form["hdn-SSN"] != "") ? Request.Form["hdn-SSN"].ToString() : "";
            }

            if (Request.Form["hdn-SSI"] != null && Request.Form["hdn-SSI"].Contains(","))
            {
                var cleanedassociateSSI = Request.Form["hdn-SSI"].Substring(Request.Form["hdn-SSI"].IndexOf(',') + 1);
                associateSSI = (cleanedassociateSSI != "") ? cleanedassociateSSI.ToString() : "";
            }
            else
            {
                associateSSI = (Request.Form["hdn-SSI"] != null && Request.Form["hdn-SSI"] != "") ? Request.Form["hdn-SSI"].ToString() : "";
            }


            if (Request.Form["hdn-Level"] != null && Request.Form["hdn-Level"].Contains(","))
            {
                var cleanedassociateLevel = Request.Form["hdn-Level"].Substring(Request.Form["hdn-Level"].IndexOf(',') + 1);
                associateLevel = (cleanedassociateLevel != "") ? cleanedassociateLevel.ToString() : "";
            }
            else
            {
                associateLevel = (Request.Form["hdn-Level"] != null && Request.Form["hdn-Level"] != "") ? Request.Form["hdn-Level"].ToString() : "";
            }

            if (Request.Form["SaveAssociate"] != null)
            {

                associateModel.MonthId = monthId;
                associateModel.YearId = yearId;
                associateModel.StoreId = locationId;

                var associateInformation = new AssociateUpdate();
                associateInformation.AssociateSSN = associateSSN;
                associateInformation.AssociateLevel = associateLevel; //(Request.Form["associate.AssociateLevel"] != null && Request.Form["associate.AssociateLevel"] != "") ? Request.Form["associate.AssociateLevel"].ToString() : "";
                associateInformation.AssociateMonthYear = monthId + "/" + yearId;
                associateInformation.AssociateStoreVolume = (Request.Form["associate.AssociateStoreVolume"] != null && Request.Form["associate.AssociateStoreVolume"] != "") ? Request.Form["associate.AssociateStoreVolume"].ToString() : "";
                associateInformation.AssociateSSI = associateSSI; //(Request.Form["associate.AssociateSSI"] != null && Request.Form["associate.AssociateSSI"] != "") ? Request.Form["associate.AssociateSSI"].ToString() : "";
                associateInformation.AssociatePayscale = (Request.Form["associate.AssociatePayscale"] != null && Request.Form["associate.AssociatePayscale"] != "") ? Request.Form["associate.AssociatePayscale"].ToString() : "";
                associateInformation.AssociateStatus = (Request.Form["associate.AssociateStatus"] != null && Request.Form["associate.AssociateStatus"] != "") ? Request.Form["associate.AssociateStatus"].ToString() : "";
                associateInformation.AssociateMentor = (Request.Form["associate.AssociateMentor"] != null && Request.Form["associate.AssociateMentor"] != "") ? Request.Form["associate.AssociateMentor"].ToString() : "";
                //associateInformation.AssociateHireDate = (Request.Form["bonusComment"] != null && Request.Form["bonusComment"] != "") ? Request.Form["bonusComment"].ToString() : "";

                if(Request.Form["compDate"] != null && Request.Form["compDate"] != "")
                {
                    associateInformation.AssociateCompetencyDate = DateTime.Parse(Request.Form["compDate"]);

                    if(associateInformation.AssociateCompetencyDate.ToShortDateString() == "1/1/0001")
                    {
                        associateInformation.AssociateCompetencyDate = new DateTime(1900, 1, 1);
                    }
                }
                else
                {
                    associateInformation.AssociateCompetencyDate = new DateTime(1900,1,1);
                    
                }

                if(Request.Form["gradDate"] != null && Request.Form["gradDate"] != "")
                {
                    associateInformation.AssociateGraduationDate = DateTime.Parse(Request.Form["gradDate"]);
                    if (associateInformation.AssociateGraduationDate.ToShortDateString() == "1/1/0001")
                    {
                        associateInformation.AssociateGraduationDate = new DateTime(1900, 1, 1);
                    }
                }
                else
                {
                    associateInformation.AssociateGraduationDate = new DateTime(1900, 1, 1);

                }

                associateInformation.AssociateHireDate = new DateTime(1900, 1, 1);

                associateInformation.UpdateDate = DateTime.Now;
                associateInformation.UpdateUser = Session["UserName"].ToString();

                var saved = SqlQueries.SaveAssociateInformation(associateInformation);
                
                var associateGoal = new Goal();

                associateGoal.AssociateSSN = associateSSN;
                associateGoal.MonthYear = monthId + "/" + yearId;
                associateGoal.UpdateUser = Session["UserName"].ToString();
                associateGoal.UpdateDate = DateTime.Now;

                associateGoal.DealCount = (Request.Form["dealCount"] != null && Request.Form["dealCount"] != "") ? Int32.Parse(Request.Form["dealCount"]) : 0;
                associateGoal.NewCount = (Request.Form["newCount"] != null && Request.Form["newCount"] != "") ? Int32.Parse(Request.Form["newCount"]) : 0;
                associateGoal.UsedCount = (Request.Form["usedCount"] != null && Request.Form["usedCount"] != "") ? Int32.Parse(Request.Form["usedCount"]) : 0;
                associateGoal.BPPCount = (Request.Form["bppCount"] != null && Request.Form["bppCount"] != "") ? Int32.Parse(Request.Form["bppCount"]) : 0;
                associateGoal.TradeCount = (Request.Form["tradeCount"] != null && Request.Form["tradeCount"] != "") ? Int32.Parse(Request.Form["tradeCount"]) : 0;
                associateGoal.FinanceCount = (Request.Form["financeCount"] != null && Request.Form["financeCount"] != "") ? Int32.Parse(Request.Form["financeCount"]) : 0;
                associateGoal.ServiceCount = (Request.Form["serviceCount"] != null && Request.Form["serviceCount"] != "") ? Int32.Parse(Request.Form["serviceCount"]) : 0;
                associateGoal.GAPCount = (Request.Form["gapCount"] != null && Request.Form["gapCount"] != "") ? Int32.Parse(Request.Form["gapCount"]) : 0;
                associateGoal.ZurichCount = (Request.Form["zurichCount"] != null && Request.Form["zurichCount"] != "") ? Int32.Parse(Request.Form["zurichCount"]) : 0;
                associateGoal.AftermarketCount = (Request.Form["aftermarketCount"] != null && Request.Form["aftermarketCount"] != "") ? Int32.Parse(Request.Form["aftermarketCount"]) : 0;
                associateGoal.SpiffCount = (Request.Form["spiffCount"] != null && Request.Form["spiffCount"] != "") ? Int32.Parse(Request.Form["spiffCount"]) : 0;

                var success = SqlQueries.SaveAssociateGoals(associateGoal);

            }
            else if (Request.Form["SaveBonus"] != null)
            {
                associateModel.MonthId = monthId;
                associateModel.YearId = yearId;
                associateModel.StoreId = locationId;

                var associateBonus = new Bonus();
                
                associateBonus.AssociateSSN = associateSSN;
                associateBonus.MonthYear = monthId + "/" + yearId;

                associateBonus.BonusDate = DateTime.Now.ToShortDateString();

                associateBonus.CreateDate = DateTime.Now;
                associateBonus.CreateUser = Session["UserName"].ToString();

                associateBonus.BonusAmount = (Request.Form["bonusAmount"] != null && Request.Form["bonusAmount"] != "") ? Int32.Parse(Request.Form["bonusAmount"]) : 0;
                associateBonus.BonusComments = (Request.Form["bonusComment"] != null && Request.Form["bonusComment"] != "") ? Request.Form["bonusComment"].ToString() : "";


                var success = SqlQueries.SaveAssociateBonus(associateBonus);

            }
            else if (Request.Form["SaveDraw"] != null)
            {
                associateModel.MonthId = monthId;
                associateModel.YearId = yearId;
                associateModel.StoreId = locationId;

                var associateDraw = new Draw();

                associateDraw.AssociateSSN = associateSSN;
                associateDraw.MonthYear = monthId + "/" + yearId;

                associateDraw.CreateDate = DateTime.Now;
                associateDraw.CreateUser = Session["UserName"].ToString();

                associateDraw.DrawHours = (Request.Form["drawHours"] != null && Request.Form["drawHours"] != "") ? Int32.Parse(Request.Form["drawHours"]) : 0;
                associateDraw.DrawAmount = (Request.Form["drawAmount"] != null && Request.Form["drawAmount"] != "") ? Int32.Parse(Request.Form["drawAmount"]) : 0;
                associateDraw.DrawGuarBegin = (Request.Form["drawGuarBegin"] != null && Request.Form["drawGuarBegin"] != "") ? Request.Form["drawGuarBegin"].ToString() : "";
                associateDraw.DrawGuarEnd = (Request.Form["drawGuarEnd"] != null && Request.Form["drawGuarEnd"] != "") ? Request.Form["drawGuarEnd"].ToString() : "";
                associateDraw.DrawDate = DateTime.Now;


                var success = SqlQueries.SaveAssociateDraws(associateDraw);

            }


            var location = SqlQueries.GetLocationCodeByStoreId(associateModel.StoreId);
            associateModel.Location = location;

            associateModel.Associates = SqlQueries.GetAssociatesByStoreAndDate(associateModel.StoreId, associateModel.YearId, associateModel.MonthId);

            if(associateModel.YearId > 2018)
            {
                associateModel.Payscales = SqlQueries.GetNewPayscales(associateModel.Location);
            }
            else
            {
                associateModel.Payscales = SqlQueries.GetPayscales(associateModel.Location);
            }
            
            associateModel.Levels = SqlQueries.GetPayLevels(associateModel.Location);
            associateModel.SSI = SqlQueries.GetSSI(associateModel.Location);
            associateModel.StoreVolumes = SqlQueries.GetStoreVolumes(associateModel.Location);
            associateModel.Statuses = SqlQueries.GetStatuses(associateModel.Location);

            return View(associateModel);
        }

        public ActionResult AftermarketPoints()
        {
            SetUserInformation();
            var aftermarketModel = new AftermarketInputModel();

            aftermarketModel.MonthId = DateTime.Now.Month;
            aftermarketModel.YearId = DateTime.Now.Year;

            return View(aftermarketModel);
        }

        [HttpPost]
        public ActionResult AftermarketPoints(AftermarketInputModel aftermarketModel)
        {

            if (Request.Form["MonthId"] != null)
            {
                //The Submit button was clicked...
                aftermarketModel = SqlQueries.GetAftermarketInputsByDate(aftermarketModel);

                if (aftermarketModel.AftermarketInputs.Count == 0)
                {
                    //Now, let's import from the previous month...
                    aftermarketModel = SqlQueries.UpdateAftermarketInputsFromPrevious(aftermarketModel);
                }
            }
            else if (Request.Form["hdn-MonthId"] != null)
            {
                //var dealershipInputsModel = new DealershipModel();

                aftermarketModel.MonthId = (Request.Form["hdn-MonthId"] != "") ? Int32.Parse(Request.Form["hdn-MonthId"]) : 0;
                aftermarketModel.YearId = (Request.Form["hdn-YearId"] != "") ? Int32.Parse(Request.Form["hdn-YearId"]) : 0;

                var inputIndex = (Request.Form["hdn-InputIndex"] != "") ? Int32.Parse(Request.Form["hdn-InputIndex"]) : 0;

                for (int i = 1; i < inputIndex; i++)
                {
                    var aftermarketInput = new AftermarketInput();

                    aftermarketInput.Key = Convert.ToInt16(Request.Form["hdn-InputKey-" + i]);
                    aftermarketInput.MonthYear = aftermarketModel.MonthId.ToString() + "/" + aftermarketModel.YearId.ToString();

                    aftermarketInput.AftermarketFieldId = Convert.ToInt16(string.IsNullOrEmpty(Request.Form["fieldId-" + i]) ? "0" : Request.Form["fieldId-" + i]);
                    aftermarketInput.AftermarketDescription = Request.Form["description-" + i];
                    aftermarketInput.AftermarketPoints = Convert.ToDecimal(string.IsNullOrEmpty(Request.Form["points-" + i]) ? "0" : Request.Form["points-" + i]);
                    aftermarketInput.AftermarketProfitPerPoint = Convert.ToInt16(string.IsNullOrEmpty(Request.Form["profitpoints-" + i]) ? "0" : Request.Form["profitpoints-" + i]);

                    aftermarketInput.UpdateUser = Session["UserName"].ToString();
                    aftermarketInput.UpdateDate = DateTime.Now;
                    

                    var success = SqlQueries.SaveAftermarketInputs(aftermarketInput);

                }

                aftermarketModel = SqlQueries.GetAftermarketInputsByDate(aftermarketModel);

            }

            return View(aftermarketModel);
        }


        public ActionResult Stores()
        {
            SetUserInformation();
            var dealershipModel = new DealershipModel();

            dealershipModel.MonthId = DateTime.Now.Month;
            dealershipModel.YearId = DateTime.Now.Year;

            return View(dealershipModel);
        }

        [HttpPost]
        public ActionResult Stores(DealershipModel dealershipModel)
        {

            if (Request.Form["MonthId"] != null)
            {
                //The Submit button was clicked...
                dealershipModel = SqlQueries.GetDealershipInputsByDate(dealershipModel);

                if (dealershipModel.DealershipInputs.Count == 0)
                {
                    //Now, let's import from the previous month...
                    dealershipModel = SqlQueries.UpdateDealershipInputsFromPrevious(dealershipModel);
                }
            }
            else if (Request.Form["hdn-MonthId"] != null)
            {
                //var dealershipInputsModel = new DealershipModel();

                dealershipModel.MonthId = (Request.Form["hdn-MonthId"] != "") ? Int32.Parse(Request.Form["hdn-MonthId"]) : 0;
                dealershipModel.YearId = (Request.Form["hdn-YearId"] != "") ? Int32.Parse(Request.Form["hdn-YearId"]) : 0;

                var inputIndex = (Request.Form["hdn-InputIndex"] != "") ? Int32.Parse(Request.Form["hdn-InputIndex"]) : 0;

                for (int i = 1; i < inputIndex; i++)
                {
                    var revenueInformation = new RevenueInformation();

                    revenueInformation.Key = Request.Form["hdn-InputKey-" + i];
                    revenueInformation.Location= Request.Form["hdn-InputLocation-" + i];
                    revenueInformation.LocationDescription= Request.Form["hdn-InputDescription-" + i];
                    revenueInformation.MonthYear = dealershipModel.MonthId.ToString() + "/" + dealershipModel.YearId.ToString();
                    revenueInformation.StoreVolume = Request.Form["storeVolume-" + i];
                    revenueInformation.HourlyRate = Convert.ToDecimal(string.IsNullOrEmpty(Request.Form["hourlyRate-" + i]) ? "0" : Request.Form["hourlyRate-" + i]);                   
                    revenueInformation.FrontNewAmount = Convert.ToDecimal(string.IsNullOrEmpty(Request.Form["frontNew-" + i]) ? "0" : Request.Form["frontNew-" + i]);
                    revenueInformation.FrontUsedAmount = Convert.ToDecimal(string.IsNullOrEmpty(Request.Form["frontUsed-" + i]) ? "0" : Request.Form["frontUsed-" + i]);
                    revenueInformation.BPPAmount = Convert.ToDecimal(string.IsNullOrEmpty(Request.Form["bpp-" + i]) ? "0" : Request.Form["bpp-" + i]); 
                    revenueInformation.FinanceAmount = Convert.ToDecimal(string.IsNullOrEmpty(Request.Form["finance-" + i]) ? "0" : Request.Form["finance-" + i]); 
                    revenueInformation.ServiceContractAmount = Convert.ToDecimal(string.IsNullOrEmpty(Request.Form["serviceContract-" + i]) ? "0" : Request.Form["serviceContract-" + i]); 
                    revenueInformation.GAPAmount = Convert.ToDecimal(string.IsNullOrEmpty(Request.Form["gap-" + i]) ? "0" : Request.Form["gap-" + i]); 
                    revenueInformation.TradeAmount = Convert.ToDecimal(string.IsNullOrEmpty(Request.Form["trade-" + i]) ? "0" : Request.Form["trade-" + i]); 
                    revenueInformation.AftermarketAmount = Convert.ToDecimal(string.IsNullOrEmpty(Request.Form["aftermarket-" + i]) ? "0" : Request.Form["aftermarket-" + i]);
                    revenueInformation.CreateUser = "";
                    revenueInformation.CreateDate = new DateTime(1900, 1, 1); 
                    revenueInformation.ModifiedUser = Session["UserName"].ToString();
                    revenueInformation.ModifiedDate = DateTime.Now;
                    revenueInformation.BrandCode= Request.Form["hdn-InputBrandCode-" + i];

                    var success = SqlQueries.SaveDealershipInputs(revenueInformation);

                }

                dealershipModel = SqlQueries.GetDealershipInputsByDate(dealershipModel);
                
            }

            return View(dealershipModel);
        }

        public ActionResult PayscaleComparison()
        {
            var payscaleComparisonModel = new PayscaleComparisonModel();

            payscaleComparisonModel.MonthId = DateTime.Now.Month;
            payscaleComparisonModel.YearId = DateTime.Now.Year;

            return View(payscaleComparisonModel);
        }

        [HttpPost]
        public ActionResult PayscaleComparison(PayscaleComparisonModel payscaleComparisonModel)
        {

            payscaleComparisonModel = SqlQueries.GetPayscalesByDate(payscaleComparisonModel);
            payscaleComparisonModel.Associates = SqlQueries.GetAssociatesByStoreAndDate(payscaleComparisonModel.StoreId, payscaleComparisonModel.YearId, payscaleComparisonModel.MonthId);
            payscaleComparisonModel.ManufacturerSpiffs = SqlQueries.GetManufacturerSpiffs(payscaleComparisonModel.YearId, payscaleComparisonModel.MonthId);            

            ViewBag.IsCommissionAdmin = Session["IsCommissionAdmin"];

            return View(payscaleComparisonModel);
        }

        public ActionResult StorePayscaleComparison()
        {
            var associatePayscaleComparisonModel = new AssociatePayscaleComparisonModel();

            associatePayscaleComparisonModel.MonthId = 12;
            associatePayscaleComparisonModel.YearId = 2018;

            associatePayscaleComparisonModel.SalesAssociates = SqlQueries.GetSalesAssociates();

            return View(associatePayscaleComparisonModel);
        }

        [HttpPost]
        public ActionResult StorePayscaleComparison(AssociatePayscaleComparisonModel associatePayscaleComparisonModel)
        {

            if (associatePayscaleComparisonModel.AssociateId != null && associatePayscaleComparisonModel.AssociateId != "")
            {
                associatePayscaleComparisonModel.AssociateInformation = SqlQueries.GetAssociateScoreCardByDate(associatePayscaleComparisonModel.AssociateId, associatePayscaleComparisonModel.YearId, associatePayscaleComparisonModel.MonthId);                
            }
            else if(associatePayscaleComparisonModel.StoreId != null && associatePayscaleComparisonModel.StoreId != "")
            {
                associatePayscaleComparisonModel.AllAssociateInformation = SqlQueries.GetAssociateScorecardsAndHistoryByStoreAndDate(associatePayscaleComparisonModel.StoreId, associatePayscaleComparisonModel.YearId, associatePayscaleComparisonModel.MonthId);
            }

            associatePayscaleComparisonModel.ManufacturerSpiffs = SqlQueries.GetManufacturerSpiffs(associatePayscaleComparisonModel.YearId, associatePayscaleComparisonModel.MonthId);
            associatePayscaleComparisonModel.NewPayscales = SqlQueries.GetNewPayscales();

            associatePayscaleComparisonModel.AssociateLevelHistory = SqlQueries.GetAssociateLevelHistory();

            associatePayscaleComparisonModel.SalesAssociates = SqlQueries.GetSalesAssociates();

            ViewBag.IsCommissionAdmin = Session["IsCommissionAdmin"];

            return View(associatePayscaleComparisonModel);
        }

        public ActionResult AssociatePayscaleComparison()
        {
            var associatePayscaleComparisonModel = new AssociatePayscaleComparisonModel();

            associatePayscaleComparisonModel.MonthId = DateTime.Now.Month;
            associatePayscaleComparisonModel.YearId = DateTime.Now.Year;

            associatePayscaleComparisonModel.SalesAssociates = SqlQueries.GetSalesAssociates();

            return View(associatePayscaleComparisonModel);
        }

        [HttpPost]
        public ActionResult AssociatePayscaleComparison(AssociatePayscaleComparisonModel associatePayscaleComparisonModel)
        {

            if (associatePayscaleComparisonModel.AssociateId != null && associatePayscaleComparisonModel.AssociateId != "")
            {
                associatePayscaleComparisonModel.AssociateInformation = SqlQueries.GetAssociateScoreCardByDate(associatePayscaleComparisonModel.AssociateId, associatePayscaleComparisonModel.YearId, associatePayscaleComparisonModel.MonthId);
            }

            associatePayscaleComparisonModel.ManufacturerSpiffs = SqlQueries.GetManufacturerSpiffs(associatePayscaleComparisonModel.YearId, associatePayscaleComparisonModel.MonthId);
            associatePayscaleComparisonModel.NewPayscales = SqlQueries.GetNewPayscales();

            associatePayscaleComparisonModel.SalesAssociates = SqlQueries.GetSalesAssociates();

            ViewBag.IsCommissionAdmin = Session["IsCommissionAdmin"];

            return View(associatePayscaleComparisonModel);
        }


        public ActionResult PayscaleComparisonAll()
        {
            var payscaleComparisonAllModel = new PayscaleComparisonAllModel();

            payscaleComparisonAllModel.MonthId = DateTime.Now.Month;
            payscaleComparisonAllModel.YearId = DateTime.Now.Year;

            return View(payscaleComparisonAllModel);
        }

        [HttpPost]
        public ActionResult PayscaleComparisonAll(PayscaleComparisonAllModel payscaleComparisonAllModel)
        {
            payscaleComparisonAllModel.StoreComparisons = new List<PayscaleComparisonModel>();

            foreach (var store in SalesCommission.Business.Enums.StoreComparison)
            {
                var payscaleComparisonModel = new PayscaleComparisonModel();
                payscaleComparisonModel.StoreId = store.StoreId;                
                payscaleComparisonModel.MonthId = payscaleComparisonAllModel.MonthId;
                payscaleComparisonModel.YearId = payscaleComparisonAllModel.YearId;

                payscaleComparisonModel = SqlQueries.GetPayscalesByDate(payscaleComparisonModel);
                payscaleComparisonModel.Associates = SqlQueries.GetAssociatesByStoreAndDate(payscaleComparisonModel.StoreId, payscaleComparisonModel.YearId, payscaleComparisonModel.MonthId);
                payscaleComparisonModel.ManufacturerSpiffs = SqlQueries.GetManufacturerSpiffs(payscaleComparisonModel.YearId, payscaleComparisonModel.MonthId);

                payscaleComparisonAllModel.StoreComparisons.Add(payscaleComparisonModel);

            }
            ViewBag.IsCommissionAdmin = Session["IsCommissionAdmin"];

            return View(payscaleComparisonAllModel);
        }



        public ActionResult Payscales()
        {
            var payscaleModel = new PayscaleModel();

            payscaleModel.MonthId = DateTime.Now.Month;
            payscaleModel.YearId = DateTime.Now.Year;

            return View(payscaleModel);
        }

        [HttpPost]
        public ActionResult Payscales(PayscaleModel payscaleModel)
        {

            if (Request.Form["Save"] != null)
            {
                var payscaleId = payscaleModel.PayscaleId;
                var monthId = payscaleModel.MonthId;
                var yearId = payscaleModel.YearId;

                //SAVE THE NEW STANDARD
                var NewStandard = new NewPayscale();

                NewStandard.ps_Key = Convert.ToInt16(Request.Form["NewStandard_Key"]);
                NewStandard.ps_PlanCode = payscaleId;
                NewStandard.ps_PayLevel = "STD";
                NewStandard.ps_NewUsedHandy = "NEW";
                NewStandard.ps_MonthYear = monthId.ToString() + "/" + yearId.ToString();
                NewStandard.ps_UpdateUser = Session["UserName"].ToString();
                NewStandard.ps_BaseCommission = Convert.ToDecimal(Request.Form["NewStandard_Base"]);
                NewStandard.ps_FullBPP = Convert.ToDecimal(Request.Form["NewStandard_FullBPP"]);
                NewStandard.ps_HalfBPP = Convert.ToDecimal(Request.Form["NewStandard_HalfBPP"]);
                NewStandard.ps_TradeIn = Convert.ToDecimal(Request.Form["NewStandard_TradeIn"]);
                NewStandard.ps_FinanceLease = Convert.ToDecimal(Request.Form["NewStandard_FinanceLease"]);
                NewStandard.ps_ServiceContract = Convert.ToDecimal(Request.Form["NewStandard_ServiceContract"]);
                NewStandard.ps_Maintenance = Convert.ToDecimal(Request.Form["NewStandard_Maintenance"]);
                NewStandard.ps_GAP = Convert.ToDecimal(Request.Form["NewStandard_GAP"]);
                NewStandard.ps_AftermarketPerItem = Convert.ToDecimal(Request.Form["NewStandard_AftermarketPerItem"]);
                NewStandard.ps_InternalSurvey = Convert.ToDecimal(Request.Form["NewStandard_InternalSurvey"]);
                NewStandard.ps_ManufacturerSalesSatisfaction = Convert.ToDecimal(Request.Form["NewStandard_ManufacturerSalesSatisfaction"]);
                NewStandard.ps_ManufacturerSpiffGuarantee = Convert.ToDecimal(Request.Form["NewStandard_ManufacturerSpiffGuarantee"]);
                NewStandard.ps_LessServiceContracts = Convert.ToDecimal(Request.Form["NewStandard_LessServiceContracts"]);
                NewStandard.ps_VolumeBonusLevel1 = Convert.ToDecimal(Request.Form["NewStandard_VolumeBonusLevel1"]);
                NewStandard.ps_VolumeBonusLevel2 = Convert.ToDecimal(Request.Form["NewStandard_VolumeBonusLevel2"]);
                NewStandard.ps_VolumeBonusLevel3 = Convert.ToDecimal(Request.Form["NewStandard_VolumeBonusLevel3"]);
                NewStandard.ps_VolumeBonusLevel4 = Convert.ToDecimal(Request.Form["NewStandard_VolumeBonusLevel4"]);
                NewStandard.ps_VolumeBonusLevel5 = Convert.ToDecimal(Request.Form["NewStandard_VolumeBonusLevel5"]);
                NewStandard.ps_VolumeBonusLevel6 = Convert.ToDecimal(Request.Form["NewStandard_VolumeBonusLevel6"]);
                NewStandard.ps_VolumeBonusLevel7 = Convert.ToDecimal(Request.Form["NewStandard_VolumeBonusLevel7"]);
                NewStandard.ps_VolumeBonusLevel8 = 0;
                NewStandard.ps_VolumeBonusLevel9 = 0;

                //Do this to prevent SQL from erroring out, these are not saved...
                NewStandard.ps_AddDate = DateTime.Now;
                NewStandard.ps_UpdateDate = DateTime.Now;

                var success1 = SqlQueries.SavePayscale(NewStandard);

                // SAVE THE NEW CERTIFIED
                var NewCertified = new NewPayscale();

                NewCertified.ps_Key = Convert.ToInt16(Request.Form["NewCertified_Key"]);
                NewCertified.ps_PlanCode = payscaleId;
                NewCertified.ps_PayLevel = "CERT";
                NewCertified.ps_NewUsedHandy = "NEW";
                NewCertified.ps_MonthYear = monthId.ToString() + "/" + yearId.ToString();
                NewCertified.ps_UpdateUser = Session["UserName"].ToString();
                NewCertified.ps_BaseCommission = Convert.ToDecimal(Request.Form["NewCertified_Base"]);
                NewCertified.ps_FullBPP = Convert.ToDecimal(Request.Form["NewCertified_FullBPP"]);
                NewCertified.ps_HalfBPP = Convert.ToDecimal(Request.Form["NewCertified_HalfBPP"]);
                NewCertified.ps_TradeIn = Convert.ToDecimal(Request.Form["NewCertified_TradeIn"]);
                NewCertified.ps_FinanceLease = Convert.ToDecimal(Request.Form["NewCertified_FinanceLease"]);
                NewCertified.ps_ServiceContract = Convert.ToDecimal(Request.Form["NewCertified_ServiceContract"]);
                NewCertified.ps_Maintenance = Convert.ToDecimal(Request.Form["NewCertified_Maintenance"]);
                NewCertified.ps_GAP = Convert.ToDecimal(Request.Form["NewCertified_GAP"]);
                NewCertified.ps_AftermarketPerItem = Convert.ToDecimal(Request.Form["NewCertified_AftermarketPerItem"]);
                NewCertified.ps_InternalSurvey = Convert.ToDecimal(Request.Form["NewCertified_InternalSurvey"]);
                NewCertified.ps_ManufacturerSalesSatisfaction = Convert.ToDecimal(Request.Form["NewCertified_ManufacturerSalesSatisfaction"]);
                NewCertified.ps_ManufacturerSpiffGuarantee = Convert.ToDecimal(Request.Form["NewCertified_ManufacturerSpiffGuarantee"]);
                NewCertified.ps_LessServiceContracts = Convert.ToDecimal(Request.Form["NewCertified_LessServiceContracts"]);
                NewCertified.ps_VolumeBonusLevel1 = Convert.ToDecimal(Request.Form["NewCertified_VolumeBonusLevel1"]);
                NewCertified.ps_VolumeBonusLevel2 = Convert.ToDecimal(Request.Form["NewCertified_VolumeBonusLevel2"]);
                NewCertified.ps_VolumeBonusLevel3 = Convert.ToDecimal(Request.Form["NewCertified_VolumeBonusLevel3"]);
                NewCertified.ps_VolumeBonusLevel4 = Convert.ToDecimal(Request.Form["NewCertified_VolumeBonusLevel4"]);
                NewCertified.ps_VolumeBonusLevel5 = Convert.ToDecimal(Request.Form["NewCertified_VolumeBonusLevel5"]);
                NewCertified.ps_VolumeBonusLevel6 = Convert.ToDecimal(Request.Form["NewCertified_VolumeBonusLevel6"]);
                NewCertified.ps_VolumeBonusLevel7 = Convert.ToDecimal(Request.Form["NewCertified_VolumeBonusLevel7"]);
                NewCertified.ps_VolumeBonusLevel8 = 0;
                NewCertified.ps_VolumeBonusLevel9 = 0;

                //Do this to prevent SQL from erroring out, these are not saved...
                NewCertified.ps_AddDate = DateTime.Now;
                NewCertified.ps_UpdateDate = DateTime.Now;

                var success2 = SqlQueries.SavePayscale(NewCertified);

                // SAVE THE NEW ELITE
                var NewElite = new NewPayscale();

                NewElite.ps_Key = Convert.ToInt16(Request.Form["NewElite_Key"]);
                NewElite.ps_PlanCode = payscaleId;
                NewElite.ps_PayLevel = "CERTELIT";
                NewElite.ps_NewUsedHandy = "NEW";
                NewElite.ps_MonthYear = monthId.ToString() + "/" + yearId.ToString();
                NewElite.ps_UpdateUser = Session["UserName"].ToString();
                NewElite.ps_BaseCommission = Convert.ToDecimal(Request.Form["NewElite_Base"]);
                NewElite.ps_FullBPP = Convert.ToDecimal(Request.Form["NewElite_FullBPP"]);
                NewElite.ps_HalfBPP = Convert.ToDecimal(Request.Form["NewElite_HalfBPP"]);
                NewElite.ps_TradeIn = Convert.ToDecimal(Request.Form["NewElite_TradeIn"]);
                NewElite.ps_FinanceLease = Convert.ToDecimal(Request.Form["NewElite_FinanceLease"]);
                NewElite.ps_ServiceContract = Convert.ToDecimal(Request.Form["NewElite_ServiceContract"]);
                NewElite.ps_Maintenance = Convert.ToDecimal(Request.Form["NewElite_Maintenance"]);
                NewElite.ps_GAP = Convert.ToDecimal(Request.Form["NewElite_GAP"]);
                NewElite.ps_AftermarketPerItem = Convert.ToDecimal(Request.Form["NewElite_AftermarketPerItem"]);
                NewElite.ps_InternalSurvey = Convert.ToDecimal(Request.Form["NewElite_InternalSurvey"]);
                NewElite.ps_ManufacturerSalesSatisfaction = Convert.ToDecimal(Request.Form["NewElite_ManufacturerSalesSatisfaction"]);
                NewElite.ps_ManufacturerSpiffGuarantee = Convert.ToDecimal(Request.Form["NewElite_ManufacturerSpiffGuarantee"]);
                NewElite.ps_LessServiceContracts = Convert.ToDecimal(Request.Form["NewElite_LessServiceContracts"]);
                NewElite.ps_VolumeBonusLevel1 = Convert.ToDecimal(Request.Form["NewElite_VolumeBonusLevel1"]);
                NewElite.ps_VolumeBonusLevel2 = Convert.ToDecimal(Request.Form["NewElite_VolumeBonusLevel2"]);
                NewElite.ps_VolumeBonusLevel3 = Convert.ToDecimal(Request.Form["NewElite_VolumeBonusLevel3"]);
                NewElite.ps_VolumeBonusLevel4 = Convert.ToDecimal(Request.Form["NewElite_VolumeBonusLevel4"]);
                NewElite.ps_VolumeBonusLevel5 = Convert.ToDecimal(Request.Form["NewElite_VolumeBonusLevel5"]);
                NewElite.ps_VolumeBonusLevel6 = Convert.ToDecimal(Request.Form["NewElite_VolumeBonusLevel6"]);
                NewElite.ps_VolumeBonusLevel7 = Convert.ToDecimal(Request.Form["NewElite_VolumeBonusLevel7"]);
                NewElite.ps_VolumeBonusLevel8 = 0;
                NewElite.ps_VolumeBonusLevel9 = 0;

                //Do this to prevent SQL from erroring out, these are not saved...
                NewElite.ps_AddDate = DateTime.Now;
                NewElite.ps_UpdateDate = DateTime.Now;

                var success3 = SqlQueries.SavePayscale(NewElite);

                //SAVE THE Used STANDARD
                var UsedStandard = new NewPayscale();

                UsedStandard.ps_Key = Convert.ToInt16(Request.Form["UsedStandard_Key"]);
                UsedStandard.ps_PlanCode = payscaleId;
                UsedStandard.ps_PayLevel = "STD";
                UsedStandard.ps_NewUsedHandy = "USED";
                UsedStandard.ps_MonthYear = monthId.ToString() + "/" + yearId.ToString();
                UsedStandard.ps_UpdateUser = Session["UserName"].ToString();
                UsedStandard.ps_BaseCommission = Convert.ToDecimal(Request.Form["UsedStandard_Base"]);
                UsedStandard.ps_FullBPP = Convert.ToDecimal(Request.Form["UsedStandard_FullBPP"]);
                UsedStandard.ps_HalfBPP = Convert.ToDecimal(Request.Form["UsedStandard_HalfBPP"]);
                UsedStandard.ps_TradeIn = Convert.ToDecimal(Request.Form["UsedStandard_TradeIn"]);
                UsedStandard.ps_FinanceLease = Convert.ToDecimal(Request.Form["UsedStandard_FinanceLease"]);
                UsedStandard.ps_ServiceContract = Convert.ToDecimal(Request.Form["UsedStandard_ServiceContract"]);
                UsedStandard.ps_Maintenance = Convert.ToDecimal(Request.Form["UsedStandard_Maintenance"]);
                UsedStandard.ps_GAP = Convert.ToDecimal(Request.Form["UsedStandard_GAP"]);
                UsedStandard.ps_AftermarketPerItem = Convert.ToDecimal(Request.Form["UsedStandard_AftermarketPerItem"]);
                UsedStandard.ps_InternalSurvey = Convert.ToDecimal(Request.Form["UsedStandard_InternalSurvey"]);
                UsedStandard.ps_ManufacturerSalesSatisfaction = Convert.ToDecimal(Request.Form["UsedStandard_ManufacturerSalesSatisfaction"]);
                UsedStandard.ps_ManufacturerSpiffGuarantee = Convert.ToDecimal(Request.Form["UsedStandard_ManufacturerSpiffGuarantee"]);
                UsedStandard.ps_LessServiceContracts = Convert.ToDecimal(Request.Form["UsedStandard_LessServiceContracts"]);
                UsedStandard.ps_VolumeBonusLevel1 = Convert.ToDecimal(Request.Form["UsedStandard_VolumeBonusLevel1"]);
                UsedStandard.ps_VolumeBonusLevel2 = Convert.ToDecimal(Request.Form["UsedStandard_VolumeBonusLevel2"]);
                UsedStandard.ps_VolumeBonusLevel3 = Convert.ToDecimal(Request.Form["UsedStandard_VolumeBonusLevel3"]);
                UsedStandard.ps_VolumeBonusLevel4 = Convert.ToDecimal(Request.Form["UsedStandard_VolumeBonusLevel4"]);
                UsedStandard.ps_VolumeBonusLevel5 = Convert.ToDecimal(Request.Form["UsedStandard_VolumeBonusLevel5"]);
                UsedStandard.ps_VolumeBonusLevel6 = Convert.ToDecimal(Request.Form["UsedStandard_VolumeBonusLevel6"]);
                UsedStandard.ps_VolumeBonusLevel7 = Convert.ToDecimal(Request.Form["UsedStandard_VolumeBonusLevel7"]);
                UsedStandard.ps_VolumeBonusLevel8 = 0;
                UsedStandard.ps_VolumeBonusLevel9 = 0;

                //Do this to prevent SQL from erroring out, these are not saved...
                UsedStandard.ps_AddDate = DateTime.Now;
                UsedStandard.ps_UpdateDate = DateTime.Now;

                var success4 = SqlQueries.SavePayscale(UsedStandard);

                // SAVE THE Used CERTIFIED
                var UsedCertified = new NewPayscale();

                UsedCertified.ps_Key = Convert.ToInt16(Request.Form["UsedCertified_Key"]);
                UsedCertified.ps_PlanCode = payscaleId;
                UsedCertified.ps_PayLevel = "CERT";
                UsedCertified.ps_NewUsedHandy = "USED";
                UsedCertified.ps_MonthYear = monthId.ToString() + "/" + yearId.ToString();
                UsedCertified.ps_UpdateUser = Session["UserName"].ToString();
                UsedCertified.ps_BaseCommission = Convert.ToDecimal(Request.Form["UsedCertified_Base"]);
                UsedCertified.ps_FullBPP = Convert.ToDecimal(Request.Form["UsedCertified_FullBPP"]);
                UsedCertified.ps_HalfBPP = Convert.ToDecimal(Request.Form["UsedCertified_HalfBPP"]);
                UsedCertified.ps_TradeIn = Convert.ToDecimal(Request.Form["UsedCertified_TradeIn"]);
                UsedCertified.ps_FinanceLease = Convert.ToDecimal(Request.Form["UsedCertified_FinanceLease"]);
                UsedCertified.ps_ServiceContract = Convert.ToDecimal(Request.Form["UsedCertified_ServiceContract"]);
                UsedCertified.ps_Maintenance = Convert.ToDecimal(Request.Form["UsedCertified_Maintenance"]);
                UsedCertified.ps_GAP = Convert.ToDecimal(Request.Form["UsedCertified_GAP"]);
                UsedCertified.ps_AftermarketPerItem = Convert.ToDecimal(Request.Form["UsedCertified_AftermarketPerItem"]);
                UsedCertified.ps_InternalSurvey = Convert.ToDecimal(Request.Form["UsedCertified_InternalSurvey"]);
                UsedCertified.ps_ManufacturerSalesSatisfaction = Convert.ToDecimal(Request.Form["UsedCertified_ManufacturerSalesSatisfaction"]);
                UsedCertified.ps_ManufacturerSpiffGuarantee = Convert.ToDecimal(Request.Form["UsedCertified_ManufacturerSpiffGuarantee"]);
                UsedCertified.ps_LessServiceContracts = Convert.ToDecimal(Request.Form["UsedCertified_LessServiceContracts"]);
                UsedCertified.ps_VolumeBonusLevel1 = Convert.ToDecimal(Request.Form["UsedCertified_VolumeBonusLevel1"]);
                UsedCertified.ps_VolumeBonusLevel2 = Convert.ToDecimal(Request.Form["UsedCertified_VolumeBonusLevel2"]);
                UsedCertified.ps_VolumeBonusLevel3 = Convert.ToDecimal(Request.Form["UsedCertified_VolumeBonusLevel3"]);
                UsedCertified.ps_VolumeBonusLevel4 = Convert.ToDecimal(Request.Form["UsedCertified_VolumeBonusLevel4"]);
                UsedCertified.ps_VolumeBonusLevel5 = Convert.ToDecimal(Request.Form["UsedCertified_VolumeBonusLevel5"]);
                UsedCertified.ps_VolumeBonusLevel6 = Convert.ToDecimal(Request.Form["UsedCertified_VolumeBonusLevel6"]);
                UsedCertified.ps_VolumeBonusLevel7 = Convert.ToDecimal(Request.Form["UsedCertified_VolumeBonusLevel7"]);
                UsedCertified.ps_VolumeBonusLevel8 = 0;
                UsedCertified.ps_VolumeBonusLevel9 = 0;

                //Do this to prevent SQL from erroring out, these are not saved...
                UsedCertified.ps_AddDate = DateTime.Now;
                UsedCertified.ps_UpdateDate = DateTime.Now;

                var success5 = SqlQueries.SavePayscale(UsedCertified);

                // SAVE THE Used ELITE
                var UsedElite = new NewPayscale();

                UsedElite.ps_Key = Convert.ToInt16(Request.Form["UsedElite_Key"]);
                UsedElite.ps_PlanCode = payscaleId;
                UsedElite.ps_PayLevel = "CERTELIT";
                UsedElite.ps_NewUsedHandy = "USED";
                UsedElite.ps_MonthYear = monthId.ToString() + "/" + yearId.ToString();
                UsedElite.ps_UpdateUser = Session["UserName"].ToString();
                UsedElite.ps_BaseCommission = Convert.ToDecimal(Request.Form["UsedElite_Base"]);
                UsedElite.ps_FullBPP = Convert.ToDecimal(Request.Form["UsedElite_FullBPP"]);
                UsedElite.ps_HalfBPP = Convert.ToDecimal(Request.Form["UsedElite_HalfBPP"]);
                UsedElite.ps_TradeIn = Convert.ToDecimal(Request.Form["UsedElite_TradeIn"]);
                UsedElite.ps_FinanceLease = Convert.ToDecimal(Request.Form["UsedElite_FinanceLease"]);
                UsedElite.ps_ServiceContract = Convert.ToDecimal(Request.Form["UsedElite_ServiceContract"]);
                UsedElite.ps_Maintenance = Convert.ToDecimal(Request.Form["UsedElite_Maintenance"]);
                UsedElite.ps_GAP = Convert.ToDecimal(Request.Form["UsedElite_GAP"]);
                UsedElite.ps_AftermarketPerItem = Convert.ToDecimal(Request.Form["UsedElite_AftermarketPerItem"]);
                UsedElite.ps_InternalSurvey = Convert.ToDecimal(Request.Form["UsedElite_InternalSurvey"]);
                UsedElite.ps_ManufacturerSalesSatisfaction = Convert.ToDecimal(Request.Form["UsedElite_ManufacturerSalesSatisfaction"]);
                UsedElite.ps_ManufacturerSpiffGuarantee = Convert.ToDecimal(Request.Form["UsedElite_ManufacturerSpiffGuarantee"]);
                UsedElite.ps_LessServiceContracts = Convert.ToDecimal(Request.Form["UsedElite_LessServiceContracts"]);
                UsedElite.ps_VolumeBonusLevel1 = Convert.ToDecimal(Request.Form["UsedElite_VolumeBonusLevel1"]);
                UsedElite.ps_VolumeBonusLevel2 = Convert.ToDecimal(Request.Form["UsedElite_VolumeBonusLevel2"]);
                UsedElite.ps_VolumeBonusLevel3 = Convert.ToDecimal(Request.Form["UsedElite_VolumeBonusLevel3"]);
                UsedElite.ps_VolumeBonusLevel4 = Convert.ToDecimal(Request.Form["UsedElite_VolumeBonusLevel4"]);
                UsedElite.ps_VolumeBonusLevel5 = Convert.ToDecimal(Request.Form["UsedElite_VolumeBonusLevel5"]);
                UsedElite.ps_VolumeBonusLevel6 = Convert.ToDecimal(Request.Form["UsedElite_VolumeBonusLevel6"]);
                UsedElite.ps_VolumeBonusLevel7 = Convert.ToDecimal(Request.Form["UsedElite_VolumeBonusLevel7"]);
                UsedElite.ps_VolumeBonusLevel8 = 0;
                UsedElite.ps_VolumeBonusLevel9 = 0;

                //Do this to prevent SQL from erroring out, these are not saved...
                UsedElite.ps_AddDate = DateTime.Now;
                UsedElite.ps_UpdateDate = DateTime.Now;

                var success6 = SqlQueries.SavePayscale(UsedElite);

                if (payscaleId != "MOCO")
                {
                    //Save the Handymans...
                    //SAVE THE HANDY STANDARD
                    var HandyStandard = new NewPayscale();

                    HandyStandard.ps_Key = Convert.ToInt16(Request.Form["HandyStandard_Key"]);
                    HandyStandard.ps_PlanCode = payscaleId;
                    HandyStandard.ps_PayLevel = "STD";
                    HandyStandard.ps_NewUsedHandy = "HANDY";
                    HandyStandard.ps_MonthYear = monthId.ToString() + "/" + yearId.ToString();
                    HandyStandard.ps_UpdateUser = Session["UserName"].ToString();
                    HandyStandard.ps_BaseCommission = Convert.ToDecimal(Request.Form["HandyStandard_Base"]);
                    HandyStandard.ps_FullBPP = Convert.ToDecimal(Request.Form["HandyStandard_FullBPP"]);
                    HandyStandard.ps_HalfBPP = Convert.ToDecimal(Request.Form["HandyStandard_HalfBPP"]);
                    HandyStandard.ps_TradeIn = Convert.ToDecimal(Request.Form["HandyStandard_TradeIn"]);
                    HandyStandard.ps_FinanceLease = Convert.ToDecimal(Request.Form["HandyStandard_FinanceLease"]);
                    HandyStandard.ps_ServiceContract = Convert.ToDecimal(Request.Form["HandyStandard_ServiceContract"]);
                    HandyStandard.ps_Maintenance = Convert.ToDecimal(Request.Form["HandyStandard_Maintenance"]);
                    HandyStandard.ps_GAP = Convert.ToDecimal(Request.Form["HandyStandard_GAP"]);
                    HandyStandard.ps_AftermarketPerItem = Convert.ToDecimal(Request.Form["HandyStandard_AftermarketPerItem"]);
                    HandyStandard.ps_InternalSurvey = Convert.ToDecimal(Request.Form["HandyStandard_InternalSurvey"]);
                    HandyStandard.ps_ManufacturerSalesSatisfaction = Convert.ToDecimal(Request.Form["HandyStandard_ManufacturerSalesSatisfaction"]);
                    HandyStandard.ps_ManufacturerSpiffGuarantee = Convert.ToDecimal(Request.Form["HandyStandard_ManufacturerSpiffGuarantee"]);
                    HandyStandard.ps_LessServiceContracts = Convert.ToDecimal(Request.Form["HandyStandard_LessServiceContracts"]);
                    HandyStandard.ps_VolumeBonusLevel1 = Convert.ToDecimal(Request.Form["HandyStandard_VolumeBonusLevel1"]);
                    HandyStandard.ps_VolumeBonusLevel2 = Convert.ToDecimal(Request.Form["HandyStandard_VolumeBonusLevel2"]);
                    HandyStandard.ps_VolumeBonusLevel3 = Convert.ToDecimal(Request.Form["HandyStandard_VolumeBonusLevel3"]);
                    HandyStandard.ps_VolumeBonusLevel4 = Convert.ToDecimal(Request.Form["HandyStandard_VolumeBonusLevel4"]);
                    HandyStandard.ps_VolumeBonusLevel5 = Convert.ToDecimal(Request.Form["HandyStandard_VolumeBonusLevel5"]);
                    HandyStandard.ps_VolumeBonusLevel6 = Convert.ToDecimal(Request.Form["HandyStandard_VolumeBonusLevel6"]);
                    HandyStandard.ps_VolumeBonusLevel7 = Convert.ToDecimal(Request.Form["HandyStandard_VolumeBonusLevel7"]);
                    HandyStandard.ps_VolumeBonusLevel8 = 0;
                    HandyStandard.ps_VolumeBonusLevel9 = 0;

                    //Do this to prevent SQL from erroring out, these are not saved...
                    HandyStandard.ps_AddDate = DateTime.Now;
                    HandyStandard.ps_UpdateDate = DateTime.Now;

                    var success7 = SqlQueries.SavePayscale(HandyStandard);

                    // SAVE THE Handy CERTIFIED
                    var HandyCertified = new NewPayscale();

                    HandyCertified.ps_Key = Convert.ToInt16(Request.Form["HandyCertified_Key"]);
                    HandyCertified.ps_PlanCode = payscaleId;
                    HandyCertified.ps_PayLevel = "CERT";
                    HandyCertified.ps_NewUsedHandy = "HANDY";
                    HandyCertified.ps_MonthYear = monthId.ToString() + "/" + yearId.ToString();
                    HandyCertified.ps_UpdateUser = Session["UserName"].ToString();
                    HandyCertified.ps_BaseCommission = Convert.ToDecimal(Request.Form["HandyCertified_Base"]);
                    HandyCertified.ps_FullBPP = Convert.ToDecimal(Request.Form["HandyCertified_FullBPP"]);
                    HandyCertified.ps_HalfBPP = Convert.ToDecimal(Request.Form["HandyCertified_HalfBPP"]);
                    HandyCertified.ps_TradeIn = Convert.ToDecimal(Request.Form["HandyCertified_TradeIn"]);
                    HandyCertified.ps_FinanceLease = Convert.ToDecimal(Request.Form["HandyCertified_FinanceLease"]);
                    HandyCertified.ps_ServiceContract = Convert.ToDecimal(Request.Form["HandyCertified_ServiceContract"]);
                    HandyCertified.ps_Maintenance = Convert.ToDecimal(Request.Form["HandyCertified_Maintenance"]);
                    HandyCertified.ps_GAP = Convert.ToDecimal(Request.Form["HandyCertified_GAP"]);
                    HandyCertified.ps_AftermarketPerItem = Convert.ToDecimal(Request.Form["HandyCertified_AftermarketPerItem"]);
                    HandyCertified.ps_InternalSurvey = Convert.ToDecimal(Request.Form["HandyCertified_InternalSurvey"]);
                    HandyCertified.ps_ManufacturerSalesSatisfaction = Convert.ToDecimal(Request.Form["HandyCertified_ManufacturerSalesSatisfaction"]);
                    HandyCertified.ps_ManufacturerSpiffGuarantee = Convert.ToDecimal(Request.Form["HandyCertified_ManufacturerSpiffGuarantee"]);
                    HandyCertified.ps_LessServiceContracts = Convert.ToDecimal(Request.Form["HandyCertified_LessServiceContracts"]);
                    HandyCertified.ps_VolumeBonusLevel1 = Convert.ToDecimal(Request.Form["HandyCertified_VolumeBonusLevel1"]);
                    HandyCertified.ps_VolumeBonusLevel2 = Convert.ToDecimal(Request.Form["HandyCertified_VolumeBonusLevel2"]);
                    HandyCertified.ps_VolumeBonusLevel3 = Convert.ToDecimal(Request.Form["HandyCertified_VolumeBonusLevel3"]);
                    HandyCertified.ps_VolumeBonusLevel4 = Convert.ToDecimal(Request.Form["HandyCertified_VolumeBonusLevel4"]);
                    HandyCertified.ps_VolumeBonusLevel5 = Convert.ToDecimal(Request.Form["HandyCertified_VolumeBonusLevel5"]);
                    HandyCertified.ps_VolumeBonusLevel6 = Convert.ToDecimal(Request.Form["HandyCertified_VolumeBonusLevel6"]);
                    HandyCertified.ps_VolumeBonusLevel7 = Convert.ToDecimal(Request.Form["HandyCertified_VolumeBonusLevel7"]);
                    HandyCertified.ps_VolumeBonusLevel8 = 0;
                    HandyCertified.ps_VolumeBonusLevel9 = 0;

                    //Do this to prevent SQL from erroring out, these are not saved...
                    HandyCertified.ps_AddDate = DateTime.Now;
                    HandyCertified.ps_UpdateDate = DateTime.Now;

                    var success8 = SqlQueries.SavePayscale(HandyCertified);

                    // SAVE THE Handy ELITE
                    var HandyElite = new NewPayscale();

                    HandyElite.ps_Key = Convert.ToInt16(Request.Form["HandyElite_Key"]);
                    HandyElite.ps_PlanCode = payscaleId;
                    HandyElite.ps_PayLevel = "CERTELIT";
                    HandyElite.ps_NewUsedHandy = "HANDY";
                    HandyElite.ps_MonthYear = monthId.ToString() + "/" + yearId.ToString();
                    HandyElite.ps_UpdateUser = Session["UserName"].ToString();
                    HandyElite.ps_BaseCommission = Convert.ToDecimal(Request.Form["HandyElite_Base"]);
                    HandyElite.ps_FullBPP = Convert.ToDecimal(Request.Form["HandyElite_FullBPP"]);
                    HandyElite.ps_HalfBPP = Convert.ToDecimal(Request.Form["HandyElite_HalfBPP"]);
                    HandyElite.ps_TradeIn = Convert.ToDecimal(Request.Form["HandyElite_TradeIn"]);
                    HandyElite.ps_FinanceLease = Convert.ToDecimal(Request.Form["HandyElite_FinanceLease"]);
                    HandyElite.ps_ServiceContract = Convert.ToDecimal(Request.Form["HandyElite_ServiceContract"]);
                    HandyElite.ps_Maintenance = Convert.ToDecimal(Request.Form["HandyElite_Maintenance"]);
                    HandyElite.ps_GAP = Convert.ToDecimal(Request.Form["HandyElite_GAP"]);
                    HandyElite.ps_AftermarketPerItem = Convert.ToDecimal(Request.Form["HandyElite_AftermarketPerItem"]);
                    HandyElite.ps_InternalSurvey = Convert.ToDecimal(Request.Form["HandyElite_InternalSurvey"]);
                    HandyElite.ps_ManufacturerSalesSatisfaction = Convert.ToDecimal(Request.Form["HandyElite_ManufacturerSalesSatisfaction"]);
                    HandyElite.ps_ManufacturerSpiffGuarantee = Convert.ToDecimal(Request.Form["HandyElite_ManufacturerSpiffGuarantee"]);
                    HandyElite.ps_LessServiceContracts = Convert.ToDecimal(Request.Form["HandyElite_LessServiceContracts"]);
                    HandyElite.ps_VolumeBonusLevel1 = Convert.ToDecimal(Request.Form["HandyElite_VolumeBonusLevel1"]);
                    HandyElite.ps_VolumeBonusLevel2 = Convert.ToDecimal(Request.Form["HandyElite_VolumeBonusLevel2"]);
                    HandyElite.ps_VolumeBonusLevel3 = Convert.ToDecimal(Request.Form["HandyElite_VolumeBonusLevel3"]);
                    HandyElite.ps_VolumeBonusLevel4 = Convert.ToDecimal(Request.Form["HandyElite_VolumeBonusLevel4"]);
                    HandyElite.ps_VolumeBonusLevel5 = Convert.ToDecimal(Request.Form["HandyElite_VolumeBonusLevel5"]);
                    HandyElite.ps_VolumeBonusLevel6 = Convert.ToDecimal(Request.Form["HandyElite_VolumeBonusLevel6"]);
                    HandyElite.ps_VolumeBonusLevel7 = Convert.ToDecimal(Request.Form["HandyElite_VolumeBonusLevel7"]);
                    HandyElite.ps_VolumeBonusLevel8 = 0;
                    HandyElite.ps_VolumeBonusLevel9 = 0;

                    //Do this to prevent SQL from erroring out, these are not saved...
                    HandyElite.ps_AddDate = DateTime.Now;
                    HandyElite.ps_UpdateDate = DateTime.Now;

                    var success9 = SqlQueries.SavePayscale(HandyElite);
                }

            }

            payscaleModel = SqlQueries.GetPayscaleByIDAndDate(payscaleModel);

            if (payscaleModel.Payscales.Count == 0)
            {
                //Now, let's import from the previous month...
                payscaleModel = SqlQueries.UpdatePayscalesFromPrevious(payscaleModel);
            }

            return View(payscaleModel);

        }

        [HttpPost]
        public ActionResult UpdateDealApproval(string dealKey, int yearId, int monthId)
        {
            var monthYear = monthId.ToString() + "/" + yearId.ToString();
            var approvalUser = Session["UserName"].ToString();

            var deals = SqlQueries.SaveDealApproval(monthYear, approvalUser, dealKey);
            return Json(deals, JsonRequestBehavior.AllowGet);
        }


        public void SetUserInformation()
        {
            string AdminLogins = ConfigurationManager.AppSettings["AdminLogins"].ToString();

            if ((Session["UserName"] == null || Session["UserName"].ToString() == ""))
            {
                if (Request.Cookies["User"] != null)
                {
                    var cookieValue = Request.Cookies["User"].Value;

                    NameValueCollection qsCollection = HttpUtility.ParseQueryString(cookieValue);

                    ViewBag.UserName = qsCollection["name"].ToString();
                    ViewBag.Login = qsCollection["login"].ToString();

                    if (AdminLogins.Contains(ViewBag.Login))
                    {
                        ViewBag.Admin = true;
                    }
                    else
                    {
                        ViewBag.Admin = false;
                    }


                }
                else
                {
                    ViewBag.UserName = "Anonymous";
                    ViewBag.Login = "Anonymous";
                    ViewBag.Admin = false;
                }

                Session.Add("UserName", ViewBag.UserName);
                Session.Add("Login", ViewBag.Login);
                Session.Add("IsAdmin", ViewBag.Admin);
            }
            else
            {
                ViewBag.UserName = Session["UserName"];
                ViewBag.Login = Session["Login"];
                ViewBag.Admin = Session["IsAdmin"];
            }

        }

        // ******************************************************************
        // THESE ARE THE NEW PAGES, THESE WILL GO AWAY, ONLY FOR TESTING NOW
        // ******************************************************************
        public ActionResult NewIndex()
        {
            var commissionModel = new CommissionModel();

            if (DateTime.Now < new DateTime(DateTime.Now.Year, DateTime.Now.Month, 6))
            {
                var previousMonth = DateTime.Now.AddMonths(-1);
                commissionModel.MonthId = previousMonth.Month;
                commissionModel.YearId = previousMonth.Year;
            }
            else
            {
                commissionModel.MonthId = DateTime.Now.Month;
                commissionModel.YearId = DateTime.Now.Year;
            }

            ViewBag.IsCommissionAdmin = Session["IsCommissionAdmin"];

            return View(commissionModel);
        }

        [HttpPost]
        public ActionResult NewIndex(CommissionModel commissionModel)
        {
            commissionModel.Associates = SqlQueries.GetAssociatesByStoreAndDate(commissionModel.StoreId, commissionModel.YearId, commissionModel.MonthId);
            commissionModel.ManufacturerSpiffs = SqlQueries.GetManufacturerSpiffs(commissionModel.YearId, commissionModel.MonthId);
            commissionModel.DealApprovals = SqlQueries.GetDealApprovalsByDate(commissionModel.YearId, commissionModel.MonthId);

            ViewBag.IsCommissionAdmin = Session["IsCommissionAdmin"];

            if (commissionModel.YearId > 2018)
            {
                return View(commissionModel);
            }
            else
            {
                return View("Index", commissionModel);
            }
            
        }

        public ActionResult NewScorecardDashboard()
        {
            var commissionModel = new CommissionModel();

            if (DateTime.Now < new DateTime(DateTime.Now.Year, DateTime.Now.Month, 6))
            {
                var previousMonth = DateTime.Now.AddMonths(-1);
                commissionModel.MonthId = previousMonth.Month;
                commissionModel.YearId = previousMonth.Year;
            }
            else
            {
                commissionModel.MonthId = DateTime.Now.Month;
                commissionModel.YearId = DateTime.Now.Year;
            }

            ViewBag.IsCommissionAdmin = Session["IsCommissionAdmin"];

            return View(commissionModel);
        }

        [HttpPost]
        public ActionResult NewScorecardDashboard(CommissionModel commissionModel)
        {

            commissionModel.Associates = SqlQueries.GetAssociateScorecardsByStoreAndDate(commissionModel.StoreId, commissionModel.YearId, commissionModel.MonthId);

            var dealMonth = new DateTime(commissionModel.YearId, commissionModel.MonthId, 1);

            commissionModel.AssociateLeads = SqlMapperUtil.StoredProcWithParams<AssociateLead>("sp_CommissionGetAssociateLeadsByDate", new { StartDate = dealMonth, EndDate = dealMonth.AddMonths(1) }, "ReynoldsData");
            commissionModel.DealApprovals = SqlQueries.GetDealApprovalsByDate(commissionModel.YearId, commissionModel.MonthId);

            ViewBag.IsCommissionAdmin = Session["IsCommissionAdmin"];


            if (commissionModel.YearId > 2018)
            {
                return View(commissionModel);
            }
            else
            {
                return View("ScorecardDashboard",commissionModel);
            }

        }

        public ActionResult AdditionalCommission()
        {
            var additionalCommissionModel = new AdditionalCommissionModel();

            if (DateTime.Now < new DateTime(DateTime.Now.Year, DateTime.Now.Month, 6))
            {
                var previousMonth = DateTime.Now.AddMonths(-1);
                additionalCommissionModel.MonthId = previousMonth.Month;
                additionalCommissionModel.YearId = previousMonth.Year;
            }
            else
            {
                additionalCommissionModel.MonthId = DateTime.Now.Month;
                additionalCommissionModel.YearId = DateTime.Now.Year;
            }

            return View(additionalCommissionModel);
        }

        [HttpPost]
        public ActionResult AdditionalCommission(AdditionalCommissionModel additionalCommissionModel)
        {

            if (Request.Form["StoreId"] != null)
            {
                //The Submit button was clicked...
                additionalCommissionModel.AdditionalCommissions = SqlQueries.GetAdditionalCommissionsByStoreAndDate(additionalCommissionModel);

                if (additionalCommissionModel.AdditionalCommissions.Count == 0)
                {
                    //Now, let's import from the previous month...
                    additionalCommissionModel.AdditionalCommissions = SqlQueries.UpdateAdditionalCommissionsFromPrevious(additionalCommissionModel);                    
                }
            }
            else if (Request.Form["hdn-MonthId"] != null)
            {
                //var dealershipInputsModel = new DealershipModel();
                additionalCommissionModel.StoreId = (Request.Form["hdn-StoreId"] != "") ? Request.Form["hdn-StoreId"] : "";
                additionalCommissionModel.MonthId = (Request.Form["hdn-MonthId"] != "") ? Int32.Parse(Request.Form["hdn-MonthId"]) : 0;
                additionalCommissionModel.YearId = (Request.Form["hdn-YearId"] != "") ? Int32.Parse(Request.Form["hdn-YearId"]) : 0;

                var inputIndex = (Request.Form["hdn-InputIndex"] != "") ? Int32.Parse(Request.Form["hdn-InputIndex"]) : 0;

                for (int i = 1; i < inputIndex; i++)
                {
                    var additionalCommission = new AdditionalCommission();

                    additionalCommission.Id = Convert.ToInt16(Request.Form["hdn-Id-" + i]);
                    additionalCommission.MonthYear = additionalCommissionModel.MonthId.ToString() + "/" + additionalCommissionModel.YearId.ToString();
                    additionalCommission.StoreId = additionalCommissionModel.StoreId;

                    additionalCommission.MakeCode = (Request.Form["hdn-MakeCode-" + i] != "") ? Request.Form["hdn-MakeCode-" + i] : "";
                    additionalCommission.MakeName = (Request.Form["hdn-MakeName-" + i] != "") ? Request.Form["hdn-MakeName-" + i] : "";
                    additionalCommission.ModelName = (Request.Form["hdn-ModelName-" + i] != "") ? Request.Form["hdn-ModelName-" + i] : "";
                    additionalCommission.AdditionalCommissionAmount = Convert.ToDecimal(string.IsNullOrEmpty(Request.Form["additional-commission-" + i]) ? "0" : Request.Form["additional-commission-" + i]);
                    
                    additionalCommission.UpdateUser = Session["UserName"].ToString();
                    additionalCommission.UpdateDate = DateTime.Now;


                    var success = SqlQueries.SaveAdditionalCommission(additionalCommission);

                }

                additionalCommissionModel.AdditionalCommissions = SqlQueries.GetAdditionalCommissionsByStoreAndDate(additionalCommissionModel);

            }           

            return View(additionalCommissionModel);
        }

        public ActionResult NewAssociate(string id, string monthId, string yearId)
        {

            if (Int32.Parse(yearId) < 2019)
            {
                return RedirectToAction("Associate", new { id = id, monthid = monthId, yearid = yearId });
            }

            //SetUserInformation();
            var associateCommissionModel = new AssociateCommissionModel();

            associateCommissionModel.AssociateId = id;
            associateCommissionModel.MonthId = Int32.Parse(monthId);
            associateCommissionModel.YearId = Int32.Parse(yearId);

            if (id != null && id != "")
            {
                associateCommissionModel.AssociateInformation = SqlQueries.GetAssociateInformationByDate(associateCommissionModel.AssociateId, associateCommissionModel.YearId, associateCommissionModel.MonthId);
                associateCommissionModel.ManufacturerSpiffs = SqlQueries.GetManufacturerSpiffs(associateCommissionModel.YearId, associateCommissionModel.MonthId);
                associateCommissionModel.DealApprovals = SqlQueries.GetDealApprovalsByDate(associateCommissionModel.YearId, associateCommissionModel.MonthId);
            }

            associateCommissionModel.MoneyDue = SqlQueries.GetAllMoneyDue();
            associateCommissionModel.TitleDue = SqlQueries.GetAllTitlesDue();

            ViewBag.IsCommissionAdmin = Session["IsCommissionAdmin"];

            return View(associateCommissionModel);
        }

        public ActionResult NewAllAssociates(string id, string monthId, string yearId)
        {
            var commissionModel = new CommissionModel();
            commissionModel.StoreId = id;
            commissionModel.MonthId = Int32.Parse(monthId);
            commissionModel.YearId = Int32.Parse(yearId);

            //SetUserInformation();
            commissionModel.Associates = SqlQueries.GetAssociatesByStoreAndDate(commissionModel.StoreId, commissionModel.YearId, commissionModel.MonthId);
            commissionModel.ManufacturerSpiffs = SqlQueries.GetManufacturerSpiffs(commissionModel.YearId, commissionModel.MonthId);
            return View(commissionModel);
        }

        public ActionResult NewScorecard(string id, string monthId, string yearId)
        {

            if (Int32.Parse(yearId) < 2019)
            {
                return RedirectToAction("Scorecard", new { id = id, monthid = monthId, yearid = yearId });
            }

            //SetUserInformation();
            var associateCommissionModel = new AssociateCommissionModel();

            associateCommissionModel.AssociateId = id;
            associateCommissionModel.MonthId = Int32.Parse(monthId);
            associateCommissionModel.YearId = Int32.Parse(yearId);

            if (id != null && id != "")
            {
                associateCommissionModel.AssociateInformation = SqlQueries.GetAssociateScoreCardByDate(associateCommissionModel.AssociateId, associateCommissionModel.YearId, associateCommissionModel.MonthId);
                associateCommissionModel.ManufacturerSpiffs = SqlQueries.GetManufacturerSpiffs(associateCommissionModel.YearId, associateCommissionModel.MonthId);
                associateCommissionModel.AssociateScorecardHistory = SqlQueries.GetAssociateScoreCardHistoryByDate(associateCommissionModel.AssociateInformation.AssociateSSN, associateCommissionModel.YearId, associateCommissionModel.MonthId);

                if (associateCommissionModel.AssociateScorecardHistory != null && associateCommissionModel.AssociateScorecardHistory.Count > 0)
                {
                    associateCommissionModel.CurrentScorecard = associateCommissionModel.AssociateScorecardHistory[0];
                }
                else
                {
                    associateCommissionModel.CurrentScorecard = new AssociateScoreCard();
                }

                var previousScorecards = new List<AssociateScoreCard>();

                for (int index = 1; index < 4; index++)
                {
                    var previousMonth = index * -1;
                    var currentDate = new DateTime(associateCommissionModel.YearId, associateCommissionModel.MonthId, 1);

                    var reportDate = currentDate.AddMonths(previousMonth);

                    var previousScorecard = SqlQueries.GetAssociateScoreCardHistoryByDate(associateCommissionModel.AssociateInformation.AssociateSSN, reportDate.Year, reportDate.Month);

                    if (previousScorecard != null && previousScorecard.Count > 0)
                    {
                        previousScorecards.Add(previousScorecard[0]);
                    }
                    else
                    {
                        previousScorecards.Add(new AssociateScoreCard());
                    }

                }

                associateCommissionModel.PreviousAssociateScorecards = previousScorecards;

                var objectivesStandardsModel = new ObjectivesStandardsModel();
                objectivesStandardsModel.YearId = associateCommissionModel.YearId;
                objectivesStandardsModel.MonthId = associateCommissionModel.MonthId;

                objectivesStandardsModel = SqlQueries.GetObjectivesAndStandardsByDate(objectivesStandardsModel);

                associateCommissionModel.StoreObjectivesStandards = objectivesStandardsModel.ObjectivesAndStandards.FindAll(x => x.StoreId == associateCommissionModel.AssociateInformation.AssociateLocation);
            }

            associateCommissionModel.AssociateList = SqlQueries.GetAssociateListForScorecard(associateCommissionModel.AssociateInformation.AssociateLocation, associateCommissionModel.YearId, associateCommissionModel.MonthId);

            ViewBag.IsCommissionAdmin = Session["IsCommissionAdmin"];

            return View(associateCommissionModel);
        }

        [HttpPost]
        public ActionResult NewScorecard(AssociateCommissionModel associateCommissionModel)
        {
            //SetUserInformation();            

            var associateId = "";
            var monthId = 0;
            var yearId = 0;

            if (Request.Form["hdn-MonthId"] != null && Request.Form["hdn-MonthId"].Contains(","))
            {
                var cleanedMonthId = Request.Form["hdn-MonthId"].Substring(Request.Form["hdn-MonthId"].IndexOf(',') + 1);
                monthId = (cleanedMonthId != "") ? Int32.Parse(cleanedMonthId) : 0;
            }
            else
            {
                monthId = (Request.Form["hdn-MonthId"] != null && Request.Form["hdn-MonthId"] != "") ? Int32.Parse(Request.Form["hdn-MonthId"]) : 0;
            }

            if (Request.Form["hdn-YearId"] != null && Request.Form["hdn-YearId"].Contains(","))
            {
                var cleanedYearId = Request.Form["hdn-YearId"].Substring(Request.Form["hdn-YearId"].IndexOf(',') + 1);
                yearId = (cleanedYearId != "") ? Int32.Parse(cleanedYearId) : 0;
            }
            else
            {
                yearId = (Request.Form["hdn-YearId"] != null && Request.Form["hdn-YearId"] != "") ? Int32.Parse(Request.Form["hdn-YearId"]) : 0;
            }

            associateId = (Request.Form["associate-id"] != null && Request.Form["associate-id"] != "") ? Request.Form["associate-id"].ToString() : "";

            // NOW GET ALL THE INFORMATION FROM THE REQUEST...CHECK TO SEE IF SAVE OR FINALIZE WAS PUSHED
            if (Request.Form["SaveComments"] != null || Request.Form["FinalizeScorecard"] != null || Request.Form["ApproveScorecard"] != null)
            {
                var associateScoreCard = new AssociateScoreCard();

                associateScoreCard.AssociateSSN = (Request.Form["associate-SSN"] != null && Request.Form["associate-SSN"] != "") ? Request.Form["associate-SSN"].ToString() : "";
                associateScoreCard.MonthYear = monthId.ToString() + "/" + yearId.ToString();
                associateScoreCard.Rolling3MonthComments = (Request.Form["comments-rollingunits"] != null && Request.Form["comments-rollingunits"] != "") ? Request.Form["comments-rollingunits"].ToString() : ""; ;
                associateScoreCard.Rolling3MonthActual = 0;
                associateScoreCard.Rolling3MonthCount = 0;
                associateScoreCard.DeliveriesComments = (Request.Form["comments-units"] != null && Request.Form["comments-units"] != "") ? Request.Form["comments-units"].ToString() : ""; ;
                associateScoreCard.DeliveriesActual = 0;
                associateScoreCard.DeliveriesCount = 0;
                associateScoreCard.BPPComments = (Request.Form["comments-bpp"] != null && Request.Form["comments-bpp"] != "") ? Request.Form["comments-bpp"].ToString() : ""; ;
                associateScoreCard.BPPActual = 0;
                associateScoreCard.BPPCount = 0;
                associateScoreCard.VSCComments = (Request.Form["comments-service"] != null && Request.Form["comments-service"] != "") ? Request.Form["comments-service"].ToString() : ""; ;
                associateScoreCard.VSCActual = 0;
                associateScoreCard.VSCCount = 0;
                associateScoreCard.TradeComments = (Request.Form["comments-trade"] != null && Request.Form["comments-trade"] != "") ? Request.Form["comments-trade"].ToString() : ""; ;
                associateScoreCard.TradeActual = 0;
                associateScoreCard.TradeCount = 0;
                associateScoreCard.FinanceComments = (Request.Form["comments-finance"] != null && Request.Form["comments-finance"] != "") ? Request.Form["comments-finance"].ToString() : ""; ;
                associateScoreCard.FinanceActual = 0;
                associateScoreCard.FinanceCount = 0;
                associateScoreCard.LeaseComments = (Request.Form["comments-lease"] != null && Request.Form["comments-lease"] != "") ? Request.Form["comments-lease"].ToString() : ""; ;
                associateScoreCard.LeaseActual = 0;
                associateScoreCard.LeaseCount = 0;
                associateScoreCard.AftermarketComments = (Request.Form["comments-aftermarket"] != null && Request.Form["comments-aftermarket"] != "") ? Request.Form["comments-aftermarket"].ToString() : ""; ;
                associateScoreCard.AftermarketActual = 0;
                associateScoreCard.AftermarketCount = 0;
                associateScoreCard.SurveyComments = (Request.Form["comments-ad-survey"] != null && Request.Form["comments-ad-survey"] != "") ? Request.Form["comments-ad-survey"].ToString() : ""; ;
                associateScoreCard.SurveyActual = 0;
                associateScoreCard.SurveyCount = 0;
                associateScoreCard.SatisfactionActual = (Request.Form["satisfaction-actual"] != null && Request.Form["satisfaction-actual"] != "") ? Request.Form["satisfaction-actual"].ToString() : "";
                associateScoreCard.SatisfactionObjective = (Request.Form["satisfaction-objective"] != null && Request.Form["satisfaction-objective"] != "") ? Request.Form["satisfaction-objective"].ToString() : "";
                associateScoreCard.SatisfactionComments = (Request.Form["comments-satisfaction"] != null && Request.Form["comments-satisfaction"] != "") ? Request.Form["comments-satisfaction"].ToString() : "";
                associateScoreCard.TrainingComments = (Request.Form["comments-training"] != null && Request.Form["comments-training"] != "") ? Request.Form["comments-training"].ToString() : "";
                associateScoreCard.FandIProcessComments = (Request.Form["comments-fandi"] != null && Request.Form["comments-fandi"] != "") ? Request.Form["comments-fandi"].ToString() : "";
                associateScoreCard.FitzwayProcessComments = (Request.Form["comments-fitzway"] != null && Request.Form["comments-fitzway"] != "") ? Request.Form["comments-fitzway"].ToString() : "";
                associateScoreCard.OverrideComments = (Request.Form["comments-override"] != null && Request.Form["comments-override"] != "") ? Request.Form["comments-override"].ToString() : "";
                associateScoreCard.CalculatedLevel = (Request.Form["calculated-level"] != null && Request.Form["calculated-level"] != "") ? Request.Form["calculated-level"].ToString() : "";
                associateScoreCard.ApprovedLevel = (Request.Form["certification-level"] != null && Request.Form["certification-level"] != "") ? Request.Form["certification-level"].ToString() : "";
                associateScoreCard.UpdateDate = DateTime.Now;
                associateScoreCard.UpdateUser = Session["UserName"].ToString();

                associateScoreCard.SatisfactionActual3Month = (Request.Form["satisfaction-actual-3month"] != null && Request.Form["satisfaction-actual-3month"] != "") ? Request.Form["satisfaction-actual-3month"].ToString() : "";
                associateScoreCard.SatisfactionObjective3Month = (Request.Form["satisfaction-objective-3month"] != null && Request.Form["satisfaction-objective-3month"] != "") ? Request.Form["satisfaction-objective-3month"].ToString() : "";

                if (Request.Form["yesnoManufactuer"] != null)
                {
                    if (Request.Form["yesnoManufactuer"].ToString().ToLower() == "yes")
                    {
                        associateScoreCard.TrainingYN = "1";
                    }
                    else
                    {
                        associateScoreCard.TrainingYN = "0";
                    }
                }
                else
                {
                    associateScoreCard.TrainingYN = "0";
                }

                if (Request.Form["yesnoFI"] != null)
                {
                    if (Request.Form["yesnoFI"].ToString().ToLower() == "yes")
                    {
                        associateScoreCard.FandIProcessYN = "1";
                    }
                    else
                    {
                        associateScoreCard.FandIProcessYN = "0";
                    }
                }
                else
                {
                    associateScoreCard.FandIProcessYN = "0";
                }

                if (Request.Form["yesnoFitzway"] != null)
                {
                    if (Request.Form["yesnoFitzway"].ToString().ToLower() == "yes")
                    {
                        associateScoreCard.FitzwayProcessYN = "1";
                    }
                    else
                    {
                        associateScoreCard.FitzwayProcessYN = "0";
                    }
                }
                else
                {
                    associateScoreCard.FitzwayProcessYN = "0";
                }

                if (Request.Form["chk-MeetsSSI"] != null)
                {
                    if (Request.Form["chk-MeetsSSI"].ToString().ToLower() == "on")
                    {
                        associateScoreCard.MeetsSSIObjective = "0";
                    }
                    else
                    {
                        associateScoreCard.MeetsSSIObjective = "1";
                    }
                }
                else
                {
                    associateScoreCard.MeetsSSIObjective = "1";
                }

                if (Request.Form["FinalizeScorecard"] != null)
                {
                    associateScoreCard.FinalizeDate = DateTime.Now;
                    associateScoreCard.FinalizeUser = Session["UserName"].ToString();

                //    // NEED TO UPDATE THE ASSOCIATES CERTIFICATION LEVEL
                    var certificationLevel = "";

                //    if (associateScoreCard.ApprovedLevel == "standard")
                //    {
                //        certificationLevel = "STD";
                //    }
                //    else if (associateScoreCard.ApprovedLevel == "certified")
                //    {
                //        certificationLevel = "CERT";
                //    }
                //    else if (associateScoreCard.ApprovedLevel == "elite")
                //    {
                //        certificationLevel = "CERTELIT";
                //    }

                    //var updateLevel = SqlQueries.UpdateAssociatLevel(associateScoreCard.AssociateSSN, associateScoreCard.MonthYear, certificationLevel);
                }
                else
                {
                    associateScoreCard.FinalizeDate = new DateTime(1900, 1, 1);
                    associateScoreCard.FinalizeUser = "";
                }

                if (Request.Form["ApproveScorecard"] != null)
                {
                    associateScoreCard.ApprovalDate = DateTime.Now;
                    associateScoreCard.ApprovalUser = Session["UserName"].ToString();

                    // NEED TO UPDATE THE ASSOCIATES CERTIFICATION LEVEL
                    var certificationLevel = "";

                    //if (associateScoreCard.ApprovedLevel == "standard")
                    //{
                    //    certificationLevel = "STD";
                    //}
                    //else if (associateScoreCard.ApprovedLevel == "certified")
                    //{
                    //    certificationLevel = "CERT";
                    //}
                    //else if (associateScoreCard.ApprovedLevel == "elite")
                    //{
                    //    certificationLevel = "CERTELIT";
                    //}

                    //var updateLevel = SqlQueries.UpdateAssociatLevel(associateScoreCard.AssociateSSN, associateScoreCard.MonthYear, certificationLevel);
                }
                else
                {
                    associateScoreCard.ApprovalDate = new DateTime(1900, 1, 1);
                    associateScoreCard.ApprovalUser = "";
                }

                var done = SqlQueries.SaveAssociateScoreCardHistory(associateScoreCard);
            }

            if (Request.Form["SaveGoals"] != null)
            {

                var associateGoal = new Goal();

                associateGoal.AssociateSSN = (Request.Form["associate-SSN"] != null && Request.Form["associate-SSN"] != "") ? Request.Form["associate-SSN"].ToString() : "";
                associateGoal.MonthYear = monthId + "/" + yearId;
                associateGoal.UpdateUser = Session["UserName"].ToString();
                associateGoal.UpdateDate = DateTime.Now;

                associateGoal.DealCount = (Request.Form["dealCount"] != null && Request.Form["dealCount"] != "") ? Int32.Parse(Request.Form["dealCount"]) : 0;
                associateGoal.NewCount = (Request.Form["newCount"] != null && Request.Form["newCount"] != "") ? Int32.Parse(Request.Form["newCount"]) : 0;
                associateGoal.UsedCount = (Request.Form["usedCount"] != null && Request.Form["usedCount"] != "") ? Int32.Parse(Request.Form["usedCount"]) : 0;
                associateGoal.BPPCount = (Request.Form["bppCount"] != null && Request.Form["bppCount"] != "") ? Int32.Parse(Request.Form["bppCount"]) : 0;
                associateGoal.TradeCount = (Request.Form["tradeCount"] != null && Request.Form["tradeCount"] != "") ? Int32.Parse(Request.Form["tradeCount"]) : 0;
                associateGoal.FinanceCount = (Request.Form["financeCount"] != null && Request.Form["financeCount"] != "") ? Int32.Parse(Request.Form["financeCount"]) : 0;
                associateGoal.ServiceCount = (Request.Form["serviceCount"] != null && Request.Form["serviceCount"] != "") ? Int32.Parse(Request.Form["serviceCount"]) : 0;
                associateGoal.GAPCount = (Request.Form["gapCount"] != null && Request.Form["gapCount"] != "") ? Int32.Parse(Request.Form["gapCount"]) : 0;
                associateGoal.ZurichCount = (Request.Form["zurichCount"] != null && Request.Form["zurichCount"] != "") ? Int32.Parse(Request.Form["zurichCount"]) : 0;
                associateGoal.AftermarketCount = (Request.Form["aftermarketCount"] != null && Request.Form["aftermarketCount"] != "") ? Int32.Parse(Request.Form["aftermarketCount"]) : 0;
                associateGoal.SpiffCount = (Request.Form["spiffCount"] != null && Request.Form["spiffCount"] != "") ? Int32.Parse(Request.Form["spiffCount"]) : 0;

                var success = SqlQueries.SaveAssociateGoals(associateGoal);

            }

            associateCommissionModel.AssociateId = associateId;
            associateCommissionModel.MonthId = monthId;
            associateCommissionModel.YearId = yearId;

            if (associateId != null && associateId != "")
            {
                associateCommissionModel.AssociateInformation = SqlQueries.GetAssociateScoreCardByDate(associateCommissionModel.AssociateId, associateCommissionModel.YearId, associateCommissionModel.MonthId);
                associateCommissionModel.ManufacturerSpiffs = SqlQueries.GetManufacturerSpiffs(associateCommissionModel.YearId, associateCommissionModel.MonthId);
                associateCommissionModel.AssociateScorecardHistory = SqlQueries.GetAssociateScoreCardHistoryByDate(associateCommissionModel.AssociateInformation.AssociateSSN, associateCommissionModel.YearId, associateCommissionModel.MonthId);

                if (associateCommissionModel.AssociateScorecardHistory != null && associateCommissionModel.AssociateScorecardHistory.Count > 0)
                {
                    associateCommissionModel.CurrentScorecard = associateCommissionModel.AssociateScorecardHistory[0];
                }
                else
                {
                    associateCommissionModel.CurrentScorecard = new AssociateScoreCard();
                }

                var previousScorecards = new List<AssociateScoreCard>();

                for (int index = 1; index < 4; index++)
                {
                    var previousMonth = index * -1;
                    var currentDate = new DateTime(associateCommissionModel.YearId, associateCommissionModel.MonthId, 1);

                    var reportDate = currentDate.AddMonths(previousMonth);

                    var previousScorecard = SqlQueries.GetAssociateScoreCardHistoryByDate(associateCommissionModel.AssociateInformation.AssociateSSN, reportDate.Year, reportDate.Month);

                    if (previousScorecard != null && previousScorecard.Count > 0)
                    {
                        previousScorecards.Add(previousScorecard[0]);
                    }
                    else
                    {
                        previousScorecards.Add(new AssociateScoreCard());
                    }

                }

                associateCommissionModel.PreviousAssociateScorecards = previousScorecards;


            }

            associateCommissionModel.AssociateList = SqlQueries.GetAssociateListForScorecard(associateCommissionModel.AssociateInformation.AssociateLocation, associateCommissionModel.YearId, associateCommissionModel.MonthId);

            ViewBag.IsCommissionAdmin = Session["IsCommissionAdmin"];

            return View(associateCommissionModel);
        }

        public ActionResult NewPayscales()
        {
            var payscaleModel = new PayscaleModel();

            payscaleModel.MonthId = DateTime.Now.Month;
            payscaleModel.YearId = DateTime.Now.Year;

            return View(payscaleModel);
        }

        [HttpPost]
        public ActionResult NewPayscales(PayscaleModel payscaleModel)
        {

            if (Request.Form["Save"] != null)
            {
                var payscaleId = payscaleModel.PayscaleId;
                var monthId = payscaleModel.MonthId;
                var yearId = payscaleModel.YearId;

                //SAVE THE NEW STANDARD
                var NewStandard = new NewPayscale();

                NewStandard.ps_Key = Convert.ToInt16(Request.Form["NewStandard_Key"]);
                NewStandard.ps_PlanCode = payscaleId;
                NewStandard.ps_PayLevel = "STD";
                NewStandard.ps_NewUsedHandy = "NEW";
                NewStandard.ps_MonthYear = monthId.ToString() + "/" + yearId.ToString();
                NewStandard.ps_UpdateUser = Session["UserName"].ToString();
                NewStandard.ps_BaseCommission = Convert.ToDecimal(Request.Form["NewStandard_Base"]);
                NewStandard.ps_FullBPP = Convert.ToDecimal(Request.Form["NewStandard_FullBPP"]);
                NewStandard.ps_HalfBPP = Convert.ToDecimal(Request.Form["NewStandard_HalfBPP"]);
                NewStandard.ps_TradeIn = Convert.ToDecimal(Request.Form["NewStandard_TradeIn"]);
                NewStandard.ps_FinanceLease = Convert.ToDecimal(Request.Form["NewStandard_FinanceLease"]);
                NewStandard.ps_ServiceContract = Convert.ToDecimal(Request.Form["NewStandard_ServiceContract"]);
                NewStandard.ps_Maintenance = Convert.ToDecimal(Request.Form["NewStandard_Maintenance"]);
                NewStandard.ps_GAP = Convert.ToDecimal(Request.Form["NewStandard_GAP"]);
                NewStandard.ps_AftermarketPerItem = Convert.ToDecimal(Request.Form["NewStandard_AftermarketPerItem"]);
                NewStandard.ps_InternalSurvey = Convert.ToDecimal(Request.Form["NewStandard_InternalSurvey"]);
                NewStandard.ps_ManufacturerSalesSatisfaction = Convert.ToDecimal(Request.Form["NewStandard_ManufacturerSalesSatisfaction"]);
                NewStandard.ps_ManufacturerSpiffGuarantee = Convert.ToDecimal(Request.Form["NewStandard_ManufacturerSpiffGuarantee"]);
                NewStandard.ps_LessServiceContracts = Convert.ToDecimal(Request.Form["NewStandard_LessServiceContracts"]);
                NewStandard.ps_VolumeBonusLevel1 = Convert.ToDecimal(Request.Form["NewStandard_VolumeBonusLevel1"]);
                NewStandard.ps_VolumeBonusLevel2 = Convert.ToDecimal(Request.Form["NewStandard_VolumeBonusLevel2"]);
                NewStandard.ps_VolumeBonusLevel3 = Convert.ToDecimal(Request.Form["NewStandard_VolumeBonusLevel3"]);
                NewStandard.ps_VolumeBonusLevel4 = Convert.ToDecimal(Request.Form["NewStandard_VolumeBonusLevel4"]);
                NewStandard.ps_VolumeBonusLevel5 = Convert.ToDecimal(Request.Form["NewStandard_VolumeBonusLevel5"]);
                NewStandard.ps_VolumeBonusLevel6 = Convert.ToDecimal(Request.Form["NewStandard_VolumeBonusLevel6"]);
                NewStandard.ps_VolumeBonusLevel7 = Convert.ToDecimal(Request.Form["NewStandard_VolumeBonusLevel7"]);
                NewStandard.ps_VolumeBonusLevel8 = 0;
                NewStandard.ps_VolumeBonusLevel9 = 0;

                //Do this to prevent SQL from erroring out, these are not saved...
                NewStandard.ps_AddDate = DateTime.Now;
                NewStandard.ps_UpdateDate = DateTime.Now;

                var success1 = SqlQueries.SavePayscale(NewStandard);

               
                //SAVE THE Used STANDARD
                var UsedStandard = new NewPayscale();

                UsedStandard.ps_Key = Convert.ToInt16(Request.Form["UsedStandard_Key"]);
                UsedStandard.ps_PlanCode = payscaleId;
                UsedStandard.ps_PayLevel = "STD";
                UsedStandard.ps_NewUsedHandy = "USED";
                UsedStandard.ps_MonthYear = monthId.ToString() + "/" + yearId.ToString();
                UsedStandard.ps_UpdateUser = Session["UserName"].ToString();
                UsedStandard.ps_BaseCommission = Convert.ToDecimal(Request.Form["UsedStandard_Base"]);
                UsedStandard.ps_FullBPP = Convert.ToDecimal(Request.Form["UsedStandard_FullBPP"]);
                UsedStandard.ps_HalfBPP = Convert.ToDecimal(Request.Form["UsedStandard_HalfBPP"]);
                UsedStandard.ps_TradeIn = Convert.ToDecimal(Request.Form["UsedStandard_TradeIn"]);
                UsedStandard.ps_FinanceLease = Convert.ToDecimal(Request.Form["UsedStandard_FinanceLease"]);
                UsedStandard.ps_ServiceContract = Convert.ToDecimal(Request.Form["UsedStandard_ServiceContract"]);
                UsedStandard.ps_Maintenance = Convert.ToDecimal(Request.Form["UsedStandard_Maintenance"]);
                UsedStandard.ps_GAP = Convert.ToDecimal(Request.Form["UsedStandard_GAP"]);
                UsedStandard.ps_AftermarketPerItem = Convert.ToDecimal(Request.Form["UsedStandard_AftermarketPerItem"]);
                UsedStandard.ps_InternalSurvey = Convert.ToDecimal(Request.Form["UsedStandard_InternalSurvey"]);
                UsedStandard.ps_ManufacturerSalesSatisfaction = Convert.ToDecimal(Request.Form["UsedStandard_ManufacturerSalesSatisfaction"]);
                UsedStandard.ps_ManufacturerSpiffGuarantee = Convert.ToDecimal(Request.Form["UsedStandard_ManufacturerSpiffGuarantee"]);
                UsedStandard.ps_LessServiceContracts = Convert.ToDecimal(Request.Form["UsedStandard_LessServiceContracts"]);
                UsedStandard.ps_VolumeBonusLevel1 = Convert.ToDecimal(Request.Form["UsedStandard_VolumeBonusLevel1"]);
                UsedStandard.ps_VolumeBonusLevel2 = Convert.ToDecimal(Request.Form["UsedStandard_VolumeBonusLevel2"]);
                UsedStandard.ps_VolumeBonusLevel3 = Convert.ToDecimal(Request.Form["UsedStandard_VolumeBonusLevel3"]);
                UsedStandard.ps_VolumeBonusLevel4 = Convert.ToDecimal(Request.Form["UsedStandard_VolumeBonusLevel4"]);
                UsedStandard.ps_VolumeBonusLevel5 = Convert.ToDecimal(Request.Form["UsedStandard_VolumeBonusLevel5"]);
                UsedStandard.ps_VolumeBonusLevel6 = Convert.ToDecimal(Request.Form["UsedStandard_VolumeBonusLevel6"]);
                UsedStandard.ps_VolumeBonusLevel7 = Convert.ToDecimal(Request.Form["UsedStandard_VolumeBonusLevel7"]);
                UsedStandard.ps_VolumeBonusLevel8 = 0;
                UsedStandard.ps_VolumeBonusLevel9 = 0;

                //Do this to prevent SQL from erroring out, these are not saved...
                UsedStandard.ps_AddDate = DateTime.Now;
                UsedStandard.ps_UpdateDate = DateTime.Now;

                var success4 = SqlQueries.SavePayscale(UsedStandard);

               

                    //Save the Handymans...
                    //SAVE THE HANDY STANDARD
                    //var HandyStandard = new NewPayscale();

                    //HandyStandard.ps_Key = Convert.ToInt16(Request.Form["HandyStandard_Key"]);
                    //HandyStandard.ps_PlanCode = payscaleId;
                    //HandyStandard.ps_PayLevel = "STD";
                    //HandyStandard.ps_NewUsedHandy = "HANDY";
                    //HandyStandard.ps_MonthYear = monthId.ToString() + "/" + yearId.ToString();
                    //HandyStandard.ps_UpdateUser = Session["UserName"].ToString();
                    //HandyStandard.ps_BaseCommission = Convert.ToDecimal(Request.Form["HandyStandard_Base"]);
                    //HandyStandard.ps_FullBPP = Convert.ToDecimal(Request.Form["HandyStandard_FullBPP"]);
                    //HandyStandard.ps_HalfBPP = Convert.ToDecimal(Request.Form["HandyStandard_HalfBPP"]);
                    //HandyStandard.ps_TradeIn = Convert.ToDecimal(Request.Form["HandyStandard_TradeIn"]);
                    //HandyStandard.ps_FinanceLease = Convert.ToDecimal(Request.Form["HandyStandard_FinanceLease"]);
                    //HandyStandard.ps_ServiceContract = Convert.ToDecimal(Request.Form["HandyStandard_ServiceContract"]);
                    //HandyStandard.ps_Maintenance = Convert.ToDecimal(Request.Form["HandyStandard_Maintenance"]);
                    //HandyStandard.ps_GAP = Convert.ToDecimal(Request.Form["HandyStandard_GAP"]);
                    //HandyStandard.ps_AftermarketPerItem = Convert.ToDecimal(Request.Form["HandyStandard_AftermarketPerItem"]);
                    //HandyStandard.ps_InternalSurvey = Convert.ToDecimal(Request.Form["HandyStandard_InternalSurvey"]);
                    //HandyStandard.ps_ManufacturerSalesSatisfaction = Convert.ToDecimal(Request.Form["HandyStandard_ManufacturerSalesSatisfaction"]);
                    //HandyStandard.ps_ManufacturerSpiffGuarantee = Convert.ToDecimal(Request.Form["HandyStandard_ManufacturerSpiffGuarantee"]);
                    //HandyStandard.ps_LessServiceContracts = Convert.ToDecimal(Request.Form["HandyStandard_LessServiceContracts"]);
                    //HandyStandard.ps_VolumeBonusLevel1 = Convert.ToDecimal(Request.Form["HandyStandard_VolumeBonusLevel1"]);
                    //HandyStandard.ps_VolumeBonusLevel2 = Convert.ToDecimal(Request.Form["HandyStandard_VolumeBonusLevel2"]);
                    //HandyStandard.ps_VolumeBonusLevel3 = Convert.ToDecimal(Request.Form["HandyStandard_VolumeBonusLevel3"]);
                    //HandyStandard.ps_VolumeBonusLevel4 = Convert.ToDecimal(Request.Form["HandyStandard_VolumeBonusLevel4"]);
                    //HandyStandard.ps_VolumeBonusLevel5 = Convert.ToDecimal(Request.Form["HandyStandard_VolumeBonusLevel5"]);
                    //HandyStandard.ps_VolumeBonusLevel6 = Convert.ToDecimal(Request.Form["HandyStandard_VolumeBonusLevel6"]);
                    //HandyStandard.ps_VolumeBonusLevel7 = Convert.ToDecimal(Request.Form["HandyStandard_VolumeBonusLevel7"]);
                    //HandyStandard.ps_VolumeBonusLevel8 = 0;
                    //HandyStandard.ps_VolumeBonusLevel9 = 0;

                    ////Do this to prevent SQL from erroring out, these are not saved...
                    //HandyStandard.ps_AddDate = DateTime.Now;
                    //HandyStandard.ps_UpdateDate = DateTime.Now;

                    //var success7 = SqlQueries.SavePayscale(HandyStandard);


            }

            payscaleModel = SqlQueries.GetPayscaleByIDAndDate(payscaleModel);

            if (payscaleModel.Payscales.Count == 0)
            {
                //Now, let's import from the previous month...
                payscaleModel = SqlQueries.UpdatePayscalesFromPrevious(payscaleModel);
            }

            return View(payscaleModel);


        }

    }
}