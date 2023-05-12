using System;
using System.Collections.Generic;

namespace CI_platform.Entities.DataModels;

public partial class NotificationSpecuser
{
    public long NotiSpecId { get; set; }

    public long? NotiTypeId { get; set; }

    public string? Notification { get; set; }

    public string? Url { get; set; }

    public long? FromUserId { get; set; }

    public long? ToUserId { get; set; }

    public byte Isread { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User? FromUser { get; set; }

    public virtual NotificationType? NotiType { get; set; }

    public virtual User? ToUser { get; set; }
}
