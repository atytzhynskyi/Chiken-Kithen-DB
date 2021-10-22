using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BaseClasses
{
    [Table("Foods")]
    public class Food
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Food> RecipeFoods = new List<Food>();
        public List<Ingredient> RecipeIngredients = new List<Ingredient>();
        //public List<RecipeItem> Recipe = new List<RecipeItem>();
        public Food() { }
        public Food(string _Name) { Name = _Name; }
        public Food(string _Name, params Ingredient[] _RecipeIngredients)
        {
            Name = _Name;
            RecipeIngredients.AddRange(_RecipeIngredients);
        }
        public Food(string _Name, params Food[] _RecipeFoods)
        {
            Name = _Name;
            RecipeFoods.AddRange(_RecipeFoods);
        }
        public Food(string _Name, List<Ingredient> _RecipeIngredients, List<Food>_RecipeFoods)
        {
            Name = _Name;
            RecipeIngredients.AddRange(_RecipeIngredients);
            RecipeFoods.AddRange(_RecipeFoods);
        }
    }
}
