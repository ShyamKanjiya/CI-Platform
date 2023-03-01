using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CI_platform.Entities.ViewModels
{
    public class userChangePasswordModel
    {
        [Required]
        public string? Email { get; set; }

        [Required]
        public string? Token { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string? Password { get; set; }

        [NotMapped]
        [Compare("Password")]
        [Required]
        public string? ConfirmPassword { get; set; }
    }
}
