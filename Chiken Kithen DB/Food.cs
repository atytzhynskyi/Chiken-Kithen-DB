using System;
using System.Collections.Generic;
using System.Text;

namespace Chiken_Kithen_DB
{
    class Food
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
        public List<Ingredient> Ingredients = new List<Ingredient>();
        public List<int> IngredientsId = new List<int>();
        public Food() { }
        public Food(string _Name, int _Count, params Ingredient[] _Ingredients)
        {
            Name = _Name;
            Count = _Count;
            Ingredients.AddRange(_Ingredients);
        }
        public Food(string _Name, params Ingredient[] _Ingredients)
        {
            Name = _Name;
            Count = 0;
            Ingredients.AddRange(_Ingredients);
        }
    }
}
