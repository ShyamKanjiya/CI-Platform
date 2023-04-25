using CI_platform.Entities.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CI_platform.Entities.ViewModels
{
    public class adminStoryDetails
    {
        public IEnumerable<Story> StoryLists { get; set; } = new List<Story>();

        public User UserDetails { get; set; }

        public User? UserOfStory { get; set; }
    }
}
