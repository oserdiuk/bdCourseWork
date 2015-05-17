using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using WorkFlow.Models.DataBaseModels;
using WorkFlow.Models;
using System.Globalization;

namespace WorkFlow.Controllers
{
    public class VacanciesController : Controller
    {
        string pathToVacancies = @"~/App_Data/";

        // GET: Vacancies
        public ActionResult Index()
        {
            DBContext context = new DBContext();
            context.DBDataTable = DatabaseController.GetDataTable("SELECT Id, CompanyId, Name, OpenDate as 'Open date', Amount FROM Vacancies");
            context.Skills = DatabaseController.DoSQL<Skills>("Select * From Skills");
            ViewBag.Skills = context.Skills;
            return View(context);
        }

        // GET: Vacancies/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Vacancies/Create
        public ActionResult Create()
        {
            DBContext context = new DBContext();
            context.Skills = DatabaseController.DoSQL<Skills>("Select * From Skills");
            ViewBag.Skills = context.Skills;
            return View();
        }

        // POST: Vacancies/Create
        //[HttpPost]
        //public ActionResult Create(Vacancies vacancy, string fileName = "")
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            Companies companyModel = new ManageController().GetAuthenticatedCompany();
        //            DatabaseController.DoSQL<Vacancies>(String.Format("INSERT INTO Vacancies (Name, OpenDate, Amount, Description, FileName, CompanyId) values (N'{0}', N'{1}', N'{2}', N'{3}', N'{4}', N'{5}')", vacancy.Name, vacancy.OpenDate, vacancy.Amount, vacancy.Description, fileName, companyModel.Id));
        //        }
        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        // GET: Vacancies/Edit/5
        public ActionResult Edit(int id)
        {
            ViewBag.Skills = DatabaseController.DoSQL<Skills>("Select * From Skills;");
            return View();
        }

        // POST: Vacancies/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Vacancies/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Vacancies/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult ShowVacanciesOf(string email)
        {

            Companies company = DatabaseController.DoSQL<Companies>(String.Format("Select * From Companies Where Email = '{0}'", email)).FirstOrDefault();
            return View("~/Views/CompanyInfo/Index.cshtml", company);
        }

        public FileResult DownloadTemplate()
        {
            string fileName = "template.rtf";
            return File(pathToVacancies + fileName, "application/msword", fileName);
        }

      
    }
}
