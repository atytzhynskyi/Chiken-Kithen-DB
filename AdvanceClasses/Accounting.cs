﻿using BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdvanceClasses
{
    public class Accounting
    {
        public double Budget { get; private set; }

        double CollectedTax = 0;
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

        public Accounting(double _Budget, double _transactionTax, double _marginProfit)
        {
            Budget = _Budget;
            transactionTax = _transactionTax;
            marginProfit = _marginProfit;
            startBudget = _Budget;
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
            return Math.Round(CalculateFoodCostPrice(Recipes, ingredientsPrice, food) * (1 + marginProfit), 2);
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
        }
        public void AddMoney(double amount)
        {
            Budget += Math.Round(amount - CalculateTransactionTax(amount), 2);
            CollectedTax += Math.Round(CalculateTransactionTax(amount), 2);
        }
        public void AddMoneyWithoutTax(double amount)
        {
            Budget += amount;
        }
        public void UseMoneyWithoutTax(double amount)
        {
            Budget -= amount;
        }
        public void SetMoney(double amount)
        {
            Budget = amount;
        }
        public double CalculateTransactionTax(double amount)
        {
            return Math.Round(amount * transactionTax, 2);
        }
    }
}