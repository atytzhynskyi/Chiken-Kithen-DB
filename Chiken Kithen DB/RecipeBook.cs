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
    class RecipeBook : DbContext
    {
        public List<Food> Recipes { get; set; }
        public RecipeBook(DataBase db)
        {
            Recipes.AddRange(db.Recipes);
        }
        
        public void AddNewFood(Food food)
        {
            Recipes.Add(food);
            SaveChanges();
        }
        public void RewriteRecipe(Food _food, string changeFoodName)
        {
            foreach (var food in from Food food in Recipes
                                 where food.Name == changeFoodName
                                 select food)
            {
                food.Ingredients = _food.Ingredients;
                food.Name = _food.Name;
                SaveChanges();
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
                    SaveChanges();
                    return;
                }
            }
        }
        public void ShowRecipe()
        {
            var recipes = Recipes.ToList();
            Console.WriteLine("Recipes List:");
            foreach (Food recipe in recipes)
            {
                Console.WriteLine(recipe.Id + " " + recipe.Name + ":");
                foreach (Ingredient ingredient in recipe.Ingredients)
                {
                    Console.Write(ingredient.Name + " ");
                }
                Console.Write(";\n");
            }
        }
    }
}
