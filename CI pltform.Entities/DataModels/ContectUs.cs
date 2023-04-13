using System;
using System.Collections.Generic;

namespace CI_platform.Entities.DataModels;

public partial class ContectUs
{
    public long ContectId { get; set; }

    public long UserId { get; set; }

    public string Subject { get; set; } = null!;

    public string Message { get; set; } = null!;

    public byte[] CreatedAt { get; set; } = null!;

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
