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
    public interface ITimesheetRepository : IGenericRepository<Timesheet>
    {
        IEnumerable<Timesheet> GetTimeSheetData(Expression<Func<Timesheet, bool>> filter);
    }
}
