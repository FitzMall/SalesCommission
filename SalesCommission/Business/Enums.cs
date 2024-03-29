﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SalesCommission.Business
{
    public class Enums
    {
        public static IEnumerable<YesNo> YesNos = new List<YesNo>
        {
            new YesNo {
                Id = "",
                Name = ""
            },
            new YesNo {
                Id = "Y",
                Name = "Yes"
            },
            new YesNo {
                Id = "N",
                Name = "No"
            }
        };

        public static IEnumerable<Month> Months = new List<Month> {
            new Month {
                MonthId = 1,
                Name = "January"
            },
            new Month {
                MonthId = 2,
                Name = "February"
            },
            new Month {
                MonthId = 3,
                Name = "March"
            },
            new Month {
                MonthId = 4,
                Name = "April"
            },
            new Month {
                MonthId = 5,
                Name = "May"
            },
            new Month {
                MonthId = 6,
                Name = "June"
            },
            new Month {
                MonthId = 7,
                Name = "July"
            },
            new Month {
                MonthId = 8,
                Name = "August"
            },
            new Month {
                MonthId = 9,
                Name = "September"
            },
            new Month {
                MonthId = 10,
                Name = "October"
            },
            new Month {
                MonthId = 11,
                Name = "November"
            },
            new Month {
                MonthId = 12,
                Name = "December"
            }
        };

        public static IEnumerable<Year> Years = new List<Year> {
            new Year {
                YearId = DateTime.Now.AddYears(-10).Year,
                Name = DateTime.Now.AddYears(-10).Year.ToString()
            },
             new Year {
                YearId = DateTime.Now.AddYears(-9).Year,
                Name = DateTime.Now.AddYears(-9).Year.ToString()
            },
            new Year {
                YearId = DateTime.Now.AddYears(-8).Year,
                Name = DateTime.Now.AddYears(-8).Year.ToString()
            },
            new Year {
                YearId = DateTime.Now.AddYears(-7).Year,
                Name = DateTime.Now.AddYears(-7).Year.ToString()
            },
            new Year {
                YearId = DateTime.Now.AddYears(-6).Year,
                Name = DateTime.Now.AddYears(-6).Year.ToString()
            },
            new Year {
                YearId = DateTime.Now.AddYears(-5).Year,
                Name = DateTime.Now.AddYears(-5).Year.ToString()
            },
            new Year {
                YearId = DateTime.Now.AddYears(-4).Year,
                Name = DateTime.Now.AddYears(-4).Year.ToString()
            },
            new Year {
                YearId = DateTime.Now.AddYears(-3).Year,
                Name = DateTime.Now.AddYears(-3).Year.ToString()
            },
            new Year {
                YearId = DateTime.Now.AddYears(-2).Year,
                Name = DateTime.Now.AddYears(-2).Year.ToString()
            },
            new Year {
                YearId = DateTime.Now.AddYears(-1).Year,
                Name = DateTime.Now.AddYears(-1).Year.ToString()
            },
            new Year {
                YearId = DateTime.Now.Year,
                Name = DateTime.Now.Year.ToString()
            },
            new Year {
                YearId = DateTime.Now.AddYears(1).Year,
                Name = DateTime.Now.AddYears(1).Year.ToString()
            },
        };

        public static IEnumerable<Store> Stores = new List<Store> {
            new Store {
                StoreId = "annapolis",
                Name = "Annapolis Cadillac/Volkswagen"
            },
            new Store {
                StoreId = "annapolis(classic)",
                Name = "Annapolis Mazda/Mitsubishi"
            },
            new Store {
                StoreId = "chambersburg",
                Name = "Chambersburg Toyota/Nissan"
            },
            new Store {
                StoreId = "clearwater",
                Name = "Clearwater"
            },
            new Store {
                StoreId = "frederick",
                Name = "Frederick Baughmans Lane"
            },
            new Store {
                StoreId = "superstore",
                Name = "Frederick Route 85"
            },
            new Store {
                StoreId = "lakeforest(russell)",
                Name = "Gaithersburg Hyundai/Subaru"
            },
            new Store {
                StoreId = "Lakeforest(355)",
                Name = "Gaithersburg Toyota"
            },
            new Store {
                StoreId = "germantown",
                Name = "Germantown"
            },
            new Store {
                StoreId = "Hagerstown(CDJR)",
                Name = "Hagerstown Chrysler"
            },
            new Store {
                StoreId = "Hagerstown(GM)",
                Name = "Hagerstown GM"
            },
            new Store {
                StoreId = "lexingtonpark(lexpark)",
                Name = "Lexington Park"
            },
            new Store {
                StoreId = "nicholson",
                Name = "Rockville Nicholson"
            },
            new Store {
                StoreId = "colonial",
                Name = "Rockville Hyundai"
            },
            new Store {
                StoreId = "subaru",
                Name = "Rockville Subaru"
            },
            new Store {
                StoreId = "wheaton",
                Name = "Wheaton"
            }
        };

        public static IEnumerable<Store> NewStores = new List<Store> {
            new Store {
                StoreId = "annapolis",
                Name = "Annapolis Cadillac/Volkswagen"
            },
            new Store {
                StoreId = "annapolis(classic)",
                Name = "Annapolis Mazda/Mitsubishi"
            },
            new Store {
                StoreId = "chambersburg",
                Name = "Chambersburg Toyota/Nissan"
            },
            new Store {
                StoreId = "clearwater",
                Name = "Clearwater"
            },
            new Store {
                StoreId = "clearwater HY",
                Name = "Clearwater Hyundai"
            },
            new Store {
                StoreId = "clearwater SU",
                Name = "Clearwater Subaru"
            },
            new Store {
                StoreId = "frederick",
                Name = "Frederick Baughmans Lane"
            },
            new Store {
                StoreId = "Hagerstown(CDJR)",
                Name = "Hagerstown Chrysler"
            },
            new Store {
                StoreId = "Hagerstown(GM)",
                Name = "Hagerstown GM"
            },
            new Store {
                StoreId = "lakeforest(russell)",
                Name = "Gaithersburg Hyundai/Subaru"
            },
            new Store {
                StoreId = "Lakeforest(355)",
                Name = "Gaithersburg Toyota"
            },
            new Store {
                StoreId = "lexingtonpark(lexpark)",
                Name = "Lexington Park"
            },
            new Store {
                StoreId = "nicholson",
                Name = "Rockville Nicholson"
            },
            new Store {
                StoreId = "colonial",
                Name = "Rockville Hyundai"
            },
            new Store {
                StoreId = "subaru",
                Name = "Rockville Subaru"
            }
        };

        public static IEnumerable<Store> ChargebackStores = new List<Store> {
            new Store {
                StoreId = "annapolis",
                Name = "Annapolis Cadillac/Volkswagen"
            },
            new Store {
                StoreId = "annapolis(classic)",
                Name = "Annapolis Mazda/Mitsubishi"
            },
            new Store {
                StoreId = "chambersburg",
                Name = "Chambersburg Toyota/Nissan"
            },
            new Store {
                StoreId = "clearwater",
                Name = "Clearwater Chrysler/Jeep"
            },
            new Store {
                StoreId = "clearwater HY",
                Name = "Clearwater Hyundai"
            },
            new Store {
                StoreId = "clearwater SU",
                Name = "Clearwater Subaru"
            },
            new Store {
                StoreId = "clearwater OC",
                Name = "Clearwater Outlet Center"
            },
            new Store {
                StoreId = "frederick",
                Name = "Frederick Baughmans Lane"
            },
            new Store {
                StoreId = "superstore",
                Name = "Frederick Route 85"
            },
            new Store {
                StoreId = "Hagerstown(CDJR)",
                Name = "Hagerstown Chrysler"
            },
            new Store {
                StoreId = "Hagerstown(GM)",
                Name = "Hagerstown GM"
            },
            new Store {
                StoreId = "lakeforest(russell)",
                Name = "Gaithersburg Hyundai/Subaru"
            },
            new Store {
                StoreId = "germantown",
                Name = "Germantown"
            },
            new Store {
                StoreId = "Lakeforest(355)",
                Name = "Gaithersburg Toyota"
            },
            new Store {
                StoreId = "lexingtonpark(lexpark)",
                Name = "Lexington Park"
            },
            new Store {
                StoreId = "nicholson",
                Name = "Rockville Nicholson"
            },
            new Store {
                StoreId = "colonial",
                Name = "Rockville Hyundai"
            },
            new Store {
                StoreId = "subaru",
                Name = "Rockville Subaru"
            },
            new Store {
                StoreId = "wheaton",
                Name = "Wheaton"
            }
        };

        public static IEnumerable<Store> StoreComparison = new List<Store> {
            new Store {
                StoreId = "annapolis",
                Name = "Annapolis"
            },
            new Store {
                StoreId = "chambersburg",
                Name = "Chambersburg Toyota/Nissan"
            },
            new Store {
                StoreId = "clearwater",
                Name = "Clearwater"
            },
            new Store {
                StoreId = "frederick",
                Name = "Frederick"
            },
            new Store {
                StoreId = "Hagerstown(CDJR)",
                Name = "Hagerstown Chrysler"
            },
            new Store {
                StoreId = "Hagerstown(GM)",
                Name = "Hagerstown GM"
            },
            new Store {
                StoreId = "lakeforest(russell)",
                Name = "Gaithersburg Hyundai/Subaru"
            },
            new Store {
                StoreId = "Lakeforest(355)",
                Name = "Gaithersburg Toyota"
            },
            new Store {
                StoreId = "lexingtonpark(lexpark)",
                Name = "Lexington Park"
            },
            new Store {
                StoreId = "nicholson",
                Name = "Rockville Nicholson"
            },
            new Store {
                StoreId = "colonial",
                Name = "Rockville Hyundai"
            },
            new Store {
                StoreId = "wheaton",
                Name = "Wheaton"
            }
        };

        public static IEnumerable<Store> FIManagerStores = new List<Store> {
            new Store {
                StoreId = "ALL",
                Name = "ALL"
            },
            new Store {
                StoreId = "FOC",
                Name = "Annapolis"
            },
            //new Store {
            //    StoreId = "FOC",
            //    Name = "Annapolis Mazda/Mitsubishi"
            //},
            new Store {
                StoreId = "FTN",
                Name = "Chambersburg"
            },
            new Store {
                StoreId = "CJE",
                Name = "Clearwater"
            },
            new Store {
                StoreId = "FAM",
                Name = "Frederick"
            },
            new Store {
                StoreId = "FHT,FHG",
                Name = "Hagerstown"
            },
            //new Store {
            //    StoreId = "FHG",
            //    Name = "Hagerstown GM"
            //},
            new Store {
                StoreId = "LFO",
                Name = "Gaithersburg Hyundai/Subaru"
            },
            new Store {
                StoreId = "LFT",
                Name = "Gaithersburg Toyota/Germantown"
            },
            new Store {
                StoreId = "FLP",
                Name = "Lexington Park"
            },
            new Store {
                StoreId = "FBS,CDO",
                Name = "Rockville"
            },
            //new Store {
            //    StoreId = "CDO",
            //    Name = "Rockville Hyundai"
            //},
            new Store {
                StoreId = "WDC",
                Name = "Wheaton"
            }
        };

        public static IEnumerable<Store> VinStores = new List<Store>
        {
            new Store {
                StoreId = "ALL",
                Name = "All Stores"
            },
            new Store {
                StoreId = "9823",
                Name = "Annapolis"
            },
            new Store {
                StoreId = "9822",
                Name = "Chambersburg"
            },
            new Store {
                StoreId = "9825",
                Name = "Clearwater"
            },
            new Store {
                StoreId = "9828",
                Name = "Frederick"
            },
            new Store {
                StoreId = "8005",
                Name = "Gaithersburg Russell Ave"
            },
            new Store {
                StoreId = "8006",
                Name = "Gaithersburg Toyota"
            },
            new Store {
                StoreId = "20193",
                Name = "Germantown"
            },
            new Store {
                StoreId = "11234",
                Name = "Hagerstown"
            },
            new Store {
                StoreId = "9826",
                Name = "Lexington Park"
            },
            new Store {
                StoreId = "9824",
                Name = "Rockville Nicholson"
            },
            new Store {
                StoreId = "9821",
                Name = "Rockville Hyundai"
            },
            new Store {
                StoreId = "9827",
                Name = "Wheaton"
            }
        };

        public static IEnumerable<Store> StoresReport = new List<Store> {
            new Store {
                StoreId = "ALL",
                Name = "All Stores"
            },
            new Store {
                StoreId = "annapolis",
                Name = "Annapolis Cadillac/Volkswagen"
            },
            new Store {
                StoreId = "annapolis(classic)",
                Name = "Annapolis Mazda/Mitsubishi"
            },
            new Store {
                StoreId = "chambersburg",
                Name = "Chambersburg Toyota/Nissan"
            },
            new Store {
                StoreId = "clearwater",
                Name = "Clearwater"
            },
            new Store {
                StoreId = "frederick",
                Name = "Frederick Baughmans Lane"
            },
            new Store {
                StoreId = "hagerstown(cdjr)",
                Name = "Hagerstown Chrysler"
            },
            new Store {
                StoreId = "hagerstown(gm)",
                Name = "Hagerstown GM"
            },
            new Store {
                StoreId = "superstore",
                Name = "Frederick Route 85"
            },
            new Store {
                StoreId = "lakeforest(russell)",
                Name = "Gaithersburg Hyundai/Subaru"
            },
            new Store {
                StoreId = "germantown",
                Name = "Germantown"
            },
            new Store {
                StoreId = "hagerstown",
                Name = "Hagerstown"
            },
            new Store {
                StoreId = "lakeforest(355)",
                Name = "Gaithersburg Toyota"
            },
            new Store {
                StoreId = "lexingtonpark(lexpark)",
                Name = "Lexington Park"
            },
            new Store {
                StoreId = "nicholson",
                Name = "Rockville Nicholson"
            },
            new Store {
                StoreId = "colonial",
                Name = "Rockville Hyundai"
            },
            new Store {
                StoreId = "subaru",
                Name = "Rockville Subaru"
            },
            new Store {
                StoreId = "wheaton",
                Name = "Wheaton"
            }
        };

        public static IEnumerable<Store> StoreDealerId = new List<Store> {
            new Store {
                StoreId = "annapolis",
                Name = "9823"
            },
            new Store {
                StoreId = "annapolis(classic)",
                Name = "9823"
            },
            new Store {
                StoreId = "chambersburg",
                Name = "9822"
            },
            new Store {
                StoreId = "clearwater",
                Name = "9825"
            },
            new Store {
                StoreId = "frederick",
                Name = "9828"
            },
            new Store {
                StoreId = "Hagerstown(CDJR)",
                Name = "11234"
            },
            new Store {
                StoreId = "Hagerstown(GM)",
                Name = "11234"
            },
            new Store {
                StoreId = "superstore",
                Name = "9828"
            },
            new Store {
                StoreId = "lakeforest(russell)",
                Name = "8005"
            },
            new Store {
                StoreId = "germantown",
                Name = "20193"
            },
            new Store {
                StoreId = "hagerstown",
                Name = "11234"
            },
            new Store {
                StoreId = "Lakeforest(355)",
                Name = "8006"
            },
            new Store {
                StoreId = "lexingtonpark(lexpark)",
                Name = "9826"
            },
            new Store {
                StoreId = "nicholson",
                Name = "9824"
            },
            new Store {
                StoreId = "colonial",
                Name = "9821"
            },
            new Store {
                StoreId = "subaru",
                Name = "9824"
            },
            new Store {
                StoreId = "wheaton",
                Name = "9827"
            }
        };

        public static IEnumerable<CertificationLevel> CertificationLevels = new List<CertificationLevel> {
            new CertificationLevel {
                CertificationID = "",
                Name = ""
            },
            //new CertificationLevel {
            //    CertificationID = "NEW",
            //    Name = "New Vehicle"
            //},
            new CertificationLevel {
                CertificationID = "MC",
                Name = "Manufacturer"
            },
            new CertificationLevel {
                CertificationID = "CUV",
                Name = "Cadillac Certified Pre-Owned"
            },
            new CertificationLevel {
                CertificationID = "MZC",
                Name = "Mazda Certified Pre-Owned"
            },
            new CertificationLevel {
                CertificationID = "GMU",
                Name = "GM Certified Pre-Owned"
            },
            new CertificationLevel {
                CertificationID = "VWC",
                Name = "Volkswagen Certified Pre-Owned"
            },
            new CertificationLevel {
                CertificationID = "FC",
                Name = "FitzWay Certified Pre-Owned"
            },
            new CertificationLevel {
                CertificationID = "FP",
                Name = "FitzWay Premium"
            },
            new CertificationLevel {
                CertificationID = "FWP",
                Name = "FitzWay Premium Used"
            },
            new CertificationLevel {
                CertificationID = "FS",
                Name = "FitzWay Select"
            },
            new CertificationLevel {
                CertificationID = "FWS",
                Name = "FitzWay Select USED"
            },
            new CertificationLevel {
                CertificationID = "FV",
                Name = "Fitzway Value"
            },
            new CertificationLevel {
                CertificationID = "FWV",
                Name = "Fitzway Value Used"
            },
            new CertificationLevel {
                CertificationID = "FWU",
                Name = "Fitzway Used"
            },
            new CertificationLevel {
                CertificationID = "HDM",
                Name = "FitzWay Handyman/Wholesale"
            },
            new CertificationLevel {
                CertificationID = "CPO",
                Name = "Manufacturer Certified Pre-Owned"
            },
            new CertificationLevel {
                CertificationID = "WS",
                Name = "Whosesale"
            }

        };

        public static IEnumerable<CertificationLevel> CPOCodes = new List<CertificationLevel> {
            new CertificationLevel {
                CertificationID = "",
                Name = ""
            },
            new CertificationLevel {
                CertificationID = "F915",
                Name = "FitzWay Value"
            },
            new CertificationLevel {
                CertificationID = "F910",
                Name = "FitzWay Certified"
            },
            new CertificationLevel {
                CertificationID = "F914",
                Name = "FitzWay Plus"
            },
            new CertificationLevel {
                CertificationID = "F916",
                Name = "FitzWay Handyman MD"
            },
            new CertificationLevel {
                CertificationID = "F917",
                Name = "FitzWay Handyman FL"
            },
            new CertificationLevel {
                CertificationID = "F918",
                Name = "FitzWay Handyman PA"
            },
            new CertificationLevel {
                CertificationID = "F924",
                Name = "Other Certs"
            },
            new CertificationLevel {
                CertificationID = "F906",
                Name = "CPO GM"
            },
            new CertificationLevel {
                CertificationID = "F907",
                Name = "CPO CDJR"
            },
            new CertificationLevel {
                CertificationID = "F911",
                Name = "CPO Toyota"
            },
            new CertificationLevel {
                CertificationID = "F912",
                Name = "CPO Subaru"
            },
            new CertificationLevel {
                CertificationID = "F909",
                Name = "CPO Volkswagen"
            },
            new CertificationLevel {
                CertificationID = "F908",
                Name = "CPO Hyundai"
            },
            new CertificationLevel {
                CertificationID = "F922",
                Name = "CPO Cadillac"
            },
            new CertificationLevel {
                CertificationID = "F913",
                Name = "CPO Nissan"
            },
            new CertificationLevel {
                CertificationID = "F923",
                Name = "CPO Mazda"
            },
        };
        


        public static IEnumerable<RateException> RateExceptions = new List<RateException> {
            new RateException {
                ExceptionID = "",
                Name = ""
            },
            new RateException {
                ExceptionID = "PMR",
                Name = "Dealer Participation Limited by Finance Source"
            },
            new RateException {
                ExceptionID = "CC",
                Name = "Customer Stated Monthly Payment Constraint"
            },
            new RateException {
                ExceptionID = "RM",
                Name = "Customer Stated Competing Offer"
            },
            //new RateException {
            //    ExceptionID = "SP",
            //    Name = "Dealer Promo"
            //},            
            new RateException {
                ExceptionID = "SVR",
                Name = "Customer Qualified for Subvened Interest Rate"
            },
            new RateException {
                ExceptionID = "FER",
                Name = "Customer Qualified for Dealership Employee Incentive Program"
            },
            //new RateException {
            //    ExceptionID = "IPL",
            //    Name = "Lender Portfolio Enhancement"
            //},
            //new RateException {
            //    ExceptionID = "OTH",
            //    Name = "Inventory Reduction Criteria"
            //},
            new RateException {
                ExceptionID = "MH",
                Name = "Max Rate Held"
            }
            //new RateException {
            //    ExceptionID = "FR",
            //    Name = "Fixed Rate"
            //}

        };

        public static IEnumerable<PriceException> PriceExceptions = new List<PriceException> {
            new PriceException {
                ExceptionID = "",
                Name = ""
            },
            new PriceException {
                ExceptionID = "DAC",
                Name = "Dealer Accommodation"
            },
            new PriceException {
                ExceptionID = "EPM",
                Name = "External Price Match"
            },
            new PriceException {
                ExceptionID = "IPM",
                Name = "Internal Price Match"
            },
            new PriceException {
                ExceptionID = "IHC",
                Name = "In-House Coupon"
            },            
            new PriceException {
                ExceptionID = "MER",
                Name = "Manager Error"
            },
            new PriceException {
                ExceptionID = "NEG",
                Name = "Negotiated"
            },
            new PriceException {
                ExceptionID = "EMP",
                Name = "Employee/Manufacturer Price"
            },
        };

        public static IEnumerable<StoreLocation> StoreLocations = new List<StoreLocation> {
            new StoreLocation {
                StoreId = "annapolis",
                LocationId = "FOC"
            },
            new StoreLocation {
                StoreId = "annapolis(classic)",
                LocationId = "FMM"
            },
            new StoreLocation {
                StoreId = "chambersburg",
                LocationId = "FTN"
            },
            new StoreLocation {
                StoreId = "clearwater",
                LocationId = "CJE"
            },
            new StoreLocation {
                StoreId = "frederick",
                LocationId = "FAM"
            },
            new StoreLocation {
                StoreId = "superstore",
                LocationId = "FSS"
            },
            new StoreLocation {
                StoreId = "Hagerstown(CDJR)",
                LocationId = "FHT"
            },
            new StoreLocation {
                StoreId = "Hagerstown(GM)",
                LocationId = "FHG"
            },
            new StoreLocation {
                StoreId = "lakeforest(russell)",
                LocationId = "LFO"
            },
            new StoreLocation {
                StoreId = "germantown",
                LocationId = "LFM"
            },
            new StoreLocation {
                StoreId = "hagerstown",
                LocationId = "FHT"
            },
            new StoreLocation {
                StoreId = "Lakeforest(355)",
                LocationId = "LFT"
            },
            new StoreLocation {
                StoreId = "lexingtonpark(lexpark)",
                LocationId = "FLP"
            },
            new StoreLocation {
                StoreId = "nicholson",
                LocationId = "FBN"
            },
            new StoreLocation {
                StoreId = "colonial",
                LocationId = "CDO"
            },
            new StoreLocation {
                StoreId = "subaru",
                LocationId = "FBS"
            },
            new StoreLocation {
                StoreId = "wheaton",
                LocationId = "WDC"
            }
        };

        public static IEnumerable<Location> Locations = new List<Location> {
            new Location {
                LocationId = "FOCCD",
                Name = "Annapolis Cadillac"
            },
            new Location {
                LocationId = "FOCVW",
                Name = "Annapolis Volkswagen"
            },
            new Location {
                LocationId = "FOCUsed",
                Name = "Annapolis Cadillac/Volkswagen Used"
            },
            new Location {
                LocationId = "FMMMA",
                Name = "Annapolis Mazda"
            },
            new Location {
                LocationId = "FMMMI",
                Name = "Annapolis Mitsubishi"
            },
            new Location {
                LocationId = "FMMUsed",
                Name = "Annapolis Mazda/Mitsubishi Used"
            },
            new Location {
                LocationId = "FTNNI",
                Name = "Chambersburg Nissan"
            },
            new Location {
                LocationId = "FTNTO",
                Name = "Chambersburg Toyota"
            },
            new Location {
                LocationId = "FTNUsed",
                Name = "Chambersburg Used"
            },
            new Location {
                LocationId = "CJECH",
                Name = "Clearwater Chrysler"
            },
            new Location {
                LocationId = "CJEUsed-CJE",
                Name = "Clearwater Chrysler Used"
            },
            new Location {
                LocationId = "CJEHY",
                Name = "Clearwater Hyundai"
            },
            new Location {
                LocationId = "CJEUsed-CHY",
                Name = "Clearwater Hyundai Used"
            },
            new Location {
                LocationId = "CJEXG",
                Name = "Clearwater Genesis"
            },
            new Location {
                LocationId = "CJEJE",
                Name = "Clearwater Jeep"
            },
            new Location {
                LocationId = "CJEUsed-COC",
                Name = "Clearwater Outlet Center"
            },

            new Location {
                LocationId = "CJESU",
                Name = "Clearwater Subaru"
            },
            new Location {
                LocationId = "CJEUsed-CSS",
                Name = "Clearwater Subaru Used"
            },

            //new Location {
            //    LocationId = "CJEUsed",
            //    Name = "Clearwater Used"
            //},
            new Location {
                LocationId = "FAMCD",
                Name = "Frederick Cadillac"
            },
            new Location {
                LocationId = "FAMCV",
                Name = "Frederick Chevrolet"
            },
            new Location {
                LocationId = "FAMMA",
                Name = "Frederick Mazda"
            },
            new Location {
                LocationId = "FAMVW",
                Name = "Frederick Volkswagen"
            },
            new Location {
                LocationId = "FAMUsed",
                Name = "Frederick Baughmans Used"
            },
            new Location {
                LocationId = "FSSUsed",
                Name = "Frederick Rt 85 Used"
            },
            new Location {
                LocationId = "FHTCH",
                Name = "Hagerstown Chrysler"
            },
            new Location {
                LocationId = "FHTDO",
                Name = "Hagerstown Dodge"
            },
            new Location {
                LocationId = "FHTJE",
                Name = "Hagerstown Jeep"
            },
            new Location {
                LocationId = "FHTRAM",
                Name = "Hagerstown RAM"
            },
            new Location {
                LocationId = "FHTUsed",
                Name = "Hagerstown Used"
            },
            new Location {
                LocationId = "FHGCD",
                Name = "Hagerstown Cadillac"
            },
            new Location {
                LocationId = "FHGCV",
                Name = "Hagerstown Chevrolet"
            },
            new Location {
                LocationId = "FHGUsed",
                Name = "Hagerstown Used"
            },
            new Location {
                LocationId = "LFOHY",
                Name = "Lakeforest Hyundai"
            },
            new Location {
                LocationId = "LFOXG",
                Name = "Lakeforest Genesis"
            },
            new Location {
                LocationId = "LFOSU",
                Name = "Lakeforest Subaru"
            },
            new Location {
                LocationId = "LFOUsed",
                Name = "Lakeforest Hyundai/Subaru Used"
            },
            new Location {
                LocationId = "LFTTO",
                Name = "Gaithersburg Toyota"
            },
            new Location {
                LocationId = "LFTUsed",
                Name = "Gaithersburg Toyota Used"
            },
            new Location {
                LocationId = "FLPCH",
                Name = "Lexington Park Chrysler"
            },
            new Location {
                LocationId = "FLPDO",
                Name = "Lexington Park Dodge"
            },
            new Location {
                LocationId = "FLPJE",
                Name = "Lexington Park Jeep"
            },
            new Location {
                LocationId = "FLPRAM",
                Name = "Lexington Park RAM"
            },
            new Location {
                LocationId = "FLPUsed",
                Name = "Lexington Park Used"
            },
            new Location {
                LocationId = "LFMUsed",
                Name = "Germantown"
            },            
            new Location {
                LocationId = "WDCUsed",
                Name = "Wheaton"
            },
            new Location {
                LocationId = "FBNBU",
                Name = "White Flint Buick"
            },
            new Location {
                LocationId = "FBNGC",
                Name = "White Flint GMC"
            },
            new Location {
                LocationId = "FBNUsed",
                Name = "White Flint Buick/GMC Used"
            },
            new Location {
                LocationId = "CDOHY",
                Name = "White Flint Hyundai"
            },
            new Location {
                LocationId = "CDOXG",
                Name = "White Flint Genesis"
            },
            new Location {
                LocationId = "CDOUsed",
                Name = "White Flint Hyundai Used"
            },
            new Location {
                LocationId = "FBSSU",
                Name = "White Flint Subaru"
            },
            new Location {
                LocationId = "FBSUsed",
                Name = "White Flint Subaru Used"
            }

        };

        public static IEnumerable<Location> SecurityLocations = new List<Location> {
            new Location {
                LocationId = "FOC",
                Name = "Annapolis Cadillac/Volkswagen"
            },
            new Location {
                LocationId = "FMM",
                Name = "Annapolis Mazda/Mitsubishi"
            },
            new Location {
                LocationId = "FTN",
                Name = "Chambersburg"
            },
            new Location {
                LocationId = "CJE",
                Name = "Clearwater"
            },
            new Location {
                LocationId = "FAM",
                Name = "Frederick Baughmans"
            },
            new Location {
                LocationId = "FSS",
                Name = "Frederick Rt 85"
            },
            new Location {
                LocationId = "LFO",
                Name = "Lakeforest Hyundai/Subaru"
            },
            new Location {
                LocationId = "LFT",
                Name = "Gaithersburg Toyota"
            },
            new Location {
                LocationId = "FLP",
                Name = "Lexington Park"
            },
            new Location {
                LocationId = "LFM",
                Name = "Germantown"
            },
            new Location {
                LocationId = "FHT",
                Name = "Hagerstown Chrysler"
            },
            new Location {
                LocationId = "FHG",
                Name = "Hagerstown GM"
            },
            new Location {
                LocationId = "WDC",
                Name = "Wheaton"
            },
            new Location {
                LocationId = "FBN",
                Name = "White Flint Buick/GMC"
            },
            new Location {
                LocationId = "CDO",
                Name = "White Flint Hyundai"
            },
            new Location {
                LocationId = "FBS",
                Name = "White Flint Subaru"
            }

        };

        public static IEnumerable<Location> AppraisalLocations = new List<Location> {
            new Location {
                LocationId = "FOC",
                Name = "Annapolis Cadillac/Volkswagen"
            },
            new Location {
                LocationId = "FMM",
                Name = "Annapolis Mazda/Mitsubishi"
            },
            new Location {
                LocationId = "FTN",
                Name = "Chambersburg"
            },
            new Location {
                LocationId = "FTO",
                Name = "Chambersburg"
            },
            new Location {
                LocationId = "CJE",
                Name = "Clearwater Chrysler/Jeep"
            },
            new Location {
                LocationId = "CHS",
                Name = "Clearwater Hyundai/Subaru"
            },
            new Location {
                LocationId = "CHY",
                Name = "Clearwater Hyundai"
            },
            new Location {
                LocationId = "CSS",
                Name = "Clearwater Subaru"
            },
            new Location {
                LocationId = "COC",
                Name = "Clearwater Outlet Center"
            },
            new Location {
                LocationId = "FAM",
                Name = "Frederick Baughmans"
            },
            new Location {
                LocationId = "FCG",
                Name = "Frederick Baughmans"
            },
            new Location {
                LocationId = "FSS",
                Name = "Frederick Rt 85"
            },
            new Location {
                LocationId = "LFO",
                Name = "Lakeforest Hyundai/Subaru"
            },
            new Location {
                LocationId = "LFT",
                Name = "Gaithersburg Toyota"
            },
            new Location {
                LocationId = "LFU",
                Name = "Gaithersburg Used"
            },
            new Location {
                LocationId = "FLP",
                Name = "Lexington Park"
            },
            new Location {
                LocationId = "LFM",
                Name = "Germantown"
            },
            new Location {
                LocationId = "FHT",
                Name = "Hagerstown Chrysler"
            },
            new Location {
                LocationId = "FHG",
                Name = "Hagerstown GM"
            },
            new Location {
                LocationId = "WDC",
                Name = "Wheaton"
            },
            new Location {
                LocationId = "WFN",
                Name = "White Flint Buick/GMC"
            },
            new Location {
                LocationId = "FBN",
                Name = "White Flint Buick/GMC"
            },
            new Location {
                LocationId = "WF",
                Name = "White Flint Hyundai/Subaru"
            },
            new Location {
                LocationId = "CDO",
                Name = "White Flint Hyundai"
            },
            new Location {
                LocationId = "FBS",
                Name = "White Flint Subaru"
            },
            new Location {
                LocationId = "FBC",
                Name = "White Flint Subaru"
            }
            //,
            //new Location {
            //    LocationId = "FBS",
            //    Name = "White Flint Subaru"
            //}

        };

        public static IEnumerable<Location> SoldLocations = new List<Location> {
            new Location {
                LocationId = "FOC",
                Name = "Annapolis"
            },
            new Location {
                LocationId = "FTN",
                Name = "Chambersburg"
            },
            new Location {
                LocationId = "FTO",
                Name = "Chambersburg"
            },
            new Location {
                LocationId = "CJE",
                Name = "Clearwater"
            },
            new Location {
                LocationId = "FAM",
                Name = "Frederick"
            },
            new Location {
                LocationId = "FCG",
                Name = "Frederick"
            },
            new Location {
                LocationId = "LFO",
                Name = "Gaithersburg Hyundai/Subaru"
            },
            new Location {
                LocationId = "LFT",
                Name = "Gaithersburg Toyota"
            },
            new Location {
                LocationId = "LFU",
                Name = "Gaithersburg Used"
            },
            new Location {
                LocationId = "FLP",
                Name = "Lexington Park"
            },
            new Location {
                LocationId = "LFM",
                Name = "Germantown"
            },
            new Location {
                LocationId = "FHT",
                Name = "Hagerstown Chrysler"
            },
            new Location {
                LocationId = "FHG",
                Name = "Hagerstown GM"
            },
            new Location {
                LocationId = "WDC",
                Name = "Wheaton"
            },
            new Location {
                LocationId = "FBS",
                Name = "Rockville Nicholson"
            },
            new Location {
                LocationId = "FBC",
                Name = "White Flint Subaru"
            },
            new Location {
                LocationId = "CDO",
                Name = "Rockville Hyundai"
            }
            //,
            //new Location {
            //    LocationId = "FBS",
            //    Name = "White Flint Subaru"
            //}

        };

        public static IEnumerable<Location> ShowroomLocations = new List<Location> {
            new Location {
                LocationId = "48",
                Name = "Rockville Hyundai Used"
            },
            new Location {
                LocationId = "70",
                Name = "Clearwater Hyundai Used"
            },
            new Location {
                LocationId = "94",
                Name = "Clearwater Chrysler/Jeep Used"
            },
            new Location {
                LocationId = "95",
                Name = "Clearwater Subaru Used"
            },
            new Location {
               LocationId = "96",
                Name = "Clearwater Outlet Center Used"
            },
            new Location {
                LocationId = "164",
                Name = "Frederick Route 85 Used"
            },
                        new Location {
                LocationId = "36",
                Name = "Frederick Baughmans Used"
            },
            new Location {
                LocationId = "5",
                Name = "Rockville Subaru Used"
            },
            new Location {
                LocationId = "9",
                Name = "Rockville Buick/GMC Used"
            },

            new Location {
                LocationId = "161",
                Name = "Hagerstown GM Used"
            },
            new Location {
                LocationId = "158",
                Name = "Hagerstown Chrysler Used"
            },
            new Location {
                LocationId = "91",
                Name = "Lexington Park Used"
            },
            new Location {
                LocationId = "24",
                Name = "Annapolis Cadillac/Volkswagen Used"
            },
            new Location {
                LocationId = "29",
                Name = "Annapolis Mazda/Mitsubishi Used"
            },
            new Location {
                LocationId = "40",
                Name = "Chambersburg Used"
            },
            new Location {
                LocationId = "13",
                Name = "Gaithersburg Hyundai/Subaru Used"
            },
            new Location {
                LocationId = "111",
                Name = "Gaithersburg Handyman"
            },
            new Location {
                LocationId = "60",
                Name = "Gaithersburg Toyota Used"
            },
            new Location {
                LocationId = "87",
                Name = "Germantown Used"
            },
            new Location {
                LocationId = "20",
                Name = "Wheaton Used"
            },
            new Location {
                LocationId = "20",
                Name = "Wheaton Used"
            },
                new Location {
                LocationId = "165", 
                Name = "InterCompany Transfer Rockville Hyundai"
            },
                
            new Location { LocationId = "166",Name = "InterCompany Transfer Rockville Subaru" },
            new Location { LocationId = "167",Name = "InterCompany Transfer Rockville Nicholson"},
            new Location { LocationId = "168",Name = "InterCompany Transfer Gaithersburg Hyundai/Subaru"},
            new Location { LocationId = "169",Name = "InterCompany Transfer Wheaton"},
            new Location { LocationId = "170",Name = "InterCompany Transfer Annapolis Cadillac/Volkswagen"},
            new Location { LocationId = "171",Name = "InterCompany Transfer Annapolis Mazda/Mitsubishi"},
            new Location { LocationId = "172",Name = "InterCompany Transfer Frederick"},
            new Location { LocationId = "173",Name = "InterCompany Transfer Chambersburg"},
            new Location { LocationId = "174",Name = "InterCompany Transfer Clearwater"},
            new Location { LocationId = "175",Name = "InterCompany Transfer Gaithersburg Toyota"},
            new Location { LocationId = "176",Name = "InterCompany Transfer Frederick Rt 85"},
            new Location { LocationId = "178",Name = "InterCompany Transfer Germantown"},
            new Location { LocationId = "179",Name = "InterCompany Transfer Lexington Park"},
            new Location { LocationId = "180",Name = "InterCompany Transfer Hagerstown Chrysler"},
            new Location { LocationId = "181",Name = "InterCompany Transfer Hagerstown GM" }

        };

        public static IEnumerable<Location> ShowroomIDMapping = new List<Location> {
            new Location {
                LocationId = "48",
                Name = "CDO"
            },
            new Location {
                LocationId = "70",
                Name = "CHY"
            },
            new Location {
                LocationId = "94",
                Name = "CJE"
            },
            new Location {
                LocationId = "95",
                Name = "CSS"
            },
            new Location {
               LocationId = "96",
                Name = "COC"
            },
            new Location {
                LocationId = "164",
                Name = "FSS"
            },
                        new Location {
                LocationId = "36",
                Name = "FCG"
            },
            new Location {
                LocationId = "5",
                Name = "FBC"
            },
            new Location {
                LocationId = "9",
                Name = "FBN"
            },

            new Location {
                LocationId = "161",
                Name = "FHG"
            },
            new Location {
                LocationId = "158",
                Name = "FHT"
            },
            new Location {
                LocationId = "91",
                Name = "FLP"
            },
            new Location {
                LocationId = "24",
                Name = "FOC"
            },
            new Location {
                LocationId = "29",
                Name = "FMM"
            },
            new Location {
                LocationId = "40",
                Name = "FTO"
            },
            new Location {
                LocationId = "13",
                Name = "LFO"
            },
            new Location {
                LocationId = "60",
                Name = "LFT"
            },
            new Location {
                LocationId = "87",
                Name = "LFM"
            },
            new Location {
                LocationId = "20",
                Name = "WDC"
            },

        };

        public static IEnumerable<VehicleStatus> VehicleStatuses = new List<VehicleStatus>
        {
            new VehicleStatus {
                StatusId = "1",
                Name = "In Stock (1)"
            },
            new VehicleStatus {
                StatusId = "2",
                Name = "Deal Pending (2)"
            },
            new VehicleStatus {
                StatusId = "3",
                Name = "Demo Vehicles (3)"
            },
            new VehicleStatus {
                StatusId = "4",
                Name = "Loaner (4)"
            },
            new VehicleStatus {
                StatusId = "5",
                Name = "Sold - Not Delivered (5)"
            },
            new VehicleStatus {
                StatusId = "6",
                Name = "Delivered (6)"
            },
            new VehicleStatus {
                StatusId = "8",
                Name = "Wholesale (8)"
            },
            new VehicleStatus {
                StatusId = "9",
                Name = "Wholesale/Auction (9)"
            },
            new VehicleStatus {
                StatusId = "14",
                Name = "Stop Sale (14)"
            },
            new VehicleStatus {
                StatusId = "15",
                Name = "Company Vehicle (15)"
            },
            new VehicleStatus {
                StatusId = "20",
                Name = "Service Loaner (20)"
            },
            new VehicleStatus {
                StatusId = "21",
                Name = "Nextcar (21)"
            },


        };
        
        public static IEnumerable<Brand> Brands = new List<Brand>
        {
            new Brand {
                BrandId = "ALL",
                Name = "All Brands"
            },
            new Brand {
                BrandId = "AA",
                Name = "Additional Gross"
            },
            new Brand {
                BrandId = "BU",
                Name = "Buick"
            },
            new Brand {
                BrandId = "CD",
                Name = "Cadillac"
            },
            new Brand {
                BrandId = "CV",
                Name = "Chevrolet"
            },
            new Brand {
                BrandId = "CH",
                Name = "Chrysler"
            },
            new Brand {
                BrandId = "DO",
                Name = "Dodge"
            },
            new Brand {
                BrandId = "GC",
                Name = "GMC"
            },
            new Brand {
                BrandId = "HY",
                Name = "Hyundai"
            },
            new Brand {
                BrandId = "JE",
                Name = "Jeep"
            },
            new Brand {
                BrandId = "MA",
                Name = "Mazda"
            },
            new Brand {
                BrandId = "MI",
                Name = "Mitsubishi"
            },
            new Brand {
                BrandId = "NI",
                Name = "Nissan"
            },
            new Brand {
                BrandId = "RAM",
                Name = "RAM"
            },
            new Brand {
                BrandId = "SU",
                Name = "Subaru"
            },
            new Brand {
                BrandId = "TO",
                Name = "Toyota"
            },
            new Brand {
                BrandId = "UU",
                Name = "Used"
            },
            new Brand {
                BrandId = "VW",
                Name = "Volkswagen"
            }
        };

        public static IEnumerable<Payscale> Payscales = new List<Payscale> {
            new Payscale {
                PayscaleID = "STDVOL",
                Name = "Standard Volume Store Plan"
            },
            new Payscale {
                PayscaleID = "LOWVOL",
                Name = "Base Volume Store Plan"
            },
            new Payscale {
                PayscaleID = "MOCO",
                Name = "Montgomery County Plan"
            },            
            new Payscale {
                PayscaleID = "FL",
                Name = "Florida Store Plan"
            },
            new Payscale {
                PayscaleID = "PROFMA",
                Name = "Proforma Pay Plan"
            }

        };

        public static IEnumerable<Payscale> NewPayscales = new List<Payscale> {
            new Payscale {
                PayscaleID = "COMSPIFF",
                Name = "Compensation Plan A"
            },
            new Payscale {
                PayscaleID = "COMNOSPIFF",
                Name = "Compensation Plan B"
            },
            new Payscale {
                PayscaleID = "COMFL",
                Name = "Compensation Plan - Florida"
            },
            new Payscale {
                PayscaleID = "COMFRED",
                Name = "Compensation Plan - Frederick"
            }
            ,
            new Payscale {
                PayscaleID = "COMTEMP2",
                Name = "Compensation Plan - Frederick Rt85"
            },
            new Payscale {
                PayscaleID = "COMCHBG",
                Name = "Compensation Plan - Chambersburg"
            },
            new Payscale {
                PayscaleID = "COMHGR",
                Name = "Compensation Plan - Hagerstown"
            },
            new Payscale {
                PayscaleID = "COMTEMP1",
                Name = "Compensation Plan Temporary - Mazda"
            },

        };

        public static IEnumerable<Payscale> PointPlans = new List<Payscale> {
            new Payscale {
                PayscaleID = "ALL",
                Name = "Aftermarket Points"
            },
            new Payscale {
                PayscaleID = "PCT",
                Name = "Aftermarket Points with Percents"
            }
        };


        public static IEnumerable<SSI> SSIs = new List<SSI> {
            new SSI {
                SSIID = "Yes",
                Name = "Yes"
            },
            new SSI {
                SSIID = "No",
                Name = "No"
            }
        };

        public static IEnumerable<SSI> SSIFTN = new List<SSI> {
            new SSI {
                SSIID = "Low",
                Name = "Low"
            },
            new SSI {
                SSIID = "Med",
                Name = "Medium"
            },
            new SSI {
                SSIID = "High",
                Name = "High"
            }
        };


        public static IEnumerable<PayLevel> PayLevelsFTN = new List<PayLevel> {
            new PayLevel {
                PayLevelID = "STD",
                Name = "Standard"
            }
        };

        public static IEnumerable<PayLevel> PayLevels = new List<PayLevel> {
            new PayLevel {
                PayLevelID = "STD",
                Name = "Standard"
            },
            new PayLevel {
                PayLevelID = "CERT",
                Name = "Certified"
            },
            new PayLevel {
                PayLevelID = "CERTELIT",
                Name = "Certified Elite"
            }

        };


        public static IEnumerable<StoreVolume> StoreVolumes = new List<StoreVolume> {
            new StoreVolume {
                StoreVolumeID = "DFLT",
                Name = "Use Default"
            },
            new StoreVolume {
                StoreVolumeID = "Low",
                Name = "Low"
            },
            new StoreVolume {
                StoreVolumeID = "High",
                Name = "High"
            }

        };

        public static IEnumerable<AssociateStatus> AssociateStatuses = new List<AssociateStatus> {
            new AssociateStatus {
                AssociateStatusID = "Mentor",
                Name = "Sales Leader"
            },
            new AssociateStatus {
                AssociateStatusID = "Mentee",
                Name = "Mentee"
            },
            new AssociateStatus {
                AssociateStatusID = "NA",
                Name = "N/A"
            }
        };

        public static IEnumerable<Condition> Conditions = new List<Condition> {
            new Condition {
                ConditionId = "ALL",
                Name = "All"
            },
            new Condition {
                ConditionId = "NEW",
                Name = "New"
            },
            new Condition {
                ConditionId = "USED",
                Name = "Used"
            }
        };

        public static IEnumerable<Order> SaleOrder = new List<Order>
        {
            new Order {
                StoreId = "annapolis",
                OrderId = 1
            },
            new Order {
                StoreId = "annapolis(classic)",
                OrderId = 2
            },
            new Order {
                StoreId = "chambersburg",
                OrderId = 3
            },
            new Order {
                StoreId = "clearwater",
                OrderId = 4
            },
            new Order {
                StoreId = "frederick",
                OrderId = 5
            },
            new Order {
                StoreId = "superstore",
                OrderId = 6
            },
            new Order {
                StoreId = "Hagerstown(CDJR)",
                OrderId = 8
            },
            new Order {
                StoreId = "Hagerstown(GM)",
                OrderId = 7
            },
            new Order {
                StoreId = "lakeforest(russell)",
                OrderId = 11
            },
            new Order {
                StoreId = "germantown",
                OrderId = 9
            },
            new Order {
                StoreId = "Lakeforest(355)",
                OrderId = 10
            },
            new Order {
                StoreId = "lexingtonpark(lexpark)",
                OrderId = 15
            },
            new Order {
                StoreId = "nicholson",
                OrderId = 14
            },
            new Order {
                StoreId = "colonial",
                OrderId = 12
            },
            new Order {
                StoreId = "subaru",
                OrderId = 13
            },
            new Order {
                StoreId = "wheaton",
                OrderId = 16
            }
        };

    }

}

public class Order
{
    public int OrderId { get; set; }
    public string StoreId { get; set; }
}

public class Condition
{

    public string ConditionId { get; set; }
    public string Name { get; set; }

}

public class YesNo
{

    public string Id { get; set; }
    public string Name { get; set; }

}
public class Store
{

    public string StoreId { get; set; }
    public string Name { get; set; }

}

public class StoreLocation
{
    public string StoreId { get; set; }
    public string LocationId { get; set; }

}

public class Month
{
    public int MonthId { get; set; }
    public string Name { get; set; }
}

public class Year
{
    public int YearId { get; set; }
    public string Name { get; set; }
}

public class Location
{
    public string LocationId { get; set; }
    public string Name { get; set; }
}

public class CertificationLevel
{

    public string CertificationID { get; set; }
    public string Name { get; set; }

}

public class RateException
{

    public string ExceptionID { get; set; }
    public string Name { get; set; }

}

public class PriceException
{

    public string ExceptionID { get; set; }
    public string Name { get; set; }

}

public class Payscale
{
    public string PayscaleID { get; set; }
    public string Name { get; set; }

}

public class SSI
{
    public string SSIID { get; set; }
    public string Name { get; set; }

}

public class PayLevel
{
    public string PayLevelID { get; set; }
    public string Name { get; set; }

}

public class StoreVolume
{
    public string StoreVolumeID { get; set; }
    public string Name { get; set; }

}

public class AssociateStatus
{
    public string AssociateStatusID { get; set; }
    public string Name { get; set; }

}

public class Brand
{
    public string BrandId { get; set; }
    public string Name { get; set; }

}

public class VehicleStatus
{
    public string StatusId { get; set; }
    public string Name { get; set; }

}