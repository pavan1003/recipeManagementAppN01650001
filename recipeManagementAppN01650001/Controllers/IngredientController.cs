using recipeManagementAppN01650001.Models.ViewModels;
using recipeManagementAppN01650001.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Diagnostics;
using System.Security.Policy;

namespace recipeManagementAppN01650001.Controllers
{
    public class IngredientController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static IngredientController()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false,
                //cookies are manually set in RequestHeader
                UseCookies = false
            };

            client = new HttpClient(handler);
            client.BaseAddress = new Uri("https://localhost:44384/api/");
        }


        /// <summary>
        /// Displays a list of ingredients.
        /// </summary>
        /// <returns>A view with a list of ingredients</returns>
        /// <example>
        /// GET: Ingredient/List
        /// </example>
        public ActionResult List()
        {
            string url = "ingredientdata/listingredients";
            HttpResponseMessage response = client.GetAsync(url).Result;

            IEnumerable<IngredientDto> ingredients = response.Content.ReadAsAsync<IEnumerable<IngredientDto>>().Result;

            return View(ingredients);
        }

        /// <summary>
        /// Displays details of a specific ingredient.
        /// </summary>
        /// <param name="id">The ID of the ingredient to show</param>
        /// <returns>A view with the ingredient details</returns>
        /// <example>
        /// GET: Ingredient/Details/5
        /// </example>
        public ActionResult Details(int id)
        {
            DetailsIngredient ViewModel = new DetailsIngredient();

            string url = "ingredientdata/findingredient/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            IngredientDto SelectedIngredient = response.Content.ReadAsAsync<IngredientDto>().Result;

            ViewModel.SelectedIngredient = SelectedIngredient;

            url = "recipedata/listrecipesforingredient/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<RecipeDto> RelatedRecipes = response.Content.ReadAsAsync<IEnumerable<RecipeDto>>().Result;

            ViewModel.RelatedRecipes = RelatedRecipes;

            return View(ViewModel);
        }
        /// <summary>
        /// Displays an error page.
        /// </summary>
        /// <returns>An error view</returns>
        /// <example>
        /// GET: Ingredient/Error
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
        /// Displays the page to create a new ingredient.
        /// </summary>
        /// <returns>A view with a form to create a new ingredient</returns>
        /// <example>
        /// GET: Ingredient/New
        /// </example>
        [Authorize]
        public ActionResult New()
        {
            GetApplicationCookie();//get token credentials
            string url = "recipedata/listrecipes/";
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<RecipeDto> RecipeOptions = response.Content.ReadAsAsync<IEnumerable<RecipeDto>>().Result;
            return View(RecipeOptions);
        }

        /// <summary>
        /// Creates a new ingredient.
        /// </summary>
        /// <param name="ingredient">The ingredient to add</param>
        /// <returns>Redirects to the list of ingredients if successful; otherwise, redirects to the error page</returns>
        /// <example>
        /// POST: Recipe/Add
        /// BODY: { "IngredientName": "Sugar", "IngredientQuantity": 1, "IngredientUnit": "cup", "RecipeId" }
        /// </example>
        [HttpPost]
        [Authorize]
        public ActionResult Create(Ingredient ingredient)
        {
            GetApplicationCookie();//get token credentials
            string url = "ingredientdata/addingredient";

            string jsonpayload = jss.Serialize(ingredient);

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
        /// Displays the page to edit an existing ingredient.
        /// </summary>
        /// <param name="id">The ID of the ingredient to edit</param>
        /// <returns>A view with a form to edit the ingredient</returns>
        /// <example>
        /// GET: Ingredient/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            GetApplicationCookie();//get token credentials
            EditIngredient ViewModel = new EditIngredient();
            string url = "ingredientdata/findingredient/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            IngredientDto SelectedIngredient = response.Content.ReadAsAsync<IngredientDto>().Result;
            ViewModel.SelectedIngredient = SelectedIngredient;

            // all recipies to choose from when updating this Ingredient
            url = "recipedata/listrecipes/";
            response = client.GetAsync(url).Result;
            IEnumerable<RecipeDto> RecipeOptions = response.Content.ReadAsAsync<IEnumerable<RecipeDto>>().Result;

            ViewModel.RecipeOptions = RecipeOptions;

            return View(ViewModel);
        }

        /// <summary>
        /// Updates an existing ingredient.
        /// </summary>
        /// <param name="id">The ID of the ingredient to update</param>
        /// <param name="ingredient">The updated ingredient data</param>
        /// <returns>Redirects to the ingredient's details if successful; otherwise, returns to the edit view</returns>
        /// <example>
        /// POST: Ingredient/Update/5
        /// BODY: { "IngredientId": 1, "IngredientName": "Salt", "IngredientQuantity": 2, "IngredientUnit": "tbsp" }
        /// </example>
        [HttpPost]
        [Authorize]
        public ActionResult Update(int id, Ingredient ingredient)
        {
            GetApplicationCookie();//get token credentials
            Debug.WriteLine("The new ingredient info is:");
            Debug.WriteLine(ingredient.IngredientId);
            Debug.WriteLine(ingredient.IngredientName);
            Debug.WriteLine(ingredient.IngredientQuantity);
            Debug.WriteLine(ingredient.IngredientUnit);
            Debug.WriteLine(ingredient.RecipeId);

            string url = "ingredientdata/updateingredient/" + id;
            string jsonpayload = jss.Serialize(ingredient);
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
        /// Displays the page to confirm deletion of a ingredient.
        /// </summary>
        /// <param name="id">The ID of the ingredient to delete</param>
        /// <returns>A view with the ingredient details for confirmation</returns>
        /// <example>
        /// GET: Ingredient/DeleteConfirm/5
        /// </example>
        [Authorize]
        public ActionResult DeleteConfirm(int id)
        {
            GetApplicationCookie();//get token credentials
            string url = "ingredientdata/findingredient/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            IngredientDto selectedIngredient = response.Content.ReadAsAsync<IngredientDto>().Result;
            return View(selectedIngredient);
        }

        /// <summary>
        /// Deletes a ingredient.
        /// </summary>
        /// <param name="id">The ID of the ingredient to delete</param>
        /// <returns>Redirects to the list of ingredients if successful; otherwise, redirects to the error page</returns>
        /// <example>
        /// POST: Ingredient/Delete/5
        /// </example>
        [HttpPost]
        [Authorize]
        public ActionResult Delete(int id)
        {
            GetApplicationCookie();//get token credentials
            string url = "ingredientdata/deleteingredient/" + id;
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