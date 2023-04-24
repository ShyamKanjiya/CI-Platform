﻿using CI_platform.Entities.DataModels;
using CI_platform.Entities.ViewModels;
using CI_platform.Repositories.GenericRepository.Interface;
using CI_platform.Repositories.MethodRepository.Interface;
using Microsoft.AspNetCore.Mvc;

namespace CI_platform.Controllers
{
    public class AdminController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStoryMethodRepository _repo;
        public AdminController(IUnitOfWork unitOfWork, IStoryMethodRepository storyMethodRepository)
        {
            _unitOfWork = unitOfWork;
            _repo = storyMethodRepository;
        }

        //---------------------- User --------------------------//

        #region User
        //User lists

        [HttpGet]
        public IActionResult AdminUserDetails()
        {
            IEnumerable<User> userLists = _unitOfWork.User.GetAccToFilter(m => m.DeletedAt == null);
            adminUserDetails obj = new();
            obj.UserLists = userLists;
            return View(obj);
        }

        [HttpGet]
        public IActionResult AdminAddUser()
        {
            return View();
        }

        [HttpPost]
        public IActionResult DeleteUserData(long userId)
        {
            if (userId > 0)
            {
                User deletingData = _unitOfWork.User.GetFirstOrDefault(m => m.UserId == userId);
                deletingData.DeletedAt = DateTime.Now;
                _unitOfWork.User.Update(deletingData);
                _unitOfWork.Save();

                return RedirectToAction("AdminUserDetails");
            }

            TempData["error"] = "Opps! something went wrong";
            return RedirectToAction("AdminUserDetails");
        }
        #endregion

        //---------------------- CMS --------------------------//

        #region CMS
        //CMS lists
        public IActionResult AdminCMSPage()
        {
            IEnumerable<CmsPage> cmsLists = _unitOfWork.CMSPage.GetAccToFilter(m => m.DeletedAt == null);
            adminCMSPageDetails obj = new()
            {
                CMSLists = cmsLists
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
            IEnumerable<Mission> missionLists = _unitOfWork.Mission.GetAccToFilter(m => m.DeletedAt == null);
            adminMissionDetails obj = new()
            {
                MissionLists = missionLists
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
            IEnumerable<MissionTheme> missionThemeLists = _unitOfWork.MissionTheme.GetAccToFilter(m => m.DeletedAt == null);
            adminMissionThemeDetails obj = new()
            {
                MissionThemeLists = missionThemeLists,
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
            IEnumerable<Skill> SkillLists = _unitOfWork.Skill.GetAccToFilter(m => m.DeletedAt == null);
            adminMissionSkillDetails obj = new()
            {
                SkillLists = SkillLists,
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

            adminMissionApplicationDetails obj = new()
            {
                MissionAppLists = missionAppListsByFilter,
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
            adminStoryDetails obj = new()
            {
                StoryLists = storyListsByFilter,
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

        //Banner Lists
        public IActionResult AdminBannerDetails()
        {
            IEnumerable<Banner> bannerLists = _unitOfWork.Banner.GetAll();
            adminBannerDetails obj = new()
            {
                BannerLists = bannerLists,
            };
            return View(obj);
        }

    }
}