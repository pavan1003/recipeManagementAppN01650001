namespace recipeManagementAppN01650001.Migrations
{
    using recipeManagementAppN01650001.Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<recipeManagementAppN01650001.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "recipeManagementAppN01650001.Models.ApplicationDbContext";
        }

        protected override void Seed(recipeManagementAppN01650001.Models.ApplicationDbContext context)
        {
            // Create some ingredients
            var ingredient1 = new Ingredient { IngredientId = 1, IngredientName = "Flour", IngredientQuantity = "2", IngredientUnit = "cups" };
            var ingredient2 = new Ingredient { IngredientId = 2, IngredientName = "Sugar", IngredientQuantity = "1", IngredientUnit = "cup" };
            var ingredient3 = new Ingredient { IngredientId = 3, IngredientName = "Eggs", IngredientQuantity = "2", IngredientUnit = "pieces" };

            // Create some instructions
            var instruction1 = new Instruction { InstructionId = 1, StepNumber = 1, Description = "Mix the flour and sugar.", RecipeId = 1 };
            var instruction2 = new Instruction { InstructionId = 2, StepNumber = 2, Description = "Add the eggs and stir.", RecipeId = 1 };

            // Create some recipes
            var recipe1 = new Recipe
            {
                RecipeId = 1,
                Title = "Simple Cake",
                Description = "A simple and delicious cake.",
                Category = "Dessert",
                CookingTime = 45,
                Ingredients = new List<Ingredient> { ingredient1, ingredient2, ingredient3 },
                Instructions = new List<Instruction> { instruction1, instruction2 }
            };

            // Add recipes to context
            context.Recipes.AddOrUpdate(r => r.RecipeId, recipe1);

            // Add ingredients to context
            context.Ingredients.AddOrUpdate(i => i.IngredientId, ingredient1, ingredient2, ingredient3);

            // Add instructions to context
            context.Instructions.AddOrUpdate(i => i.InstructionId, instruction1, instruction2);
        }
    }
}
