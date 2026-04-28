using System;
using System.Collections.Generic;

namespace DBApplication.Models;

public partial class PatientPhone
{
    public string PatientId { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public virtual Patient Patient { get; set; } = null!;
}
