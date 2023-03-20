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

        public IEnumerable<Country> Countries { get; set; }

        public IEnumerable<City> Cities { get; set; }

        public IEnumerable<MissionTheme> MissionThemes { get; set; }

        public IEnumerable<MissionMedium> MissionMedias { get; set; }

        public IEnumerable<Skill> Skills { get; set; }

        public IEnumerable<Mission> RelatedMissions { get; set; }

        public IEnumerable<GoalMission> Goal { get; set; }

        public GoalMission GoalMissions { get; set; }

        public IEnumerable<User> Volunteeres { get; set; }

        public IEnumerable<FavoriteMission> favoriteMissions { get; set; }

        public User userDetails { get; set; }

        public IEnumerable<Comment> commentList { get; set; }    

        public int Missionrate { get; set; }

        public int RatedVolunteeres { get; set; }
    }
}
