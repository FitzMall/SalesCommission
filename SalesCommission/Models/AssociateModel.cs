using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SalesCommission.Models
{
    public class AssociateModel
    {
        public string StoreId { get; set; }
        public int MonthId { get; set; }
        public int YearId { get; set; }
        public string Location { get; set; }
        public List<Associate> Associates { get; set; }
        public List<Mentor> Mentors { get; set; }
        public List<SelectListItem> Payscales { get; set; }
        public List<SelectListItem> Levels { get; set; }
        public List<SelectListItem> SSI { get; set; }
        public List<SelectListItem> Statuses { get; set; }
        public List<SelectListItem> StoreVolumes { get; set; }
    }

    public class Associate
    {
        public int sa_pkey { get; set; }
        public string AssociateSSN { get; set; }
        public string AssociateLevel { get; set; }
        public string AssociateMonthYear { get; set; }
        public string AssociateStoreVolume { get; set; }
        public string AssociateSSI { get; set; }
        public string AssociatePayscale { get; set; }
        public string AssociateStatus { get; set; }
        public string AssociateMentor { get; set; }
        public DateTime AssociateHireDate { get; set; }
        public DateTime AssociateCompetencyDate { get; set; }
        public DateTime AssociateGraduationDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string UpdateUser { get; set; }
        public decimal AssociateWage { get; set; }
        public int emp_pkey { get; set; }
        public string AssociateLocation { get; set; }
        public string AssociateMall { get; set; }
        public string AssociateFirstName { get; set; }
        public string AssociateLastName { get; set; }
        public string AssociateMiddleName { get; set; }
        public string AssociateFullName { get; set; }
        public string AssociateNumber { get; set; }
        public string AssociatePosition { get; set; }
        public string AssociateDepartment { get; set; }
        public string AssociateDepartmentLocation { get; set; }
        public string AssociateDepartmentDescription { get; set; }
        public DateTime AssociateTerminationDate { get; set; }
        public DateTime AssociateRehireDate { get; set; }
        public List<Draw> AssociateDraws { get; set; }
        public List<Bonus> AssociateBonus { get; set; }
        public List<Goal> AssociateGoals { get; set; }
        public List<AssociateDeals> AssociateDeals { get; set; }
        public List<PreviousMonthsDeals> PreviousAssociateDeals { get; set; }
        public DealCommissionCounts AssociateDealCounts { get; set; }
        public List<DealCommissionCounts> PreviousAssociateDealCounts { get; set; }
        public List<Payscale> AssociateOldPayscales { get; set; }
        public List<NewPayscale> AssociatePayscales { get; set; }
        public List<NewPayscale> AllPayscales { get; set; }
        public List<NewPayscaleSetup> AssociatePayscaleSetup { get; set; }
        public List<AssociateHours> AssociateHours { get; set; }
        public List<AssociateHours> PreviousAssociateHours { get; set; }
        public List<AssociateUnits> AssociateUnits { get; set; }
        public List<AssociateLead> AssociateLeads { get; set; }
        public List<AssociateAppointment> AssociateAppointments { get; set; }
        public PayLevelCalculation PayLevelCalculation { get; set; }
    }

    public class AssociateLead
    {
        public int DealerId { get; set; }
        public string LeadId { get; set; }
        public DateTime LeadCreatedEastTime { get; set; }
        public string LeadStatusId { get; set; }
        public string LeadStatusName { get; set; }
        public string LeadStatusTypeName { get; set; }
        public string LeadSourceName { get; set; }
        public string LeadSourceTypeName { get; set; }
        public string LeadSourceGroupName { get; set; }
        public string LeadGroupMapping { get; set; }
        public string SalesRepUserId { get; set; }
        public string Sales_LastName { get; set; }
        public string Sales_FirstName { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public bool HasBeenContacted { get; set; }
        public string Team { get; set; }
        public string VOfInterest_InventoryType { get; set; }
        public string VOfInterest_Make { get; set; }
        public string VOfInterest_Model { get; set; }
        public string VOfInterest_StockNumber { get; set; }
        public int AdjustedResponseTimeInMinutes { get; set; }
        public int ActualResponseTimeInMinutes { get; set; }

    }

    public class AssociateAppointment
    {
       public string AssociateName { get; set; }
        public string DealerID { get; set; }
        public string AppointmentID { get; set; }
        public string CustomerID { get; set; }
        public string LeadID { get; set; }
        public string VisitID { get; set; }
        public bool IsShow { get; set; }
        public bool IsNoShow { get; set; }
        public string AppointmentType { get; set; }
        public string AssignedToUserID { get; set; }
        public DateTime AppointmentStartUTCDate { get; set; }
        public DateTime RescheduledUTCDate { get; set; }
        public DateTime CompletedUTCDate { get; set; }
	    public string AppointmentStatus { get; set; }
        public DateTime LastUpdatedUTCDate { get; set; }
    }

    public class PreviousMonthsDeals
    {
        public List<AssociateDeals> AssociateDeals { get; set; }
    }


    public class AssociateUpdate
    {
        public string AssociateSSN { get; set; }
        public string AssociateLevel { get; set; }
        public string AssociateMonthYear { get; set; }
        public string AssociateStoreVolume { get; set; }
        public string AssociateSSI { get; set; }
        public string AssociatePayscale { get; set; }
        public string AssociateStatus { get; set; }
        public string AssociateMentor { get; set; }
        public DateTime AssociateHireDate { get; set; }
        public DateTime AssociateCompetencyDate { get; set; }
        public DateTime AssociateGraduationDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string UpdateUser { get; set; }
    }


    public class AssociateUnits
    {
        public DateTime UnitDate { get; set; }
        public decimal UnitCount { get; set; }

    }

    public class AssociateHours
    {
        public string AssociateNumber { get; set; }
        public DateTime HourDate { get; set; }
        public decimal Hours { get; set; }
    }

    public class AssociateDeals
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
        public string VehicleCategory { get; set; }
        public string VehicleMake { get; set; }
        public string VehicleModel { get; set; }
        public string VehicleYear { get; set; }
        public string VehicleMiles { get; set; }
        public string VehicleCertification { get; set; }
        public decimal VehiclePrice { get; set; }
        public int VehicleTerm { get; set; }
        public string CustomerLastName { get; set; }
        public string StockNumber { get; set; }
        public string SalesAssociate1 { get; set; }
        public string SalesAssociate2 { get; set; }
        public decimal NewDealCount { get; set; }
        public decimal UsedDealCount { get; set; }
        public decimal LeaseCount { get; set; }
        public decimal BPPCount { get; set; }
        public decimal FinanceCount { get; set; }
        public decimal ServiceContractCount { get; set; }
        public decimal MaintenanceContractCount { get; set; }
        public decimal GAPCount { get; set; }
        public decimal TradeCount { get; set; }
        public decimal AftermarketCount { get; set; }
        public decimal HoursWorked { get; set; }
        public string MakeCode { get; set; }

    }

    public class DealCommissionCounts
    {
        public DateTime DealMonth { get; set; }
        public decimal NewDealCount { get; set; }
        public decimal UsedDealCount { get; set; }
        public decimal TotalDealCount { get; set; }
        public decimal LeaseCount { get; set; }
        public decimal BPPCount { get; set; }
        public decimal TradeCount { get; set; }
        public decimal FinanceCount { get; set; }
        public decimal ServiceContractCount { get; set; }
        public decimal MaintenanceContractCount { get; set; }
        public decimal GAPCount { get; set; }
        public decimal AftermarketCount { get; set; }
    }

    public class Goal
    {
        public int Id { get; set; }
        public string AssociateSSN { get; set; }
        public string MonthYear { get; set; }
        public decimal DealCount { get; set; }
        public decimal NewCount { get; set; }
        public decimal UsedCount { get; set; }
        public decimal BPPCount { get; set; }
        public decimal TradeCount { get; set; }
        public decimal FinanceCount { get; set; }
        public decimal ServiceCount { get; set; }
        public decimal GAPCount { get; set; }
        public decimal ZurichCount { get; set; }
        public decimal AftermarketCount { get; set; }
        public decimal SpiffCount { get; set; }
        public DateTime UpdateDate { get; set; }
        public string UpdateUser { get; set; }
    }

    public class Draw
    {
        public int Id { get; set; }
        public string AssociateSSN { get; set; }
        public string MonthYear { get; set; }
        public DateTime DrawDate { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateUser { get; set; }
        public int DrawHours { get; set; }
        public decimal DrawAmount { get; set; }
        public string DrawGuarBegin { get; set; }
        public string DrawGuarEnd { get; set; }

    }

    public class Bonus
    {
        public int Id { get; set; }
        public string AssociateSSN { get; set; }
        public string MonthYear { get; set; }
        public string BonusDate { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateUser { get; set; }
        public decimal BonusAmount { get; set; }
        public string BonusComments { get; set; }

    }

    public class Mentor
    {
        public string AssociateSSN { get; set; }
        public string StoreId { get; set; }
        public int MonthId { get; set; }
        public int YearId { get; set; }
        public string AssociateFirstName { get; set; }
        public string AssociateLastName { get; set; }
        public string AssociateFullName { get; set; }
    }

}