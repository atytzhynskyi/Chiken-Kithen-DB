using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Chiken_Kithen_DB
{
    [Table("Recipes")]
    class Food
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [ForeignKey("Ingredients")]
        public List<Ingredient> Ingredients = new List<Ingredient>();
        public Food() { }
        public Food(string _Name, int _Count, params Ingredient[] _Ingredients)
        {
            Name = _Name;
            Ingredients.AddRange(_Ingredients);
        }
        public Food(params string[] nameAndIngredient)
        {
            Name = nameAndIngredient[0];
            foreach(string ingredientName in nameAndIngredient)
            {
                if (Name == ingredientName) continue;
                Ingredients.Add(new Ingredient(ingredientName));
            }
        }
        public Food(string _Name, params Ingredient[] _Ingredients)
        {
            Name = _Name;
            Ingredients.AddRange(_Ingredients);
        }
    }
}
