using CI_platform.Entities.DataModels;
using CI_platform.Repositories.GenericRepository.Interface;
using CI_platform.Repositories.MethodRepository.Interface;
using CI_pltform.Entities.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CI_platform.Controllers
{
    public class CommonController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CommonController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public User GetThisUser()
        {
            var identity = User.Identity as ClaimsIdentity;
            var email = identity?.FindFirst(ClaimTypes.Email)?.Value;

            var user = _unitOfWork.User.GetFirstOrDefault(m => m.Email == email);
            return user;
        }

        //---------------------- Notification Preferences --------------------------//

        #region Notification Preferences

        [HttpPost]
        public void SaveUserNotificationPreferences(long[] notifPref)
        {
            User user = GetThisUser();

            IEnumerable<NotificationPreference> userPrefList = _unitOfWork.NotificationPreference.GetAccToFilter(m => m.UserId == user.UserId);
            _unitOfWork.NotificationPreference.RemoveRange(userPrefList);
            _unitOfWork.Save();

            if (notifPref.Length > 0 && user != null)
            {
                foreach(var preId in notifPref)
                {
                    NotificationPreference userNotificationPreference = new()
                    {
                        UserId = user.UserId,
                        NotiTypeId = preId,
                    };
                    _unitOfWork.NotificationPreference.Add(userNotificationPreference);
                    _unitOfWork.Save();
                }
            }
        }
        
        #endregion

        //---------------------- Notification Read --------------------------//

        #region Notification Read
        
        public void MarkNotAsRead(long notId)
        {
            if(notId > 0)
            {
                NotificationSpecuser notification = _unitOfWork.NotificationSpecuser.GetFirstOrDefault(m => m.NotiSpecId == notId);
                if(notification != null)
                {
                    notification.Isread = 1;
                    _unitOfWork.NotificationSpecuser.Update(notification);
                    _unitOfWork.Save();
                }
            }
        }
        
        #endregion

        //---------------------- Notification --------------------------//

        #region Notification
        
        public IActionResult GetNotificationData()
        {
            User user = GetThisUser();

            IEnumerable<NotificationType> notificationTypeList = _unitOfWork.NotificationType.GetAll();
            IEnumerable<long?> userNotificationPreferenceList = new List<long?>();
            IEnumerable<NotificationSpecuser> notificationToUserList = new List<NotificationSpecuser>();

            if(user != null)
            {
                notificationToUserList = _unitOfWork.NotificationSpecuser.GetAccToFilter(m => m.ToUserId == user.UserId);
                userNotificationPreferenceList = _unitOfWork.NotificationPreference.GetAccToFilter(m => m.UserId == user.UserId).Select(m => m.NotiTypeId);
            }

            if (userNotificationPreferenceList.Any())
            {
                notificationToUserList = notificationToUserList.Where(m => userNotificationPreferenceList.Contains(m.NotiTypeId)).ToList();
            }

            userNotificationData notificationData = new()
            {
                NotificationTypeList = notificationTypeList,
                NotificationToUserList = notificationToUserList,
                NotificationCount = notificationToUserList.Count(),
            };

            if(userNotificationPreferenceList != null)
            {
                notificationData.UserNotifPrefList = userNotificationPreferenceList;
            }

            return Json(notificationData);
        }

        #endregion
    }
}
