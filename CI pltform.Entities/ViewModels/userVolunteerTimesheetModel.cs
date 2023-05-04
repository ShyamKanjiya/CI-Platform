using CI_platform.Entities.DataModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CI_platform.Entities.ViewModels
{
    public class userVolunteerTimesheetModel
    {
        public long TimeSheetId { get; set; }

        [Required(ErrorMessage = "Required!")]
        public long MissionId { get; set; }

        [Range(00, 23, ErrorMessage = "Hours must between 00 to 23")]
        [Required(ErrorMessage = "Required!")]
        public int Hours { get; set; }

        [Required(ErrorMessage = "Required!")]
        [Range(00, 59, ErrorMessage = "Minutes must between 00 to 59")]
        public int Minutes { get; set; }

        [Range(1, Double.PositiveInfinity, ErrorMessage = "Minimum value must be 1")]
        [Required(ErrorMessage = "Required!")]
        public int Action { get; set; }

        [Required(ErrorMessage = "Required!")]
        public DateTime DateVolunteered { get; set; }

        [Required(ErrorMessage = "Required!")]
        public string? Notes { get; set; }

        public IEnumerable<Timesheet> TimesheetsForTime { get; set; }

        public IEnumerable<Timesheet> TimesheetsForGoal{ get; set; }

        public IEnumerable<MissionApplication> MissionApplicationForTime { get; set; }

        public IEnumerable<MissionApplication> MissionApplicationForGoal { get; set; }

        public User? UserDetails { get; set; }

        public IEnumerable<Mission> Missions { get; set; }

        public IEnumerable<City> Cities { get; set; }
    }
}
