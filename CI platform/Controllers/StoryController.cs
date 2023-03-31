using CI_platform.Entities.DataModels;
using CI_platform.Entities.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
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

            List<MissionApplication> draftMissAppList = new List<MissionApplication>();
            List<MissionApplication> usersAppMission = _dbContext.MissionApplications.Where(m => m.UserId == user.UserId && m.ApprovalStatus == "APPROVE").ToList();
            foreach (var mission in usersAppMission)
            {
                var story = _dbContext.Stories.Where(m => m.MissionId == mission.MissionId && m.UserId == user.UserId).FirstOrDefault();
                if (story != null && (story.Status == "DRAFT" || story.Status == "DECLINED"))
                {
                    draftMissAppList.Add(mission);
                }
                else if (story == null)
                {
                    draftMissAppList.Add(mission);
                }

            }

            try
            {
                userAddStoryModel.MissionApplication = draftMissAppList;
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


            var alreadyPostedStory = _dbContext.Stories.Where(m => m.MissionId == MissionId && m.UserId == uid && m.Status == "DRAFT").FirstOrDefault();
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
 
            else if (alreadyPostedStory != null)
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

            List<MissionApplication> draftMissAppList = new List<MissionApplication>();
            List<MissionApplication> usersAppMission = _dbContext.MissionApplications.Where(m => m.UserId == user.UserId && m.ApprovalStatus == "APPROVE").ToList();
            foreach (var mission in usersAppMission)
            {
                var story = _dbContext.Stories.Where(m => m.MissionId == mission.MissionId && m.UserId == user.UserId).FirstOrDefault();
                if (story != null && (story.Status == "DRAFT" || story.Status == "DECLINED"))
                {
                    draftMissAppList.Add(mission);
                }
                else if (story == null)
                {
                    draftMissAppList.Add(mission);
                }

            }

            model.MissionApplication = draftMissAppList;
            model.Missions = _dbContext.Missions.ToList();
            return View("StoryAddPage", model);

        }


        [HttpPost]
        public IActionResult GetMissionDetails(long missionId)
        {
            var query = (from st in _dbContext.Stories
                         join md in _dbContext.StoryMedia
                         on st.StoryId equals md.StoryId
                         where st.MissionId == missionId && st.Status == "DRAFT"
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

        public IActionResult VolunteeringStoryPage(long storyId, int views)
        {
            var storyForView = _dbContext.Stories.Where(m => m.StoryId == storyId).FirstOrDefault();

            if(storyForView.Views < views)
            {
                storyForView.Views = views;
                _dbContext.Update(storyForView);
                _dbContext.SaveChanges();
            }


            User? user = GetThisUser();
            User? FindingStoryCreator = _dbContext.Stories.Where(m => m.StoryId == storyId).Select(m => m.User).FirstOrDefault();
            List<User> ListOfUsers = _dbContext.Users.ToList();
            userVolunteerStoryModel userVolunteerStoryModel = new()
            {
                User = user,
                UserList = ListOfUsers,
                UserOfStory = FindingStoryCreator,
                StoryDetails = _dbContext.Stories.Where(m => m.StoryId == storyId).FirstOrDefault(),
                StoryMedia = _dbContext.StoryMedia.Where(m => m.StoryId == storyId).ToList()
            };

            return View(userVolunteerStoryModel);
        }

        #region Recommanded To Co-worker

        public void RecommandToCoworker(int[]? userIds, long sId, int totalViews)
        {
            var thisUser = GetThisUser();
            Story thisStory = _dbContext.Stories.Find(sId);

            if (userIds != null)
            {
                foreach (var id in userIds)
                {
                    var user = _dbContext.Users.Where(m => m.UserId == id).FirstOrDefault();
                    StoryInvite obj = new()
                    {
                        StoryId = sId,
                        ToUserId = user.UserId,
                        FromUserId = thisUser.UserId
                    };
                    _dbContext.Add(obj);
                    _dbContext.SaveChanges();

                    var inviteLink = Url.Action("ViewStory", "Story", new { storyId = sId, views = totalViews }, Request.Scheme);
                    var fromAddress = new MailAddress("kanjiyashyam15@gmail.com", "CI Platform");
                    var toAddress = new MailAddress(user.Email);
                    var subject = "Mission Invite";
                    var body = $"Hello <b>{@user.FirstName} {@user.LastName}</b> ,<br /><br /> Your Colleague <b>{thisUser.FirstName} {thisUser.LastName}</b>sent you an intrested story <b><i>{thisStory.Title}</i></b><br /><br />Click the following link to read the story,<br /><br /><a href='{inviteLink}'>{inviteLink}</a><br /><br />Regards,<br/>";
                    var msg = new MailMessage(fromAddress, toAddress)
                    {
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    };

                    var smtpClient = new SmtpClient("smtp.gmail.com", 587)
                    {
                        Credentials = new NetworkCredential("kanjiyashyam15@gmail.com", "swbylrxevxoaubor"),
                        EnableSsl = true,
                    };
                    smtpClient.Send(msg);
                }
            }
        }

        #endregion

        #endregion
    }
}
