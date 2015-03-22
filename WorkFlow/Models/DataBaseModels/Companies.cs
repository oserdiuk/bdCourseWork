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

        [Required(ErrorMessage = "���������� ������ �������� ��������.")]
        [Display(Name = "�������� ��������*")]
        public string Name { get; set; }

        [Required(ErrorMessage = "���������� ������ ����� �����.")]
        [Display(Name = "�������������� ��������� �����*")]
        public string Address { get; set; }

        [Required(ErrorMessage = "���������� ������ ����� ���-�����.")]
        [Display(Name = "���-����*")]
        public string Website { get; set; }

        [Required(ErrorMessage = "���������� ������ �������� ������.")]
        [Display(Name = "�����*")]
        public string City { get; set; }

        [Required(ErrorMessage = "���������� ������ ����� ����������� �����.")]
        [EmailAddress]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [Display(Name = "���������� �������")]
        public Nullable<long> Phone { get; set; }

        [Display(Name = "������� ��������")]
        public string Logo { get; set; }

        [Display(Name = "����� �������������")]
        [StringLength(50)]
        public string PropertyForm { get; set; }

        [Column(TypeName = "date")]
        [Display(Name = "���� �������� ��������")]
        public DateTime? CreatingDate { get; set; }

        [MinLength(6, ErrorMessage="����� ������ ������ ���� ������ 6 ��������.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public virtual ICollection<Vacancies> Vacancies { get; set; }
    }
}
