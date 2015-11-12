use MedAgenda;
GO

INSERT INTO PatientChecks
(PatientID, SpecialtyID, CheckinDateTime, CheckoutDateTime) 
VALUES (2, 8, '10/01/2015 11:00', '10/01/2015 11:30');

INSERT INTO PatientChecks
(PatientID, SpecialtyID, CheckinDateTime) 
VALUES (2, 3, '10/31/2015');
GO

INSERT INTO Appointments
(PatientID, DoctorID, ExamRoomID, CheckinDateTime, CheckoutDateTime) 
VALUES (2, 6, 7, '10/01/2015 11:00', '10/01/2015 11:30');

INSERT INTO Appointments
(PatientID, DoctorID, ExamRoomID, CheckinDateTime) 
VALUES (2, 5, 8, '10/31/2015');
GO

select * from PatientChecks;
select * from Appointments;