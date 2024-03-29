﻿using System;
using System.Collections.Generic;

namespace CI_platform.Entities.DataModels;

public partial class MissionMedium
{
    public long MissionMediaId { get; set; }

    public long MissionId { get; set; }

    public string MediaName { get; set; } = null!;

    public string MediaType { get; set; } = null!;

    public string? MediaPath { get; set; }

    public int Default { get; set; }

    public byte[] CreatedAt { get; set; } = null!;

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual Mission Mission { get; set; } = null!;
}
