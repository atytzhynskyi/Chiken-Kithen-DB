using BaseClasses;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChikenKitchenDataBase
{
    public class IngredientComponent
    {
        public int Id { get; set; }
        public int RecipeId { get; set; }
        [ForeignKey("RecipeId")]
        public Food Recipe { get; set; }
        public int IngredientId { get; set; }
        [ForeignKey("IngredientId")]
        public Ingredient Ingredient { get; set; }
        public IngredientComponent() { }
        public IngredientComponent(Food _Recipe, Ingredient _Ingredient) {
            Ingredient = _Ingredient;
            Recipe = _Recipe;
        }
    }
}
