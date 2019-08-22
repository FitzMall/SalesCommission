using System;
using SalesCommission.Models;
using SalesCommission.Business;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Linq;

namespace SalesCommission.Business
{
    public class SqlQueries
    {

        public static List<ObjectivesAndStandardsDetails> GetReportingObjectivesAndStandards(DateTime reportDate, bool includeHandyman)
        {

            var objectivesAndStandardsDetails = SqlMapperUtil.StoredProcWithParams<ObjectivesAndStandardsDetails>("sp_SalesLogReportingObjectivesStandards", new { ReportDate = reportDate, IncludeHandyman = includeHandyman }, "SalesCommission");

            var bppCollection = SqlMapperUtil.StoredProcWithParams<BPPCollection>("sp_SalesLogGetBPPCollection", new { ReportDate = reportDate.ToShortDateString() }, "JJFServerFOX");


            foreach(var detail in objectivesAndStandardsDetails)
            {
                var locationCode = "";
                //Determine Location Code
                foreach (var location in Enums.StoreLocations)
                {
                    if (location.StoreId.ToLower() == detail.AutoMall.ToLower())
                    {
                        locationCode = location.LocationId;
                    }

                }

                var locationBPP = new List<BPPCollection>();
                if (detail.BrandId.ToUpper() == "UU")
                {
                    locationBPP = bppCollection.FindAll(o => o.Location == locationCode && o.ConditionAtSold == "U");
                }
                else
                {
                    locationBPP = bppCollection.FindAll(o => o.Location == locationCode && o.ConditionAtSold == "N" && o.VehMake.ToUpper().Contains(detail.MakeName.ToUpper()));
                }
                //Set the BPP Collection percentage
                //var locationBPP = bppCollection.FindAll(o => o.Location == locationCode);

                double totalListAmount = 0;
                double totalListCollected = 0;

                foreach (var locBPP in locationBPP)
                {
                    totalListAmount += locBPP.BPPListAmt;
                    totalListCollected += locBPP.CollectedAmt;
                }

                detail.BPPCollectionPercent = totalListCollected / totalListAmount;
            }

            return objectivesAndStandardsDetails;

        }


        public static SalesLogReportModel GetMonthlySalesReportByStoreAndDate(SalesLogReportModel salesLogReportModel, bool includeHandyman)
        {

            var completeSalesLogReportModel = new SalesLogReportModel();
            var reportDate = new DateTime(salesLogReportModel.YearId, salesLogReportModel.MonthId, 1);
            var salesReportDetails = SqlMapperUtil.StoredProcWithParams<SalesReportDetail>("sp_SalesLogReportByDateAndStore", new { AutoMall = salesLogReportModel.StoreId, ReportDate = reportDate, IncludeHandyman = includeHandyman }, "SalesCommission");

            var leaseCounts = SqlMapperUtil.StoredProcWithParams<SalesReportDetail>("sp_SalesLogReportLeaseCountByDateAndStore", new { AutoMall = salesLogReportModel.StoreId, ReportDate = reportDate, IncludeHandyman = includeHandyman }, "SalesCommission");
            //BPP Collection Percentage
            var certifiedCounts = SqlMapperUtil.StoredProcWithParams<CertifiedCount>("sp_SalesLogReportCertifiedCountByDateAndStore", new { AutoMall = salesLogReportModel.StoreId, ReportDate = reportDate }, "SalesCommission");

            var bppCollection = SqlMapperUtil.StoredProcWithParams<BPPCollection>("sp_SalesLogGetBPPCollection", new { ReportDate = reportDate.ToShortDateString() }, "JJFServerFOX");

            completeSalesLogReportModel.MonthId = salesLogReportModel.MonthId;
            completeSalesLogReportModel.YearId = salesLogReportModel.YearId;
            completeSalesLogReportModel.StoreId = salesLogReportModel.StoreId;
            completeSalesLogReportModel.IncludeHandyman = includeHandyman;

            foreach (var detail in salesReportDetails)
            {
                detail.TotalAmount =  detail.DealGrossAmount + detail.FinIncAmount + detail.VSCAmount + detail.GapAmount + detail.MCAmount; //detail.OtherAmount + detail.FTDAmount
                detail.VariancePercentage = (double)detail.VarianceCount / (double)detail.DealCount;

                if (detail.DealCount > 0 && detail.TotalAmount != 0)
                {
                    detail.PVRAmount = (double)detail.TotalAmount / (double)detail.DealCount;
                }

                var firstOfMonth = new DateTime(completeSalesLogReportModel.YearId, completeSalesLogReportModel.MonthId,1);
                var lastOfMonth = new DateTime();
                lastOfMonth = firstOfMonth.AddMonths(1);

                var sqlJoin = " FROM dbo.mall mall INNER JOIN dbo.make make ON mall.id = make.mall_id INNER JOIN saleslog ON make.id = saleslog.sl_make_id WHERE mall.automall='" + detail.AutoMall + "' AND (saleslog.sl_dealmonth > '" + firstOfMonth + "'  AND saleslog.sl_dealmonth < '" + lastOfMonth + "' )";
                var sqlGroup = " Group by saleslog.sl_make_id , make , automall";

                var sqlHoldbackCount = "SELECT count([saleslog].[sl_dealGross]) as cntDeals " + sqlJoin + " AND ( [saleslog].[sl_dxHoldback] IS NULL OR [saleslog].[sl_dxHoldback] = 0 ) AND saleslog.sl_make_id=" + detail.MakeId + " AND saleslog.sl_valInternet IN ('V','V2') " + sqlGroup;
                var holdbackCount = SqlMapperUtil.SqlWithParams<int>(sqlHoldbackCount, null, "SalesCommission");

                var sqlHoldbackAmount = "SELECT sum( [saleslog].[sl_dxHoldback] ) as sumHoldback " + sqlJoin + " AND ( [saleslog].[sl_dxHoldback] IS NOT NULL OR [saleslog].[sl_dxHoldback] <> 0 )  AND saleslog.sl_make_id=" + detail.MakeId + sqlGroup;
                var holdbackAmount = SqlMapperUtil.SqlWithParams<decimal>(sqlHoldbackAmount, null, "SalesCommission");

                if (holdbackCount.Count > 0)
                {
                    detail.HoldbackCount = holdbackCount[0];
                    detail.ValuePercentage = (double)holdbackCount[0] / (double)detail.DealCount;
                }
                else
                {
                    detail.HoldbackCount = 0;
                    detail.ValuePercentage = 0;
                }

                //Set the lease counts...

                if (leaseCounts != null && leaseCounts.Count > 0)
                {
                    var leases = leaseCounts.Find(o => o.MakeId == detail.MakeId);
                    if (leases != null)
                    {
                        detail.LeaseCount = leases.LeaseCount;
                    }
                }
                var locationCode = "";
                //Determine Location Code
                foreach(var location in Enums.StoreLocations)
                {
                    if(location.StoreId.ToLower() == salesLogReportModel.StoreId.ToLower())
                    {
                        locationCode = location.LocationId;
                    }
                
                }

                var locationBPP = new List<BPPCollection>();
                if (bppCollection != null)
                {
                    if (detail.BrandId.ToUpper() == "UU")
                    {
                        locationBPP = bppCollection.FindAll(o => o.Location == locationCode && o.ConditionAtSold == "U");
                    }
                    else
                    {
                        try
                        {
                            locationBPP = bppCollection.FindAll(o => o.Location == locationCode && o.ConditionAtSold == "N" && o.VehMake.ToUpper().Contains(detail.MakeName.ToUpper()));
                        }
                        catch(Exception ex)
                        {
                            locationBPP = new List<BPPCollection>();
                        }
                    }
                }
                               
                //Set the BPP Collection percentage
                //var locationBPP = bppCollection.FindAll(o => o.Location == locationCode);

                double totalListAmount = 0;
                double totalListCollected = 0;

                foreach (var locBPP in locationBPP)
                {
                    totalListAmount += locBPP.BPPListAmt;
                    totalListCollected += locBPP.CollectedAmt;
                }

                detail.BPPCollectionPercent = totalListCollected / totalListAmount;

                if (detail.BrandId.ToUpper() != "UU")// || detail.MakeId == "87")
                {
                    
                    if (certifiedCounts != null && certifiedCounts.Count > 0)
                    {
                        try
                        {
                            var makeName = "";

                                makeName = detail.MakeName.ToUpper();
                            detail.CPOCount = 0;


                            var cpoListCount = certifiedCounts.FindAll(o => o.CertifiedMake.ToUpper().Contains(makeName));

                            if(detail.MakeId == "41")
                            {
                                cpoListCount = certifiedCounts.FindAll(o => o.CertifiedMake.ToUpper().Contains(makeName) || o.CertifiedMake.ToUpper().Contains("RAM") || o.CertifiedMake.ToUpper().Contains("DODGE"));
                            }

                            if (cpoListCount != null && cpoListCount.Count > 0)
                            {
                                foreach (var cpoCount in cpoListCount)
                                {
                                    detail.CPOCount += cpoCount.CertifiedMakeCount;
                                }

                            }
                        }
                        catch(Exception ex)
                        {
                            detail.CPOCount = 0;
                        }

                    }
                }
                

            }

            completeSalesLogReportModel.SalesReportDetails = salesReportDetails;

            return completeSalesLogReportModel;
        }

        public static AssociatePerformanceModel GetAssociatePerformanceByDate(AssociatePerformanceModel associatePerformanceModel)
        {
            var reportDate = new DateTime(Convert.ToInt32(associatePerformanceModel.YearId), Convert.ToInt32(associatePerformanceModel.MonthId), 1);
            var reportEndDate = reportDate.AddMonths(1).AddDays(-1);

            var associateAppraisals = SqlMapperUtil.StoredProcWithParams<AppraisalDetail>("sp_CommissionGetAssociateAppraisalDetails", new { StartDate = reportDate.Date, EndDate = reportEndDate.Date }, "ReynoldsData");
            var associateOpportunities = SqlMapperUtil.StoredProcWithParams<OpportunityDetail>("sp_CommissionGetAssociateOpportunityDetails", new { StartDate = reportDate.Date, EndDate = reportEndDate.Date }, "ReynoldsData");

            var associateName = associatePerformanceModel.AssociateName;

            var lastName = associateName.Substring(associateName.LastIndexOf(' ')+1).ToLower();
            var partialFirstName = associateName.Substring(0,3).ToLower();


            var filteredAppraisals = associateAppraisals.FindAll(x => x.SalesPerson.ToLower().Contains(lastName) && x.SalesPerson.ToLower().Contains(partialFirstName));
            var filteredOpportunities = associateOpportunities.FindAll(x => x.Sales_LastName.ToLower().Contains(lastName) && x.Sales_LastName.ToLower().Contains(partialFirstName));

            if (filteredAppraisals == null || filteredAppraisals.Count == 0)
            {
                filteredAppraisals = associateAppraisals.FindAll(x => x.SalesPerson.ToLower().Contains(lastName));
            }
           
            if (filteredOpportunities == null || filteredOpportunities.Count == 0)
            {
                filteredOpportunities = associateOpportunities.FindAll(x => x.Sales_LastName.ToLower().Contains(lastName));
            }
           
            associatePerformanceModel.AssociateAppraisals = filteredAppraisals;
            associatePerformanceModel.AssociateOpportunities = filteredOpportunities;

            return associatePerformanceModel;
        }
        public static TradeReportModel GetMonthlyTradeReportByDate(TradeReportModel tradeReportModel, bool includeHandyman)
        {
            var completeTradeReportModel = new TradeReportModel();

            var reportDate = new DateTime(tradeReportModel.YearId, tradeReportModel.MonthId, 1);
            var reportEndDate = reportDate.AddMonths(1).AddDays(-1);
            var tradeReportDetails = SqlMapperUtil.StoredProcWithParams<TradeReport>("sp_SalesLogTradeReportByDate", new { ReportDate = reportDate, IncludeHandyman = includeHandyman }, "SalesCommission");

            var monthlyTradeReportDetails = new List<TradeReportDetail>();

            var tradeReportDetailsGroup = new TradeReportDetail();
            tradeReportDetailsGroup.TradeReports = new List<TradeReport>();

            var autoMall = "";

            if (tradeReportDetails != null && tradeReportDetails.Count > 0)
            {
                autoMall = tradeReportDetails[0].AutoMall;
            }

            foreach (var detail in tradeReportDetails)
            {
                if (detail.AutoMall != autoMall)
                {
                    tradeReportDetailsGroup.TradeDeals = SqlMapperUtil.StoredProcWithParams<TradeDeal>("sp_SalesLogReportGetTradeDealsByDateAndStore", new { ReportDate = reportDate, AutoMall = autoMall }, "SalesCommission");

                    monthlyTradeReportDetails.Add(tradeReportDetailsGroup);
                    autoMall = detail.AutoMall;

                    tradeReportDetailsGroup = new TradeReportDetail();
                    tradeReportDetailsGroup.TradeReports = new List<TradeReport>();
                }

                tradeReportDetailsGroup.AutoMall = detail.AutoMall;
                tradeReportDetailsGroup.AutoMallId = detail.AutoMallId;

                foreach(var store in Business.Enums.StoreDealerId)
                {
                    if(store.StoreId.ToLower() == detail.AutoMall.ToLower())
                    {
                        tradeReportDetailsGroup.DealerId = Int16.Parse(store.Name);
                    }
                }

                

                //tradeReportDetailsGroup.BrandId = detail.BrandId;
                tradeReportDetailsGroup.TradeReports.Add(detail);
                
            }
            
            tradeReportDetailsGroup.TradeDeals = SqlMapperUtil.StoredProcWithParams<TradeDeal>("sp_SalesLogReportGetTradeDealsByDateAndStore", new { ReportDate = reportDate, AutoMall = autoMall }, "SalesCommission");
            monthlyTradeReportDetails.Add(tradeReportDetailsGroup);

            completeTradeReportModel.TradeReportDetails = monthlyTradeReportDetails;

            completeTradeReportModel.AssociateTradeReports = SqlMapperUtil.StoredProcWithParams<AssociateTradeReport>("sp_SalesLogTradeReportByDateAndAssociate", new { ReportDate = reportDate, IncludeHandyman = includeHandyman }, "SalesCommission");

            var salesAssociates = GetSalesAssociates();

            foreach(var associate in completeTradeReportModel.AssociateTradeReports)
            {
                if (associate.SalesAssociateId != null)
                {
                    try
                    {
                        associate.SalesAssociateName = salesAssociates.Find(o => o.Value == associate.SalesAssociateId).Text;
                    }
                    catch(Exception ex)
                    {
                        associate.SalesAssociateName = "Not Found";
                    }
                }
                else
                {
                    associate.SalesAssociateName = "None set";
                }
            }

            completeTradeReportModel.AssociateAppraisalsOld = SqlMapperUtil.StoredProcWithParams<AppraisalsOld>("usp_RPT_AppraisalPerformanceBySalesperson", new { startDate = reportDate, endDate = reportEndDate }, "ReynoldsData");
            // completeTradeReportModel.AssociateAppraisals = SqlMapperUtil.StoredProcWithParams<Appraisals>("sp_CommissionGetAssociateAppraisalsByDate", new { startDate = reportDate, endDate = reportEndDate }, "ReynoldsData");
            //completeTradeReportModel.AssociateOpportunities = SqlMapperUtil.StoredProcWithParams<Opportunities>("sp_CommissionGetAssociateOpportunitiesByDate", new { startDate = reportDate, endDate = reportEndDate }, "ReynoldsData");

            var associateAppraisals = SqlMapperUtil.StoredProcWithParams<AppraisalDetail>("sp_CommissionGetAssociateAppraisalDetails", new { StartDate = reportDate.Date, EndDate = reportEndDate.Date }, "ReynoldsData");
            var associateOpportunities = SqlMapperUtil.StoredProcWithParams<OpportunityDetail>("sp_CommissionGetAssociateOpportunityDetails", new { StartDate = reportDate.Date, EndDate = reportEndDate.Date }, "ReynoldsData");

            completeTradeReportModel.AssociateAppraisals = associateAppraisals;
            completeTradeReportModel.AssociateOpportunities = associateOpportunities;

            completeTradeReportModel.AssociateUserGroups = SqlMapperUtil.StoredProcWithParams<AssociateUserGroups>("sp_CommissionGetAssociateUserGroups", new { }, "ReynoldsData");
            completeTradeReportModel.StoreUserGroups = SqlMapperUtil.StoredProcWithParams<AssociateUserGroups>("sp_CommissionGetUserGroups", new { }, "ReynoldsData");
            

            completeTradeReportModel.MonthId = tradeReportModel.MonthId;
            completeTradeReportModel.YearId = tradeReportModel.YearId;
            completeTradeReportModel.SelectedStores = tradeReportModel.SelectedStores;
            completeTradeReportModel.SelectedBrands = tradeReportModel.SelectedBrands;
            completeTradeReportModel.IncludeHandyman = includeHandyman;
            completeTradeReportModel.IncludeDeals = tradeReportModel.IncludeDeals;

            return completeTradeReportModel;

        }

        public static MonthlySalesLogReportModel GetMonthlySalesReportByDate(MonthlySalesLogReportModel salesLogReportModel, bool includeHandyman)
        {

            var completeSalesLogReportModel = new MonthlySalesLogReportModel();
            var reportDate = new DateTime(salesLogReportModel.YearId, salesLogReportModel.MonthId, 1);
            var salesReportDetails = SqlMapperUtil.StoredProcWithParams<SalesReportDetail>("sp_SalesLogReportByDate", new { ReportDate = reportDate, IncludeHandyman = includeHandyman }, "SalesCommission");

            var leaseCounts = SqlMapperUtil.StoredProcWithParams<SalesReportDetail>("sp_SalesLogReportLeaseCountByDate", new { ReportDate = reportDate, IncludeHandyman = includeHandyman }, "SalesCommission");
            //BPP Collection Percentage
            var certifiedCounts = SqlMapperUtil.StoredProcWithParams<CertifiedCount>("sp_SalesLogReportCertifiedCountByDate", new { ReportDate = reportDate }, "SalesCommission");

            var bppCollection = SqlMapperUtil.StoredProcWithParams<BPPCollection>("sp_SalesLogGetBPPCollection", new { ReportDate = reportDate.ToShortDateString() }, "JJFServerFOX");

            completeSalesLogReportModel.MonthId = salesLogReportModel.MonthId;
            completeSalesLogReportModel.YearId = salesLogReportModel.YearId;
            completeSalesLogReportModel.SelectedStores = salesLogReportModel.SelectedStores;
            completeSalesLogReportModel.SelectedBrands = salesLogReportModel.SelectedBrands;
            completeSalesLogReportModel.IncludeHandyman = includeHandyman;

            foreach (var detail in salesReportDetails)
            {
                detail.TotalAmount = detail.DealGrossAmount + detail.FinIncAmount + detail.VSCAmount + detail.GapAmount + detail.MCAmount; //detail.OtherAmount + detail.FTDAmount
                detail.VariancePercentage = (double)detail.VarianceCount / (double)detail.DealCount;

                if (detail.DealCount > 0 && detail.TotalAmount != 0)
                {
                    detail.PVRAmount = (double)detail.TotalAmount / (double)detail.DealCount;
                }

                var firstOfMonth = new DateTime(completeSalesLogReportModel.YearId, completeSalesLogReportModel.MonthId, 1);
                var lastOfMonth = new DateTime();
                lastOfMonth = firstOfMonth.AddMonths(1);

                var sqlJoin = " FROM dbo.mall mall INNER JOIN dbo.make make ON mall.id = make.mall_id INNER JOIN saleslog ON make.id = saleslog.sl_make_id WHERE mall.automall='" + detail.AutoMall + "' AND (saleslog.sl_dealmonth > '" + firstOfMonth + "'  AND saleslog.sl_dealmonth < '" + lastOfMonth + "' )";
                var sqlGroup = " Group by saleslog.sl_make_id , make , automall";

                var sqlHoldbackCount = "SELECT count([saleslog].[sl_dealGross]) as cntDeals " + sqlJoin + " AND ( [saleslog].[sl_dxHoldback] IS NULL OR [saleslog].[sl_dxHoldback] = 0 ) AND saleslog.sl_make_id=" + detail.MakeId + " AND saleslog.sl_valInternet IN ('V','V2') " + sqlGroup;
                var holdbackCount = SqlMapperUtil.SqlWithParams<int>(sqlHoldbackCount, null, "SalesCommission");

                var sqlHoldbackAmount = "SELECT sum( [saleslog].[sl_dxHoldback] ) as sumHoldback " + sqlJoin + " AND ( [saleslog].[sl_dxHoldback] IS NOT NULL OR [saleslog].[sl_dxHoldback] <> 0 )  AND saleslog.sl_make_id=" + detail.MakeId + sqlGroup;
                var holdbackAmount = SqlMapperUtil.SqlWithParams<decimal>(sqlHoldbackAmount, null, "SalesCommission");

                if (holdbackCount.Count > 0)
                {
                    detail.HoldbackCount = holdbackCount[0];
                    detail.ValuePercentage = (double)holdbackCount[0] / (double)detail.DealCount;
                }
                else
                {
                    detail.HoldbackCount = 0;
                    detail.ValuePercentage = 0;
                }

                //Set the lease counts...

                if (leaseCounts != null && leaseCounts.Count > 0)
                {
                    var leases = leaseCounts.Find(o => o.MakeId == detail.MakeId);
                    if (leases != null)
                    {
                        detail.LeaseCount = leases.LeaseCount;
                    }
                }
                var locationCode = "";
                //Determine Location Code
                foreach (var location in Enums.StoreLocations)
                {
                    if (location.StoreId.ToLower() == detail.AutoMall.ToLower())
                    {
                        locationCode = location.LocationId;
                    }

                }

                var locationBPP = new List<BPPCollection>();
                if (locationBPP != null)
                {
                    if (detail.BrandId.ToUpper() == "UU")
                    {
                        locationBPP = bppCollection.FindAll(o => o.Location == locationCode && o.ConditionAtSold == "U");
                    }
                    else
                    {
                        try
                        {
                            locationBPP = bppCollection.FindAll(o => o.Location == locationCode && o.ConditionAtSold == "N" && o.VehMake.ToUpper().Contains(detail.MakeName.ToUpper()));
                        }
                        catch(Exception ex)
                        {
                            locationBPP = new List<BPPCollection>();
                        }
                    }
                }

                //Set the BPP Collection percentage
                //var locationBPP = bppCollection.FindAll(o => o.Location == locationCode);

                double totalListAmount = 0;
                double totalListCollected = 0;

                foreach (var locBPP in locationBPP)
                {
                    totalListAmount += locBPP.BPPListAmt;
                    totalListCollected += locBPP.CollectedAmt;
                }

                detail.BPPCollectionPercent = totalListCollected / totalListAmount;

                if (detail.BrandId.ToUpper() != "UU")// || detail.MakeId == "87")
                {

                    if (certifiedCounts != null && certifiedCounts.Count > 0)
                    {
                        try
                        {
                            var makeName = "";

                            makeName = detail.MakeName.ToUpper();
                            detail.CPOCount = 0;


                            var cpoListCount = certifiedCounts.FindAll(o => o.CertifiedMake.ToUpper().Contains(makeName) && o.AutoMall == detail.AutoMall);

                            if (detail.MakeId == "41")
                            {
                                cpoListCount = certifiedCounts.FindAll(o => o.CertifiedMake.ToUpper().Contains(makeName) || o.CertifiedMake.ToUpper().Contains("RAM") || o.CertifiedMake.ToUpper().Contains("DODGE"));
                            }

                            if (cpoListCount != null && cpoListCount.Count > 0)
                            {
                                foreach (var cpoCount in cpoListCount)
                                {
                                    detail.CPOCount += cpoCount.CertifiedMakeCount;
                                }

                            }
                        }
                        catch (Exception ex)
                        {
                            detail.CPOCount = 0;
                        }

                    }
                }



            }

            completeSalesLogReportModel.SalesReportDetails = salesReportDetails;

            return completeSalesLogReportModel;
        }

        public static List<DealDetail> GetSalesLogDealsByStoreAndDate(string makeId, int yearId, int monthId)
        {
            var reportDate = new DateTime(yearId, monthId, 1);
            var dealDetails = new List<DealDetail>();

            var makeIdLocation = "";

            //If there is a comma, we are getting multiple makes...
            if (makeId.Contains(","))
            {
                var makeIds = makeId.Split(',');
                foreach(var make in makeIds)
                {
                    var tempDetails = SqlMapperUtil.StoredProcWithParams<DealDetail>("sp_SalesLogDealsByDateAndStore", new { AutoMallID = make, ReportDate = reportDate }, "SalesCommission");
                    foreach(var detail in tempDetails)
                    {
                        dealDetails.Add(detail);
                    }
                    makeIdLocation = make;
                }

            }
            else
            {
                dealDetails = SqlMapperUtil.StoredProcWithParams<DealDetail>("sp_SalesLogDealsByDateAndStore", new { AutoMallID = makeId, ReportDate = reportDate }, "SalesCommission");
                makeIdLocation = makeId;
            }

            //var otherDetails = SqlMapperUtil.StoredProcWithParams<DealDetail>("sp_AllDealDetailsByDate", new { ReportDate = reportDate }, "ReynoldsData");
            var locationCode = GetLocationCodeByMakeId(makeIdLocation);
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

            var associates = GetSalesAssociatesByStore(dealLocation);
            var allAssociates = GetSalesAssociates();

            var dealKeys = "";
            foreach (var deal in dealDetails)
            {
                dealKeys = dealKeys + "'" + deal.DealKey + "',";
            }

            dealKeys = dealKeys.TrimEnd(',');

            var DealHistoryList = GetSalesLogDealHistoryListByDealKeys(dealKeys);

            foreach (var deal in dealDetails)
            {
                
                deal.ReportDate = reportDate;
                if (deal.CustomerName == null || deal.CustomerName == "")
                {
                    deal.CustomerName = deal.BuyerName;

                }

                if (deal.SalesAssociate1 != null && deal.SalesAssociate1 != "")
                {
                    var associate = associates.Find(o => o.Value == deal.SalesAssociate1);
                    if (associate != null)
                    {
                        deal.SalesAssociate1 = associate.Text;
                    }
                    else
                    {
                        var otherAssociate = allAssociates.Find(o => o.Value == deal.SalesAssociate1);
                        if (otherAssociate != null)
                        {
                            deal.SalesAssociate1 = otherAssociate.Text;
                        }
                    }
                }

                if (deal.SalesAssociate2 != null && deal.SalesAssociate2 != "")
                {
                    var associate = associates.Find(o => o.Value == deal.SalesAssociate2);
                    if (associate != null)
                    {
                        deal.SalesAssociate2 = associate.Text;
                    }
                    else
                    {
                        var otherAssociate = allAssociates.Find(o => o.Value == deal.SalesAssociate1);
                        if (otherAssociate != null)
                        {
                            deal.SalesAssociate1 = otherAssociate.Text;
                        }
                    }
                }

                if ((deal.OfficeValidatedBy != null && deal.OfficeValidatedBy != "") && (deal.ShowroomValidatedBy != null && deal.ShowroomValidatedBy != ""))
                {

                    var dealTotal = deal.DealGrossAmount +
                        deal.VSCAmount +
                        deal.MaintenanceContractAmount +
                        deal.GapAmount +
                        deal.FinIncAmount +
                        deal.BankFee;

                    //var dealHistory = SqlQueries.GetSalesLogDealHistoryByDealKey(deal.DealKey);

                    var dealHistory = DealHistoryList.FindAll(x => x.sl_dealkey == deal.DealKey);

                    if (dealHistory.Count > 1)
                    {
                        var showroomValidated = dealHistory[1];

                        var previousDealTotal = showroomValidated.sl_dealGross +
                                    showroomValidated.sl_serviceContract +
                                    showroomValidated.sl_maintenanceContract +
                                    showroomValidated.sl_gap +
                                    showroomValidated.sl_financeInc +
                                    showroomValidated.sl_bankFee;

                        deal.DealVariance = (decimal)dealTotal - previousDealTotal;
                    }
                }

            }

            return dealDetails;
        }

        public static IndividualDeal GetSalesLogDealByDealKey(string dealKey)
        {
            var dealDetails = SqlMapperUtil.StoredProcWithParams<IndividualDeal>("sp_SalesLogDealDetailByDealKey", new { DealKey = dealKey }, "SalesCommission");

            if(dealDetails != null && dealDetails.Count > 0)
            {            
                if(dealDetails[0].sl_CustomerName == null || dealDetails[0].sl_CustomerName == "")
                {
                    dealDetails[0].sl_CustomerName = dealDetails[0].BuyerName;
//                    dealDetails[0].sl_SalesAssociate1 = dealDetails[0].SalesAssociate1;
//                   dealDetails[0].sl_SalesAssociate2 = dealDetails[0].SalesAssociate2;
                }

                if (dealDetails[0].sl_rate_exception != null && dealDetails[0].sl_rate_exception != "")
                {
                    dealDetails[0].sl_rate_exception = dealDetails[0].sl_rate_exception.Trim();
                }
                return dealDetails[0];
            }
            else
            {
                return null;
            }

        }

        public static List<IndividualDealDetails> GetSalesLogDealHistoryByDealKey(string dealKey)
        {
            var dealHistory = SqlMapperUtil.SqlWithParams<IndividualDealDetails>("Select * from SalesLog_History where sl_dealkey = @DealKey order by sl_historykey desc", new { DealKey = dealKey }, "SalesCommission");
            return dealHistory;
        }

        public static List<IndividualDealDetails> GetSalesLogDealHistoryListByDealKeys(string dealKeys)
        {
            var dealHistory = SqlMapperUtil.SqlWithParams<IndividualDealDetails>("Select * from SalesLog_History where sl_dealkey in (" + dealKeys + ") order by sl_historykey desc", null, "SalesCommission");
            return dealHistory;
        }

        public static AftermarketReportModel GetAftermarketReportByDate(AftermarketReportModel aftermarketReportModel, bool includeHandyman)
        {

            var completeAftermarketReportModel = new AftermarketReportModel();
            var reportDate = new DateTime(aftermarketReportModel.YearId, aftermarketReportModel.MonthId, 1);
            var aftermarketRecords = SqlMapperUtil.StoredProcWithParams<AftermarketRecord>("sp_SalesLogAftermarketReportByDate", new { ReportDate = reportDate, @IncludeHandyman = includeHandyman }, "SalesCommission");

            if (aftermarketRecords != null && aftermarketRecords.Count > 0)
            {


                var aftermarketDealGroups = new List<AftermarketDealGroup>();

                var aftermarketDealGroup = new AftermarketDealGroup();
                aftermarketDealGroup.AftermarketDealDetails = new List<AftermarketDealDetail>();

                var autoMall = "";
                autoMall = aftermarketRecords[0].AutoMall;

                foreach (var aftermarketRecord in aftermarketRecords)
                {
                    var aftermarketDealDetail = new AftermarketDealDetail();

                    aftermarketDealDetail.MakeId = aftermarketRecord.MakeId;
                    aftermarketDealDetail.MakeName = aftermarketRecord.MakeName;
                    aftermarketDealDetail.BrandId = aftermarketRecord.BrandId;
                    aftermarketDealDetail.ModelName = aftermarketRecord.ModelName;
                    aftermarketDealDetail.DealKey = aftermarketRecord.DealKey;
                    aftermarketDealDetail.DealGrossAmount = aftermarketRecord.DealGrossAmount;
                    aftermarketDealDetail.BPPAmount = aftermarketRecord.BPPAmount;
                    aftermarketDealDetail.NitrogenAmount = aftermarketRecord.NitrogenAmount;
                    aftermarketDealDetail.ZurichAmount = aftermarketRecord.ZurichAmount;
                    aftermarketDealDetail.TireWheelAmount = aftermarketRecord.TireWheelAmount;
                    aftermarketDealDetail.SecurityAmount = aftermarketRecord.SecurityAmount;

                    aftermarketDealDetail.AdjustmentAmount = aftermarketRecord.AdjustmentAmount;
                    aftermarketDealDetail.CertFeeAmount = aftermarketRecord.CertFeeAmount;
                    aftermarketDealDetail.GAPAmount = aftermarketRecord.GAPAmount;
                    aftermarketDealDetail.VSCAmount = aftermarketRecord.VSCAmount;
                    aftermarketDealDetail.FinanceIncomeAmount = aftermarketRecord.FinanceIncomeAmount;
                    aftermarketDealDetail.MaintenanceAmount = aftermarketRecord.MaintenanceAmount;

                    aftermarketDealDetail.OtherAmount = aftermarketRecord.OtherAmount;
                    aftermarketDealDetail.Loaner = aftermarketRecord.Loaner;
                    aftermarketDealDetail.BPP = aftermarketRecord.BPP;


                    aftermarketDealDetail.SalesAssociate1 = aftermarketRecord.SalesAssociate1;
                    aftermarketDealDetail.SalesAssociate2 = aftermarketRecord.SalesAssociate2;

                    aftermarketDealDetail.FandIManager = aftermarketRecord.FandIManager;

                    var allAssociates = SqlQueries.GetSalesAssociates();

                    if(aftermarketRecord.SalesAssociate1 != null && aftermarketRecord.SalesAssociate1 != "")
                    {
                        var associate1 = allAssociates.Find(x => x.Value == aftermarketRecord.SalesAssociate1);

                        if(associate1 != null && associate1.Text != null)
                        {
                            aftermarketDealDetail.SalesAssociate1 = associate1.Text;
                        }
                    }

                    if (aftermarketRecord.SalesAssociate2 != null && aftermarketRecord.SalesAssociate2 != "")
                    {
                        var associate2 = allAssociates.Find(x => x.Value == aftermarketRecord.SalesAssociate2);

                        if (associate2 != null && associate2.Text != null)
                        {
                            aftermarketDealDetail.SalesAssociate2 = associate2.Text;
                        }
                    }




                    var aftermarketItems = new List<AftermarketItem>();

                    if (aftermarketRecord.AFTP1 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD1;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP1;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST1;
                        aItem.AftermarketId = 1;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP2 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD2;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP2;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST2;
                        aItem.AftermarketId = 2;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP3 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD3;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP3;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST3;
                        aItem.AftermarketId = 3;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP4 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD4;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP4;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST4;
                        aItem.AftermarketId = 4;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP5 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD5;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP5;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST5;
                        aItem.AftermarketId = 5;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP6 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD6;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP6;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST6;
                        aItem.AftermarketId = 6;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP7 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD7;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP7;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST7;
                        aItem.AftermarketId = 7;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP8 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD8;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP8;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST8;
                        aItem.AftermarketId = 8;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP9 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD9;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP9;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST9;
                        aItem.AftermarketId = 9;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP10 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD10;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP10;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST10;
                        aItem.AftermarketId = 10;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP11 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD11;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP11;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST11;
                        aItem.AftermarketId = 11;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP12 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD12;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP12;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST12;
                        aItem.AftermarketId = 12;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP13 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD13;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP13;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST13;
                        aItem.AftermarketId = 13;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP14 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD14;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP14;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST14;
                        aItem.AftermarketId = 14;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP15 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD15;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP15;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST15;
                        aItem.AftermarketId = 15;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP16 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD16;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP16;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST16;
                        aItem.AftermarketId = 16;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP17 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD17;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP17;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST17;
                        aItem.AftermarketId = 17;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP18 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD18;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP18;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST18;
                        aItem.AftermarketId = 18;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP19 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD19;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP19;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST19;
                        aItem.AftermarketId = 19;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP20 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD20;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP20;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST20;
                        aItem.AftermarketId = 20;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP21 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD21;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP21;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST21;
                        aItem.AftermarketId = 21;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP22 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD22;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP22;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST22;
                        aItem.AftermarketId = 22;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP23 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD23;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP23;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST23;
                        aItem.AftermarketId = 23;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP24 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD24;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP24;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST24;
                        aItem.AftermarketId = 24;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP25 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD25;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP25;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST25;
                        aItem.AftermarketId = 25;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP26 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD26;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP26;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST26;
                        aItem.AftermarketId = 26;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP27 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD27;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP27;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST27;
                        aItem.AftermarketId = 27;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP28 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD28;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP28;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST28;
                        aItem.AftermarketId = 28;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP29 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD29;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP29;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST29;
                        aItem.AftermarketId = 29;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP30 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD30;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP30;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST30;
                        aItem.AftermarketId = 30;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP31 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD31;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP31;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST31;
                        aItem.AftermarketId = 31;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP32 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD32;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP32;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST32;
                        aItem.AftermarketId = 32;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP33 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD33;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP33;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST33;
                        aItem.AftermarketId = 33;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP34 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD34;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP34;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST34;
                        aItem.AftermarketId = 34;

                        aftermarketItems.Add(aItem);
                    }

                    aftermarketDealDetail.AftermarketItems = aftermarketItems;

                    // We have and aftermarket deal detail, needs to be added to proper group....

                    if (aftermarketRecord.AutoMall != autoMall)
                    {
                        //aftermarketDealGroup.AftermarketDealDetails.Add(aftermarketDealDetail);

                        aftermarketDealGroups.Add(aftermarketDealGroup);
                        autoMall = aftermarketRecord.AutoMall;

                        aftermarketDealGroup = new AftermarketDealGroup();
                        aftermarketDealGroup.AftermarketDealDetails = new List<AftermarketDealDetail>();
                    }

                    aftermarketDealGroup.AutoMall = aftermarketRecord.AutoMall;
                    aftermarketDealGroup.AftermarketDealDetails.Add(aftermarketDealDetail);

                }

                //aftermarketDealGroup.AftermarketDealDetails.Add(aftermarketDealDetail);
                aftermarketDealGroups.Add(aftermarketDealGroup);


                completeAftermarketReportModel.AftermarketDealGroups = aftermarketDealGroups;
            }

            completeAftermarketReportModel.MonthId = aftermarketReportModel.MonthId;
            completeAftermarketReportModel.YearId = aftermarketReportModel.YearId;
            completeAftermarketReportModel.ConditionId = aftermarketReportModel.ConditionId;
            completeAftermarketReportModel.SelectedStores = aftermarketReportModel.SelectedStores;
            completeAftermarketReportModel.SelectedBrands = aftermarketReportModel.SelectedBrands;
            completeAftermarketReportModel.IncludeHandyman = includeHandyman;
            completeAftermarketReportModel.IncludeDeals = aftermarketReportModel.IncludeDeals;

            return completeAftermarketReportModel;
        }

        public static FICommissionModel GetFIManagerDealsByDate(FICommissionModel FICommissionModel)
        {
            var completeFICommissionModel = new FICommissionModel();
            var reportDate = new DateTime(FICommissionModel.YearId, FICommissionModel.MonthId, 1);

            var FIManagers = SqlMapperUtil.StoredProcWithParams<Associate>("sp_CommissionGetFIManagersByLocation", new { Location = FICommissionModel.StoreId }, "SalesCommission");
            completeFICommissionModel.FIManagers = FIManagers;

            var aftermarketRecords = SqlMapperUtil.StoredProcWithParams<AftermarketRecord>("sp_CommissionGetFIManagerDealsByDateAndLocation", new { ReportDate = reportDate, @IncludeHandyman = true, DealLocation = FICommissionModel.StoreId }, "SalesCommission");

            if (aftermarketRecords != null && aftermarketRecords.Count > 0)
            {

                var aftermarketDealDetails = new List<AftermarketDealDetail>();

                foreach (var aftermarketRecord in aftermarketRecords)
                {
                    var aftermarketDealDetail = new AftermarketDealDetail();

                    aftermarketDealDetail.MakeId = aftermarketRecord.MakeId;
                    aftermarketDealDetail.MakeName = aftermarketRecord.MakeName;
                    aftermarketDealDetail.BrandId = aftermarketRecord.BrandId;
                    aftermarketDealDetail.ModelName = aftermarketRecord.ModelName;


                    aftermarketDealDetail.VehicleCondition = aftermarketRecord.VehicleCondition;
                    aftermarketDealDetail.VehicleMake = aftermarketRecord.VehicleMake;
                    aftermarketDealDetail.VehicleYear = aftermarketRecord.VehicleYear;
                    aftermarketDealDetail.VehicleCarline = aftermarketRecord.VehicleCarline;
                    aftermarketDealDetail.VehicleModelNumber = aftermarketRecord.VehicleModelNumber;
                    aftermarketDealDetail.VehicleStockNumber = aftermarketRecord.VehicleStockNumber;
                    aftermarketDealDetail.VehicleVIN = aftermarketRecord.VehicleVIN;
                    aftermarketDealDetail.VehicleDaysInStock = aftermarketRecord.VehicleDaysInStock;
                    aftermarketDealDetail.CertificationLevel = aftermarketRecord.CertificationLevel;
                    aftermarketDealDetail.VehicleMiles = aftermarketRecord.VehicleMiles;


                    aftermarketDealDetail.DealKey = aftermarketRecord.DealKey;
                    aftermarketDealDetail.DealGrossAmount = aftermarketRecord.DealGrossAmount;
                    aftermarketDealDetail.BPPAmount = aftermarketRecord.BPPAmount;
                    aftermarketDealDetail.NitrogenAmount = aftermarketRecord.NitrogenAmount;
                    aftermarketDealDetail.ZurichAmount = aftermarketRecord.ZurichAmount;
                    aftermarketDealDetail.TireWheelAmount = aftermarketRecord.TireWheelAmount;
                    aftermarketDealDetail.SecurityAmount = aftermarketRecord.SecurityAmount;

                    aftermarketDealDetail.AdjustmentAmount = aftermarketRecord.AdjustmentAmount;
                    aftermarketDealDetail.CertFeeAmount = aftermarketRecord.CertFeeAmount;
                    aftermarketDealDetail.GAPAmount = aftermarketRecord.GAPAmount;
                    aftermarketDealDetail.VSCAmount = aftermarketRecord.VSCAmount;
                    aftermarketDealDetail.FinanceIncomeAmount = aftermarketRecord.FinanceIncomeAmount;
                    aftermarketDealDetail.MaintenanceAmount = aftermarketRecord.MaintenanceAmount;

                    aftermarketDealDetail.OtherAmount = aftermarketRecord.OtherAmount;
                    aftermarketDealDetail.Loaner = aftermarketRecord.Loaner;
                    aftermarketDealDetail.BPP = aftermarketRecord.BPP;


                    aftermarketDealDetail.SalesAssociate1 = aftermarketRecord.SalesAssociate1;
                    aftermarketDealDetail.SalesAssociate2 = aftermarketRecord.SalesAssociate2;

                    aftermarketDealDetail.FandIManager = aftermarketRecord.FandIManager;

                    var allAssociates = SqlQueries.GetSalesAssociates();

                    if (aftermarketRecord.SalesAssociate1 != null && aftermarketRecord.SalesAssociate1 != "")
                    {
                        var associate1 = allAssociates.Find(x => x.Value == aftermarketRecord.SalesAssociate1);

                        if (associate1 != null && associate1.Text != null)
                        {
                            aftermarketDealDetail.SalesAssociate1 = associate1.Text;
                        }
                    }

                    if (aftermarketRecord.SalesAssociate2 != null && aftermarketRecord.SalesAssociate2 != "")
                    {
                        var associate2 = allAssociates.Find(x => x.Value == aftermarketRecord.SalesAssociate2);

                        if (associate2 != null && associate2.Text != null)
                        {
                            aftermarketDealDetail.SalesAssociate2 = associate2.Text;
                        }
                    }

                    var aftermarketItems = new List<AftermarketItem>();

                    if (aftermarketRecord.AFTP1 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD1;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP1;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST1;
                        aItem.AftermarketId = 1;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP2 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD2;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP2;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST2;
                        aItem.AftermarketId = 2;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP3 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD3;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP3;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST3;
                        aItem.AftermarketId = 3;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP4 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD4;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP4;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST4;
                        aItem.AftermarketId = 4;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP5 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD5;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP5;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST5;
                        aItem.AftermarketId = 5;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP6 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD6;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP6;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST6;
                        aItem.AftermarketId = 6;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP7 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD7;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP7;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST7;
                        aItem.AftermarketId = 7;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP8 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD8;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP8;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST8;
                        aItem.AftermarketId = 8;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP9 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD9;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP9;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST9;
                        aItem.AftermarketId = 9;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP10 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD10;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP10;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST10;
                        aItem.AftermarketId = 10;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP11 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD11;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP11;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST11;
                        aItem.AftermarketId = 11;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP12 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD12;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP12;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST12;
                        aItem.AftermarketId = 12;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP13 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD13;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP13;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST13;
                        aItem.AftermarketId = 13;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP14 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD14;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP14;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST14;
                        aItem.AftermarketId = 14;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP15 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD15;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP15;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST15;
                        aItem.AftermarketId = 15;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP16 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD16;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP16;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST16;
                        aItem.AftermarketId = 16;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP17 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD17;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP17;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST17;
                        aItem.AftermarketId = 17;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP18 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD18;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP18;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST18;
                        aItem.AftermarketId = 18;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP19 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD19;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP19;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST19;
                        aItem.AftermarketId = 19;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP20 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD20;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP20;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST20;
                        aItem.AftermarketId = 20;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP21 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD21;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP21;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST21;
                        aItem.AftermarketId = 21;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP22 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD22;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP22;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST22;
                        aItem.AftermarketId = 22;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP23 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD23;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP23;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST23;
                        aItem.AftermarketId = 23;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP24 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD24;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP24;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST24;
                        aItem.AftermarketId = 24;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP25 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD25;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP25;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST25;
                        aItem.AftermarketId = 25;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP26 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD26;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP26;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST26;
                        aItem.AftermarketId = 26;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP27 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD27;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP27;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST27;
                        aItem.AftermarketId = 27;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP28 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD28;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP28;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST28;
                        aItem.AftermarketId = 28;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP29 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD29;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP29;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST29;
                        aItem.AftermarketId = 29;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP30 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD30;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP30;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST30;
                        aItem.AftermarketId = 30;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP31 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD31;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP31;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST31;
                        aItem.AftermarketId = 31;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP32 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD32;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP32;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST32;
                        aItem.AftermarketId = 32;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP33 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD33;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP33;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST33;
                        aItem.AftermarketId = 33;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP34 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD34;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP34;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST34;
                        aItem.AftermarketId = 34;

                        aftermarketItems.Add(aItem);
                    }

                    aftermarketDealDetail.AftermarketItems = aftermarketItems;

                    aftermarketDealDetails.Add(aftermarketDealDetail);

                }


                completeFICommissionModel.AftermarketDealDetails = aftermarketDealDetails;
            }

            completeFICommissionModel.MonthId = FICommissionModel.MonthId;
            completeFICommissionModel.YearId = FICommissionModel.YearId;            
            completeFICommissionModel.StoreId = FICommissionModel.StoreId;


            return completeFICommissionModel;
        }


        public static AftermarketReportModel GetAftermarketFIReportByDate(AftermarketReportModel aftermarketReportModel, bool includeHandyman)
        {

            var completeAftermarketReportModel = new AftermarketReportModel();
            var reportDate = new DateTime(aftermarketReportModel.YearId, aftermarketReportModel.MonthId, 1);
            var aftermarketRecords = SqlMapperUtil.StoredProcWithParams<AftermarketRecord>("sp_SalesLogAftermarketReportByDate", new { ReportDate = reportDate, @IncludeHandyman = includeHandyman }, "SalesCommission");

            if (aftermarketRecords != null && aftermarketRecords.Count > 0)
            {


                var aftermarketDealGroups = new List<AftermarketDealGroup>();

                var aftermarketDealGroup = new AftermarketDealGroup();
                aftermarketDealGroup.AftermarketDealDetails = new List<AftermarketDealDetail>();

                var autoMall = "";
                autoMall = aftermarketRecords[0].AutoMall;

                foreach (var aftermarketRecord in aftermarketRecords)
                {
                    var aftermarketDealDetail = new AftermarketDealDetail();

                    aftermarketDealDetail.MakeId = aftermarketRecord.MakeId;
                    aftermarketDealDetail.MakeName = aftermarketRecord.MakeName;
                    aftermarketDealDetail.BrandId = aftermarketRecord.BrandId;
                    aftermarketDealDetail.ModelName = aftermarketRecord.ModelName;
                    aftermarketDealDetail.DealKey = aftermarketRecord.DealKey;
                    aftermarketDealDetail.DealGrossAmount = aftermarketRecord.DealGrossAmount;
                    aftermarketDealDetail.BPPAmount = aftermarketRecord.BPPAmount;
                    aftermarketDealDetail.NitrogenAmount = aftermarketRecord.NitrogenAmount;
                    aftermarketDealDetail.ZurichAmount = aftermarketRecord.ZurichAmount;
                    aftermarketDealDetail.TireWheelAmount = aftermarketRecord.TireWheelAmount;
                    aftermarketDealDetail.SecurityAmount = aftermarketRecord.SecurityAmount;

                    aftermarketDealDetail.AdjustmentAmount = aftermarketRecord.AdjustmentAmount;
                    aftermarketDealDetail.CertFeeAmount = aftermarketRecord.CertFeeAmount;
                    aftermarketDealDetail.GAPAmount = aftermarketRecord.GAPAmount;
                    aftermarketDealDetail.VSCAmount = aftermarketRecord.VSCAmount;
                    aftermarketDealDetail.FinanceIncomeAmount = aftermarketRecord.FinanceIncomeAmount;
                    aftermarketDealDetail.MaintenanceAmount = aftermarketRecord.MaintenanceAmount;

                    aftermarketDealDetail.OtherAmount = aftermarketRecord.OtherAmount;
                    aftermarketDealDetail.Loaner = aftermarketRecord.Loaner;
                    aftermarketDealDetail.BPP = aftermarketRecord.BPP;


                    aftermarketDealDetail.SalesAssociate1 = aftermarketRecord.SalesAssociate1;
                    aftermarketDealDetail.SalesAssociate2 = aftermarketRecord.SalesAssociate2;

                    aftermarketDealDetail.FandIManager = aftermarketRecord.FandIManager;

                    var allAssociates = SqlQueries.GetSalesAssociates();

                    if (aftermarketRecord.SalesAssociate1 != null && aftermarketRecord.SalesAssociate1 != "")
                    {
                        var associate1 = allAssociates.Find(x => x.Value == aftermarketRecord.SalesAssociate1);

                        if (associate1 != null && associate1.Text != null)
                        {
                            aftermarketDealDetail.SalesAssociate1 = associate1.Text;
                        }
                    }

                    if (aftermarketRecord.SalesAssociate2 != null && aftermarketRecord.SalesAssociate2 != "")
                    {
                        var associate2 = allAssociates.Find(x => x.Value == aftermarketRecord.SalesAssociate2);

                        if (associate2 != null && associate2.Text != null)
                        {
                            aftermarketDealDetail.SalesAssociate2 = associate2.Text;
                        }
                    }




                    var aftermarketItems = new List<AftermarketItem>();

                    if (aftermarketRecord.AFTP1 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD1;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP1;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST1;
                        aItem.AftermarketId = 1;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP2 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD2;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP2;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST2;
                        aItem.AftermarketId = 2;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP3 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD3;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP3;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST3;
                        aItem.AftermarketId = 3;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP4 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD4;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP4;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST4;
                        aItem.AftermarketId = 4;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP5 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD5;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP5;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST5;
                        aItem.AftermarketId = 5;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP6 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD6;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP6;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST6;
                        aItem.AftermarketId = 6;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP7 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD7;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP7;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST7;
                        aItem.AftermarketId = 7;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP8 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD8;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP8;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST8;
                        aItem.AftermarketId = 8;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP9 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD9;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP9;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST9;
                        aItem.AftermarketId = 9;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP10 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD10;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP10;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST10;
                        aItem.AftermarketId = 10;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP11 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD11;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP11;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST11;
                        aItem.AftermarketId = 11;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP12 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD12;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP12;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST12;
                        aItem.AftermarketId = 12;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP13 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD13;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP13;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST13;
                        aItem.AftermarketId = 13;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP14 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD14;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP14;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST14;
                        aItem.AftermarketId = 14;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP15 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD15;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP15;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST15;
                        aItem.AftermarketId = 15;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP16 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD16;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP16;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST16;
                        aItem.AftermarketId = 16;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP17 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD17;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP17;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST17;
                        aItem.AftermarketId = 17;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP18 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD18;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP18;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST18;
                        aItem.AftermarketId = 18;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP19 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD19;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP19;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST19;
                        aItem.AftermarketId = 19;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP20 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD20;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP20;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST20;
                        aItem.AftermarketId = 20;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP21 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD21;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP21;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST21;
                        aItem.AftermarketId = 21;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP22 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD22;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP22;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST22;
                        aItem.AftermarketId = 22;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP23 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD23;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP23;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST23;
                        aItem.AftermarketId = 23;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP24 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD24;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP24;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST24;
                        aItem.AftermarketId = 24;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP25 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD25;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP25;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST25;
                        aItem.AftermarketId = 25;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP26 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD26;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP26;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST26;
                        aItem.AftermarketId = 26;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP27 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD27;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP27;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST27;
                        aItem.AftermarketId = 27;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP28 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD28;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP28;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST28;
                        aItem.AftermarketId = 28;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP29 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD29;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP29;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST29;
                        aItem.AftermarketId = 29;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP30 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD30;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP30;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST30;
                        aItem.AftermarketId = 30;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP31 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD31;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP31;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST31;
                        aItem.AftermarketId = 31;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP32 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD32;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP32;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST32;
                        aItem.AftermarketId = 32;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP33 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD33;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP33;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST33;
                        aItem.AftermarketId = 33;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP34 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD34;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP34;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST34;
                        aItem.AftermarketId = 34;

                        aftermarketItems.Add(aItem);
                    }

                    aftermarketDealDetail.AftermarketItems = aftermarketItems;

                    // We have and aftermarket deal detail, needs to be added to proper group....

                    if (aftermarketRecord.AutoMall != autoMall)
                    {
                        //aftermarketDealGroup.AftermarketDealDetails.Add(aftermarketDealDetail);

                        aftermarketDealGroups.Add(aftermarketDealGroup);
                        autoMall = aftermarketRecord.AutoMall;

                        aftermarketDealGroup = new AftermarketDealGroup();
                        aftermarketDealGroup.AftermarketDealDetails = new List<AftermarketDealDetail>();
                    }

                    aftermarketDealGroup.AutoMall = aftermarketRecord.AutoMall;
                    aftermarketDealGroup.AftermarketDealDetails.Add(aftermarketDealDetail);

                }

                //aftermarketDealGroup.AftermarketDealDetails.Add(aftermarketDealDetail);
                aftermarketDealGroups.Add(aftermarketDealGroup);


                completeAftermarketReportModel.AftermarketDealGroups = aftermarketDealGroups;
            }

            completeAftermarketReportModel.MonthId = aftermarketReportModel.MonthId;
            completeAftermarketReportModel.YearId = aftermarketReportModel.YearId;
            completeAftermarketReportModel.ConditionId = aftermarketReportModel.ConditionId;
            completeAftermarketReportModel.StoreId = aftermarketReportModel.StoreId;
            completeAftermarketReportModel.SelectedStores = aftermarketReportModel.SelectedStores;
            completeAftermarketReportModel.SelectedBrands = aftermarketReportModel.SelectedBrands;
            completeAftermarketReportModel.IncludeHandyman = includeHandyman;
            completeAftermarketReportModel.IncludeDeals = aftermarketReportModel.IncludeDeals;

            return completeAftermarketReportModel;
        }
        public static AftermarketReportModel GetAftermarketReportByDateAndStore(AftermarketReportModel aftermarketReportModel, bool includeHandyman, string associateNumber = "")
        {
            var autoMall = aftermarketReportModel.StoreId;
            var completeAftermarketReportModel = new AftermarketReportModel();
            var reportDate = new DateTime(aftermarketReportModel.YearId, aftermarketReportModel.MonthId, 1);

            var aftermarketRecords = new List<AftermarketRecord>();
            if (associateNumber == "")
            {
                aftermarketRecords = SqlMapperUtil.StoredProcWithParams<AftermarketRecord>("sp_SalesLogAftermarketReportByDateAndStore", new { Automall = autoMall, ReportDate = reportDate, @IncludeHandyman = includeHandyman }, "SalesCommission");
            }
            else
            {
                aftermarketRecords = SqlMapperUtil.StoredProcWithParams<AftermarketRecord>("sp_SalesLogAftermarketReportByDateAndAssociate", new { AssociateNumber = associateNumber, ReportDate = reportDate, @IncludeHandyman = includeHandyman }, "SalesCommission");
            }

            if (aftermarketRecords != null && aftermarketRecords.Count > 0)
            {

                // Get a distinct list of all associates Ids, then loop through that list and get their deals...
                var associate1Ids = aftermarketRecords.Select(x => x.SalesAssociate1).Distinct().ToList();
                associate1Ids.RemoveAll(x => string.IsNullOrWhiteSpace(x));
                        
                var associate2Ids = aftermarketRecords.Select(x => x.SalesAssociate2).Distinct().ToList();
                associate2Ids.RemoveAll(x => string.IsNullOrWhiteSpace(x));

                var allAssoicateIds = associate1Ids.Union(associate2Ids).ToList();

                if (associateNumber != "")
                {
                    allAssoicateIds.RemoveAll(x => x != associateNumber);
                }

                var aftermarketDealGroups = new List<AftermarketDealGroup>();

                foreach (var associateID in allAssoicateIds)
                {

                    var aftermarketDealGroup = new AftermarketDealGroup();
                    aftermarketDealGroup.AftermarketDealDetails = new List<AftermarketDealDetail>();

                    var associateName = "";
                    try
                    {
                        associateName = aftermarketRecords.Find(x => x.SalesAssociate1 == associateID).SalesAssociateFullName;
                    }
                    catch
                    {
                        associateName = aftermarketRecords.Find(x => x.SalesAssociate2 == associateID).SalesAssociateFullName;
                    }
                    aftermarketDealGroup.AutoMall = associateName; // NEED TO PULL IN THE ASSOCIATE NAME

                    var associateRecords = aftermarketRecords.FindAll(x => x.SalesAssociate1 == associateID || x.SalesAssociate2 == associateID);

                    foreach (var aftermarketRecord in associateRecords)
                    {
                        var aftermarketDealDetail = new AftermarketDealDetail();

                        aftermarketDealDetail.MakeId = aftermarketRecord.MakeId;
                        aftermarketDealDetail.MakeName = aftermarketRecord.MakeName;
                        aftermarketDealDetail.BrandId = aftermarketRecord.BrandId;
                        aftermarketDealDetail.ModelName = aftermarketRecord.ModelName;
                        aftermarketDealDetail.DealKey = aftermarketRecord.DealKey;
                        aftermarketDealDetail.DealGrossAmount = aftermarketRecord.DealGrossAmount;
                        aftermarketDealDetail.BPPAmount = aftermarketRecord.BPPAmount;
                        aftermarketDealDetail.NitrogenAmount = aftermarketRecord.NitrogenAmount;
                        aftermarketDealDetail.ZurichAmount = aftermarketRecord.ZurichAmount;
                        aftermarketDealDetail.TireWheelAmount = aftermarketRecord.TireWheelAmount;
                        aftermarketDealDetail.SecurityAmount = aftermarketRecord.SecurityAmount;

                        aftermarketDealDetail.AdjustmentAmount = aftermarketRecord.AdjustmentAmount;
                        aftermarketDealDetail.CertFeeAmount = aftermarketRecord.CertFeeAmount;
                        aftermarketDealDetail.GAPAmount = aftermarketRecord.GAPAmount;
                        aftermarketDealDetail.VSCAmount = aftermarketRecord.VSCAmount;
                        aftermarketDealDetail.FinanceIncomeAmount = aftermarketRecord.FinanceIncomeAmount;
                        aftermarketDealDetail.MaintenanceAmount = aftermarketRecord.MaintenanceAmount;

                        aftermarketDealDetail.OtherAmount = aftermarketRecord.OtherAmount;
                        aftermarketDealDetail.Loaner = aftermarketRecord.Loaner;
                        aftermarketDealDetail.BPP = aftermarketRecord.BPP;

                        var aftermarketItems = new List<AftermarketItem>();

                        if (aftermarketRecord.AFTP1 > 0)
                        {
                            var aItem = new AftermarketItem();
                            aItem.AftermarketName = aftermarketRecord.AFTD1;
                            aItem.AftermarketPrice = aftermarketRecord.AFTP1;
                            aItem.AftermarketCost = aftermarketRecord.AFTCOST1;
                            aItem.AftermarketId = 1;

                            aftermarketItems.Add(aItem);
                        }

                        if (aftermarketRecord.AFTP2 > 0)
                        {
                            var aItem = new AftermarketItem();
                            aItem.AftermarketName = aftermarketRecord.AFTD2;
                            aItem.AftermarketPrice = aftermarketRecord.AFTP2;
                            aItem.AftermarketCost = aftermarketRecord.AFTCOST2;
                            aItem.AftermarketId = 2;

                            aftermarketItems.Add(aItem);
                        }

                        if (aftermarketRecord.AFTP3 > 0)
                        {
                            var aItem = new AftermarketItem();
                            aItem.AftermarketName = aftermarketRecord.AFTD3;
                            aItem.AftermarketPrice = aftermarketRecord.AFTP3;
                            aItem.AftermarketCost = aftermarketRecord.AFTCOST3;
                            aItem.AftermarketId = 3;

                            aftermarketItems.Add(aItem);
                        }

                        if (aftermarketRecord.AFTP4 > 0)
                        {
                            var aItem = new AftermarketItem();
                            aItem.AftermarketName = aftermarketRecord.AFTD4;
                            aItem.AftermarketPrice = aftermarketRecord.AFTP4;
                            aItem.AftermarketCost = aftermarketRecord.AFTCOST4;
                            aItem.AftermarketId = 4;

                            aftermarketItems.Add(aItem);
                        }

                        if (aftermarketRecord.AFTP5 > 0)
                        {
                            var aItem = new AftermarketItem();
                            aItem.AftermarketName = aftermarketRecord.AFTD5;
                            aItem.AftermarketPrice = aftermarketRecord.AFTP5;
                            aItem.AftermarketCost = aftermarketRecord.AFTCOST5;
                            aItem.AftermarketId = 5;

                            aftermarketItems.Add(aItem);
                        }

                        if (aftermarketRecord.AFTP6 > 0)
                        {
                            var aItem = new AftermarketItem();
                            aItem.AftermarketName = aftermarketRecord.AFTD6;
                            aItem.AftermarketPrice = aftermarketRecord.AFTP6;
                            aItem.AftermarketCost = aftermarketRecord.AFTCOST6;
                            aItem.AftermarketId = 6;

                            aftermarketItems.Add(aItem);
                        }

                        if (aftermarketRecord.AFTP7 > 0)
                        {
                            var aItem = new AftermarketItem();
                            aItem.AftermarketName = aftermarketRecord.AFTD7;
                            aItem.AftermarketPrice = aftermarketRecord.AFTP7;
                            aItem.AftermarketCost = aftermarketRecord.AFTCOST7;
                            aItem.AftermarketId = 7;

                            aftermarketItems.Add(aItem);
                        }

                        if (aftermarketRecord.AFTP8 > 0)
                        {
                            var aItem = new AftermarketItem();
                            aItem.AftermarketName = aftermarketRecord.AFTD8;
                            aItem.AftermarketPrice = aftermarketRecord.AFTP8;
                            aItem.AftermarketCost = aftermarketRecord.AFTCOST8;
                            aItem.AftermarketId = 8;

                            aftermarketItems.Add(aItem);
                        }

                        if (aftermarketRecord.AFTP9 > 0)
                        {
                            var aItem = new AftermarketItem();
                            aItem.AftermarketName = aftermarketRecord.AFTD9;
                            aItem.AftermarketPrice = aftermarketRecord.AFTP9;
                            aItem.AftermarketCost = aftermarketRecord.AFTCOST9;
                            aItem.AftermarketId = 9;

                            aftermarketItems.Add(aItem);
                        }

                        if (aftermarketRecord.AFTP10 > 0)
                        {
                            var aItem = new AftermarketItem();
                            aItem.AftermarketName = aftermarketRecord.AFTD10;
                            aItem.AftermarketPrice = aftermarketRecord.AFTP10;
                            aItem.AftermarketCost = aftermarketRecord.AFTCOST10;
                            aItem.AftermarketId = 10;

                            aftermarketItems.Add(aItem);
                        }

                        if (aftermarketRecord.AFTP11 > 0)
                        {
                            var aItem = new AftermarketItem();
                            aItem.AftermarketName = aftermarketRecord.AFTD11;
                            aItem.AftermarketPrice = aftermarketRecord.AFTP11;
                            aItem.AftermarketCost = aftermarketRecord.AFTCOST11;
                            aItem.AftermarketId = 11;

                            aftermarketItems.Add(aItem);
                        }

                        if (aftermarketRecord.AFTP12 > 0)
                        {
                            var aItem = new AftermarketItem();
                            aItem.AftermarketName = aftermarketRecord.AFTD12;
                            aItem.AftermarketPrice = aftermarketRecord.AFTP12;
                            aItem.AftermarketCost = aftermarketRecord.AFTCOST12;
                            aItem.AftermarketId = 12;

                            aftermarketItems.Add(aItem);
                        }

                        if (aftermarketRecord.AFTP13 > 0)
                        {
                            var aItem = new AftermarketItem();
                            aItem.AftermarketName = aftermarketRecord.AFTD13;
                            aItem.AftermarketPrice = aftermarketRecord.AFTP13;
                            aItem.AftermarketCost = aftermarketRecord.AFTCOST13;
                            aItem.AftermarketId = 13;

                            aftermarketItems.Add(aItem);
                        }

                        if (aftermarketRecord.AFTP14 > 0)
                        {
                            var aItem = new AftermarketItem();
                            aItem.AftermarketName = aftermarketRecord.AFTD14;
                            aItem.AftermarketPrice = aftermarketRecord.AFTP14;
                            aItem.AftermarketCost = aftermarketRecord.AFTCOST14;
                            aItem.AftermarketId = 14;

                            aftermarketItems.Add(aItem);
                        }

                        if (aftermarketRecord.AFTP15 > 0)
                        {
                            var aItem = new AftermarketItem();
                            aItem.AftermarketName = aftermarketRecord.AFTD15;
                            aItem.AftermarketPrice = aftermarketRecord.AFTP15;
                            aItem.AftermarketCost = aftermarketRecord.AFTCOST15;
                            aItem.AftermarketId = 15;

                            aftermarketItems.Add(aItem);
                        }

                        if (aftermarketRecord.AFTP16 > 0)
                        {
                            var aItem = new AftermarketItem();
                            aItem.AftermarketName = aftermarketRecord.AFTD16;
                            aItem.AftermarketPrice = aftermarketRecord.AFTP16;
                            aItem.AftermarketCost = aftermarketRecord.AFTCOST16;
                            aItem.AftermarketId = 16;

                            aftermarketItems.Add(aItem);
                        }

                        if (aftermarketRecord.AFTP17 > 0)
                        {
                            var aItem = new AftermarketItem();
                            aItem.AftermarketName = aftermarketRecord.AFTD17;
                            aItem.AftermarketPrice = aftermarketRecord.AFTP17;
                            aItem.AftermarketCost = aftermarketRecord.AFTCOST17;
                            aItem.AftermarketId = 17;

                            aftermarketItems.Add(aItem);
                        }

                        if (aftermarketRecord.AFTP18 > 0)
                        {
                            var aItem = new AftermarketItem();
                            aItem.AftermarketName = aftermarketRecord.AFTD18;
                            aItem.AftermarketPrice = aftermarketRecord.AFTP18;
                            aItem.AftermarketCost = aftermarketRecord.AFTCOST18;
                            aItem.AftermarketId = 18;

                            aftermarketItems.Add(aItem);
                        }

                        if (aftermarketRecord.AFTP19 > 0)
                        {
                            var aItem = new AftermarketItem();
                            aItem.AftermarketName = aftermarketRecord.AFTD19;
                            aItem.AftermarketPrice = aftermarketRecord.AFTP19;
                            aItem.AftermarketCost = aftermarketRecord.AFTCOST19;
                            aItem.AftermarketId = 19;

                            aftermarketItems.Add(aItem);
                        }

                        if (aftermarketRecord.AFTP20 > 0)
                        {
                            var aItem = new AftermarketItem();
                            aItem.AftermarketName = aftermarketRecord.AFTD20;
                            aItem.AftermarketPrice = aftermarketRecord.AFTP20;
                            aItem.AftermarketCost = aftermarketRecord.AFTCOST20;
                            aItem.AftermarketId = 20;

                            aftermarketItems.Add(aItem);
                        }

                        if (aftermarketRecord.AFTP21 > 0)
                        {
                            var aItem = new AftermarketItem();
                            aItem.AftermarketName = aftermarketRecord.AFTD21;
                            aItem.AftermarketPrice = aftermarketRecord.AFTP21;
                            aItem.AftermarketCost = aftermarketRecord.AFTCOST21;
                            aItem.AftermarketId = 21;

                            aftermarketItems.Add(aItem);
                        }

                        if (aftermarketRecord.AFTP22 > 0)
                        {
                            var aItem = new AftermarketItem();
                            aItem.AftermarketName = aftermarketRecord.AFTD22;
                            aItem.AftermarketPrice = aftermarketRecord.AFTP22;
                            aItem.AftermarketCost = aftermarketRecord.AFTCOST22;
                            aItem.AftermarketId = 22;

                            aftermarketItems.Add(aItem);
                        }

                        if (aftermarketRecord.AFTP23 > 0)
                        {
                            var aItem = new AftermarketItem();
                            aItem.AftermarketName = aftermarketRecord.AFTD23;
                            aItem.AftermarketPrice = aftermarketRecord.AFTP23;
                            aItem.AftermarketCost = aftermarketRecord.AFTCOST23;
                            aItem.AftermarketId = 23;

                            aftermarketItems.Add(aItem);
                        }

                        if (aftermarketRecord.AFTP24 > 0)
                        {
                            var aItem = new AftermarketItem();
                            aItem.AftermarketName = aftermarketRecord.AFTD24;
                            aItem.AftermarketPrice = aftermarketRecord.AFTP24;
                            aItem.AftermarketCost = aftermarketRecord.AFTCOST24;
                            aItem.AftermarketId = 24;

                            aftermarketItems.Add(aItem);
                        }

                        if (aftermarketRecord.AFTP25 > 0)
                        {
                            var aItem = new AftermarketItem();
                            aItem.AftermarketName = aftermarketRecord.AFTD25;
                            aItem.AftermarketPrice = aftermarketRecord.AFTP25;
                            aItem.AftermarketCost = aftermarketRecord.AFTCOST25;
                            aItem.AftermarketId = 25;

                            aftermarketItems.Add(aItem);
                        }

                        if (aftermarketRecord.AFTP26 > 0)
                        {
                            var aItem = new AftermarketItem();
                            aItem.AftermarketName = aftermarketRecord.AFTD26;
                            aItem.AftermarketPrice = aftermarketRecord.AFTP26;
                            aItem.AftermarketCost = aftermarketRecord.AFTCOST26;
                            aItem.AftermarketId = 26;

                            aftermarketItems.Add(aItem);
                        }

                        if (aftermarketRecord.AFTP27 > 0)
                        {
                            var aItem = new AftermarketItem();
                            aItem.AftermarketName = aftermarketRecord.AFTD27;
                            aItem.AftermarketPrice = aftermarketRecord.AFTP27;
                            aItem.AftermarketCost = aftermarketRecord.AFTCOST27;
                            aItem.AftermarketId = 27;

                            aftermarketItems.Add(aItem);
                        }

                        if (aftermarketRecord.AFTP28 > 0)
                        {
                            var aItem = new AftermarketItem();
                            aItem.AftermarketName = aftermarketRecord.AFTD28;
                            aItem.AftermarketPrice = aftermarketRecord.AFTP28;
                            aItem.AftermarketCost = aftermarketRecord.AFTCOST28;
                            aItem.AftermarketId = 28;

                            aftermarketItems.Add(aItem);
                        }

                        if (aftermarketRecord.AFTP29 > 0)
                        {
                            var aItem = new AftermarketItem();
                            aItem.AftermarketName = aftermarketRecord.AFTD29;
                            aItem.AftermarketPrice = aftermarketRecord.AFTP29;
                            aItem.AftermarketCost = aftermarketRecord.AFTCOST29;
                            aItem.AftermarketId = 29;

                            aftermarketItems.Add(aItem);
                        }

                        if (aftermarketRecord.AFTP30 > 0)
                        {
                            var aItem = new AftermarketItem();
                            aItem.AftermarketName = aftermarketRecord.AFTD30;
                            aItem.AftermarketPrice = aftermarketRecord.AFTP30;
                            aItem.AftermarketCost = aftermarketRecord.AFTCOST30;
                            aItem.AftermarketId = 30;

                            aftermarketItems.Add(aItem);
                        }

                        if (aftermarketRecord.AFTP31 > 0)
                        {
                            var aItem = new AftermarketItem();
                            aItem.AftermarketName = aftermarketRecord.AFTD31;
                            aItem.AftermarketPrice = aftermarketRecord.AFTP31;
                            aItem.AftermarketCost = aftermarketRecord.AFTCOST31;
                            aItem.AftermarketId = 31;

                            aftermarketItems.Add(aItem);
                        }

                        if (aftermarketRecord.AFTP32 > 0)
                        {
                            var aItem = new AftermarketItem();
                            aItem.AftermarketName = aftermarketRecord.AFTD32;
                            aItem.AftermarketPrice = aftermarketRecord.AFTP32;
                            aItem.AftermarketCost = aftermarketRecord.AFTCOST32;
                            aItem.AftermarketId = 32;

                            aftermarketItems.Add(aItem);
                        }

                        if (aftermarketRecord.AFTP33 > 0)
                        {
                            var aItem = new AftermarketItem();
                            aItem.AftermarketName = aftermarketRecord.AFTD33;
                            aItem.AftermarketPrice = aftermarketRecord.AFTP33;
                            aItem.AftermarketCost = aftermarketRecord.AFTCOST33;
                            aItem.AftermarketId = 33;

                            aftermarketItems.Add(aItem);
                        }

                        if (aftermarketRecord.AFTP34 > 0)
                        {
                            var aItem = new AftermarketItem();
                            aItem.AftermarketName = aftermarketRecord.AFTD34;
                            aItem.AftermarketPrice = aftermarketRecord.AFTP34;
                            aItem.AftermarketCost = aftermarketRecord.AFTCOST34;
                            aItem.AftermarketId = 34;

                            aftermarketItems.Add(aItem);
                        }

                        aftermarketDealDetail.AftermarketItems = aftermarketItems;

                        // We have and aftermarket deal detail, needs to be added to proper group....

                        if ((aftermarketRecord.SalesAssociate1 != null && aftermarketRecord.SalesAssociate1 != "") && (aftermarketRecord.SalesAssociate2 != null && aftermarketRecord.SalesAssociate2 != ""))
                        {
                            aftermarketDealDetail.DealValue = .5M;
                        }
                        else
                        {
                            aftermarketDealDetail.DealValue = 1;
                        }

                        //if (aftermarketRecord.SalesAssociate1 != associateId)
                        //{
                        //    //Check and see if there are any Half Deals...


                        //    //aftermarketDealGroup.AftermarketDealDetails.Add(aftermarketDealDetail);

                        //    aftermarketDealGroups.Add(aftermarketDealGroup);
                        //    associateId = aftermarketRecord.SalesAssociate1;

                        //    aftermarketDealGroup = new AftermarketDealGroup();
                        //    aftermarketDealGroup.AftermarketDealDetails = new List<AftermarketDealDetail>();
                        //}

                        
                        aftermarketDealGroup.AftermarketDealDetails.Add(aftermarketDealDetail);

                    }
                    aftermarketDealGroups.Add(aftermarketDealGroup);

                }


                completeAftermarketReportModel.AftermarketDealGroups = aftermarketDealGroups;
            }

            completeAftermarketReportModel.MonthId = aftermarketReportModel.MonthId;
            completeAftermarketReportModel.YearId = aftermarketReportModel.YearId;
            completeAftermarketReportModel.StoreId = aftermarketReportModel.StoreId;
            //completeAftermarketReportModel.SelectedStores = aftermarketReportModel.SelectedStores;
            //completeAftermarketReportModel.SelectedBrands = aftermarketReportModel.SelectedBrands;
            completeAftermarketReportModel.IncludeHandyman = includeHandyman;
            completeAftermarketReportModel.IncludeDeals = aftermarketReportModel.IncludeDeals;

            return completeAftermarketReportModel;
        }


        public static List<AftermarketItem> GetAftermarketItemsByDealKey(string dealKey, DateTime dealMonthYear, bool returnAll = false)
        {
            var location = "";
            var dealNumber = "";

            location = dealKey.Substring(0, 3);
            dealNumber = dealKey.Replace(location, "");

            //Set deal number and location...

            var monthYear = dealMonthYear.Month.ToString() + "/" + dealMonthYear.Year.ToString();
            var aftermarketInputs = SqlMapperUtil.StoredProcWithParams<AftermarketInput>("sp_CommissionGetAftermarketInputsByDate", new { MonthYear = monthYear }, "SalesCommission");

            var aftermarketTable = SqlMapperUtil.SqlWithParams<AftermarketTable>("Select * from aftermarket2 where loc = @Location and DEALNO = @DealNumber", new { Location = location, DealNumber = dealNumber }, "ReynoldsData");
            var aftermarketItems = new List<AftermarketItem>();
            //Now convert the AftermarketTable to a List of AftermarketItem

            if(aftermarketTable != null && aftermarketTable.Count > 0)
            {
                var aftermarketRecord = aftermarketTable[0];

                //if(aftermarketRecord.AFTP1 > 0)
                //{
                //    var aItem = new AftermarketItem();
                //    aItem.AftermarketName = aftermarketRecord.AFTD1;
                //    aItem.AftermarketPrice = aftermarketRecord.AFTP1;
                //    aItem.AftermarketCost = aftermarketRecord.AFTCOST1;
                //    aItem.AftermarketId = 1;

                //    aftermarketItems.Add(aItem);
                //}

                if (returnAll)
                {
                    if (aftermarketRecord.AFTP2 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD2;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP2;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST2;
                        aItem.AftermarketId = 2;

                        aItem.AftermarketPoints = aftermarketInputs.Find(x => x.AftermarketFieldId == aItem.AftermarketId).AftermarketPoints;
                        aItem.AftermarketProfitPerPoint = aftermarketInputs.Find(x => x.AftermarketFieldId == aItem.AftermarketId).AftermarketProfitPerPoint;

                        aftermarketItems.Add(aItem);
                    }

                    if (aftermarketRecord.AFTP3 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD3;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP3;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST3;
                        aItem.AftermarketId = 3;

                        aItem.AftermarketPoints = aftermarketInputs.Find(x => x.AftermarketFieldId == aItem.AftermarketId).AftermarketPoints;
                        aItem.AftermarketProfitPerPoint = aftermarketInputs.Find(x => x.AftermarketFieldId == aItem.AftermarketId).AftermarketProfitPerPoint;

                        aftermarketItems.Add(aItem);
                    }
                }
                if (aftermarketRecord.AFTP4 > 0)
                {
                    var aItem = new AftermarketItem();
                    aItem.AftermarketName = aftermarketRecord.AFTD4;
                    aItem.AftermarketPrice = aftermarketRecord.AFTP4;
                    aItem.AftermarketCost = aftermarketRecord.AFTCOST4;
                    aItem.AftermarketId = 4;

                    aItem.AftermarketPoints = aftermarketInputs.Find(x => x.AftermarketFieldId == aItem.AftermarketId).AftermarketPoints;
                    aItem.AftermarketProfitPerPoint = aftermarketInputs.Find(x => x.AftermarketFieldId == aItem.AftermarketId).AftermarketProfitPerPoint;

                    aftermarketItems.Add(aItem);
                }

                if (returnAll)
                {
                    if (aftermarketRecord.AFTP5 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD5;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP5;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST5;
                        aItem.AftermarketId = 5;

                        aItem.AftermarketPoints = aftermarketInputs.Find(x => x.AftermarketFieldId == aItem.AftermarketId).AftermarketPoints;
                        aItem.AftermarketProfitPerPoint = aftermarketInputs.Find(x => x.AftermarketFieldId == aItem.AftermarketId).AftermarketProfitPerPoint;

                        aftermarketItems.Add(aItem);
                    }
                }

                if (aftermarketRecord.AFTP6 > 0)
                {
                    var aItem = new AftermarketItem();
                    aItem.AftermarketName = aftermarketRecord.AFTD6;
                    aItem.AftermarketPrice = aftermarketRecord.AFTP6;
                    aItem.AftermarketCost = aftermarketRecord.AFTCOST6;
                    aItem.AftermarketId = 6;

                    aItem.AftermarketPoints = aftermarketInputs.Find(x => x.AftermarketFieldId == aItem.AftermarketId).AftermarketPoints;
                    aItem.AftermarketProfitPerPoint = aftermarketInputs.Find(x => x.AftermarketFieldId == aItem.AftermarketId).AftermarketProfitPerPoint;

                    aftermarketItems.Add(aItem);
                }

                if (aftermarketRecord.AFTP7 > 0)
                {
                    var aItem = new AftermarketItem();
                    aItem.AftermarketName = aftermarketRecord.AFTD7;
                    aItem.AftermarketPrice = aftermarketRecord.AFTP7;
                    aItem.AftermarketCost = aftermarketRecord.AFTCOST7;
                    aItem.AftermarketId = 7;

                    aItem.AftermarketPoints = aftermarketInputs.Find(x => x.AftermarketFieldId == aItem.AftermarketId).AftermarketPoints;
                    aItem.AftermarketProfitPerPoint = aftermarketInputs.Find(x => x.AftermarketFieldId == aItem.AftermarketId).AftermarketProfitPerPoint;

                    aftermarketItems.Add(aItem);
                }

                if (aftermarketRecord.AFTP8 > 0)
                {
                    var aItem = new AftermarketItem();
                    aItem.AftermarketName = aftermarketRecord.AFTD8;
                    aItem.AftermarketPrice = aftermarketRecord.AFTP8;
                    aItem.AftermarketCost = aftermarketRecord.AFTCOST8;
                    aItem.AftermarketId = 8;

                    aItem.AftermarketPoints = aftermarketInputs.Find(x => x.AftermarketFieldId == aItem.AftermarketId).AftermarketPoints;
                    aItem.AftermarketProfitPerPoint = aftermarketInputs.Find(x => x.AftermarketFieldId == aItem.AftermarketId).AftermarketProfitPerPoint;

                    aftermarketItems.Add(aItem);
                }

                if (returnAll)
                {
                    if (aftermarketRecord.AFTP9 > 0)
                    {
                        var aItem = new AftermarketItem();
                        aItem.AftermarketName = aftermarketRecord.AFTD9;
                        aItem.AftermarketPrice = aftermarketRecord.AFTP9;
                        aItem.AftermarketCost = aftermarketRecord.AFTCOST9;
                        aItem.AftermarketId = 9;

                        aItem.AftermarketPoints = aftermarketInputs.Find(x => x.AftermarketFieldId == aItem.AftermarketId).AftermarketPoints;
                        aItem.AftermarketProfitPerPoint = aftermarketInputs.Find(x => x.AftermarketFieldId == aItem.AftermarketId).AftermarketProfitPerPoint;

                        aftermarketItems.Add(aItem);
                    }
                }

                if (aftermarketRecord.AFTP10 > 0)
                {
                    var aItem = new AftermarketItem();
                    aItem.AftermarketName = aftermarketRecord.AFTD10;
                    aItem.AftermarketPrice = aftermarketRecord.AFTP10;
                    aItem.AftermarketCost = aftermarketRecord.AFTCOST10;
                    aItem.AftermarketId = 10;

                    aItem.AftermarketPoints = aftermarketInputs.Find(x => x.AftermarketFieldId == aItem.AftermarketId).AftermarketPoints;
                    aItem.AftermarketProfitPerPoint = aftermarketInputs.Find(x => x.AftermarketFieldId == aItem.AftermarketId).AftermarketProfitPerPoint;

                    aftermarketItems.Add(aItem);
                }

                if (aftermarketRecord.AFTP11 > 0)
                {
                    var aItem = new AftermarketItem();
                    aItem.AftermarketName = aftermarketRecord.AFTD11;
                    aItem.AftermarketPrice = aftermarketRecord.AFTP11;
                    aItem.AftermarketCost = aftermarketRecord.AFTCOST11;
                    aItem.AftermarketId = 11;

                    aItem.AftermarketPoints = aftermarketInputs.Find(x => x.AftermarketFieldId == aItem.AftermarketId).AftermarketPoints;
                    aItem.AftermarketProfitPerPoint = aftermarketInputs.Find(x => x.AftermarketFieldId == aItem.AftermarketId).AftermarketProfitPerPoint;

                    aftermarketItems.Add(aItem);
                }

                if (aftermarketRecord.AFTP12 > 0)
                {
                    var aItem = new AftermarketItem();
                    aItem.AftermarketName = aftermarketRecord.AFTD12;
                    aItem.AftermarketPrice = aftermarketRecord.AFTP12;
                    aItem.AftermarketCost = aftermarketRecord.AFTCOST12;
                    aItem.AftermarketId = 12;

                    aItem.AftermarketPoints = aftermarketInputs.Find(x => x.AftermarketFieldId == aItem.AftermarketId).AftermarketPoints;
                    aItem.AftermarketProfitPerPoint = aftermarketInputs.Find(x => x.AftermarketFieldId == aItem.AftermarketId).AftermarketProfitPerPoint;

                    aftermarketItems.Add(aItem);
                }

                if (aftermarketRecord.AFTP13 > 0)
                {
                    var aItem = new AftermarketItem();
                    aItem.AftermarketName = aftermarketRecord.AFTD13;
                    aItem.AftermarketPrice = aftermarketRecord.AFTP13;
                    aItem.AftermarketCost = aftermarketRecord.AFTCOST13;
                    aItem.AftermarketId = 13;

                    aItem.AftermarketPoints = aftermarketInputs.Find(x => x.AftermarketFieldId == aItem.AftermarketId).AftermarketPoints;
                    aItem.AftermarketProfitPerPoint = aftermarketInputs.Find(x => x.AftermarketFieldId == aItem.AftermarketId).AftermarketProfitPerPoint;

                    aftermarketItems.Add(aItem);
                }

                if (aftermarketRecord.AFTP14 > 0)
                {
                    var aItem = new AftermarketItem();
                    aItem.AftermarketName = aftermarketRecord.AFTD14;
                    aItem.AftermarketPrice = aftermarketRecord.AFTP14;
                    aItem.AftermarketCost = aftermarketRecord.AFTCOST14;
                    aItem.AftermarketId = 14;

                    aItem.AftermarketPoints = aftermarketInputs.Find(x => x.AftermarketFieldId == aItem.AftermarketId).AftermarketPoints;
                    aItem.AftermarketProfitPerPoint = aftermarketInputs.Find(x => x.AftermarketFieldId == aItem.AftermarketId).AftermarketProfitPerPoint;

                    aftermarketItems.Add(aItem);
                }

                if (aftermarketRecord.AFTP15 > 0)
                {
                    var aItem = new AftermarketItem();
                    aItem.AftermarketName = aftermarketRecord.AFTD15;
                    aItem.AftermarketPrice = aftermarketRecord.AFTP15;
                    aItem.AftermarketCost = aftermarketRecord.AFTCOST15;
                    aItem.AftermarketId = 15;

                    aItem.AftermarketPoints = aftermarketInputs.Find(x => x.AftermarketFieldId == aItem.AftermarketId).AftermarketPoints;
                    aItem.AftermarketProfitPerPoint = aftermarketInputs.Find(x => x.AftermarketFieldId == aItem.AftermarketId).AftermarketProfitPerPoint;

                    aftermarketItems.Add(aItem);
                }

                if (aftermarketRecord.AFTP16 > 0)
                {
                    var aItem = new AftermarketItem();
                    aItem.AftermarketName = aftermarketRecord.AFTD16;
                    aItem.AftermarketPrice = aftermarketRecord.AFTP16;
                    aItem.AftermarketCost = aftermarketRecord.AFTCOST16;
                    aItem.AftermarketId = 16;

                    aItem.AftermarketPoints = aftermarketInputs.Find(x => x.AftermarketFieldId == aItem.AftermarketId).AftermarketPoints;
                    aItem.AftermarketProfitPerPoint = aftermarketInputs.Find(x => x.AftermarketFieldId == aItem.AftermarketId).AftermarketProfitPerPoint;

                    aftermarketItems.Add(aItem);
                }

                if (aftermarketRecord.AFTP17 > 0)
                {
                    var aItem = new AftermarketItem();
                    aItem.AftermarketName = aftermarketRecord.AFTD17;
                    aItem.AftermarketPrice = aftermarketRecord.AFTP17;
                    aItem.AftermarketCost = aftermarketRecord.AFTCOST17;
                    aItem.AftermarketId = 17;

                    aItem.AftermarketPoints = aftermarketInputs.Find(x => x.AftermarketFieldId == aItem.AftermarketId).AftermarketPoints;
                    aItem.AftermarketProfitPerPoint = aftermarketInputs.Find(x => x.AftermarketFieldId == aItem.AftermarketId).AftermarketProfitPerPoint;
                    aftermarketItems.Add(aItem);
                }

                if (aftermarketRecord.AFTP18 > 0)
                {
                    var aItem = new AftermarketItem();
                    aItem.AftermarketName = aftermarketRecord.AFTD18;
                    aItem.AftermarketPrice = aftermarketRecord.AFTP18;
                    aItem.AftermarketCost = aftermarketRecord.AFTCOST18;
                    aItem.AftermarketId = 18;

                    aItem.AftermarketPoints = aftermarketInputs.Find(x => x.AftermarketFieldId == aItem.AftermarketId).AftermarketPoints;
                    aItem.AftermarketProfitPerPoint = aftermarketInputs.Find(x => x.AftermarketFieldId == aItem.AftermarketId).AftermarketProfitPerPoint;
                    aftermarketItems.Add(aItem);
                }

                if (aftermarketRecord.AFTP19 > 0)
                {
                    var aItem = new AftermarketItem();
                    aItem.AftermarketName = aftermarketRecord.AFTD19;
                    aItem.AftermarketPrice = aftermarketRecord.AFTP19;
                    aItem.AftermarketCost = aftermarketRecord.AFTCOST19;
                    aItem.AftermarketId = 19;

                    aItem.AftermarketPoints = aftermarketInputs.Find(x => x.AftermarketFieldId == aItem.AftermarketId).AftermarketPoints;
                    aItem.AftermarketProfitPerPoint = aftermarketInputs.Find(x => x.AftermarketFieldId == aItem.AftermarketId).AftermarketProfitPerPoint;
                    aftermarketItems.Add(aItem);
                }

                if (aftermarketRecord.AFTP20 > 0)
                {
                    var aItem = new AftermarketItem();
                    aItem.AftermarketName = aftermarketRecord.AFTD20;
                    aItem.AftermarketPrice = aftermarketRecord.AFTP20;
                    aItem.AftermarketCost = aftermarketRecord.AFTCOST20;
                    aItem.AftermarketId = 20;

                    aftermarketItems.Add(aItem);
                }

                if (aftermarketRecord.AFTP21 > 0)
                {
                    var aItem = new AftermarketItem();
                    aItem.AftermarketName = aftermarketRecord.AFTD21;
                    aItem.AftermarketPrice = aftermarketRecord.AFTP21;
                    aItem.AftermarketCost = aftermarketRecord.AFTCOST21;
                    aItem.AftermarketId = 21;

                    aftermarketItems.Add(aItem);
                }

                if (aftermarketRecord.AFTP22 > 0)
                {
                    var aItem = new AftermarketItem();
                    aItem.AftermarketName = aftermarketRecord.AFTD22;
                    aItem.AftermarketPrice = aftermarketRecord.AFTP22;
                    aItem.AftermarketCost = aftermarketRecord.AFTCOST22;
                    aItem.AftermarketId = 22;

                    aftermarketItems.Add(aItem);
                }

                if (aftermarketRecord.AFTP23 > 0)
                {
                    var aItem = new AftermarketItem();
                    aItem.AftermarketName = aftermarketRecord.AFTD23;
                    aItem.AftermarketPrice = aftermarketRecord.AFTP23;
                    aItem.AftermarketCost = aftermarketRecord.AFTCOST23;
                    aItem.AftermarketId = 23;

                    aftermarketItems.Add(aItem);
                }

                if (aftermarketRecord.AFTP24 > 0)
                {
                    var aItem = new AftermarketItem();
                    aItem.AftermarketName = aftermarketRecord.AFTD24;
                    aItem.AftermarketPrice = aftermarketRecord.AFTP24;
                    aItem.AftermarketCost = aftermarketRecord.AFTCOST24;
                    aItem.AftermarketId = 24;

                    aftermarketItems.Add(aItem);
                }

                if (aftermarketRecord.AFTP25 > 0)
                {
                    var aItem = new AftermarketItem();
                    aItem.AftermarketName = aftermarketRecord.AFTD25;
                    aItem.AftermarketPrice = aftermarketRecord.AFTP25;
                    aItem.AftermarketCost = aftermarketRecord.AFTCOST25;
                    aItem.AftermarketId = 25;

                    aftermarketItems.Add(aItem);
                }

                if (aftermarketRecord.AFTP26 > 0)
                {
                    var aItem = new AftermarketItem();
                    aItem.AftermarketName = aftermarketRecord.AFTD26;
                    aItem.AftermarketPrice = aftermarketRecord.AFTP26;
                    aItem.AftermarketCost = aftermarketRecord.AFTCOST26;
                    aItem.AftermarketId = 26;

                    aftermarketItems.Add(aItem);
                }

                if (aftermarketRecord.AFTP27 > 0)
                {
                    var aItem = new AftermarketItem();
                    aItem.AftermarketName = aftermarketRecord.AFTD27;
                    aItem.AftermarketPrice = aftermarketRecord.AFTP27;
                    aItem.AftermarketCost = aftermarketRecord.AFTCOST27;
                    aItem.AftermarketId = 27;

                    aftermarketItems.Add(aItem);
                }

                if (aftermarketRecord.AFTP28 > 0)
                {
                    var aItem = new AftermarketItem();
                    aItem.AftermarketName = aftermarketRecord.AFTD28;
                    aItem.AftermarketPrice = aftermarketRecord.AFTP28;
                    aItem.AftermarketCost = aftermarketRecord.AFTCOST28;
                    aItem.AftermarketId = 28;

                    aftermarketItems.Add(aItem);
                }

                if (aftermarketRecord.AFTP29 > 0)
                {
                    var aItem = new AftermarketItem();
                    aItem.AftermarketName = aftermarketRecord.AFTD29;
                    aItem.AftermarketPrice = aftermarketRecord.AFTP29;
                    aItem.AftermarketCost = aftermarketRecord.AFTCOST29;
                    aItem.AftermarketId = 29;

                    aftermarketItems.Add(aItem);
                }

                if (aftermarketRecord.AFTP30 > 0)
                {
                    var aItem = new AftermarketItem();
                    aItem.AftermarketName = aftermarketRecord.AFTD30;
                    aItem.AftermarketPrice = aftermarketRecord.AFTP30;
                    aItem.AftermarketCost = aftermarketRecord.AFTCOST30;
                    aItem.AftermarketId = 30;

                    aftermarketItems.Add(aItem);
                }

                if (aftermarketRecord.AFTP31 > 0)
                {
                    var aItem = new AftermarketItem();
                    aItem.AftermarketName = aftermarketRecord.AFTD31;
                    aItem.AftermarketPrice = aftermarketRecord.AFTP31;
                    aItem.AftermarketCost = aftermarketRecord.AFTCOST31;
                    aItem.AftermarketId = 31;

                    aftermarketItems.Add(aItem);
                }

                if (aftermarketRecord.AFTP32 > 0)
                {
                    var aItem = new AftermarketItem();
                    aItem.AftermarketName = aftermarketRecord.AFTD32;
                    aItem.AftermarketPrice = aftermarketRecord.AFTP32;
                    aItem.AftermarketCost = aftermarketRecord.AFTCOST32;
                    aItem.AftermarketId = 32;

                    aftermarketItems.Add(aItem);
                }

                if (aftermarketRecord.AFTP33 > 0)
                {
                    var aItem = new AftermarketItem();
                    aItem.AftermarketName = aftermarketRecord.AFTD33;
                    aItem.AftermarketPrice = aftermarketRecord.AFTP33;
                    aItem.AftermarketCost = aftermarketRecord.AFTCOST33;
                    aItem.AftermarketId = 33;

                    aftermarketItems.Add(aItem);
                }

                if (aftermarketRecord.AFTP34 > 0)
                {
                    var aItem = new AftermarketItem();
                    aItem.AftermarketName = aftermarketRecord.AFTD34;
                    aItem.AftermarketPrice = aftermarketRecord.AFTP34;
                    aItem.AftermarketCost = aftermarketRecord.AFTCOST34;
                    aItem.AftermarketId = 34;

                    aftermarketItems.Add(aItem);
                }

            }

            return aftermarketItems;
        }

        public static List<FactoryToDealerCash> GetFTDByStore(MonthlySalesLogReportModel salesLogReportModel)
        {
            
            var factoryToDealerCash = SqlMapperUtil.StoredProcWithParams<FactoryToDealerCash>("sp_SalesLogGetFTDByDate", new { MonthId = salesLogReportModel.MonthId, YearId = salesLogReportModel.YearId}, "SalesCommission");

            return factoryToDealerCash;
        }
        public static List<FactoryToDealerCash> GetFTDByStoreAndDate(SalesLogReportModel salesLogReportModel)
        {
            var locationId = "";

            foreach(var mapping in SalesCommission.Business.Enums.StoreLocations)
                {
                if (mapping.StoreId == salesLogReportModel.StoreId)
                {
                    locationId = mapping.LocationId;
                    break;
                }
            }


            var factoryToDealerCash = SqlMapperUtil.StoredProcWithParams<FactoryToDealerCash>("sp_SalesLogGetFTDByDateAndStore", new {MonthId = salesLogReportModel.MonthId, YearId = salesLogReportModel.YearId, StoreId = locationId}, "SalesCommission");

            return factoryToDealerCash;
        }

        public static int SaveUserPermissions(UserPersmissions userPermissions)
        {      
            int saveObjStn = SqlMapperUtil.InsertUpdateOrDeleteStoredProc("sp_SecuritySaveUserPermissions", userPermissions, "SalesCommission");
            return saveObjStn;
        }

        public static int SaveFactoryToDealerCash(FactoryToDealerCash factoryToDealerCash)
        {

            //Now save everything to the database and save the files...
           int saveObjStn = SqlMapperUtil.InsertUpdateOrDeleteStoredProc("sp_SalesLogSaveFTD", factoryToDealerCash, "SalesCommission");

            return saveObjStn;
        }

        public static int SaveFactoryToDealerCashManagerComments(FactoryToDealerCash factoryToDealerCash)
        {

            //Now save everything to the database and save the files...
            int saveObjStn = SqlMapperUtil.InsertUpdateOrDeleteStoredProc("sp_SalesLogSaveManagerComment", factoryToDealerCash, "SalesCommission");

            return saveObjStn;
        }


        public static int DeleteSalesLogDealByDealKey(string dealKey, string deleteUser, string deleteReason)
        {

            //Now save everything to the database and save the files...
            int deleteDeal = SqlMapperUtil.InsertUpdateOrDeleteStoredProc("sp_SalesLogDeleteDeal", new { DealKey = dealKey, UpdateUser = deleteUser, DeleteReason = deleteReason }, "SalesCommission");

            return deleteDeal;
        }

        public static int SaveSalesLogDealComments(string primaryKey, string dealKey, string commentUser, string comments)
        {
            int saveComments = SqlMapperUtil.InsertUpdateOrDeleteStoredProc("sp_SalesLogSaveDealComments", new {sl_pkey = primaryKey, sl_dealkey = dealKey, sl_commentUser = commentUser, sl_dealComments = comments }, "SalesCommission");
            return saveComments;
        }

        public static DealComments GetSalesLogDealComments(string primaryKey, string dealKey)
        {
            var comment = new DealComments();
            var sqlGet = "Select sl_dealComments as Comment, sl_commentUser as CommentUser, sl_commentDate as CommentDate from saleslog_comments where sl_pkey = '" + primaryKey + "' and sl_dealkey = '" + dealKey + "'";

            var comments = SqlMapperUtil.SqlWithParams<DealComments>(sqlGet, null, "SalesCommission");
            if (comments.Count > 0)
            {
                comment = comments[0];
            }

            return comment;
        }

        public static string GetLocationCodeByStoreId(string storeId)
        {
            var locationCode = "";
            var sqlGet = "Select Top 1 Substring(sl_dealKey, 1, 3) as LocationCode from salesLog S join dbo.mall M on S.sl_mall_id = M.id where M.automall = '" + storeId + "' and S.sl_datecreated > '1/2/2016'";

            var locationCodes = SqlMapperUtil.SqlWithParams<string>(sqlGet, null, "SalesCommission");
            if(locationCodes.Count > 0)
            {
                locationCode = locationCodes[0];
            }

            return locationCode;
        }

        public static string GetLocationCodeByMallId(string mallId)
        {
            var locationCode = "";
            var sqlGet = "Select mallCode as LocationCode from mall where mall.id='" + mallId + "'";

            var locationCodes = SqlMapperUtil.SqlWithParams<string>(sqlGet, null, "SalesCommission");
            if (locationCodes.Count > 0)
            {
                locationCode = locationCodes[0];
            }

            return locationCode;
        }

        public static string GetLocationCodeByMakeId(string makeId)
        {
            var locationCode = "";
            var sqlGet = "Select mallCode as LocationCode from make join mall on make.mall_id = mall.id where make.id='" + makeId + "'";

            var locationCodes = SqlMapperUtil.SqlWithParams<string>(sqlGet, null, "SalesCommission");
            if (locationCodes.Count > 0)
            {
                locationCode = locationCodes[0];
            }

            return locationCode;
        }
        


        public static ObjectivesStandardsModel GetObjectivesAndStandardsByDate(ObjectivesStandardsModel objectivesStandardsModel)
        {
            var objectivesStandards = SqlMapperUtil.StoredProcWithParams<ObjectivesAndStandards>("sp_SalesLogGetObjectivesStandardsByDate",new {YearId = objectivesStandardsModel.YearId, MonthId = objectivesStandardsModel.MonthId}, "SalesCommission");
            objectivesStandardsModel.ObjectivesAndStandards = objectivesStandards;

            var objectiveDocuments = SqlMapperUtil.StoredProcWithParams<SavedDocument>("sp_SalesLogGetObjectiveDocuments", null, "SalesCommission");
            objectivesStandardsModel.SavedDocuments = objectiveDocuments;

            return objectivesStandardsModel;
        }

        public static ObjectivesStandardsModel UpdateObjectivesAndStandardsFromPrevious(ObjectivesStandardsModel objectivesStandardsModel)
        {
            // Take the current date and create new records for the previous month
            var objectivesStandards = SqlMapperUtil.StoredProcWithParams<ObjectivesAndStandards>("sp_UpdateObjectivesStandardsFromPreviousByDate", new { YearId = objectivesStandardsModel.YearId, MonthId = objectivesStandardsModel.MonthId }, "SalesCommission");
            objectivesStandardsModel.ObjectivesAndStandards = objectivesStandards;

            var objectiveDocuments = SqlMapperUtil.StoredProcWithParams<SavedDocument>("sp_SalesLogGetObjectiveDocuments", null, "SalesCommission");
            objectivesStandardsModel.SavedDocuments = objectiveDocuments;

            return objectivesStandardsModel;
        }


        public static ObjectivesStandardsModel SaveObjectivesAndStandards(ObjectivesAndStandards objectivesAndStandards, IEnumerable<HttpPostedFileBase> files)
        {
            var objectivesStandardsModel = new ObjectivesStandardsModel();

            //Now save everything to the database and save the files...
            int saveObjStn = SqlMapperUtil.InsertUpdateOrDeleteStoredProc("sp_SalesLogSaveObjectivesStandards", objectivesAndStandards, "SalesCommission");

            // End database saving

            if (files != null)
            {
                foreach (var file in files)
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        var guid = Guid.NewGuid();
                        var path = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("/uploads"), guid + System.IO.Path.GetExtension(file.FileName));
                        file.SaveAs(path);
                        // Now save this into the database
                        var savedDocument = new SavedDocument();
                        savedDocument.DocumentPath = "/uploads/" + guid + System.IO.Path.GetExtension(file.FileName);
                        savedDocument.DocumentTitle = file.FileName;
                        savedDocument.DocumentMimeType = file.ContentType;
                        savedDocument.DocumentSize = file.ContentLength;
                        savedDocument.LocationId = objectivesAndStandards.LocationId;
                        savedDocument.YearId = objectivesAndStandards.YearId;
                        savedDocument.MonthId = objectivesAndStandards.MonthId;

                        int insertDoc = SqlMapperUtil.InsertUpdateOrDeleteStoredProc("sp_SalesLogSaveObjectiveDocuments", savedDocument, "SalesCommission");

                        // End database saving
                    }
                }
            }

            objectivesStandardsModel.MonthId = objectivesAndStandards.MonthId;
            objectivesStandardsModel.YearId = objectivesAndStandards.YearId;

            objectivesStandardsModel = GetObjectivesAndStandardsByDate(objectivesStandardsModel);

            return objectivesStandardsModel;
        }


        public static List<Status5> GetStatus5CountByLocationAndDate(string locationCode, int yearId, int monthId)
        {
            var reportDate = new DateTime(yearId, monthId, 1);
            var sqlSelect = "SELECT S.Make as MakeName, projname as NewUsedStatus, Count(S.Make) as VehicleCount FROM[JUNK].[dbo].[csv_NUstatus5] S left join (Select distinct Serial_no, stk_no, category, [status] from [REYDATA].[dbo].[RCI_fimaster]) FI on S.SERIALno = FI.serial_no and s.STOCKNO = FI.stk_no WHERE S.loc = '" + locationCode + "' and(SALEDTE >= '" + reportDate.Date + "' and SALEDTE < '" + reportDate.AddMonths(1).Date + "') and FI.category <> 'W' and FI.[status] = 'C' group by S.Make, projname";
            var status5List = SqlMapperUtil.SqlWithParams<Status5>(sqlSelect, null, "SQLServer");

            return status5List;

        }

        public static List<FiscalMonth> GetFiscalMonthByMonthYearLocation(string locationCode, int yearId, int monthId)
        {
            
            var fiscalMonth = SqlMapperUtil.StoredProcWithParams<FiscalMonth>("sp_SalesGetFiscalMonthByMonthYearLocation", new { LocationCode = locationCode, YearId = yearId, monthId = monthId }, "SalesCommission");

            return fiscalMonth;
           
        }

        public static List<FiscalMonth> GetFiscalMonthByMonthYear(int yearId, int monthId)
        {

            var fiscalMonth = SqlMapperUtil.StoredProcWithParams<FiscalMonth>("sp_SalesGetFiscalMonthByMonthYear", new { YearId = yearId, monthId = monthId }, "SalesCommission");

            return fiscalMonth;

        }

        public static List<FIPayscale> GetFIPayscaleByIDAndDate(int yearId, int monthId, string payscaleId)
        {
            var monthYear = monthId + "/" + yearId;
            var FIPayscales = SqlMapperUtil.StoredProcWithParams<FIPayscale>("sp_CommissionGetFIPayscaleByIDAndDate", new { MonthYear = monthYear, PlanCode = payscaleId }, "SalesCommission");

            return FIPayscales;

        }

        public static List<SelectListItem> GetMakes(string mallId)
        {
            var sqlGet = "Select id as MakeId, make as MakeName, mall_id as MallId, makecode from [Make] where on_off <> 'D' and mall_id = '" + mallId + "' order by MallId, Id";
            var makes = SqlMapperUtil.SqlWithParams<Make>(sqlGet, null, "SalesCommission");

            var items = new List<SelectListItem>();

            var blankItem = new SelectListItem();
            blankItem.Text = string.Empty;
            blankItem.Value = string.Empty;
            items.Add(blankItem);

            foreach (var make in makes)
            {
                var item = new SelectListItem();
                item.Text = make.MakeName.Trim();
                item.Value = make.MakeId.Trim();
                items.Add(item);
            }

            return items;
        }

        public static List<SelectListItem> GetFIPayscaleSelectList()
        {
            var sqlGet = "SELECT distinct [PlanCode], [PlanName] FROM [SalesCommission].[dbo].[CommissionFIPayscales]";
            var payscales = SqlMapperUtil.SqlWithParams<FIPayscale>(sqlGet, null, "SalesCommission");

            var items = new List<SelectListItem>();

            var blankItem = new SelectListItem();
            blankItem.Text = string.Empty;
            blankItem.Value = string.Empty;
            items.Add(blankItem);

            foreach (var payscale in payscales)
            {
                var item = new SelectListItem();
                item.Text = payscale.PlanName.Trim();
                item.Value = payscale.PlanCode.Trim();
                items.Add(item);
            }

            return items;
        }

        public static List<SelectListItem> GetMalls()
        {
            var sqlGet = "Select Id as MallId, automall as MallName, mallCode as MallCode from [mall]";
            var malls = SqlMapperUtil.SqlWithParams<Mall>(sqlGet, null, "SalesCommission");

            var items = new List<SelectListItem>();

            var blankItem = new SelectListItem();
            blankItem.Text = string.Empty;
            blankItem.Value = string.Empty;
            items.Add(blankItem);

            foreach (var mall in malls)
            {
                var item = new SelectListItem();
                item.Text = mall.MallName.Trim();
                item.Value = mall.MallId.Trim();
                items.Add(item);
            }

            return items;
        }

        public static List<AssociateLevel> GetAssociateLevelHistory()
        {
            var sqlGet = @"SELECT [sa_ssn] as AssociateSSN
                ,[sa_level] as AssociateCertificationLevel
                ,[sa_monthyear] as MonthYear
                ,[sa_payscale] as AssociatePayscale
                FROM [SalesCommission].[dbo].[comm_slsassoc] where sa_monthYear = '10/2018' or sa_monthYear = '11/2018'";
            var associateLevelHistory = SqlMapperUtil.SqlWithParams<AssociateLevel>(sqlGet, null, "SalesCommission");

            return associateLevelHistory;
        }


        public static List<SelectListItem> GetSalesAssociates()
        {
            var sqlGet = "Select distinct emp_empnumber as AssociateId, emp_lname, (emp_fname + ' ' + emp_Lname) as AssociateName from ivory.dbo.employees where emp_pos = 'SLS ASSOC' or emp_pos = 'SLS MGR' or emp_pos = 'FIN MGR' order by emp_lname";
            var salesAssociates = SqlMapperUtil.SqlWithParams<SalesAssociate>(sqlGet, null, "JJFServer");

            var items = new List<SelectListItem>();

            var blankItem = new SelectListItem();
            blankItem.Text = string.Empty;
            blankItem.Value = string.Empty;
            items.Add(blankItem);

            foreach (var associate in salesAssociates)
            {
                var item = new SelectListItem();
                item.Text = associate.AssociateName.Trim();
                item.Value = associate.AssociateId.Trim();
                items.Add(item);
            }

            var houseItem = new SelectListItem();
            houseItem.Text = "HOUSE";
            houseItem.Value = "99999";
            items.Add(houseItem);

            return items;
        }

        public static List<SelectListItem> GetSalesAssociatesByStore(string locationCode)
        {
            var sqlGet = "Select distinct emp_empnumber as AssociateId, emp_lname, (emp_fname + ' ' + emp_Lname) as AssociateName from ivory.dbo.employees where (emp_pos = 'SLS ASSOC' or emp_pos = 'SLS MGR' or emp_pos = 'FIN MGR') and emp_loc = '" + locationCode + "' order by emp_lname";
            var salesAssociates = SqlMapperUtil.SqlWithParams<SalesAssociate>(sqlGet, null, "JJFServer");

            var items = new List<SelectListItem>();

            var blankItem = new SelectListItem();
            blankItem.Text = string.Empty;
            blankItem.Value = string.Empty;
            items.Add(blankItem);

            foreach (var associate in salesAssociates)
            {
                var item = new SelectListItem();
                item.Text = associate.AssociateName.Trim();
                item.Value = associate.AssociateId.Trim();
                items.Add(item);
            }

            var houseItem = new SelectListItem();
            houseItem.Text = "HOUSE";
            houseItem.Value = "99999";
            items.Add(houseItem);

            return items;
        }


        public static List<SelectListItem> GetFinanceManagers()
        {
            var sqlGet = "select fm_pkey as ManagerId, fm_name as ManagerName, fm_loc from financeManagers order by fm_name";
            var financeManagers = SqlMapperUtil.SqlWithParams<FinanceManager>(sqlGet, null, "JJFServer");

            var items = new List<SelectListItem>();

            var blankItem = new SelectListItem();
            blankItem.Text = string.Empty;
            blankItem.Value = string.Empty;
            items.Add(blankItem);

            foreach (var manager in financeManagers)
            {
                var item = new SelectListItem();
                item.Text = manager.ManagerName;
                item.Value = manager.ManagerId;
                items.Add(item);
            }

            return items;
        }

        public static List<SelectListItem> GetFinanceSources()
        {
            var sqlGet = "select fs_pkey as SourceId, fs_name as SourceName from financeSources order by fs_name";
            var financeSources = SqlMapperUtil.SqlWithParams<FinanceSource>(sqlGet, null, "JJFServer");

            var items = new List<SelectListItem>();

            var blankItem = new SelectListItem();
            blankItem.Text = string.Empty;
            blankItem.Value = string.Empty;
            items.Add(blankItem);

            foreach (var sources in financeSources)
            {
                var item = new SelectListItem();
                item.Text = sources.SourceName;
                item.Value = sources.SourceId;
                items.Add(item);
            }

            return items;
        }

        public static List<SelectListItem> GetServiceCompanies()
        {
            var sqlGet = "SELECT distinct sc_name as CompanyId, sc_name as CompanyName FROM ServiceCompanies";
            var serviceCompanies = SqlMapperUtil.SqlWithParams<ServiceCompany>(sqlGet, null, "JJFServer");

            var items = new List<SelectListItem>();

            var blankItem = new SelectListItem();
            blankItem.Text = string.Empty;
            blankItem.Value = string.Empty;
            items.Add(blankItem);

            foreach (var company in serviceCompanies)
            {
                var item = new SelectListItem();
                item.Text = company.CompanyName;
                item.Value = company.CompanyId;
                items.Add(item);
            }

            return items;
        }



        public static bool UpdateOfficeValidateDealsByDealKeys(List<IndividualDealDetails> validateDeals)
        {
            foreach (var deal in validateDeals)
            {                
                int saveObjStn = SqlMapperUtil.InsertUpdateOrDeleteStoredProc("sp_SalesLogOfficeValidateDeal", new { sl_updatedate = deal.sl_updatedate, sl_updateuser = deal.sl_updateuser, sl_dealkey = deal.sl_dealkey, sl_officeValidatedBy = deal.sl_officeValidatedBy, sl_officeValidatedDate = deal.sl_officeValidatedDate }, "SalesCommission");
            }

            return true;
        }

        public static void SaveSalesLogDeal(IndividualDeal individualDeal)
        {
            var individualDealDetails = new IndividualDealDetails();

            individualDealDetails.sl_pkey = individualDeal.sl_pkey;
            individualDealDetails.sl_dealkey = individualDeal.sl_dealkey;
            individualDealDetails.sl_make_id = individualDeal.sl_make_id;
            individualDealDetails.sl_mall_id = individualDeal.sl_mall_id;
            individualDealDetails.sl_dealmonth = individualDeal.sl_dealmonth;
            individualDealDetails.sl_rate_exception = individualDeal.sl_rate_exception;
            individualDealDetails.sl_price_variance_exception = individualDeal.sl_price_variance_exception;
            individualDealDetails.sl_price_Exception_Comments = individualDeal.sl_price_Exception_Comments;

            //These are just needed to be passed into the Stored Proc, they are not updated, or at least they should not be...
            individualDealDetails.sl_officeValidatedDate = new DateTime(1900, 1, 1);
            individualDealDetails.sl_showroomValidatedDate = new DateTime(1900, 1, 1);
            individualDealDetails.sl_datecreated = new DateTime(1900, 1, 1);
            individualDealDetails.sl_dateupdated = new DateTime(1900, 1, 1);
            individualDealDetails.sl_updatedate = DateTime.Now;

            individualDealDetails.sl_VehicleDealDate = new DateTime(1900, 1, 1);
            individualDealDetails.sl_VehicleDeliveryDate = new DateTime(1900, 1, 1);


            //********************* ADD THIS BACK IN OR YOU WILL NEVER GET OFFICE VALIDATED DEALS...
            int saveObjStn = SqlMapperUtil.InsertUpdateOrDeleteStoredProc("sp_SalesLogSaveDealReasons", individualDealDetails, "SalesCommission");
            //********************* ADD THIS BACK IN OR YOU WILL NEVER GET OFFICE VALIDATED DEALS...

        }
        public static void UpdateSalesLogDeal(IndividualDeal individualDeal, decimal includedTotalPre)
        {
            var individualDealDetails = new IndividualDealDetails();

            individualDealDetails.sl_pkey = individualDeal.sl_pkey;
            individualDealDetails.sl_dealkey = individualDeal.sl_dealkey;
            individualDealDetails.sl_make_id = individualDeal.sl_make_id;
            individualDealDetails.sl_mall_id = individualDeal.sl_mall_id;
            individualDealDetails.sl_DealTotal = individualDeal.sl_DealTotal;
            individualDealDetails.sl_SellPrice = individualDeal.sl_SellPrice;
            individualDealDetails.sl_Adjustments = individualDeal.sl_Adjustments;
            individualDealDetails.sl_CertFee = individualDeal.sl_CertFee;

            var includedTotalPost = Convert.ToDecimal(individualDeal.sl_BPP) + individualDeal.sl_maint + individualDeal.sl_insurance + individualDeal.sl_leaseWnT + individualDeal.sl_etch + individualDeal.sl_Adjustments + individualDeal.sl_CertFee + individualDeal.sl_otheram;

            var difference = includedTotalPost - includedTotalPre;
            var dealGross = individualDeal.sl_dealGross + difference;

            individualDealDetails.sl_dealGross = dealGross;//(individualDeal.sl_DealTotal - individualDeal.sl_financeInc - individualDeal.sl_serviceContract - individualDeal.sl_gap - individualDeal.sl_maintenanceContract - individualDeal.sl_bankFee);
            individualDealDetails.sl_RRDealGross = individualDeal.sl_RRDealGross;
            individualDealDetails.sl_holdback = individualDeal.sl_holdback;
            individualDealDetails.sl_serviceContract = individualDeal.sl_serviceContract;
            individualDealDetails.sl_maintenanceContract = individualDeal.sl_maintenanceContract;
            individualDealDetails.sl_serviceCompany = individualDeal.sl_serviceCompany;
            individualDealDetails.sl_roadHaz = individualDeal.sl_roadHaz;
            individualDealDetails.sl_gap = individualDeal.sl_gap;
            individualDealDetails.sl_insurance = individualDeal.sl_insurance;
            individualDealDetails.sl_etch = individualDeal.sl_etch;
            individualDealDetails.sl_otheram = individualDeal.sl_otheram;
            individualDealDetails.sl_maint = individualDeal.sl_maint;
            individualDealDetails.sl_FinMgr = individualDeal.sl_FinMgr;
            individualDealDetails.sl_leaseWnT = individualDeal.sl_leaseWnT;
            individualDealDetails.sl_financeInc = individualDeal.sl_financeInc;
            individualDealDetails.sl_financeSrc = individualDeal.sl_financeSrc;
            individualDealDetails.sl_bankFee = individualDeal.sl_bankFee;
            individualDealDetails.sl_ftdpc1 = individualDeal.sl_ftdpc1;
            individualDealDetails.sl_ftdpc2 = individualDeal.sl_ftdpc2;
            individualDealDetails.sl_ftdpc3 = individualDeal.sl_ftdpc3;
            individualDealDetails.sl_ftdpc_1 = individualDeal.sl_ftdpc_1;
            individualDealDetails.sl_ftdpc_2 = individualDeal.sl_ftdpc_2;
            individualDealDetails.sl_ftdpc_3 = individualDeal.sl_ftdpc_3;
            individualDealDetails.sl_ftdpcadj = individualDeal.sl_ftdpcadj;
            individualDealDetails.sl_ftdpc_adj = individualDeal.sl_ftdpc_adj;
            individualDealDetails.sl_rebpc1 = individualDeal.sl_rebpc1;
            individualDealDetails.sl_rebpc2 = individualDeal.sl_rebpc2;
            individualDealDetails.sl_rebpc3 = individualDeal.sl_rebpc3;
            individualDealDetails.sl_rebpc4 = individualDeal.sl_rebpc4;
            individualDealDetails.sl_rebpc5 = individualDeal.sl_rebpc5;
            individualDealDetails.sl_rebpc_1 = individualDeal.sl_rebpc_1;
            individualDealDetails.sl_rebpc_2 = individualDeal.sl_rebpc_2;
            individualDealDetails.sl_rebpc_3 = individualDeal.sl_rebpc_3;
            individualDealDetails.sl_rebpc_4 = individualDeal.sl_rebpc_4;
            individualDealDetails.sl_rebpc_5 = individualDeal.sl_rebpc_5;
            individualDealDetails.sl_dealmonth = individualDeal.sl_dealmonth;
            individualDealDetails.sl_descOftrade = individualDeal.sl_descOftrade;
            individualDealDetails.sl_descOftrade2 = individualDeal.sl_descOftrade2;
            individualDealDetails.sl_RepeatBuyer = individualDeal.sl_RepeatBuyer;
            individualDealDetails.sl_ReferralYN = individualDeal.sl_ReferralYN;
            individualDealDetails.sl_Referral = individualDeal.sl_Referral;
            individualDealDetails.sl_appOfVehc = individualDeal.sl_appOfVehc;
            individualDealDetails.sl_cashVal = individualDeal.sl_cashVal;
            individualDealDetails.sl_cashVal2 = individualDeal.sl_cashVal2;
            individualDealDetails.sl_tradeVal = individualDeal.sl_tradeVal;
            individualDealDetails.sl_tradeVal2 = individualDeal.sl_tradeVal2;
            individualDealDetails.sl_valInternet = individualDeal.sl_valInternet;
            individualDealDetails.sl_dxHoldback = individualDeal.sl_dxHoldback;
            individualDealDetails.sl_datecreated = individualDeal.sl_datecreated;
            individualDealDetails.sl_dateupdated = individualDeal.sl_dateupdated;
            individualDealDetails.sl_enteredby = individualDeal.sl_enteredby;
            individualDealDetails.sl_msiYN = individualDeal.sl_msiYN;
            individualDealDetails.sl_handymanYN = individualDeal.sl_handymanYN;
            individualDealDetails.sl_certificationLevel = individualDeal.sl_certificationLevel;
            individualDealDetails.sl_paintRepair = individualDeal.sl_paintRepair;
            individualDealDetails.sl_leadSrc = individualDeal.sl_leadSrc;
            individualDealDetails.sl_AddGross = individualDeal.sl_AddGross;
            individualDealDetails.sl_rate_exception = individualDeal.sl_rate_exception;
            individualDealDetails.sl_price_variance = individualDeal.sl_price_variance;
            individualDealDetails.sl_price_variance_exception = individualDeal.sl_price_variance_exception;
            individualDealDetails.sl_price_Exception_Comments = individualDeal.sl_price_Exception_Comments;
            individualDealDetails.sl_price_Exception_Dealer = individualDeal.sl_price_Exception_Dealer;

            individualDealDetails.sl_buyRate = individualDeal.sl_buyRate;
            individualDealDetails.sl_apr = individualDeal.sl_apr;

            individualDealDetails.sl_CustomerName = individualDeal.sl_CustomerName;
            individualDealDetails.sl_SalesAssociate1 = individualDeal.sl_SalesAssociate1;
            individualDealDetails.sl_SalesAssociate2 = individualDeal.sl_SalesAssociate2;

            if (individualDeal.Validation == "Office Validate")
            {
                individualDealDetails.sl_officeValidatedBy = individualDeal.UpdateUser;
                individualDealDetails.sl_officeValidatedDate = DateTime.Now;
            }
            else
            {
                individualDealDetails.sl_officeValidatedBy = individualDeal.sl_officeValidatedBy;
                individualDealDetails.sl_officeValidatedDate = individualDeal.sl_officeValidatedDate;
            }

            if (individualDeal.Validation == "Showroom Validate")
            {
                individualDealDetails.sl_showroomValidatedBy = individualDeal.UpdateUser;
                individualDealDetails.sl_showroomValidatedDate = DateTime.Now;
                individualDealDetails.sl_officeValidatedBy = "";
                individualDealDetails.sl_officeValidatedDate = new DateTime(1900, 1, 1);

            }
            else
            {
                individualDealDetails.sl_showroomValidatedBy = individualDeal.sl_showroomValidatedBy;
                individualDealDetails.sl_showroomValidatedDate = individualDeal.sl_showroomValidatedDate;
            }

            individualDealDetails.sl_BPP = individualDeal.sl_BPP;
            individualDealDetails.sl_gapCompany = individualDeal.sl_gapCompany;

            individualDealDetails.sl_updatedate = DateTime.Now;
            individualDealDetails.sl_updateuser = individualDeal.UpdateUser;


            //Doing this so the SQL Statement doesn't fail, not actually changing anything...
            individualDealDetails.sl_VehicleDealDate = new DateTime(1900, 1, 1);
            individualDealDetails.sl_VehicleDeliveryDate = new DateTime(1900, 1, 1);

            //Now save everything to the database and save the files...

            //********************* ADD THIS BACK IN OR YOU WILL NEVER GET OFFICE VALIDATED DEALS...
            int saveObjStn = SqlMapperUtil.InsertUpdateOrDeleteStoredProc("sp_SalesLogSaveDeal", individualDealDetails, "SalesCommission");
            //********************* ADD THIS BACK IN OR YOU WILL NEVER GET OFFICE VALIDATED DEALS...

            //If it has been office validated, then send it to commissions...
            if (individualDeal.Validation == "Office Validate")
            {
                var commissionDeal = new CommissionDeal();

                var locationCode = GetLocationCodeByMakeId(individualDeal.sl_make_id);
                var commDealKey = locationCode + individualDeal.sl_dealkey.Remove(0, 3);

                commissionDeal.sl_pkey = Convert.ToDecimal(individualDeal.sl_pkey);
                commissionDeal.sl_dealkey = commDealKey;
                commissionDeal.sl_serviceContract = Convert.ToDecimal(individualDeal.sl_serviceContract);
                commissionDeal.sl_roadHaz = individualDeal.sl_roadHaz;
                commissionDeal.sl_gap = individualDeal.sl_gap;
                commissionDeal.sl_insurance = individualDeal.sl_insurance;
                commissionDeal.sl_etch = individualDeal.sl_etch;
                commissionDeal.sl_otheram = individualDeal.sl_otheram;
                commissionDeal.sl_maint = individualDeal.sl_maint;
                commissionDeal.sl_leaseWnT = individualDeal.sl_leaseWnT;
                commissionDeal.sl_financeInc = individualDeal.sl_financeInc;
                commissionDeal.sl_dealmonth = individualDeal.sl_dealmonth;
                commissionDeal.sl_cashVal = individualDeal.sl_cashVal;
                commissionDeal.sl_cashVal2 = individualDeal.sl_cashVal2;
                commissionDeal.sl_valInternet = individualDeal.sl_valInternet;
                //commissionDeal.sl_msiYN = individualDeal.sl_msiYN;
                //commissionDeal.sl_handymanYN = individualDeal.sl_handymanYN;
                commissionDeal.sl_paintRepair = individualDeal.sl_paintRepair;
                commissionDeal.sl_BPP = individualDeal.sl_BPP;
                commissionDeal.sl_CustomerName = individualDeal.sl_CustomerName;
                commissionDeal.sl_SalesAssociate1 = individualDeal.sl_SalesAssociate1;
                commissionDeal.sl_SalesAssociate2 = individualDeal.sl_SalesAssociate2;
                commissionDeal.sl_maintenanceContract = individualDeal.sl_maintenanceContract;
                commissionDeal.sl_VehicleNU = individualDeal.sl_VehicleNU;
                commissionDeal.sl_VehicleLoc = locationCode;
                commissionDeal.sl_VehicleDealNo = Convert.ToDecimal(individualDeal.sl_VehicleDealNo);
                commissionDeal.sl_VehicleMake = individualDeal.sl_VehicleMake;
                commissionDeal.sl_VehicleYear = individualDeal.sl_VehicleYear;
                commissionDeal.sl_VehicleCarline = individualDeal.sl_VehicleCarline;
                commissionDeal.sl_VehicleModelNumber = individualDeal.sl_VehicleModelNumber;
                commissionDeal.sl_VehicleStockNumber = individualDeal.sl_VehicleStockNumber;
                commissionDeal.sl_VehicleVIN = individualDeal.sl_VehicleVIN;
                commissionDeal.sl_VehicleCustomer = individualDeal.sl_VehicleCustomer;
                commissionDeal.sl_VehicleBuyerLast = individualDeal.sl_VehicleBuyerLast;
                commissionDeal.sl_VehicleBuyerName = individualDeal.sl_VehicleBuyerName;
                commissionDeal.sl_VehicleDeliveryDate = individualDeal.sl_VehicleDeliveryDate;
                commissionDeal.sl_VehicleTerm = individualDeal.sl_VehicleTerm;
                commissionDeal.sl_VehicleDaysInStock = individualDeal.sl_VehicleDaysInStock;
                // commissionDeal.sl_VehicleMfgRebate = individualDeal.sl_VehicleMfgRebate;
                commissionDeal.sl_VehicleCategory = individualDeal.sl_VehicleCategory;
                commissionDeal.sl_VehicleDealDate = individualDeal.sl_VehicleDealDate;
                // commissionDeal.sl_VehicleSalesManager = individualDeal.sl_VehicleSalesManager;
                commissionDeal.sl_tradeVal = individualDeal.sl_tradeVal;
                commissionDeal.sl_tradeVal2 = individualDeal.sl_tradeVal2;



                int saveCommission = SqlMapperUtil.InsertUpdateOrDeleteStoredProc("sp_CommissionImportSalesLogData",commissionDeal, "JJFServer");
            }
        }

        public static void ExportSalesToCommission()
        {
            var dealsOfficeValidated = SqlMapperUtil.SqlWithParams<IndividualDeal>("SELECT * FROM[SalesCommission].[dbo].[saleslog] where sl_dealmonth > '2/1/2018' and sl_officeValidatedBy != '' ", new {  }, "SalesCommission");

            var bFound = false;
            var foundCount = 0;
            var notfoundCount = 0;

            foreach (var individualDeal in dealsOfficeValidated)
            {
                var locationCode = GetLocationCodeByMakeId(individualDeal.sl_make_id);
                var commDealKey = locationCode + individualDeal.sl_dealkey.Remove(0, 3);

                var commDeal = SqlMapperUtil.SqlWithParams<decimal>("Select d_pkey from comm_deals where d_LocDeal = '" + commDealKey + "'", new { }, "JJFServer");

                if(commDeal != null && commDeal.Count > 0)
                {
                    foundCount += 1;
                    bFound = true;
                }
                else
                {
                    notfoundCount += 1;
                    bFound = false;
                }

                if (!bFound)
                {
                    var commissionDeal = new CommissionDeal();
                    commissionDeal.sl_pkey = Convert.ToDecimal(individualDeal.sl_pkey);
                    commissionDeal.sl_dealkey = commDealKey;
                    commissionDeal.sl_serviceContract = Convert.ToDecimal(individualDeal.sl_serviceContract);
                    commissionDeal.sl_roadHaz = individualDeal.sl_roadHaz;
                    commissionDeal.sl_gap = individualDeal.sl_gap;
                    commissionDeal.sl_insurance = individualDeal.sl_insurance;
                    commissionDeal.sl_etch = individualDeal.sl_etch;
                    commissionDeal.sl_otheram = individualDeal.sl_otheram;
                    commissionDeal.sl_maint = individualDeal.sl_maint;
                    commissionDeal.sl_leaseWnT = individualDeal.sl_leaseWnT;
                    commissionDeal.sl_financeInc = individualDeal.sl_financeInc;
                    commissionDeal.sl_dealmonth = individualDeal.sl_dealmonth;
                    commissionDeal.sl_cashVal = individualDeal.sl_cashVal;
                    commissionDeal.sl_cashVal2 = individualDeal.sl_cashVal2;
                    commissionDeal.sl_valInternet = individualDeal.sl_valInternet;
                    //commissionDeal.sl_msiYN = individualDeal.sl_msiYN;
                    //commissionDeal.sl_handymanYN = individualDeal.sl_handymanYN;
                    commissionDeal.sl_paintRepair = individualDeal.sl_paintRepair;
                    commissionDeal.sl_BPP = individualDeal.sl_BPP;
                    commissionDeal.sl_CustomerName = individualDeal.sl_CustomerName;
                    commissionDeal.sl_SalesAssociate1 = individualDeal.sl_SalesAssociate1;
                    commissionDeal.sl_SalesAssociate2 = individualDeal.sl_SalesAssociate2;
                    commissionDeal.sl_maintenanceContract = individualDeal.sl_maintenanceContract;
                    commissionDeal.sl_VehicleNU = individualDeal.sl_VehicleNU;
                    commissionDeal.sl_VehicleLoc = locationCode;
                    commissionDeal.sl_VehicleDealNo = Convert.ToDecimal(individualDeal.sl_VehicleDealNo);
                    commissionDeal.sl_VehicleMake = individualDeal.sl_VehicleMake;
                    commissionDeal.sl_VehicleYear = individualDeal.sl_VehicleYear;
                    commissionDeal.sl_VehicleCarline = individualDeal.sl_VehicleCarline;
                    commissionDeal.sl_VehicleModelNumber = individualDeal.sl_VehicleModelNumber;
                    commissionDeal.sl_VehicleStockNumber = individualDeal.sl_VehicleStockNumber;
                    commissionDeal.sl_VehicleVIN = individualDeal.sl_VehicleVIN;
                    commissionDeal.sl_VehicleCustomer = individualDeal.sl_VehicleCustomer;
                    commissionDeal.sl_VehicleBuyerLast = individualDeal.sl_VehicleBuyerLast;
                    commissionDeal.sl_VehicleBuyerName = individualDeal.sl_VehicleBuyerName;
                    commissionDeal.sl_VehicleDeliveryDate = individualDeal.sl_VehicleDeliveryDate;
                    commissionDeal.sl_VehicleTerm = individualDeal.sl_VehicleTerm;
                    commissionDeal.sl_VehicleDaysInStock = individualDeal.sl_VehicleDaysInStock;
                    // commissionDeal.sl_VehicleMfgRebate = individualDeal.sl_VehicleMfgRebate;
                    commissionDeal.sl_VehicleCategory = individualDeal.sl_VehicleCategory;
                    commissionDeal.sl_VehicleDealDate = individualDeal.sl_VehicleDealDate;
                    // commissionDeal.sl_VehicleSalesManager = individualDeal.sl_VehicleSalesManager;
                    commissionDeal.sl_tradeVal = individualDeal.sl_tradeVal;
                    commissionDeal.sl_tradeVal2 = individualDeal.sl_tradeVal2;

                    int saveCommission = SqlMapperUtil.InsertUpdateOrDeleteStoredProc("sp_CommissionImportSalesLogData", commissionDeal, "JJFServer");
                }
            }

        }

        public static List<ManufacturerSpiff> GetManufacturerSpiffs(int yearId, int monthId)
        {
            var manufacturerSpiffs = new List<ManufacturerSpiff>();
            var monthYear = monthId.ToString() + "/" + yearId.ToString();

            manufacturerSpiffs = SqlMapperUtil.SqlWithParams<ManufacturerSpiff>("SELECT [msg_pkey] as SpiffKey ,[msg_ManuName] as Manufacturer, [msg_ManuDesc] as SpiffPaid, [msg_MonthYear] as MonthYear FROM [comm_ManuSpiffGuar] where msg_MonthYear = '" + monthYear + "' order by msg_ManuName", null, "SalesCommission");

            return manufacturerSpiffs;
        }

        public static ManufacturerSpiffModel UpdateManufacturerSpiffsFromPrevious(ManufacturerSpiffModel manufacturerSpiffModel)
        {
            // Take the current date and create new records for the previous month
            var manufacturerSpiffs = SqlMapperUtil.StoredProcWithParams<ManufacturerSpiff>("sp_UpdateManufacturerSpiffsFromPreviousByDate", new { YearId = manufacturerSpiffModel.YearId, MonthId = manufacturerSpiffModel.MonthId }, "SalesCommission");
            manufacturerSpiffModel.ManufacturerSpiffs = manufacturerSpiffs;

            return manufacturerSpiffModel;
        }

        public static int SaveManufacturerSpiffs(ManufacturerSpiff manufacturerSpiff)
        {

            //Now save everything to the database and save the files...
            int saveInputs = SqlMapperUtil.InsertUpdateOrDeleteStoredProc("sp_CommissionSaveManufacturerSpiffsByDate", manufacturerSpiff, "SalesCommission");

            // End database saving

            return saveInputs;
        }


        public static List<DealDetail> GetDealsByLocationAndCustomerName(string locationCode, string monthId, string yearId, string customerName)
        {

            var reportDate = new DateTime(Int32.Parse(yearId), Int32.Parse(monthId), 1);

            var deals = SqlMapperUtil.StoredProcWithParams<DealDetail>("sp_SalesLogGetDealsByLocationAndCustomerName", new { LocationCode = locationCode, CustomerName = customerName, ReportDate = reportDate.Date }, "SalesCommission");

            //var otherDetails = SqlMapperUtil.StoredProcWithParams<DealDetail>("sp_AllDealDetailsByDate", new { ReportDate = reportDate }, "ReynoldsData");

            var associates = GetSalesAssociates();

            foreach (var deal in deals)
            {
                //var otherDetails = SqlMapperUtil.StoredProcWithParams<DealDetail>("sp_DealDetailsByLocDeal", new { LocDeal = deal.DealKey }, "SalesCommission");
                deal.ReportDate = reportDate;

                if (deal.CustomerName == null || deal.CustomerName == "")
                {
                    deal.CustomerName = deal.BuyerName;
                }

                if(deal.SalesAssociate1 != null && deal.SalesAssociate1 != "")
                {
                    var associate = associates.Find(o => o.Value == deal.SalesAssociate1);
                    if(associate != null)
                    {
                        deal.SalesAssociate1 = associate.Text;
                    }
                }

                if (deal.SalesAssociate2 != null && deal.SalesAssociate2 != "")
                {
                    var associate = associates.Find(o => o.Value == deal.SalesAssociate2);
                    if (associate != null)
                    {
                        deal.SalesAssociate2 = associate.Text;
                    }
                }
                //var otherDeal = otherDetails.Find(o => o.DealKey.Trim() == deal.DealKey.Trim());

                ////if(otherDetails != null && otherDetails.Count > 0)
                //if (otherDeal != null)
                //{ 
                //    //var otherDeal = otherDetails[0];

                //        deal.CustomerName = otherDeal.CustomerName;
                //        deal.SalesAssociate1 = otherDeal.SalesAssociate1;
                //        deal.SalesAssociate2 = otherDeal.SalesAssociate2;
                //        deal.ModelNumber = otherDeal.ModelNumber;
                //        deal.Carline = otherDeal.Carline;
                //        deal.StockNumber = otherDeal.StockNumber;
                //        deal.DealDate = otherDeal.DealDate;
                //        deal.DaysInStock = otherDeal.DaysInStock;
                //        deal.Year = otherDeal.Year;
                //        deal.FinanceManager = otherDeal.FinanceManager;

                //   }
            }

            return deals;
        }

        public static List<DealDetail> GetSalesLogLeaseDeals(string makeId, string monthId, string yearId)
        {
            var reportDate = new DateTime(Int32.Parse(yearId), Int32.Parse(monthId), 1);
            var dealDetails = new List<DealDetail>();

            //If there is a comma, we are getting multiple makes...
            if (makeId.Contains(","))
            {
                var makeIds = makeId.Split(',');
                foreach (var make in makeIds)
                {
                    var tempDetails = SqlMapperUtil.StoredProcWithParams<DealDetail>("sp_SalesLogDealsByDateAndStore", new { AutoMallID = make, ReportDate = reportDate }, "SalesCommission");
                    foreach (var detail in tempDetails)
                    {
                        dealDetails.Add(detail);
                    }
                }

            }
            else
            {
                dealDetails = SqlMapperUtil.StoredProcWithParams<DealDetail>("sp_SalesLogDealsByDateAndStore", new { AutoMallID = makeId, ReportDate = reportDate }, "SalesCommission");
            }

            //var otherDetails = SqlMapperUtil.StoredProcWithParams<DealDetail>("sp_AllDealDetailsByDate", new { ReportDate = reportDate }, "ReynoldsData");
            var associates = GetSalesAssociates();
            var leaseDeals = new List<DealDetail>();

            foreach (var deal in dealDetails)
            {
                deal.ReportDate = reportDate;
                if (deal.CustomerName == null || deal.CustomerName == "")
                {
                    deal.CustomerName = deal.BuyerName;

                }

                if (deal.SalesAssociate1 != null && deal.SalesAssociate1 != "")
                {
                    var associate = associates.Find(o => o.Value == deal.SalesAssociate1);
                    if (associate != null)
                    {
                        deal.SalesAssociate1 = associate.Text;
                    }
                }

                if (deal.SalesAssociate2 != null && deal.SalesAssociate2 != "")
                {
                    var associate = associates.Find(o => o.Value == deal.SalesAssociate2);
                    if (associate != null)
                    {
                        deal.SalesAssociate2 = associate.Text;
                    }
                }

                if (deal.Category != null && deal.Category == "L")
                {
                    if (deal.ShowroomValidatedBy != null && deal.ShowroomValidatedBy != "")
                    {
                        leaseDeals.Add(deal);
                    }
                }

                
                //var otherDeal = otherDetails.Find(o => o.DealKey.Trim() == deal.DealKey.Trim());

                ////if(otherDetails != null && otherDetails.Count > 0)
                //if (otherDeal != null)
                //{
                //    //var otherDeal = otherDetails[0];
                //    if (otherDeal.Category != null && otherDeal.Category == "L")
                //    {                        
                //        deal.CustomerName = otherDeal.CustomerName;
                //        deal.SalesAssociate1 = otherDeal.SalesAssociate1;
                //        deal.SalesAssociate2 = otherDeal.SalesAssociate2;
                //        deal.ModelNumber = otherDeal.ModelNumber;
                //        deal.Carline = otherDeal.Carline;
                //        deal.StockNumber = otherDeal.StockNumber;
                //        deal.DealDate = otherDeal.DealDate;
                //        deal.DaysInStock = otherDeal.DaysInStock;
                //        deal.Year = otherDeal.Year;
                //        deal.FinanceManager = otherDeal.FinanceManager;

                //    }
                //}
            }

            return leaseDeals;
        }

        public static List<DealDetail> GetSalesLogNextCarDeals(string makeId, string monthId, string yearId)
        {
            var reportDate = new DateTime(Int32.Parse(yearId), Int32.Parse(monthId), 1);
            var dealDetails = new List<DealDetail>();

            //If there is a comma, we are getting multiple makes...
            if (makeId.Contains(","))
            {
                var makeIds = makeId.Split(',');
                foreach (var make in makeIds)
                {
                    var tempDetails = SqlMapperUtil.StoredProcWithParams<DealDetail>("sp_SalesLogDealsByDateAndStore", new { AutoMallID = make, ReportDate = reportDate }, "SalesCommission");
                    foreach (var detail in tempDetails)
                    {
                        dealDetails.Add(detail);
                    }
                }

            }
            else
            {
                dealDetails = SqlMapperUtil.StoredProcWithParams<DealDetail>("sp_SalesLogDealsByDateAndStore", new { AutoMallID = makeId, ReportDate = reportDate }, "SalesCommission");
            }

            //var otherDetails = SqlMapperUtil.StoredProcWithParams<DealDetail>("sp_AllDealDetailsByDate", new { ReportDate = reportDate }, "ReynoldsData");
            var associates = GetSalesAssociates();
            var nextCarDeals = new List<DealDetail>();

            foreach (var deal in dealDetails)
            {
                deal.ReportDate = reportDate;
                if (deal.CustomerName == null || deal.CustomerName == "")
                {
                    deal.CustomerName = deal.BuyerName;

                }

                if (deal.SalesAssociate1 != null && deal.SalesAssociate1 != "")
                {
                    var associate = associates.Find(o => o.Value == deal.SalesAssociate1);
                    if (associate != null)
                    {
                        deal.SalesAssociate1 = associate.Text;
                    }
                }

                if (deal.SalesAssociate2 != null && deal.SalesAssociate2 != "")
                {
                    var associate = associates.Find(o => o.Value == deal.SalesAssociate2);
                    if (associate != null)
                    {
                        deal.SalesAssociate2 = associate.Text;
                    }
                }

                if (deal.StockNumber != null && deal.StockNumber != "" && (deal.StockNumber.Substring(1,1) == "F" || deal.StockNumber.Substring(1, 1) == "N") && !Char.IsLetter(deal.StockNumber[deal.StockNumber.Length - 1]))
                {
                    if (deal.ShowroomValidatedBy != null && deal.ShowroomValidatedBy != "")
                    {
                        nextCarDeals.Add(deal);
                    }
                }

            }

            return nextCarDeals;
        }

        public static List<DealDetail> GetSalesLogCertificationLevelDeals(string makeId, string monthId, string yearId, string certLevel, string cpoMakeName = "")
        {
            var reportDate = new DateTime(Int32.Parse(yearId), Int32.Parse(monthId), 1);
            var dealDetails = new List<DealDetail>();

            //If there is a comma, we are getting multiple makes...
            if (makeId.Contains(","))
            {
                var makeIds = makeId.Split(',');
                foreach (var make in makeIds)
                {
                    var tempDetails = SqlMapperUtil.StoredProcWithParams<DealDetail>("sp_SalesLogDealsByDateAndStore", new { AutoMallID = make, ReportDate = reportDate }, "SalesCommission");
                    foreach (var detail in tempDetails)
                    {
                        dealDetails.Add(detail);
                    }
                }

            }
            else
            {
                dealDetails = SqlMapperUtil.StoredProcWithParams<DealDetail>("sp_SalesLogDealsByDateAndStore", new { AutoMallID = makeId, ReportDate = reportDate }, "SalesCommission");
            }

            //var otherDetails = SqlMapperUtil.StoredProcWithParams<DealDetail>("sp_AllDealDetailsByDate", new { ReportDate = reportDate }, "ReynoldsData");
            var associates = GetSalesAssociates();
            var handymanDeals = new List<DealDetail>();

            foreach (var deal in dealDetails)
            {
                //var otherDetails = SqlMapperUtil.StoredProcWithParams<DealDetail>("sp_DealDetailsByLocDeal", new { LocDeal = deal.DealKey }, "SalesCommission");

                    //var otherDeal = otherDetails[0];
                if (deal.CertificationLevel != null && deal.CertificationLevel != "" && certLevel.Contains(deal.CertificationLevel))
                {
                    deal.ReportDate = reportDate;
                    if (deal.CustomerName == null || deal.CustomerName == "")
                    {
                        deal.CustomerName = deal.BuyerName;

                    }

                    if (deal.SalesAssociate1 != null && deal.SalesAssociate1 != "")
                    {
                        var associate = associates.Find(o => o.Value == deal.SalesAssociate1);
                        if (associate != null)
                        {
                            deal.SalesAssociate1 = associate.Text;
                        }
                    }

                    if (deal.SalesAssociate2 != null && deal.SalesAssociate2 != "")
                    {
                        var associate = associates.Find(o => o.Value == deal.SalesAssociate2);
                        if (associate != null)
                        {
                            deal.SalesAssociate2 = associate.Text;
                        }
                    }

                    //var otherDeal = otherDetails.Find(o => o.DealKey.Trim() == deal.DealKey.Trim());

                    ////if(otherDetails != null && otherDetails.Count > 0)
                    //if (otherDeal != null)
                    //{
                    //    deal.CustomerName = otherDeal.CustomerName;
                    //    deal.SalesAssociate1 = otherDeal.SalesAssociate1;
                    //    deal.SalesAssociate2 = otherDeal.SalesAssociate2;
                    //    deal.ModelNumber = otherDeal.ModelNumber;
                    //    deal.Carline = otherDeal.Carline;
                    //    deal.StockNumber = otherDeal.StockNumber;
                    //    deal.DealDate = otherDeal.DealDate;
                    //    deal.DaysInStock = otherDeal.DaysInStock;
                    //    deal.Year = otherDeal.Year;
                    //    deal.FinanceManager = otherDeal.FinanceManager;

                    //    deal.Category = otherDeal.Category;
                    //}

                    if (deal.ShowroomValidatedBy != null && deal.ShowroomValidatedBy != "")
                    {

                        if (cpoMakeName != null && cpoMakeName != "" && deal.CarMake != null)
                        {
                            if(deal.CarMake.ToUpper().Contains(cpoMakeName.ToUpper()))
                            {
                                handymanDeals.Add(deal);
                            }
                        }
                        else
                        {
                            handymanDeals.Add(deal);
                        }
                    }
                }
            }

            return handymanDeals;
        }

        public static List<DealDetail> GetSalesLogDealLists(string listType, string makeId, string monthId, string yearId)
        {

            var reportDate = new DateTime(Int32.Parse(yearId), Int32.Parse(monthId), 1);
            var dealDetails = new List<DealDetail>();

            //If there is a comma, we are getting multiple makes...
            if (makeId.Contains(","))
            {
                var makeIds = makeId.Split(',');
                foreach (var make in makeIds)
                {
                    var tempDetails = SqlMapperUtil.StoredProcWithParams<DealDetail>("sp_SalesLogDealsByDateAndStore", new { AutoMallID = make, ReportDate = reportDate }, "SalesCommission");
                    foreach (var detail in tempDetails)
                    {
                        dealDetails.Add(detail);
                    }
                }

            }
            else
            {
                dealDetails = SqlMapperUtil.StoredProcWithParams<DealDetail>("sp_SalesLogDealsByDateAndStore", new { AutoMallID = makeId, ReportDate = reportDate }, "SalesCommission");
            }
            var associates = GetSalesAssociates();
            //var otherDetails = SqlMapperUtil.StoredProcWithParams<DealDetail>("sp_AllDealDetailsByDate", new { ReportDate = reportDate }, "ReynoldsData");

            var listODeals = new List<DealDetail>();

            foreach (var deal in dealDetails)
            {
                deal.ReportDate = reportDate;
                if (deal.CustomerName == null || deal.CustomerName == "")
                {
                    deal.CustomerName = deal.BuyerName;

                }

                if (deal.SalesAssociate1 != null && deal.SalesAssociate1 != "")
                {
                    var associate = associates.Find(o => o.Value == deal.SalesAssociate1);
                    if (associate != null)
                    {
                        deal.SalesAssociate1 = associate.Text;
                    }
                }

                if (deal.SalesAssociate2 != null && deal.SalesAssociate2 != "")
                {
                    var associate = associates.Find(o => o.Value == deal.SalesAssociate2);
                    if (associate != null)
                    {
                        deal.SalesAssociate2 = associate.Text;
                    }
                }
                //var otherDeal = otherDetails.Find(o => o.DealKey.Trim() == deal.DealKey.Trim());

                //if (otherDeal != null)
                //{
                //    //var otherDeal = otherDetails[0];

                //    deal.CustomerName = otherDeal.CustomerName;
                //    deal.SalesAssociate1 = otherDeal.SalesAssociate1;
                //    deal.SalesAssociate2 = otherDeal.SalesAssociate2;
                //    deal.ModelNumber = otherDeal.ModelNumber;
                //    deal.Carline = otherDeal.Carline;
                //    deal.StockNumber = otherDeal.StockNumber;
                //    deal.DealDate = otherDeal.DealDate;
                //    deal.DaysInStock = otherDeal.DaysInStock;
                //    deal.Year = otherDeal.Year;
                //    deal.FinanceManager = otherDeal.FinanceManager;

                //}

                switch (listType)
                {
                    // Only showroom validated deals
                    case "Finance":
                        if (deal.FinIncAmount > 0 && (deal.ShowroomValidatedBy != null && deal.ShowroomValidatedBy != ""))
                        {
                            listODeals.Add(deal);
                        }
                        break;
                    case "VSC":
                        if (deal.VSCAmount > 0 && (deal.ShowroomValidatedBy != null && deal.ShowroomValidatedBy != ""))
                        {
                            listODeals.Add(deal);
                        }
                        break;
                    case "GAP":
                        if (deal.GapAmount > 0 && (deal.ShowroomValidatedBy != null && deal.ShowroomValidatedBy != ""))
                        {
                            listODeals.Add(deal);
                        }
                        break;
                    case "Aftermarket":
                        if (deal.OtherAmount > 0 && (deal.ShowroomValidatedBy != null && deal.ShowroomValidatedBy != ""))
                        {
                            listODeals.Add(deal);
                        }
                        break;
                    case "BPP":
                        if (Convert.ToDouble(deal.BPPAmount) > 0 && (deal.ShowroomValidatedBy != null && deal.ShowroomValidatedBy != ""))
                        {
                            listODeals.Add(deal);
                        }
                        break;
                    case "Trade":
                        if (deal.TradeAmount > 0 && (deal.ShowroomValidatedBy != null && deal.ShowroomValidatedBy != ""))
                        {
                            listODeals.Add(deal);
                        }
                        break;
                }



            }

            return listODeals;

        }

        public static List<DealDetail> GetSalesLogDealsNotShowroomValidated (string makeId, string monthId, string yearId)
        {
            var reportDate = new DateTime(Int32.Parse(yearId), Int32.Parse(monthId), 1);
            var dealDetails = new List<DealDetail>();

            //If there is a comma, we are getting multiple makes...
            if (makeId.Contains(","))
            {
                var makeIds = makeId.Split(',');
                foreach (var make in makeIds)
                {
                    var tempDetails = SqlMapperUtil.StoredProcWithParams<DealDetail>("sp_SalesLogDealsByDateAndStore", new { AutoMallID = make, ReportDate = reportDate }, "SalesCommission");
                    foreach (var detail in tempDetails)
                    {
                        dealDetails.Add(detail);
                    }
                }

            }
            else
            {
                dealDetails = SqlMapperUtil.StoredProcWithParams<DealDetail>("sp_SalesLogDealsByDateAndStore", new { AutoMallID = makeId, ReportDate = reportDate }, "SalesCommission");
            }

            //var otherDetails = SqlMapperUtil.StoredProcWithParams<DealDetail>("sp_AllDealDetailsByDate", new { ReportDate = reportDate }, "ReynoldsData");
            var associates = GetSalesAssociates();
            var unvalidatedDeals = new List<DealDetail>();

            foreach (var deal in dealDetails)
            {
                if (deal.ShowroomValidatedBy == null || deal.ShowroomValidatedBy == "")
                {
                    //var otherDetails = SqlMapperUtil.StoredProcWithParams<DealDetail>("sp_DealDetailsByLocDeal", new { LocDeal = deal.DealKey }, "SalesCommission");
                    deal.ReportDate = reportDate;
                    if (deal.CustomerName == null || deal.CustomerName == "")
                    {
                        deal.CustomerName = deal.BuyerName;

                    }

                    if (deal.SalesAssociate1 != null && deal.SalesAssociate1 != "")
                    {
                        var associate = associates.Find(o => o.Value == deal.SalesAssociate1);
                        if (associate != null)
                        {
                            deal.SalesAssociate1 = associate.Text;
                        }
                    }

                    if (deal.SalesAssociate2 != null && deal.SalesAssociate2 != "")
                    {
                        var associate = associates.Find(o => o.Value == deal.SalesAssociate2);
                        if (associate != null)
                        {
                            deal.SalesAssociate2 = associate.Text;
                        }
                    }

                    //var otherDeal = otherDetails.Find(o => o.DealKey.Trim() == deal.DealKey.Trim());

                    ////if(otherDetails != null && otherDetails.Count > 0)
                    //if (otherDeal != null)
                    //{
                    //    //var otherDeal = otherDetails[0];

                    //    deal.CustomerName = otherDeal.CustomerName;
                    //    deal.SalesAssociate1 = otherDeal.SalesAssociate1;
                    //    deal.SalesAssociate2 = otherDeal.SalesAssociate2;
                    //    deal.ModelNumber = otherDeal.ModelNumber;
                    //    deal.Carline = otherDeal.Carline;
                    //    deal.StockNumber = otherDeal.StockNumber;
                    //    deal.DealDate = otherDeal.DealDate;
                    //    deal.DaysInStock = otherDeal.DaysInStock;
                    //    deal.Year = otherDeal.Year;
                    //    deal.FinanceManager = otherDeal.FinanceManager;

                    //}

                    unvalidatedDeals.Add(deal);
                }

            }

            return unvalidatedDeals;
        }

        public static List<DealDetail> GetSalesLogDealsShowroomValidated(string makeId, string monthId, string yearId)
        {
            var reportDate = new DateTime(Int32.Parse(yearId), Int32.Parse(monthId), 1);
            var dealDetails = new List<DealDetail>();

            //If there is a comma, we are getting multiple makes...
            if (makeId.Contains(","))
            {
                var makeIds = makeId.Split(',');
                foreach (var make in makeIds)
                {
                    var tempDetails = SqlMapperUtil.StoredProcWithParams<DealDetail>("sp_SalesLogDealsByDateAndStore", new { AutoMallID = make, ReportDate = reportDate }, "SalesCommission");
                    foreach (var detail in tempDetails)
                    {
                        dealDetails.Add(detail);
                    }
                }

            }
            else
            {
                dealDetails = SqlMapperUtil.StoredProcWithParams<DealDetail>("sp_SalesLogDealsByDateAndStore", new { AutoMallID = makeId, ReportDate = reportDate }, "SalesCommission");
            }

            //var otherDetails = SqlMapperUtil.StoredProcWithParams<DealDetail>("sp_AllDealDetailsByDate", new { ReportDate = reportDate }, "ReynoldsData");

            var validatedDeals = new List<DealDetail>();
            var associates = GetSalesAssociates();

            foreach (var deal in dealDetails)
            {
                if ((deal.ShowroomValidatedBy != null && deal.ShowroomValidatedBy != "") && (deal.OfficeValidatedBy == null || deal.OfficeValidatedBy == ""))
                {
                    //var otherDetails = SqlMapperUtil.StoredProcWithParams<DealDetail>("sp_DealDetailsByLocDeal", new { LocDeal = deal.DealKey }, "SalesCommission");
                    deal.ReportDate = reportDate;
                    if (deal.CustomerName == null || deal.CustomerName == "")
                    {
                        deal.CustomerName = deal.BuyerName;

                    }

                    if (deal.SalesAssociate1 != null && deal.SalesAssociate1 != "")
                    {
                        var associate = associates.Find(o => o.Value == deal.SalesAssociate1);
                        if (associate != null)
                        {
                            deal.SalesAssociate1 = associate.Text;
                        }
                    }

                    if (deal.SalesAssociate2 != null && deal.SalesAssociate2 != "")
                    {
                        var associate = associates.Find(o => o.Value == deal.SalesAssociate2);
                        if (associate != null)
                        {
                            deal.SalesAssociate2 = associate.Text;
                        }
                    }

                    //var otherDeal = otherDetails.Find(o => o.DealKey.Trim() == deal.DealKey.Trim());

                    ////if(otherDetails != null && otherDetails.Count > 0)
                    //if (otherDeal != null)
                    //{
                    //    //var otherDeal = otherDetails[0];

                    //    deal.CustomerName = otherDeal.CustomerName;
                    //    deal.SalesAssociate1 = otherDeal.SalesAssociate1;
                    //    deal.SalesAssociate2 = otherDeal.SalesAssociate2;
                    //    deal.ModelNumber = otherDeal.ModelNumber;
                    //    deal.Carline = otherDeal.Carline;
                    //    deal.StockNumber = otherDeal.StockNumber;
                    //    deal.DealDate = otherDeal.DealDate;
                    //    deal.DaysInStock = otherDeal.DaysInStock;
                    //    deal.Year = otherDeal.Year;
                    //    deal.FinanceManager = otherDeal.FinanceManager;

                    //}

                    validatedDeals.Add(deal);
                }

            }

            return validatedDeals;
        }

        public static List<AssociateScoreCard> GetAssociateScoreCardHistoryByDate(string associateSSN, int yearId, int monthId)
        {
            var monthYear = monthId.ToString() + "/" + yearId.ToString();

            var associateScorecards = SqlMapperUtil.StoredProcWithParams<AssociateScoreCard>("sp_CommissionGetAssociateScorecardsByDate", new { AssociateSSN = associateSSN, MonthYear = monthYear }, "SalesCommission");

            return associateScorecards;
        }

        public static int SaveAssociateScoreCardHistory(AssociateScoreCard associateScoreCard)
        {
            int saveObjStn = SqlMapperUtil.InsertUpdateOrDeleteStoredProc("sp_CommissionAddAssociateScorecardsByDate", associateScoreCard, "SalesCommission");
            return saveObjStn;
        }

        public static List<Associate> GetAssociateListForScorecard(string location, int yearId, int monthId)
        {            
            var monthYear = monthId.ToString() + "/" + yearId.ToString();

            var associates = SqlMapperUtil.StoredProcWithParams<Associate>("sp_CommissionGetAssociatesByStoreAndDate", new { Location = location, MonthYear = monthYear }, "SalesCommission");

            return associates;
        }

        public static Associate GetAssociateScoreCardByDate(string associateId, int yearId, int monthId)
        {
            var associateList = new List<Associate>();

            var monthYear = monthId.ToString() + "/" + yearId.ToString();
            var dealMonth = new DateTime(yearId, monthId, 1);
            var associate = SqlMapperUtil.StoredProcWithParams<Associate>("sp_CommissionGetAssociateByIdAndDate", new { EmployeeNumber = associateId, MonthYear = monthYear }, "SalesCommission");
            var completeAssociate = new Associate();

            if (associate != null && associate.Count > 0)
            {
                completeAssociate = associate[0];
                if (completeAssociate.AssociatePayscale == "")
                {
                    completeAssociate.AssociatePayscale = "STDVOL";
                }

                var payCalculation = SqlMapperUtil.SqlWithParams<PayLevelCalculation>("SELECT TOP 1 * FROM [SalesCommission].[dbo].[CommissionCalculatedPayLevel] where pl_associatessn = '" + completeAssociate.AssociateSSN + "' order by pl_updateDate desc", null, "SalesCommission");

                if (payCalculation != null && payCalculation.Count > 0)
                {
                    completeAssociate.PayLevelCalculation = payCalculation[0];
                }
                else
                {
                    completeAssociate.PayLevelCalculation = new PayLevelCalculation();
                }

                var associateGoals = new List<Goal>();

                var currentGoals = SqlMapperUtil.StoredProcWithParams<Goal>("sp_CommissionGetAssociatesGoalsByStoreAndDate", new { AssociateSSN = completeAssociate.AssociateSSN, MonthYear = monthYear }, "SalesCommission");
                if (currentGoals != null && currentGoals.Count > 0)
                {
                    associateGoals.Add(currentGoals[0]);
                }

                var previousDate = dealMonth.AddMonths(-1);
                var previousMonthYear = previousDate.Month.ToString() + "/" + previousDate.Year.ToString();
                var previousGoals = SqlMapperUtil.StoredProcWithParams<Goal>("sp_CommissionGetAssociatesGoalsByStoreAndDate", new { AssociateSSN = completeAssociate.AssociateSSN, MonthYear = previousMonthYear }, "SalesCommission");
                if (previousGoals != null && previousGoals.Count > 0)
                {
                    associateGoals.Add(previousGoals[0]);
                }

                var previous2Date = dealMonth.AddMonths(-2);
                var previous2MonthYear = previous2Date.Month.ToString() + "/" + previous2Date.Year.ToString();
                var previous2Goals = SqlMapperUtil.StoredProcWithParams<Goal>("sp_CommissionGetAssociatesGoalsByStoreAndDate", new { AssociateSSN = completeAssociate.AssociateSSN, MonthYear = previous2MonthYear }, "SalesCommission");
                if (previous2Goals != null && previous2Goals.Count > 0)
                {
                    associateGoals.Add(previous2Goals[0]);
                }


                completeAssociate.AssociateGoals = associateGoals;

                completeAssociate.AssociateHours = SqlMapperUtil.StoredProcWithParams<AssociateHours>("sp_CommissionGetAssociatesHoursByStoreAndDate", new { AssociateNumber = completeAssociate.AssociateNumber, ReportDate = dealMonth }, "SalesCommission");

                completeAssociate.AssociateLeads = SqlMapperUtil.StoredProcWithParams<AssociateLead>("sp_CommissionGetAssociateLeadsByDate", new { StartDate = dealMonth.AddMonths(-3), EndDate = dealMonth.AddMonths(1) }, "ReynoldsData");
                completeAssociate.AssociateAppointments = SqlMapperUtil.StoredProcWithParams<AssociateAppointment>("sp_CommissionGetAssociateAppointmentsByDate", new { StartDate = dealMonth.AddMonths(-3), EndDate = dealMonth.AddMonths(1) }, "SalesCommission");

                var previousHours = new List<AssociateHours>();

                var previous1Hours = SqlMapperUtil.StoredProcWithParams<AssociateHours>("sp_CommissionGetAssociatesHoursByStoreAndDate", new { AssociateNumber = completeAssociate.AssociateNumber, ReportDate = dealMonth.AddMonths(-1) }, "SalesCommission");
                if(previous1Hours != null && previous1Hours.Count > 0)
                {
                    previousHours.Add(previous1Hours[0]);
                }

                var previous2Hours = SqlMapperUtil.StoredProcWithParams<AssociateHours>("sp_CommissionGetAssociatesHoursByStoreAndDate", new { AssociateNumber = completeAssociate.AssociateNumber, ReportDate = dealMonth.AddMonths(-2) }, "SalesCommission");
                if (previous2Hours != null && previous2Hours.Count > 0)
                {
                    previousHours.Add(previous2Hours[0]);
                }

                var previous3Hours = SqlMapperUtil.StoredProcWithParams<AssociateHours>("sp_CommissionGetAssociatesHoursByStoreAndDate", new { AssociateNumber = completeAssociate.AssociateNumber, ReportDate = dealMonth.AddMonths(-3) }, "SalesCommission");
                if (previous3Hours != null && previous3Hours.Count > 0)
                {
                    previousHours.Add(previous3Hours[0]);
                }

                completeAssociate.PreviousAssociateHours = previousHours;

                completeAssociate.AssociateDeals = SqlMapperUtil.StoredProcWithParams<AssociateDeals>("sp_CommissionGetAssociatesDealsByIdAndDate", new { EmployeeNumber = completeAssociate.AssociateNumber, ReportDate = dealMonth, Location = completeAssociate.AssociateLocation }, "SalesCommission");
                                
                var previousDeals = new List<PreviousMonthsDeals>();
                var previousDealCounts = new List<DealCommissionCounts>();

                var previousMonth1 = new PreviousMonthsDeals();
                var previousMonthDeals = SqlMapperUtil.StoredProcWithParams<AssociateDeals>("sp_CommissionGetAssociatesDealsByIdAndDate", new { EmployeeNumber = completeAssociate.AssociateNumber, ReportDate = dealMonth.AddMonths(-1), Location = completeAssociate.AssociateLocation }, "SalesCommission");
                previousMonth1.AssociateDeals = previousMonthDeals;
                previousDeals.Add(previousMonth1);

                var previousCounts1 = new DealCommissionCounts();
                previousCounts1.DealMonth = dealMonth.AddMonths(-1);
                foreach (var deal in previousMonthDeals)
                {
                    previousCounts1.NewDealCount += deal.NewDealCount;
                    previousCounts1.UsedDealCount += deal.UsedDealCount;
                    previousCounts1.TotalDealCount += (deal.NewDealCount + deal.UsedDealCount);
                    previousCounts1.LeaseCount += deal.LeaseCount;
                    previousCounts1.BPPCount += deal.BPPCount;
                    previousCounts1.FinanceCount += deal.FinanceCount;
                    previousCounts1.ServiceContractCount += deal.ServiceContractCount;
                    previousCounts1.MaintenanceContractCount += deal.MaintenanceContractCount;
                    previousCounts1.GAPCount += deal.GAPCount;
                    previousCounts1.TradeCount += deal.TradeCount;
                    previousCounts1.AftermarketCount += deal.AftermarketCount;
                }

                previousDealCounts.Add(previousCounts1);
                

                var previousMonth2 = new PreviousMonthsDeals();
                var previousMonth2Deals = SqlMapperUtil.StoredProcWithParams<AssociateDeals>("sp_CommissionGetAssociatesDealsByIdAndDate", new { EmployeeNumber = completeAssociate.AssociateNumber, ReportDate = dealMonth.AddMonths(-2), Location = completeAssociate.AssociateLocation }, "SalesCommission");
                previousMonth2.AssociateDeals = previousMonth2Deals;
                previousDeals.Add(previousMonth2);

                var previousCounts2 = new DealCommissionCounts();
                previousCounts2.DealMonth = dealMonth.AddMonths(-2);
                foreach (var deal in previousMonth2Deals)
                {
                    previousCounts2.NewDealCount += deal.NewDealCount;
                    previousCounts2.UsedDealCount += deal.UsedDealCount;
                    previousCounts2.TotalDealCount += (deal.NewDealCount + deal.UsedDealCount);
                    previousCounts2.LeaseCount += deal.LeaseCount;
                    previousCounts2.BPPCount += deal.BPPCount;
                    previousCounts2.FinanceCount += deal.FinanceCount;
                    previousCounts2.ServiceContractCount += deal.ServiceContractCount;
                    previousCounts2.MaintenanceContractCount += deal.MaintenanceContractCount;
                    previousCounts2.GAPCount += deal.GAPCount;
                    previousCounts2.TradeCount += deal.TradeCount;
                    previousCounts2.AftermarketCount += deal.AftermarketCount;
                }

                previousDealCounts.Add(previousCounts2);


                var previousMonth3 = new PreviousMonthsDeals();
                var previousMonth3Deals = SqlMapperUtil.StoredProcWithParams<AssociateDeals>("sp_CommissionGetAssociatesDealsByIdAndDate", new { EmployeeNumber = completeAssociate.AssociateNumber, ReportDate = dealMonth.AddMonths(-3), Location = completeAssociate.AssociateLocation }, "SalesCommission");
                previousMonth3.AssociateDeals = previousMonth3Deals;
                previousDeals.Add(previousMonth3);

                var previousCounts3 = new DealCommissionCounts();
                previousCounts3.DealMonth = dealMonth.AddMonths(-3);
                foreach (var deal in previousMonth3Deals)
                {
                    previousCounts3.NewDealCount += deal.NewDealCount;
                    previousCounts3.UsedDealCount += deal.UsedDealCount;
                    previousCounts3.TotalDealCount += (deal.NewDealCount + deal.UsedDealCount);
                    previousCounts3.LeaseCount += deal.LeaseCount;
                    previousCounts3.BPPCount += deal.BPPCount;
                    previousCounts3.FinanceCount += deal.FinanceCount;
                    previousCounts3.ServiceContractCount += deal.ServiceContractCount;
                    previousCounts3.MaintenanceContractCount += deal.MaintenanceContractCount;
                    previousCounts3.GAPCount += deal.GAPCount;
                    previousCounts3.TradeCount += deal.TradeCount;
                    previousCounts3.AftermarketCount += deal.AftermarketCount;
                }

                previousDealCounts.Add(previousCounts3);

                completeAssociate.PreviousAssociateDeals = previousDeals;
                completeAssociate.PreviousAssociateDealCounts = previousDealCounts;

                // GET THE ASSOCIATE UNITS
                var associateUnits = new List<AssociateUnits>();
                var unitDate = new DateTime();
                unitDate = dealMonth;


                for (int i = 0; i < 4; i++)
                {
                    decimal totalUnits = 0;
                    var associateMonthlyUnits = new AssociateUnits();

                    var monthlyUnits = SqlMapperUtil.StoredProcWithParams<decimal>("sp_CommissionGetAssociatesDealCountByIdAndDate", new { EmployeeNumber = completeAssociate.AssociateNumber, ReportDate = unitDate.AddMonths(-i), Location = completeAssociate.AssociateLocation }, "SalesCommission");
                    foreach (var unitCount in monthlyUnits)
                    {
                        totalUnits += unitCount;
                    }
                    associateMonthlyUnits.UnitCount = totalUnits;
                    associateMonthlyUnits.UnitDate = unitDate.AddMonths(-i);

                    associateUnits.Add(associateMonthlyUnits);

                }

                completeAssociate.AssociateUnits = associateUnits;

                var newPayscales = SqlMapperUtil.StoredProcWithParams<Models.NewPayscale>("sp_CommissionGetNewPayscaleByIDAndDate", new { MonthYear = monthYear, PayscaleID = completeAssociate.AssociatePayscale }, "SalesCommission");
                completeAssociate.AllPayscales = newPayscales;

                if (newPayscales != null)
                {
                    completeAssociate.AssociatePayscales = newPayscales.FindAll(o => o.ps_PayLevel.Trim() == completeAssociate.AssociateLevel.Trim());
                }

                completeAssociate.AssociatePayscaleSetup = SqlMapperUtil.StoredProcWithParams<Models.NewPayscaleSetup>("sp_CommissionGetNewPayscaleSetupById", new { PayscaleID = completeAssociate.AssociatePayscale }, "SalesCommission");


                var associateDealCounts = new DealCommissionCounts();
                associateDealCounts.DealMonth = dealMonth;

                foreach (var deal in completeAssociate.AssociateDeals)
                {
                    associateDealCounts.NewDealCount += deal.NewDealCount;
                    associateDealCounts.UsedDealCount += deal.UsedDealCount;
                    associateDealCounts.TotalDealCount += (deal.NewDealCount + deal.UsedDealCount);
                    associateDealCounts.LeaseCount += deal.LeaseCount;
                    associateDealCounts.BPPCount += deal.BPPCount;
                    associateDealCounts.FinanceCount += deal.FinanceCount;
                    associateDealCounts.ServiceContractCount += deal.ServiceContractCount;
                    associateDealCounts.MaintenanceContractCount += deal.MaintenanceContractCount;
                    associateDealCounts.GAPCount += deal.GAPCount;
                    associateDealCounts.TradeCount += deal.TradeCount;
                    associateDealCounts.AftermarketCount += deal.AftermarketCount;
                }

                completeAssociate.AssociateDealCounts = associateDealCounts;

                


            }

            return completeAssociate;
        }

        public static List<DealApproval> GetDealApprovalsByDate(int yearId, int monthId)
        {
            var monthYear = monthId.ToString() + "/" + yearId.ToString();

            var dealApprovals = SqlMapperUtil.StoredProcWithParams<DealApproval>("sp_CommissionGetDealApprovalsByDate", new { MonthYear = monthYear }, "SalesCommission");

            return dealApprovals;

        }

        public static List<PaidBonus> GetBonusesPaidByDate(int yearId, int monthId)
        {
            var monthYear = monthId.ToString() + "/" + yearId.ToString();

            var paidBonuses = SqlMapperUtil.StoredProcWithParams<PaidBonus>("sp_CommissionGetPaidBonusByDate", new { MonthYear = monthYear }, "SalesCommission");

            return paidBonuses;

        }

        public static int SaveDealApproval(string monthYear, string approvalUser, string dealKey)
        {
            int saveObjStn = SqlMapperUtil.InsertUpdateOrDeleteStoredProc("sp_CommissionUpdateDealApprovalByUser", new { MonthYear = monthYear, DealKey = dealKey, ApprovalUser = approvalUser } , "SalesCommission");
            return saveObjStn;
        }

        public static Associate GetAssociateInformationDrawsAndBonus(string associateId, int yearId, int monthId)
        {
            var monthYear = monthId.ToString() + "/" + yearId.ToString();
            var dealMonth = new DateTime(yearId, monthId, 1);

            var associate = SqlMapperUtil.StoredProcWithParams<Associate>("sp_CommissionGetAssociateByIdAndDate", new { EmployeeNumber = associateId, MonthYear = monthYear }, "SalesCommission");

            var completeAssociate = new Associate();

            if (associate != null && associate.Count > 0)
            {
                completeAssociate = associate[0];

                completeAssociate.AssociateDraws = SqlMapperUtil.StoredProcWithParams<Draw>("sp_CommissionGetAssociatesDrawsByStoreAndDate", new { AssociateSSN = completeAssociate.AssociateSSN, MonthYear = monthYear }, "SalesCommission");
                completeAssociate.AssociateBonus = SqlMapperUtil.StoredProcWithParams<Bonus>("sp_CommissionGetAssociatesBonusByStoreAndDate", new { AssociateSSN = completeAssociate.AssociateSSN, MonthYear = monthYear }, "SalesCommission");

            }

            return completeAssociate;
        }

        public static Associate GetFIAssociateInformationByDate(string associateId)
        {
            var associate = SqlMapperUtil.StoredProcWithParams<Associate>("sp_CommissionGetFIAssociateById", new { EmployeeNumber = associateId }, "SalesCommission");

            var completeAssociate = new Associate();
            if (associate != null && associate.Count > 0)
            {
                completeAssociate = associate[0];
            }
            return completeAssociate;
        }

        public static Associate GetAssociateInformationByDate(string associateId, int yearId, int monthId)
        {
            var monthYear = monthId.ToString() + "/" + yearId.ToString();
            var dealMonth = new DateTime(yearId, monthId, 1);

            var associate = SqlMapperUtil.StoredProcWithParams<Associate>("sp_CommissionGetAssociateByIdAndDate", new { EmployeeNumber = associateId, MonthYear = monthYear }, "SalesCommission");

            var completeAssociate = new Associate();

            if (associate != null && associate.Count > 0)
            {
                completeAssociate = associate[0];

                if (completeAssociate.AssociatePayscale == "")
                {
                    completeAssociate.AssociatePayscale = "HOME";
                }

                completeAssociate.AssociateDraws = SqlMapperUtil.StoredProcWithParams<Draw>("sp_CommissionGetAssociatesDrawsByStoreAndDate", new { AssociateSSN = completeAssociate.AssociateSSN, MonthYear = monthYear }, "SalesCommission");
                completeAssociate.AssociateBonus = SqlMapperUtil.StoredProcWithParams<Bonus>("sp_CommissionGetAssociatesBonusByStoreAndDate", new { AssociateSSN = completeAssociate.AssociateSSN, MonthYear = monthYear }, "SalesCommission");
                completeAssociate.AssociateDeals = SqlMapperUtil.StoredProcWithParams<AssociateDeals>("sp_CommissionGetAssociatesDealsByIdAndDate", new { EmployeeNumber = completeAssociate.AssociateNumber, ReportDate = dealMonth, Location = completeAssociate.AssociateLocation }, "SalesCommission");
                completeAssociate.AssociateHours = SqlMapperUtil.StoredProcWithParams<AssociateHours>("sp_CommissionGetAssociatesHoursByStoreAndDate", new { AssociateNumber = completeAssociate.AssociateNumber, ReportDate = dealMonth }, "SalesCommission");
                completeAssociate.AssociateGoals = SqlMapperUtil.StoredProcWithParams<Goal>("sp_CommissionGetAssociatesGoalsByStoreAndDate", new { AssociateSSN = completeAssociate.AssociateSSN, MonthYear = monthYear }, "SalesCommission");

                var payCalculation = SqlMapperUtil.SqlWithParams<PayLevelCalculation>("SELECT TOP 1 * FROM [SalesCommission].[dbo].[CommissionCalculatedPayLevel] where pl_associatessn = '" + completeAssociate.AssociateSSN + "' order by pl_updateDate desc", null, "SalesCommission");

                if (payCalculation != null && payCalculation.Count > 0)
                {
                    completeAssociate.PayLevelCalculation = payCalculation[0];
                }
                else
                {
                    completeAssociate.PayLevelCalculation = new PayLevelCalculation();
                }

                completeAssociate.AssociateWage = GetDealershipWageByDateAndStore(monthYear, completeAssociate.AssociateLocation);

                var associateUnits = new List<AssociateUnits>();
                var unitDate = new DateTime();
                unitDate = dealMonth;


                for (int i = 0; i < 13; i++)
                {
                    decimal totalUnits = 0;
                    var associateMonthlyUnits = new AssociateUnits();

                    var monthlyUnits = SqlMapperUtil.StoredProcWithParams<decimal>("sp_CommissionGetAssociatesDealCountByIdAndDate", new { EmployeeNumber = completeAssociate.AssociateNumber, ReportDate = unitDate.AddMonths(-i), Location = completeAssociate.AssociateLocation }, "SalesCommission");
                    foreach(var unitCount in monthlyUnits)
                    {
                        totalUnits += unitCount;
                    }
                    associateMonthlyUnits.UnitCount = totalUnits;
                    associateMonthlyUnits.UnitDate = unitDate.AddMonths(-i);

                    associateUnits.Add(associateMonthlyUnits);

                }

                completeAssociate.AssociateUnits = associateUnits;

                //var payscales = SqlMapperUtil.StoredProcWithParams<Models.Payscale>("sp_CommissionGetPayscaleByIDAndDate",new { MonthYear = monthYear, PayscaleID = completeAssociate.AssociatePayscale},"SalesCommission");

                //if (payscales != null)
                //{
                //    completeAssociate.AssociateOldPayscales = payscales.FindAll(o => o.ps_Level.Trim() == completeAssociate.AssociateLevel.Trim());
                //}

                var newPayscales = SqlMapperUtil.StoredProcWithParams<Models.NewPayscale>("sp_CommissionGetNewPayscaleByIDAndDate", new { MonthYear = monthYear, PayscaleID = completeAssociate.AssociatePayscale }, "SalesCommission");
                completeAssociate.AllPayscales = newPayscales;

                if (newPayscales != null)
                {
                    completeAssociate.AssociatePayscales = newPayscales.FindAll(o => o.ps_PayLevel.Trim() == completeAssociate.AssociateLevel.Trim());
                }

                completeAssociate.AssociatePayscaleSetup = SqlMapperUtil.StoredProcWithParams<Models.NewPayscaleSetup>("sp_CommissionGetNewPayscaleSetupById", new { PayscaleID = completeAssociate.AssociatePayscale }, "SalesCommission");

                var associateDealCounts = new DealCommissionCounts();

                foreach (var deal in completeAssociate.AssociateDeals)
                {
                    associateDealCounts.NewDealCount += deal.NewDealCount;
                    associateDealCounts.UsedDealCount += deal.UsedDealCount;
                    associateDealCounts.TotalDealCount += (deal.NewDealCount + deal.UsedDealCount);
                    associateDealCounts.LeaseCount += deal.LeaseCount;
                    associateDealCounts.BPPCount += deal.BPPCount;
                    associateDealCounts.FinanceCount += deal.FinanceCount;
                    associateDealCounts.ServiceContractCount += deal.ServiceContractCount;
                    associateDealCounts.MaintenanceContractCount += deal.MaintenanceContractCount;
                    associateDealCounts.GAPCount += deal.GAPCount;
                    associateDealCounts.TradeCount += deal.TradeCount;
                    associateDealCounts.AftermarketCount += deal.AftermarketCount;
                }

                completeAssociate.AssociateDealCounts = associateDealCounts;

            }

            return completeAssociate;
        }

        public static List<Associate> GetAssociateScorecardsByStoreAndDate(string storeId, int yearId, int monthId)
        {
            var location = SqlQueries.GetLocationCodeByStoreId(storeId);
            if(location == "FMM")
            {
                location = "FOC";
            }

            var monthYear = monthId.ToString() + "/" + yearId.ToString();

            var dealMonth = new DateTime(yearId, monthId, 1);

            var associates = SqlMapperUtil.StoredProcWithParams<Associate>("sp_CommissionGetAssociatesByStoreAndDate", new { Location = location, MonthYear = monthYear }, "SalesCommission");

            if (associates != null)
            {
                foreach (var associate in associates)
                {
                    if (associate.AssociatePayscale == "")
                    {
                        associate.AssociatePayscale = "STDVOL";
                    }

                    associate.AssociateGoals = SqlMapperUtil.StoredProcWithParams<Goal>("sp_CommissionGetAssociatesGoalsByStoreAndDate", new { AssociateSSN = associate.AssociateSSN, MonthYear = monthYear }, "SalesCommission");

                    associate.AssociateDeals = SqlMapperUtil.StoredProcWithParams<AssociateDeals>("sp_CommissionGetAssociatesDealsByIdAndDate", new { EmployeeNumber = associate.AssociateNumber, ReportDate = dealMonth, Location = associate.AssociateLocation }, "SalesCommission");

                    associate.AssociateHours = SqlMapperUtil.StoredProcWithParams<AssociateHours>("sp_CommissionGetAssociatesHoursByStoreAndDate", new { AssociateNumber = associate.AssociateNumber, ReportDate = dealMonth }, "SalesCommission");

                    //associate.AssociateLeads = SqlMapperUtil.StoredProcWithParams<AssociateLead>("sp_CommissionGetAssociateLeadsByDate", new { StartDate = dealMonth.AddMonths(-3), EndDate = dealMonth.AddMonths(1) }, "ReynoldsData");

                    associate.AssociateAppointments = SqlMapperUtil.StoredProcWithParams<AssociateAppointment>("sp_CommissionGetAssociateAppointmentsByDate", new { StartDate = dealMonth.AddMonths(-3), EndDate = dealMonth.AddMonths(1) }, "SalesCommission");


                    // GET THE ASSOCIATE UNITS
                    var associateUnits = new List<AssociateUnits>();
                    var unitDate = new DateTime();
                    unitDate = dealMonth;


                    for (int i = 0; i < 3; i++)
                    {
                        decimal totalUnits = 0;
                        var associateMonthlyUnits = new AssociateUnits();

                        var monthlyUnits = SqlMapperUtil.StoredProcWithParams<decimal>("sp_CommissionGetAssociatesDealCountByIdAndDate", new { EmployeeNumber = associate.AssociateNumber, ReportDate = unitDate.AddMonths(-i), Location = associate.AssociateLocation }, "SalesCommission");
                        foreach (var unitCount in monthlyUnits)
                        {
                            totalUnits += unitCount;
                        }
                        associateMonthlyUnits.UnitCount = totalUnits;
                        associateMonthlyUnits.UnitDate = unitDate.AddMonths(-i);

                        associateUnits.Add(associateMonthlyUnits);

                    }

                    associate.AssociateUnits = associateUnits;


                    var associateDealCounts = new DealCommissionCounts();

                    foreach (var deal in associate.AssociateDeals)
                    {
                        associateDealCounts.NewDealCount += deal.NewDealCount;
                        associateDealCounts.UsedDealCount += deal.UsedDealCount;
                        associateDealCounts.TotalDealCount += (deal.NewDealCount + deal.UsedDealCount);
                        associateDealCounts.LeaseCount += deal.LeaseCount;
                        associateDealCounts.BPPCount += deal.BPPCount;
                        associateDealCounts.FinanceCount += deal.FinanceCount;
                        associateDealCounts.ServiceContractCount += deal.ServiceContractCount;
                        associateDealCounts.MaintenanceContractCount += deal.MaintenanceContractCount;
                        associateDealCounts.GAPCount += deal.GAPCount;
                        associateDealCounts.TradeCount += deal.TradeCount;
                        associateDealCounts.AftermarketCount += deal.AftermarketCount;
                    }

                    associate.AssociateDealCounts = associateDealCounts;
                    

                }
            }

            return associates;
        }

        public static List<Associate> GetAssociateScorecardsAndHistoryByStoreAndDate(string storeId, int yearId, int monthId)
        {
            var location = SqlQueries.GetLocationCodeByStoreId(storeId);
            if (location == "FMM")
            {
                location = "FOC";
            }

            var monthYear = monthId.ToString() + "/" + yearId.ToString();

            var dealMonth = new DateTime(yearId, monthId, 1);

            var associates = SqlMapperUtil.StoredProcWithParams<Associate>("sp_CommissionGetAssociatesByStoreAndDate", new { Location = location, MonthYear = monthYear }, "SalesCommission");

            if (associates != null)
            {
                foreach (var associate in associates)
                {
                    if (associate.AssociatePayscale == "")
                    {
                        associate.AssociatePayscale = "STDVOL";
                    }

                    associate.AssociateGoals = SqlMapperUtil.StoredProcWithParams<Goal>("sp_CommissionGetAssociatesGoalsByStoreAndDate", new { AssociateSSN = associate.AssociateSSN, MonthYear = monthYear }, "SalesCommission");

                    associate.AssociateDeals = SqlMapperUtil.StoredProcWithParams<AssociateDeals>("sp_CommissionGetAssociatesDealsByIdAndDate", new { EmployeeNumber = associate.AssociateNumber, ReportDate = dealMonth, Location = associate.AssociateLocation }, "SalesCommission");

                    associate.AssociateHours = SqlMapperUtil.StoredProcWithParams<AssociateHours>("sp_CommissionGetAssociatesHoursByStoreAndDate", new { AssociateNumber = associate.AssociateNumber, ReportDate = dealMonth }, "SalesCommission");

                    //associate.AssociateLeads = SqlMapperUtil.StoredProcWithParams<AssociateLead>("sp_CommissionGetAssociateLeadsByDate", new { StartDate = dealMonth.AddMonths(-3), EndDate = dealMonth.AddMonths(1) }, "ReynoldsData");

                    associate.AssociateAppointments = SqlMapperUtil.StoredProcWithParams<AssociateAppointment>("sp_CommissionGetAssociateAppointmentsByDate", new { StartDate = dealMonth.AddMonths(-3), EndDate = dealMonth.AddMonths(1) }, "SalesCommission");

                    var newPayscales = SqlMapperUtil.StoredProcWithParams<Models.NewPayscale>("sp_CommissionGetNewPayscaleByIDAndDate", new { MonthYear = monthYear, PayscaleID = associate.AssociatePayscale }, "SalesCommission");
                    associate.AllPayscales = newPayscales;

                    if (newPayscales != null)
                    {
                        associate.AssociatePayscales = newPayscales.FindAll(o => o.ps_PayLevel.Trim() == associate.AssociateLevel.Trim());
                    }

                    associate.AssociatePayscaleSetup = SqlMapperUtil.StoredProcWithParams<Models.NewPayscaleSetup>("sp_CommissionGetNewPayscaleSetupById", new { PayscaleID = associate.AssociatePayscale }, "SalesCommission");

                    var previousHours = new List<AssociateHours>();

                    var previous1Hours = SqlMapperUtil.StoredProcWithParams<AssociateHours>("sp_CommissionGetAssociatesHoursByStoreAndDate", new { AssociateNumber = associate.AssociateNumber, ReportDate = dealMonth.AddMonths(-1) }, "SalesCommission");
                    if (previous1Hours != null && previous1Hours.Count > 0)
                    {
                        previousHours.Add(previous1Hours[0]);
                    }

                    var previous2Hours = SqlMapperUtil.StoredProcWithParams<AssociateHours>("sp_CommissionGetAssociatesHoursByStoreAndDate", new { AssociateNumber = associate.AssociateNumber, ReportDate = dealMonth.AddMonths(-2) }, "SalesCommission");
                    if (previous2Hours != null && previous2Hours.Count > 0)
                    {
                        previousHours.Add(previous2Hours[0]);
                    }

                    var previous3Hours = SqlMapperUtil.StoredProcWithParams<AssociateHours>("sp_CommissionGetAssociatesHoursByStoreAndDate", new { AssociateNumber = associate.AssociateNumber, ReportDate = dealMonth.AddMonths(-3) }, "SalesCommission");
                    if (previous3Hours != null && previous3Hours.Count > 0)
                    {
                        previousHours.Add(previous3Hours[0]);
                    }

                    associate.PreviousAssociateHours = previousHours;

                    //associate.AssociateDeals = SqlMapperUtil.StoredProcWithParams<AssociateDeals>("sp_CommissionGetAssociatesDealsByIdAndDate", new { EmployeeNumber = completeAssociate.AssociateNumber, ReportDate = dealMonth, Location = completeAssociate.AssociateLocation }, "SalesCommission");

                    var previousDeals = new List<PreviousMonthsDeals>();
                    var previousDealCounts = new List<DealCommissionCounts>();

                    var previousMonth1 = new PreviousMonthsDeals();
                    var previousMonthDeals = SqlMapperUtil.StoredProcWithParams<AssociateDeals>("sp_CommissionGetAssociatesDealsByIdAndDate", new { EmployeeNumber = associate.AssociateNumber, ReportDate = dealMonth.AddMonths(-1), Location = associate.AssociateLocation }, "SalesCommission");
                    previousMonth1.AssociateDeals = previousMonthDeals;
                    previousDeals.Add(previousMonth1);

                    var previousCounts1 = new DealCommissionCounts();
                    previousCounts1.DealMonth = dealMonth.AddMonths(-1);
                    foreach (var deal in previousMonthDeals)
                    {
                        previousCounts1.NewDealCount += deal.NewDealCount;
                        previousCounts1.UsedDealCount += deal.UsedDealCount;
                        previousCounts1.TotalDealCount += (deal.NewDealCount + deal.UsedDealCount);
                        previousCounts1.LeaseCount += deal.LeaseCount;
                        previousCounts1.BPPCount += deal.BPPCount;
                        previousCounts1.FinanceCount += deal.FinanceCount;
                        previousCounts1.ServiceContractCount += deal.ServiceContractCount;
                        previousCounts1.MaintenanceContractCount += deal.MaintenanceContractCount;
                        previousCounts1.GAPCount += deal.GAPCount;
                        previousCounts1.TradeCount += deal.TradeCount;
                        previousCounts1.AftermarketCount += deal.AftermarketCount;
                    }

                    previousDealCounts.Add(previousCounts1);


                    var previousMonth2 = new PreviousMonthsDeals();
                    var previousMonth2Deals = SqlMapperUtil.StoredProcWithParams<AssociateDeals>("sp_CommissionGetAssociatesDealsByIdAndDate", new { EmployeeNumber = associate.AssociateNumber, ReportDate = dealMonth.AddMonths(-2), Location = associate.AssociateLocation }, "SalesCommission");
                    previousMonth2.AssociateDeals = previousMonth2Deals;
                    previousDeals.Add(previousMonth2);

                    var previousCounts2 = new DealCommissionCounts();
                    previousCounts2.DealMonth = dealMonth.AddMonths(-2);
                    foreach (var deal in previousMonth2Deals)
                    {
                        previousCounts2.NewDealCount += deal.NewDealCount;
                        previousCounts2.UsedDealCount += deal.UsedDealCount;
                        previousCounts2.TotalDealCount += (deal.NewDealCount + deal.UsedDealCount);
                        previousCounts2.LeaseCount += deal.LeaseCount;
                        previousCounts2.BPPCount += deal.BPPCount;
                        previousCounts2.FinanceCount += deal.FinanceCount;
                        previousCounts2.ServiceContractCount += deal.ServiceContractCount;
                        previousCounts2.MaintenanceContractCount += deal.MaintenanceContractCount;
                        previousCounts2.GAPCount += deal.GAPCount;
                        previousCounts2.TradeCount += deal.TradeCount;
                        previousCounts2.AftermarketCount += deal.AftermarketCount;
                    }

                    previousDealCounts.Add(previousCounts2);


                    var previousMonth3 = new PreviousMonthsDeals();
                    var previousMonth3Deals = SqlMapperUtil.StoredProcWithParams<AssociateDeals>("sp_CommissionGetAssociatesDealsByIdAndDate", new { EmployeeNumber = associate.AssociateNumber, ReportDate = dealMonth.AddMonths(-3), Location = associate.AssociateLocation }, "SalesCommission");
                    previousMonth3.AssociateDeals = previousMonth3Deals;
                    previousDeals.Add(previousMonth3);

                    var previousCounts3 = new DealCommissionCounts();
                    previousCounts3.DealMonth = dealMonth.AddMonths(-3);
                    foreach (var deal in previousMonth3Deals)
                    {
                        previousCounts3.NewDealCount += deal.NewDealCount;
                        previousCounts3.UsedDealCount += deal.UsedDealCount;
                        previousCounts3.TotalDealCount += (deal.NewDealCount + deal.UsedDealCount);
                        previousCounts3.LeaseCount += deal.LeaseCount;
                        previousCounts3.BPPCount += deal.BPPCount;
                        previousCounts3.FinanceCount += deal.FinanceCount;
                        previousCounts3.ServiceContractCount += deal.ServiceContractCount;
                        previousCounts3.MaintenanceContractCount += deal.MaintenanceContractCount;
                        previousCounts3.GAPCount += deal.GAPCount;
                        previousCounts3.TradeCount += deal.TradeCount;
                        previousCounts3.AftermarketCount += deal.AftermarketCount;
                    }

                    previousDealCounts.Add(previousCounts3);

                    associate.PreviousAssociateDeals = previousDeals;
                    associate.PreviousAssociateDealCounts = previousDealCounts;
                    

                    // GET THE ASSOCIATE UNITS
                    var associateUnits = new List<AssociateUnits>();
                    var unitDate = new DateTime();
                    unitDate = dealMonth;


                    for (int i = 0; i < 3; i++)
                    {
                        decimal totalUnits = 0;
                        var associateMonthlyUnits = new AssociateUnits();

                        var monthlyUnits = SqlMapperUtil.StoredProcWithParams<decimal>("sp_CommissionGetAssociatesDealCountByIdAndDate", new { EmployeeNumber = associate.AssociateNumber, ReportDate = unitDate.AddMonths(-i), Location = associate.AssociateLocation }, "SalesCommission");
                        foreach (var unitCount in monthlyUnits)
                        {
                            totalUnits += unitCount;
                        }
                        associateMonthlyUnits.UnitCount = totalUnits;
                        associateMonthlyUnits.UnitDate = unitDate.AddMonths(-i);

                        associateUnits.Add(associateMonthlyUnits);

                    }

                    associate.AssociateUnits = associateUnits;


                    var associateDealCounts = new DealCommissionCounts();

                    foreach (var deal in associate.AssociateDeals)
                    {
                        associateDealCounts.NewDealCount += deal.NewDealCount;
                        associateDealCounts.UsedDealCount += deal.UsedDealCount;
                        associateDealCounts.TotalDealCount += (deal.NewDealCount + deal.UsedDealCount);
                        associateDealCounts.LeaseCount += deal.LeaseCount;
                        associateDealCounts.BPPCount += deal.BPPCount;
                        associateDealCounts.FinanceCount += deal.FinanceCount;
                        associateDealCounts.ServiceContractCount += deal.ServiceContractCount;
                        associateDealCounts.MaintenanceContractCount += deal.MaintenanceContractCount;
                        associateDealCounts.GAPCount += deal.GAPCount;
                        associateDealCounts.TradeCount += deal.TradeCount;
                        associateDealCounts.AftermarketCount += deal.AftermarketCount;
                    }

                    associate.AssociateDealCounts = associateDealCounts;


                }
            }

            return associates;
        }


        public static LeadReportModel GetLeadReportByDateAndStore(LeadReportModel leadReportModel, bool bReturnDeals = true)
        {
            var StoreLeads = new List<StoreLeadInformation>();

            var selectedStores = leadReportModel.SelectedStores;

            if (selectedStores == null || (selectedStores != null && selectedStores.Contains("ALL")))
            {

                var x = Enums.StoresReport.ToArray();

                selectedStores = new string[] { "annapolis", "chambersburg", "clearwater", "frederick", "lakeforest(russell)", "Lakeforest(355)", "lexingtonpark(lexpark)", "nicholson", "colonial", "wheaton" };
            }

            foreach (var storeId in selectedStores)
            {
                var storeLead = new StoreLeadInformation();

                storeLead.StoreId = storeId;

                var location = SqlQueries.GetLocationCodeByStoreId(storeId);
                if (location == "FMM")
                {
                    location = "FOC";
                }

                storeLead.Location = location;

                foreach (var dealer in Enums.StoreDealerId)
                {
                    if (dealer.StoreId == storeId)
                    {
                        storeLead.DealerId = dealer.Name;
                    }
                }


                var monthYear = leadReportModel.ReportEndDate.Month.ToString() + "/" + leadReportModel.ReportEndDate.Year.ToString();

                //var dealMonth = new DateTime(leadReportModel.ReportEndDate.Year, leadReportModel.ReportEndDate.Month, 1);

                var associates = SqlMapperUtil.StoredProcWithParams<Associate>("sp_CommissionGetAssociatesByStoreAndDate", new { Location = location, MonthYear = monthYear }, "SalesCommission");

                if (bReturnDeals)
                {
                    foreach (var associate in associates)
                    {
                        associate.AssociateDeals = SqlMapperUtil.StoredProcWithParams<AssociateDeals>("sp_CommissionGetAssociatesDealsByIdAndDateRange", new { EmployeeNumber = associate.AssociateNumber, StartDate = leadReportModel.ReportStartDate, EndDate = leadReportModel.ReportEndDate, Location = associate.AssociateLocation }, "SalesCommission");


                        var dealCounts = new DealCommissionCounts();
                        dealCounts.DealMonth = leadReportModel.ReportEndDate;
                        foreach (var deal in associate.AssociateDeals)
                        {
                            dealCounts.NewDealCount += deal.NewDealCount;
                            dealCounts.UsedDealCount += deal.UsedDealCount;
                            dealCounts.TotalDealCount += (deal.NewDealCount + deal.UsedDealCount);
                            dealCounts.LeaseCount += deal.LeaseCount;
                            dealCounts.BPPCount += deal.BPPCount;
                            dealCounts.FinanceCount += deal.FinanceCount;
                            dealCounts.ServiceContractCount += deal.ServiceContractCount;
                            dealCounts.MaintenanceContractCount += deal.MaintenanceContractCount;
                            dealCounts.GAPCount += deal.GAPCount;
                            dealCounts.TradeCount += deal.TradeCount;
                            dealCounts.AftermarketCount += deal.AftermarketCount;
                        }

                        associate.AssociateDealCounts = dealCounts;


                    }
                }
                storeLead.Associates = associates;

                StoreLeads.Add(storeLead);

            }
            var associateLeads = SqlMapperUtil.StoredProcWithParams<AssociateLead>("sp_CommissionGetAssociateLeadsByDate", new { StartDate = leadReportModel.ReportStartDate, EndDate = leadReportModel.ReportEndDate.AddDays(1) }, "ReynoldsData");
            if (leadReportModel.IncludeHandyman == false)
            {
                associateLeads = associateLeads.FindAll(x => !x.LeadSourceName.EndsWith("~"));
            }



            var associateAppointments = SqlMapperUtil.StoredProcWithParams<AssociateAppointment>("sp_CommissionGetAssociateAppointmentsByDate", new { StartDate = leadReportModel.ReportStartDate, EndDate = leadReportModel.ReportEndDate.AddDays(1) }, "SalesCommission");

            leadReportModel.StoreLeadInformation = StoreLeads;
            leadReportModel.AssociateLeads = associateLeads;
            leadReportModel.AssociateAppointments = associateAppointments;

            return leadReportModel;
        }

        public static LeadReportModel GetLeadReportNewByDateAndStore(LeadReportModel leadReportModel, bool bReturnDeals = true)
        {
            var StoreLeads = new List<StoreLeadInformation>();

            var selectedStores = leadReportModel.SelectedStores;

            if (selectedStores == null || (selectedStores != null && selectedStores.Contains("ALL")))
            {

                var x = Enums.StoresReport.ToArray();

                selectedStores = new string[] { "annapolis", "chambersburg", "clearwater", "frederick", "hagerstown", "lakeforest(russell)", "Lakeforest(355)", "lexingtonpark(lexpark)", "nicholson", "colonial", "wheaton" };
            }

            foreach (var storeId in selectedStores)
            {
                var storeLead = new StoreLeadInformation();

                storeLead.StoreId = storeId;

                var location = SqlQueries.GetLocationCodeByStoreId(storeId);
                if (location == "FMM")
                {
                    location = "FOC";
                }

                storeLead.Location = location;

                foreach(var dealer in Enums.StoreDealerId)
                {
                    if(dealer.StoreId == storeId)
                    {
                        storeLead.DealerId = dealer.Name;
                    }
                }


                var monthYear = leadReportModel.ReportEndDate.Month.ToString() + "/" + leadReportModel.ReportEndDate.Year.ToString();

                //var dealMonth = new DateTime(leadReportModel.ReportEndDate.Year, leadReportModel.ReportEndDate.Month, 1);

                //var associates = SqlMapperUtil.StoredProcWithParams<Associate>("sp_CommissionGetAssociatesByStoreAndDate", new { Location = location, MonthYear = monthYear }, "SalesCommission");

                //if (bReturnDeals)
                //{
                //    foreach (var associate in associates)
                //    {
                //        associate.AssociateDeals = SqlMapperUtil.StoredProcWithParams<AssociateDeals>("sp_CommissionGetAssociatesDealsByIdAndDateRange", new { EmployeeNumber = associate.AssociateNumber, StartDate = leadReportModel.ReportStartDate, EndDate = leadReportModel.ReportEndDate, Location = associate.AssociateLocation }, "SalesCommission");


                //        var dealCounts = new DealCommissionCounts();
                //        dealCounts.DealMonth = leadReportModel.ReportEndDate;
                //        foreach (var deal in associate.AssociateDeals)
                //        {
                //            dealCounts.NewDealCount += deal.NewDealCount;
                //            dealCounts.UsedDealCount += deal.UsedDealCount;
                //            dealCounts.TotalDealCount += (deal.NewDealCount + deal.UsedDealCount);
                //            dealCounts.LeaseCount += deal.LeaseCount;
                //            dealCounts.BPPCount += deal.BPPCount;
                //            dealCounts.FinanceCount += deal.FinanceCount;
                //            dealCounts.ServiceContractCount += deal.ServiceContractCount;
                //            dealCounts.MaintenanceContractCount += deal.MaintenanceContractCount;
                //            dealCounts.GAPCount += deal.GAPCount;
                //            dealCounts.TradeCount += deal.TradeCount;
                //            dealCounts.AftermarketCount += deal.AftermarketCount;
                //        }

                //        associate.AssociateDealCounts = dealCounts;


                //    }
                //}
                //storeLead.Associates = associates;

                StoreLeads.Add(storeLead);

            }
            var associateLeads = SqlMapperUtil.StoredProcWithParams<AssociateLead>("sp_CommissionGetAssociateLeadsByDate", new { StartDate = leadReportModel.ReportStartDate, EndDate = leadReportModel.ReportEndDate }, "ReynoldsData"); //ReportEndDate.AddDays(1)
            //if(leadReportModel.IncludeHandyman == false)
            //{
            //    associateLeads = associateLeads.FindAll(x => !x.LeadSourceName.EndsWith("~"));
            //}

            foreach (var lead in associateLeads)
            {
                if (lead.LeadSourceGroupName == null)
                {
                    lead.LeadSourceGroupName = "";
                }

                if (lead.LeadGroupMapping == null)
                {
                    lead.LeadGroupMapping = "";
                }
                if (lead.VOfInterest_Make == null)
                {
                    lead.VOfInterest_Make = "";
                }
                if (lead.VOfInterest_Model == null)
                {
                    lead.VOfInterest_Model = "";
                }
                if (lead.VOfInterest_StockNumber == null)
                {
                    lead.VOfInterest_StockNumber = "";
                }
                if (lead.VOfInterest_InventoryType == null)
                {
                    lead.VOfInterest_InventoryType = "";
                }
            }

            var associateAppointments = SqlMapperUtil.StoredProcWithParams<AssociateAppointment>("sp_CommissionGetAssociateAppointmentsByDate", new { StartDate = leadReportModel.ReportStartDate, EndDate = leadReportModel.ReportEndDate.AddDays(1) }, "SalesCommission");

            leadReportModel.AssociateLeads = associateLeads;
            leadReportModel.AssociateAppointments = associateAppointments;

            /////////////////////////////////////// - Comparison Dates

            if (leadReportModel.CompareDates)
            {
                var comparisonLeads = SqlMapperUtil.StoredProcWithParams<AssociateLead>("sp_CommissionGetAssociateLeadsByDate", new { StartDate = leadReportModel.ComparisonReportStartDate, EndDate = leadReportModel.ComparisonReportEndDate }, "ReynoldsData"); //ComparisonReportEndDate.AddDays(1)
                //if (leadReportModel.IncludeHandyman == false)
                //{
                //    comparisonLeads = comparisonLeads.FindAll(x => !x.LeadSourceName.EndsWith("~"));
                //}

                foreach (var lead in comparisonLeads)
                {
                    if (lead.LeadSourceGroupName == null)
                    {
                        lead.LeadSourceGroupName = "";
                    }
                    if (lead.LeadGroupMapping == null)
                    {
                        lead.LeadGroupMapping = "";
                    }
                    if (lead.VOfInterest_Make == null)
                    {
                        lead.VOfInterest_Make = "";
                    }
                    if (lead.VOfInterest_Model == null)
                    {
                        lead.VOfInterest_Model = "";
                    }
                    if (lead.VOfInterest_StockNumber == null)
                    {
                        lead.VOfInterest_StockNumber = "";
                    }
                    if (lead.VOfInterest_InventoryType == null)
                    {
                        lead.VOfInterest_InventoryType = "";
                    }
                }

                var comparisonAppointments = SqlMapperUtil.StoredProcWithParams<AssociateAppointment>("sp_CommissionGetAssociateAppointmentsByDate", new { StartDate = leadReportModel.ComparisonReportStartDate, EndDate = leadReportModel.ComparisonReportEndDate.AddDays(1) }, "SalesCommission");
                leadReportModel.ComparisonLeads = comparisonLeads;
                leadReportModel.ComparisonAppointments = comparisonAppointments;
            }


            leadReportModel.StoreLeadInformation = StoreLeads;


            return leadReportModel;
        }

        public static LeadSourceReportModel GetLeadSourceReportByDateAndStore(LeadSourceReportModel leadReportModel)
        {
            var StoreLeads = new List<StoreLeadSourceInformation>();

            var selectedStores = leadReportModel.SelectedStores;

            if (selectedStores.Contains("ALL"))
            {
                selectedStores = new string[] { "9823", "9822", "9825", "9828", "8005", "8006", "9826", "9824", "9821", "9827" };
            }


            foreach (var storeId in selectedStores)
            {
                var storeInfo = new StoreLeadSourceInformation();
                storeInfo.StoreName = "";

                foreach (var store in SalesCommission.Business.Enums.VinStores)
                {
                    if(storeId == store.StoreId)
                    {
                        storeInfo.StoreName = store.Name;
                    }
                }

                storeInfo.StoreId = storeId;
                
                var storeLeads = SqlMapperUtil.StoredProcWithParams<AssociateLead>("sp_CommissionGetSourceLeadsByDateAndStore", new { StartDate = leadReportModel.ReportStartDate, EndDate = leadReportModel.ReportEndDate.AddDays(1), DealerId = storeId }, "ReynoldsData");
                if (leadReportModel.IncludeHandyman == false)
                {
                    storeLeads = storeLeads.FindAll(x => !x.LeadSourceName.EndsWith("~"));
                }

                storeInfo.StoreLeads = storeLeads;
                storeInfo.DealerId = storeId;

                StoreLeads.Add(storeInfo);

            }

            leadReportModel.StoreLeadInformation = StoreLeads;

            return leadReportModel;
        }

        public static List<Associate> GetAssociatesByStoreAndDate(string storeId, int yearId, int monthId)
        {
            var location = SqlQueries.GetLocationCodeByStoreId(storeId);
            if (location == "FMM")
            {
                location = "FOC";
            }

            var monthYear = monthId.ToString() + "/" + yearId.ToString();

            var dealMonth = new DateTime(yearId, monthId, 1);

            var associates = SqlMapperUtil.StoredProcWithParams<Associate>("sp_CommissionGetAssociatesByStoreAndDate", new { Location = location, MonthYear = monthYear }, "SalesCommission");

            if (associates != null)
            {
                foreach (var associate in associates)
                {
                    if (associate.AssociatePayscale == "")
                    {
                        associate.AssociatePayscale = "STDVOL";
                    }

                    associate.AssociateDraws = SqlMapperUtil.StoredProcWithParams<Draw>("sp_CommissionGetAssociatesDrawsByStoreAndDate", new { AssociateSSN = associate.AssociateSSN, MonthYear = monthYear }, "SalesCommission");
                    associate.AssociateBonus = SqlMapperUtil.StoredProcWithParams<Bonus>("sp_CommissionGetAssociatesBonusByStoreAndDate", new { AssociateSSN = associate.AssociateSSN, MonthYear = monthYear }, "SalesCommission");
                    associate.AssociateGoals = SqlMapperUtil.StoredProcWithParams<Goal>("sp_CommissionGetAssociatesGoalsByStoreAndDate", new { AssociateSSN = associate.AssociateSSN, MonthYear = monthYear }, "SalesCommission");

                    associate.AssociateDeals = SqlMapperUtil.StoredProcWithParams<AssociateDeals>("sp_CommissionGetAssociatesDealsByIdAndDate", new { EmployeeNumber = associate.AssociateNumber, ReportDate = dealMonth, Location = associate.AssociateLocation }, "SalesCommission");

                    associate.AssociateHours = SqlMapperUtil.StoredProcWithParams<AssociateHours>("sp_CommissionGetAssociatesHoursByStoreAndDate", new { AssociateNumber = associate.AssociateNumber, ReportDate = dealMonth }, "SalesCommission");

                    associate.AssociateWage = GetDealershipWageByDateAndStore(monthYear, associate.AssociateLocation);


                    var newPayscales = SqlMapperUtil.StoredProcWithParams<Models.NewPayscale>("sp_CommissionGetNewPayscaleByIDAndDate", new { MonthYear = monthYear, PayscaleID = associate.AssociatePayscale }, "SalesCommission");

                    if (newPayscales != null)
                    {
                        associate.AssociatePayscales = newPayscales.FindAll(o => o.ps_PayLevel.Trim() == associate.AssociateLevel.Trim());
                    }

                    associate.AssociatePayscaleSetup = SqlMapperUtil.StoredProcWithParams<Models.NewPayscaleSetup>("sp_CommissionGetNewPayscaleSetupById", new { PayscaleID = associate.AssociatePayscale }, "SalesCommission");


                    var associateDealCounts = new DealCommissionCounts();

                    foreach (var deal in associate.AssociateDeals)
                    {
                        associateDealCounts.NewDealCount += deal.NewDealCount;
                        associateDealCounts.UsedDealCount += deal.UsedDealCount;
                        associateDealCounts.TotalDealCount += (deal.NewDealCount + deal.UsedDealCount);
                        associateDealCounts.LeaseCount += deal.LeaseCount;
                        associateDealCounts.BPPCount += deal.BPPCount;
                        associateDealCounts.FinanceCount += deal.FinanceCount;
                        associateDealCounts.ServiceContractCount += deal.ServiceContractCount;
                        associateDealCounts.MaintenanceContractCount += deal.MaintenanceContractCount;
                        associateDealCounts.GAPCount += deal.GAPCount;
                        associateDealCounts.TradeCount += deal.TradeCount;
                        associateDealCounts.AftermarketCount += deal.AftermarketCount;
                    }

                    associate.AssociateDealCounts = associateDealCounts;

                }
            }

            return associates;
        }

        public static decimal GetDealershipWageByDateAndStore(string monthYear, string locationCode)
        {
            decimal wage = 0;

            var wages = SqlMapperUtil.StoredProcWithParams<RevenueInformation>("sp_CommissionGetDealershipInputsByDateAndStore", new { MonthDate = monthYear, LocationCode = locationCode }, "SalesCommission");
            
            if(wages != null && wages.Count > 0)
            {
                wage = wages[0].HourlyRate;
            }

            return wage;
        }

        public static DealershipModel GetDealershipInputsByDate(DealershipModel dealershipModel)
        {
            var monthYear = dealershipModel.MonthId.ToString() + "/" + dealershipModel.YearId.ToString();

            dealershipModel.DealershipInputs = SqlMapperUtil.StoredProcWithParams<RevenueInformation>("sp_CommissionGetDealershipInputsByDate", new { MonthYear = monthYear }, "SalesCommission");
            
            return dealershipModel;
        }

        public static List<Chargeback> GetChargebacksByStoreAndDate(int monthId, int yearId, string storeId)
        {
            var monthYear = monthId.ToString() + "/" + yearId.ToString();

            var chargebacks = SqlMapperUtil.StoredProcWithParams<Chargeback>("sp_SalesLogGetChargebacksByStoreAndDate", new { StoreId = storeId, MonthYear = monthYear }, "SalesCommission");

            return chargebacks;
        }

        public static List<Chargeback> GetClearwaterChargebacksByDate(int monthId, int yearId, string storeId)
        {
            var monthYear = monthId.ToString() + "/" + yearId.ToString();

            var chargebacks = SqlMapperUtil.StoredProcWithParams<Chargeback>("sp_SalesLogGetClearwaterChargebacksDate", new { StoreId = storeId, MonthYear = monthYear }, "SalesCommission");

            return chargebacks;
        }

        public static List<Chargeback> UpdateChargebacksFromPrevious(int monthId, int yearId, string storeId)
        {
            // Take the current date and create new records for the previous month
            var chargebacks = SqlMapperUtil.StoredProcWithParams<Chargeback>("sp_UpdateChargebacksFromPreviousByDate", new { YearId = yearId, MonthId = monthId, StoreId = storeId }, "SalesCommission");

            return chargebacks;
        }


        public static DealershipModel UpdateDealershipInputsFromPrevious(DealershipModel dealershipModel)
        {
            // Take the current date and create new records for the previous month
            var dealershipInputs = SqlMapperUtil.StoredProcWithParams<RevenueInformation>("sp_UpdateDealershipInputsFromPreviousByDate", new { YearId = dealershipModel.YearId, MonthId = dealershipModel.MonthId }, "SalesCommission");
            dealershipModel.DealershipInputs = dealershipInputs;

            return dealershipModel;
        }

        public static AftermarketInputModel GetAftermarketInputsByDate(AftermarketInputModel aftermarketInputModel)
        {
            var monthYear = aftermarketInputModel.MonthId.ToString() + "/" + aftermarketInputModel.YearId.ToString();

            aftermarketInputModel.AftermarketInputs = SqlMapperUtil.StoredProcWithParams<AftermarketInput>("sp_CommissionGetAftermarketInputsByDate", new { MonthYear = monthYear }, "SalesCommission");

            return aftermarketInputModel;
        }

        public static AftermarketInputModel UpdateAftermarketInputsFromPrevious(AftermarketInputModel aftermarketInputModel)
        {
            // Take the current date and create new records for the previous month
            var aftermarketInputs = SqlMapperUtil.StoredProcWithParams<AftermarketInput>("sp_UpdateAftermarketInputsFromPreviousByDate", new { YearId = aftermarketInputModel.YearId, MonthId = aftermarketInputModel.MonthId }, "SalesCommission");
            aftermarketInputModel.AftermarketInputs = aftermarketInputs;

            return aftermarketInputModel;
        }

        public static int SaveAftermarketInputs(AftermarketInput aftermarketInput)
        {

            //Now save everything to the database and save the files...
            int saveInputs = SqlMapperUtil.InsertUpdateOrDeleteStoredProc("sp_CommissionSaveAftermarketInputsByDate", aftermarketInput, "SalesCommission");

            // End database saving

            return saveInputs;
        }


        public static int SaveAssociateInformation(AssociateUpdate associateInformation)
        {

            //Now save everything to the database and save the files...
            int saveInputs = SqlMapperUtil.InsertUpdateOrDeleteStoredProc("sp_CommissionSaveAssociateByDate", associateInformation, "SalesCommission");

            // End database saving

            return saveInputs;
        }

        public static int SaveAssociateDraws(Draw associateDraw)
        {

            //Now save everything to the database and save the files...
            int saveInputs = SqlMapperUtil.InsertUpdateOrDeleteStoredProc("sp_CommissionSaveAssociateDrawByDate", associateDraw, "SalesCommission");

            // End database saving

            return saveInputs;
        }

        public static int SaveAssociateBonus(Bonus associateBonus)
        {

            //Now save everything to the database and save the files...
            int saveInputs = SqlMapperUtil.InsertUpdateOrDeleteStoredProc("sp_CommissionSaveAssociateBonusByDate", associateBonus, "SalesCommission");

            // End database saving

            return saveInputs;
        }

        public static int UpdateAssociateDraws(Draw associateDraw)
        {

            //Now save everything to the database and save the files...
            int saveInputs = SqlMapperUtil.InsertUpdateOrDeleteStoredProc("sp_CommissionUpdateAssociateDrawByDate", associateDraw, "SalesCommission");

            // End database saving

            return saveInputs;
        }

        public static int UpdateAssociatLevel(string associateSSN, string monthYear, string certificationLevel)
        {

            //Now save everything to the database and save the files...
            int saveInputs = SqlMapperUtil.InsertUpdateOrDeleteStoredProc("sp_CommissionUpdateAssociateLevel", new { AssociateSSN = associateSSN, MonthYear = monthYear, CertificationLevel = certificationLevel } , "SalesCommission");

            // End database saving

            return saveInputs;
        }        

        public static int UpdateAssociateBonus(Bonus associateBonus)
        {

            //Now save everything to the database and save the files...
            int saveInputs = SqlMapperUtil.InsertUpdateOrDeleteStoredProc("sp_CommissionUpdateAssociateBonusByDate", associateBonus, "SalesCommission");

            // End database saving

            return saveInputs;
        }

        public static int DeleteAssociateDraws(Draw associateDraw)
        {

            //Now save everything to the database and save the files...
            int saveInputs = SqlMapperUtil.InsertUpdateOrDeleteStoredProc("sp_CommissionDeleteAssociateDrawByDate", associateDraw, "SalesCommission");

            // End database saving

            return saveInputs;
        }

        public static int DeleteAssociateBonus(Bonus associateBonus)
        {

            //Now save everything to the database and save the files...
            int saveInputs = SqlMapperUtil.InsertUpdateOrDeleteStoredProc("sp_CommissionDeleteAssociateBonusByDate", associateBonus, "SalesCommission");

            // End database saving

            return saveInputs;
        }

        public static int SaveAssociateGoals(Goal associateGoal)
        {

            //Now save everything to the database and save the files...
            int saveInputs = SqlMapperUtil.InsertUpdateOrDeleteStoredProc("sp_CommissionSaveAssociateGoalsByDate", associateGoal, "SalesCommission");

            // End database saving

            return saveInputs;
        }

        public static int SaveChargeback(Chargeback chargeback)
        {

            //Now save everything to the database and save the files...
            int saveInputs = SqlMapperUtil.InsertUpdateOrDeleteStoredProc("sp_SalesLogSaveChargebacks", chargeback, "SalesCommission");

            // End database saving

            return saveInputs;
        }

        public static int SaveDealershipInputs(RevenueInformation revenueInformation)
        {

            //Now save everything to the database and save the files...
            int saveInputs = SqlMapperUtil.InsertUpdateOrDeleteStoredProc("sp_CommissionSaveDealershipInputsByDate", revenueInformation, "SalesCommission");

            // End database saving

            return saveInputs;
        }

        public static int SavePayscale(NewPayscale payscale)
        {

            //Now save everything to the database and save the files...
            int saveInputs = SqlMapperUtil.InsertUpdateOrDeleteStoredProc("sp_CommissionSavePayscaleByDate", payscale, "SalesCommission");

            // End database saving

            return saveInputs;
        }

        public static PayscaleModel GetPayscaleByIDAndDate(PayscaleModel payscaleModel)
        {
            var monthYear = payscaleModel.MonthId.ToString() + "/" + payscaleModel.YearId.ToString();

            //payscaleModel.OldPayscales = SqlMapperUtil.StoredProcWithParams<Models.Payscale>("sp_CommissionGetPayscaleByIDAndDate", new { MonthYear = monthYear, PayscaleID = payscaleModel.PayscaleId }, "SalesCommission");
            payscaleModel.Payscales = SqlMapperUtil.StoredProcWithParams<Models.NewPayscale>("sp_CommissionGetNewPayscaleByIDAndDate", new { MonthYear = monthYear, PayscaleID = payscaleModel.PayscaleId }, "SalesCommission");
            payscaleModel.PayscaleSetup = SqlMapperUtil.StoredProcWithParams<Models.NewPayscaleSetup>("sp_CommissionGetNewPayscaleSetupById", new { PayscaleID = payscaleModel.PayscaleId }, "SalesCommission");
            return payscaleModel;
        }

        public static PayscaleModel GetNewPayscales()
        {
            var monthYear = DateTime.Now.Month.ToString() + "/" + DateTime.Now.Year.ToString();
            var payscaleComparisonModel = new PayscaleModel();
            
            payscaleComparisonModel.Payscales = SqlMapperUtil.SqlWithParams<Models.NewPayscale>("	Select * from CommissionPayscales where ps_MonthYear = @MonthYear and ps_PlanCode like 'COM%'", new { MonthYear = monthYear }, "SalesCommission");
            payscaleComparisonModel.PayscaleSetup = SqlMapperUtil.SqlWithParams<Models.NewPayscaleSetup>("Select * from CommissionPayscaleSetup where ps_PlanCode like '%COM%'", null, "SalesCommission");
            return payscaleComparisonModel;
        }

        public static PayscaleComparisonModel GetPayscalesByDate(PayscaleComparisonModel payscaleComparisonModel)
        {
            var monthYear = payscaleComparisonModel.MonthId.ToString() + "/" + payscaleComparisonModel.YearId.ToString();

            //payscaleModel.OldPayscales = SqlMapperUtil.StoredProcWithParams<Models.Payscale>("sp_CommissionGetPayscaleByIDAndDate", new { MonthYear = monthYear, PayscaleID = payscaleModel.PayscaleId }, "SalesCommission");
            payscaleComparisonModel.Payscales = SqlMapperUtil.StoredProcWithParams<Models.NewPayscale>("sp_CommissionGetNewPayscalesDate", new { MonthYear = monthYear }, "SalesCommission");
            payscaleComparisonModel.PayscaleSetup = SqlMapperUtil.StoredProcWithParams<Models.NewPayscaleSetup>("sp_CommissionGetNewPayscaleSetups", null, "SalesCommission");
            return payscaleComparisonModel;
        }

        public static PayscaleModel UpdatePayscalesFromPrevious(PayscaleModel payscaleModel)
        {
            // Take the current date and create new records for the previous month
            var payscales = SqlMapperUtil.StoredProcWithParams<NewPayscale>("sp_UpdatePayscalesFromPreviousByDate", new { YearId = payscaleModel.YearId, MonthId = payscaleModel.MonthId, PayscaleID = payscaleModel.PayscaleId }, "SalesCommission");            
            payscaleModel.Payscales = payscales;

            var monthYear = payscaleModel.MonthId.ToString() + "/" + payscaleModel.YearId.ToString();

            payscaleModel.OldPayscales = SqlMapperUtil.StoredProcWithParams<Models.Payscale>("sp_CommissionGetPayscaleByIDAndDate", new { MonthYear = monthYear, PayscaleID = payscaleModel.PayscaleId }, "SalesCommission");
            
            payscaleModel.PayscaleSetup = SqlMapperUtil.StoredProcWithParams<Models.NewPayscaleSetup>("sp_CommissionGetNewPayscaleSetupById", new { PayscaleID = payscaleModel.PayscaleId }, "SalesCommission");

            return payscaleModel;
        }

        public static List<AdditionalCommission> GetAdditionalCommissionsByStoreAndDate(AdditionalCommissionModel additionalCommissionModel)
        {
            var monthYear = additionalCommissionModel.MonthId.ToString() + "/" + additionalCommissionModel.YearId.ToString();

            //payscaleModel.OldPayscales = SqlMapperUtil.StoredProcWithParams<Models.Payscale>("sp_CommissionGetPayscaleByIDAndDate", new { MonthYear = monthYear, PayscaleID = payscaleModel.PayscaleId }, "SalesCommission");
            var addtionalCommissions = SqlMapperUtil.StoredProcWithParams<Models.AdditionalCommission>("sp_CommissionGetAdditionalCommissionByStoreAndDate", new { MonthYear = monthYear, StoreId = additionalCommissionModel.StoreId }, "SalesCommission");
            
            return addtionalCommissions;
        }

        public static int SaveAdditionalCommission(AdditionalCommission additionalCommission)
        {

            //Now save everything to the database and save the files...
            int saveInputs = SqlMapperUtil.InsertUpdateOrDeleteStoredProc("sp_CommissionSaveAdditionalCommission", additionalCommission, "SalesCommission");

            // End database saving

            return saveInputs;
        }

        public static List<AdditionalCommission> UpdateAdditionalCommissionsFromPrevious(AdditionalCommissionModel additionalCommissionModel)
        {
            // Take the current date and create new records for the previous month
            var addtionalCommissions = SqlMapperUtil.StoredProcWithParams<AdditionalCommission>("sp_UpdateAdditionalCommissionFromPreviousByDate", new { YearId = additionalCommissionModel.YearId, MonthId = additionalCommissionModel.MonthId, StoreId = additionalCommissionModel.StoreId }, "SalesCommission");

            return addtionalCommissions;
        }

        public static List<LeadMapping> GetLeadMappings()
        {
            var leadMappings = SqlMapperUtil.StoredProcWithParams<LeadMapping>("sp_LeadsGetLeadMappings", null, "SalesCommission");            

            return leadMappings;
        }

        public static List<LeadGroup> GetLeadGroups()
        {
            var leadGroups = SqlMapperUtil.StoredProcWithParams<LeadGroup>("sp_LeadsGetLeadGroups", null, "SalesCommission");

            return leadGroups;
        }

        public static List<LeadMapping> GetAllVINLeadSources()
        {
            var leadMappings = SqlMapperUtil.SqlWithParams<LeadMapping>("Select distinct LeadSourceName, LeadSourceGroupName from [VINSolutions_API].[dbo].[Vin_LeadInfo] where LeadCreatedEastTime > '12/31/2018' ", null, "SQLServer");

            return leadMappings;
        }

        public static int AddLeadSourceMapping(LeadMapping leadMapping)
        {
            //Now save everything to the database and save the files...
           int saveObjStn = SqlMapperUtil.InsertUpdateOrDeleteStoredProc("sp_LeadsAddNewLeadSource", leadMapping, "SalesCommission");

            return saveObjStn;
        }

        public static int SaveLeadSourceMapping(LeadMapping leadMapping)
        {
            //Now save everything to the database and save the files...
            int saveObjStn = SqlMapperUtil.InsertUpdateOrDeleteStoredProc("sp_LeadsSaveLeadSource", leadMapping, "SalesCommission");

            return saveObjStn;
        }

        public static int AddLeadGroup(LeadGroup leadGroup)
        {
            //Now save everything to the database and save the files...
            int saveObjStn = SqlMapperUtil.InsertUpdateOrDeleteStoredProc("sp_LeadsAddNewLeadGroup", leadGroup, "SalesCommission");

            return saveObjStn;
        }


        public static List<SelectListItem> GetPayscales(string location)
        {
            
            var items = new List<SelectListItem>();

            var blankItem = new SelectListItem();
            blankItem.Text = string.Empty;
            blankItem.Value = string.Empty;
            items.Add(blankItem);

            foreach (var payscale in Enums.Payscales)
            {
                var item = new SelectListItem();
                item.Text = payscale.Name;
                item.Value = payscale.PayscaleID;
                items.Add(item);
            }

            return items;
        }

        public static List<SelectListItem> GetNewPayscales(string location)
        {

            var items = new List<SelectListItem>();

            var blankItem = new SelectListItem();
            blankItem.Text = string.Empty;
            blankItem.Value = string.Empty;
            items.Add(blankItem);

            foreach (var payscale in Enums.NewPayscales)
            {
                var item = new SelectListItem();
                item.Text = payscale.Name;
                item.Value = payscale.PayscaleID;
                items.Add(item);
            }

            return items;
        }

        public static List<SelectListItem> GetPayLevels(string location)
        {

            var items = new List<SelectListItem>();

            var blankItem = new SelectListItem();
            blankItem.Text = string.Empty;
            blankItem.Value = string.Empty;
            items.Add(blankItem);

            if (location == "FTN")
            {
                foreach (var payscale in Enums.PayLevelsFTN)
                {
                    var item = new SelectListItem();
                    item.Text = payscale.Name;
                    item.Value = payscale.PayLevelID;
                    items.Add(item);
                }
            }
            else
            {
                foreach (var payscale in Enums.PayLevels)
                {
                    var item = new SelectListItem();
                    item.Text = payscale.Name;
                    item.Value = payscale.PayLevelID;
                    items.Add(item);
                }
            }
            return items;
        }

        public static List<SelectListItem> GetSSI(string location)
        {

            var items = new List<SelectListItem>();

            var blankItem = new SelectListItem();
            blankItem.Text = string.Empty;
            blankItem.Value = string.Empty;
            items.Add(blankItem);

            if (location == "FTN")
            {
                foreach (var payscale in Enums.SSIFTN)
                {
                    var item = new SelectListItem();
                    item.Text = payscale.Name;
                    item.Value = payscale.SSIID;
                    items.Add(item);
                }
            }
            else
            {
                foreach (var payscale in Enums.SSIs)
                {
                    var item = new SelectListItem();
                    item.Text = payscale.Name;
                    item.Value = payscale.SSIID;
                    items.Add(item);
                }
            }
            return items;
        }

        public static List<SelectListItem> GetStatuses(string location)
        {

            var items = new List<SelectListItem>();

            var blankItem = new SelectListItem();
            blankItem.Text = string.Empty;
            blankItem.Value = string.Empty;
            items.Add(blankItem);

            foreach (var payscale in Enums.AssociateStatuses)
            {
                var item = new SelectListItem();
                item.Text = payscale.Name;
                item.Value = payscale.AssociateStatusID;
                items.Add(item);
            }

            return items;
        }
        
        public static List<SelectListItem> GetStoreVolumes(string location)
        {

            var items = new List<SelectListItem>();

            var blankItem = new SelectListItem();
            blankItem.Text = string.Empty;
            blankItem.Value = string.Empty;
            items.Add(blankItem);

            foreach (var payscale in Enums.StoreVolumes)
            {
                var item = new SelectListItem();
                item.Text = payscale.Name;
                item.Value = payscale.StoreVolumeID;
                items.Add(item);
            }

            return items;
        }


        public static CommissionRevenueModel GetAssociateRevenues(CommissionRevenueModel revenueModel)
        {
            var associateRevenues = new List<AssociateRevenue>();

            var locationCode = "";
            //Determine Location Code
            foreach (var location in Enums.StoreLocations)
            {
                if (location.StoreId.ToLower() == revenueModel.StoreId.ToLower())
                {
                    locationCode = location.LocationId;
                }

            }

            var reportDate = revenueModel.MonthId.ToString() + "/" + revenueModel.YearId.ToString();

            revenueModel.RevenueInputs = SqlMapperUtil.StoredProcWithParams<RevenueInformation>("sp_CommissionGetDealershipInputsByDateAndStore", new { MonthDate = reportDate, LocationCode = locationCode }, "SalesCommission");
            var associates = SqlMapperUtil.StoredProcWithParams<Associate>("sp_CommissionGetAssociatesByStoreAndDate", new { Location = locationCode, MonthYear = reportDate }, "SalesCommission");

            revenueModel.AssociateRevenues = new List<AssociateRevenue>();

            foreach (var associate in associates)
            {
                var associateRevenue = new AssociateRevenue();

                associateRevenue.AssociateSSN = associate.AssociateSSN;
                associateRevenue.AssociateLocation = associate.AssociateLocation;
                associateRevenue.AssociateMall = associate.AssociateMall;
                associateRevenue.AssociateNumber = associate.AssociateNumber;
                associateRevenue.AssociateFirstName = associate.AssociateFirstName;
                associateRevenue.AssociateLastName = associate.AssociateLastName;
                associateRevenue.AssociateFullName = associate.AssociateFullName;
                associateRevenue.AssociatePayscale = associate.AssociatePayscale;
                associateRevenue.AssociateLevel = associate.AssociateLevel;
                associateRevenue.AssociateStoreVolume = associate.AssociateStoreVolume;
                associateRevenue.AssociateSSI = associate.AssociateSSI;
                associateRevenue.AssociateStatus = associate.AssociateStatus;
                associateRevenue.AssociateHireDate = associate.AssociateHireDate;
                associateRevenue.AssociateCompetencyDate = associate.AssociateCompetencyDate;
                associateRevenue.AssociateGraduationDate = associate.AssociateGraduationDate;
                associateRevenue.AssociateTerminationDate = associate.AssociateTerminationDate;
                associateRevenue.AssociateRehireDate = associate.AssociateRehireDate;
                associateRevenue.AssociatePosition = associate.AssociatePosition;
                associateRevenue.AssociateDepartment = associate.AssociateDepartment;
                associateRevenue.AssociateDepartmentLocation = associate.AssociateDepartmentLocation;
                associateRevenue.AssociateDepartmentDescription = associate.AssociateDepartmentDescription;

                var reportFullDate = new DateTime(revenueModel.YearId, revenueModel.MonthId, 1);
                var associateDeals = SqlMapperUtil.StoredProcWithParams<AssociateRevenueDeals>("sp_CommissionGetAssociateRevenuesByDate", new { ReportDate = reportFullDate, EmployeeNumber = associateRevenue.AssociateNumber.Trim() }, "SalesCommission");

                associateRevenue.RevenueDeals = associateDeals;

                foreach (var deal in associateDeals)
                {

                    associateRevenue.NewDealCount += deal.NewDealCount;
                    associateRevenue.UsedDealCount += deal.UsedDealCount;
                    associateRevenue.DealCount += (deal.NewDealCount + deal.UsedDealCount);
                    associateRevenue.BPPCount += deal.BPPCount;
                    associateRevenue.FinanceCount += deal.FinanceCount;
                    associateRevenue.ServiceContractCount += deal.ServiceContractCount;
                    associateRevenue.GAPCount += deal.GAPCount;
                    associateRevenue.TradeCount += deal.TradeCount;
                    associateRevenue.AftermarketCount += deal.AftermarketCount;
                    associateRevenue.HoursWorked = deal.HoursWorked;

                }


                revenueModel.AssociateRevenues.Add(associateRevenue);
            }


            return revenueModel;
        }

        public static List<JJFUser> GetJJFUsers()
        {
            var sqlGet = "SELECT ID, Email, UserId, LastName, FirstName, LastName + ', ' + FirstName + ' (' + UserId + ')' as DisplayName, Location, Mall,Showroom,VinUser,VinName,VinStore,VinUserKey, DMS_Id FROM [FITZDB].[dbo].[users] where import = 'Y' order by lastname, firstname";
            var users = SqlMapperUtil.SqlWithParams<JJFUser>(sqlGet, null, "JJFServer");
           
            return users;
        }

        public static List<JJFUser> GetJJFSalesAssociates()
        {
            var sqlGet = "SELECT ID, Email, UserId, LastName, FirstName, LastName + ', ' + FirstName + ' (' + UserId + ')' as DisplayName, Location, Mall,Showroom,VinUser,VinName,VinStore,VinUserKey, DMS_Id FROM [FITZDB].[dbo].[users] where import = 'Y' order by lastname, firstname";
            var users = SqlMapperUtil.SqlWithParams<JJFUser>(sqlGet, null, "JJFServer");

            return users;
        }

        public static List<JJFUser> GetJJFUserByUserId(string userId)
        {
            var users = SqlMapperUtil.StoredProcWithParams<JJFUser>("sp_SalesCommissionGetUserByJJFLogin", new { UserId = userId }, "SalesCommission");

            return users;
        }

        public static List<UserPersmissions> GetUserPermissions(string userId)
        {
            var permissions = SqlMapperUtil.StoredProcWithParams<UserPersmissions>("sp_SecurityGetUserPermissionsByUserId", new { userId = userId}, "SalesCommission");

            return permissions;
        }

        public static string GetSalesAssociatesLocationById(string associateId)
        {
            var locationId = "";
            var sqlGet = "Select emp_loc from ivory.dbo.employees where emp_empnumber = '" + associateId + "'";
            var locations = SqlMapperUtil.SqlWithParams<string>(sqlGet, null, "JJFServer");

            if(locations != null && locations.Count > 0)
            {
                locationId = locations[0];
            }

            return locationId;
        }


    }

}