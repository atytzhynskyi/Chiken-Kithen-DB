using AdvanceClasses;
using BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CommandsModule
{
    public class Order : ICommand
    {
        private const string ORDER_OPTION_CONFIG_KEY = "";

        public string FullCommand { get; private set; }
        public string CommandType { get; private set; }
        public string Result { get; private set; }
        public bool IsAllowed { get; set; }


        private string orderOption;
        public void SetOrderOption(string option)
        {
            orderOption = option;
        }

        private string _orderName;
        private Ingredient Ingredient;
        private Food Food;
        public int Amount;


        private Accounting accounting { get; set; }
        private Kitchen kitchen { get; set; }
        public Order(Accounting Accounting, Kitchen Kitchen, string _FullCommand, string _orderOption)
        {
            FullCommand = _FullCommand;
            CommandType = FullCommand.Split(", ")[0];
            IsAllowed = false;
            orderOption = _orderOption;
            kitchen = Kitchen;
            accounting = Accounting;

            int.TryParse(FullCommand.Split(", ")[2], out Amount);
        }
        public void ExecuteCommand()
        {
            SetOrdersFromCommand();
            if(!object.Equals(Result, null)) return;
                        
            SetResultIfCommandIssues();
            if (!object.Equals(Result, null)) return;

            switch (orderOption)
            {
                case ("Ingredients"):
                    OrderIngredients();
                    break;
                case ("Dishes"):
                    OrderFoods();
                    break;
                case "All":
                    OrderIngredientsAndFoods();
                    break;
                case "No":
                default:
                    Result = "Command not allowed";
                    return;
            }

        }

        private void SetResultIfCommandIssues()
        {
            if (!IsAllowed)
            {
                Result = "Command not allowed";
                return;
            }
            if (Amount <= 0)
            {
                Result = "Amount can't be negative or zero";
            }
        }

        private void SetOrdersFromCommand()
        {
            Regex regex = new Regex(@" (.+?), (\d+)");
            _orderName = regex.Matches(FullCommand).First().Groups[1].ToString();
            
            if(kitchen.Storage.Recipes.Any(r=>r.Name == _orderName))
            {
                Food = kitchen.Storage.Recipes.Find(r => r.Name == _orderName);
            }
            else if(kitchen.Storage.Ingredients.Any(i=>i.Name == _orderName))
            {
                Ingredient = kitchen.Storage.Ingredients.Find(i=>i.Name == _orderName);
            }
            else
            {
                Result = "Food or Ingredient not found";
            }
            return;
        }

        private void OrderIngredientsAndFoods()
        {
            if(Ingredient != null)
            {
                OrderIngredients();
                return;
            }
            if(Food != null)
            {
                OrderFoods();
                return;
            }
            Result = "Food or Ingredient not found";
        }

        private void OrderIngredients()
        {
            double pricesSum = accounting.IngredientsPrice[Ingredient] * Amount;

            double finalPrice = Math.Round(pricesSum + accounting.CalculateTransactionTax(pricesSum), 2);

            if (finalPrice > accounting.Budget)
            {
                Result = "Not enough money";
                return;
            }

            accounting.UseMoney(pricesSum);
            kitchen.Storage.AddIngredientAmount(Ingredient.Name, Amount);

            Result = $"success; money used:{finalPrice}; tax:{Math.Round(finalPrice-pricesSum, 2)}";
        }

        private void OrderFoods()
        {
            double pricesSum = accounting.CalculateFoodMenuPrice(kitchen.Storage.Recipes, Food);

            double finalPrice = Math.Round(pricesSum + accounting.CalculateTransactionTax(pricesSum), 2);
            
            if(finalPrice > accounting.Budget)
            {
                Result = "Not enough money";
                return;
            }

            accounting.UseMoney(pricesSum);
            kitchen.Storage.AddFoodAmount(Food.Name, Amount);

            Result = $"success; money used:{finalPrice}; tax:{Math.Round(finalPrice - pricesSum, 2)}";
        }
    }
}
