using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=storage;Trusted_Connection=True;");
        }
        public void AddBaseRecipe()
        {
            Recipes.Add(new Food("Emperor Chicken", 0, new Ingredient("Fat Cat Chiken"), new Ingredient("Spicy Sauce"), new Ingredient("Tuna Cake")));
            Recipes.Add(new Food("Fat Cat Chiken", 0, new Ingredient("Princess Chicken"), new Ingredient("Youth Sauce"), new Ingredient("Fries"), new Ingredient("Tuna Cake")));
            Recipes.Add(new Food("Princess Chicken", 0, new Ingredient("Chicken"), new Ingredient("Youth Sauce")));
            Recipes.Add(new Food("Youth Sauce", 0, new Ingredient("Asparagus"), new Ingredient("Milk"), new Ingredient("Honey")));
            Recipes.Add(new Food("Spicy Sauce", 0, new Ingredient("Paprika"), new Ingredient("Garlic"), new Ingredient("Water")));
            Recipes.Add(new Food("Omega Sauce", 0, new Ingredient("Lemon"), new Ingredient("Water")));
            Recipes.Add(new Food("Diamond Salad", 0, new Ingredient("Tomatoes"), new Ingredient("Pickles"), new Ingredient("Feta")));
            Recipes.Add(new Food("Ruby Salad", 0, new Ingredient("Tomatoes"), new Ingredient("Vinegar")));
            Recipes.Add(new Food("Fries", 0, new Ingredient("Potatoes")));
            Recipes.Add(new Food("Smashed Potatoes", 0, new Ingredient("Potatoes")));
            Recipes.Add(new Food("Tuna Cake", 0, new Ingredient("Tuna"), new Ingredient("Chocolate"), new Ingredient("Youth Sauce")));
            Recipes.Add(new Food("Fish In Water", 0, new Ingredient("Tuna"), new Ingredient("Omega Sauce"), new Ingredient("Ruby Salad")));
            SaveChanges();
        }
        public void AddNewFood(Food food)
        {
            Recipes.Add(food);
            SaveChanges();
        }
        public void DeleteRecipe(string _foodName)
        {
            foreach(Food food in Recipes)
            {
                if(food.Name == _foodName)
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
                foreach(Ingredient ingredient in recipe.Ingredients)
                {
                    Console.Write(ingredient.Name + " ");
                }
                Console.Write(";\n");
            }
        }
    }
}
