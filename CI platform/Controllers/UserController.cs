using CI_platform.Entities.DataModels;
using CI_platform.Entities.ViewModels;
using CI_platform.Repositories.GenericRepository;
using CI_platform.Repositories.GenericRepository.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CI_platform.Controllers
{
    public class UserController : Controller
    {

        private readonly ILogger<UserController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public UserController(ILogger<UserController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public User GetThisUser()
        {
            var identity = User.Identity as ClaimsIdentity;
            var email = identity?.FindFirst(ClaimTypes.Email)?.Value;

            var user =  _unitOfWork.User.GetFirstOrDefault(m => m.Email == email);
            return user;
        }

        //---------------------- Volunteeer Profile --------------------------//

        #region Volunteer Profile
        [HttpGet]
        public IActionResult VolunteerProfile()
        {
            User user = GetThisUser();
            IEnumerable<Skill> skillList = _unitOfWork.Skill.GetAll();
            IEnumerable<Country> countryList = _unitOfWork.Country.GetAll();
            IEnumerable<City> cityList = _unitOfWork.City.GetAll();
            userViewModel obj = new()
            {
                userDetails = user,
                Skills = skillList,
                Countries = countryList,
                Cities = cityList,
            };
            return View(obj);
        }

        [HttpPost]
        public IActionResult ChangePassword(string OldPassword, string NewPassword) 
        {
            User user = GetThisUser();

            if (user != null && OldPassword != null && NewPassword != null)
            {
                if (user.Password == OldPassword)
                {
                    user.Password = NewPassword;
                    user.UpdatedAt = DateTime.Now;
                    _unitOfWork.User.Update(user);
                    _unitOfWork.Save();

                    return Json(1);
                }
            }
            return Json(0);
        }

        #endregion

        //---------------------- Volunteer Timesheet --------------------------//

        #region Volunteer Timesheet
        public IActionResult VolunteerTimesheet()
        {
            return View();
        }
        #endregion

        //---------------------- Volunteer Policy --------------------------//

        #region Volunteer Policy
        public IActionResult VolunteerPolicy()
        {
            return View();
        }

        #endregion
    }
}
