using CI_platform.Entities.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CI_platform.Entities.ViewModels
{
    public class adminMissionDetails
    {
        public IEnumerable<Mission> MissionLists { get; set; } = new List<Mission>();

        public IEnumerable<Country> CountryList { get; set; } = new List<Country>();

        public IEnumerable<UserSkill>? UserSkillList { get; set; }

        public IEnumerable<Skill>? SkillsList { get; set; }

        public User UserDetails { get; set; }

        public long MissionId { get; set; }

        public long MissionThemeId { get; set; }

        [Required(ErrorMessage = " Required!")]
        public long CityId { get; set; }

        [Required(ErrorMessage = " Required!")]
        public long CountryId { get; set; }

        [Required(ErrorMessage = " Required!")]
        public string Title { get; set; } = null!;

        public string? ShortDescription { get; set; }

        public string? Description { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Required(ErrorMessage = " Required!")]
        public string MissionType { get; set; } = null!;

        public int? Status { get; set; }

        public string? OrganizationName { get; set; }

        public string? OrganizationDetail { get; set; }

        public string? Availability { get; set; }

        public DateTime? Deadline { get; set; }

        public int? Seats { get; set; }
    }
}
