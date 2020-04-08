using System;
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


    public class AppraisalDetail
    {

        public string DealerAlias { get; set; }
        public string Id { get; set; }
        public string VIN { get; set; }
        public string VA_Appraiser { get; set; }
        public string DealeRName { get; set; }
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

    }

}