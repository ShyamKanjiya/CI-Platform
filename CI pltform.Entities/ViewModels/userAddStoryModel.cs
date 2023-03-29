using CI_platform.Entities.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CI_platform.Entities.ViewModels
{
    public class userAddStoryModel
    {
        public List<Mission> Missions { get; set; }

        public long MissionId { get; set; }

        public List<User> Users { get; set; }

        public List<MissionApplication> MissionApplication { get; set; }

        public List<Story> Stories { get; set; }

        public Story Story { get; set; }

        public List<StoryMedium> StoryMedium { get; set; }

        public User User { get; set; }

        public string Result { get; set; }
    }
}
