using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using BaseClasses;
using CsvHelper;

namespace ChikenKithen
{
    class Menu
    {
        public List<Food> Foods = new List<Food>();
        public List<Food> GetFoods() => Foods;
        public Menu()
        {
        }
        public Menu(List<Food> _Foods)
        {
            Foods.AddRange(_Foods);
        }
        private void ReadFromFile()
        {
            using var streamReader = File.OpenText(@"..\..\..\Ingredients.csv");
            using var csvReader = new CsvReader(streamReader, CultureInfo.CurrentCulture);
        }
        public void AddNewFood()
        {
            Console.WriteLine("What is name of new food?");
            string foodName = Console.ReadLine();
            Food food = new Food(foodName);
            Console.WriteLine("What are in the recipe? (please use ',' between ingredients)");
            string[] ingredientsRecipeSplit = Console.ReadLine().Split(", ");
            List<RecipeItem> Recipe = new List<RecipeItem>();
            foreach (string ingredientNameStr in ingredientsRecipeSplit)
            {
                Recipe.Add(new RecipeItem(food, new Ingredient(ingredientNameStr)));
            }
            food.Recipe.AddRange(Recipe);
            Foods.Add(food);
            Console.WriteLine(food.Name + " added to the menu");
        }
        public void ShowAll()
        {
            foreach (Food food in Foods)
            {
                Console.WriteLine(food.Name);
            }
        }
    }
}
