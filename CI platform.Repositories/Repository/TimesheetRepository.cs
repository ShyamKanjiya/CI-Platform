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
    public class TimesheetRepository : GenericRepository<Timesheet>, ITimesheetRepository
    {
        public TimesheetRepository(CIDbContext db) : base(db)
        {

        }
        public IEnumerable<Timesheet> GetTimeSheetData(Expression<Func<Timesheet, bool>> filter)
        {
            IQueryable<Timesheet> query = dbSet;
            query = query.Where(filter).Include(t => t.Mission);
            return query.ToList();
        }
    }
}
