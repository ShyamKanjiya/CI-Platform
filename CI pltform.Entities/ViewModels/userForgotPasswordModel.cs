using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CI_platform.Entities.ViewModels
{
    public class userForgotPasswordModel
    {
        [Required(ErrorMessage = "Email is required.")]
        public string? Email { get; set; }

        [Required]
        public string? Token { get; set; }
    }
}
