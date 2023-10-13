using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SalesCommission.Models
{
    public class PayscaleModel
    {
        public string PayscaleId { get; set; }
        public int MonthId { get; set; }
        public int YearId { get; set; }
        public string PlanId { get; set; }
        public List<Payscale> OldPayscales { get; set; }
        public List<NewPayscale> Payscales { get; set; }
        public List<NewPayscaleSetup> PayscaleSetup { get; set; }
        public List<SelectListItem> AftermarketPointsSelectList { get; set; }
    }

    public class FIPayscaleModel
    {
        public string PayscaleId { get; set; }
        public int MonthId { get; set; }
        public int YearId { get; set; }
        public List<SelectListItem> FIPayscaleSelectList { get; set; }
        public List<FIPayscale> FIPayscales { get; set; }
        public List<FIPayscaleAftermarket> FIPayscaleAftermarket { get; set; }
        public decimal GrossPercentagePaid { get; set; }
        public decimal MentorPercentagePaid { get; set; }
        public string NewPayscaleName { get; set; }
        public decimal CommissionPercentage { get; set;}
        public decimal ProductBonusPercent1 { get; set; }
        public decimal ProductBonusThreshold1 { get; set; }
        public decimal ProductBonusPercent2 { get; set; }
        public decimal ProductBonusThreshold2 { get; set; }
        public decimal ProductBonusPercent3 { get; set; }
        public decimal ProductBonusThreshold3 { get; set; }
        public decimal ProductBonusPercent4 { get; set; }
        public decimal ProductBonusThreshold4 { get; set; }
        public decimal ProductBonusPercent5 { get; set; }
        public decimal ProductBonusThreshold5 { get; set; }
        public decimal ProductBonusPercent6 { get; set; }
        public decimal ProductBonusThreshold6 { get; set; }
        public decimal ProductBonusPercent7 { get; set; }
        public decimal ProductBonusThreshold7 { get; set; }
        public decimal ProductBonusPercent8 { get; set; }
        public decimal ProductBonusThreshold8 { get; set; }

        public decimal StandardFinancePerUnit { get; set; }
        public decimal StandardFinancePercent { get; set; }
        public decimal StandardServicePerUnit { get; set; }
        public decimal StandardServicePercent { get; set; }
        public decimal StandardMaintenancePerUnit { get; set; }
        public decimal StandardMaintenancePercent { get; set; }
        public decimal StandardGAPPerUnit { get; set; }
        public decimal StandardGAPPercent { get; set; }
        public decimal StandardZurichPerUnit { get; set; }
        public decimal StandardZurichPercent { get; set; }
        public decimal StandardSelectProtectPerUnit { get; set; }
        public decimal StandardSelectProtectPercent { get; set; }
        public decimal StandardTireWheelPerUnit { get; set; }
        public decimal StandardTireWheelPercent { get; set; }
        public string StandardsExpectations1 { get; set; }
        public string StandardsExpectations2 { get; set; }
        public string StandardsExpectations3 { get; set; }
        public string StandardsExpectations4 { get; set; }
        public string StandardsExpectations5 { get; set; }
        public string StandardsExpectations6 { get; set; }
        public string StandardsExpectations7 { get; set; }
        public string StandardsExpectations8 { get; set; }
        public int ActivePayscale { get; set; }
        public int PayscaleWithProducts { get; set; }
    }

    public class FIAdjustmentModel
    {
        public int MonthId { get; set; }
        public int YearId { get; set; }
        public string FinanceManagerNumber { get; set; }
        public Associate AssociateInformation { get; set; }
        public List<SelectListItem> FIManagers { get; set; }
        public List<SelectListItem> AdjustmentType { get; set; }
        public List<SelectListItem> AdjustmentLine { get; set; }
        public List<FIAdjustment> FIAdjustments { get; set; }
        public IEnumerable<SelectListItem> FIDealNumbers { get; set; }

        public int Chargeback1Id { get; set; }
        public string Chargeback1 { get; set; }
        public string Chargeback1Type { get; set; }
        public decimal Chargeback1Amount { get; set; }
        public string Chargeback1Line { get; set; }
        public string Chargeback1Deal { get; set; }

        public int Chargeback2Id { get; set; }
        public string Chargeback2 { get; set; }
        public string Chargeback2Type { get; set; }
        public decimal Chargeback2Amount { get; set; }
        public string Chargeback2Line { get; set; }
        public string Chargeback2Deal { get; set; }


        public int Chargeback3Id { get; set; }
        public string Chargeback3 { get; set; }
        public string Chargeback3Type { get; set; }
        public decimal Chargeback3Amount { get; set; }
        public string Chargeback3Line { get; set; }
        public string Chargeback3Deal { get; set; }


        public int Chargeback4Id { get; set; }
        public string Chargeback4 { get; set; }
        public string Chargeback4Type { get; set; }
        public decimal Chargeback4Amount { get; set; }
        public string Chargeback4Line { get; set; }
        public string Chargeback4Deal { get; set; }


        public int Differences1Id { get; set; }
        public string Differences1 { get; set; }
        public string Differences1Type { get; set; }
        public decimal Differences1Amount { get; set; }
        public string Differences1Line { get; set; }
        public string Differences1Deal { get; set; }

        public int Differences2Id { get; set; }
        public string Differences2 { get; set; }
        public string Differences2Type { get; set; }
        public decimal Differences2Amount { get; set; }
        public string Differences2Line { get; set; }
        public string Differences2Deal { get; set; }

        public int Differences3Id { get; set; }
        public string Differences3 { get; set; }
        public string Differences3Type { get; set; }
        public decimal Differences3Amount { get; set; }
        public string Differences3Line { get; set; }
        public string Differences3Deal { get; set; }

        public int Differences4Id { get; set; }
        public string Differences4 { get; set; }
        public string Differences4Type { get; set; }
        public decimal Differences4Amount { get; set; }
        public string Differences4Line { get; set; }
        public string Differences4Deal { get; set; }


        public int Quality1Id { get; set; }
        public string Quality1 { get; set; }
        public string Quality1Type { get; set; }
        public decimal Quality1Amount { get; set; }
        public string Quality1Line { get; set; }
        public string Quality1Deal { get; set; }

        public int CSI1Id { get; set; }
        public string CSI1 { get; set; }
        public string CSI1Type { get; set; }
        public decimal CSI1Amount { get; set; }
        public string CSI1Line { get; set; }
        public string CSI1Deal { get; set; }

        public int Other1Id { get; set; }
        public string Other1 { get; set; }
        public string Other1Comment { get; set; }
        public string Other1Type { get; set; }
        public decimal Other1Amount { get; set; }
        public string Other1Line { get; set; }
        public string Other1Deal { get; set; }

    }

    public class FIAdjustment
    {
        public int Id { get; set; }
        public string MonthYear { get; set; }
        public string AssociateNumber { get; set; }
        public string Adjustment { get; set; }
        public string AdjustmentType { get; set; }
        public decimal AdjustmentAmount { get; set; }
        public string AdjustmentLine { get; set; }
        public string DealNumber { get; set; }
          public string Comments { get; set; }
          public string UpdateUser { get; set; }
          public DateTime UpdateDate { get; set; }

    }

    public class PayscaleComparisonModel
    {
        public string StoreId { get; set; }
        public int MonthId { get; set; }
        public int YearId { get; set; }
        public string[] PayscaleId { get; set; }
        public List<Associate> Associates { get; set; }
        public List<ManufacturerSpiff> ManufacturerSpiffs { get; set; }
        public List<NewPayscale> Payscales { get; set; }
        public List<NewPayscaleSetup> PayscaleSetup { get; set; }
    }

    public class PayscaleComparisonAllModel
    {
        public string StoreId { get; set; }
        public string StoreName { get; set; }
        public string[] PayscaleId { get; set; }
        public int MonthId { get; set; }
        public int YearId { get; set; }
        public List<ManufacturerSpiff> ManufacturerSpiffs { get; set; }
        public List<NewPayscale> Payscales { get; set; }
        public List<NewPayscaleSetup> PayscaleSetup { get; set; }
        public List<PayscaleComparisonModel> StoreComparisons { get; set; }
    }

    public class NewPayscale
    {
        // Fields from the CommissionPayscale Table that holds saved values
        public int ps_Key { get; set; }
        public string ps_PlanCode { get; set; }
        public string ps_PayLevel { get; set; }
        public string ps_NewUsedHandy { get; set; }
        public string ps_MonthYear { get; set; }
        public DateTime ps_AddDate { get; set; }
        public string ps_AddUser { get; set; }
        public DateTime ps_UpdateDate { get; set; }
        public string ps_UpdateUser { get; set; }
        public decimal ps_BaseCommission { get; set; }
        public decimal ps_FullBPP { get; set; }
        public decimal ps_HalfBPP { get; set; }
        public decimal ps_TradeIn { get; set; }
        public decimal ps_FinanceLease { get; set; }
        public decimal ps_ServiceContract { get; set; }
        public decimal ps_Maintenance { get; set; }
        public decimal ps_GAP { get; set; }
        public decimal ps_AftermarketPerItem { get; set; }
        public decimal ps_InternalSurvey { get; set; }
        public decimal ps_ManufacturerSalesSatisfaction { get; set; }
        public decimal ps_ManufacturerSpiffGuarantee { get; set; }
        public decimal ps_LessServiceContracts { get; set; }
        public decimal ps_VolumeBonusLevel1 { get; set; }
        public decimal ps_VolumeBonusLevel2 { get; set; }
        public decimal ps_VolumeBonusLevel3 { get; set; }
        public decimal ps_VolumeBonusLevel4 { get; set; }
        public decimal ps_VolumeBonusLevel5 { get; set; }
        public decimal ps_VolumeBonusLevel6 { get; set; }
        public decimal ps_VolumeBonusLevel7 { get; set; }
        public decimal ps_VolumeBonusLevel8 { get; set; }
        public decimal ps_VolumeBonusLevel9 { get; set; }
        public string ps_AftermarketPlanId { get; set; }
    }
    public class NewPayscaleSetup
    {
        // Fields from the CommissionPayscaleSetup Table that determines what to show
        public int ps_DetailKey { get; set; }
        public string ps_PlanCode { get; set; }
        public string ps_PayLevel { get; set; }
        public string ps_NewUsedHandy { get; set; }
        public string ps_PayFields { get; set; }
        public decimal ps_VolumeBonusLevel1Low { get; set; }
        public decimal ps_VolumeBonusLevel1High { get; set; }
        public decimal ps_VolumeBonusLevel2Low { get; set; }
        public decimal ps_VolumeBonusLevel2High { get; set; }
        public decimal ps_VolumeBonusLevel3Low { get; set; }
        public decimal ps_VolumeBonusLevel3High { get; set; }
        public decimal ps_VolumeBonusLevel4Low { get; set; }
        public decimal ps_VolumeBonusLevel4High { get; set; }
        public decimal ps_VolumeBonusLevel5Low { get; set; }
        public decimal ps_VolumeBonusLevel5High { get; set; }
        public decimal ps_VolumeBonusLevel6Low { get; set; }
        public decimal ps_VolumeBonusLevel6High { get; set; }
        public decimal ps_VolumeBonusLevel7Low { get; set; }
        public decimal ps_VolumeBonusLevel7High { get; set; }
        public decimal ps_VolumeBonusLevel8Low { get; set; }
        public decimal ps_VolumeBonusLevel8High { get; set; }
        public decimal ps_VolumeBonusLevel9Low { get; set; }
        public decimal ps_VolumeBonusLevel9High { get; set; }
        public string ps_PlanName { get; set; }
        public string ps_AftermarketPlanId { get; set; }
    }

    public class FIPayscale
    {
        public int  Id  { get; set; }
        public string  MonthYear  { get; set; }
        public string PlanCode  { get; set; }
        public string PlanName  { get; set; }
        public string Associate  { get; set; }
        public string PayType  { get; set; }
        public string PayTypeCode  { get; set; }
        public string PayTypeName   { get; set; }
	    public int  Order { get; set; }
	    public decimal  LevelAmount  { get; set; }
	    public decimal PayAmount  { get; set; }
	    public DateTime  UpdateDate  { get; set; }
	    public string  UpdateUser  { get; set; }

    }

    public class FIPayscaleAftermarket
    {
        public int Id { get; set; }
        public string MonthYear { get; set; }
        public string PlanCode { get; set; }
        public string PlanName { get; set; }
        public int AftermarketOrder { get; set; }
        public string AftermarketItem { get; set; }
        public int AftermarketIndex { get; set; }
        public bool CoreItem { get; set; }
        public bool PaidItem { get; set; }

    }

    public class FIPayscaleSetup
    {
        public int Id { get; set; }
        public string MonthYear { get; set; }
        public string PlanCode { get; set; }
        public string PlanName { get; set; }
        public decimal GrossPercentagePaid { get; set; }
        public decimal MentorPercentagePaid { get; set; }
        public decimal CommissionPercentage { get; set; }
        public decimal ProductBonusPercent1 { get; set; }
        public decimal ProductBonusThreshold1 { get; set; }
        public decimal ProductBonusPercent2 { get; set; }
        public decimal ProductBonusThreshold2 { get; set; }
        public decimal ProductBonusPercent3 { get; set; }
        public decimal ProductBonusThreshold3 { get; set; }
        public decimal ProductBonusPercent4 { get; set; }
        public decimal ProductBonusThreshold4 { get; set; }
        public decimal ProductBonusPercent5 { get; set; }
        public decimal ProductBonusThreshold5 { get; set; }
        public decimal ProductBonusPercent6 { get; set; }
        public decimal ProductBonusThreshold6 { get; set; }
        public decimal ProductBonusPercent7 { get; set; }
        public decimal ProductBonusThreshold7 { get; set; }
        public decimal ProductBonusPercent8 { get; set; }
        public decimal ProductBonusThreshold8 { get; set; }
        public decimal StandardFinancePerUnit { get; set; }
        public decimal StandardFinancePercent { get; set; }
        public decimal StandardServicePerUnit { get; set; }
        public decimal StandardServicePercent { get; set; }
        public decimal StandardMaintenancePerUnit { get; set; }
        public decimal StandardMaintenancePercent { get; set; }
        public decimal StandardGAPPerUnit { get; set; }
        public decimal StandardGAPPercent { get; set; }
        public decimal StandardZurichPerUnit { get; set; }
        public decimal StandardZurichPercent { get; set; }
        public decimal StandardSelectProtectPerUnit { get; set; }
        public decimal StandardSelectProtectPercent { get; set; }
        public decimal StandardTireWheelPerUnit { get; set; }
        public decimal StandardTireWheelPercent { get; set; }
        public string StandardsExpectations1 { get; set; }
        public string StandardsExpectations2 { get; set; }
        public string StandardsExpectations3 { get; set; }
        public string StandardsExpectations4 { get; set; }
        public string StandardsExpectations5 { get; set; }
        public string StandardsExpectations6 { get; set; }
        public string StandardsExpectations7 { get; set; }
        public string StandardsExpectations8 { get; set; }
        public int ActivePayscale { get; set; }
        public int PayscaleWithProducts { get; set; }
    }

    public class Payscale
    {
      public int ps_pkey { get; set; }
      public DateTime ps_psdate { get; set; }
      public string ps_Level { get; set; }
      public string ps_NewUsed { get; set; }
      public string ps_MonthYear { get; set; }
      public decimal ps_ValuePrice { get; set; }
      public decimal ps_ValuePriceC2 { get; set; }
      public decimal ps_ValuePriceC3 { get; set; }
      public decimal ps_ValuePriceC4 { get; set; }
      public decimal ps_InternetPrice { get; set; }
      public decimal ps_InternetPriceC2 { get; set; }
      public decimal ps_InternetPriceC3 { get; set; }
      public decimal ps_Handyman { get; set; }
      public decimal ps_InternalCSISurvey { get; set; }
      public decimal ps_TradeIn { get; set; }
      public decimal ps_FinanceRetailLease { get; set; }
      public decimal ps_ServiceContract { get; set; }
      public decimal ps_Lojack { get; set; }
      public decimal ps_GAP { get; set; }
      public decimal ps_CreditLifeAH { get; set; }
      public decimal ps_SecureGuard { get; set; }
      public decimal ps_Aftermarket { get; set; }
      public decimal ps_ManuSpiffGuar { get; set; }
      public decimal ps_Simoniz { get; set; }
      public decimal ps_Ecp { get; set; }
      public decimal ps_Etch_vp { get; set; }
      public decimal ps_Etch_ip { get; set; }
      public decimal ps_EtchC2_vp { get; set; }
      public decimal ps_EtchC2_ip { get; set; }
      public decimal ps_Nitrogen_vp { get; set; }
      public decimal ps_Nitrogen_ip { get; set; }
      public decimal ps_NitrogenC2_vp { get; set; }
      public decimal ps_NitrogenC2_ip { get; set; }
      public decimal ps_VBDelHigh_1_5 { get; set; }
      public decimal ps_VBDelHigh_6_10 { get; set; }
      public decimal ps_VBDelHigh_11_15 { get; set; }
      public decimal ps_VBDelHigh_16_20 { get; set; }
      public decimal ps_VBDelHigh_21_25 { get; set; }
      public decimal ps_VBDelHigh_26_30 { get; set; }
      public decimal ps_VBDelHigh_31_99 { get; set; }
      public decimal ps_VBDelLow_1_4 { get; set; }
      public decimal ps_VBDelLow_5_8 { get; set; }
      public decimal ps_VBDelLow_9_12 { get; set; }
      public decimal ps_VBDelLow_13_16 { get; set; }
      public decimal ps_VBDelLow_17_20 { get; set; }
      public decimal ps_VBDelLow_21_24 { get; set; }
      public decimal ps_VBDelLow_25_99 { get; set; }
      public decimal ps_ManuSalesSat { get; set; }
      public decimal ps_ManuSalesSatNo { get; set; }
      public string ps_Loc { get; set; }
      public decimal ps_SoldVehicle { get; set; }
      public decimal ps_BPP_Loaner { get; set; }
      public decimal ps_Data_Dots { get; set; }
      public decimal ps_Cars_Level1 { get; set; }
      public decimal ps_Cars_Level2 { get; set; }
      public decimal ps_Cars_Level3 { get; set; }
      public decimal ps_Cars_Level4 { get; set; }
      public decimal ps_Cars_Level5 { get; set; }
      public decimal ps_Cars_Level6 { get; set; }
      public decimal ps_Cars_Level7 { get; set; }
      public decimal ps_90_93_Percent { get; set; }
      public decimal ps_93_96_Percent { get; set; }
      public decimal ps_96_100_Percent { get; set; }
      public decimal ps_New_In_Stock { get; set; }
      public decimal ps_New_DX { get; set; }
      public decimal ps_Used_Vehicle { get; set; }
      public decimal ps_Fitzway_Advantage { get; set; }
      public decimal ps_Finance_Deal { get; set; }
      public decimal ps_Accessories_Chemical { get; set; }
      public decimal ps_Over_120_Used { get; set; }
      public decimal ps_Purchased_Appraisal { get; set; }
      public decimal ps_Cars_Level1_Count { get; set; }
      public decimal ps_Cars_Level2_Count { get; set; }
      public decimal ps_Cars_Level3_Count { get; set; }
      public decimal ps_Cars_Level4_Count { get; set; }
      public decimal ps_Cars_Level5_Count { get; set; }
      public decimal ps_Cars_Level6_Count { get; set; }
      public decimal ps_Cars_Level7_Count { get; set; }
    }

}