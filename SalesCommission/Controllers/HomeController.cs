using SalesCommission.Models;
using SalesCommission.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SalesCommission.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var associateHomeModel = new AssociateHomeModel();

            var yearId = DateTime.Now.Year;
            var monthId = DateTime.Now.Month;

            var currentUserId = "";
            if (Session["AssociateId"] != null)
            {
                currentUserId = Session["AssociateId"].ToString();
            }
            var locationId = "";
            if (Session["LocationId"] != null)
            {
                locationId = Session["LocationId"].ToString();
            }

            associateHomeModel.AssociateInformation = SqlQueries.GetFIAssociateInformationByDate(locationId, currentUserId, yearId, monthId);

            if (associateHomeModel.AssociateInformation != null && associateHomeModel.AssociateInformation.AssociatePosition != null)
            {

                var titleDue = SqlQueries.GetAllTitlesDue();
                var moneyDue = SqlQueries.GetAllMoneyDue();
                var moneyDueHistory = SqlQueries.GetAllMoneyDueHistory();

                if (associateHomeModel.AssociateInformation.AssociatePosition.Trim().ToUpper() == "SLS ASSOC" || associateHomeModel.AssociateInformation.AssociatePosition.Trim().ToUpper() == "SALES ASSOCIATE")
                {
                    associateHomeModel.AssociateTitleDue = titleDue.FindAll(x => (x.SalesAssociate1 == currentUserId || x.SalesAssociate2 == currentUserId) && (x.LienDueCustomer == true || x.TitleDueCustomer == true || x.OdomDueCustomer == true));

                }

                else if (associateHomeModel.AssociateInformation.AssociatePosition.Trim().ToUpper() == "FIN MGR" || associateHomeModel.AssociateInformation.AssociatePosition.Trim().ToUpper() == "FINANCE MANAGER")
                {
                    associateHomeModel.AssociateTitleDue = titleDue.FindAll(x => x.FinanceManagerId == currentUserId);
                    associateHomeModel.AssociateMoneyDue = moneyDue.FindAll(x => x.FIManagerNumber == currentUserId);
                    associateHomeModel.AssociateMoneyDueHistory = moneyDueHistory.FindAll(x => x.FIManagerNumber == currentUserId);
                }

                else if (associateHomeModel.AssociateInformation.AssociatePosition.Trim().ToUpper() == "SLS MGR" || associateHomeModel.AssociateInformation.AssociatePosition.Trim().ToUpper() == "SALES MANAGER")
                {
                    associateHomeModel.AssociateTitleDue = titleDue.FindAll(x => x.SalesManagerId == currentUserId);
                    associateHomeModel.AssociateMoneyDue = moneyDue.FindAll(x => x.SalesManagerNumber == currentUserId);
                    associateHomeModel.AssociateMoneyDueHistory = moneyDueHistory.FindAll(x => x.SalesManagerNumber == currentUserId);
                }
                else
                {
                    associateHomeModel.AssociateTitleDue = titleDue.FindAll(x => x.Location == associateHomeModel.AssociateInformation.AssociateLocation);
                    associateHomeModel.AssociateMoneyDue = moneyDue.FindAll(x => x.Location == associateHomeModel.AssociateInformation.AssociateLocation);
                    associateHomeModel.AssociateMoneyDueHistory = moneyDueHistory.FindAll(x => x.Location == associateHomeModel.AssociateInformation.AssociateLocation);
                }

            }
            return View(associateHomeModel);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}