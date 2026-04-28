using System;
using System.Collections.Generic;

namespace DBApplication.Models;

public partial class Patient
{
    public string PatientId { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateOnly? Dob { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<PatientPhone> PatientPhones { get; set; } = new List<PatientPhone>();

    public virtual ICollection<Prescribtion> Prescribtions { get; set; } = new List<Prescribtion>();
}
