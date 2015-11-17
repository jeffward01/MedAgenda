namespace MedAgenda.CORE.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAddressFieldsToPatientAndDoctor : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Doctors", "Address1", c => c.String());
            AddColumn("dbo.Doctors", "Address2", c => c.String());
            AddColumn("dbo.Doctors", "City", c => c.String());
            AddColumn("dbo.Doctors", "State", c => c.String());
            AddColumn("dbo.Doctors", "Zip", c => c.String());
            AddColumn("dbo.Patients", "Address1", c => c.String());
            AddColumn("dbo.Patients", "Address2", c => c.String());
            AddColumn("dbo.Patients", "City", c => c.String());
            AddColumn("dbo.Patients", "State", c => c.String());
            AddColumn("dbo.Patients", "Zip", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Patients", "Zip");
            DropColumn("dbo.Patients", "State");
            DropColumn("dbo.Patients", "City");
            DropColumn("dbo.Patients", "Address2");
            DropColumn("dbo.Patients", "Address1");
            DropColumn("dbo.Doctors", "Zip");
            DropColumn("dbo.Doctors", "State");
            DropColumn("dbo.Doctors", "City");
            DropColumn("dbo.Doctors", "Address2");
            DropColumn("dbo.Doctors", "Address1");
        }
    }
}
