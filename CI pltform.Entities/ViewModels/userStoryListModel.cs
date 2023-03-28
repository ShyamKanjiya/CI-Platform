using CI_platform.Entities.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CI_platform.Entities.ViewModels
{
    public class userStoryListModel
    {
        public IEnumerable<Story> Stories { get; set; } 

        public IEnumerable<Mission> Missions { get; set; }

        public IEnumerable<User> Users { get; set; }

        public IEnumerable<MissionTheme> MissionThemes { get; set; }    

        public IEnumerable<MissionApplication> missionApplications { get; set; }
    }
}
