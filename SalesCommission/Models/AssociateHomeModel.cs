using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SalesCommission.Models
{
    public class AssociateHomeModel
    {
        public Associate AssociateInformation { get; set; }
        public List<TitleDue> AssociateTitleDue { get; set; }
        public List<MoneyDue> AssociateMoneyDue { get; set; }
        public List<MoneyDue> AssociateMoneyDueHistory { get; set; }
    }
}