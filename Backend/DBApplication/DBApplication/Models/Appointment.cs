using System;
using System.Collections.Generic;

namespace DBApplication.Models;

public partial class Appointment
{
    public short ApptId { get; set; }

    public DateOnly ApptDate { get; set; }

    public TimeOnly ApptTime { get; set; }

    public string Status { get; set; } = null!;

    public string PatientId { get; set; } = null!;

    public string DoctorId { get; set; } = null!;

    public virtual Doctor Doctor { get; set; } = null!;

    public virtual Patient Patient { get; set; } = null!;
}
