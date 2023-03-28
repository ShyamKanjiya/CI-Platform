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

        public StoryController(ILogger<StoryController> logger, CIDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
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

        public IActionResult StoryAddPage()
        {
            return View();
        }

        public IActionResult SaveStoryToDB(long missionId,string? storyTitle, string? storyDescription, DateTime? publishDate, string? url, string[]? storyImages, int status )
        {
            User user = GetThisUser();

            Story story = new Story() 
            {
                MissionId = missionId,
                UserId = user.UserId,
                Title = storyTitle,
                Description = storyDescription,
                PublishedAt = publishDate,
            };



            if(status == 0)
            {
                //for SAVE button
                story.Status = "DRAFT";
                _dbContext.Stories.Add(story);
                _dbContext.SaveChanges();
            }
            else
            {
                //for Submit button
                story.Status = "PENDING";
                _dbContext.Stories.Update(story);
                _dbContext.SaveChanges();
            }

            return View();
        }

        #endregion
    }
}
