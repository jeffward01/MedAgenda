use MedAgenda;
GO

UPDATE Patients
SET Archived = 1
WHERE PatientID=10;

UPDATE Patients
SET Archived = 1
WHERE PatientID=15;

SELECT * FROM Patients;