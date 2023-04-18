using CI_platform.Entities.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CI_platform.Entities.ViewModels
{
    public class adminUserDetails
    {
        public IEnumerable<User> UserLists { get; set; } = new List<User>();
    }
}
