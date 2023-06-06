using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SalesCommission.Models;
using SalesCommission.Business;
using System.Net.Mail;

namespace SalesCommission.Controllers
{
    public class ReportsController : Controller
    {
        // GET: Reports
        public ActionResult Index()
        {
            //SetUserInformation();

            return View();
        }

        public ActionResult MoneyDue()
        {
            var moneyDueModel = new MoneyDueModel();

            moneyDueModel.MoneyDue = SqlQueries.GetAllMoneyDue();
            moneyDueModel.MoneyDueHistory = SqlQueries.GetAllMoneyDueHistory();
            moneyDueModel.ReportFilter = "all";
            return View(moneyDueModel);
        }

        [HttpPost]
        public ActionResult MoneyDue(string filter)
        {
            var moneyDueModel = new MoneyDueModel();

            moneyDueModel.MoneyDue = SqlQueries.GetAllMoneyDue();

            moneyDueModel.MoneyDueHistory = SqlQueries.GetAllMoneyDueHistory();

            if (filter != null)
            {
                if(filter == "funded")
                {
                    var filteredMoneyDue = new List<MoneyDue>();

                    foreach (var moneyDue in moneyDueModel.MoneyDue)
                    {
                        var moneyDueHistory = new List<MoneyDue>();
                        if (moneyDue.DealNumber != null && moneyDue.DealNumber != "")
                        {
                            moneyDueHistory = moneyDueModel.MoneyDueHistory.FindAll(x => x.Location == moneyDue.Location && x.CustomerNumber == moneyDue.CustomerNumber && x.DueFrom == moneyDue.DueFrom && x.DealNumber == moneyDue.DealNumber).OrderByDescending(x => x.CommentOrder).ToList();

                        }
                        else
                        {
                            moneyDueHistory = moneyDueModel.MoneyDueHistory.FindAll(x => x.Location == moneyDue.Location && x.CustomerNumber == moneyDue.CustomerNumber && x.DueFrom == moneyDue.DueFrom).OrderByDescending(x => x.CommentOrder).ToList();
                        }

                        var currentHistory = new SalesCommission.Models.MoneyDue();

                        if (moneyDueHistory != null && moneyDueHistory.Count > 0)
                        {
                            currentHistory = moneyDueHistory[0];

                            if (currentHistory.FundedStatus == "FUNDED")
                            {
                                filteredMoneyDue.Add(moneyDue);
                            }
                        }

                    }
                    moneyDueModel.MoneyDue = filteredMoneyDue;


                }
                else if (filter == "notfunded")
                {
                    var filteredMoneyDue = new List<MoneyDue>();

                    foreach (var moneyDue in moneyDueModel.MoneyDue)
                    {
                        var moneyDueHistory = new List<MoneyDue>();
                        if (moneyDue.DealNumber != null && moneyDue.DealNumber != "")
                        {
                            moneyDueHistory = moneyDueModel.MoneyDueHistory.FindAll(x => x.Location == moneyDue.Location && x.CustomerNumber == moneyDue.CustomerNumber && x.DueFrom == moneyDue.DueFrom && x.DealNumber == moneyDue.DealNumber).OrderByDescending(x => x.CommentOrder).ToList();

                        }
                        else
                        {
                            moneyDueHistory = moneyDueModel.MoneyDueHistory.FindAll(x => x.Location == moneyDue.Location && x.CustomerNumber == moneyDue.CustomerNumber && x.DueFrom == moneyDue.DueFrom).OrderByDescending(x => x.CommentOrder).ToList();
                        }
                       
                        var currentHistory = new SalesCommission.Models.MoneyDue();

                        if (moneyDueHistory != null && moneyDueHistory.Count > 0)
                        {
                            currentHistory = moneyDueHistory[0];

                            if (currentHistory.FundedStatus != "FUNDED")
                            {
                                filteredMoneyDue.Add(moneyDue);
                            }
                        }
                        else
                        {
                            filteredMoneyDue.Add(moneyDue);

                        }

                    }
                    moneyDueModel.MoneyDue = filteredMoneyDue;
                }


                moneyDueModel.ReportFilter = filter;
            }

            return View(moneyDueModel);
        }
        
        public ActionResult AddTitleDue(string vin = "")
        {
            var titleDue = new TitleDue();
            ViewBag.IsLookup = false;

            if(vin != null && vin != "")
            {
                ViewBag.IsLookup = true;
                var oldtitleStatus = SqlQueries.GetTitleStatus(vin, null, null);

                if (oldtitleStatus != null && oldtitleStatus.Count > 0)
                {
                    titleDue = oldtitleStatus[0];

                    if (titleDue.Model == null || titleDue.Model == "")
                    {

                        // Update vehicle trade information
                        var VehicleData = SqlQueries.GetVehicleDataByVIN(vin);
                        if (VehicleData != null && VehicleData.Model != null && VehicleData.Model != "")
                        {
                            titleDue.Make = VehicleData.Make;
                            titleDue.Model = VehicleData.Model;
                            if (VehicleData.ModelYear != null && VehicleData.ModelYear != "")
                            {
                                titleDue.Year = Int32.Parse(VehicleData.ModelYear);
                            }

                            //titleDue.Location = VehicleData.Location;
                            titleDue.StockNumber = VehicleData.StockNumber;
                            //titleDue.VIN = VehicleData.VIN;
                            titleDue.InventoryStatus = VehicleData.InventoryStatus;

                            //if (titleDue.DealDate < new DateTime(2000, 1, 1))
                            //{
                            //    titleDue.DealDate = DateTime.Now.AddDays(VehicleData.DaysInStock * -1);
                            //}
                        }
                    }
                }
                else
                {
                    var VehicleData = SqlQueries.GetVehicleDataByVIN(vin);
                    var TradeInfo = SqlQueries.GetTradeInfoByVIN(vin);

                    if (TradeInfo != null)
                    {
                        titleDue.DealDate = TradeInfo.DealDate;
                        titleDue.DealKey = TradeInfo.Dealkey;
                        titleDue.BuyerName = TradeInfo.BuyerName;
                        titleDue.FinanceManager = TradeInfo.FinanceManager;
                        titleDue.SalesAssociate1 = TradeInfo.SalesAssociate1;
                        titleDue.SalesAssociate2 = TradeInfo.SalesAssociate2;
                        titleDue.SalesManager = TradeInfo.SalesManager;

                    }

                    if (VehicleData != null)
                    {
                        titleDue.Make = VehicleData.Make;
                        titleDue.Model = VehicleData.Model;
                        if (VehicleData.ModelYear != null && VehicleData.ModelYear != "")
                        {
                            titleDue.Year = Int32.Parse(VehicleData.ModelYear);
                        }

                        titleDue.Location = VehicleData.Location;
                        titleDue.StockNumber = VehicleData.StockNumber;
                        titleDue.VIN = VehicleData.VIN;
                        titleDue.InventoryStatus = VehicleData.InventoryStatus;

                        if (titleDue.DealDate < new DateTime(2000, 1, 1))
                        {
                            titleDue.DealDate = DateTime.Now.AddDays(VehicleData.DaysInStock * -1);
                        }
                    }

                }
            }

            ViewBag.CanOfficeValidate = bool.Parse(Session["CanOfficeValidate"].ToString());

            return View(titleDue);
        }

        [HttpPost]
        public ActionResult AddTitleDue(TitleDue titleDue)
        {
            ViewBag.IsLookup = false;
            ViewBag.CanOfficeValidate = bool.Parse(Session["CanOfficeValidate"].ToString());

            if (Request.Form != null && Request.Form["btnLookup"] != null)
            {
                ViewBag.IsLookup = true;

                if (Request.Form["vinlookup"] != null && Request.Form["vinlookup"] != "")
                {
                    var vinLookup = Request.Form["vinlookup"];

                    // Lookup the VIN in AllInventory and the Sales Log to determine information...

                    //First check to see if the VIN is in TitleDue

                    var oldtitleStatus = SqlQueries.GetTitleStatus(vinLookup,null,null);

                    if (oldtitleStatus != null && oldtitleStatus.Count > 0)
                    {
                        titleDue = oldtitleStatus[0];
                        if (titleDue.Model == null || titleDue.Model == "")
                        {
                            var VehicleData = SqlQueries.GetVehicleDataByVIN(titleDue.VIN);
                            if (VehicleData != null && VehicleData.Model != null && VehicleData.Model != "")
                            {
                                titleDue.Make = VehicleData.Make;
                                titleDue.Model = VehicleData.Model;
                                if (VehicleData.ModelYear != null && VehicleData.ModelYear != "")
                                {
                                    titleDue.Year = Int32.Parse(VehicleData.ModelYear);
                                }

                                //titleDue.Location = VehicleData.Location;
                                titleDue.StockNumber = VehicleData.StockNumber;
                                //titleDue.VIN = VehicleData.VIN;
                                titleDue.InventoryStatus = VehicleData.InventoryStatus;

                                //if (titleDue.DealDate < new DateTime(2000, 1, 1))
                                //{
                                //    titleDue.DealDate = DateTime.Now.AddDays(VehicleData.DaysInStock * -1);
                                //}
                            }
                        }

                    }
                    else
                    {
                        var VehicleData = SqlQueries.GetVehicleDataByVIN(vinLookup);
                        var TradeInfo = SqlQueries.GetTradeInfoByVIN(vinLookup);

                        if (TradeInfo != null)
                        {
                            titleDue.DealDate = TradeInfo.DealDate;
                            titleDue.DealKey = TradeInfo.Dealkey;
                            titleDue.BuyerName = TradeInfo.BuyerName;
                            titleDue.FinanceManager = TradeInfo.FinanceManager;
                            titleDue.SalesAssociate1 = TradeInfo.SalesAssociate1;
                            titleDue.SalesAssociate2 = TradeInfo.SalesAssociate2;
                            titleDue.SalesManager = TradeInfo.SalesManager;

                        }

                        if (VehicleData != null)
                        {
                            titleDue.Make = VehicleData.Make;
                            titleDue.Model = VehicleData.Model;
                            if (VehicleData.ModelYear != null && VehicleData.ModelYear != "")
                            {
                                titleDue.Year = Int32.Parse(VehicleData.ModelYear);
                            }

                            titleDue.Location = VehicleData.Location;
                            titleDue.StockNumber = VehicleData.StockNumber;
                            titleDue.VIN = VehicleData.VIN;
                            titleDue.InventoryStatus = VehicleData.InventoryStatus;

                            if (titleDue.DealDate < new DateTime(2000,1,1))
                            {
                                titleDue.DealDate = DateTime.Now.AddDays(VehicleData.DaysInStock * -1);
                            }
                        }

                    }
                    
                }
            }

            if (Request.Form != null && Request.Form["btnAdd"] != null)
            {
                //Add the new titleDue
                titleDue.UpdateDate = DateTime.Now;
                titleDue.UpdateUser = Session["UserName"].ToString();

                if(titleDue.FinanceManager == null)
                {
                    titleDue.FinanceManager = "Not Set";
                }
                var saved = SqlQueries.UpdateTitleStatusByIdDealDetail(titleDue);
                
                ViewBag.Status = "New Title Status Added for " + titleDue.VIN;

                titleDue = new TitleDue();

            }

            ModelState.Clear();
            return View(titleDue);
        }

        public ActionResult TitleDue()
        {
            var titleDueModel = new TitleDueModel();

            string[] status = new string[1];
            string[] invstatus = new string[1];

            titleDueModel.status = status;
            titleDueModel.loc = "";
            titleDueModel.invstatus = invstatus;

            titleDueModel.TitleDue = SqlQueries.GetAllTitlesDue();
           // titleDueModel.TitleDueHistory = SqlQueries.GetAllTitlesDueHistory();

            return View(titleDueModel);
        }

        [HttpPost]
        public ActionResult TitleDue(string loc, string[] status, string[] invstatus, string action)
        {
            var titleDueModel = new TitleDueModel();

            if (status == null)
            {
                titleDueModel.status = new string[1];
            }
            else
            {
                titleDueModel.status = status;
            }

            titleDueModel.loc = loc;

            if (invstatus == null)
            {
                titleDueModel.invstatus = new string[1];
            }
            else
            {
                titleDueModel.invstatus = invstatus;
            }
            

            titleDueModel.TitleDue = SqlQueries.GetAllTitlesDue();

            var filteredTitleDue = new List<TitleDue>();
            var finalTitleDue = new List<TitleDue>();

            if (loc != null && loc != "" && loc != "ALL")
            {
                var locTitleDue = titleDueModel.TitleDue.FindAll(x => x.Location == loc);

                if (status != null && status.Length > 0)
                {

                    foreach (var stat in status)
                    {
                        if (stat != "" && stat != "ALL")
                        {
                            var statTitleDue = new List<TitleDue>();
                            switch (stat)
                            {
                                case "0":
                                    //no status
                                    statTitleDue = locTitleDue.FindAll(x => x.ClearTitle == false && x.TitleDueBank == false && (x.TitleDueCustomer == false || x.TitleDueCustomer == null) && x.LienDueCustomer == false && x.TitleDueInterco == false && x.TitleDueAuction == false && x.LienDueBank == false && x.OdomDueCustomer == false && x.POADueCust == false && x.PayoffDueCust == false && x.WaitingOutSTTitle == false && x.Other == false && x.DuplicateTitleAppliedFor == false);
                                    break;
                                case "2":
                                    statTitleDue = locTitleDue.FindAll(x => x.TitleDueBank == true);
                                    break;
                                case "3":
                                    statTitleDue = locTitleDue.FindAll(x => x.TitleDueCustomer == true);
                                    break;
                                case "4":
                                    statTitleDue = locTitleDue.FindAll(x => x.LienDueCustomer == true);
                                    break;
                                case "5":
                                    statTitleDue = locTitleDue.FindAll(x => x.TitleDueInterco == true);
                                    break;
                                case "6":
                                    statTitleDue = locTitleDue.FindAll(x => x.TitleDueAuction == true);
                                    break;
                                case "7":
                                    statTitleDue = locTitleDue.FindAll(x => x.LienDueBank == true);
                                    break;
                                case "8":
                                    statTitleDue = locTitleDue.FindAll(x => x.OdomDueCustomer == true);
                                    break;
                                case "9":
                                    statTitleDue = locTitleDue.FindAll(x => x.POADueCust == true);
                                    break;
                                case "10":
                                    statTitleDue = locTitleDue.FindAll(x => x.PayoffDueCust == true);
                                    break;
                                case "11":
                                    statTitleDue = locTitleDue.FindAll(x => x.WaitingOutSTTitle == true);
                                    break;
                                case "12":
                                    statTitleDue = locTitleDue.FindAll(x => x.Other == true);
                                    break;
                                case "13":
                                    statTitleDue = locTitleDue.FindAll(x => x.DuplicateTitleAppliedFor == true);
                                    break;
                                case "14":
                                    statTitleDue = locTitleDue.FindAll(x => x.NoTitleDispose == true);
                                    break;

                                case "15":
                                    statTitleDue = locTitleDue.FindAll(x => x.ElectronicTitle == true);
                                    break;


                            }
                            filteredTitleDue.AddRange(statTitleDue);
                        }
                        else
                        {
                            filteredTitleDue = locTitleDue;
                            break;
                        }
                        

                    }

                    if (invstatus != null && invstatus.Length > 0)
                    {
                        foreach (var stat in invstatus)
                        {
                            if (stat != "" && stat != "ALL")
                            {
                                var statTitleDue = new List<TitleDue>();

                                statTitleDue = filteredTitleDue.FindAll(x => x.InventoryStatus == Int32.Parse(stat));
                                finalTitleDue.AddRange(statTitleDue);
                            }
                            else
                            {
                                finalTitleDue = filteredTitleDue;
                                break;
                            }
                        }

                    }
                    else
                    {
                        finalTitleDue = filteredTitleDue;
                    }

                }
                else if (invstatus != null && invstatus.Length > 0)
                {
                    foreach (var stat in invstatus)
                    {
                        if (stat != "" && stat != "ALL")
                        {
                            var statTitleDue = new List<TitleDue>();

                            statTitleDue = locTitleDue.FindAll(x => x.InventoryStatus == Int32.Parse(stat));
                            finalTitleDue.AddRange(statTitleDue);
                        }
                        else
                        {
                            finalTitleDue = filteredTitleDue;
                            break;
                        }
                    }


                }
                else
                {
                    finalTitleDue = locTitleDue;
                }

                titleDueModel.TitleDue = finalTitleDue;

            }
            else if (status != null && status.Length > 0)
            {

                foreach (var stat in status)
                {
                    if (stat != "" && stat != "ALL")
                    {
                        var statTitleDue = new List<TitleDue>();
                        switch (stat)
                        {
                            case "0":
                                //no status
                                statTitleDue = titleDueModel.TitleDue.FindAll(x => x.ClearTitle == false && x.TitleDueBank == false && (x.TitleDueCustomer == false || x.TitleDueCustomer == null) && x.LienDueCustomer == false && x.TitleDueInterco == false && x.TitleDueAuction == false && x.LienDueBank == false && x.OdomDueCustomer == false && x.POADueCust == false && x.PayoffDueCust == false && x.WaitingOutSTTitle == false && x.Other == false && x.DuplicateTitleAppliedFor == false);
                                break;
                            case "2":
                                statTitleDue = titleDueModel.TitleDue.FindAll(x => x.TitleDueBank == true);
                                break;
                            case "3":
                                statTitleDue = titleDueModel.TitleDue.FindAll(x => x.TitleDueCustomer == true);
                                break;
                            case "4":
                                statTitleDue = titleDueModel.TitleDue.FindAll(x => x.LienDueCustomer == true);
                                break;
                            case "5":
                                statTitleDue = titleDueModel.TitleDue.FindAll(x => x.TitleDueInterco == true);
                                break;
                            case "6":
                                statTitleDue = titleDueModel.TitleDue.FindAll(x => x.TitleDueAuction == true);
                                break;
                            case "7":
                                statTitleDue = titleDueModel.TitleDue.FindAll(x => x.LienDueBank == true);
                                break;
                            case "8":
                                statTitleDue = titleDueModel.TitleDue.FindAll(x => x.OdomDueCustomer == true);
                                break;
                            case "9":
                                statTitleDue = titleDueModel.TitleDue.FindAll(x => x.POADueCust == true);
                                break;
                            case "10":
                                statTitleDue = titleDueModel.TitleDue.FindAll(x => x.PayoffDueCust == true);
                                break;
                            case "11":
                                statTitleDue = titleDueModel.TitleDue.FindAll(x => x.WaitingOutSTTitle == true);
                                break;
                            case "12":
                                statTitleDue = titleDueModel.TitleDue.FindAll(x => x.Other == true);
                                break;
                            case "13":
                                statTitleDue = titleDueModel.TitleDue.FindAll(x => x.DuplicateTitleAppliedFor == true);
                                break;
                            case "14":
                                statTitleDue = titleDueModel.TitleDue.FindAll(x => x.NoTitleDispose == true);
                                break;

                            case "15":
                                statTitleDue = titleDueModel.TitleDue.FindAll(x => x.ElectronicTitle == true);
                                break;

                        }

                        filteredTitleDue.AddRange(statTitleDue);
                    }
                    else
                    {
                        filteredTitleDue = titleDueModel.TitleDue;
                        break;
                    }
                }



                if (invstatus != null && invstatus.Length > 0)
                {
                    foreach (var stat in invstatus)
                    {
                        if (stat != "" && stat != "ALL")
                        {
                            var statTitleDue = new List<TitleDue>();

                            statTitleDue = filteredTitleDue.FindAll(x => x.InventoryStatus == Int32.Parse(stat));
                            finalTitleDue.AddRange(statTitleDue);
                        }
                        else
                        {
                            finalTitleDue = filteredTitleDue;
                            break;
                        }
                    }
                }
                else
                {
                    finalTitleDue = filteredTitleDue;
                }


                titleDueModel.TitleDue = finalTitleDue;

            }
            else if (invstatus != null && invstatus.Length > 0)
            {
                foreach (var stat in invstatus)
                {
                    if (stat != "" && stat != "ALL")
                    {
                        var statTitleDue = new List<TitleDue>();

                        statTitleDue = titleDueModel.TitleDue.FindAll(x => x.InventoryStatus == Int32.Parse(stat));
                        finalTitleDue.AddRange(statTitleDue);
                    }
                    else
                    {
                        finalTitleDue = titleDueModel.TitleDue;
                        break;
                    }
                }

                titleDueModel.TitleDue = finalTitleDue;
            }

            if (action == "on")
            {
                var olderTitleDue = titleDueModel.TitleDue.FindAll(x => x.UpdateDate < DateTime.Now.AddDays(-15));
                titleDueModel.TitleDue = olderTitleDue;
            }

            var removeDups   = titleDueModel.TitleDue.Distinct().ToList();
            titleDueModel.TitleDue = removeDups;

            titleDueModel.action = action;

            return View(titleDueModel);
        }

        public ActionResult ObjectivesAndStandards()
        {
            //SetUserInformation();

            var reportModel = new ReportsModel();
            
            if (DateTime.Now < new DateTime(DateTime.Now.Year, DateTime.Now.Month, 6))
            {
                var previousMonth = DateTime.Now.AddMonths(-1);
                reportModel.MonthId = previousMonth.Month;
                reportModel.YearId = previousMonth.Year;
            }
            else
            {
                reportModel.MonthId = DateTime.Now.Month;
                reportModel.YearId = DateTime.Now.Year;
            }

            reportModel.IncludeHandyman = false;

            var reportDate = new DateTime(reportModel.YearId, reportModel.MonthId, 1);
            reportModel.ObjectivesAndStandardsDetails = SqlQueries.GetReportingObjectivesAndStandards(reportDate, reportModel.IncludeHandyman);

            return View(reportModel);
        }

        [HttpPost]
        public ActionResult ObjectivesAndStandards(ReportsModel reportModel)
        {
            //SetUserInformation();

            var reportDate = new DateTime(reportModel.YearId, reportModel.MonthId, 1);

            var includeHandyMan = false;
            if (Request.Form["chkIncludeHandyman"] != null && Request.Form["chkIncludeHandyman"] == "on")
            {
                includeHandyMan = true;
            }

            reportModel.ObjectivesAndStandardsDetails = SqlQueries.GetReportingObjectivesAndStandards(reportDate, includeHandyMan);

            if(reportModel.StoreId != null)
            {
                reportModel.ObjectivesAndStandardsDetails = reportModel.ObjectivesAndStandardsDetails.FindAll(o => o.AutoMall.ToLower() == reportModel.StoreId.ToLower());
            }

            reportModel.IncludeHandyman = includeHandyMan;
            return View(reportModel);
        }

        public ActionResult ReportBuilder()
        {
            //SetUserInformation();

            return View();
        }

        public ActionResult Sales()
        {
            //SetUserInformation();

            var salesLogReportModel = new MonthlySalesLogReportModel();

            if (DateTime.Now < new DateTime(DateTime.Now.Year, DateTime.Now.Month, 6))
            {
                var previousMonth = DateTime.Now.AddMonths(-1);
                salesLogReportModel.MonthId = previousMonth.Month;
                salesLogReportModel.YearId = previousMonth.Year;
            }
            else
            {
                salesLogReportModel.MonthId = DateTime.Now.Month;
                salesLogReportModel.YearId = DateTime.Now.Year;
            }



            salesLogReportModel.IncludeHandyman = true;
            salesLogReportModel.ShowTransfers = true;

            salesLogReportModel = SqlQueries.GetMonthlySalesReportByDate(salesLogReportModel, salesLogReportModel.IncludeHandyman, salesLogReportModel.ShowTransfers);

            var objectivesStandardsModel = new ObjectivesStandardsModel();
            objectivesStandardsModel.YearId = salesLogReportModel.YearId;
            objectivesStandardsModel.MonthId = salesLogReportModel.MonthId;

            objectivesStandardsModel = SqlQueries.GetObjectivesAndStandardsByDate(objectivesStandardsModel);

            salesLogReportModel.ObjectivesAndStandards = objectivesStandardsModel.ObjectivesAndStandards;
            salesLogReportModel.FactoryToDealerCash = SqlQueries.GetFTDByStore(salesLogReportModel);

            salesLogReportModel.Status5 = new List<Status5>();
            //salesLogReportModel.Status5 = SqlQueries.GetStatus5CountByLocationAndDate(locationCode, salesLogReportModel.YearId, salesLogReportModel.MonthId);
            salesLogReportModel.MonthlySalesReportDetails = SeparateMonthlyReportDetails(salesLogReportModel.SalesReportDetails);
            salesLogReportModel.FiscalMonth = SqlQueries.GetFiscalMonthByMonthYear(salesLogReportModel.YearId, salesLogReportModel.MonthId);


            return View(salesLogReportModel);
        }

        public ActionResult AfterSales()
        {
            //SetUserInformation();

            var aftermarketReportModel = new AftermarketReportModel();

            if (DateTime.Now < new DateTime(DateTime.Now.Year, DateTime.Now.Month, 6))
            {
                var previousMonth = DateTime.Now.AddMonths(-1);
                aftermarketReportModel.MonthId = previousMonth.Month;
                aftermarketReportModel.YearId = previousMonth.Year;
            }
            else
            {
                aftermarketReportModel.MonthId = DateTime.Now.Month;
                aftermarketReportModel.YearId = DateTime.Now.Year;
            }



            aftermarketReportModel.ConditionId = "ALL";
            aftermarketReportModel.IncludeHandyman = true;
            aftermarketReportModel.IncludeDeals = true;
            aftermarketReportModel = SqlQueries.GetAftermarketReportByDate(aftermarketReportModel, aftermarketReportModel.IncludeHandyman);


            return View(aftermarketReportModel);
        }

        public ActionResult AfterSalesStores()
        {
            //SetUserInformation();

            var aftermarketReportModel = new AftermarketReportModel();

            if (DateTime.Now < new DateTime(DateTime.Now.Year, DateTime.Now.Month, 6))
            {
                var previousMonth = DateTime.Now.AddMonths(-1);
                aftermarketReportModel.MonthId = previousMonth.Month;
                aftermarketReportModel.YearId = previousMonth.Year;
            }
            else
            {
                aftermarketReportModel.MonthId = DateTime.Now.Month;
                aftermarketReportModel.YearId = DateTime.Now.Year;
            }



            aftermarketReportModel.ConditionId = "ALL";
            aftermarketReportModel.IncludeHandyman = true;
            aftermarketReportModel.IncludeDeals = true;
            aftermarketReportModel = SqlQueries.GetAftermarketReportByDate(aftermarketReportModel, aftermarketReportModel.IncludeHandyman);


            return View(aftermarketReportModel);
        }

        public ActionResult AfterSalesFandI()
        {
            //SetUserInformation();

            var aftermarketReportModel = new AftermarketReportModel();

            if (DateTime.Now < new DateTime(DateTime.Now.Year, DateTime.Now.Month, 6))
            {
                var previousMonth = DateTime.Now.AddMonths(-1);
                aftermarketReportModel.MonthId = previousMonth.Month;
                aftermarketReportModel.YearId = previousMonth.Year;
            }
            else
            {
                aftermarketReportModel.MonthId = DateTime.Now.Month;
                aftermarketReportModel.YearId = DateTime.Now.Year;
            }



            aftermarketReportModel.ConditionId = "ALL";
            aftermarketReportModel.IncludeHandyman = true;
            aftermarketReportModel.IncludeDeals = true;
            aftermarketReportModel = SqlQueries.GetAftermarketFIReportByDate(aftermarketReportModel, aftermarketReportModel.IncludeHandyman);

            var objectivesStandardsModel = new ObjectivesStandardsModel();
            objectivesStandardsModel.MonthId = aftermarketReportModel.MonthId;
            objectivesStandardsModel.YearId = aftermarketReportModel.YearId;
            objectivesStandardsModel = SqlQueries.GetObjectivesAndStandardsByDate(objectivesStandardsModel);

            aftermarketReportModel.ObjectivesAndStandards = objectivesStandardsModel.ObjectivesAndStandards;

            //We must remove all HOUSE deals and AS IS deals
            //foreach (var dealGroup in aftermarketReportModel.AftermarketDealGroups)
            //{

            //    var HouseRemoved = dealGroup.AftermarketDealDetails.RemoveAll(o => o.FandIManager.Contains("HOUSE"));

            //    var AsIsRemoved = dealGroup.AftermarketDealDetails.RemoveAll(o => o.FandIManager.Contains("AS IS"));

            //}

            return View(aftermarketReportModel);
        }

        public ActionResult AfterSalesAssociatesStore(string location, string monthId, string yearId)
        {

            var aftermarketReportModel = new AftermarketReportModel();

            aftermarketReportModel.MonthId = Int32.Parse(monthId);
            aftermarketReportModel.YearId = Int32.Parse(yearId);
            aftermarketReportModel.StoreId = location;

            aftermarketReportModel.IncludeHandyman = true;            
            aftermarketReportModel.IncludeDeals = true;

            aftermarketReportModel = SqlQueries.GetAftermarketReportByDateAndStore(aftermarketReportModel, aftermarketReportModel.IncludeHandyman);

            return View("AfterSalesAssociates", aftermarketReportModel);
        }

        public ActionResult AfterSalesAssociates()
        {
            //SetUserInformation();

            var aftermarketReportModel = new AftermarketReportModel();

            if (DateTime.Now < new DateTime(DateTime.Now.Year, DateTime.Now.Month, 6))
            {
                var previousMonth = DateTime.Now.AddMonths(-1);
                aftermarketReportModel.MonthId = previousMonth.Month;
                aftermarketReportModel.YearId = previousMonth.Year;
            }
            else
            {
                aftermarketReportModel.MonthId = DateTime.Now.Month;
                aftermarketReportModel.YearId = DateTime.Now.Year;
            }


            //aftermarketReportModel.ConditionId = "ALL";
            aftermarketReportModel.IncludeHandyman = true;
            aftermarketReportModel.IncludeDeals = true;
            //aftermarketReportModel = SqlQueries.GetAftermarketReportByDate(aftermarketReportModel, aftermarketReportModel.IncludeHandyman);


            return View(aftermarketReportModel);
        }

        public ActionResult Overdraws()
        {
            //SetUserInformation();

            var overdrawReportModel = new OverdrawReportModel();
            overdrawReportModel.YearId = DateTime.Now.Year;

            return View(overdrawReportModel);
        }

        [HttpPost]
        public ActionResult Overdraws(OverdrawReportModel overdrawReportModel)
        {
            //SetUserInformation();

            return View(overdrawReportModel);
        }

        public ActionResult AfterSalesBrands()
        {
            //SetUserInformation();

            var aftermarketReportModel = new AftermarketReportModel();

            if (DateTime.Now < new DateTime(DateTime.Now.Year, DateTime.Now.Month, 6))
            {
                var previousMonth = DateTime.Now.AddMonths(-1);
                aftermarketReportModel.MonthId = previousMonth.Month;
                aftermarketReportModel.YearId = previousMonth.Year;
            }
            else
            {
                aftermarketReportModel.MonthId = DateTime.Now.Month;
                aftermarketReportModel.YearId = DateTime.Now.Year;
            }

            aftermarketReportModel.ConditionId = "ALL";
            aftermarketReportModel.IncludeHandyman = true;
            aftermarketReportModel.IncludeDeals = true;
            aftermarketReportModel = SqlQueries.GetAftermarketReportByDate(aftermarketReportModel, aftermarketReportModel.IncludeHandyman);


            return View(aftermarketReportModel);
        }


        [HttpPost]
        public ActionResult AfterSalesAssociates(AftermarketReportModel aftermarketReportModel)
        {
            //SetUserInformation();

            var includeHandyMan = false;
            if (Request.Form["chkIncludeHandyman"] != null && Request.Form["chkIncludeHandyman"] == "on")
            {
                includeHandyMan = true;
            }
            aftermarketReportModel.IncludeHandyman = includeHandyMan;

            var includeDeals = false;
            if (Request.Form["chkIncludeDeals"] != null && Request.Form["chkIncludeDeals"] == "on")
            {
                includeDeals = true;
            }
            aftermarketReportModel.IncludeDeals = includeDeals;

            aftermarketReportModel = SqlQueries.GetAftermarketReportByDateAndStore(aftermarketReportModel, aftermarketReportModel.IncludeHandyman);

            return View(aftermarketReportModel);
        }

        public ActionResult AfterSalesAssociatesFilter(string id, string monthId, string yearId)
        {
            //SetUserInformation();
            var aftermarketReportModel = new AftermarketReportModel();

            aftermarketReportModel.IncludeHandyman = true;
            aftermarketReportModel.IncludeDeals = true;
            aftermarketReportModel.MonthId = Int32.Parse(monthId);
            aftermarketReportModel.YearId = Int32.Parse(yearId);

            aftermarketReportModel = SqlQueries.GetAftermarketReportByDateAndStore(aftermarketReportModel, aftermarketReportModel.IncludeHandyman, id);

            aftermarketReportModel.IsAssociateOnly = true;

            return View("AfterSalesAssociates",aftermarketReportModel);
        }

        [HttpPost]
        public ActionResult AfterSales(AftermarketReportModel aftermarketReportModel)
        {
            //SetUserInformation();

            var includeHandyMan = false;
            if (Request.Form["chkIncludeHandyman"] != null && Request.Form["chkIncludeHandyman"] == "on")
            {
                includeHandyMan = true;
            }
            aftermarketReportModel.IncludeHandyman = includeHandyMan;

            var includeDeals = false;
            if (Request.Form["chkIncludeDeals"] != null && Request.Form["chkIncludeDeals"] == "on")
            {
                includeDeals = true;
            }
            aftermarketReportModel.IncludeDeals = includeDeals;

            aftermarketReportModel = SqlQueries.GetAftermarketReportByDate(aftermarketReportModel, aftermarketReportModel.IncludeHandyman);

            if (aftermarketReportModel.SelectedStores == null || aftermarketReportModel.SelectedStores.Count() == 0)
            {
                aftermarketReportModel.SelectedStores = new List<string> { "ALL" }.ToArray();
            }

            //if (aftermarketReportModel.SelectedBrands == null || aftermarketReportModel.SelectedBrands.Count() == 0)
            //{
            //    aftermarketReportModel.SelectedBrands = new List<string> { "ALL" }.ToArray();
            //}

            if (!aftermarketReportModel.SelectedStores.Contains("ALL"))
            {
                var storeDetailsRemoved = aftermarketReportModel.AftermarketDealGroups.RemoveAll(o => !(aftermarketReportModel.SelectedStores.Contains(o.AutoMall.ToLower())));
            }

            if (aftermarketReportModel.ConditionId != "ALL")
            {
                var x = 1;

                foreach(var dealGroup in aftermarketReportModel.AftermarketDealGroups)
                {
                    if(aftermarketReportModel.ConditionId == "NEW")
                    {
                        // REMOVE THE USED ONES
                        var UsedRemoved = dealGroup.AftermarketDealDetails.RemoveAll(o => o.BrandId == "UU");
                    }
                    else
                    {
                        // REMOVE THE NEW ONES
                        var UsedRemoved = dealGroup.AftermarketDealDetails.RemoveAll(o => o.BrandId != "UU");
                    }

                }
                //var storeDetailsRemoved = aftermarketReportModel.AftermarketDealGroups.RemoveAll(o => o.AftermarketDealDetails.));
            }

            //if (!tradeReportModel.SelectedBrands.Contains("ALL"))
            //{
            //    foreach (var detail in tradeReportModel.TradeReportDetails)
            //    {
            //        var brandDetailsRemoved = detail.TradeReports.RemoveAll(o => !(tradeReportModel.SelectedBrands.Contains(o.BrandId)));
            //    }


            //}


            return View(aftermarketReportModel);
        }

        [HttpPost]
        public ActionResult AfterSalesFandI(AftermarketReportModel aftermarketReportModel)
        {
            //SetUserInformation();

            var includeHandyMan = false;
            if (Request.Form["chkIncludeHandyman"] != null && Request.Form["chkIncludeHandyman"] == "on")
            {
                includeHandyMan = true;
            }
            aftermarketReportModel.IncludeHandyman = includeHandyMan;

            var includeDeals = false;
            if (Request.Form["chkIncludeDeals"] != null && Request.Form["chkIncludeDeals"] == "on")
            {
                includeDeals = true;
            }
            aftermarketReportModel.IncludeDeals = includeDeals;

            aftermarketReportModel = SqlQueries.GetAftermarketFIReportByDate(aftermarketReportModel, aftermarketReportModel.IncludeHandyman);

            if (aftermarketReportModel.SelectedStores == null || aftermarketReportModel.SelectedStores.Count() == 0)
            {
                aftermarketReportModel.SelectedStores = new List<string> { "ALL" }.ToArray();
            }

            if (!aftermarketReportModel.SelectedStores.Contains("ALL"))
            {
                var storeDetailsRemoved = aftermarketReportModel.AftermarketDealGroups.RemoveAll(o => !(aftermarketReportModel.SelectedStores.Contains(o.AutoMall.ToLower())));
            }

            if (aftermarketReportModel.ConditionId != "ALL")
            {
                var x = 1;

                foreach (var dealGroup in aftermarketReportModel.AftermarketDealGroups)
                {
                    if (aftermarketReportModel.ConditionId == "NEW")
                    {
                        // REMOVE THE USED ONES
                        var UsedRemoved = dealGroup.AftermarketDealDetails.RemoveAll(o => o.BrandId == "UU");
                    }
                    else
                    {
                        // REMOVE THE NEW ONES
                        var UsedRemoved = dealGroup.AftermarketDealDetails.RemoveAll(o => o.BrandId != "UU");
                    }

                }
                //var storeDetailsRemoved = aftermarketReportModel.AftermarketDealGroups.RemoveAll(o => o.AftermarketDealDetails.));
            }

            var objectivesStandardsModel = new ObjectivesStandardsModel();
            objectivesStandardsModel.MonthId = aftermarketReportModel.MonthId;
            objectivesStandardsModel.YearId = aftermarketReportModel.YearId;
            objectivesStandardsModel = SqlQueries.GetObjectivesAndStandardsByDate(objectivesStandardsModel);

            aftermarketReportModel.ObjectivesAndStandards = objectivesStandardsModel.ObjectivesAndStandards;


            //We must remove all HOUSE deals and AS IS deals
            //foreach (var dealGroup in aftermarketReportModel.AftermarketDealGroups)
            //{
            //    try
            //    {
            //        var HouseRemoved = dealGroup.AftermarketDealDetails.RemoveAll(o => o.FandIManager.Contains("HOUSE"));
            //    }
            //    catch(Exception ex)
            //    {

            //    }
            //    try
            //    {
            //        var AsIsRemoved = dealGroup.AftermarketDealDetails.RemoveAll(o => o.FandIManager.Contains("AS IS"));
            //    }
            //    catch (Exception ex)
            //    {

            //    }


            //}


            return View(aftermarketReportModel);
        }

        [HttpPost]
        public ActionResult AfterSalesStores(AftermarketReportModel aftermarketReportModel)
        {
            //SetUserInformation();

            var includeHandyMan = false;
            if (Request.Form["chkIncludeHandyman"] != null && Request.Form["chkIncludeHandyman"] == "on")
            {
                includeHandyMan = true;
            }
            aftermarketReportModel.IncludeHandyman = includeHandyMan;

            var includeDeals = false;
            if (Request.Form["chkIncludeDeals"] != null && Request.Form["chkIncludeDeals"] == "on")
            {
                includeDeals = true;
            }
            aftermarketReportModel.IncludeDeals = includeDeals;

            aftermarketReportModel = SqlQueries.GetAftermarketReportByDate(aftermarketReportModel, aftermarketReportModel.IncludeHandyman);

            if (aftermarketReportModel.SelectedStores == null || aftermarketReportModel.SelectedStores.Count() == 0)
            {
                aftermarketReportModel.SelectedStores = new List<string> { "ALL" }.ToArray();
            }

            if (!aftermarketReportModel.SelectedStores.Contains("ALL"))
            {
                var storeDetailsRemoved = aftermarketReportModel.AftermarketDealGroups.RemoveAll(o => !(aftermarketReportModel.SelectedStores.Contains(o.AutoMall.ToLower())));
            }

            if (aftermarketReportModel.ConditionId != "ALL")
            {
                var x = 1;

                foreach (var dealGroup in aftermarketReportModel.AftermarketDealGroups)
                {
                    if (aftermarketReportModel.ConditionId == "NEW")
                    {
                        // REMOVE THE USED ONES
                        var UsedRemoved = dealGroup.AftermarketDealDetails.RemoveAll(o => o.BrandId == "UU");
                    }
                    else
                    {
                        // REMOVE THE NEW ONES
                        var UsedRemoved = dealGroup.AftermarketDealDetails.RemoveAll(o => o.BrandId != "UU");
                    }

                }
                //var storeDetailsRemoved = aftermarketReportModel.AftermarketDealGroups.RemoveAll(o => o.AftermarketDealDetails.));
            }

            return View(aftermarketReportModel);
        }

        [HttpPost]
        public ActionResult AfterSalesBrands(AftermarketReportModel aftermarketReportModel)
        {
            //SetUserInformation();

            var includeHandyMan = false;
            if (Request.Form["chkIncludeHandyman"] != null && Request.Form["chkIncludeHandyman"] == "on")
            {
                includeHandyMan = true;
            }
            aftermarketReportModel.IncludeHandyman = includeHandyMan;

            var includeDeals = false;
            if (Request.Form["chkIncludeDeals"] != null && Request.Form["chkIncludeDeals"] == "on")
            {
                includeDeals = true;
            }
            aftermarketReportModel.IncludeDeals = includeDeals;

            aftermarketReportModel = SqlQueries.GetAftermarketReportByDate(aftermarketReportModel, aftermarketReportModel.IncludeHandyman);

            if (aftermarketReportModel.SelectedStores == null || aftermarketReportModel.SelectedStores.Count() == 0)
            {
                aftermarketReportModel.SelectedStores = new List<string> { "ALL" }.ToArray();
            }

            //if (aftermarketReportModel.SelectedBrands == null || aftermarketReportModel.SelectedBrands.Count() == 0)
            //{
            //    aftermarketReportModel.SelectedBrands = new List<string> { "ALL" }.ToArray();
            //}

            if (!aftermarketReportModel.SelectedStores.Contains("ALL"))
            {
                var storeDetailsRemoved = aftermarketReportModel.AftermarketDealGroups.RemoveAll(o => !(aftermarketReportModel.SelectedStores.Contains(o.AutoMall.ToLower())));
            }

            if (aftermarketReportModel.ConditionId != "ALL")
            {
                var x = 1;

                foreach (var dealGroup in aftermarketReportModel.AftermarketDealGroups)
                {
                    if (aftermarketReportModel.ConditionId == "NEW")
                    {
                        // REMOVE THE USED ONES
                        var UsedRemoved = dealGroup.AftermarketDealDetails.RemoveAll(o => o.BrandId == "UU");
                    }
                    else
                    {
                        // REMOVE THE NEW ONES
                        var UsedRemoved = dealGroup.AftermarketDealDetails.RemoveAll(o => o.BrandId != "UU");
                    }

                }
                //var storeDetailsRemoved = aftermarketReportModel.AftermarketDealGroups.RemoveAll(o => o.AftermarketDealDetails.));
            }

            //if (!tradeReportModel.SelectedBrands.Contains("ALL"))
            //{
            //    foreach (var detail in tradeReportModel.TradeReportDetails)
            //    {
            //        var brandDetailsRemoved = detail.TradeReports.RemoveAll(o => !(tradeReportModel.SelectedBrands.Contains(o.BrandId)));
            //    }


            //}


            return View(aftermarketReportModel);
        }



        public ActionResult Trades()
        {
            //SetUserInformation();

            var tradeReportModel = new TradeReportModel();

            if (DateTime.Now < new DateTime(DateTime.Now.Year, DateTime.Now.Month, 6))
            {
                var previousMonth = DateTime.Now.AddMonths(-1);
                tradeReportModel.MonthId = previousMonth.Month;
                tradeReportModel.YearId = previousMonth.Year;
            }
            else
            {
                tradeReportModel.MonthId = DateTime.Now.Month;
                tradeReportModel.YearId = DateTime.Now.Year;
            }

            tradeReportModel.IncludeHandyman = true;
            tradeReportModel.IncludeDeals = true;
            tradeReportModel = SqlQueries.GetMonthlyTradeReportByDate(tradeReportModel, tradeReportModel.IncludeHandyman);


            return View(tradeReportModel);
        }

        [HttpPost]
        public ActionResult Trades(TradeReportModel tradeReportModel)
        {
            //SetUserInformation();

            var includeHandyMan = false;
            if (Request.Form["chkIncludeHandyman"] != null && Request.Form["chkIncludeHandyman"] == "on")
            {
                includeHandyMan = true;
            }
            tradeReportModel.IncludeHandyman = includeHandyMan;

            var includeDeals = false;
            if (Request.Form["chkIncludeDeals"] != null && Request.Form["chkIncludeDeals"] == "on")
            {
                includeDeals = true;
            }
            tradeReportModel.IncludeDeals = includeDeals;

            tradeReportModel = SqlQueries.GetMonthlyTradeReportByDate(tradeReportModel, tradeReportModel.IncludeHandyman);

            if (tradeReportModel.SelectedStores == null || tradeReportModel.SelectedStores.Count() == 0)
            {
                tradeReportModel.SelectedStores = new List<string> { "ALL" }.ToArray();
            }

            if (tradeReportModel.SelectedBrands == null || tradeReportModel.SelectedBrands.Count() == 0)
            {
                tradeReportModel.SelectedBrands = new List<string> { "ALL" }.ToArray();
            }

            if (!tradeReportModel.SelectedStores.Contains("ALL"))
            {
                var storeDetailsRemoved = tradeReportModel.TradeReportDetails.RemoveAll(o => !(tradeReportModel.SelectedStores.Contains(o.AutoMall.ToLower())));
            }

            if (!tradeReportModel.SelectedBrands.Contains("ALL"))
            {
                foreach (var detail in tradeReportModel.TradeReportDetails)
                {
                    var brandDetailsRemoved = detail.TradeReports.RemoveAll(o => !(tradeReportModel.SelectedBrands.Contains(o.BrandId)));
                }
                

            }


            return View(tradeReportModel);
        }
       
        public List<MonthlySalesReportDetail> SeparateMonthlyReportDetails(List<SalesReportDetail> salesReportDetails)
        {
            var monthlySalesReportDetails = new List<MonthlySalesReportDetail>();

            var salesReportDetailsGroup = new MonthlySalesReportDetail();
            salesReportDetailsGroup.SalesReportDetails = new List<SalesReportDetail>();

            var autoMall = "";

            if(salesReportDetails != null && salesReportDetails.Count > 0)
            {
                autoMall = salesReportDetails[0].AutoMall;
                foreach (var store in Enums.Stores)
                {
                    if (store.StoreId.ToLower() == autoMall.ToLower())
                    {
                        salesReportDetailsGroup.AutoMallName = store.Name;
                    }
                }
                foreach (var store in Enums.SaleOrder)
                {
                    if (store.StoreId.ToLower() == autoMall.ToLower())
                    {
                        salesReportDetailsGroup.OrderId = store.OrderId;
                    }
                }
            }


            foreach (var detail in salesReportDetails)
            {
                if(detail.AutoMall != autoMall)
                {
                    monthlySalesReportDetails.Add(salesReportDetailsGroup);
                    autoMall = detail.AutoMall;

                    salesReportDetailsGroup = new MonthlySalesReportDetail();
                    salesReportDetailsGroup.SalesReportDetails = new List<SalesReportDetail>();
                    foreach (var store in Enums.Stores)
                    {
                        if (store.StoreId.ToLower() == autoMall.ToLower())
                        {
                            salesReportDetailsGroup.AutoMallName = store.Name;
                        }
                    }

                    foreach (var store in Enums.SaleOrder)
                    {
                        if(store.StoreId.ToLower() == autoMall.ToLower())
                        {
                            salesReportDetailsGroup.OrderId = store.OrderId;
                        }
                    }
                }

                salesReportDetailsGroup.SalesReportDetails.Add(detail);

            }
            monthlySalesReportDetails.Add(salesReportDetailsGroup);
            return monthlySalesReportDetails.OrderBy(x => x.AutoMallName).ToList();
        }


        [HttpPost]
        public ActionResult Sales(MonthlySalesLogReportModel salesLogReportModel)
        {
            //SetUserInformation();

            //var salesLogReportModel = new SalesLogReportModel();


            var includeHandyMan = false;
            if (Request.Form["chkIncludeHandyman"] != null && Request.Form["chkIncludeHandyman"] == "on")
            {
                includeHandyMan = true;
            }
            salesLogReportModel.IncludeHandyman = includeHandyMan;

            var showTransfers = false;
            if (Request.Form["chkShowTransfers"] != null && Request.Form["chkShowTransfers"] == "on")
            {
                showTransfers = true;
            }
            salesLogReportModel.ShowTransfers = showTransfers;

            salesLogReportModel = SqlQueries.GetMonthlySalesReportByDate(salesLogReportModel, salesLogReportModel.IncludeHandyman, salesLogReportModel.ShowTransfers);

            if(salesLogReportModel.SelectedStores == null || salesLogReportModel.SelectedStores.Count() == 0)
            {
                salesLogReportModel.SelectedStores = new List<string>{"ALL"}.ToArray();
            }

            if (salesLogReportModel.SelectedBrands == null || salesLogReportModel.SelectedBrands.Count() == 0)
            {
                salesLogReportModel.SelectedBrands = new List<string> { "ALL" }.ToArray();
            }

            if (!salesLogReportModel.SelectedStores.Contains("ALL"))
            {

                    var storeDetailsRemoved = salesLogReportModel.SalesReportDetails.RemoveAll(o => !(salesLogReportModel.SelectedStores.Contains(o.AutoMall.ToLower())));
            }

            if (!salesLogReportModel.SelectedBrands.Contains("ALL"))
            {
                    var brandDetailsRemoved = salesLogReportModel.SalesReportDetails.RemoveAll(o => !(salesLogReportModel.SelectedBrands.Contains(o.BrandId)));
               
            }

            var objectivesStandardsModel = new ObjectivesStandardsModel();
            objectivesStandardsModel.YearId = salesLogReportModel.YearId;
            objectivesStandardsModel.MonthId = salesLogReportModel.MonthId;

            objectivesStandardsModel = SqlQueries.GetObjectivesAndStandardsByDate(objectivesStandardsModel);

            salesLogReportModel.ObjectivesAndStandards = objectivesStandardsModel.ObjectivesAndStandards;
            salesLogReportModel.FactoryToDealerCash = SqlQueries.GetFTDByStore(salesLogReportModel);

            salesLogReportModel.Status5 = new List<Status5>();
            //salesLogReportModel.Status5 = SqlQueries.GetStatus5CountByLocationAndDate(locationCode, salesLogReportModel.YearId, salesLogReportModel.MonthId);
            salesLogReportModel.MonthlySalesReportDetails = SeparateMonthlyReportDetails(salesLogReportModel.SalesReportDetails);


            return View(salesLogReportModel);
        }

        public ActionResult AssociatePerformance(string monthId, string yearId, string location, string associateName)
        {
            var associatePerformanceModel = new AssociatePerformanceModel();
            associatePerformanceModel.MonthId = monthId;
            associatePerformanceModel.YearId = yearId;
            associatePerformanceModel.AssociateName = associateName;

            associatePerformanceModel = SqlQueries.GetAssociatePerformanceByDate(associatePerformanceModel);

            return View(associatePerformanceModel);
        }


        public ActionResult RevenueGenerated()
        {
            var revenueModel = new CommissionRevenueModel();

            revenueModel.MonthId = DateTime.Now.Month;
            revenueModel.YearId = DateTime.Now.Year;

            return View(revenueModel);
        }

        [HttpPost]
        public ActionResult RevenueGenerated(CommissionRevenueModel revenueModel)
        {
            revenueModel = SqlQueries.GetAssociateRevenues(revenueModel);
            return View(revenueModel);
        }

        public ActionResult BonusPaid()
        {
            var paidBonusModel = new PaidBonusModel();

            paidBonusModel.MonthId = DateTime.Now.Month;
            paidBonusModel.YearId = DateTime.Now.Year;

            paidBonusModel.PaidBonuses = SqlQueries.GetBonusesPaidByDate(paidBonusModel.YearId, paidBonusModel.MonthId);

            return View(paidBonusModel);
        }

        [HttpPost]
        public ActionResult BonusPaid(PaidBonusModel paidBonusModel)
        {
            paidBonusModel.PaidBonuses = SqlQueries.GetBonusesPaidByDate(paidBonusModel.YearId,paidBonusModel.MonthId);
            return View(paidBonusModel);
        }

        public ActionResult Leads()
        {
            //SetUserInformation();

            var leadReportModel = new LeadReportModel();



            leadReportModel.ReportStartDate = DateTime.Now.AddMonths(-1);
            leadReportModel.ReportEndDate = DateTime.Now;

            leadReportModel.IncludeHandyman = true;

            //leadReportModel = SqlQueries.GetLeadReportByDateAndStore(leadReportModel);


            return View(leadReportModel);
        }

        public ActionResult AppraisalReportLeadDetails(string stock, string vin)
        {
            var appraisalReportLeadModel = new AppraisalReportLeadDetailsModel();

            appraisalReportLeadModel.StockNumber = stock;
            appraisalReportLeadModel.VIN = vin;

            appraisalReportLeadModel.AssociateLeads = SqlQueries.GetVehicleLeadInformation(stock, vin);

            return View(appraisalReportLeadModel);
        }

        public ActionResult ExceptionReport()
        {
            var exceptionReportModel = new ExceptionReportModel();

            exceptionReportModel.DealType = "Finance";
            exceptionReportModel.ReportEndMonth = DateTime.Now.Month;
            exceptionReportModel.ReportEndYear = DateTime.Now.Year;

            exceptionReportModel.ReportStartMonth = DateTime.Now.AddMonths(-1).Month;
            exceptionReportModel.ReportStartYear = DateTime.Now.AddMonths(-1).Year;
            
            return View(exceptionReportModel);
        }

        [HttpPost]
        public ActionResult ExceptionReport(ExceptionReportModel exceptionReportModel)
        {
            if (Request.Form["breakdown1"] != null)
            {
                exceptionReportModel.BreakDownLevel1 = Request.Form["breakdown1"];
            }

            if (Request.Form["breakdown2"] != null)
            {
                exceptionReportModel.BreakDownLevel2 = Request.Form["breakdown2"];
            }

            if (Request.Form["breakdown3"] != null)
            {
                exceptionReportModel.BreakDownLevel3 = Request.Form["breakdown3"];
            }

            if (Request.Form["breakdown4"] != null)
            {
                exceptionReportModel.BreakDownLevel4 = Request.Form["breakdown4"];
            }

            var dealType = "All";
            if (Request.Form["chkDealType"] != null)
            {
                dealType = Request.Form["chkDealType"];
            }
            else if (Request.Form["chkDealType"] != null)
            {
                dealType = Request.Form["chkDealType"];
            }
            exceptionReportModel.DealType = dealType;

            var reportDate = new DateTime(Convert.ToInt32(exceptionReportModel.ReportStartYear), Convert.ToInt32(exceptionReportModel.ReportStartMonth), 4);
            var reportEndDate = new DateTime(Convert.ToInt32(exceptionReportModel.ReportEndYear), Convert.ToInt32(exceptionReportModel.ReportEndMonth), 6); //.AddMonths(1);

            exceptionReportModel.ExceptionDetails = SqlMapperUtil.StoredProcWithParams<IndividualDealDetails>("sp_SalesLogReportGetAllDealsByDateRange", new { ReportStartDate = reportDate.Date, ReportEndDate = reportEndDate.Date }, "SalesCommission");


            //Exclude the Lease Deals
            exceptionReportModel.ExceptionDetails = exceptionReportModel.ExceptionDetails.FindAll(x => x.sl_VehicleCategory != "L");

            if (dealType != "All")
            {
                if (dealType == "Finance")
                {
                    exceptionReportModel.ExceptionDetails = exceptionReportModel.ExceptionDetails.FindAll(x => x.sl_VehicleTerm != 1);

                }
                else if (dealType == "Cash")
                {
                    exceptionReportModel.ExceptionDetails = exceptionReportModel.ExceptionDetails.FindAll(x => x.sl_VehicleTerm == 1);

                }

            }

            foreach (var detail in exceptionReportModel.ExceptionDetails)
            {
                if (detail.sl_FinMgr == null)
                {
                    detail.sl_FinMgr = "None";
                }
                if (detail.sl_SalesAssociate1 == null)
                {
                    detail.sl_SalesAssociate1 = "None";
                }
                if (detail.sl_rate_exception == null || detail.sl_rate_exception == "")
                {
                    detail.sl_rate_exception = "Cash Deal";
                }
                if (detail.sl_price_variance_exception == null)
                {
                    detail.sl_price_variance_exception = "";
                }
            }

            return View(exceptionReportModel);
        }

        public ActionResult AppraisalReport()
        {
            var appraisalReportModel = new AppraisalReportModel();

            appraisalReportModel.VehicleType = "Retail";
            appraisalReportModel.AcquisitionType = "All";
            appraisalReportModel.StatusOnFM = "OnFitzMall";
            appraisalReportModel.ReportType = "sales";
            appraisalReportModel.StatusType = "1,2";

            appraisalReportModel.ReportEndMonth = DateTime.Now.Month;
            appraisalReportModel.ReportEndYear = DateTime.Now.Year;

            appraisalReportModel.ReportStartMonth = DateTime.Now.AddMonths(-1).Month;
            appraisalReportModel.ReportStartYear = DateTime.Now.AddMonths(-1).Year;

            appraisalReportModel.ReportComboEndMonth = DateTime.Now.Month;
            appraisalReportModel.ReportComboEndYear = DateTime.Now.Year;

            appraisalReportModel.ReportComboStartMonth = DateTime.Now.AddMonths(-1).Month;
            appraisalReportModel.ReportComboStartYear = DateTime.Now.AddMonths(-1).Year;

            appraisalReportModel.ReportComparisonEndMonth = DateTime.Now.AddMonths(-1).Month;
            appraisalReportModel.ReportComparisonEndYear = DateTime.Now.AddMonths(-1).Year;

            appraisalReportModel.ReportComparisonStartMonth = DateTime.Now.AddMonths(-2).Month;
            appraisalReportModel.ReportComparisonStartYear = DateTime.Now.AddMonths(-2).Year;

            return View(appraisalReportModel);
        }


        [HttpPost]
        public ActionResult AppraisalReport(AppraisalReportModel appraisalReportModel)
        {
            var vehicleType = "All";
            var acquisitionType = "All";
            var statusType = "1";
            var statusOnFM = "All";
            var breakdownIndex = 0;

            var breakdown1Filters = "";
            var breakdown2Filters = "";
            var breakdown3Filters = "";
            var breakdown4Filters = "";

            var bFilterResults = false;

            if (Request.Form["Filter"] != null && Request.Form["Filter"] == "Filter")
            {
                if (Request.Form["vt"] != null)
                {
                    vehicleType = Request.Form["vt"];
                }
                appraisalReportModel.VehicleType = vehicleType;

                if (Request.Form["at"] != null)
                {
                    acquisitionType = Request.Form["at"];
                }
                appraisalReportModel.AcquisitionType = acquisitionType;

                if (Request.Form["rt"] != null)
                {
                    appraisalReportModel.ReportType = Request.Form["rt"];
                }

                if (Request.Form["st"] != null)
                {
                    statusType = Request.Form["st"];
                }

                appraisalReportModel.StatusType = statusType;

                if (Request.Form["sfm"] != null)
                {
                    statusOnFM = Request.Form["sfm"];
                }

                appraisalReportModel.StatusOnFM = statusOnFM;

                if (Request.Form["bd1"] != null)
                {
                    appraisalReportModel.BreakDownLevel1 = Request.Form["bd1"].ToString();
                    bFilterResults = true;
                }

                if (Request.Form["bd2"] != null)
                {
                    appraisalReportModel.BreakDownLevel2 = Request.Form["bd2"].ToString();
                    bFilterResults = true;
                }

                if (Request.Form["bd3"] != null)
                {
                    appraisalReportModel.BreakDownLevel3 = Request.Form["bd3"].ToString();
                    bFilterResults = true;
                }

                if (Request.Form["bd4"] != null)
                {
                    appraisalReportModel.BreakDownLevel4 = Request.Form["bd4"].ToString();
                    bFilterResults = true;
                }

                appraisalReportModel.ReportStartYear = Convert.ToInt32(Request.Form["startyear"]);
                appraisalReportModel.ReportStartMonth = Convert.ToInt32(Request.Form["startmonth"]);

                appraisalReportModel.ReportEndYear = Convert.ToInt32(Request.Form["endyear"]);
                appraisalReportModel.ReportEndMonth = Convert.ToInt32(Request.Form["endmonth"]);

                appraisalReportModel.ReportComboStartYear = Convert.ToInt32(Request.Form["startyear"]);
                appraisalReportModel.ReportComboStartMonth = Convert.ToInt32(Request.Form["startmonth"]);

                appraisalReportModel.ReportComboEndYear = Convert.ToInt32(Request.Form["endyear"]);
                appraisalReportModel.ReportComboEndMonth = Convert.ToInt32(Request.Form["endmonth"]);

                if (Request.Form["filterBreakdown1"] != null)
                {
                    breakdown1Filters = Request.Form["filterBreakdown1"].ToString();
                    appraisalReportModel.BreakDown1Filters = breakdown1Filters;
                }

                if (Request.Form["filterBreakdown2"] != null)
                {
                    breakdown2Filters = Request.Form["filterBreakdown2"].ToString();
                    appraisalReportModel.BreakDown2Filters = breakdown2Filters;
                }

                if (Request.Form["filterBreakdown3"] != null)
                {
                    breakdown3Filters = Request.Form["filterBreakdown3"].ToString();
                    appraisalReportModel.BreakDown3Filters = breakdown3Filters;
                }

                if (Request.Form["filterBreakdown4"] != null)
                {
                    breakdown4Filters = Request.Form["filterBreakdown4"].ToString();
                    appraisalReportModel.BreakDown4Filters = breakdown4Filters;
                }


            }
            else
            {

                if (Request.Form["chkVehicleType"] != null)
                {
                    vehicleType = Request.Form["chkVehicleType"];
                }
                else if (Request.Form["chkVehicleType3"] != null)
                {
                    vehicleType = Request.Form["chkVehicleType3"];
                }

                appraisalReportModel.VehicleType = vehicleType;

                if (Request.Form["chkAcquisitionType"] != null)
                {
                    acquisitionType = Request.Form["chkAcquisitionType"];
                }
                else if (Request.Form["chkAcquisitionType2"] != null)
                {
                    acquisitionType = Request.Form["chkAcquisitionType2"];
                }
                else if (Request.Form["chkAcquisitionType3"] != null)
                {
                    acquisitionType = Request.Form["chkAcquisitionType3"];
                }

                appraisalReportModel.AcquisitionType = acquisitionType;

                if (Request.Form["reportType"] != null)
                {
                    appraisalReportModel.ReportType = Request.Form["reportType"];
                }


                if (Request.Form["chkStatusType"] != null)
                {
                    statusType = Request.Form["chkStatusType"];
                }

                appraisalReportModel.StatusType = statusType;

                if (Request.Form["chkStatusFM"] != null)
                {
                    statusOnFM = Request.Form["chkStatusFM"];
                }

                appraisalReportModel.StatusOnFM = statusOnFM;


                if (appraisalReportModel.ReportType == "sales")
                {
                    breakdownIndex = 0;
                }
                else if (appraisalReportModel.ReportType == "combined")
                {
                    breakdownIndex = 1;
                }
                else
                {
                    breakdownIndex = 2;
                }

                if (Request.Form["breakdown1"] != null)
                {
                    appraisalReportModel.BreakDownLevel1 = Request.Form["breakdown1"].ToString().Split(',')[breakdownIndex];
                }

                if (Request.Form["breakdown2"] != null && Request.Form["breakdown2"] != ",")
                {
                    appraisalReportModel.BreakDownLevel2 = Request.Form["breakdown2"].ToString().Split(',')[breakdownIndex];
                }

                if (Request.Form["breakdown3"] != null && Request.Form["breakdown3"] != "" && Request.Form["breakdown3"] != ",")
                {
                    appraisalReportModel.BreakDownLevel3 = Request.Form["breakdown3"].ToString().Split(',')[breakdownIndex];
                }

                if (Request.Form["breakdown4"] != null && Request.Form["breakdown4"] != "" && Request.Form["breakdown4"] != ",")
                {
                    appraisalReportModel.BreakDownLevel4 = Request.Form["breakdown4"].ToString().Split(',')[breakdownIndex];
                }

            }
            var reportDate = new DateTime(Convert.ToInt32(appraisalReportModel.ReportStartYear), Convert.ToInt32(appraisalReportModel.ReportStartMonth), 1);
            var reportEndDate = new DateTime(Convert.ToInt32(appraisalReportModel.ReportEndYear), Convert.ToInt32(appraisalReportModel.ReportEndMonth), 1); //.AddMonths(1);

            if(appraisalReportModel.ReportType == "combined")
            {
                reportDate = new DateTime(Convert.ToInt32(appraisalReportModel.ReportComboStartYear), Convert.ToInt32(appraisalReportModel.ReportComboStartMonth), 1);
                reportEndDate = new DateTime(Convert.ToInt32(appraisalReportModel.ReportComboEndYear), Convert.ToInt32(appraisalReportModel.ReportComboEndMonth), 1); //.AddMonths(1);
            }

            //if (appraisalReportModel.ReportType == "sales")
            //{
            appraisalReportModel.AppraisalSoldDetails = SqlMapperUtil.StoredProcWithParams<AppraisalSoldDetail>("sp_CommissionGetAssociateAppraisalsSoldDetails", new { StartDate = reportDate.Date, EndDate = reportEndDate.Date }, "ReynoldsData");

                //Remove Duplicates
                appraisalReportModel.AppraisalSoldDetails = appraisalReportModel.AppraisalSoldDetails.OrderByDescending(z => z.VIN).ThenByDescending(z => z.LastModifiedDate).GroupBy(x => x.VIN).Select(g => g.First()).ToList();

                if (vehicleType != "All")
                {
                    if (vehicleType == "Retail")
                    {
                        appraisalReportModel.AppraisalSoldDetails = appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.WholesaleRetail == "R");

                    }
                    else if (vehicleType == "Wholesale")
                    {
                        appraisalReportModel.AppraisalSoldDetails = appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.WholesaleRetail == "W");

                    }

                }

                if (acquisitionType != "ALL")
                {
                    if (acquisitionType == "Trade")
                    {
                        appraisalReportModel.AppraisalSoldDetails = appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.TradePurchase == "T");
                    }
                    else if (acquisitionType == "Purchase")
                    {
                        appraisalReportModel.AppraisalSoldDetails = appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.TradePurchase == "P");
                    }
                }

                foreach (var appraisal in appraisalReportModel.AppraisalSoldDetails)
                {
                    var stockSource = "";
                    var stockSourceName = "";
                    var lastChar = "";

                    if (appraisal.StockNumber != null && appraisal.StockNumber.Length > 1)
                    {
                        stockSource = appraisal.StockNumber.Substring(1, 1);
                        lastChar = appraisal.StockNumber.Substring(appraisal.StockNumber.Length - 1);
                    }
                    switch (stockSource)
                    {
                        case "A":
                            stockSourceName = "Auction or Wholesale";
                            break;
                        case "B":
                            stockSourceName = "Lease Buyout";
                            break;
                    case "C":
                        stockSourceName = "Car Offer";
                        break;
                    case "D":
                            stockSourceName = "Demo Prev Rental";
                            break;
                        case "E":
                            stockSourceName = "Demo";
                            break;
                        case "F":
                            stockSourceName = "Nextcar Bad Hist";
                            break;
                        case "G":
                            stockSourceName = "Government Auction";
                            break;
                        case "K":
                            stockSourceName = "Kelly ICO";
                            break;
                        case "L":
                            stockSourceName = "In House Loaner";
                            break;
                        case "M":
                            stockSourceName = "Facebook Marketplace";
                            break;
                        case "N":
                            stockSourceName = "Nextcar Purchase";
                            break;
                        case "P":
                            stockSourceName = "Purchase/Lease Cust";
                            break;
                        case "R":
                            stockSourceName = "Rental";
                            break;
                        case "X":
                            stockSourceName = "Previous Repo";
                            break;
                    case "V":
                        stockSourceName = "VinCue";
                        break;
                    default:
                            stockSource = "";
                            stockSourceName = "";
                            break;
                    }

                    if (lastChar != "" && Char.IsLetter(lastChar, 0))
                    {
                        switch (lastChar)
                        {
                        case "K":
                            stockSourceName = "Kelly ICO";
                            break;
                        case "M":
                            stockSourceName = "Facebook Marketplace";
                            break;
                        default:
                            stockSourceName = "Trade";
                            break;
                    }
                        
                    }

                    appraisal.VehicleSource = stockSourceName;

                    if(appraisal.Certification == null || appraisal.Certification == "")
                    {
                        appraisal.Certification = "None";
                    }

                if (appraisal.Certification == "FWP")
                {
                    appraisal.Certification = "FP";
                }

                if (appraisal.Certification == "MC" || appraisal.Certification == "CUV" || appraisal.Certification == "MZC" || appraisal.Certification == "GMU" || appraisal.Certification == "VWC")
                {
                    appraisal.Certification = "CPO";
                }

                if (appraisal.SalesAssociate == null)
                    {
                        appraisal.SalesAssociate = "";
                    }
                if (appraisal.Make == null)
                {
                    appraisal.Make = "";
                }
                else
                {
                    appraisal.Make = appraisal.Make.Replace(" TRUCK", "");
                }
                
                if(appraisal.Model != null && appraisal.Model.IndexOf(" ") > 0)
                {
                    appraisal.Model = appraisal.Model.Substring(0, appraisal.Model.IndexOf(" "));
                }

                if (appraisal.ZipCode == null || appraisal.ZipCode == "")
                {
                    appraisal.ZipCode = "None";
                }

                if (appraisal.Appraiser == null)
                    {
                        appraisal.Appraiser = "";
                    }
                }
            //}
            //else if(appraisalReportModel.ReportType == "inventory")
            //{
                appraisalReportModel.TradeAcquisitionDetails = SqlQueries.GetTradeAcquisitionReportDetailsByDate(reportDate,reportEndDate);


                appraisalReportModel.TradeAcquisitionDetails = appraisalReportModel.TradeAcquisitionDetails.OrderByDescending(z => z.VIN).ThenByDescending(z => z.AppraisalDate).GroupBy(x => x.VIN).Select(g => g.First()).ToList();

            appraisalReportModel.TradeAcquisitionDetails = appraisalReportModel.TradeAcquisitionDetails.FindAll(x => statusType.Split(',').ToList().Contains(x.Status.ToString()));

            if (statusOnFM.ToUpper() != "ALL")
            {
                if (statusOnFM.ToUpper() == "ONFITZMALL")
                {
                    appraisalReportModel.TradeAcquisitionDetails = appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.ListAmount > 0);
                }
                else if(statusOnFM.ToUpper() == "MISSINGFITZMALL")
                {
                    appraisalReportModel.TradeAcquisitionDetails = appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.ListAmount < 1);

                }

            }

            //if (statusType.ToUpper() != "ALL")
            //    {
            //        if (statusType.ToUpper() == "RETAIL")
            //        {

            //        }
            //        else if (statusType.ToUpper() == "ONLINE")
            //        {
            //            appraisalReportModel.TradeAcquisitionDetails = appraisalReportModel.TradeAcquisitionDetails.FindAll(x => (x.Status == 1 || x.Status == 2) && x.ListAmount > 0);
            //        }
            //        else if (statusType.ToUpper() == "OTHER")
            //        {
            //            appraisalReportModel.TradeAcquisitionDetails = appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.Status != 1 && x.Status != 2);
            //        }
            //    }

            if (acquisitionType != "ALL")
            {
                if (acquisitionType == "Trade")
                {
                    appraisalReportModel.TradeAcquisitionDetails = appraisalReportModel.TradeAcquisitionDetails.FindAll(x => Char.IsLetter(x.StockNumber.Substring(x.StockNumber.Length - 1), 0));
                }
                else if (acquisitionType == "Purchase")
                {
                    appraisalReportModel.TradeAcquisitionDetails = appraisalReportModel.TradeAcquisitionDetails.FindAll(x => !Char.IsLetter(x.StockNumber.Substring(x.StockNumber.Length - 1), 0));
                }
            }

            var additionalDetails = SqlQueries.GetTradeAcquisitionAdditionalDetails();

            foreach (var appraisal in appraisalReportModel.TradeAcquisitionDetails)
                {
                    var stockSource = "";
                    var stockSourceName = "";
                    var lastChar = "";

                
                    if (appraisal.StockNumber != null && appraisal.StockNumber.Length > 1)
                    {
                        stockSource = appraisal.StockNumber.Substring(1, 1);
                        lastChar = appraisal.StockNumber.Substring(appraisal.StockNumber.Length - 1);

                        var details = additionalDetails.Find(x => x.StockNumber.TrimEnd() == appraisal.StockNumber.TrimEnd());
                        if(details != null)
                        {
                            appraisal.StyleId = details.StyleId;
                            appraisal.Certification = details.Certification;
                            appraisal.BodyStyle = details.BodyStyle;
                        }
                    else
                    {
                        appraisal.StyleId = "None";
                        appraisal.Certification = "None";
                        appraisal.BodyStyle = "unknown";
                    }

                    }
                switch (stockSource)
                    {
                        case "A":
                            stockSourceName = "Auction or Wholesale";
                            break;
                        case "B":
                            stockSourceName = "Lease Buyout";
                            break;
                    case "C":
                        stockSourceName = "Car Offer";
                        break;

                    case "D":
                            stockSourceName = "Demo Prev Rental";
                            break;
                        case "E":
                            stockSourceName = "Demo";
                            break;
                        case "F":
                            stockSourceName = "Nextcar Bad Hist";
                            break;
                        case "G":
                            stockSourceName = "Government Auction";
                            break;
                        case "K":
                            stockSourceName = "Kelly ICO";
                            break;
                        case "L":
                            stockSourceName = "In House Loaner";
                            break;
                        case "M":
                            stockSourceName = "Facebook Marketplace";
                            break;
                        case "N":
                            stockSourceName = "Nextcar Purchase";
                            break;
                        case "P":
                            stockSourceName = "Purchase/Lease Cust";
                            break;
                        case "R":
                            stockSourceName = "Rental";
                            break;
                        case "X":
                            stockSourceName = "Previous Repo";
                            break;
                        case "V":
                            stockSourceName = "VinCue";
                            break;
                    default:
                            stockSource = "";
                            stockSourceName = "";
                            break;
                    }

                    if (lastChar != "" && Char.IsLetter(lastChar, 0))
                    {
                    switch (lastChar)
                    {
                        case "K":
                            stockSourceName = "Kelly ICO";
                            break;
                        case "M":
                            stockSourceName = "Facebook Marketplace";
                            break;
                        default:
                            stockSourceName = "Trade";
                            break;
                    }
                }

                    appraisal.VehicleSource = stockSourceName;

                if (appraisal.Make == null)
                {
                    appraisal.Make = "";
                }

                if (appraisal.Carline != null && appraisal.Carline.IndexOf(" ") > 0)
                {
                    appraisal.Carline = appraisal.Carline.Substring(0, appraisal.Carline.IndexOf(" "));
                }

                if (appraisal.LeadSourceName == null)
                    {
                        appraisal.LeadSourceName = "";
                    }

                    if (appraisal.LeadGroup == null)
                    {
                        appraisal.LeadGroup = "";
                    }

                    if (appraisal.Appraiser == null)
                    {
                        appraisal.Appraiser = "";
                    }
                }

            //}

            // NOW CHECK TO SEE IF FILTERS WERE SELECTED AND FILTER DATA
            if (bFilterResults)
            {
                #region Breakdown 1

                if (appraisalReportModel.BreakDownLevel1 != null && breakdown1Filters != "")
                {
                    var filteredSoldResults = new List<AppraisalSoldDetail>();
                    var filteredAcquisitionResults = new List<TradeAcquisitionDetail>();

                    switch (appraisalReportModel.BreakDownLevel1)
                    {

                        case "appraiser":
                            foreach (var filter in breakdown1Filters.Split(','))
                            {
                                filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Appraiser == filter));
                                filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.Appraiser == filter));
                            }
                            break;


                        case "vehiclesource":

                            foreach(var filter in breakdown1Filters.Split(','))
                            {
                                filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.VehicleSource == filter));
                                filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.VehicleSource == filter));
                            }

                            break;

                        case "certified":
                            foreach (var filter in breakdown1Filters.Split(','))
                            {
                                foreach (var cert in SalesCommission.Business.Enums.CertificationLevels)
                                {
                                    if(cert.Name == filter)
                                    {
                                        switch (filter)
                                        {
                                            case "FitzWay Value":
                                                filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Certification.Trim() == "FW"));
                                                filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.Certification.Trim() == "F915"));
                                                break;
                                            case "FitzWay Plus":
                                                filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Certification.Trim() == "FP"));
                                                filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.Certification.Trim() == "F914"));
                                                break;
                                            case "FitzWay Certified":
                                                filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Certification.Trim() == "FC"));
                                                filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.Certification.Trim() == "F910"));
                                                break;
                                            case "FitzWay Handyman":
                                                filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Certification.Trim() == "HDM"));
                                                filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.Certification.Trim() == "F916" || x.Certification.Trim() == "F917" || x.Certification.Trim() == "F918"));
                                                break;
                                            case "Manufacturer CPO":
                                                filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Certification.Trim() == "MC" || x.Certification.Trim() == "CUV" || x.Certification.Trim() == "MZC" || x.Certification.Trim() == "GMU" || x.Certification.Trim() == "VWC" || x.Certification.Trim() == "CPO"));
                                                filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.Certification.Trim() == "F906" || x.Certification.Trim() == "F907" || x.Certification.Trim() == "F908" || x.Certification.Trim() == "F909" || x.Certification.Trim() == "F911" || x.Certification.Trim() == "F912" || x.Certification.Trim() == "F913" || x.Certification.Trim() == "F922" || x.Certification.Trim() == "F923"));
                                                break;
                                            case "Other":
                                                filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Certification.Trim() == ""));
                                                filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.Certification.Trim() == ""));
                                                break;

                                        }
                                    }
                                }
                            }
                            

                            break;

                        case "soldloc":
                            foreach (var filter in breakdown1Filters.Split(','))
                            {
                                var searchLoc = "";
                                foreach (var store in SalesCommission.Business.Enums.SoldLocations)
                                {
                                    if (store.Name == filter)
                                    {
                                        searchLoc = store.LocationId;

                                        if (searchLoc == "FAM")
                                        {
                                            filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.SoldLocation == "FAM"));
                                            filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.DRloc == "FCG"));

                                        }
                                        else if (searchLoc == "FTN")
                                        {
                                            filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.SoldLocation == "FTN"));
                                            filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.DRloc == "FTO"));

                                        }
                                        else
                                        {
                                            filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.SoldLocation == searchLoc));
                                            filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.DRloc == searchLoc));
                                        }
                                        
                                    }

                                }
                            }
                            break;

                        case "make":
                            foreach (var filter in breakdown1Filters.Split(','))
                            {
                                filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Make.ToUpper() == filter.ToUpper()));
                                filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.Make.ToUpper() == filter.ToUpper()));
                            }

                            break;


                        case "model":
                            foreach (var filter in breakdown1Filters.Split(','))
                            {
                                filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Model.ToUpper() == filter.ToUpper()));
                                filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.Carline.ToUpper() == filter.ToUpper()));
                            }
                            break;

                        case "monthyear":
                            foreach (var filter in breakdown1Filters.Split(','))
                            {
                                filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.DealMonthYear.ToString("MM/yyyy") == filter));
                                filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.LastModifiedDate.ToString("MM/yyyy") == filter));
                            }
                            break;


                        case "modelyear":
                            foreach (var filter in breakdown1Filters.Split(','))
                            {
                                filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Year == filter));
                                filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.Year == filter));
                            }
                            break;

                        case "showroom":
                            foreach (var filter in breakdown1Filters.Split(','))
                            {

                                foreach (var store in SalesCommission.Business.Enums.ShowroomIDMapping)
                                {
                                    if (store.Name == filter)
                                    {
                                        filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.Loc == filter));
                                    }

                                }

                                foreach (var store in SalesCommission.Business.Enums.AppraisalLocations)
                                {
                                    if (store.Name == filter)
                                    {
                                        filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Showroom == filter));

                                    }
                                }

                            }
                            break;


                        case "days":
                            foreach (var filter in breakdown1Filters.Split(','))
                            {
                                if (filter == "> 90")
                                {
                                    filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => Convert.ToDecimal(Convert.ToDecimal(x.DaysInStock)) >= 90));
                                    filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => Convert.ToDecimal(x.Days) >= 90));
                                }
                                else
                                {
                                    var daysbreakdown = Convert.ToDecimal(filter.Substring(filter.IndexOf("-") + 2)) + 1;

                                    filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => Convert.ToDecimal(x.DaysInStock) < daysbreakdown && Convert.ToDecimal(x.DaysInStock) >= (daysbreakdown - 30)));
                                    filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => Convert.ToDecimal(x.Days) < daysbreakdown && Convert.ToDecimal(x.Days) >= (daysbreakdown - 30)));
                                    
                                }
                            }
                            break;

                        case "mileage":
                            foreach (var filter in breakdown1Filters.Split(','))
                            {
                                if (filter == "> 100000")
                                {
                                    filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => Convert.ToDecimal(x.Miles) >= 100000));
                                    filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => Convert.ToDecimal(x.Miles) >= 100000));
                                }
                                else
                                {
                                    var milesbreakdown = Convert.ToDecimal(filter.Substring(filter.IndexOf("-") + 2)) + 1;
                                    
                                    filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => Convert.ToDecimal(x.Miles) < milesbreakdown && Convert.ToDecimal(x.Miles) >= (milesbreakdown - 25000)));
                                    filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => Convert.ToDecimal(x.Miles) < milesbreakdown && Convert.ToDecimal(x.Miles) >= (milesbreakdown - 25000)));
                                }
                            }
                                
                            break;

                        case "price":
                            foreach (var filter in breakdown1Filters.Split(','))
                            {
                                
                                if (filter == "> 100000")
                                {
                                    filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.SellingPrice >= 100000));
                                    filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.ListAmount >= 100000));
                                }
                                else
                                {
                                    var pricebreakdown = Convert.ToDecimal(filter.Substring(filter.IndexOf("-") + 2)) + 1;

                                    filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.SellingPrice < pricebreakdown && x.SellingPrice >= (pricebreakdown - 10000)));
                                    filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.ListAmount < pricebreakdown && x.ListAmount >= (pricebreakdown - 10000)));
                                }
                            }
                            break;



                        //Specific to Individual Reports
                        case "associate":
                            foreach (var filter in breakdown1Filters.Split(','))
                            {
                                filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.SalesAssociate == filter));
                                //filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.Make.ToUpper() == filter.ToUpper()));
                            }
                            break;

                        case "location":
                            foreach (var filter in breakdown1Filters.Split(','))
                            {

                                foreach (var store in SalesCommission.Business.Enums.AppraisalLocations)
                                {
                                    if (store.Name == filter)
                                    {
                                        filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Location.ToUpper() == store.LocationId.ToUpper()));
                                        
                                    }
                                }

                                foreach (var store in SalesCommission.Business.Enums.SoldLocations)
                                {
                                    if (store.Name == filter)
                                    {
                                        filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.DRloc.ToUpper() == store.LocationId.ToUpper()));
                                    }
                                }

                            }
                            break;

                        case "stock":
                            foreach (var filter in breakdown1Filters.Split(','))
                            {
                                filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.StockNumber == filter));
                                filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.StockNumber == filter));
                            }
                            break;

                        case "zipcode":
                            foreach (var filter in breakdown1Filters.Split(','))
                            {
                                filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.ZipCode == filter));
                                //filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.ZipCode == filter));
                            }
                            break;

                        case "leadgroup":
                            foreach (var filter in breakdown1Filters.Split(','))
                            {
                                //filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Make.ToUpper() == filter.ToUpper()));
                                filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.LeadGroup == filter));
                            }
                            break;

                        case "lead":
                            foreach (var filter in breakdown1Filters.Split(','))
                            {
                                //filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Make.ToUpper() == filter.ToUpper()));
                                filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.LeadSourceName == filter));
                            }
                            break;

                        case "status":
                            foreach (var filter in breakdown1Filters.Split(','))
                            {
                                //filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Make.ToUpper() == filter.ToUpper()));
                                filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.Status.ToString() == filter));
                            }
                            break;

                        default:
                            break;

                    }
                    appraisalReportModel.AppraisalSoldDetails = filteredSoldResults;
                    appraisalReportModel.TradeAcquisitionDetails = filteredAcquisitionResults;
                }
                #endregion Breadown 1

                #region Breakdown 2

                if (appraisalReportModel.BreakDownLevel2 != null && breakdown2Filters != "")
                {
                    var filteredSoldResults = new List<AppraisalSoldDetail>();
                    var filteredAcquisitionResults = new List<TradeAcquisitionDetail>();

                    switch (appraisalReportModel.BreakDownLevel2)
                    {

                        case "appraiser":
                            foreach (var filter in breakdown2Filters.Split(','))
                            {
                                filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Appraiser == filter));
                                filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.Appraiser == filter));
                            }
                            break;
                            
                        case "vehiclesource":

                            foreach (var filter in breakdown2Filters.Split(','))
                            {
                                filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.VehicleSource == filter));
                                filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.VehicleSource == filter));
                            }

                            break;

                        case "certified":
                            foreach (var filter in breakdown2Filters.Split(','))
                            {
                                foreach (var cert in SalesCommission.Business.Enums.CertificationLevels)
                                {
                                    if (cert.Name == filter)
                                    {
                                        switch (filter)
                                        {
                                            case "FitzWay Value":
                                                filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Certification.Trim() == "FW"));
                                                filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.Certification.Trim() == "F915"));
                                                break;
                                            case "FitzWay Plus":
                                                filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Certification.Trim() == "FP"));
                                                filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.Certification.Trim() == "F914"));
                                                break;
                                            case "FitzWay Certified":
                                                filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Certification.Trim() == "FC"));
                                                filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.Certification.Trim() == "F910"));
                                                break;
                                            case "FitzWay Handyman":
                                                filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Certification.Trim() == "HDM"));
                                                filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.Certification.Trim() == "F916" || x.Certification.Trim() == "F917" || x.Certification.Trim() == "F918"));
                                                break;
                                            case "Manufacturer CPO":
                                                filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Certification.Trim() == "MC" || x.Certification.Trim() == "CUV" || x.Certification.Trim() == "MZC" || x.Certification.Trim() == "GMU" || x.Certification.Trim() == "VWC" || x.Certification.Trim() == "CPO"));
                                                filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.Certification.Trim() == "F906" || x.Certification.Trim() == "F907" || x.Certification.Trim() == "F908" || x.Certification.Trim() == "F909" || x.Certification.Trim() == "F911" || x.Certification.Trim() == "F912" || x.Certification.Trim() == "F913" || x.Certification.Trim() == "F922" || x.Certification.Trim() == "F923"));
                                                break;
                                            case "Other":
                                                filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Certification.Trim() == ""));
                                                filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.Certification.Trim() == ""));
                                                break;

                                        }
                                    }
                                }
                            }


                            break;

                        case "soldloc":
                            foreach (var filter in breakdown2Filters.Split(','))
                            {
                                var searchLoc = "";
                                foreach (var store in SalesCommission.Business.Enums.SoldLocations)
                                {
                                    if (store.Name == filter)
                                    {
                                        searchLoc = store.LocationId;

                                        if (searchLoc == "FAM")
                                        {
                                            filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.SoldLocation == "FAM"));
                                            filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.DRloc == "FCG"));

                                        }
                                        else if (searchLoc == "FTN")
                                        {
                                            filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.SoldLocation == "FTN"));
                                            filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.DRloc == "FTO"));

                                        }
                                        else
                                        {
                                            filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.SoldLocation == searchLoc));
                                            filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.DRloc == searchLoc));
                                        }

                                    }

                                }
                            }
                            break;

                        case "make":
                            foreach (var filter in breakdown2Filters.Split(','))
                            {
                                filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Make.ToUpper() == filter.ToUpper()));
                                filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.Make.ToUpper() == filter.ToUpper()));
                            }

                            break;
                            
                        case "model":
                            foreach (var filter in breakdown2Filters.Split(','))
                            {
                                filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Model.ToUpper() == filter.ToUpper()));
                                filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.Carline.ToUpper() == filter.ToUpper()));
                            }
                            break;

                        case "monthyear":
                            foreach (var filter in breakdown2Filters.Split(','))
                            {
                                filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.DealMonthYear.ToString("MM/yyyy") == filter));
                                filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.LastModifiedDate.ToString("MM/yyyy") == filter));
                            }
                            break;
                            
                        case "modelyear":
                            foreach (var filter in breakdown2Filters.Split(','))
                            {
                                filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Year == filter));
                                filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.Year == filter));
                            }
                            break;

                        case "showroom":
                            foreach (var filter in breakdown2Filters.Split(','))
                            {

                                foreach (var store in SalesCommission.Business.Enums.ShowroomIDMapping)
                                {
                                    if (store.Name == filter)
                                    {
                                        filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.Loc == filter));
                                    }

                                }

                                foreach (var store in SalesCommission.Business.Enums.AppraisalLocations)
                                {
                                    if (store.Name == filter)
                                    {
                                        filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Showroom == filter));

                                    }
                                }

                            }
                            break;


                        case "days":
                            foreach (var filter in breakdown2Filters.Split(','))
                            {
                                if (filter == "> 90")
                                {
                                    filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => Convert.ToDecimal(Convert.ToDecimal(x.DaysInStock)) >= 90));
                                    filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => Convert.ToDecimal(x.Days) >= 90));
                                }
                                else
                                {
                                    var daysbreakdown = Convert.ToDecimal(filter.Substring(filter.IndexOf("-") + 2)) + 1;

                                    filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => Convert.ToDecimal(x.DaysInStock) < daysbreakdown && Convert.ToDecimal(x.DaysInStock) >= (daysbreakdown - 30)));
                                    filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => Convert.ToDecimal(x.Days) < daysbreakdown && Convert.ToDecimal(x.Days) >= (daysbreakdown - 30)));

                                }
                            }
                            break;

                        case "mileage":
                            foreach (var filter in breakdown2Filters.Split(','))
                            {
                                if (filter == "> 100000")
                                {
                                    filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => Convert.ToDecimal(x.Miles) >= 100000));
                                    filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => Convert.ToDecimal(x.Miles) >= 100000));
                                }
                                else
                                {
                                    var milesbreakdown = Convert.ToDecimal(filter.Substring(filter.IndexOf("-") + 2)) + 1;

                                    filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => Convert.ToDecimal(x.Miles) < milesbreakdown && Convert.ToDecimal(x.Miles) >= (milesbreakdown - 25000)));
                                    filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => Convert.ToDecimal(x.Miles) < milesbreakdown && Convert.ToDecimal(x.Miles) >= (milesbreakdown - 25000)));
                                }
                            }

                            break;

                        case "price":
                            foreach (var filter in breakdown2Filters.Split(','))
                            {

                                if (filter == "> 100000")
                                {
                                    filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.SellingPrice >= 100000));
                                    filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.ListAmount >= 100000));
                                }
                                else
                                {
                                    var pricebreakdown = Convert.ToDecimal(filter.Substring(filter.IndexOf("-") + 2)) + 1;

                                    filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.SellingPrice < pricebreakdown && x.SellingPrice >= (pricebreakdown - 10000)));
                                    filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.ListAmount < pricebreakdown && x.ListAmount >= (pricebreakdown - 10000)));
                                }
                            }
                            break;


                        //Specific to Individual Reports
                        case "associate":
                            foreach (var filter in breakdown2Filters.Split(','))
                            {
                                filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.SalesAssociate == filter));
                                //filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.Make.ToUpper() == filter.ToUpper()));
                            }
                            break;

                        case "location":
                            foreach (var filter in breakdown2Filters.Split(','))
                            {

                                foreach (var store in SalesCommission.Business.Enums.AppraisalLocations)
                                {
                                    if (store.Name == filter)
                                    {
                                        filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Location.ToUpper() == store.LocationId.ToUpper()));

                                    }
                                }

                                foreach (var store in SalesCommission.Business.Enums.SoldLocations)
                                {
                                    if (store.Name == filter)
                                    {
                                        filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.DRloc.ToUpper() == store.LocationId.ToUpper()));
                                    }
                                }

                            }
                            break;

                        case "stock":
                            foreach (var filter in breakdown2Filters.Split(','))
                            {
                                filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.StockNumber == filter));
                                filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.StockNumber == filter));
                            }
                            break;

                        case "zipcode":
                            foreach (var filter in breakdown2Filters.Split(','))
                            {
                                filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.ZipCode == filter));
                                //filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.ZipCode == filter));
                            }
                            break;

                        case "leadgroup":
                            foreach (var filter in breakdown2Filters.Split(','))
                            {
                                //filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Make.ToUpper() == filter.ToUpper()));
                                filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.LeadGroup == filter));
                            }
                            break;

                        case "lead":
                            foreach (var filter in breakdown2Filters.Split(','))
                            {
                                //filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Make.ToUpper() == filter.ToUpper()));
                                filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.LeadSourceName == filter));
                            }
                            break;

                        case "status":
                            foreach (var filter in breakdown2Filters.Split(','))
                            {
                                //filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Make.ToUpper() == filter.ToUpper()));
                                filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.Status.ToString() == filter));
                            }
                            break;

                        default:
                            break;

                    }
                    appraisalReportModel.AppraisalSoldDetails = filteredSoldResults;
                    appraisalReportModel.TradeAcquisitionDetails = filteredAcquisitionResults;
                }
                #endregion Breadown 2

                #region Breakdown 3

                if (appraisalReportModel.BreakDownLevel3 != null && breakdown3Filters != "")
                {
                    var filteredSoldResults = new List<AppraisalSoldDetail>();
                    var filteredAcquisitionResults = new List<TradeAcquisitionDetail>();

                    switch (appraisalReportModel.BreakDownLevel3)
                    {

                        case "appraiser":
                            foreach (var filter in breakdown3Filters.Split(','))
                            {
                                filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Appraiser == filter));
                                filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.Appraiser == filter));
                            }
                            break;
                            
                        case "vehiclesource":

                            foreach (var filter in breakdown3Filters.Split(','))
                            {
                                filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.VehicleSource == filter));
                                filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.VehicleSource == filter));
                            }

                            break;

                        case "certified":
                            foreach (var filter in breakdown3Filters.Split(','))
                            {
                                foreach (var cert in SalesCommission.Business.Enums.CertificationLevels)
                                {
                                    if (cert.Name == filter)
                                    {
                                        switch (filter)
                                        {
                                            case "FitzWay Value":
                                                filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Certification.Trim() == "FW"));
                                                filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.Certification.Trim() == "F915"));
                                                break;
                                            case "FitzWay Plus":
                                                filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Certification.Trim() == "FP"));
                                                filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.Certification.Trim() == "F914"));
                                                break;
                                            case "FitzWay Certified":
                                                filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Certification.Trim() == "FC"));
                                                filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.Certification.Trim() == "F910"));
                                                break;
                                            case "FitzWay Handyman":
                                                filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Certification.Trim() == "HDM"));
                                                filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.Certification.Trim() == "F916" || x.Certification.Trim() == "F917" || x.Certification.Trim() == "F918"));
                                                break;
                                            case "Manufacturer CPO":
                                                filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Certification.Trim() == "MC" || x.Certification.Trim() == "CUV" || x.Certification.Trim() == "MZC" || x.Certification.Trim() == "GMU" || x.Certification.Trim() == "VWC" || x.Certification.Trim() == "CPO"));
                                                filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.Certification.Trim() == "F906" || x.Certification.Trim() == "F907" || x.Certification.Trim() == "F908" || x.Certification.Trim() == "F909" || x.Certification.Trim() == "F911" || x.Certification.Trim() == "F912" || x.Certification.Trim() == "F913" || x.Certification.Trim() == "F922" || x.Certification.Trim() == "F923"));
                                                break;
                                            case "Other":
                                                filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Certification.Trim() == ""));
                                                filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.Certification.Trim() == ""));
                                                break;

                                        }
                                    }
                                }
                            }


                            break;

                        case "soldloc":
                            foreach (var filter in breakdown3Filters.Split(','))
                            {
                                var searchLoc = "";
                                foreach (var store in SalesCommission.Business.Enums.SoldLocations)
                                {
                                    if (store.Name == filter)
                                    {
                                        searchLoc = store.LocationId;

                                        if (searchLoc == "FAM")
                                        {
                                            filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.SoldLocation == "FAM"));
                                            filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.DRloc == "FCG"));

                                        }
                                        else if (searchLoc == "FTN")
                                        {
                                            filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.SoldLocation == "FTN"));
                                            filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.DRloc == "FTO"));

                                        }
                                        else
                                        {
                                            filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.SoldLocation == searchLoc));
                                            filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.DRloc == searchLoc));
                                        }

                                    }

                                }
                            }
                            break;

                        case "make":
                            foreach (var filter in breakdown3Filters.Split(','))
                            {
                                filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Make.ToUpper() == filter.ToUpper()));
                                filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.Make.ToUpper() == filter.ToUpper()));
                            }

                            break;
                            
                        case "model":
                            foreach (var filter in breakdown3Filters.Split(','))
                            {
                                filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Model.ToUpper() == filter.ToUpper()));
                                filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.Carline.ToUpper() == filter.ToUpper()));
                            }
                            break;

                        case "monthyear":
                            foreach (var filter in breakdown3Filters.Split(','))
                            {
                                filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.DealMonthYear.ToString("MM/yyyy") == filter));
                                filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.LastModifiedDate.ToString("MM/yyyy") == filter));
                            }
                            break;
                            
                        case "modelyear":
                            foreach (var filter in breakdown3Filters.Split(','))
                            {
                                filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Year == filter));
                                filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.Year == filter));
                            }
                            break;

                        case "showroom":
                            foreach (var filter in breakdown3Filters.Split(','))
                            {

                                foreach (var store in SalesCommission.Business.Enums.ShowroomIDMapping)
                                {
                                    if (store.Name == filter)
                                    {
                                        filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.Loc == filter));
                                    }

                                }

                                foreach (var store in SalesCommission.Business.Enums.AppraisalLocations)
                                {
                                    if (store.Name == filter)
                                    {
                                        filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Showroom == filter));

                                    }
                                }

                            }
                            break;


                        case "days":
                            foreach (var filter in breakdown3Filters.Split(','))
                            {
                                if (filter == "> 90")
                                {
                                    filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => Convert.ToDecimal(Convert.ToDecimal(x.DaysInStock)) >= 90));
                                    filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => Convert.ToDecimal(x.Days) >= 90));
                                }
                                else
                                {
                                    var daysbreakdown = Convert.ToDecimal(filter.Substring(filter.IndexOf("-") + 2)) + 1;

                                    filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => Convert.ToDecimal(x.DaysInStock) < daysbreakdown && Convert.ToDecimal(x.DaysInStock) >= (daysbreakdown - 30)));
                                    filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => Convert.ToDecimal(x.Days) < daysbreakdown && Convert.ToDecimal(x.Days) >= (daysbreakdown - 30)));

                                }
                            }
                            break;

                        case "mileage":
                            foreach (var filter in breakdown3Filters.Split(','))
                            {
                                if (filter == "> 100000")
                                {
                                    filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => Convert.ToDecimal(x.Miles) >= 100000));
                                    filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => Convert.ToDecimal(x.Miles) >= 100000));
                                }
                                else
                                {
                                    var milesbreakdown = Convert.ToDecimal(filter.Substring(filter.IndexOf("-") + 2)) + 1;

                                    filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => Convert.ToDecimal(x.Miles) < milesbreakdown && Convert.ToDecimal(x.Miles) >= (milesbreakdown - 25000)));
                                    filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => Convert.ToDecimal(x.Miles) < milesbreakdown && Convert.ToDecimal(x.Miles) >= (milesbreakdown - 25000)));
                                }
                            }

                            break;

                        case "price":
                            foreach (var filter in breakdown3Filters.Split(','))
                            {

                                if (filter == "> 100000")
                                {
                                    filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.SellingPrice >= 100000));
                                    filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.ListAmount >= 100000));
                                }
                                else
                                {
                                    var pricebreakdown = Convert.ToDecimal(filter.Substring(filter.IndexOf("-") + 2)) + 1;

                                    filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.SellingPrice < pricebreakdown && x.SellingPrice >= (pricebreakdown - 10000)));
                                    filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.ListAmount < pricebreakdown && x.ListAmount >= (pricebreakdown - 10000)));
                                }
                            }
                            break;


                        //Specific to Individual Reports
                        case "associate":
                            foreach (var filter in breakdown3Filters.Split(','))
                            {
                                filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.SalesAssociate == filter));
                                //filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.Make.ToUpper() == filter.ToUpper()));
                            }
                            break;

                        case "location":
                            foreach (var filter in breakdown3Filters.Split(','))
                            {

                                foreach (var store in SalesCommission.Business.Enums.AppraisalLocations)
                                {
                                    if (store.Name == filter)
                                    {
                                        filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Location.ToUpper() == store.LocationId.ToUpper()));

                                    }
                                }

                                foreach (var store in SalesCommission.Business.Enums.SoldLocations)
                                {
                                    if (store.Name == filter)
                                    {
                                        filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.DRloc.ToUpper() == store.LocationId.ToUpper()));
                                    }
                                }

                            }
                            break;

                        case "stock":
                            foreach (var filter in breakdown3Filters.Split(','))
                            {
                                filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.StockNumber == filter));
                                filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.StockNumber == filter));
                            }
                            break;

                        case "zipcode":
                            foreach (var filter in breakdown3Filters.Split(','))
                            {
                                filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.ZipCode == filter));
                                //filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.ZipCode == filter));
                            }
                            break;

                        case "leadgroup":
                            foreach (var filter in breakdown3Filters.Split(','))
                            {
                                //filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Make.ToUpper() == filter.ToUpper()));
                                filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.LeadGroup == filter));
                            }
                            break;

                        case "lead":
                            foreach (var filter in breakdown3Filters.Split(','))
                            {
                                //filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Make.ToUpper() == filter.ToUpper()));
                                filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.LeadSourceName == filter));
                            }
                            break;

                        case "status":
                            foreach (var filter in breakdown3Filters.Split(','))
                            {
                                //filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Make.ToUpper() == filter.ToUpper()));
                                filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.Status.ToString() == filter));
                            }
                            break;

                        default:
                            break;

                    }
                    appraisalReportModel.AppraisalSoldDetails = filteredSoldResults;
                    appraisalReportModel.TradeAcquisitionDetails = filteredAcquisitionResults;
                }
                #endregion Breadown 3

                #region Breakdown 4

                if (appraisalReportModel.BreakDownLevel4 != null && breakdown4Filters != "")
                {
                    var filteredSoldResults = new List<AppraisalSoldDetail>();
                    var filteredAcquisitionResults = new List<TradeAcquisitionDetail>();

                    switch (appraisalReportModel.BreakDownLevel4)
                    {

                        case "appraiser":
                            foreach (var filter in breakdown4Filters.Split(','))
                            {
                                filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Appraiser == filter));
                                filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.Appraiser == filter));
                            }
                            break;
                            
                        case "vehiclesource":

                            foreach (var filter in breakdown4Filters.Split(','))
                            {
                                filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.VehicleSource == filter));
                                filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.VehicleSource == filter));
                            }

                            break;

                        case "certified":
                            foreach (var filter in breakdown4Filters.Split(','))
                            {
                                foreach (var cert in SalesCommission.Business.Enums.CertificationLevels)
                                {
                                    if (cert.Name == filter)
                                    {
                                        switch (filter)
                                        {
                                            case "FitzWay Value":
                                                filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Certification.Trim() == "FW"));
                                                filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.Certification.Trim() == "F915"));
                                                break;
                                            case "FitzWay Plus":
                                                filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Certification.Trim() == "FP"));
                                                filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.Certification.Trim() == "F914"));
                                                break;
                                            case "FitzWay Certified":
                                                filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Certification.Trim() == "FC"));
                                                filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.Certification.Trim() == "F910"));
                                                break;
                                            case "FitzWay Handyman":
                                                filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Certification.Trim() == "HDM"));
                                                filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.Certification.Trim() == "F916" || x.Certification.Trim() == "F917" || x.Certification.Trim() == "F918"));
                                                break;
                                            case "Manufacturer CPO":
                                                filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Certification.Trim() == "MC" || x.Certification.Trim() == "CUV" || x.Certification.Trim() == "MZC" || x.Certification.Trim() == "GMU" || x.Certification.Trim() == "VWC" || x.Certification.Trim() == "CPO"));
                                                filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.Certification.Trim() == "F906" || x.Certification.Trim() == "F907" || x.Certification.Trim() == "F908" || x.Certification.Trim() == "F909" || x.Certification.Trim() == "F911" || x.Certification.Trim() == "F912" || x.Certification.Trim() == "F913" || x.Certification.Trim() == "F922" || x.Certification.Trim() == "F923"));
                                                break;
                                            case "Other":
                                                filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Certification.Trim() == ""));
                                                filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.Certification.Trim() == ""));
                                                break;

                                        }
                                    }
                                }
                            }


                            break;

                        case "soldloc":
                            foreach (var filter in breakdown4Filters.Split(','))
                            {
                                var searchLoc = "";
                                foreach (var store in SalesCommission.Business.Enums.SoldLocations)
                                {
                                    if (store.Name == filter)
                                    {
                                        searchLoc = store.LocationId;

                                        if (searchLoc == "FAM")
                                        {
                                            filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.SoldLocation == "FAM"));
                                            filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.DRloc == "FCG"));

                                        }
                                        else if (searchLoc == "FTN")
                                        {
                                            filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.SoldLocation == "FTN"));
                                            filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.DRloc == "FTO"));

                                        }
                                        else
                                        {
                                            filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.SoldLocation == searchLoc));
                                            filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.DRloc == searchLoc));
                                        }

                                    }

                                }
                            }
                            break;

                        case "make":
                            foreach (var filter in breakdown4Filters.Split(','))
                            {
                                filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Make.ToUpper() == filter.ToUpper()));
                                filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.Make.ToUpper() == filter.ToUpper()));
                            }

                            break;
                            
                        case "model":
                            foreach (var filter in breakdown4Filters.Split(','))
                            {
                                filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Model.ToUpper() == filter.ToUpper()));
                                filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.Carline.ToUpper() == filter.ToUpper()));
                            }
                            break;

                        case "monthyear":
                            foreach (var filter in breakdown4Filters.Split(','))
                            {
                                filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.DealMonthYear.ToString("MM/yyyy") == filter));
                                filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.LastModifiedDate.ToString("MM/yyyy") == filter));
                            }
                            break;
                            
                        case "modelyear":
                            foreach (var filter in breakdown4Filters.Split(','))
                            {
                                filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Year == filter));
                                filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.Year == filter));
                            }
                            break;

                        case "showroom":
                            foreach (var filter in breakdown4Filters.Split(','))
                            {

                                foreach (var store in SalesCommission.Business.Enums.ShowroomIDMapping)
                                {
                                    if (store.Name == filter)
                                    {
                                        filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.Loc == filter));
                                    }

                                }

                                foreach (var store in SalesCommission.Business.Enums.AppraisalLocations)
                                {
                                    if (store.Name == filter)
                                    {
                                        filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Showroom == filter));

                                    }
                                }

                            }
                            break;


                        case "days":
                            foreach (var filter in breakdown4Filters.Split(','))
                            {
                                if (filter == "> 90")
                                {
                                    filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => Convert.ToDecimal(Convert.ToDecimal(x.DaysInStock)) >= 90));
                                    filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => Convert.ToDecimal(x.Days) >= 90));
                                }
                                else
                                {
                                    var daysbreakdown = Convert.ToDecimal(filter.Substring(filter.IndexOf("-") + 2)) + 1;

                                    filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => Convert.ToDecimal(x.DaysInStock) < daysbreakdown && Convert.ToDecimal(x.DaysInStock) >= (daysbreakdown - 30)));
                                    filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => Convert.ToDecimal(x.Days) < daysbreakdown && Convert.ToDecimal(x.Days) >= (daysbreakdown - 30)));

                                }
                            }
                            break;

                        case "mileage":
                            foreach (var filter in breakdown4Filters.Split(','))
                            {
                                if (filter == "> 100000")
                                {
                                    filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => Convert.ToDecimal(x.Miles) >= 100000));
                                    filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => Convert.ToDecimal(x.Miles) >= 100000));
                                }
                                else
                                {
                                    var milesbreakdown = Convert.ToDecimal(filter.Substring(filter.IndexOf("-") + 2)) + 1;

                                    filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => Convert.ToDecimal(x.Miles) < milesbreakdown && Convert.ToDecimal(x.Miles) >= (milesbreakdown - 25000)));
                                    filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => Convert.ToDecimal(x.Miles) < milesbreakdown && Convert.ToDecimal(x.Miles) >= (milesbreakdown - 25000)));
                                }
                            }

                            break;

                        case "price":
                            foreach (var filter in breakdown4Filters.Split(','))
                            {

                                if (filter == "> 100000")
                                {
                                    filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.SellingPrice >= 100000));
                                    filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.ListAmount >= 100000));
                                }
                                else
                                {
                                    var pricebreakdown = Convert.ToDecimal(filter.Substring(filter.IndexOf("-") + 2)) + 1;

                                    filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.SellingPrice < pricebreakdown && x.SellingPrice >= (pricebreakdown - 10000)));
                                    filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.ListAmount < pricebreakdown && x.ListAmount >= (pricebreakdown - 10000)));
                                }
                            }
                            break;


                        //Specific to Individual Reports
                        case "associate":
                            foreach (var filter in breakdown4Filters.Split(','))
                            {
                                filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.SalesAssociate == filter));
                                //filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.Make.ToUpper() == filter.ToUpper()));
                            }
                            break;

                        case "location":
                            foreach (var filter in breakdown4Filters.Split(','))
                            {

                                foreach (var store in SalesCommission.Business.Enums.AppraisalLocations)
                                {
                                    if (store.Name == filter)
                                    {
                                        filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Location.ToUpper() == store.LocationId.ToUpper()));

                                    }
                                }

                                foreach (var store in SalesCommission.Business.Enums.SoldLocations)
                                {
                                    if (store.Name == filter)
                                    {
                                        filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.DRloc.ToUpper() == store.LocationId.ToUpper()));
                                    }
                                }

                            }
                            break;

                        case "stock":
                            foreach (var filter in breakdown4Filters.Split(','))
                            {
                                filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.StockNumber == filter));
                                filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.StockNumber == filter));
                            }
                            break;

                        case "zipcode":
                            foreach (var filter in breakdown4Filters.Split(','))
                            {
                                filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.ZipCode == filter));
                                //filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.ZipCode == filter));
                            }
                            break;

                        case "leadgroup":
                            foreach (var filter in breakdown4Filters.Split(','))
                            {
                                //filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Make.ToUpper() == filter.ToUpper()));
                                filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.LeadGroup == filter));
                            }
                            break;

                        case "lead":
                            foreach (var filter in breakdown4Filters.Split(','))
                            {
                                //filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Make.ToUpper() == filter.ToUpper()));
                                filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.LeadSourceName == filter));
                            }
                            break;

                        case "status":
                            foreach (var filter in breakdown4Filters.Split(','))
                            {
                                //filteredSoldResults.AddRange(appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Make.ToUpper() == filter.ToUpper()));
                                filteredAcquisitionResults.AddRange(appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.Status.ToString() == filter));
                            }
                            break;

                        default:
                            break;

                    }
                    appraisalReportModel.AppraisalSoldDetails = filteredSoldResults;
                    appraisalReportModel.TradeAcquisitionDetails = filteredAcquisitionResults;
                }
                #endregion Breadown 4
            }



            return View(appraisalReportModel);
            
        }

        public ActionResult AppraisalReportDetails(string startmonth, string startyear, string endmonth, string endyear, string bd1, string bd2, string bd3, string bd4, string vt, string at, string days)
        {
            var appraisalReportModel = new AppraisalReportModel();
            var appraisalReportDetailsModel = new AppraisalReportDetailModel();

            var bdValue1 = "";
            var bdValue2 = "";
            var bdValue3 = "";
            var bdValue4 = "";

            if (bd1 != null && bd1 != "")
            {
                appraisalReportModel.BreakDownLevel1 = bd1.Split(',')[0];
                //bdValue1 = bd1.Split(',')[1];
                bdValue1 = bd1.Replace(bd1.Split(',')[0] + ",", "");

                appraisalReportDetailsModel.BreakDownLevel1 = bd1.Split(',')[0];
                appraisalReportDetailsModel.BreakDownLevel1Value = bdValue1;
            }

            if (bd2 != null && bd2 != "")
            {
                appraisalReportModel.BreakDownLevel2 = bd2.Split(',')[0];
                //bdValue2 = bd2.Split(',')[1];
                bdValue2 = bd2.Replace(bd2.Split(',')[0] + ",", "");

                appraisalReportDetailsModel.BreakDownLevel2 = bd2.Split(',')[0];
                appraisalReportDetailsModel.BreakDownLevel2Value = bdValue2;
            }

            if (bd3 != null && bd3 != "")
            {
                appraisalReportModel.BreakDownLevel3 = bd3.Split(',')[0];
                //bdValue3 = bd3.Split(',')[1];
                bdValue3 = bd3.Replace(bd3.Split(',')[0] + ",", "");

                appraisalReportDetailsModel.BreakDownLevel3 = bd3.Split(',')[0];
                appraisalReportDetailsModel.BreakDownLevel3Value = bdValue3;
            }

            if (bd4 != null && bd4 != "")
            {
                appraisalReportModel.BreakDownLevel4 = bd4.Split(',')[0];
                //bdValue4 = bd4.Split(',')[1];
                bdValue4 = bd4.Replace(bd4.Split(',')[0] + ",", "");

                appraisalReportDetailsModel.BreakDownLevel4 = bd4.Split(',')[0];
                appraisalReportDetailsModel.BreakDownLevel4Value = bdValue4;
            }

            if (vt != null && vt != "")
            {
                appraisalReportModel.VehicleType = vt;
                appraisalReportDetailsModel.VehicleType = vt;
            }

            if (at != null && at != "")
            {
                appraisalReportModel.AcquisitionType = at;
                appraisalReportDetailsModel.AcquisitionType = at;
            }


            appraisalReportModel.ReportStartMonth = Convert.ToInt32(startmonth);
            appraisalReportDetailsModel.ReportStartMonth = Convert.ToInt32(startmonth);

            appraisalReportModel.ReportEndMonth = Convert.ToInt32(endmonth);
            appraisalReportDetailsModel.ReportEndMonth = Convert.ToInt32(endmonth);

            appraisalReportModel.ReportStartYear = Convert.ToInt32(startyear);
            appraisalReportDetailsModel.ReportStartYear = Convert.ToInt32(startyear);

            appraisalReportModel.ReportEndYear = Convert.ToInt32(endyear);
            appraisalReportDetailsModel.ReportEndYear = Convert.ToInt32(endyear);

            var reportDate = new DateTime(Convert.ToInt32(appraisalReportModel.ReportStartYear), Convert.ToInt32(appraisalReportModel.ReportStartMonth), 1);
            var reportEndDate = new DateTime(Convert.ToInt32(appraisalReportModel.ReportEndYear), Convert.ToInt32(appraisalReportModel.ReportEndMonth), 1);//.AddMonths(1);

            appraisalReportModel.AppraisalSoldDetails = SqlMapperUtil.StoredProcWithParams<AppraisalSoldDetail>("sp_CommissionGetAssociateAppraisalsSoldDetails", new { StartDate = reportDate.Date, EndDate = reportEndDate.Date }, "ReynoldsData");
            //Remove Duplicates
            appraisalReportModel.AppraisalSoldDetails = appraisalReportModel.AppraisalSoldDetails.OrderByDescending(z => z.VIN).ThenByDescending(z => z.LastModifiedDate).GroupBy(x => x.VIN).Select(g => g.First()).ToList();


            if (vt != "All")
            {
                if (vt == "Retail")
                {
                    appraisalReportModel.AppraisalSoldDetails = appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.WholesaleRetail == "R");

                }
                else if (vt == "Wholesale")
                {
                    appraisalReportModel.AppraisalSoldDetails = appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.WholesaleRetail == "W");

                }

            }

            if (at != "ALL")
            {
                if (at == "Trade")
                {
                    appraisalReportModel.AppraisalSoldDetails = appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.TradePurchase == "T");
                }
                else if (at == "Purchase")
                {
                    appraisalReportModel.AppraisalSoldDetails = appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.TradePurchase == "P");
                }
            }

            if (days != null && days != "")
            {
                switch (days)
                {
                    case "30":
                        appraisalReportModel.AppraisalSoldDetails = appraisalReportModel.AppraisalSoldDetails.FindAll(x => Int32.Parse(x.DaysInStock) < 30);
                        break;
                    case "3060":
                        appraisalReportModel.AppraisalSoldDetails = appraisalReportModel.AppraisalSoldDetails.FindAll(x => Int32.Parse(x.DaysInStock) >= 30 && Int32.Parse(x.DaysInStock) < 60);

                        break;
                    case "6090":
                        appraisalReportModel.AppraisalSoldDetails = appraisalReportModel.AppraisalSoldDetails.FindAll(x => Int32.Parse(x.DaysInStock) >= 60 && Int32.Parse(x.DaysInStock) < 90);
                        break;
                    case "90":
                        appraisalReportModel.AppraisalSoldDetails = appraisalReportModel.AppraisalSoldDetails.FindAll(x => Int32.Parse(x.DaysInStock) > 90);
                        break;
                }
            }

            foreach (var appraisal in appraisalReportModel.AppraisalSoldDetails)
            {
                var stockSource = "";
                var stockSourceName = "";
                var lastChar = "";

                if (appraisal.StockNumber != null)
                {
                    stockSource = appraisal.StockNumber.Substring(1, 1);
                    lastChar = appraisal.StockNumber.Substring(appraisal.StockNumber.Length - 1);
                }
                switch (stockSource)
                {
                    case "A":
                        stockSourceName = "Auction or Wholesale";
                        break;
                    case "B":
                        stockSourceName = "Lease Buyout";
                        break;
                    case "C":
                        stockSourceName = "Car Offer";
                        break;
                    case "D":
                        stockSourceName = "Demo Prev Rental";
                        break;
                    case "E":
                        stockSourceName = "Demo";
                        break;
                    case "F":
                        stockSourceName = "Nextcar Bad Hist";
                        break;
                    case "G":
                        stockSourceName = "Government Auction";
                        break;
                    case "K":
                        stockSourceName = "Kelly ICO";
                        break;
                    case "L":
                        stockSourceName = "In House Loaner";
                        break;
                    case "M":
                        stockSourceName = "Facebook Marketplace";
                        break;
                    case "N":
                        stockSourceName = "Nextcar Purchase";
                        break;
                    case "P":
                        stockSourceName = "Purchase/Lease Cust";
                        break;
                    case "R":
                        stockSourceName = "Rental";
                        break;
                    case "X":
                        stockSourceName = "Previous Repo";
                        break;
                    case "V":
                        stockSourceName = "VinCue";
                        break;
                    default:
                        stockSource = "";
                        stockSourceName = "";
                        break;
                }

                if (lastChar != "" && Char.IsLetter(lastChar, 0))
                {
                    switch (lastChar)
                    {
                        case "K":
                            stockSourceName = "Kelly ICO";
                            break;
                        case "M":
                            stockSourceName = "Facebook Marketplace";
                            break;
                        default:
                            stockSourceName = "Trade";
                            break;
                    }
                }

                appraisal.VehicleSource = stockSourceName;

                if (appraisal.ZipCode == null || appraisal.ZipCode == "")
                {
                    appraisal.ZipCode = "None";
                }

                if (appraisal.Certification == null || appraisal.Certification == "")
                {
                    appraisal.Certification = "None";
                }

                if (appraisal.Certification == "FWP")
                {
                    appraisal.Certification = "FP";
                }

                if (appraisal.Certification == "MC" || appraisal.Certification == "CUV" || appraisal.Certification == "MZC" || appraisal.Certification == "GMU" || appraisal.Certification == "VWC")
                {
                    appraisal.Certification = "CPO";
                }

                if (appraisal.Make == null)
                {
                    appraisal.Make = "";
                }
                else
                {
                    appraisal.Make = appraisal.Make.Replace(" TRUCK", "");
                }

                if (appraisal.Model != null && appraisal.Model.IndexOf(" ") > 0)
                {
                    appraisal.Model = appraisal.Model.Substring(0, appraisal.Model.IndexOf(" "));
                }

                if (appraisal.ZipCode == null || appraisal.ZipCode == "")
                {
                    appraisal.ZipCode = "None";
                }

                if (appraisal.Appraiser == null)
                {
                    appraisal.Appraiser = "";
                }
            
        }

            var Label1Value = "";
            var Label2Value = "";
            var Label3Value = "";
            var Label4Value = "";

            var BreakDown1filteredappraisals = new List<SalesCommission.Models.AppraisalSoldDetail>();
            var BreakDown2filteredappraisals = new List<SalesCommission.Models.AppraisalSoldDetail>();
            var BreakDown3filteredappraisals = new List<SalesCommission.Models.AppraisalSoldDetail>();
            var BreakDown4filteredappraisals = new List<SalesCommission.Models.AppraisalSoldDetail>();

            if (appraisalReportModel.AppraisalSoldDetails != null && appraisalReportModel.AppraisalSoldDetails.Count > 0)
            {



                #region Breakdown 1

                if (appraisalReportModel.BreakDownLevel1 != null)
                {
                    Label1Value = bdValue1;
                    switch (appraisalReportModel.BreakDownLevel1)
                    {

                        case "appraiser":
                            BreakDown1filteredappraisals = appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Appraiser == bdValue1);
                            break;

                        case "associate":
                            BreakDown1filteredappraisals = appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.SalesAssociate == bdValue1);
                            break;

                        case "location":
                            BreakDown1filteredappraisals = appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Location == bdValue1);

                            foreach (var store in SalesCommission.Business.Enums.AppraisalLocations)
                            {
                                if (store.LocationId.ToLower().Trim() == bdValue1.ToLower().Trim())
                                {
                                    Label1Value = store.Name;
                                }
                            }

                            break;

                        case "vehiclesource":
                            BreakDown1filteredappraisals = appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.VehicleSource == bdValue1);
                            break;

                        case "certified":
                            BreakDown1filteredappraisals = appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Certification == bdValue1);
                            foreach (var cert in SalesCommission.Business.Enums.CertificationLevels)
                            {
                                if (cert.CertificationID.ToLower().Trim() == bdValue1.ToLower().Trim())
                                {
                                    Label1Value = cert.Name;
                                }
                            }
                            if (Label1Value == "")
                            {
                                Label1Value = "None";
                            }
                            break;

                        case "soldloc":
                            BreakDown1filteredappraisals = appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.SoldLocation == bdValue1);
                            break;

                        case "make":
                            BreakDown1filteredappraisals = appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Make.Contains(bdValue1));
                            break;

                        case "stock":
                            BreakDown1filteredappraisals = appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.StockNumber == bdValue1);
                            break;

                        case "model":
                            BreakDown1filteredappraisals = appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Model == bdValue1);
                            break;

                        case "monthyear":
                            BreakDown1filteredappraisals = appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.LastModifiedDate.ToString("MM/yyyy") == bdValue1);
                            var dealDate = Convert.ToDateTime(bdValue1);
                            Label1Value = dealDate.Month + "/" + dealDate.Year;

                            break;

                        case "zipcode":
                            BreakDown1filteredappraisals = appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.ZipCode == bdValue1);
                            break;

                        case "modelyear":
                            BreakDown1filteredappraisals = appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Year == bdValue1);
                            break;

                        case "showroom":
                            BreakDown1filteredappraisals = appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Showroom == bdValue1);
                            break;


                        case "days":
                            var daysbreakdown = Convert.ToDecimal(bdValue1);

                            if (daysbreakdown == 120)
                            {
                                BreakDown1filteredappraisals = appraisalReportModel.AppraisalSoldDetails.FindAll(x => Convert.ToDecimal(x.DaysInStock) >= 90);
                            }
                            else
                            {
                                BreakDown1filteredappraisals = appraisalReportModel.AppraisalSoldDetails.FindAll(x => Convert.ToDecimal(x.DaysInStock) < daysbreakdown && Convert.ToDecimal(x.DaysInStock) >= (daysbreakdown - 30));
                            }
                            break;

                        case "mileage":
                            var milesbreakdown = Convert.ToDecimal(bdValue1);

                            if (milesbreakdown == 500000)
                            {
                                BreakDown1filteredappraisals = appraisalReportModel.AppraisalSoldDetails.FindAll(x => Convert.ToDecimal(x.Miles) >= 100000);
                            }
                            else
                            {
                                BreakDown1filteredappraisals = appraisalReportModel.AppraisalSoldDetails.FindAll(x => Convert.ToDecimal(x.Miles) < milesbreakdown && Convert.ToDecimal(x.Miles) >= (milesbreakdown - 25000));
                            }
                            break;

                        case "price":
                            var pricebreakdown = Convert.ToDecimal(bdValue1);

                            if (pricebreakdown == 200000)
                            {
                                BreakDown1filteredappraisals = appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.SellingPrice >= 100000);
                            }
                            else
                            {
                                BreakDown1filteredappraisals = appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.SellingPrice < pricebreakdown && x.SellingPrice >= (pricebreakdown - 10000));
                            }
                            break;

                        default:
                            BreakDown1filteredappraisals = appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Location == bdValue1);
                            break;

                    }

                    appraisalReportDetailsModel.AppraisalSoldDetails = BreakDown1filteredappraisals;
                }
                #endregion Breadown 1

                #region Breakdown 2

                if (appraisalReportModel.BreakDownLevel2 != null)
                {
                    Label2Value = bdValue2;
                    switch (appraisalReportModel.BreakDownLevel2)
                    {

                        case "appraiser":
                            BreakDown2filteredappraisals = BreakDown1filteredappraisals.FindAll(x => x.Appraiser == bdValue2);
                            break;

                        case "associate":
                            BreakDown2filteredappraisals = BreakDown1filteredappraisals.FindAll(x => x.SalesAssociate == bdValue2);
                            break;

                        case "location":
                            BreakDown2filteredappraisals = BreakDown1filteredappraisals.FindAll(x => x.Location == bdValue2);

                            foreach (var store in SalesCommission.Business.Enums.AppraisalLocations)
                            {
                                if (store.LocationId.ToLower().Trim() == bdValue2.ToLower().Trim())
                                {
                                    Label2Value = store.Name;
                                }
                            }

                            break;

                        case "vehiclesource":
                            BreakDown2filteredappraisals = BreakDown1filteredappraisals.FindAll(x => x.VehicleSource == bdValue2);
                            break;

                        case "certified":
                            BreakDown2filteredappraisals = BreakDown1filteredappraisals.FindAll(x => x.Certification == bdValue2);
                            foreach (var cert in SalesCommission.Business.Enums.CertificationLevels)
                            {
                                if (cert.CertificationID.ToLower().Trim() == bdValue2.ToLower().Trim())
                                {
                                    Label2Value = cert.Name;
                                }
                            }
                            if (Label2Value == "")
                            {
                                Label2Value = "None";
                            }
                            break;

                        case "soldloc":
                            BreakDown2filteredappraisals = BreakDown1filteredappraisals.FindAll(x => x.SoldLocation == bdValue2);
                            break;

                        case "make":
                            BreakDown2filteredappraisals = BreakDown1filteredappraisals.FindAll(x => x.Make.Contains(bdValue2));
                            break;


                        case "stock":
                            BreakDown2filteredappraisals = BreakDown1filteredappraisals.FindAll(x => x.StockNumber == bdValue2);
                            break;

                        case "model":
                            BreakDown2filteredappraisals = BreakDown1filteredappraisals.FindAll(x => x.Model == bdValue2);
                            break;

                        case "monthyear":
                            BreakDown2filteredappraisals = BreakDown1filteredappraisals.FindAll(x => x.LastModifiedDate.ToString("MM/yyyy") == bdValue2);
                            var dealDate = Convert.ToDateTime(bdValue2);
                            Label2Value = dealDate.Month + "/" + dealDate.Year;
                            break;

                        case "zipcode":
                            BreakDown2filteredappraisals = BreakDown1filteredappraisals.FindAll(x => x.ZipCode == bdValue2);
                            break;

                        case "modelyear":
                            BreakDown2filteredappraisals = BreakDown1filteredappraisals.FindAll(x => x.Year == bdValue2);
                            break;

                        case "showroom":
                            BreakDown2filteredappraisals = BreakDown1filteredappraisals.FindAll(x => x.Showroom == bdValue2);
                            break;

                        case "days":
                            var daysbreakdown = Convert.ToDecimal(bdValue2);

                            if (daysbreakdown == 120)
                            {
                                BreakDown2filteredappraisals = BreakDown1filteredappraisals.FindAll(x => Convert.ToDecimal(x.DaysInStock) >= 90);
                            }
                            else
                            {
                                BreakDown2filteredappraisals = BreakDown1filteredappraisals.FindAll(x => Convert.ToDecimal(x.DaysInStock) < daysbreakdown && Convert.ToDecimal(x.DaysInStock) >= (daysbreakdown - 30));
                            }
                            break;

                        case "mileage":
                            var milesbreakdown = Convert.ToDecimal(bdValue2);

                            if (milesbreakdown == 500000)
                            {
                                BreakDown2filteredappraisals = BreakDown1filteredappraisals.FindAll(x => Convert.ToDecimal(x.Miles) >= 100000);
                            }
                            else
                            {
                                BreakDown2filteredappraisals = BreakDown1filteredappraisals.FindAll(x => Convert.ToDecimal(x.Miles) < milesbreakdown && Convert.ToDecimal(x.Miles) >= (milesbreakdown - 25000));
                            }
                            break;

                        case "price":
                            var pricebreakdown = Convert.ToDecimal(bdValue2);

                            if (pricebreakdown == 200000)
                            {
                                BreakDown2filteredappraisals = BreakDown1filteredappraisals.FindAll(x => x.SellingPrice >= 100000);
                            }
                            else
                            {
                                BreakDown2filteredappraisals = BreakDown1filteredappraisals.FindAll(x => x.SellingPrice < pricebreakdown && x.SellingPrice >= (pricebreakdown - 10000));
                            }
                            break;

                        default:
                            BreakDown2filteredappraisals = BreakDown1filteredappraisals.FindAll(x => x.Location == bdValue2);
                            break;


                    }
                    appraisalReportDetailsModel.AppraisalSoldDetails = BreakDown2filteredappraisals;
                }
                #endregion Breadown 2

                #region Breakdown 3

                if (appraisalReportModel.BreakDownLevel3 != null)
                {
                    Label3Value = bdValue3;
                    switch (appraisalReportModel.BreakDownLevel3)
                    {

                        case "appraiser":
                            BreakDown3filteredappraisals = BreakDown2filteredappraisals.FindAll(x => x.Appraiser == bdValue3);
                            break;

                        case "associate":
                            BreakDown3filteredappraisals = BreakDown2filteredappraisals.FindAll(x => x.SalesAssociate == bdValue3);
                            break;

                        case "location":
                            BreakDown3filteredappraisals = BreakDown2filteredappraisals.FindAll(x => x.Location == bdValue3);

                            foreach (var store in SalesCommission.Business.Enums.AppraisalLocations)
                            {
                                if (store.LocationId.ToLower().Trim() == bdValue3.ToLower().Trim())
                                {
                                    Label3Value = store.Name;
                                }
                            }

                            break;


                        case "vehiclesource":
                            BreakDown3filteredappraisals = BreakDown2filteredappraisals.FindAll(x => x.VehicleSource == bdValue3);
                            break;

                        case "certified":
                            BreakDown3filteredappraisals = BreakDown2filteredappraisals.FindAll(x => x.Certification == bdValue3);
                            foreach (var cert in SalesCommission.Business.Enums.CertificationLevels)
                            {
                                if (cert.CertificationID.ToLower().Trim() == bdValue3.ToLower().Trim())
                                {
                                    Label3Value = cert.Name;
                                }
                            }
                            if (Label3Value == "")
                            {
                                Label3Value = "None";
                            }
                            break;

                        case "soldloc":
                            BreakDown3filteredappraisals = BreakDown2filteredappraisals.FindAll(x => x.SoldLocation == bdValue3);
                            break;

                        case "make":
                            BreakDown3filteredappraisals = BreakDown2filteredappraisals.FindAll(x => x.Make.Contains(bdValue3));
                            break;


                        case "stock":
                            BreakDown3filteredappraisals = BreakDown2filteredappraisals.FindAll(x => x.StockNumber == bdValue3);
                            break;

                        case "model":
                            BreakDown3filteredappraisals = BreakDown2filteredappraisals.FindAll(x => x.Model == bdValue3);
                            break;

                        case "monthyear":
                            BreakDown3filteredappraisals = BreakDown2filteredappraisals.FindAll(x => x.LastModifiedDate.ToString("MM/yyyy") == bdValue3);
                            var dealDate = Convert.ToDateTime(bdValue3);
                            Label3Value = dealDate.Month + "/" + dealDate.Year;
                            break;

                        case "zipcode":
                            BreakDown3filteredappraisals = BreakDown2filteredappraisals.FindAll(x => x.ZipCode == bdValue3);
                            break;

                        case "modelyear":
                            BreakDown3filteredappraisals = BreakDown2filteredappraisals.FindAll(x => x.Year == bdValue3);
                            break;


                        case "showroom":
                            BreakDown3filteredappraisals = BreakDown2filteredappraisals.FindAll(x => x.Showroom == bdValue3);
                            break;

                        case "days":
                            var daysbreakdown = Convert.ToDecimal(bdValue3);

                            if (daysbreakdown == 120)
                            {
                                BreakDown3filteredappraisals = BreakDown2filteredappraisals.FindAll(x => Convert.ToDecimal(x.DaysInStock) >= 90);
                            }
                            else
                            {
                                BreakDown3filteredappraisals = BreakDown2filteredappraisals.FindAll(x => Convert.ToDecimal(x.DaysInStock) < daysbreakdown && Convert.ToDecimal(x.DaysInStock) >= (daysbreakdown - 30));
                            }
                            break;

                        case "mileage":
                            var milesbreakdown = Convert.ToDecimal(bdValue3);

                            if (milesbreakdown == 500000)
                            {
                                BreakDown3filteredappraisals = BreakDown2filteredappraisals.FindAll(x => Convert.ToDecimal(x.Miles) >= 100000);
                            }
                            else
                            {
                                BreakDown3filteredappraisals = BreakDown2filteredappraisals.FindAll(x => Convert.ToDecimal(x.Miles) < milesbreakdown && Convert.ToDecimal(x.Miles) >= (milesbreakdown - 25000));
                            }
                            break;

                        case "price":
                            var pricebreakdown = Convert.ToDecimal(bdValue3);

                            if (pricebreakdown == 200000)
                            {
                                BreakDown3filteredappraisals = BreakDown2filteredappraisals.FindAll(x => x.SellingPrice >= 100000);
                            }
                            else
                            {
                                BreakDown3filteredappraisals = BreakDown2filteredappraisals.FindAll(x => x.SellingPrice < pricebreakdown && x.SellingPrice >= (pricebreakdown - 10000));
                            }
                            break;
                        default:
                            BreakDown3filteredappraisals = BreakDown2filteredappraisals.FindAll(x => x.Location == bdValue3);
                            break;

                    }
                    appraisalReportDetailsModel.AppraisalSoldDetails = BreakDown3filteredappraisals;
                }
                #endregion Breadown 3

                #region Breakdown 4

                if (appraisalReportModel.BreakDownLevel4 != null)
                {
                    Label4Value = bdValue4;
                    switch (appraisalReportModel.BreakDownLevel4)
                    {

                        case "appraiser":
                            BreakDown4filteredappraisals = BreakDown3filteredappraisals.FindAll(x => x.Appraiser == bdValue4);
                            break;

                        case "associate":
                            BreakDown4filteredappraisals = BreakDown3filteredappraisals.FindAll(x => x.SalesAssociate == bdValue4);
                            break;

                        case "location":
                            BreakDown4filteredappraisals = BreakDown3filteredappraisals.FindAll(x => x.Location == bdValue4);

                            foreach (var store in SalesCommission.Business.Enums.Locations)
                            {
                                if (store.LocationId.ToLower().Trim() == bdValue4.ToLower().Trim())
                                {
                                    Label4Value = store.Name;
                                }
                            }

                            break;

                        case "vehiclesource":
                            BreakDown4filteredappraisals = BreakDown3filteredappraisals.FindAll(x => x.VehicleSource == bdValue4);
                            break;

                        case "certified":
                            BreakDown4filteredappraisals = BreakDown3filteredappraisals.FindAll(x => x.Certification == bdValue4);
                            foreach (var cert in SalesCommission.Business.Enums.CertificationLevels)
                            {
                                if (cert.CertificationID.ToLower().Trim() == bdValue4.ToLower().Trim())
                                {
                                    Label4Value = cert.Name;
                                }
                            }
                            if (Label4Value == "")
                            {
                                Label4Value = "None";
                            }
                            break;

                        case "soldloc":
                            BreakDown4filteredappraisals = BreakDown3filteredappraisals.FindAll(x => x.SoldLocation == bdValue4);
                            break;

                        case "make":
                            BreakDown4filteredappraisals = BreakDown3filteredappraisals.FindAll(x => x.Make.Contains(bdValue4));
                            break;

                        case "stock":
                            BreakDown4filteredappraisals = BreakDown3filteredappraisals.FindAll(x => x.StockNumber == bdValue4);
                            break;

                        case "model":
                            BreakDown4filteredappraisals = BreakDown3filteredappraisals.FindAll(x => x.Model == bdValue4);
                            break;

                        case "monthyear":
                            BreakDown4filteredappraisals = BreakDown3filteredappraisals.FindAll(x => x.LastModifiedDate.ToString("MM/yyyy") == bdValue4);
                            var dealDate = Convert.ToDateTime(bdValue4);
                            Label4Value = dealDate.Month + "/" + dealDate.Year;
                            break;

                        case "zipcode":
                            BreakDown4filteredappraisals = BreakDown3filteredappraisals.FindAll(x => x.ZipCode == bdValue4);
                            break;

                        case "modelyear":
                            BreakDown4filteredappraisals = BreakDown3filteredappraisals.FindAll(x => x.Year == bdValue4);
                            break;

                        case "showroom":
                            BreakDown4filteredappraisals = BreakDown3filteredappraisals.FindAll(x => x.Showroom == bdValue4);
                            break;

                        case "days":
                            var daysbreakdown = Convert.ToDecimal(bdValue4);

                            if (daysbreakdown == 120)
                            {
                                BreakDown4filteredappraisals = BreakDown3filteredappraisals.FindAll(x => Convert.ToDecimal(x.DaysInStock) >= 90);
                            }
                            else
                            {
                                BreakDown4filteredappraisals = BreakDown3filteredappraisals.FindAll(x => Convert.ToDecimal(x.DaysInStock) < daysbreakdown && Convert.ToDecimal(x.DaysInStock) >= (daysbreakdown - 30));
                            }
                            break;

                        case "mileage":
                            var milesbreakdown = Convert.ToDecimal(bdValue4);

                            if (milesbreakdown == 500000)
                            {
                                BreakDown4filteredappraisals = BreakDown3filteredappraisals.FindAll(x => Convert.ToDecimal(x.Miles) >= 100000);
                            }
                            else
                            {
                                BreakDown4filteredappraisals = BreakDown3filteredappraisals.FindAll(x => Convert.ToDecimal(x.Miles) < milesbreakdown && Convert.ToDecimal(x.Miles) >= (milesbreakdown - 25000));
                            }
                            break;

                        case "price":
                            var pricebreakdown = Convert.ToDecimal(bdValue4);

                            if (pricebreakdown == 200000)
                            {
                                BreakDown4filteredappraisals = BreakDown3filteredappraisals.FindAll(x => x.SellingPrice >= 100000);
                            }
                            else
                            {
                                BreakDown4filteredappraisals = BreakDown3filteredappraisals.FindAll(x => x.SellingPrice < pricebreakdown && x.SellingPrice >= (pricebreakdown - 10000));
                            }
                            break;
                        default:
                            BreakDown4filteredappraisals = BreakDown3filteredappraisals.FindAll(x => x.Location == bdValue4);
                            break;

                    }
                    appraisalReportDetailsModel.AppraisalSoldDetails = BreakDown4filteredappraisals;
                }
                #endregion Breadown 4

            }

            //NOW GO THROUGH BREAKDOWNS AND RETURN THE LAST BREAKDOWN


            return View(appraisalReportDetailsModel);
        }


        public ActionResult ExceptionReportDetails(string startmonth, string startyear, string endmonth, string endyear, string bd1, string bd2, string bd3, string bd4, string dt)
        {
            var exceptionReportModel = new ExceptionReportModel();
            var exceptionReportDetailsModel = new ExceptionReportDetailModel();

            var bdValue1 = "";
            var bdValue2 = "";
            var bdValue3 = "";
            var bdValue4 = "";

            if (bd1 != null && bd1 != "")
            {
                exceptionReportModel.BreakDownLevel1 = bd1.Split(',')[0];
                //bdValue1 = bd1.Split(',')[1];
                bdValue1 = bd1.Replace(bd1.Split(',')[0] + ",", "");

                exceptionReportDetailsModel.BreakDownLevel1 = bd1.Split(',')[0];
                exceptionReportDetailsModel.BreakDownLevel1Value = bdValue1;
            }

            if (bd2 != null && bd2 != "")
            {
                exceptionReportModel.BreakDownLevel2 = bd2.Split(',')[0];
                //bdValue2 = bd2.Split(',')[1];
                bdValue2 = bd2.Replace(bd2.Split(',')[0] + ",", "");

                exceptionReportDetailsModel.BreakDownLevel2 = bd2.Split(',')[0];
                exceptionReportDetailsModel.BreakDownLevel2Value = bdValue2;
            }

            if (bd3 != null && bd3 != "")
            {
                exceptionReportModel.BreakDownLevel3 = bd3.Split(',')[0];
                //bdValue3 = bd3.Split(',')[1];
                bdValue3 = bd3.Replace(bd3.Split(',')[0] + ",", "");

                exceptionReportDetailsModel.BreakDownLevel3 = bd3.Split(',')[0];
                exceptionReportDetailsModel.BreakDownLevel3Value = bdValue3;
            }

            if (bd4 != null && bd4 != "")
            {
                exceptionReportModel.BreakDownLevel4 = bd4.Split(',')[0];
                //bdValue4 = bd4.Split(',')[1];
                bdValue4 = bd4.Replace(bd4.Split(',')[0] + ",", "");

                exceptionReportDetailsModel.BreakDownLevel4 = bd4.Split(',')[0];
                exceptionReportDetailsModel.BreakDownLevel4Value = bdValue4;
            }

            if (dt != null && dt != "")
            {
                exceptionReportModel.DealType = dt;
                exceptionReportDetailsModel.DealType = dt;
            }

            exceptionReportModel.ReportStartMonth = Convert.ToInt32(startmonth);
            exceptionReportDetailsModel.ReportStartMonth = Convert.ToInt32(startmonth);

            exceptionReportModel.ReportEndMonth = Convert.ToInt32(endmonth);
            exceptionReportDetailsModel.ReportEndMonth = Convert.ToInt32(endmonth);

            exceptionReportModel.ReportStartYear = Convert.ToInt32(startyear);
            exceptionReportDetailsModel.ReportStartYear = Convert.ToInt32(startyear);

            exceptionReportModel.ReportEndYear = Convert.ToInt32(endyear);
            exceptionReportDetailsModel.ReportEndYear = Convert.ToInt32(endyear);

            var reportDate = new DateTime(Convert.ToInt32(exceptionReportModel.ReportStartYear), Convert.ToInt32(exceptionReportModel.ReportStartMonth), 4);
            var reportEndDate = new DateTime(Convert.ToInt32(exceptionReportModel.ReportEndYear), Convert.ToInt32(exceptionReportModel.ReportEndMonth), 6);//.AddMonths(1);

            exceptionReportModel.ExceptionDetails = SqlMapperUtil.StoredProcWithParams<IndividualDealDetails>("sp_SalesLogReportGetAllDealsByDateRange", new { ReportStartDate = reportDate.Date, ReportEndDate = reportEndDate.Date }, "SalesCommission");

            if (dt != "All")
            {
                if (dt == "Finance")
                {
                    exceptionReportModel.ExceptionDetails = exceptionReportModel.ExceptionDetails.FindAll(x => x.sl_VehicleTerm != 1);

                }
                else if (dt == "Cash")
                {
                    exceptionReportModel.ExceptionDetails = exceptionReportModel.ExceptionDetails.FindAll(x => x.sl_VehicleTerm == 1);

                }

            }

            foreach (var detail in exceptionReportModel.ExceptionDetails)
            {
                if (detail.sl_FinMgr == null)
                {
                    detail.sl_FinMgr = "None";
                }
                if (detail.sl_SalesAssociate1 == null)
                {
                    detail.sl_SalesAssociate1 = "None";
                }
                if (detail.sl_rate_exception == null || detail.sl_rate_exception == "")
                {
                    detail.sl_rate_exception = "Cash Deal";
                }
                if (detail.sl_price_variance_exception == null)
                {
                    detail.sl_price_variance_exception = "";
                }
            }

            var Label1Value = "";
            var Label2Value = "";
            var Label3Value = "";
            var Label4Value = "";

            var BreakDown1filteredappraisals = new List<SalesCommission.Models.IndividualDealDetails>();
            var BreakDown2filteredappraisals = new List<SalesCommission.Models.IndividualDealDetails>();
            var BreakDown3filteredappraisals = new List<SalesCommission.Models.IndividualDealDetails>();
            var BreakDown4filteredappraisals = new List<SalesCommission.Models.IndividualDealDetails>();

            if (exceptionReportModel.ExceptionDetails != null && exceptionReportModel.ExceptionDetails.Count > 0)
            {
                #region Breakdown 1

                if (exceptionReportModel.BreakDownLevel1 != null)
                {
                    Label1Value = bdValue1;
                    switch (exceptionReportModel.BreakDownLevel1)
                    {
                        
                        case "location":
                            BreakDown1filteredappraisals = exceptionReportModel.ExceptionDetails.FindAll(x => x.sl_VehicleLoc == bdValue1);
                            
                            break;

                        case "bankname":
                            BreakDown1filteredappraisals = exceptionReportModel.ExceptionDetails.FindAll(x => x.sl_BankName == bdValue1);
                            break;

                        case "financemanager":
                            BreakDown1filteredappraisals = exceptionReportModel.ExceptionDetails.FindAll(x => x.sl_FinMgr == bdValue1);
                            break;

                        case "priceexception":
                            BreakDown1filteredappraisals = exceptionReportModel.ExceptionDetails.FindAll(x => x.sl_price_variance_exception == bdValue1);
                            
                            break;

                        case "rateexception":
                            BreakDown1filteredappraisals = exceptionReportModel.ExceptionDetails.FindAll(x => x.sl_rate_exception == bdValue1);
                            
                            break;

                        case "salesassociate":
                            BreakDown1filteredappraisals = exceptionReportModel.ExceptionDetails.FindAll(x => x.sl_SalesAssociate1 == bdValue1);
                            break;

                    }

                    exceptionReportDetailsModel.ExceptionDetails = BreakDown1filteredappraisals;
                }
                #endregion Breadown 1

                #region Breakdown 2

                if (exceptionReportModel.BreakDownLevel2 != null)
                {
                    Label2Value = bdValue2;
                    switch (exceptionReportModel.BreakDownLevel2)
                    {
                        
                        case "location":
                            BreakDown2filteredappraisals = BreakDown1filteredappraisals.FindAll(x => x.sl_VehicleLoc == bdValue2);

                            break;

                        case "bankname":
                            BreakDown2filteredappraisals = BreakDown1filteredappraisals.FindAll(x => x.sl_BankName == bdValue2);
                            break;

                        case "financemanager":
                            BreakDown2filteredappraisals = BreakDown1filteredappraisals.FindAll(x => x.sl_FinMgr == bdValue2);
                            break;

                        case "priceexception":
                            BreakDown2filteredappraisals = BreakDown1filteredappraisals.FindAll(x => x.sl_price_variance_exception == bdValue2);

                            break;

                        case "rateexception":
                            BreakDown2filteredappraisals = BreakDown1filteredappraisals.FindAll(x => x.sl_rate_exception == bdValue2);

                            break;

                        case "salesassociate":
                            BreakDown2filteredappraisals = BreakDown1filteredappraisals.FindAll(x => x.sl_SalesAssociate1 == bdValue2);
                            break;


                    }
                    exceptionReportDetailsModel.ExceptionDetails = BreakDown2filteredappraisals;
                }
                #endregion Breadown 2

                #region Breakdown 3

                if (exceptionReportModel.BreakDownLevel3 != null)
                {
                    Label3Value = bdValue3;
                    switch (exceptionReportModel.BreakDownLevel3)
                    {


                        case "location":
                            BreakDown3filteredappraisals = BreakDown2filteredappraisals.FindAll(x => x.sl_VehicleLoc == bdValue3);

                            break;

                        case "bankname":
                            BreakDown3filteredappraisals = BreakDown2filteredappraisals.FindAll(x => x.sl_BankName == bdValue3);
                            break;

                        case "financemanager":
                            BreakDown3filteredappraisals = BreakDown2filteredappraisals.FindAll(x => x.sl_FinMgr == bdValue3);
                            break;

                        case "priceexception":
                            BreakDown3filteredappraisals = BreakDown2filteredappraisals.FindAll(x => x.sl_price_variance_exception == bdValue3);

                            break;

                        case "rateexception":
                            BreakDown3filteredappraisals = BreakDown2filteredappraisals.FindAll(x => x.sl_rate_exception == bdValue3);

                            break;

                        case "salesassociate":
                            BreakDown3filteredappraisals = BreakDown2filteredappraisals.FindAll(x => x.sl_SalesAssociate1 == bdValue3);
                            break;

                    }
                    exceptionReportDetailsModel.ExceptionDetails = BreakDown3filteredappraisals;
                }
                #endregion Breadown 3

                #region Breakdown 4

                if (exceptionReportModel.BreakDownLevel4 != null)
                {
                    Label4Value = bdValue4;
                    switch (exceptionReportModel.BreakDownLevel4)
                    {

                        case "location":
                            BreakDown4filteredappraisals = BreakDown3filteredappraisals.FindAll(x => x.sl_VehicleLoc == bdValue4);

                            break;

                        case "bankname":
                            BreakDown4filteredappraisals = BreakDown3filteredappraisals.FindAll(x => x.sl_BankName == bdValue4);
                            break;

                        case "financemanager":
                            BreakDown4filteredappraisals = BreakDown3filteredappraisals.FindAll(x => x.sl_FinMgr == bdValue4);
                            break;

                        case "priceexception":
                            BreakDown4filteredappraisals = BreakDown3filteredappraisals.FindAll(x => x.sl_price_variance_exception == bdValue4);

                            break;

                        case "rateexception":
                            BreakDown4filteredappraisals = BreakDown3filteredappraisals.FindAll(x => x.sl_rate_exception == bdValue4);

                            break;

                        case "salesassociate":
                            BreakDown4filteredappraisals = BreakDown3filteredappraisals.FindAll(x => x.sl_SalesAssociate1 == bdValue4);
                            break;


                    }
                    exceptionReportDetailsModel.ExceptionDetails = BreakDown4filteredappraisals;
                }
                #endregion Breadown 4

            }

            //NOW GO THROUGH BREAKDOWNS AND RETURN THE LAST BREAKDOWN


            return View(exceptionReportDetailsModel);
        }

        public ActionResult AppraisalReportTradeDetails()
        {
            var appraisalReportDetailsModel = new AppraisalReportDetailModel();

            var bdValue1 = "";
            var bdValue2 = "";
            var bdValue3 = "";
            var bdValue4 = "";

            if (Request.Form["bd1"] != null && Request.Form["bd1"] != "")
            {
                bdValue1 = Request.Form["bd1"].Replace(Request.Form["bd1"].Split(',')[0] + ",", "");

                appraisalReportDetailsModel.BreakDownLevel1 = Request.Form["bd1"].Split(',')[0];
                appraisalReportDetailsModel.BreakDownLevel1Value = bdValue1;
            }

            if (Request.Form["bd2"] != null && Request.Form["bd2"] != "")
            {
                bdValue2 = Request.Form["bd2"].Replace(Request.Form["bd2"].Split(',')[0] + ",", "");

                appraisalReportDetailsModel.BreakDownLevel2 = Request.Form["bd2"].Split(',')[0];
                appraisalReportDetailsModel.BreakDownLevel2Value = bdValue2;
            }

            if (Request.Form["bd3"] != null && Request.Form["bd3"] != "")
            {
                bdValue3 = Request.Form["bd3"].Replace(Request.Form["bd3"].Split(',')[0] + ",", "");

                appraisalReportDetailsModel.BreakDownLevel3 = Request.Form["bd3"].Split(',')[0];
                appraisalReportDetailsModel.BreakDownLevel3Value = bdValue3;
            }

            if (Request.Form["bd4"] != null && Request.Form["bd4"] != "")
            {
                bdValue4 = Request.Form["bd4"].Replace(Request.Form["bd4"].Split(',')[0] + ",", "");

                appraisalReportDetailsModel.BreakDownLevel4 = Request.Form["bd4"].Split(',')[0];
                appraisalReportDetailsModel.BreakDownLevel4Value = bdValue4;
            }

            if (Request.Form["vt"] != null && Request.Form["vt"] != "")
            {
                appraisalReportDetailsModel.VehicleType = Request.Form["vt"];
            }

            if (Request.Form["at"] != null && Request.Form["at"] != "")
            {
                appraisalReportDetailsModel.AcquisitionType = Request.Form["at"];
            }


            appraisalReportDetailsModel.ReportStartMonth = Convert.ToInt32(Request.Form["startmonth"]);

            appraisalReportDetailsModel.ReportEndMonth = Convert.ToInt32(Request.Form["endmonth"]);

            appraisalReportDetailsModel.ReportStartYear = Convert.ToInt32(Request.Form["startyear"]);

            appraisalReportDetailsModel.ReportEndYear = Convert.ToInt32(Request.Form["endyear"]);



            if (Request.Form["tradeDeals"] != null)
            {
                var deals = Request.Form["tradeDeals"];
                var trimmedDeals = "";

                var tradeDeals = SqlQueries.GetTradeDealDetails(deals);

                foreach(var deal in deals.Split(','))
                {
                    if (deal != "")
                    {
                        trimmedDeals += deal.Substring(3) + ",";
                    }
                }

                var wholesaleDeals = SqlQueries.GetWholeSaleTradeDealDetails(trimmedDeals);

                tradeDeals.AddRange(wholesaleDeals);

                //Now get the VIN List
                var VINList = new List<string>();
                foreach(var deal in tradeDeals)
                {
                    if(deal.TradeVIN != null && deal.TradeVIN != "")
                    {
                        VINList.Add(deal.TradeVIN);
                    }
                    if (deal.Trade2VIN != null && deal.Trade2VIN != "")
                    {
                        VINList.Add(deal.Trade2VIN);
                    }
                }

                SqlQueries.DeleteVINTradeSearchTable();
                SqlQueries.InsertIntoVINTradeSearchTable(VINList);
                appraisalReportDetailsModel.TradeAcquisitionDetails =  SqlQueries.GetTradeAcquisitionReportDetailsByVIN();
                appraisalReportDetailsModel.TradeAcquisitionDetails = appraisalReportDetailsModel.TradeAcquisitionDetails.OrderByDescending(z => z.VIN).ThenByDescending(z => z.AppraisalDate).GroupBy(x => x.VIN).Select(g => g.First()).ToList();

                //appraisalReportDetailsModel.TradeAcquisitionDetails = appraisalReportDetailsModel.TradeAcquisitionDetails.FindAll(x => appraisalReportDetailsModel.StatusType.Split(',').ToList().Contains(x.Status.ToString()));

                var additionalDetails = SqlQueries.GetTradeAcquisitionAdditionalDetails();

                appraisalReportDetailsModel.vAutoInventoryDetails = SqlQueries.GetVAutoCurrentInventory();

                foreach (var appraisal in appraisalReportDetailsModel.TradeAcquisitionDetails)
                {
                    var stockSource = "";
                    var stockSourceName = "";
                    var lastChar = "";


                    if (appraisal.StockNumber != null && appraisal.StockNumber.Length > 1)
                    {
                        stockSource = appraisal.StockNumber.Substring(1, 1);
                        lastChar = appraisal.StockNumber.Substring(appraisal.StockNumber.Length - 1);

                        var details = additionalDetails.Find(x => x.StockNumber.Trim() == appraisal.StockNumber);
                        if (details != null)
                        {
                            appraisal.StyleId = details.StyleId;
                            appraisal.Certification = details.Certification;
                            appraisal.BodyStyle = details.BodyStyle;
                            appraisal.XrefId = details.XrefId;
                            appraisal.LeadCount = details.LeadCount;
                        }
                        else
                        {
                            appraisal.StyleId = "None";
                            appraisal.Certification = "None";
                            appraisal.BodyStyle = "unknown";
                            appraisal.BodyStyle = "";
                        }

                    }
                    switch (stockSource)
                    {
                        case "A":
                            stockSourceName = "Auction or Wholesale";
                            break;
                        case "B":
                            stockSourceName = "Lease Buyout";
                            break;
                        case "C":
                            stockSourceName = "Car Offer";
                            break;
                        case "D":
                            stockSourceName = "Demo Prev Rental";
                            break;
                        case "E":
                            stockSourceName = "Demo";
                            break;
                        case "F":
                            stockSourceName = "Nextcar Bad Hist";
                            break;
                        case "G":
                            stockSourceName = "Government Auction";
                            break;
                        case "K":
                            stockSourceName = "Kelly ICO";
                            break;
                        case "L":
                            stockSourceName = "In House Loaner";
                            break;
                        case "M":
                            stockSourceName = "Facebook Marketplace";
                            break;
                        case "N":
                            stockSourceName = "Nextcar Purchase";
                            break;
                        case "P":
                            stockSourceName = "Purchase/Lease Cust";
                            break;
                        case "R":
                            stockSourceName = "Rental";
                            break;
                        case "X":
                            stockSourceName = "Previous Repo";
                            break;
                        case "V":
                            stockSourceName = "VinCue";
                            break;
                        default:
                            stockSource = "";
                            stockSourceName = "";
                            break;
                    }

                    if (lastChar != "" && Char.IsLetter(lastChar, 0))
                    {
                        switch (lastChar)
                        {
                            case "K":
                                stockSourceName = "Kelly ICO";
                                break;
                            case "M":
                                stockSourceName = "Facebook Marketplace";
                                break;
                            default:
                                stockSourceName = "Trade";
                                break;
                        }
                    }

                    appraisal.VehicleSource = stockSourceName;

                    if (appraisal.LeadSourceName == null)
                    {
                        appraisal.LeadSourceName = "";
                    }

                    if (appraisal.LeadGroup == null)
                    {
                        appraisal.LeadGroup = "";
                    }

                    if (appraisal.Certification == null || appraisal.Certification == "")
                    {
                        appraisal.Certification = "None";
                    }

                    if (appraisal.Appraiser == null)
                    {
                        appraisal.Appraiser = "";
                    }

                    if (appraisal.Make == null)
                    {
                        appraisal.Make = "";
                    }

                    if (appraisal.Carline != null && appraisal.Carline.IndexOf(" ") > 0)
                    {
                        appraisal.Carline = appraisal.Carline.Substring(0, appraisal.Carline.IndexOf(" "));
                    }
                }


                var reportDate = new DateTime(Convert.ToInt32(appraisalReportDetailsModel.ReportStartYear), Convert.ToInt32(appraisalReportDetailsModel.ReportStartMonth), 1);

                appraisalReportDetailsModel.AppraisalSoldDetails = SqlMapperUtil.StoredProcWithParams<AppraisalSoldDetail>("sp_CommissionGetAssociateAppraisalsSoldDetailsByVIN", new { StartDate = reportDate.Date }, "ReynoldsData");
                appraisalReportDetailsModel.AppraisalSoldDetails = appraisalReportDetailsModel.AppraisalSoldDetails.OrderByDescending(z => z.VIN).ThenByDescending(z => z.LastModifiedDate).GroupBy(x => x.VIN).Select(g => g.First()).ToList();

                foreach (var appraisal in appraisalReportDetailsModel.AppraisalSoldDetails)
                {
                    var stockSource = "";
                    var stockSourceName = "";
                    var lastChar = "";

                    if (appraisal.StockNumber != null)
                    {
                        stockSource = appraisal.StockNumber.Substring(1, 1);
                        lastChar = appraisal.StockNumber.Substring(appraisal.StockNumber.Length - 1);
                    }
                    switch (stockSource)
                    {
                        case "A":
                            stockSourceName = "Auction or Wholesale";
                            break;
                        case "B":
                            stockSourceName = "Lease Buyout";
                            break;
                        case "C":
                            stockSourceName = "Car Offer";
                            break;
                        case "D":
                            stockSourceName = "Demo Prev Rental";
                            break;
                        case "E":
                            stockSourceName = "Demo";
                            break;
                        case "F":
                            stockSourceName = "Nextcar Bad Hist";
                            break;
                        case "G":
                            stockSourceName = "Government Auction";
                            break;
                        case "K":
                            stockSourceName = "Kelly ICO";
                            break;
                        case "L":
                            stockSourceName = "In House Loaner";
                            break;
                        case "M":
                            stockSourceName = "Facebook Marketplace";
                            break;
                        case "N":
                            stockSourceName = "Nextcar Purchase";
                            break;
                        case "P":
                            stockSourceName = "Purchase/Lease Cust";
                            break;
                        case "R":
                            stockSourceName = "Rental";
                            break;
                        case "X":
                            stockSourceName = "Previous Repo";
                            break;
                        default:
                            stockSource = "";
                            stockSourceName = "";
                            break;
                    }

                    if (lastChar != "" && Char.IsLetter(lastChar, 0))
                    {
                        switch (lastChar)
                        {
                            case "K":
                                stockSourceName = "Kelly ICO";
                                break;
                            case "M":
                                stockSourceName = "Facebook Marketplace";
                                break;
                            default:
                                stockSourceName = "Trade";
                                break;
                        }
                    }

                    appraisal.VehicleSource = stockSourceName;

                    if (appraisal.ZipCode == null || appraisal.ZipCode == "")
                    {
                        appraisal.ZipCode = "None";
                    }

                    if (appraisal.Certification == null || appraisal.Certification == "")
                    {
                        appraisal.Certification = "None";
                    }

                    if (appraisal.Certification == "FWP")
                    {
                        appraisal.Certification = "FP";
                    }

                    if (appraisal.Certification == "MC" || appraisal.Certification == "CUV" || appraisal.Certification == "MZC" || appraisal.Certification == "GMU" || appraisal.Certification == "VWC")
                    {
                        appraisal.Certification = "CPO";
                    }

                    if (appraisal.Make == null)
                    {
                        appraisal.Make = "";
                    }
                    else
                    {
                        appraisal.Make = appraisal.Make.Replace(" TRUCK", "");
                    }

                    if (appraisal.Model != null && appraisal.Model.IndexOf(" ") > 0)
                    {
                        appraisal.Model = appraisal.Model.Substring(0, appraisal.Model.IndexOf(" "));
                    }
                }


            }


            return View(appraisalReportDetailsModel);
        }

        //public ActionResult AppraisalReportTradeDetails(string startmonth, string startyear, string endmonth, string endyear, string bd1, string bd2, string bd3, string bd4, string vt, string at, string days)
        //{
        //    var appraisalReportModel = new AppraisalReportModel();
        //    var appraisalReportDetailsModel = new AppraisalReportDetailModel();

        //    var bdValue1 = "";
        //    var bdValue2 = "";
        //    var bdValue3 = "";
        //    var bdValue4 = "";

        //    if (bd1 != null && bd1 != "")
        //    {
        //        appraisalReportModel.BreakDownLevel1 = bd1.Split(',')[0];
        //        //bdValue1 = bd1.Split(',')[1];
        //        bdValue1 = bd1.Replace(bd1.Split(',')[0] + ",","");

        //        appraisalReportDetailsModel.BreakDownLevel1 = bd1.Split(',')[0];
        //        appraisalReportDetailsModel.BreakDownLevel1Value = bdValue1;
        //    }

        //    if (bd2 != null && bd2 != "")
        //    {
        //        appraisalReportModel.BreakDownLevel2 = bd2.Split(',')[0];
        //        //bdValue2 = bd2.Split(',')[1];
        //        bdValue2 = bd2.Replace(bd2.Split(',')[0] + ",", "");

        //        appraisalReportDetailsModel.BreakDownLevel2 = bd2.Split(',')[0];
        //        appraisalReportDetailsModel.BreakDownLevel2Value = bdValue2;
        //    }

        //    if (bd3 != null && bd3 != "")
        //    {
        //        appraisalReportModel.BreakDownLevel3 = bd3.Split(',')[0];
        //        //bdValue3 = bd3.Split(',')[1];
        //        bdValue3 = bd3.Replace(bd3.Split(',')[0] + ",", "");

        //        appraisalReportDetailsModel.BreakDownLevel3 = bd3.Split(',')[0];
        //        appraisalReportDetailsModel.BreakDownLevel3Value = bdValue3;
        //    }

        //    if (bd4 != null && bd4 != "")
        //    {
        //        appraisalReportModel.BreakDownLevel4 = bd4.Split(',')[0];
        //        //bdValue4 = bd4.Split(',')[1];
        //        bdValue4 = bd4.Replace(bd4.Split(',')[0] + ",", "");

        //        appraisalReportDetailsModel.BreakDownLevel4 = bd4.Split(',')[0];
        //        appraisalReportDetailsModel.BreakDownLevel4Value = bdValue4;
        //    }

        //    if (vt != null && vt != "")
        //    {
        //        appraisalReportModel.VehicleType = vt;
        //        appraisalReportDetailsModel.VehicleType = vt;
        //    }

        //    if (at != null && at != "")
        //    {
        //        appraisalReportModel.AcquisitionType = at;
        //        appraisalReportDetailsModel.AcquisitionType = at;
        //    }


        //    appraisalReportModel.ReportStartMonth = Convert.ToInt32(startmonth);
        //    appraisalReportDetailsModel.ReportStartMonth = Convert.ToInt32(startmonth);

        //    appraisalReportModel.ReportEndMonth = Convert.ToInt32(endmonth);
        //    appraisalReportDetailsModel.ReportEndMonth = Convert.ToInt32(endmonth);

        //    appraisalReportModel.ReportStartYear = Convert.ToInt32(startyear);
        //    appraisalReportDetailsModel.ReportStartYear = Convert.ToInt32(startyear);

        //    appraisalReportModel.ReportEndYear = Convert.ToInt32(endyear);
        //    appraisalReportDetailsModel.ReportEndYear = Convert.ToInt32(endyear);

        //    var reportDate = new DateTime(Convert.ToInt32(appraisalReportModel.ReportStartYear), Convert.ToInt32(appraisalReportModel.ReportStartMonth), 1);
        //    var reportEndDate = new DateTime(Convert.ToInt32(appraisalReportModel.ReportEndYear), Convert.ToInt32(appraisalReportModel.ReportEndMonth), 1);//.AddMonths(1);

        //    appraisalReportModel.AppraisalSoldDetails = SqlMapperUtil.StoredProcWithParams<AppraisalSoldDetail>("sp_CommissionGetAssociateAppraisalsSoldDetails", new { StartDate = reportDate.Date, EndDate = reportEndDate.Date }, "ReynoldsData");
        //    //Remove Duplicates
        //    appraisalReportModel.AppraisalSoldDetails = appraisalReportModel.AppraisalSoldDetails.OrderByDescending(z => z.VIN).ThenByDescending(z => z.LastModifiedDate).GroupBy(x => x.VIN).Select(g => g.First()).ToList();


        //    if (vt != "All")
        //    {
        //        if (vt == "Retail")
        //        {
        //            appraisalReportModel.AppraisalSoldDetails = appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.WholesaleRetail == "R");

        //        }
        //        else if (vt == "Wholesale")
        //        {
        //            appraisalReportModel.AppraisalSoldDetails = appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.WholesaleRetail == "W");

        //        }

        //    }

        //    if (at != "ALL")
        //    {
        //        if (at == "Trade")
        //        {
        //            appraisalReportModel.AppraisalSoldDetails = appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.TradePurchase == "T");
        //        }
        //        else if (at == "Purchase")
        //        {
        //            appraisalReportModel.AppraisalSoldDetails = appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.TradePurchase == "P");
        //        }
        //    }

        //    if (days != null && days != "")
        //    {
        //        switch (days)
        //        {
        //            case "30":
        //                appraisalReportModel.AppraisalSoldDetails = appraisalReportModel.AppraisalSoldDetails.FindAll(x => Int32.Parse(x.DaysInStock) < 30);
        //                break;
        //            case "3060":
        //                appraisalReportModel.AppraisalSoldDetails = appraisalReportModel.AppraisalSoldDetails.FindAll(x => Int32.Parse(x.DaysInStock) >= 30 && Int32.Parse(x.DaysInStock) < 60);

        //                break;
        //            case "6090":
        //                appraisalReportModel.AppraisalSoldDetails = appraisalReportModel.AppraisalSoldDetails.FindAll(x => Int32.Parse(x.DaysInStock) >= 60 && Int32.Parse(x.DaysInStock) < 90);
        //                break;
        //            case "90":
        //                appraisalReportModel.AppraisalSoldDetails = appraisalReportModel.AppraisalSoldDetails.FindAll(x => Int32.Parse(x.DaysInStock) > 90);
        //                break;
        //        }
        //    }

        //    foreach (var appraisal in appraisalReportModel.AppraisalSoldDetails)
        //    {
        //        var stockSource = "";
        //        var stockSourceName = "";
        //        var lastChar = "";

        //        if (appraisal.StockNumber != null)
        //        {
        //            stockSource = appraisal.StockNumber.Substring(1, 1);
        //            lastChar = appraisal.StockNumber.Substring(appraisal.StockNumber.Length - 1);
        //        }
        //        switch (stockSource)
        //        {
        //            case "A":
        //                stockSourceName = "Auction or Wholesale";
        //                break;
        //            case "B":
        //                stockSourceName = "Lease Buyout";
        //                break;
        //            case "C":
        //                stockSourceName = "Car Offer";
        //                break;
        //            case "D":
        //                stockSourceName = "Demo Prev Rental";
        //                break;
        //            case "E":
        //                stockSourceName = "Demo";
        //                break;
        //            case "F":
        //                stockSourceName = "Nextcar Bad Hist";
        //                break;
        //            case "G":
        //                stockSourceName = "Government Auction";
        //                break;
        //            case "K":
        //                stockSourceName = "Kelly ICO";
        //                break;
        //            case "L":
        //                stockSourceName = "In House Loaner";
        //                break;
        //            case "M":
        //                stockSourceName = "Facebook Marketplace";
        //                break;
        //            case "N":
        //                stockSourceName = "Nextcar Purchase";
        //                break;
        //            case "P":
        //                stockSourceName = "Purchase/Lease Cust";
        //                break;
        //            case "R":
        //                stockSourceName = "Rental";
        //                break;
        //            case "X":
        //                stockSourceName = "Previous Repo";
        //                break;
        //            default:
        //                stockSource = "";
        //                stockSourceName = "";
        //                break;
        //        }

        //        if (lastChar != "" && Char.IsLetter(lastChar, 0))
        //        {
        //            switch (lastChar)
        //            {
        //                case "K":
        //                    stockSourceName = "Kelly ICO";
        //                    break;
        //                case "M":
        //                    stockSourceName = "Facebook Marketplace";
        //                    break;
        //                default:
        //                    stockSourceName = "Trade";
        //                    break;
        //            }
        //        }

        //        appraisal.VehicleSource = stockSourceName;

        //        if(appraisal.ZipCode == null || appraisal.ZipCode == "")
        //        {
        //            appraisal.ZipCode = "None";
        //        }

        //        if(appraisal.Certification == null || appraisal.Certification == "")
        //        {
        //            appraisal.Certification = "None";
        //        }

        //        if (appraisal.Certification == "FWP")
        //        {
        //            appraisal.Certification = "FP";
        //        }

        //        if (appraisal.Certification == "MC" || appraisal.Certification == "CUV" || appraisal.Certification == "MZC" || appraisal.Certification == "GMU" || appraisal.Certification == "VWC")
        //        {
        //            appraisal.Certification = "CPO";
        //        }

        //        if(appraisal.Make == null)
        //        {
        //            appraisal.Make = "";
        //        }
        //        else
        //        {
        //            appraisal.Make = appraisal.Make.Replace(" TRUCK", "");
        //        }

        //        if (appraisal.Model != null && appraisal.Model.IndexOf(" ") > 0)
        //        {
        //            appraisal.Model = appraisal.Model.Substring(0, appraisal.Model.IndexOf(" "));
        //        }
        //    }

        //    var Label1Value = "";
        //    var Label2Value = "";
        //    var Label3Value = "";
        //    var Label4Value = "";

        //    var BreakDown1filteredappraisals = new List<SalesCommission.Models.AppraisalSoldDetail>();
        //    var BreakDown2filteredappraisals = new List<SalesCommission.Models.AppraisalSoldDetail>();
        //    var BreakDown3filteredappraisals = new List<SalesCommission.Models.AppraisalSoldDetail>();
        //    var BreakDown4filteredappraisals = new List<SalesCommission.Models.AppraisalSoldDetail>();

        //    if (appraisalReportModel.AppraisalSoldDetails != null && appraisalReportModel.AppraisalSoldDetails.Count > 0)
        //    {



        //        #region Breakdown 1

        //        if (appraisalReportModel.BreakDownLevel1 != null)
        //        {
        //            Label1Value = bdValue1;
        //            switch (appraisalReportModel.BreakDownLevel1)
        //            {

        //                case "appraiser":
        //                    BreakDown1filteredappraisals = appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Appraiser == bdValue1);
        //                    break;

        //                case "associate":
        //                    BreakDown1filteredappraisals = appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.SalesAssociate == bdValue1);
        //                    break;

        //                case "location":
        //                    BreakDown1filteredappraisals = appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Location == bdValue1);

        //                    foreach (var store in SalesCommission.Business.Enums.AppraisalLocations)
        //                    {
        //                        if (store.LocationId.ToLower().Trim() == bdValue1.ToLower().Trim())
        //                        {
        //                            Label1Value = store.Name;
        //                        }
        //                    }

        //                    break;

        //                case "vehiclesource":
        //                    BreakDown1filteredappraisals = appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.VehicleSource == bdValue1);
        //                    break;

        //                case "certified":
        //                    BreakDown1filteredappraisals = appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Certification == bdValue1);
        //                    foreach (var cert in SalesCommission.Business.Enums.CertificationLevels)
        //                    {
        //                        if (cert.CertificationID.ToLower().Trim() == bdValue1.ToLower().Trim())
        //                        {
        //                            Label1Value = cert.Name;
        //                        }
        //                    }
        //                    if (Label1Value == "")
        //                    {
        //                        Label1Value = "None";
        //                    }
        //                    break;

        //                case "soldloc":
        //                    BreakDown1filteredappraisals = appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.SoldLocation == bdValue1);
        //                    break;

        //                case "make":
        //                    BreakDown1filteredappraisals = appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Make.Contains(bdValue1));
        //                    break;

        //                case "stock":
        //                    BreakDown1filteredappraisals = appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.StockNumber == bdValue1);
        //                    break;

        //                case "model":
        //                    BreakDown1filteredappraisals = appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Model == bdValue1);
        //                    break;

        //                case "monthyear":
        //                    BreakDown1filteredappraisals = appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.LastModifiedDate.ToString("MM/yyyy") == bdValue1);
        //                    var dealDate = Convert.ToDateTime(bdValue1);
        //                    Label1Value = dealDate.Month + "/" + dealDate.Year;

        //                    break;

        //                case "zipcode":
        //                    BreakDown1filteredappraisals = appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.ZipCode == bdValue1);
        //                    break;

        //                case "modelyear":
        //                    BreakDown1filteredappraisals = appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Year == bdValue1);
        //                    break;

        //                case "showroom":
        //                    BreakDown1filteredappraisals = appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Showroom == bdValue1);
        //                    break;


        //                case "days":
        //                    var daysbreakdown = Convert.ToDecimal(bdValue1);

        //                    if (daysbreakdown == 120)
        //                    {
        //                        BreakDown1filteredappraisals = appraisalReportModel.AppraisalSoldDetails.FindAll(x => Convert.ToDecimal(x.DaysInStock) >= 90);
        //                    }
        //                    else
        //                    {
        //                        BreakDown1filteredappraisals = appraisalReportModel.AppraisalSoldDetails.FindAll(x => Convert.ToDecimal(x.DaysInStock) < daysbreakdown && Convert.ToDecimal(x.DaysInStock) >= (daysbreakdown - 30));
        //                    }
        //                    break;

        //                case "mileage":
        //                    var milesbreakdown = Convert.ToDecimal(bdValue1);

        //                    if (milesbreakdown == 500000)
        //                    {
        //                        BreakDown1filteredappraisals = appraisalReportModel.AppraisalSoldDetails.FindAll(x => Convert.ToDecimal(x.Miles) >= 100000);
        //                    }
        //                    else
        //                    {
        //                        BreakDown1filteredappraisals = appraisalReportModel.AppraisalSoldDetails.FindAll(x => Convert.ToDecimal(x.Miles) < milesbreakdown && Convert.ToDecimal(x.Miles) >= (milesbreakdown - 25000));
        //                    }
        //                    break;

        //                case "price":
        //                    var pricebreakdown = Convert.ToDecimal(bdValue1);

        //                    if (pricebreakdown == 200000)
        //                    {
        //                        BreakDown1filteredappraisals = appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.SellingPrice >= 100000);
        //                    }
        //                    else
        //                    {
        //                        BreakDown1filteredappraisals = appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.SellingPrice < pricebreakdown && x.SellingPrice >= (pricebreakdown - 10000));
        //                    }
        //                    break;

        //                default:
        //                    BreakDown1filteredappraisals = appraisalReportModel.AppraisalSoldDetails.FindAll(x => x.Location == bdValue1);
        //                    break;

        //            }

        //            appraisalReportDetailsModel.AppraisalSoldDetails = BreakDown1filteredappraisals;
        //        }
        //        #endregion Breadown 1

        //        #region Breakdown 2

        //        if (appraisalReportModel.BreakDownLevel2 != null)
        //        {
        //            Label2Value = bdValue2;
        //            switch (appraisalReportModel.BreakDownLevel2)
        //            {

        //                case "appraiser":
        //                    BreakDown2filteredappraisals = BreakDown1filteredappraisals.FindAll(x => x.Appraiser == bdValue2);
        //                    break;

        //                case "associate":
        //                    BreakDown2filteredappraisals = BreakDown1filteredappraisals.FindAll(x => x.SalesAssociate == bdValue2);
        //                    break;

        //                case "location":
        //                    BreakDown2filteredappraisals = BreakDown1filteredappraisals.FindAll(x => x.Location == bdValue2);

        //                    foreach (var store in SalesCommission.Business.Enums.AppraisalLocations)
        //                    {
        //                        if (store.LocationId.ToLower().Trim() == bdValue2.ToLower().Trim())
        //                        {
        //                            Label2Value = store.Name;
        //                        }
        //                    }

        //                    break;

        //                case "vehiclesource":
        //                    BreakDown2filteredappraisals = BreakDown1filteredappraisals.FindAll(x => x.VehicleSource == bdValue2);
        //                    break;

        //                case "certified":
        //                    BreakDown2filteredappraisals = BreakDown1filteredappraisals.FindAll(x => x.Certification == bdValue2);
        //                    foreach (var cert in SalesCommission.Business.Enums.CertificationLevels)
        //                    {
        //                        if (cert.CertificationID.ToLower().Trim() == bdValue2.ToLower().Trim())
        //                        {
        //                            Label2Value = cert.Name;
        //                        }
        //                    }
        //                    if (Label2Value == "")
        //                    {
        //                        Label2Value = "None";
        //                    }
        //                    break;

        //                case "soldloc":
        //                    BreakDown2filteredappraisals = BreakDown1filteredappraisals.FindAll(x => x.SoldLocation == bdValue2);
        //                    break;

        //                case "make":
        //                    BreakDown2filteredappraisals = BreakDown1filteredappraisals.FindAll(x => x.Make.Contains(bdValue2));
        //                    break;


        //                case "stock":
        //                    BreakDown2filteredappraisals = BreakDown1filteredappraisals.FindAll(x => x.StockNumber == bdValue2);
        //                    break;

        //                case "model":
        //                    BreakDown2filteredappraisals = BreakDown1filteredappraisals.FindAll(x => x.Model == bdValue2);
        //                    break;

        //                case "monthyear":
        //                    BreakDown2filteredappraisals = BreakDown1filteredappraisals.FindAll(x => x.LastModifiedDate.ToString("MM/yyyy") == bdValue2);
        //                    var dealDate = Convert.ToDateTime(bdValue2);
        //                    Label2Value = dealDate.Month + "/" + dealDate.Year;
        //                    break;

        //                case "zipcode":
        //                    BreakDown2filteredappraisals = BreakDown1filteredappraisals.FindAll(x => x.ZipCode == bdValue2);
        //                    break;

        //                case "modelyear":
        //                    BreakDown2filteredappraisals = BreakDown1filteredappraisals.FindAll(x => x.Year == bdValue2);
        //                    break;

        //                case "showroom":
        //                    BreakDown2filteredappraisals = BreakDown1filteredappraisals.FindAll(x => x.Showroom == bdValue2);
        //                    break;

        //                case "days":
        //                    var daysbreakdown = Convert.ToDecimal(bdValue2);

        //                    if (daysbreakdown == 120)
        //                    {
        //                        BreakDown2filteredappraisals = BreakDown1filteredappraisals.FindAll(x => Convert.ToDecimal(x.DaysInStock) >= 90);
        //                    }
        //                    else
        //                    {
        //                        BreakDown2filteredappraisals = BreakDown1filteredappraisals.FindAll(x => Convert.ToDecimal(x.DaysInStock) < daysbreakdown && Convert.ToDecimal(x.DaysInStock) >= (daysbreakdown - 30));
        //                    }
        //                    break;

        //                case "mileage":
        //                    var milesbreakdown = Convert.ToDecimal(bdValue2);

        //                    if (milesbreakdown == 500000)
        //                    {
        //                        BreakDown2filteredappraisals = BreakDown1filteredappraisals.FindAll(x => Convert.ToDecimal(x.Miles) >= 100000);
        //                    }
        //                    else
        //                    {
        //                        BreakDown2filteredappraisals = BreakDown1filteredappraisals.FindAll(x => Convert.ToDecimal(x.Miles) < milesbreakdown && Convert.ToDecimal(x.Miles) >= (milesbreakdown - 25000));
        //                    }
        //                    break;

        //                case "price":
        //                    var pricebreakdown = Convert.ToDecimal(bdValue2);

        //                    if (pricebreakdown == 200000)
        //                    {
        //                        BreakDown2filteredappraisals = BreakDown1filteredappraisals.FindAll(x => x.SellingPrice >= 100000);
        //                    }
        //                    else
        //                    {
        //                        BreakDown2filteredappraisals = BreakDown1filteredappraisals.FindAll(x => x.SellingPrice < pricebreakdown && x.SellingPrice >= (pricebreakdown - 10000));
        //                    }
        //                    break;

        //                default:
        //                    BreakDown2filteredappraisals = BreakDown1filteredappraisals.FindAll(x => x.Location == bdValue2);
        //                    break;


        //            }
        //            appraisalReportDetailsModel.AppraisalSoldDetails = BreakDown2filteredappraisals;
        //        }
        //        #endregion Breadown 2

        //        #region Breakdown 3

        //        if (appraisalReportModel.BreakDownLevel3 != null)
        //        {
        //            Label3Value = bdValue3;
        //            switch (appraisalReportModel.BreakDownLevel3)
        //            {

        //                case "appraiser":
        //                    BreakDown3filteredappraisals = BreakDown2filteredappraisals.FindAll(x => x.Appraiser == bdValue3);
        //                    break;

        //                case "associate":
        //                    BreakDown3filteredappraisals = BreakDown2filteredappraisals.FindAll(x => x.SalesAssociate == bdValue3);
        //                    break;

        //                case "location":
        //                    BreakDown3filteredappraisals = BreakDown2filteredappraisals.FindAll(x => x.Location == bdValue3);

        //                    foreach (var store in SalesCommission.Business.Enums.AppraisalLocations)
        //                    {
        //                        if (store.LocationId.ToLower().Trim() == bdValue3.ToLower().Trim())
        //                        {
        //                            Label3Value = store.Name;
        //                        }
        //                    }

        //                    break;


        //                case "vehiclesource":
        //                    BreakDown3filteredappraisals = BreakDown2filteredappraisals.FindAll(x => x.VehicleSource == bdValue3);
        //                    break;

        //                case "certified":
        //                    BreakDown3filteredappraisals = BreakDown2filteredappraisals.FindAll(x => x.Certification == bdValue3);
        //                    foreach (var cert in SalesCommission.Business.Enums.CertificationLevels)
        //                    {
        //                        if (cert.CertificationID.ToLower().Trim() == bdValue3.ToLower().Trim())
        //                        {
        //                            Label3Value = cert.Name;
        //                        }
        //                    }
        //                    if (Label3Value == "")
        //                    {
        //                        Label3Value = "None";
        //                    }
        //                    break;

        //                case "soldloc":
        //                    BreakDown3filteredappraisals = BreakDown2filteredappraisals.FindAll(x => x.SoldLocation == bdValue3);
        //                    break;

        //                case "make":
        //                    BreakDown3filteredappraisals = BreakDown2filteredappraisals.FindAll(x => x.Make.Contains(bdValue3));
        //                    break;


        //                case "stock":
        //                    BreakDown3filteredappraisals = BreakDown2filteredappraisals.FindAll(x => x.StockNumber == bdValue3);
        //                    break;

        //                case "model":
        //                    BreakDown3filteredappraisals = BreakDown2filteredappraisals.FindAll(x => x.Model == bdValue3);
        //                    break;

        //                case "monthyear":
        //                    BreakDown3filteredappraisals = BreakDown2filteredappraisals.FindAll(x => x.LastModifiedDate.ToString("MM/yyyy") == bdValue3);
        //                    var dealDate = Convert.ToDateTime(bdValue3);
        //                    Label3Value = dealDate.Month + "/" + dealDate.Year;
        //                    break;

        //                case "zipcode":
        //                    BreakDown3filteredappraisals = BreakDown2filteredappraisals.FindAll(x => x.ZipCode == bdValue3);
        //                    break;

        //                case "modelyear":
        //                    BreakDown3filteredappraisals = BreakDown2filteredappraisals.FindAll(x => x.Year == bdValue3);
        //                    break;


        //                case "showroom":
        //                    BreakDown3filteredappraisals = BreakDown2filteredappraisals.FindAll(x => x.Showroom == bdValue3);
        //                    break;

        //                case "days":
        //                    var daysbreakdown = Convert.ToDecimal(bdValue3);

        //                    if (daysbreakdown == 120)
        //                    {
        //                        BreakDown3filteredappraisals = BreakDown2filteredappraisals.FindAll(x => Convert.ToDecimal(x.DaysInStock) >= 90);
        //                    }
        //                    else
        //                    {
        //                        BreakDown3filteredappraisals = BreakDown2filteredappraisals.FindAll(x => Convert.ToDecimal(x.DaysInStock) < daysbreakdown && Convert.ToDecimal(x.DaysInStock) >= (daysbreakdown - 30));
        //                    }
        //                    break;

        //                case "mileage":
        //                    var milesbreakdown = Convert.ToDecimal(bdValue3);

        //                    if (milesbreakdown == 500000)
        //                    {
        //                        BreakDown3filteredappraisals = BreakDown2filteredappraisals.FindAll(x => Convert.ToDecimal(x.Miles) >= 100000);
        //                    }
        //                    else
        //                    {
        //                        BreakDown3filteredappraisals = BreakDown2filteredappraisals.FindAll(x => Convert.ToDecimal(x.Miles) < milesbreakdown && Convert.ToDecimal(x.Miles) >= (milesbreakdown - 25000));
        //                    }
        //                    break;

        //                case "price":
        //                    var pricebreakdown = Convert.ToDecimal(bdValue3);

        //                    if (pricebreakdown == 200000)
        //                    {
        //                        BreakDown3filteredappraisals = BreakDown2filteredappraisals.FindAll(x => x.SellingPrice >= 100000);
        //                    }
        //                    else
        //                    {
        //                        BreakDown3filteredappraisals = BreakDown2filteredappraisals.FindAll(x => x.SellingPrice < pricebreakdown && x.SellingPrice >= (pricebreakdown - 10000));
        //                    }
        //                    break;
        //                default:
        //                    BreakDown3filteredappraisals = BreakDown2filteredappraisals.FindAll(x => x.Location == bdValue3);
        //                    break;

        //            }
        //            appraisalReportDetailsModel.AppraisalSoldDetails = BreakDown3filteredappraisals;
        //        }
        //        #endregion Breadown 3

        //        #region Breakdown 4

        //        if (appraisalReportModel.BreakDownLevel4 != null)
        //        {
        //            Label4Value = bdValue4;
        //            switch (appraisalReportModel.BreakDownLevel4)
        //            {

        //                case "appraiser":
        //                    BreakDown4filteredappraisals = BreakDown3filteredappraisals.FindAll(x => x.Appraiser == bdValue4);
        //                    break;

        //                case "associate":
        //                    BreakDown4filteredappraisals = BreakDown3filteredappraisals.FindAll(x => x.SalesAssociate == bdValue4);
        //                    break;

        //                case "location":
        //                    BreakDown4filteredappraisals = BreakDown3filteredappraisals.FindAll(x => x.Location == bdValue4);

        //                    foreach (var store in SalesCommission.Business.Enums.Locations)
        //                    {
        //                        if (store.LocationId.ToLower().Trim() == bdValue4.ToLower().Trim())
        //                        {
        //                            Label4Value = store.Name;
        //                        }
        //                    }

        //                    break;

        //                case "vehiclesource":
        //                    BreakDown4filteredappraisals = BreakDown3filteredappraisals.FindAll(x => x.VehicleSource == bdValue4);
        //                    break;

        //                case "certified":
        //                    BreakDown4filteredappraisals = BreakDown3filteredappraisals.FindAll(x => x.Certification == bdValue4);
        //                    foreach (var cert in SalesCommission.Business.Enums.CertificationLevels)
        //                    {
        //                        if (cert.CertificationID.ToLower().Trim() == bdValue4.ToLower().Trim())
        //                        {
        //                            Label4Value = cert.Name;
        //                        }
        //                    }
        //                    if (Label4Value == "")
        //                    {
        //                        Label4Value = "None";
        //                    }
        //                    break;

        //                case "soldloc":
        //                    BreakDown4filteredappraisals = BreakDown3filteredappraisals.FindAll(x => x.SoldLocation == bdValue4);
        //                    break;

        //                case "make":
        //                    BreakDown4filteredappraisals = BreakDown3filteredappraisals.FindAll(x => x.Make.Contains(bdValue4));
        //                    break;

        //                case "stock":
        //                    BreakDown4filteredappraisals = BreakDown3filteredappraisals.FindAll(x => x.StockNumber == bdValue4);
        //                    break;

        //                case "model":
        //                    BreakDown4filteredappraisals = BreakDown3filteredappraisals.FindAll(x => x.Model == bdValue4);
        //                    break;

        //                case "monthyear":
        //                    BreakDown4filteredappraisals = BreakDown3filteredappraisals.FindAll(x => x.LastModifiedDate.ToString("MM/yyyy") == bdValue4);
        //                    var dealDate = Convert.ToDateTime(bdValue4);
        //                    Label4Value = dealDate.Month + "/" + dealDate.Year;
        //                    break;

        //                case "zipcode":
        //                    BreakDown4filteredappraisals = BreakDown3filteredappraisals.FindAll(x => x.ZipCode == bdValue4);
        //                    break;

        //                case "modelyear":
        //                    BreakDown4filteredappraisals = BreakDown3filteredappraisals.FindAll(x => x.Year == bdValue4);
        //                    break;

        //                case "showroom":
        //                    BreakDown4filteredappraisals = BreakDown3filteredappraisals.FindAll(x => x.Showroom == bdValue4);
        //                    break;

        //                case "days":
        //                    var daysbreakdown = Convert.ToDecimal(bdValue4);

        //                    if (daysbreakdown == 120)
        //                    {
        //                        BreakDown4filteredappraisals = BreakDown3filteredappraisals.FindAll(x => Convert.ToDecimal(x.DaysInStock) >= 90);
        //                    }
        //                    else
        //                    {
        //                        BreakDown4filteredappraisals = BreakDown3filteredappraisals.FindAll(x => Convert.ToDecimal(x.DaysInStock) < daysbreakdown && Convert.ToDecimal(x.DaysInStock) >= (daysbreakdown - 30));
        //                    }
        //                    break;

        //                case "mileage":
        //                    var milesbreakdown = Convert.ToDecimal(bdValue4);

        //                    if (milesbreakdown == 500000)
        //                    {
        //                        BreakDown4filteredappraisals = BreakDown3filteredappraisals.FindAll(x => Convert.ToDecimal(x.Miles) >= 100000);
        //                    }
        //                    else
        //                    {
        //                        BreakDown4filteredappraisals = BreakDown3filteredappraisals.FindAll(x => Convert.ToDecimal(x.Miles) < milesbreakdown && Convert.ToDecimal(x.Miles) >= (milesbreakdown - 25000));
        //                    }
        //                    break;

        //                case "price":
        //                    var pricebreakdown = Convert.ToDecimal(bdValue4);

        //                    if (pricebreakdown == 200000)
        //                    {
        //                        BreakDown4filteredappraisals = BreakDown3filteredappraisals.FindAll(x => x.SellingPrice >= 100000);
        //                    }
        //                    else
        //                    {
        //                        BreakDown4filteredappraisals = BreakDown3filteredappraisals.FindAll(x => x.SellingPrice < pricebreakdown && x.SellingPrice >= (pricebreakdown - 10000));
        //                    }
        //                    break;
        //                default:
        //                    BreakDown4filteredappraisals = BreakDown3filteredappraisals.FindAll(x => x.Location == bdValue4);
        //                    break;

        //            }
        //            appraisalReportDetailsModel.AppraisalSoldDetails = BreakDown4filteredappraisals;
        //        }
        //        #endregion Breadown 4

        //    }

        //    //NOW GO THROUGH BREAKDOWNS AND RETURN THE LAST BREAKDOWN

        //    var tradeDeals = new List<string>();
        //    foreach (var detail in appraisalReportDetailsModel.AppraisalSoldDetails)
        //    {
        //        if(detail.TradeCount > 0)
        //        {
        //            tradeDeals.Add(detail.DealNumber);
        //        }

        //    }

        //    return View(appraisalReportDetailsModel);
        //}

        public ActionResult AppraisalReportInvDetails(string bd1, string bd2, string bd3, string bd4, string vs, string at, string fm, string days)
        {
            var appraisalReportModel = new AppraisalReportModel();
            var appraisalReportDetailsModel = new AppraisalReportDetailModel();

            var bdValue1 = "";
            var bdValue2 = "";
            var bdValue3 = "";
            var bdValue4 = "";

            if (bd1 != null && bd1 != "")
            {
                appraisalReportModel.BreakDownLevel1 = bd1.Split(',')[0];
                //bdValue1 = bd1.Split(',')[1];
                bdValue1 = bd1.Replace(bd1.Split(',')[0] + ",", "");

                appraisalReportDetailsModel.BreakDownLevel1 = bd1.Split(',')[0];
                appraisalReportDetailsModel.BreakDownLevel1Value = bdValue1;
            }

            if (bd2 != null && bd2 != "")
            {
                appraisalReportModel.BreakDownLevel2 = bd2.Split(',')[0];
                //bdValue2 = bd2.Split(',')[1];
                bdValue2 = bd2.Replace(bd2.Split(',')[0] + ",", "");

                appraisalReportDetailsModel.BreakDownLevel2 = bd2.Split(',')[0];
                appraisalReportDetailsModel.BreakDownLevel2Value = bdValue2;
            }

            if (bd3 != null && bd3 != "")
            {
                appraisalReportModel.BreakDownLevel3 = bd3.Split(',')[0];
                //bdValue3 = bd3.Split(',')[1];
                bdValue3 = bd3.Replace(bd3.Split(',')[0] + ",", "");

                appraisalReportDetailsModel.BreakDownLevel3 = bd3.Split(',')[0];
                appraisalReportDetailsModel.BreakDownLevel3Value = bdValue3;
            }

            if (bd4 != null && bd4 != "")
            {
                appraisalReportModel.BreakDownLevel4 = bd4.Split(',')[0];
                //bdValue4 = bd4.Split(',')[1];
                bdValue4 = bd4.Replace(bd4.Split(',')[0] + ",", "");

                appraisalReportDetailsModel.BreakDownLevel4 = bd4.Split(',')[0];
                appraisalReportDetailsModel.BreakDownLevel4Value = bdValue4;
            }

            if (vs != null && vs != "")
            {
                appraisalReportModel.StatusType = vs;
                appraisalReportDetailsModel.StatusType = vs;
            }

            if (at != null && at != "")
            {
                appraisalReportModel.AcquisitionType = at;
                appraisalReportDetailsModel.AcquisitionType = at;
            }


            if (fm != null && fm != "")
            {
                appraisalReportModel.StatusOnFM = fm;
            }
            
            var startmonth = DateTime.Now.Month;
            var endmonth = DateTime.Now.Month;

            var startyear = DateTime.Now.Year;
            var endyear = DateTime.Now.Year;

            appraisalReportModel.ReportStartMonth = Convert.ToInt32(startmonth);
            appraisalReportDetailsModel.ReportStartMonth = Convert.ToInt32(startmonth);

            appraisalReportModel.ReportEndMonth = Convert.ToInt32(endmonth);
            appraisalReportDetailsModel.ReportEndMonth = Convert.ToInt32(endmonth);

            appraisalReportModel.ReportStartYear = Convert.ToInt32(startyear);
            appraisalReportDetailsModel.ReportStartYear = Convert.ToInt32(startyear);

            appraisalReportModel.ReportEndYear = Convert.ToInt32(endyear);
            appraisalReportDetailsModel.ReportEndYear = Convert.ToInt32(endyear);

            var reportDate = new DateTime(Convert.ToInt32(appraisalReportModel.ReportStartYear), Convert.ToInt32(appraisalReportModel.ReportStartMonth), 1);
            var reportEndDate = new DateTime(Convert.ToInt32(appraisalReportModel.ReportEndYear), Convert.ToInt32(appraisalReportModel.ReportEndMonth), 1);//.AddMonths(1);

            appraisalReportModel.TradeAcquisitionDetails = SqlQueries.GetTradeAcquisitionReportDetailsByDate(reportDate, reportEndDate);
            appraisalReportModel.TradeAcquisitionDetails = appraisalReportModel.TradeAcquisitionDetails.OrderByDescending(z => z.VIN).ThenByDescending(z => z.AppraisalDate).GroupBy(x => x.VIN).Select(g => g.First()).ToList();

            appraisalReportModel.TradeAcquisitionDetails = appraisalReportModel.TradeAcquisitionDetails.FindAll(x => appraisalReportModel.StatusType.Split(',').ToList().Contains(x.Status.ToString()));

            if (appraisalReportModel.StatusOnFM.ToUpper() != "ALL")
            {
                if (appraisalReportModel.StatusOnFM.ToUpper() == "ONFITZMALL")
                {
                    appraisalReportModel.TradeAcquisitionDetails = appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.ListAmount > 0);
                }
                else if (appraisalReportModel.StatusOnFM.ToUpper() == "MISSINGFITZMALL")
                {
                    appraisalReportModel.TradeAcquisitionDetails = appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.ListAmount < 1);

                }

            }

            if (days != null && days != "")
            {
                switch (days)
                {
                    case "30":
                        appraisalReportModel.TradeAcquisitionDetails = appraisalReportModel.TradeAcquisitionDetails.FindAll(x => Int32.Parse(x.Days) < 30);
                        break;
                    case "3060":
                        appraisalReportModel.TradeAcquisitionDetails = appraisalReportModel.TradeAcquisitionDetails.FindAll(x => Int32.Parse(x.Days) >= 30 && Int32.Parse(x.Days) < 60);
                        
                        break;
                    case "6090":
                        appraisalReportModel.TradeAcquisitionDetails = appraisalReportModel.TradeAcquisitionDetails.FindAll(x => Int32.Parse(x.Days) >= 60 && Int32.Parse(x.Days) < 90);
                        break;
                    case "90":
                        appraisalReportModel.TradeAcquisitionDetails = appraisalReportModel.TradeAcquisitionDetails.FindAll(x => Int32.Parse(x.Days) >90);                        
                        break;
                }
            }

            //if (appraisalReportModel.StatusType.ToUpper() != "ALL")
            //{
            //    if (appraisalReportModel.StatusType.ToUpper() == "RETAIL")
            //    {
            //        appraisalReportModel.TradeAcquisitionDetails = appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.Status == 1 || x.Status == 2);
            //    }
            //    else if (appraisalReportModel.StatusType.ToUpper() == "ONLINE")
            //    {
            //        appraisalReportModel.TradeAcquisitionDetails = appraisalReportModel.TradeAcquisitionDetails.FindAll(x => (x.Status == 1 || x.Status == 2) && x.ListAmount > 0);
            //    }
            //    else if (appraisalReportModel.StatusType.ToUpper() == "OTHER")
            //    {
            //        appraisalReportModel.TradeAcquisitionDetails = appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.Status != 1 && x.Status != 2);
            //    }
            //}

            var additionalDetails = SqlQueries.GetTradeAcquisitionAdditionalDetails();

            appraisalReportDetailsModel.vAutoInventoryDetails = SqlQueries.GetVAutoCurrentInventory();

            foreach (var appraisal in appraisalReportModel.TradeAcquisitionDetails)
            {
                var stockSource = "";
                var stockSourceName = "";
                var lastChar = "";


                if (appraisal.StockNumber != null && appraisal.StockNumber.Length > 1)
                {
                    stockSource = appraisal.StockNumber.Substring(1, 1);
                    lastChar = appraisal.StockNumber.Substring(appraisal.StockNumber.Length - 1);

                    var details = additionalDetails.Find(x => x.StockNumber.Trim() == appraisal.StockNumber);
                    if (details != null)
                    {
                        appraisal.StyleId = details.StyleId;
                        appraisal.Certification = details.Certification;
                        appraisal.BodyStyle = details.BodyStyle;
                        appraisal.XrefId = details.XrefId;
                        appraisal.LeadCount = details.LeadCount;
                        appraisal.ActiveLeadCount = details.ActiveLeadCount;
                        appraisal.LastActiveLeadDate = details.LastActiveLeadDate;
                    }
                    else
                    {
                        appraisal.StyleId = "None";
                        appraisal.Certification = "None";
                        appraisal.BodyStyle = "unknown";
                        appraisal.BodyStyle = "";
                    }

                }
                switch (stockSource)
                {
                    case "A":
                        stockSourceName = "Auction or Wholesale";
                        break;
                    case "B":
                        stockSourceName = "Lease Buyout";
                        break;
                    case "C":
                        stockSourceName = "Car Offer";
                        break;
                    case "D":
                        stockSourceName = "Demo Prev Rental";
                        break;
                    case "E":
                        stockSourceName = "Demo";
                        break;
                    case "F":
                        stockSourceName = "Nextcar Bad Hist";
                        break;
                    case "G":
                        stockSourceName = "Government Auction";
                        break;
                    case "K":
                        stockSourceName = "Kelly ICO";
                        break;
                    case "L":
                        stockSourceName = "In House Loaner";
                        break;
                    case "M":
                        stockSourceName = "Facebook Marketplace";
                        break;
                    case "N":
                        stockSourceName = "Nextcar Purchase";
                        break;
                    case "P":
                        stockSourceName = "Purchase/Lease Cust";
                        break;
                    case "R":
                        stockSourceName = "Rental";
                        break;
                    case "X":
                        stockSourceName = "Previous Repo";
                        break;
                    case "V":
                        stockSourceName = "VinCue";
                        break;
                    default:
                        stockSource = "";
                        stockSourceName = "";
                        break;
                }

                if (lastChar != "" && Char.IsLetter(lastChar, 0))
                {
                    switch (lastChar)
                    {
                        case "K":
                            stockSourceName = "Kelly ICO";
                            break;
                        case "M":
                            stockSourceName = "Facebook Marketplace";
                            break;
                        default:
                            stockSourceName = "Trade";
                            break;
                    }
                }

                appraisal.VehicleSource = stockSourceName;

                if (appraisal.LeadSourceName == null)
                {
                    appraisal.LeadSourceName = "";
                }

                if (appraisal.LeadGroup == null)
                {
                    appraisal.LeadGroup = "";
                }

                if(appraisal.Certification == null || appraisal.Certification == "")
                {
                    appraisal.Certification = "None";
                }

                if (appraisal.Appraiser == null)
                {
                    appraisal.Appraiser = "";
                }

                if(appraisal.Make == null)
                {
                    appraisal.Make = "";
                }

                if (appraisal.Carline != null && appraisal.Carline.IndexOf(" ") > 0)
                {
                    appraisal.Carline = appraisal.Carline.Substring(0, appraisal.Carline.IndexOf(" "));
                }
            }


            var Label1Value = "";
            var Label2Value = "";
            var Label3Value = "";
            var Label4Value = "";

            var BreakDown1filteredappraisals = new List<SalesCommission.Models.TradeAcquisitionDetail>();
            var BreakDown2filteredappraisals = new List<SalesCommission.Models.TradeAcquisitionDetail>();
            var BreakDown3filteredappraisals = new List<SalesCommission.Models.TradeAcquisitionDetail>();
            var BreakDown4filteredappraisals = new List<SalesCommission.Models.TradeAcquisitionDetail>();

            if (appraisalReportModel.TradeAcquisitionDetails != null && appraisalReportModel.TradeAcquisitionDetails.Count > 0)
            {

                
                #region Breakdown 1

                if (appraisalReportModel.BreakDownLevel1 != null)
                {
                    Label1Value = bdValue1;
                    switch (appraisalReportModel.BreakDownLevel1)
                    {

                        case "appraiser":
                            BreakDown1filteredappraisals = appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.Appraiser == bdValue1);
                            break;
                            
                        case "location":
                            BreakDown1filteredappraisals = appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.DRloc == bdValue1);

                            foreach (var store in SalesCommission.Business.Enums.AppraisalLocations)
                            {
                                if (store.LocationId.ToLower().Trim() == bdValue1.ToLower().Trim())
                                {
                                    Label1Value = store.Name;
                                }
                            }

                            break;

                        case "showroom":
                            BreakDown1filteredappraisals = appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.Loc == bdValue1);

                            foreach (var store in SalesCommission.Business.Enums.ShowroomLocations)
                            {
                                if (store.LocationId.ToLower().Trim() == bdValue1.ToLower().Trim())
                                {
                                    Label1Value = store.Name;
                                }
                            }

                            break;

                        case "vehiclesource":
                            BreakDown1filteredappraisals = appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.VehicleSource == bdValue1);
                            break;

                        case "lead":
                            BreakDown1filteredappraisals = appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.LeadSourceName == bdValue1);
                            break;

                        case "leadgroup":
                            BreakDown1filteredappraisals = appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.LeadGroup == bdValue1);
                            break;


                        case "stock":
                            BreakDown1filteredappraisals = appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.StockNumber == bdValue1);
                            break;

                        case "status":
                            BreakDown1filteredappraisals = appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.Status.ToString() == bdValue1);

                            foreach (var status in SalesCommission.Business.Enums.VehicleStatuses)
                            {
                                if (status.StatusId.ToLower().Trim() == bdValue1.ToLower().Trim())
                                {
                                    Label1Value = status.Name;
                                }
                            }
                            break;

                        case "make":
                            BreakDown1filteredappraisals = appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.Make.ToUpper().Contains(bdValue1.ToUpper()));
                            break;

                        case "model":
                            BreakDown1filteredappraisals = appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.Carline.ToUpper() == bdValue1.ToUpper());
                            break;

                        case "certified":
                            BreakDown1filteredappraisals = appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.Certification.Trim() == bdValue1);
                            foreach (var status in SalesCommission.Business.Enums.CPOCodes)
                            {
                                if (status.CertificationID.ToLower().Trim() == bdValue1.ToLower().Trim())
                                {
                                    Label1Value = status.Name;
                                }
                            }
                            break;

                        case "modelyear":
                            BreakDown1filteredappraisals = appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.Year == bdValue1);
                            break;


                        case "body":
                            BreakDown1filteredappraisals = appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.BodyStyle == bdValue1);
                            break;

                        case "days":
                            var daysbreakdown = Convert.ToDecimal(bdValue1);

                            if (daysbreakdown == 120)
                            {
                                BreakDown1filteredappraisals = appraisalReportModel.TradeAcquisitionDetails.FindAll(x => Convert.ToDecimal(x.Days) >= 90);
                            }
                            else
                            {
                                BreakDown1filteredappraisals = appraisalReportModel.TradeAcquisitionDetails.FindAll(x => Convert.ToDecimal(x.Days) < daysbreakdown && Convert.ToDecimal(x.Days) >= (daysbreakdown - 30));
                            }
                            break;

                        case "mileage":
                            var milesbreakdown = Convert.ToDecimal(bdValue1);

                            if (milesbreakdown == 500000)
                            {
                                BreakDown1filteredappraisals = appraisalReportModel.TradeAcquisitionDetails.FindAll(x => Convert.ToDecimal(x.Miles) >= 100000);
                            }
                            else
                            {
                                BreakDown1filteredappraisals = appraisalReportModel.TradeAcquisitionDetails.FindAll(x => Convert.ToDecimal(x.Miles) < milesbreakdown && Convert.ToDecimal(x.Miles) >= (milesbreakdown - 25000));
                            }
                            break;

                        case "price":
                            var pricebreakdown = Convert.ToDecimal(bdValue1);

                            if (pricebreakdown == 200000)
                            {
                                BreakDown1filteredappraisals = appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.ListAmount >= 100000);
                            }
                            else
                            {
                                BreakDown1filteredappraisals = appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.ListAmount < pricebreakdown && x.ListAmount >= (pricebreakdown - 10000));
                            }
                            break;


                        case "monthyear":
                            BreakDown1filteredappraisals = appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.LastModifiedDate.ToString("MM/yyyy") == bdValue1);
                            var dealDate = Convert.ToDateTime(bdValue1);
                            Label1Value = dealDate.Month + "/" + dealDate.Year;

                            break;

                        default:
                            BreakDown1filteredappraisals = appraisalReportModel.TradeAcquisitionDetails.FindAll(x => x.Loc == bdValue1);
                            break;

                    }

                    appraisalReportDetailsModel.TradeAcquisitionDetails = BreakDown1filteredappraisals;
                }
                #endregion Breadown 1

                #region Breakdown 2

            
                if (appraisalReportModel.BreakDownLevel2 != null)
                {
                    Label2Value = bdValue2;
                    switch (appraisalReportModel.BreakDownLevel2)
                    {

                        case "appraiser":
                            BreakDown2filteredappraisals = BreakDown1filteredappraisals.FindAll(x => x.Appraiser == bdValue2);
                            break;

                        case "location":
                            BreakDown2filteredappraisals = BreakDown1filteredappraisals.FindAll(x => x.DRloc == bdValue2);

                            foreach (var store in SalesCommission.Business.Enums.AppraisalLocations)
                            {
                                if (store.LocationId.ToLower().Trim() == bdValue2.ToLower().Trim())
                                {
                                    Label2Value = store.Name;
                                }
                            }

                            break;

                        case "showroom":
                            BreakDown2filteredappraisals = BreakDown1filteredappraisals.FindAll(x => x.Loc == bdValue2);

                            foreach (var store in SalesCommission.Business.Enums.ShowroomLocations)
                            {
                                if (store.LocationId.ToLower().Trim() == bdValue2.ToLower().Trim())
                                {
                                    Label2Value = store.Name;
                                }
                            }

                            break;

                        case "vehiclesource":
                            BreakDown2filteredappraisals = BreakDown1filteredappraisals.FindAll(x => x.VehicleSource == bdValue2);
                            break;

                        case "lead":
                            BreakDown2filteredappraisals = BreakDown1filteredappraisals.FindAll(x => x.LeadSourceName == bdValue2);
                            break;


                        case "stock":
                            BreakDown2filteredappraisals = BreakDown1filteredappraisals.FindAll(x => x.StockNumber == bdValue2);
                            break;

                        case "leadgroup":
                            BreakDown2filteredappraisals = BreakDown1filteredappraisals.FindAll(x => x.LeadGroup == bdValue2);
                            break;

                        case "status":
                            BreakDown2filteredappraisals = BreakDown1filteredappraisals.FindAll(x => x.Status.ToString() == bdValue2);

                            foreach (var status in SalesCommission.Business.Enums.VehicleStatuses)
                            {
                                if (status.StatusId.ToLower().Trim() == bdValue2.ToLower().Trim())
                                {
                                    Label2Value = status.Name;
                                }
                            }
                            break;

                        case "make":
                            BreakDown2filteredappraisals = BreakDown1filteredappraisals.FindAll(x => x.Make.ToUpper().Contains(bdValue2.ToUpper()));
                            break;

                        case "model":
                            BreakDown2filteredappraisals = BreakDown1filteredappraisals.FindAll(x => x.Carline.ToUpper() == bdValue2.ToUpper());
                            break;

                        case "certified":
                            BreakDown2filteredappraisals = BreakDown1filteredappraisals.FindAll(x => x.Certification.Trim() == bdValue2);
                            foreach (var status in SalesCommission.Business.Enums.CPOCodes)
                            {
                                if (status.CertificationID.ToLower().Trim() == bdValue2.ToLower().Trim())
                                {
                                    Label2Value = status.Name;
                                }
                            }
                            break;

                        case "modelyear":
                            BreakDown2filteredappraisals = BreakDown1filteredappraisals.FindAll(x => x.Year == bdValue2);
                            break;


                        case "body":
                            BreakDown2filteredappraisals = BreakDown1filteredappraisals.FindAll(x => x.BodyStyle == bdValue2);
                            break;

                        case "days":
                            var daysbreakdown = Convert.ToDecimal(bdValue2);

                            if (daysbreakdown == 120)
                            {
                                BreakDown2filteredappraisals = BreakDown1filteredappraisals.FindAll(x => Convert.ToDecimal(x.Days) >= 90);
                            }
                            else
                            {
                                BreakDown2filteredappraisals = BreakDown1filteredappraisals.FindAll(x => Convert.ToDecimal(x.Days) < daysbreakdown && Convert.ToDecimal(x.Days) >= (daysbreakdown - 30));
                            }
                            break;

                        case "mileage":
                            var milesbreakdown = Convert.ToDecimal(bdValue2);

                            if (milesbreakdown == 500000)
                            {
                                BreakDown2filteredappraisals = BreakDown1filteredappraisals.FindAll(x => Convert.ToDecimal(x.Miles) >= 100000);
                            }
                            else
                            {
                                BreakDown2filteredappraisals = BreakDown1filteredappraisals.FindAll(x => Convert.ToDecimal(x.Miles) < milesbreakdown && Convert.ToDecimal(x.Miles) >= (milesbreakdown - 25000));
                            }
                            break;

                        case "price":
                            var pricebreakdown = Convert.ToDecimal(bdValue2);

                            if (pricebreakdown == 200000)
                            {
                                BreakDown2filteredappraisals = BreakDown1filteredappraisals.FindAll(x => x.ListAmount >= 100000);
                            }
                            else
                            {
                                BreakDown2filteredappraisals = BreakDown1filteredappraisals.FindAll(x => x.ListAmount < pricebreakdown && x.ListAmount >= (pricebreakdown - 10000));
                            }
                            break;


                        case "monthyear":
                            BreakDown2filteredappraisals = BreakDown1filteredappraisals.FindAll(x => x.LastModifiedDate.ToString("MM/yyyy") == bdValue2);
                            var dealDate = Convert.ToDateTime(bdValue2);
                            Label2Value = dealDate.Month + "/" + dealDate.Year;
                            break;

                        default:
                            BreakDown2filteredappraisals = BreakDown1filteredappraisals.FindAll(x => x.Loc == bdValue2);
                            break;


                    }
                    appraisalReportDetailsModel.TradeAcquisitionDetails = BreakDown2filteredappraisals;
                }
                #endregion Breadown 2

                #region Breakdown 3

                if (appraisalReportModel.BreakDownLevel3 != null)
                {
                    Label3Value = bdValue3;
                    switch (appraisalReportModel.BreakDownLevel3)
                    {

                        case "appraiser":
                            BreakDown3filteredappraisals = BreakDown2filteredappraisals.FindAll(x => x.Appraiser == bdValue3);
                            break;
                            
                        case "location":
                            BreakDown3filteredappraisals = BreakDown2filteredappraisals.FindAll(x => x.DRloc == bdValue3);

                            foreach (var store in SalesCommission.Business.Enums.AppraisalLocations)
                            {
                                if (store.LocationId.ToLower().Trim() == bdValue3.ToLower().Trim())
                                {
                                    Label3Value = store.Name;
                                }
                            }

                            break;

                        case "showroom":
                            BreakDown3filteredappraisals = BreakDown2filteredappraisals.FindAll(x => x.Loc == bdValue3);

                            foreach (var store in SalesCommission.Business.Enums.ShowroomLocations)
                            {
                                if (store.LocationId.ToLower().Trim() == bdValue3.ToLower().Trim())
                                {
                                    Label3Value = store.Name;
                                }
                            }

                            break;

                        case "stock":
                            BreakDown3filteredappraisals = BreakDown2filteredappraisals.FindAll(x => x.StockNumber == bdValue3);
                            break;

                        case "vehiclesource":
                            BreakDown3filteredappraisals = BreakDown2filteredappraisals.FindAll(x => x.VehicleSource == bdValue3);
                            break;

                        case "lead":
                            BreakDown3filteredappraisals = BreakDown2filteredappraisals.FindAll(x => x.LeadSourceName == bdValue3);
                            break;

                        case "leadgroup":
                            BreakDown3filteredappraisals = BreakDown2filteredappraisals.FindAll(x => x.LeadGroup == bdValue3);
                            break;

                        case "status":
                            BreakDown3filteredappraisals = BreakDown2filteredappraisals.FindAll(x => x.Status.ToString() == bdValue3);

                            foreach (var status in SalesCommission.Business.Enums.VehicleStatuses)
                            {
                                if (status.StatusId.ToLower().Trim() == bdValue3.ToLower().Trim())
                                {
                                    Label3Value = status.Name;
                                }
                            }
                            break;


                        case "make":
                            BreakDown3filteredappraisals = BreakDown2filteredappraisals.FindAll(x => x.Make.ToUpper().Contains(bdValue3.ToUpper()));
                            break;

                        case "model":
                            BreakDown3filteredappraisals = BreakDown2filteredappraisals.FindAll(x => x.Carline.ToUpper() == bdValue3.ToUpper());
                            break;

                        case "certified":
                            BreakDown3filteredappraisals = BreakDown2filteredappraisals.FindAll(x => x.Certification.Trim() == bdValue3);
                           foreach (var status in SalesCommission.Business.Enums.CPOCodes)
                            {
                                if (status.CertificationID.ToLower().Trim() == bdValue3.ToLower().Trim())
                                {
                                    Label3Value = status.Name;
                                }
                            }
                            break;

                        case "modelyear":
                            BreakDown3filteredappraisals = BreakDown2filteredappraisals.FindAll(x => x.Year == bdValue3);
                            break;


                        case "body":
                            BreakDown3filteredappraisals = BreakDown2filteredappraisals.FindAll(x => x.BodyStyle == bdValue3);
                            break;

                        case "days":
                            var daysbreakdown = Convert.ToDecimal(bdValue3);

                            if (daysbreakdown == 120)
                            {
                                BreakDown3filteredappraisals = BreakDown2filteredappraisals.FindAll(x => Convert.ToDecimal(x.Days) >= 90);
                            }
                            else
                            {
                                BreakDown3filteredappraisals = BreakDown2filteredappraisals.FindAll(x => Convert.ToDecimal(x.Days) < daysbreakdown && Convert.ToDecimal(x.Days) >= (daysbreakdown - 30));
                            }
                            break;

                        case "mileage":
                            var milesbreakdown = Convert.ToDecimal(bdValue3);

                            if (milesbreakdown == 500000)
                            {
                                BreakDown3filteredappraisals = BreakDown2filteredappraisals.FindAll(x => Convert.ToDecimal(x.Miles) >= 100000);
                            }
                            else
                            {
                                BreakDown3filteredappraisals = BreakDown2filteredappraisals.FindAll(x => Convert.ToDecimal(x.Miles) < milesbreakdown && Convert.ToDecimal(x.Miles) >= (milesbreakdown - 25000));
                            }
                            break;

                        case "price":
                            var pricebreakdown = Convert.ToDecimal(bdValue3);

                            if (pricebreakdown == 200000)
                            {
                                BreakDown3filteredappraisals = BreakDown2filteredappraisals.FindAll(x => x.ListAmount >= 100000);
                            }
                            else
                            {
                                BreakDown3filteredappraisals = BreakDown2filteredappraisals.FindAll(x => x.ListAmount < pricebreakdown && x.ListAmount >= (pricebreakdown - 10000));
                            }
                            break;

                        case "monthyear":
                            BreakDown3filteredappraisals = BreakDown2filteredappraisals.FindAll(x => x.LastModifiedDate.ToString("MM/yyyy") == bdValue3);
                            var dealDate = Convert.ToDateTime(bdValue3);
                            Label3Value = dealDate.Month + "/" + dealDate.Year;
                            break;

                        default:
                            BreakDown3filteredappraisals = BreakDown2filteredappraisals.FindAll(x => x.Loc == bdValue3);
                            break;

                    }
                    appraisalReportDetailsModel.TradeAcquisitionDetails = BreakDown3filteredappraisals;
                }
                #endregion Breadown 3

                #region Breakdown 4

                if (appraisalReportModel.BreakDownLevel4 != null)
                {
                    Label4Value = bdValue4;
                    switch (appraisalReportModel.BreakDownLevel4)
                    {

                        case "appraiser":
                            BreakDown4filteredappraisals = BreakDown3filteredappraisals.FindAll(x => x.Appraiser == bdValue4);
                            break;

                        case "location":
                            BreakDown4filteredappraisals = BreakDown3filteredappraisals.FindAll(x => x.DRloc == bdValue4);

                            foreach (var store in SalesCommission.Business.Enums.AppraisalLocations)
                            {
                                if (store.LocationId.ToLower().Trim() == bdValue4.ToLower().Trim())
                                {
                                    Label4Value = store.Name;
                                }
                            }

                            break;

                        case "showroom":
                            BreakDown4filteredappraisals = BreakDown3filteredappraisals.FindAll(x => x.Loc == bdValue4);

                            foreach (var store in SalesCommission.Business.Enums.ShowroomLocations)
                            {
                                if (store.LocationId.ToLower().Trim() == bdValue4.ToLower().Trim())
                                {
                                    Label4Value = store.Name;
                                }
                            }

                            break;


                        case "vehiclesource":
                            BreakDown4filteredappraisals = BreakDown3filteredappraisals.FindAll(x => x.VehicleSource == bdValue4);
                            break;


                        case "lead":
                            BreakDown4filteredappraisals = BreakDown3filteredappraisals.FindAll(x => x.LeadSourceName == bdValue4);
                            break;

                        case "leadgroup":
                            BreakDown4filteredappraisals = BreakDown3filteredappraisals.FindAll(x => x.LeadGroup == bdValue4);
                            break;


                        case "stock":
                            BreakDown4filteredappraisals = BreakDown3filteredappraisals.FindAll(x => x.StockNumber == bdValue4);
                            break;

                        case "status":
                            BreakDown4filteredappraisals = BreakDown3filteredappraisals.FindAll(x => x.Status.ToString() == bdValue4);

                            foreach (var status in SalesCommission.Business.Enums.VehicleStatuses)
                            {
                                if (status.StatusId.ToLower().Trim() == bdValue4.ToLower().Trim())
                                {
                                    Label4Value = status.Name;
                                }
                            }
                            break;


                        case "make":
                            BreakDown4filteredappraisals = BreakDown3filteredappraisals.FindAll(x => x.Make.ToUpper().Contains(bdValue4.ToUpper()));
                            break;

                        case "model":
                            BreakDown4filteredappraisals = BreakDown3filteredappraisals.FindAll(x => x.Carline.ToUpper() == bdValue4.ToUpper());
                            break;


                        case "certified":
                            BreakDown4filteredappraisals = BreakDown3filteredappraisals.FindAll(x => x.Certification.Trim() == bdValue4);
                            foreach (var status in SalesCommission.Business.Enums.CPOCodes)
                            {
                                if (status.CertificationID.ToLower().Trim() == bdValue4.ToLower().Trim())
                                {
                                    Label4Value = status.Name;
                                }
                            }
                            break;

                        case "modelyear":
                            BreakDown4filteredappraisals = BreakDown3filteredappraisals.FindAll(x => x.Year == bdValue4);
                            break;

                        case "body":
                            BreakDown4filteredappraisals = BreakDown3filteredappraisals.FindAll(x => x.BodyStyle == bdValue4);
                            break;

                        case "days":
                            var daysbreakdown = Convert.ToDecimal(bdValue4);

                            if (daysbreakdown == 120)
                            {
                                BreakDown4filteredappraisals = BreakDown3filteredappraisals.FindAll(x => Convert.ToDecimal(x.Days) >= 90);
                            }
                            else
                            {
                                BreakDown4filteredappraisals = BreakDown3filteredappraisals.FindAll(x => Convert.ToDecimal(x.Days) < daysbreakdown && Convert.ToDecimal(x.Days) >= (daysbreakdown - 30));
                            }
                            break;

                        case "mileage":
                            var milesbreakdown = Convert.ToDecimal(bdValue4);

                            if (milesbreakdown == 500000)
                            {
                                BreakDown4filteredappraisals = BreakDown3filteredappraisals.FindAll(x => Convert.ToDecimal(x.Miles) >= 100000);
                            }
                            else
                            {
                                BreakDown4filteredappraisals = BreakDown3filteredappraisals.FindAll(x => Convert.ToDecimal(x.Miles) < milesbreakdown && Convert.ToDecimal(x.Miles) >= (milesbreakdown - 25000));
                            }
                            break;

                        case "price":
                            var pricebreakdown = Convert.ToDecimal(bdValue4);

                            if (pricebreakdown == 200000)
                            {
                                BreakDown4filteredappraisals = BreakDown3filteredappraisals.FindAll(x => x.ListAmount >= 100000);
                            }
                            else
                            {
                                BreakDown4filteredappraisals = BreakDown3filteredappraisals.FindAll(x => x.ListAmount < pricebreakdown && x.ListAmount >= (pricebreakdown - 10000));
                            }
                            break;

                        case "monthyear":
                            BreakDown4filteredappraisals = BreakDown3filteredappraisals.FindAll(x => x.LastModifiedDate.ToString("MM/yyyy") == bdValue4);
                            var dealDate = Convert.ToDateTime(bdValue4);
                            Label4Value = dealDate.Month + "/" + dealDate.Year;
                            break;

                        default:
                            BreakDown4filteredappraisals = BreakDown3filteredappraisals.FindAll(x => x.Loc == bdValue4);
                            break;

                    }
                    appraisalReportDetailsModel.TradeAcquisitionDetails = BreakDown4filteredappraisals;
                }
                #endregion Breadown 4

            }

            //NOW GO THROUGH BREAKDOWNS AND RETURN THE LAST BREAKDOWN


            return View(appraisalReportDetailsModel);
        }


        public ActionResult AcquisitionReport()
        {
            var acquisitionReport = new TradeAcquisitionReportModel();

            acquisitionReport.TradeAcquisitionDetails = SqlQueries.GetTradeAcquisitionReportDetails();

            return View(acquisitionReport);

        }

        public ActionResult AppraiserReport()
        {
            var appraiserReport = new AppraiserReportModel();

            var startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            appraiserReport.YearId = startDate.Year;
            appraiserReport.MonthId = startDate.Month;

            appraiserReport.AppraiserDetails = SqlQueries.GetAppraiserReport(startDate, endDate);

            return View(appraiserReport);
        }

        [HttpPost]
        public ActionResult AppraiserReport(AppraiserReportModel appraiserReport)
        {
            
            var startDate = new DateTime(appraiserReport.YearId, appraiserReport.MonthId, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            appraiserReport.AppraiserDetails = SqlQueries.GetAppraiserReport(startDate, endDate);

            return View(appraiserReport);
        }

        public ActionResult SalesReport()
        {

            var salesReportModel = new SalesReportModel();
            
            salesReportModel.ReportEndMonth = DateTime.Now.Month;
            salesReportModel.ReportEndYear = DateTime.Now.Year;

            salesReportModel.ReportStartMonth = DateTime.Now.AddMonths(-1).Month;
            salesReportModel.ReportStartYear = DateTime.Now.AddMonths(-1).Year;

            salesReportModel.ReportComparisonEndMonth = DateTime.Now.AddMonths(-1).Month;
            salesReportModel.ReportComparisonEndYear = DateTime.Now.AddMonths(-1).Year;

            salesReportModel.ReportComparisonStartMonth = DateTime.Now.AddMonths(-2).Month;
            salesReportModel.ReportComparisonStartYear = DateTime.Now.AddMonths(-2).Year;


            salesReportModel.IncludeHandyman = true;
            salesReportModel.VehicleType = "All";
            salesReportModel.ExcludeBadDuplicates = true;
            salesReportModel.ShowExcludedGroups = true;
            salesReportModel.ExcludeAllBad = false;

            return View(salesReportModel);
        }

        [HttpPost]
        public ActionResult SalesReport(SalesReportModel salesReportModel)
        {

            var vehicleType = "All";
            if (Request.Form["chkVehicleType"] != null)
            {
                vehicleType = Request.Form["chkVehicleType"];
            }
            salesReportModel.VehicleType = vehicleType;

            if (Request.Form["breakdown1"] != null)
            {
                salesReportModel.BreakDownLevel1 = Request.Form["breakdown1"];
            }

            if (Request.Form["breakdown2"] != null)
            {
                salesReportModel.BreakDownLevel2 = Request.Form["breakdown2"];
            }

            if (Request.Form["breakdown3"] != null)
            {
                salesReportModel.BreakDownLevel3 = Request.Form["breakdown3"];
            }

            if (Request.Form["breakdown4"] != null)
            {
                salesReportModel.BreakDownLevel4 = Request.Form["breakdown4"];
            }

            //salesReportModel.ReportEndMonth = DateTime.Now.Month;
            //salesReportModel.ReportEndYear = DateTime.Now.Year;

            //salesReportModel.ReportStartMonth = DateTime.Now.AddMonths(-1).Month;
            //salesReportModel.ReportStartYear = DateTime.Now.AddMonths(-1).Year;

            //salesReportModel.ReportComparisonEndMonth = DateTime.Now.AddMonths(-1).Month;
            //salesReportModel.ReportComparisonEndYear = DateTime.Now.AddMonths(-1).Year;

            //salesReportModel.ReportComparisonStartMonth = DateTime.Now.AddMonths(-2).Month;
            //salesReportModel.ReportComparisonStartYear = DateTime.Now.AddMonths(-2).Year;


            //salesReportModel.IncludeHandyman = true;
            //salesReportModel.VehicleType = "All";
            //salesReportModel.ExcludeBadDuplicates = true;
            //salesReportModel.ShowExcludedGroups = true;
            //salesReportModel.ExcludeAllBad = false;

            salesReportModel = SqlQueries.GetSalesReportByDateRange(salesReportModel);

            if(salesReportModel.VehicleType != null && salesReportModel.VehicleType.ToUpper() != "ALL" )
            {

                var filteredReports = new List<SalesReportDetail>();

               if(salesReportModel.VehicleType.ToUpper() == "NEW")
                {
                    filteredReports = salesReportModel.SalesReportDetails.FindAll(x => x.BrandId != "UU");
                }
               else
                {
                    filteredReports = salesReportModel.SalesReportDetails.FindAll(x => x.BrandId == "UU");

                    if(salesReportModel.VehicleType.ToUpper() == "HANDYMAN")
                    {

                        foreach (var report in filteredReports)
                        {
                            if (report.Deals != null)
                            {
                                report.Deals.RemoveAll(x => x.CertificationLevel != "HDM");
                            }
                        }
                    }

                }

                salesReportModel.SalesReportDetails = filteredReports;


            }
            salesReportModel.FactoryToDealerCash = SqlQueries.GetFTDByDateRange(salesReportModel.ReportStartYear, salesReportModel.ReportStartMonth, salesReportModel.ReportEndYear, salesReportModel.ReportEndMonth);
            return View(salesReportModel);
        }

        

        public ActionResult LeadReport()
        {
            //SetUserInformation();

            var leadReportModel = new LeadReportModel();

            var currentFiscalMonth = SqlQueries.GetFiscalMonthByMonthYear(DateTime.Now.Year, DateTime.Now.Month);
            var previousFiscalMonth = SqlQueries.GetFiscalMonthByMonthYear(DateTime.Now.AddMonths(-1).Year, DateTime.Now.AddMonths(-1).Month);

            if (currentFiscalMonth != null)
            {
                leadReportModel.ReportStartDate = currentFiscalMonth[0].StartDate;
                leadReportModel.ReportEndDate = currentFiscalMonth[0].EndDate;
            }
            else
            {
                leadReportModel.ReportStartDate = DateTime.Now.AddMonths(-1);
                leadReportModel.ReportEndDate = DateTime.Now;
            }

            if (previousFiscalMonth != null)
            {
                leadReportModel.ComparisonReportStartDate = previousFiscalMonth[0].StartDate;
                leadReportModel.ComparisonReportEndDate = previousFiscalMonth[0].EndDate;
            }
            else
            {
                leadReportModel.ComparisonReportStartDate = DateTime.Now.AddMonths(-2);
                leadReportModel.ComparisonReportEndDate = DateTime.Now.AddMonths(-1);
            }

            leadReportModel.IncludeHandyman = true;
            leadReportModel.VehicleType = "All";
            leadReportModel.ExcludeBadDuplicates = true;
            leadReportModel.ShowExcludedGroups = true;
            leadReportModel.ExcludeAllBad = false;

            //leadReportModel = SqlQueries.GetLeadReportByDateAndStore(leadReportModel);


            return View(leadReportModel);
        }

        public ActionResult LeadMapping()
        {
            var leadMappingModel = new LeadMappingModel();

            leadMappingModel.VINLeadSourceMappings = SqlQueries.GetAllVINLeadSources();
            leadMappingModel.LeadSourceMappings = SqlQueries.GetLeadMappings();

            foreach (var leadSource in leadMappingModel.VINLeadSourceMappings)
            {
                var success = leadMappingModel.LeadSourceMappings.Find(x => x.LeadSourceName == leadSource.LeadSourceName && x.LeadSourceGroupName == leadSource.LeadSourceGroupName);

                if(success == null)
                {
                    // NEW LEAD SOURCE NAMES, ADD THEM TO THE MAPPING TABLE...
                    var newMapping = new LeadMapping();

                    newMapping.LeadGroup = "Unassigned";
                    newMapping.LeadGroupId = 16;

                    newMapping.LeadSourceName = leadSource.LeadSourceName;
                    newMapping.LeadSourceGroupName = leadSource.LeadSourceGroupName;
                    newMapping.UpdateDate = DateTime.Now;
                    newMapping.UpdateUser = "New Source Import";

                    var bAdded = SqlQueries.AddLeadSourceMapping(newMapping);

                }

            }

            //GET THEM AGAIN TO GET THE LATEST ONES THAT HAVE BEEN ADDED
            leadMappingModel.LeadSourceMappings = SqlQueries.GetLeadMappings();
            leadMappingModel.LeadGroups = SqlQueries.GetLeadGroups();
            

            return View(leadMappingModel);
        }

        [HttpPost]
        public ActionResult LeadMapping(LeadMappingModel leadMappingModel)
        {

            if(Request.Form["AddGroup"] != null)
            {
                var newGroup = new LeadGroup();

                if (Request.Form["newGroupName"] != null && Request.Form["newGroupName"] != "")
                {
                    newGroup.LeadGroupName = Request.Form["newGroupName"];
                    newGroup.UpdateDate = DateTime.Now;
                    newGroup.UpdateUser = Session["Username"].ToString();

                    var bAdded = SqlQueries.AddLeadGroup(newGroup);
                }
            }
            else if(Request.Form["LoadGroup"] != null)
            {
                if (Request.Form["leadGroups"] != null && Request.Form["leadGroups"] != "")
                {
                    var leadGroup = Request.Form["leadGroups"];
                    leadMappingModel.SelectedLeadGroupId = Int32.Parse(leadGroup);
                    


                }
            }
            else if (Request.Form["SaveGroup"] != null)
            {

                var leadGroups = SqlQueries.GetLeadGroups();

                //First Update the Group Information...
                var saveLeadGroup = leadGroups.Find(x => x.Id == leadMappingModel.SelectedLeadGroupId);

                saveLeadGroup.UpdateDate = DateTime.Now;
                saveLeadGroup.UpdateUser = Session["Username"].ToString();

                var excludeFromReporting = false;
                if (Request.Form["chkExcludeFromReporting"] != null && Request.Form["chkExcludeFromReporting"] == "on")
                {
                    excludeFromReporting = true;
                }
                saveLeadGroup.ExcludeFromReporting = excludeFromReporting;

                var bGroupSaved = SqlQueries.SaveLeadGroup(saveLeadGroup);


                var totalIndex = 0;

                if (Request.Form["hdnIndex"] != null && Request.Form["hdnIndex"] != "")
                {
                    var hiddenIndex = Request.Form["hdnIndex"];
                    totalIndex  = Int32.Parse(hiddenIndex);
                }

                if(totalIndex > 0)
                {
                    //var leadGroups = SqlQueries.GetLeadGroups();

                    for (var i = 0; i < totalIndex; i++)
                    {

                        var oldGroupId = 0;
                        var newGroupId = 0;
                        var mappingId = 0;

                        if (Request.Form["hdnGroupId-" + i] != null)
                        {
                            oldGroupId = Int32.Parse(Request.Form["hdnGroupId-" + i]);
                        }

                        if (Request.Form["hdnMappingId-" + i] != null)
                        {
                            mappingId = Int32.Parse(Request.Form["hdnMappingId-" + i]);
                        }

                        if (Request.Form["leadGroupMapping-" + i] != null)
                        {
                            newGroupId = Int32.Parse(Request.Form["leadGroupMapping-" + i]);
                        }

                        if(newGroupId != oldGroupId)
                        {
                            var newLeadGroupName = leadGroups.Find(o => o.Id == newGroupId).LeadGroupName;

                            var newMapping = new LeadMapping();

                            newMapping.Id = mappingId;
                            newMapping.LeadGroup = newLeadGroupName;
                            newMapping.LeadGroupId = newGroupId;
                            
                            newMapping.UpdateDate = DateTime.Now;
                            newMapping.UpdateUser = Session["Username"].ToString();

                            var bSaved = SqlQueries.SaveLeadSourceMapping(newMapping);

                        }


                    }

                }



            }

            leadMappingModel.VINLeadSourceMappings = SqlQueries.GetAllVINLeadSources();
            leadMappingModel.LeadSourceMappings = SqlQueries.GetLeadMappings();
            leadMappingModel.LeadGroups = SqlQueries.GetLeadGroups();

            if (leadMappingModel.SelectedLeadGroupId > 0)
            {
                var selectedLeadGroup = leadMappingModel.LeadGroups.Find(x => x.Id == leadMappingModel.SelectedLeadGroupId);
                if (selectedLeadGroup != null)
                {
                    leadMappingModel.ExcludeFromReporting = selectedLeadGroup.ExcludeFromReporting;
                    leadMappingModel.SelectedLeadGroupName = selectedLeadGroup.LeadGroupName;
                }
            }

            return View(leadMappingModel);
        }

        public ActionResult LeadReportDetails(DateTime startdate, DateTime enddate, string bd1, string bd2, string bd3, string bd4, string vt, string lt, string ft)
        {
            var leadReportModel = new LeadReportModel();
            var leadReportDetailsModel = new LeadReportDetailsModel();

            var bdValue1 = "";
            var bdValue2 = "";
            var bdValue3 = "";
            var bdValue4 = "";

            if (bd1 != null && bd1 != "")
            {
                leadReportModel.BreakDownLevel1 = bd1.Split(',')[0];                
                bdValue1 = bd1.Split(',')[1];

                leadReportDetailsModel.BreakDownLevel1 = bd1.Split(',')[0];
                leadReportDetailsModel.BreakDownLevel1Value = bdValue1;
            }

            if (bd2 != null && bd2 != "")
            {
                leadReportModel.BreakDownLevel2 = bd2.Split(',')[0];
                bdValue2 = bd2.Split(',')[1];

                leadReportDetailsModel.BreakDownLevel2 = bd2.Split(',')[0];
                leadReportDetailsModel.BreakDownLevel2Value = bdValue2;
            }

            if (bd3 != null && bd3 != "")
            {
                leadReportModel.BreakDownLevel3 = bd3.Split(',')[0];
                bdValue3 = bd3.Split(',')[1];

                leadReportDetailsModel.BreakDownLevel3 = bd3.Split(',')[0];
                leadReportDetailsModel.BreakDownLevel3Value = bdValue3;
            }

            if (bd4 != null && bd4 != "")
            {
                leadReportModel.BreakDownLevel4 = bd4.Split(',')[0];
                bdValue4 = bd4.Split(',')[1];

                leadReportDetailsModel.BreakDownLevel4 = bd4.Split(',')[0];
                leadReportDetailsModel.BreakDownLevel4Value = bdValue4;
            }

            if (vt != null && vt != "")
            {
                leadReportModel.VehicleType = vt;
                leadReportDetailsModel.VehicleType = vt;
            }

            leadReportModel.IncludeHandyman = true;

            if (ft != null && ft != "")
            {
                if(ft.Split(',')[0] == "t")
                {
                    leadReportModel.ExcludeBadDuplicates = true;
                    leadReportDetailsModel.ExcludeBadDuplicates = true;
                }

                if (ft.Split(',')[1] == "t")
                {
                    leadReportModel.ShowExcludedGroups = true;
                    leadReportDetailsModel.ShowExcludedGroups = true;
                }

                if (ft.Split(',')[2] == "t")
                {
                    leadReportModel.ExcludeAllBad = true;
                    leadReportDetailsModel.ExcludeAllBad = true;
                }

                if (ft.Split(',')[3] == "t")
                {
                    leadReportModel.IncludeHandyman = false;
                    leadReportDetailsModel.IncludeHandyman = false;
                }
            }

        
            leadReportModel.ReportStartDate = startdate;
            leadReportDetailsModel.ReportStartDate = startdate;

            leadReportModel.ReportEndDate = enddate;
            leadReportDetailsModel.ReportEndDate = enddate;

            leadReportModel = SqlQueries.GetLeadReportNewByDateAndStore(leadReportModel, false);


            var Label1Value = "";
            var Label2Value = "";
            var Label3Value = "";
            var Label4Value = "";

            var BreakDown1filteredLeads = new List<SalesCommission.Models.AssociateLead>();
            var BreakDown2filteredLeads = new List<SalesCommission.Models.AssociateLead>();
            var BreakDown3filteredLeads = new List<SalesCommission.Models.AssociateLead>();
            var BreakDown4filteredLeads = new List<SalesCommission.Models.AssociateLead>();

            if (leadReportModel.AssociateLeads != null && leadReportModel.AssociateLeads.Count > 0)
            {

                #region Breakdown 1
                
                if (leadReportModel.BreakDownLevel1 != null)
                {
                    Label1Value = bdValue1;
                    switch (leadReportModel.BreakDownLevel1)
                    {
                        case "associatename":
                            BreakDown1filteredLeads = leadReportModel.AssociateLeads.FindAll(x => x.Sales_LastName + " " + x.Sales_FirstName == bdValue1);
                            break;

                        case "brand":
                            BreakDown1filteredLeads = leadReportModel.AssociateLeads.FindAll(x => x.VOfInterest_Make.Trim().ToUpper() == bdValue1);
                            break;

                        case "leadgroupname":
                            BreakDown1filteredLeads = leadReportModel.AssociateLeads.FindAll(x => x.LeadGroupMapping == bdValue1);
                            break;

                        case "leadsourcename":
                            BreakDown1filteredLeads = leadReportModel.AssociateLeads.FindAll(x => x.LeadSourceName == bdValue1);
                            break;

                        case "leadstatusname":
                            BreakDown1filteredLeads = leadReportModel.AssociateLeads.FindAll(x => x.LeadStatusName == bdValue1);
                            break;

                        case "leadstatustype":
                            BreakDown1filteredLeads = leadReportModel.AssociateLeads.FindAll(x => x.LeadStatusTypeName == bdValue1);
                            break;

                        case "locationid":
                            BreakDown1filteredLeads = leadReportModel.AssociateLeads.FindAll(x => x.DealerId.ToString() == bdValue1);

                            foreach (var store in SalesCommission.Business.Enums.VinStores)
                            {
                                if (store.StoreId == bdValue1)
                                {
                                    Label1Value = store.Name;
                                }
                            }

                            break;

                        case "salesteam":
                            BreakDown1filteredLeads = leadReportModel.AssociateLeads.FindAll(x => x.Team == bdValue1);
                            break;

                        case "make":
                            BreakDown1filteredLeads = leadReportModel.AssociateLeads.FindAll(x => x.VOfInterest_Make.Trim().ToUpper() == bdValue1);
                            break;

                        case "model":
                            BreakDown1filteredLeads = leadReportModel.AssociateLeads.FindAll(x => x.VOfInterest_Model == bdValue1);
                            break;

                        case "stock":
                            BreakDown1filteredLeads = leadReportModel.AssociateLeads.FindAll(x => x.VOfInterest_StockNumber == bdValue1);
                            break;

                        case "zip":
                            BreakDown1filteredLeads = leadReportModel.AssociateLeads.FindAll(x => x.PostalCode == bdValue1);
                            break;

                        case "inventorytype":
                            BreakDown1filteredLeads = leadReportModel.AssociateLeads.FindAll(x => x.VOfInterest_InventoryType == bdValue1);
                            break;

                        case "leaddate":
                            BreakDown1filteredLeads = leadReportModel.AssociateLeads.FindAll(x => x.LeadCreatedEastTime.ToShortDateString() == bdValue1);
                            break;

                        case "leadtime":
                            BreakDown1filteredLeads = leadReportModel.AssociateLeads.FindAll(x => x.LeadCreatedEastTime.Hour.ToString() == bdValue1);

                            TimeSpan result = TimeSpan.FromHours(Int32.Parse(bdValue1));
                            string fromTimeString = result.ToString("hh':'mm");

                            Label1Value = fromTimeString;

                            break;
                            
                        case "leadmonthyear":
                            BreakDown1filteredLeads = leadReportModel.AssociateLeads.FindAll(x => x.LeadCreatedEastTime.ToString("MM/yyyy") == bdValue1);
                            BreakDown1filteredLeads = leadReportModel.AssociateLeads.FindAll(x => ((x.LeadStatusTypeName.ToUpper() != "SOLD" && x.LeadCreatedEastTime.ToString("MM/yyyy") == bdValue1) || (x.LeadStatusTypeName.ToUpper() == "SOLD" && x.VehicleSoldEastTime.ToString("MM/yyyy") == bdValue1)));

                            break;

                        default:
                            BreakDown1filteredLeads = leadReportModel.AssociateLeads.FindAll(x => x.DealerId.ToString() == bdValue1);
                            break;
                    }

                    leadReportDetailsModel.AssociateLeads = BreakDown1filteredLeads;
                }
                #endregion Breadown 1

                #region Breakdown 2

                if (leadReportModel.BreakDownLevel2 != null)
                {
                    Label2Value = bdValue2;
                    switch (leadReportModel.BreakDownLevel2)
                    {
                        case "associatename":
                            BreakDown2filteredLeads = BreakDown1filteredLeads.FindAll(x => x.Sales_LastName + " " + x.Sales_FirstName == bdValue2);
                            break;

                        case "brand":
                            BreakDown2filteredLeads = BreakDown1filteredLeads.FindAll(x => x.VOfInterest_Make.Trim().ToUpper() == bdValue2);
                            break;

                        case "leadgroupname":
                            BreakDown2filteredLeads = BreakDown1filteredLeads.FindAll(x => x.LeadGroupMapping == bdValue2);
                            break;

                        case "leadsourcename":
                            BreakDown2filteredLeads = BreakDown1filteredLeads.FindAll(x => x.LeadSourceName == bdValue2);
                            break;

                        case "leadstatusname":
                            BreakDown2filteredLeads = BreakDown1filteredLeads.FindAll(x => x.LeadStatusName == bdValue2);
                            break;

                        case "leadstatustype":
                            BreakDown2filteredLeads = BreakDown1filteredLeads.FindAll(x => x.LeadStatusTypeName == bdValue2);
                            break;

                        case "locationid":
                            BreakDown2filteredLeads = BreakDown1filteredLeads.FindAll(x => x.DealerId.ToString() == bdValue2);

                            foreach (var store in SalesCommission.Business.Enums.VinStores)
                            {
                                if (store.StoreId == bdValue2)
                                {
                                    Label2Value = store.Name;
                                }
                            }

                            break;

                        case "salesteam":
                            BreakDown2filteredLeads = BreakDown1filteredLeads.FindAll(x => x.Team == bdValue2);
                            break;

                        case "make":
                            BreakDown2filteredLeads = BreakDown1filteredLeads.FindAll(x => x.VOfInterest_Make.Trim().ToUpper() == bdValue2);
                            break;

                        case "model":
                            BreakDown2filteredLeads = BreakDown1filteredLeads.FindAll(x => x.VOfInterest_Model == bdValue2);
                            break;

                        case "stock":
                            BreakDown2filteredLeads = BreakDown1filteredLeads.FindAll(x => x.VOfInterest_StockNumber == bdValue2);
                            break;

                        case "zip":
                            BreakDown2filteredLeads = BreakDown1filteredLeads.FindAll(x => x.PostalCode == bdValue2);
                            break;

                        case "inventorytype":
                            BreakDown2filteredLeads = BreakDown1filteredLeads.FindAll(x => x.VOfInterest_InventoryType == bdValue2);
                            break;

                        case "leaddate":
                            BreakDown2filteredLeads = BreakDown1filteredLeads.FindAll(x => x.LeadCreatedEastTime.ToShortDateString() == bdValue2);
                            break;

                        case "leadtime":
                            BreakDown2filteredLeads = BreakDown1filteredLeads.FindAll(x => x.LeadCreatedEastTime.Hour.ToString() == bdValue2);

                            TimeSpan result = TimeSpan.FromHours(Int32.Parse(bdValue2));
                            string fromTimeString = result.ToString("hh':'mm");

                            Label2Value = fromTimeString;

                            break;


                        case "leadmonthyear":
                            //BreakDown2filteredLeads = BreakDown1filteredLeads.FindAll(x => x.LeadCreatedEastTime.ToString("MM/yyyy") == bdValue2);
                            BreakDown2filteredLeads = BreakDown1filteredLeads.FindAll(x => ((x.LeadStatusTypeName.ToUpper() != "SOLD" && x.LeadCreatedEastTime.ToString("MM/yyyy") == bdValue2) || (x.LeadStatusTypeName.ToUpper() == "SOLD" && x.VehicleSoldEastTime.ToString("MM/yyyy") == bdValue2)));

                            break;

                        default:
                            BreakDown2filteredLeads = BreakDown1filteredLeads.FindAll(x => x.DealerId.ToString() == bdValue2);
                            break;
                    }
                    leadReportDetailsModel.AssociateLeads = BreakDown2filteredLeads;
                }
                #endregion Breadown 2

                #region Breakdown 3

                if (leadReportModel.BreakDownLevel3 != null)
                {
                    Label3Value = bdValue3;
                    switch (leadReportModel.BreakDownLevel3)
                    {
                        case "associatename":
                            BreakDown3filteredLeads = BreakDown2filteredLeads.FindAll(x => x.Sales_LastName + " " + x.Sales_FirstName == bdValue3);
                            break;

                        case "brand":
                            BreakDown3filteredLeads = BreakDown2filteredLeads.FindAll(x => x.VOfInterest_Make.Trim().ToUpper() == bdValue3);
                            break;

                        case "leadgroupname":
                            BreakDown3filteredLeads = BreakDown2filteredLeads.FindAll(x => x.LeadGroupMapping == bdValue3);
                            break;

                        case "leadsourcename":
                            BreakDown3filteredLeads = BreakDown2filteredLeads.FindAll(x => x.LeadSourceName == bdValue3);
                            break;

                        case "leadstatusname":
                            BreakDown3filteredLeads = BreakDown2filteredLeads.FindAll(x => x.LeadStatusName == bdValue3);
                            break;

                        case "leadstatustype":
                            BreakDown3filteredLeads = BreakDown2filteredLeads.FindAll(x => x.LeadStatusTypeName == bdValue3);
                            break;

                        case "locationid":
                            BreakDown3filteredLeads = BreakDown2filteredLeads.FindAll(x => x.DealerId.ToString() == bdValue3);

                            foreach (var store in SalesCommission.Business.Enums.VinStores)
                            {
                                if (store.StoreId == bdValue3)
                                {
                                    Label3Value = store.Name;
                                }
                            }

                            break;

                        case "salesteam":
                            BreakDown3filteredLeads = BreakDown2filteredLeads.FindAll(x => x.Team == bdValue3);
                            break;

                        case "make":
                            BreakDown3filteredLeads = BreakDown2filteredLeads.FindAll(x => x.VOfInterest_Make.Trim().ToUpper() == bdValue3);
                            break;

                        case "model":
                            BreakDown3filteredLeads = BreakDown2filteredLeads.FindAll(x => x.VOfInterest_Model == bdValue3);
                            break;

                        case "stock":
                            BreakDown3filteredLeads = BreakDown2filteredLeads.FindAll(x => x.VOfInterest_StockNumber == bdValue3);
                            break;

                        case "zip":
                            BreakDown3filteredLeads = BreakDown2filteredLeads.FindAll(x => x.PostalCode == bdValue3);
                            break;

                        case "inventorytype":
                            BreakDown3filteredLeads = BreakDown2filteredLeads.FindAll(x => x.VOfInterest_InventoryType == bdValue3);
                            break;

                        case "leaddate":
                            BreakDown3filteredLeads = BreakDown2filteredLeads.FindAll(x => x.LeadCreatedEastTime.ToShortDateString() == bdValue3);
                            break;

                        case "leadtime":
                            BreakDown3filteredLeads = BreakDown2filteredLeads.FindAll(x => x.LeadCreatedEastTime.Hour.ToString() == bdValue3);

                            TimeSpan result = TimeSpan.FromHours(Int32.Parse(bdValue3));
                            string fromTimeString = result.ToString("hh':'mm");

                            Label3Value = fromTimeString;

                            break;


                        case "leadmonthyear":
                            //BreakDown3filteredLeads = BreakDown2filteredLeads.FindAll(x => x.LeadCreatedEastTime.ToString("MM/yyyy") == bdValue3);
                            BreakDown3filteredLeads = BreakDown2filteredLeads.FindAll(x => ((x.LeadStatusTypeName.ToUpper() != "SOLD" && x.LeadCreatedEastTime.ToString("MM/yyyy") == bdValue3) || (x.LeadStatusTypeName.ToUpper() == "SOLD" && x.VehicleSoldEastTime.ToString("MM/yyyy") == bdValue3)));

                            break;

                        default:
                            BreakDown3filteredLeads = BreakDown2filteredLeads.FindAll(x => x.DealerId.ToString() == bdValue3);
                            break;
                    }
                    leadReportDetailsModel.AssociateLeads = BreakDown3filteredLeads;
                }
                #endregion Breadown 3

                #region Breakdown 4

                if (leadReportModel.BreakDownLevel4 != null)
                {
                    Label4Value = bdValue4;
                    switch (leadReportModel.BreakDownLevel4)
                    {
                        case "associatename":
                            BreakDown4filteredLeads = BreakDown3filteredLeads.FindAll(x => x.Sales_LastName + " " + x.Sales_FirstName == bdValue4);
                            break;

                        case "brand":
                            BreakDown4filteredLeads = BreakDown3filteredLeads.FindAll(x => x.VOfInterest_Make.Trim().ToUpper() == bdValue4);
                            break;

                        case "leadgroupname":
                            BreakDown4filteredLeads = BreakDown3filteredLeads.FindAll(x => x.LeadGroupMapping == bdValue4);
                            break;

                        case "leadsourcename":
                            BreakDown4filteredLeads = BreakDown3filteredLeads.FindAll(x => x.LeadSourceName == bdValue4);
                            break;

                        case "leadstatusname":
                            BreakDown4filteredLeads = BreakDown3filteredLeads.FindAll(x => x.LeadStatusName == bdValue4);
                            break;

                        case "leadstatustype":
                            BreakDown4filteredLeads = BreakDown3filteredLeads.FindAll(x => x.LeadStatusTypeName == bdValue4);
                            break;

                        case "locationid":
                            BreakDown4filteredLeads = BreakDown3filteredLeads.FindAll(x => x.DealerId.ToString() == bdValue4);

                            foreach (var store in SalesCommission.Business.Enums.VinStores)
                            {
                                if (store.StoreId == bdValue4)
                                {
                                    Label4Value = store.Name;
                                }
                            }

                            break;

                        case "salesteam":
                            BreakDown4filteredLeads = BreakDown3filteredLeads.FindAll(x => x.Team == bdValue4);
                            break;

                        case "make":
                            BreakDown4filteredLeads = BreakDown3filteredLeads.FindAll(x => x.VOfInterest_Make.Trim().ToUpper() == bdValue4);
                            break;

                        case "model":
                            BreakDown4filteredLeads = BreakDown3filteredLeads.FindAll(x => x.VOfInterest_Model == bdValue4);
                            break;

                        case "stock":
                            BreakDown4filteredLeads = BreakDown3filteredLeads.FindAll(x => x.VOfInterest_StockNumber == bdValue4);
                            break;

                        case "zip":
                            BreakDown4filteredLeads = BreakDown3filteredLeads.FindAll(x => x.PostalCode == bdValue4);
                            break;

                        case "inventorytype":
                            BreakDown4filteredLeads = BreakDown3filteredLeads.FindAll(x => x.VOfInterest_InventoryType == bdValue4);
                            break;

                        case "leaddate":
                            BreakDown4filteredLeads = BreakDown3filteredLeads.FindAll(x => x.LeadCreatedEastTime.ToShortDateString() == bdValue4);
                            break;

                        case "leadtime":
                            BreakDown4filteredLeads = BreakDown3filteredLeads.FindAll(x => x.LeadCreatedEastTime.Hour.ToString() == bdValue4);

                            TimeSpan result = TimeSpan.FromHours(Int32.Parse(bdValue4));
                            string fromTimeString = result.ToString("hh':'mm");

                            Label4Value = fromTimeString;

                            break;


                        case "leadmonthyear":
                            //BreakDown4filteredLeads = BreakDown3filteredLeads.FindAll(x => x.LeadCreatedEastTime.ToString("MM/yyyy") == bdValue4);
                            BreakDown4filteredLeads = BreakDown3filteredLeads.FindAll(x => ((x.LeadStatusTypeName.ToUpper() != "SOLD" && x.LeadCreatedEastTime.ToString("MM/yyyy") == bdValue4) || (x.LeadStatusTypeName.ToUpper() == "SOLD" && x.VehicleSoldEastTime.ToString("MM/yyyy") == bdValue4)));

                            break;

                        default:
                            BreakDown4filteredLeads = BreakDown3filteredLeads.FindAll(x => x.DealerId.ToString() == bdValue4);
                            break;
                    }
                    leadReportDetailsModel.AssociateLeads = BreakDown4filteredLeads;
                }
                #endregion Breadown 4

            }

            //NOW GO THROUGH BREAKDOWNS AND RETURN THE LAST BREAKDOWN


            return View(leadReportDetailsModel);
        }
        public ActionResult SavedLeadReport(DateTime startdate, DateTime enddate, string bd1, string bd2,string bd3,string bd4,string vt, DateTime? compstartdate = null, DateTime? compenddate = null)
        {
            //SetUserInformation();
            var leadReportModel = new LeadReportModel();

            var includeHandyMan = false;
            if (Request.Form["chkIncludeHandyman"] != null && Request.Form["chkIncludeHandyman"] == "on")
            {
                includeHandyMan = true;
            }

            if (bd1 != null && bd1 != "")
            {
                leadReportModel.BreakDownLevel1 = bd1;
            }

            if (bd2 != null && bd2 != "")
            {
                leadReportModel.BreakDownLevel2 = bd2;
            }

            if (bd3 != null && bd3 != "")
            {
                leadReportModel.BreakDownLevel3 =bd3;
            }

            if (bd4 != null && bd4 != "")
            {
                leadReportModel.BreakDownLevel4 = bd4;
            }

            if (vt != null && vt != "")
            {
                leadReportModel.VehicleType = vt;
            }

            leadReportModel.IncludeHandyman = includeHandyMan;

            //var startDate = new DateTime();
            //var endDate = new DateTime();

            //if (Request.Form["datepickerStart"] != null)
            //{
            //    startDate = Convert.ToDateTime(Request.Form["datepickerStart"]);
            //}

            //if (Request.Form["datepickerEnd"] != null)
            //{
            //    endDate = Convert.ToDateTime(Request.Form["datepickerEnd"]);
            //}

            //var compstartDate = new DateTime();
            //var compendDate = new DateTime();

            //if (Request.Form["datepickerCompStart"] != null)
            //{
            //    compstartDate = Convert.ToDateTime(Request.Form["datepickerCompStart"]);
            //}

            //if (Request.Form["datepickerCompEnd"] != null)
            //{
            //    compendDate = Convert.ToDateTime(Request.Form["datepickerCompEnd"]);
            //}


            if (compstartdate != null)
            {
                leadReportModel.ComparisonReportStartDate = (DateTime) compstartdate;
            }
            if (compenddate != null)
            {
                leadReportModel.ComparisonReportEndDate = (DateTime) compenddate;
            }

            //leadReportModel.ComparisonReportStartDate = compstartdate;
            //leadReportModel.ComparisonReportEndDate = compenddate;

            leadReportModel.ReportStartDate = startdate;
            leadReportModel.ReportEndDate = enddate;

            var compareDates = false;
            if (compstartdate != null)
            {
                compareDates = true;
            }
            leadReportModel.CompareDates = compareDates;


            leadReportModel = SqlQueries.GetLeadReportNewByDateAndStore(leadReportModel, false);

            return View("LeadReport",leadReportModel);
        }

        [HttpPost]
        public ActionResult SavedLeadReport(LeadReportModel leadReportModel)
        {
            //SetUserInformation();

            var includeHandyMan = false;
            if (Request.Form["chkIncludeHandyman"] != null && Request.Form["chkIncludeHandyman"] == "on")
            {
                includeHandyMan = true;
            }

            if (Request.Form["breakdown1"] != null)
            {
                leadReportModel.BreakDownLevel1 = Request.Form["breakdown1"];
            }

            if (Request.Form["breakdown2"] != null)
            {
                leadReportModel.BreakDownLevel2 = Request.Form["breakdown2"];
            }

            if (Request.Form["breakdown3"] != null)
            {
                leadReportModel.BreakDownLevel3 = Request.Form["breakdown3"];
            }

            if (Request.Form["breakdown4"] != null)
            {
                leadReportModel.BreakDownLevel4 = Request.Form["breakdown4"];
            }

            leadReportModel.IncludeHandyman = includeHandyMan;

            var startDate = new DateTime();
            var endDate = new DateTime();

            if (Request.Form["datepickerStart"] != null)
            {
                startDate = Convert.ToDateTime(Request.Form["datepickerStart"]);
            }

            if (Request.Form["datepickerEnd"] != null)
            {
                endDate = Convert.ToDateTime(Request.Form["datepickerEnd"]);
            }

            var compstartDate = new DateTime();
            var compendDate = new DateTime();

            if (Request.Form["datepickerCompStart"] != null)
            {
                compstartDate = Convert.ToDateTime(Request.Form["datepickerCompStart"]);
            }

            if (Request.Form["datepickerCompEnd"] != null)
            {
                compendDate = Convert.ToDateTime(Request.Form["datepickerCompEnd"]);
            }

            leadReportModel.ComparisonReportStartDate = compstartDate;
            leadReportModel.ComparisonReportEndDate = compendDate;

            leadReportModel.ReportStartDate = startDate;
            leadReportModel.ReportEndDate = endDate;

            var compareDates = false;
            if (Request.Form["chkComparison"] != null && Request.Form["chkComparison"] == "on")
            {
                compareDates = true;
            }
            leadReportModel.CompareDates = compareDates;

            var excludeBadDups = false;
            if (Request.Form["chkBadDuplicates"] != null && Request.Form["chkBadDuplicates"] == "on")
            {
                excludeBadDups = true;
            }
            leadReportModel.ExcludeBadDuplicates = excludeBadDups;

            var excludeAllBad = false;
            if (Request.Form["chkExcludeAllBad"] != null && Request.Form["chkExcludeAllBad"] == "on")
            {
                excludeAllBad = true;
            }
            leadReportModel.ExcludeAllBad = excludeAllBad;

            var showExcluded = false;
            if (Request.Form["chkShowExcluded"] != null && Request.Form["chkShowExcluded"] == "on")
            {
                showExcluded = true;
            }
            leadReportModel.ShowExcludedGroups = showExcluded;

            leadReportModel = SqlQueries.GetLeadReportNewByDateAndStore(leadReportModel, false);

            return View("LeadReport", leadReportModel);
        }

        [HttpPost]
        public ActionResult LeadReport(LeadReportModel leadReportModel)
        {
            //SetUserInformation();

            var vehicleType = "All";
            if (Request.Form["chkVehicleType"] != null)
            {
                vehicleType = Request.Form["chkVehicleType"];
            }
            leadReportModel.VehicleType = vehicleType;

            if (Request.Form["breakdown1"] != null)
            {
                leadReportModel.BreakDownLevel1 = Request.Form["breakdown1"];
            }

            if (Request.Form["breakdown2"] != null)
            {
                leadReportModel.BreakDownLevel2 = Request.Form["breakdown2"];
            }

            if (Request.Form["breakdown3"] != null)
            {
                leadReportModel.BreakDownLevel3 = Request.Form["breakdown3"];
            }

            if (Request.Form["breakdown4"] != null)
            {
                leadReportModel.BreakDownLevel4 = Request.Form["breakdown4"];
            }

            var startDate = new DateTime();
            var endDate = new DateTime();

            if (Request.Form["datepickerStart"] != null)
            {
                startDate = Convert.ToDateTime(Request.Form["datepickerStart"]);
            }

            if (Request.Form["datepickerEnd"] != null)
            {
                endDate = Convert.ToDateTime(Request.Form["datepickerEnd"]);
            }

            var compstartDate = new DateTime();
            var compendDate = new DateTime();

            if (Request.Form["datepickerCompStart"] != null)
            {
                compstartDate = Convert.ToDateTime(Request.Form["datepickerCompStart"]);
            }

            if (Request.Form["datepickerCompEnd"] != null)
            {
                compendDate = Convert.ToDateTime(Request.Form["datepickerCompEnd"]);
            }

            leadReportModel.ComparisonReportStartDate = compstartDate;
            leadReportModel.ComparisonReportEndDate = compendDate;

            leadReportModel.ReportStartDate = startDate;
            leadReportModel.ReportEndDate = endDate;

            var compareDates = false;
            if (Request.Form["chkComparison"] != null && Request.Form["chkComparison"].Contains("on"))
            {
                compareDates = true;
            }

            var excludeBadDups = false;
            if (Request.Form["chkBadDuplicates"] != null && Request.Form["chkBadDuplicates"].Contains("on"))
            {
                excludeBadDups = true;
            }
            
            var excludeAllBad = false;
            if (Request.Form["chkExcludeAllBad"] != null && Request.Form["chkExcludeAllBad"].Contains("on"))
            {
                excludeAllBad = true;
            }
            
            var showExcluded = false;
            if (Request.Form["chkShowExcluded"] != null && Request.Form["chkShowExcluded"].Contains("on"))
            {
                showExcluded = true;
            }

            var includeHandyMan = true;
            if (Request.Form["chkIncludeHandyman"] != null && Request.Form["chkIncludeHandyman"].Contains("on"))
            {
                includeHandyMan = false;
            }

            leadReportModel.CompareDates = compareDates;
            leadReportModel.ExcludeBadDuplicates = excludeBadDups;
            leadReportModel.ExcludeAllBad = excludeAllBad;
            leadReportModel.ShowExcludedGroups = showExcluded;
            leadReportModel.IncludeHandyman = includeHandyMan;

            leadReportModel = SqlQueries.GetLeadReportNewByDateAndStore(leadReportModel,false);

            if (vehicleType == "CPO")
            {
                
            }
            else if (vehicleType == "ManagerSpecial")
            {

            }

            return View(leadReportModel);
        }

        [HttpPost]
        public ActionResult Leads(LeadReportModel leadReportModel)
        {
            //SetUserInformation();

            var includeHandyMan = false;
            if (Request.Form["chkIncludeHandyman"] != null && Request.Form["chkIncludeHandyman"] == "on")
            {
                includeHandyMan = true;
            }
            leadReportModel.IncludeHandyman = includeHandyMan;

            var startDate = new DateTime();
            var endDate = new DateTime();

            if (Request.Form["datepickerStart"] != null)
            {
                startDate = Convert.ToDateTime(Request.Form["datepickerStart"]);
            }

            if (Request.Form["datepickerEnd"] != null)
            {
                endDate = Convert.ToDateTime(Request.Form["datepickerEnd"]);
            }

            leadReportModel.ReportEndDate = endDate;
            leadReportModel.ReportStartDate = startDate;

            leadReportModel = SqlQueries.GetLeadReportByDateAndStore(leadReportModel);

            if (leadReportModel.SelectedStores == null || leadReportModel.SelectedStores.Count() == 0)
            {
                leadReportModel.SelectedStores = new List<string> { "ALL" }.ToArray();
            }

            return View(leadReportModel);
        }

        public ActionResult LeadSource()
        {
            //SetUserInformation();

            var leadSourceReportModel = new LeadSourceReportModel();

            leadSourceReportModel.ReportStartDate = DateTime.Now.AddMonths(-1);
            leadSourceReportModel.ReportEndDate = DateTime.Now;

            leadSourceReportModel.IncludeHandyman = true;

            return View(leadSourceReportModel);
        }

        [HttpPost]
        public ActionResult LeadSource(LeadSourceReportModel leadSourceReportModel)
        {
            //SetUserInformation();

            var includeHandyMan = false;
            if (Request.Form["chkIncludeHandyman"] != null && Request.Form["chkIncludeHandyman"] == "on")
            {
                includeHandyMan = true;
            }
            leadSourceReportModel.IncludeHandyman = includeHandyMan;

            var startDate = new DateTime();
            var endDate = new DateTime();

            if (Request.Form["datepickerStart"] != null)
            {
                startDate = Convert.ToDateTime(Request.Form["datepickerStart"]);
            }

            if (Request.Form["datepickerEnd"] != null)
            {
                endDate = Convert.ToDateTime(Request.Form["datepickerEnd"]);
            }

            leadSourceReportModel.ReportStartDate = startDate;
            leadSourceReportModel.ReportEndDate = endDate;

            leadSourceReportModel = SqlQueries.GetLeadSourceReportByDateAndStore(leadSourceReportModel);

            if (leadSourceReportModel.SelectedStores == null || leadSourceReportModel.SelectedStores.Count() == 0)
            {
                leadSourceReportModel.SelectedStores = new List<string> { "ALL" }.ToArray();
            }

            return View(leadSourceReportModel);
        }

        public ActionResult UpdateTitleStatus(string vin = "", string stock = "", string deal = "")
        {
            var titleDueModel = new TitleDueModel();

            titleDueModel.TitleDue = SqlQueries.GetTitleStatus(vin, stock,deal);

            if (titleDueModel.TitleDue != null && titleDueModel.TitleDue.Count > 0)
            {
                titleDueModel.TitleDueHistory = SqlQueries.GetTitleStatusHistory(titleDueModel.TitleDue[0].Id);
            }

            titleDueModel.JJFUsers = SqlQueries.GetJJFEmailUsers();

            ViewBag.CanOfficeValidate = bool.Parse(Session["CanOfficeValidate"].ToString());

            return View(titleDueModel);
        }

        [HttpPost]
        public ActionResult UpdateTitleStatus()
        {
            var titleDueModel = new TitleDueModel();
            titleDueModel.JJFUsers = SqlQueries.GetJJFEmailUsers();

            if (Request.Form != null)
            {
                var updateTitleDue = new TitleDue();

                var id = Request.Form["TitleDue[0].Id"];
                var stockNumber = Request.Form["TitleDue[0].StockNumber"];
                var year = Request.Form["TitleDue[0].Year"];
                var make = Request.Form["TitleDue[0].Make"];
                var model = Request.Form["TitleDue[0].Model"];

                var clearTitle = Request.Form["TitleDue[0].ClearTitle"];
                var titleDueBank = Request.Form["TitleDue[0].TitleDueBank"];
                var titleDueCustomer = Request.Form["TitleDueCustomer"];
                var titleDueInterco = Request.Form["TitleDue[0].TitleDueInterco"];
                var titleDueAuction = Request.Form["TitleDue[0].TitleDueAuction"];
                var odomDueCustomer = Request.Form["TitleDue[0].OdomDueCustomer"];
                var duplicateTitle = Request.Form["TitleDue[0].DuplicateTitleAppliedFor"];
                var LienDueBank = Request.Form["TitleDue[0].LienDueBank"];
                var lienDueCustomer = Request.Form["TitleDue[0].LienDueCustomer"];
                var POADue = Request.Form["TitleDue[0].POADueCust"];
                var payoffDue = Request.Form["TitleDue[0].PayoffDueCust"];
                var waitingOUT = Request.Form["TitleDue[0].WaitingOutSTTitle"];
                var other = Request.Form["TitleDue[0].Other"];

                var noTitleDispose = Request.Form["TitleDue[0].NoTitleDispose"];
                var eTitle = Request.Form["TitleDue[0].ElectronicTitle"];

                var comments = Request.Form["TitleDue[0].Comments"];

                updateTitleDue.Id = Int32.Parse(id);
                updateTitleDue.StockNumber = stockNumber;
                updateTitleDue.Year = Int32.Parse(year);
                updateTitleDue.Make = make;
                updateTitleDue.Model = model;

                updateTitleDue.UpdateDate = DateTime.Now;
                updateTitleDue.DealDate = DateTime.Now;

                if (clearTitle.Contains(","))
                {
                    clearTitle = clearTitle.Substring(0, clearTitle.IndexOf(","));
                }

                if (titleDueBank.Contains(","))
                {
                    titleDueBank = titleDueBank.Substring(0, titleDueBank.IndexOf(","));
                }

                if (titleDueCustomer.Contains(","))
                {
                    titleDueCustomer = titleDueCustomer.Substring(0, titleDueCustomer.IndexOf(","));
                }

                if (titleDueInterco.Contains(","))
                {
                    titleDueInterco = titleDueInterco.Substring(0, titleDueInterco.IndexOf(","));
                }

                if (titleDueAuction.Contains(","))
                {
                    titleDueAuction = titleDueAuction.Substring(0, titleDueAuction.IndexOf(","));
                }

                if (odomDueCustomer.Contains(","))
                {
                    odomDueCustomer = odomDueCustomer.Substring(0, odomDueCustomer.IndexOf(","));
                }

                if (duplicateTitle.Contains(","))
                {
                    duplicateTitle = duplicateTitle.Substring(0, duplicateTitle.IndexOf(","));
                }

                if (LienDueBank.Contains(","))
                {
                    LienDueBank = LienDueBank.Substring(0, LienDueBank.IndexOf(","));
                }

                if (lienDueCustomer.Contains(","))
                {
                    lienDueCustomer = lienDueCustomer.Substring(0, lienDueCustomer.IndexOf(","));
                }
                if (POADue.Contains(","))
                {
                    POADue = POADue.Substring(0, POADue.IndexOf(","));
                }
                if (payoffDue.Contains(","))
                {
                    payoffDue = payoffDue.Substring(0, payoffDue.IndexOf(","));
                }
                if (waitingOUT.Contains(","))
                {
                    waitingOUT = waitingOUT.Substring(0, waitingOUT.IndexOf(","));
                }
                if (other.Contains(","))
                {
                    other = other.Substring(0, other.IndexOf(","));
                }


                if (noTitleDispose.Contains(","))
                {
                    noTitleDispose = noTitleDispose.Substring(0, noTitleDispose.IndexOf(","));
                }

                if (eTitle.Contains(","))
                {
                    eTitle = eTitle.Substring(0, eTitle.IndexOf(","));
                }


                updateTitleDue.ClearTitle = bool.Parse(clearTitle);
                updateTitleDue.TitleDueBank= bool.Parse(titleDueBank);
                updateTitleDue.TitleDueCustomer= bool.Parse(titleDueCustomer);
                updateTitleDue.TitleDueInterco= bool.Parse(titleDueInterco);
                updateTitleDue.TitleDueAuction= bool.Parse(titleDueAuction);
                updateTitleDue.OdomDueCustomer = bool.Parse(odomDueCustomer);
                updateTitleDue.DuplicateTitleAppliedFor= bool.Parse(duplicateTitle);
                updateTitleDue.LienDueBank= bool.Parse(LienDueBank);
                updateTitleDue.LienDueCustomer= bool.Parse(lienDueCustomer);
                updateTitleDue.POADueCust= bool.Parse(POADue);
                updateTitleDue.PayoffDueCust= bool.Parse(payoffDue);
                updateTitleDue.WaitingOutSTTitle= bool.Parse(waitingOUT);
                updateTitleDue.Other= bool.Parse(other);

                updateTitleDue.NoTitleDispose = bool.Parse(noTitleDispose);
                updateTitleDue.ElectronicTitle = bool.Parse(eTitle);


                updateTitleDue.Notes = comments;
                updateTitleDue.UpdateUser = Session["UserName"].ToString();

                var bNotify = false;


                if (Request.Form["chkSendNotification"] != null)
                {

                    var isChecked = Request.Form["chkSendNotification"];

                    if (isChecked.ToUpper() == "ON")
                    {
                        bNotify = true;
                    }
                }

                var notifyIds = Request.Form["notifyUsers"];
                var emails = "";
                var emailList = new List<MailAddress>();
                if (notifyIds != null)
                {
                    foreach (var email in notifyIds.Split(','))
                    {
                        var emailItem = new MailAddress(email);
                        emailList.Add(emailItem);
                    }

                }

                if (bNotify && notifyIds != null)
                {
                    updateTitleDue.EmailAddresses = notifyIds.Replace(",", ";");
                }
                updateTitleDue.EmailSent = bNotify;
                

                var success = SqlQueries.UpdateTitleStatusById(updateTitleDue);

                    
                    if(bNotify)
                    {

                        if (notifyIds != null)
                        {

                            //Now send the email to everyone in the list

                            var updatedTitleStatus = SqlMapperUtil.SqlWithParams<TitleDue>("Select * from TitleDue where Id = @id", new { id = updateTitleDue.Id },"SalesCommission");
                            foreach(var titleDue in updatedTitleStatus)
                            {
                                var user = titleDueModel.JJFUsers.Find(x => x.DMS_Id == titleDue.SalesAssociate1);
                                titleDue.SalesAssociate1Name = user.FirstName + " " + user.LastName;

                                var locationName = titleDue.Location;

                                switch (titleDue.Location)
                                {
                                    case "FLP":
                                        locationName = "Lexington Park";
                                        break;
                                    case "LFO":
                                        locationName = "Gaithersburg Hyundai/Subaru";
                                        break;
                                    case "LFT":
                                        locationName = "Gaithersburg Toyota/Middlebrook";
                                        break;
                                    case "FOC":
                                        locationName = "Annapolis";
                                        break;
                                    case "FAM":
                                        locationName = "Frederick";
                                        break;
                                    case "WDC":
                                        locationName = "Wheaton";
                                        break;
                                    case "CDO":
                                        locationName = "Rockville Hyundai";
                                        break;
                                    case "FBS":
                                        locationName = "Rockville Buick Subaru";
                                        break;
                                    case "FTN":
                                        locationName = "Chambersburg";
                                        break;
                                    case "CJE":
                                        locationName = "Clearwater";
                                        break;
                                    case "FHG":
                                        locationName = "Hagerstown GM";
                                        break;
                                    case "FHT":
                                        locationName = "Hagerstown Chrysler";
                                        break;

                                }

                                titleDue.LocationName = locationName;
                            }
                            using (var client = new SmtpClient())
                            {
                                var forward = new MailMessage();
                                try
                                {

                                    var fromAddress = new MailAddress("idd04@fitzmall.com", "JJFServer Title Update");
                                    MailMessage mail = new MailMessage();

                                    mail.Subject = "Title Due Update by " + updatedTitleStatus[0].UpdateUser;
                                    mail.Body = "The following Vehicle was updated...";

                                    forward.From = fromAddress;
                                    mail.To.Clear();
                                    foreach(var email in emailList)
                                    {
                                        forward.To.Add(email);
                                    }

                                    mail.CC.Clear();
                                    forward.Subject = mail.Subject;
                                    forward.Body = PopulateNotificationEmail(updatedTitleStatus[0]);
                                    forward.IsBodyHtml = true;
                                    client.Send(forward);
                                    Console.WriteLine("Email was sent...");
                                }
                                catch (Exception ex)
                                {
                                    
                                    throw ex;
                                }
                            }
                        }

                    }
                


            }


            return View(titleDueModel);
        }

        public ActionResult UpdateTitleDue(string vin = "", string stock = "", string deal = "")
        {
            var titleDueModel = new TitleDueModel();

            titleDueModel.TitleDue = SqlQueries.GetTitleStatus(vin, stock, deal);

            if (titleDueModel.TitleDue != null && titleDueModel.TitleDue.Count > 0)
            {
                titleDueModel.TitleDueHistory = SqlQueries.GetTitleStatusHistory(titleDueModel.TitleDue[0].Id);
            }

            titleDueModel.JJFUsers = SqlQueries.GetJJFEmailUsers();

            return View(titleDueModel);
        }

        [HttpPost]
        public ActionResult UpdateTitleDue()
        {
            var titleDueModel = new TitleDueModel();
            titleDueModel.JJFUsers = SqlQueries.GetJJFEmailUsers();

            if (Request.Form != null)
            {
                var updateTitleDue = new TitleDue();

                var id = Request.Form["TitleDue[0].Id"];
                var stockNumber = Request.Form["TitleDue[0].StockNumber"];
                var year = Request.Form["TitleDue[0].Year"];
                var make = Request.Form["TitleDue[0].Make"];
                var model = Request.Form["TitleDue[0].Model"];

                var clearTitle = Request.Form["TitleDue[0].ClearTitle"];
                var titleDueBank = Request.Form["TitleDue[0].TitleDueBank"];
                var titleDueCustomer = Request.Form["TitleDueCustomer"];
                var titleDueInterco = Request.Form["TitleDue[0].TitleDueInterco"];
                var titleDueAuction = Request.Form["TitleDue[0].TitleDueAuction"];
                var odomDueCustomer = Request.Form["TitleDue[0].OdomDueCustomer"];
                var duplicateTitle = Request.Form["TitleDue[0].DuplicateTitleAppliedFor"];
                var LienDueBank = Request.Form["TitleDue[0].LienDueBank"];
                var lienDueCustomer = Request.Form["TitleDue[0].LienDueCustomer"];
                var POADue = Request.Form["TitleDue[0].POADueCust"];
                var payoffDue = Request.Form["TitleDue[0].PayoffDueCust"];
                var waitingOUT = Request.Form["TitleDue[0].WaitingOutSTTitle"];
                var other = Request.Form["TitleDue[0].Other"];

                var noTitleDispose = Request.Form["TitleDue[0].NoTitleDispose"];
                var eTitle = Request.Form["TitleDue[0].ElectronicTitle"];


                var comments = Request.Form["TitleDue[0].Comments"];

                updateTitleDue.Id = Int32.Parse(id);
                updateTitleDue.StockNumber = stockNumber;
                updateTitleDue.Year = Int32.Parse(year);
                updateTitleDue.Make = make;
                updateTitleDue.Model = model;

                updateTitleDue.UpdateDate = DateTime.Now;
                updateTitleDue.DealDate = DateTime.Now;

                if (clearTitle.Contains(","))
                {
                    clearTitle = clearTitle.Substring(0, clearTitle.IndexOf(","));
                }

                if (titleDueBank.Contains(","))
                {
                    titleDueBank = titleDueBank.Substring(0, titleDueBank.IndexOf(","));
                }

                if (titleDueCustomer.Contains(","))
                {
                    titleDueCustomer = titleDueCustomer.Substring(0, titleDueCustomer.IndexOf(","));
                }

                if (titleDueInterco.Contains(","))
                {
                    titleDueInterco = titleDueInterco.Substring(0, titleDueInterco.IndexOf(","));
                }

                if (titleDueAuction.Contains(","))
                {
                    titleDueAuction = titleDueAuction.Substring(0, titleDueAuction.IndexOf(","));
                }

                if (odomDueCustomer.Contains(","))
                {
                    odomDueCustomer = odomDueCustomer.Substring(0, odomDueCustomer.IndexOf(","));
                }

                if (duplicateTitle.Contains(","))
                {
                    duplicateTitle = duplicateTitle.Substring(0, duplicateTitle.IndexOf(","));
                }

                if (LienDueBank.Contains(","))
                {
                    LienDueBank = LienDueBank.Substring(0, LienDueBank.IndexOf(","));
                }

                if (lienDueCustomer.Contains(","))
                {
                    lienDueCustomer = lienDueCustomer.Substring(0, lienDueCustomer.IndexOf(","));
                }
                if (POADue.Contains(","))
                {
                    POADue = POADue.Substring(0, POADue.IndexOf(","));
                }
                if (payoffDue.Contains(","))
                {
                    payoffDue = payoffDue.Substring(0, payoffDue.IndexOf(","));
                }
                if (waitingOUT.Contains(","))
                {
                    waitingOUT = waitingOUT.Substring(0, waitingOUT.IndexOf(","));
                }
                if (other.Contains(","))
                {
                    other = other.Substring(0, other.IndexOf(","));
                }

                if (noTitleDispose.Contains(","))
                {
                    noTitleDispose = noTitleDispose.Substring(0, noTitleDispose.IndexOf(","));
                }

                if (eTitle.Contains(","))
                {
                    eTitle = eTitle.Substring(0, eTitle.IndexOf(","));
                }


                updateTitleDue.ClearTitle = bool.Parse(clearTitle);
                updateTitleDue.TitleDueBank = bool.Parse(titleDueBank);
                updateTitleDue.TitleDueCustomer = bool.Parse(titleDueCustomer);
                updateTitleDue.TitleDueInterco = bool.Parse(titleDueInterco);
                updateTitleDue.TitleDueAuction = bool.Parse(titleDueAuction);
                updateTitleDue.OdomDueCustomer = bool.Parse(odomDueCustomer);
                updateTitleDue.DuplicateTitleAppliedFor = bool.Parse(duplicateTitle);
                updateTitleDue.LienDueBank = bool.Parse(LienDueBank);
                updateTitleDue.LienDueCustomer = bool.Parse(lienDueCustomer);
                updateTitleDue.POADueCust = bool.Parse(POADue);
                updateTitleDue.PayoffDueCust = bool.Parse(payoffDue);
                updateTitleDue.WaitingOutSTTitle = bool.Parse(waitingOUT);
                updateTitleDue.Other = bool.Parse(other);

                updateTitleDue.NoTitleDispose = bool.Parse(noTitleDispose);
                updateTitleDue.ElectronicTitle = bool.Parse(eTitle);


                updateTitleDue.Notes = comments;
                updateTitleDue.UpdateUser = Session["UserName"].ToString();

                var bNotify = false;

                
                if (Request.Form["chkSendNotification"] != null)
                {
                 
                    var isChecked = Request.Form["chkSendNotification"];

                    if (isChecked.ToUpper() == "ON")
                    {
                        bNotify = true;
                    }
                }

                var notifyIds = Request.Form["notifyUsers"];
                var emails = "";
                var emailList = new List<MailAddress>();
                if(notifyIds != null)
                {
                    foreach (var email in notifyIds.Split(','))
                    {
                        var emailItem = new MailAddress(email);
                        emailList.Add(emailItem);
                    }

                }

                if (bNotify && notifyIds != null)
                {
                    updateTitleDue.EmailAddresses = notifyIds.Replace(",", ";");
                }
                updateTitleDue.EmailSent = bNotify;
               

                var success = SqlQueries.UpdateTitleStatusById(updateTitleDue);


                if (bNotify)
                    {
                        
        
                        if (notifyIds != null)
                        {
                
                            //Now send the email to everyone in the list

                            var updatedTitleStatus = SqlMapperUtil.SqlWithParams<TitleDue>("Select * from TitleDue where Id = @id", new { id = updateTitleDue.Id }, "SalesCommission");
                            foreach (var titleDue in updatedTitleStatus)
                            {
                                var user = titleDueModel.JJFUsers.Find(x => x.DMS_Id == titleDue.SalesAssociate1);
                                titleDue.SalesAssociate1Name = user.FirstName + " " + user.LastName;

                                var locationName = titleDue.Location;

                                switch (titleDue.Location)
                                {
                                    case "FLP":
                                        locationName = "Lexington Park";
                                        break;
                                    case "LFO":
                                        locationName = "Gaithersburg Hyundai/Subaru";
                                        break;
                                    case "LFT":
                                        locationName = "Gaithersburg Toyota/Middlebrook";
                                        break;
                                    case "FOC":
                                        locationName = "Annapolis";
                                        break;
                                    case "FAM":
                                        locationName = "Frederick";
                                        break;
                                    case "WDC":
                                        locationName = "Wheaton";
                                        break;
                                    case "CDO":
                                        locationName = "Rockville Hyundai";
                                        break;
                                    case "FBS":
                                        locationName = "Rockville Buick Subaru";
                                        break;
                                    case "FTN":
                                        locationName = "Chambersburg";
                                        break;
                                    case "CJE":
                                        locationName = "Clearwater";
                                        break;
                                    case "FHG":
                                        locationName = "Hagerstown GM";
                                        break;
                                    case "FHT":
                                        locationName = "Hagerstown Chrysler";
                                        break;

                                }

                                titleDue.LocationName = locationName;
                            }
                            using (var client = new SmtpClient())
                            {
                                var forward = new MailMessage();
                                try
                                {

                                    var fromAddress = new MailAddress("idd04@fitzmall.com", "JJFServer Title Update");
                                    MailMessage mail = new MailMessage();

                                    mail.Subject = "Title Due Update by " + updatedTitleStatus[0].UpdateUser;
                                    mail.Body = "The following Vehicle was updated...";

                                    forward.From = fromAddress;
                                    mail.To.Clear();
                                    foreach (var email in emailList)
                                    {
                                        forward.To.Add(email);
                                    }

                                    mail.CC.Clear();
                                    forward.Subject = mail.Subject;
                                    forward.Body = PopulateNotificationEmail(updatedTitleStatus[0]);
                                    forward.IsBodyHtml = true;
                                    client.Send(forward);
                                    Console.WriteLine("Email was sent...");
                                }
                                catch (Exception ex)
                                {

                                    throw ex;
                                }
                            }
                        }

                    }
                


            }


            return View(titleDueModel);
        }



        public string PopulateNotificationEmail(TitleDue updatedTitleDue)
        {
            var titleDueEmail = @"
<html xmlns='http://www.w3.org/1999/xhtml'>
	<head>
		<meta http-equiv='Content-Type' content='text/html; charset=UTF-8'>
		<meta name='viewport' content='width=device-width, initial-scale=1.0'>
		<style>
			body { font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif;font-size: 14px;line-height: 1.42857143;color: #333;}
	   </style>
	</head>
	<body style='margin: 0; padding: 0 15px; width='100%'>
		<table align='center' border='0' cellpadding='0' cellspacing='0' width='800px'>
			<tbody>
				<tr style='text-align:center; color:#000;'>
					<td style='padding-bottom:15px;'>
						<h2>Title Due Status Update</h2>
					</td>
				</tr>
			</tbody>
		</table>
		<table align='center' border='0' cellpadding='0' cellspacing='0' width='800px' style='margin-top:5px;'>
			<tr>
				<td>
					<h3 style='text-align:center;'>Title Information</h3>
					<table  border='0' cellpadding='0' cellspacing='0'>
						<tr>
							<td style='text-align:right;'><strong>Location: </strong></td>
							<td>{location}</td>
						</tr>
						<tr>
							<td style='text-align:right;'><strong>Customer: </strong></td>
							<td>{customer}</td>
						</tr>
						<tr>
							<td style='text-align:right;'><strong>Sales Associate: </strong></td>
							<td>{associate}</td>
						</tr>
						<tr>
							<td style='text-align:right;'><strong>Finance Manager: </strong></td>
							<td>{financemanager}</td>
						</tr>
						<tr>
							<td style='text-align:right;'><strong>Sales Manager: </strong></td>
							<td>{salesmanager}</td>
						</tr>

					</table>
					<h3 style='text-align:center;'>Vehicle Information</h3>
					<table  border='0' cellpadding='0' cellspacing='0' class='table table-bordered'>
						<tr>
							<td style='text-align:right;'><strong>VIN: </strong></td>
							<td>{vin}</td>
						</tr>
						<tr>
							<td style='text-align:right;'><strong>Stock #: </strong></td>
							<td>{stock}</td>
						</tr>
						<tr>
							<td style='text-align:right;'><strong>Year: </strong></td>
							<td>{year}</td>
						</tr>
						<tr>
							<td style='text-align:right;'><strong>Make: </strong></td>
							<td>{make}</td>
						</tr>
						<tr>
							<td style='text-align:right;'><strong>Model: </strong></td>
							<td>{model}</td>
						</tr>


					</table>
				</td>
			</tr>
			<tr>
				<td colspan='2'>
					<hr/>
				</td>
			</tr>
			<tr>
				<td colspan='2'>
					<p><strong>Current Title Status: </strong>{status}</p>
					<p><strong>Last Updated: </strong>{updatedate}</p>
					<p><strong>Last Updated By: </strong>{updateuser}</p>
					<p><strong>Comments: </strong>{comment}</p>
				</td>
			</tr>
			<tr>
				<td colspan='2' style='text-align:center; padding-top:15px;'>
					<a href='{updatelink}' target='_blank'><img src='https://www.fitzmall.com/assets/images/viewdetails.png'/></a>
				</td>
			</tr>
		</table>
	</body>
</html>            ";

            if (updatedTitleDue != null)
            {

                titleDueEmail = titleDueEmail.Replace("{updatelink}", "http://jjfserver/SalesCommission/Reports/UpdateTitleDue?vin=" + updatedTitleDue.VIN.Substring(updatedTitleDue.VIN.Length - 6));

                titleDueEmail = titleDueEmail.Replace("{location}", updatedTitleDue.LocationName);
                titleDueEmail = titleDueEmail.Replace("{customer}", updatedTitleDue.BuyerName);
                titleDueEmail = titleDueEmail.Replace("{associate}", updatedTitleDue.SalesAssociate1Name);
                titleDueEmail = titleDueEmail.Replace("{financemanager}", updatedTitleDue.FinanceManager);
                titleDueEmail = titleDueEmail.Replace("{salesmanager}", updatedTitleDue.SalesManager);
                titleDueEmail = titleDueEmail.Replace("{vin}", updatedTitleDue.VIN);
                titleDueEmail = titleDueEmail.Replace("{stock}", updatedTitleDue.StockNumber);
                titleDueEmail = titleDueEmail.Replace("{year}", updatedTitleDue.Year.ToString());
                titleDueEmail = titleDueEmail.Replace("{make}", updatedTitleDue.Make);
                titleDueEmail = titleDueEmail.Replace("{model}", updatedTitleDue.Model);

                var status = "";

                if (updatedTitleDue.TitleDueBank)
                {
                    status += "Title Bank, ";

                }
                if (updatedTitleDue.TitleDueCustomer == true)
                {
                    status += "Title Customer, ";

                }
                if (updatedTitleDue.TitleDueInterco)
                {
                    status += "Title Interco, ";

                }
                if (updatedTitleDue.TitleDueAuction)
                {
                    status += "Title Auction, ";

                }

                if (updatedTitleDue.LienDueBank)
                {
                    status += "Lien Bank, ";

                }
                if (updatedTitleDue.LienDueCustomer)
                {
                    status += "Lien Customer, ";

                }

                if (updatedTitleDue.OdomDueCustomer)
                {
                    status += "Odom Customer, ";

                }

                if (updatedTitleDue.POADueCust)
                {
                    status += "POA Due, ";

                }
                if (updatedTitleDue.PayoffDueCust)
                {
                    status += "Payoff Due, ";

                }
                if (updatedTitleDue.WaitingOutSTTitle)
                {
                    status += "Waiting Out, ";

                }
                if (updatedTitleDue.DuplicateTitleAppliedFor)
                {
                    status += "Dup Title, ";

                }
                if (updatedTitleDue.Other)
                {
                    status += "Other, ";

                }

                status = status.TrimEnd(' ').TrimEnd(',');

                titleDueEmail = titleDueEmail.Replace("{status}", status);
                titleDueEmail = titleDueEmail.Replace("{updatedate}", updatedTitleDue.UpdateDate.ToString());
                titleDueEmail = titleDueEmail.Replace("{updateuser}", updatedTitleDue.UpdateUser);
                titleDueEmail = titleDueEmail.Replace("{comment}", updatedTitleDue.Notes);
            }

            return titleDueEmail;
        }

        public string PopulateNotificationEmailMoneyDue(MoneyDue moneyDue)
        {
            var moneyDueEmail = @"
<html xmlns='http://www.w3.org/1999/xhtml'>
	<head>
		<meta http-equiv='Content-Type' content='text/html; charset=UTF-8'>
		<meta name='viewport' content='width=device-width, initial-scale=1.0'>
		<style>
			body { font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif;font-size: 14px;line-height: 1.42857143;color: #333;}
	   </style>
	</head>
	<body style='margin: 0; padding: 0 15px; width='100%'>
		<table align='center' border='0' cellpadding='0' cellspacing='0' width='800px'>
			<tbody>
				<tr style='text-align:center; color:#000;'>
					<td style='padding-bottom:15px;'>
						<h2>Money Due Status Update</h2>
					</td>
				</tr>
			</tbody>
		</table>
		<table align='center' border='0' cellpadding='0' cellspacing='0' width='800px' style='margin-top:5px;'>
			<tr>
				<td>
					<h3 style='text-align:center;'>Money Due Information</h3>
					<table  border='0' cellpadding='0' cellspacing='0'>
						<tr>
							<td style='text-align:right;'><strong>Location: </strong></td>
							<td>{location}</td>
						</tr>
						<tr>
							<td style='text-align:right;'><strong>Customer: </strong></td>
							<td>{customer}</td>
						</tr>
						<tr>
							<td style='text-align:right;'><strong>Finance Manager: </strong></td>
							<td>{financemanager}</td>
						</tr>

					</table>
					<h3 style='text-align:center;'>Vehicle Information</h3>
					<table  border='0' cellpadding='0' cellspacing='0' class='table table-bordered'>
						<tr>
							<td style='text-align:right;'><strong>Deal: </strong></td>
							<td>{deal}</td>
						</tr>
						<tr>
							<td style='text-align:right;'><strong>Due From: </strong></td>
							<td>{duefrom}</td>
						</tr>
						<tr>
							<td style='text-align:right;'><strong>Amount Due: </strong></td>
							<td>{amount}</td>
						</tr>
						<tr>
							<td style='text-align:right;'><strong>Bank Name: </strong></td>
							<td>{bank}</td>
						</tr>
						<tr>
							<td style='text-align:right;'><strong>Schedule Days: </strong></td>
							<td>{days}</td>
						</tr>
						<tr>
							<td style='text-align:right;'><strong>Root Cause: </strong></td>
							<td>{root}</td>
						</tr>

					</table>
				</td>
			</tr>
			<tr>
				<td colspan='2'>
					<hr/>
				</td>
			</tr>
			<tr>
				<td colspan='2'>
					<p><strong>Funded Status: </strong>{status}</p>
					<p><strong>Last Updated: </strong>{updatedate}</p>
					<p><strong>Last Updated By: </strong>{updateuser}</p>
					<p><strong>Comments: </strong>{comment}</p>
				</td>
			</tr>
			<tr>
				<td colspan='2' style='text-align:center; padding-top:15px;'>
					<a href='{updatelink}' target='_blank'><img src='https://www.fitzmall.com/assets/images/viewdetails.png'/></a>
				</td>
			</tr>
		</table>
	</body>
</html>            ";

            if (moneyDue != null)
            {

               moneyDueEmail = moneyDueEmail.Replace("{updatelink}", "http://jjfserver/SalesCommission/Reports/UpdateMoneyDueStatus?id=" + moneyDue.CustomerNumber + "&location=" + moneyDue.Location + "&dueFrom=" + moneyDue.DueFrom);

                moneyDueEmail = moneyDueEmail.Replace("{location}", moneyDue.Location);
                moneyDueEmail = moneyDueEmail.Replace("{customer}", moneyDue.CustomerFirstName + " " + moneyDue.CustomerLastName);
                moneyDueEmail = moneyDueEmail.Replace("{financemanager}", moneyDue.FIManager);

                var dueFrom = "";
                if(moneyDue.DueFrom.Contains("106"))
                    {
                        dueFrom = "Bank";
                    }
                else if (moneyDue.DueFrom.Contains("111"))
                {
                    dueFrom = "Customer";
                }


moneyDueEmail = moneyDueEmail.Replace("{deal}", moneyDue.DealNumber);
                moneyDueEmail = moneyDueEmail.Replace("{duefrom}", dueFrom);
                moneyDueEmail = moneyDueEmail.Replace("{amount}", moneyDue.ControlBalance.ToString());
                moneyDueEmail = moneyDueEmail.Replace("{bank}", moneyDue.BankName);
                moneyDueEmail = moneyDueEmail.Replace("{days}", moneyDue.ScheduleDays.ToString());
                moneyDueEmail = moneyDueEmail.Replace("{root}", moneyDue.RootCause);
                moneyDueEmail = moneyDueEmail.Replace("{status}", moneyDue.FundedStatus);

                moneyDueEmail = moneyDueEmail.Replace("{updatedate}", moneyDue.CommentDate.ToString());
                moneyDueEmail = moneyDueEmail.Replace("{updateuser}", moneyDue.CommentUser);
                moneyDueEmail = moneyDueEmail.Replace("{comment}", moneyDue.Comment);
            }

            return moneyDueEmail;
        }

        public ActionResult UpdateMoneyDue(string id, string location, string dueFrom)
        {
            var moneyDueModel = new MoneyDueModel();

            moneyDueModel.JJFUsers = SqlQueries.GetJJFEmailUsers();
            var allMoneyDue = SqlQueries.GetAllMoneyDue();
            var allMoneyDueHistory = SqlQueries.GetAllMoneyDueHistory();

            moneyDueModel.MoneyDue = allMoneyDue.FindAll(x => x.CustomerNumber == id && x.DueFrom == dueFrom && x.Location == location);

            var dealNumber = "";
            if (moneyDueModel.MoneyDue != null && moneyDueModel.MoneyDue.Count > 0)
            {
                dealNumber = moneyDueModel.MoneyDue[0].DealNumber;
            }

            if (dealNumber != "")
            {
                moneyDueModel.MoneyDueHistory = allMoneyDueHistory.FindAll(x => x.CustomerNumber == id && x.DueFrom == dueFrom && x.Location == location && x.DealNumber == dealNumber).OrderByDescending(x => x.CommentOrder).ToList();
            }
            else
            {
                moneyDueModel.MoneyDueHistory = allMoneyDueHistory.FindAll(x => x.CustomerNumber == id && x.DueFrom == dueFrom && x.Location == location).OrderByDescending(x => x.CommentOrder).ToList();
            }

            moneyDueModel.FIManagers = SqlQueries.GetSalesAssociates();

            if (moneyDueModel.MoneyDue != null && moneyDueModel.MoneyDue.Count > 0)
            {
                moneyDueModel.FIManagerNumber = moneyDueModel.MoneyDue[0].FIManagerNumber;
            }

            if(moneyDueModel.MoneyDueHistory != null && moneyDueModel.MoneyDueHistory.Count > 0)
            {
                if (moneyDueModel.MoneyDueHistory[0].FIManagerNumber != null && moneyDueModel.MoneyDueHistory[0].FIManagerNumber != "")
                {                    
                    //Check to see if we have an update in the history table
                    moneyDueModel.FIManagerNumber = moneyDueModel.MoneyDueHistory[0].FIManagerNumber;                    
                }

            }



            return View(moneyDueModel);
        }

        public ActionResult UpdateMoneyDueStatus(string id, string location, string dueFrom)
        {
            var moneyDueModel = new MoneyDueModel();

            moneyDueModel.JJFUsers = SqlQueries.GetJJFEmailUsers();
            var allMoneyDue = SqlQueries.GetAllMoneyDue();
            var allMoneyDueHistory = SqlQueries.GetAllMoneyDueHistory();

            moneyDueModel.MoneyDue = allMoneyDue.FindAll(x => x.CustomerNumber == id && x.DueFrom == dueFrom && x.Location == location);

            var dealNumber = "";
            if (moneyDueModel.MoneyDue != null && moneyDueModel.MoneyDue.Count > 0)
            {
                dealNumber = moneyDueModel.MoneyDue[0].DealNumber;
            }

            if (dealNumber != "")
            {
                moneyDueModel.MoneyDueHistory = allMoneyDueHistory.FindAll(x => x.CustomerNumber == id && x.DueFrom == dueFrom && x.Location == location && x.DealNumber == dealNumber).OrderByDescending(x => x.CommentOrder).ToList();
            }
            else
            {
                moneyDueModel.MoneyDueHistory = allMoneyDueHistory.FindAll(x => x.CustomerNumber == id && x.DueFrom == dueFrom && x.Location == location).OrderByDescending(x => x.CommentOrder).ToList();
            }

            moneyDueModel.FIManagers = SqlQueries.GetSalesAssociates();

            if (moneyDueModel.MoneyDue != null && moneyDueModel.MoneyDue.Count > 0)
            {
                moneyDueModel.FIManagerNumber = moneyDueModel.MoneyDue[0].FIManagerNumber;
            }

            if (moneyDueModel.MoneyDueHistory != null && moneyDueModel.MoneyDueHistory.Count > 0)
            {
                if (moneyDueModel.MoneyDueHistory[0].FIManagerNumber != null && moneyDueModel.MoneyDueHistory[0].FIManagerNumber != "")
                {
                    //Check to see if we have an update in the history table
                    moneyDueModel.FIManagerNumber = moneyDueModel.MoneyDueHistory[0].FIManagerNumber;
                }

            }



            return View(moneyDueModel);
        }

        [HttpPost]
        public ActionResult UpdateMoneyDue()
        {

            var newComment = new MoneyDue();

            if (Request.Form != null)
            {
                newComment.Location = Request.Form["hdn-Location"];
                newComment.StockNumber = Request.Form["hdn-StockNumber"];
                newComment.ScheduleDays = (Request.Form["hdn-ScheduleDays"] != null ? Convert.ToInt32(Request.Form["hdn-ScheduleDays"]) : 0);
                newComment.DealDate = (Request.Form["hdn-DealDate"] != null ? Convert.ToDateTime(Request.Form["hdn-DealDate"]) : new DateTime(1900, 1, 1));

                if (newComment.DealDate < new DateTime(1900, 1, 1))
                {
                    newComment.DealDate = new DateTime(1900, 1, 1);
                }

                newComment.DueFrom = Request.Form["hdn-DueFrom"];
                newComment.CustomerNumber = Request.Form["hdn-CustomerNumber"];
                newComment.ControlBalance = (Request.Form["hdn-ControlBalance"] != null ? Convert.ToDecimal(Request.Form["hdn-ControlBalance"]) : 0);
                newComment.CustomerFirstName = Request.Form["hdn-CustomerFirstName"];
                newComment.CustomerLastName = Request.Form["hdn-CustomerLastName"];
                newComment.FIManager = Request.Form["hdn-FIManager"];
                newComment.BankName = Request.Form["hdn-BankName"];
                newComment.FIManagerNumber = Request.Form["FIManagerNumber"];
                newComment.DealNumber = Request.Form["hdn-DealNumber"];

                newComment.BusinessPhone = Request.Form["hdn-BusinessPhone"];
                newComment.ResidencePhone = Request.Form["hdn-ResidencePhone"];
                newComment.SalesManager = Request.Form["hdn-SalesManager"];

                newComment.CommentOrder = (Request.Form["hdn-CommentOrder"] != null ? Convert.ToInt32(Request.Form["hdn-CommentOrder"]) : 0) + 1;

                var rootCause = Request.Form["rootCause"];
                var fundedStatus = Request.Form["fundedStatus"];
                var userComments = "(" + Session["UserName"].ToString() + "-" + DateTime.Now + ")" + Request.Form["userComments"];

                var oldStatus = Request.Form["hdn-FundedStatus"];
                var oldcommentUser = Request.Form["hdn-CommentUser"];

                newComment.RootCause = rootCause;

                newComment.Comment = userComments;

                if (oldStatus != fundedStatus)
                {
                    newComment.FundedStatus = fundedStatus;
                    newComment.CommentDate = DateTime.Now;
                    newComment.CommentUser = Session["UserName"].ToString();
                }
                else
                {
                    newComment.FundedStatus = fundedStatus;
                    newComment.CommentDate = (Request.Form["hdn-CommentDate"] != null ? Convert.ToDateTime(Request.Form["hdn-CommentDate"]) : new DateTime(1900, 1, 1));

                    if(newComment.CommentDate < new DateTime(1900, 1, 1))
                    {
                        newComment.CommentDate = DateTime.Now;
                    }

                    newComment.CommentUser = oldcommentUser;
                }

                newComment.LastUpdatedDate = DateTime.Now;

                var bNotify = false;
                if (Request.Form["chkSendNotification"] != null)
                {

                    var isChecked = Request.Form["chkSendNotification"];

                    if (isChecked.ToUpper() == "ON")
                    {
                        bNotify = true;
                    }
                }

                var notifyIds = Request.Form["notifyUsers"];
                var emails = "";
                var emailList = new List<MailAddress>();
                if (notifyIds != null)
                {
                    foreach (var email in notifyIds.Split(','))
                    {
                        var emailItem = new MailAddress(email);
                        emailList.Add(emailItem);
                    }

                }

                newComment.EmailSent = bNotify;
                if (bNotify && notifyIds != null)
                {
                    newComment.EmailAddresses = notifyIds.Replace(",", ";");
                }

                var success = SqlQueries.UpdateMoneyDueReport(newComment);

                
                    if (bNotify)
                    {
                        
                        if (notifyIds != null)
                        {
                            
                            //Now send the email to everyone in the list
                            
                            using (var client = new SmtpClient())
                            {
                                var forward = new MailMessage();
                                try
                                {

                                    var fromAddress = new MailAddress("idd04@fitzmall.com", "JJFServer Money Due Update");
                                    MailMessage mail = new MailMessage();

                                    mail.Subject = "Money Due Update by " + newComment.CommentUser;
                                    mail.Body = "The following information was updated...";

                                    forward.From = fromAddress;
                                    mail.To.Clear();
                                    foreach (var email in emailList)
                                    {
                                        forward.To.Add(email);
                                    }

                                    mail.CC.Clear();
                                    forward.Subject = mail.Subject;
                                    forward.Body = PopulateNotificationEmailMoneyDue(newComment);
                                    forward.IsBodyHtml = true;
                                    client.Send(forward);
                                    Console.WriteLine("Email was sent...");
                                }
                                catch (Exception ex)
                                {

                                    throw ex;
                                }
                            }
                        }
                    }
                

                }

            return new EmptyResult();
        }


        [HttpPost]
        public ActionResult UpdateMoneyDueStatus()
        {

            var newComment = new MoneyDue();

            if (Request.Form != null)
            {
                newComment.Location = Request.Form["hdn-Location"];
                newComment.StockNumber = Request.Form["hdn-StockNumber"];
                newComment.ScheduleDays = (Request.Form["hdn-ScheduleDays"] != null ? Convert.ToInt32(Request.Form["hdn-ScheduleDays"]) : 0);
                newComment.DealDate = (Request.Form["hdn-DealDate"] != null ? Convert.ToDateTime(Request.Form["hdn-DealDate"]) : new DateTime(1900, 1, 1));

                if (newComment.DealDate < new DateTime(1900, 1, 1))
                {
                    newComment.DealDate = new DateTime(1900, 1, 1);
                }

                newComment.DueFrom = Request.Form["hdn-DueFrom"];
                newComment.CustomerNumber = Request.Form["hdn-CustomerNumber"];
                newComment.ControlBalance = (Request.Form["hdn-ControlBalance"] != null ? Convert.ToDecimal(Request.Form["hdn-ControlBalance"]) : 0);
                newComment.CustomerFirstName = Request.Form["hdn-CustomerFirstName"];
                newComment.CustomerLastName = Request.Form["hdn-CustomerLastName"];
                newComment.FIManager = Request.Form["hdn-FIManager"];
                newComment.BankName = Request.Form["hdn-BankName"];
                newComment.FIManagerNumber = Request.Form["FIManagerNumber"];
                newComment.DealNumber = Request.Form["hdn-DealNumber"];

                newComment.BusinessPhone = Request.Form["hdn-BusinessPhone"];
                newComment.ResidencePhone = Request.Form["hdn-ResidencePhone"];
                newComment.SalesManager = Request.Form["hdn-SalesManager"];

                newComment.CommentOrder = (Request.Form["hdn-CommentOrder"] != null ? Convert.ToInt32(Request.Form["hdn-CommentOrder"]) : 0) + 1;

                var rootCause = Request.Form["rootCause"];
                var fundedStatus = Request.Form["fundedStatus"];
                var userComments = "(" + Session["UserName"].ToString() + "-" + DateTime.Now + ")" + Request.Form["userComments"];

                var oldStatus = Request.Form["hdn-FundedStatus"];
                var oldcommentUser = Request.Form["hdn-CommentUser"];

                newComment.RootCause = rootCause;

                newComment.Comment = userComments;

                if (oldStatus != fundedStatus)
                {
                    newComment.FundedStatus = fundedStatus;
                    newComment.CommentDate = DateTime.Now;
                    newComment.CommentUser = Session["UserName"].ToString();
                }
                else
                {
                    newComment.FundedStatus = fundedStatus;
                    newComment.CommentDate = (Request.Form["hdn-CommentDate"] != null ? Convert.ToDateTime(Request.Form["hdn-CommentDate"]) : new DateTime(1900, 1, 1));

                    if (newComment.CommentDate < new DateTime(1900, 1, 1))
                    {
                        newComment.CommentDate = DateTime.Now;
                    }

                    newComment.CommentUser = oldcommentUser;
                }

                newComment.LastUpdatedDate = DateTime.Now;

                var bNotify = false;
                if (Request.Form["chkSendNotification"] != null)
                {

                    var isChecked = Request.Form["chkSendNotification"];

                    if (isChecked.ToUpper() == "ON")
                    {
                        bNotify = true;
                    }
                }

                var notifyIds = Request.Form["notifyUsers"];
                var emails = "";
                var emailList = new List<MailAddress>();
                if (notifyIds != null)
                {
                    foreach (var email in notifyIds.Split(','))
                    {
                        var emailItem = new MailAddress(email);
                        emailList.Add(emailItem);
                    }

                }

                newComment.EmailSent = bNotify;
                if (bNotify && notifyIds != null)
                {
                    newComment.EmailAddresses = notifyIds.Replace(",", ";");
                }

                var success = SqlQueries.UpdateMoneyDueReport(newComment);
                
                    if (bNotify)
                    {

                        if (notifyIds != null)
                        {
                            //Now send the email to everyone in the list

                            using (var client = new SmtpClient())
                            {
                                var forward = new MailMessage();
                                try
                                {

                                    var fromAddress = new MailAddress("idd04@fitzmall.com", "JJFServer Money Due Update");
                                    MailMessage mail = new MailMessage();

                                mail.Subject = "Money Due Update by " + newComment.CommentUser;
                                    mail.Body = "The following information was updated...";

                                    forward.From = fromAddress;
                                    mail.To.Clear();
                                    foreach (var email in emailList)
                                    {
                                        forward.To.Add(email);
                                    }

                                    mail.CC.Clear();
                                    forward.Subject = mail.Subject;
                                    forward.Body = PopulateNotificationEmailMoneyDue(newComment);
                                    forward.IsBodyHtml = true;
                                    client.Send(forward);
                                    Console.WriteLine("Email was sent...");
                                }
                                catch (Exception ex)
                                {

                                    throw ex;
                                }
                            }
                        }
                    }
                

            }

            var moneyDueModel = new MoneyDueModel();
            return View(moneyDueModel);
        }

        [HttpPost]
        public ActionResult DeleteTitleDue(string vin)
        {
            
            if (vin != null)
            {
                var success = SqlQueries.DeleteTitleDue(vin);
                return Json(success, JsonRequestBehavior.AllowGet);
            }
            else
            {
                throw new ApplicationException("Invalid vin");
            }

        }

        //public void SetUserInformation()
        //{
        //    string AdminLogins = System.Configuration.ConfigurationManager.AppSettings["AdminLogins"].ToString();

        //    if ((Session["UserName"] == null || Session["UserName"].ToString() == ""))
        //    {
        //        if (Request.Cookies["User"] != null)
        //        {
        //            var cookieValue = Request.Cookies["User"].Value;

        //            System.Collections.Specialized.NameValueCollection qsCollection = HttpUtility.ParseQueryString(cookieValue);

        //            ViewBag.UserName = qsCollection["name"].ToString();
        //            ViewBag.Login = qsCollection["login"].ToString();

        //            if (AdminLogins.Contains(ViewBag.Login))
        //            {
        //                ViewBag.Admin = true;
        //            }
        //            else
        //            {
        //                ViewBag.Admin = false;
        //            }


        //        }
        //        else
        //        {
        //            ViewBag.UserName = "Anonymous";
        //            ViewBag.Login = "Anonymous";
        //            ViewBag.Admin = false;
        //        }

        //        Session.Add("UserName", ViewBag.UserName);
        //        Session.Add("Login", ViewBag.Login);
        //        Session.Add("IsAdmin", ViewBag.Admin);
        //    }
        //    else
        //    {
        //        ViewBag.UserName = Session["UserName"];
        //        ViewBag.Login = Session["Login"];
        //        ViewBag.Admin = Session["IsAdmin"];
        //    }

        //}
    }
}