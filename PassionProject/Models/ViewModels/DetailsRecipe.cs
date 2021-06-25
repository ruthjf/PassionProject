using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PassionProject.Models.ViewModels
{
    public class DetailsRecipe
    {
        public RecipeDto SelectedRecipe { get; set; }
        public IEnumerable<IngredientDto> IngredientsNeeded { get; set; }
        public IEnumerable<IngredientDto> AvailableIngredients { get; set; }
    }
}