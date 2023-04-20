using CI_platform.Entities.DataModels;
using CI_platform.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CI_platform.Repositories.Repository.Interface
{
    public interface IMissionApplicationRepository : IGenericRepository<MissionApplication>
    {
        IEnumerable<MissionApplication> GetMissionApplicationList(Expression<Func<MissionApplication, bool>> filter);
        IEnumerable<MissionApplication> GetAllMissAppList();
    }
}
