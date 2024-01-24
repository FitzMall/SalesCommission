using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SalesCommission.Models
{
    public class AftermarketReportModel
    {
        public string StoreId { get; set; }
        public string[] SelectedBrands { get; set; }
        public string[] SelectedStores { get; set; }
        public string ConditionId { get; set; }
        public int MonthId { get; set; }
        public int YearId { get; set; }
        public bool IncludeHandyman { get; set; }
        public bool IncludeDeals { get; set; }
        public bool ShowOnlyHandyman { get; set; }
        public bool IsAssociateOnly { get; set; }
        public List<AftermarketDealGroup> AftermarketDealGroups { get; set; }
        public List<ObjectivesAndStandards> ObjectivesAndStandards { get; set; }
    }


    public class AftermarketDealGroup
    {
        public string AutoMall { get; set; }
        public List<AftermarketDealDetail> AftermarketDealDetails { get; set; }
    }

    public class PreviousAftermarketDealDetails
    {
        public int MonthId { get; set; }
        public int YearId { get; set; }
        public List<AftermarketDealDetail> AftermarketDealDetails { get; set; }
    }

    public class AftermarketDealDetail
    {
        public string MakeId { get; set; }
        public string MakeName { get; set; }
        public string BrandId { get; set; }    
        public string ModelName { get; set; }        
		public string VehicleCondition { get; set; }
        public string VehicleCategory { get; set; }
        public string VehicleMake { get; set; }
        public string VehicleYear { get; set; }
        public string VehicleCarline { get; set; }
        public string VehicleModelNumber { get; set; }
        public string VehicleStockNumber { get; set; }
        public string VehicleVIN { get; set; }
        public string VehicleDaysInStock { get; set; }
        public int VehicleTerm { get; set; }
        public decimal APR { get; set; }
        public decimal BuyRate { get; set; }
        public string VehicleBank { get; set; }
        public string CertificationLevel { get; set; }
        public string VehicleMiles { get; set; }
        public string DealKey { get; set; }
        public decimal DealGrossAmount { get; set; }
        public decimal BPPAmount { get; set; }
        public decimal NitrogenAmount { get; set; }
        public decimal ZurichAmount { get; set; }
        public decimal TireWheelAmount { get; set; }
        public decimal SecurityAmount { get; set; }        
        public decimal OtherAmount { get; set; }
        public decimal AdjustmentAmount { get; set; }
        public decimal CertFeeAmount { get; set; }
        public decimal GAPAmount { get; set; }
        public decimal VSCAmount { get; set; }
        public decimal FinanceIncomeAmount { get; set; }
        public decimal MaintenanceAmount { get; set; }
        public decimal DealValue { get; set; }
        public string Loaner { get; set; }
        public string BPP { get; set; }
        public string SalesAssociate1 { get; set; }
        public string SalesAssociateId1 { get; set; }
        public string SalesAssociate2 { get; set; }
        public string FandIManager { get; set; }
        public string FinanceManagerNumber { get; set; }
        public string ShowroomValidatedBy { get; set; }
        public string Trade1VIN { get; set; }
        public string Trade2VIN { get; set; }
        public List<AftermarketItem> AftermarketItems { get; set; }
    }

    public class AftermarketRecord
    {
        public string MakeId { get; set; }
        public string MakeName { get; set; }
        public string BrandId { get; set; }
        public string ModelName { get; set; }
        public string VehicleCondition { get; set; }
        public string VehicleCategory { get; set; }
        public string VehicleMake { get; set; }
        public string VehicleYear { get; set; }
        public string VehicleCarline { get; set; }
        public string VehicleModelNumber { get; set; }
        public string VehicleStockNumber { get; set; }
        public string VehicleVIN { get; set; }
        public string VehicleDaysInStock { get; set; }
        public int VehicleTerm { get; set; }
        public decimal APR { get; set; }
        public decimal BuyRate { get; set; }
        public string VehicleBank { get; set; }
        public string CertificationLevel { get; set; }
        public string VehicleMiles { get; set; }
        public string AutoMall { get; set; }
        public string DealKey { get; set; }
        public string SalesAssociate1 { get; set; }
        public string SalesAssociateLastName { get; set; }
        public string SalesAssociateFullName { get; set; }
        public string SalesAssociate2 { get; set; }
        public string FandIManager { get; set; }
        public string FinanceManagerNumber { get; set; }
        public decimal DealGrossAmount { get; set; }
        public decimal BPPAmount { get; set; }
        public decimal NitrogenAmount { get; set; }
        public decimal ZurichAmount { get; set; }
        public decimal TireWheelAmount { get; set; }
        public decimal SecurityAmount { get; set; }
        public decimal OtherAmount { get; set; }
        public decimal AdjustmentAmount { get; set; }
        public decimal CertFeeAmount { get; set; }
        public decimal GAPAmount { get; set; }
        public decimal VSCAmount { get; set; }
        public decimal FinanceIncomeAmount { get; set; }
        public decimal MaintenanceAmount { get; set; }
        public string Loaner { get; set; }
        public string BPP { get; set; }
        public string ShowroomValidatedBy { get; set; }
        public string Trade1VIN { get; set; }
        public string Trade2VIN { get; set; }
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

    }

    public class AftermarketInputModel
    {
        public int MonthId { get; set; }
        public int YearId { get; set; }
        public string PlanId { get; set; }
        public List<AftermarketInput> AftermarketInputs { get; set; }
        public List<SelectListItem> AftermarketPointsSelectList { get; set; }
    }

    public class AftermarketInput
    {
        public Int32 Key { get; set; }
        public string MonthYear { get; set; }
        public Int32 AftermarketFieldId { get; set; }
        public string AftermarketDescription { get; set; }
        public decimal AftermarketPoints { get; set; }
        public string UpdateUser { get; set; }
        public DateTime UpdateDate { get; set; }
        public Int32 AftermarketProfitPerPoint { get; set; }
    }

}