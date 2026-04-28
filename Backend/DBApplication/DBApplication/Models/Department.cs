using System;
using System.Collections.Generic;

namespace DBApplication.Models;

public partial class Department
{
    public short DeptId { get; set; }

    public string DeptName { get; set; } = null!;

    public string? Location { get; set; }

    public string? ManagerDocId { get; set; }

    public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();

    public virtual Doctor? ManagerDoc { get; set; }
}
