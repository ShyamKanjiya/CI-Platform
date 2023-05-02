using CI_platform.Entities.DataModels;
using CI_platform.Repositories.GenericRepository;
using CI_platform.Repositories.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CI_platform.Repositories.Repository
{
    public class MissionRepository : GenericRepository<Mission> , IMissionRepository
    {
        public MissionRepository(CIDbContext db) : base(db)
        { 
        
        }

        public IEnumerable<Mission> GetAllMissions()
        {
            IQueryable<Mission> query = dbSet;
            query = query.Where(m => m.DeletedAt == null).Include(m => m.MissionMedia)
                .Include(m => m.MissionSkills);
            return query.ToList();
        }
    }
}
