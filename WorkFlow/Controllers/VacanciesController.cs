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
            vacancy.Requirements = WorkFlow.Controllers.DatabaseController.DoSQL<WorkFlow.Models.DataBaseModels.Requirements>("Select * From Requirements Where VacancyId = " + vacancy.Id);
            foreach (var r in vacancy.Requirements)
            {
                r.Skills = DatabaseController.DoSQL<Skills>(String.Format(@"Select * From Skills Where Id = {0}", r.SkillId)).FirstOrDefault();
            }
            return View("~/Views/Vacancies/EditVacancyInfo.cshtml", vacancy);
        }

        public ActionResult DeleteVacancy(int id)
        {
            DatabaseController.DoSQL<Vacancies>(@"delete from Vacancies where Vacancies.Id = " + id).LastOrDefault();

            return RedirectToAction("Index", "Home");
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

        public FileResult Download(int id)
        {
            Vacancies vacancy = DatabaseController.GetVacancyById(id);
            string fileName = Server.MapPath(@"~/App_Data/VacancyReport.rtf");
            string fileNameToDownload = Server.MapPath(@"~/App_Data/VacancyReportToDownload.rtf");
            if (System.IO.File.Exists(fileNameToDownload))
            {
                System.IO.File.Delete(fileNameToDownload);
            }
            System.IO.File.Copy(fileName, fileNameToDownload);
            InsertInfo(fileNameToDownload, vacancy);
            return File(fileNameToDownload, "application/msword", vacancy.Name + ".rtf");
        }

        public void InsertInfo(string source, Vacancies vacancy)
        {
            var fileContents = System.IO.File.ReadAllText(source);
            Companies company = DatabaseController.DoSQL<Companies>(String.Format("Select C.* From Companies C, Vacancies V Where C.Id = {0}", vacancy.CompanyId)).LastOrDefault();
            using (RichEditControl richEditControl = new RichEditControl())
            {
                richEditControl.LoadDocument(source, DocumentFormat.Rtf);
                string v = richEditControl.RtfText;
                v = v.Replace("#address", String.Format("{0}, {1}", company.Address, company.City));
                v = v.Replace("#name", company.Name);
                v = v.Replace("#site", company.Website);
                v = v.Replace("#email", company.Email);
                v = v.Replace("year", company.CreatingDate.Year.ToString());
                v = v.Replace("#vacname", vacancy.Name);
                v = v.Replace("#description", vacancy.Description);
                v = v.Replace("#amount", vacancy.Amount.ToString());
                v = v.Replace("#date", String.Format("{0}/{1}/{2}", vacancy.OpenDate.Day, vacancy.OpenDate.Month, vacancy.OpenDate.Year));
                   
                int index = v.IndexOf(welcomeString);
                List<Requirements> requirements = DatabaseController.DoSQL<Requirements>(String.Format("Select Distinct R.* From Vacancies V, Requirements R Where R.VacancyId = {0}", vacancy.Id));

                if (requirements.Count() != 0)
                {
                    requirements[0].Skills = DatabaseController.DoSQL<Skills>(String.Format("Select Distinct S.* From Skills S Where S.Id = {0}", requirements[0].SkillId)).LastOrDefault();
                    stringWithRequirements = stringWithRequirements.Replace("#name", requirements[0].Skills.Name);
                    stringWithRequirements = requirements[0].MinValue != null ? stringWithRequirements.Replace("#minpoint", requirements[0].MinValue.ToString()) : stringWithRequirements.Replace("#minpoint", " ");
                    stringWithRequirements = requirements[0].MaxValue != null ? stringWithRequirements.Replace("#maxpoint", requirements[0].MaxValue.ToString()) : stringWithRequirements.Replace("#maxpoint", " ");
                    stringWithRequirements = stringWithRequirements.Replace("#range", requirements[0].Skills.UnitMeasure);
                    v = v.Insert(index, stringWithRequirements);
                    index = v.IndexOf(lastString);
                }

                for (int i = 1; i < requirements.Count(); i++ )
                {
                    string temp = requirementTemplate;
                    requirements[i].Skills = DatabaseController.DoSQL<Skills>(String.Format("Select Distinct S.* From Skills S Where S.Id = {0}", requirements[i].SkillId)).LastOrDefault();
                    temp = temp.Replace("#name", requirements[i].Skills.Name);
                    temp = requirements[i].MinValue != null ? temp.Replace("#minpoint", requirements[i].MinValue.ToString()) : temp.Replace("#minpoint", " ");
                    temp = requirements[i].MaxValue != null ? temp.Replace("#maxpoint", requirements[i].MaxValue.ToString()) : temp.Replace("#maxpoint", " ");
                    temp = temp.Replace("#range", requirements[i].Skills.UnitMeasure);
                    v = v.Insert(index, temp);
                    index += temp.Length;
                }

                richEditControl.RtfText = v;
                richEditControl.SaveDocument(source, DocumentFormat.Rtf);
            }
        }

        private string welcomeString = @"{\lang1033\langfe1033\i\fs28\cf2                    Welcome to us! We will be glad to see you here!}\lang1033\langfe1033\i\fs28\cf2\par\pard\plain\ql\sl258\slmult1\sa160\lang1033\langfe1033\b\ul\fs28\cf3\par}";
        private string requirementTemplate = @"\lang1033\langfe1033\fs28\cf3\cell\trowd\irow1\irowband0\ts6\trgaph108\tphmrg\tposx1\tpvpara\tposy122\tdfrmtxtLeft180\tdfrmtxtRight180\trleft-108\trrh536\trftsWidth1\trftsWidthB3\trftsWidthA3\trautofit1\trpaddfl3\trpaddl108\trpaddfr3\trpaddr108\tbllkhdrcols\tbllkhdrrows\tbllknocolband\tblindtype3\tblind0\clvertalt\clbrdrt\brdrnone\clbrdrl\brdrnone\clbrdrb\brdrnone\clbrdrr\brdrnone\cltxlrtb\clftsWidth3\clwWidth4837\clpadfr3\clpadr108\clpadft3\clpadt108\cellx4755\clvertalt\clbrdrt\brdrnone\clbrdrl\brdrnone\clbrdrb\brdrnone\clbrdrr\brdrnone\cltxlrtb\clftsWidth3\clwWidth1391\clpadfr3\clpadr108\clpadft3\clpadt108\cellx6165\clvertalt\clbrdrt\brdrnone\clbrdrl\brdrnone\clbrdrb\brdrnone\clbrdrr\brdrnone\cltxlrtb\clftsWidth3\clwWidth1440\clpadfr3\clpadr108\clpadft3\clpadt108\cellx7620\clvertalt\clbrdrt\brdrnone\clbrdrl\brdrnone\clbrdrb\brdrnone\clbrdrr\brdrnone\cltxlrtb\clftsWidth3\clwWidth2187\clpadfr3\clpadr108\clpadft3\clpadt108\cellx9810\row\pard\plain\pvpara\phmrg\posx0\posy122\absh0\absw0\ql\intbl\yts6{\lang1033\langfe1033\fs28\cf3 #name}\lang1033\langfe1033\fs28\cf3\cell\pard\plain\pvpara\phmrg\posx0\posy122\absh0\absw0\ql\intbl\yts6{\lang1033\langfe1033\fs28\cf3 #minpoint}\lang1033\langfe1033\fs28\cf3\cell\pard\plain\pvpara\phmrg\posx0\posy122\absh0\absw0\ql\intbl\yts6{\lang1033\langfe1033\fs28\cf3 #maxpoint}\lang1033\langfe1033\fs28\cf3\cell\pard\plain\pvpara\phmrg\posx0\posy122\absh0\absw0\ql\intbl\yts6{\lang1033\langfe1033\fs28\cf3 #range}";
        private string lastString = @"\lang1033\langfe1033\fs28\cf3\cell\trowd\irow2\irowband1\lastrow\ts6\trgaph108\tphmrg\tposx1\tpvpara\tposy122\tdfrmtxtLeft180\tdfrmtxtRight180\trleft-108\trrh536\trftsWidth1\trftsWidthB3\trftsWidthA3\trautofit1\trpaddfl3\trpaddl108\trpaddfr3\trpaddr10";
        private string stringWithRequirements = @"{\lang1033\langfe1033\b\i\fs28\cf2 Requirements}{\lang1033\langfe1033\i\fs28\cf3 :}\lang1033\langfe1033\i\fs28\cf3\par\trowd\irow0\irowband-1\ts6\trgaph108\tphmrg\tposx1\tpvpara\tposy122\tdfrmtxtLeft180\tdfrmtxtRight180\trleft-108\trrh491\trftsWidth1\trftsWidthB3\trftsWidthA3\trautofit1\trpaddfl3\trpaddl108\trpaddfr3\trpaddr108\tbllkhdrcols\tbllkhdrrows\tbllknocolband\tblindtype3\tblind0\clvertalt\clbrdrt\brdrnone\clbrdrl\brdrnone\clbrdrb\brdrnone\clbrdrr\brdrnone\cltxlrtb\clftsWidth3\clwWidth4837\clpadfr3\clpadr108\clpadft3\clpadt108\cellx4755\clvertalt\clbrdrt\brdrnone\clbrdrl\brdrnone\clbrdrb\brdrnone\clbrdrr\brdrnone\cltxlrtb\clftsWidth3\clwWidth1391\clpadfr3\clpadr108\clpadft3\clpadt108\cellx6165\clvertalt\clbrdrt\brdrnone\clbrdrl\brdrnone\clbrdrb\brdrnone\clbrdrr\brdrnone\cltxlrtb\clftsWidth3\clwWidth1440\clpadfr3\clpadr108\clpadft3\clpadt108\cellx7620\clvertalt\clbrdrt\brdrnone\clbrdrl\brdrnone\clbrdrb\brdrnone\clbrdrr\brdrnone\cltxlrtb\clftsWidth3\clwWidth2187\clpadfr3\clpadr108\clpadft3\clpadt108\cellx9810\pard\plain\pvpara\phmrg\posx0\posy122\absh0\absw0\ql\intbl\fi990\yts6{\langnp1058\langfenp1058\noproof\fs28\cf2 Name}\lang1049\langfe1049\fs28\cf2\cell\pard\plain\pvpara\phmrg\posx0\posy122\absh0\absw0\qc\intbl\yts6{\langnp1058\langfenp1058\noproof\fs28\cf2 Min point}\langnp1058\langfenp1058\noproof\fs28\cf2\cell\pard\plain\pvpara\phmrg\posx0\posy122\absh0\absw0\qc\intbl\yts6{\langnp1058\langfenp1058\noproof\fs28\cf2 Max point}\langnp1058\langfenp1058\noproof\fs28\cf2\cell\pard\plain\pvpara\phmrg\posx0\posy122\absh0\absw0\qc\intbl\fi-378\yts6{\langnp1058\langfenp1058\noproof\fs28\cf2 Range}\lang1033\langfe1033\fs28\cf3\cell\trowd\irow0\irowband-1\ts6\trgaph108\tphmrg\tposx1\tpvpara\tposy122\tdfrmtxtLeft180\tdfrmtxtRight180\trleft-108\trrh491\trftsWidth1\trftsWidthB3\trftsWidthA3\trautofit1\trpaddfl3\trpaddl108\trpaddfr3\trpaddr108\tbllkhdrcols\tbllkhdrrows\tbllknocolband\tblindtype3\tblind0\clvertalt\clbrdrt\brdrnone\clbrdrl\brdrnone\clbrdrb\brdrnone\clbrdrr\brdrnone\cltxlrtb\clftsWidth3\clwWidth4837\clpadfr3\clpadr108\clpadft3\clpadt108\cellx4755\clvertalt\clbrdrt\brdrnone\clbrdrl\brdrnone\clbrdrb\brdrnone\clbrdrr\brdrnone\cltxlrtb\clftsWidth3\clwWidth1391\clpadfr3\clpadr108\clpadft3\clpadt108\cellx6165\clvertalt\clbrdrt\brdrnone\clbrdrl\brdrnone\clbrdrb\brdrnone\clbrdrr\brdrnone\cltxlrtb\clftsWidth3\clwWidth1440\clpadfr3\clpadr108\clpadft3\clpadt108\cellx7620\clvertalt\clbrdrt\brdrnone\clbrdrl\brdrnone\clbrdrb\brdrnone\clbrdrr\brdrnone\cltxlrtb\clftsWidth3\clwWidth2187\clpadfr3\clpadr108\clpadft3\clpadt108\cellx9810\row\pard\plain\pvpara\phmrg\posx0\posy122\absh0\absw0\ql\intbl\yts6{\lang1033\langfe1033\fs28\cf3 #name}\lang1033\langfe1033\fs28\cf3\cell\pard\plain\pvpara\phmrg\posx0\posy122\absh0\absw0\ql\intbl\yts6{\lang1033\langfe1033\fs28\cf3 #minpoint}\lang1033\langfe1033\fs28\cf3\cell\pard\plain\pvpara\phmrg\posx0\posy122\absh0\absw0\ql\intbl\yts6{\lang1033\langfe1033\fs28\cf3 #maxpoint}\lang1033\langfe1033\fs28\cf3\cell\pard\plain\pvpara\phmrg\posx0\posy122\absh0\absw0\ql\intbl\yts6{\lang1033\langfe1033\fs28\cf3 #range}\lang1033\langfe1033\fs28\cf3\cell\trowd\irow2\irowband1\lastrow\ts6\trgaph108\tphmrg\tposx1\tpvpara\tposy122\tdfrmtxtLeft180\tdfrmtxtRight180\trleft-108\trrh536\trftsWidth1\trftsWidthB3\trftsWidthA3\trautofit1\trpaddfl3\trpaddl108\trpaddfr3\trpaddr108\tbllkhdrcols\tbllkhdrrows\tbllknocolband\tblindtype3\tblind0\clvertalt\clbrdrt\brdrnone\clbrdrl\brdrnone\clbrdrb\brdrnone\clbrdrr\brdrnone\cltxlrtb\clftsWidth3\clwWidth4837\clpadfr3\clpadr108\clpadft3\clpadt108\cellx4755\clvertalt\clbrdrt\brdrnone\clbrdrl\brdrnone\clbrdrb\brdrnone\clbrdrr\brdrnone\cltxlrtb\clftsWidth3\clwWidth1391\clpadfr3\clpadr108\clpadft3\clpadt108\cellx6165\clvertalt\clbrdrt\brdrnone\clbrdrl\brdrnone\clbrdrb\brdrnone\clbrdrr\brdrnone\cltxlrtb\clftsWidth3\clwWidth1440\clpadfr3\clpadr108\clpadft3\clpadt108\cellx7620\clvertalt\clbrdrt\brdrnone\clbrdrl\brdrnone\clbrdrb\brdrnone\clbrdrr\brdrnone\cltxlrtb\clftsWidth3\clwWidth2187\clpadfr3\clpadr108\clpadft3\clpadt108\cellx9810\row\pard\plain\ql\sl258\slmult1\sa160\lang1033\langfe1033\i\fs28\cf3\par\pard\plain\ql\sl258\slmult1\sa160\lang1033\langfe1033\fs28\cf3\par\pard\plain\qc\sl258\slmult1\sa160";
    }
}
