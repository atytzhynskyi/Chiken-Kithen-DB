using AdvanceClasses;
using BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandsModule
{
    public class Table : ICommand
    {
        public string FullCommand { get; private set; }
        public string CommandType { get; private set; }
        public string Result { get; private set; }
        public bool IsAllowed { get; set; }

        private Accounting accounting { get; set; }
        private Kitchen kitchen { get; set; }
        private Hall hall { get; set; }

        public List<Buy> Buys = new List<Buy>();

        List<Customer> _Customers = new List<Customer>();
        List<Food> _Orders = new List<Food>();

        public Table(Accounting accounting, Hall hall, Kitchen kitchen, string _FullCommand)
        {
            this.accounting = accounting;
            this.hall = hall;
            this.kitchen = kitchen;

            FullCommand = _FullCommand;
            CommandType = FullCommand.Split(", ")[0];
            IsAllowed = false;

            foreach (var foodName in GetFoodsFromCommand())
            {
                Food searchFood = kitchen.Storage.GetRecipeByName(foodName);
                if (!object.Equals(searchFood, null)) _Orders.Add(searchFood);
            }

            foreach (var customerName in GetCustomersFromCommand())
            {
                Customer searchCustomer = hall.GetCustomer(customerName);
                if (!object.Equals(searchCustomer, null)) _Customers.Add(searchCustomer);
            }
        }
        public void ExecuteCommand()
        {
            if (!IsAllowed)
            {
                Result = "Command not allowed";
                return;
            }

            SetCustomersOrders();

            SetResultIfIssues();
            if (!object.Equals(Result, null)) return;

            Buys = FormBuysFromCommand();

            double startBudget = accounting.Budget;
            foreach (var buy in Buys)
            {
                buy.ExecuteCommand();
            }

            double moneyAmount = accounting.Budget - startBudget;
            double tax = 0;
            Result = $"success; money amount: {moneyAmount}; tax:{moneyAmount * accounting.transactionTax}\n"+'{';

            Buys.ForEach(x => Result += $"\n\t{x.FullCommand} -> {x.Result}");
            Result += "\n}";
        }

        private void SetCustomersOrders()
        {
            if (_Customers.Count != _Orders.Count)
            {
                //I dont set result to "FAIL" becouse this method must only set customers orders
                return;
            }
            for (int i = 0; i < _Customers.Count; i++)
            {
                _Customers[i].Order = _Orders[i];
            }
        }

        private List<Buy> FormBuysFromCommand()
        {
            List<Buy> buys = new List<Buy>();


            if (_Customers.Count != _Orders.Count)
            {
                Result = "Customers and Orders counts doesnt equal";
                return buys;
            }

            for (int i = 0; i < _Customers.Count; i++)
            {
                buys.Add(new Buy(accounting, hall, kitchen, $"Buy, {_Customers[i].Name}, {_Orders[i].Name}"));
                buys[i].IsAllowed = true;
            }
            return buys;
        }
        private List<string> GetCustomersFromCommand()
        {
            List<string> commandSplit = new List<string>(FullCommand.Split(", "));
            List<string> customersName = (List<string>)commandSplit.Where(s => hall.GetCustomer(s).Name != "NULL").ToList();
            return customersName;
        }
        private List<string> GetFoodsFromCommand()
        {
            List<string> commandSplit = new List<string>(FullCommand.Split(", "));
            List<string> foodsName = commandSplit.Where(s => !object.Equals(kitchen.Storage.GetRecipeByName(s), null)).ToList();
            return foodsName;
        }
        private void SetResultIfIssues()
        {
            if (_Customers.Count > _Orders.Count)
            {
                Result = "FAIL. Every person needs something to eat. So, whole table fails.";
                return;
            }

            if (_Customers.Count < _Orders.Count)
            {
                Result = "FAIL. One person can have one type of food only. So, whole table fails";
                return;
            }

            if (_Customers.Count != _Customers.Distinct().Count())
            {
                Result = "FAIL. One person cant be by one table twice. So, whole table fails.";
                return;
            }

            if (!IsEnoughIngredients())
            {
                Result = "FAIL. We dont have enough ingredients";
                return;
            }

            if (_Customers.Any(c => c.budget < accounting.CalculateFoodMenuPrice(
                                                         kitchen.Storage.Recipes, kitchen.Storage.IngredientsPrice, c.Order)))
            {
                Result = "FAIL. One or more persons dont have enough money";
                return;
            }
        }

        private bool IsEnoughIngredients()
        {
            List<string> foodsName = GetFoodsFromCommand();

            //form MEGAfood recipe which contain every recipes of foods in orders
            Food megaFood = new Food("");
            foreach (var foodName in foodsName)
            {
                Food searchFood = kitchen.Storage.GetRecipeByName(foodName); //Get Food by Name
                megaFood.RecipeFoods.AddRange(
                    searchFood.RecipeFoods.
                    Where(x => true)); //"Where" using to prevent linking to one list

                megaFood.RecipeIngredients.AddRange(
                    searchFood.RecipeIngredients.Where(x => true));
            }

            return true;
        }
    }
}
