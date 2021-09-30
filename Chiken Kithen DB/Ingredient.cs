using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Chiken_Kithen_DB
{
    [Table("Ingredients")]
    class Ingredient
    {
        public int IngredientId { get; set; }
        public string Name { get; set; }
        public List<RecipeItem> recipeItems { get; set; } = new List<RecipeItem>();
        public Ingredient() { }
        public Ingredient(string _Name,int _IngredientId)
        {
            IngredientId = _IngredientId;
            Name = _Name;
        }
        public Ingredient(string _Name) {
            Name = _Name;
        }
    }
}
