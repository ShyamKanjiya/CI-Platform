using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(10, MinimumLength = 6,ErrorMessage = "Password not valid" )]
        public string Password { get; set; } = null!;

        [Required]
        [RegularExpression(@"^(\d{10})$", ErrorMessage = "Mobile no not valid")]
        public int PhoneNumber { get; set; }

        public long CityId { get; set; }

        public long CountryId { get; set; }
    }
}
