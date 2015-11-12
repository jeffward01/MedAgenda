use MedAgenda;
GO

INSERT INTO DoctorChecks
(DoctorID, ExamRoomID, CheckinDateTime, CheckoutDateTime) 
VALUES (2, 8, '10/01/2015 11:00', '10/01/2015 11:30');

INSERT INTO DoctorChecks
(DoctorID, ExamRoomID, CheckinDateTime) 
VALUES (4, 7, '10/31/2015');
GO

INSERT INTO DoctorChecks
(ExamRoomID, DoctorID, CheckinDateTime, CheckoutDateTime) 
VALUES (9, 5, '10/01/2015 11:00', '10/01/2015 11:30');


GO

select * from DoctorChecks;
