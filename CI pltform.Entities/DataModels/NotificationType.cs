using System;
using System.Collections.Generic;

namespace CI_platform.Entities.DataModels;

public partial class NotificationType
{
    public long NotiTypeId { get; set; }

    public string? NotiType { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<NotificationPreference> NotificationPreferences { get; } = new List<NotificationPreference>();

    public virtual ICollection<NotificationSpecuser> NotificationSpecusers { get; } = new List<NotificationSpecuser>();
}
