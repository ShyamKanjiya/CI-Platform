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

        #region Volunteer Profile

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
                UserDetails = user
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
                obj.UserDetails = user;
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


            string imgExt = Path.GetExtension(avatar.FileName);
            if (imgExt == ".jpg" || imgExt == ".png" || imgExt == ".jpeg")
            {
                if (user.Avatar != null)
                {
                    string ImageName = user.UserId + Path.GetFileName(avatar.FileName);
                    var imgSaveTo = Path.Combine(_iweb.WebRootPath, "/Avatars/", ImageName);
                    string finalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/" + imgSaveTo);

                    if (!imgSaveTo.Equals(user.Avatar))
                    {
                        string alrExists = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/" + user.Avatar);
                        if (System.IO.File.Exists(alrExists))
                        {
                            System.IO.File.Delete(alrExists);
                        }

                        using (FileStream stream = new(finalPath, FileMode.Create))
                        {
                            avatar.CopyTo(stream);
                        }
                        user.Avatar = imgSaveTo;
                        user.UpdatedAt = DateTime.Now;
                        _unitOfWork.User.Update(user);
                    }
                }
                else
                {
                    string ImageName = user.UserId + Path.GetFileName(avatar.FileName);
                    var imgSaveTo = Path.Combine(_iweb.WebRootPath, "/Avatars/", ImageName);
                    string finalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/" + imgSaveTo);

                    using (FileStream stream = new(finalPath, FileMode.Create))
                    {
                        avatar.CopyTo(stream);
                    }
                    user.Avatar = imgSaveTo;
                    user.UpdatedAt = DateTime.Now;
                    _unitOfWork.User.Update(user);
                }
            }
            _unitOfWork.Save();
            return RedirectToAction("VolunteerProfile");
        }


        [HttpPost]
        public IActionResult ChangePassword(userProfileModel obj)
        {
            User user = GetThisUser();

            if (user != null && obj.OldPassword != null && obj.NewPassword != null)
            {
                if (user.Password == obj.OldPassword)
                {
                    user.Password = obj.NewPassword;
                    user.UpdatedAt = DateTime.Now;
                    _unitOfWork.User.Update(user);
                    _unitOfWork.Save();

                    return Json(1);
                }
            }
            return Json(0);
        }


        public JsonResult FilterCity(long countryId)
        {
            User user = GetThisUser();
            var cityId = user.CityId;
            IEnumerable<City> cityList = _unitOfWork.City.GetAccToFilter(m => m.CountryId == countryId);
            return new JsonResult(new { CityId = cityId, Cities = cityList });
        }

        #endregion

        //---------------------- Volunteer Timesheet --------------------------//

        #region Volunteer Timesheet
        public IActionResult VolunteerTimesheet()
        {
            User user = GetThisUser();

            IEnumerable<MissionApplication> draftMissAppListForTime = _unitOfWork.MissionApplication.GetAccToFilter(m => m.UserId == user.UserId && m.ApprovalStatus == "APPROVE" && m.Mission.MissionType == "TIME");
            IEnumerable<MissionApplication> draftMissAppListForGoal = _unitOfWork.MissionApplication.GetAccToFilter(m => m.UserId == user.UserId && m.ApprovalStatus == "APPROVE" && m.Mission.MissionType == "GOAL");
            IEnumerable<Timesheet> dataOfTimeBasedMission = _unitOfWork.Timesheet.GetTimeSheetData(timeData => timeData.Mission.MissionType == "Time" && timeData.DeletedAt == null);
            IEnumerable<Timesheet> dataOfGoalBasedMission = _unitOfWork.Timesheet.GetTimeSheetData(goalData => goalData.Mission.MissionType == "Goal" && goalData.DeletedAt == null);

            userVolunteerTimesheetModel obj = new();
            obj.UserDetails = user;
            obj.MissionApplicationForTime = draftMissAppListForTime;
            obj.MissionApplicationForGoal = draftMissAppListForGoal;
            obj.TimesheetsForTime = dataOfTimeBasedMission;
            obj.TimesheetsForGoal = dataOfGoalBasedMission;

            obj.Missions = _unitOfWork.Mission.GetAll();
            obj.Cities = _unitOfWork.City.GetAll();

            return View(obj);
        }

        public bool CheckEnterdTime(int Minutes, int Hours)
        {
            if(Hours == 0 && Minutes == 0)
            {
                return false;
            }
            return true;
        }

        [HttpPost]
        public IActionResult ChangeInTimesheet(userVolunteerTimesheetModel obj)
        {
            User user = GetThisUser();
            var hours = obj.Hours;
            var minutes = obj.Minutes;
            TimeSpan time = new(hours, minutes, 0);

            if (obj.TimeSheetId == 0)
            {
                Timesheet data = new()
                {
                    UserId = user.UserId,
                    MissionId = obj.MissionId,
                    Time = time,
                    Action = obj.Action,
                    DateVolunteered = obj.DateVolunteered,
                    Notes = obj.Notes,
                    Status = "SUBMIT_FOR_APPROVAL",
                };
                _unitOfWork.Timesheet.Add(data);
                _unitOfWork.Save();
                TempData["success"] = "Data added successfully!";
                return RedirectToAction("VolunteerTimesheet");
            }
            else
            {
                Timesheet updatedData = _unitOfWork.Timesheet.GetFirstOrDefault(timeSheetData => timeSheetData.TimesheetId == obj.TimeSheetId);
                if (updatedData != null)
                {
                    updatedData.Action = obj.Action;
                    updatedData.Time = time;
                    updatedData.Notes = obj.Notes;
                    updatedData.DateVolunteered = obj.DateVolunteered;
                    updatedData.UpdatedAt = DateTime.Now;

                    _unitOfWork.Timesheet.Update(updatedData);
                    _unitOfWork.Save();

                    TempData["success"] = "Data updated successfully!";
                    return RedirectToAction("VolunteerTimesheet");
                }
            }

            TempData["error"] = "Something went wrong!";
            return RedirectToAction("VolunteerTimesheet");
        }


        public IActionResult GetTimeSheetData(long timesheetId)
        {
            Timesheet timesheetData = _unitOfWork.Timesheet.GetFirstOrDefault(m => m.TimesheetId == timesheetId);
            return Json(timesheetData);
        }

        [HttpPost]
        public IActionResult DeleteTimeSheetData(long timesheetId)
        {
            if (timesheetId > 0)
            {
                Timesheet deletingData = _unitOfWork.Timesheet.GetFirstOrDefault(m => m.TimesheetId == timesheetId);
                deletingData.DeletedAt = DateTime.Now;
                _unitOfWork.Timesheet.Update(deletingData);
                _unitOfWork.Save();

                return RedirectToAction("VolunteerTimesheet");
            }

            TempData["error"] = "Opps! something went wrong";
            return RedirectToAction("VolunteerTimesheet");
        }

        [HttpGet]
        public IActionResult getDate(long missionId)
        {
            var mission = _unitOfWork.Mission.GetAccToFilter(m => m.MissionId == missionId);
            return Json(mission);
        }

        #endregion

        //---------------------- Volunteer Policy --------------------------//

        #region Volunteer Policy
        public IActionResult VolunteerPolicy()
        {
            userProfileModel userProfile = new()
            {
                UserDetails = GetThisUser()
            };
            return View(userProfile);
        }

        #endregion

        //---------------------- Contect Us --------------------------//

        #region Contect Us

        [HttpGet]
        public userViewModel ContectUs(userViewModel userView)
        {
            userView.UserDetails = GetThisUser();
            return userView;
        }

        [HttpPost]
        public void ContectUs(string subject, string message)
        {
            User user = GetThisUser();
            ContectUs contectUs = new()
            {
                UserId = user.UserId,
                Subject = subject,
                Message = message
            };

            _unitOfWork.ContectUs.Add(contectUs);
            _unitOfWork.Save();
        }

        #endregion
    }
}
