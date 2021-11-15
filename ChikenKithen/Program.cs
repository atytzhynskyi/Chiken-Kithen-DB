using BaseClasses;
using ChikenKitchenDataBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdvanceClasses;
using CommandsModule;
using jsonReadModule;

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

                Storage storage = new Storage(
                    applicationContext.GetFoods(), applicationContext.Ingredients.ToList(), 
                    applicationContext.GetIngredientsAmount(), applicationContext.GetIngredientsPrice());
                
                Kitchen kitchen = new Kitchen(storage, applicationContext.GetBudget());
                Hall hall = new Hall(applicationContext.GetCustomers(), kitchen.Storage.Recipes);

                while (2 + 2 != 5)
                {
                    Console.WriteLine("Restaurant budget: {0}", kitchen.Budget);
                    Warehouse(kitchen);
                    string input = Console.ReadLine();
                    Command command = CommandBuilder.Build(hall, kitchen, input);
                    command.ExecuteCommand(hall, kitchen);
                    Console.WriteLine($"{command.FullCommand} -> {command.Result}");
                    applicationContext.SaveAll(storage.Ingredients, storage.IngredientsAmount, storage.IngredientsPrice, kitchen.Storage.Recipes, hall.Customers, kitchen.Budget);
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
            foreach(Food food in kitchen.Storage.Recipes)
            {
                if (kitchen.Storage.FoodAmount[food] == 0)
                {
                    continue;
                }
                Console.Write($"{food.Name} {kitchen.Storage.FoodAmount[food]}, ");
            }
            Console.Write('\n');
        }
    }
}
