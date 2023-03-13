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

        public PagesController(ILogger<PagesController> logger, CIDbContext dbContext, int pageindex = 1, int pageSize = 9)
        {
            _logger = logger;
            _dbContext = dbContext;
        }


        //---------------------- CARD VIEW --------------------------//
        [HttpGet]
        public IActionResult platformLandingPage(int pageindex = 1, int pageSize = 9)
        {
            var viewModel = new userMissionModel
            {
                Countries = _dbContext.Countries.ToList(),
                Cities = _dbContext.Cities.ToList(),
                MissionThemes = _dbContext.MissionThemes.ToList(),
                Skills = _dbContext.Skills.ToList(),
                totalrecord = _dbContext.Missions.Count(),
                currentPage = pageindex
            };

            return View(viewModel);
        }


        public IActionResult bringMissionsToGridView(string sortBy, string missionToSearch, int pageindex = 1, int pageSize = 9)
        {
            sortBy = String.IsNullOrEmpty(sortBy) ? "Newest" : sortBy;

            List<Mission> missions = _dbContext.Missions
                .Include(m => m.GoalMissions)
                .Include(m => m.MissionApplications)
                .Include(m => m.MissionMedia)
                .Include(m => m.FavoriteMissions)
                .Include(m => m.MissionTheme)
                .Include(m => m.City)
                .Include(m => m.Country)
                .ToList();
            List<userMissionModel> missionVmList = new();

            if (missionToSearch != null)
            {
                missions = missions.Where(m => m.Title.ToLower().Contains(missionToSearch)).ToList();

                foreach (var currMisssion in missions)
                {
                    missionVmList.Add(convertDataModelToMissionModel(currMisssion));
                }
                missionVmList = missionVmList.Skip((pageindex - 1) * pageSize).Take(pageSize).ToList();
                return PartialView("_cardView", missionVmList);


            }

            /*switch (sortBy)
            {
                case "Newest":
                    viewModel.Missions = viewModel.Missions.OrderByDescending(mission => mission.CreatedAt).ToList();
                    break;
                case "Oldest":
                    viewModel.Missions = viewModel.Missions.OrderBy(mission => mission.CreatedAt).ToList();
                    break;
                default:
                    viewModel.Missions = viewModel.Missions.OrderBy(mission => mission.CreatedAt).ToList();
                    break;
            }*/


            foreach (var currMisssion in missions)
            {
                missionVmList.Add(convertDataModelToMissionModel(currMisssion));
            }
            missionVmList = missionVmList.Skip((pageindex - 1) * pageSize).Take(pageSize).ToList();
            return PartialView("_cardView", missionVmList);
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

        public userMissionModel convertDataModelToMissionModel(Mission mission)
        {
            GoalMission mg = new();
            if (mission.GoalMissions.Count() > 0)
            {
                mg = mission?.GoalMissions.Where(X => X.MissionId == mission.MissionId).First();
            }
            userMissionModel missionModel = new userMissionModel();
            missionModel.MissionId = mission.MissionId;
            missionModel.CityId = mission.CityId;
            missionModel.City = mission.City;
            missionModel.ThemeId = mission.MissionThemeId;
            missionModel.Theme = mission.MissionTheme;
            missionModel.Title = mission.Title;
            missionModel.ShortDescription = mission.ShortDescription;
            missionModel.StartDate = mission.StartDate.ToString().Remove(10);
            missionModel.EndDate = mission.EndDate.ToString().Remove(10);
            missionModel.OrganizationName = mission.OrganizationName;
            missionModel.MissionType = mission.MissionType;
            missionModel.GoalMission = mg;
            return missionModel;
        }
    }
}
