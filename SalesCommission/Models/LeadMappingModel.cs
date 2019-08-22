using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SalesCommission.Models
{
    public class LeadMappingModel
    {
        public List<LeadGroup> LeadGroups { get; set; }
        public List<LeadMapping> LeadSourceMappings { get; set; }
        public List<LeadMapping> VINLeadSourceMappings { get; set; }

        public int SelectedLeadGroupId { get; set; }
    }

    public class LeadMapping
    {
        public int Id { get; set; }
        public string LeadGroup { get; set; }
        public int LeadGroupId { get; set; }
        public string LeadSourceName { get; set; }
        public string LeadSourceGroupName { get; set; }
        public string UpdateUser { get; set; }
        public DateTime UpdateDate { get; set; }
    }

    public class LeadGroup
    {
        public int Id { get; set; }
        public string LeadGroupName { get; set; }
        public string UpdateUser { get; set; }
        public DateTime UpdateDate { get; set; }

    }

}