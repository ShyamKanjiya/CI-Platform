﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CI_pltform.Entities.ViewModels
{
    public class userProfileModel
    {
        [Required]
        public string? OldPassword { get; set; }

        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,15}$", ErrorMessage = "Enter Valid Password")]
        public string NewPassword { get; set; } = null!;

        [NotMapped]
        [Compare("Password")]
        [Required]
        public string? ConfirmPassword { get; set; }

        //User details
        [Required]
        public string? FirstName { get; set; }

        [Required]
        public string? LastName { get; set; }

        public string? WhyIVolunteer { get; set; }

        public string? EmployeeId { get; set; }
        
        public string? Department { get; set; }

        [Required]
        public long CityId { get; set; }

        [Required]
        public long CountryId { get; set; }

        [Required]
        public string? ProfileText { get; set; }

        public string? LinkedInUrl { get; set; }
        
        public string? Title { get; set; }
    }
}