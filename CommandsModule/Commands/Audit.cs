using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BaseClasses;
using ChikenKithen;

namespace CommandsModule
{
    public class Audit : Command
    {
        List<Record> Records = new List<Record>();
        public Audit(Hall hall, Kitchen kitchen, string commandString) : base(hall, kitchen, commandString)
        {
            Records.Add(new Record(kitchen));
        }
        public void ExecuteCommand()
        {
            if (!IsAllowed)
            {
                Result = "Command not allowed";
                return;
            }

            string fileName = "";
            for (int i = 0; true; i++)
            {
                if (!File.Exists($@"..\..\..\Audit{i}"))
                {
                    fileName = $@"..\..\..\Audit{i}";
                    break;
                }
            }

            using (StreamWriter fs = new StreamWriter(File.Create(fileName)))
            {
                fs.Write("INIT\n");
                foreach (Record record in Records)
                {
                    fs.WriteLine(record.CommandAndResult);
                    fs.WriteLine(GetWareHouse(record.IngredientsAmount, record.FoodsAmount));
                    fs.WriteLine($"Budget: {record.Budget}");

                    fs.WriteLine("");
                    if (Records.First() == record)
                    {
                        fs.WriteLine("START\n");
                    }
                }
            }
        }
        public void AddRecordIfSomeChange(string command, Kitchen kitchen, Hall hall)
        {
            if (!IsSomethingChanged(kitchen))
            {
                return;
            }
            string commandType = command.Split(", ")[0];
            if (commandType == "Buy")
            {
                AddBuyRecords(command, kitchen, hall);
                return;
            }
            if (commandType == "Table")
            {
                AddTableRecord(command, kitchen, hall);
            }

        }

        private void AddTableRecord(string command, Kitchen kitchen, Hall hall)
        {
            if (IsFoodsChanged(kitchen.Storage.FoodAmount) || IsIngredientsChanged(kitchen.Storage.IngredientsAmount))
            {
                List<string> commandSplit = new List<string>(command.Split(", "));

                List<Customer> customers = new List<Customer>();
                for (int i = 1; i < commandSplit.Count; i++)
                {
                    Customer customerTemp = hall.GetCustomer(commandSplit[i]);
                    if (customerTemp.Name != "NULL") customers.Add(customerTemp);
                    else break;
                }

                List<Food> orders = new List<Food>();
                for (int i = commandSplit.Count - 1; i > 1; i--)
                {
                    Food order = kitchen.Storage.Recipes.Where(r => r.Name == commandSplit[i]).FirstOrDefault();
                    if (object.Equals(order, null))
                    {
                        if (hall.GetCustomer(commandSplit[i]).Name != "NULL")
                        break;
                    }
                    orders.Add(order);
                }
                orders.Reverse();

                for (int i = 0; i < customers.Count; i++)
                {
                    customers[i].Order = orders[i];
                    if (kitchen.CalculateFoodMenuPrice(customers[i].Order) > customers[i].budget)
                    {
                        Console.WriteLine("FAILURE. One person can't pay for food. So, whole table fails.");
                        return;
                    }
                }


                List<string> buyRecords = new List<string>();
                foreach (Customer customer in customers)
                {
                    string recordTemp = FormBuyRecord($"Buy, {customer.Name}, {customer.Order}", kitchen, hall);
                    if (recordTemp == "NULL") continue;

                    buyRecords.Add(recordTemp);
                }
                string tableRecord = ($"{command}" + "{\n");
                foreach (string buyRecord in buyRecords)
                {
                    tableRecord += $"\t{buyRecord}\n";
                }
                tableRecord += "}";
                Records.Add(new Record(kitchen, tableRecord));
            }
        }

        public void AddBuyRecords(string command, Kitchen kitchen, Hall hall)
        {
            string record = FormBuyRecord(command, kitchen, hall);

            if (record == "NULL") return;
            else Records.Add(new Record(kitchen, record));
        }
        public string FormBuyRecord(string command, Kitchen kitchen, Hall hall)
        {
            Customer customer = hall.GetCustomer(command.Split(", ")[1]);
            Food order = kitchen.Storage.GetRecipeByName(command.Split(", ")[2]);
            if (IsFoodsChanged(kitchen.Storage.FoodAmount) || IsIngredientsChanged(kitchen.Storage.IngredientsAmount))
            {
                if (kitchen.Budget != Records.Last().Budget)
                {
                    return ($"{command} -> success");
                }
                return ($"{command} -> Customer allergic to {customer.isAllergic(kitchen.Storage.Recipes, order).Item2}");
            }
            else return "NULL";
        }
        public bool IsSomethingChanged(Kitchen kitchen)
        {
            if (kitchen.Budget != Records.Last().Budget)
            {
                return true;
            }

            if (IsFoodsChanged(kitchen.Storage.FoodAmount))
            {
                return true;
            }

            if (IsIngredientsChanged(kitchen.Storage.IngredientsAmount))
            {
                return true;
            }

            return false;
        }
        public bool IsFoodsChanged(Dictionary<Food, int> foodsAmount)
        {
            foreach (Food food in Records.Last().FoodsAmount.Keys)
            {
                if (Records.Last().FoodsAmount[food] != foodsAmount[food])
                {
                    return true;
                }
            }
            return false;
        }
        public bool IsIngredientsChanged(Dictionary<Ingredient, int> ingredientsAmount)
        {
            foreach (Ingredient ingredient in Records.Last().IngredientsAmount.Keys)
            {
                if (Records.Last().IngredientsAmount[ingredient] != ingredientsAmount[ingredient])
                {
                    return true;
                }
            }
            return false;
        }
        public string GetWareHouse(Dictionary<Ingredient, int> ingredientsAmount, Dictionary<Food, int> foodsAmount)
        {
            string WareHouse = "Warehouse: ";
            foreach (Ingredient ingredient in ingredientsAmount.Keys)
            {
                WareHouse += $"{ingredient.Name} {ingredientsAmount[ingredient]}, ";
            }
            foreach (Food food in foodsAmount.Keys)
            {
                WareHouse += $"{food.Name} {foodsAmount[food]}, ";
            }
            return WareHouse;
        }
    }
}
