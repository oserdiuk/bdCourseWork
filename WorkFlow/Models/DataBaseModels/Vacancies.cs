namespace WorkFlow.Models.DataBaseModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Vacancies
    {
        public Vacancies()
        {
            Requirements = new HashSet<Requirements>();
        }

        public int Id { get; set; }

        public int CompanyId { get; set; }

        [Required]
        [Display(Name = "Название вакансии*")]
        public string Name { get; set; }

        [Column(TypeName = "date")]
        [Display(Name = "Дата открытия вакансии*")]
        public DateTime? OpenDate { get; set; }

        [Display(Name = "Количество мест")]
        public int? Amount { get; set; }

        public virtual Companies Companies { get; set; }

        public virtual ICollection<Requirements> Requirements { get; set; }
    }
}
