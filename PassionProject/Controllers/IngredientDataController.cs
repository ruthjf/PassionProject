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
    public class IngredientDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Returns all ingredients in the system.
        /// </summary>
        /// <returns>
        /// HEADER: 200 OK
        /// CONTENT: all ingredients in the database
        /// </returns>
        /// <example>
        /// GET: api/IngredientData/ListIngredients
        /// </example>
        [HttpGet]
        public IEnumerable<IngredientDto> ListIngredients(string SearchKey = null)
        {
            List<Ingredient> Ingredients = db.Ingredients.ToList();
            List<IngredientDto> IngredientDtos = new List<IngredientDto>();

            // searching the database with the searchkey
            if (SearchKey != null)
            {
                Ingredients = db.Ingredients.Where(a => a.IngredientName.Contains(SearchKey)).ToList();
            }

            Ingredients.ForEach(a => IngredientDtos.Add(new IngredientDto()
            {
                IngredientID = a.IngredientID,
                IngredientName = a.IngredientName,
                //RecipeID = a.Recipe.RecipeID,
                //RecipeTitle = a.Recipe.RecipeTitle
            }));

            return IngredientDtos;
        }

        /// <summary>
        /// Returns all ingredients in the system
        /// </summary>
        /// <returns>
        /// HEADER: 200 OK
        /// CONTENT: all ingredients in the database with associated recipe
        /// </returns>
        /// <param name="id">Recipe Primary Key</param>
        /// <example>
        /// GET: api/IngredientData/ListIngredientsForRecipe/5
        /// </example>
        [HttpGet]
        public IEnumerable<IngredientDto> ListIngredientsForRecipe(int id)
        {
            List<Ingredient> Ingredients = db.Ingredients.Where(
                i=>i.Recipes.Any(
                    r=>r.RecipeID==id)
                ).ToList();
            List<IngredientDto> IngredientDtos = new List<IngredientDto>();

            Ingredients.ForEach(a => IngredientDtos.Add(new IngredientDto()
            {
                IngredientID = a.IngredientID,
                IngredientName = a.IngredientName,
                //RecipeID = a.Recipe.RecipeID,
                //RecipeTitle = a.Recipe.RecipeTitle
            }));

            return IngredientDtos;
        }

        /// <summary>
        /// Returns all ingredients in the system that are not included in a particular recipe
        /// </summary>
        /// <returns>
        /// HEADER: 200 OK
        /// CONTENT: all ingredients in the database not associated with a recipe
        /// </returns>
        /// <param name="id">Recipe Primary Key</param>
        /// <example>
        /// GET: api/IngredientData/ListIngredientsNotForRecipe/5
        /// </example>
        [HttpGet]
        public IEnumerable<IngredientDto> ListIngredientsNotForRecipe(int id)
        {
            List<Ingredient> Ingredients = db.Ingredients.Where(
                i => !i.Recipes.Any(
                    r => r.RecipeID == id)
                ).ToList();
            List<IngredientDto> IngredientDtos = new List<IngredientDto>();

            Ingredients.ForEach(a => IngredientDtos.Add(new IngredientDto()
            {
                IngredientID = a.IngredientID,
                IngredientName = a.IngredientName,
                //RecipeID = a.Recipe.RecipeID,
                //RecipeTitle = a.Recipe.RecipeTitle
            }));

            return IngredientDtos;
        }

        /// <summary>
        /// Returns an ingredient in the system
        /// </summary>
        /// <returns>
        /// HEADER: 200 OK
        /// CONTENT: An ingredient in the system with a matching recipe id (primary key)
        /// </returns>
        /// <param name="id">Primary key of an ingredient</param>
        /// <example>
        /// GET: api/IngredientData/FindIngredient/5
        /// </example>
        [ResponseType(typeof(Ingredient))]
        [HttpGet]
        public IHttpActionResult FindIngredient(int id)
        {
            Ingredient Ingredient = db.Ingredients.Find(id);

            IngredientDto IngredientDto = new IngredientDto()
            {
                IngredientID = Ingredient.IngredientID,
                IngredientName = Ingredient.IngredientName,
                //RecipeID = Ingredient.Recipe.RecipeID,
                //RecipeTitle = Ingredient.Recipe.RecipeTitle
            };

            if (Ingredient == null)
            {
                return NotFound();
            }

            return Ok(IngredientDto);
        }

        /// <summary>
        /// Updates a particular ingredient in the system with POST data input
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
        /// POST: api/IngredientData/UpdateIngredient/5
        /// FORM DATA: Recipe JSON Object
        /// </example>
        [ResponseType(typeof(void))]
        [HttpPost]
        [Authorize]
        public IHttpActionResult UpdateIngredient(int id, Ingredient ingredient)
        {
            Debug.WriteLine("This is the update ingredient method!");
            if (!ModelState.IsValid)
            {
                //Debug.WriteLine("Model state is invalid");
                return BadRequest(ModelState);
            }

            if (id != ingredient.IngredientID)
            {
                //Debug.WriteLine("ID does not match");
                //Debug.WriteLine("GET parameter"+id);
                //Debug.WriteLine("POST parameter"+ ingredient.IngredientID);
                //Debug.WriteLine("POST parameter" + ingredient.IngredientName);
                return BadRequest();
            }

            db.Entry(ingredient).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IngredientExists(id))
                {
                    //Debug.WriteLine("Ingredient not found");
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
        /// Adds an ingredient to the system
        /// </summary>
        /// <param name="recipe">JSON Form Data of an ingredient</param>
        /// <returns>
        /// HEADER: 204 Success, no content response
        /// or
        /// HEADER: 400 Bad Request
        /// or
        /// HEADER: 404 Not Found
        /// </returns>
        /// <example>
        /// POST: api/IngredientData/AddRecipe
        /// </example>
        [ResponseType(typeof(Ingredient))]
        [HttpPost]
        [Authorize]
        public IHttpActionResult AddIngredient(Ingredient ingredient)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Ingredients.Add(ingredient);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = ingredient.IngredientID }, ingredient);
        }

        /// <summary>
        /// Deletes an ingredient from the system by its ID
        /// </summary>
        /// <param name="id">Ingredient ID primary key</param>
        /// <returns>
        /// HEADER: 200 OK
        /// or
        /// HEADER: 404 Not Found
        /// </returns>
        /// <example>
        /// POST: api/IngredientData/DeleteIngredient/5
        /// FORM DATA: empty
        /// </example>
        [ResponseType(typeof(Ingredient))]
        [HttpPost]
        [Authorize]
        public IHttpActionResult DeleteIngredient(int id)
        {
            Ingredient ingredient = db.Ingredients.Find(id);
            if (ingredient == null)
            {
                return NotFound();
            }

            db.Ingredients.Remove(ingredient);
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

        private bool IngredientExists(int id)
        {
            return db.Ingredients.Count(e => e.IngredientID == id) > 0;
        }
    }
}