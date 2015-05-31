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
        public List<string> RequiredSkills { get; set; }

        public VacancyFilter()
        {
            RequiredSkills = new List<string>();
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
        public string Company { get; set; }
        public List<string> Skills { get; set; }

        public SearchVacancyCriteria()
        {
            Skills = new List<string>();
        }
    }

    public class SkillStatisticModel
    {
        public string Id  { get; set; }
        public string CategoryName  { get; set; }
        public string SkillName  { get; set; }
        public string Popularity  { get; set; }

        public List<string> Vacancies { get; set; }

        public SkillStatisticModel(DataRow row)
        {
            this.Id = row.ItemArray[0].ToString();
            this.CategoryName = row.ItemArray[1].ToString();
            this.SkillName = row.ItemArray[2].ToString();
            this.Popularity = row.ItemArray[4].ToString();
        }
    }

    public class SkillsStatistics
    {
        public List<string> Columns { get; set; }
        public List<SkillStatisticModel> SkillsInfo { get; set; }

        public SkillsStatistics(DataTable dataTable)
        {
            this.Columns = new List<string>();
            this.SkillsInfo = new List<SkillStatisticModel>();

            foreach (DataColumn column in dataTable.Columns)
            {
                this.Columns.Add(column.Caption);
            }

            foreach (DataRow row in dataTable.Rows)
            {
                var skill = new SkillStatisticModel(row);
                skill.Vacancies = dataTable.Select("Id = " + skill.Id).Select(x => x.ItemArray[3]).Cast<string>().ToList();
                this.SkillsInfo.Add(skill);
            }
        }
    }


}