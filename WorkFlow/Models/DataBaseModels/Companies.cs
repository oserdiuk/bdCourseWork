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

        [Required(ErrorMessage = "���������� ������ �������� ��������.")]
        //!!!!!!!
        //[Remote("doesUserNameExist", "Account", HttpMethod = "POST", ErrorMessage = "User name already exists. Please enter a different user name.")]
        [Display(Name = "�������� �������� (�����)*")]
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

        [Display(Name = "E-mail")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "���������� �������")]
        public Nullable<long> Phone { get; set; }

        [Display(Name = "������� ��������")]
        public string Logo { get; set; }

        [Display(Name = "����� �������������")]
        [StringLength(50)]
        public string PropertyForm { get; set; }

        //[Column(TypeName = "date")]

        [DataType(DataType.Date, ErrorMessage="������� ���� � ���� �����/����/���")]
        [Display(Name = "���� �������� ��������")]
        public DateTime? CreatingDate { get; set; }

        public virtual ICollection<Vacancies> Vacancies { get; set; }
    }
}
