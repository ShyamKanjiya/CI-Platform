using CI_platform.Entities.DataModels;
using CI_platform.Entities.ViewModels;
using CI_pltform.Entities.ViewModels;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Net.Mail;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Security.Principal;
using System.Xml.Schema;

namespace CI_platform.Controllers
{
    public class PagesController : Controller
    {
        private readonly ILogger<PagesController> _logger;
        private readonly CIDbContext _dbContext;

        public PagesController(ILogger<PagesController> logger, CIDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }


        //---------------------- CARD VIEW --------------------------//

        #region Card View
        [HttpGet]
        public IActionResult platformLandingPage()
        {

            userViewModel viewModel = new userViewModel
            {
                Countries = _dbContext.Countries.ToList(),
                Cities = _dbContext.Cities.ToList(),
                MissionThemes = _dbContext.MissionThemes.ToList(),
                Skills = _dbContext.Skills.ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult bringMissions(string[]? country, string[]? cities, string[]? theme, string[]? skill, string? sortBy, string? missionToSearch, int pg = 1)
        {
            var user = GetThisUser();

            userViewModel userView = new userViewModel
            {
                Missions = _dbContext.Missions.ToList(),
                GoalMissions = _dbContext.GoalMissions.ToList(),
                Countries = _dbContext.Countries.ToList(),
                Cities = _dbContext.Cities.ToList(),
                MissionThemes = _dbContext.MissionThemes.ToList(),
                Skills = _dbContext.Skills.ToList(),
                userDetails = GetThisUser(),
                FavoriteMissions = _dbContext.FavoriteMissions.ToList(),
                MissionApplications = _dbContext.MissionApplications.ToList(),
            };

            List<Mission> missions = _dbContext.Missions.ToList();

            if (country.Count() > 0 || cities.Count() > 0 || theme.Count() > 0)
            {
                missions = filterMission(missions, country, cities, theme, skill);
            }

            if (missionToSearch != null)
            {
                missions = missions.Where(m => m.Title.ToLower().Contains(missionToSearch.ToLower())).ToList();
            }

            missions = sortMission(sortBy, missions);

            const int pageSize = 3;
            if (pg < 1)
                pg = 1;

            int recsCount = missions.Count();

            var pager = new userPager(recsCount, pg, pageSize);

            int recSkip = (pg - 1) * pageSize;

            missions = missions.Skip(recSkip).Take(pager.PageSize).ToList();

            this.ViewBag.userPager = pager;

            userView.Missions = missions;

            ViewBag.missionCount = recsCount;

            if (user != null)
            {
                userView.Volunteers = _dbContext.Users.Where(m => m.UserId != user.UserId).ToList();
            }
            else
            {
                userView.Volunteers = _dbContext.Users.ToList();
            }

            foreach (var mission in userView.Missions)
            {
                try
                {
                    userView.RateMission = _dbContext.MissionRatings.Where(r => r.MissionId == mission.MissionId).ToList();

                }
                catch
                {
                    userView.RateMission = null;
                }

            }

            if (recsCount == 0)
            {
                return RedirectToAction("noMissionFound", "Pages");
            }
            else
            {
                return PartialView("_cardView", userView);
            }
        }

        public List<Mission> sortMission(string sortBy, List<Mission> missions)
        {
            switch (sortBy)
            {
                case "Newest":
                    return missions.OrderBy(m => m.StartDate).ToList();

                case "Oldest":
                    return missions.OrderByDescending(m => m.StartDate).ToList();

                case "AZ":
                    return missions.OrderBy(m => m.Title).ToList();

                case "ZA":
                    return missions.OrderByDescending(m => m.Title).ToList();

                case "GOAL":
                    return missions.Where(m => m.MissionType.Equals("GOAL")).ToList();

                case "TIME":
                    return missions.Where(m => m.MissionType.Equals("TIME")).ToList();

                default:
                    return missions.ToList();
            }
        }

        public List<Mission> filterMission(List<Mission> missions, string[] country, string[] cities, string[] theme, string[] skill)
        {
            if (country.Length > 0)
            {
                missions = missions.Where(s => country.Contains(s.Country.Name)).ToList();
            }
            if (cities.Length > 0)
            {
                missions = missions.Where(s => cities.Contains(s.City.Name)).ToList();
            }
            if (theme.Length > 0)
            {
                missions = missions.Where(s => theme.Contains(s.MissionTheme.Title)).ToList();
            }
            /* if (skill.Length > 0)
             {
                 missions = missions.Where(s => skill.Contains(s.)).ToList();
             }*/
            return missions.ToList();
        }


        #endregion

        //---------------------- No Mission Found --------------------------//

        #region No Mission Found
        public IActionResult noMissionFound()
        {
            return View();
        }

        #endregion

        //---------------------- Volunteering Mission Page --------------------------//

        #region Volunteering Mission Page

        public IActionResult volunteeringMissionPage(int id)
        {
            var missionDetail = _dbContext.Missions.Where(m => m.MissionId == id).FirstOrDefault();
            var user = GetThisUser();

            var thismission = _dbContext.Missions.Where(m => m.MissionId.Equals(id)).FirstOrDefault();
            var relatedmissions = _dbContext.Missions.Where(s => s.MissionId != id && (s.CityId == thismission.CityId || s.CountryId == thismission.CountryId || s.MissionThemeId == thismission.MissionThemeId)).Take(3).ToList();
            var missionRatings = GetMissionRatings(id);

            var cities = _dbContext.Cities.ToList();
            var countries = _dbContext.Countries.ToList();
            var missionTheme = _dbContext.MissionThemes.ToList();
            var goalMission = _dbContext.GoalMissions.Where(m => m.MissionId == id).FirstOrDefault();
            var goal = _dbContext.GoalMissions.ToList();
            var favouriteMission = _dbContext.FavoriteMissions.ToList();
            var commentList = _dbContext.Comments.Where(m => m.MissionId == id).OrderByDescending(m => m.CreatedAt).ToList();
            var missionApp = _dbContext.MissionApplications.Where(m => m.MissionId == id && m.ApprovalStatus == "APPROVE").ToList();
            var missionDocuments = _dbContext.MissionDocuments.Where(m => m.MissionId == id).ToList();


            userVolunteerMission viewModel = new()
            {
                MissionDetail = missionDetail,
                Cities = cities,
                Countries = countries,
                MissionThemes = missionTheme,
                GoalMissions = goalMission,
                RelatedMissions = relatedmissions,
                Goal = goal,
                userDetails = user,
                favoriteMissions = favouriteMission,
                commentList = commentList,
                MissionApp = missionApp,
                MissionDocuments = missionDocuments,
            };

            if (user != null)
            {
                viewModel.Volunteers = _dbContext.Users.Where(m => m.UserId != user.UserId).ToList();
                viewModel.MissionApplications = _dbContext.MissionApplications.Where(m => m.MissionId == id && m.UserId == user.UserId).FirstOrDefault();
            }
            else
            {
                viewModel.Volunteers = _dbContext.Users.ToList();
            }

            if (viewModel.userDetails != null)
            {
                viewModel.RateMission = _dbContext.MissionRatings.Where(m => m.UserId == viewModel.userDetails.UserId && m.MissionId == id).FirstOrDefault();
            }

            try
            {
                viewModel.RatedVolunteeres = missionRatings.Count();
                viewModel.Missionrate = (int)missionRatings.Average(m => m.Rating);
            }
            catch
            {
                viewModel.RatedVolunteeres = 0;
                viewModel.Missionrate = 0;
            };

            return View(viewModel);
        }

        public User GetThisUser()
        {
            var identity = User.Identity as ClaimsIdentity;
            var email = identity?.FindFirst(ClaimTypes.Email)?.Value;

            var user = _dbContext.Users.Where(m => m.Email == email).FirstOrDefault();
            return user;
        }

        public void AddToFavourite(int missionId)
        {
            var user = GetThisUser();
            var favMission = _dbContext.FavoriteMissions.Where(m => m.UserId.Equals(user.UserId) && m.MissionId == missionId).FirstOrDefault();

            if (favMission == null)
            {
                var favoriteMission = new FavoriteMission()
                {
                    UserId = user.UserId,
                    MissionId = missionId
                };

                _dbContext.Add(favoriteMission);
                _dbContext.SaveChanges();
            }
            else
            {
                _dbContext.Remove(favMission);
                _dbContext.SaveChanges();
            }
        }

        public IActionResult AddComment(int missionId, string? comment_area)
        {
            var user = GetThisUser();
            Comment obj = new()
            {
                CommentText = comment_area,
                UserId = user.UserId,
                MissionId = (long)missionId,
                CreatedAt = DateTime.UtcNow
            };
            _dbContext.Comments.Add(obj);
            _dbContext.SaveChanges();

            return RedirectToAction("volunteeringMissionPage", new { id = missionId });
        }

        public void RecommandToCoworkers(int[]? userIds, long missionId)
        {
            var thisUser = GetThisUser();
            if (userIds != null)
            {
                foreach (var id in userIds)
                {
                    var inviteLink = Url.Action("volunteeringMissionPage", "Home", new { id = missionId }, Request.Scheme);
                    var user = _dbContext.Users.Where(m => m.UserId == id).FirstOrDefault();


                    MissionInvite obj = new()
                    {
                        MissionId = missionId,
                        ToUserId = user.UserId,
                        FromUserId = thisUser.UserId
                    };
                    _dbContext.Add(obj);
                    _dbContext.SaveChanges();



                    var fromAddress = new MailAddress("kanjiyashyam15@gmail.com", "Shyam Kanjiya");
                    var toAddress = new MailAddress(user.Email);
                    var subject = "Mission Invite";
                    var body = $"Hi,<br /><br /> you are invited by your friend to enroll to the mission at CIPlatform.<br /><br />Click the following link to get the details of mission,<br /><br /><a href='{inviteLink}'>{inviteLink}</a>";


                    var message = new MailMessage(fromAddress, toAddress)
                    {
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    };

                    var smtpClient = new SmtpClient("smtp.gmail.com", 587)
                    {
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential("kanjiyashyam15@gmail.com", "mbxfpwtiaztubcng"),
                        EnableSsl = true
                    };
                    smtpClient.Send(message);
                }
            }
        }

        public IEnumerable<MissionRating> GetMissionRatings(int id)
        {
            var missionratings = new List<MissionRating>();
            if (id != 0)
            {
                missionratings = _dbContext.MissionRatings.Where(m => m.MissionId == id).ToList();
            }
            return missionratings;
        }

        public IActionResult UpdateAndAddRate(int missionId, int rating)
        {
            var user = GetThisUser();
            var mission_rating = _dbContext.MissionRatings.Include(m => m.Mission).Include(m => m.User).ToList();
            var rate_update = mission_rating.SingleOrDefault(m => m.User.UserId == user.UserId && m.Mission.MissionId == missionId);

            //Update Rating
            if (rate_update != null)
            {
                rate_update.UpdatedAt = DateTime.Now;
                rate_update.Rating = rating;
                _dbContext.Update(rate_update);
                _dbContext.SaveChanges();

            }

            //Add Rating for the first time user
            if (rate_update == null)
            {
                var userId = _dbContext.Users.Where(u => u.UserId == user.UserId).Select(u => u.UserId).FirstOrDefault();
                var missionrating = new MissionRating
                {
                    MissionId = missionId,
                    UserId = userId,
                    Rating = rating,
                };

                _dbContext.Add(missionrating);
                _dbContext.SaveChanges();
            }

            return RedirectToAction("volunteeringMissionPage", new { id = missionId });
        }

        public IActionResult applyMission(int missionId)
        {
            var thisUser = GetThisUser();
            var status = _dbContext.MissionApplications.Where(m => m.MissionId == missionId && m.UserId == thisUser.UserId).FirstOrDefault();
            if (status.ApprovalStatus == "DECLINE")
            {
                status.ApprovalStatus = "PENDING";
                status.AppliedAt = DateTime.Now;
                status.UpdatedAt = DateTime.Now;
                _dbContext.Update(status);
                _dbContext.SaveChanges();
            }
            else
            {
                MissionApplication obj = new()
                {
                    MissionId = missionId,
                    UserId = thisUser.UserId,
                    AppliedAt = DateTime.Now
                };
                _dbContext.MissionApplications.Add(obj);
                _dbContext.SaveChanges();
            }

            return RedirectToAction("volunteeringMissionPage", new { id = missionId });
        }

        public IActionResult volunteerPage(int pg, int id)
        {
            var missionApp = _dbContext.MissionApplications.Where(m => m.MissionId == id && m.ApprovalStatus == "APPROVE").ToList();


            userVolunteerMission viewModel = new()
            {
                Volunteers = _dbContext.Users.ToList(),
                MissionApp = missionApp,
                MissionDetail = _dbContext.Missions.Where(m => m.MissionId == id).FirstOrDefault()
            };

            const int pageSize = 9;
            if (pg < 1)
                pg = 1;

            int recsCount = missionApp.Count();

            var pager = new userPager(recsCount, pg, pageSize);

            int recSkip = (pg - 1) * pageSize;

            missionApp = missionApp.Skip(recSkip).Take(pager.PageSize).ToList();

            this.ViewBag.userPager = pager;

            viewModel.MissionApp = missionApp;

            return PartialView("_VolunteerList", viewModel);
        }

        #endregion Volunteering Mission Page

        
    }
}
