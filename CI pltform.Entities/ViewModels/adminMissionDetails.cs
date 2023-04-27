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
        public long MissionId { get; set; }

        [Required(ErrorMessage = "Required!")]
        public long MissionThemeId { get; set; }

        [Required(ErrorMessage = "Required!")]
        public long CityId { get; set; }

        [Required(ErrorMessage = "Required!")]
        public long CountryId { get; set; }

        [Required(ErrorMessage = "Required!")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "Required!")]
        public string? ShortDescription { get; set; }

        [Required(ErrorMessage = "Required!")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Required!")]
        public DateTime? StartDate { get; set; }

        [Required(ErrorMessage = "Required!")]
        public DateTime? EndDate { get; set; }

        [Required(ErrorMessage = "Required!")]
        public string MissionType { get; set; } = null!;

        [Required(ErrorMessage = "Required!")]
        public int Status { get; set; }

        [Required(ErrorMessage = "Required!")]
        public string? OrganizationName { get; set; }

        [Required(ErrorMessage = "Required!")]
        public string? OrganizationDetail { get; set; }

        [Required(ErrorMessage = "Required!")]
        public string? Availability { get; set; }

        public DateTime? MissionDeadline { get; set; }

        public int? TotalSeats { get; set; }

        [Required(ErrorMessage = "Required!")]
        [RegularExpression("^((?:https?:)?\\/\\/)?((?:www|m)\\.)?((?:youtube(-nocookie)?\\.com|youtu.be))(\\/(?:[\\w\\-]+\\?v=|embed\\/|v\\/)?)([\\w\\-]+)(\\S+)?$", ErrorMessage = "Enter valid youtube link!")]
        public string? VideoUrl { get; set; }
        public IEnumerable<Mission> MissionLists { get; set; } = new List<Mission>();
        public IEnumerable<Country> CountryList { get; set; } = new List<Country>();
        public IEnumerable<MissionTheme> ThemeList { get; set; } = new List<MissionTheme>();
        public IEnumerable<Skill> SkillList { get; set; } = new List<Skill>();
        public IEnumerable<MissionMedium> MissionMediumList { get; set; } = new List<MissionMedium>();
        public IEnumerable<MissionDocument> MissionDocumentList { get; set; } = new List<MissionDocument>();
        public IEnumerable<MissionSkill> MissionSkillList { get; set; } = new List<MissionSkill>();
        public User UserDetails { get; set; }
    }
}
