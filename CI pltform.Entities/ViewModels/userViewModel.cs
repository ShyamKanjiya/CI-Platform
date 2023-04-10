using CI_platform.Entities.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CI_platform.Entities.ViewModels
{
    public class userViewModel
    {
        public IEnumerable<Country> Countries { get; set; }

        public IEnumerable<City> Cities { get; set; }

        public IEnumerable<MissionTheme> MissionThemes { get; set; }

        public IEnumerable<MissionMedium> MissionMedias { get; set; }

        public IEnumerable<Skill> Skills { get; set; }

        public IEnumerable<Mission> Missions { get; set; }  

        public IEnumerable<GoalMission> GoalMissions { get; set; }

        public IEnumerable<FavoriteMission> FavoriteMissions { get; set; }

        public IEnumerable<User> Volunteers { get; set; }

        public User userDetails { get; set; }

        public Mission MissionDetail { get; set; }

        public IEnumerable<MissionRating> RateMission { get; set; }

        public IEnumerable<MissionApplication> MissionApplications { get; set; }
    }
}
