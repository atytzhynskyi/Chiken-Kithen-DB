using BaseClasses;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChikenKitchenDataBase
{
    public class FoodComponent
    {
        public int Id { get; set; }
        public int RecipeId { get; set; }
        [ForeignKey("RecipeId")]
        public Food Recipe { get; set; }
        public int? FoodId { get; set; }
        [ForeignKey("FoodId")]
        public Food Food { get; set; }
        public FoodComponent() { }
        public FoodComponent(Food _Recipe, Food _Food)
        {
            Food = _Food;
            Recipe = _Recipe;
        }
    }
}
