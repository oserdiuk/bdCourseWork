namespace WorkFlow.Models.DataBaseModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Skills
    {
        public Skills()
        {
            Requirements = new HashSet<Requirements>();
        }

        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [StringLength(50)]
        public string UnitMeasure { get; set; }

        public int CategoryId { get; set; }

        public virtual Categories Categories { get; set; }

        public virtual ICollection<Requirements> Requirements { get; set; }
    }
}
