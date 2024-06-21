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

namespace recipeManagementAppN01650001.Controllers
{
    public class IngredientDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Return a list of ingredients
        /// </summary>
        /// <returns>An array of Ingredients</returns>
        /// <example>
        /// GET: api/IngredientData/ListIngredients
        /// </example>
        [HttpGet]
        [Route("api/IngredientData/ListIngredients")]
        public List<IngredientDto> ListIngredients()
        {
            List<Ingredient> Ingredients = db.Ingredients.ToList();
            List<IngredientDto> IngredientDtos = new List<IngredientDto>();

            Ingredients.ForEach(a => IngredientDtos.Add(new IngredientDto()
            {
                IngredientId = a.IngredientId,
                IngredientName = a.IngredientName,
                IngredientQuantity = a.IngredientQuantity,
                IngredientUnit = a.IngredientUnit,
                RecipeId = a.RecipeId,
                RecipeTitle = a.Recipe.Title
            }));

            return IngredientDtos;
        }

        /// <summary>
        /// Finds a specific ingredient by its ID
        /// </summary>
        /// <param name="id">The ID of the ingredient</param>
        /// <returns>The ingredient with the specified ID</returns>
        /// <example>
        /// GET: api/IngredientData/FindIngredient/5
        /// </example>
        [ResponseType(typeof(Ingredient))]
        [HttpGet]
        [Route("api/IngredientData/FindIngredient/{id}")]
        public IHttpActionResult FindIngredient(int id)
        {
            Ingredient ingredient = db.Ingredients.Find(id);
            if (ingredient == null)
            {
                return NotFound();
            }

            IngredientDto ingredientDto = new IngredientDto
            {
                IngredientId = ingredient.IngredientId,
                IngredientName = ingredient.IngredientName,
                IngredientQuantity = ingredient.IngredientQuantity,
                IngredientUnit = ingredient.IngredientUnit,
                RecipeId = ingredient.RecipeId
            };

            return Ok(ingredientDto);
        }

        /// <summary>
        /// Adds a new ingredient
        /// </summary>
        /// <param name="ingredient">The ingredient to add</param>
        /// <returns>Confirmation of ingredient creation</returns>
        /// <example>
        /// POST: api/IngredientData/AddIngredient
        /// BODY: { "IngredientName": "Sugar", "IngredientQuantity": 1, "IngredientUnit": "cup" }
        /// </example>
        [ResponseType(typeof(Ingredient))]
        [HttpPost]
        [Route("api/IngredientData/AddIngredient")]
        [Authorize]
        public IHttpActionResult AddIngredient(Ingredient ingredient)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Ingredients.Add(ingredient);
            db.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Deletes a specific ingredient by its ID
        /// </summary>
        /// <param name="id">The ID of the ingredient to delete</param>
        /// <returns>Confirmation of ingredient deletion</returns>
        /// <example>
        /// POST: api/IngredientData/DeleteIngredient/5
        /// </example>
        [ResponseType(typeof(Ingredient))]
        [HttpPost]
        [Route("api/IngredientData/DeleteIngredient/{id}")]
        [Authorize]
        public IHttpActionResult DeleteIngredient(int id)
        {
            Ingredient ingredient = db.Ingredients.Find(id);
            if (ingredient == null)
            {
                return NotFound();
            }

            db.Ingredients.Remove(ingredient);
            db.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Updates a specific ingredient by its ID
        /// </summary>
        /// <param name="id">The ID of the ingredient to update</param>
        /// <param name="ingredient">The updated ingredient data</param>
        /// <returns>No content if update is successful</returns>
        /// <example>
        /// POST: api/IngredientData/UpdateIngredient/5
        /// BODY: { "IngredientId": 5, "IngredientName": "Salt", "IngredientQuantity": 2, "IngredientUnit": "tbsp" }
        /// </example>
        [ResponseType(typeof(void))]
        [HttpPost]
        [Route("api/IngredientData/UpdateIngredient/{id}")]
        [Authorize]
        public IHttpActionResult UpdateIngredient(int id, Ingredient ingredient)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != ingredient.IngredientId)
            {
                return BadRequest();
            }

            db.Entry(ingredient).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IngredientExists(id))
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
        /// Checks if an ingredient exists by its ID
        /// </summary>
        /// <param name="id">The ID of the ingredient</param>
        /// <returns>true if the ingredient exists; otherwise, false</returns>
        /// <example>
        /// bool exists = IngredientExists(5);
        /// </example>
        private bool IngredientExists(int id)
        {
            return db.Ingredients.Count(e => e.IngredientId == id) > 0;
        }
    }
}
