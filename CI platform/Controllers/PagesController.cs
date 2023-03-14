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


        public IActionResult bringMissionsToGridView(string sortBy, string missionToSearch, int pg = 1)
        {
            sortBy = String.IsNullOrEmpty(sortBy) ? "Newest" : sortBy;

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

            if (missionToSearch != null)
            {
                missions = missions.Where(m => m.Title.ToLower().Contains(missionToSearch)).ToList();
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

            return PartialView("_cardView", userView);

        }

        public List<Mission> sortMission(string sortBy, List<Mission> missions)
        {
            switch (sortBy)
            {
                case "Newest":
                    return missions.OrderByDescending(m => m.StartDate).ToList();

                case "Oldest":
                    return missions.OrderBy(m => m.StartDate).ToList();

                default:
                    return missions.OrderBy(m => m.StartDate).ToList();
            }
        }














        public IActionResult noMissionFound()
        {
            return View();
        }

        public IActionResult volunteeringMissionPage()
        {
            var viewModel = new userViewModel
            {
                Missions = _dbContext.Missions.ToList(),
                GoalMissions = _dbContext.GoalMissions.ToList(),
                Countries = _dbContext.Countries.ToList(),
                Cities = _dbContext.Cities.ToList(),
                MissionThemes = _dbContext.MissionThemes.ToList(),
                Skills = _dbContext.Skills.ToList()
            };

            return View(viewModel);
        }

        public IActionResult storyListingPage()
        {
            return View();
        }

    }
}
