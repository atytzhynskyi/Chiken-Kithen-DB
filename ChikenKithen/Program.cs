using ChikenKitchenDataBase;
using System;
using System.Linq;
using AdvanceClasses;
using CommandsModule;
using jsonReadModule;
using Randomizer;

namespace ChikenKithen
{
    class Program
    {
        static void Main(string[] args)
        {
            using (ApplicationContext applicationContext = new ApplicationContext())
            {
                var rnd = new Rnd();

                applicationContext.InitializeFromFiles();
                applicationContext.SetPropertiesIngredientsId();

                var TaxConfigs = JsonRead.ReadFromJson<double>(@"..\..\..\Configs\TaxConfigs.json");
                var tipConfig = JsonRead.ReadFromJson<double>(@"..\..\..\Configs\TipConfig.json");
                Accounting accounting = new Accounting(applicationContext.GetBudget(),
                                                        TaxConfigs.Where(k => k.Key == "transaction tax").First().Value,
                                                        TaxConfigs.Where(k => k.Key == "profit margin").First().Value,
                                                        TaxConfigs.Where(k => k.Key == "daily tax").First().Value,
                                                        tipConfig.Where(k => k.Key == "max tip").First().Value,
                                                        TaxConfigs.Where(k => k.Key == "tip tax").First().Value,
                                                        TaxConfigs.Where(k=> k.Key == "waste tax").First().Value,
                                                        applicationContext.GetIngredientsPrice(),
                                                        rnd) ;

                var WarehouseSize = JsonRead.ReadFromJson<int>(@"..\..\..\Configs\WarehouseSize.json");
                var spoilConfig = JsonRead.ReadFromJson<double>(@"..\..\..\Configs\SpoilConfig.json");
                var ordersDeliveries = JsonRead.ReadFromJson<int>(@"..\..\..\Configs\OrdersDeliveries.json");
                Storage storage = new Storage(applicationContext.GetFoods(),
                                              applicationContext.Ingredients.ToList(),
                                              applicationContext.GetFoodsAmount(),
                                              applicationContext.GetIngredientsAmount(),
                                              applicationContext.GetIngredientsTrashAmount(),
                                              applicationContext.GetTotalTrashAmount(),
                                              WarehouseSize.Where(k => k.Key == "max ingredient type").First().Value,
                                              WarehouseSize.Where(k => k.Key == "max dish type").First().Value,
                                              WarehouseSize.Where(k => k.Key == "total maximum").First().Value,
                                              WarehouseSize.Where(k => k.Key == "waste limit").First().Value,
                                              spoilConfig.Where(k => k.Key == "spoil rate").First().Value,
                                              ordersDeliveries.Where(k => k.Key == "order ingredient volatility").First().Value,
                                              ordersDeliveries.Where(k => k.Key == "order dish volatility").First().Value,
                                              rnd);

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
                    //command.IsAllowed = JsonRead.ReadFromJson<bool>(@"..\..\..\CommandsAllow.json");

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
                        storage.TrashAmount);

                    recordsBase.AddRecordIfSomeChange(command, kitchen, accounting);
                }
                
            }
        }

        static void ShowWarehouse(Kitchen kitchen)
        {
            foreach (var ingredient in kitchen.Storage.IngredientsAmount.Where(i => i.Value != 0))
            {
                Console.Write($"{ingredient.Key.Name} {ingredient.Value}, ");
            }
            foreach (var food in kitchen.Storage.FoodAmount.Where(f => f.Value != 0))
            {
                Console.Write($"{food.Key.Name} {food.Value}, ");
            }
            Console.Write("\nWaste:");
            foreach(var ingredient in kitchen.Storage.IngredientsTrashAmount.Where(i=>i.Value != 0))
            {
                Console.Write($"{ingredient.Key.Name} {ingredient.Value}, ");
            }
            Console.Write("\n");
        }
    }
}
