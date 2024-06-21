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
    public class InstructionController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static InstructionController()
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
        /// Displays a list of instructions.
        /// </summary>
        /// <returns>A view with a list of instructions</returns>
        /// <example>
        /// GET: Instruction/List
        /// </example>
        public ActionResult List()
        {
            string url = "instructiondata/listinstructions";
            HttpResponseMessage response = client.GetAsync(url).Result;

            IEnumerable<InstructionDto> instructions = response.Content.ReadAsAsync<IEnumerable<InstructionDto>>().Result;

            return View(instructions);
        }

        /// <summary>
        /// Displays details of a specific instruction.
        /// </summary>
        /// <param name="id">The ID of the instruction to show</param>
        /// <returns>A view with the instruction details</returns>
        /// <example>
        /// GET: Instruction/Details/5
        /// </example>
        public ActionResult Details(int id)
        {
            DetailsInstruction ViewModel = new DetailsInstruction();

            string url = "instructiondata/findinstruction/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            InstructionDto SelectedInstruction = response.Content.ReadAsAsync<InstructionDto>().Result;

            ViewModel.SelectedInstruction = SelectedInstruction;

            url = "recipedata/listrecipesforinstruction/" + id;
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
        /// GET: Instruction/Error
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
        /// Displays the page to create a new instruction.
        /// </summary>
        /// <returns>A view with a form to create a new instruction</returns>
        /// <example>
        /// GET: Instruction/New
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
        /// Creates a new instruction.
        /// </summary>
        /// <param name="instruction">The instruction to add</param>
        /// <returns>Redirects to the list of instructions if successful; otherwise, redirects to the error page</returns>
        /// <example>
        /// POST: Recipe/Add
        /// BODY: { "InstructionName": "Sugar", "InstructionQuantity": 1, "InstructionUnit": "cup", "RecipeId" }
        /// </example>
        [HttpPost]
        [Authorize]
        public ActionResult Create(Instruction instruction)
        {
            GetApplicationCookie();//get token credentials
            string url = "instructiondata/addinstruction";

            string jsonpayload = jss.Serialize(instruction);

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
        /// Displays the page to edit an existing instruction.
        /// </summary>
        /// <param name="id">The ID of the instruction to edit</param>
        /// <returns>A view with a form to edit the instruction</returns>
        /// <example>
        /// GET: Instruction/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            GetApplicationCookie();//get token credentials
            EditInstruction ViewModel = new EditInstruction();
            string url = "instructiondata/findinstruction/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            InstructionDto SelectedInstruction = response.Content.ReadAsAsync<InstructionDto>().Result;
            ViewModel.SelectedInstruction = SelectedInstruction;

            // all recipies to choose from when updating this Instruction
            url = "recipedata/listrecipes/";
            response = client.GetAsync(url).Result;
            IEnumerable<RecipeDto> RecipeOptions = response.Content.ReadAsAsync<IEnumerable<RecipeDto>>().Result;

            ViewModel.RecipeOptions = RecipeOptions;

            return View(ViewModel);
        }

        /// <summary>
        /// Updates an existing instruction.
        /// </summary>
        /// <param name="id">The ID of the instruction to update</param>
        /// <param name="instruction">The updated instruction data</param>
        /// <returns>Redirects to the instruction's details if successful; otherwise, returns to the edit view</returns>
        /// <example>
        /// POST: Instruction/Update/5
        /// BODY: { "InstructionId": 1, "InstructionName": "Salt", "InstructionQuantity": 2, "InstructionUnit": "tbsp" }
        /// </example>
        [HttpPost]
        [Authorize]
        public ActionResult Update(int id, Instruction instruction)
        {
            GetApplicationCookie();//get token credentials
            Debug.WriteLine("The new instruction info is:");
            Debug.WriteLine(instruction.InstructionId);
            Debug.WriteLine(instruction.StepNumber);
            Debug.WriteLine(instruction.Description);
            Debug.WriteLine(instruction.RecipeId);

            string url = "instructiondata/updateinstruction/" + id;
            string jsonpayload = jss.Serialize(instruction);
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
        /// Displays the page to confirm deletion of a instruction.
        /// </summary>
        /// <param name="id">The ID of the instruction to delete</param>
        /// <returns>A view with the instruction details for confirmation</returns>
        /// <example>
        /// GET: Instruction/DeleteConfirm/5
        /// </example>
        [Authorize]
        public ActionResult DeleteConfirm(int id)
        {
            GetApplicationCookie();//get token credentials
            string url = "instructiondata/findinstruction/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            InstructionDto selectedInstruction = response.Content.ReadAsAsync<InstructionDto>().Result;
            return View(selectedInstruction);
        }

        /// <summary>
        /// Deletes a instruction.
        /// </summary>
        /// <param name="id">The ID of the instruction to delete</param>
        /// <returns>Redirects to the list of instructions if successful; otherwise, redirects to the error page</returns>
        /// <example>
        /// POST: Instruction/Delete/5
        /// </example>
        [HttpPost]
        [Authorize]
        public ActionResult Delete(int id)
        {
            GetApplicationCookie();//get token credentials
            string url = "instructiondata/deleteinstruction/" + id;
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