using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PassionProject.Models
{
    public class Recipe
    {   
        [Key]
        public int RecipeID { get; set; }
        public string RecipeTitle { get; set; }
        public string RecipeInstructions { get; set; }
        public string RecipeType { get; set; }
        public string RecipeDifficultyLevel { get; set; }
        // choose from easy, moderate, hard


        // a recipe belongs to one app user
        // an app user can have many recipes
        [ForeignKey("AppUsers")]
        public int? AppUserID { get; set; }

        //navigation property
        public virtual AppUser AppUsers { get; set; }

        // bridging table
        // a recipe can have many ingredients
        public ICollection<Ingredient> Ingredients { get; set; }
    }

    public class RecipeDto
    {
        public int RecipeID { get; set; }
        public string RecipeTitle { get; set; }
        public string RecipeInstructions { get; set; }
        public string RecipeType { get; set; }
        public string RecipeDifficultyLevel { get; set; }
        public string AppUsername { get; set; }
        public int AppUserID { get; set; }
    }
}