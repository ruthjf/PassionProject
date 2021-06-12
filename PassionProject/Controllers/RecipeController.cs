using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Diagnostics;
using PassionProject.Models;
using System.Web.Script.Serialization;

namespace PassionProject.Controllers
{
    public class RecipeController : Controller
    {
        private static readonly HttpClient client;
        JavaScriptSerializer jss = new JavaScriptSerializer();

        static RecipeController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44301/api/recipedata/");
        }



        // GET: Recipe/List
        public ActionResult List()
        {

            //objective: communicate with recipe data api to retrieve a list of recipes
            // curl "https://localhost:44301/api/recipedata/listrecipes"

            // establish URL communication
            string url = "listrecipes";
            HttpResponseMessage response = client.GetAsync(url).Result;

            //Debug.WriteLine("The response code is ");
            //Debug.WriteLine(response.StatusCode);

            //parse content of response into IEnumerable
            IEnumerable<RecipeDto> recipes = response.Content.ReadAsAsync<IEnumerable<RecipeDto>>().Result;
            //Debug.WriteLine("Number of recipes: ");
            //Debug.WriteLine(recipes.Count());

            return View(recipes);

        }

        // GET: Recipe/Details/5
        public ActionResult Details(int id)
        {
            //objective: communicate with recipe data api to retrieve one recipe
            // curl "https://localhost:44301/api/recipedata/findrecipe/{id}"

            // establish URL communication
            string url = "findrecipe/"+id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            //Debug.WriteLine("The response code is ");
            //Debug.WriteLine(response.StatusCode);

            //parse content of response into IEnumerable
            RecipeDto selectedrecipe = response.Content.ReadAsAsync<RecipeDto>().Result;
            //Debug.WriteLine("Recipe: ");
            //Debug.WriteLine(selectedrecipe.RecipeTitle);

            return View(selectedrecipe);
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

        // POST: Recipe/Create
        [HttpPost]
        public ActionResult Create(Recipe recipe)
        {
            //Debug.WriteLine("The json payload is: ");
            //Debug.WriteLine(recipe.RecipeTitle);

            //objective: add a new recipe into our system using the API
            // curl -d @recipe.json -H "Content-Type:application/json" https://localhost:44301/api/recipedata/addrecipe
            string url = "AddRecipe";

            // convert recipe object to json
            string jsonpayload = jss.Serialize(recipe);

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

        // GET: Recipe/Edit/5
        public ActionResult Edit(int id)
        {
            // objective: users are able to find the recipe to edit

            // establish URL communication
            string url = "findrecipe/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            //parse content of response into IEnumerable
            RecipeDto selectedrecipe = response.Content.ReadAsAsync<RecipeDto>().Result;

            return View(selectedrecipe);
        }

        // POST: Recipe/Update/5
        [HttpPost]
        public ActionResult Update(int id, Recipe recipe)
        {
            Debug.WriteLine("The json payload is: ");
            Debug.WriteLine(recipe.RecipeTitle);

            //objective: edit an existing recipe in our system using the API
            // curl -d @recipe.json -H "Content-Type:application/json" https://localhost:44301/api/recipedata/addrecipe
            string url = "UpdateRecipe/"+id;

            // convert recipe object to json
            string jsonpayload = jss.Serialize(recipe);

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

        
        // GET: Recipe/Delete/5
        public ActionResult DeleteConfirm(int id)
        {
            string url = "findrecipe/"+id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            RecipeDto selectedrecipe = response.Content.ReadAsAsync<RecipeDto>().Result;

            return View(selectedrecipe);

        }
        

        // POST: Recipe/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            // objective: delete a recipe from the system
            // curl /api/recipedata/deleterecipe -d ""

            string url = "deleterecipe/"+id;
            HttpContent content = new StringContent("");

            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if(response.IsSuccessStatusCode)
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
