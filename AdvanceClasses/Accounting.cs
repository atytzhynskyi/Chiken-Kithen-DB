using BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdvanceClasses
{
    public class Accounting
    {
        public int Budget { get; private set; }

        int CollectedTax = 0;
        readonly double transactionTax;
        readonly double dailyTax;
        readonly double marginProfit;
        readonly int startBudget;

        public Accounting(int _Budget)
        {
            Budget = _Budget;
            transactionTax = 0.10;
            dailyTax = 0;
            marginProfit = 0;
            startBudget = _Budget;
        }

        public Accounting(int _Budget, double _transactionTax, double _marginProfit)
        {

            Budget = _Budget;
            transactionTax = _transactionTax;
            marginProfit = _marginProfit;
            startBudget = _Budget;
        }

        public int CalculateFoodCostPrice(List<Food> Recipes, Dictionary<Ingredient, int> ingredientsPrice, Food food)
        {
            int price = 0;
            foreach (Food foodRecipe in food.RecipeFoods)
            {
                price += CalculateFoodCostPrice(Recipes, ingredientsPrice, foodRecipe);
            }
            foreach (Ingredient ingredient in food.RecipeIngredients)
            {
                price += ingredientsPrice[ingredientsPrice.Keys.Where(i => i.Name == ingredient.Name).First()];
            }
            return price;
        }
        public int CalculateFoodMenuPrice(List<Food> Recipes, Dictionary<Ingredient, int> ingredientsPrice, Food food)
        {
            return Convert.ToInt32(CalculateFoodCostPrice(Recipes, ingredientsPrice, food) *(1 + marginProfit));
        }
        public int CalculateDailyTax()
        {
            int profit = Budget - startBudget - CollectedTax;
            int dailyTax = Convert.ToInt32(profit * this.dailyTax);

            if (dailyTax < 0) return 0;

            return dailyTax;
        }
        public void UseMoney(int amount)
        {
            Budget -= amount - CalculateTransactionTax(amount);
            CollectedTax += CalculateTransactionTax(amount);
        }
        public void AddMoney(int amount)
        {
            Budget += amount + CalculateTransactionTax(amount);
            CollectedTax += CalculateTransactionTax(amount);
        }
        public void AddMoneyWithoutTax(int amount)
        {
            Budget += amount;
        }
        public void UseMoneyWithoutTax(int amount)
        {
            Budget -= amount;
        }
        public void SetMoney(int amount)
        {
            Budget = amount;
        }
        public int CalculateTransactionTax(int amount)
        {
            return Convert.ToInt32(amount * transactionTax);
        }
    }
}
