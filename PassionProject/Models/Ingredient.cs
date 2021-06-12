using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace PassionProject.Models
{
    public class Ingredient
    {
        [Key]
        public int IngredientID { get; set; }
        public string IngredientName { get; set; }

        /* possibly add Quantity */

        // bridging table
        // an ingredient can be used in many recipes
        public ICollection<Recipe> Recipes { get; set; }
    }
}