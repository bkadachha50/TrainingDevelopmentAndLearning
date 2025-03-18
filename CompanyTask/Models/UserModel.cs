using System.ComponentModel.DataAnnotations;

namespace CompanyTask.Models
{
    public class UserModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Username is required!")]
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = "Email is required!")]
        [RegularExpression("^([\\w-\\.]+)@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.)|(([\\w-]+\\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\\]?)$", ErrorMessage = "Invalid Email!")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password is required!")]
        [DataType(DataType.Password)]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}$", ErrorMessage = "At least 1 UpperCase, 1 LowerCase, 1 Number, 1 Special Symbol and Minimum 8 Characters are required!")]
        public string Password { get; set; } = null!;
    }
}
