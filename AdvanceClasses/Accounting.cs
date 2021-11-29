using BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdvanceClasses
{
    public class Accounting
    {
        public double Budget { get; private set; }

        public double CollectedTax { get; private set; } = 0;
        public readonly double transactionTax;
        public readonly double dailyTax;
        public readonly double marginProfit;
        readonly double startBudget;

        public Accounting(double _Budget)
        {
            Budget = _Budget;
            transactionTax = 0.10;
            dailyTax = 0;
            marginProfit = 0;
            startBudget = _Budget;
        }
        public Accounting(double _Budget, int _transactionTax, int _marginProfit, int _dailyTax)
        {
            Budget = _Budget;
            startBudget = _Budget;
            transactionTax = Math.Round((float)_transactionTax/100,2);
            marginProfit = Math.Round((float)_marginProfit /100, 2);
            dailyTax = Math.Round((float)_dailyTax /100, 2);
        }
        public Accounting(double _Budget, double _transactionTax, double _marginProfit, double _dailyTax)
        {
            Budget = _Budget;
            startBudget = _Budget;
            transactionTax = _transactionTax;
            marginProfit = _marginProfit;
            dailyTax = _dailyTax;
        }

        public double CalculateFoodCostPrice(List<Food> Recipes, Dictionary<Ingredient, int> ingredientsPrice, Food food)
        {

            double price = 0;
            foreach (Food foodRecipe in food.RecipeFoods)
            {
                price += CalculateFoodCostPrice(Recipes, ingredientsPrice, foodRecipe);
            }
            foreach (Ingredient ingredient in food.RecipeIngredients)
            {
                price += ingredientsPrice[ingredientsPrice.Keys.Where(i => i.Name == ingredient.Name).First()];
            }
            return Math.Round(price,2);
        }
        public double CalculateFoodMenuPrice(List<Food> Recipes, Dictionary<Ingredient, int> ingredientsPrice, Food food)
        {
            return Math.Round(CalculateFoodCostPrice(Recipes, ingredientsPrice, food) * (1 + marginProfit) * (1 + transactionTax), 2);
        }
        public void PayDayTax()
        {
            Budget = Math.Round(Budget - CalculateDailyTax(),2);
        }
        public double CalculateDailyTax()
        {
            double profit = Budget - startBudget - CollectedTax;
            double dailyTax = profit * this.dailyTax;

            if (dailyTax <= 0) return 0;

            return Math.Round(dailyTax, 2);
        }
        public void UseMoney(double amount)
        {
            Budget -= Math.Round(amount + CalculateTransactionTax(amount), 2);
            CollectedTax += Math.Round(CalculateTransactionTax(amount), 2);
            Budget = Math.Round(Budget, 2);
        }
        public void AddMoney(double amount)
        {
            Budget = Math.Round(Budget + amount - CalculateTransactionTax(amount), 2);
            CollectedTax = Math.Round(CollectedTax + CalculateTransactionTax(amount), 2);
        }
        public void AddMoneyWithoutTax(double amount)
        {
            Budget += amount;
        }
        public void UseMoneyWithoutTax(double amount)
        {
            Budget -= amount;
            Budget = Math.Round(Budget, 2);
        }
        public void SetMoney(double amount)
        {
            Budget = amount;
        }
        public double CalculateTransactionTax(double amount)
        {
            return Math.Round(amount * transactionTax, 2);
        }
        public double GetStartBudget()
        {
            return startBudget;
        }
    }
}
