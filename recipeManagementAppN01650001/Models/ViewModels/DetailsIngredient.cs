using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace recipeManagementAppN01650001.Models.ViewModels
{
    public class DetailsIngredient
    {
        public IngredientDto SelectedIngredient { get; set; }
        public IEnumerable<RecipeDto> RelatedRecipes { get; set; }
    }
}