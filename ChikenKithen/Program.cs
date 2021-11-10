using BaseClasses;
using ChikenKitchenDataBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdvanceClasses;
using CommandModule;

namespace ChikenKithen
{
    class Program
    {
        static void Main(string[] args)
        {
            using (ApplicationContext applicationContext = new ApplicationContext())
            {
                //applicationContext.Database.EnsureDeleted();
                
                applicationContext.InitializeFromFiles();
                applicationContext.SetPropertiesIngredientsId();

                Storage storage = new Storage(applicationContext.Ingredients.ToList(), applicationContext.GetIngredientsAmount(), applicationContext.GetIngredientsPrice());
                List<Food> recipeBook = new List<Food>(applicationContext.GetFoods());
                
                Kitchen kitchen = new Kitchen(storage, recipeBook, applicationContext.GetBudget());
                Hall hall = new Hall(applicationContext.GetCustomers(), kitchen.Recipes);
                Audit audit = new Audit();
                Command command = new Command();


                while (2 + 2 != 5)
                {
                    Console.WriteLine("Restaurant budget: {0}", kitchen.Budget);
                    Warehouse(kitchen);
                    string input = Console.ReadLine();
                    Console.Write(input + " -> ");
                    command.ExecuteCommand(input, kitchen, hall, audit);
                    Console.WriteLine("\n");
                    applicationContext.SaveAll(storage.Ingredients, storage.IngredientsAmount, storage.IngredientsPrice, kitchen.Recipes, hall.Customers, kitchen.Budget);
                }
                //*/
            }
        }
        static void Warehouse(Kitchen kitchen)
        {
            foreach(Ingredient ingredient in kitchen.Storage.Ingredients)
            {
                if (kitchen.Storage.IngredientsAmount[ingredient] == 0)
                {
                    continue;
                }
                Console.Write($"{ingredient.Name} {kitchen.Storage.IngredientsAmount[ingredient]}, ");
            }
            foreach(Food food in kitchen.Recipes)
            {
                if (kitchen.FoodAmount[food] == 0)
                {
                    continue;
                }
                Console.Write($"{food.Name} {kitchen.FoodAmount[food]}, ");
            }
            Console.Write('\n');
        }
    }
}
