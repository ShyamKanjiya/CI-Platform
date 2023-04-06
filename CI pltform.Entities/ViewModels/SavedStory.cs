using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CI_pltform.Entities.ViewModels
{
    public class SavedStory
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? PublishedAt { get; set; }
        public string Path { get; set; }
        public string Type { get; set; }
        public long StoryId { get; set; }
    }
}
