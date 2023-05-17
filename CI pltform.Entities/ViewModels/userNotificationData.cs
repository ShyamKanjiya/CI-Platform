using CI_platform.Entities.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CI_pltform.Entities.ViewModels
{
    public class userNotificationData
    {
        public IEnumerable<NotificationType> NotificationTypeList { get; set; } = new List<NotificationType>();
        public IEnumerable<NotificationSpecuser> NotificationToUserList { get; set; } = new List<NotificationSpecuser>();
        public IEnumerable<long?> UserNotifPrefList { get; set; } = new List<long?>();
        public int NotificationCount { get; set; } = 0;
        public int UnreadNotificationCount { get; set; }
    }
}
