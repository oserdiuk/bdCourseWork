using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkFlow.Models.DataBaseModels;
using Dapper;
using System.Web.Routing;
using System.Text.RegularExpressions;
using WorkFlow.Models;
using System.Data;
using System.Diagnostics;


namespace WorkFlow.Controllers
{
    #region enums
    public enum ActionWithTable
    {
        SortDesc = 1, Sort = 2
    }

    public enum Statistics
    {
        CompanyInCities, SkillsForVacancy, VacanciesInRegion
    }
    #endregion


    public class DatabaseController : Controller
    {

        // GET: Database
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ShowImage(int id)
        {
            string pathToImage = GetCompanyById(id).Logo;
            string path = String.Format(@"~/App_Data/Images/{0}", pathToImage);
            return File(path, "image/jpg");
        }

        public static Companies GetCompanyById(int id)
        {
            return DatabaseController.DoSQL<Companies>("Select * From Companies Where Id = '" + id + "';").FirstOrDefault();
        }

        public static Vacancies GetVacancyById(int id)
        {
            return DatabaseController.DoSQL<Vacancies>("Select * From Vacancies Where Id = '" + id + "';").FirstOrDefault();
        }

        public static Requirements GetRequirementById(int id)
        {
            return DatabaseController.DoSQL<Requirements>("Select * From Requirements Where Id = '" + id + "';").FirstOrDefault();
        }

        public static Categories GetCategoryById(int id)
        {
            return DatabaseController.DoSQL<Categories>("Select * From Categories Where Id = '" + id + "';").FirstOrDefault();
        }

        public static Skills GetSkillById(int id)
        {
            return DatabaseController.DoSQL<Skills>("Select * From Skills Where Id = '" + id + "';").FirstOrDefault();
        }

        public static List<T> DoSQL<T>(string query)
        {
            List<T> result;
            string connectionString = ConfigurationManager.ConnectionStrings["DatabaseModel1"].ConnectionString;
            using (SqlConnection sqlconn = new SqlConnection(connectionString))
            {
                sqlconn.Open();
                result = sqlconn.Query<T>(query).ToList<T>();
                sqlconn.Close();
            }
            return result;
        }


        public PartialViewResult RunQuery(QueryEditorInfo model)
        {
            model.DateTable = GetDataTable(model.Query);
            ViewBag.FilterCompanySucceed = true;
            return PartialView("~/Views/QueryEditor/_QueryListPartial.cshtml", model);
        }

        public static DataTable GetDataTable(string query)
        {
            DataTable dt = new DataTable();
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["DatabaseModel1"].ConnectionString;
                SqlDataAdapter adp = new SqlDataAdapter(query, connectionString);
                adp.Fill(dt);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
            return dt;
        }

        public PartialViewResult UpdateTable(DBContext context, ActionWithTable actionNumber, string tableName, string fieldToSort = null)
        {
            //List<Companies> updatedCompanies = null;
            //List<Vacancies> updatedVacancies = null;
            switch (actionNumber)
            {
                case ActionWithTable.Sort:
                case ActionWithTable.SortDesc:
                    //if (tableName == "Companies")
                    //{
                    //    updatedCompanies = this.Sort<Companies>(tableName, fieldToSort, actionNumber == ActionWithTable.SortDesc, true);
                    //}
                    //else
                    //{
                    //    updatedVacancies = this.Sort<Vacancies>(tableName, fieldToSort, actionNumber == ActionWithTable.SortDesc, true);
                    //}
                    context = new DBContext();
                    string sortOrder = actionNumber == ActionWithTable.SortDesc ? "DESC" : "ASC";
                    if (tableName == "Companies")
                    {
                        context.DBDataTable = GetDataTable(String.Format("SELECT Id, Name, City, Email, Address, Phone, Website, PropertyForm, CreatingDate FROM {0} ORDER BY {1} {2}", tableName, fieldToSort, sortOrder));
                        ViewBag.FilterCompanySucceed = true;
                        return PartialView("~/Views/Companies/_CompaniesListPartial.cshtml", context);
                    }
                    else
                    {
                        context.DBDataTable = GetDataTable(String.Format("SELECT Id, CompanyId, Name, OpenDate as 'Open date', Amount FROM {0} ORDER BY {1} {2}", tableName, fieldToSort, sortOrder));
                        return PartialView("~/Views/Vacancies/_VacanciesListPartial.cshtml", context);
                    }
                default: break;
            }
            //context = new DBContext();
            //context.Companies = updatedCompanies;
            //context.Vacancies = updatedVacancies;
            return PartialView("~/Views/Companies/_CompaniesListPartial.cshtml", context);

        }

        public List<T> Sort<T>(string tableName, string field, bool descSort, bool isCompany)
        {
            string sortOrder = descSort ? "DESC" : "ASC";
            tableName = tableName == "Companies" ? "Id, Name, City, Email, Address, Phone, Website, PropertyForm, CreatingDate From " + tableName : tableName;
            return DoSQL<T>(String.Format("SELECT {0} ORDER BY {1} {2}", tableName, field, sortOrder));
        }

        public static List<string> GetCities()
        {
            return DoSQL<string>("Select Distinct City From Companies");
        }

        public PartialViewResult FilterCompany(CompanyFilter model)
        {
            string query = "";
            DBContext context = new DBContext();

            if (model.City == null && model.NumberOfVacancyMax == null && model.NumberOfVacancyMin == null)
            {
                query = "Select * From Companies;";
            }

            else
            {
                string cities = "C.City in (";
                string and = "";
                string where = "";
                string having = "";
                string initializeColumn = "";
                string checkForId = "";

                if (model.NumberOfVacancyMax != null || model.NumberOfVacancyMin != null)
                {
                    having = "Having";
                    and = model.NumberOfVacancyMax != null && model.NumberOfVacancyMin != null ? "And" : "";
                    initializeColumn = @", Count(V.CompanyId) as 'Number of vacancies'";
                    checkForId = @", Vacancies V Where C.Id = V.CompanyId";
                    where = "And ";
                }
                else
                {
                    where = " Where ";
                }
                if (model.City != null)
                {
                    if (model.City.Count() == 1)
                    {
                        cities = "C.City = N'" + model.City[0] + "'";
                    }
                    else
                    {
                        foreach (var c in model.City)
                        {
                            cities += model.City.Count() != (model.City.IndexOf(c) + 1) ? "N'" + c + "'" + ", " : "N'" + c + "')";
                        }
                    }
                }
                else
                {
                    cities = "";
                }

                string vacNumberForQueryMin = model.NumberOfVacancyMin != null ? " Count(V.CompanyId) >= " + model.NumberOfVacancyMin : "";
                string vacNumberForQueryMax = model.NumberOfVacancyMax != null ? " Count(V.CompanyId) <= " + model.NumberOfVacancyMax : "";

                query = String.Format(@"Select C.Id, C.Name, C.Website, C.City, C.Address, C.Email{0} From Companies C{1} {2} {3} Group by C.Id, C.Name, C.Website, C.City, C.Address, C.Email {4} {5} {6} {7};", initializeColumn, checkForId, where, cities, having, vacNumberForQueryMin, and, vacNumberForQueryMax);
            }

            context.DBDataTable = GetDataTable(query);
            ViewBag.FilterCompanySucceed = true;
            return PartialView("~/Views/Companies/_CompaniesListPartial.cshtml", context);
        }

        public PartialViewResult FilterVacancy(VacancyFilter model)
        {

            string query = "";
            DBContext context = new DBContext();
            DateTime dateDefault = new DateTime(0001, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            if (model.MaxOpenDate == dateDefault & model.MinOpenDate == dateDefault & model.RequiredSkills.Count() == 0)
            {
                query = @"SELECT Distinct V.Id, V.CompanyId, V.Name, V.OpenDate FROM Vacancies V ";
            }
            else
            {
                string skills = "X.Name IN (";

                if (model.RequiredSkills.Count() != 0)
                {
                    if (model.RequiredSkills.Count() == 1)
                    {
                        skills = "X.Name = N'" + model.RequiredSkills[0] + "'";
                    }
                    else
                    {
                        foreach (var c in model.RequiredSkills)
                        {
                            skills += model.RequiredSkills.Count() != (model.RequiredSkills.IndexOf(c) + 1) ? "N'" + c + "'" + ", " : "N'" + c + "')";
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

                if (model.MinOpenDate != dateDefault) 
                {
                    query = DatabaseController.AppendANDStatementToQuery(query, "V.OpenDate >= '" + model.MinOpenDate + "'");
                }

                if (model.MaxOpenDate != dateDefault)
                {
                    query = DatabaseController.AppendANDStatementToQuery(query, "V.OpenDate <= '" + model.MaxOpenDate + "'");
                }
            }

            context.DBDataTable = GetDataTable(query);
            return PartialView("~/Views/Vacancies/_VacanciesListPartial.cshtml", context);
        }

        public static string AppendANDStatementToQuery(string query, string statement)
        {
            int whereStatementIndex = query.ToLower().LastIndexOf("where");
            if (whereStatementIndex != -1)
            {
                string lastQueryPathComponent = query.Substring(whereStatementIndex).Trim();
                string andStatementIfNeeded = lastQueryPathComponent.ToLower().Equals("where") ? " " : " AND ";
                query += String.Format("{0}{1}", andStatementIfNeeded, statement);
            }

            return query;
        }
    }
}