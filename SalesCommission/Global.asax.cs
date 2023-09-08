using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SalesCommission
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            RouteTable.Routes.MapRoute(
                "Adjustments",
                "FICommission/Adjustments/{id}/{monthId}/{yearId}", // URL with parameters
                new { controller = "FICommission", action = "Adjustments", id = UrlParameter.Optional, monthId = UrlParameter.Optional, yearId = UrlParameter.Optional }
            );

            RouteTable.Routes.MapRoute(
                "AssociateCommission",
                "Commission/Associate/{id}/{monthId}/{yearId}", // URL with parameters
                new { controller = "Commission", action = "Associate", id = UrlParameter.Optional, monthId = UrlParameter.Optional, yearId = UrlParameter.Optional }
            );

            RouteTable.Routes.MapRoute(
                "NewAssociateCommission",
                "Commission/NewAssociate/{id}/{monthId}/{yearId}", // URL with parameters
                new { controller = "Commission", action = "NewAssociate", id = UrlParameter.Optional, monthId = UrlParameter.Optional, yearId = UrlParameter.Optional }
            );

            RouteTable.Routes.MapRoute(
                "FIAssociateCommission",
                "FICommission/Associate/{location}/{id}/{monthId}/{yearId}", // URL with parameters
                new { controller = "FICommission", action = "Associate", id = UrlParameter.Optional, monthId = UrlParameter.Optional, yearId = UrlParameter.Optional }
            );

            RouteTable.Routes.MapRoute(
                "FINewAssociateCommission",
                "FICommission/NewAssociate/{location}/{id}/{monthId}/{yearId}", // URL with parameters
                new { controller = "FICommission", action = "NewAssociate", id = UrlParameter.Optional, monthId = UrlParameter.Optional, yearId = UrlParameter.Optional }
            );

            RouteTable.Routes.MapRoute(
                "FIAssociateValidate",
                "FICommission/ValidateDeals/{location}/{id}/{monthId}/{yearId}", // URL with parameters
                new { controller = "FICommission", action = "ValidateDeals", id = UrlParameter.Optional, monthId = UrlParameter.Optional, yearId = UrlParameter.Optional }
            );

            RouteTable.Routes.MapRoute(
                "AssociateDrawsAndBonus",
                "Commission/BonusAndDraws/{id}/{monthId}/{yearId}", // URL with parameters
                new { controller = "Commission", action = "BonusAndDraws", id = UrlParameter.Optional, monthId = UrlParameter.Optional, yearId = UrlParameter.Optional }
            );

            RouteTable.Routes.MapRoute(
                "FIAssociateDrawsAndBonus",
                "FICommission/BonusAndDraws/{location}/{id}/{monthId}/{yearId}", // URL with parameters
                new { controller = "FICommission", action = "BonusAndDraws", location = UrlParameter.Optional, id = UrlParameter.Optional, monthId = UrlParameter.Optional, yearId = UrlParameter.Optional }
            );

            RouteTable.Routes.MapRoute(
                "AllAssociatesCommission",
                "Commission/AllAssociates/{id}/{monthId}/{yearId}", // URL with parameters
                new { controller = "Commission", action = "AllAssociates", id = UrlParameter.Optional, monthId = UrlParameter.Optional, yearId = UrlParameter.Optional }
            );

            RouteTable.Routes.MapRoute(
                "NewAllAssociatesCommission",
                "Commission/NewAllAssociates/{id}/{monthId}/{yearId}", // URL with parameters
                new { controller = "Commission", action = "NewAllAssociates", id = UrlParameter.Optional, monthId = UrlParameter.Optional, yearId = UrlParameter.Optional }
            );

            RouteTable.Routes.MapRoute(
                "AssociateScorecard",
                "Commission/Scorecard/{id}/{monthId}/{yearId}", // URL with parameters
                new { controller = "Commission", action = "Scorecard", id = UrlParameter.Optional, monthId = UrlParameter.Optional, yearId = UrlParameter.Optional }
            );

            RouteTable.Routes.MapRoute(
                "NewAssociateScorecard",
                "Commission/NewScorecard/{id}/{monthId}/{yearId}", // URL with parameters
                new { controller = "Commission", action = "NewScorecard", id = UrlParameter.Optional, monthId = UrlParameter.Optional, yearId = UrlParameter.Optional }
            );

            RouteTable.Routes.MapRoute(
                "FIAssociateScorecard",
                "FICommission/Scorecard/{id}/{monthId}/{yearId}", // URL with parameters
                new { controller = "FICommission", action = "Scorecard", id = UrlParameter.Optional, monthId = UrlParameter.Optional, yearId = UrlParameter.Optional }
            );

            RouteTable.Routes.MapRoute(
                "DealListingsName",
                "Sales/DealListingByName/{locationCode}/{monthId}/{yearId}/{name}", // URL with parameters
                new { controller = "Sales", action = "DealListingByName", locationCode = UrlParameter.Optional, monthId = UrlParameter.Optional, yearId = UrlParameter.Optional, name = UrlParameter.Optional }
            );

            RouteTable.Routes.MapRoute(            
                "DealListings",
                "Sales/UnvalidatedDeals/{makeId}/{monthId}/{yearId}/", // URL with parameters
                new { controller = "Sales", action = "UnvalidatedDeals", makeId = UrlParameter.Optional, monthId = UrlParameter.Optional, yearId = UrlParameter.Optional}
            );

            RouteTable.Routes.MapRoute(
                "TypeListings",
                "Sales/DealListing/{listType}/{makeId}/{monthId}/{yearId}/", // URL with parameters
                new { controller = "Sales", action = "DealListing", listType = UrlParameter.Optional, makeId = UrlParameter.Optional, monthId = UrlParameter.Optional, yearId = UrlParameter.Optional }
            );

            RouteTable.Routes.MapRoute(
                "LeaseDeals",
                "Sales/LeaseDeals/{makeId}/{monthId}/{yearId}/", // URL with parameters
                new { controller = "Sales", action = "LeaseDeals", makeId = UrlParameter.Optional, monthId = UrlParameter.Optional, yearId = UrlParameter.Optional }
            );

            RouteTable.Routes.MapRoute(
                "CPODeals",
                "Sales/CPODeals/{makeId}/{monthId}/{yearId}/{cpoMakeName}", // URL with parameters
                new { controller = "Sales", action = "CPODeals", makeId = UrlParameter.Optional, monthId = UrlParameter.Optional, yearId = UrlParameter.Optional, cpoMakeName = UrlParameter.Optional }
            );

            RouteTable.Routes.MapRoute(
                "HandymanDeals",
                "Sales/HandymanDeals/{makeId}/{monthId}/{yearId}/", // URL with parameters
                new { controller = "Sales", action = "HandymanDeals", makeId = UrlParameter.Optional, monthId = UrlParameter.Optional, yearId = UrlParameter.Optional }
            );

            RouteTable.Routes.MapRoute(
                "NextCarDeals",
                "Sales/NextCarDeals/{makeId}/{monthId}/{yearId}/", // URL with parameters
                new { controller = "Sales", action = "NextCarDeals", makeId = UrlParameter.Optional, monthId = UrlParameter.Optional, yearId = UrlParameter.Optional }
            );

            RouteTable.Routes.MapRoute(
                "OfficeValidateDeals",
                "Sales/OfficeValidateDeals/{makeId}/{monthId}/{yearId}/", // URL with parameters
                new { controller = "Sales", action = "OfficeValidateDeals", makeId = UrlParameter.Optional, monthId = UrlParameter.Optional, yearId = UrlParameter.Optional }
            );

            RouteTable.Routes.MapRoute(
               "AssociatePerformance",
               "Reports/AssociatePerformance/{monthId}/{yearId}/{location}/{associateName}", // URL with parameters
               new { controller = "Reports", action = "AssociatePerformance", monthId = UrlParameter.Optional, yearId = UrlParameter.Optional, location = UrlParameter.Optional, assoicateName = UrlParameter.Optional }
           );

            RouteTable.Routes.MapRoute(
               "PrintTradeReport",
               "Print/TradeReport/{storeId}/{monthId}/{yearId}/{includeHandyman}", // URL with parameters
               new { controller = "Print", action = "TradeReport", storeId = UrlParameter.Optional, monthId = UrlParameter.Optional, yearId = UrlParameter.Optional,  includeHandyman = UrlParameter.Optional }
           );
            RouteTable.Routes.MapRoute(
               "AfterSalesAssociatesFilter",
               "Reports/AfterSalesAssociatesFilter/{id}/{monthId}/{yearId}", // URL with parameters
               new { controller = "Reports", action = "AfterSalesAssociatesFilter", id = UrlParameter.Optional, monthId = UrlParameter.Optional, yearId = UrlParameter.Optional }
           );
            RouteTable.Routes.MapRoute(
               "AfterSalesAssociatesStore",
               "Reports/AfterSalesAssociatesStore/{location}/{monthId}/{yearId}", // URL with parameters
               new { controller = "Reports", action = "AfterSalesAssociatesStore", location = UrlParameter.Optional, monthId = UrlParameter.Optional, yearId = UrlParameter.Optional }
           );

            RouteTable.Routes.MapRoute(
               "Chargebacks",
               "Sales/Chargebacks/{storeId}/{monthId}/{yearId}", // URL with parameters
               new { controller = "Sales", action = "Chargebacks", storeId = UrlParameter.Optional, monthId = UrlParameter.Optional, yearId = UrlParameter.Optional}
           );

            RouteTable.Routes.MapRoute(
               "UpdateMoneyDue",
               "Reports/UpdateMoneyDue/{Id}/{location}/{dueFrom}", // URL with parameters
               new { controller = "Reports", action = "UpdateMoneyDue", Id = UrlParameter.Optional,location = UrlParameter.Optional, dueFrom = UrlParameter.Optional }
           );

        }

        protected void Session_Start(object sender, EventArgs e)
        {
            SetUserInformation();
        }

        public void SetUserInformation()
        {

            if (Request.Cookies["User"] != null)
            {
                var cookieValue = Request.Cookies["User"].Value;

                NameValueCollection qsCollection = HttpUtility.ParseQueryString(cookieValue);

                var userIdFromCookie = qsCollection["login"].ToString();
                var userNameFromCookie = qsCollection["name"].ToString();
                var isAdmin = false;
                var isCommissionAdmin = false;
                var canSeeReporting = false;
                var isAssociateAdmin = false;
                var canOfficeValidate = false;
                var locationId = "";

                var userPermissions = SalesCommission.Business.SqlQueries.GetUserPermissions(userIdFromCookie);

                if(userPermissions != null)
                {
                    foreach(var permission in userPermissions)
                    {
                        if(permission.IsAdmin == true)
                        {
                            isAdmin = true;
                            break;
                        }
                    }
                    foreach (var permission in userPermissions)
                    {
                        if (permission.IsCommissionAdmin == true)
                        {
                            isCommissionAdmin = true;
                            break;
                        }
                    }
                    foreach (var permission in userPermissions)
                    {
                        if (permission.CanCreateReports == true)
                        {
                            canSeeReporting = true;
                            break;
                        }
                    }
                    foreach (var permission in userPermissions)
                    {
                        if (permission.IsAssociateAdmin == true)
                        {
                            isAssociateAdmin = true;
                            break;
                        }
                    }
                    foreach (var permission in userPermissions)
                    {
                        if (permission.CanOfficeValidateDeal == true)
                        {
                            canOfficeValidate = true;
                            break;
                        }
                    }

                }
                //var associateList = Business.SqlQueries.GetSalesAssociates();
                var associateList = Business.SqlQueries.GetJJFUserByUserId(userIdFromCookie.Trim());

                var associate = new Models.JJFUser();
                var associateId = "007";
                var VinName = "associate";

                if (associateList != null && associateList.Count > 0)
                {
                    associate = associateList[0];
                    
                    associateId = associate.DMS_Id;
                    locationId = associate.Location.Trim();// Business.SqlQueries.GetSalesAssociatesLocationById(associateId);

                    if(userIdFromCookie.Trim().ToLower() == "christianc")
                    {
                        locationId = "FOC";
                    }

                    if(locationId == "LFC")
                    {
                        locationId = "LFO";
                    }
                    VinName = associate.VinName;
                }
                Session.Add("userVinName", VinName);
                Session.Add("AssociateId", associateId);
                Session.Add("LocationId", locationId);
                Session.Add("UserId", userIdFromCookie);
                Session.Add("UserName", userNameFromCookie);
                Session.Add("IsAdmin", isAdmin);
                Session.Add("IsCommissionAdmin", isCommissionAdmin);
                Session.Add("IsAssociateAdmin", isAssociateAdmin);
                Session.Add("CanSeeReporting", canSeeReporting);
                Session.Add("CanOfficeValidate", canOfficeValidate);

            }
            else
            {
                Session.Add("AssociateId", "3363");
                Session.Add("LocationId", "JJF");
                Session.Add("UserId", "statlerc");
                Session.Add("UserName", "Not Logged In");
                Session.Add("IsAdmin", true);
                Session.Add("IsCommissionAdmin", true);
                Session.Add("IsAssociateAdmin", true);
                Session.Add("CanSeeReporting", true);
                Session.Add("CanOfficeValidate", false);

                //Session.Add("UserId", "shaver");
                //Session.Add("UserName", "The person formally known as Chris");
                //Session.Add("IsAdmin", false);

                //Session.Add("AssociateId", "1864");
                //Session.Add("UserId", "bdavis");
                //Session.Add("UserName", "Bobby Davis");
                //Session.Add("IsAdmin", false);
                //Session.Add("IsCommissionAdmin", false);
            }

        }

    }
}
