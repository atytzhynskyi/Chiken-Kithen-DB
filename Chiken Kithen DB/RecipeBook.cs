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
        public DbSet<Food> Recipes { get; set; }
        public RecipeBook()
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=ChikenKitchen;Trusted_Connection=True;MultipleActiveResultSets=True;");
        }
        public void AddBaseRecipe()
        {
            using var streamReader = File.OpenText("Foods.csv"); 
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
            };
            using var csvReader = new CsvReader(streamReader, config);
            string name;
            while (csvReader.Read())
            {
                List<string> fileLine = new List<string>();
                for (int i = 0; csvReader.TryGetField<string>(i, out name); i++)
                {
                    fileLine.Add(name);
                }
                Recipes.Add(new Food(fileLine.ToArray()));
            }
            SaveChanges();
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
