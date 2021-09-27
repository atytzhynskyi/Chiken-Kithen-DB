using System;
using System.Collections.Generic;
using System.Text;

namespace Chiken_Kithen_DB
{
    class Ingredient
    {
        public int IngredientId { get; set; }
        public string Name { get; set; }
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
