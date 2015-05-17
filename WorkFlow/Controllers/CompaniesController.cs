using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkFlow.Models;
using WorkFlow.Models.DataBaseModels;
using Dapper;
using DevExpress.XtraRichEdit;

namespace WorkFlow.Controllers
{
    public class CompaniesController : Controller
    {

        // GET: Companies
        public ActionResult Index()
        {
            DBContext context = new DBContext();
            context.Companies = DatabaseController.DoSQL<Companies>("SELECT * FROM Companies");
            ViewBag.Companies = context.Companies;
            ViewBag.Cities = DatabaseController.GetCities();
            return View(context);
        }

        public PartialViewResult Statistics(Statistics statistic = 0)
        {
            string query = "";

            switch (statistic)
            {
                case Controllers.Statistics.CompanyInCities:
                    query = "Select Distinct City as 'City1', City as 'City', Count(Id) as 'Number of companies' From Companies Group by City;";
                    break;
                case Controllers.Statistics.SkillSForVacancy:
                    break;
                case Controllers.Statistics.VacanciesInRegion:
                    break;
                default:
                    break;
            }

            DBContext context = new DBContext();
            context.DBDataTable = DatabaseController.GetDataTable(query);
            ViewBag.FilterCompanySucceed = true;
            ViewBag.StatisticCompanySucceed = true;
            return PartialView("~/Views/Companies/_StatisticsPartial.cshtml", context);
        }


    }
}