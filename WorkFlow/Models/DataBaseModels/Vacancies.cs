namespace WorkFlow.Models.DataBaseModels
{
    using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Data.Entity.Spatial;

    public class Vacancies
    {
        //public Vacancies()
        //{
        //    Requirements = new HashSet<Requirements>();
        //}

        public int Id { get; set; }

        public int CompanyId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [Display(Name = "Vacancy name*")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Date is required")]
        [DataType(DataType.Date, ErrorMessage = "Wrong date")]
        [Display(Name = "Vacancy opening date*")]
        public DateTime OpenDate { get; set; }

        [Range(0, 250, ErrorMessage = "Please enter a number between 0 .")]
        [DataAnnotationsExtensions.Integer(ErrorMessage = "Please enter a valid number.")]
        [Display(Name = "Amount of free positions")]
        public int? Amount { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        public string FileName { get; set; }

        public virtual Companies Companies { get; set; }

        public virtual List<Requirements> Requirements { get; set; }
    }
}
