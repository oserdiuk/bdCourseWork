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

        [Required(ErrorMessage = "Необходимо ввести название компании.")]
        //!!!!!!!
        //[Remote("doesUserNameExist", "Account", HttpMethod = "POST", ErrorMessage = "User name already exists. Please enter a different user name.")]
        [Display(Name = "Название компании (Логин)*")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Необходимо ввести адрес офиса.")]
        [Display(Name = "Местоположение основного офиса*")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Необходимо ввести адрес веб-сайта.")]
        [Display(Name = "Веб-сайт*")]
        public string Website { get; set; }

        [Required(ErrorMessage = "Необходимо ввести название города.")]
        [Display(Name = "Город*")]
        public string City { get; set; }

        [Display(Name = "E-mail")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "Контактный телефон")]
        public Nullable<long> Phone { get; set; }

        [Display(Name = "Логотип компании")]
        public string Logo { get; set; }

        [Display(Name = "Форма собственности")]
        [StringLength(50)]
        public string PropertyForm { get; set; }

        //[Column(TypeName = "date")]

        [DataType(DataType.Date, ErrorMessage="Введите дату в виде месяц/день/год")]
        [Display(Name = "День рождения компании")]
        public DateTime? CreatingDate { get; set; }

        public virtual ICollection<Vacancies> Vacancies { get; set; }
    }
}
