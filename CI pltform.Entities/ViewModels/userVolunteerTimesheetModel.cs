using CI_platform.Entities.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CI_platform.Entities.ViewModels
{
    public class userVolunteerTimesheetModel
    {
        public List<Mission> Missions { get; set; }

        public long MissionId { get; set; }

        public List<User> Users { get; set; }

        public List<Timesheet> Timesheets { get; set; }

        public List<MissionApplication> MissionApplicationForTime { get; set; }

        public List<MissionApplication> MissionApplicationForGoal { get; set; }

        public List<City> Cities { get; set; }
    }
}
