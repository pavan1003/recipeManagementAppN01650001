using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using recipeManagementAppN01650001.Models;

namespace recipeManagementAppN01650001.Controllers
{
    public class RecipeController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static RecipeController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44384/api/RecipeData/");
        }

        /// <summary>
        /// Displays a list of recipes.
        /// </summary>
        /// <returns>A view with a list of recipes</returns>
        /// <example>
        /// GET: Recipe/List
        /// </example>
        public ActionResult List()
        {
            string url = "ListRecipes";

            HttpResponseMessage response = client.GetAsync(url).Result;

            IEnumerable<RecipeDto> Recipe = response.Content.ReadAsAsync<IEnumerable<RecipeDto>>().Result;

            return View(Recipe);
        }

        /// <summary>
        /// Displays details of a specific recipe.
        /// </summary>
        /// <param name="id">The ID of the recipe to show</param>
        /// <returns>A view with the recipe details</returns>
        /// <example>
        /// GET: Recipe/Show/5
        /// </example>
        public ActionResult Show(int id)
        {
            string url = "FindRecipe/" + id;

            HttpResponseMessage response = client.GetAsync(url).Result;

            RecipeDto Recipe = response.Content.ReadAsAsync<RecipeDto>().Result;
            // Initialize Ingredients and Instructions if they are null
            //Recipe.Ingredients = Recipe.Ingredients ?? new List<IngredientDto>();
            //Recipe.Instructions = Recipe.Instructions ?? new List<InstructionDto>();

            return View(Recipe);
        }

        /// <summary>
        /// Displays an error page.
        /// </summary>
        /// <returns>An error view</returns>
        /// <example>
        /// GET: Recipe/Error
        /// </example>
        public ActionResult Error()
        {

            return View();
        }

        /// <summary>
        /// Displays the page to create a new recipe.
        /// </summary>
        /// <returns>A view with a form to create a new recipe</returns>
        /// <example>
        /// GET: Recipe/New
        /// </example>
        public ActionResult New()
        {
            return View();
        }

        /// <summary>
        /// Adds a new recipe.
        /// </summary>
        /// <param name="recipe">The recipe to add</param>
        /// <returns>Redirects to the list of recipes if successful; otherwise, redirects to the error page</returns>
        /// <example>
        /// POST: Recipe/Add
        /// BODY: { "Title": "New Recipe", "Description": "Description", "Category": "Category", "CookingTime": 30, "Ingredients": [{ "IngredientName": "Sugar", "IngredientQuantity": 1, "IngredientUnit": "cup" }], "Instructions": [{ "StepNumber": 1, "Description": "Mix ingredients" }] }
        /// </example>
        [HttpPost]
        public ActionResult Add(Recipe recipe)
        {
            Debug.WriteLine("the json payload is :");

            string url = "AddRecipe";

            string jsonpayload = jss.Serialize(recipe);

            Debug.WriteLine(jsonpayload);

            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }

        }

        /// <summary>
        /// Displays the page to edit an existing recipe.
        /// </summary>
        /// <param name="id">The ID of the recipe to edit</param>
        /// <returns>A view with a form to edit the recipe</returns>
        /// <example>
        /// GET: Recipe/Edit/5
        public ActionResult Edit(int id)
        {

            string url = "FindRecipe/" + id;

            HttpResponseMessage response = client.GetAsync(url).Result;

            RecipeDto Recipe = response.Content.ReadAsAsync<RecipeDto>().Result;
            // Initialize Ingredients and Instructions if they are null
            //Recipe.Ingredients = Recipe.Ingredients ?? new List<IngredientDto>();
            //Recipe.Instructions = Recipe.Instructions ?? new List<InstructionDto>();

            return View(Recipe);
        }

        /// <summary>
        /// Updates an existing recipe.
        /// </summary>
        /// <param name="id">The ID of the recipe to update</param>
        /// <param name="recipe">The updated recipe data</param>
        /// <returns>Redirects to the recipe's details if successful; otherwise, returns to the edit view</returns>
        /// <example>
        /// POST: Recipe/Update/5
        /// BODY: { "RecipeId": 5, "Title": "Updated Recipe", "Description": "Updated Description", "Category": "Updated Category", "CookingTime": 45, "Ingredients": [{ "IngredientId": 1, "IngredientName": "Salt", "IngredientQuantity": 2, "IngredientUnit": "tbsp" }], "Instructions": [{ "InstructionId": 1, "StepNumber": 1, "Description": "Updated Step" }] }
        /// </example>
        [HttpPost]
        public ActionResult Update(int id, Recipe recipe)
        {
            try
            {
                Debug.WriteLine("The new recipe info is:");
                Debug.WriteLine(recipe.RecipeId);
                Debug.WriteLine(recipe.Title);
                Debug.WriteLine(recipe.Description);
                Debug.WriteLine(recipe.Category);
                Debug.WriteLine(recipe.CookingTime);
                Debug.WriteLine(recipe.Instructions.ToString());
                Debug.WriteLine(recipe.Ingredients.ToString());

                //serialize into JSON
                //Send the request to the API

                string url = "UpdateRecipe/" + id;

                string jsonpayload = jss.Serialize(recipe);
                Debug.WriteLine(jsonpayload);

                HttpContent content = new StringContent(jsonpayload);
                content.Headers.ContentType.MediaType = "application/json";

                HttpResponseMessage response = client.PostAsync(url, content).Result;

                return RedirectToAction("Show/" + id);
            }
            catch
            {
                return View();
            }
        }

        /// <summary>
        /// Displays the page to confirm deletion of a recipe.
        /// </summary>
        /// <param name="id">The ID of the recipe to delete</param>
        /// <returns>A view with the recipe details for confirmation</returns>
        /// <example>
        /// GET: Recipe/DeleteConfirm/5
        /// </example>
        public ActionResult DeleteConfirm(int id)
        {
            string url = "FindRecipe/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            RecipeDto Recipe = response.Content.ReadAsAsync<RecipeDto>().Result;
            return View(Recipe);
        }

        /// <summary>
        /// Deletes a recipe.
        /// </summary>
        /// <param name="id">The ID of the recipe to delete</param>
        /// <returns>Redirects to the list of recipes if successful; otherwise, redirects to the error page</returns>
        /// <example>
        /// POST: Recipe/Delete/5
        /// </example>
        [HttpPost]
        public ActionResult Delete(int id)
        {
            string url = "DeleteRecipe/" + id;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

    }
}