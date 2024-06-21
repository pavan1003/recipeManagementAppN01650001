using recipeManagementAppN01650001.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using static recipeManagementAppN01650001.Models.Recipe;
using System.Diagnostics;

namespace recipeManagementAppN01650001.Controllers
{
    public class RecipeDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // <summary>
        /// Return a list of recipes
        /// </summary>
        /// <returns>An Array of Recipes </returns>
        /// <example>
        /// GET: api/RecipeData/ListRecipes
        /// </example>
        [HttpGet]
        [Route("api/RecipeData/ListRecipes")]
        public List<RecipeDto> ListRecipies()
        {
            List<Recipe> Recipes = db.Recipes.ToList();
            List<RecipeDto> RecipeDtos = new List<RecipeDto>();

            Recipes.ForEach(a => RecipeDtos.Add(new RecipeDto()
            {
                RecipeId = a.RecipeId,
                Title = a.Title,
                Description = a.Description,
                Category = a.Category,
                CookingTime = a.CookingTime,
                Ingredients = a.Ingredients.Select(i => new IngredientDto()
                {
                    IngredientId = i.IngredientId,
                    IngredientName = i.IngredientName,
                    IngredientQuantity = i.IngredientQuantity,
                    IngredientUnit = i.IngredientUnit
                }).ToList(),
                Instructions = a.Instructions.Select(ins => new InstructionDto()
                {
                    InstructionId = ins.InstructionId,
                    StepNumber = ins.StepNumber,
                    Description = ins.Description
                }).ToList()
            }));


            return RecipeDtos;
        }
        /// <summary>
        /// Returns all recipes related to a particular ingredient ID
        /// </summary>
        /// <param name="id">Ingredient ID.</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: All recipes related to the specified ingredient ID
        /// </returns>
        /// <example>
        /// GET: api/RecipeData/ListRecipesForIngredient/3
        /// </example>
        [HttpGet]
        [ResponseType(typeof(RecipeDto))]
        [Route("api/RecipeData/ListRecipesForIngredient/{id}")]
        public IHttpActionResult ListRecipesForIngredient(int id)
        {
            List<Recipe> Recipes = db.Recipes.Where(r => r.Ingredients.Any(i => i.IngredientId == id)).ToList();
            List<RecipeDto> RecipeDtos = new List<RecipeDto>();

            Recipes.ForEach(a => RecipeDtos.Add(new RecipeDto()
            {
                RecipeId = a.RecipeId,
                Title = a.Title,
                Description = a.Description,
                Category = a.Category,
                CookingTime = a.CookingTime
            }));

            return Ok(RecipeDtos);
        }

        /// <summary>
        /// Returns all recipes related to a particular instruction ID
        /// </summary>
        /// <param name="id">Instruction ID.</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: All recipes related to the specified instruction ID
        /// </returns>
        /// <example>
        /// GET: api/RecipeData/ListRecipesForInstruction/3
        /// </example>
        [HttpGet]
        [ResponseType(typeof(RecipeDto))]
        [Route("api/RecipeData/ListRecipesForInstruction/{id}")]
        public IHttpActionResult ListRecipesForInstruction(int id)
        {
            List<Recipe> Recipes = db.Recipes.Where(r => r.Instructions.Any(i => i.InstructionId == id)).ToList();
            List<RecipeDto> RecipeDtos = new List<RecipeDto>();

            Recipes.ForEach(a => RecipeDtos.Add(new RecipeDto()
            {
                RecipeId = a.RecipeId,
                Title = a.Title,
                Description = a.Description,
                Category = a.Category,
                CookingTime = a.CookingTime
            }));

            return Ok(RecipeDtos);
        }

        /// <summary>
        /// Finds a specific recipe by its ID, including ingredients and instructions
        /// </summary>
        /// <param name="id">The ID of the recipe</param>
        /// <returns>The recipe with the specified ID, including ingredients and instructions</returns>
        /// <example>
        /// GET: api/RecipeData/FindRecipe/5
        /// </example>
        [ResponseType(typeof(Recipe))]
        [HttpGet]
        [Route("api/RecipeData/FindRecipe/{id}")]
        public IHttpActionResult FindRecipe(int id)
        {
            var Recipe = db.Recipes.Find(id);

            var RecipeDto = new RecipeDto
            {
                RecipeId = Recipe.RecipeId,
                Title = Recipe.Title,
                Description = Recipe.Description,
                Category = Recipe.Category,
                CookingTime = Recipe.CookingTime,
                Ingredients = Recipe.Ingredients.Select(i => new IngredientDto
                {
                    IngredientId = i.IngredientId,
                    IngredientName = i.IngredientName,
                    IngredientQuantity = i.IngredientQuantity,
                    IngredientUnit = i.IngredientUnit
                }).ToList(),
                Instructions = Recipe.Instructions.Select(ins => new InstructionDto
                {
                    InstructionId = ins.InstructionId,
                    StepNumber = ins.StepNumber,
                    Description = ins.Description
                }).ToList()
            };
            if (Recipe == null)
            {
                return NotFound();
            }

            return Ok(RecipeDto);
        }

        /// <summary>
        /// Adds a new recipe
        /// </summary>
        /// <param name="recipe">The recipe to add</param>
        /// <returns>Confirmation of recipe creation</returns>
        /// <example>
        /// POST: api/RecipeData/AddRecipe
        /// BODY: { "Title": "New Recipe", "Description": "Description", "Category": "Category", "CookingTime": 30, "Ingredients": [{ "IngredientName": "Sugar", "IngredientQuantity": 1, "IngredientUnit": "cup" }], "Instructions": [{ "StepNumber": 1, "Description": "Mix ingredients" }] }
        /// </example>
        [ResponseType(typeof(Recipe))]
        [HttpPost]
        [Route("api/RecipeData/AddRecipe")]
        [Authorize]
        public IHttpActionResult AddRecipe(Recipe recipe)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Recipes.Add(recipe);
            db.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Deletes a specific recipe by its ID
        /// </summary>
        /// <param name="id">The ID of the recipe to delete</param>
        /// <returns>Confirmation of recipe deletion</returns>
        /// <example>
        /// POST: api/RecipeData/DeleteRecipe/5
        /// </example>
        [ResponseType(typeof(Recipe))]
        [HttpPost]
        [Route("api/RecipeData/DeleteRecipe/{id}")]
        [Authorize]
        public IHttpActionResult DeleteRecipe(int id)
        {
            Recipe recipe = db.Recipes.Find(id);
            if (recipe == null)
            {
                return NotFound();
            }

            db.Recipes.Remove(recipe);
            db.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Updates a specific recipe by its ID
        /// </summary>
        /// <param name="id">The ID of the recipe to update</param>
        /// <param name="recipe">The updated recipe data</param>
        /// <returns>No content if update is successful</returns>
        /// <example>
        /// POST: api/RecipeData/UpdateRecipe/5
        /// BODY: { "RecipeId": 5, "Title": "Updated Recipe", "Description": "Updated Description", "Category": "Updated Category", "CookingTime": 45, "Ingredients": [{ "IngredientId": 1, "IngredientName": "Salt", "IngredientQuantity": 2, "IngredientUnit": "tbsp" }], "Instructions": [{ "InstructionId": 1, "StepNumber": 1, "Description": "Updated Step" }] }
        /// </example>
        [ResponseType(typeof(void))]
        [HttpPost]
        [Route("api/RecipeData/UpdateRecipe/{id}")]
        [Authorize]
        public IHttpActionResult UpdateRecipe(int id, Recipe recipe)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != recipe.RecipeId)
            {
                Debug.WriteLine("ID MISMATCH");
                Debug.WriteLine(recipe.RecipeId);
                Debug.WriteLine(recipe.Title);

                return BadRequest();
            }

            var ExistingRecipe = db.Recipes.Include(r => r.Ingredients).Include(r => r.Instructions).FirstOrDefault(r => r.RecipeId == id);
            if (ExistingRecipe == null)
            {
                return NotFound();
            }

            // Update recipe properties
            ExistingRecipe.Title = recipe.Title;
            ExistingRecipe.Description = recipe.Description;
            ExistingRecipe.Category = recipe.Category;
            ExistingRecipe.CookingTime = recipe.CookingTime;

            // Update or remove ingredients
            foreach (var ExistingIngredient in ExistingRecipe.Ingredients.ToList())
            {
                var matchingIngredient = recipe.Ingredients.FirstOrDefault(i => i.IngredientId == ExistingIngredient.IngredientId);
                if (matchingIngredient != null)
                {
                    // Update existing ingredient
                    ExistingIngredient.IngredientName = matchingIngredient.IngredientName;
                    ExistingIngredient.IngredientQuantity = matchingIngredient.IngredientQuantity;
                    ExistingIngredient.IngredientUnit = matchingIngredient.IngredientUnit;
                }
                else
                {
                    // Remove ingredient if not present in the updated recipe
                    db.Ingredients.Remove(ExistingIngredient);
                }
            }

            // Add new ingredients
            foreach (var ingredient in recipe.Ingredients.Where(i => i.IngredientId == 0))
            {
                ExistingRecipe.Ingredients.Add(ingredient);
            }

            // Update or remove instructions
            foreach (var ExistingInstruction in ExistingRecipe.Instructions.ToList())
            {
                var matchingInstruction = recipe.Instructions.FirstOrDefault(i => i.InstructionId == ExistingInstruction.InstructionId);
                if (matchingInstruction != null)
                {
                    // Update existing instruction
                    ExistingInstruction.StepNumber = matchingInstruction.StepNumber;
                    ExistingInstruction.Description = matchingInstruction.Description;
                }
                else
                {
                    // Remove instruction if not present in the updated recipe
                    db.Instructions.Remove(ExistingInstruction);
                }
            }

            // Add new instructions
            foreach (var instruction in recipe.Instructions.Where(i => i.InstructionId == 0))
            {
                ExistingRecipe.Instructions.Add(instruction);
            }

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RecipeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return StatusCode(HttpStatusCode.NoContent);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Checks if a recipe exists by its ID
        /// </summary>
        /// <param name="id">The ID of the recipe</param>
        /// <returns>true if the recipe exists; otherwise, false</returns>
        /// <example>
        /// bool exists = RecipeExists(5);
        /// </example>
        private bool RecipeExists(int id)
        {
            return db.Recipes.Count(e => e.RecipeId == id) > 0;
        }
    }
}
