using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SalesCommission.Models
{
    public class SalesLogDeal
    {

        public string MakeId { get; set; }
        public string MakeName { get; set; }
        public string DealKey { get; set; }
        public double DealGrossAmount { get; set; }
        public double FinIncAmount { get; set; }
        public string FinInstCode { get; set; }
        public double VSCAmount { get; set; }
        public double FTDAmount { get; set; }
        public double VarianceAmount { get; set; }
        public string VarianceReason { get; set; }
        public double GapAmount { get; set; }
        public double OtherAmount { get; set; }
        public string ValueInternet { get; set; }
        public string CustomerName { get; set; }
        public string SalesAssociate1 { get; set; }
        public string SalesAssociate2 { get; set; }
        public string FinanceManager { get; set; }
        public string ModelNumber { get; set; }
        public string Carline { get; set; }
        public string StockNumber { get; set; }
        public string Year { get; set; }
        public string DaysInStock { get; set; }
        public DateTime DealDate { get; set; }

    }

    public class IndividualDeal
    {
        public IEnumerable<SelectListItem> FinanceSources { get; set; }
        public IEnumerable<SelectListItem> SalesAssociates { get; set; }
        public IEnumerable<SelectListItem> SalesAssociates2 { get; set; }
        public IEnumerable<SelectListItem> FinanceManagers { get; set; }
        public IEnumerable<SelectListItem> ServiceCompanies { get; set; }
        public IEnumerable<SelectListItem> Makes { get; set; }
        public IEnumerable<SelectListItem> Malls { get; set; }
        public string sl_pkey { get; set; }
        public string sl_dealkey { get; set; }
        public string sl_make_id { get; set; }
        public int sl_mall_id { get; set; }
        public decimal sl_dealGross { get; set; }
        public decimal sl_holdback { get; set; }
        public decimal sl_serviceContract { get; set; }
        public decimal sl_maintenanceContract { get; set; }
        public string sl_serviceCompany { get; set; }
        public decimal sl_roadHaz { get; set; }
        public decimal sl_gap { get; set; }
        public decimal sl_insurance { get; set; }
        public decimal sl_etch { get; set; }
        public decimal sl_otheram { get; set; }
        public decimal sl_maint { get; set; }
        public string sl_FinMgr { get; set; }
        public decimal sl_leaseWnT { get; set; }
        public decimal sl_financeInc { get; set; }
        public string sl_financeSrc { get; set; }
        public decimal sl_bankFee { get; set; }
        public decimal sl_ftdpc1 { get; set; }
        public decimal sl_ftdpc2 { get; set; }
        public decimal sl_ftdpc3 { get; set; }
        public decimal sl_ftdpcadj { get; set; }
        public string sl_ftdpc_1 { get; set; }
        public string sl_ftdpc_2 { get; set; }
        public string sl_ftdpc_3 { get; set; }
        public string sl_ftdpc_adj { get; set; }
        public string sl_rebpc1 { get; set; }
        public string sl_rebpc2 { get; set; }
        public string sl_rebpc3 { get; set; }
        public string sl_rebpc4 { get; set; }
        public string sl_rebpc5 { get; set; }
        public string sl_rebpc_1 { get; set; }
        public string sl_rebpc_2 { get; set; }
        public string sl_rebpc_3 { get; set; }
        public string sl_rebpc_4 { get; set; }
        public string sl_rebpc_5 { get; set; }
        public DateTime sl_dealmonth { get; set; }
        public string sl_descOftrade { get; set; }
        public string sl_descOftrade2 { get; set; }
        public string sl_RepeatBuyer { get; set; }
        public string sl_ReferralYN { get; set; }
        public string sl_Referral { get; set; }
        public string sl_appOfVehc { get; set; }
        public decimal sl_cashVal { get; set; }
        public decimal sl_cashVal2 { get; set; }
        public decimal sl_tradeVal { get; set; }
        public decimal sl_tradeVal2 { get; set; }
        public decimal sl_RRDealGross { get; set; }
        public string sl_valInternet { get; set; }
        public decimal sl_dxHoldback { get; set; }
        public DateTime sl_datecreated { get; set; }
        public DateTime sl_dateupdated { get; set; }
        public string sl_enteredby { get; set; }
        public string sl_msiYN { get; set; }
        public string sl_handymanYN { get; set; }
        public string sl_certificationLevel { get; set; }
        public decimal sl_paintRepair { get; set; }
        public string sl_leadSrc { get; set; }
        public decimal sl_AddGross { get; set; }
        public string sl_rate_exception { get; set; }
        public decimal sl_price_variance { get; set; }
        public string sl_price_variance_exception { get; set; }
        public string sl_price_Exception_Comments { get; set; }
        public string sl_price_Exception_Dealer { get; set; }
        public string sl_officeValidatedBy { get; set; }
        public DateTime sl_officeValidatedDate { get; set; }
        public string sl_showroomValidatedBy { get; set; }
        public DateTime sl_showroomValidatedDate { get; set; }
        public string sl_BPP { get; set; }
        public string sl_gapCompany { get; set; }
        public string sl_CustomerName { get; set; }
        public string sl_SalesAssociate1 { get; set; }
        public string sl_SalesAssociate2 { get; set; }
        public string NewUsed { get; set; }
        public string Location { get; set; }
        public string LocationCode { get; set; }
        public string DealNumber  { get; set; }
        public string Make  { get; set; }
        public string Year  { get; set; }
        public string Carline  { get; set; }
        public string ModelNumber  { get; set; }
        public string StockNumer  { get; set; }
        public string VIN  { get; set; }
        public string  CustomerName  { get; set; }
        public string  BuyerLastName  { get; set; }
        public string BuyerName  { get; set; } 
        public DateTime DeliveryDate  { get; set; }
        public int Term  { get; set; }
        public string DaysInStock  { get; set; }
        public decimal SellPriceAmount  { get; set; } 
        public decimal RebateAmount  { get; set; }
        public string Validation { get; set; }
        public string UpdateUser { get; set; }
        public string SalesAssociate1 { get; set; }
        public string SalesAssociate2 { get; set; }
        public string Category { get; set; }
        public decimal sl_buyRate { get; set; }
        public decimal sl_apr { get; set; }
        public decimal sl_SellPrice { get; set; }
        public decimal sl_DealTotal { get; set; }
        public decimal sl_Adjustments { get; set; }
        public decimal sl_CertFee { get; set; }
        public string sl_VehicleNU { get; set; }
        public string sl_VehicleLoc { get; set; }
        public string sl_VehicleDealNo { get; set; }
        public string sl_VehicleMake { get; set; }
        public string sl_VehicleYear { get; set; }
        public string sl_VehicleCarline { get; set; }
        public string sl_VehicleModelNumber { get; set; }
        public string sl_VehicleStockNumber { get; set; }
        public string sl_VehicleVIN { get; set; }
        public string sl_VehicleCustomer { get; set; }
        public string sl_VehicleBuyerLast { get; set; }
        public string sl_VehicleBuyerName { get; set; }
        public DateTime sl_VehicleDeliveryDate { get; set; }
        public int sl_VehicleTerm { get; set; }
        public int sl_VehicleDaysInStock { get; set; }
        public decimal sl_VehicleMfgRebate { get; set; }
        public string sl_VehicleCategory { get; set; }
        public DateTime sl_VehicleDealDate { get; set; }
        public string sl_VehicleSalesManager { get; set; }
        public int sl_VehicleMiles { get; set; }
        public string sl_BankName { get; set; }
        public string sl_TradeVIN { get; set; }
        public string sl_TradeVIN2 { get; set; }
        public string sl_FinanceManagerNumber { get; set; }
        public List<IndividualDealDetails> DealHistory { get; set; }
        public IndividualDealDetails PreviousSavedValues { get; set; }
        public List<AftermarketItem> AftermarketItems { get; set; }
        public DealComments DealComments { get; set; }
        public bool DealFound { get; set; } = false;
    }


    public class IndividualDealDetails
    {
        public string sl_updateuser { get; set; }
        public DateTime sl_updatedate { get; set; }
        public string sl_pkey { get; set; }
        public string sl_dealkey { get; set; }
        public string sl_make_id { get; set; }
        public int sl_mall_id { get; set; }
        public decimal sl_dealGross { get; set; }
        public decimal sl_holdback { get; set; }
        public decimal sl_serviceContract { get; set; }
        public decimal sl_maintenanceContract { get; set; }
        public string sl_serviceCompany { get; set; }
        public decimal sl_roadHaz { get; set; }
        public decimal sl_gap { get; set; }
        public decimal sl_insurance { get; set; }
        public decimal sl_etch { get; set; }
        public decimal sl_otheram { get; set; }
        public decimal sl_maint { get; set; }
        public string sl_FinMgr { get; set; }
        public decimal sl_leaseWnT { get; set; }
        public decimal sl_financeInc { get; set; }
        public string sl_financeSrc { get; set; }
        public decimal sl_bankFee { get; set; }
        public decimal sl_ftdpc1 { get; set; }
        public decimal sl_ftdpc2 { get; set; }
        public decimal sl_ftdpc3 { get; set; }
        public decimal sl_ftdpcadj { get; set; }
        public string sl_ftdpc_1 { get; set; }
        public string sl_ftdpc_2 { get; set; }
        public string sl_ftdpc_3 { get; set; }
        public string sl_ftdpc_adj { get; set; }
        public string sl_rebpc1 { get; set; }
        public string sl_rebpc2 { get; set; }
        public string sl_rebpc3 { get; set; }
        public string sl_rebpc4 { get; set; }
        public string sl_rebpc5 { get; set; }
        public string sl_rebpc_1 { get; set; }
        public string sl_rebpc_2 { get; set; }
        public string sl_rebpc_3 { get; set; }
        public string sl_rebpc_4 { get; set; }
        public string sl_rebpc_5 { get; set; }
        public DateTime sl_dealmonth { get; set; }
        public string sl_descOftrade { get; set; }
        public string sl_descOftrade2 { get; set; }
        public string sl_RepeatBuyer { get; set; }
        public string sl_ReferralYN { get; set; }
        public string sl_Referral { get; set; }
        public string sl_appOfVehc { get; set; }
        public decimal sl_cashVal { get; set; }
        public decimal sl_cashVal2 { get; set; }
        public decimal sl_tradeVal { get; set; }
        public decimal sl_tradeVal2 { get; set; }
        public decimal sl_RRDealGross { get; set; }
        public string sl_valInternet { get; set; }
        public decimal sl_dxHoldback { get; set; }
        public DateTime sl_datecreated { get; set; }
        public DateTime sl_dateupdated { get; set; }
        public string sl_enteredby { get; set; }
        public string sl_msiYN { get; set; }
        public string sl_handymanYN { get; set; }
        public string sl_certificationLevel { get; set; }
        public decimal sl_paintRepair { get; set; }
        public string sl_leadSrc { get; set; }
        public decimal sl_AddGross { get; set; }
        public string sl_rate_exception { get; set; }
        public decimal sl_price_variance { get; set; }
        public string sl_price_variance_exception { get; set; }
        public string sl_price_Exception_Comments { get; set; }
        public string sl_price_Exception_Dealer { get; set; }
        public string sl_officeValidatedBy { get; set; }
        public DateTime sl_officeValidatedDate { get; set; }
        public string sl_showroomValidatedBy { get; set; }
        public DateTime sl_showroomValidatedDate { get; set; }
        public string sl_BPP { get; set; }
        public string sl_gapCompany { get; set; }
        public string sl_CustomerName { get; set; }
        public string sl_SalesAssociate1 { get; set; }
        public string sl_SalesAssociate2 { get; set; }
        public decimal sl_buyRate { get; set; }
        public decimal sl_apr { get; set; }
        public decimal sl_SellPrice { get; set; }
        public decimal sl_DealTotal { get; set; }
        public decimal sl_Adjustments { get; set; }
        public decimal sl_CertFee { get; set; }

        public string sl_VehicleNU { get; set; }
        public string sl_VehicleLoc { get; set; }
        public string sl_VehicleDealNo { get; set; }
        public string sl_VehicleMake { get; set; }
        public string sl_VehicleYear { get; set; }
        public string sl_VehicleCarline { get; set; }
        public string sl_VehicleModelNumber { get; set; }
        public string sl_VehicleStockNumber { get; set; }
        public string sl_VehicleVIN { get; set; }
        public string sl_VehicleCustomer { get; set; }
        public string sl_VehicleBuyerLast { get; set; }
        public string sl_VehicleBuyerName { get; set; }
        public DateTime sl_VehicleDeliveryDate { get; set; }
        public int sl_VehicleTerm { get; set; }
        public int sl_VehicleDaysInStock { get; set; }
        public decimal sl_VehicleMfgRebate { get; set; }
        public string sl_VehicleCategory { get; set; }
        public DateTime sl_VehicleDealDate { get; set; }
        public string sl_VehicleSalesManager { get; set; }
        public int sl_VehicleMiles { get; set; }
        public string sl_BankName { get; set; }
        public string sl_TradeVIN { get; set; }
        public string sl_TradeVIN2 { get; set; }
        public string sl_FinanceManagerNumber { get; set; }
    }
    public class DealComments
    {
        public string Comment { get; set; }
        public string CommentUser { get; set; }
        public DateTime CommentDate { get; set; }
    }
    public class Make
    {
        public string MakeId { get; set; }
        public string MakeName { get; set; }
        public string MallId { get; set; }
        public string MakeCode { get; set; }
    }

    public class Mall
    {
        public string MallId { get; set; }
        public string MallName { get; set; }
        public string MallCode { get; set; }
    }

    public class SalesAssociate
    {
        public string AssociateId { get; set; }
        public string AssociateName { get; set; }
    }
    public class FinanceManager
    {
        public string ManagerId { get; set; }
        public string ManagerName { get; set; }
    }

    public class FinanceSource
    {
        public string SourceId { get; set; }
        public string SourceName { get; set; }

    }

    public class ServiceCompany
    {
        public string CompanyId { get; set; }
        public string CompanyName { get; set; }

    }

    public class Status5
    {
        public string MakeName { get; set; }
        public string NewUsedStatus { get; set; }
        public int VehicleCount { get; set; }
    }

    public class FiscalMonth
    {
        
        public int MonthId { get; set; }
        public int YearId { get; set; }
        public string LocationCode { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CurrentDate { get; set; }
        public int DaysInMonth { get; set; }
        public int CurrentDay { get; set; }
        public decimal Factor { get; set; }

    }

    public class AftermarketItem
    {
        public int AftermarketId { get; set; }
        public string AftermarketName { get; set; }
        public decimal AftermarketCost { get; set; }
        public decimal AftermarketPrice { get; set; }
        public decimal AftermarketPoints { get; set; }
        public int AftermarketProfitPerPoint { get; set; }

    }

    public class AftermarketTable
    {
        public string AFTID { get; set; }
        public string LOC { get; set; }
        public string CUSTNO { get; set; }
        public string DEALNO { get; set; }
        public string DEAL_DATE { get; set; }
        public string CATEGORY { get; set; }
        public string MAKE { get; set; }
        public string MDL_NO { get; set; }
        public string STK { get; set; }
        public string PRICE { get; set; }
        public string VIN { get; set; }
        public string AFTTOTPRICE { get; set; }
        public string LOANER { get; set; }
        public string BPP { get; set; }
        public decimal AFTP1 { get; set; }
        public decimal AFTP2 { get; set; }
        public decimal AFTP3 { get; set; }
        public decimal AFTP4 { get; set; }
        public decimal AFTP5 { get; set; }
        public decimal AFTP6 { get; set; }
        public decimal AFTP7 { get; set; }
        public decimal AFTP8 { get; set; }
        public decimal AFTP9 { get; set; }
        public decimal AFTP10 { get; set; }
        public decimal AFTP11 { get; set; }
        public decimal AFTP12 { get; set; }
        public decimal AFTP13 { get; set; }
        public decimal AFTP14 { get; set; }
        public decimal AFTP15 { get; set; }
        public decimal AFTP16 { get; set; }
        public decimal AFTP17 { get; set; }
        public decimal AFTP18 { get; set; }
        public decimal AFTP19 { get; set; }
        public decimal AFTP20 { get; set; }
        public decimal AFTP21 { get; set; }
        public decimal AFTP22 { get; set; }
        public decimal AFTP23 { get; set; }
        public decimal AFTP24 { get; set; }
        public decimal AFTP25 { get; set; }
        public decimal AFTP26 { get; set; }
        public decimal AFTP27 { get; set; }
        public decimal AFTP28 { get; set; }
        public decimal AFTP29 { get; set; }
        public decimal AFTP30 { get; set; }
        public decimal AFTP31 { get; set; }
        public decimal AFTP32 { get; set; }
        public decimal AFTP33 { get; set; }
        public decimal AFTP34 { get; set; }
        public string LAST_UPDATE { get; set; }
        public string NUO { get; set; }
        public string LOCATION { get; set; }
        public string AFTD1 { get; set; }
        public string AFTD2 { get; set; }
        public string AFTD3 { get; set; }
        public string AFTD4 { get; set; }
        public string AFTD5 { get; set; }
        public string AFTD6 { get; set; }
        public string AFTD7 { get; set; }
        public string AFTD8 { get; set; }
        public string AFTD9 { get; set; }
        public string AFTD10 { get; set; }
        public string AFTD11 { get; set; }
        public string AFTD12 { get; set; }
        public string AFTD13 { get; set; }
        public string AFTD14 { get; set; }
        public string AFTD15 { get; set; }
        public string AFTD16 { get; set; }
        public string AFTD17 { get; set; }
        public string AFTD18 { get; set; }
        public string AFTD19 { get; set; }
        public string AFTD20 { get; set; }
        public string AFTD21 { get; set; }
        public string AFTD22 { get; set; }
        public string AFTD23 { get; set; }
        public string AFTD24 { get; set; }
        public string AFTD25 { get; set; }
        public string AFTD26 { get; set; }
        public string AFTD27 { get; set; }
        public string AFTD28 { get; set; }
        public string AFTD29 { get; set; }
        public string AFTD30 { get; set; }
        public string AFTD31 { get; set; }
        public string AFTD32 { get; set; }
        public string AFTD33 { get; set; }
        public string AFTD34 { get; set; }
        public decimal AFTCOST1 { get; set; }
        public decimal AFTCOST2 { get; set; }
        public decimal AFTCOST3 { get; set; }
        public decimal AFTCOST4 { get; set; }
        public decimal AFTCOST5 { get; set; }
        public decimal AFTCOST6 { get; set; }
        public decimal AFTCOST7 { get; set; }
        public decimal AFTCOST8 { get; set; }
        public decimal AFTCOST9 { get; set; }
        public decimal AFTCOST10 { get; set; }
        public decimal AFTCOST11 { get; set; }
        public decimal AFTCOST12 { get; set; }
        public decimal AFTCOST13 { get; set; }
        public decimal AFTCOST14 { get; set; }
        public decimal AFTCOST15 { get; set; }
        public decimal AFTCOST16 { get; set; }
        public decimal AFTCOST17 { get; set; }
        public decimal AFTCOST18 { get; set; }
        public decimal AFTCOST19 { get; set; }
        public decimal AFTCOST20 { get; set; }
        public decimal AFTCOST21 { get; set; }
        public decimal AFTCOST22 { get; set; }
        public decimal AFTCOST23 { get; set; }
        public decimal AFTCOST24 { get; set; }
        public decimal AFTCOST25 { get; set; }
        public decimal AFTCOST26 { get; set; }
        public decimal AFTCOST27 { get; set; }
        public decimal AFTCOST28 { get; set; }
        public decimal AFTCOST29 { get; set; }
        public decimal AFTCOST30 { get; set; }
        public decimal AFTCOST31 { get; set; }
        public decimal AFTCOST32 { get; set; }
        public decimal AFTCOST33 { get; set; }
        public decimal AFTCOST34 { get; set; }
        public string LOCDEAL { get; set; }
        public string csvupdate { get; set; }
    }

}