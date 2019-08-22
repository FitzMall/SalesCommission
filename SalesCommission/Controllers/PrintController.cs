using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SalesCommission.Models;
using SalesCommission.Business;

namespace SalesCommission.Controllers
{
    public class PrintController : Controller
    {
        // GET: Print
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TradeReport(string storeId, string monthId, string yearId, string includeHandyman)
        {
            //SetUserInformation();
            var tradeReportModel = new TradeReportModel();

            tradeReportModel.IncludeDeals = true;
            tradeReportModel.IncludeHandyman = true;
            tradeReportModel.MonthId = Convert.ToInt32(monthId);
            tradeReportModel.YearId = Convert.ToInt32(yearId);


            tradeReportModel = SqlQueries.GetMonthlyTradeReportByDate(tradeReportModel, tradeReportModel.IncludeHandyman);


            var storeDetailsRemoved = tradeReportModel.TradeReportDetails.RemoveAll(o => storeId != o.AutoMallId.ToString());

            return View(tradeReportModel);
        }

    }
}