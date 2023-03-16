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
                Skills = _dbContext.Skills.ToList()
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

        public IActionResult volunteeringMissionPage(int id)
        {
            /*userVolunteerMission viewModel = new()
            {
                MissionDetail = _dbContext.Missions.Where(m => m.MissionId == id).FirstOrDefault(),
                City = _dbContext.Cities.ToList(),

            };
*/
                          
                       

            return View();
        }

        public IActionResult storyListingPage()
        {
            return View();
        }

    }
}
