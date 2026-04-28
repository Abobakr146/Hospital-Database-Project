using System;
using System.Collections.Generic;

namespace DBApplication.Models;

public partial class Prescribtion
{
    public string PatientId { get; set; } = null!;

    public short MedCode { get; set; }

    public string DocId { get; set; } = null!;

    public DateTime PrescribtionDate { get; set; }

    public virtual Doctor Doc { get; set; } = null!;

    public virtual Medication MedCodeNavigation { get; set; } = null!;

    public virtual Patient Patient { get; set; } = null!;
}
