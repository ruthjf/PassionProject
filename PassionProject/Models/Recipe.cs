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
        public string RecipeDifficultyLevel { get; set; }
        // choose from easy, moderate, hard

        public bool Vegan { get; set; }
        public bool Vegetarian { get; set; }
        public bool GlutenFree { get; set; }

        //data needed for keeping track of recipe images uploaded
        //images deposited into /Content/Images/Recipes/{id}.{extension}
        public bool RecipeHasPic { get; set; }
        public string PicExtension { get; set; }

        // bridging table
        // a recipe can have many ingredients
        public ICollection<Ingredient> Ingredients { get; set; }

    }

    public class RecipeDto
    {
        public int RecipeID { get; set; }
        public string RecipeTitle { get; set; }
        public string RecipeInstructions { get; set; }
        public string RecipeDifficultyLevel { get; set; }
        public bool Vegan { get; set; }
        public bool Vegetarian { get; set; }
        public bool GlutenFree { get; set; }
        public bool RecipeHasPic { get; set; }
        public string PicExtension { get; set; }

    }
}