using CI_platform.Entities.DataModels;
using CI_pltform.Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CI_platform.Repositories.MethodRepository.Interface
{
    public interface IStoryMethodRepository
    {
        public User UserOfStory(long storyId);
        public List<SavedStory> GetStory(long missionId, long userId);
    }
}
