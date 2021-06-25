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
    public class RecipeController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static RecipeController()
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


        // GET: Recipe/List
        public ActionResult List()
        {

            //objective: communicate with recipe data api to retrieve a list of recipes
            // curl "https://localhost:44301/api/recipedata/listrecipes"

            // establish URL communication
            string url = "recipedata/listrecipes";
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
            DetailsRecipe ViewModel = new DetailsRecipe();

            //objective: communicate with recipe data api to retrieve one recipe
            // curl "https://localhost:44301/api/recipedata/findrecipe/{id}"

            // establish URL communication
            string url = "recipedata/findrecipe/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            //Debug.WriteLine("The response code is ");
            //Debug.WriteLine(response.StatusCode);

            //parse content of response
            RecipeDto SelectedRecipe = response.Content.ReadAsAsync<RecipeDto>().Result;
            //Debug.WriteLine("Recipe: ");
            //Debug.WriteLine(SelectedRecipe.RecipeTitle);

            ViewModel.SelectedRecipe = SelectedRecipe;

            //show associated ingredients with this recipe
            url = "ingredientdata/listingredientsforrecipe/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<IngredientDto> IngredientsNeeded = response.Content.ReadAsAsync<IEnumerable<IngredientDto>>().Result;

            ViewModel.IngredientsNeeded = IngredientsNeeded;

            // show ingredients that are available to add to this recipe
            url = "ingredientdata/listingredientsnotforrecipe/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<IngredientDto> AvailableIngredients = response.Content.ReadAsAsync<IEnumerable<IngredientDto>>().Result;

            ViewModel.AvailableIngredients = AvailableIngredients;

            return View(ViewModel);
        }

        //POST: Animal/Associate/{recipeid}
        [HttpPost]
        [Authorize]
        public ActionResult Associate(int id, int IngredientID)
        {

            //Debug.WriteLine("Attempting to associate recipe : " + id + " with ingredient " + IngredientID);

            // gets the asp.net application cookie to authenticate on the webapi level
            GetApplicationCookie();

            //call api to associate recipe with ingredient
            string url = "recipedata/associaterecipewithingredient/" + id + "/" + IngredientID;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;



            return RedirectToAction("Details/" + id);
        }

        //GET: Recipe/UnAssociate/{id}?IngredientID={IngredientID)
        [HttpGet]
        [Authorize]
        public ActionResult UnAssociate(int id, int IngredientID)
        {
            Debug.WriteLine("Attempting to unassociate recipe : " + id + " with ingredient " + IngredientID);

            // gets the asp.net application cookie to authenticate on the webapi level
            GetApplicationCookie();

            //call api to associate recipe with ingredient
            string url = "recipedata/unassociaterecipewithingredient/" + id + "/" + IngredientID;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            return RedirectToAction("Details/" + id);
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
        [Authorize]
        public ActionResult Create(Recipe recipe)
        {
            //Debug.WriteLine("The json payload is: ");
            //Debug.WriteLine(recipe.RecipeTitle);

            // gets the asp.net application cookie to authenticate on the webapi level
            GetApplicationCookie();
            //objective: add a new recipe into our system using the API
            // curl -d @recipe.json -H "Content-Type:application/json" https://localhost:44301/api/recipedata/addrecipe
            string url = "recipedata/addrecipe";

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
        [Authorize]
        public ActionResult Edit(int id)
        {
            // objective: users are able to find the recipe to edit

            // gets the asp.net application cookie to authenticate on the webapi level
            GetApplicationCookie();

            // establish URL communication
            string url = "recipedata/findrecipe/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            //parse content of response
            RecipeDto selectedrecipe = response.Content.ReadAsAsync<RecipeDto>().Result;

            return View(selectedrecipe);
        }

        // POST: Recipe/Update/5
        [HttpPost]
        [Authorize]
        public ActionResult Update(int id, Recipe recipe, HttpPostedFileBase RecipePic)
        {
            //Debug.WriteLine("The json payload is: ");
            //Debug.WriteLine(recipe.RecipeTitle);

            // gets the asp.net application cookie to authenticate on the webapi level
            GetApplicationCookie();

            //objective: edit an existing recipe in our system using the API
            // curl -d @recipe.json -H "Content-Type:application/json" https://localhost:44301/api/recipedata/addrecipe
            string url = "recipedata/updaterecipe/" + id;

            // convert recipe object to json
            string jsonpayload = jss.Serialize(recipe);

            //Debug.WriteLine(jsonpayload);

            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode && RecipePic != null)
            {

                //Updating the recipe picture as a separate request
                Debug.WriteLine("Calling Update Image method.");
                //Send over image data for player
                url = "RecipeData/UploadRecipePic/" + id;
                //Debug.WriteLine("Received Recipe Picture "+RecipePic.FileName);

                MultipartFormDataContent requestcontent = new MultipartFormDataContent();
                HttpContent imagecontent = new StreamContent(RecipePic.InputStream);
                requestcontent.Add(imagecontent, "RecipePic", RecipePic.FileName);
                response = client.PostAsync(url, requestcontent).Result;


                return RedirectToAction("List");
            }
            else if (response.IsSuccessStatusCode)
            {
                //No image upload, but update still successful
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }


        // GET: Recipe/Delete/5
        [Authorize]
        public ActionResult DeleteConfirm(int id)
        {

            // gets the asp.net application cookie to authenticate on the webapi level
            GetApplicationCookie();

            string url = "recipedata/findrecipe/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            RecipeDto selectedrecipe = response.Content.ReadAsAsync<RecipeDto>().Result;

            return View(selectedrecipe);

        }
        

        // POST: Recipe/Delete/5
        [HttpPost]
        [Authorize]
        public ActionResult Delete(int id)
        {
            // objective: delete a recipe from the system
            // curl /api/recipedata/deleterecipe -d ""

            // gets the asp.net application cookie to authenticate on the webapi level
            GetApplicationCookie();

            string url = "recipedata/deleterecipe/" + id;
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
