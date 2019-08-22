using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SalesCommission.Models
{
    public class SecurityModel
    {
        public string UserId { get; set; }
        public List<JJFUser> JJFUsers { get; set; }
        public List<UserPersmissions> UserPermissions { get; set; }
    }

    public class ImportModel
    {
        public string ImportAction { get; set; }

    }

    public class UserPersmissions
    {
        public string SecurityKey { get; set; }
        public string UserId { get; set; }
        public string UserSSN { get; set; }
        public string Location { get; set; }
        public bool IsAdmin { get; set; }
        public bool CanShowroomValidateDeal { get; set; }
        public bool CanShowroomUpdateDeal { get; set; }
        public bool CanOfficeValidateDeal { get; set; }
        public bool CanDeleteDeal { get; set; }
        public bool CanSetObjectivesStandards { get; set; }
        public bool CanCreateReports { get; set; }
        public bool IsCommissionAdmin { get; set; }
        public bool IsAssociateAdmin { get; set; }
    }


    public class JJFUser
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string UserId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string DisplayName { get; set; }

        public string Location { get; set; }
        public string Mall { get; set; }
        public string Showroom { get; set; }
        public string VinUser { get; set; }
        public string VinName{ get; set; }
        public string VinStore { get; set; }
        public string VinUserKey { get; set; }
        public string DMS_Id { get; set; }


    }
}