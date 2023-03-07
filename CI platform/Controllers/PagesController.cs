using CI_platform.Entities.DataModels;
using CI_platform.Entities.ViewModels;
using CI_pltform.Entities.ViewModels;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            var viewModel = new userViewModel
            {
                Missions = _dbContext.Missions.ToList(),
                GoalMissions = _dbContext.GoalMissions.ToList(),
                Countries = _dbContext.Countries.ToList(),
                Cities = _dbContext.Cities.ToList(),
                MissionThemes = _dbContext.MissionThemes.ToList(),
                Skills = _dbContext.Skills.ToList()
            };

            // Combine the MissionMedia and GoalMissions tables
            var missionsFromMedia = from mm in _dbContext.MissionMedia
                                    join m in _dbContext.Missions on mm.MissionId equals m.MissionId
                                    select m;

            var combinedMissions = viewModel.Missions.Union(viewModel.GoalMissions.Select(gm => gm.Mission)).Union(missionsFromMedia).ToList();

            int missionCount = 0;
            foreach (var mission in combinedMissions)
            {
                _dbContext.Entry(mission).Reference(c => c.City).Load();
                _dbContext.Entry(mission).Reference(t => t.MissionTheme).Load();
                missionCount++;
            }

            viewModel.Missions = combinedMissions;
            viewModel.MissionCount = missionCount;

            //const int pageSize = 9;
            //if(pg < 1)
            //    pg = 1;

            //int recsCount = obj.Missions.Count();

            //var pager = new userPagerModel(recsCount, pg, pageSize);

            //int recSkip = (pg - 1) * pageSize;

            //var data = obj.Missions.Skip(recSkip).Take(pager.PageSize).ToList();

            //this.ViewBag.Pager = pager;

            return View(viewModel);
        }

        
       
        public IActionResult noMissionFound()
        {
            return View();
        }

        public IActionResult volunteeringMissionPage()
        {
            return View();
        }

        public IActionResult storyListingPage()
        {
            return View();
        }
    }
}
