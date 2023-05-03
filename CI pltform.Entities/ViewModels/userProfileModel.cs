using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CI_platform.Entities.DataModels;

namespace CI_pltform.Entities.ViewModels
{
    public class userProfileModel
    {
        private string firstName;
        private string lastName;

        [Required]
        public string? OldPassword { get; set; }

        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,15}$", ErrorMessage = "Minimum 8 characters and maximum 15, at least one uppercase letter, one lowercase letter, one number and one special character")]
        public string NewPassword { get; set; } = null!;

        [NotMapped]
        [Compare("NewPassword")]
        [Required]
        public string? ConfirmPassword { get; set; }

        //User details
        [Required]
        public string? FirstName
        {
            get
            {
                return firstName?.Trim();
            }
            set
            {
                firstName = value;
            }
        }

        [Required]
        public string? LastName {
            get
            {
                return lastName?.Trim();
            }
            set
            {
                lastName = value;
            }
        }

        public string? WhyIVolunteer { get; set; }

        public string? Avatar { get; set; }

        public string? EmployeeId { get; set; }

        public string? Manager { get; set; }

        public string? Department { get; set; }

        [Required]
        public long CityId { get; set; }

        [Required]
        public long CountryId { get; set; }

        [Required]
        public string? ProfileText { get; set; }

        public string? LinkedInUrl { get; set; }

        public string? Availibility { get; set; }

        public string? Title { get; set; }

        public IEnumerable<UserSkill>? UserSkillList { get; set; }
        public IEnumerable<City>? Cities { get; set; }
        public IEnumerable<Country>? Countries { get; set; }
        public IEnumerable<Skill>? SkillsList { get; set; }
        public User? UserDetails { get; set; }
    }
}
