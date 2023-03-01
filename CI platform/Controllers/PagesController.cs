using Microsoft.AspNetCore.Mvc;

namespace CI_platform.Controllers
{
    public class PagesController : Controller
    {
        public IActionResult platformLandingPage()
        {
            return View();
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
