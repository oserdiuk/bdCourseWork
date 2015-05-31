using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkFlow.Models;
using WorkFlow.Models.DataBaseModels;

namespace WorkFlow.Controllers
{
    public class SearchController : Controller
    {
        // GET: Search
        public ActionResult Index()
        {
            return View();
        }

        public PartialViewResult SearchForCompany(SearchCompanyCriteria model)
        {
            string query = "";
            if (model.VacancyName != null || model.Name != null || model.City != null)
            {
                string vacancySync = model.VacancyName != null ? @", Vacancies V Where C.Id = V.CompanyId" : " Where ";
                string selectList = model.VacancyName == null ? "" : ", V.Name as N'Vacancy name'";
                string and = model.VacancyName != null ? " And " : "";
                string nameSearch = model.Name == null ? "" : String.Format(" {0} C.Name Like '{1}%'", and, model.Name);
                and = model.VacancyName != null || model.Name != null ? " And " : "";
                string citySearch = model.City == null ? "" : String.Format("{0} C.City Like N'{1}%'", and, model.City);
                string vacancySearch = "";
                if (model.VacancyName != null)
                {
                    var vacancyArray = model.VacancyName.Split(' ');
                    foreach (var v in vacancyArray)
                    {
                        vacancySearch += String.Format(" And V.Name Like '%{0}%'", v);
                    }
                }
                query = String.Format(@"Select Distinct C.Id, C.Name, C.Website, C.City,  C.Address, C.Email {0} From Companies C{1} {2} {3} {4}",
                    selectList, vacancySync, nameSearch, citySearch, vacancySearch);
            }
            DBContext context = new DBContext();
            context.DBDataTable = DatabaseController.GetDataTable(query);

            return PartialView("~/Views/Search/_SearchResultPartial.cshtml", context);
        }

        public PartialViewResult SearchForVacancy(SearchVacancyCriteria model)
        {
            string query = "";
            string query11 = @"SELECT Distinct V.Id, V.CompanyId, V.Name, V.OpenDate 
                        FROM Vacancies V LEFT OUTER JOIN (Select * From Skills S, Requirements R Where S.Id = R.SkillId) X 
                        ON V.Id = X.VacancyId Where V.CompanyId = 7 And X.Name IN (N'RUP', N'ASP.NET MVC') And V.Name = N'Project Manager'";

            DBContext context = new DBContext();

            if (model.Company == null & model.Name == null & model.Skills.Count() == 0)
            {
                query = @"SELECT Distinct V.Id, V.CompanyId, V.Name, V.OpenDate FROM Vacancies V ";
            }
            else
            {
                string skills = "X.Name IN (";

                if (model.Skills.Count() != 0)
                {
                    if (model.Skills.Count() == 1)
                    {
                        skills = "X.Name = N'" + model.Skills[0] + "'";
                    }
                    else
                    {
                        foreach (var c in model.Skills)
                        {
                            skills += model.Skills.Count() != (model.Skills.IndexOf(c) + 1) ? "N'" + c + "'" + ", " : "N'" + c + "')";
                        }
                    }
                }
                else
                {
                    skills = "";
                }

                query = String.Format(@"SELECT Distinct V.Id, V.CompanyId, V.Name, V.OpenDate 
                        FROM Vacancies V LEFT OUTER JOIN (Select * From Skills S, Requirements R Where S.Id = R.SkillId) X 
                        ON V.Id = X.VacancyId Where {0} ", skills);

                if (model.Company != null)
                {
                    query = DatabaseController.AppendANDStatementToQuery(query, "V.CompanyId = N'" + model.Company + "'");
                }

                if (model.Name != null)
                {
                    query = DatabaseController.AppendANDStatementToQuery(query, "V.Name = N'" + model.Name + "'");
                }
            }

            context.DBDataTable = DatabaseController.GetDataTable(query);
            return PartialView("~/Views/Vacancies/_VacanciesListPartial.cshtml", context);


            //if (model. != null || model.Name != null || model.City != null)
            //{
            //    string vacancySync = model.VacancyName != null ? @", Vacancies V Where C.Id = V.CompanyId" : " Where ";
            //    string selectList = model.VacancyName == null ? "" : ", V.Name as N'Vacancy name'";
            //    string and = model.VacancyName != null ? " And " : "";
            //    string nameSearch = model.Name == null ? "" : String.Format(" {0} C.Name Like '{1}%'", and, model.Name);
            //    and = model.VacancyName != null || model.Name != null ? " And " : "";
            //    string citySearch = model.City == null ? "" : String.Format("{0} C.City Like N'{1}%'", and, model.City);
            //    string vacancySearch = "";
            //    if (model.VacancyName != null)
            //    {
            //        var vacancyArray = model.VacancyName.Split(' ');
            //        foreach (var v in vacancyArray)
            //        {
            //            vacancySearch += String.Format(" And V.Name Like '%{0}%'", v);
            //        }
            //    }
            //    query = String.Format(@"Select Distinct C.Id, C.Name, C.Website, C.City,  C.Address, C.Email {0} From Companies C{1} {2} {3} {4}",
            //        selectList, vacancySync, nameSearch, citySearch, vacancySearch);
            //}
            //DBContext context = new DBContext();
            //context.DBDataTable = DatabaseController.GetDataTable(query);

            //return PartialView("~/Views/Search/_SearchResultPartial.cshtml", context);  
        }


    }
}