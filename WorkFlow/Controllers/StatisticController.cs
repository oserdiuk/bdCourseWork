using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkFlow.Models;
using WorkFlow.Models.DataBaseModels;

namespace WorkFlow.Controllers
{
    public class StatisticController : Controller
    {
        // GET: Statistic
        public ActionResult Index()
        {
            DBContext context = new DBContext();
            string query = "Select Distinct City as 'City1', City as 'City', Count(Id) as 'Number of companies' From Companies Group by City;";
            context.DBDataTable = DatabaseController.GetDataTable(query); 
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
                case Controllers.Statistics.SkillsForVacancy:
                    query = @"SELECT S.Id, Cat.CategoryName as 'Category', S.Name as 'Name of skill', V.Name as 'Vacancy name', FORMAT(res.skillPopularity, 'p') as '%'
                            FROM Categories Cat, Vacancies V, Requirements R, Skills S
                            INNER JOIN (
                            SELECT S1.Id, COUNT(V1.Id) * 1.0 / (Select COUNT(Id) From Vacancies) as skillPopularity 
                            FROM Vacancies V1, Skills S1, Requirements R1 WHERE V1.Id = R1.VacancyId AND R1.SkillId = S1.Id 
                            GROUP BY S1.Id 
                            ) AS res ON S.Id = res.Id
                            WHERE S.CategoryId = Cat.Id And R.VacancyId = V.Id And R.SkillId = S.Id 
                            GROUP By Cat.CategoryName, S.Name, S.Id, V.Name, res.skillPopularity 
                            ORDER BY res.skillPopularity DESC;
                            ";
                    break;
                case Controllers.Statistics.VacanciesInRegion:
                    query = @"SELECT V.Id, V.Name as 'Vacancy name', C.Name 'Company name', V.OpenDate as 'Open date' From Vacancies V, Companies C Where C.Id = V.CompanyId AND V.OpenDate > DATEADD(day, -30, GETDATE())";
                    break;
                default:
                    break;
            }

            DBContext context = new DBContext();
            context.DBDataTable = DatabaseController.GetDataTable(query);
            ViewBag.FilterCompanySucceed = true;
            ViewBag.StatisticCompanySucceed = true;

            if (statistic == Controllers.Statistics.SkillsForVacancy)
            {
                SkillsStatistics model = new SkillsStatistics(context.DBDataTable);
                return PartialView("~/Views/Statistic/_SkillStatisticPartial.cshtml", model);
            }
            return PartialView("~/Views/Statistic/_CompanyStatisticResultPartial.cshtml", context);
        }
    }
}