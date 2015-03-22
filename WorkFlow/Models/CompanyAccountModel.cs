using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using WorkFlow.Models.DataBaseModels;
using Dapper;
using System.Configuration;


namespace WorkFlow.Models
{
    public enum SignUpResult
    {
        Success, WrongPassword, ExistedEmail, ExistedName, SignInError
    }

    public class CompanyAccountModel
    {
        public static SignUpResult RegisterCompany(RegisterCompanyModel model)
        {
            string connString = ConfigurationManager.ConnectionStrings["DatabaseModel1"].ConnectionString;

            Companies company = model.Company;
            using (SqlConnection sqlconn = new SqlConnection(connString))
            {
                sqlconn.Open();

                if (sqlconn.Query<Companies>("SELECT * FROM Companies WHERE Name = '" + company.Name + "'").Count() != 0)
                {
                    return SignUpResult.ExistedName;
                }
                if (sqlconn.Query<Companies>("SELECT * FROM Companies WHERE Email = '" + company.Email + "'").Count() != 0)
                {
                    return SignUpResult.ExistedEmail;
                }
                if (company.Password.Length < 6)
                {
                    return SignUpResult.WrongPassword;
                }

                SqlCommand cmd = new SqlCommand(String.Format("INSERT INTO Companies (Name, Address, Website, City, Email, Phone, PropertyForm, CreatingDate, Password) values (N'{0]', N'{1]', N'{2]', N'{3]', N'{4]', N'{5]', N'{6]', N'{7]', N'{8}')", company.Name, company.Address, company.Website, company.City, company.Email, company.Phone, company.PropertyForm, company.CreatingDate, company.Password), sqlconn);
                cmd.ExecuteNonQuery();
            }
            return SignUpResult.Success;
        }

        public static SignUpResult Login(string login, string password)
        {
            string connString = ConfigurationManager.ConnectionStrings["DatabaseModel1"].ConnectionString;

            List<Companies> company = new List<Companies>();
            using (SqlConnection sqlconn = new SqlConnection(connString))
            {
                sqlconn.Open();
                company = sqlconn.Query<Companies>("SELECT * FROM Companies WHERE Name = '" + login + "' and Password = '" + password + "'").ToList();
            }
            if (company.Count() == 0)
            {
                return SignUpResult.SignInError;
            }
            return SignUpResult.Success;
        }
    }
}