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
        // GET: Recipe
        public ActionResult List()
        {
            string url = "ListRecipes";

            HttpResponseMessage response = client.GetAsync(url).Result;

            IEnumerable<RecipeDto> Recipe = response.Content.ReadAsAsync<IEnumerable<RecipeDto>>().Result;

            return View(Recipe);
        }

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

        public ActionResult Error()
        {

            return View();
        }

        // GET: Recipe/New
        public ActionResult New()
        {
            return View();
        }

        // POST: Recipe/Add
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

        // GET: Recipe/Edit/5
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

        // POST: Recipe/Update/5
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

        // GET: Recipe/DeleteConfirm/5
        public ActionResult DeleteConfirm(int id)
        {
            string url = "FindRecipe/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            RecipeDto Recipe = response.Content.ReadAsAsync<RecipeDto>().Result;
            return View(Recipe);
        }

        // POST: Recipe/Delete/5
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