using CI_platform.Entities.DataModels;
using CI_platform.Entities.ViewModels;
using CI_platform.Repositories.GenericRepository.Interface;
using Microsoft.AspNetCore.Mvc;

namespace CI_platform.Controllers
{
    public class AdminController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public AdminController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public IActionResult AdminUserDetails()
        {
            IEnumerable<User> userLists = _unitOfWork.User.GetAll();
            adminUserDetails obj = new();
            obj.UserLists = userLists;
            return View(obj);
        }


        //CMS lists
        public IActionResult AdminCMSPage()
        {
            IEnumerable<CmsPage> cmsLists = _unitOfWork.CMSPage.GetAll();
            adminCMSPageDetails obj = new()
            {
                CMSLists = cmsLists
            };
            return View(obj);
        }


        //Mission lists
        public IActionResult AdminMissionDetails()
        {
            IEnumerable<Mission> missionLists = _unitOfWork.Mission.GetAll();
            adminMissionDetails obj = new()
            {
                MissionLists = missionLists
            };
            return View(obj);
        }


        //MissionTheme lists
        public IActionResult AdminMissionThemeDetails()
        {
            IEnumerable<MissionTheme> missionThemeLists = _unitOfWork.MissionTheme.GetAll();
            adminMissionThemeDetails obj = new()
            {
                MissionThemeLists = missionThemeLists,
            };
            return View(obj);
        }


        //Skill Lists
        public IActionResult AdminMissionSkillDetails()
        {
            IEnumerable<Skill> SkillLists = _unitOfWork.Skill.GetAll();
            adminMissionSkillDetails obj = new()
            {
                SkillLists = SkillLists,
            };
            return View(obj);
        }


        //MissionApplication Lists
        public IActionResult AdminMissionApplicationDetails()
        {
            IEnumerable<MissionApplication> missionAppLists = _unitOfWork.MissionApplication.GetAllMissAppList();
            adminMissionApplicationDetails obj = new()
            {
                MissionAppLists = missionAppLists,
            };
            return View(obj);
        }


        //Story lists
        public IActionResult AdminStoryDetails()
        {
            IEnumerable<Story> storyLists = _unitOfWork.Story.GetAllStory();
            adminStoryDetails obj = new()
            {
                StoryLists = storyLists,
            };
            return View(obj);
        }


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
