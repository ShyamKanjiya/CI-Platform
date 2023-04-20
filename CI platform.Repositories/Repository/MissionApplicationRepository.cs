using CI_platform.Entities.DataModels;
using CI_platform.Repositories.GenericRepository;
using CI_platform.Repositories.Interface;
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
    public class MissionApplicationRepository : GenericRepository<MissionApplication> , IMissionApplicationRepository
    {
        public MissionApplicationRepository(CIDbContext db) : base(db) 
        {
        
        }
        public IEnumerable<MissionApplication> GetMissionApplicationList(Expression<Func<MissionApplication, bool>> filter)
        {
            IQueryable<MissionApplication> query = dbSet;
            query = query.Where(filter).Include(m => m.Mission).Include(m => m.Mission.City);
            return query.ToList();
        }
        public IEnumerable<MissionApplication> GetAllMissAppList()
        {
            IQueryable<MissionApplication> query = dbSet;
            query = query.Include(m => m.Mission).Include(m => m.User);
            return query.ToList();
        }
    }
}
