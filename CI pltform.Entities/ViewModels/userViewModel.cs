using CI_platform.Entities.DataModels;
using System;
using System.Collections.Generic;
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

        public int MissionCount { get; set; }

        public int totalrecord { get; set; }
        public int currentPage { get; set; }
    }
}
