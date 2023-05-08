using CI_platform.Entities.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CI_platform.Entities.ViewModels
{
    public class userVolunteerStoryModel
    {
        public Story? StoryDetails { get; set; }

        public User? UserDetails { get; set; }

        public IEnumerable<User> UserList { get; set; }

        public IEnumerable<StoryMedium> StoryMedia { get; set; }
    }
}
