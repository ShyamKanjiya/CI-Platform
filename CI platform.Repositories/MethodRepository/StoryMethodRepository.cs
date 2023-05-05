using CI_platform.Entities.DataModels;
using CI_platform.Repositories.MethodRepository.Interface;
using CI_pltform.Entities.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CI_platform.Repositories.MethodRepository
{
    public class StoryMethodRepository : IStoryMethodRepository
    {
        private readonly CIDbContext _dbContext;
        public StoryMethodRepository(CIDbContext db)
        {
            _dbContext = db;
        }
        public User UserOfStory(long storyId)
        {
                User? FindingStoryCreator = _dbContext.Stories.Where(s => s.StoryId == storyId).Select(s => s.User).FirstOrDefault();
                return FindingStoryCreator;
        }
        public Story StoryData(long storyId)
        {
            IQueryable<Story> FindingStoryCreator = _dbContext.Stories.Where(s => s.StoryId == storyId).Include(s => s.User).Include(s => s.StoryMedia);
            return FindingStoryCreator.FirstOrDefault();
        }

        public List<SavedStory> GetStory (long missionId, long userId)
        {
            var query = (from st in _dbContext.Stories
                         join md in _dbContext.StoryMedia
                         on st.StoryId equals md.StoryId into g
                         from md in g.DefaultIfEmpty()
                         where st.MissionId == missionId && st.UserId == userId && st.Status == "DRAFT"
                         orderby st.StoryId descending
                         select new SavedStory
                         {
                             StoryId = st.StoryId,
                             Title = st.Title,
                             Description = st.Description,
                             PublishedAt = st.PublishedAt,
                             Path = md.Path,
                             Type = md.Type
                         }).ToList();
            return query;
        }

        public List<MissionApplication> GetMissionApplicationsData (long userId)
        {
            var quary = from ma in _dbContext.MissionApplications
                        join gm in _dbContext.GoalMissions
                        on ma.MissionId equals gm.MissionId into g
                        from gm in g.DefaultIfEmpty()
                        where ma.UserId == userId && ma.ApprovalStatus == "APPROVE" && ma.Mission.MissionType == "GOAL" && gm.TotalValue > gm.GoalValue
                        select ma;

            return quary.ToList();
        }
    }
}
