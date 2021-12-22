using ChikenKitchenDataBase;
using System;
using System.Linq;
using AdvanceClasses;
using CommandsModule;
using jsonReadModule;
using System.IO;

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
                                              applicationContext.GetIngredientsTrashAmount(),
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

                    applicationContext.SaveAll(
                        storage.Ingredients, 
                        storage.IngredientsAmount, 
                        accounting.IngredientsPrice, 
                        kitchen.Storage.Recipes, 
                        storage.FoodAmount, 
                        hall.Customers, 
                        accounting.Budget, 
                        storage.IngredientsTrashAmount, 
                        storage.ThrowTrashAway);

                    recordsBase.AddRecordIfSomeChange(command,kitchen, accounting);

                    if (storage.ThrowTrashAway)
                    {
                        var fileName = $@"..\..\..\TrashHistory.txt";

                        if (!File.Exists(fileName))
                        {
                            File.WriteAllText(fileName, "");
                        }

                        var trashHistory = File.ReadAllText(fileName);


                        var currentTrash = "Current trash: ";

                        foreach (var trash in storage.IngredientsTrashAmount)
                        {
                            if (trash.Value > 0)
                            {
                                currentTrash += $"{trash.Key.Name}: {trash.Value},";
                            }
                        }

                        var trashes = applicationContext.GetTrashes();

                        var totalTrash = "Total trash: ";
                        foreach (var trash in trashes)
                        {
                            if (trash.Value > 0)
                            {
                                totalTrash += $"{trash.Key.Name}: {trash.Value},";
                            }
                        }

                        trashHistory += "\n=================================\n" + currentTrash + "\n" + totalTrash + "\n";

                        File.WriteAllText(fileName, trashHistory);


                        storage.IngredientsTrashAmount = applicationContext.GetIngredientsTrashAmount();
                        storage.ThrowTrashAway = false;
                    }
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
