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
        /// <summary>
        /// Return a list of recipes
        /// </summary>
        /// <returns>An Array of Recipes </returns>
        /// <example>
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

        // GET: api/RecipeData/FindRecipe/5
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

        // POST: api/RecipeData/AddRecipe
        [ResponseType(typeof(Recipe))]
        [HttpPost]
        [Route("api/RecipeData/AddRecipe")]
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

        // POST: api/RecipeData/DeleteRecipe/5
        [ResponseType(typeof(Recipe))]
        [HttpPost]
        [Route("api/RecipeData/DeleteRecipe/{id}")]
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

        // POST: api/RecipeData/UpdateRecipe/5
        [ResponseType(typeof(void))]
        [HttpPost]
        [Route("api/RecipeData/UpdateRecipe/{id}")]
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
                Debug.WriteLine("ID MISMATCH");

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

        private bool RecipeExists(int id)
        {
            return db.Recipes.Count(e => e.RecipeId == id) > 0;
        }
    }
}
