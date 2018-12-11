using System;
using System.ComponentModel.DataAnnotations;

namespace Svr.Core.Entities
{
    /// <summary>
    /// ������� ����� ��� ���������
    /// </summary>
    public abstract class BaseEntity
    {
        protected const string ErrorStringEmpty ="����������, ��������� ����: {0}";
        protected const string ErrorStringMaxLength = "������������ ����� ����: {0} �� ����� {1} ��������";
        /// <summary>
        /// ���������� ��� ������ ������������� ��������
        /// </summary>
        [Key]
        public long Id { get; set; }
        /// <summary>
        /// ���� � ����� ��������
        /// </summary>
        [Display(Name = "���� ��������")]
        public DateTime CreatedOnUtc { get; set; }
        /// <summary>
        /// ���� � ����� ����������
        /// </summary>
        [Display(Name = "���� ���������")]
        public DateTime UpdatedOnUtc { get; set; }
        //[NotMapped]//����� �� ���������� ������� � �������.
        public override string ToString()=> "������� ��������";
        // ��������� ���������� �������� https://metanit.com/sharp/entityframeworkcore/3.2.php
    }
}
