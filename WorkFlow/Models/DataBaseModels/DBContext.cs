namespace WorkFlow.Models.DataBaseModels
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Collections.Generic;
    using System.Data;
    using WorkFlow.Controllers;

    public partial class DBContext : DbContext
    {
        public DBContext()
            : base("name=DatabaseModel1")
        {
            this.Companies = DatabaseController.DoSQL<Companies>("Select * From Companies");
            this.Vacancies = DatabaseController.DoSQL<Vacancies>("Select * From Vacancies");
            this.DBDataTable = new DataTable();
            this.Requirements = DatabaseController.DoSQL<Requirements>("Select * From Requirements");
            this.Skills = DatabaseController.DoSQL<Skills>("Select * From Skills");
            this.Categories = DatabaseController.DoSQL<Categories>("Select * From Categories");
        }

        public DBContext(string queryToGetVacancies):this()
        {
            this.DBDataTable = DatabaseController.GetDataTable(queryToGetVacancies);
        }

        public virtual List<Categories> Categories { get; set; }
        public virtual List<Companies> Companies { get; set; }
        public virtual List<Requirements> Requirements { get; set; }
        public virtual List<Skills> Skills { get; set; }
        public virtual List<Vacancies> Vacancies { get; set; }

        public DataTable DBDataTable { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Categories>()
                .HasMany(e => e.Skills)
                .WithRequired(e => e.Categories)
                .HasForeignKey(e => e.CategoryId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Companies>()
                .HasMany(e => e.Vacancies)
                .WithRequired(e => e.Companies)
                .HasForeignKey(e => e.CompanyId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Skills>()
                .HasMany(e => e.Requirements)
                .WithRequired(e => e.Skills)
                .HasForeignKey(e => e.SkillId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Vacancies>()
                .HasMany(e => e.Requirements)
                .WithRequired(e => e.Vacancies)
                .HasForeignKey(e => e.VacancyId)
                .WillCascadeOnDelete(false);
        }
    }
}
