using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Diagnostics;
using PassionProject.Models;
using PassionProject.Models.ViewModels;
using System.Web.Script.Serialization;

namespace PassionProject.Controllers
{
    public class IngredientController : Controller
    {
        private static readonly HttpClient client;
        JavaScriptSerializer jss = new JavaScriptSerializer();

        static IngredientController()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false,
                // cookies are manually set in RequestHeader
                UseCookies = false
            };

            client = new HttpClient(handler);
            client.BaseAddress = new Uri("https://localhost:44301/api/");
        }


        /// <summary>
        /// Grabs the authentication cookie sent to this controller.
        /// For proper WebAPI authentication, you can send a post request with login credentials to the WebAPI and log the access token from the response. The controller already knows this token, so we're just passing it up the chain.
        /// 
        /// Here is a descriptive article which walks through the process of setting up authorization/authentication directly.
        /// https://docs.microsoft.com/en-us/aspnet/web-api/overview/security/individual-accounts-in-web-api
        /// </summary>
        private void GetApplicationCookie()
        {
            string token = "";
            //HTTP client is set up to be reused, otherwise it will exhaust server resources.
            //This is a bit dangerous because a previously authenticated cookie could be cached for
            //a follow-up request from someone else. Reset cookies in HTTP client before grabbing a new one.
            client.DefaultRequestHeaders.Remove("Cookie");
            if (!User.Identity.IsAuthenticated) return;

            HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies.Get(".AspNet.ApplicationCookie");
            if (cookie != null) token = cookie.Value;

            //collect token as it is submitted to the controller
            //use it to pass along to the WebAPI.
            Debug.WriteLine("Token Submitted is : " + token);
            if (token != "") client.DefaultRequestHeaders.Add("Cookie", ".AspNet.ApplicationCookie=" + token);

            return;
        }


        // GET: Ingredient/List
        public ActionResult List()
        {
            //objective: communicate with ingredient data api to retrieve a list of ingredients
            // curl "https://localhost:44301/api/ingredientdata/listingredients"

            //establish URL communication
            string url = "ingredientdata/listingredients";
            HttpResponseMessage response = client.GetAsync(url).Result;

            //Debug.WriteLine("The response code is ");
            //Debug.WriteLine(response.StatusCode);

            //parse content of response into IEnumerable
            IEnumerable<IngredientDto> ingredients = response.Content.ReadAsAsync<IEnumerable<IngredientDto>>().Result;
            //Debug.WriteLine("Number of ingredients: ");
            //Debug.WriteLine(recipes.Count());

            return View(ingredients);
        }

        // GET: Ingredient/Details/5
        public ActionResult Details(int id)
        {
            DetailsIngredient ViewModel = new DetailsIngredient();

            //objective: communicate with ingredient data api to retrieve one ingredient
            // curl "https://localhost:44301/api/ingredientdata/findingredient/{id}"

            //establish URL communication
            string url = "ingredientdata/findingredient/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            //Debug.WriteLine("The response code is ");
            //Debug.WriteLine(response.StatusCode);

            //parse content of response
            IngredientDto SelectedIngredient = response.Content.ReadAsAsync<IngredientDto>().Result;
            //Debug.WriteLine("Ingredient: ");
            //Debug.WriteLine(SelectedIngredient.IngredientName);

            ViewModel.SelectedIngredient = SelectedIngredient;

            //show all recipes associated with this ingredient
            url = "recipedata/listrecipesforingredient/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<RecipeDto> RelatedRecipes = response.Content.ReadAsAsync<IEnumerable<RecipeDto>>().Result;

            ViewModel.RelatedRecipes = RelatedRecipes;

            return View(ViewModel);
        }

        public ActionResult Error()
        {
            return View();
        }

        // GET: Ingredient/New
        public ActionResult New()
        {
            return View();
        }

        // POST: Ingredient/Create
        [HttpPost]
        [Authorize]
        public ActionResult Create(Ingredient ingredient)
        {
            //Debug.WriteLine("The json payload is: ");
            //Debug.WriteLine(ingredient.IngredientName);

            GetApplicationCookie(); // get token credentials
            //objective: add a new ingredient into our system using the API
            // curl -d @ingredient.json -H "Content-Type:application/json" https://localhost:44301/api/ingredientdata/addingredient
            string url = "ingredientdata/addingredient";

            // convert ingredient object to json
            string jsonpayload = jss.Serialize(ingredient);

            //Debug.WriteLine(jsonpayload);

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

        // GET: Ingredient/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            // objective: users are able to find the ingredient to edit

            GetApplicationCookie(); // get token credentials
            // establish URL communication
            string url = "ingredientdata/findingredient/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            //parse content of response
            IngredientDto selectedingredient = response.Content.ReadAsAsync<IngredientDto>().Result;

            return View(selectedingredient);
        }

        // POST: Ingredient/Update/5
        [HttpPost]
        [Authorize]
        public ActionResult Update(int id, Ingredient ingredient)
        {
            Debug.WriteLine("The json payload is: ");
            Debug.WriteLine(ingredient.IngredientName);

            GetApplicationCookie(); // get token credentials

            //objective: edit an existing ingredient in our system using the API
            // curl -d @recipe.json -H "Content-Type:application/json" https://localhost:44301/api/ingredientdata/addingredient
            string url = "ingredientdata/updateingredient/" + id;

            // convert recipe object to json
            string jsonpayload = jss.Serialize(ingredient);

            //Debug.WriteLine(jsonpayload);

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

        // GET: Ingredient/Delete/5
        [Authorize]
        public ActionResult DeleteConfirm(int id)
        {
            GetApplicationCookie(); // get token credentials

            string url = "ingredientdata/findingredient/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            IngredientDto selectedingredient = response.Content.ReadAsAsync<IngredientDto>().Result;

            return View(selectedingredient);

        }

        // POST: Ingredient/Delete/5
        [HttpPost]
        [Authorize]
        public ActionResult Delete(int id)
        {
            GetApplicationCookie(); // get token credentials

            // objective: delete an ingredient from the system
            // curl /api/ingredientdata/deleteingredient -d ""

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
