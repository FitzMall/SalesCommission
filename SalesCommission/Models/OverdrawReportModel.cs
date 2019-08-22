using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SalesCommission.Models
{
    public class OverdrawReportModel
    {
        public string StoreId { get; set; }
        public string[] SelectedStores { get; set; }
        public int YearId { get; set; }
    }
}