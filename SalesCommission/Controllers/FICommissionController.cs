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
        public ActionResult OldIndex()
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

            FICommissionModel.IncludeDeals = true;

            return View(FICommissionModel);
        }

        [HttpPost]
        public ActionResult OldIndex(FICommissionModel FICommissionModel)
        {

            FICommissionModel = SqlQueries.GetFIManagerDealsByDate(FICommissionModel);

            var objectivesStandardsModel = new ObjectivesStandardsModel();
            objectivesStandardsModel.MonthId = FICommissionModel.MonthId;
            objectivesStandardsModel.YearId = FICommissionModel.YearId;
            objectivesStandardsModel = SqlQueries.GetObjectivesAndStandardsByDate(objectivesStandardsModel);

            FICommissionModel.ObjectivesAndStandards = objectivesStandardsModel.ObjectivesAndStandards;

            FICommissionModel.FIManagerDealDetails = MapFIManagerDeals(FICommissionModel);
            FICommissionModel.MoneyDue = SqlQueries.GetAllMoneyDue();
            FICommissionModel.TitleDue = SqlQueries.GetAllTitlesDue();

            FICommissionModel.DealApprovals = SqlQueries.GetFIDealApprovalsByDate(objectivesStandardsModel.YearId, objectivesStandardsModel.MonthId);

            var includeDeals = false;
            if (Request.Form["chkIncludeDeals"] != null && Request.Form["chkIncludeDeals"] == "on")
            {
                includeDeals = true;
            }
            FICommissionModel.IncludeDeals = includeDeals;

            return View(FICommissionModel);
        }

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
            FICommissionModel.ConditionFilter = "ALL";
            FICommissionModel.IncludeDeals = true;

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
            FICommissionModel.MoneyDue = SqlQueries.GetAllMoneyDue();
            FICommissionModel.TitleDue = SqlQueries.GetAllTitlesDue();

            FICommissionModel.FIPayscales = SqlQueries.GetFIPayscaleSetupsByDate(FICommissionModel.YearId,FICommissionModel.MonthId);

            FICommissionModel.DealApprovals = SqlQueries.GetFIDealApprovalsByDate(objectivesStandardsModel.YearId, objectivesStandardsModel.MonthId);

            var includeDeals = false;
            if (Request.Form["chkIncludeDeals"] != null && Request.Form["chkIncludeDeals"] == "on")
            {
                includeDeals = true;
            }
            FICommissionModel.IncludeDeals = includeDeals;

            return View(FICommissionModel);
        }

        public List<FIManagerDealDetails> MapFIManagerDeals(FICommissionModel FICommissionModel)
        {
            var FIManagerDealMappings = new List<FIManagerDealDetails>();

            if (FICommissionModel.AftermarketDealDetails != null)
            {
                
                var otherAftermarketDealDetails = new List<AftermarketDealDetail>(FICommissionModel.AftermarketDealDetails);

                //otherAftermarketDealDetails = FICommissionModel.AftermarketDealDetails;

                var FIManagers = FICommissionModel.AftermarketDealDetails.Select(x => x.FinanceManagerNumber).Distinct().ToList();

                var otherManagerDealDetails = new FIManagerDealDetails();
                otherManagerDealDetails.FIManagerName = "Other Managers";
                otherManagerDealDetails.FIManagerLastName = "ZZZZ";
                otherManagerDealDetails.FIManagerAssociateNumber = "000";
                otherManagerDealDetails.FIManagerHireDate = DateTime.Now;
                otherManagerDealDetails.FIManagerLocation = FICommissionModel.StoreId.Replace(",","");
                otherManagerDealDetails.FIManagerSSN = FICommissionModel.StoreId.Replace(",", "") + "000";
                otherManagerDealDetails.FIDepartmentCode = FICommissionModel.StoreId.Replace(",", "");
                otherManagerDealDetails.FIDepartmentDescription = FICommissionModel.StoreId.Replace(",", "");
                otherManagerDealDetails.FIManagerPayscale = FICommissionModel.StoreId.Replace(",", "");

                foreach (var manager in FIManagers)
                {
                    var managerDealDetails = new FIManagerDealDetails();

                    var managerInformation = FICommissionModel.FIManagers.Find(x => x.AssociateNumber.Trim() == manager);
                    var managerDeals = FICommissionModel.AftermarketDealDetails.FindAll(x => x.FinanceManagerNumber == manager);

                    if (managerInformation != null)
                    {
                        otherAftermarketDealDetails.RemoveAll(x => x.FinanceManagerNumber == manager);

                        managerDealDetails.FIManagerName = managerInformation.AssociateFirstName + " " + managerInformation.AssociateLastName;
                        managerDealDetails.FIManagerLastName = managerInformation.AssociateLastName;
                        managerDealDetails.FIManagerAssociateNumber = managerInformation.AssociateNumber;
                        managerDealDetails.FIManagerHireDate = managerInformation.AssociateHireDate;
                        managerDealDetails.FIManagerLocation = managerInformation.AssociateLocation;
                        managerDealDetails.FIManagerSSN = managerInformation.AssociateSSN;
                        managerDealDetails.FIDepartmentCode = managerInformation.AssociateDepartment;
                        managerDealDetails.FIDepartmentDescription = managerInformation.AssociateDepartmentDescription;
                        managerDealDetails.FIManagerPayscale = managerInformation.AssociatePayscale;

                        var aftermarketDealDetails = new List<AftermarketDealDetail>();
                        aftermarketDealDetails = managerDeals;

                        managerDealDetails.AftermarketDealDetails = aftermarketDealDetails;

                        FIManagerDealMappings.Add(managerDealDetails);
                    }
                    else
                    {
                        //Why is this person not found
                        var x = manager;
                    }
                }


                otherManagerDealDetails.AftermarketDealDetails = otherAftermarketDealDetails;
                FIManagerDealMappings.Add(otherManagerDealDetails);

                var orderedFIManagerDealMappings = FIManagerDealMappings.OrderBy(x => x.FIManagerLastName).ToList();

                return orderedFIManagerDealMappings;
            }
            else
            {
                return FIManagerDealMappings;
            }
        }

        public ActionResult Adjustments(string id, string monthId, string yearId)
        {
            var adjustmentModel = new FIAdjustmentModel();

            if (id == null)
            {
                adjustmentModel.MonthId = DateTime.Now.Month;
                adjustmentModel.YearId = DateTime.Now.Year;
            }
            else
            {
                adjustmentModel.FinanceManagerNumber = id;
                adjustmentModel.MonthId = Int32.Parse(monthId);
                adjustmentModel.YearId = Int32.Parse(yearId);
                adjustmentModel.FIAdjustments = SqlQueries.GetFIManagerAdjustments(adjustmentModel.FinanceManagerNumber, adjustmentModel.YearId, adjustmentModel.MonthId);

                if (adjustmentModel.FIAdjustments != null)
                {
                    var chargeback1 = adjustmentModel.FIAdjustments.Find(x => x.Adjustment == "Chargeback1");
                    if (chargeback1 != null)
                    {
                        adjustmentModel.Chargeback1Id = chargeback1.Id;
                        adjustmentModel.Chargeback1 = chargeback1.Adjustment;
                        adjustmentModel.Chargeback1Type = (chargeback1.AdjustmentType != null ? chargeback1.AdjustmentType : "");
                        adjustmentModel.Chargeback1Amount = chargeback1.AdjustmentAmount;
                        adjustmentModel.Chargeback1Line = (chargeback1.AdjustmentLine != null ? chargeback1.AdjustmentLine : "");
                        adjustmentModel.Chargeback1Deal = (chargeback1.DealNumber != null ? chargeback1.DealNumber : "");
                    }


                    var chargeback2 = adjustmentModel.FIAdjustments.Find(x => x.Adjustment == "Chargeback2");
                    if (chargeback2 != null)
                    {
                        adjustmentModel.Chargeback2Id = chargeback2.Id;
                        adjustmentModel.Chargeback2 = chargeback2.Adjustment;
                        adjustmentModel.Chargeback2Type = (chargeback2.AdjustmentType != null ? chargeback2.AdjustmentType : "");
                        adjustmentModel.Chargeback2Amount = chargeback2.AdjustmentAmount;
                        adjustmentModel.Chargeback2Line = (chargeback2.AdjustmentLine != null ? chargeback2.AdjustmentLine : "");
                        adjustmentModel.Chargeback2Deal = (chargeback2.DealNumber != null ? chargeback2.DealNumber : "");
                    }

                    var chargeback3 = adjustmentModel.FIAdjustments.Find(x => x.Adjustment == "Chargeback3");
                    if (chargeback3 != null)
                    {
                        adjustmentModel.Chargeback3Id = chargeback3.Id;
                        adjustmentModel.Chargeback3 = chargeback3.Adjustment;
                        adjustmentModel.Chargeback3Type = (chargeback3.AdjustmentType != null ? chargeback3.AdjustmentType : "");
                        adjustmentModel.Chargeback3Amount = chargeback3.AdjustmentAmount;
                        adjustmentModel.Chargeback3Line = (chargeback3.AdjustmentLine != null ? chargeback3.AdjustmentLine : "");
                        adjustmentModel.Chargeback3Deal = (chargeback3.DealNumber != null ? chargeback3.DealNumber : "");
                    }

                    var chargeback4 = adjustmentModel.FIAdjustments.Find(x => x.Adjustment == "Chargeback4");
                    if (chargeback4 != null)
                    {
                        adjustmentModel.Chargeback4Id = chargeback4.Id;
                        adjustmentModel.Chargeback4 = chargeback4.Adjustment;
                        adjustmentModel.Chargeback4Type = (chargeback4.AdjustmentType != null ? chargeback4.AdjustmentType : "");
                        adjustmentModel.Chargeback4Amount = chargeback4.AdjustmentAmount;
                        adjustmentModel.Chargeback4Line = (chargeback4.AdjustmentLine != null ? chargeback4.AdjustmentLine : "");
                        adjustmentModel.Chargeback4Deal = (chargeback4.DealNumber != null ? chargeback4.DealNumber : "");
                    }

                    var difference1 = adjustmentModel.FIAdjustments.Find(x => x.Adjustment == "Difference1");
                    if (difference1 != null)
                    {
                        adjustmentModel.Differences1Id = difference1.Id;
                        adjustmentModel.Differences1 = difference1.Adjustment;
                        adjustmentModel.Differences1Type = (difference1.AdjustmentType != null ? difference1.AdjustmentType : "");
                        adjustmentModel.Differences1Amount = difference1.AdjustmentAmount;
                        adjustmentModel.Differences1Line = (difference1.AdjustmentLine != null ? difference1.AdjustmentLine : "");
                        adjustmentModel.Differences1Deal = (difference1.DealNumber != null ? difference1.DealNumber : "");
                    }

                    var difference2 = adjustmentModel.FIAdjustments.Find(x => x.Adjustment == "Difference2");
                    if (difference2 != null)
                    {
                        adjustmentModel.Differences2Id = difference2.Id;
                        adjustmentModel.Differences2 = difference2.Adjustment;
                        adjustmentModel.Differences2Type = (difference2.AdjustmentType != null ? difference2.AdjustmentType : "");
                        adjustmentModel.Differences2Amount = difference2.AdjustmentAmount;
                        adjustmentModel.Differences2Line = (difference2.AdjustmentLine != null ? difference2.AdjustmentLine : "");
                        adjustmentModel.Differences2Deal = (difference2.DealNumber != null ? difference2.DealNumber : "");
                    }

                    var difference3 = adjustmentModel.FIAdjustments.Find(x => x.Adjustment == "Difference3");
                    if (difference3 != null)
                    {
                        adjustmentModel.Differences3Id = difference3.Id;
                        adjustmentModel.Differences3 = difference3.Adjustment;
                        adjustmentModel.Differences3Type = (difference3.AdjustmentType != null ? difference3.AdjustmentType : "");
                        adjustmentModel.Differences3Amount = difference3.AdjustmentAmount;
                        adjustmentModel.Differences3Line = (difference3.AdjustmentLine != null ? difference3.AdjustmentLine : "");
                        adjustmentModel.Differences3Deal = (difference3.DealNumber != null ? difference3.DealNumber : "");
                    }

                    var difference4 = adjustmentModel.FIAdjustments.Find(x => x.Adjustment == "Difference4");
                    if (difference4 != null)
                    {
                        adjustmentModel.Differences4Id = difference4.Id;
                        adjustmentModel.Differences4 = difference4.Adjustment;
                        adjustmentModel.Differences4Type = (difference4.AdjustmentType != null ? difference4.AdjustmentType : "");
                        adjustmentModel.Differences4Amount = difference4.AdjustmentAmount;
                        adjustmentModel.Differences4Line = (difference4.AdjustmentLine != null ? difference4.AdjustmentLine : "");
                        adjustmentModel.Differences4Deal = (difference4.DealNumber != null ? difference4.DealNumber : "");
                    }

                    var quality = adjustmentModel.FIAdjustments.Find(x => x.Adjustment == "Quality1");
                    if (quality != null)
                    {
                        adjustmentModel.Quality1Id = quality.Id;
                        adjustmentModel.Quality1 = quality.Adjustment;
                        adjustmentModel.Quality1Type = (quality.AdjustmentType != null ? quality.AdjustmentType : "");
                        adjustmentModel.Quality1Amount = quality.AdjustmentAmount;
                        adjustmentModel.Quality1Line = (quality.AdjustmentLine != null ? quality.AdjustmentLine : "");
                        adjustmentModel.Quality1Deal = (quality.DealNumber != null ? quality.DealNumber : "");
                    }

                    var csi = adjustmentModel.FIAdjustments.Find(x => x.Adjustment == "CSI1");
                    if (csi != null)
                    {
                        adjustmentModel.CSI1Id = csi.Id;
                        adjustmentModel.CSI1 = csi.Adjustment;
                        adjustmentModel.CSI1Type = (csi.AdjustmentType != null ? csi.AdjustmentType : "");
                        adjustmentModel.CSI1Amount = csi.AdjustmentAmount;
                        adjustmentModel.CSI1Line = (csi.AdjustmentLine != null ? csi.AdjustmentLine : "");
                        adjustmentModel.CSI1Deal = (csi.DealNumber != null ? csi.DealNumber : "");
                    }

                    var other = adjustmentModel.FIAdjustments.Find(x => x.Adjustment == "Other1");
                    if (other != null)
                    {
                        adjustmentModel.Other1Id = other.Id;
                        adjustmentModel.Other1 = other.Adjustment;
                        adjustmentModel.Other1Type = (other.AdjustmentType != null ? other.AdjustmentType : "");
                        adjustmentModel.Other1Amount = other.AdjustmentAmount;
                        adjustmentModel.Other1Line = (other.AdjustmentLine != null ? other.AdjustmentLine : "");
                        adjustmentModel.Other1Deal = (other.DealNumber != null ? other.DealNumber : "");
                        adjustmentModel.Other1Comment = other.Comments;
                    }

                }

                adjustmentModel.FIDealNumbers = SqlQueries.GetFIManagerDealNumbers(adjustmentModel);

            }

            adjustmentModel.FIManagers = SqlQueries.GetSalesAssociates();

            adjustmentModel.AdjustmentType = SqlQueries.GetFIAdjustmentTypes();
            adjustmentModel.AdjustmentLine = SqlQueries.GetFIAdjustmentLines();
            return View(adjustmentModel);
        }

        [HttpPost]
        public ActionResult Adjustments(FIAdjustmentModel adjustmentModel)
        {

            if (Request.Form["save"] != null)
            {
                var monthYear = adjustmentModel.MonthId.ToString() + "/" + adjustmentModel.YearId.ToString();
                var adjustment1 = new FIAdjustment();

                adjustment1.MonthYear = monthYear;
                adjustment1.AssociateNumber = adjustmentModel.FinanceManagerNumber;
                adjustment1.Id = adjustmentModel.Chargeback1Id;
                adjustment1.Adjustment = "Chargeback1";
                adjustment1.AdjustmentType = adjustmentModel.Chargeback1Type;
                adjustment1.AdjustmentAmount = adjustmentModel.Chargeback1Amount;
                adjustment1.AdjustmentLine = adjustmentModel.Chargeback1Line;
                adjustment1.DealNumber = adjustmentModel.Chargeback1Deal;

                adjustment1.UpdateDate = DateTime.Now;
                adjustment1.UpdateUser = Session["UserName"].ToString();

                var success = SqlQueries.SaveFIManagerAdjustments(adjustment1);

                var adjustment2 = new FIAdjustment();
                adjustment2.MonthYear = monthYear;
                adjustment2.AssociateNumber = adjustmentModel.FinanceManagerNumber;
                adjustment2.Id = adjustmentModel.Chargeback2Id;
                adjustment2.Adjustment = "Chargeback2";
                adjustment2.AdjustmentType = adjustmentModel.Chargeback2Type;
                adjustment2.AdjustmentAmount = adjustmentModel.Chargeback2Amount;
                adjustment2.AdjustmentLine = adjustmentModel.Chargeback2Line;
                adjustment2.DealNumber = adjustmentModel.Chargeback2Deal;

                adjustment2.UpdateDate = DateTime.Now;
                adjustment2.UpdateUser = Session["UserName"].ToString();

                var success2 = SqlQueries.SaveFIManagerAdjustments(adjustment2);

                var adjustment3 = new FIAdjustment();
                adjustment3.MonthYear = monthYear;
                adjustment3.AssociateNumber = adjustmentModel.FinanceManagerNumber;
                adjustment3.Id = adjustmentModel.Chargeback3Id;
                adjustment3.Adjustment = "Chargeback3";
                adjustment3.AdjustmentType = adjustmentModel.Chargeback3Type;
                adjustment3.AdjustmentAmount = adjustmentModel.Chargeback3Amount;
                adjustment3.AdjustmentLine = adjustmentModel.Chargeback3Line;
                adjustment3.DealNumber = adjustmentModel.Chargeback3Deal;

                adjustment3.UpdateDate = DateTime.Now;
                adjustment3.UpdateUser = Session["UserName"].ToString();

                var success3 = SqlQueries.SaveFIManagerAdjustments(adjustment3);

                var adjustment4 = new FIAdjustment();
                adjustment4.MonthYear = monthYear;
                adjustment4.AssociateNumber = adjustmentModel.FinanceManagerNumber;
                adjustment4.Id = adjustmentModel.Chargeback4Id;
                adjustment4.Adjustment = "Chargeback4";
                adjustment4.AdjustmentType = adjustmentModel.Chargeback4Type;
                adjustment4.AdjustmentAmount = adjustmentModel.Chargeback4Amount;
                adjustment4.AdjustmentLine = adjustmentModel.Chargeback4Line;
                adjustment4.DealNumber = adjustmentModel.Chargeback4Deal;

                adjustment4.UpdateDate = DateTime.Now;
                adjustment4.UpdateUser = Session["UserName"].ToString();

                var success4 = SqlQueries.SaveFIManagerAdjustments(adjustment4);

                var differences1 = new FIAdjustment();
                differences1.MonthYear = monthYear;
                differences1.AssociateNumber = adjustmentModel.FinanceManagerNumber;
                differences1.Id = adjustmentModel.Differences1Id;
                differences1.Adjustment = "Difference1";
                differences1.AdjustmentType = adjustmentModel.Differences1Type;
                differences1.AdjustmentAmount = adjustmentModel.Differences1Amount;
                differences1.AdjustmentLine = adjustmentModel.Differences1Line;
                differences1.DealNumber = adjustmentModel.Differences1Deal;

                differences1.UpdateDate = DateTime.Now;
                differences1.UpdateUser = Session["UserName"].ToString();

                var success5 = SqlQueries.SaveFIManagerAdjustments(differences1);

                var differences2 = new FIAdjustment();
                differences2.MonthYear = monthYear;
                differences2.AssociateNumber = adjustmentModel.FinanceManagerNumber;
                differences2.Id = adjustmentModel.Differences2Id;
                differences2.Adjustment = "Difference2";
                differences2.AdjustmentType = adjustmentModel.Differences2Type;
                differences2.AdjustmentAmount = adjustmentModel.Differences2Amount;
                differences2.AdjustmentLine = adjustmentModel.Differences2Line;
                differences2.DealNumber = adjustmentModel.Differences2Deal;

                differences2.UpdateDate = DateTime.Now;
                differences2.UpdateUser = Session["UserName"].ToString();

                var success6 = SqlQueries.SaveFIManagerAdjustments(differences2);

                var differences3 = new FIAdjustment();
                differences3.MonthYear = monthYear;
                differences3.AssociateNumber = adjustmentModel.FinanceManagerNumber;
                differences3.Id = adjustmentModel.Differences3Id;
                differences3.Adjustment = "Difference3";
                differences3.AdjustmentType = adjustmentModel.Differences3Type;
                differences3.AdjustmentAmount = adjustmentModel.Differences3Amount;
                differences3.AdjustmentLine = adjustmentModel.Differences3Line;
                differences3.DealNumber = adjustmentModel.Differences3Deal;

                differences3.UpdateDate = DateTime.Now;
                differences3.UpdateUser = Session["UserName"].ToString();

                var success7 = SqlQueries.SaveFIManagerAdjustments(differences3);

                var differences4 = new FIAdjustment();
                differences4.MonthYear = monthYear;
                differences4.AssociateNumber = adjustmentModel.FinanceManagerNumber;
                differences4.Id = adjustmentModel.Differences4Id;
                differences4.Adjustment = "Difference4";
                differences4.AdjustmentType = adjustmentModel.Differences4Type;
                differences4.AdjustmentAmount = adjustmentModel.Differences4Amount;
                differences4.AdjustmentLine = adjustmentModel.Differences4Line;
                differences4.DealNumber = adjustmentModel.Differences4Deal;

                differences4.UpdateDate = DateTime.Now;
                differences4.UpdateUser = Session["UserName"].ToString();

                var success8 = SqlQueries.SaveFIManagerAdjustments(differences4);


                var quality1 = new FIAdjustment();
                quality1.MonthYear = monthYear;
                quality1.AssociateNumber = adjustmentModel.FinanceManagerNumber;
                quality1.Id = adjustmentModel.Quality1Id;
                quality1.Adjustment = "Quality1";
                quality1.AdjustmentType = adjustmentModel.Quality1Type;
                quality1.AdjustmentAmount = adjustmentModel.Quality1Amount;
                quality1.AdjustmentLine = adjustmentModel.Quality1Line;
                quality1.DealNumber = adjustmentModel.Quality1Deal;

                quality1.UpdateDate = DateTime.Now;
                quality1.UpdateUser = Session["UserName"].ToString();

                var success9 = SqlQueries.SaveFIManagerAdjustments(quality1);


                var csi1 = new FIAdjustment();
                csi1.MonthYear = monthYear;
                csi1.AssociateNumber = adjustmentModel.FinanceManagerNumber;
                csi1.Id = adjustmentModel.CSI1Id;
                csi1.Adjustment = "CSI1";
                csi1.AdjustmentType = adjustmentModel.CSI1Type;
                csi1.AdjustmentAmount = adjustmentModel.CSI1Amount;
                csi1.AdjustmentLine = adjustmentModel.CSI1Line;
                csi1.DealNumber = adjustmentModel.CSI1Deal;

                csi1.UpdateDate = DateTime.Now;
                csi1.UpdateUser = Session["UserName"].ToString();

                var success10 = SqlQueries.SaveFIManagerAdjustments(csi1);


                var other1 = new FIAdjustment();
                other1.MonthYear = monthYear;
                other1.AssociateNumber = adjustmentModel.FinanceManagerNumber;
                other1.Id = adjustmentModel.Other1Id;
                other1.Adjustment = "Other1";
                other1.AdjustmentType = adjustmentModel.Other1Type;
                other1.AdjustmentAmount = adjustmentModel.Other1Amount;
                other1.AdjustmentLine = adjustmentModel.Other1Line;
                other1.DealNumber = adjustmentModel.Other1Deal;
                other1.Comments = adjustmentModel.Other1Comment;

                other1.UpdateDate = DateTime.Now;
                other1.UpdateUser = Session["UserName"].ToString();

                var success11 = SqlQueries.SaveFIManagerAdjustments(other1);

            }

            adjustmentModel.AssociateInformation = SqlQueries.GetAssociateInformationByDate(adjustmentModel.FinanceManagerNumber,  adjustmentModel.YearId, adjustmentModel.MonthId);

            adjustmentModel.FIAdjustments = SqlQueries.GetFIManagerAdjustments(adjustmentModel.FinanceManagerNumber, adjustmentModel.YearId, adjustmentModel.MonthId);

            if (adjustmentModel.FIAdjustments != null)
            {
                var chargeback1 = adjustmentModel.FIAdjustments.Find(x => x.Adjustment == "Chargeback1");
                if (chargeback1 != null)
                {
                    adjustmentModel.Chargeback1Id = chargeback1.Id;
                    adjustmentModel.Chargeback1 = chargeback1.Adjustment;
                    adjustmentModel.Chargeback1Type = (chargeback1.AdjustmentType != null ? chargeback1.AdjustmentType : "");
                    adjustmentModel.Chargeback1Amount = chargeback1.AdjustmentAmount;
                    adjustmentModel.Chargeback1Line = (chargeback1.AdjustmentLine != null ? chargeback1.AdjustmentLine : "");
                    adjustmentModel.Chargeback1Deal = (chargeback1.DealNumber != null ? chargeback1.DealNumber : "");
                }


                var chargeback2 = adjustmentModel.FIAdjustments.Find(x => x.Adjustment == "Chargeback2");
                if (chargeback2 != null)
                {
                    adjustmentModel.Chargeback2Id = chargeback2.Id;
                    adjustmentModel.Chargeback2 = chargeback2.Adjustment;
                    adjustmentModel.Chargeback2Type = (chargeback2.AdjustmentType != null ? chargeback2.AdjustmentType : "");
                    adjustmentModel.Chargeback2Amount = chargeback2.AdjustmentAmount;
                    adjustmentModel.Chargeback2Line = (chargeback2.AdjustmentLine != null ? chargeback2.AdjustmentLine : "");
                    adjustmentModel.Chargeback2Deal = (chargeback2.DealNumber != null ? chargeback2.DealNumber : "");
                }

                var chargeback3 = adjustmentModel.FIAdjustments.Find(x => x.Adjustment == "Chargeback3");
                if (chargeback3 != null)
                {
                    adjustmentModel.Chargeback3Id = chargeback3.Id;
                    adjustmentModel.Chargeback3 = chargeback3.Adjustment;
                    adjustmentModel.Chargeback3Type = (chargeback3.AdjustmentType != null ? chargeback3.AdjustmentType : "");
                    adjustmentModel.Chargeback3Amount = chargeback3.AdjustmentAmount;
                    adjustmentModel.Chargeback3Line = (chargeback3.AdjustmentLine != null ? chargeback3.AdjustmentLine : "");
                    adjustmentModel.Chargeback3Deal = (chargeback3.DealNumber != null ? chargeback3.DealNumber : "");
                }

                var chargeback4 = adjustmentModel.FIAdjustments.Find(x => x.Adjustment == "Chargeback4");
                if (chargeback4 != null)
                {
                    adjustmentModel.Chargeback4Id = chargeback4.Id;
                    adjustmentModel.Chargeback4 = chargeback4.Adjustment;
                    adjustmentModel.Chargeback4Type = (chargeback4.AdjustmentType != null ? chargeback4.AdjustmentType : "");
                    adjustmentModel.Chargeback4Amount = chargeback4.AdjustmentAmount;
                    adjustmentModel.Chargeback4Line = (chargeback4.AdjustmentLine != null ? chargeback4.AdjustmentLine : "");
                    adjustmentModel.Chargeback4Deal = (chargeback4.DealNumber != null ? chargeback4.DealNumber : "");
                }

                var difference1 = adjustmentModel.FIAdjustments.Find(x => x.Adjustment == "Difference1");
                if (difference1 != null)
                {
                    adjustmentModel.Differences1Id = difference1.Id;
                    adjustmentModel.Differences1 = difference1.Adjustment;
                    adjustmentModel.Differences1Type = (difference1.AdjustmentType != null ? difference1.AdjustmentType : "");
                    adjustmentModel.Differences1Amount = difference1.AdjustmentAmount;
                    adjustmentModel.Differences1Line = (difference1.AdjustmentLine != null ? difference1.AdjustmentLine : "");
                    adjustmentModel.Differences1Deal = (difference1.DealNumber != null ? difference1.DealNumber : "");
                }

                var difference2 = adjustmentModel.FIAdjustments.Find(x => x.Adjustment == "Difference2");
                if (difference2 != null)
                {
                    adjustmentModel.Differences2Id = difference2.Id;
                    adjustmentModel.Differences2 = difference2.Adjustment;
                    adjustmentModel.Differences2Type = (difference2.AdjustmentType != null ? difference2.AdjustmentType : "");
                    adjustmentModel.Differences2Amount = difference2.AdjustmentAmount;
                    adjustmentModel.Differences2Line = (difference2.AdjustmentLine != null ? difference2.AdjustmentLine : "");
                    adjustmentModel.Differences2Deal = (difference2.DealNumber != null ? difference2.DealNumber : "");
                }

                var difference3 = adjustmentModel.FIAdjustments.Find(x => x.Adjustment == "Difference3");
                if (difference3 != null)
                {
                    adjustmentModel.Differences3Id = difference3.Id;
                    adjustmentModel.Differences3 = difference3.Adjustment;
                    adjustmentModel.Differences3Type = (difference3.AdjustmentType != null ? difference3.AdjustmentType : "");
                    adjustmentModel.Differences3Amount = difference3.AdjustmentAmount;
                    adjustmentModel.Differences3Line = (difference3.AdjustmentLine != null ? difference3.AdjustmentLine : "");
                    adjustmentModel.Differences3Deal = (difference3.DealNumber != null ? difference3.DealNumber : "");
                }

                var difference4 = adjustmentModel.FIAdjustments.Find(x => x.Adjustment == "Difference4");
                if (difference4 != null)
                {
                    adjustmentModel.Differences4Id = difference4.Id;
                    adjustmentModel.Differences4 = difference4.Adjustment;
                    adjustmentModel.Differences4Type = (difference4.AdjustmentType != null ? difference4.AdjustmentType : "");
                    adjustmentModel.Differences4Amount = difference4.AdjustmentAmount;
                    adjustmentModel.Differences4Line = (difference4.AdjustmentLine != null ? difference4.AdjustmentLine : "");
                    adjustmentModel.Differences4Deal = (difference4.DealNumber != null ? difference4.DealNumber : "");
                }

                var quality = adjustmentModel.FIAdjustments.Find(x => x.Adjustment == "Quality1");
                if (quality != null)
                {
                    adjustmentModel.Quality1Id = quality.Id;
                    adjustmentModel.Quality1 = quality.Adjustment;
                    adjustmentModel.Quality1Type = (quality.AdjustmentType != null ? quality.AdjustmentType : "");
                    adjustmentModel.Quality1Amount = quality.AdjustmentAmount;
                    adjustmentModel.Quality1Line = (quality.AdjustmentLine != null ? quality.AdjustmentLine : "");
                    adjustmentModel.Quality1Deal = (quality.DealNumber != null ? quality.DealNumber : "");
                }

                var csi = adjustmentModel.FIAdjustments.Find(x => x.Adjustment == "CSI1");
                if (csi != null)
                {
                    adjustmentModel.CSI1Id = csi.Id;
                    adjustmentModel.CSI1 = csi.Adjustment;
                    adjustmentModel.CSI1Type = (csi.AdjustmentType != null ? csi.AdjustmentType : "");
                    adjustmentModel.CSI1Amount = csi.AdjustmentAmount;
                    adjustmentModel.CSI1Line = (csi.AdjustmentLine != null ? csi.AdjustmentLine : "");
                    adjustmentModel.CSI1Deal = (csi.DealNumber != null ? csi.DealNumber : "");
                }

                var other = adjustmentModel.FIAdjustments.Find(x => x.Adjustment == "Other1");
                if (other != null)
                {
                    adjustmentModel.Other1Id = other.Id;
                    adjustmentModel.Other1 = other.Adjustment;
                    adjustmentModel.Other1Type = (other.AdjustmentType != null ? other.AdjustmentType : "");
                    adjustmentModel.Other1Amount = other.AdjustmentAmount;
                    adjustmentModel.Other1Line = (other.AdjustmentLine != null ? other.AdjustmentLine : "");
                    adjustmentModel.Other1Deal = (other.DealNumber != null ? other.DealNumber : "");
                    adjustmentModel.Other1Comment = other.Comments;
                }

            }
            adjustmentModel.FIManagers = SqlQueries.GetSalesAssociates();

            adjustmentModel.FIDealNumbers = SqlQueries.GetFIManagerDealNumbers(adjustmentModel);

            adjustmentModel.AdjustmentType = SqlQueries.GetFIAdjustmentTypes();
            adjustmentModel.AdjustmentLine = SqlQueries.GetFIAdjustmentLines();

            return View(adjustmentModel);
        }

        public ActionResult FIAssociates()
        {
            var fiAssociatesModel = new FIManagersModel();

            fiAssociatesModel.MonthId = DateTime.Now.Month;
            fiAssociatesModel.YearId = DateTime.Now.Year;
            fiAssociatesModel.FIManagers = SqlQueries.GetSalesAssociatesWithKey();
            fiAssociatesModel.FIPayscaleSelectList = SqlQueries.GetFIPayscaleSelectList().OrderBy(x => x.Text).ToList();

            return View(fiAssociatesModel);
        }

        [HttpPost]
        public ActionResult FIAssociates(FIManagersModel fiAssociatesModel)
        {

            if (Request.Form["save"] != null)
            {
                // SAVE THE INFORMATION

                var selectedManager = fiAssociatesModel.SelectedManager;

                selectedManager.UpdateDate = DateTime.Now;
                selectedManager.UpdateUser = Session["UserName"].ToString();

                var success = SqlQueries.SaveFIManagerDetails(selectedManager);

            }


            fiAssociatesModel.FIManagers = SqlQueries.GetSalesAssociatesWithKey();
            fiAssociatesModel.SelectedManager = SqlQueries.GetSelectedFIManagerByKey(fiAssociatesModel.MonthId, fiAssociatesModel.YearId, fiAssociatesModel.AssociateId);
            fiAssociatesModel.FIPayscaleSelectList = SqlQueries.GetFIPayscaleSelectList().OrderBy(x => x.Text).ToList();

            return View(fiAssociatesModel);
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
            var bNewPayScale = false;

            if (Request.Form["create"] != null)
            {
                var payscaleId = Request.Form["PayscaleId"];
                var monthId = Request.Form["MonthId"];
                var yearId = Request.Form["YearId"];

                var monthYear = monthId + "/" + yearId;
                var newPayscaleName = Request.Form["newpayscalename"];

                var payscaleFISetup = new Models.FIPayscaleSetup();
                payscaleFISetup.PlanCode = payscaleId;
                payscaleFISetup.PlanName = newPayscaleName;
                payscaleFISetup.MonthYear = monthYear;
                
                var success = SqlQueries.CreateNewFIPayscale(payscaleFISetup);

                bNewPayScale = true;
            }

            if (Request.Form["save"] != null)
            {
                // Now save all this information
                var payscaleId = Request.Form["PayscaleId"];
                var monthId = Request.Form["MonthId"];
                var yearId = Request.Form["YearId"];

                var monthYear = monthId + "/" + yearId;

                var settingIds = Request.Form["settingIds"];
                var aftermarketIds = Request.Form["aftermarketIds"];

                var grossPercent = Request.Form["grosspercent"];
                var mentorPercent = Request.Form["mentorpercent"];
                
                var payscaleFISetup = new Models.FIPayscaleSetup();               
                payscaleFISetup.PlanCode = payscaleId;
                payscaleFISetup.MonthYear = monthYear;
                payscaleFISetup.GrossPercentagePaid = decimal.Parse(grossPercent);
                payscaleFISetup.MentorPercentagePaid = decimal.Parse(mentorPercent);

                payscaleFISetup.CommissionPercentage = decimal.Parse(Request.Form["commissionpercent"]);
                payscaleFISetup.ProductBonusPercent1 = decimal.Parse(Request.Form["Bonus1percent"]);
                payscaleFISetup.ProductBonusThreshold1 = decimal.Parse(Request.Form["Bonus1threshold"]);
                payscaleFISetup.ProductBonusPercent2 = decimal.Parse(Request.Form["Bonus2percent"]);
                payscaleFISetup.ProductBonusThreshold2 = decimal.Parse(Request.Form["Bonus2threshold"]);
                payscaleFISetup.ProductBonusPercent3 = decimal.Parse(Request.Form["Bonus3percent"]);
                payscaleFISetup.ProductBonusThreshold3 = decimal.Parse(Request.Form["Bonus3threshold"]);
                payscaleFISetup.ProductBonusPercent4 = decimal.Parse(Request.Form["Bonus4percent"]);
                payscaleFISetup.ProductBonusThreshold4 = decimal.Parse(Request.Form["Bonus4threshold"]);
                payscaleFISetup.ProductBonusPercent5 = decimal.Parse(Request.Form["Bonus5percent"]);
                payscaleFISetup.ProductBonusThreshold5 = decimal.Parse(Request.Form["Bonus5threshold"]);
                payscaleFISetup.ProductBonusPercent6 = decimal.Parse(Request.Form["Bonus6percent"]);
                payscaleFISetup.ProductBonusThreshold6 = decimal.Parse(Request.Form["Bonus6threshold"]);
                payscaleFISetup.ProductBonusPercent7 = decimal.Parse(Request.Form["Bonus7percent"]);
                payscaleFISetup.ProductBonusThreshold7 = decimal.Parse(Request.Form["Bonus7threshold"]);
                payscaleFISetup.ProductBonusPercent8 = decimal.Parse(Request.Form["Bonus8percent"]);
                payscaleFISetup.ProductBonusThreshold8 = decimal.Parse(Request.Form["Bonus8threshold"]);
                payscaleFISetup.StandardFinancePerUnit = decimal.Parse(Request.Form["StandardFinancePerUnit"]);
                payscaleFISetup.StandardFinancePercent = decimal.Parse(Request.Form["StandardFinancePercent"]);
                payscaleFISetup.StandardServicePerUnit = decimal.Parse(Request.Form["StandardServicePerUnit"]);
                payscaleFISetup.StandardServicePercent = decimal.Parse(Request.Form["StandardServicePercent"]);
                payscaleFISetup.StandardMaintenancePerUnit = decimal.Parse(Request.Form["StandardMaintenancePerUnit"]);
                payscaleFISetup.StandardMaintenancePercent = decimal.Parse(Request.Form["StandardMaintenancePercent"]);
                payscaleFISetup.StandardGAPPerUnit = decimal.Parse(Request.Form["StandardGAPPerUnit"]);
                payscaleFISetup.StandardGAPPercent = decimal.Parse(Request.Form["StandardGAPPercent"]);
                payscaleFISetup.StandardZurichPerUnit = decimal.Parse(Request.Form["StandardZurichPerUnit"]);
                payscaleFISetup.StandardZurichPercent = decimal.Parse(Request.Form["StandardZurichPercent"]);
                payscaleFISetup.StandardSelectProtectPerUnit = decimal.Parse(Request.Form["StandardSelectProtectPerUnit"]);
                payscaleFISetup.StandardSelectProtectPercent = decimal.Parse(Request.Form["StandardSelectProtectPercent"]);
                payscaleFISetup.StandardTireWheelPerUnit = decimal.Parse(Request.Form["StandardTireWheelPerUnit"]);
                payscaleFISetup.StandardTireWheelPercent = decimal.Parse(Request.Form["StandardTireWheelPercent"]);

                payscaleFISetup.StandardsExpectations1 = Request.Form["StandardsExpectations1"];
                payscaleFISetup.StandardsExpectations2 = Request.Form["StandardsExpectations2"];
                payscaleFISetup.StandardsExpectations3 = Request.Form["StandardsExpectations3"];
                payscaleFISetup.StandardsExpectations4 = Request.Form["StandardsExpectations4"];
                payscaleFISetup.StandardsExpectations5 = Request.Form["StandardsExpectations5"];
                payscaleFISetup.StandardsExpectations6 = Request.Form["StandardsExpectations6"];
                payscaleFISetup.StandardsExpectations7 = Request.Form["StandardsExpectations7"];
                payscaleFISetup.StandardsExpectations8 = Request.Form["StandardsExpectations8"];
                payscaleFISetup.ActivePayscale = int.Parse(Request.Form["ActivePayscale"]);
                payscaleFISetup.PayscaleWithProducts = int.Parse(Request.Form["PayscaleWithProducts"]);

                var success = SqlQueries.SaveFIPayscaleSetup(payscaleFISetup);
                //SAVE THE SETUP

                if(settingIds != null)
                {
                    foreach(var id in settingIds.Split(','))
                    {
                        if (id != "")
                        {
                            var payscaleSettings = new Models.FIPayscale();

                            var payType = Request.Form["payType-" + id].ToString();
                            var levelAmount = Request.Form["levelAmount-" + id].ToString();
                            var payAmount = Request.Form["payAmount-" + id].ToString();

                            payscaleSettings.Id = Int32.Parse(id);
                            payscaleSettings.LevelAmount = decimal.Parse(levelAmount);
                            payscaleSettings.PayAmount = decimal.Parse(payAmount);
                            payscaleSettings.PayType = payType;
                            //payscaleSettings.PayTypeCode = payTypeCode;
                            //payscaleSettings.PayTypeName = payTypeName;
                            payscaleSettings.PlanCode = payscaleId;
                            payscaleSettings.MonthYear = monthYear;

                            payscaleSettings.UpdateDate = DateTime.Now;
                            payscaleSettings.UpdateUser = Session["UserName"].ToString();

                            //SAVE THE SETTINGS
                            var successSettings = SqlQueries.SaveFIPayscaleSettings(payscaleSettings);
                        }

                    }
                }


                if(aftermarketIds != null)
                {
                    foreach (var id in aftermarketIds.Split(','))
                    {
                        if (id != "")
                        {
                            var payscaleAftermarket = new Models.FIPayscaleAftermarket();
                            var bCore = false;
                            var bPaid = false;


                            if (Request.Form["aftermarketcore-" + id] != null)
                            {
                                var aftermarketcore = Request.Form["aftermarketcore-" + id].ToString();
                                if (aftermarketcore == "on")
                                {
                                    bCore = true;
                                }
                            }

                            if (Request.Form["aftermarketpaid-" + id] != null)
                            {
                                var aftermarketpaid = Request.Form["aftermarketpaid-" + id].ToString();
                                if (aftermarketpaid == "on")
                                {
                                    bPaid = true;
                                }

                            }

                            payscaleAftermarket.Id = Int32.Parse(id);
                            payscaleAftermarket.MonthYear = monthYear;
                            payscaleAftermarket.PlanCode = payscaleId;
                            payscaleAftermarket.PaidItem = bPaid;
                            payscaleAftermarket.CoreItem = bCore;

                            //SAVE THE AFTERMARKET ITEMS
                            var successAftermarket = SqlQueries.SaveFIPayscaleAftermarket(payscaleAftermarket);
                        }
                    }
                }


            }


            payscaleModel.MonthId = FIPayscaleModel.MonthId;
            payscaleModel.YearId = FIPayscaleModel.YearId;
            payscaleModel.PayscaleId = FIPayscaleModel.PayscaleId;
            payscaleModel.FIPayscaleSelectList = SqlQueries.GetFIPayscaleSelectList().OrderBy(x => x.Text).ToList();

            payscaleModel.FIPayscales = SqlQueries.GetFIPayscaleByIDAndDate(FIPayscaleModel.YearId, FIPayscaleModel.MonthId, FIPayscaleModel.PayscaleId);

            if((payscaleModel.FIPayscales == null || payscaleModel.FIPayscales.Count == 0) && !bNewPayScale)
            {
                var success = SqlQueries.UpdateFIPayscalesFromPrevious(payscaleModel);
                payscaleModel.FIPayscales = SqlQueries.GetFIPayscaleByIDAndDate(FIPayscaleModel.YearId, FIPayscaleModel.MonthId, FIPayscaleModel.PayscaleId);
            }

            if (!bNewPayScale)
            {
                payscaleModel.FIPayscaleAftermarket = SqlQueries.GetFIPayscaleAftermarketByIDAndDate(FIPayscaleModel.YearId, FIPayscaleModel.MonthId, FIPayscaleModel.PayscaleId);

                var payscaleSetup = SqlQueries.GetFIPayscaleSetupByIDAndDate(FIPayscaleModel.YearId, FIPayscaleModel.MonthId, FIPayscaleModel.PayscaleId);

                if (payscaleSetup != null && payscaleSetup.Count > 0)
                {
                    var setup = payscaleSetup[0];
                    payscaleModel.GrossPercentagePaid = setup.GrossPercentagePaid;
                    payscaleModel.MentorPercentagePaid = setup.MentorPercentagePaid;
                    payscaleModel.CommissionPercentage = setup.CommissionPercentage;
                    payscaleModel.ProductBonusPercent1 = setup.ProductBonusPercent1;
                    payscaleModel.ProductBonusThreshold1 = setup.ProductBonusThreshold1;
                    payscaleModel.ProductBonusPercent2 = setup.ProductBonusPercent2;
                    payscaleModel.ProductBonusThreshold2 = setup.ProductBonusThreshold2;
                    payscaleModel.ProductBonusPercent3 = setup.ProductBonusPercent3;
                    payscaleModel.ProductBonusThreshold3 = setup.ProductBonusThreshold3;
                    payscaleModel.ProductBonusPercent4 = setup.ProductBonusPercent4;
                    payscaleModel.ProductBonusThreshold4 = setup.ProductBonusThreshold4;
                    payscaleModel.ProductBonusPercent5 = setup.ProductBonusPercent5;
                    payscaleModel.ProductBonusThreshold5 = setup.ProductBonusThreshold5;
                    payscaleModel.ProductBonusPercent6 = setup.ProductBonusPercent6;
                    payscaleModel.ProductBonusThreshold6 = setup.ProductBonusThreshold6;
                    payscaleModel.ProductBonusPercent7 = setup.ProductBonusPercent7;
                    payscaleModel.ProductBonusThreshold7 = setup.ProductBonusThreshold7;
                    payscaleModel.ProductBonusPercent8 = setup.ProductBonusPercent8;
                    payscaleModel.ProductBonusThreshold8 = setup.ProductBonusThreshold8;
                    payscaleModel.StandardFinancePerUnit = setup.StandardFinancePerUnit;
                    payscaleModel.StandardFinancePercent = setup.StandardFinancePercent;
                    payscaleModel.StandardServicePerUnit = setup.StandardServicePerUnit;
                    payscaleModel.StandardServicePercent = setup.StandardServicePercent;
                    payscaleModel.StandardMaintenancePerUnit = setup.StandardMaintenancePerUnit;
                    payscaleModel.StandardMaintenancePercent = setup.StandardMaintenancePercent;
                    payscaleModel.StandardGAPPerUnit = setup.StandardGAPPerUnit;
                    payscaleModel.StandardGAPPercent = setup.StandardGAPPercent;
                    payscaleModel.StandardZurichPerUnit = setup.StandardZurichPerUnit;
                    payscaleModel.StandardZurichPercent = setup.StandardZurichPercent;
                    payscaleModel.StandardSelectProtectPerUnit = setup.StandardSelectProtectPerUnit;
                    payscaleModel.StandardSelectProtectPercent = setup.StandardSelectProtectPercent;
                    payscaleModel.StandardTireWheelPerUnit = setup.StandardTireWheelPerUnit;
                    payscaleModel.StandardTireWheelPercent = setup.StandardTireWheelPercent;

                    payscaleModel.StandardsExpectations1 = setup.StandardsExpectations1;
                    payscaleModel.StandardsExpectations2 = setup.StandardsExpectations2;
                    payscaleModel.StandardsExpectations3 = setup.StandardsExpectations3;
                    payscaleModel.StandardsExpectations4 = setup.StandardsExpectations4;
                    payscaleModel.StandardsExpectations5 = setup.StandardsExpectations5;
                    payscaleModel.StandardsExpectations6 = setup.StandardsExpectations6;
                    payscaleModel.StandardsExpectations7 = setup.StandardsExpectations7;
                    payscaleModel.StandardsExpectations8 = setup.StandardsExpectations8;
                    payscaleModel.ActivePayscale = setup.ActivePayscale;
                    payscaleModel.PayscaleWithProducts = setup.PayscaleWithProducts;
                }
            }
            return View(payscaleModel);
        }

        public ActionResult ValidateDeals(string location, string id, string monthId, string yearId)
        {

            if(Request.Form != null && Request.Form.AllKeys.Count() > 0)
            {

                var FIDealApproval = new FIDealApproval();

                var dealKey = Request.Form["dealkey"].ToString();
                var comments = Request.Form["comments"].ToString();

                FIDealApproval.DealKey = dealKey;
                FIDealApproval.Comments = comments;

                FIDealApproval.FIManagerNumber = id;
                FIDealApproval.MonthYear = monthId + "/" + yearId;

                FIDealApproval.FinanceIncomePaid = true;
                FIDealApproval.FinanceIncomeAmount = Decimal.Parse(Request.Form["hdnAmount-finance"]);

                FIDealApproval.ServiceContractPaid = true;
                FIDealApproval.ServiceContractAmount = Decimal.Parse(Request.Form["hdnAmount-service"]);

                FIDealApproval.MaintenanceContractPaid = true;
                FIDealApproval.MaintenanceContractAmount = Decimal.Parse(Request.Form["hdnAmount-maintenance"]);

                FIDealApproval.GAPPaid = true;
                FIDealApproval.GAPAmount = Decimal.Parse(Request.Form["hdnAmount-GAP"]);
                
                
                for(int i=1; i < 20; i++)
                {
                    if (Request.Form["chk-" + i] != null)
                    {
                        var bAftermarket = Request.Form["chk-" + i];
                        var aftermarketAmount = Decimal.Parse(Request.Form["hdnAmount-" + i]);
                        switch(i)
                        {
                            case 1: //BPP
                                if(bAftermarket == "on")
                                {
                                    FIDealApproval.BPPPaid = true;                                    
                                }
                                else
                                {
                                    FIDealApproval.BPPPaid = false;
                                }
                                FIDealApproval.BPPAmount = aftermarketAmount;
                                break;

                            case 2: //Nitrogen
                                if (bAftermarket == "on")
                                { 
                                    FIDealApproval.NitrogenPaid = true;
                                }
                                else
                                {
                                    FIDealApproval.NitrogenPaid = false;
                                }
                                FIDealApproval.NitrogenAmount = aftermarketAmount;
                                break;

                            case 3: //Zurichshield
                                if (bAftermarket == "on")
                                {
                                    FIDealApproval.ZurichShieldPaid = true;
                                }
                                else
                                {
                                    FIDealApproval.ZurichShieldPaid = false;
                                }
                                FIDealApproval.ZurichShieldAmount = aftermarketAmount;
                                break;

                            case 4: // Select Protect
                                if (bAftermarket == "on")
                                {
                                    FIDealApproval.SelectProtectionPaid = true;
                                }
                                else
                                {
                                    FIDealApproval.SelectProtectionPaid = false;
                                }
                                FIDealApproval.SelectProtectionAmount = aftermarketAmount;
                                break;

                            case 5: // Tire and Wheel
                                if (bAftermarket == "on")
                                {
                                    FIDealApproval.TireWheelPaid = true;
                                }
                                else
                                {                           
                                    FIDealApproval.TireWheelPaid = false;
                                }
                                FIDealApproval.TireWheelAmount = aftermarketAmount;
                                break;

                            case 6: // Key Replacement 
                                if (bAftermarket == "on")
                                {
                                    FIDealApproval.KeyReplacementPaid = true;
                                }
                                else
                                {
                                    FIDealApproval.KeyReplacementPaid = false;
                                }
                                FIDealApproval.KeyReplacementAmount = aftermarketAmount;
                                break;

                            case 7: //Windshield Protection
                                if (bAftermarket == "on")
                                {
                                    FIDealApproval.WindshieldProtectionPaid = true;
                                }
                                else
                                {
                                    FIDealApproval.WindshieldProtectionPaid = false;
                                }
                                FIDealApproval.WindshieldProtectionAmount = aftermarketAmount;
                                break;

                            case 8: //Excess Wear and Tear
                                if (bAftermarket == "on")
                                {
                                    FIDealApproval.WearAndTearPaid = true;
                                }
                                else
                                {
                                    FIDealApproval.WearAndTearPaid = false;
                                }
                                FIDealApproval.WearAndTearAmount = aftermarketAmount;
                                break;

                            case 9: //Secure Guard
                                if (bAftermarket == "on")
                                {
                                    FIDealApproval.SecureGuardPaid = true;
                                }
                                else
                                {
                                    FIDealApproval.SecureGuardPaid = false;
                                }
                                FIDealApproval.SecureGuardAmount = aftermarketAmount;
                                break;

                            case 10: //Fitzgerald Total Package
                                if (bAftermarket == "on")
                                {
                                    FIDealApproval.FitzTotalPackagePaid = true;
                                }
                                else
                                {
                                    FIDealApproval.FitzTotalPackagePaid = false;
                                }
                                FIDealApproval.FitzTotalPackageAmount = aftermarketAmount;
                                break;

                            case 11: //Rust Inhibitor/Undercoating
                                if (bAftermarket == "on")
                                {
                                    FIDealApproval.RustInhibitUnderCoatPaid = true;
                                }
                                else
                                {
                                    FIDealApproval.RustInhibitUnderCoatPaid = false;
                                }
                                FIDealApproval.RustInhibitUnderCoatAmount = aftermarketAmount;
                                break;

                            case 12: //Undercoating
                                if (bAftermarket == "on")
                                {
                                    FIDealApproval.UndercoatingPaid = true;
                                }
                                else
                                {
                                    FIDealApproval.UndercoatingPaid = false;
                                }
                                FIDealApproval.UndercoatingAmount = aftermarketAmount;
                                break;

                            case 13: //Rust Inhibitor
                                if (bAftermarket == "on")
                                {
                                    FIDealApproval.RustInhibitorPaid = true;
                                }
                                else
                                {
                                    FIDealApproval.RustInhibitorPaid = false;
                                }
                                FIDealApproval.RustInhibitorAmount = aftermarketAmount;
                                break;

                            case 14: //Ultimate Titanium - Data Dots
                                if (bAftermarket == "on")
                                {
                                    FIDealApproval.DataDotsPaid = true;
                                }
                                else
                                {
                                    FIDealApproval.DataDotsPaid = false;
                                }
                                FIDealApproval.DataDotsAmount = aftermarketAmount;
                                break;

                            case 15: //Paint and Dent Repair
                                if (bAftermarket == "on")
                                {
                                    FIDealApproval.PaintDentPaid = true;
                                }
                                else
                                {
                                    FIDealApproval.PaintDentPaid = false;
                                }
                                FIDealApproval.PaintDentAmount = aftermarketAmount;
                                break;

                            case 16: //Other Aftermarket 1
                                if (bAftermarket == "on")
                                {
                                    FIDealApproval.Miscellaneous1Paid = true;
                                }
                                else
                                {
                                    FIDealApproval.Miscellaneous1Paid = false;
                                }
                                FIDealApproval.Miscellaneous1Amount = aftermarketAmount;
                                break;

                            case 17: //Other Aftermarket 2
                                if (bAftermarket == "on")
                                {
                                    FIDealApproval.Miscellaneous2Paid = true;
                                }
                                else
                                {
                                    FIDealApproval.Miscellaneous2Paid = false;
                                }
                                FIDealApproval.Miscellaneous2Amount = aftermarketAmount;
                                break;

                            case 18:  //Other Aftermarket 3
                                if (bAftermarket == "on")
                                {
                                    FIDealApproval.Miscellaneous3Paid = true;
                                }
                                else
                                {
                                    FIDealApproval.Miscellaneous3Paid = false;
                                }
                                FIDealApproval.Miscellaneous3Amount = aftermarketAmount;
                                break;

                            case 19: //Other Aftermarket 4
                                if (bAftermarket == "on")
                                {
                                    FIDealApproval.Miscellaneous4Paid = true;
                                }
                                else
                                {
                                    FIDealApproval.Miscellaneous4Paid = false;
                                }
                                FIDealApproval.Miscellaneous4Amount = aftermarketAmount;
                                break;
                                
                        }

                    }
                }

                FIDealApproval.ApprovalDate = DateTime.Now;
                FIDealApproval.ApprovalUser = Session["UserName"].ToString();
                SqlQueries.SaveFIDealApprovals(FIDealApproval);

                if(FIDealApproval.BPPPaid && Request.Form["submit"] != "Re-Validate")
                {
                    // Now deduct $50 from the associate for the current month
                    var associateId = Request.Form["hdn-sales-associate-id"];

                    if(associateId != null && associateId != "")
                    {

                        var associateInformation = SqlQueries.GetAssociateInformationByDate(associateId, Int32.Parse(yearId), Int32.Parse(monthId));

                        var bonusDate = DateTime.Now.ToShortDateString();
                        var bonusAmount = -50;
                        var bonusComments = "F and I BPP Sale for Deal " + FIDealApproval.DealKey;

                        var associateBonus = new Bonus();

                        associateBonus.Id = Int32.Parse(id);
                        associateBonus.AssociateSSN = associateInformation.AssociateSSN;
                        associateBonus.MonthYear = monthId + "/" + yearId;

                        associateBonus.CreateDate = DateTime.Now;
                        associateBonus.CreateUser = Session["UserName"].ToString();

                        associateBonus.BonusDate = bonusDate;
                        associateBonus.BonusAmount = bonusAmount;
                        associateBonus.BonusComments = bonusComments;

                        var success = SqlQueries.SaveAssociateBonus(associateBonus);
                    }

                    if(FIDealApproval.FIManagerNumber != null && FIDealApproval.FIManagerNumber != "")
                    {

                        var associateInformation = SqlQueries.GetAssociateInformationByDate(FIDealApproval.FIManagerNumber, Int32.Parse(yearId), Int32.Parse(monthId));

                        var bonusDate = DateTime.Now.ToShortDateString();
                        var bonusAmount = 50;
                        var bonusComments = "F and I BPP Sale for Deal " + FIDealApproval.DealKey;

                        var associateBonus = new Bonus();

                        associateBonus.Id = Int32.Parse(id);
                        associateBonus.AssociateSSN = associateInformation.AssociateSSN;
                        associateBonus.MonthYear = monthId + "/" + yearId;

                        associateBonus.CreateDate = DateTime.Now;
                        associateBonus.CreateUser = Session["UserName"].ToString();

                        associateBonus.BonusDate = bonusDate;
                        associateBonus.BonusAmount = bonusAmount;
                        associateBonus.BonusComments = bonusComments;

                        var success = SqlQueries.SaveAssociateBonus(associateBonus);
                    }

                }

            }
            
            //SetUserInformation();
            var associateCommissionModel = new FIAssociateCommissionModel();

            associateCommissionModel.AssociateId = id;
            associateCommissionModel.MonthId = Int32.Parse(monthId);
            associateCommissionModel.YearId = Int32.Parse(yearId);

            if (id != null && id != "")
            {
                associateCommissionModel.AssociateInformation = SqlQueries.GetFIAssociateInformationByDate(location, associateCommissionModel.AssociateId, associateCommissionModel.YearId, associateCommissionModel.MonthId);

                
                var FICommissionModel = new FICommissionModel();
                FICommissionModel.MonthId = associateCommissionModel.MonthId;
                FICommissionModel.YearId = associateCommissionModel.YearId;
                FICommissionModel.StoreId = associateCommissionModel.AssociateInformation.AssociateLocation;

                FICommissionModel = SqlQueries.GetFIManagerDealsByDateAndId(FICommissionModel, id);


                associateCommissionModel.AftermarketDealDetails = FICommissionModel.AftermarketDealDetails;


                var associateLocation = associateCommissionModel.AssociateInformation.AssociateLocation;

                if (associateLocation == "FBS")
                {
                    associateLocation = "CDO";
                }

                var payscaleId = "";

                var managerPayscale = SqlQueries.GetSelectedFIManager(associateCommissionModel.MonthId, associateCommissionModel.YearId, associateCommissionModel.AssociateId, associateLocation);
                if (managerPayscale != null)
                {
                    payscaleId = managerPayscale.ManagerPayscaleID;
                }

                if (payscaleId == null || payscaleId == "")
                {
                    var payscaleList = SqlQueries.GetFIPayscaleSelectList();

                    payscaleId = payscaleList.Find(x => x.Value.Contains(associateLocation) && !x.Value.Contains("CST")).Value;
                }

                associateCommissionModel.FIPayscales = SqlQueries.GetFIPayscaleByIDAndDate(associateCommissionModel.YearId, associateCommissionModel.MonthId, payscaleId);

                associateCommissionModel.DealApprovals = SqlQueries.GetFIDealApprovalsByDate(Int32.Parse(yearId), Int32.Parse(monthId), id);

                associateCommissionModel.FIPayscaleAftermarket = SqlQueries.GetFIPayscaleAftermarketByIDAndDate(FICommissionModel.YearId, FICommissionModel.MonthId, payscaleId);


            }

            associateCommissionModel.MoneyDue = SqlQueries.GetAllMoneyDue();
            associateCommissionModel.TitleDue = SqlQueries.GetAllTitlesDue();

            ViewBag.IsCommissionAdmin = Session["IsCommissionAdmin"];

            return View(associateCommissionModel);
        }

        public ActionResult NewAssociate(string location, string id, string monthId, string yearId)
        {

            //SetUserInformation();
            var associateCommissionModel = new FIAssociateCommissionModel();

            associateCommissionModel.AssociateId = id;
            associateCommissionModel.MonthId = Int32.Parse(monthId);
            associateCommissionModel.YearId = Int32.Parse(yearId);

            var associateLocation = "";

            if (id != null && id != "")
            {
                associateCommissionModel.AssociateInformation = SqlQueries.GetFIAssociateInformationByDate(location, associateCommissionModel.AssociateId, associateCommissionModel.YearId, associateCommissionModel.MonthId);

                // BUTCHERY AND HACKERY UNTIL WE CAN GET THE ACTUAL EMPLOYEE NUMBER FROM REYNOLDS
                var FICommissionModel = new FICommissionModel();
                FICommissionModel.MonthId = associateCommissionModel.MonthId;
                FICommissionModel.YearId = associateCommissionModel.YearId;
                FICommissionModel.StoreId = associateCommissionModel.AssociateInformation.AssociateLocation;

                FICommissionModel = SqlQueries.GetFIManagerDealsByDateAndId(FICommissionModel, id);

                // FICommissionModel.FIManagerDealDetails = MapFIManagerDealsById(FICommissionModel);

                //END THE BUTCHERY AND HACKERY

                //var FIManagerDetails = FICommissionModel.FIManagerDealDetails.Find(x => x.FIManagerAssociateNumber.Trim() == id);
                //if (FIManagerDetails != null)
                //{ 
                associateCommissionModel.AftermarketDealDetails = FICommissionModel.AftermarketDealDetails;
                //}

                associateLocation = associateCommissionModel.AssociateInformation.AssociateLocation;

                var bNewPayscale = false;
                if (associateLocation == "FBS" || associateLocation == "CDO")
                {
                    associateLocation = "CDO";
                    bNewPayscale = true;
                }

                var payscaleId = "";

                var managerPayscale = SqlQueries.GetSelectedFIManager(associateCommissionModel.MonthId, associateCommissionModel.YearId, associateCommissionModel.AssociateId, associateLocation);
                if (managerPayscale != null)
                {
                    payscaleId = managerPayscale.ManagerPayscaleID;

                }
                if (payscaleId == null || payscaleId == "")
                {
                    var payscaleList = SqlQueries.GetFIPayscaleSelectList();

                    if (bNewPayscale)
                    {
                        payscaleId = payscaleList.Find(x => x.Value.Contains(associateLocation) && x.Text.Contains("2023")).Value;
                    }
                    else
                    {
                        payscaleId = payscaleList.Find(x => x.Value.Contains(associateLocation) && !x.Text.Contains("2023")).Value;
                    }

                    //payscaleId = payscaleList.Find(x => x.Value.Contains(associateLocation) && !x.Value.Contains("CST")).Value;
                }

                associateCommissionModel.FIPayscales = SqlQueries.GetFIPayscaleByIDAndDate(associateCommissionModel.YearId, associateCommissionModel.MonthId, payscaleId);

                var payscaleSetup = SqlQueries.GetFIPayscaleSetupByIDAndDate(associateCommissionModel.YearId, associateCommissionModel.MonthId, payscaleId);


                if (payscaleSetup != null && payscaleSetup.Count > 0)
                {
                    var setup = payscaleSetup[0];
                    associateCommissionModel.GrossPercentagePaid = setup.GrossPercentagePaid;
                    associateCommissionModel.MentorPercentagePaid = setup.MentorPercentagePaid;
                    associateCommissionModel.CommissionPercentage = setup.CommissionPercentage;
                    associateCommissionModel.ProductBonusPercent1 = setup.ProductBonusPercent1;
                    associateCommissionModel.ProductBonusThreshold1 = setup.ProductBonusThreshold1;
                    associateCommissionModel.ProductBonusPercent2 = setup.ProductBonusPercent2;
                    associateCommissionModel.ProductBonusThreshold2 = setup.ProductBonusThreshold2;
                    associateCommissionModel.ProductBonusPercent3 = setup.ProductBonusPercent3;
                    associateCommissionModel.ProductBonusThreshold3 = setup.ProductBonusThreshold3;
                    associateCommissionModel.ProductBonusPercent4 = setup.ProductBonusPercent4;
                    associateCommissionModel.ProductBonusThreshold4 = setup.ProductBonusThreshold4;
                    associateCommissionModel.ProductBonusPercent5 = setup.ProductBonusPercent5;
                    associateCommissionModel.ProductBonusThreshold5 = setup.ProductBonusThreshold5;
                    associateCommissionModel.ProductBonusPercent6 = setup.ProductBonusPercent6;
                    associateCommissionModel.ProductBonusThreshold6 = setup.ProductBonusThreshold6;
                    associateCommissionModel.ProductBonusPercent7 = setup.ProductBonusPercent7;
                    associateCommissionModel.ProductBonusThreshold7 = setup.ProductBonusThreshold7;
                    associateCommissionModel.ProductBonusPercent8 = setup.ProductBonusPercent8;
                    associateCommissionModel.ProductBonusThreshold8 = setup.ProductBonusThreshold8;
                    associateCommissionModel.StandardFinancePerUnit = setup.StandardFinancePerUnit;
                    associateCommissionModel.StandardFinancePercent = setup.StandardFinancePercent;
                    associateCommissionModel.StandardServicePerUnit = setup.StandardServicePerUnit;
                    associateCommissionModel.StandardServicePercent = setup.StandardServicePercent;
                    associateCommissionModel.StandardMaintenancePerUnit = setup.StandardMaintenancePerUnit;
                    associateCommissionModel.StandardMaintenancePercent = setup.StandardMaintenancePercent;
                    associateCommissionModel.StandardGAPPerUnit = setup.StandardGAPPerUnit;
                    associateCommissionModel.StandardGAPPercent = setup.StandardGAPPercent;
                    associateCommissionModel.StandardZurichPerUnit = setup.StandardZurichPerUnit;
                    associateCommissionModel.StandardZurichPercent = setup.StandardZurichPercent;
                    associateCommissionModel.StandardSelectProtectPerUnit = setup.StandardSelectProtectPerUnit;
                    associateCommissionModel.StandardSelectProtectPercent = setup.StandardSelectProtectPercent;
                    associateCommissionModel.StandardTireWheelPerUnit = setup.StandardTireWheelPerUnit;
                    associateCommissionModel.StandardTireWheelPercent = setup.StandardTireWheelPercent;

                    associateCommissionModel.StandardsExpectations1 = setup.StandardsExpectations1;
                    associateCommissionModel.StandardsExpectations2 = setup.StandardsExpectations2;
                    associateCommissionModel.StandardsExpectations3 = setup.StandardsExpectations3;
                    associateCommissionModel.StandardsExpectations4 = setup.StandardsExpectations4;
                    associateCommissionModel.StandardsExpectations5 = setup.StandardsExpectations5;
                    associateCommissionModel.StandardsExpectations6 = setup.StandardsExpectations6;
                    associateCommissionModel.StandardsExpectations7 = setup.StandardsExpectations7;
                    associateCommissionModel.StandardsExpectations8 = setup.StandardsExpectations8;
                    associateCommissionModel.ActivePayscale = setup.ActivePayscale;
                    associateCommissionModel.PayscaleWithProducts = setup.PayscaleWithProducts;
                }

                associateCommissionModel.DealApprovals = SqlQueries.GetFIDealApprovalsByDate(Int32.Parse(yearId), Int32.Parse(monthId), id);

                associateCommissionModel.FIPayscaleAftermarket = SqlQueries.GetFIPayscaleAftermarketByIDAndDate(FICommissionModel.YearId, FICommissionModel.MonthId, payscaleId);

            }

            associateCommissionModel.MoneyDue = SqlQueries.GetAllMoneyDue();
            associateCommissionModel.TitleDue = SqlQueries.GetAllTitlesDue();
            associateCommissionModel.MoneyDueHistory = SqlQueries.GetAllMoneyDueHistory();

            associateCommissionModel.DealApprovals = SqlQueries.GetFIDealApprovalsByDate(Int32.Parse(yearId), Int32.Parse(monthId), id);

            associateCommissionModel.FIAdjustments = SqlQueries.GetFIManagerAdjustments(id, Int32.Parse(yearId), Int32.Parse(monthId));

            var selectedFIManager = SqlQueries.GetSelectedFIManager(associateCommissionModel.MonthId, associateCommissionModel.YearId, associateCommissionModel.AssociateId, associateLocation);
            if (selectedFIManager != null)
            {
                associateCommissionModel.ManagerSalary = selectedFIManager.ManagerSalary;
            }

            associateCommissionModel.FIManagerList = SqlQueries.GetSalesAssociateList();

            ViewBag.IsCommissionAdmin = Session["IsCommissionAdmin"];

            return View(associateCommissionModel);
        }

        public ActionResult Associate(string location, string id, string monthId, string yearId)
        {

            //SetUserInformation();
            var associateCommissionModel = new FIAssociateCommissionModel();

            associateCommissionModel.AssociateId = id;
            associateCommissionModel.MonthId = Int32.Parse(monthId);
            associateCommissionModel.YearId = Int32.Parse(yearId);

            var associateLocation = "";

            if (id != null && id != "")
            {
                associateCommissionModel.AssociateInformation = SqlQueries.GetFIAssociateInformationByDate(location, associateCommissionModel.AssociateId, associateCommissionModel.YearId, associateCommissionModel.MonthId);

                // BUTCHERY AND HACKERY UNTIL WE CAN GET THE ACTUAL EMPLOYEE NUMBER FROM REYNOLDS
                var FICommissionModel = new FICommissionModel();
                FICommissionModel.MonthId = associateCommissionModel.MonthId;
                FICommissionModel.YearId = associateCommissionModel.YearId;
                FICommissionModel.StoreId = associateCommissionModel.AssociateInformation.AssociateLocation;

                FICommissionModel = SqlQueries.GetFIManagerDealsByDateAndId(FICommissionModel,id);

               // FICommissionModel.FIManagerDealDetails = MapFIManagerDealsById(FICommissionModel);

                //END THE BUTCHERY AND HACKERY

                //var FIManagerDetails = FICommissionModel.FIManagerDealDetails.Find(x => x.FIManagerAssociateNumber.Trim() == id);
                //if (FIManagerDetails != null)
                //{ 
                    associateCommissionModel.AftermarketDealDetails = FICommissionModel.AftermarketDealDetails;
                //}

                associateLocation = associateCommissionModel.AssociateInformation.AssociateLocation;

                if (associateLocation == "FBS")
                {
                    associateLocation = "CDO";
                }

                var payscaleId = "";

                var managerPayscale = SqlQueries.GetSelectedFIManager(associateCommissionModel.MonthId, associateCommissionModel.YearId, associateCommissionModel.AssociateId, associateLocation);
                if (managerPayscale != null)
                {
                    payscaleId = managerPayscale.ManagerPayscaleID;
                }
                if (payscaleId == null || payscaleId == "")
                {
                    var payscaleList = SqlQueries.GetFIPayscaleSelectList();

                    payscaleId = payscaleList.Find(x => x.Value.Contains(associateLocation) && !x.Value.Contains("CST")).Value;
                }

                associateCommissionModel.FIPayscales = SqlQueries.GetFIPayscaleByIDAndDate(associateCommissionModel.YearId, associateCommissionModel.MonthId, payscaleId);
                
                var payscaleSetup = SqlQueries.GetFIPayscaleSetupByIDAndDate(associateCommissionModel.YearId, associateCommissionModel.MonthId, payscaleId);

                if (payscaleSetup != null && payscaleSetup.Count > 0)
                {
                    var setup = payscaleSetup[0];
                    associateCommissionModel.GrossPercentagePaid = setup.GrossPercentagePaid;
                    associateCommissionModel.MentorPercentagePaid = setup.MentorPercentagePaid;
                }

                associateCommissionModel.DealApprovals = SqlQueries.GetFIDealApprovalsByDate(Int32.Parse(yearId), Int32.Parse(monthId), id);

                associateCommissionModel.FIPayscaleAftermarket = SqlQueries.GetFIPayscaleAftermarketByIDAndDate(FICommissionModel.YearId, FICommissionModel.MonthId, payscaleId);

            }

            associateCommissionModel.MoneyDue = SqlQueries.GetAllMoneyDue();
            associateCommissionModel.TitleDue = SqlQueries.GetAllTitlesDue();
            associateCommissionModel.MoneyDueHistory = SqlQueries.GetAllMoneyDueHistory();

            associateCommissionModel.DealApprovals = SqlQueries.GetFIDealApprovalsByDate(Int32.Parse(yearId), Int32.Parse(monthId), id);

            associateCommissionModel.FIAdjustments = SqlQueries.GetFIManagerAdjustments(id, Int32.Parse(yearId), Int32.Parse(monthId));

            var selectedFIManager = SqlQueries.GetSelectedFIManager(associateCommissionModel.MonthId, associateCommissionModel.YearId, associateCommissionModel.AssociateId,associateLocation);
            if (selectedFIManager != null)
            {
                associateCommissionModel.ManagerSalary = selectedFIManager.ManagerSalary;
            }

            associateCommissionModel.FIManagerList = SqlQueries.GetSalesAssociateList();

            ViewBag.IsCommissionAdmin = Session["IsCommissionAdmin"];

            return View(associateCommissionModel);
        }

        public ActionResult BonusAndDraws(string location, string id, string monthId, string yearId)
        {
            //SetUserInformation();
            var associateCommissionModel = new FIAssociateCommissionModel();

            associateCommissionModel.AssociateId = id;
            associateCommissionModel.MonthId = Int32.Parse(monthId);
            associateCommissionModel.YearId = Int32.Parse(yearId);

            if (id != null && id != "")
            {
                associateCommissionModel.AssociateInformation = SqlQueries.GetFIAssociateInformationDrawsAndBonus(location,associateCommissionModel.AssociateId, associateCommissionModel.YearId, associateCommissionModel.MonthId);
            }

            var associateLocation = associateCommissionModel.AssociateInformation.AssociateLocation;

            if (associateLocation == "FBS")
            {
                associateLocation = "CDO";
            }


            var selectedFIManager = SqlQueries.GetSelectedFIManager(associateCommissionModel.MonthId, associateCommissionModel.YearId, associateCommissionModel.AssociateId,associateLocation);
            if (selectedFIManager != null)
            {
                associateCommissionModel.ManagerSalary = selectedFIManager.ManagerSalary;
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
                if (key.Contains("-draw-id"))
                {
                    drawIds.Add(key.Replace("-draw-id", ""));
                }

                if (key.Contains("-bonus-id"))
                {
                    bonusIds.Add(key.Replace("-bonus-id", ""));
                }
            }

            foreach (var id in bonusIds)
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
                associateDraw.DrawDate = (Request.Form["new-draw-date"] != null ? Convert.ToDateTime(Request.Form["new-draw-date"]) : new DateTime(1900, 1, 1));
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