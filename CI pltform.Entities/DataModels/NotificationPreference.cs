using System;
using System.Collections.Generic;

namespace CI_platform.Entities.DataModels;

public partial class NotificationPreference
{
    public long NotiPrefId { get; set; }

    public long? UserId { get; set; }

    public long? NotiTypeId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual NotificationType? NotiType { get; set; }

    public virtual User? User { get; set; }
}
