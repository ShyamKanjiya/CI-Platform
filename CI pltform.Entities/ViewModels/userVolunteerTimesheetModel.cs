using CI_platform.Entities.DataModels;
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

        [Required(ErrorMessage = "Required!")]
        public int Hours { get; set; }

        [Required(ErrorMessage = "Required!")]
        public int Minutes { get; set; }

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
