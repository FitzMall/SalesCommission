using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SalesCommission.Models
{

    public class CommissionModel
    {
        public string StoreId { get; set; }
        public int MonthId { get; set; }
        public int YearId { get; set; }
        public List<Associate> Associates { get; set; }
        public List<ManufacturerSpiff> ManufacturerSpiffs { get; set; }
        public List<DealApproval> DealApprovals { get; set; }
        public List<AssociateLead> AssociateLeads { get; set; }
        public List<MoneyDue> MoneyDue { get; set; }
        public List<TitleDue> TitleDue { get; set; }
    }

    public class FICommissionModel
    {
        public string StoreId { get; set; }
        public int MonthId { get; set; }
        public int YearId { get; set; }
        public bool IncludeDeals { get; set; }
        public string ConditionFilter { get; set; }
        public List<Associate> FIManagers { get; set; }
        public List<ObjectivesAndStandards> ObjectivesAndStandards { get; set; }
        public List<AftermarketDealDetail> AftermarketDealDetails { get; set; }
        public List<FIManagerDealDetails> FIManagerDealDetails { get; set; }
        public List<MoneyDue> MoneyDue { get; set; }
        public List<TitleDue> TitleDue { get; set; }
        public List<FIDealApproval> DealApprovals { get; set; }
        public List<FIPayscaleSetup> FIPayscales { get; set; }
    }

    public class FIManagerDealDetails
    {
        public string FIManagerName { get; set; } 
        public string FIManagerLastName { get; set; }
        public string FIManagerLocation { get; set; }
        public string FIManagerAssociateNumber { get; set;}
        public string FIManagerSSN { get; set; }
        public string FIDepartmentCode { get; set; }
        public string FIDepartmentDescription { get; set; }
        public DateTime FIManagerHireDate { get; set; }    
        public string FIManagerPayscale { get; set; }
        public List<AftermarketDealDetail> AftermarketDealDetails { get; set; }
    }

    public class AdditionalCommissionModel
    {
        public string StoreId { get; set; }
        public int MonthId { get; set; }
        public int YearId { get; set; }
        public List<AdditionalCommission> AdditionalCommissions { get; set; }        

    }

    public class AssociateCommissionModel
    {
        public string AssociateId { get; set; }
        public int MonthId { get; set; }
        public int YearId { get; set; }
        public Associate AssociateInformation { get; set; }
        public List<ManufacturerSpiff> ManufacturerSpiffs { get; set; }
        public List<DealApproval> DealApprovals { get; set; }
        public AssociateScoreCard CurrentScorecard { get; set; }
        public List<AssociateScoreCard> AssociateScorecardHistory { get; set; }
        public List<AssociateScoreCard> PreviousAssociateScorecards { get; set; }
        public List<ObjectivesAndStandards> StoreObjectivesStandards { get; set; }
        public List<Associate> AssociateList { get; set; }
        public List<MoneyDue> MoneyDue { get; set; }
        public List<TitleDue> TitleDue { get; set; }
    }

    public class FIAssociateCommissionModel
    {
        public string AssociateId { get; set; }
        public int MonthId { get; set; }
        public int YearId { get; set; }
        public Associate AssociateInformation { get; set; }
        public FIAssociateScoreCard CurrentScorecard { get; set; }
        public List<FIAssociateScoreCard> ScorecardHistory { get; set; }
        public List<AftermarketDealDetail> AftermarketDealDetails { get; set; }
        public List<PreviousAftermarketDealDetails> PreviousAftermarketDealDetails { get; set; }
        public List<FIDealApproval> DealApprovals { get; set; }
        public List<FIPayscale> FIPayscales { get; set; }
        public List<MoneyDue> MoneyDue { get; set; }
        public List<TitleDue> TitleDue { get; set; }
        public List<MoneyDue> MoneyDueHistory { get; set; }
        public List<FIPayscaleAftermarket> FIPayscaleAftermarket { get; set; }
        public List<FIAdjustment> FIAdjustments { get; set; }
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
        public decimal ManagerSalary { get; set; }
        public List<Associate> FIManagerList { get; set;}
    }

    public class AssociatePayscaleComparisonModel
    {
        public string StoreId { get; set; }
        public string AssociateId { get; set; }
        public int MonthId { get; set; }
        public int YearId { get; set; }
        public IEnumerable<SelectListItem> SalesAssociates { get; set; }
        public Associate AssociateInformation { get; set; }
        public List<ManufacturerSpiff> ManufacturerSpiffs { get; set; }
        public PayscaleModel NewPayscales { get; set; }
        public List<Associate> AllAssociateInformation { get; set; }
        public List<AssociateLevel> AssociateLevelHistory { get; set; }

    }

    public class AdditionalCommission
    {
      public int Id { get; set; }
      public string StoreId { get; set; }
      public string MonthYear { get; set; }
      public string MakeCode { get; set; }
      public string MakeName { get; set; }
      public string ModelName { get; set; }
      public decimal AdditionalCommissionAmount{ get; set; }
      public DateTime UpdateDate { get; set; }
      public string UpdateUser { get; set; }
    }

    public class AssociateLevel
    {
        public string AssociateSSN { get; set; }
        public string MonthYear { get; set; }
        public string AssociateCertificationLevel { get; set; }
        public string AssociatePayscale { get; set; }
    }

    public class FIAssociateScoreCard
    {
        public int pkey { get; set; }
        public string AssociateSSN { get; set; }
        public string EmployeeNumber { get; set; }
        public string MonthYear { get; set; }
        public string Rolling3Month_Comments { get; set; }
        public string Deliveries_Comments { get; set; }
        public string FinancePercent_Comments { get; set; }
        public string FinanceReserve_Comments { get; set; }
        public string VSCPercent_Comments { get; set; }
        public string VSCPerItem_Comments { get; set; }
        public string MaintenancePercent_Comments { get; set; }
        public string MaintenancePerItem_Comments { get; set; }
        public string GAPPercent_Comments { get; set; }
        public string GAPPerItem_Comments { get; set; }
        public string ZurichPercent_Comments { get; set; }
        public string ZurichPerItem_Comments { get; set; }
        public string SelectProtectPercent_Comments { get; set; }
        public string SelectProtectPerItem_Comments { get; set; }
        public string TireAndWheelPercent_Comments { get; set; }
        public string TireAndWheelPerItem_Comments { get; set; }
        public string LeasePercent_Comments { get; set; }
        public string LeasePerItem_Comments { get; set; }
        public string OtherPercent_Comments { get; set; }
        public string OtherPerItem_Comments { get; set; }
        public string TotalGross_Comments { get; set; }
        public string TotalDollarPerDeal_Comments { get; set; }
        public string TotalProductDollar_Comments { get; set; }
        public string TotalProductDollerPerDeal_Comments { get; set; }
        public string ProductRation_Comments { get; set; }
        public string KeyImprovementOpportunities { get; set; }
        public decimal Rolling3Month_Standard { get; set; }
        public decimal Deliveries_Standard { get; set; }
        public decimal FinancePercent_Standard { get; set; }
        public decimal FinanceReserve_Standard { get; set; }
        public decimal VSCPercent_Standard { get; set; }
        public decimal VSCPerItem_Standard { get; set; }
        public decimal MaintenancePercent_Standard { get; set; }
        public decimal MaintenancePerItem_Standard { get; set; }
        public decimal GAPPercent_Standard { get; set; }
        public decimal GAPPerItem_Standard { get; set; }
        public decimal ZurichPercent_Standard { get; set; }
        public decimal ZurichPerItem_Standard { get; set; }
        public decimal SelectProtectPercent_Standard { get; set; }
        public decimal SelectProtectPerItem_Standard { get; set; }
        public decimal TireAndWheelPercent_Standard { get; set; }
        public decimal TireAndWheelPerItem_Standard { get; set; }
        public decimal LeasePercent_Standard { get; set; }
        public decimal LeasePerItem_Standard { get; set; }
        public decimal OtherPercent_Standard { get; set; }
        public decimal OtherPerItem_Standard { get; set; }
        public decimal TotalGross_Standard { get; set; }
        public decimal TotalDollarPerDeal_Standard { get; set; }
        public decimal TotalProductDollar_Standard { get; set; }
        public decimal TotalProductDollerPerDeal_Standard { get; set; }
        public decimal ProductRation_Standard { get; set; }
        public DateTime UpdateDate { get; set; }
        public string UpdateUser { get; set; }
        public DateTime FinalizeDate { get; set; }
        public string FinalizeUser { get; set; }
        public DateTime ApprovalDate { get; set; }
        public string ApprovalUser { get; set; }
    }

    public class AssociateScoreCard
    {
        public string AssociateSSN { get; set; }
        public string MonthYear { get; set; }
        public string Rolling3MonthComments { get; set; }
        public decimal Rolling3MonthActual { get; set; }
        public int Rolling3MonthCount { get; set; }
        public string DeliveriesComments { get; set; }
        public decimal DeliveriesActual { get; set; }
        public int DeliveriesCount{ get; set; }
        public string BPPComments { get; set; }
        public decimal BPPActual { get; set; }
        public int BPPCount { get; set; }
        public string VSCComments { get; set; }
        public decimal VSCActual { get; set; }
        public int VSCCount { get; set; }
        public string TradeComments { get; set; }
        public decimal TradeActual { get; set; }
        public int TradeCount { get; set; }
        public string FinanceComments { get; set; }
        public decimal FinanceActual { get; set; }
        public int FinanceCount { get; set; }        
        public string LeaseComments { get; set; }
        public decimal LeaseActual { get; set; }
        public int LeaseCount { get; set; }
        public string AftermarketComments { get; set; }
        public decimal AftermarketActual { get; set; }
        public int AftermarketCount { get; set; }
        public string SurveyComments { get; set; }
        public decimal SurveyActual { get; set; }
        public int SurveyCount { get; set; }
        public string SatisfactionActual { get; set; }
        public string SatisfactionObjective { get; set; }
        public string SatisfactionComments { get; set; }
        public string TrainingComments { get; set; }
        public string FandIProcessComments { get; set; }
        public string FitzwayProcessComments { get; set; }
        public string CalculatedLevel { get; set; }
        public string ApprovedLevel { get; set; }
        public DateTime UpdateDate { get; set; }
        public string UpdateUser { get; set; }
        public DateTime FinalizeDate { get; set; }
        public string FinalizeUser { get; set; }
        public string TrainingYN { get; set; }
        public string FandIProcessYN { get; set; }
        public string FitzwayProcessYN { get; set; }
        public string SatisfactionActual3Month { get; set; }
        public string SatisfactionObjective3Month { get; set; }
        public string MeetsSSIObjective { get; set; }
        public string OverrideComments { get; set; }
        public DateTime ApprovalDate { get; set; }
        public string ApprovalUser { get; set; }
    }

    public class PayLevelCalculation
    {
        public string pl_AssociateSSN { get; set; }
        public DateTime pl_UpdateDate { get; set; }
        public string pl_PayLevel { get; set; }
        public string pl_PayScale { get; set; }
        public string pl_CalculatedPayLevel { get; set; }
        public decimal pl_3MonthUnits { get; set; }
        public string pl_Training { get; set; }
        public string pl_MeetsSSI { get; set; }
        public string pl_FitzwayProcess { get; set; }
        public decimal pl_BPPPercentage { get; set; }
        public decimal pl_SVCPercentage { get; set; }
    }
    public class DealApproval
    {
        public string DealKey { get; set; }
        public string ApprovalUser { get; set; }
        public DateTime ApprovalDate { get; set; }
    }

    public class FIDealApproval
    {
        public int ApprovalKey { get; set; }
        public string DealKey { get; set; }
        public string  MonthYear { get; set; }
        public string  FIManagerName { get; set; }
        public string  FIManagerNumber { get; set; }
        public bool  FinanceIncomePaid { get; set; }
        public bool  ServiceContractPaid { get; set; }
        public bool  MaintenanceContractPaid { get; set; }
        public bool  GAPPaid { get; set; }
        public bool  ZurichShieldPaid { get; set; }
        public bool  NitrogenPaid { get; set; }
        public bool  SelectProtectionPaid { get; set; }
        public bool  TireWheelPaid { get; set; }
        public bool  KeyReplacementPaid { get; set; }
        public bool  WindshieldProtectionPaid { get; set; }
        public bool  WearAndTearPaid { get; set; }
        public bool  SecureGuardPaid { get; set; }
        public bool  FitzTotalPackagePaid { get; set; }
        public bool  RustInhibitUnderCoatPaid { get; set; }
        public bool  RustInhibitorPaid { get; set; }
        public bool  UndercoatingPaid { get; set; }
        public bool  DataDotsPaid { get; set; }
        public bool PaintDentPaid { get; set; }
        public bool Miscellaneous1Paid { get; set; }
        public bool Miscellaneous2Paid { get; set; }
        public bool Miscellaneous3Paid { get; set; }
        public bool Miscellaneous4Paid { get; set; }
        public bool BPPPaid { get; set; }
        public decimal FinanceIncomeAmount { get; set; }
        public decimal ServiceContractAmount { get; set; }
        public decimal MaintenanceContractAmount { get; set; }
        public decimal GAPAmount { get; set; }
        public decimal ZurichShieldAmount { get; set; }
        public decimal NitrogenAmount { get; set; }
        public decimal SelectProtectionAmount { get; set; }
        public decimal TireWheelAmount { get; set; }
        public decimal KeyReplacementAmount { get; set; }
        public decimal WindshieldProtectionAmount { get; set; }
        public decimal WearAndTearAmount { get; set; }
        public decimal SecureGuardAmount { get; set; }
        public decimal FitzTotalPackageAmount { get; set; }
        public decimal RustInhibitUnderCoatAmount { get; set; }
        public decimal RustInhibitorAmount { get; set; }
        public decimal UndercoatingAmount { get; set; }
        public decimal DataDotsAmount { get; set; }
        public decimal PaintDentAmount { get; set; }
        public decimal Miscellaneous1Amount { get; set; }
        public decimal Miscellaneous2Amount { get; set; }
        public decimal Miscellaneous3Amount { get; set; }
        public decimal Miscellaneous4Amount { get; set; }
        public decimal BPPAmount { get; set; }
        public string Comments { get; set; }
        public string ApprovalUser { get; set; }
        public DateTime ApprovalDate { get; set; }
    }

    public class ManufacturerSpiffModel
    {
        public int MonthId { get; set; }
        public int YearId { get; set; }
        public List<ManufacturerSpiff> ManufacturerSpiffs { get; set; }
    }


    public class ManufacturerSpiff
    {
        public int SpiffKey { get; set; }
        public string Manufacturer { get; set; }
        public string SpiffPaid { get; set; }
        public string MonthYear { get; set; }
    }
}