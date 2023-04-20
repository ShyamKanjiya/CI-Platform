using CI_platform.Entities.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CI_platform.Entities.ViewModels
{
    public class adminCMSPageDetails
    {
        public IEnumerable<CmsPage> CMSLists { get; set; } = new List<CmsPage>();
    }
}
