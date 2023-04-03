using CI_platform.Entities.DataModels;
using CI_platform.Repositories.GenericRepository;
using CI_platform.Repositories.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CI_platform.Repositories.Repository
{
    public class PasswordResetRepository : GenericRepository<PasswordReset> , IPasswordResetRepository
    {
        public PasswordResetRepository(CIDbContext db) : base(db) 
        { 
        
        }
    }
}
