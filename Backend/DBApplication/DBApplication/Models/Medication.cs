using System;
using System.Collections.Generic;

namespace DBApplication.Models;

public partial class Medication
{
    public short MedCode { get; set; }

    public string MedName { get; set; } = null!;

    public decimal Dosage { get; set; }

    public string Unit { get; set; } = null!;

    public virtual ICollection<Prescribtion> Prescribtions { get; set; } = new List<Prescribtion>();
}
