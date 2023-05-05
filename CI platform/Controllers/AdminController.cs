using CI_platform.Entities.DataModels;
using CI_platform.Entities.ViewModels;
using CI_platform.Repositories.GenericRepository.Interface;
using CI_platform.Repositories.MethodRepository.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Xml.Serialization;

namespace CI_platform.Controllers
{
    [Authorize(Roles = "ADMIN")]
    public class AdminController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStoryMethodRepository _repo;
        private readonly IWebHostEnvironment _iweb;
        public AdminController(IUnitOfWork unitOfWork, IStoryMethodRepository storyMethodRepository, IWebHostEnvironment iweb)
        {
            _unitOfWork = unitOfWork;
            _repo = storyMethodRepository;
            _iweb = iweb;
        }

        public User GetThisUser()
        {
            var identity = User.Identity as ClaimsIdentity;
            var email = identity?.FindFirst(ClaimTypes.Email)?.Value;

            var user = _unitOfWork.User.GetFirstOrDefault(m => m.Email == email);
            return user;
        }

        //---------------------- User --------------------------//

        #region User
        //User lists

        [HttpGet]
        public IActionResult AdminUserDetails()
        {
            User user = GetThisUser();
            IEnumerable<User> userLists = _unitOfWork.User.GetAccToFilter(m => m.DeletedAt == null);
            adminUserDetails obj = new();
            obj.UserLists = userLists;
            obj.UserDetails = user;
            return View(obj);
        }

        #region Add 
        [HttpPost]
        public JsonResult CascadeCity(long countryId)
        {
            IEnumerable<City> cityList = _unitOfWork.City.GetAccToFilter(cities => cities.CountryId == countryId);
            return new JsonResult(cityList);
        }

        [HttpGet]
        public IActionResult AdminAddUser()
        {
            User user = GetThisUser();
            IEnumerable<Country> Countries = _unitOfWork.Country.GetAll();
            adminUserDetails obj = new()
            {
                UserDetails = user,
                CountryList = Countries
            };
            return View(obj);
        }

        [HttpPost]
        public IActionResult AdminAddUser(adminUserDetails obj, IFormFile userAvatar)
        {
            User? status = _unitOfWork.User.GetFirstOrDefault(m => m.Email.ToLower() == obj.Email.Trim().ToLower());

            if (status == null)
            {
                User user = new()
                {
                    FirstName = obj.FirstName,
                    LastName = obj.LastName,
                    PhoneNumber = obj.PhoneNumber,
                    Email = obj.Email,
                    Password = obj.Password,
                    CountryId = obj.CountryId,
                    CityId = obj.CityId,
                    EmployeeId = obj.EmployeeId,
                    Department = obj.Department,
                    Status = 1
                };
                _unitOfWork.User.Add(user);
                _unitOfWork.Save();

                if (userAvatar != null)
                {
                    User userForAvatar = _unitOfWork.User.GetFirstOrDefault(m => m.Email.Trim().ToLower() == obj.Email.Trim().ToLower());
                    string imgExt = Path.GetExtension(userAvatar.FileName);
                    if (imgExt == ".jpg" || imgExt == ".png" || imgExt == ".jpeg")
                    {
                        string ImageName = user.UserId + Path.GetFileName(userAvatar.FileName);
                        var imgSaveTo = Path.Combine(_iweb.WebRootPath, "/Avatars/", ImageName);
                        string finalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/" + imgSaveTo);

                        using (FileStream stream = new(finalPath, FileMode.Create))
                        {
                            userAvatar.CopyTo(stream);
                        }
                        user.Avatar = imgSaveTo;
                        user.UpdatedAt = DateTime.Now;
                        _unitOfWork.User.Update(user);
                        _unitOfWork.Save();
                    }
                }
                TempData["success"] = "User added successfully!";
                return RedirectToAction("AdminUserDetails");

            }
            TempData["error"] = "Email Already Exists!";
            IEnumerable<Country> countryList = _unitOfWork.Country.GetAll();
            obj.CountryList = countryList;
            return View(obj);
        }
        #endregion

        [HttpPost]
        public IActionResult DeleteUserData(long userId)
        {
            if (userId > 0)
            {
                User deletingData = _unitOfWork.User.GetFirstOrDefault(m => m.UserId == userId);
                if (deletingData != null)
                {
                    deletingData.DeletedAt = DateTime.Now;
                    _unitOfWork.User.Update(deletingData);
                    
                    string alrExists = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/" + deletingData.Avatar);
                    System.IO.File.Delete(alrExists);
                    _unitOfWork.User.Update(deletingData);
                }

                var deletingComments = _unitOfWork.Comment.GetAccToFilter(m => m.UserId == userId);
                if (deletingComments != null)
                {
                    foreach (var comment in deletingComments)
                    {
                        comment.DeletedAt = DateTime.Now;
                        _unitOfWork.Comment.Update(comment);
                    }
                }

                var deletingContectUs = _unitOfWork.ContectUs.GetAccToFilter(m => m.UserId == userId);
                if (deletingContectUs != null)
                {
                    foreach (var contact in deletingContectUs)
                    {
                        contact.DeletedAt = DateTime.Now;
                        _unitOfWork.ContectUs.Update(contact);
                    }
                }

                var favrouiteMission = _unitOfWork.FavouriteMission.GetAccToFilter(m => m.UserId == userId);
                if (favrouiteMission != null)
                {
                    foreach (var mission in favrouiteMission)
                    {
                        mission.DeletedAt = DateTime.Now;
                        _unitOfWork.FavouriteMission.Update(mission);
                    }
                }

                var deletingMissionApplication = _unitOfWork.MissionApplication.GetAccToFilter(m => m.UserId == userId);
                if (deletingMissionApplication != null)
                {
                    foreach (var app in deletingMissionApplication)
                    {
                        app.ApprovalStatus = "DECLINE";
                        app.DeletedAt = DateTime.Now;
                        _unitOfWork.MissionApplication.Update(app);
                    }
                }

                var deletingMissionInviteFrom = _unitOfWork.MissionInvite.GetAccToFilter(m => m.FromUserId == userId);
                if (deletingMissionInviteFrom != null)
                {
                    foreach (var inviteFrom in deletingMissionInviteFrom)
                    {
                        inviteFrom.DeletedAt = DateTime.Now;
                        _unitOfWork.MissionInvite.Update(inviteFrom);
                    }
                }

                var deletingMissionInviteTo = _unitOfWork.MissionInvite.GetAccToFilter(m => m.ToUserId == userId);
                if (deletingMissionInviteTo != null)
                {
                    foreach (var inviteTo in deletingMissionInviteTo)
                    {
                        inviteTo.DeletedAt = DateTime.Now;
                        _unitOfWork.MissionInvite.Update(inviteTo);
                    }
                }

                var deletingRating = _unitOfWork.MissionRating.GetAccToFilter(m => m.UserId == userId);
                if (deletingRating != null)
                {
                    foreach (var rate in deletingRating)
                    {
                        rate.DeletedAt = DateTime.Now;
                        _unitOfWork.MissionRating.Update(rate);
                    }
                }

                var deletingRelatedStory = _unitOfWork.Story.GetAccToFilter(m => m.UserId == userId);
                if (deletingRelatedStory != null)
                {
                    foreach (var story in deletingRelatedStory)
                    {
                        story.DeletedAt = DateTime.Now;
                        _unitOfWork.Story.Update(story);

                        var deletingStoryInvite = _unitOfWork.StoryInvite.GetFirstOrDefault(m => m.StoryId == story.StoryId);

                        if (deletingStoryInvite != null)
                        {
                            deletingStoryInvite.DeletedAt = DateTime.Now;
                            _unitOfWork.StoryInvite.Update(deletingStoryInvite);
                        }

                        var deletingStoryMedia = _unitOfWork.StoryMedia.GetAccToFilter(m => m.StoryId == story.StoryId);

                        if (deletingStoryMedia != null)
                        {
                            foreach (var media in deletingStoryMedia)
                            {
                                if (media.Type != "video")
                                {
                                    string alrExists = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/StoryImages/" + media.Path);
                                    System.IO.File.Delete(alrExists);
                                }

                                media.DeletedAt = DateTime.Now;
                                _unitOfWork.StoryMedia.Update(media);
                            }
                        }
                    }
                }

                var deletingTimesheet = _unitOfWork.Timesheet.GetAccToFilter(m => m.UserId == userId);
                if (deletingTimesheet != null)
                {
                    foreach (var del in deletingTimesheet)
                    {
                        del.DeletedAt = DateTime.Now;
                        _unitOfWork.Timesheet.Update(del);
                    }
                }

                var deletingUserSkills = _unitOfWork.UserSkill.GetAccToFilter(m => m.UserId == userId);
                if (deletingUserSkills != null)
                {
                    foreach (var del in deletingUserSkills)
                    {
                        del.DeletedAt = DateTime.Now;
                        _unitOfWork.UserSkill.Update(del);
                    }
                }

                _unitOfWork.Save();
                return RedirectToAction("AdminUserDetails");
            }

            TempData["error"] = "Opps! something went wrong";
            return RedirectToAction("AdminUserDetails");
        }

        #region Edit

        [HttpPost]
        public JsonResult CascadeCityForEdit(long countryId, int userId)
        {
            User user = new();
            long cityId = 0;
            if (userId > 0)
            {
                user = _unitOfWork.User.GetFirstOrDefault(user => user.UserId == userId);
                if (user != null)
                {
                    cityId = user.CityId;
                }
            }
            IEnumerable<City> cityList = _unitOfWork.City.GetAccToFilter(city => city.CountryId == countryId);
            return new JsonResult(new { CityId = cityId, Cities = cityList });
        }

        [HttpGet]
        public IActionResult AdminEditUser(long userId)
        {
            User userDetail = GetThisUser();
            IEnumerable<Country> countryList = _unitOfWork.Country.GetAll();
            adminUserDetails obj = new();
            if (userId > 0)
            {
                User user = _unitOfWork.User.GetFirstOrDefault(m => m.UserId == userId);
                if (User != null)
                {
                    obj.UserDetails = userDetail;
                    obj.UserId = user.UserId;
                    obj.Avatar = user.Avatar;
                    obj.FirstName = user.FirstName;
                    obj.LastName = user.LastName;
                    obj.PhoneNumber = user.PhoneNumber;
                    obj.Email = user.Email;
                    obj.EmployeeId = user.EmployeeId;
                    obj.Department = user.Department;
                    obj.CityId = user.CityId;
                    obj.CountryId = user.CountryId;
                    obj.Status = (int)user.Status;
                }
            }
            obj.CountryList = countryList;
            return View(obj);
        }

        [HttpPost]
        public IActionResult AdminEditUser(adminUserDetails obj, IFormFile userAvatar)
        {
            if (obj.UserId > 0)
            {
                User user = _unitOfWork.User.GetFirstOrDefault(m => m.UserId == obj.UserId);
                user.FirstName = obj.FirstName;
                user.LastName = obj.LastName;
                user.PhoneNumber = obj.PhoneNumber;
                user.Email = obj.Email;
                user.EmployeeId = obj.EmployeeId;
                user.Department = obj.Department;
                user.CityId = obj.CityId;
                user.CountryId = obj.CountryId;
                user.Status = obj.Status;

                if (user != null && userAvatar != null)
                {
                    string imgExt = Path.GetExtension(userAvatar.FileName);
                    if (imgExt == ".jpg" || imgExt == ".png" || imgExt == ".jpeg")
                    {
                        if (user.Avatar != null)
                        {
                            string ImageName = user.UserId + Path.GetFileName(userAvatar.FileName);
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
                                    userAvatar.CopyTo(stream);
                                }
                                user.Avatar = imgSaveTo;
                                user.UpdatedAt = DateTime.Now;
                                _unitOfWork.User.Update(user);
                            }
                        }
                        else
                        {
                            string ImageName = user.UserId + Path.GetFileName(userAvatar.FileName);
                            var imgSaveTo = Path.Combine(_iweb.WebRootPath, "/Avatars/", ImageName);
                            string finalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/" + imgSaveTo);

                            using (FileStream stream = new(finalPath, FileMode.Create))
                            {
                                userAvatar.CopyTo(stream);
                            }
                            user.Avatar = imgSaveTo;
                            user.UpdatedAt = DateTime.Now;
                            _unitOfWork.User.Update(user);
                        }
                    }
                }

                _unitOfWork.User.Update(user);
                _unitOfWork.Save();
                TempData["success"] = "User edited successfully!";
                return RedirectToAction("AdminUserDetails");
            }
            TempData["error"] = "Something went wrong";
            return View(obj);
        }

        #endregion

        #endregion

        //---------------------- CMS --------------------------//

        #region CMS
        //CMS lists
        public IActionResult AdminCMSPage()
        {
            User user = GetThisUser();
            IEnumerable<CmsPage> cmsLists = _unitOfWork.CMSPage.GetAccToFilter(m => m.DeletedAt == null);
            adminCMSPageDetails obj = new()
            {
                CMSLists = cmsLists,
                UserDetails = user
            };
            return View(obj);
        }

        [HttpPost]
        public IActionResult AddAndUpdateCMSPage(adminCMSPageDetails obj)
        {
            if (obj.CMSId == 0)
            {
                CmsPage data = new()
                {
                    Title = obj.CMSTitle,
                    Description = obj.CMSDescription,
                    Slug = obj.CMSSlug,
                    Status = 1
                };

                _unitOfWork.CMSPage.Add(data);
                _unitOfWork.Save();

                TempData["success"] = "Data added successfully!";
                return RedirectToAction("AdminCMSPage");
            }
            else
            {
                CmsPage updatedData = _unitOfWork.CMSPage.GetFirstOrDefault(m => m.CmsPageId == obj.CMSId);
                if (updatedData != null)
                {
                    updatedData.CmsPageId = obj.CMSId;
                    updatedData.Title = obj.CMSTitle;
                    updatedData.Description = obj.CMSDescription;
                    updatedData.Slug = obj.CMSSlug;
                    updatedData.Status = (byte)obj.Status;
                    updatedData.UpdatedAt = DateTime.Now;
                    _unitOfWork.CMSPage.Update(updatedData);
                    _unitOfWork.Save();

                    TempData["success"] = "Data updated successfully!";
                    return RedirectToAction("AdminCMSPage");
                }
            }

            TempData["error"] = "Something went wrong!";
            return RedirectToAction("AdminCMSPage");
        }

        [HttpPost]
        public IActionResult GetCMSData(long CMSId)
        {
            CmsPage cmsPageData = _unitOfWork.CMSPage.GetFirstOrDefault(m => m.CmsPageId == CMSId);
            return Json(cmsPageData);
        }

        [HttpPost]
        public IActionResult DeleteCMSData(long CMSId)
        {
            if (CMSId > 0)
            {
                CmsPage deletingData = _unitOfWork.CMSPage.GetFirstOrDefault(m => m.CmsPageId == CMSId);
                deletingData.DeletedAt = DateTime.Now;
                _unitOfWork.CMSPage.Update(deletingData);
                _unitOfWork.Save();

                return RedirectToAction("AdminCMSPage");
            }

            TempData["error"] = "Opps! something went wrong";
            return RedirectToAction("AdminCMSPage");
        }
        #endregion

        //---------------------- Mission --------------------------//

        #region Mission
        //Mission lists
        public IActionResult AdminMissionDetails()
        {
            User user = GetThisUser();
            IEnumerable<Mission> missionLists = _unitOfWork.Mission.GetAccToFilter(m => m.DeletedAt == null);
            adminMissionDetails obj = new()
            {
                MissionLists = missionLists,
                UserDetails = user
            };
            return View(obj);
        }

        #region Add
        [HttpGet]
        public IActionResult AdminAddMission()
        {
            User user = GetThisUser();
            IEnumerable<Country> Countries = _unitOfWork.Country.GetAll();
            IEnumerable<MissionTheme> themeList = _unitOfWork.MissionTheme.GetAll();
            IEnumerable<Skill> skillList = _unitOfWork.Skill.GetAll();
            adminMissionDetails obj = new()
            {
                UserDetails = user,
                CountryList = Countries,
                ThemeList = themeList,
                SkillList = skillList,
            };
            return View(obj);
        }

        public JsonResult AdminAddMissCityCascade(long countryId)
        {
            IEnumerable<City> cityList = _unitOfWork.City.GetAccToFilter(cities => cities.CountryId == countryId);
            return new JsonResult(cityList);
        }

        [HttpPost]
        public IActionResult AdminAddMission(adminMissionDetails obj, List<long> finalMissSkillList, List<IFormFile> MissionDocFiles, List<IFormFile> MissionImageFiles, string[] preloadedmissimage, string[] preloadedmissdocs)
        {
            if (obj != null)
            {
                Mission mission = new()
                {
                    Title = obj.Title,
                    ShortDescription = obj.ShortDescription,
                    Description = obj.Description,
                    CityId = obj.CityId,
                    CountryId = obj.CountryId,
                    OrganizationName = obj.OrganizationName,
                    OrganizationDetail = obj.OrganizationDetail,
                    StartDate = obj.StartDate,
                    EndDate = obj.EndDate,
                    MissionType = obj.MissionType,
                    MissionThemeId = obj.MissionThemeId,
                    Seats = obj.TotalSeats,
                    Deadline = obj.MissionDeadline,
                    Availability = obj.Availability,
                };

                _unitOfWork.Mission.Add(mission);
                _unitOfWork.Save();
                long addedMissionId = mission.MissionId;
                IEnumerable<MissionMedium> missionMediaList = _unitOfWork.MissionMedia.GetAccToFilter(media => media.MissionId == addedMissionId);
                IEnumerable<MissionDocument> missionDocumentList = _unitOfWork.MissionDocument.GetAccToFilter(media => media.MissionId == addedMissionId);
                SaveMissionMediaAndDocs(addedMissionId, obj.VideoUrl, missionMediaList, missionDocumentList, MissionDocFiles, MissionImageFiles, preloadedmissdocs, preloadedmissimage);
                var MissionSkillsId = _unitOfWork.MissionSkills.GetAccToFilter(missionSkill => missionSkill.MissionId == addedMissionId).Select(missionSkill => missionSkill.MissionId);

                AddMissionSkills(addedMissionId, MissionSkillsId, finalMissSkillList);

                return RedirectToAction("AdminMissionDetails");
            }
            return View(obj);
        }
        #endregion

        [HttpPost]
        public IActionResult DeleteMissionData(long missionId)
        {
            if (missionId > 0)
            {
                Mission deletingData = _unitOfWork.Mission.GetFirstOrDefault(m => m.MissionId == missionId);
                if (deletingData != null)
                {
                    deletingData.DeletedAt = DateTime.Now;
                    _unitOfWork.Mission.Update(deletingData);
                }

                var deletingComments = _unitOfWork.Comment.GetAccToFilter(m => m.MissionId == missionId);
                if (deletingComments != null)
                {
                    foreach (var comment in deletingComments)
                    {
                        comment.DeletedAt = DateTime.Now;
                        _unitOfWork.Comment.Update(comment);
                    }
                }

                var favrouiteMission = _unitOfWork.FavouriteMission.GetAccToFilter(m => m.MissionId == missionId);
                if (favrouiteMission != null)
                {
                    foreach (var mission in favrouiteMission)
                    {
                        mission.DeletedAt = DateTime.Now;
                        _unitOfWork.FavouriteMission.Update(mission);
                    }
                }

                var goalMission = _unitOfWork.GoalMission.GetFirstOrDefault(m => m.MissionId == missionId);
                if (goalMission != null)
                {
                    goalMission.DeletedAt = DateTime.Now;
                    _unitOfWork.GoalMission.Update(goalMission);
                }

                var missionDoc = _unitOfWork.MissionDocument.GetAccToFilter(m => m.MissionId == missionId);
                if(missionDoc != null)
                {
                    foreach(var doc in missionDoc)
                    {
                        string alrExists = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/MissionDocuments/" + doc.DocumentPath);
                        System.IO.File.Delete(alrExists);

                        doc.DeletedAt = DateTime.Now;
                        _unitOfWork.MissionDocument.Update(doc);
                    }
                }

                var missionMedia = _unitOfWork.MissionMedia.GetAccToFilter(m => m.MissionId == missionId);
                if (missionMedia != null)
                {
                    foreach (var media in missionMedia)
                    {
                        if(media.MediaType != "video")
                        {
                            string alrExists = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/MissionMedia/" + media.MediaPath);
                            System.IO.File.Delete(alrExists); 
                        }

                        media.DeletedAt = DateTime.Now;
                        _unitOfWork.MissionMedia.Update(media);
                    }
                }

                var inviteMission = _unitOfWork.MissionInvite.GetAccToFilter(m => m.MissionId == missionId);
                if (inviteMission != null)
                {
                    foreach (var mission in inviteMission)
                    {
                        mission.DeletedAt = DateTime.Now;
                        _unitOfWork.MissionInvite.Update(mission);
                    }
                }

                var deletingRating = _unitOfWork.MissionRating.GetAccToFilter(m => m.MissionId == missionId);
                if (deletingRating != null)
                {
                    foreach (var rate in deletingRating)
                    {
                        rate.DeletedAt = DateTime.Now;
                        _unitOfWork.MissionRating.Update(rate);
                    }
                }

                var deletingSkill = _unitOfWork.MissionSkills.GetAccToFilter(m => m.MissionId == missionId);
                if (deletingSkill != null)
                {
                    foreach (var del in deletingSkill)
                    {
                        del.DeletedAt = DateTime.Now;
                        _unitOfWork.MissionSkills.Update(del);
                    }
                }

                var deletingMissionApplication = _unitOfWork.MissionApplication.GetAccToFilter(m => m.MissionId == missionId);
                if (deletingMissionApplication != null)
                {
                    foreach (var app in deletingMissionApplication)
                    {
                        app.ApprovalStatus = "DECLINE";
                        app.DeletedAt = DateTime.Now;
                        _unitOfWork.MissionApplication.Update(app);
                    }
                }

                var deletingRelatedStory = _unitOfWork.Story.GetAccToFilter(m => m.MissionId == missionId);
                if(deletingRelatedStory != null)
                {
                    foreach (var story in deletingRelatedStory)
                    {
                        story.DeletedAt = DateTime.Now;
                        _unitOfWork.Story.Update(story);

                        var deletingStoryInvite = _unitOfWork.StoryInvite.GetFirstOrDefault(m => m.StoryId == story.StoryId);

                        if (deletingStoryInvite != null)
                        {
                            deletingStoryInvite.DeletedAt = DateTime.Now;
                            _unitOfWork.StoryInvite.Update(deletingStoryInvite);
                        }

                        var deletingStoryMedia = _unitOfWork.StoryMedia.GetAccToFilter(m => m.StoryId == story.StoryId);

                        if (deletingStoryMedia != null)
                        {
                            foreach (var media in deletingStoryMedia)
                            {
                                if (media.Type != "video")
                                {
                                    string alrExists = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/StoryImages/" + media.Path);
                                    System.IO.File.Delete(alrExists);
                                }

                                media.DeletedAt = DateTime.Now;
                                _unitOfWork.StoryMedia.Update(media);
                            }
                        }
                    }
                }

                var deletingTimesheet = _unitOfWork.Timesheet.GetAccToFilter(m => m.MissionId == missionId);
                if (deletingTimesheet != null)
                {
                    foreach (var del in deletingTimesheet)
                    {
                        del.DeletedAt = DateTime.Now;
                        _unitOfWork.Timesheet.Update(del);
                    }
                }


                _unitOfWork.Save();
                return RedirectToAction("AdminMissionDetails");
            }

            TempData["error"] = "Opps! something went wrong";
            return RedirectToAction("AdminMissionDetails");
        }

        #region Edit
        [HttpGet]
        public IActionResult AdminEditMission(long missionId)
        {
            if (missionId > 0)
            {
                User user = GetThisUser();
                Mission thisMissionDetails = _unitOfWork.Mission.GetFirstOrDefault(mission => mission.MissionId == missionId);
                if (thisMissionDetails != null)
                {
                    IEnumerable<MissionSkill> missionSkillList = _unitOfWork.MissionSkills.GetAccToFilter(missionSkills => missionSkills.MissionId == missionId);
                    IEnumerable<MissionMedium> missionMediaList = _unitOfWork.MissionMedia.GetAccToFilter(missionMedia => missionMedia.MissionId == missionId);
                    IEnumerable<MissionDocument> missionDocList = _unitOfWork.MissionDocument.GetAccToFilter(missionDoc => missionDoc.MissionId == missionId);
                    IEnumerable<Country> countryList = _unitOfWork.Country.GetAll();
                    IEnumerable<Skill> skillList = _unitOfWork.Skill.GetAll();
                    IEnumerable<MissionTheme> themeList = _unitOfWork.MissionTheme.GetAll();
                    adminMissionDetails obj = new()
                    {
                        MissionId = thisMissionDetails.MissionId,
                        Title = thisMissionDetails.Title,
                        ShortDescription = thisMissionDetails.ShortDescription,
                        Description = thisMissionDetails.Description,
                        CountryId = thisMissionDetails.CountryId,
                        CityId = thisMissionDetails.CityId,
                        OrganizationName = thisMissionDetails.OrganizationName,
                        OrganizationDetail = thisMissionDetails.OrganizationDetail,
                        StartDate = thisMissionDetails.StartDate,
                        EndDate = thisMissionDetails.EndDate,
                        MissionType = thisMissionDetails.MissionType,
                        TotalSeats = thisMissionDetails.Seats,
                        MissionDeadline = thisMissionDetails.Deadline,
                        MissionThemeId = thisMissionDetails.MissionThemeId,
                        Availability = thisMissionDetails.Availability,
                        UserDetails = user
                    };

                    obj.CountryList = countryList;
                    obj.ThemeList = themeList;
                    obj.SkillList = skillList;
                    obj.MissionSkillList = missionSkillList;
                    obj.MissionMediumList = missionMediaList;
                    obj.MissionDocumentList = missionDocList;

                    return View(obj);
                }
            }

            return RedirectToAction("AdminMissionDetails");
        }

        //Edit mission post method 
        [HttpPost]
        public IActionResult AdminEditMission(adminMissionDetails obj, List<long> finalMissSkillList, List<IFormFile> MissionDocFiles, List<IFormFile> MissionImageFiles, string[] preloadedmissimage, string[] preloadedmissdocs)
        {
            if (obj != null)
            {
                if (obj?.MissionId != null)
                {
                    Mission editThisMission = _unitOfWork.Mission.GetFirstOrDefault(mission => mission.MissionId == obj.MissionId);
                    if (editThisMission != null)
                    {
                        editThisMission.Title = obj.Title;
                        editThisMission.ShortDescription = obj.ShortDescription;
                        editThisMission.Description = obj.Description;
                        editThisMission.CountryId = obj.CountryId;
                        editThisMission.CityId = obj.CityId;
                        editThisMission.OrganizationName = obj.OrganizationName;
                        editThisMission.OrganizationDetail = obj.OrganizationDetail;
                        editThisMission.StartDate = obj.StartDate;
                        editThisMission.EndDate = obj.EndDate;
                        editThisMission.MissionType = obj.MissionType;
                        editThisMission.Seats = obj.TotalSeats;
                        editThisMission.Deadline = obj.MissionDeadline;
                        editThisMission.MissionThemeId = obj.MissionThemeId;
                        editThisMission.Availability = obj.Availability;

                        _unitOfWork.Mission.Update(editThisMission);

                        IEnumerable<MissionMedium> missionMediaList = _unitOfWork.MissionMedia.GetAccToFilter(media => media.MissionId == editThisMission.MissionId);
                        IEnumerable<MissionDocument> missionDocumentList = _unitOfWork.MissionDocument.GetAccToFilter(media => media.MissionId == editThisMission.MissionId);
                        SaveMissionMediaAndDocs(editThisMission.MissionId, obj.VideoUrl, missionMediaList, missionDocumentList, MissionDocFiles, MissionImageFiles, preloadedmissdocs, preloadedmissimage);
                        var MissionSkillsId = _unitOfWork.MissionSkills.GetAccToFilter(missionSkill => missionSkill.MissionId == obj.MissionId).Select(missionSkill => missionSkill.SkillId);

                        AddMissionSkills(obj.MissionId, MissionSkillsId, finalMissSkillList);

                        _unitOfWork.Save();
                        return RedirectToAction("AdminMissionDetails");

                    }
                }
            }
            return View(obj);
        }

        //cascade city for mission edit get method
        public JsonResult AdminEditMissCasCity(long countryId, long missionId)
        {
            Mission mission = new();
            long cityId = 0;
            if (missionId > 0)
            {
                mission = _unitOfWork.Mission.GetFirstOrDefault(mission => mission.MissionId == missionId);
                if (mission != null)
                {
                    cityId = mission.CityId;
                }
            }
            IEnumerable<City> cityList = _unitOfWork.City.GetAccToFilter(city => city.CountryId == countryId);
            return new JsonResult(new { CityId = cityId, Cities = cityList });
        }
        #endregion

        #region Media,docs,skills
        [HttpPost]
        public void SaveMissionMediaAndDocs(long addedMissionId, string? videoUrl, IEnumerable<MissionMedium> missionMediaList, IEnumerable<MissionDocument> missionDocumentList, List<IFormFile> missionDocFiles, List<IFormFile> missionImageFiles, string[] preloadedmissdocs, string[] preloadedmissimage)
        {
            if (videoUrl != null)
            {
                foreach (var video in missionMediaList)
                {
                    if (video != null && video.MediaType == "video")
                    {
                        _unitOfWork.MissionMedia.Remove(video);
                    }
                }
                MissionMedium missMedForVid = new()
                {
                    MissionId = addedMissionId,
                    MediaName = "youtube viedo",
                    MediaType = "video",
                    MediaPath = videoUrl,
                };
                _unitOfWork.MissionMedia.Add(missMedForVid);
            }

            //for images
            foreach (var missionMedia in missionMediaList)
            {
                if (missionMedia.MediaType != "video")
                {
                    if (preloadedmissimage.Length < 1)
                    {
                        string missImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/MissionMedia/", missionMedia.MediaPath);

                        if (System.IO.File.Exists(missImagePath))
                        {
                            System.IO.File.Delete(missImagePath);
                        }

                        _unitOfWork.MissionMedia.Remove(missionMedia);
                    }
                    else
                    {
                        bool flag = false;
                        for (int i = 0; i < preloadedmissimage.Length; i++)
                        {
                            string imgName = preloadedmissimage[i][14..];

                            if (imgName.Equals(missionMedia.MediaPath))
                            {
                                flag = true;
                            }
                        }
                        if (!flag)
                        {
                            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/MissionMedia/", missionMedia.MediaPath);

                            if (System.IO.File.Exists(imagePath))
                            {
                                System.IO.File.Delete(imagePath);
                            }

                            _unitOfWork.MissionMedia.Remove(missionMedia);
                        }
                    }
                }
            }

            if (missionImageFiles?.Count > 0)
            {
                foreach (var image in missionImageFiles)
                {
                    string imgExt = Path.GetExtension(image.FileName);
                    if (imgExt == ".jpg" || imgExt == ".png" || imgExt == ".jpeg")
                    {
                        string imageName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                        string imageNameForDb = image.FileName;
                        string imgSaveTo = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/MissionMedia/" + imageName);
                        using (FileStream stream = new(imgSaveTo, FileMode.Create))
                        {
                            image.CopyTo(stream);
                        }
                        MissionMedium missionMed = new()
                        {
                            MissionId = addedMissionId,
                            MediaName = imageNameForDb,
                            MediaType = imgExt,
                            MediaPath = imageName,
                        };
                        _unitOfWork.MissionMedia.Add(missionMed);
                    }
                }
            }

            //for documents
            foreach (var missionDoc in missionDocumentList)
            {
                if (preloadedmissdocs.Length < 1)
                {
                    string missDocPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/MissionDocuments/", missionDoc.DocumentPath);

                    if (System.IO.File.Exists(missDocPath))
                    {
                        System.IO.File.Delete(missDocPath);
                    }

                    _unitOfWork.MissionDocument.Remove(missionDoc);
                }
                else
                {
                    bool flag = false;
                    for (int i = 0; i < preloadedmissdocs.Length; i++)
                    {
                        string docName = preloadedmissdocs[i][18..];

                        if (docName.Equals(missionDoc.DocumentPath))
                        {
                            flag = true;
                        }
                    }
                    if (!flag)
                    {
                        string docPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/MissionDocuments/", missionDoc.DocumentPath);

                        if (System.IO.File.Exists(docPath))
                        {
                            System.IO.File.Delete(docPath);
                        }

                        _unitOfWork.MissionDocument.Remove(missionDoc);
                    }
                }
            }

            if (missionDocFiles?.Count > 0)
            {
                foreach (var docs in missionDocFiles)
                {
                    string docExt = Path.GetExtension(docs.FileName);
                    if (docExt == ".docx" || docExt == ".pdf" || docExt == ".xlsx")
                    {
                        string docName = docs.FileName;
                        string docSaveTo = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/MissionDocuments/" + docName);
                        using (FileStream stream = new(docSaveTo, FileMode.Create))
                        {
                            docs.CopyTo(stream);
                        }
                        MissionDocument missionDoc = new()
                        {
                            MissionId = addedMissionId,
                            DocumentName = docName,
                            DocumentType = docExt,
                            DocumentPath = docName,
                        };
                        _unitOfWork.MissionDocument.Add(missionDoc);
                    }
                }
            }

            _unitOfWork.Save();
        }

        //add edit mission skills
        public void AddMissionSkills(long missionId, IEnumerable<long> missionSkillsId, List<long> finalMissSkillList)
        {
            if (finalMissSkillList.Count == 0)
            {
                foreach (var mId in missionSkillsId)
                {
                    MissionSkill deleteSkill = _unitOfWork.MissionSkills.GetFirstOrDefault(missionSkill => missionSkill.MissionId == mId);
                    _unitOfWork.MissionSkills.Remove(deleteSkill);
                }
            }
            else
            {
                var skillToBeAdded = finalMissSkillList.Except(missionSkillsId);
                foreach (var skillId in skillToBeAdded)
                {
                    MissionSkill missSkills = new()
                    {
                        MissionId = missionId,
                        SkillId = skillId,
                    };
                    _unitOfWork.MissionSkills.Add(missSkills);
                }

                var skillToBeDeleted = missionSkillsId.Except(finalMissSkillList);
                foreach (var skillid in skillToBeDeleted)
                {
                    MissionSkill deleteSkill = _unitOfWork.MissionSkills.GetFirstOrDefault(missionSkill => missionSkill.SkillId == skillid);
                    _unitOfWork.MissionSkills.Remove(deleteSkill);
                }
            }
            _unitOfWork.Save();
        }
        #endregion

        #endregion

        //---------------------- Mission Theme --------------------------//

        #region Mission Theme
        //MissionTheme lists
        [HttpGet]
        public IActionResult AdminMissionThemeDetails()
        {
            User user = GetThisUser();
            IEnumerable<MissionTheme> missionThemeLists = _unitOfWork.MissionTheme.GetAccToFilter(m => m.DeletedAt == null);
            adminMissionThemeDetails obj = new()
            {
                MissionThemeLists = missionThemeLists,
                UserDetails = user
            };
            return View(obj);
        }

        [HttpPost]
        public IActionResult AddAndUpdateMissionTheme(adminMissionThemeDetails obj)
        {
            var Edata1 = _unitOfWork.MissionTheme.GetAccToFilter(m => m.Title == obj.MissionThemeTitle && m.DeletedAt == null);
            var Edata2 = _unitOfWork.MissionTheme.GetFirstOrDefault(m => m.Title == obj.MissionThemeTitle && m.DeletedAt == null && m.MissionThemeId != obj.MissionThemeId);

            if (obj.MissionThemeId == 0)
            {
                if (Edata1.Count == 0)
                {
                    MissionTheme data = new()
                    {
                        Title = obj.MissionThemeTitle,
                        Status = 1
                    };

                    _unitOfWork.MissionTheme.Add(data);
                    _unitOfWork.Save();

                    TempData["success"] = "Data added successfully!";
                    return RedirectToAction("AdminMissionThemeDetails");
                }
            }
            else
            {
                if (Edata2 == null)
                {
                    MissionTheme updatedData = _unitOfWork.MissionTheme.GetFirstOrDefault(m => m.MissionThemeId == obj.MissionThemeId);
                    if (updatedData != null)
                    {
                        updatedData.MissionThemeId = obj.MissionThemeId;
                        updatedData.Title = obj.MissionThemeTitle;
                        updatedData.Status = (byte)obj.Status;
                        updatedData.UpdatedAt = DateTime.Now;
                        _unitOfWork.MissionTheme.Update(updatedData);
                        _unitOfWork.Save();

                        TempData["success"] = "Data updated successfully!";
                        return RedirectToAction("AdminMissionThemeDetails");
                    }
                }
            }

            TempData["error"] = "Something went wrong!";
            return RedirectToAction("AdminMissionThemeDetails");
        }

        [HttpPost]
        public IActionResult GetMissionThemeData(long missionThemeId)
        {
            MissionTheme missionThemeData = _unitOfWork.MissionTheme.GetFirstOrDefault(m => m.MissionThemeId == missionThemeId);
            return Json(missionThemeData);
        }

        [HttpPost]
        public IActionResult DeleteMissionThemeData(long missionThemeId)
        {
            var status = _unitOfWork.Mission.GetAccToFilter(m => m.MissionThemeId == missionThemeId);
            if (status.Count == 0)
            {
                if (missionThemeId > 0)
                {
                    MissionTheme deletingData = _unitOfWork.MissionTheme.GetFirstOrDefault(m => m.MissionThemeId == missionThemeId);
                    deletingData.DeletedAt = DateTime.Now;
                    _unitOfWork.MissionTheme.Update(deletingData);
                    _unitOfWork.Save();

                    return Json(true);
                }
            }
            TempData["error"] = "Opps! something went wrong";
            return Json(false);
        }
        #endregion

        //---------------------- Skills --------------------------//

        #region Skills
        //Skill Lists
        public IActionResult AdminMissionSkillDetails()
        {
            User user = GetThisUser();
            IEnumerable<Skill> SkillLists = _unitOfWork.Skill.GetAccToFilter(m => m.DeletedAt == null);
            adminMissionSkillDetails obj = new()
            {
                SkillLists = SkillLists,
                UserDetails = user
            };
            return View(obj);
        }

        [HttpPost]
        public IActionResult AddAndUpdateSkill(adminMissionSkillDetails obj)
        {
            var Edata1 = _unitOfWork.Skill.GetFirstOrDefault(m => m.SkillName == obj.SkillName && m.DeletedAt == null);
            var Edata2 = _unitOfWork.Skill.GetFirstOrDefault(m => m.SkillName == obj.SkillName && m.DeletedAt == null && m.SkillId != obj.SkillIds);

            if (obj.SkillIds == 0)
            {
                if (Edata1 == null)
                {
                    Skill data = new()
                    {
                        SkillName = obj.SkillName,
                        Status = 1
                    };

                    _unitOfWork.Skill.Add(data);
                    _unitOfWork.Save();

                    TempData["success"] = "Data added successfully!";
                    return RedirectToAction("AdminMissionSkillDetails");
                }
            }
            else
            {
                if (Edata2 == null)
                {
                    Skill updatedData = _unitOfWork.Skill.GetFirstOrDefault(m => m.SkillId == obj.SkillIds);
                    if (updatedData != null)
                    {
                        updatedData.SkillId = obj.SkillIds;
                        updatedData.SkillName = obj.SkillName;
                        updatedData.Status = (byte)obj.Status;

                        _unitOfWork.Skill.Update(updatedData);
                        _unitOfWork.Save();

                        TempData["success"] = "Data updated successfully!";
                        return RedirectToAction("AdminMissionSkillDetails");
                    }
                }
            }

            TempData["error"] = "Something went wrong!";
            return RedirectToAction("AdminMissionSkillDetails");
        }

        [HttpPost]
        public IActionResult GetSkillData(long skillId)
        {
            Skill skillData = _unitOfWork.Skill.GetFirstOrDefault(m => m.SkillId == skillId);
            return Json(skillData);
        }

        [HttpPost]
        public IActionResult DeleteSkillData(long skillId)
        {
            if (skillId > 0)
            {
                Skill deletingData = _unitOfWork.Skill.GetFirstOrDefault(m => m.SkillId == skillId);
                deletingData.DeletedAt = DateTime.Now;
                _unitOfWork.Skill.Update(deletingData);
                _unitOfWork.Save();

                return RedirectToAction("AdminMissionSkillDetails");
            }

            TempData["error"] = "Opps! something went wrong";
            return RedirectToAction("AdminMissionSkillDetails");
        }
        #endregion

        //---------------------- Mission Application --------------------------//

        #region Mission Appliction
        //MissionApplication Lists
        [HttpGet]
        public IActionResult AdminMissionApplicationDetails()
        {
            IEnumerable<MissionApplication> missionAppLists = _unitOfWork.MissionApplication.GetAllMissAppList();
            IEnumerable<MissionApplication> missionAppListsByFilter = missionAppLists.Where(m => m.ApprovalStatus == "PENDING").ToList();
            User user = GetThisUser();

            adminMissionApplicationDetails obj = new()
            {
                MissionAppLists = missionAppListsByFilter,
                UserDetails = user
            };
            return View(obj);
        }

        [HttpPost]
        public IActionResult ApproveAndDeclineMissionApplication(long missionApplicationId, int flag)
        {
            if (missionApplicationId > 0)
            {
                MissionApplication missionData = _unitOfWork.MissionApplication.GetFirstOrDefault(m => m.MissionApplicationId == missionApplicationId);

                if (missionData != null)
                {
                    if (flag == 1)
                    {
                        Mission data = _unitOfWork.Mission.GetFirstOrDefault(m => m.MissionId == missionData.MissionId);
                        if(data.Seats > 0)
                        {
                            data.Seats = data.Seats - 1;
                            missionData.ApprovalStatus = "APPROVE";

                            if (data.Seats == 0)
                            {
                                data.Status = 0;
                            }

                            TempData["success"] = "Application Approved successfully!";
                            _unitOfWork.Mission.Update(data);
                        }
                        else
                        {
                            missionData.ApprovalStatus = "DECLINE";
                            TempData["error"] = "Application can't be added";
                        }

                    }
                    else
                    {
                        missionData.ApprovalStatus = "DECLINE";
                        TempData["success"] = "Application Declined successfully!";
                    }

                    _unitOfWork.MissionApplication.Update(missionData);
                    _unitOfWork.Save();
                    return RedirectToAction("AdminMissionSkillDetails");
                }
            }
            TempData["error"] = "Opps! something went wrong";
            return RedirectToAction("AdminMissionApplicationDetails");
        }
        #endregion

        //---------------------- Story --------------------------//

        #region Story
        //Story lists
        [HttpGet]
        public IActionResult AdminStoryDetails()
        {
            IEnumerable<Story> storyLists = _unitOfWork.Story.GetAllStory();
            IEnumerable<Story> storyListsByFilter = storyLists.Where(m => m.Status == "PENDING").ToList();
            User user = GetThisUser();

            adminStoryDetails obj = new()
            {
                StoryLists = storyListsByFilter,
                UserDetails = user,
            };
            return View(obj);
        }

        [HttpPost]
        public IActionResult ApproveAndDeclineStory(long storyId, int flag)
        {
            if (storyId > 0)
            {
                Story storyData = _unitOfWork.Story.GetFirstOrDefault(m => m.StoryId == storyId);

                if (storyData != null)
                {
                    if (flag == 1)
                    {
                        storyData.Status = "PUBLISHED";
                        TempData["success"] = "Story published successfully!";
                    }
                    else
                    {
                        storyData.Status = "DECLINED";
                        TempData["success"] = "Data Declined successfully!";
                    }
                    _unitOfWork.Story.Update(storyData);
                    _unitOfWork.Save();
                    return RedirectToAction("AdminStoryDetails");
                }
            }
            TempData["error"] = "Opps! something went wrong";
            return RedirectToAction("AdminStoryDetails");
        }

        [HttpPost]
        public IActionResult GetStoryDetails(long storyId)
        {
            Story FindingStoryCreator = _repo.StoryData(storyId);

            adminStoryDetails storyDetails = new()
            {
                StoryData = FindingStoryCreator,
            };

            return Json(storyDetails);
        }
        #endregion

        //---------------------- Banner --------------------------//

        #region Banner
        //Banner Lists
        public IActionResult AdminBannerDetails()
        {
            IEnumerable<Banner> bannerLists = _unitOfWork.Banner.GetAccToFilter(m => m.DeletedAt == null);
            User user = GetThisUser();
            adminBannerDetails obj = new()
            {
                BannerLists = bannerLists,
                UserDetails = user
            };
            return View(obj);
        }

        [HttpPost]
        public IActionResult AddAndUpdateBanner(adminBannerDetails obj, IFormFile banner)
        {
            var Bdata = _unitOfWork.Banner.GetFirstOrDefault(m => m.DeletedAt == null && m.BannerId == obj.BannerId);

            if (obj.BannerId == 0)
            {
                Banner data = new()
                {
                    Text = obj.BannerText,
                    SortOrder = obj.BannerNumber
                };

                if (banner != null)
                {
                    string imgExt = Path.GetExtension(banner.FileName);
                    if (imgExt == ".jpg" || imgExt == ".png" || imgExt == ".jpeg")
                    {
                        string ImageName = Path.GetFileName(banner.FileName);
                        var imgSaveTo = Path.Combine(_iweb.WebRootPath, "/Banners/", ImageName);
                        string finalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/" + imgSaveTo);

                        using (FileStream stream = new(finalPath, FileMode.Create))
                        {
                            banner.CopyTo(stream);
                        }
                        data.Image = imgSaveTo;
                    }
                }

                _unitOfWork.Banner.Add(data);
                _unitOfWork.Save();

                TempData["success"] = "Data added successfully!";
                return RedirectToAction("AdminBannerDetails");

            }
            else
            {
                if (Bdata != null)
                {
                    Banner updatedData = _unitOfWork.Banner.GetFirstOrDefault(m => m.BannerId == obj.BannerId);
                    if (updatedData != null)
                    {
                        updatedData.BannerId = obj.BannerId;
                        updatedData.Text = obj.BannerText;
                        updatedData.SortOrder = obj.BannerNumber;

                        if (updatedData != null && banner != null)
                        {
                            string imgExt = Path.GetExtension(banner.FileName);
                            if (imgExt == ".jpg" || imgExt == ".png" || imgExt == ".jpeg")
                            {
                                if (updatedData.Image != null)
                                {
                                    string ImageName = Path.GetFileName(banner.FileName);
                                    var imgSaveTo = Path.Combine(_iweb.WebRootPath, "/Banners/", ImageName);
                                    string finalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/" + imgSaveTo);

                                    if (!imgSaveTo.Equals(updatedData.Image))
                                    {
                                        string alrExists = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/" + updatedData.Image);
                                        if (System.IO.File.Exists(alrExists))
                                        {
                                            System.IO.File.Delete(alrExists);
                                        }

                                        using (FileStream stream = new(finalPath, FileMode.Create))
                                        {
                                            banner.CopyTo(stream);
                                        }
                                        updatedData.Image = imgSaveTo;
                                        updatedData.UpdatedAt = DateTime.Now;
                                    }
                                }
                                else
                                {
                                    string ImageName = Path.GetFileName(banner.FileName);
                                    var imgSaveTo = Path.Combine(_iweb.WebRootPath, "/Banners/", ImageName);
                                    string finalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/" + imgSaveTo);

                                    using (FileStream stream = new(finalPath, FileMode.Create))
                                    {
                                        banner.CopyTo(stream);
                                    }
                                    updatedData.Image = imgSaveTo;
                                    updatedData.UpdatedAt = DateTime.Now;
                                }
                            }
                        }

                        _unitOfWork.Banner.Update(updatedData);
                        _unitOfWork.Save();

                        TempData["success"] = "Data updated successfully!";
                        return RedirectToAction("AdminBannerDetails");
                    }
                }
            }
            TempData["success"] = "Data added successfully!";
            return RedirectToAction("AdminBannerDetails");
        }

        [HttpPost]
        public IActionResult GetBannerData(long bannerId)
        {
            Banner bannerData = _unitOfWork.Banner.GetFirstOrDefault(m => m.BannerId == bannerId);
            return Json(bannerData);
        }

        [HttpPost]
        public IActionResult DeleteBannerData(long bannerId)
        {
            if (bannerId > 0)
            {
                Banner deletingData = _unitOfWork.Banner.GetFirstOrDefault(m => m.BannerId == bannerId);
                deletingData.DeletedAt = DateTime.Now;

                string alrExists = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/" + deletingData.Image);
                System.IO.File.Delete(alrExists);

                _unitOfWork.Banner.Update(deletingData);
                _unitOfWork.Save();

                return RedirectToAction("AdminBannerDetails");
            }

            TempData["error"] = "Opps! something went wrong";
            return RedirectToAction("AdminBannerDetails");
        }

        [HttpPost]
        public IActionResult CheckNumber(int bannerNumber)
        {
            var status = _unitOfWork.Banner.GetFirstOrDefault(m => m.SortOrder == bannerNumber && m.DeletedAt == null);

            if (status == null)
            {
                return Json(true);
            }
            return Json(false);
        }
        #endregion

        //---------------------- END --------------------------//
    }
}