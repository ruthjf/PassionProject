using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using PassionProject.Models;
using PassionProject.Models.ViewModels;
using System.Diagnostics;
using System.Web;
using System.IO;

namespace PassionProject.Controllers
{
    public class RecipeDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Returns all recipes in the system.
        /// </summary>
        /// <returns>
        /// HEADER: 200 OK ???
        /// CONTENT: all recipes in the database and their associated appusernames
        /// </returns>
        /// <example>
        /// GET: api/RecipeData/ListRecipes
        /// </example>
        [HttpGet]
        public IEnumerable<RecipeDto> ListRecipes()
        {
            List<Recipe> Recipes = db.Recipes.ToList();
            List<RecipeDto> RecipeDtos = new List<RecipeDto>();

            Recipes.ForEach(a => RecipeDtos.Add(new RecipeDto()
            {
                RecipeID = a.RecipeID,
                RecipeTitle = a.RecipeTitle,
                RecipeInstructions = a.RecipeInstructions,
                RecipeDifficultyLevel = a.RecipeDifficultyLevel,
                Vegan = a.Vegan,
                Vegetarian = a.Vegetarian,
                GlutenFree = a.GlutenFree

            }));

            return RecipeDtos;
        }

        /// <summary>
        /// Gathers information about recipes related to a particular ingredient
        /// </summary>
        /// <returns>
        /// HEADER: 200 OK ???
        /// CONTENT: all recipes in the database that match to a particular ingredient id
        /// </returns>
        /// <param name="id">Ingredient ID</param>
        /// <example>
        /// GET: api/RecipeData/ListRecipesForIngredient/9
        /// </example>
        [HttpGet]
        [ResponseType(typeof(RecipeDto))]
        public IHttpActionResult ListRecipesForIngredient(int id)
        {
            // all recipes that have ingredients which match with the id
            List<Recipe> Recipes = db.Recipes.Where(
                a=>a.Ingredients.Any(
                i=>i.IngredientID==id
                )).ToList();
            List<RecipeDto> RecipeDtos = new List<RecipeDto>();

            Recipes.ForEach(a => RecipeDtos.Add(new RecipeDto()
            {
                RecipeID = a.RecipeID,
                RecipeTitle = a.RecipeTitle,
                RecipeInstructions = a.RecipeInstructions,
                RecipeDifficultyLevel = a.RecipeDifficultyLevel,
                Vegan = a.Vegan,
                Vegetarian = a.Vegetarian,
                GlutenFree = a.GlutenFree

            }));

            return Ok(RecipeDtos);
        }

        /// <summary>
        /// Associates a particular ingredient with a  particular recipe
        /// </summary>
        /// <param name="recipeid">Recipe ID primary key</param>
        /// <param name="ingredientid">Ingredient ID primary key</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST api/recipedata/associaterecipewithingredient/5/8
        /// </example>
        [HttpPost]
        [Route("api/recipedata/AssociateRecipeWithIngredient/{recipeid}/{ingredientid}")]
        [Authorize]
        public IHttpActionResult AssociateRecipeWithIngredient(int recipeid, int ingredientid)
        {
            // take in recipeid and associate with ingredientid
            Recipe SelectedRecipe = db.Recipes.Include(a=>a.Ingredients).Where(a=>a.RecipeID==recipeid).FirstOrDefault();
            Ingredient SelectedIngredient = db.Ingredients.Find(ingredientid);

            if (SelectedRecipe == null || SelectedIngredient == null)
            {
                return NotFound();
            }

            // tie recipeid and ingredientid together
            SelectedRecipe.Ingredients.Add(SelectedIngredient);
            db.SaveChanges();

            return Ok();
        }


        /// <summary>
        /// Removes an association between particular ingredient and a particular recipe
        /// </summary>
        /// <param name="recipeid">Recipe ID primary key</param>
        /// <param name="ingredientid">Ingredient ID primary key</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST api/recipedata/associaterecipewithingredient/5/8
        /// </example>
        [HttpPost]
        [Route("api/recipedata/UnAssociateRecipeWithIngredient/{recipeid}/{ingredientid}")]
        [Authorize]
        public IHttpActionResult UnAssociateRecipeWithIngredient(int recipeid, int ingredientid)
        {
            // take in recipeid and associate with ingredientid
            Recipe SelectedRecipe = db.Recipes.Include(a => a.Ingredients).Where(a => a.RecipeID == recipeid).FirstOrDefault();
            Ingredient SelectedIngredient = db.Ingredients.Find(ingredientid);

            if (SelectedRecipe == null || SelectedIngredient == null)
            {
                return NotFound();
            }

            // tie recipeid and ingredientid together
            SelectedRecipe.Ingredients.Remove(SelectedIngredient);
            db.SaveChanges();

            return Ok();
        }



        /// <summary>
        /// Returns a recipe in the system
        /// </summary>
        /// <returns>
        /// HEADER: 200 OK
        /// CONTENT: A recipe in the system with a matching recipe id (primary key)
        /// </returns>
        /// <param name="id">Primary key of a recipe</param>
        /// <example>
        /// GET: api/RecipeData/FindRecipe/5
        /// </example>
        [ResponseType(typeof(Recipe))]
        [HttpGet]
        public IHttpActionResult FindRecipe(int id)
        {
            Recipe Recipe = db.Recipes.Find(id);
            RecipeDto RecipeDto = new RecipeDto()
            {
                RecipeID = Recipe.RecipeID,
                RecipeTitle = Recipe.RecipeTitle,
                RecipeInstructions = Recipe.RecipeInstructions,
                RecipeDifficultyLevel = Recipe.RecipeDifficultyLevel,
                Vegan = Recipe.Vegan,
                Vegetarian = Recipe.Vegetarian,
                GlutenFree = Recipe.GlutenFree,
                RecipeHasPic = Recipe.RecipeHasPic,
                PicExtension = Recipe.PicExtension
            };
            if (Recipe == null)
            {
                return NotFound();
            }

            return Ok(RecipeDto);
        }

        /// <summary>
        /// Updates a particular recipe in the system with POST data input
        /// </summary>
        /// <param name="id">Recipe ID primary key</param>
        /// <param name="recipe">JSON form data of an animal</param>
        /// <returns>
        /// HEADER: 204 Success, no content response
        /// or
        /// HEADER: 400 Bad Request
        /// or
        /// HEADER: 404 Not Found
        /// </returns>
        /// <example>
        /// POST: api/RecipeData/UpdateRecipe/5
        /// FORM DATA: Recipe JSON Object
        /// </example>
        [ResponseType(typeof(void))]
        [HttpPost]
        [Authorize]
        public IHttpActionResult UpdateRecipe(int id, Recipe recipe)
        {
            //Debug.WriteLine("This is the update recipe method!");
            if (!ModelState.IsValid)
            {
               //Debug.WriteLine("Model state is invalid");
                return BadRequest(ModelState);
            }

            if (id != recipe.RecipeID)
            {
                //Debug.WriteLine("ID does not match");
                //Debug.WriteLine("GET parameter"+id);
                //Debug.WriteLine("POST parameter"+ recipe.RecipeID);
                //Debug.WriteLine("POST parameter" + recipe.RecipeTitle);
                //Debug.WriteLine("POST parameter" + recipe.RecipeInstructions);
                return BadRequest();
            }

            db.Entry(recipe).State = EntityState.Modified;
            // picture upload is handled by another method (UploadRecipePic)
            db.Entry(recipe).Property(a => a.RecipeHasPic).IsModified = false;
            db.Entry(recipe).Property(a => a.PicExtension).IsModified = false;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RecipeExists(id))
                {
                    //Debug.WriteLine("Recipe not found");
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            //Debug.WriteLine("None of the conditions triggered");
            return StatusCode(HttpStatusCode.NoContent);
        }



        /// <summary>
        /// Receives recipe picture data, uploads it to the webserver and updates the recipe's HasPic option
        /// </summary>
        /// <param name="id">Recipe ID</param>
        /// <returns>status code 200 if successful.</returns>
        /// <example>
        /// curl -F recipepic=@file.jpg "https://localhost:xx/api/recipedata/uploadrecipepic/37"
        /// POST: api/RecipeData/UpdateRecipePic/3
        /// HEADER: enctype=multipart/form-data
        /// FORM-DATA: image
        /// </example>
        /// https://stackoverflow.com/questions/28369529/how-to-set-up-a-web-api-controller-for-multipart-form-data

        [HttpPost]
        public IHttpActionResult UploadRecipePic(int id)
        {

            bool haspic = false;
            string picextension;
            if (Request.Content.IsMimeMultipartContent())
            {
                Debug.WriteLine("Received multipart form data.");

                int numfiles = HttpContext.Current.Request.Files.Count;
                Debug.WriteLine("Files Received: " + numfiles);

                //Check if a file is posted
                if (numfiles == 1 && HttpContext.Current.Request.Files[0] != null)
                {
                    var RecipePic = HttpContext.Current.Request.Files[0];
                    //Check if the file is empty
                    if (RecipePic.ContentLength > 0)
                    {
                        //establish valid file types (can be changed to other file extensions if desired!)
                        var valtypes = new[] { "jpeg", "jpg", "png", "gif" };
                        var extension = Path.GetExtension(RecipePic.FileName).Substring(1);
                        //Check the extension of the file
                        if (valtypes.Contains(extension))
                        {
                            try
                            {
                                //file name is the id of the image
                                string fn = id + "." + extension;

                                //get a direct file path to ~/Content/Images/Recipes/{id}.{extension}
                                string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Content/Images/Recipes/"), fn);

                                //save the file
                                RecipePic.SaveAs(path);

                                //if these are all successful then we can set these fields
                                haspic = true;
                                picextension = extension;

                                //Update the recipe haspic and picextension fields in the database
                                Recipe SelectedRecipe = db.Recipes.Find(id);
                                SelectedRecipe.RecipeHasPic = haspic;
                                SelectedRecipe.PicExtension = extension;
                                db.Entry(SelectedRecipe).State = EntityState.Modified;

                                db.SaveChanges();

                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine("Recipe image not saved successfully.");
                                Debug.WriteLine("Exception:" + ex);
                                return BadRequest();
                            }
                        }
                    }

                }

                return Ok();
            }
            else
            {
                //not multipart form data
                return BadRequest();

            }

        }

        /// <summary>
        /// Adds a recipe to the system
        /// </summary>
        /// <param name="recipe">JSON Form Data of a recipe</param>
        /// <returns>
        /// HEADER: 204 Success, no content response
        /// or
        /// HEADER: 400 Bad Request
        /// or
        /// HEADER: 404 Not Found
        /// </returns>
        /// <example>
        /// POST: api/RecipeData/AddRecipe
        /// </example>
        [ResponseType(typeof(Recipe))]
        [HttpPost]
        [Authorize]
        public IHttpActionResult AddRecipe(Recipe recipe)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Recipes.Add(recipe);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = recipe.RecipeID }, recipe);
        }

        /// <summary>
        /// Deletes a recipe from the system by its ID
        /// </summary>
        /// <param name="id">Recipe ID primary key</param>
        /// <returns>
        /// HEADER: 200 OK
        /// or
        /// HEADER: 404 Not Found
        /// </returns>
        /// <example>
        /// POST: api/RecipeData/DeleteRecipe/5
        /// FORM DATA: empty
        /// </example>
        [ResponseType(typeof(Recipe))]
        [HttpPost]
        [Authorize]
        public IHttpActionResult DeleteRecipe(int id)
        {
            Recipe recipe = db.Recipes.Find(id);
            if (recipe == null)
            {
                return NotFound();
            }
            if (recipe.RecipeHasPic && recipe.PicExtension != "")
            {
                //also delete image from path
                string path = HttpContext.Current.Server.MapPath("~/Content/Images/Recipes/" + id + "." + recipe.PicExtension);
                if (System.IO.File.Exists(path))
                {
                    Debug.WriteLine("File exists... preparing to delete!");
                    System.IO.File.Delete(path);
                }
            }


            db.Recipes.Remove(recipe);
            db.SaveChanges();

            return Ok();
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
            return db.Recipes.Count(e => e.RecipeID == id) > 0;
        }
    }
}