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

        public User UserDetails { get; set; }

        public string? CMSTitle { get; set; }

        public string? CMSDescription { get; set; }

        public string? CMSSlug { get; set; }

        public long CMSId { get; set; }

        public int Status { get; set; }
    }
}
