using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;
using WorkFlow.Models.DataBaseModels;

namespace WorkFlow.Models
{
    public class CompanyFilter
    {
        public List<string> City { get; set; }
        public string NumberOfVacancyMin { get; set; }
        public string NumberOfVacancyMax { get; set; }
    }

    public class VacancyFilter
    {
        [DataType(DataType.Date, ErrorMessage = "Wrong date")]
        public DateTime MinOpenDate { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Wrong date")]
        public DateTime MaxOpenDate { get; set; }
        public List<Skills> RequiredSkills { get; set; }

        public VacancyFilter()
        {
            RequiredSkills = new List<Skills>();
        }
    }

    public class QueryEditorInfo
    {
        public DataTable DateTable { get; set; }
        [DataType(DataType.MultilineText)]
        public string Query { get; set; }


        public QueryEditorInfo()
        {
            this.DateTable = new DataTable();
        }
    }

    public class SearchCompanyCriteria
    {
        public string Name { get; set; }
        public string City { get; set; }
        public string VacancyName { get; set; }
    }

    public class SearchVacancyCriteria
    {
        public string Name { get; set; }
        public string Skill { get; set; }
    }


}