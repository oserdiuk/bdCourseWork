namespace WorkFlow.Models.DataBaseModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Requirements
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int VacancyId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int SkillId { get; set; }

        [StringLength(50)]
        public string MinValue { get; set; }

        [StringLength(50)]
        public string MaxValue { get; set; }

        public virtual Vacancies Vacancies { get; set; }

        public virtual Skills Skills { get; set; }
    }
}
