using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PassionProject.Models
{
    // explicit bridging table to store extra information (ingredient quantity)
    public class RecipeXIngredient
    {
        public int RecipeXIngredientID { get; set; }
        
        [ForeignKey("Recipe")]
        public int RecipeID { get; set; }
        public virtual Recipe Recipe { get; set; }

        [ForeignKey("Ingredient")]
        public int IngredientID { get; set; }
        public virtual Ingredient Ingredient { get; set; }

        // logs the quantity and unit of measurement of ingredient
        public decimal Quantity { get; set; }
        public string Unit { get; set; }
    }

    public class RecipeXIngredientDto
    {
        public int RecipeXIngredientID { get; set; }

        public int RecipeID { get; set; }
        public int IngredientID { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; }
    }
}