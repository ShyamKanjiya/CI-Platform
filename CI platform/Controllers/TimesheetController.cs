using CI_platform.Entities.DataModels;
using Microsoft.AspNetCore.Mvc;

namespace CI_platform.Controllers
{
    public class TimesheetController : Controller
    {

        private readonly ILogger<TimesheetController> _logger;
        private readonly CIDbContext _dbContext;

        public TimesheetController(ILogger<TimesheetController> logger, CIDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public IActionResult VolunteerTimesheet()
        {
            return View();
        }
    }
}
