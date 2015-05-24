using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using WorkFlow.Models;
using WorkFlow.Models.DataBaseModels;
using System.IO;
using System.Drawing;
using DevExpress.XtraRichEdit;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace WorkFlow.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        private string PathDirectory = "~/App_Data/Images/";

        public ManageController()
        {
        }

        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Manage/Index
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.SetTwoFactorSuccess ? "Your two-factor authentication provider has been set."
                : message == ManageMessageId.Error ? "An error has occurred."
                : message == ManageMessageId.AddPhoneSuccess ? "Your phone number was added."
                : message == ManageMessageId.RemovePhoneSuccess ? "Your phone number was removed."
                : "";

            var userId = User.Identity.GetUserId();
            var model = new IndexViewModel
            {
                HasPassword = HasPassword(),
                PhoneNumber = await UserManager.GetPhoneNumberAsync(userId),
                TwoFactor = await UserManager.GetTwoFactorEnabledAsync(userId),
                Logins = await UserManager.GetLoginsAsync(userId),
                BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(userId)
            };
            return View(model);
        }

        public ActionResult EditInformationAbout(IndexViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var c = GetAuthenticatedCompany();
            return View(c); ;
        }

        public Companies GetAuthenticatedCompany()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            var c = DatabaseController.DoSQL<Companies>("Select * From Companies Where Email = '" + user.Email + "';").FirstOrDefault();
            return c;
        }

        public ActionResult Edit(Companies company)
        {
            DatabaseController.DoSQL<Companies>(String.Format(@"UPDATE Companies SET Name = N'{0}', Address = N'{1}', Website = N'{2}', City = N'{3}', Email = N'{4}', Phone = N'{5}', PropertyForm = N'{6}', CreatingDate = '{7}' WHERE Id =  N'{8}';", company.Name, company.Address, company.Website, company.City, company.Email, company.Phone, company.PropertyForm, company.CreatingDate.ToString(), company.Id));
            return RedirectToAction("EditInformationAbout", company);
        }

        [HttpPost]
        public ActionResult UploadLogo(HttpPostedFileBase file)
        {
            string path = "";
            Companies c = GetAuthenticatedCompany();
            string imageName = c.Id + ".";
            try
            {
                if (file.ContentLength > 0)
                {
                    var fileName = imageName + Path.GetFileName(file.FileName).Split('.').LastOrDefault();
                    path = Path.Combine(Server.MapPath(PathDirectory), fileName);
                    file.SaveAs(path);
                    DatabaseController.DoSQL<Companies>(String.Format(@"UPDATE Companies SET Logo = N'{0}' WHERE Id =  N'{1}';", fileName, c.Id));
                    c.Logo = fileName;
                }
            }
            catch (Exception ex)
            {

            }
            return RedirectToAction("Edit", c);
        }


        [HttpPost]
        public ActionResult DeleteLogo(int id)
        {
            Companies company = DatabaseController.GetCompanyById(id);
            string path = "";
            path = Path.Combine(Server.MapPath(PathDirectory), company.Logo);
            System.IO.File.Delete(path);
            DatabaseController.DoSQL<Companies>(String.Format(@"UPDATE Companies SET Logo = N'anon.jpg' WHERE Id =  N'{0}';", company.Id));
            company.Logo = "anon.jpg";
            return RedirectToAction("Edit", company);
        }





        //
        // POST: /Manage/RemoveLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
        {
            ManageMessageId? message;
            var result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("ManageLogins", new { Message = message });
        }

        //
        // GET: /Manage/AddPhoneNumber
        public ActionResult AddPhoneNumber()
        {
            return View();
        }

        //
        // POST: /Manage/AddPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // Generate the token and send it
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), model.Number);
            if (UserManager.SmsService != null)
            {
                var message = new IdentityMessage
                {
                    Destination = model.Number,
                    Body = "Your security code is: " + code
                };
                await UserManager.SmsService.SendAsync(message);
            }
            return RedirectToAction("VerifyPhoneNumber", new { PhoneNumber = model.Number });
        }

        //
        // POST: /Manage/EnableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EnableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), true);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // POST: /Manage/DisableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DisableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), false);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // GET: /Manage/VerifyPhoneNumber
        public async Task<ActionResult> VerifyPhoneNumber(string phoneNumber)
        {
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), phoneNumber);
            // Send an SMS through the SMS provider to verify the phone number
            return phoneNumber == null ? View("Error") : View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
        }

        //
        // POST: /Manage/VerifyPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePhoneNumberAsync(User.Identity.GetUserId(), model.PhoneNumber, model.Code);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.AddPhoneSuccess });
            }
            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "Failed to verify phone");
            return View(model);
        }

        //
        // GET: /Manage/RemovePhoneNumber
        public async Task<ActionResult> RemovePhoneNumber()
        {
            var result = await UserManager.SetPhoneNumberAsync(User.Identity.GetUserId(), null);
            if (!result.Succeeded)
            {
                return RedirectToAction("Index", new { Message = ManageMessageId.Error });
            }
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", new { Message = ManageMessageId.RemovePhoneSuccess });
        }

        //
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);
            return View(model);
        }

        //
        // GET: /Manage/SetPassword
        public ActionResult SetPassword()
        {
            return View();
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    }
                    return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Manage/ManageLogins
        public async Task<ActionResult> ManageLogins(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
            {
                return View("Error");
            }
            var userLogins = await UserManager.GetLoginsAsync(User.Identity.GetUserId());
            var otherLogins = AuthenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();
            ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;
            return View(new ManageLoginsViewModel
            {
                CurrentLogins = userLogins,
                OtherLogins = otherLogins
            });
        }

        //
        // POST: /Manage/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new AccountController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"), User.Identity.GetUserId());
        }

        //
        // GET: /Manage/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            return result.Succeeded ? RedirectToAction("ManageLogins") : RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }


        #region VacancyCreating
        string pathToVacancies = @"~/App_Data/";

        [HttpPost]
        public ActionResult CreateVacancy(Vacancies vacancy)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    vacancy.FileName = "";
                    AddVacancy(vacancy);
                }
                return RedirectToAction("Index", "Vacancies");
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        public PartialViewResult CreateRequirement(Requirements requirement)
        {
            DatabaseController.DoSQL<Requirements>(String.Format(@"INSERT INTO Requirements
                (VacancyId, SkillId, MinValue, MaxValue) values (0, N'{0}', N'{1}', N'{2}')", requirement.SkillId, requirement.MinValue, requirement.MaxValue));
            DBContext context = new DBContext();
            context.Requirements = DatabaseController.DoSQL<Requirements>("Select * From Requirements Where VacancyId = 0");
            return PartialView("~/Views/QueryEditor/_RequirementsListPartial.cshtml", context);
        }


        public void AddVacancy(Vacancies vacancy)
        {
            DatabaseController.DoSQL<Vacancies>(String.Format("INSERT INTO Vacancies (Name, OpenDate, Amount, Description, FileName, CompanyId) values (N'{0}', N'{1}', N'{2}', N'{3}', N'{4}', N'{5}')", vacancy.Name, vacancy.OpenDate, vacancy.Amount, vacancy.Description, vacancy.FileName, GetAuthenticatedCompany().Id));
        }

        [HttpPost]
        public ActionResult AddNewRequirement()
        {
            Requirements requirement = new Requirements();//test
            return PartialView("~/Views/Shared/EditorTemplates/Requirements.cshtml", requirement);
        }

        [HttpPost]
        public ActionResult UploadFileWithVacancies(HttpPostedFileBase file)
        {
            string path = "";
            try
            {
                if (file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    path = Path.Combine(Server.MapPath("~/App_Data/Vacancies/"), fileName);
                    file.SaveAs(path);
                    ParseFileToVacancy(path);
                }
                ViewBag.Message = "Upload successful";
                return RedirectToAction("Create", "Vacancies");
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Upload failed";
                return RedirectToAction("Create", "Vacancies");
            }
        }

        public void ParseFileToVacancy(string source)
        {
            string rtfCode = GetRTFCode(source);
            List<string> infoFromFile = new List<string>();
            Regex vacancyDividerExpression = new Regex(@"\[Everything that is between this header and the next one will be referred to one vacancy\](.*?)\[Everything that is between this header and the next one will be referred to one vacancy\]");
            MatchCollection matchesVac = vacancyDividerExpression.Matches(rtfCode);
            var vacanciesText = matchesVac.Cast<System.Text.RegularExpressions.Match>().Select(match => match.Value).ToList();

            foreach (var expr in vacanciesText)
            {
                string controlPhrase = @"[Everything that is between this header and the next one will be referred to one vacancy]";
                expr.Remove(expr.IndexOf(controlPhrase), controlPhrase.Length);
                expr.Remove(expr.LastIndexOf(controlPhrase), controlPhrase.Length);

                Regex searchExpression = new Regex(@"cf[0-9]* [a-zA-Z0-9.,:;!№*( )?%""'/|\+-=@]*");
                MatchCollection matches = searchExpression.Matches(expr);
                infoFromFile = matches.Cast<System.Text.RegularExpressions.Match>().Select(match => match.Value).ToList();

                Vacancies vacancy = new Vacancies();
                for (int i = 0; i < infoFromFile.Count() - 1; i++)
                {
                    var temp = infoFromFile[i + 1].Split(' ').ToList();
                    temp.RemoveAt(0);
                    var curValue = "";
                    foreach (var l in temp)
                    {
                        curValue += l + " ";
                    }

                    if (infoFromFile[i].Contains("Vacancy name:"))
                    {
                        vacancy.Name = curValue;
                    }
                    else if (infoFromFile[i].Contains("Description:"))
                    {
                        vacancy.Description = curValue;
                    }
                    else if (infoFromFile[i].Contains("Amount of position:"))
                    {
                        try
                        {
                            vacancy.Amount = Convert.ToInt32(curValue);
                        }
                        catch
                        {
                            return;
                        }
                    }
                    else if (infoFromFile[i].Contains("Open date:"))
                    {
                        try
                        {
                            var numbers = curValue.Split('.', '/');
                            DateTime MyDateTime = new DateTime(Convert.ToInt32(numbers[2]), Convert.ToInt32(numbers[1]), Convert.ToInt32(numbers[0]));
                            vacancy.OpenDate = MyDateTime;
                        }
                        catch
                        {
                            return;
                        }
                    }
                    else if (infoFromFile[i].Contains("Skills:"))
                    {
                        try
                        {
                            GetSkillsFromFile(vacanciesText);
                        }
                        catch (Exception ex)
                        {
                            return;
                        }
                    }
                }

                vacancy.FileName = Guid.NewGuid().ToString();

                //int id = (DatabaseController.DoSQL<Vacancies>(String.Format(@"Select V.Id From Vacancies V Where V.Name = N'{0}' Order By V.Id;", vacancy.Name))).LastOrDefault().Id;
                using (RichEditControl rtc = new RichEditControl())
                {
                    rtc.RtfText = expr;
                    rtc.SaveDocument(Server.MapPath(pathToVacancies + "//" + vacancy.FileName + ".rtf"), DocumentFormat.Rtf);
                }

            }
        }

        private void GetSkillsFromFile(List<string> vacanciesText)
        {
            string findingCategory = @"lang1033\\langfe1033\\b\\i\\fs22\\cf3";
            string findingSkills = @"lang1033\\langfe1033\\b\\i\\fs22\\cf3\\cell\\pard\\plain\\ql\\intbl\\yts8{\\lang1033\\langfe1033\\i\\fs22\\cf3 ";

            foreach (var expr in vacanciesText)
            {
                Regex vacancyDividerExpression = new Regex(String.Format(@"{0}(.*?){0}", findingCategory));

                List<string> infoFromFile = new List<string>();

                Regex searchExpression = new Regex(String.Format(@"{0}[a-zA-Z0-9.,:;!№*( )?%""'//|\+-=@]*}", findingSkills));
                MatchCollection matches = searchExpression.Matches(expr);
                infoFromFile = matches.Cast<System.Text.RegularExpressions.Match>().Select(match => match.Value).ToList();

                for (int i = 0; i < infoFromFile.Count() - 1; i++)
                {
                    var temp = infoFromFile[i + 1].Split(' ').ToList();
                    temp.RemoveAt(0);

                    if (infoFromFile[i].Contains("++ Windows") || infoFromFile[i].Contains("Objective-C") || infoFromFile[i].Contains(".NET Technologies") ||
                        infoFromFile[i].Contains("Objective-C") || infoFromFile[i].Contains(".NET Technologies") || infoFromFile[i].Contains("C# ORM") ||
                        infoFromFile[i].Contains("C# 3rd Party Libraries") || infoFromFile[i].Contains("Java JavaSE") || infoFromFile[i].Contains("Java Tools") ||infoFromFile[i].Contains("PHP Frameworks") || infoFromFile[i].Contains("Ruby Frameworks ") || 
                        infoFromFile[i].Contains("Python Frameworks & Tools") || infoFromFile[i].Contains("JavaScript") ||
                        infoFromFile[i].Contains("Graphic packages") || infoFromFile[i].Contains("Markup languages") || infoFromFile[i].Contains("Tools") ||
                        infoFromFile[i].Contains("Web-servers") || infoFromFile[i].Contains("Networking Protocols") || infoFromFile[i].Contains("Databases") ||
                        infoFromFile[i].Contains("Version control systems") || infoFromFile[i].Contains("Defect management systems") || 
                        infoFromFile[i].Contains("Automated testing tools") || infoFromFile[i].Contains("Virtualization") || 
                        infoFromFile[i].Contains("Analysis, design, project management") || infoFromFile[i].Contains("Personal characteristics") ||
                        infoFromFile[i].Contains("Foreign languages"))
                    {
                        Regex searchSkillExpression = new Regex(String.Format(@"{0}[a-zA-Z0-9.,:;!№*( )?%""'/|\+-=@]*}", findingSkills));
                        MatchCollection matchesSkill = searchSkillExpression.Matches(expr);
                        infoFromFile = matchesSkill.Cast<System.Text.RegularExpressions.Match>().Select(match => match.Value).ToList();
                    }

                }
            }
        }


        public string GetHtmlTextFromRTF(string source)
        {
            string html = "";
            using (RichEditControl richEditControl = new RichEditControl())
            {

                richEditControl.LoadDocument(source, DocumentFormat.Rtf);
                html = richEditControl.HtmlText;
            }

            return html;
        }

        public string GetRTFCode(string source)
        {
            var fileContents = System.IO.File.ReadAllText(source);
            string rtf = "";
            using (RichEditControl richEditControl = new RichEditControl())
            {

                richEditControl.LoadDocument(source, DocumentFormat.Rtf);
                rtf = richEditControl.RtfText;
            }

            return rtf;
        }

        #endregion


        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private bool HasPhoneNumber()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PhoneNumber != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

        #endregion
    }
}