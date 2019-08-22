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
                    newGroup.UpdateUser = "New Source Import";

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
                var totalIndex = 0;

                if (Request.Form["hdnIndex"] != null && Request.Form["hdnIndex"] != "")
                {
                    var hiddenIndex = Request.Form["hdnIndex"];
                    totalIndex  = Int32.Parse(hiddenIndex);
                }

                if(totalIndex > 0)
                {
                    var leadGroups = SqlQueries.GetLeadGroups();

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

            return View(leadMappingModel);
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
            if (Request.Form["chkComparison"] != null && Request.Form["chkComparison"] == "on")
            {
                compareDates = true;
            }
            leadReportModel.CompareDates = compareDates;

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