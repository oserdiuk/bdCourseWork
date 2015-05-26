using DevExpress.XtraRichEdit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkFlow.Models.DataBaseModels;

namespace WorkFlow.Controllers
{
    public class CompanyInfoController : Controller
    {
        // GET: CompanyInfo
        public ActionResult Index(int id)
        {
            return View(DatabaseController.GetCompanyById(id));
        }

        public FileResult Download(int id)
        {
            Companies company = DatabaseController.GetCompanyById(id);
            string fileName = Server.MapPath(@"~/App_Data/CompanyReport.rtf");
            string fileNameToDownload = Server.MapPath(@"~/App_Data/CompanyReportToDownload.rtf");
            if (System.IO.File.Exists(fileNameToDownload))
            {
                System.IO.File.Delete(fileNameToDownload);
            }
            System.IO.File.Copy(fileName, fileNameToDownload);
            InsertInfo(fileNameToDownload, company);
            return File(fileNameToDownload, "application/msword", company.Name + ".rtf");
        }

        public void InsertInfo(string source, Companies company)
        {
            var fileContents = System.IO.File.ReadAllText(source);
            using (RichEditControl richEditControl = new RichEditControl())
            {
                richEditControl.LoadDocument(source, DocumentFormat.Rtf);
                string v = richEditControl.RtfText;
                v = v.Replace("address", String.Format("{0}, {1}", company.Address, company.City));
                v = v.Replace("name", company.Name);
                v = v.Replace("site", company.Website);
                v = v.Replace("email", company.Email);
                v = v.Replace("year", company.CreatingDate.Year.ToString());

                int index = v.IndexOf(lastString);
                List<Vacancies> vacancies = DatabaseController.DoSQL<Vacancies>(String.Format("Select Distinct V.* From Vacancies V, Companies C Where V.CompanyId = {0}", company.Id));

                if (vacancies.Count() != 0)
                {
                    string temp = @"{\lang1033\langfe1033\b\i\fs28\cf2 Attention! }{\lang1033\langfe1033\fs28\cf3 We have next vacancies available:}";
                    v = v.Insert(index, temp);
                    index += temp.Length;
                }
                foreach (var vac in vacancies)
                {
                    string temp = vacancyTemplate;
                    temp = temp.Replace("vacName", vac.Name);
                    temp = temp.Replace("description", vac.Description);
                    temp = temp.Replace("amount", vac.Amount.ToString());
                    temp = temp.Replace("date", String.Format("{0}/{1}/{2}", vac.OpenDate.Day, vac.OpenDate.Month, vac.OpenDate.Year));
                    v = v.Insert(index, temp);
                    index += temp.Length;
                }

                richEditControl.RtfText = v;
                richEditControl.SaveDocument(source, DocumentFormat.Rtf);
            }
        }

        private string vacancyTemplate = @"\lang1033\langfe1033\fs28\cf3\par\pard\plain\ql\sl258\slmult1\sa160\lang1033\langfe1033\fs12\cf3\par\pard\plain\ql\sl258\slmult1\sa160{\lang1033\langfe1033\b\ul\fs28\cf2 vacName}{\lang1049\langfe1049\b\ul\fs28\cf2 .}{\lang1049\langfe1049\fs28\cf3  }{\lang1033\langfe1033\fs28\cf3 description}\lang1049\langfe1049\fs28\cf3\par\pard\plain\ql\sl258\slmult1\sa160{\lang1033\langfe1033\fs28\cf3 Vacancy is opened from date. We are waiting for amount specialists in this sphere.}\lang1033\langfe1033\fs28\cf3\par\pard\plain\ql\sl258\slmult1\sa160\lang1033\langfe1033\fs28\cf3\par\pard\plain\qc\sl258\slmult1\sa160";
        private string lastString = @"{\lang1033\langfe1033\i\fs28\cf2 Welcome! We would be glad to see you with us!}\lang1033\langfe1033\i\fs28\cf2\par\pard\plain\ql\sl258\slmult1\sa160\lang1033\langfe1033\b\ul\fs28\cf3\par}";

        public ActionResult VacancyInfo(int id)
        {
            var v = DatabaseController.GetVacancyById(id);
            return View(v);
        }
    }
}
