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
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false,
                //cookies are manually set in RequestHeader
                UseCookies = false
            };

            client = new HttpClient(handler);
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
        /// Retrieves the authentication token from the application's cookie and sets it in the HTTP client's headers.
        /// This method ensures that the HTTP client is prepared for making authenticated requests to the WebAPI.
        /// </summary>
        /// <example>
        /// Usage:
        /// <code>
        /// GetApplicationCookie();
        /// // Now the client has the authentication token set in the headers and can make authenticated requests.
        /// </code>
        /// </example>
        private void GetApplicationCookie()
        {
            string token = "";

            // Remove any existing cookies from the HTTP client's headers to prevent caching issues.
            client.DefaultRequestHeaders.Remove("Cookie");

            // Check if the user is authenticated before proceeding.
            if (!User.Identity.IsAuthenticated) return;

            // Retrieve the authentication cookie from the current HTTP context.
            HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies.Get(".AspNet.ApplicationCookie");
            if (cookie != null) token = cookie.Value;

            // Log the token for debugging purposes.
            Debug.WriteLine("Token Submitted is : " + token);

            // If a token is found, add it to the HTTP client's headers.
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Add("Cookie", ".AspNet.ApplicationCookie=" + token);
            }

            return;
        }


        /// <summary>
        /// Displays the page to create a new recipe.
        /// </summary>
        /// <returns>A view with a form to create a new recipe</returns>
        /// <example>
        /// GET: Recipe/New
        /// </example>
        [Authorize]
        public ActionResult New()
        {
            GetApplicationCookie();//get token credentials
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
        [Authorize]
        public ActionResult Add(Recipe recipe)
        {
            GetApplicationCookie();//get token credentials
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
        [Authorize]
        public ActionResult Edit(int id)
        {
            GetApplicationCookie();//get token credentials

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
        [Authorize]
        public ActionResult Update(int id, Recipe recipe)
        {
            GetApplicationCookie();//get token credentials
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
        [Authorize]
        public ActionResult DeleteConfirm(int id)
        {
            GetApplicationCookie();//get token credentials
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
        [Authorize]
        public ActionResult Delete(int id)
        {
            GetApplicationCookie();//get token credentials
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