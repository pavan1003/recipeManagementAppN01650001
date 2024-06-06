namespace recipeManagementAppN01650001.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class recipeIngredientInstruction : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Ingredients",
                c => new
                    {
                        IngredientId = c.Int(nullable: false, identity: true),
                        IngredientName = c.String(),
                        IngredientQuantity = c.String(),
                        IngredientUnit = c.String(),
                        RecipeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IngredientId)
                .ForeignKey("dbo.Recipes", t => t.RecipeId, cascadeDelete: true)
                .Index(t => t.RecipeId);
            
            CreateTable(
                "dbo.Recipes",
                c => new
                    {
                        RecipeId = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Description = c.String(),
                        Category = c.String(),
                        CookingTime = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.RecipeId);
            
            CreateTable(
                "dbo.Instructions",
                c => new
                    {
                        InstructionId = c.Int(nullable: false, identity: true),
                        StepNumber = c.Int(nullable: false),
                        Description = c.String(),
                        RecipeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.InstructionId)
                .ForeignKey("dbo.Recipes", t => t.RecipeId, cascadeDelete: true)
                .Index(t => t.RecipeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Ingredients", "RecipeId", "dbo.Recipes");
            DropForeignKey("dbo.Instructions", "RecipeId", "dbo.Recipes");
            DropIndex("dbo.Instructions", new[] { "RecipeId" });
            DropIndex("dbo.Ingredients", new[] { "RecipeId" });
            DropTable("dbo.Instructions");
            DropTable("dbo.Recipes");
            DropTable("dbo.Ingredients");
        }
    }
}
