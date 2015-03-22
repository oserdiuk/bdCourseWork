namespace WorkFlow.Models.DataBaseModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Companies
    {
        public Companies()
        {
            Vacancies = new HashSet<Vacancies>();
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Необходимо ввести название компании.")]
        [Display(Name = "Название компании*")]
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

        [Required(ErrorMessage = "Необходимо ввести адрес электронной почты.")]
        [EmailAddress]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [Display(Name = "Контактный телефон")]
        public Nullable<long> Phone { get; set; }

        [Display(Name = "Логотип компании")]
        public string Logo { get; set; }

        [Display(Name = "Форма собственности")]
        [StringLength(50)]
        public string PropertyForm { get; set; }

        [Column(TypeName = "date")]
        [Display(Name = "День рождения компании")]
        public DateTime? CreatingDate { get; set; }

        [MinLength(6, ErrorMessage="Длина пароля должна быть больше 6 символов.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public virtual ICollection<Vacancies> Vacancies { get; set; }
    }
}
