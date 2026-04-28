using System;
using System.Collections.Generic;

namespace DBApplication.Models;

public partial class Doctor
{
    public string DoctorId { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Specialty { get; set; } = null!;

    public short DeptId { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<Department> Departments { get; set; } = new List<Department>();

    public virtual Department Dept { get; set; } = null!;

    public virtual ICollection<Prescribtion> Prescribtions { get; set; } = new List<Prescribtion>();
}
