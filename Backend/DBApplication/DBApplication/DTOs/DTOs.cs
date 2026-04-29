namespace DBApplication.DTOs
{
    public class AppointmentDto
    {
        public DateOnly ApptDate { get; set; }
        public TimeOnly ApptTime { get; set; }
        public string Status { get; set; } = null!;
        public string PatientId { get; set; } = null!;
        public string DoctorId { get; set; } = null!;
    }

    public class DepartmentDto
    {
        public string DeptName { get; set; } = null!;
        public string? Location { get; set; }
        public string? Manager_DocID { get; set; }
    }

    public class DoctorDto
    {
        public string DoctorId { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Specialty { get; set; } = null!;
        public short DeptId { get; set; }
    }

    public class MedicationDto
    {
        public string MedName { get; set; } = null!;
        public decimal Dosage { get; set; }
        public string Unit { get; set; } = null!;
    }

    public class PatientDto
    {
        public string PatientId { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public DateOnly? DOB { get; set; }
    }

    public class PatientPhoneDto
    {
        public string PatientId { get; set; } = null!;
        public string Phone { get; set; } = null!;
    }

    public class PrescribtionDto
    {
        public string PatientId { get; set; } = null!;
        public short MedCode { get; set; }
        public string DocId { get; set; } = null!;
        public DateTime PrescribtionDate { get; set; }
    }
}