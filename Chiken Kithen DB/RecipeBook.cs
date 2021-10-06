using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Chiken_Kithen_DB
{
    class RecipeBook
    {
        public List<Food> Recipes { get; set; } = new List<Food>();
        public RecipeBook(ApplicationContext db)
        {
            Recipes.AddRange(db.Recipes);
        }
        
        public void AddNewFood(Food food)
        {
            Recipes.Add(food);
        }
        public void RewriteRecipe(Food _food, string changeFoodName)
        {
            foreach (var food in from Food food in Recipes
                                 where food.Name == changeFoodName
                                 select food)
            {
                food.Recipe = _food.Recipe;
                food.Name = _food.Name;
                return;
            }
        }
        public void DeleteRecipe(string _foodName)
        {
            foreach (Food food in Recipes)
            {
                if (food.Name == _foodName)
                {
                    Recipes.Remove(food);
                    return;
                }
            }
        }
        public void ShowRecipes()
        {
            Console.WriteLine("Recipes List:");
            foreach (Food recipe in Recipes.ToList())
            {
                Console.WriteLine(recipe.Id + " " + recipe.Name + ":");
                foreach (RecipeItem recipeItem in recipe.Recipe)
                {
                    Console.Write(recipeItem.Ingredient.Name + ", ");
                }
                Console.Write("\n");
            }
        }
    }
}
