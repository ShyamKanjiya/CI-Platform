using CI_platform.Entities.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CI_platform.Entities.ViewModels
{
    public class userVolunteerMission
    {
        public Mission MissionDetail { get; set; }
        
        public IEnumerable<GoalMission> Goalmissions { get; set; }

        public IEnumerable<Mission> Relatedmissions { get; set; }

        public IEnumerable<User> Volunteeres { get; set; }

        public IEnumerable<GoalMission> GoalMissions { get; set; }

        public IEnumerable<City> Cities { get; set; }

        public IEnumerable<Country> Countries { get; set; }


        public int Missionrate { get; set; }

        public int RatedVolunteeres { get; set; }
    }
}
