namespace MedAgenda.CORE.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MergeWithMaster : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Doctors", "Archived", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Doctors", "Archived");
        }
    }
}
