using BaseClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AdvanceClasses;
using ChikenKithen;

namespace CommandModule
{
    public class Command
    {
        Dictionary<string, bool> permissions = new Dictionary<string, bool>();
        public Command() { }

        public void SetPermisionsFromFile()
        {
            using(FileStream fs = new FileStream("config.json", FileMode.Open))
            {
                //permissions.Add()
            }
        }


        public void ExecuteCommand(string command, Kitchen kitchen, Hall hall, Audit audit)
        {
            string commandType = command.Split(", ")[0];
            switch (commandType)
            {
                case ("Buy"):
                    if (kitchen.Budget <= 0)
                    {
                        Console.WriteLine("RESTAURANT BANKRUPT");
                        return;
                    }
                    Buy(command.Split(", ")[1], command.Split(", ")[2], kitchen, hall);
                    break;
                case ("Order"):
                    if (kitchen.Budget <= 0)
                    {
                        Console.WriteLine("RESTAURANT BANKRUPT");
                        return;
                    }
                    Order(command, kitchen);
                    break;
                case ("Table"):
                    if (kitchen.Budget <= 0)
                    {
                        Console.WriteLine("RESTAURANT BANKRUPT");
                        return;
                    }
                    Table(command, kitchen, hall);
                    break;
                case ("Cook"):
                    CookFood(command, kitchen);
                    break;
                case ("Ingredients"):
                    ShowIngredients(kitchen);
                    break;
                case ("Foods"):
                    ShowFoods(kitchen);
                    break;
                case ("Customers"):
                    ShowCustomers(hall);
                    break;
                case ("ExecuteFileCommands"):
                    ExecuteFileCommands(kitchen, hall, audit);
                    break;
                case ("Budget"):
                    ChangeBudget(command, kitchen);
                    break;
                case ("Audit"):
                    audit.CreateAuditFile();
                    break;
                default:
                    Console.WriteLine("Unknow command");
                    return;
            }
            audit.AddRecordIfSomeChange(command, kitchen, hall);
        }

        private void CookFood(string command, Kitchen kitchen)
        {
            string foodName = command.Split(", ")[1];
            if (!int.TryParse(command.Split(", ")[2], out int cookAmount))
            {
                Console.Write("ERROR");
                return;
            }
            Food food = kitchen.GetRecipeByName(foodName);
            if(food.Name == "NULL")
            {
                Console.Write("ERROR");
                return;
            }
            for(int i = 0; i<cookAmount; i++)
            {
                if(kitchen.IsEnoughIngredients(food))
                    kitchen.Cook(food);
            }
            Console.Write("success");
        }

        private void Table(string command, Kitchen kitchen, Hall hall)
        {
            List<string> commandSplit = new List<string>(command.Split(", "));
            
            List<Customer> customers = new List<Customer>();
            for(int i=1; i<commandSplit.Count; i++)
            {
                Customer customerTemp = hall.GetCustomer(commandSplit[i]);
                if (customerTemp.Name != "NULL") customers.Add(customerTemp);
                else break;
            }

            List<Food> orders = new List<Food>();
            for(int i = commandSplit.Count-1; i>1; i--)
            {
                Food order = kitchen.Recipes.Where(r=>r.Name == commandSplit[i]).FirstOrDefault();
                if (object.Equals(order, null))
                {
                    if(hall.GetCustomer(commandSplit[i]).Name!="NULL");
                    break;
                }
                orders.Add(order);
            }
            orders.Reverse();

            if (customers.Count > orders.Count)
            {
                Console.Write("ERROR. Every person needs something to eat. So, whole table fails.");
                return;
            }

            if(customers.Count < orders.Count)
            {
                Console.Write("ERROR. One person can have one type of food only. So, whole table fails");
                return;
            }

            if (customers.Count!=customers.Distinct().Count())
            {
                Console.Write("ERROR. One person cant be by one table twice. So, whole table fails.");
                return;
            }

            for(int i = 0; i <customers.Count; i++)
            {
                customers[i].Order = orders[i];
                if (kitchen.CalculateFoodMenuPrice(customers[i].Order) > customers[i].budget)
                {
                    Console.WriteLine("FAILURE. One person can't pay for food. So, whole table fails.");
                    return;
                }
            }

            Console.WriteLine("\n{");
            foreach(Customer customer in customers)
            {
                string commandBuy = $", {customer.Name}, {customer.Order.Name}";
                Console.Write($"\t{customer.Name}, {customer.Order.Name} -> ");
                Buy(customer.Name, customer.Order.Name, kitchen, hall);
                Console.Write('\n');
            }
            Console.Write("}\n");
        }

        private void ShowCustomers(Hall hall)
        {
            foreach(Customer customer in hall.Customers)
            {
                Console.WriteLine("Name: {0}", customer.Name);
                Console.WriteLine("Budget: {0}", customer.budget);
                Console.Write("Allergies: ");
                foreach(Ingredient ingredient in customer.Allergies.Distinct())
                {
                    Console.Write("{0}, ",ingredient.Name);
                }
                Console.Write("\n\n");
            }
        }

        private void ChangeBudget(string command, Kitchen kitchen)
        {
            if (!int.TryParse(command.Split(", ")[2], out int count)) { return; }
            string signCommand = command.Split(", ")[1];
            switch (signCommand)
            {
                case ("="):
                    EquateBudget(kitchen, count);
                    break;
                case ("-"):
                    MinuseBudget(kitchen, count);
                    break;
                case ("+"):
                    PlusBudget(kitchen, count);
                    break;
                default:
                    break;
            }
        }

        private void PlusBudget(Kitchen kitchen, int count)
        {
            kitchen.Budget += count;
        }

        private void MinuseBudget(Kitchen kitchen, int count)
        {
            kitchen.Budget -= count;
        }

        private void EquateBudget(Kitchen kitchen, int budget)
        {
            kitchen.Budget = budget;
        }

        public void ShowFoods(Kitchen kitchen)
        {
            foreach (Food food in kitchen.Recipes)
            {
                Console.WriteLine(food.Name);
                {
                    foreach (var recipeFood in food.RecipeFoods)
                    {
                        Console.Write(recipeFood.Name);
                        Console.Write(", ");
                    }
                    foreach (Ingredient ingredient in food.RecipeIngredients)
                    {
                        Console.Write(ingredient.Name);
                        Console.Write(", ");
                    }
                    Console.WriteLine("\n=====");
                }
            }
        }
        public void Buy(string customerName, string foodName, Kitchen kitchen, Hall hall)
        {
            if (hall.isNewCustomer(customerName))
            {
                return;
            }
            Customer customer = hall.GetCustomer(customerName);
            if (!kitchen.Recipes.Any(f => f.Name == foodName))
            {
                return;
            }
            customer.Order = kitchen.GetRecipeByName(foodName);
            //customer.SetOrder(kitchen.RecipeBook.Recipes, food);
            if (customer.budget < kitchen.CalculateFoodMenuPrice(customer.Order))
            {
                Console.Write("Can't order: customer dont have enough money");
                return;
            }

            if (!kitchen.IsEnoughIngredients(customer.Order))
            {
                Console.Write("Can't order: dont have enough ingredients");
                return;
            }

            if (kitchen.FoodAmount[customer.Order] <= 0)
            {
                kitchen.Cook(customer.Order);
            }
            hall.GiveFood(kitchen, customer);

            if (customer.isAllergic(kitchen.Recipes, customer.Order).Item1)
            {
                
                Console.WriteLine($"Can't eat: allergic to: {customer.isAllergic(kitchen.Recipes, customer.Order).Item2.Name}");
                return;
            }

            hall.GetPaid(kitchen, customer);
            Console.Write("success");
        }
        public void Order(string command, Kitchen kitchen)
        {
            Ingredient ingredient = new Ingredient(command.Split(", ")[1]);
            int amount = Convert.ToInt32(command.Split(", ")[2]);
            if (!kitchen.Storage.Ingredients.Any(i => i.Name == ingredient.Name))
            {
                return;
            }
            ingredient = kitchen.Storage.Ingredients.Where(i => i.Name == ingredient.Name).First();
            if (amount * kitchen.Storage.IngredientsPrice[ingredient] > kitchen.Budget)
            {
                Console.WriteLine("Can't order: dont have enough money");
            }
            kitchen.Budget -= amount * kitchen.Storage.IngredientsPrice[ingredient];
            kitchen.Storage.IngredientsAmount[ingredient] += Convert.ToInt32(command.Split(",")[2]);
            Console.WriteLine("success");
        }
        public void ShowIngredients(Kitchen kitchen)
        {
            kitchen.Storage.ShowIngredients();
        }
        public void ExecuteFileCommands(Kitchen kitchen, Hall hall, Audit audit)
        {
            using (StreamReader sr = new StreamReader(@"..\..\..\Commands.csv"))
            {
                string readLine;
                while ((readLine = sr.ReadLine()) != null)
                {
                    ExecuteCommand(readLine, kitchen, hall, audit);
                }
            }
        }
    }
}
