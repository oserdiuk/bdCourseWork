namespace WorkFlow.Models.DataBaseModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Web.Mvc;

    public class Companies
    {
        public Companies()
        {
            Vacancies = new HashSet<Vacancies>();
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Company name is required.")]
        //!!!!!!!
        //[Remote("doesUserNameExist", "Account", HttpMethod = "POST", ErrorMessage = "User name already exists. Please enter a different user name.")]
        [Display(Name = "Company name (Username)*")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [Display(Name = "Address of a main office*")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Web-site is required.")]
        [Display(Name = "Web-site*")]
        public string Website { get; set; }

        [Required(ErrorMessage = "City is required.")]
        [Display(Name = "City*")]
        public string City { get; set; }

        [Display(Name = "E-mail*")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "Phone")]
        public Nullable<long> Phone { get; set; }

        [Display(Name = "Company logo")]
        public string Logo { get; set; }

        [Display(Name = "Property form")]
        [StringLength(50)]
        public string PropertyForm { get; set; }

        //[Column(TypeName = "date")]

        [DataType(DataType.Date, ErrorMessage="Enter date as mm/dd/yyyy.")]
        [Display(Name = "Company opening date")]
        public DateTime CreatingDate { get; set; }

        public virtual ICollection<Vacancies> Vacancies { get; set; }
    }
}
