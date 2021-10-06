﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Chiken_Kithen_DB
{
    [Table("Foods")]
    public class Food
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<RecipeItem> Recipe = new List<RecipeItem>();
        public Food() { }
        public Food(string _Name) { Name = _Name; }
        public Food(string _Name, params RecipeItem[] _RecipeItem)
        {
            Name = _Name;
            Recipe.AddRange(_RecipeItem);
        }
        public Food(string _Name, params Ingredient[] _RecipeItem)
        {
            Name = _Name;
            foreach (Ingredient ingredient in _RecipeItem)
            {
                Recipe.Add(new RecipeItem(this, ingredient));
            }
        }
        public Food(List<Ingredient> ingredients, params string[] nameAndIngredient)
        {
            Name = nameAndIngredient[0];
            foreach (string ingredientName in nameAndIngredient)
            {
                if (Name == ingredientName) continue;
                bool isFound = false;
                foreach (Ingredient ingredient in ingredients)
                {
                    if (ingredientName == ingredient.Name)
                    {
                        isFound = true;
                        Recipe.Add(new RecipeItem(this, ingredient));
                    }
                }
                if (!isFound)
                {
                    Recipe.Add(new RecipeItem(this, new Ingredient(ingredientName)));
                }
            }
        }
    }
}
