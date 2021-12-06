using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using BaseClasses;
using AdvanceClasses;
using ChikenKitchenDataBase;
using ChikenKithen;
using CommandsModule;

namespace ChickenKitchenTests
{
    class TestAccountings
    {
        Ingredient coffee;
        Ingredient milk;
        Ingredient water;
        Ingredient sugar;
        Dictionary<Ingredient, int> ingredientsPrice;
        Food americano;
        Ingredient[] americanoIngredients;
        Food cappuccino;
        List<Ingredient> cappuccinoIngredients;
        List<Food> cappuccinoFoods;
        List<Food> foodsList;
        double budget;
        double tax;
        double profit;

        [SetUp]
        public void SetUp()
        {
            budget = 100;
            tax = 0.1;
            profit = 0.2;

            coffee = new Ingredient("coffee");
            milk = new Ingredient("milk");
            water = new Ingredient("water");
            sugar = new Ingredient("sugar");

            ingredientsPrice = new Dictionary<Ingredient, int>
            {
                { coffee, 20 },
                { milk, 10 },
                { water, 5 },
                { sugar, 8 }
            };

            americanoIngredients = new Ingredient[] { coffee, water };
            americano = new Food("americano", americanoIngredients);

            cappuccinoIngredients = new List<Ingredient> { coffee, milk, sugar };
            cappuccinoFoods = new List<Food> { americano };
            cappuccino = new Food("cappuccino", cappuccinoIngredients, cappuccinoFoods);

            foodsList = new List<Food> { cappuccino, americano };

        }

        [Test]
        public void CheckFoodCostPrice()                           // public double CalculateFoodCostPrice(List<Food> Recipes, Dictionary<Ingredient, int> ingredientsPrice, Food food)
        {
            // Given
            Accounting accounting = new Accounting(budget, tax, profit);

            // When
            double actual = accounting.CalculateFoodCostPrice(foodsList, ingredientsPrice, cappuccino);

            // Then
            Assert.AreEqual(63, actual);
        }

        [Test]
        public void CheckFoodCostPriceForFoodIsNull()              // public double CalculateFoodCostPrice(List<Food> Recipes, Dictionary<Ingredient, int> ingredientsPrice, Food food)
        {
            // Given
            Accounting accounting = new Accounting(budget, tax, profit);

            // When
            double actual = accounting.CalculateFoodCostPrice(foodsList, ingredientsPrice, null);

            // Then
            Assert.AreEqual(null, actual);
        }

        [Test]
        public void CheckFoodPriceInMenu()                         // public double CalculateFoodMenuPrice(List<Food> Recipes, Dictionary<Ingredient, int> ingredientsPrice, Food food)
        {
            // Given
            Accounting accounting = new Accounting(budget, tax, profit);

            // When
            double actual = accounting.CalculateFoodMenuPrice(foodsList, ingredientsPrice, cappuccino);

            // Then
            Assert.AreEqual(75.6, actual);
        }

        [Test]
        public void CheckFoodPriceInMenuForFoodIsNull()            // public double CalculateFoodMenuPrice(List<Food> Recipes, Dictionary<Ingredient, int> ingredientsPrice, Food food)
        {
            // Given
            Accounting accounting = new Accounting(budget, tax, profit);

            // When
            double actual = accounting.CalculateFoodMenuPrice(foodsList, ingredientsPrice, null);

            // Then
            Assert.AreEqual(null, actual);
        }

        [Test]
        public void CalculateDailyTaxTest()                        // public double CalculateDailyTax()
        {
            // Given
            Accounting accounting = new Accounting(budget, tax, profit);

            accounting.AddMoney(20);

            // When
            double actual = accounting.CalculateDailyTax();       // need change public readonly double dailyTax

            // Then
            Assert.AreEqual(10, actual);
        }

        [Test]
        public void CheckUseMoney()                               // public void UseMoney(double amount)
        {
            // Given
            Accounting accounting = new Accounting(budget, tax, profit);

            // When
            accounting.UseMoney(50);

            // Then
            Assert.AreEqual(45, accounting.Budget);
            Assert.AreEqual(5, accounting.CollectedTax);
        }

        [Test]
        public void CheckAddMoney()                               // public void AddMoney(double amount)
        {
            // Given
            Accounting accounting = new Accounting(budget, tax, profit);

            // When
            accounting.AddMoney(50);

            // Then
            Assert.AreEqual(145, accounting.Budget);
            // Assert.AreEqual(10, accounting.CollectedTax);
        }

        [Test]
        public void CheckAddMoneyWithoutTax()                    // public void AddMoneyWithoutTax(double amount)
        {
            // Given
            Accounting accounting = new Accounting(budget, tax, profit);

            // When
            accounting.AddMoneyWithoutTax(50);

            // Then
            Assert.AreEqual(150, accounting.Budget);
        }

        [Test]
        public void CheckUseMoneyWithoutTax()                    // public void UseMoneyWithoutTax(double amount)
        {
            // Given
            Accounting accounting = new Accounting(budget, tax, profit);

            // When
            accounting.UseMoneyWithoutTax(50);

            // Then
            Assert.AreEqual(50, accounting.Budget);
        }

        [Test]
        public void CheckSetMoney()                              // public void SetMoney(double amount)
        {
            // Given
            Accounting accounting = new Accounting(budget, tax, profit);

            // When
            accounting.SetMoney(10);

            // Then
            Assert.AreEqual(10, accounting.Budget);
        }
        
        [Test]
        public void CheckCalculateTransactionTax()               // public double CalculateTransactionTax(double amount)
        {
            // Given
            Accounting accounting = new Accounting(budget, tax, profit);

            // When
            accounting.CalculateTransactionTax(10);

            // Then
            Assert.AreEqual(0.1, accounting.transactionTax);
        }
    }
}
