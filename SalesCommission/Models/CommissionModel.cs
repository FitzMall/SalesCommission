﻿using System;
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

    }

    public class FICommissionModel
    {
        public string StoreId { get; set; }
        public int MonthId { get; set; }
        public int YearId { get; set; }
        public List<Associate> FIManagers { get; set; }
        public List<ObjectivesAndStandards> ObjectivesAndStandards { get; set; }
        public List<AftermarketDealDetail> AftermarketDealDetails { get; set; }
        public List<FIManagerDealDetails> FIManagerDealDetails { get; set; }

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
    }

    public class FIAssociateCommissionModel
    {
        public string AssociateId { get; set; }
        public int MonthId { get; set; }
        public int YearId { get; set; }
        public Associate AssociateInformation { get; set; }
        public List<AftermarketDealDetail> AftermarketDealDetails { get; set; }
        public List<DealApproval> DealApprovals { get; set; }
        public List<FIPayscale> FIPayscales { get; set; }

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