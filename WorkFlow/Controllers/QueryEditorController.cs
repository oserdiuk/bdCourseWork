using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WorkFlow.Controllers
{
    public class QueryEditorController : Controller
    {
        // GET: QueryEditor
        public ActionResult Index()
        {
            return View(new WorkFlow.Models.QueryEditorInfo());
        }
    }
}