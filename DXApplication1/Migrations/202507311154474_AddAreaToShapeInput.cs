namespace DXApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAreaToShapeInput : DbMigration
    {
        public override void Up()
        {
            //AddColumn("dbo.ShapeInputs", "Area", c => c.Double());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ShapeInputs", "Area");
        }
    }
}
