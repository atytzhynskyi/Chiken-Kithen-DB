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
        public int Amount;

        public string FullCommand { get; private set; }
        public string CommandType { get; private set; }
        public string Result { get; private set; }
        public bool IsAllowed { get; set; }


        private string orderOption;
        public void SetOrderOption(string option)
        {
            orderOption = option;
        }

        private List<Ingredient> Ingredients = new List<Ingredient>();
        private List<Food> Foods = new List<Food>();

        private Dictionary<string, int> orders = new Dictionary<string, int>();
        private Dictionary<string, double> ordersVolatility = new Dictionary<string, double>();

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
            if (!object.Equals(Result, null)) return;

            SetIngredientsAndFoods();
            if (!object.Equals(Result, null)) return;

            SetResultIfCommandIssues();
            if (!object.Equals(Result, null)) return;

            if (orderOption != "No")
            {
                //maybe, it's need to run only in the "OrderIngredients()"
                kitchen.Storage.RunSpoiling();
            }

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

        private void SetIngredientsAndFoods()
        {
            foreach (var order in orders)
            {
                if (!object.Equals(kitchen.Storage.GetIngredient(order.Key), null))
                {
                    Ingredients.Add(kitchen.Storage.GetIngredient(order.Key));
                    continue;
                }
                if (!object.Equals(kitchen.Storage.GetRecipe(order.Key), null))
                {
                    Foods.Add(kitchen.Storage.GetRecipe(order.Key));
                    continue;
                }
                Result = "Ingredient or Food not found";
            }
        }

        private void SetResultIfCommandIssues()
        {
            if (!IsAllowed)
            {
                Result = "Command not allowed";
                return;
            }
            if (orders.Values.Any(a => a < 0))
            {
                Result = "Amount can't be negative";
            }
        }

        private void SetOrdersFromCommand()
        {
            Regex regex = new Regex(@" (.+?), (\d+)");
            foreach (Match m in regex.Matches(FullCommand))
            {
                orders.Add(m.Groups[1].ToString(), Convert.ToInt32(m.Groups[2].ToString()));
            }
            return;
        }

        private void OrderIngredientsAndFoods()
        {
            double pricesSum = 0;

            foreach (var item in Foods)
            {
                double price = accounting.CalculateFoodCostPrice(kitchen.Storage.Recipes, item) * orders[item.Name];
                var volatility = GetVolatilityFactor(kitchen.Storage.GetDishVolatility());
                pricesSum = Math.Round(pricesSum + price * volatility, 2);

                ordersVolatility.Add(item.Name, volatility);
            }

            foreach (var item in Ingredients)
            {
                double price = accounting.IngredientsPrice[item] * orders[item.Name];
                var volatility = GetVolatilityFactor(kitchen.Storage.GetIngredientVolatility());
                pricesSum = Math.Round(pricesSum + price * volatility, 2);

                ordersVolatility.Add(item.Name, volatility);
            }

            double finalPrice = pricesSum + accounting.CalculateTransactionTax(pricesSum);
            finalPrice = Math.Round(finalPrice, 2);

            if (finalPrice > accounting.Budget)
            {
                Result = "Not enough money";
                return;
            }

            OrderIngredients();

            OrderFoods();

            Result = $"success; money used:{finalPrice}; tax:{Math.Round(finalPrice - pricesSum, 2)}";
        }

        private void OrderIngredients()
        {
            double pricesSum = 0;
            var isVolatility = ordersVolatility.Count() > 0;

            foreach (var item in Ingredients)
            {
                double volatility;
                double price = accounting.IngredientsPrice[item] * orders[item.Name];

                if (isVolatility)
                {
                    volatility = ordersVolatility[item.Name];
                }
                else
                {
                    volatility = GetVolatilityFactor(kitchen.Storage.GetIngredientVolatility());
                }

                pricesSum = Math.Round(pricesSum + price * volatility, 2);
            }

            double finalPrice = pricesSum + accounting.CalculateTransactionTax(pricesSum);
            finalPrice = Math.Round(finalPrice, 2);

            if (finalPrice > accounting.Budget)
            {
                Result = "Not enough money";
                return;
            }

            accounting.UseMoney(pricesSum);

            foreach (Ingredient ingredient in Ingredients)
            {
                kitchen.Storage.AddIngredientAmount(ingredient.Name, orders[ingredient.Name]);
            }
            Result = $"success; money used:{finalPrice}; tax:{Math.Round(finalPrice - pricesSum, 2)}";
        }

        private void OrderFoods()
        {
            double pricesSum = 0;
            var isVolatility = ordersVolatility.Count() > 0;

            foreach (var item in Foods)
            {
                double volatility;
                double price = accounting.CalculateFoodCostPrice(kitchen.Storage.Recipes, item) * orders[item.Name];

                if (isVolatility)
                {
                    volatility = ordersVolatility[item.Name];
                }
                else
                {
                    volatility = GetVolatilityFactor(kitchen.Storage.GetDishVolatility());
                }

                pricesSum = Math.Round(pricesSum + price * volatility, 2);
            }

            double finalPrice = pricesSum + accounting.CalculateTransactionTax(pricesSum);
            finalPrice = Math.Round(finalPrice, 2);

            if (finalPrice > accounting.Budget)
            {
                Result = "Not enough money";
                return;
            }

            accounting.UseMoney(pricesSum);

            foreach (Food food in Foods)
            {
                kitchen.Storage.AddFoodAmount(food.Name, orders[food.Name]);
            }

            Result = $"success; money used:{finalPrice}; tax:{Math.Round(finalPrice - pricesSum, 2)}";
        }
        private double GetVolatilityFactor(double volatility)
        {
            return volatility / 100 + 1;
        }

    }
}
