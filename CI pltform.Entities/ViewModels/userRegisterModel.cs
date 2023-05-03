using CI_platform.Entities.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CI_platform.Entities.ViewModels
{
    public class userRegisterModel
    {
        [Required]
        public string? FirstName { get; set; }

        [Required]
        public string? LastName { get; set; }

        [Required]
        public string Email { get; set; } = null!;

        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,15}$", ErrorMessage = "Minimum 8 characters and maximum 15, at least one uppercase letter, one lowercase letter, one number and one special character")]
        public string Password { get; set; } = null!;

        [NotMapped]
        [Compare("Password")]
        [Required]
        public string? ConfirmPassword { get; set; }

        [Required]
        [RegularExpression(@"^(\d{10})$", ErrorMessage = "Mobile is not valid")]
        public long PhoneNumber { get; set; }

        public long CityId { get; set; }

        public long CountryId { get; set; }

        public IEnumerable<Banner> banners { get; set; }
    }
}
