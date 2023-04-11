using CI_platform.Entities.DataModels;
using CI_platform.Entities.ViewModels;
using CI_platform.Repositories.GenericRepository;
using CI_platform.Repositories.GenericRepository.Interface;
using CI_pltform.Entities.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CI_platform.Controllers
{
    public class UserController : Controller
    {

        private readonly ILogger<UserController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _iweb;

        public UserController(ILogger<UserController> logger, IUnitOfWork unitOfWork, IWebHostEnvironment iweb)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _iweb = iweb;
        }

        public User GetThisUser()
        {
            var identity = User.Identity as ClaimsIdentity;
            var email = identity?.FindFirst(ClaimTypes.Email)?.Value;

            var user = _unitOfWork.User.GetFirstOrDefault(m => m.Email == email);
            return user;
        }

        //---------------------- Volunteeer Profile --------------------------//


        [HttpGet]
        public IActionResult VolunteerProfile()
        {
            User user = GetThisUser();
            List<UserSkill> userSkill = _unitOfWork.UserSkill.GetAccToFilter(userSkill => userSkill.UserId == user.UserId);
            IEnumerable<Skill> skillList = _unitOfWork.Skill.GetAll();
            IEnumerable<Country> countryList = _unitOfWork.Country.GetAll();
            IEnumerable<City> cityList = _unitOfWork.City.GetAll();
            userProfileModel obj = new()
            {

                FirstName = user.FirstName,
                LastName = user.LastName,
                WhyIVolunteer = user.WhyIVolunteer,
                Avatar = user.Avatar,
                EmployeeId = user.EmployeeId,
                Department = user.Department,
                CityId = user.CityId,
                CountryId = user.CountryId,
                ProfileText = user.ProfileText,
                LinkedInUrl = user.LinkedInUrl,
                Title = user.Title,
                UserSkillList = userSkill,
                SkillsList = skillList,
                Countries = countryList,
                Cities = cityList,
            };
            return View(obj);
        }


        //update user details
        [HttpPost]
        public IActionResult VolunteerProfile(userProfileModel obj, List<long> finalSkillList)
        {
            User? user = GetThisUser();
            var idOfUserSkills = _unitOfWork.UserSkill.GetAccToFilter(userSkill => userSkill.UserId == user.UserId).Select(m => m.SkillId);
            if (user != null)
            {
                user.FirstName = obj.FirstName;
                user.LastName = obj.LastName;
                user.ProfileText = obj.ProfileText;
                user.CityId = obj.CityId;
                user.CountryId = obj.CountryId;
                user.EmployeeId = obj.EmployeeId;
                user.Department = obj.Department;
                user.Title = obj.Title;
                user.WhyIVolunteer = obj.WhyIVolunteer;
                user.LinkedInUrl = obj.LinkedInUrl;
                user.UpdatedAt = DateTime.Now;

                _unitOfWork.User.Update(user);

                if (finalSkillList.Any())
                {
                    var AddSkills = finalSkillList.Except(idOfUserSkills);
                    foreach (var skillId in AddSkills)
                    {
                        UserSkill addUserSkills = new()
                        {
                            UserId = user.UserId,
                            SkillId = skillId,
                        };
                        _unitOfWork.UserSkill.Add(addUserSkills);
                    }

                    var DeleteSkills = idOfUserSkills.Except(finalSkillList);
                    foreach (var skillid in DeleteSkills)
                    {
                        UserSkill deleteUserSkill = _unitOfWork.UserSkill.GetFirstOrDefault(userSkill => userSkill.SkillId == skillid);
                        _unitOfWork.UserSkill.Remove(deleteUserSkill);
                    }
                }
                _unitOfWork.Save();
            }
            return RedirectToAction("VolunteerProfile");
        }


        [HttpPost]
        public IActionResult ChangeAvatar(IFormFile avatar)
        {
            User user = GetThisUser();

            if (user.Avatar == null)
            {
                string imgExt = Path.GetExtension(avatar.Name);
                if (imgExt == ".jpg" || imgExt == ".png" || imgExt == ".jpeg")
                {
                    string ImageName = user.UserId + Path.GetExtension(avatar.Name);
                    var imgSaveTo = Path.Combine(_iweb.WebRootPath, "StoryImages", ImageName);
                    /*var stream = new FileStream(imgSaveTo, FileMode.Create);
                    avatar.CopyTo(stream);*/
                    using (FileStream stream = new(imgSaveTo, FileMode.Create))
                    {
                        avatar.CopyTo(stream);
                    }

                    user.Avatar = ImageName;

                    _unitOfWork.User.Add(user);
                }
            }
            else
            {
                if (user.UserId + Path.GetExtension(avatar.Name) != user.Avatar)
                {
                    string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/StoryImages/", user.Avatar);

                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }

                    string ImageName = user.UserId + Path.GetExtension(avatar.Name);
                    var imgSaveTo = Path.Combine(_iweb.WebRootPath, "StoryImages", ImageName);
                    /*var stream = new FileStream(imgSaveTo, FileMode.Create);
                    avatar.CopyTo(stream);*/
                    using (FileStream stream = new(imgSaveTo, FileMode.Create))
                    {
                        avatar.CopyTo(stream);
                    }

                    user.Avatar = ImageName;

                    _unitOfWork.User.Update(user);
                }

            }
            _unitOfWork.Save();
            return View();
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
