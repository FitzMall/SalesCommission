using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SalesCommission.Models
{
    public class ChargebackModel
    {
        public string StoreId { get; set; }
        public int MonthId { get; set; }
        public int YearId { get; set; }
        public string Location { get; set; }
        public List<Chargeback> Chargebacks { get; set; }

    }

    public class Chargeback
    {
        public int Id { get; set; }
        public string StoreId { get; set; }
        public string MonthYear { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public decimal FrontGrossAmount { get; set; }
        public decimal UsedFrontGrossAmount { get; set; }
        public decimal FinanceAmount { get; set; }
        public decimal ServiceContractAmount { get; set; }
        public decimal ROAmount { get; set; }
        public decimal CertFeeAmount { get; set; }
        public decimal RebateAdjustmentAmount { get; set; }
        public decimal CustomerWorkOrderAmount { get; set; }
        public decimal OtherAdjustmentAmount { get; set; }
        public decimal UsedROAmount { get; set; }
        public decimal UsedCertFeeAmount { get; set; }
        public decimal UsedRebateAdjustmentAmount { get; set; }
        public decimal UsedCustomerWorkOrderAmount { get; set; }
        public decimal UsedOtherAdjustmentAmount { get; set; }

        public decimal GAPAmount { get; set; }
        public decimal OtherAmount { get; set; }        
        public DateTime UpdateDate { get; set; }
        public string UpdateUser { get; set; }
        public string OtherComments { get; set; }
        public string DisplayValues { get; set; }

    }

}