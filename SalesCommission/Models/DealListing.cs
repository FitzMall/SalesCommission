using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SalesCommission.Models
{
    public class DealListing
    {
        public string LocationCode { set; get; }
        public string MonthId { get; set; }
        public string YearId { get; set; }
        public List<DealDetail> Deals { get; set; }
    }
}