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
using System.Diagnostics;

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
                AppUsername = a.AppUsers.AppUsername,
                RecipeDifficultyLevel = a.RecipeDifficultyLevel,
                RecipeType = a.RecipeType

            }));

            return RecipeDtos;
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
                AppUsername = Recipe.AppUsers.AppUsername,
                AppUserID = Recipe.AppUsers.AppUserID,
                RecipeDifficultyLevel = Recipe.RecipeDifficultyLevel,
                RecipeType = Recipe.RecipeType
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
        public IHttpActionResult DeleteRecipe(int id)
        {
            Recipe recipe = db.Recipes.Find(id);
            if (recipe == null)
            {
                return NotFound();
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