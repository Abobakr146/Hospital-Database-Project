-- disable Doctor and Department tables' constraints temporarily to drop tables
IF OBJECT_ID('fk_department_doctor', 'f') IS NOT NULL
	ALTER TABLE Doctor DROP CONSTRAINT fk_department_doctor;
IF OBJECT_ID('fk_doctor_department', 'f') IS NOT NULL
	ALTER TABLE Department DROP CONSTRAINT fk_doctor_department;

-- DROP tables if exist
DROP TABLE IF EXISTS Appointment;
DROP TABLE IF EXISTS Prescribtion;
DROP TABLE IF EXISTS Patient_Phone;
DROP TABLE IF EXISTS Doctor;
DROP TABLE IF EXISTS Department;
DROP TABLE IF EXISTS Medication;
DROP TABLE IF EXISTS Patient;

--CREATE tables

CREATE TABLE Patient(
	PatientID VARCHAR(50) NOT NULL PRIMARY KEY,
	FirstName NVARCHAR(50) NOT NULL,
	LastName NVARCHAR(50) NOT NULL,
	DOB DATE
);

CREATE TABLE Patient_Phone(
	PatientID VARCHAR(50) NOT NULL, -- FK to Patient Table, composite key
	Phone VARCHAR(14) NOT NULL, -- composite key
	PRIMARY KEY(PatientID, Phone),
	CONSTRAINT fk_patient_patientPhone
		FOREIGN KEY (PatientID) REFERENCES Patient(PatientID) ON DELETE CASCADE
);

CREATE TABLE Medication(
	MedCode SMALLINT IDENTITY(1,1) PRIMARY KEY,
	MedName NVARCHAR(50) NOT NULL,
	Dosage DECIMAL(10, 3) NOT NULL,
	Unit NVARCHAR(10) NOT NULL
);

CREATE TABLE Department(
	DeptID SMALLINT IDENTITY(1,1) PRIMARY KEY,
	DeptName NVARCHAR(50) NOT NULL,
	"Location" NVARCHAR(50),
	Manager_DocID VARCHAR(50) NULL, -- FK to Doctor Table (constraint added at the end, in the ALTER query)
);

CREATE TABLE Doctor(
	DoctorID VARCHAR(50) NOT NULL PRIMARY KEY,
	FirstName NVARCHAR(50) NOT NULL,
	LastName NVARCHAR(50) NOT NULL,
	Specialty NVARCHAR(40) NOT NULL,
	DeptID SMALLINT NOT NULL, -- FK to Department Table
	CONSTRAINT fk_department_doctor
		FOREIGN KEY (DeptID) REFERENCES Department(DeptID)
);

CREATE TABLE Appointment(
	ApptID SMALLINT IDENTITY(1,1) PRIMARY KEY,
	ApptDate DATE NOT NULL,
	ApptTime TIME(0) NOT NULL,
	"Status" NVARCHAR(10) NOT NULL,
	PatientID VARCHAR(50) NOT NULL, -- FK to Patient Table
	DoctorID VARCHAR(50) NOT NULL, -- FK to Doctor Table
	CONSTRAINT fk_doctor_appointment
		FOREIGN KEY (DoctorID) REFERENCES Doctor(DoctorID),
	CONSTRAINT fk_patient_appointment
		FOREIGN KEY (PatientID) REFERENCES Patient(PatientID)
);

CREATE TABLE Prescribtion(
	PatientID VARCHAR(50) NOT NULL, -- FK to Patient Table, composite key
	MedCode SMALLINT NOT NULL, -- FK to Medicine table, composite key
	DocID VARCHAR(50) NOT NULL, -- FK to Doctor table, composite key
	PrescribtionDate DATETIME2(0) NOT NULL,
	PRIMARY KEY (PatientID, MedCode, DocID),
	CONSTRAINT fk_patient_prescribtion
		FOREIGN KEY (PatientID) REFERENCES Patient(PatientID),
	CONSTRAINT fk_medication_prescribtion
		FOREIGN KEY (MedCode) REFERENCES Medication(MedCode),
		CONSTRAINT fk_doctor_prescribtion
		FOREIGN KEY (DocID) REFERENCES Doctor(DoctorID)
);

-- ALTER table Department to include the foreign key constraint
ALTER TABLE Department
	ADD CONSTRAINT fk_doctor_department
		FOREIGN KEY (Manager_DocID) REFERENCES Doctor(DoctorID);

-- INSERT rows in each table (seeding)

INSERT INTO Patient (PatientID, FirstName, LastName, DOB) VALUES
	('123456789', 'Abobakr', 'Mostafa', '2003-09-26'),
	('234567891', 'Mahmoud', 'Essam', '2002-05-08'),
	('345678912', 'Abdullah', 'Mohamed', '2003-06-12');

INSERT INTO Patient_Phone (PatientID, Phone) VALUES
	('123456789', '01185743691'),
	('234567891', '01125364897'),
	('345678912', '01002543687');

INSERT INTO Department(DeptName, "Location") VALUES
	(N'جراحة', 'Floor1'),
	(N'عظام', 'Floor2'),
	(N'اعصاب', 'Floor3');

INSERT INTO Doctor (DoctorID, FirstName, LastName, Specialty, DeptID) VALUES
	('456789123', 'Amr', 'Khaled', N'جراحة', 1),
	('567891234', 'Abdelrahman', 'Ashour', N'عظام', 2),
	('678912345', 'Ali', 'ElSayed', N'جراحة', 1);

UPDATE Department
	SET Manager_DocID = CASE
	WHEN DeptName = N'جراحة' THEN '678912345'
	WHEN DeptName = N'عظام' THEN '567891234'
END
WHERE DeptName IN (N'جراحة', N'عظام');

INSERT INTO Medication (MedName, Dosage, Unit) VALUES
	('Panadol', 6, 'Capsules'),
	('Cataflam', 4, 'Capsules'),
	('Congestal', 5, 'ml');

INSERT INTO Appointment (ApptDate, ApptTime, "Status", PatientID, DoctorID) VALUES
	('2026-03-06', '12:30:00 PM', 'Completed', '123456789', '456789123'),
	('2026-05-07', '02:00:00 PM', 'Completed', '234567891', '567891234'),
	('2026-04-27', '03:15:00 PM', 'Cancelled', '345678912', '678912345');

INSERT INTO Prescribtion (PatientID, MedCode, DocID, PrescribtionDate) VALUES
	('123456789', 1, '456789123', '2026-03-06 12:50:00 PM'),
	('234567891', 2, '567891234', '2026-05-07 02:30:00 PM'),
	('345678912', 3, '678912345', '2026-04-30 03:15:00 PM')