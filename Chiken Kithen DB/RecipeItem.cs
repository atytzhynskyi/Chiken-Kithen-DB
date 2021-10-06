using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Chiken_Kithen_DB
{
    public class RecipeItem
    {
        public int Id { get; set; }
        public int FoodId { get; set; }
        [ForeignKey("FoodId")]
        public Food Food { get; set; }
        public int IngredientId { get; set; }
        [ForeignKey("IngredientId")]
        public Ingredient Ingredient{ get; set; }
        public RecipeItem(){ }
        public RecipeItem(Food _Food, Ingredient _Ingredients)
        {
            Food = _Food;
            Ingredient = _Ingredients;
        }
    }
}
