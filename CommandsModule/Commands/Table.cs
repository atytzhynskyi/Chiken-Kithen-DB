using BaseClasses;
using ChikenKithen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandsModule
{
    public class Table : Command
    {
        public List<Buy> Buys = new List<Buy>();

        List<Customer> _Customers = new List<Customer>();
        List<Food> _Orders = new List<Food>();

        public Table(Hall _Hall, Kitchen _Kitchen, string FullCommand) : base(_Hall, _Kitchen, FullCommand)
        {
            foreach(var foodName in GetFoodsFromCommand())
            {
                Food searchFood = kitchen.Storage.GetRecipeByName(foodName);
                if(searchFood.Name != "NULL") _Orders.Add(searchFood);
            }

            foreach(var customerName in GetCustomersFromCommand()) 
            {
                Customer searchCustomer = hall.GetCustomer(customerName);
                if (searchCustomer.Name != "NULL") _Customers.Add(searchCustomer);
            }
        }
        public override void ExecuteCommand()
        {
            if(!IsAllowed)
            {
                Result = "Command not allowed";
                return;
            }

            SetCustomersOrders();
            SetResultIfIssues();

            if(!object.Equals(Result, null))
            {
                Buys = FormBuysFromCommand();
                foreach (Command buy in Buys)
                {
                    buy.ExecuteCommand();
                }
                Result = "success";
            }
        }

        private void SetCustomersOrders()
        {
            if(_Customers.Count != _Orders.Count)
            {
                //I dont set result to "FAIL" becouse this method must only set customers orders
                return;
            }
            for(int i=0; i < _Customers.Count; i++)
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

            for(int i = 0; i < _Customers.Count; i++)
            {
                buys.Add(new Buy(hall, kitchen, $"Buy, {_Customers[i].Name}, {_Orders[i].Name}"));
            }
            return buys;
        }
        private List<string> GetCustomersFromCommand()
        {
            List<string> commandSplit = new List<string>(FullCommand.Split(", "));
            List<string> customersName = (List<string>)commandSplit.Where(s => hall.GetCustomer(s).Name != "NULL");
            return customersName;
        }
        private List<string> GetFoodsFromCommand()
        {
            List<string> commandSplit = new List<string>(FullCommand.Split(", "));
            List<string> foodsName = (List<string>)commandSplit.Where(s => kitchen.Storage.GetRecipeByName(s).Name != "NULL");
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

            if(_Customers.Any(c=>c.budget < kitchen.CalculateFoodMenuPrice(c.Order)))
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
            foreach(var foodName in foodsName)
            {
                Food searchFood = kitchen.Storage.GetRecipeByName(foodName); //Get Food by Name
                megaFood.RecipeFoods.AddRange(
                    searchFood.RecipeFoods.
                    Where(x=>true)); //"Where" using to prevent linking to one list

                megaFood.RecipeIngredients.AddRange(
                    searchFood.RecipeIngredients.Where(x => true));
            }

            return true;
        }
    }
}
