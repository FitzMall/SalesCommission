using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SalesCommission.Models
{
    public class CommissionRevenueModel
    {
        public string StoreId { get; set; }
        public int MonthId { get; set; }
        public int YearId { get; set; }
        public List<RevenueInformation> RevenueInputs { get; set; }
        public List<AssociateRevenue> AssociateRevenues { get; set; }
    }
     public class CommissionDeal
    {
        public decimal sl_pkey { get; set; }
        public string sl_dealkey { get; set; }
        public decimal sl_serviceContract { get; set; }
        public decimal sl_roadHaz { get; set; }
        public decimal sl_gap { get; set; }
        public decimal sl_insurance { get; set; }
        public decimal sl_etch { get; set; }
        public decimal sl_otheram { get; set; }
        public decimal sl_maint { get; set; }
        public decimal sl_leaseWnT { get; set; }
        public decimal sl_financeInc { get; set; }
        public DateTime sl_dealmonth { get; set; }
        public decimal sl_cashVal { get; set; }
        public decimal sl_cashVal2 { get; set; }
        public string sl_valInternet { get; set; }
       // public string sl_msiYN { get; set; }
       // public string sl_handymanYN { get; set; }
        public decimal sl_paintRepair { get; set; }
        public string sl_BPP { get; set; }
        public string sl_CustomerName { get; set; }
        public string sl_SalesAssociate1 { get; set; }
        public string sl_SalesAssociate2 { get; set; }
        public decimal sl_maintenanceContract { get; set; }
        public string sl_VehicleNU { get; set; }
        public string sl_VehicleLoc { get; set; }
        public decimal sl_VehicleDealNo { get; set; }
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
       // public decimal sl_VehicleMfgRebate { get; set; }
        public string sl_VehicleCategory { get; set; }
        public DateTime sl_VehicleDealDate { get; set; }
       // public string sl_VehicleSalesManager { get; set; }
        public decimal sl_tradeVal { get; set; }
        public decimal sl_tradeVal2 { get; set; }
    }
    public class AssociateRevenue
    {
        public string AssociateSSN { get; set; }
        public string AssociateLocation { get; set; }
        public string AssociateMall { get; set; }
        public string AssociateNumber { get; set; }
        public string AssociateFirstName { get; set; }
        public string AssociateLastName { get; set; }
        public string AssociateFullName { get; set; }
        public string AssociatePayscale { get; set; }
        public string AssociateLevel { get; set; }
        public string AssociateStoreVolume { get; set; }
        public string AssociateSSI { get; set; }
        public string AssociateStatus { get; set; }
        public DateTime AssociateHireDate { get; set; }
        public DateTime AssociateCompetencyDate { get; set; }
        public DateTime AssociateGraduationDate { get; set; }
        public DateTime AssociateTerminationDate { get; set; }
        public DateTime AssociateRehireDate { get; set; }
        public string AssociatePosition { get; set; }
        public string AssociateDepartment { get; set; }
        public string AssociateDepartmentLocation { get; set; }
        public string AssociateDepartmentDescription { get; set; }
        public decimal DealCount { get; set; }
        public decimal NewDealCount { get; set; }
        public decimal UsedDealCount { get; set; }
        public decimal BPPCount { get; set; }
        public decimal FinanceCount { get; set; }
        public decimal ServiceContractCount { get; set; }
        public decimal GAPCount { get; set; }
        public decimal TradeCount { get; set; }
        public decimal AftermarketCount { get; set; }
        public decimal HoursWorked { get; set; }
        public List<AssociateRevenueDeals> RevenueDeals { get; set; }
    }

    public class AssociateRevenueDeals
    {

        public string DealKey { get; set; }
        public decimal DealGross { get; set; }
        public decimal DealHoldback { get; set; }
        public decimal BPPAmount { get; set; }
        public decimal FinanceIncomeAmount { get; set; }
        public decimal ServiceContractAmount { get; set; }
        public decimal MaintenanceContractAmount { get; set; }
        public decimal GAPAmount { get; set; }
        public decimal Trade1Amount { get; set; }
        public decimal Trade2Amount { get; set; }
        public decimal OtherAftermarketAmount { get; set; }
	    public string VehicleCondition { get; set; }
        public string VehicleMake { get; set; }
	    public string SalesAssociate1 { get; set; }
	    public string SalesAssociate2 { get; set; }
        public decimal NewDealCount { get; set; }
        public decimal UsedDealCount { get; set; }
        public decimal BPPCount { get; set; }
        public decimal FinanceCount { get; set; }
        public decimal ServiceContractCount { get; set; }
        public decimal GAPCount { get; set; }
        public decimal TradeCount { get; set; }
        public decimal AftermarketCount { get; set; }
        public decimal HoursWorked { get; set; }

    }

}