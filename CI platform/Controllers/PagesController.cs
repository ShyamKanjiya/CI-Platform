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
using CI_platform.Repositories.GenericRepository.Interface;

namespace CI_platform.Controllers
{
    public class PagesController : Controller
    {
        private readonly ILogger<PagesController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public PagesController(ILogger<PagesController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        //---------------------- Platform Landing Page --------------------------//

        #region Platform Landing Page
        [HttpGet]
        public IActionResult platformLandingPage()
        {
            User user = GetThisUser();

            userViewModel viewModel = new userViewModel
            {
                UserDetails = user,
                Countries = _unitOfWork.Country.GetAll(),
                Cities = _unitOfWork.City.GetAll(),
                MissionThemes = _unitOfWork.MissionTheme.GetAll(),
                Skills = _unitOfWork.Skill.GetAll(),
            };
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult bringMissions(string[]? country, string[]? cities, string[]? theme, string[]? skill, string? sortBy, string? missionToSearch, int pg = 1)
        {
            var user = GetThisUser();

            userViewModel userView = new userViewModel
            {
                Missions = _unitOfWork.Mission.GetAllMissions(),
                GoalMissions = _unitOfWork.GoalMission.GetAll(),
                Countries = _unitOfWork.Country.GetAll(),
                Cities = _unitOfWork.City.GetAll(),
                MissionThemes = _unitOfWork.MissionTheme.GetAll(),
                Skills = _unitOfWork.Skill.GetAll(),
                UserDetails = GetThisUser(),
                FavoriteMissions = _unitOfWork.FavouriteMission.GetAll(),
                MissionApplications = _unitOfWork.MissionApplication.GetAll(),
            };

            List<Mission> missions = (List<Mission>)_unitOfWork.Mission.GetAllMissions();


            if (country.Count() > 0 || cities.Count() > 0 || theme.Count() > 0 || skill.Count() > 0)
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
                userView.Volunteers = _unitOfWork.User.GetAccToFilter(m => m.UserId != user.UserId);
            }
            else
            {
                userView.Volunteers = _unitOfWork.User.GetAll();
            }

            foreach (var mission in userView.Missions)
            {
                try
                {
                    userView.RateMission = _unitOfWork.MissionRating.GetAccToFilter(r => r.MissionId == mission.MissionId);
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

        public List<Mission> filterMission(List<Mission> missions, string[] country, string[] cities, string[] theme, string[] skills)
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
            if (skills.Length > 0)
            {
                missions = missions.Where(mission => mission.MissionSkills.Any(skill => skills.Contains(skill.Skill.SkillName))).ToList();
            }
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
            var missionDetail = _unitOfWork.Mission.GetFirstOrDefault(m => m.MissionId == id);
            var user = GetThisUser();

            var thismission = _unitOfWork.Mission.GetFirstOrDefault(m => m.MissionId.Equals(id));
            var relatedmissions = _unitOfWork.Mission.GetAccToFilter(s => s.MissionId != id && (s.CityId == thismission.CityId || s.CountryId == thismission.CountryId || s.MissionThemeId == thismission.MissionThemeId)).Take(3);
            var missionRatings = GetMissionRatings(id);

            var cities = _unitOfWork.City.GetAll();
            var countries = _unitOfWork.Country.GetAll();
            var missionTheme = _unitOfWork.MissionTheme.GetAll();
            var goalMission = _unitOfWork.GoalMission.GetFirstOrDefault(m => m.MissionId == id);
            var goal = _unitOfWork.GoalMission.GetAll();
            IEnumerable<FavoriteMission> favouriteMission = _unitOfWork.FavouriteMission.GetAll();

            IEnumerable<Comment> comments = _unitOfWork.Comment.GetAccToFilter(m => m.MissionId == id).OrderByDescending(m => m.CreatedAt);
            var missionApp = _unitOfWork.MissionApplication.GetAccToFilter(m => m.MissionId == id && m.ApprovalStatus == "APPROVE");
            var missionDocuments = _unitOfWork.MissionDocument.GetAccToFilter(m => m.MissionId == id);


            userVolunteerMission viewModel = new()
            {
                MissionDetail = missionDetail,
                Cities = cities,
                Countries = countries,
                MissionThemes = missionTheme,
                GoalMissions = goalMission,
                RelatedMissions = relatedmissions,
                Goal = goal,
                UserDetails = user,
                commentList = comments,
                favoriteMissions = favouriteMission,
                MissionApp = missionApp,
                MissionDocuments = missionDocuments,
            };


            if (user != null)
            {
                viewModel.Volunteers = _unitOfWork.User.GetAccToFilter(m => m.UserId != user.UserId);
                viewModel.MissionApplications = _unitOfWork.MissionApplication.GetFirstOrDefault(m => m.MissionId == id && m.UserId == user.UserId);
            }
            else
            {
                viewModel.Volunteers = _unitOfWork.User.GetAll();
            }

            if (viewModel.UserDetails != null)
            {
                viewModel.RateMission = _unitOfWork.MissionRating.GetFirstOrDefault(m => m.UserId == viewModel.UserDetails.UserId && m.MissionId == id);
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

            var user = _unitOfWork.User.GetFirstOrDefault(m => m.Email == email);
            return user;
        }

        public void AddToFavourite(int missionId)
        {
            var user = GetThisUser();
            var favMission = _unitOfWork.FavouriteMission.GetFirstOrDefault(m => m.UserId.Equals(user.UserId) && m.MissionId == missionId);

            if (favMission == null)
            {
                var favoriteMission = new FavoriteMission()
                {
                    UserId = user.UserId,
                    MissionId = missionId
                };

                _unitOfWork.FavouriteMission.Add(favoriteMission);
                _unitOfWork.Save();
            }
            else
            {
                _unitOfWork.FavouriteMission.Remove(favMission);
                _unitOfWork.Save();
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
            _unitOfWork.Comment.Add(obj);
            _unitOfWork.Save();

            return RedirectToAction("volunteeringMissionPage", new { id = missionId });
        }

        public void RecommandToCoworkers(int[]? userIds, long missionId)
        {
            var thisUser = GetThisUser();
            if (userIds != null)
            {
                foreach (var id in userIds)
                {
                    var inviteLink = Url.Action("volunteeringMissionPage", "Pages", new { id = missionId }, Request.Scheme);
                    var user = _unitOfWork.User.GetFirstOrDefault(m => m.UserId == id);


                    MissionInvite obj = new()
                    {
                        MissionId = missionId,
                        ToUserId = user.UserId,
                        FromUserId = thisUser.UserId
                    };
                    _unitOfWork.MissionInvite.Add(obj);
                    _unitOfWork.Save();



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
                        Credentials = new NetworkCredential("kanjiyashyam15@gmail.com", "nfuugkmtxtjcnect"),
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
                missionratings = _unitOfWork.MissionRating.GetAccToFilter(m => m.MissionId == id);
            }
            return missionratings;
        }

        public IActionResult UpdateAndAddRate(int missionId, int rating)
        {
            var user = GetThisUser();
            var rate_update = _unitOfWork.MissionRating.GetFirstOrDefault(m => m.User.UserId == user.UserId && m.Mission.MissionId == missionId);

            //Update Rating
            if (rate_update != null)
            {
                rate_update.UpdatedAt = DateTime.Now;
                rate_update.Rating = rating;
                _unitOfWork.MissionRating.Update(rate_update);
                _unitOfWork.Save();

            }

            //Add Rating for the first time user
            if (rate_update == null)
            {
                var missionrating = new MissionRating
                {
                    MissionId = missionId,
                    UserId = user.UserId,
                    Rating = rating,
                };

                _unitOfWork.MissionRating.Add(missionrating);
                _unitOfWork.Save();
            }

            return RedirectToAction("volunteeringMissionPage", new { id = missionId });
        }

        public IActionResult applyMission(int missionId)
        {
            var thisUser = GetThisUser();
            var status = _unitOfWork.MissionApplication.GetFirstOrDefault(m => m.MissionId == missionId && m.UserId == thisUser.UserId);
            if (status != null)
            {
                if (status.ApprovalStatus == "DECLINE")
                {
                    status.ApprovalStatus = "PENDING";
                    status.AppliedAt = DateTime.Now;
                    status.UpdatedAt = DateTime.Now;
                    _unitOfWork.MissionApplication.Update(status);
                    _unitOfWork.Save();
                }
            }
            else
            {
                MissionApplication obj = new()
                {
                    MissionId = missionId,
                    UserId = thisUser.UserId,
                    AppliedAt = DateTime.Now
                };
                _unitOfWork.MissionApplication.Add(obj);
                _unitOfWork.Save();
            }

            return RedirectToAction("volunteeringMissionPage", new { id = missionId });
        }

        public IActionResult volunteerPage(int pg, int id)
        {
            var missionApp = _unitOfWork.MissionApplication.GetAccToFilter(m => m.MissionId == id && m.ApprovalStatus == "APPROVE");


            userVolunteerMission viewModel = new()
            {
                Volunteers = _unitOfWork.User.GetAll(),
                MissionApp = missionApp,
                MissionDetail = _unitOfWork.Mission.GetFirstOrDefault(m => m.MissionId == id)
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
