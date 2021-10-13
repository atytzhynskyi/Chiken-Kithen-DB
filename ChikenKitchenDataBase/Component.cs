using BaseClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ChikenKitchenDataBase
{
    public class Component
    {
        public int Id { get; set; }
        public int RecipeId { get; set; }
        [ForeignKey("RecipeId")]
        public Food Recipe { get; set; }
        public int? IngredientId { get; set; }
        [ForeignKey("IngredientId")]
        public Ingredient Ingredient{get; set;}
        public int? FoodId { get; set; }
        [ForeignKey("FoodId")]
        public Food Food { get; set; }
        public Component() { }
        public Component(Food _Recipe, Ingredient _Ingredient) {
            Recipe = _Recipe;
            Ingredient = _Ingredient;
        }
        public Component(Food _Recipe, Food _Food)
        {
            Recipe = _Recipe;
            Food = _Food;
        }
    }
}
