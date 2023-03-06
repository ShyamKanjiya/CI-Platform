using CI_platform.Entities.DataModels;
using CI_platform.Entities.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

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
            userViewModel obj = new userViewModel();
            obj = partial();
            return View(obj);
        }

        public userViewModel partial()
        {
            userViewModel obj = new()
            {
                Countries = GetCountries(),
                Cities = GetCities(),
                Missions = GetMissions(),
                MissionThemes = GetMissionThemes(),
                Skills = GetSkills(),
                GoalMissions =  GetGoalMissions()
            };

            return obj;
        }

        public IEnumerable<Mission> GetMissions()
        {
            return _dbContext.Missions.ToList();
        }

        public IEnumerable<Country> GetCountries()
        {
            return _dbContext.Countries.ToList();
        }

        public IEnumerable<City> GetCities()
        {
            return _dbContext.Cities.ToList();
        }

        public IEnumerable<MissionTheme> GetMissionThemes() 
        {
            return _dbContext.MissionThemes.ToList();
        }

        public IEnumerable<Skill> GetSkills() 
        {   
            return _dbContext.Skills.ToList();
        }

        public IEnumerable<GoalMission> GetGoalMissions()        
        {
            return _dbContext.GoalMissions.ToList();
        }



        public IActionResult noMissionFound()
        {
            return View();
        }

        public IActionResult volunteeringMissionPage()
        {
            userViewModel obj = new userViewModel();
            obj = partial();
            return View(obj);
        }

        public IActionResult storyListingPage()
        {
            return View();
        }
    }
}
