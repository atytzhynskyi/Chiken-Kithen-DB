using ChikenKitchenDataBase;
using System;
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

                var TaxConfigs = JsonRead.ReadFromJson<int>(@"..\..\..\Configs\TaxConfigs.json");
                Accounting accounting = new Accounting(applicationContext.GetBudget(),
                                                        TaxConfigs.Where(k => k.Key == "transaction tax").First().Value,
                                                        TaxConfigs.Where(k => k.Key == "profit margin").First().Value,
                                                        TaxConfigs.Where(k => k.Key == "daily tax").First().Value,
                                                        applicationContext.GetIngredientsPrice());

                var WarehouseSize = JsonRead.ReadFromJson<int>(@"..\..\..\Configs\WarehouseSize.json");
                var spoilConfig = JsonRead.ReadFromJson<double>(@"..\..\..\Configs\SpoilConfig.json");
                Storage storage = new Storage(applicationContext.GetFoods(),
                                              applicationContext.Ingredients.ToList(),
                                              applicationContext.GetFoodsAmount(),
                                              applicationContext.GetIngredientsAmount(),
                                              applicationContext.GetTrash(),
                                              WarehouseSize.Where(k => k.Key == "max ingredient type").First().Value,
                                              WarehouseSize.Where(k => k.Key == "max dish type").First().Value,
                                              WarehouseSize.Where(k => k.Key == "total maximum").First().Value,
                                              WarehouseSize.Where(k => k.Key == "waste limit").First().Value,
                                              spoilConfig.Where(k => k.Key == "spoil rate").First().Value);
                
                Kitchen kitchen = new Kitchen(storage);
                Hall hall = new Hall(applicationContext.GetCustomers(), kitchen.Storage.Recipes);
                RecordsBase recordsBase = new RecordsBase(accounting, kitchen);

                while (2 + 2 != 5)
                {
                    Console.WriteLine("Restaurant budget: {0}", accounting.Budget);
                    ShowWarehouse(kitchen);

                    string input = Console.ReadLine();

                    ICommand command = CommandBuilder.Build(accounting, hall, kitchen, input, recordsBase);
                    command.IsAllowed = true;

                    command.ExecuteCommand();

                    Console.WriteLine($"{command.FullCommand} -> {command.Result}\n");

                    applicationContext.SaveAll(storage.Ingredients, storage.IngredientsAmount, accounting.IngredientsPrice, kitchen.Storage.Recipes, storage.FoodAmount, hall.Customers, accounting.Budget, storage.Trash);
                    recordsBase.AddRecordIfSomeChange(command,kitchen, accounting);
                }
                //*/
            }
        }
        static void ShowWarehouse(Kitchen kitchen)
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
