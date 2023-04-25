using CI_platform.Entities.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CI_platform.Entities.ViewModels
{
    public class adminMissionThemeDetails
    {
        public IEnumerable<MissionTheme> MissionThemeLists { get; set; } = new List<MissionTheme>();

        public User UserDetails { get; set; }

        [Required(ErrorMessage = "Required!")]
        public string? MissionThemeTitle { get; set; }

        public long MissionThemeId { get; set; }

        public int Status { get; set; }
    }
}
