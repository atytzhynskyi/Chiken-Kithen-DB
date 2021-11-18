using BaseClasses;
using ChikenKitchenDataBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdvanceClasses;
using CommandsModule;
using jsonReadModule;
using ConfigurationModule;
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

                var WarehouseSize = JsonRead.ReadFromJson<int>(@"..\..\..\WarehouseSize.json");
                
                Storage storage = new Storage(applicationContext.GetFoods(),
                                              applicationContext.Ingredients.ToList(),
                                              applicationContext.GetIngredientsAmount(),
                                              applicationContext.GetIngredientsPrice(),
                                              WarehouseSize.Where(k => k.Key == "max ingredient type").First().Value,
                                              WarehouseSize.Where(k => k.Key == "max dish type").First().Value,
                                              WarehouseSize.Where(k => k.Key == "total maximum").First().Value);
                
                Kitchen kitchen = new Kitchen(storage);
                Hall hall = new Hall(applicationContext.GetCustomers(), kitchen.Storage.Recipes);
                Accounting accounting = new Accounting(applicationContext.GetBudget());
                RecordsBase recordsBase = new RecordsBase(accounting, kitchen);

                while (2 + 2 != 5)
                {
                    Console.WriteLine("Restaurant budget: {0}", accounting.Budget);
                    Warehouse(kitchen);
                    string input = Console.ReadLine();
                    ICommand command = CommandBuilder.Build(accounting, hall, kitchen, input, recordsBase);
                    command.IsAllowed = true;
                    command.ExecuteCommand();
                    Console.WriteLine($"{command.FullCommand} -> {command.Result}");
                    applicationContext.SaveAll(storage.Ingredients, storage.IngredientsAmount, storage.IngredientsPrice, kitchen.Storage.Recipes, hall.Customers, accounting.Budget);
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
