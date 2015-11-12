namespace MedAgenda.CORE.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Appointments",
                c => new
                    {
                        AppointmentID = c.Int(nullable: false, identity: true),
                        PatientID = c.Int(nullable: false),
                        DoctorID = c.Int(nullable: false),
                        ExamRoomID = c.Int(nullable: false),
                        CheckinDateTime = c.DateTime(nullable: false),
                        CheckoutDateTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.AppointmentID)
                .ForeignKey("dbo.Doctors", t => t.DoctorID, cascadeDelete: true)
                .ForeignKey("dbo.Patients", t => t.PatientID, cascadeDelete: true)
                .ForeignKey("dbo.ExamRooms", t => t.ExamRoomID, cascadeDelete: true)
                .Index(t => t.PatientID)
                .Index(t => t.DoctorID)
                .Index(t => t.ExamRoomID);
            
            CreateTable(
                "dbo.Doctors",
                c => new
                    {
                        DoctorID = c.Int(nullable: false, identity: true),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Email = c.String(),
                        Telephone = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        SpecialtyID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.DoctorID)
                .ForeignKey("dbo.Specialties", t => t.SpecialtyID, cascadeDelete: true)
                .Index(t => t.SpecialtyID);
            
            CreateTable(
                "dbo.DoctorChecks",
                c => new
                    {
                        DoctorCheckID = c.Int(nullable: false, identity: true),
                        DoctorID = c.Int(nullable: false),
                        ExamRoomID = c.Int(nullable: false),
                        CheckinDateTime = c.DateTime(nullable: false),
                        CheckoutDateTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.DoctorCheckID)
                .ForeignKey("dbo.Doctors", t => t.DoctorID, cascadeDelete: true)
                .ForeignKey("dbo.ExamRooms", t => t.ExamRoomID, cascadeDelete: true)
                .Index(t => t.DoctorID)
                .Index(t => t.ExamRoomID);
            
            CreateTable(
                "dbo.ExamRooms",
                c => new
                    {
                        ExamRoomID = c.Int(nullable: false, identity: true),
                        ExamRoomName = c.String(),
                    })
                .PrimaryKey(t => t.ExamRoomID);
            
            CreateTable(
                "dbo.Specialties",
                c => new
                    {
                        SpecialtyID = c.Int(nullable: false, identity: true),
                        SpecialtyName = c.String(),
                    })
                .PrimaryKey(t => t.SpecialtyID);
            
            CreateTable(
                "dbo.PatientChecks",
                c => new
                    {
                        PatientCheckID = c.Int(nullable: false, identity: true),
                        PatientID = c.Int(nullable: false),
                        SpecialtyID = c.Int(nullable: false),
                        CheckinDateTime = c.DateTime(nullable: false),
                        CheckoutDateTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.PatientCheckID)
                .ForeignKey("dbo.Patients", t => t.PatientID, cascadeDelete: true)
                .ForeignKey("dbo.Specialties", t => t.SpecialtyID, cascadeDelete: true)
                .Index(t => t.PatientID)
                .Index(t => t.SpecialtyID);
            
            CreateTable(
                "dbo.Patients",
                c => new
                    {
                        PatientID = c.Int(nullable: false, identity: true),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Birthdate = c.DateTime(nullable: false),
                        Telephone = c.String(),
                        Email = c.String(),
                        BloodType = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.PatientID);
            
            CreateTable(
                "dbo.EmergencyContacts",
                c => new
                    {
                        EmergencyContactID = c.Int(nullable: false, identity: true),
                        PatientID = c.Int(nullable: false),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Telephone = c.String(),
                        Email = c.String(),
                        Relationship = c.String(),
                    })
                .PrimaryKey(t => t.EmergencyContactID)
                .ForeignKey("dbo.Patients", t => t.PatientID, cascadeDelete: true)
                .Index(t => t.PatientID);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Appointments", "ExamRoomID", "dbo.ExamRooms");
            DropForeignKey("dbo.Doctors", "SpecialtyID", "dbo.Specialties");
            DropForeignKey("dbo.PatientChecks", "SpecialtyID", "dbo.Specialties");
            DropForeignKey("dbo.PatientChecks", "PatientID", "dbo.Patients");
            DropForeignKey("dbo.EmergencyContacts", "PatientID", "dbo.Patients");
            DropForeignKey("dbo.Appointments", "PatientID", "dbo.Patients");
            DropForeignKey("dbo.DoctorChecks", "ExamRoomID", "dbo.ExamRooms");
            DropForeignKey("dbo.DoctorChecks", "DoctorID", "dbo.Doctors");
            DropForeignKey("dbo.Appointments", "DoctorID", "dbo.Doctors");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.EmergencyContacts", new[] { "PatientID" });
            DropIndex("dbo.PatientChecks", new[] { "SpecialtyID" });
            DropIndex("dbo.PatientChecks", new[] { "PatientID" });
            DropIndex("dbo.DoctorChecks", new[] { "ExamRoomID" });
            DropIndex("dbo.DoctorChecks", new[] { "DoctorID" });
            DropIndex("dbo.Doctors", new[] { "SpecialtyID" });
            DropIndex("dbo.Appointments", new[] { "ExamRoomID" });
            DropIndex("dbo.Appointments", new[] { "DoctorID" });
            DropIndex("dbo.Appointments", new[] { "PatientID" });
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.EmergencyContacts");
            DropTable("dbo.Patients");
            DropTable("dbo.PatientChecks");
            DropTable("dbo.Specialties");
            DropTable("dbo.ExamRooms");
            DropTable("dbo.DoctorChecks");
            DropTable("dbo.Doctors");
            DropTable("dbo.Appointments");
        }
    }
}
