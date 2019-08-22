using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SalesCommission.Models
{
    public class ObjectivesStandardsModel
    {
        public int MonthId { get; set; }
        public int YearId { get; set; }
        public string LocationId { get; set; }
        public List<ObjectivesAndStandards> ObjectivesAndStandards { get; set; }
        public List<SavedDocument> SavedDocuments { get; set; }
        public List<UserPersmissions> UserPermissions { get; internal set; }
    }

    public class ObjectivesAndStandards
    {
        public int MonthId { get; set; }
        public int YearId { get; set; }
        public string LocationId { get; set; }
        public string StoreId { get; set; }
        public string BrandId { get; set; }
        public decimal FinCntPercent { get; set; }
        public decimal FinCntPerCnt { get; set; }
        public decimal VSCPercent { get; set; }
        public decimal VSCPerCnt { get; set; }
        public decimal GAPPercent { get; set; }
        public decimal GAPPerCnt { get; set; }
        public decimal AftermarketPercent { get; set; }
        public decimal AftermarketPerCnt { get; set; }
        public decimal BPPPercent { get; set; }
        public decimal BPPCollectionPercent { get; set; }
        public decimal TradePercent { get; set; }
        public decimal FrontPVR { get; set; }
        public decimal BackPVR { get; set; }
        public decimal ManufacturerObjective { get; set; }
        public decimal FitzgeraldObjective { get; set; }
        public decimal GPURObjective { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
    }

    public class SavedDocument
    {
        public string LocationId { get; set; }
        public int MonthId { get; set; }
        public int YearId { get; set; }
        public string DocumentPath { get; set; }
        public string DocumentTitle { get; set; }
        public string DocumentMimeType { get; set; }
        public long DocumentSize { get; set; }

    }

    public class FactoryToDealerCash
    {
        public int MonthId { get; set; }
        public int YearId { get; set; }
        public string LocationId { get; set; }
        public string StoreId { get; set; }
        public string BrandId { get; set; }
        public decimal FTDAmount { get; set; }
        public decimal FTDCPOAmount { get; set; }
        public string FTDComment { get; set; }
        public string ManagerComment { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdateDate { get; set; }
    }

}