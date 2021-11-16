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

                RecordsBase recordsBase = new RecordsBase(kitchen);

                while (2 + 2 != 5)
                {
                    Console.WriteLine("Restaurant budget: {0}", kitchen.Budget);
                    Warehouse(kitchen);
                    string input = Console.ReadLine();
                    Command command = CommandBuilder.Build(hall, kitchen, input, recordsBase);
                    command.ExecuteCommand(hall, kitchen);
                    Console.WriteLine($"{command.FullCommand} -> {command.Result}");
                    applicationContext.SaveAll(storage.Ingredients, storage.IngredientsAmount, storage.IngredientsPrice, kitchen.Storage.Recipes, hall.Customers, kitchen.Budget);
                }
                //*/
            }
        }
        static void Warehouse(Kitchen kitchen)
        {
            foreach(var ingredient in kitchen.Storage.IngredientsAmount.Where(i => i.Value != 0))
            {
                Console.Write($"{ingredient.Key.Name} {ingredient.Value}, ");
            }
            foreach(var food in kitchen.Storage.FoodAmount.Where(f=>f.Value != 0))
            {
                Console.Write($"{food.Key.Name} {food.Value}, ");
            }
            Console.Write('\n');
        }
    }
}
