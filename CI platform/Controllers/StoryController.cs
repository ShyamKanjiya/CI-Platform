using CI_platform.Entities.DataModels;
using CI_platform.Entities.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using static System.Net.Mime.MediaTypeNames;
using CI_platform.Repositories.GenericRepository;
using CI_platform.Repositories.GenericRepository.Interface;
using CI_platform.Repositories.MethodRepository.Interface;

namespace CI_platform.Controllers
{
    public class StoryController : Controller
    {
        private readonly ILogger<StoryController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _iweb;
        private readonly IStoryMethodRepository _repo;

        public StoryController(ILogger<StoryController> logger, IUnitOfWork unitOfWork, IWebHostEnvironment iweb, IStoryMethodRepository storyMethodRepository)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _iweb = iweb;
            _repo = storyMethodRepository;
        }

        public User GetThisUser()
        {
            var identity = User.Identity as ClaimsIdentity;
            var email = identity?.FindFirst(ClaimTypes.Email)?.Value;

            var user = _unitOfWork.User.GetFirstOrDefault(m => m.Email == email);
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
            List<Story> stories = (List<Story>)_unitOfWork.Story.GetAll();

            userStoryListModel userStory = new userStoryListModel
            {
                Stories = _unitOfWork.Story.GetAll(),
                Missions = _unitOfWork.Mission.GetAll(),
                Users = _unitOfWork.User.GetAll(),
                MissionThemes = _unitOfWork.MissionTheme.GetAll()
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

            userAddStoryModel.Missions = (List<Mission>)_unitOfWork.Mission.GetAll();

            List<MissionApplication> draftMissAppList = new List<MissionApplication>();
            List<MissionApplication> usersAppMission = _unitOfWork.MissionApplication.GetAccToFilter(m => m.UserId == user.UserId && m.ApprovalStatus == "APPROVE");
            foreach (var mission in usersAppMission)
            {
                var story = _unitOfWork.Story.GetFirstOrDefault(m => m.MissionId == mission.MissionId && m.UserId == user.UserId);
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
                userAddStoryModel.Story = _unitOfWork.Story.GetFirstOrDefault(m => m.UserId == user.UserId && m.Status == "DRAFT");
                userAddStoryModel.StoryMedium = _unitOfWork.StoryMedia.GetAccToFilter(m => m.StoryId == userAddStoryModel.Story.StoryId);
            }
            catch
            {

            }

            return View(userAddStoryModel);
        }


        [HttpPost]
        public IActionResult StoryAddPage(long MissionId, string storyTitle, string storyDate, string storyDesc, string videoURL, List<IFormFile> files, string[] preloaded)
        {
            var user = GetThisUser();
            var uid = user.UserId;
            userAddStoryModel obj = new userAddStoryModel();


            var alreadyPostedStory = _unitOfWork.Story.GetFirstOrDefault(m => m.MissionId == MissionId && m.UserId == uid && m.Status == "DRAFT");
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

                _unitOfWork.Story.Add(story);
                _unitOfWork.Save();

            }

            else if (alreadyPostedStory != null)
            {
                alreadyPostedStory.Title = storyTitle;
                alreadyPostedStory.Description = storyDesc;
                alreadyPostedStory.PublishedAt = DateTime.Parse(storyDate);

                _unitOfWork.Save();
            }


            var st = _unitOfWork.Story.GetFirstOrDefault(m => m.UserId == user.UserId && m.MissionId == MissionId);
            var media = _unitOfWork.StoryMedia.GetAccToFilter(m => m.StoryId == st.StoryId);


            if (videoURL != null)
            {
                foreach (var video in media)
                {
                    if (video.Type == "video")
                    {
                        if (video != null)
                        {
                            _unitOfWork.StoryMedia.Remove(video);
                        }
                    }
                }
                StoryMedium objModel = new()
                {
                    StoryId = st.StoryId,
                    Type = "video",
                    Path = videoURL,
                };
                _unitOfWork.StoryMedia.Add(objModel);
                _unitOfWork.Save();
            }
            else
            {
                foreach (var video in media)
                {
                    if (video.Type == "video")
                    {
                        if (video != null)
                        {
                            _unitOfWork.StoryMedia.Remove(video);
                        }
                    }
                }
                _unitOfWork.Save();
            }


            foreach (var img in media)
            {
                if (img.Type != "video")
                {
                    if (preloaded.Length < 1)
                    {
                        string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/StoryImages/", img.Path);

                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }

                        _unitOfWork.StoryMedia.Remove(img);
                        _unitOfWork.Save();
                    }
                    else
                    {
                        bool flag = false;

                        for (int i = 0; i < preloaded.Length; i++)
                        {
                            var imgName = preloaded[i].Substring(13);

                            if (imgName.Equals(img.Path))
                            {
                                flag = true;
                            }
                        }
                        if (!flag)
                        {
                            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/StoryImages/", img.Path);

                            if (System.IO.File.Exists(imagePath))
                            {
                                System.IO.File.Delete(imagePath);
                            }

                            _unitOfWork.StoryMedia.Remove(img);
                            _unitOfWork.Save();
                        }

                    }
                }
            }

            var data = _unitOfWork.Story.GetFirstOrDefault(m => m.UserId == user.UserId && m.MissionId == MissionId);
            foreach (IFormFile img in files)
            {
                string imgExt = Path.GetExtension(img.FileName);
                if (imgExt == ".jpg" || imgExt == ".png" || imgExt == ".jpeg")
                {
                    string ImageName = Guid.NewGuid().ToString() + Path.GetExtension(img.FileName);
                    var imgSaveTo = Path.Combine(_iweb.WebRootPath, "StoryImages", ImageName);
                    /*var stream = new FileStream(imgSaveTo, FileMode.Create);
                    img.CopyTo(stream);*/
                    using(FileStream stream = new (imgSaveTo, FileMode.Create)) {
                        img.CopyTo(stream);
                    }

                        StoryMedium storyMedium = new StoryMedium();
                    storyMedium.StoryId = data.StoryId;
                    storyMedium.Type = imgExt;
                    storyMedium.Path = ImageName;

                    _unitOfWork.StoryMedia.Add(storyMedium);
                    _unitOfWork.Save();
                }
            }

            userAddStoryModel model = new userAddStoryModel();

            List<MissionApplication> draftMissAppList = new List<MissionApplication>();
            List<MissionApplication> usersAppMission = _unitOfWork.MissionApplication.GetAccToFilter(m => m.UserId == user.UserId && m.ApprovalStatus == "APPROVE");
            foreach (var mission in usersAppMission)
            {
                var story = _unitOfWork.Story.GetFirstOrDefault(m => m.MissionId == mission.MissionId && m.UserId == user.UserId);
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
            model.Missions = (List<Mission>)_unitOfWork.Mission.GetAll();
            return View("StoryAddPage", model);

        }


        [HttpPost]
        public IActionResult GetMissionDetails(long missionId)
        {
            User user = GetThisUser();
            var query = _repo.GetStory(missionId, user.UserId);

            return Json(query);
        }


        [HttpPost]
        public IActionResult SubmitStory(long missionId)
        {
            User user = GetThisUser();
            Story storyOfUser = _unitOfWork.Story.GetFirstOrDefault(m => m.MissionId == missionId && m.UserId == user.UserId && m.Status == "DRAFT");
            if (storyOfUser != null)
            {
                storyOfUser.Status = "PENDING";

                _unitOfWork.Story.Update(storyOfUser);
                _unitOfWork.Save();
            }
            return RedirectToAction("StoryListingPage");
        }
        #endregion

        //---------------------- Volunteering Story Page --------------------------//

        #region Volunteering Story Page

        public IActionResult VolunteeringStoryPage(long storyId, int views)
        {
            var storyForView = _unitOfWork.Story.GetFirstOrDefault(m => m.StoryId == storyId);

            if (storyForView.Views < views)
            {
                storyForView.Views = views;
                _unitOfWork.Story.Update(storyForView);
                _unitOfWork.Save();
            }


            User? user = GetThisUser();
            User? FindingStoryCreator = _repo.UserOfStory(storyId);
            List<User> ListOfUsers = (List<User>)_unitOfWork.User.GetAll();
            userVolunteerStoryModel userVolunteerStoryModel = new()
            {
                User = user,
                UserList = ListOfUsers,
                UserOfStory = FindingStoryCreator,
                StoryDetails = _unitOfWork.Story.GetFirstOrDefault(m => m.StoryId == storyId),
                StoryMedia = _unitOfWork.StoryMedia.GetAccToFilter(m => m.StoryId == storyId)
            };

            return View(userVolunteerStoryModel);
        }

        #region Recommanded To Co-worker

        public void RecommandToCoworker(int[]? userIds, long sId, int totalViews)
        {
            var thisUser = GetThisUser();
            Story thisStory = _unitOfWork.Story.GetFirstOrDefault(m => m.StoryId == sId);

            if (userIds != null)
            {
                foreach (var id in userIds)
                {
                    var user = _unitOfWork.User.GetFirstOrDefault(m => m.UserId == id);
                    StoryInvite obj = new()
                    {
                        StoryId = sId,
                        ToUserId = user.UserId,
                        FromUserId = thisUser.UserId
                    };
                    _unitOfWork.StoryInvite.Add(obj);
                    _unitOfWork.Save();

                    var inviteLink = Url.Action("VolunteeringStoryPage", "Story", new { storyId = sId, views = totalViews }, Request.Scheme);
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
                        Credentials = new NetworkCredential("kanjiyashyam15@gmail.com", "aswytlgxbpbrjpmn"),
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
