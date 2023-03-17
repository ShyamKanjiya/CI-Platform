using CI_platform.Entities.DataModels;
using CI_platform.Entities.ViewModels;
using CI_pltform.Entities.ViewModels;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using System.Security.Claims;
using System.Security.Principal;
using System.Xml.Schema;

namespace CI_platform.Controllers
{
    public class PagesController : Controller
    {
        private readonly ILogger<PagesController> _logger;
        private readonly CIDbContext _dbContext;

        public PagesController(ILogger<PagesController> logger, CIDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }


        //---------------------- CARD VIEW --------------------------//
        [HttpGet]
        public IActionResult platformLandingPage()
        {
            userViewModel viewModel = new userViewModel
            {
                Countries = _dbContext.Countries.ToList(),
                Cities = _dbContext.Cities.ToList(),
                MissionThemes = _dbContext.MissionThemes.ToList(),
                Skills = _dbContext.Skills.ToList()
            };
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult bringMissionsToGridView(string[]? country, string[]? cities, string[]? theme, string[]? skill, string? sortBy, string? missionToSearch, int pg = 1 )
        {

            userViewModel userView = new userViewModel
            {
                Missions = _dbContext.Missions.ToList(),
                GoalMissions = _dbContext.GoalMissions.ToList(),
                Countries = _dbContext.Countries.ToList(),
                Cities = _dbContext.Cities.ToList(),
                MissionThemes = _dbContext.MissionThemes.ToList(),
                Skills = _dbContext.Skills.ToList(),
                users = _dbContext.Users.ToList()
            };

            List<Mission> missions = _dbContext.Missions.ToList();
            
            if(country.Count() > 0 || cities.Count() > 0 || theme.Count() > 0)
            {
                missions = filterMission(missions, country, cities, theme, skill);
            }

            if (missionToSearch != null)
            {
                missions = missions.Where(m => m.Title.ToLower().Contains(missionToSearch.ToLower())).ToList();
            }

            missions = sortMission(sortBy, missions);

            const int pageSize = 3;
            if (pg < 1)
                pg = 1;

            int recsCount = missions.Count();

            var pager = new userPager(recsCount, pg, pageSize);

            int recSkip = (pg - 1) * pageSize;

            missions = missions.Skip(recSkip).Take(pager.PageSize).ToList();

            this.ViewBag.userPager = pager;

            userView.Missions = missions;

            ViewBag.missionCount = recsCount;

            if (recsCount == 0)
            {
                return RedirectToAction("noMissionFound","Pages");
            }
            else
            {
                return PartialView("_cardView", userView);
            }
        }

        public List<Mission> sortMission(string sortBy, List<Mission> missions)
        {
            switch (sortBy)
            {
                case "Newest":
                    return missions.OrderBy(m => m.StartDate).ToList();

                case "Oldest":
                    return missions.OrderByDescending(m => m.StartDate).ToList();

                case "AZ":
                    return missions.OrderBy(m => m.Title).ToList();

                case "ZA":
                    return missions.OrderByDescending(m => m.Title).ToList();

                case "GOAL":
                    return missions.Where(m => m.MissionType.Equals("GOAL")).ToList();

                case "TIME":
                    return missions.Where(m => m.MissionType.Equals("TIME")).ToList();

                default:
                    return missions.ToList();
            }
        }


        public List<Mission> filterMission(List<Mission> missions,string[] country, string[] cities, string[] theme, string[] skill)
        {
            if (country.Length > 0)
            {
                missions = missions.Where(s => country.Contains(s.Country.Name)).ToList();
            }
            if (cities.Length > 0)
            {
                missions = missions.Where(s => cities.Contains(s.City.Name)).ToList();
            }
            if (theme.Length > 0)
            {
                missions = missions.Where(s => theme.Contains(s.MissionTheme.Title)).ToList();
            }
           /* if (skill.Length > 0)
            {
                missions = missions.Where(s => skill.Contains(s.)).ToList();
            }*/
            return missions.ToList();
        }











        public IActionResult noMissionFound()
        {
            return View();
        }

        #region Volunteering Mission Page
        public IActionResult volunteeringMissionPage(int id)
        {
            var missionDetail = _dbContext.Missions.Where(m => m.MissionId == id).FirstOrDefault();

            userVolunteerMission viewModel = new()
            {
                MissionDetail = missionDetail,
                Cities = _dbContext.Cities.Where(c => c.CityId == missionDetail.CityId).FirstOrDefault(),
                MissionThemes = _dbContext.MissionThemes.Where(m => m.MissionThemeId == missionDetail.MissionThemeId).FirstOrDefault(),
                GoalMissions = _dbContext.GoalMissions.Where(m => m.MissionId == id).FirstOrDefault(),
                RelatedMissions = RelatedMissions(id),
                Goal = _dbContext.GoalMissions.ToList(),
                users = _dbContext.Users.ToList()
            };

            return View(viewModel);
        }


        public IEnumerable<Mission> RelatedMissions(int id)
        {
            
            var relatedmissions = new List<Mission>();
            var thismission = _dbContext.Missions.Where(m => m.MissionId.Equals(id)).FirstOrDefault();
            var relatedmissions_by_city = _dbContext.Missions.Where(m => m.MissionId != thismission.MissionId && m.CityId == thismission.CityId).Take(3).ToList();
            var relatedmissions_by_country = _dbContext.Missions.Where(m => m.MissionId != thismission.MissionId && m.CountryId == thismission.CountryId).Take(3).ToList();
            var relatedmissions_by_theme = _dbContext.Missions.Where(m => m.MissionId != thismission.MissionId && m.MissionThemeId == thismission.MissionThemeId).Take(3).ToList();

            if (relatedmissions_by_city.Count() > 0)
            {
                relatedmissions = relatedmissions_by_city;
            }
            else if (relatedmissions_by_country.Count() > 0)
            {
                relatedmissions = relatedmissions_by_country;
            }
            else
            {
                relatedmissions = relatedmissions_by_theme;
            }

            foreach (var mission in relatedmissions)
            {
                _dbContext.Entry(mission).Reference(c => c.City).Load();
                _dbContext.Entry(mission).Reference(t => t.MissionTheme).Load();
            }
            return relatedmissions;
        }
/*
        public void FavouriteMission(long missionId,string? emailId)
        {
            var user = _dbContext.Users.Where(m => m.Email.Equals(emailId)).Select(m => m.UserId).FirstOrDefault();
            var FavMission = _dbContext.FavoriteMissions.Where(m => m.UserId.Equals(user)).FirstOrDefault();

            if (FavMission == null)
            {
                var favoriteMission = new FavoriteMission()
                {
                    UserId = user,
                    MissionId = missionId
                };

                _dbContext.Add(favoriteMission);
                _dbContext.SaveChanges();
            }
            else
            {
                _dbContext.Remove(FavMission);
                _dbContext.SaveChanges();
            }
        }
*/
        #endregion Volunteering Mission Page

        public IActionResult storyListingPage()
        {
            return View();
        }

    }
}
