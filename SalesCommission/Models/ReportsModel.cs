﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SalesCommission.Models
{
    public class ReportsModel
    {
        public string StoreId { get; set; }
        public int MonthId { get; set; }
        public int YearId { get; set; }
        public bool IncludeHandyman { get; set; }
        public List<ObjectivesAndStandardsDetails> ObjectivesAndStandardsDetails { get; set; }
    }

    public class PaidBonusModel
    {
        public int MonthId { get; set; }
        public int YearId { get; set; }
        public List<PaidBonus> PaidBonuses { get; set; }
    }

    public class PaidBonus
    {
        public string Store { get; set; }
        public string AssociateSSN { get; set; }
        public string AssociateName { get; set; }
        public string MonthYear { get; set; }
        public DateTime SaveDate { get; set; }
        public string SaveUser { get; set; }
        public decimal Amount { get; set; }
        public string Comments { get; set; }
    }

    public class MonthlySalesLogReportModel
    {
        public string StoreId { get; set; }
        public string[] SelectedBrands { get; set; }
        public string[] SelectedStores { get; set; }
        public int MonthId { get; set; }
        public int YearId { get; set; }
        public string DealNumber { get; set; }
        public string CustomerName { get; set; }
        public bool IncludeHandyman { get; set; }
        public bool ShowOnlyHandyman { get; set; }
        public bool ShowTransfers { get; set; }
        public List<SalesReportDetail> SalesReportDetails { get; set; }
        public List<MonthlySalesReportDetail> MonthlySalesReportDetails { get; set; }
        public List<ObjectivesAndStandards> ObjectivesAndStandards { get; set; }
        public List<FactoryToDealerCash> FactoryToDealerCash { get; set; }
        public List<Status5> Status5 { get; set; }
        public List<FiscalMonth> FiscalMonth { get; set; }
    }

    public class MonthlySalesReportDetail
    {
        public string AutoMallName { get; set; }
        public int OrderId { get; set; }
        public List<SalesReportDetail> SalesReportDetails { get; set; }
    }

    public class ObjectivesAndStandardsDetails
    {
        public string MakeId { get; set; }
        public string MakeName { get; set; }
        public string BrandId { get; set; }
        public string AutoMall { get; set; }
        public int DealCount { get; set; }
        public double DealGrossAmount { get; set; }
        public double FinIncAmount { get; set; }
        public double VSCAmount { get; set; }
        public double FTDAmount { get; set; }
        public double EtchAmount { get; set; }
        public double GapAmount { get; set; }
        public double OtherAmount { get; set; }
        public int VarianceCount { get; set; }
        public double DXAmount { get; set; }
        public int DealGrossCount { get; set; }
        public int HoldbackCount { get; set; }
        public double HoldbackAmount { get; set; }
        public double TotalAmount { get; set; }
        public double PVRAmount { get; set; }
        public double ValuePercentage { get; set; }
        public double VariancePercentage { get; set; }
        public double GAPCount { get; set; }
        public double VSCCount { get; set; }
        public double FinanceIncCount { get; set; }
        public double BPPCount { get; set; }
        public double AftermarketCount { get; set; }
        public double TradeCount { get; set; }
        public double BPPCollectionPercent { get; set; }
    }

    public class StoreGroupDeals
    {
        public string StoreGroupName { get; set; }
        public List<AssociateNameWithDeals> AssociateDeals { get; set; }
    }

    public class AssociateNameWithDeals
    {
        public string AssociateName { get; set; }
        public List<AssociateTradeReport> AssociateTradeReports { get; set; }
    }

    public class ExceptionReportModel
    {
        public int ReportStartMonth { get; set; }
        public int ReportStartYear { get; set; }
        public int ReportEndMonth { get; set; }
        public int ReportEndYear { get; set; }

        public string BreakDownLevel1 { get; set; }
        public string BreakDownLevel2 { get; set; }
        public string BreakDownLevel3 { get; set; }
        public string BreakDownLevel4 { get; set; }
        public string DealType { get; set; }
        public List<IndividualDealDetails> ExceptionDetails { get; set; }

    }

    public class ExceptionReportDetailModel
    {
        public int ReportStartMonth { get; set; }
        public int ReportStartYear { get; set; }
        public int ReportEndMonth { get; set; }
        public int ReportEndYear { get; set; }

        public string BreakDownLevel1 { get; set; }
        public string BreakDownLevel2 { get; set; }
        public string BreakDownLevel3 { get; set; }
        public string BreakDownLevel4 { get; set; }
        public string BreakDownLevel1Value { get; set; }
        public string BreakDownLevel2Value { get; set; }
        public string BreakDownLevel3Value { get; set; }
        public string BreakDownLevel4Value { get; set; }


        public string DealType { get; set; }
        public List<IndividualDealDetails> ExceptionDetails { get; set; }

    }


    public class AppraisalReportModel
    {
        public string StoreId { get; set; }
        public string[] SelectedStores { get; set; }
        public int ReportStartMonth { get; set; }
        public int ReportStartYear { get; set; }
        public int ReportEndMonth { get; set; }
        public int ReportEndYear { get; set; }
        public string ReportType { get; set; }
        public List<AppraisalDetail> AppraisalDetails { get; set; }
        public List<AppraisalSoldDetail> AppraisalSoldDetails { get; set; }
        public List<TradeAcquisitionDetail> TradeAcquisitionDetails { get; set; }
        public int ReportComparisonStartMonth { get; set; }
        public int ReportComparisonStartYear { get; set; }
        public int ReportComparisonEndMonth { get; set; }
        public int ReportComparisonEndYear { get; set; }
        public List<AppraisalDetail> CompareAppraisalDetails { get; set; }
        public List<AppraisalSoldDetail> CompareAppraisalSoldDetails { get; set; }
        public List<TradeAcquisitionDetail> CompareTradeAcquisitionDetails { get; set; }
        public int ReportComboStartMonth { get; set; }
        public int ReportComboStartYear { get; set; }
        public int ReportComboEndMonth { get; set; }
        public int ReportComboEndYear { get; set; }

        public bool CompareDates { get; set; }
        public string BreakDownLevel1 { get; set; }
        public string BreakDownLevel2 { get; set; }
        public string BreakDownLevel3 { get; set; }
        public string BreakDownLevel4 { get; set; }

        public string BreakDown1Filters { get; set; }
        public string BreakDown2Filters { get; set; }
        public string BreakDown3Filters { get; set; }
        public string BreakDown4Filters { get; set; }

        public string VehicleType { get; set; }
        public string AcquisitionType { get; set; }
        public string StatusType { get; set; }
        public string StatusOnFM { get; set; }
    }

    public class AppraisalReportDetailModel
    {
        public string StoreId { get; set; }
        public string[] SelectedStores { get; set; }
        public int ReportStartMonth { get; set; }
        public int ReportStartYear { get; set; }
        public int ReportEndMonth { get; set; }
        public int ReportEndYear { get; set; }
        public string BreakDownLevel1 { get; set; }
        public string BreakDownLevel2 { get; set; }
        public string BreakDownLevel3 { get; set; }
        public string BreakDownLevel4 { get; set; }
        public string BreakDownLevel1Value { get; set; }
        public string BreakDownLevel2Value { get; set; }
        public string BreakDownLevel3Value { get; set; }
        public string BreakDownLevel4Value { get; set; }
        public string VehicleType { get; set; }
        public string AcquisitionType { get; set; }
        public string StatusType { get; set; }
        public string StatusOnFM { get; set; }
        public List<AppraisalDetail> AppraisalDetails { get; set; }
        public List<AppraisalSoldDetail> AppraisalSoldDetails { get; set; }
        public List<TradeAcquisitionDetail> TradeAcquisitionDetails { get; set; }
        public List<vAutoInventory> vAutoInventoryDetails { get; set; }
    }


    public class SalesReportModel
    {
        public string StoreId { get; set; }
        public string[] SelectedStores { get; set; }
        public int ReportStartMonth { get; set; }
        public int ReportStartYear { get; set; }
        public int ReportEndMonth { get; set; }
        public int ReportEndYear { get; set; }
        public List<SalesReportDetail> SalesReportDetails { get; set; }
        public List<MonthlySalesReportDetail> MonthlySalesReportDetails { get; set; }
        public List<ObjectivesAndStandards> ObjectivesAndStandards { get; set; }
        public List<FactoryToDealerCash> FactoryToDealerCash { get; set; }
        public List<Status5> Status5 { get; set; }
        public List<FiscalMonth> FiscalMonth { get; set; }

        public int ReportComparisonStartMonth { get; set; }
        public int ReportComparisonStartYear { get; set; }
        public int ReportComparisonEndMonth { get; set; }
        public int ReportComparisonEndYear { get; set; }
        public List<SalesReportDetail> CompareSalesReportDetails { get; set; }
        public List<MonthlySalesReportDetail> CompareMonthlySalesReportDetails { get; set; }
        public List<ObjectivesAndStandards> CompareObjectivesAndStandards { get; set; }
        public List<FactoryToDealerCash> CompareFactoryToDealerCash { get; set; }
        public List<Status5> CompareStatus5 { get; set; }
        public List<FiscalMonth> CompareFiscalMonth { get; set; }


        public bool IncludeHandyman { get; set; }
        public bool CompareDates { get; set; }
        public bool ShowOnlyHandyman { get; set; }
        public bool ExcludeBadDuplicates { get; set; }
        public bool ExcludeAllBad { get; set; }
        public bool ShowExcludedGroups { get; set; }
        public string BreakDownLevel1 { get; set; }
        public string BreakDownLevel2 { get; set; }
        public string BreakDownLevel3 { get; set; }
        public string BreakDownLevel4 { get; set; }
        public string VehicleType { get; set; }
    }
    public class LeadReportModel
    {
        public string StoreId { get; set; }
        public string[] SelectedStores { get; set; }
        public DateTime ReportStartDate { get; set; }
        public DateTime ReportEndDate { get; set; }
        public DateTime ComparisonReportStartDate { get; set; }
        public DateTime ComparisonReportEndDate { get; set; }
        public bool IncludeHandyman { get; set; }
        public bool CompareDates { get; set; }
        public bool ShowOnlyHandyman { get; set; }
        public bool ExcludeBadDuplicates { get; set; }
        public bool ExcludeAllBad { get; set; }
        public bool ShowExcludedGroups { get; set; }
        public string BreakDownLevel1 { get; set; }
        public string BreakDownLevel2 { get; set; }
        public string BreakDownLevel3 { get; set; }
        public string BreakDownLevel4 { get; set; }
        public string VehicleType { get; set; }
        public List<StoreLeadInformation> StoreLeadInformation { get; set; }
        public List<AssociateLead> AssociateLeads { get; set; }
        public List<AssociateAppointment> AssociateAppointments { get; set; }
        public List<AssociateLead> ComparisonLeads { get; set; }
        public List<AssociateAppointment> ComparisonAppointments { get; set; }
    }

    public class LeadReportDetailsModel
    {
        public string StoreId { get; set; }
        public string[] SelectedStores { get; set; }
        public DateTime ReportStartDate { get; set; }
        public DateTime ReportEndDate { get; set; }
        public bool IncludeHandyman { get; set; }
        public bool ExcludeBadDuplicates { get; set; }
        public bool ExcludeAllBad { get; set; }
        public bool ShowExcludedGroups { get; set; }
        public string BreakDownLevel1 { get; set; }
        public string BreakDownLevel2 { get; set; }
        public string BreakDownLevel3 { get; set; }
        public string BreakDownLevel4 { get; set; }
        public string BreakDownLevel1Value { get; set; }
        public string BreakDownLevel2Value { get; set; }
        public string BreakDownLevel3Value { get; set; }
        public string BreakDownLevel4Value { get; set; }
        public string VehicleType { get; set; }
        public List<StoreLeadInformation> StoreLeadInformation { get; set; }
        public List<AssociateLead> AssociateLeads { get; set; }

    }

    public class LeadSourceReportModel
    {
        public string StoreId { get; set; }
        public string[] SelectedStores { get; set; }
        public DateTime ReportStartDate { get; set; }
        public DateTime ReportEndDate { get; set; }
        public bool IncludeHandyman { get; set; }
        public bool ShowOnlyHandyman { get; set; }
        public List<StoreLeadSourceInformation> StoreLeadInformation { get; set; }
    }

    public class AppraisalReportLeadDetailsModel
    {
        public string StockNumber { get; set; }
        public string VIN { get; set; }
        public List<AssociateLead> AssociateLeads { get; set; }
        public List<StoreLeadSourceInformation> StoreLeadInformation { get; set; }

    }

    public class StoreLeadSourceInformation
    {
        public string StoreId { get; set; }
        public string StoreName { get; set; }
        public string DealerId { get; set; }
        public List<AssociateLead> StoreLeads { get; set; }

    }

    public class StoreLeadInformation
    {
        public string StoreId { get; set; }
        public string Location { get; set; }
        public string DealerId { get; set; }
        public List<Associate> Associates { get; set; }

    }

    public class TradeReportModel
    {
        public string StoreId { get; set; }
        public string[] SelectedBrands { get; set; }
        public string[] SelectedStores { get; set; }
        public int MonthId { get; set; }
        public int YearId { get; set; }
        public bool IncludeHandyman { get; set; }
        public bool IncludeDeals { get; set; }
        public bool ShowOnlyHandyman { get; set; }
        public List<TradeReportDetail> TradeReportDetails { get; set; }
        public List<AssociateTradeReport> AssociateTradeReports { get; set; }
        public List<AppraisalsOld> AssociateAppraisalsOld { get; set; }
        public List<AppraisalDetail> AssociateAppraisals { get; set; }
        public List<OpportunityDetail> AssociateOpportunities { get; set; }
        public List<AssociateUserGroups> AssociateUserGroups { get; set; }
        public List<AssociateUserGroups> StoreUserGroups { get; set; }
    }

    public class TradeReportDetail
    {
        public string BrandId { get; set; }
        public string AutoMall { get; set; }
        public int AutoMallId { get; set; }
        public int DealerId { get; set; }
        public List<TradeReport> TradeReports { get; set; }
        public List<TradeDeal> TradeDeals { get; set; }

    }

    public class Appraisals
    {
        public string Location { get; set; }
        public string DealerID { get; set; }
        public string DealerName { get; set; }
        public string SalesAssociateName { get; set; }
        public int TotalAppraisals { get; set; }
    }

    public class Opportunities
    {
        public string DealerID { get; set; }
        public string DealerName { get; set; }
        public string SalesAssociateID { get; set; }
        public string SalesAssociateName { get; set; }
        public string UserGroupName { get; set; }
        public int TotalOpportunities { get; set; }
    }

    public class AssociateUserGroups
    {
        public string DealerID { get; set; }
        public string DealerName { get; set; }
        public string SalesAssociateID { get; set; }
        public string SalesAssociateName { get; set; }
        public string SalesAssociateLastName { get; set; }
        public string UserGroupName { get; set; }

    }

    public class AppraisalsOld
    {
        public string loc { get; set; }
        public string DealerName { get; set; }
        public string DealerID { get; set; }
        public string SalesPerson { get; set; }
        public int TotOpp { get; set; }
        public int TotAppraisals { get; set; }
        public int TotTrades { get; set; }
    }

    public class TradeDeal
    {
        public string DealKey { get; set; }
        public string VehicleCategory { get; set; }
        public DateTime DealDate { get; set; }
        public string MakeId { get; set; }
        public int MallId { get; set; }
        public string SalesAssociate1Id { get; set; }
        public string SalesAssociate2Id { get; set; }
        public string VehicleYear { get; set; }
        public string VehicleMake { get; set; }
        public string VehicleModel { get; set; }
        public string VehicleNewUsed { get; set; }
        public string TradeDescription1 { get; set; }
        public decimal TradeValue1 { get; set; }
        public decimal TradeCashValue1 { get; set; }
        public string TradeDescription2 { get; set; }
        public decimal TradeValue2 { get; set; }
        public decimal TradeCashValue2 { get; set; }
        public string Location { get; set; }
        public string MakeName { get; set; }

    }
    public class TradeReport
    {
        public string MakeId { get; set; }
        public string MakeName { get; set; }
        public string BrandId { get; set; }
        public string AutoMall { get; set; }
        public int AutoMallId { get; set; }
        public int DealCount { get; set; }
        public int LeaseDealCount { get; set; }
        public int RetailDealCount { get; set; }
        public int TotalTradeCount { get; set; }
        public int LeaseTradeCount { get; set; }
        public int RetailTradeCount { get; set; }
    }

    public class AssociateTradeReport
    {
        public string SalesAssociateId { get; set; }
        public string SalesAssociateName { get; set; }
        public string VehicleNewUsed { get; set; }
        public int MallId { get; set; }
        public int DealCount { get; set; }
        public int LeaseDealCount { get; set; }
        public int RetailDealCount { get; set; }
        public int TotalTradeCount { get; set; }
        public int LeaseTradeCount { get; set; }
        public int RetailTradeCount { get; set; }
    }

    public class AssociatePerformanceModel
    {
        public string MonthId { get; set; }
        public string YearId { get; set; }
        public string AssociateName { get; set; }
        public string Location { get; set; }
        public List<AppraisalDetail> AssociateAppraisals { get; set; }
        public List<OpportunityDetail> AssociateOpportunities { get; set; }
    }

    public class FitzwaySoldAppraisal
    {
        public string CustomerId { get; set; }
        public string CustomerLastName { get; set; }
        public string SLD_VIN { get; set; }
        public DateTime SLD_SoldUTCDate { get; set; }
        public string TitleNames { get; set; }
        public DateTime AppraisalDate { get; set; }

    }

    public class FitzwayAppraisal
    {
        public string Loc { get; set; }
        public string CreatedBy { get; set; }
        public DateTime AppraisalDate { get; set; }
        public string Status { get; set; }
        public int StepId { get; set; }
        public string SalePerson { get; set; }
        public string Mall { get; set; }
        public string Showroom { get; set; }
        public int Year { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string VinNumber { get; set; }
    }

    public class AppraisalDetail
    {

        public string DealerAlias { get; set; }
        public string Id { get; set; }
        public string VIN { get; set; }
        public string VA_Appraiser { get; set; }
        public string DealerName { get; set; }
        public string Loc { get; set; }
        public DateTime VA_LastModifiedDate { get; set; }
        public decimal ReconditioningCost { get; set; }
        public decimal AppraisedValue { get; set; }
        public int TotAppraisals { get; set; }
        public int TotTrades { get; set; }
        public string CarSource { get; set; }
        public DateTime LoadDate { get; set; }
        public string Year { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string Purchase { get; set; }
        public decimal ProfitObj { get; set; }
        public decimal adjustedPctMarket { get; set; }
        public string vRank { get; set; }
        public decimal AskingPrice { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerHomePhone { get; set; }
        public string SalesPerson { get; set; }
    }

    public class AppraisalSoldDetail
    {
        public string Location { get; set; }
        public string SoldLocation { get; set; }
        public string Appraiser { get; set; }
        public string SalesAssociate { get; set; }
        public string VIN { get; set; }
        public string StockNumber { get; set; }
        public string DaysInStock { get; set; }
        public string Year { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public decimal ReconditioningCost { get; set; }
        public decimal ActualReconditioningCost { get; set; }
        public decimal AppraisedValue { get; set; }
        public decimal AskingPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal SellingLocation { get; set; }
        public decimal ProfitObjective { get; set; }
        public decimal AdjustedPctMarket { get; set; }
        public decimal FrontEndProfit { get; set; }
        public decimal BackEndProfit { get; set; }
        public decimal AverageGrossProfit { get; set; }
        public decimal SL_FrontEndProfit { get; set; }
        public decimal SL_BackEndProfit { get; set; }
        public decimal SL_AverageGrossProfit { get; set; }
        public string DealNumber { get; set; }
        public string WholesaleRetail { get; set; }
        public string TradePurchase { get; set; }
        public string VehicleSource { get; set; }
        public string Certification { get; set; }
        public string ZipCode { get; set; }
        public string State { get; set; }
        public string Miles { get; set; }
        public string Showroom { get; set; }
        public DateTime DealMonthYear { get; set; }
        public DateTime DealDate { get; set; }
        public string BodyStyle { get; set; }
        public int TradeCount { get; set; }
    }

    public class OpportunityDetail
    {
        public string DealerId { get; set; }
        public string VisitId { get; set; }
        public string LeadId { get; set; }
        public DateTime StartUTCDate { get; set; }
        public DateTime StartLocalDate { get; set; }
        public string SalesRepUserId { get; set; }
        public string Sales_LastName { get; set; }
        public string Sales_FirstName { get; set; }
        public int BeBack { get; set; }
        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }

    }

    public class MoneyDueModel
    {
        public List<MoneyDue> MoneyDue { get; set; }
        public List<MoneyDue> MoneyDueHistory { get; set; }
        public IEnumerable<SelectListItem> FIManagers { get; set; }
        public string FIManagerNumber { get; set; }
        public string ReportFilter { get; set; }
        public List<JJFUser> JJFUsers { get; set; }
    }

    public class MoneyDue {
        public string Location { get; set; }
        public string LocationName { get; set; }
        public string StockNumber { get; set; }
        public int ScheduleDays { get; set; }
        public DateTime DealDate { get; set; }
        public string DueFrom { get; set; }
        public string CustomerNumber { get; set; }
        public decimal ControlBalance { get; set; }
        public string CustomerName { get; set; }
        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }
        public string BusinessPhone { get; set; }
        public string ResidencePhone { get; set; }
        public string SalesManager { get; set; }
        public string FIManager { get; set; }
        public string BankName { get; set; }
        public string FIManagerNumber { get; set; }
        public string SalesManagerNumber { get; set; }
        public string DealNumber { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public string Comment { get; set; }
        public DateTime CommentDate { get; set; }
        public string CommentUser { get; set; }
        public int CommentOrder { get; set; }
        public string RootCause { get; set; }
        public string FundedStatus { get; set; }
        public bool EmailSent { get; set; }
        public string EmailAddresses { get; set; }
    }


    public class VehicleData
    {
        public string StockNumber { get; set; }
        public string Location { get; set; }
        public string ModelYear { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string VIN { get; set; }
        public int DaysInStock { get; set; }
        public int InventoryStatus { get; set; }
    }

    public class TradeInformation
    {
        public string Dealkey { get; set; }
        public string FinanceManager { get; set; }
        public string SalesAssociate1 { get; set; }
        public string SalesAssociate2 { get; set; }
        public DateTime DealDate { get; set; }
        public string BuyerName { get; set; }
        public string SalesManager { get; set; }
    }

    public class TitleDueStatusModel
    {
        public TitleDue TitleDue { get; set; }
        public List<TitleDue> TitleDueHistory { get; set; }

    }

    public class TitleDueModel
    {
        public List<TitleDue> TitleDue { get; set; }
        public List<TitleDue> TitleDueHistory { get; set; }
        public List<JJFUser> JJFUsers { get; set; }
        public string loc { get; set; }
        public string[] status { get; set; }
        public string[] invstatus { get; set; }
        public string action { get; set; }
    }

    public class TitleDue
    {
        public int Id { get; set; }
        public int DealId { get; set; }
        public string DealKey { get; set; }
        public DateTime DealDate { get; set; }
        public string Location { get; set; }
        public string LocationName { get; set; }
        public string BuyerName { get; set; }
        public string BuyerLastName { get; set; }
        public string SalesAssociate1 { get; set; }
        public string SalesAssociate2 { get; set; }
        public string FinanceManager { get; set; }
        public string SalesManager { get; set; }
        public string SalesAssociate1Name { get; set; }
        public string SalesAssociate2Name { get; set; }
        public string FinanceManagerName { get; set; }
        public string SalesManagerName { get; set; }
        public string VIN { get; set; }
        public bool ClearTitle { get; set; }
        public bool TitleDueBank { get; set; }
        public bool? TitleDueCustomer { get; set; }
        public bool LienDueCustomer { get; set; }
        public bool TitleDueInterco { get; set; }
        public bool TitleDueAuction { get; set; }
        public bool LienDueBank { get; set; }
        public bool OdomDueCustomer { get; set; }
        public bool POADueCust { get; set; }
        public bool PayoffDueCust { get; set; }
        public bool WaitingOutSTTitle { get; set; }
        public bool DuplicateTitleAppliedFor { get; set; }
        public bool Other { get; set; }
        public string Notes { get; set; }
        public DateTime UpdateDate { get; set; }
        public string UpdateUser { get; set; }
        public string StockNumber { get; set; }
        public int Year { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string Comments { get; set; }
        public int InventoryStatus { get; set; }
        public string BankName { get; set; }
        public string BuyerPhone { get; set; }
        public string BuyerEmail { get; set; }
        public bool NoTitleDispose { get; set; }
        public bool ElectronicTitle { get; set; }
        public string FinanceManagerId { get; set; }
        public string SalesManagerId { get; set; }
        public bool EmailSent { get; set; }
        public string EmailAddresses { get; set; }
    }


    public class AppraiserReportModel
    {
        public int MonthId { get; set; }
        public int YearId { get; set; }
        public string AppraiserName { get; set; }
        public List<AppraiserDetail> AppraiserDetails { get; set; }
    }

    public class AppraiserDetail
    {
        public string Appraiser { get; set; }
        public DateTime AppraisalDate { get; set; }
        public string Mall { get; set; }
        public string Showroom { get; set; }
        public string Location { get; set; }
        public int Status { get; set; }
        public string Days { get; set; }
        public string StockNumber { get; set; }
        public string VIN { get; set; }
        public string Year { get; set; }
        public string Make { get; set; }
        public string Carline { get; set; }
        public int Miles { get; set; }
        public decimal InitialACV { get; set; }
        public decimal InventoryAmount { get; set; }

        public string MSI { get; set; }
        public decimal Cost { get; set; }
        public decimal ListAmount { get; set; }
        public DateTime PurchasedDate { get; set; }
        public string PurchasedBy { get; set; }
        public string PurchasedUnder { get; set; }
        public string PurchasedFrom { get; set; }
        public decimal PurchasedPrice { get; set; }
        public decimal Offer2Purchase { get; set; }
        public string PrintOfferYN { get; set; }

        public string DealKey { get; set; }
        public decimal FrontGross { get; set; }
    }

    public class TradeAcquisitionReportModel
    {

        public List<TradeAcquisitionDetail> TradeAcquisitionDetails { get; set; }
    }

    public class TradeAcquisitionDetail
    {
        //from sp_SalesLogReportGetInventoryAcquisitionReport 
        public string Branch { get; set; }
        public string Loc { get; set; }
        public string DRloc { get; set; }
        public int Status { get; set; }
        public string Days { get; set; }
        public DateTime ReceivedDate { get; set; }
        public string StockNumber { get; set; }
        public string VIN { get; set; }
        public string Year { get; set; }
        public string Make { get; set; }
        public string Carline { get; set; }
        public int Miles { get; set; }
        public decimal InitialACV { get; set; }
        public decimal InventoryAmount { get; set; }
        public string Appraiser { get; set; }
        public string MSI { get; set; }
        public decimal Cost { get; set; }
        public decimal ListAmount { get; set; }
        public DateTime PurchasedDate { get; set; }
        public string PurchaseBy { get; set; }
        public string PurchasedUnder { get; set; }
        public string PurchasedFrom { get; set; }
        public decimal PurchasedPrice { get; set; }
        public DateTime AppraisalDate { get; set; }
        public string AppraisalAppraiser { get; set; }
        public decimal Offer2Purchase { get; set; }
        public string PrintOfferYN { get; set; }
        public string DealKey1 { get; set; }
        public decimal Trade1Value { get; set; }
        public string Trade1VIN { get; set; }
        public string Trade1Customer { get; set; }
        public string DealKey2 { get; set; }
        public decimal Trade2Value { get; set; }
        public string Trade2VIN { get; set; }
        public string Trade2Customer { get; set; }
        public string LeadSourceName { get; set; }
        public string LeadGroup { get; set; }
        public string VehicleSource { get; set; }
        public decimal ReconditioningCost { get; set; }
        public decimal ActualReconditioningCost { get; set; }
        public decimal AppraisedValue { get; set; }
        public decimal AskingPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal SellingLocation { get; set; }
        public decimal ProfitObjective { get; set; }
        public decimal AdjustedPctMarket { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string Certification { get; set; }
        public string StyleId { get; set; }
        public string BodyStyle { get; set; }
        public string vRank { get; set; }
        public string XrefId { get; set;}
        public int LeadCount { get; set; }
        public int ActiveLeadCount { get; set; }
        public DateTime LastActiveLeadDate { get; set; }
    }

    public class vAutoInventory
    {
        public string VIN { get; set; }
        public string StockNumber { get; set; }
        public decimal UnitCost { get; set; }
        public decimal AverageListPrice { get; set; }
        public int AverageOdometer { get; set; }
        public decimal CostToMarket { get; set; }
        public decimal EffectiveCostToMarket { get; set; }
        public decimal EffectivePercentOfMarket { get; set; }
        public int ExactDaySupply { get; set; }
        public int Rank { get; set; }
        public int Size { get; set; }
        public int VRank { get; set; }
        public int YearMakeModelDaySupply { get; set; }
        public DateTime DownloadDateTime { get; set; }
    }

    public class VehiclePriceChange
    {
        public DateTime PriceDate { get; set; }
        public string StockNumber { get; set; }
        public string Location { get; set; }
        public int DaysInventory { get; set; }
        public decimal ListAmount { get; set; }
    }

}