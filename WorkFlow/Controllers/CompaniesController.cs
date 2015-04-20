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

namespace WorkFlow.Controllers
{
    public class CompaniesController : Controller
    {

        // GET: Companies
        public ActionResult Index()
        {
            DBContext context  = new DBContext();
            context.Companies = DatabaseController.DoSQL<Companies>("SELECT * FROM Companies");
            ViewBag.Companies = context.Companies;
            ViewBag.Cities = DatabaseController.GetCities();
            return View(context);
        }
    }
}