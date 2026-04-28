using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DBApplication.Models;

public partial class HospitalDbContext : DbContext
{
    public HospitalDbContext()
    {
    }

    public HospitalDbContext(DbContextOptions<HospitalDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Doctor> Doctors { get; set; }

    public virtual DbSet<Medication> Medications { get; set; }

    public virtual DbSet<Patient> Patients { get; set; }

    public virtual DbSet<PatientPhone> PatientPhones { get; set; }

    public virtual DbSet<Prescribtion> Prescribtions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.;Database=HospitalDB;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.ApptId).HasName("PK__Appointm__EDACF695C68802B5");

            entity.ToTable("Appointment");

            entity.Property(e => e.ApptId).HasColumnName("ApptID");
            entity.Property(e => e.ApptTime).HasPrecision(0);
            entity.Property(e => e.DoctorId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("DoctorID");
            entity.Property(e => e.PatientId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("PatientID");
            entity.Property(e => e.Status).HasMaxLength(10);

            entity.HasOne(d => d.Doctor).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_doctor_appointment");

            entity.HasOne(d => d.Patient).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_patient_appointment");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.DeptId).HasName("PK__Departme__0148818EEF209936");

            entity.ToTable("Department");

            entity.Property(e => e.DeptId).HasColumnName("DeptID");
            entity.Property(e => e.DeptName).HasMaxLength(50);
            entity.Property(e => e.Location).HasMaxLength(50);
            entity.Property(e => e.ManagerDocId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Manager_DocID");

            entity.HasOne(d => d.ManagerDoc).WithMany(p => p.Departments)
                .HasForeignKey(d => d.ManagerDocId)
                .HasConstraintName("fk_doctor_department");
        });

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.HasKey(e => e.DoctorId).HasName("PK__Doctor__2DC00EDF8FA8C872");

            entity.ToTable("Doctor");

            entity.Property(e => e.DoctorId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("DoctorID");
            entity.Property(e => e.DeptId).HasColumnName("DeptID");
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.Specialty).HasMaxLength(40);

            entity.HasOne(d => d.Dept).WithMany(p => p.Doctors)
                .HasForeignKey(d => d.DeptId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_department_doctor");
        });

        modelBuilder.Entity<Medication>(entity =>
        {
            entity.HasKey(e => e.MedCode).HasName("PK__Medicati__B7F94872CEBFC0C5");

            entity.ToTable("Medication");

            entity.Property(e => e.Dosage).HasColumnType("decimal(10, 3)");
            entity.Property(e => e.MedName).HasMaxLength(50);
            entity.Property(e => e.Unit).HasMaxLength(10);
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.PatientId).HasName("PK__Patient__970EC3463076C374");

            entity.ToTable("Patient");

            entity.Property(e => e.PatientId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("PatientID");
            entity.Property(e => e.Dob).HasColumnName("DOB");
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
        });

        modelBuilder.Entity<PatientPhone>(entity =>
        {
            entity.HasKey(e => new { e.PatientId, e.Phone }).HasName("PK__Patient___72C9201F70C05C58");

            entity.ToTable("Patient_Phone");

            entity.Property(e => e.PatientId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("PatientID");
            entity.Property(e => e.Phone)
                .HasMaxLength(14)
                .IsUnicode(false);

            entity.HasOne(d => d.Patient).WithMany(p => p.PatientPhones)
                .HasForeignKey(d => d.PatientId)
                .HasConstraintName("fk_patient_patientPhone");
        });

        modelBuilder.Entity<Prescribtion>(entity =>
        {
            entity.HasKey(e => new { e.PatientId, e.MedCode, e.DocId }).HasName("PK__Prescrib__204FA64941AD9E7C");

            entity.ToTable("Prescribtion");

            entity.Property(e => e.PatientId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("PatientID");
            entity.Property(e => e.DocId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("DocID");
            entity.Property(e => e.PrescribtionDate).HasPrecision(0);

            entity.HasOne(d => d.Doc).WithMany(p => p.Prescribtions)
                .HasForeignKey(d => d.DocId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_doctor_prescribtion");

            entity.HasOne(d => d.MedCodeNavigation).WithMany(p => p.Prescribtions)
                .HasForeignKey(d => d.MedCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_medication_prescribtion");

            entity.HasOne(d => d.Patient).WithMany(p => p.Prescribtions)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_patient_prescribtion");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
