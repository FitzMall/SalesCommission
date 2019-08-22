using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SalesCommission.Models
{
    public class DealershipModel
    {
        public int MonthId { get; set; }
        public int YearId { get; set; }
        public List<RevenueInformation> DealershipInputs { get; set; }
        public List<SelectListItem> StoreVolumes { get; set; }
    }

    public class RevenueInformation
    {
        public string Key { get; set; }
        public string Location { get; set; }
        public string LocationDescription { get; set; }
        public string MonthYear { get; set; }
        public string StoreVolume { get; set; }
        public decimal HourlyRate { get; set; }
        public decimal FrontNewAmount { get; set; }
        public decimal FrontUsedAmount { get; set; }
        public decimal BPPAmount { get; set; }
        public decimal FinanceAmount { get; set; }
        public decimal ServiceContractAmount { get; set; }
        public decimal GAPAmount { get; set; }
        public decimal TradeAmount { get; set; }
        public decimal AftermarketAmount { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateDate { get; set; }        
        public string ModifiedUser { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string BrandCode { get; set; }

    }
}