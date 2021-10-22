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
                if (Recipes.Any(r => r.Name == ingredientName)) {
                    food.RecipeFoods.Add(Recipes.Where(r => r.Name == ingredientName).FirstOrDefault());
                    continue;
                }
                food.RecipeIngredients.Add(new Ingredient(ingredientName));
            }
            Recipes.Add(food);
        }
        public void ShowRecipes()
        {
            Console.WriteLine("Recipes List:");
            foreach (Food recipe in Recipes.ToList())
            {
                Console.WriteLine(recipe.Id + " " + recipe.Name + ":");
                foreach (Food food in recipe.RecipeFoods)
                {
                    Console.Write(food.Name + ", ");
                }
                foreach(Ingredient ingredient in recipe.RecipeIngredients)
                {
                    Console.Write(ingredient.Name + ", ");
                }
                Console.Write("\n");
            }
        }
    }
}
