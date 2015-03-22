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

        DBContext context = new DBContext();
        DataSet ds = new DataSet();

        string connString = System.Configuration.ConfigurationManager.ConnectionStrings["DatabaseModel1"].ConnectionString;

        // GET: Companies
        public ActionResult Index()
        {
            try
            {
                SqlConnection sqlconn = new SqlConnection(connString);
                sqlconn.Open();
                context.Companies = sqlconn.Query<Companies>("SELECT * FROM Companies").ToList();
                //SqlDataAdapter oda = new SqlDataAdapter("SELECT * FROM Companies", sqlconn);
                //oda.Fill(ds);

                sqlconn.Close();
            }
            catch (Exception ex)
            {
                //System.Web.HttpContext.Current.Response.Write("<SCRIPT LANGUAGE=""JavaScript"">alert("Hello this is an Alert")</SCRIPT>")
            }

            return View(context);
        }
    }
}