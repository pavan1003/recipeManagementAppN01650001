using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace recipeManagementAppN01650001.Models
{
    public class Recipe
    {
        [Key]
        public int RecipeId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public int CookingTime { get; set; }

        // A Recipe can have many Ingredients
        public virtual ICollection<Ingredient> Ingredients { get; set; }

        // A Recipe can have many Instructions
        public virtual ICollection<Instruction> Instructions { get; set; }


    }
    public class RecipeDto
    {
        public int RecipeId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public int CookingTime { get; set; }
        public List<IngredientDto> Ingredients { get; set; }
        public List<InstructionDto> Instructions { get; set; }
    }
}