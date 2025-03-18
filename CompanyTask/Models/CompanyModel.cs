using System;
using System.ComponentModel.DataAnnotations;

namespace CompanyTask.Models
{
    public class CompanyModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Company Name is required!")]
        [StringLength(100, MinimumLength = 2)]
        public string CompanyName { get; set; } = null!;

        [Required(ErrorMessage = "Date is required!")]
        [DataType(DataType.Date)]
        public DateTime Startdate { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
    }
}
