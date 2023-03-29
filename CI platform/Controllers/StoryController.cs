using CI_platform.Entities.DataModels;
using CI_platform.Entities.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CI_platform.Controllers
{
    public class StoryController : Controller
    {
        private readonly ILogger<StoryController> _logger;
        private readonly CIDbContext _dbContext;
        private readonly IWebHostEnvironment _iweb;

        public StoryController(ILogger<StoryController> logger, CIDbContext dbContext, IWebHostEnvironment iweb)
        {
            _logger = logger;
            _dbContext = dbContext;
            _iweb = iweb;
        }

        public User GetThisUser()
        {
            var identity = User.Identity as ClaimsIdentity;
            var email = identity?.FindFirst(ClaimTypes.Email)?.Value;

            var user = _dbContext.Users.Where(m => m.Email == email).FirstOrDefault();
            return user;
        }

        //---------------------- Story Listing Page --------------------------//

        #region Story Listing Page

        public IActionResult storyListingPage()
        {
            return View();
        }

        public IActionResult bringStories(int pg = 1)
        {
            List<Story> stories = _dbContext.Stories.ToList();

            userStoryListModel userStory = new userStoryListModel
            {
                Stories = _dbContext.Stories.ToList(),
                Missions = _dbContext.Missions.ToList(),
                Users = _dbContext.Users.ToList(),
                MissionThemes = _dbContext.MissionThemes.ToList()
            };

            const int pageSize = 3;
            if (pg < 1)
                pg = 1;

            int recsCount = stories.Count();

            var pager = new userPager(recsCount, pg, pageSize);

            int recSkip = (pg - 1) * pageSize;

            stories = stories.Skip(recSkip).Take(pager.PageSize).ToList();

            this.ViewBag.userPager = pager;

            userStory.Stories = stories;

            ViewBag.missionCount = recsCount;

            return PartialView("_StoryCardView", userStory);
        }

        #endregion

        //---------------------- Story Add Page --------------------------//

        #region Story Add Page
        [HttpGet]
        public IActionResult StoryAddPage()
        {
            User user = GetThisUser();
            userAddStoryModel userAddStoryModel = new userAddStoryModel();

            userAddStoryModel.Missions = _dbContext.Missions.ToList();
            try
            {
                userAddStoryModel.MissionApplication = _dbContext.MissionApplications.Where(m => m.UserId == user.UserId && m.ApprovalStatus == "APPROVE").ToList();
                userAddStoryModel.Story = _dbContext.Stories.Where(m => m.UserId == user.UserId && m.Status == "DRAFT").FirstOrDefault();
                userAddStoryModel.StoryMedium = _dbContext.StoryMedia.Where(m => m.StoryId == userAddStoryModel.Story.StoryId).ToList();
            }
            catch
            {
                
            }



            return View(userAddStoryModel);
        }


        [HttpPost]
        public IActionResult StoryAddPage(long MissionId, string storyTitle, string storyDate, string storyDesc, string videoURL, List<IFormFile> files)
        {
            var user = GetThisUser();
            var uid = user.UserId;
            userAddStoryModel obj = new userAddStoryModel();

            ViewBag.mid = MissionId;
            ViewBag.uid = uid;

            var alreadyPostedStory = _dbContext.Stories.Where(m => m.MissionId == MissionId && m.UserId == uid).FirstOrDefault();
            obj.Result = "false";

            if (alreadyPostedStory == null)
            {
                Story story = new Story
                {
                    Title = storyTitle,
                    Description = storyDesc,
                    Status = "DRAFT",
                    MissionId = MissionId,
                    UserId = uid,
                    PublishedAt = DateTime.Parse(storyDate)
                };

                _dbContext.Stories.Add(story);
                _dbContext.SaveChanges();

                var data = _dbContext.Stories.Where(m => m.UserId == user.UserId && m.MissionId == MissionId).FirstOrDefault();

                foreach (IFormFile img in files)
                {
                    string imgExt = Path.GetExtension(img.FileName);
                    if (imgExt == ".jpg" || imgExt == ".png" || imgExt == ".jpeg")
                    {
                        var imgSaveTo = Path.Combine(_iweb.WebRootPath, "StoryImages", img.FileName);
                        var stream = new FileStream(imgSaveTo, FileMode.Create);
                        img.CopyTo(stream);

                        StoryMedium storyMedium = new StoryMedium();
                        storyMedium.StoryId = data.StoryId;
                        storyMedium.Type = imgExt;
                        storyMedium.Path = imgSaveTo;

                        _dbContext.StoryMedia.Add(storyMedium);
                        _dbContext.SaveChanges();
                    }
                }

            }
            else if(alreadyPostedStory != null)
            {
                alreadyPostedStory.Title = storyTitle;
                alreadyPostedStory.Description = storyDesc;
                alreadyPostedStory.PublishedAt = DateTime.Parse(storyDate);

                _dbContext.SaveChanges();

                var imageCheck = _dbContext.StoryMedia.Where(m => m.StoryId == alreadyPostedStory.StoryId).ToList();
                if (imageCheck != null)
                {
                    foreach (var img in imageCheck)
                    {
                        _dbContext.Remove(img);
                        _dbContext.SaveChanges();
                    }

                    var data = _dbContext.Stories.Where(m => m.UserId == user.UserId && m.MissionId == MissionId).FirstOrDefault();

                    foreach (IFormFile img in files)
                    {
                        string imgExt = Path.GetExtension(img.FileName);
                        if (imgExt == ".jpg" || imgExt == ".png" || imgExt == ".jpeg")
                        {
                            var imgSaveTo = Path.Combine(_iweb.WebRootPath, "StoryImages", img.FileName);
                            var stream = new FileStream(imgSaveTo, FileMode.Create);
                            img.CopyTo(stream);

                            StoryMedium storyMedium = new StoryMedium();
                            storyMedium.StoryId = data.StoryId;
                            storyMedium.Type = imgExt;
                            storyMedium.Path = imgSaveTo;

                            _dbContext.StoryMedia.Add(storyMedium);
                            _dbContext.SaveChanges();
                        }
                    }
                }
            }

            userAddStoryModel model = new userAddStoryModel();
            model.MissionApplication = _dbContext.MissionApplications.Where(m => m.UserId == user.UserId && m.ApprovalStatus == "APPROVE").ToList();
            model.Missions = _dbContext.Missions.ToList();
            return View("StoryAddPage", model);

        }


        [HttpPost]
        public IActionResult GetMissionDetails(long missionId)
        {
            var user = GetThisUser();

            var query = (from st in _dbContext.Stories
                         join md in _dbContext.StoryMedia
                         on st.StoryId equals md.StoryId
                         where st.MissionId == missionId
                         orderby st.StoryId descending
                         select new
                         {
                             st.Title,
                             st.Description,
                             st.PublishedAt,
                             md.Path
                         }).ToList();

            return Json(query);

        }

        #endregion

        //---------------------- Volunteering Story Page --------------------------//

        #region Volunteering Story Page

        public IActionResult VolunteeringStoryPage()
        {
            return View();
        }

        #endregion
    }
}
