using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using SalesCommission.Models;
using SalesCommission.Business;
using System.Collections.Specialized;
using System.Configuration;


namespace SalesCommission.Controllers
{
    public class SalesController : Controller
    {
        // GET: Sales
        public ActionResult Index(string locationId, string monthId, string yearId)
        {

            //SetUserInformation();

            var salesLogReportModel = new SalesLogReportModel();

            if(DateTime.Now < new DateTime(DateTime.Now.Year,DateTime.Now.Month,6))
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

            if(locationId != null && locationId != "")
            {
                
                if (monthId != null && monthId != "")
                {
                    salesLogReportModel.MonthId = Int32.Parse(monthId);
                }
                if (yearId != null && yearId != "")
                {
                    salesLogReportModel.YearId = Int32.Parse(yearId);
                }

                var locationCode = locationId;

                foreach (var store in Enums.StoreLocations)
                {
                    if(store.LocationId.ToUpper() == locationCode.ToUpper())
                    {
                        salesLogReportModel.StoreId = store.StoreId;
                    }
                }

                salesLogReportModel.IncludeHandyman = true;
                salesLogReportModel = SqlQueries.GetMonthlySalesReportByStoreAndDate(salesLogReportModel, salesLogReportModel.IncludeHandyman);

                var objectivesStandardsModel = new ObjectivesStandardsModel();
                objectivesStandardsModel.YearId = salesLogReportModel.YearId;
                objectivesStandardsModel.MonthId = salesLogReportModel.MonthId;

                objectivesStandardsModel = SqlQueries.GetObjectivesAndStandardsByDate(objectivesStandardsModel);

                salesLogReportModel.ObjectivesAndStandards = objectivesStandardsModel.ObjectivesAndStandards;

                salesLogReportModel.FactoryToDealerCash = SqlQueries.GetFTDByStoreAndDate(salesLogReportModel);

                salesLogReportModel.Status5 = SqlQueries.GetStatus5CountByLocationAndDate(locationCode, salesLogReportModel.YearId, salesLogReportModel.MonthId);

                salesLogReportModel.FiscalMonth = SqlQueries.GetFiscalMonthByMonthYearLocation(locationCode, salesLogReportModel.YearId, salesLogReportModel.MonthId);

                salesLogReportModel.Chargebacks = SqlQueries.GetChargebacksByStoreAndDate(salesLogReportModel.MonthId, salesLogReportModel.YearId, salesLogReportModel.StoreId);

            }


            return View(salesLogReportModel);
        }

        public ActionResult ObjectivesStandards()
        {

            //SetUserInformation();
            var objectivesStandardsModel = new ObjectivesStandardsModel();

            objectivesStandardsModel.MonthId = DateTime.Now.Month;
            objectivesStandardsModel.YearId = DateTime.Now.Year;

            return View(objectivesStandardsModel);
        }

        public ActionResult Chargebacks(string storeId, string monthId, string yearId)
        {

            //SetUserInformation();

            var chargebackModel = new ChargebackModel();

            if(storeId !=null && storeId != "")
            {
                chargebackModel.StoreId = storeId;
                chargebackModel.MonthId = Int32.Parse(monthId);
                chargebackModel.YearId = Int32.Parse(yearId);

                chargebackModel.Chargebacks = SqlQueries.GetChargebacksByStoreAndDate(chargebackModel.MonthId, chargebackModel.YearId, chargebackModel.StoreId);
            }
            else{
                chargebackModel.MonthId = DateTime.Now.Month;
                chargebackModel.YearId = DateTime.Now.Year;
            }
            return View(chargebackModel);
        }

        [HttpPost]
        public ActionResult Chargebacks(ChargebackModel chargebackModel)
        {

            if (Request.Form != null && Request.Form["hdn-ChargebackIds"] != null)
            {
                var monthId = 0;
                var yearId = 0;
                var monthYear = "";
                var storeId = "";

                monthId = (Request.Form["hdn-MonthId"] != "") ? Int32.Parse(Request.Form["hdn-MonthId"]) : 0;
                yearId = (Request.Form["hdn-YearId"] != "") ? Int32.Parse(Request.Form["hdn-YearId"]) : 0;
                storeId = Request.Form["hdn-StoreId"];
                monthYear = monthId.ToString() + "/" + yearId.ToString();

                chargebackModel.MonthId = monthId;
                chargebackModel.YearId = yearId;
                chargebackModel.StoreId = storeId;

                var chargebackIds = Request.Form["hdn-ChargebackIds"].ToString().Split(',');

                foreach (var charbackId in chargebackIds)
                {

                    var chargeback = new Chargeback();

                    chargeback.Id = Int32.Parse(charbackId);
                    chargeback.MonthYear = monthYear;
                    chargeback.StoreId = storeId;

                    chargeback.Name = Request.Form[charbackId + "-hdn-name"];
                    chargeback.Type = Request.Form[charbackId + "-hdn-type"];
                    chargeback.FrontGrossAmount = (Request.Form[charbackId + "-FrontGrossAmount"] != "" && Request.Form[charbackId + "-FrontGrossAmount"] != null) ? decimal.Parse(Request.Form[charbackId + "-FrontGrossAmount"]) : 0;
                    chargeback.UsedFrontGrossAmount = (Request.Form[charbackId + "-UsedFrontGrossAmount"] != "" && Request.Form[charbackId + "-UsedFrontGrossAmount"] != null) ? decimal.Parse(Request.Form[charbackId + "-UsedFrontGrossAmount"]) : 0;
                    chargeback.FinanceAmount = (Request.Form[charbackId + "-FinanceAmount"] != "") ? decimal.Parse(Request.Form[charbackId + "-FinanceAmount"]) : 0;
                    chargeback.ServiceContractAmount = (Request.Form[charbackId + "-ServiceContractAmount"] != "") ? decimal.Parse(Request.Form[charbackId + "-ServiceContractAmount"]) : 0;
                    chargeback.GAPAmount = (Request.Form[charbackId + "-GAPAmount"] != "") ? decimal.Parse(Request.Form[charbackId + "-GAPAmount"]) : 0;
                    chargeback.OtherAmount = (Request.Form[charbackId + "-OtherAmount"] != "" && Request.Form[charbackId + "-OtherAmount"] != null) ? decimal.Parse(Request.Form[charbackId + "-OtherAmount"]) : 0;
                    chargeback.OtherComments = (Request.Form[charbackId + "-OtherAmount-Comments"] != "") ? Request.Form[charbackId + "-OtherAmount-Comments"] : "";

                    chargeback.ROAmount = (Request.Form[charbackId + "-ROAmount"] != "" && Request.Form[charbackId + "-ROAmount"] != null) ? decimal.Parse(Request.Form[charbackId + "-ROAmount"]) : 0;
                    chargeback.UsedROAmount = (Request.Form[charbackId + "-UsedROAmount"] != "" && Request.Form[charbackId + "-UsedROAmount"] != null) ? decimal.Parse(Request.Form[charbackId + "-UsedROAmount"]) : 0;
                    chargeback.CertFeeAmount = (Request.Form[charbackId + "-CertFeeAmount"] != "" && Request.Form[charbackId + "-CertFeeAmount"] != null) ? decimal.Parse(Request.Form[charbackId + "-CertFeeAmount"]) : 0;
                    chargeback.UsedCertFeeAmount = (Request.Form[charbackId + "-UsedCertFeeAmount"] != "" && Request.Form[charbackId + "-UsedCertFeeAmount"] != null) ? decimal.Parse(Request.Form[charbackId + "-UsedCertFeeAmount"]) : 0;
                    chargeback.RebateAdjustmentAmount = (Request.Form[charbackId + "-RebateAdjustmentAmount"] != "" && Request.Form[charbackId + "-RebateAdjustmentAmount"] != null) ? decimal.Parse(Request.Form[charbackId + "-RebateAdjustmentAmount"]) : 0;
                    chargeback.UsedRebateAdjustmentAmount = (Request.Form[charbackId + "-UsedRebateAdjustmentAmount"] != "" && Request.Form[charbackId + "-UsedRebateAdjustmentAmount"] != null) ? decimal.Parse(Request.Form[charbackId + "-UsedRebateAdjustmentAmount"]) : 0;
                    chargeback.CustomerWorkOrderAmount = (Request.Form[charbackId + "-CustomerWorkOrderAmount"] != "" && Request.Form[charbackId + "-CustomerWorkOrderAmount"] != null) ? decimal.Parse(Request.Form[charbackId + "-CustomerWorkOrderAmount"]) : 0;
                    chargeback.UsedCustomerWorkOrderAmount = (Request.Form[charbackId + "-UsedCustomerWorkOrderAmount"] != "" && Request.Form[charbackId + "-UsedCustomerWorkOrderAmount"] != null) ? decimal.Parse(Request.Form[charbackId + "-UsedCustomerWorkOrderAmount"]) : 0;
                    chargeback.OtherAdjustmentAmount = (Request.Form[charbackId + "-OtherAdjustmentAmount"] != "" && Request.Form[charbackId + "-OtherAdjustmentAmount"] != null) ? decimal.Parse(Request.Form[charbackId + "-OtherAdjustmentAmount"]) : 0;
                    chargeback.UsedOtherAdjustmentAmount = (Request.Form[charbackId + "-UsedOtherAdjustmentAmount"] != "" && Request.Form[charbackId + "-UsedOtherAdjustmentAmount"] != null) ? decimal.Parse(Request.Form[charbackId + "-UsedOtherAdjustmentAmount"]) : 0;

                    chargeback.UpdateDate = new DateTime(1900, 1, 1);
                    chargeback.UpdateUser = Session["UserName"].ToString();

                    var sucess = SqlQueries.SaveChargeback(chargeback);
                }
            }

            chargebackModel.Chargebacks = SqlQueries.GetChargebacksByStoreAndDate(chargebackModel.MonthId, chargebackModel.YearId, chargebackModel.StoreId);

            if(chargebackModel.Chargebacks == null || chargebackModel.Chargebacks.Count == 0)
            {
                chargebackModel.Chargebacks = SqlQueries.UpdateChargebacksFromPrevious(chargebackModel.MonthId, chargebackModel.YearId, chargebackModel.StoreId); 
            }

            return View(chargebackModel);
        }

        public ActionResult DealDetail(string id)
        {
            //SetUserInformation();

            if (id != null && id != "")
            {

                var dealInfo = SqlQueries.GetSalesLogDealByDealKey(id);
                if (dealInfo != null)
                {
                    dealInfo.DealHistory = SqlQueries.GetSalesLogDealHistoryByDealKey(id);
                    if (dealInfo.DealHistory.Count > 1)
                    {

                        var dealHistory = dealInfo.DealHistory.FindAll(o => (o.sl_updateuser != "System Import Version"));
                        dealInfo.DealHistory = dealHistory;

                        //var previousValues = new IndividualDealDetails();
                        //foreach(var oldDeal in dealHistory)
                        //{
                        //    if((previousValues.sl_otheram != oldDeal.sl_otheram) && (oldDeal.sl_otheram != dealInfo.sl_otheram))
                        //    {
                        //        previousValues.sl_otheram = oldDeal.sl_otheram;
                        //    }
                        //}


                        if (dealInfo.sl_officeValidatedBy != null && dealInfo.sl_officeValidatedBy != "")
                        {
                            //It is office validated, get the last showroom validated version
                            //!o.sl_updateuser.Contains("Save by")) &&
                            dealInfo.PreviousSavedValues = dealInfo.DealHistory.Find(o => (o.sl_updateuser != "System Import Version") && (o.sl_showroomValidatedBy != null && o.sl_showroomValidatedBy != ""));
                        }
                        else if (dealInfo.sl_showroomValidatedBy != null && dealInfo.sl_showroomValidatedBy != "")
                        {
                            dealInfo.PreviousSavedValues = dealInfo.DealHistory.Find(o => (o.sl_showroomValidatedBy == null || o.sl_showroomValidatedBy == ""));
                        }
                        else
                        {
                            dealInfo.PreviousSavedValues = dealInfo.DealHistory[0];
                        }

                    }

                    //dealInfo.LocationCode = SqlQueries.GetLocationCodeByMakeId(dealInfo.sl_make_id);

                    dealInfo.LocationCode = SqlQueries.GetLocationCodeByMallId(dealInfo.sl_mall_id.ToString());

                    dealInfo.AftermarketItems = SqlQueries.GetAftermarketItemsByDealKey(id, dealInfo.sl_dealmonth);

                    dealInfo.SalesAssociates = SqlQueries.GetSalesAssociates();
                    dealInfo.SalesAssociates2 = SqlQueries.GetSalesAssociates();

                    dealInfo.FinanceSources = SqlQueries.GetFinanceSources();
                    dealInfo.FinanceManagers = SqlQueries.GetSalesAssociates();
                    dealInfo.ServiceCompanies = SqlQueries.GetServiceCompanies();
                    dealInfo.Malls = SqlQueries.GetMalls();
                    dealInfo.Makes = SqlQueries.GetMakes(dealInfo.sl_mall_id.ToString());
                    dealInfo.DealComments = SqlQueries.GetSalesLogDealComments(dealInfo.sl_pkey, dealInfo.sl_dealkey);

                    GetUserDealPermissions(dealInfo.LocationCode);
                    
                    dealInfo.DealFound = true;

                    return View(dealInfo);
                }
                else
                {
                    return View(new IndividualDeal());
                }
            }
            else
            {
                return View(new IndividualDeal());
            }
            
        }

        public ActionResult OfficeValidateDeals(string makeId, string monthId, string yearId)
        {


            if (makeId == null)
            {
                return RedirectToAction("Index", "Sales");
            }

            if (Request.Form != null && Request.Form["btnValidate"] != null)
            {
                var indexCount = Int32.Parse(Request.Form["indexValue"]);
                var dealsToOfficeValidate = new List<IndividualDealDetails>();

                if (indexCount > 1)
                {
                    for (int i = 1; i < indexCount; i++)
                    {
                        if(Request.Form["dealValidate-" + i] == "on")
                        {
                            var dealKey = Request.Form["dealKey-" + i];
                            var deal = new IndividualDealDetails();

                            deal.sl_dealkey = dealKey;
                            deal.sl_officeValidatedBy = Session["UserName"].ToString();
                            deal.sl_updateuser = Session["UserName"].ToString();
                            deal.sl_officeValidatedDate = DateTime.Now;
                            deal.sl_updatedate = DateTime.Now;

                            dealsToOfficeValidate.Add(deal);
                        }


                    }
                    var success = SqlQueries.UpdateOfficeValidateDealsByDealKeys(dealsToOfficeValidate);
                }
            }
            var dealListing = new DealListing();
            dealListing.Deals = SqlQueries.GetSalesLogDealsShowroomValidated(makeId, monthId, yearId);

            var locationCode = "";
            if (makeId.Contains(","))
            {
                var makeIds = makeId.Split(',');
                locationCode = SqlQueries.GetLocationCodeByMakeId(makeIds[0]);
            }
            else
            {
                locationCode = SqlQueries.GetLocationCodeByMakeId(makeId);
            }

            dealListing.LocationCode = locationCode;

            dealListing.MonthId = monthId;
            dealListing.YearId = yearId;

            GetUserDealPermissions(dealListing.LocationCode);

            return View(dealListing);
        }

        public ActionResult UnvalidatedDeals(string makeId, string monthId, string yearId)
        {
            //SetUserInformation();

            if (makeId == null)
            {
                return RedirectToAction("Index", "Sales");
            }

            var dealListing = new DealListing();
            dealListing.Deals = SqlQueries.GetSalesLogDealsNotShowroomValidated(makeId, monthId, yearId);

            var locationCode = "";
            if (makeId.Contains(","))
            {
                var makeIds = makeId.Split(',');
                locationCode = SqlQueries.GetLocationCodeByMakeId(makeIds[0]);
            }
            else
            {
                locationCode = SqlQueries.GetLocationCodeByMakeId(makeId);
            }

            dealListing.LocationCode = locationCode;

            dealListing.MonthId = monthId;
            dealListing.YearId = yearId;

            return View(dealListing);
        }

        public ActionResult DealListing(string listType, string makeId, string monthId, string yearId)
        {
            //SetUserInformation();

            if (makeId == null)
            {
                return RedirectToAction("Index", "Sales");
            }

            var dealListing = new DealListing();
            dealListing.Deals = SqlQueries.GetSalesLogDealLists(listType, makeId, monthId, yearId);

            var locationCode = "";
            if (makeId.Contains(","))
            {
                var makeIds = makeId.Split(',');
                locationCode = SqlQueries.GetLocationCodeByMakeId(makeIds[0]);
            }
            else
            {
                locationCode = SqlQueries.GetLocationCodeByMakeId(makeId);
            }

            dealListing.LocationCode = locationCode;

            //dealListing.LocationCode = SqlQueries.GetLocationCodeByMakeId(makeId);
            dealListing.MonthId = monthId;
            dealListing.YearId = yearId;

            ViewBag.ListType = listType;

            return View(dealListing);
        }

        public ActionResult LeaseDeals(string makeId, string monthId, string yearId)
        {
            //SetUserInformation();

            if (makeId == null)
            {
                return RedirectToAction("Index", "Sales");
            }

            var dealListing = new DealListing();
            dealListing.Deals = SqlQueries.GetSalesLogLeaseDeals(makeId, monthId, yearId);

            var locationCode = "";
            if (makeId.Contains(","))
            {
                var makeIds = makeId.Split(',');
                locationCode = SqlQueries.GetLocationCodeByMakeId(makeIds[0]);
            }
            else
            {
                locationCode = SqlQueries.GetLocationCodeByMakeId(makeId);
            }

            dealListing.LocationCode = locationCode;
            dealListing.MonthId = monthId;
            dealListing.YearId = yearId;

            return View(dealListing);
        }

        public ActionResult CPODeals(string makeId, string monthId, string yearId, string cpoMakeName)
        {
            //SetUserInformation();

            if (makeId == null)
            {
                return RedirectToAction("Index", "Sales");
            }
            
            var dealListing = new DealListing();
            dealListing.Deals = SqlQueries.GetSalesLogCertificationLevelDeals(makeId, monthId, yearId, "MC,MZC,GMU,VWC,CUV", cpoMakeName);

            var locationCode = "";
            if (makeId.Contains(","))
            {
                var makeIds = makeId.Split(',');
                locationCode = SqlQueries.GetLocationCodeByMakeId(makeIds[0]);
            }
            else
            {
                locationCode = SqlQueries.GetLocationCodeByMakeId(makeId);
            }

            dealListing.LocationCode = locationCode;

            dealListing.MonthId = monthId;
            dealListing.YearId = yearId;

            return View(dealListing);
        }

        public ActionResult HandymanDeals(string makeId, string monthId, string yearId)
        {
//            SetUserInformation();

            if (makeId == null)
            {
                return RedirectToAction("Index", "Sales");
            }

            var dealListing = new DealListing();
            dealListing.Deals = SqlQueries.GetSalesLogCertificationLevelDeals(makeId, monthId, yearId, "HDM");

            var locationCode = "";
            if (makeId.Contains(","))
            {
                var makeIds = makeId.Split(',');
                locationCode = SqlQueries.GetLocationCodeByMakeId(makeIds[0]);
            }
            else
            {
                locationCode = SqlQueries.GetLocationCodeByMakeId(makeId);
            }

            dealListing.LocationCode = locationCode;

            dealListing.MonthId = monthId;
            dealListing.YearId = yearId;

            return View(dealListing);
        }

        public ActionResult NextCarDeals(string makeId, string monthId, string yearId)
        {
            //            SetUserInformation();

            if (makeId == null)
            {
                return RedirectToAction("Index", "Sales");
            }

            var dealListing = new DealListing();
            dealListing.Deals = SqlQueries.GetSalesLogNextCarDeals(makeId, monthId, yearId);

            var locationCode = "";
            if (makeId.Contains(","))
            {
                var makeIds = makeId.Split(',');
                locationCode = SqlQueries.GetLocationCodeByMakeId(makeIds[0]);
            }
            else
            {
                locationCode = SqlQueries.GetLocationCodeByMakeId(makeId);
            }

            dealListing.LocationCode = locationCode;

            dealListing.MonthId = monthId;
            dealListing.YearId = yearId;

            return View(dealListing);
        }


        public ActionResult DealListingByName(string locationCode, string monthId, string yearId, string name)
        {

//            SetUserInformation();

            if (locationCode == null)
            {
                return RedirectToAction("Index", "Sales");
            }

            var dealListing = new DealListing();
            dealListing.Deals = SqlQueries.GetDealsByLocationAndCustomerName(locationCode, monthId, yearId, name);

            dealListing.LocationCode = locationCode;
            dealListing.MonthId = monthId;
            dealListing.YearId = yearId;

            return View(dealListing);
        }


        [HttpPost]
        public ActionResult GetMakes(string mallID)
        {
            var makes = SqlQueries.GetMakes(mallID.ToString());
            if (makes != null)
            {
                return Json(makes, JsonRequestBehavior.AllowGet);
            }
            else
            {
                throw new ApplicationException("Invalid Mall ID");
            }

        }

        //public static ArrayList GetMakes(int mallID)
        //{

        //    var makes = SqlQueries.GetMakes(mallID.ToString());
        //    if (makes != null)
        //    {
        //        var makeArray = new ArrayList();

        //        foreach(var make in makes)
        //        {
        //            makeArray.Add(new { Value = make.Value, Display = make.Text });
        //        }

        //        return makeArray;                
        //    }            
        //    else
        //    {
        //        throw new ApplicationException("Invalid Gender ID");
        //    }
        //}


        [HttpPost]
        public ActionResult DealDetail(IndividualDeal individualDeal)
        {
            //            SetUserInformation();

            if (Request.Form != null && Request.Form["btnDelete"] != null)
            {
                var dealPrimaryKey = individualDeal.sl_pkey;
                var dealKey = individualDeal.sl_dealkey;
                var deleteReason = Request.Form["deleteReason"];
                var deleteUser = Session["UserId"].ToString();

                var success = SqlQueries.DeleteSalesLogDealByDealKey(dealKey, deleteUser, deleteReason);
                //Delete the deal

                var month = Request.Form["sl_dealmonth.Month"];
                var year = Request.Form["sl_dealmonth.Year"];

                if (month != "" && year != "")
                {
                    individualDeal.sl_dealmonth = new DateTime(Int16.Parse(year), Int16.Parse(month), 5);
                }
                return RedirectToAction("OfficeValidateDeals", "Sales", new { makeId = individualDeal.sl_make_id, monthId = month.ToString(), yearId = year.ToString() });
            }

            if (Request.Form != null && Request.Form["btnSave"] != null)
            {
                // Only save the comments and other fields
                var dealPrimaryKey = individualDeal.sl_pkey;
                var dealKey = individualDeal.sl_dealkey;
                var comments = Request.Form["dealComments"];
                var commentUser = Session["UserName"].ToString();

                var success = SqlQueries.SaveSalesLogDealComments(dealPrimaryKey, dealKey, commentUser, comments);

                //NOW SAVE ALL THE OTHER FIELDS

                var month = Request.Form["sl_dealmonth.Month"];
                var year = Request.Form["sl_dealmonth.Year"];

                if (month != "" && year != "")
                {
                    individualDeal.sl_dealmonth = new DateTime(Int16.Parse(year), Int16.Parse(month), 5);
                }

                //WTF, why, you bastard, why?
                if (Request.Form["sl_Mall_id"].Contains(","))
                {
                    var mallId = Request.Form["sl_Mall_id"].Substring(Request.Form["sl_Mall_id"].IndexOf(',') + 1);
                    individualDeal.sl_mall_id = Convert.ToInt16(mallId);
                }

                if (Request.Form["sl_Make_id"].Contains(","))
                {
                    var makeId = Request.Form["sl_Make_id"].Substring(Request.Form["sl_Make_id"].IndexOf(',') + 1);
                    individualDeal.sl_make_id = makeId;
                }

                if (Request.Form["sl_rate_exception"].Contains(","))
                {
                    var rateException = Request.Form["sl_rate_exception"].Substring(Request.Form["sl_rate_exception"].IndexOf(',') + 1);
                    individualDeal.sl_rate_exception = rateException;
                }

                if (Request.Form["sl_price_variance_exception"].Contains(","))
                {
                    var varianceException = Request.Form["sl_price_variance_exception"].Substring(Request.Form["sl_price_variance_exception"].IndexOf(',') + 1);
                    individualDeal.sl_price_variance_exception = varianceException;
                }


                //If not showroom validated, save only select fields
                if (individualDeal.sl_showroomValidatedBy == null || individualDeal.sl_showroomValidatedBy == "")
                {
                    //SAVE THE ITEMS
                    SqlQueries.SaveSalesLogDeal(individualDeal);
                }
                else
                {
                    if (Request.Form["sl_price_variance_exception"].Contains(","))
                    {
                        var varianceException = Request.Form["sl_price_variance_exception"].Substring(Request.Form["sl_price_variance_exception"].IndexOf(',') + 1);
                        individualDeal.sl_price_variance_exception = varianceException;
                    }

                    if (Request.Form["sl_certificationLevel"].Contains(","))
                    {
                        var certificationLevel = "";
                        if (individualDeal.sl_VehicleNU == "USED")
                        {
                            certificationLevel = Request.Form["sl_certificationLevel"].Substring(Request.Form["sl_certificationLevel"].IndexOf(',') + 1);
                        }
                        else
                        {
                            certificationLevel = Request.Form["sl_certificationLevel"].Substring(0, Request.Form["sl_certificationLevel"].IndexOf(','));
                        }
                        individualDeal.sl_certificationLevel = certificationLevel;
                    }

                    decimal includedTotalPre = 0;
                    if (Request.Form["includedTotalPre"] != null)
                    {
                        includedTotalPre = Convert.ToDecimal(Request.Form["includedTotalPre"]);
                    }
                    individualDeal.Validation = "";
                    individualDeal.UpdateUser = "Save by " + Session["UserName"].ToString();
                    SqlQueries.UpdateSalesLogDeal(individualDeal, includedTotalPre);
                }

                return RedirectToAction("DealDetail", "Sales", new { id = individualDeal.sl_dealkey });
            }

            if (Request.Form != null && Request.Form["btnSubmit"] != null)
            {
                individualDeal.Validation = Request.Form["btnSubmit"];
                individualDeal.UpdateUser = Session["UserName"].ToString();

                var month = Request.Form["sl_dealmonth.Month"];
                var year = Request.Form["sl_dealmonth.Year"];

                if (month != "" && year != "")
                {
                    individualDeal.sl_dealmonth = new DateTime(Int16.Parse(year), Int16.Parse(month), 5);
                }

                //WTF, why, you bastard, why?
                if (Request.Form["sl_Mall_id"].Contains(","))
                {
                    var mallId = Request.Form["sl_Mall_id"].Substring(Request.Form["sl_Mall_id"].IndexOf(',') + 1);
                    individualDeal.sl_mall_id = Convert.ToInt16(mallId);
                }

                if (Request.Form["sl_Make_id"].Contains(","))
                {
                    var makeId = Request.Form["sl_Make_id"].Substring(Request.Form["sl_Make_id"].IndexOf(',')+1);
                    individualDeal.sl_make_id = makeId;
                }

                if (Request.Form["sl_rate_exception"].Contains(","))
                {
                    var rateException = Request.Form["sl_rate_exception"].Substring(Request.Form["sl_rate_exception"].IndexOf(',') + 1);
                    individualDeal.sl_rate_exception = rateException;
                }

                if (Request.Form["sl_price_variance_exception"].Contains(","))
                {
                    var varianceException = Request.Form["sl_price_variance_exception"].Substring(Request.Form["sl_price_variance_exception"].IndexOf(',') + 1);
                    individualDeal.sl_price_variance_exception = varianceException;
                }

                if (Request.Form["sl_certificationLevel"].Contains(","))
                {
                    var certificationLevel = "";
                    if(individualDeal.sl_VehicleNU == "USED")
                    {
                        certificationLevel = Request.Form["sl_certificationLevel"].Substring(Request.Form["sl_certificationLevel"].IndexOf(',') + 1);
                    }
                    else
                    {
                        certificationLevel = Request.Form["sl_certificationLevel"].Substring(0, Request.Form["sl_certificationLevel"].IndexOf(','));
                    }
                    individualDeal.sl_certificationLevel = certificationLevel;
                }

                decimal includedTotalPre = 0;
                if (Request.Form["includedTotalPre"] != null)
                {
                    includedTotalPre = Convert.ToDecimal(Request.Form["includedTotalPre"]);
                }
                    //if (individualDeal.sl_make_id != Request.Form["sl_Make_id"])
                    //{
                    //    var makeId = Request.Form["sl_Make_id"];
                    //    makeId = makeId.Replace(individualDeal.sl_make_id, "");
                    //    makeId = makeId.Replace(",", "");

                    //}
                    SqlQueries.UpdateSalesLogDeal(individualDeal,includedTotalPre);

                if (individualDeal.Validation == "Showroom Validate")
                {
                    //Got the StoreID, need to get location Code...                
                    return RedirectToAction("UnvalidatedDeals", "Sales", new { makeId = individualDeal.sl_make_id, monthId = month.ToString(), yearId = year.ToString() });
                }
                if (individualDeal.Validation == "Office Validate")
                {
                    //Got the StoreID, need to get location Code...                
                    return RedirectToAction("OfficeValidateDeals", "Sales", new { makeId = individualDeal.sl_make_id, monthId = month.ToString(), yearId = year.ToString() });

                }


            }

            var dealInfo = SqlQueries.GetSalesLogDealByDealKey(individualDeal.sl_dealkey);
            dealInfo.DealHistory = SqlQueries.GetSalesLogDealHistoryByDealKey(individualDeal.sl_dealkey);
            if (dealInfo.DealHistory.Count > 1)
            {

                if(dealInfo.sl_officeValidatedBy != null && dealInfo.sl_officeValidatedBy != "")
                {
                    //It is office validated, get the last showroom validated version
                    dealInfo.PreviousSavedValues = dealInfo.DealHistory.Find(o => (o.sl_updateuser != "System Import Version" && !o.sl_updateuser.Contains("Save by")) && (o.sl_showroomValidatedBy != null && o.sl_showroomValidatedBy != ""));
                }
                else if (dealInfo.sl_showroomValidatedBy != null && dealInfo.sl_showroomValidatedBy != "")
                {
                    dealInfo.PreviousSavedValues = dealInfo.DealHistory.Find(o => (o.sl_showroomValidatedBy == null || o.sl_showroomValidatedBy == ""));
                }
                else
                {
                    dealInfo.PreviousSavedValues = dealInfo.DealHistory[0];
                }

            }

            dealInfo.LocationCode = SqlQueries.GetLocationCodeByMakeId(dealInfo.sl_make_id);

            dealInfo.AftermarketItems = SqlQueries.GetAftermarketItemsByDealKey(individualDeal.sl_dealkey, individualDeal.sl_dealmonth);

            // Save the form details
            dealInfo.FinanceSources = SqlQueries.GetFinanceSources();
            dealInfo.FinanceManagers = SqlQueries.GetSalesAssociates();
            dealInfo.ServiceCompanies = SqlQueries.GetServiceCompanies();
            dealInfo.SalesAssociates = SqlQueries.GetSalesAssociates();
            dealInfo.SalesAssociates2 = SqlQueries.GetSalesAssociates();
            dealInfo.Makes = SqlQueries.GetMakes(dealInfo.sl_mall_id.ToString());
            dealInfo.Malls = SqlQueries.GetMalls();
            dealInfo.DealComments = SqlQueries.GetSalesLogDealComments(dealInfo.sl_pkey, dealInfo.sl_dealkey);
            GetUserDealPermissions(dealInfo.LocationCode);

            dealInfo.DealFound = true;
            return View(dealInfo);
        }
        

        [HttpPost]
        public ActionResult Index(SalesLogReportModel salesLogReportModel)
        {
            //SetUserInformation();

            if (Request.Form != null && Request.Form["btnSave"] != null)
            {
                salesLogReportModel = new SalesLogReportModel();
                salesLogReportModel.StoreId = Request.Form["StoreId"];
                salesLogReportModel.MonthId = Int32.Parse(Request.Form["MonthId"]);
                salesLogReportModel.YearId = Int32.Parse(Request.Form["YearId"]);
                

                //factoryToDealerCash.StoreId
                var indexCount = Int32.Parse(Request.Form["indexValue"]);
                
                for (int i = 1; i < indexCount; i++)
                {
                    var factoryToDealerCash = new FactoryToDealerCash();

                    factoryToDealerCash.MonthId = Int32.Parse(Request.Form["MonthId"]);
                    factoryToDealerCash.YearId = Int32.Parse(Request.Form["YearId"]);
                    factoryToDealerCash.StoreId = Request.Form["locationCode"];
                    factoryToDealerCash.BrandId = Request.Form["brandId-" + i];
                    factoryToDealerCash.LocationId = factoryToDealerCash.StoreId + factoryToDealerCash.BrandId;

                    decimal ftdAmount = 0;

                    if (decimal.TryParse(Request.Form["ftdAmt-" + i], out ftdAmount))
                    {
                        factoryToDealerCash.FTDAmount = ftdAmount;
                    }
                    else
                    {
                        factoryToDealerCash.FTDAmount = 0;
                    }

                    decimal ftdCPOAmount = 0;

                    if (decimal.TryParse(Request.Form["ftdCPOAmt-" + i], out ftdCPOAmount))
                    {
                        factoryToDealerCash.FTDCPOAmount = ftdCPOAmount;
                    }
                    else
                    {
                        factoryToDealerCash.FTDCPOAmount = 0;
                    }


                    //    factoryToDealerCash.FTDAmount = decimal.Parse(Request.Form["ftdAmt-" + i]);
                    factoryToDealerCash.FTDComment = Request.Form["ftdComments-" + i];                    
                    factoryToDealerCash.CreatedBy = Session["UserName"].ToString();
                    factoryToDealerCash.UpdatedBy = Session["UserName"].ToString();

                    factoryToDealerCash.UpdateDate = new DateTime(1900, 1, 1);

                    var success = SqlQueries.SaveFactoryToDealerCash(factoryToDealerCash);
                }
                

                //We have a Save of the FTD info...
            }

            if (Request.Form != null && Request.Form["btnSaveComments"] != null)
            {
                salesLogReportModel = new SalesLogReportModel();
                salesLogReportModel.StoreId = Request.Form["StoreId"];
                salesLogReportModel.MonthId = Int32.Parse(Request.Form["MonthId"]);
                salesLogReportModel.YearId = Int32.Parse(Request.Form["YearId"]);


                //factoryToDealerCash.StoreId
                var indexCount = Int32.Parse(Request.Form["indexValue"]);

                for (int i = 1; i < indexCount; i++)
                {
                    var factoryToDealerCash = new FactoryToDealerCash();

                    factoryToDealerCash.MonthId = Int32.Parse(Request.Form["MonthId"]);
                    factoryToDealerCash.YearId = Int32.Parse(Request.Form["YearId"]);
                    factoryToDealerCash.StoreId = Request.Form["locationCode"];
                    factoryToDealerCash.BrandId = Request.Form["brandId-" + i];
                    factoryToDealerCash.LocationId = factoryToDealerCash.StoreId + factoryToDealerCash.BrandId;
                    factoryToDealerCash.ManagerComment = Request.Form["managerComments-" + i];
                    factoryToDealerCash.CreatedBy = Session["UserName"].ToString();
                    factoryToDealerCash.UpdatedBy = Session["UserName"].ToString();

                    factoryToDealerCash.UpdateDate = new DateTime(1900, 1, 1);

                    var success = SqlQueries.SaveFactoryToDealerCashManagerComments(factoryToDealerCash);
                }


                //We have a Save of the FTD info...
            }

            var locationCode = SqlQueries.GetLocationCodeByStoreId(salesLogReportModel.StoreId);

            if ((salesLogReportModel.DealNumber != null && salesLogReportModel.DealNumber != "") && (salesLogReportModel.StoreId != null && salesLogReportModel.StoreId !=  ""))
            {
                var dealLocation = "";

                switch (locationCode)
                {
                    case "FMM":
                        dealLocation = "FOC";
                        break;
                    case "FSS":
                        dealLocation = "FAM";
                        break;
                    case "LFM":
                        dealLocation = "LFT";
                        break;
                    default:
                        dealLocation = locationCode;
                        break;

                }

                //Got the StoreID, need to get location Code...                
                return RedirectToAction("DealDetail", "Sales",new {id= dealLocation + salesLogReportModel.DealNumber });
                
            }
            else if ((salesLogReportModel.CustomerName != null && salesLogReportModel.CustomerName != "") && (salesLogReportModel.StoreId != null && salesLogReportModel.StoreId != ""))
            {
                //Got the StoreID, need to get location Code...
                return RedirectToAction("DealListingByName", "Sales", new { locationCode = locationCode, monthId = salesLogReportModel.MonthId, yearId = salesLogReportModel.YearId, name = salesLogReportModel.CustomerName });

            }


            var includeHandyMan = false;
            if((Request.Form["chkIncludeHandyman"] != null && Request.Form["chkIncludeHandyman"] == "on") || (Request.Form["IncludeHandyman"] != null && Request.Form["IncludeHandyman"] == "True"))
            {
                includeHandyMan = true;
            }

            var showChargebacks = false;
            if ((Request.Form["chkShowChargebacks"] != null && Request.Form["chkShowChargebacks"] == "on") || (Request.Form["chkShowChargebacks"] != null && Request.Form["chkShowChargebacks"] == "True"))
            {
                showChargebacks = true;
            }

            salesLogReportModel = SqlQueries.GetMonthlySalesReportByStoreAndDate(salesLogReportModel,includeHandyMan);

            var objectivesStandardsModel = new ObjectivesStandardsModel();
            objectivesStandardsModel.YearId = salesLogReportModel.YearId;
            objectivesStandardsModel.MonthId = salesLogReportModel.MonthId;

            objectivesStandardsModel = SqlQueries.GetObjectivesAndStandardsByDate(objectivesStandardsModel);

            salesLogReportModel.ObjectivesAndStandards = objectivesStandardsModel.ObjectivesAndStandards;

            salesLogReportModel.FactoryToDealerCash = SqlQueries.GetFTDByStoreAndDate(salesLogReportModel);

            var newLocationCode = "";
            if(locationCode == "FMM")
            {
                newLocationCode = "FOC";
            }
            else
            {
                newLocationCode = locationCode;
            }
            salesLogReportModel.Status5 = SqlQueries.GetStatus5CountByLocationAndDate(newLocationCode, salesLogReportModel.YearId, salesLogReportModel.MonthId);

            salesLogReportModel.FiscalMonth = SqlQueries.GetFiscalMonthByMonthYearLocation(locationCode, salesLogReportModel.YearId, salesLogReportModel.MonthId);

            if (salesLogReportModel.StoreId == "clearwater")
            {
                salesLogReportModel.Chargebacks = SqlQueries.GetClearwaterChargebacksByDate(salesLogReportModel.MonthId, salesLogReportModel.YearId, salesLogReportModel.StoreId);
            }
            else
            {
                salesLogReportModel.Chargebacks = SqlQueries.GetChargebacksByStoreAndDate(salesLogReportModel.MonthId, salesLogReportModel.YearId, salesLogReportModel.StoreId);
            }

            salesLogReportModel.ShowChargebacks = showChargebacks;


            //foreach (var detail in salesLogReportModel.SalesReportDetails)
            //{
            //    detail.Deals = SqlQueries.GetSalesLogDealsByStoreAndDate(detail.MakeId, salesLogReportModel.YearId, salesLogReportModel.MonthId);
            //}

            return View(salesLogReportModel);
        }

        [HttpPost]
        public ActionResult ObjectivesStandards(ObjectivesStandardsModel objectivesStandardsModel, IEnumerable<HttpPostedFileBase> files)
        {


            if (Request.Form["MonthId"] != null)
            {
                //The Submit button was clicked...
                objectivesStandardsModel = SqlQueries.GetObjectivesAndStandardsByDate(objectivesStandardsModel);

                if(objectivesStandardsModel.ObjectivesAndStandards.Count == 0)
                {
                    //Now, let's import from the previous month...
                    objectivesStandardsModel = SqlQueries.UpdateObjectivesAndStandardsFromPrevious(objectivesStandardsModel);
                }
            }
            else if(Request.Form["hdn-locationId"] != null)
            {
                var objectivesAndStandards = new ObjectivesAndStandards();

                objectivesAndStandards.MonthId = (Request.Form["hdn-MonthId"] != "") ? Int32.Parse(Request.Form["hdn-MonthId"]) : 0;
                objectivesAndStandards.YearId = (Request.Form["hdn-YearId"] != "") ? Int32.Parse(Request.Form["hdn-YearId"]) : 0;

                objectivesAndStandards.LocationId = Request.Form["hdn-locationId"];
                objectivesAndStandards.StoreId = Request.Form["hdn-locationId"].Substring(0, 3);
                objectivesAndStandards.BrandId = Request.Form["hdn-locationId"].Substring(3);

                objectivesAndStandards.FinCntPercent = (Request.Form["finCNT"] != "") ? decimal.Parse(Request.Form["finCNT"]) : 0;
                objectivesAndStandards.FinCntPerCnt = (Request.Form["finCNTPerCNT"] != "") ? decimal.Parse(Request.Form["finCNTPerCNT"]): 0;
                objectivesAndStandards.VSCPercent = (Request.Form["vsc"] != "") ? decimal.Parse(Request.Form["vsc"]) : 0;
                objectivesAndStandards.VSCPerCnt = (Request.Form["vscPerCNT"] != "") ? decimal.Parse(Request.Form["vscPerCNT"]) : 0;
                objectivesAndStandards.GAPPercent = (Request.Form["gap"] != "") ? decimal.Parse(Request.Form["gap"]) : 0;
                objectivesAndStandards.GAPPerCnt = (Request.Form["gapPerCnt"] != "") ? decimal.Parse(Request.Form["gapPerCnt"]) : 0;
                objectivesAndStandards.AftermarketPercent = (Request.Form["am"] != "") ? decimal.Parse(Request.Form["am"]) : 0;
                objectivesAndStandards.AftermarketPerCnt = (Request.Form["amPerCnt"] != "") ? decimal.Parse(Request.Form["amPerCnt"]) : 0;
                objectivesAndStandards.BPPPercent = (Request.Form["bpp"] != "") ? decimal.Parse(Request.Form["bpp"]) : 0;
                objectivesAndStandards.BPPCollectionPercent = (Request.Form["bppColl"] != "") ? decimal.Parse(Request.Form["bppColl"]) : 0;
                objectivesAndStandards.TradePercent = (Request.Form["tradePercent"] != "") ? decimal.Parse(Request.Form["tradePercent"]) : 0;
                objectivesAndStandards.FrontPVR = (Request.Form["frontPVR"] != "") ? decimal.Parse(Request.Form["frontPVR"]) : 0;
                objectivesAndStandards.BackPVR = (Request.Form["backPVR"] != "") ? decimal.Parse(Request.Form["backPVR"]) : 0;
                objectivesAndStandards.ManufacturerObjective = (Request.Form["manObj"] != "") ? decimal.Parse(Request.Form["manObj"]) : 0; 
                objectivesAndStandards.FitzgeraldObjective = (Request.Form["fitzObj"] != "") ? decimal.Parse(Request.Form["fitzObj"]) : 0;
                objectivesAndStandards.GPURObjective = objectivesAndStandards.FrontPVR + objectivesAndStandards.BackPVR;
                objectivesAndStandards.CreatedBy = Session["UserName"].ToString();
                objectivesAndStandards.UpdatedBy = Session["UserName"].ToString();

                objectivesStandardsModel = SqlQueries.SaveObjectivesAndStandards(objectivesAndStandards,files);

                objectivesStandardsModel.LocationId = objectivesAndStandards.LocationId;

            }

            objectivesStandardsModel.UserPermissions = GetUserObjectivesPermissions();

            return View(objectivesStandardsModel);
        }

        [HttpPost]
        public ActionResult GetBrandDeals(string makeId, int yearId, int monthId)
        {
            var deals = SqlQueries.GetSalesLogDealsByStoreAndDate(makeId, yearId, monthId);

            return Json(deals, JsonRequestBehavior.AllowGet);
        }

        public void GetUserDealPermissions(string locationCode)
        {

            var userIdFromSession = Session["UserId"].ToString();
            var userPermissions = SalesCommission.Business.SqlQueries.GetUserPermissions(userIdFromSession);

            if (userPermissions != null)
            {
                var permissions = userPermissions.Find(o => o.Location == locationCode);
                if(permissions != null)
                {
                    ViewBag.CanDelete = permissions.CanDeleteDeal;
                    ViewBag.CanShowroomValidate = permissions.CanShowroomValidateDeal;
                    ViewBag.CanShowroomUpdate = permissions.CanShowroomUpdateDeal;
                    ViewBag.CanOfficeValidate = permissions.CanOfficeValidateDeal;
                    ViewBag.IsAdmin = permissions.IsAdmin;
                }
                else
                {
                    ViewBag.CanDelete = false;
                    ViewBag.CanShowroomValidate = false;
                    ViewBag.CanShowroomUpdate = false;
                    ViewBag.CanOfficeValidate = false;
                    ViewBag.IsAdmin = false;
                }
            }
            else
            {
                ViewBag.CanDelete = false;
                ViewBag.CanShowroomValidate = false;
                ViewBag.CanShowroomUpdate = false;
                ViewBag.CanOfficeValidate = false;
                ViewBag.IsAdmin = false;
            }
        }
        public List<UserPersmissions> GetUserObjectivesPermissions()
        {

            var userIdFromSession = Session["UserId"].ToString();
            var userPermissions = SalesCommission.Business.SqlQueries.GetUserPermissions(userIdFromSession);

            return userPermissions;

            //string AdminLogins = ConfigurationManager.AppSettings["AdminLogins"].ToString();

            //if ((Session["UserName"] == null || Session["UserName"].ToString() == ""))
            //{
            //    if (Request.Cookies["User"] != null)
            //    {
            //        var cookieValue = Request.Cookies["User"].Value;

            //        NameValueCollection qsCollection = HttpUtility.ParseQueryString(cookieValue);

            //        ViewBag.UserName = qsCollection["name"].ToString();
            //        ViewBag.Login = qsCollection["login"].ToString();

            //        if (AdminLogins.Contains(ViewBag.Login))
            //        {
            //            ViewBag.Admin = true;
            //        }
            //        else
            //        {
            //            ViewBag.Admin = false;
            //        }


            //    }
            //    else
            //    {
            //        ViewBag.UserName = "Anonymous";
            //        ViewBag.Login = "Anonymous";
            //        ViewBag.Admin = false;
            //    }

            //    Session.Add("UserName", ViewBag.UserName);
            //    Session.Add("Login", ViewBag.Login);
            //    Session.Add("IsAdmin", ViewBag.Admin);
            //}
            //else
            //{
            //    ViewBag.UserName = Session["UserName"];
            //    ViewBag.Login = Session["Login"];
            //    ViewBag.Admin = Session["IsAdmin"];
            //}

        }
    }

}