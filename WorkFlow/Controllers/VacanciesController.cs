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
        public ActionResult EditVacancyInfo(int id)
        {
            Vacancies vacancy = DatabaseController.DoSQL<Vacancies>("Select * From Vacancies Where Id = " + id).LastOrDefault();
            return View("~/Views/Vacancies/EditVacancyInfo.cshtml", vacancy);
        }

        public ActionResult DeleteVacancy(int id)
        {
            DatabaseController.DoSQL<Vacancies>(@"delete from (select V.Id from Vacancies V join Requirements R
on (R.CompanyId = C.Id)) x where x.Id = " + id).LastOrDefault();
            
            return View("Index", "Home");
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

        //public FileResult Download(int id)
        //{
        //    Vacancies vacancy= DatabaseController.GetVacancyById(id);
        //    string fileName = Server.MapPath(@"~/App_Data/VacancyReport.rtf");
        //    string fileNameToDownload = Server.MapPath(@"~/App_Data/CompanyReportToDownload.rtf");
        //    if (System.IO.File.Exists(fileNameToDownload))
        //    {
        //        System.IO.File.Delete(fileNameToDownload);
        //    }
        //    System.IO.File.Copy(fileName, fileNameToDownload);
        //    InsertInfo(fileNameToDownload, company);
        //    return File(fileNameToDownload, "application/msword", company.Name + ".rtf");
        //}

        //public void InsertInfo(string source, Companies company)
        //{
        //    var fileContents = System.IO.File.ReadAllText(source);
        //    using (RichEditControl richEditControl = new RichEditControl())
        //    {
        //        richEditControl.LoadDocument(source, DocumentFormat.Rtf);
        //        string v = richEditControl.RtfText;
        //        v = v.Replace("address", String.Format("{0}, {1}", company.Address, company.City));
        //        v = v.Replace("name", company.Name);
        //        v = v.Replace("site", company.Website);
        //        v = v.Replace("email", company.Email);
        //        v = v.Replace("year", company.CreatingDate.Year.ToString());

        //        int index = v.IndexOf(lastString);
        //        List<Vacancies> vacancies = DatabaseController.DoSQL<Vacancies>(String.Format("Select Distinct V.* From Vacancies V, Companies C Where V.CompanyId = {0}", company.Id));

        //        if (vacancies.Count() != 0)
        //        {
        //            string temp = @"{\lang1033\langfe1033\b\i\fs28\cf2 Attention! }{\lang1033\langfe1033\fs28\cf3 We have next vacancies available:}";
        //            v = v.Insert(index, temp);
        //            index += temp.Length;
        //        }
        //        foreach (var vac in vacancies)
        //        {
        //            string temp = vacancyTemplate;
        //            temp = temp.Replace("vacName", vac.Name);
        //            temp = temp.Replace("description", vac.Description);
        //            temp = temp.Replace("amount", vac.Amount.ToString());
        //            temp = temp.Replace("date", String.Format("{0}/{1}/{2}", vac.OpenDate.Day, vac.OpenDate.Month, vac.OpenDate.Year));
        //            v = v.Insert(index, temp);
        //            index += temp.Length;
        //        }

        //        richEditControl.RtfText = v;
        //        richEditControl.SaveDocument(source, DocumentFormat.Rtf);
        //    }
        //}
    }
}
