using System;
using System.Collections.Generic;

namespace CI_platform.Entities.DataModels;

public partial class MissionApplication
{
    public long MissionApplicationId { get; set; }

    public long MissionId { get; set; }

    public long UserId { get; set; }

    public DateTime AppliedAt { get; set; }

    public string ApprovalStatus { get; set; } = null!;

    public byte[] CreatedAt { get; set; } = null!;

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual Mission Mission { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
