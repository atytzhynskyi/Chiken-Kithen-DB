using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using BaseClasses;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;

namespace ChikenKithen
{
    public class RecipeBook
    {
        public List<Food> Recipes { get; set; } = new List<Food>();
        public RecipeBook(List<Food> _Recipes)
        {
            Recipes.AddRange(_Recipes);
        }
        
        public void AddNewRecipe(Food food)
        {
            Recipes.Add(food);
        }
        public void AddNewRecipe()
        {
            Food food = new Food();
            Console.WriteLine("What is name of this food?");
            food.Name = Console.ReadLine();
            Console.WriteLine("What's in the recipe?(please use ',' between ingredients)");
            foreach (string ingredientName in Console.ReadLine().Split(", "))
            {
                food.Recipe.Add(new RecipeItem(food, new Ingredient(ingredientName)));
            }
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
