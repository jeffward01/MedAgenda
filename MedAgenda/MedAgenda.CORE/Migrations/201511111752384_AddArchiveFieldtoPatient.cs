namespace MedAgenda.CORE.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddArchiveFieldtoPatient : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Patients", "Archived", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Patients", "Archived");
        }
    }
}
