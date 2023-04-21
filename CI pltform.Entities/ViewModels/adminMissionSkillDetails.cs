using CI_platform.Entities.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CI_platform.Entities.ViewModels
{
    public class adminMissionSkillDetails
    {
        public IEnumerable<Skill> SkillLists { get; set; } = new List<Skill>();

        [Required(ErrorMessage = "Required!")]
        public string? SkillName { get; set; }

        public long SkillIds { get; set; }

        public int Status { get; set; }
    }
}
