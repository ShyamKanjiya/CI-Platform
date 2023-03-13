using CI_platform.Entities.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CI_platform.Entities.ViewModels
{
    public class userMissionModel
    {
        public long MissionId { get; set; }

        public long ThemeId { get; set; }

        public long CityId { get; set; }

        public long CountryId { get; set; }

        public string? GoalObjectiveText { get; set; }

        public string Title { get; set; } = null!;

        public string? ShortDescription { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }

        public string MissionType { get; set; } = null!;

        public bool Status { get; set; }

        public string? OrganizationName { get; set; }

        public string? Availability { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }
        public int totalrecord { get; set; }
        public int currentPage { get; set; }

        public virtual City City { get; set; } = null!;

        public virtual Country Country { get; set; } = null!;

        public virtual ICollection<FavoriteMission> FavouriteMissions { get; } = new List<FavoriteMission>();

        public virtual ICollection<GoalMission> GoalMissions { get; } = new List<GoalMission>();

        public virtual ICollection<MissionInvite> MissionInvites { get; } = new List<MissionInvite>();

        public virtual ICollection<MissionMedium> MissionMedia { get; } = new List<MissionMedium>();

        public virtual ICollection<MissionRating> MissionRatings { get; } = new List<MissionRating>();

        public virtual MissionTheme Theme { get; set; } = null!;

        public GoalMission? GoalMission { get; set; }

        public IEnumerable<Country> Countries { get; set; }

        public IEnumerable<City> Cities { get; set; }

        public IEnumerable<MissionTheme> MissionThemes { get; set; }

        public IEnumerable<MissionMedium> MissionMedias { get; set; }

        public IEnumerable<Skill> Skills { get; set; }

        public IEnumerable<Mission> Missions { get; set; }
    }
}
