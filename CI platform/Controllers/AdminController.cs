using CI_platform.Entities.DataModels;
using CI_platform.Entities.ViewModels;
using CI_platform.Repositories.GenericRepository.Interface;
using CI_platform.Repositories.MethodRepository.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Security.Claims;

namespace CI_platform.Controllers
{
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
                    Department = obj.Department
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

                string alrExists = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/" + deletingData.Avatar);
                System.IO.File.Delete(alrExists);

                deletingData.DeletedAt = DateTime.Now;
                _unitOfWork.User.Update(deletingData);
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

        [HttpPost]
        public IActionResult DeleteMissionData(long missionId)
        {
            if (missionId > 0)
            {
                Mission deletingData = _unitOfWork.Mission.GetFirstOrDefault(m => m.MissionId == missionId);
                deletingData.DeletedAt = DateTime.Now;
                _unitOfWork.Mission.Update(deletingData);
                _unitOfWork.Save();

                return RedirectToAction("AdminMissionDetails");
            }

            TempData["error"] = "Opps! something went wrong";
            return RedirectToAction("AdminMissionDetails");
        }
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
                if (Edata1 == null)
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
            if (missionThemeId > 0)
            {
                MissionTheme deletingData = _unitOfWork.MissionTheme.GetFirstOrDefault(m => m.MissionThemeId == missionThemeId);
                deletingData.DeletedAt = DateTime.Now;
                _unitOfWork.MissionTheme.Update(deletingData);
                _unitOfWork.Save();

                return RedirectToAction("AdminMissionThemeDetails");
            }

            TempData["error"] = "Opps! something went wrong";
            return RedirectToAction("AdminMissionThemeDetails");
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
                        missionData.ApprovalStatus = "APPROVE";
                        TempData["success"] = "Data Approved successfully!";
                    }
                    else
                    {
                        missionData.ApprovalStatus = "DECLINE";
                        TempData["success"] = "Data Declined successfully!";
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
                UserDetails = user
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
            User? FindingStoryCreator = _repo.UserOfStory(storyId);

            adminStoryDetails storyDetails = new()
            {
                UserOfStory = FindingStoryCreator,
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
            var Bdata = _unitOfWork.Banner.GetFirstOrDefault(m => m.DeletedAt == null && m.BannerId != obj.BannerId);

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
                if (Bdata == null)
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
        #endregion

        //---------------------- END --------------------------//
    }
}
