using CI_platform.Entities.DataModels;
using CI_platform.Repositories.GenericRepository;
using CI_platform.Repositories.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CI_platform.Repositories.Repository
{
    public class StoryRepository : GenericRepository<Story> , IStoryRepository
    {
        public StoryRepository(CIDbContext db) : base(db) 
        { 
        
        }
        public Story GetStoryAndMedia(Expression<Func<Story, bool>> filter)
        {
            IQueryable<Story> query = dbSet;
            query = query.Where(filter).Include(story => story.StoryMedia);
            return query.FirstOrDefault();
        }
        public IEnumerable<Story> GetAllStory()
        {
            IQueryable<Story> query = dbSet;
            query = query.Include(story => story.User).Include(story => story.Mission);
            return query.ToList();
        }
    }
}
