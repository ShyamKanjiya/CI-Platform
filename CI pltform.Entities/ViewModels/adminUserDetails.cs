using CI_platform.Entities.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CI_platform.Entities.ViewModels
{
    public class adminUserDetails
    {
        public long UserId { get; set; }
        public string? Avatar { get; set; }

        [Required(ErrorMessage = " Required!")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = " Required!")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = " Required!")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = " Required!")]
        [DataType(DataType.Password)]
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,15}$", ErrorMessage = "Minimum 8 characters and maximum 15, at least one uppercase letter, one lowercase letter, one number and one special character")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = " Required!")]
        [Compare("Password")]
        public string ConfirmPassword { get; set; } = null!;

        [Required(ErrorMessage = " Required!")]
        [RegularExpression("^([0-9]{10})$", ErrorMessage = "Invalid Mobile Number.")]
        public long? PhoneNumber { get; set; }

        public string? EmployeeId { get; set; }

        public string? Department { get; set; }

        public string? ProfileText { get; set; }

        [Required(ErrorMessage = " Required!")]
        public long CityId { get; set; }

        [Required(ErrorMessage = " Required!")]
        public long CountryId { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Compare("NewPassword")]
        public string ConfirmPasswordEdit { get; set; } = null!;
        public IEnumerable<User> UserLists { get; set; } = new List<User>();
        public IEnumerable<Country> CountryList { get; set; } = new List<Country>();

        public User UserDetails { get; set; }
    }
}
