using CI_platform.Entities.DataModels;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet]
        public IActionResult platformLandingPage()
        {
            List<Mission> missions = _dbContext.Missions.ToList();
            foreach (var mission in missions)
            {
                _dbContext.Entry(mission).Reference(c => c.City).Load();
                _dbContext.Entry(mission).Reference(t => t.MissionTheme).Load();
            }
            return View(missions);
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
