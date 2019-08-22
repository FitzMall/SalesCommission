using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SalesCommission.Models
{
    public class SalesLogReportModel
    {
        public string StoreId { get; set; }
        public string[] SelectedBrands { get; set; }
        public string[] SelectedStores { get; set; }
        public int MonthId { get; set; }
        public int YearId { get; set; }
        public string DealNumber { get; set; }
        public string CustomerName { get; set; }
        public bool IncludeHandyman { get; set; }
        public bool ShowChargebacks { get; set; }
        public List<SalesReportDetail> SalesReportDetails { get; set; }
        public List<ObjectivesAndStandards> ObjectivesAndStandards { get; set; }
        public List<FactoryToDealerCash> FactoryToDealerCash { get; set; }
        public List<Status5> Status5 { get; set; }
        public List<FiscalMonth> FiscalMonth { get; set; }
        public List <Chargeback> Chargebacks { get; set; }
    }

    public class CertifiedCount
    {
        public string MakeId { get; set; }
        public string MakeName { get; set; }
        public string BrandId { get; set; }
        public string AutoMall { get; set; }
        public string CertifiedMake { get; set; }
        public int CertifiedMakeCount { get; set; }
    }

    public class SalesReportDetail
    {
        public string MakeId { get; set; }
        public string MakeName { get; set; }
        public string BrandId { get; set; }
        public string AutoMall { get; set; }
        public int DealCount { get; set; }
        public int UnValidatedCount { get; set; }
        public int OfficeValidatedCount { get; set; }
        public int LeaseCount { get; set; }
        public int HandymanCount { get; set; }
        public int CPOCount { get; set; }
        public int NextCarCount { get; set; }
        public double DealGrossAmount { get; set; }
        public double FinIncAmount { get; set; }
        public double VSCAmount { get; set; }
        public double MCAmount { get; set; }
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
        public double MCCount { get; set; }
        public double FinanceIncCount { get; set; }
        public double BPPCount { get; set; }
        public double AftermarketCount { get; set; }
        public double TradeCount { get; set; }
        public double BPPCollectionPercent { get; set; }
        public List<DealDetail> Deals { get; set; }

    }

    public class BPPCollection
    {
        public string Loc { get; set; }
        public string Mall { get; set; }
        public string Location { get; set; }
        public string VehMake { get; set; }
        public string ConditionAtSold { get; set; }
        public double BPPListAmt { get; set; }
        public double CollectedAmt { get; set; }
    }

    public class DealDetail
    {
        public string MakeId { get; set; }
        public string MakeName { get; set; }
        public string DealKey { get; set; }
        public double DealGrossAmount { get; set; }
        public double FinIncAmount { get; set; }
        public double BankFee { get; set; }
        public string FinInstCode { get; set; }
        public double VSCAmount { get; set; }
        public double MaintenanceContractAmount { get; set; }
        public double FTDAmount { get; set; }
        public double VarianceAmount { get; set; }
        public string VarianceReason { get; set; }
        public double GapAmount { get; set; }
        public double OtherAmount { get; set; }
        public string ValueInternet { get; set; }
        public string BPPAmount { get; set; }
        public string CustomerName { get; set; }
        public string BuyerName { get; set; }
        public string BuyerLastName { get; set; }
        public string SalesAssociate1 { get; set; }
        public string SalesAssociate2 { get; set; }
        public string FinanceManager { get; set; }
        public string SalesManager { get; set; }
        public decimal APR { get; set; }
        public decimal BuyRate { get; set; }
        public string ModelNumber { get; set; }
        public string Carline { get; set; }
        public string CarMake { get; set; }
        public string StockNumber { get; set; }
        public string Year { get; set; }
        public string DaysInStock { get; set; }
        public DateTime DealDate { get; set; }
        public string OfficeValidatedBy { get; set; }
        public DateTime OfficeValidatedDate { get; set; }
        public string ShowroomValidatedBy { get; set; }
        public DateTime ShowroomValidatedDate { get; set; }
        public DateTime ReportDate { get; set; }
        public decimal DealVariance { get; set; }
        public string Category { get; set; }
        public string CertificationLevel { get; set; }
        public double TradeAmount { get; set; }
        public string Location { get; set; }
        
    }
}