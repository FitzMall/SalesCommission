using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SalesCommission.Models;
using SalesCommission.Business;

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

            return View(moneyDueModel);
        }

        public ActionResult TitleDue()
        {
            var titleDueModel = new TitleDueModel();

            titleDueModel.TitleDue = SqlQueries.GetAllTitlesDue();
            //titleDueModel.TitleDueHistory = SqlQueries.GetAllTitlesDueHistory();

            return View(titleDueModel);
        }

        [HttpPost]
        public ActionResult TitleDue(string loc, string status)
        {
            var titleDueModel = new TitleDueModel();
            titleDueModel.status = status;
            titleDueModel.loc = loc;

            titleDueModel.TitleDue = SqlQueries.GetAllTitlesDue();

            var filteredTitleDue = new List<TitleDue>();

            if(loc != null && loc != "" && loc != "ALL")
            {
                var locTitleDue = titleDueModel.TitleDue.FindAll(x => x.Location == loc);

                if(status != null && status != "" && status != "ALL")
                {

                    switch(status)
                    {
                        case "0":
                            filteredTitleDue = locTitleDue.FindAll(x => x.ClearTitle == true);
                            break;
                        case "2":
                            filteredTitleDue = locTitleDue.FindAll(x => x.TitleDueBank == true);
                            break;
                        case "3":
                            filteredTitleDue = locTitleDue.FindAll(x => x.TitleDueCustomer == true);
                            break;
                        case "4":
                            filteredTitleDue = locTitleDue.FindAll(x => x.LienDueCustomer == true);
                            break;
                        case "5":
                            filteredTitleDue = locTitleDue.FindAll(x => x.TitleDueInterco == true);
                            break;
                        case "6":
                            filteredTitleDue = locTitleDue.FindAll(x => x.TitleDueAuction == true);
                            break;
                        case "7":
                            filteredTitleDue = locTitleDue.FindAll(x => x.LienDueBank == true);
                            break;
                        case "8":
                            filteredTitleDue = locTitleDue.FindAll(x => x.OdomDueCustomer == true);
                            break;
                        case "9":
                            filteredTitleDue = locTitleDue.FindAll(x => x.POADueCust == true);
                            break;
                        case "10":
                            filteredTitleDue = locTitleDue.FindAll(x => x.PayoffDueCust == true);
                            break;
                        case "11":
                            filteredTitleDue = locTitleDue.FindAll(x => x.WaitingOutSTTitle == true);
                            break;
                        case "12":
                            filteredTitleDue = locTitleDue.FindAll(x => x.Other == true);
                            break;
                        case "13":
                            filteredTitleDue = locTitleDue.FindAll(x => x.DuplicateTitleAppliedFor == true);
                            break;
                            
                    }


                    
                }
                else
                {
                    filteredTitleDue = locTitleDue;
                }


                titleDueModel.TitleDue = filteredTitleDue;

            }
            else if (status != null && status != "" && status != "ALL")
            {

                switch (status)
                {
                    case "0":
                        filteredTitleDue = titleDueModel.TitleDue.FindAll(x => x.ClearTitle == true);
                        break;
                    case "2":
                        filteredTitleDue = titleDueModel.TitleDue.FindAll(x => x.TitleDueBank == true);
                        break;
                    case "3":
                        filteredTitleDue = titleDueModel.TitleDue.FindAll(x => x.TitleDueCustomer == true);
                        break;
                    case "4":
                        filteredTitleDue = titleDueModel.TitleDue.FindAll(x => x.LienDueCustomer == true);
                        break;
                    case "5":
                        filteredTitleDue = titleDueModel.TitleDue.FindAll(x => x.TitleDueInterco == true);
                        break;
                    case "6":
                        filteredTitleDue = titleDueModel.TitleDue.FindAll(x => x.TitleDueAuction == true);
                        break;
                    case "7":
                        filteredTitleDue = titleDueModel.TitleDue.FindAll(x => x.LienDueBank == true);
                        break;
                    case "8":
                        filteredTitleDue = titleDueModel.TitleDue.FindAll(x => x.OdomDueCustomer == true);
                        break;
                    case "9":
                        filteredTitleDue = titleDueModel.TitleDue.FindAll(x => x.POADueCust == true);
                        break;
                    case "10":
                        filteredTitleDue = titleDueModel.TitleDue.FindAll(x => x.PayoffDueCust == true);
                        break;
                    case "11":
                        filteredTitleDue = titleDueModel.TitleDue.FindAll(x => x.WaitingOutSTTitle == true);
                        break;
                    case "12":
                        filteredTitleDue = titleDueModel.TitleDue.FindAll(x => x.Other == true);
                        break;
                    case "13":
                        filteredTitleDue = titleDueModel.TitleDue.FindAll(x => x.DuplicateTitleAppliedFor == true);
                        break;

                }



                titleDueModel.TitleDue = filteredTitleDue;

            }




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

            salesLogReportModel = SqlQueries.GetMonthlySalesReportByDate(salesLogReportModel, salesLogReportModel.IncludeHandyman);

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
                }

                salesReportDetailsGroup.SalesReportDetails.Add(detail);

            }
            monthlySalesReportDetails.Add(salesReportDetailsGroup);
            return monthlySalesReportDetails;
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

            salesLogReportModel = SqlQueries.GetMonthlySalesReportByDate(salesLogReportModel, salesLogReportModel.IncludeHandyman);

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

            if(ft != null && ft != "")
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
                
            }

            leadReportModel.IncludeHandyman = false;
            
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
                            BreakDown1filteredLeads = leadReportModel.AssociateLeads.FindAll(x => x.Sales_LastName == bdValue1);
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
                            BreakDown1filteredLeads = leadReportModel.AssociateLeads.FindAll(x => x.VOfInterest_Make == bdValue1);
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
                            BreakDown2filteredLeads = BreakDown1filteredLeads.FindAll(x => x.Sales_LastName == bdValue2);
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
                            BreakDown2filteredLeads = BreakDown1filteredLeads.FindAll(x => x.VOfInterest_Make == bdValue2);
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
                            BreakDown3filteredLeads = BreakDown2filteredLeads.FindAll(x => x.Sales_LastName == bdValue3);
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
                            BreakDown3filteredLeads = BreakDown2filteredLeads.FindAll(x => x.VOfInterest_Make == bdValue3);
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
                            BreakDown4filteredLeads = BreakDown3filteredLeads.FindAll(x => x.Sales_LastName == bdValue4);
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
                            BreakDown4filteredLeads = BreakDown3filteredLeads.FindAll(x => x.VOfInterest_Make == bdValue4);
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

            //leadReportModel.IncludeHandyman = includeHandyMan;

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

            leadReportModel.CompareDates = compareDates;
            leadReportModel.ExcludeBadDuplicates = excludeBadDups;
            leadReportModel.ExcludeAllBad = excludeAllBad;
            leadReportModel.ShowExcludedGroups = showExcluded;

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
            //titleDueModel.TitleDueHistory = SqlQueries.GetTitleStatusHistory(vin, stock, deal);

            return View(titleDueModel);
        }

        [HttpPost]
        public ActionResult UpdateTitleStatus()
        {
            var titleDueModel = new TitleDueModel();

            //titleDueModel.TitleDue = SqlQueries.GetAllTitlesDue();
            //titleDueModel.TitleDueHistory = SqlQueries.GetAllTitlesDueHistory();

            return View(titleDueModel);
        }


        public ActionResult UpdateMoneyDue(string id, string location, string dueFrom)
        {
            var moneyDueModel = new MoneyDueModel();

            var allMoneyDue = SqlQueries.GetAllMoneyDue();
            var allMoneyDueHistory = SqlQueries.GetAllMoneyDueHistory();

            moneyDueModel.MoneyDue = allMoneyDue.FindAll(x => x.CustomerNumber == id && x.DueFrom == dueFrom && x.Location == location);
            moneyDueModel.MoneyDueHistory = allMoneyDueHistory.FindAll(x => x.CustomerNumber == id && x.DueFrom == dueFrom && x.Location == location).OrderByDescending(x=>x.CommentOrder).ToList();

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

                var success = SqlQueries.UpdateMoneyDueReport(newComment);
            }

            return new EmptyResult();
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