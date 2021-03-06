using AdvanceClasses;
using BaseClasses;
using CommandsModule;
using CommandsModule.Commands;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randomizer;
using System;
using System.Collections.Generic;

namespace ChikenKItchenDB.CommandsModule
{
    [TestClass()]
    public class EndDayTest
    {
        Ingredient salt = new Ingredient("Salt");
        Ingredient water = new Ingredient("Water");
        Food saltWater;
        List<Ingredient> ingredients;
        List<Food> Recipes;
        Storage storage;
        Accounting accounting;
        Kitchen kitchen;
        Customer bill;
        Customer den;
        Hall hall;

        Buy BuyCommand;
        [TestInitialize]
        public void SetupContext()
        {
            Dictionary<Ingredient, int> ingredientsPrice = new Dictionary<Ingredient, int>();
            ingredientsPrice.Add(salt, 10);
            ingredientsPrice.Add(water, 10);

            accounting = new Accounting(500, 0.5, 0, 0.1, 0.1, 0.1, 0.1, ingredientsPrice, new Rnd(0));

            ingredients = new List<Ingredient> { salt, water };

            var ingredientsAmount = new Dictionary<Ingredient, int>();
            ingredientsAmount.Add(salt, 10);
            ingredientsAmount.Add(water, 10);
            
            var ingredientsTrashAmount = new Dictionary<Ingredient, int>();
            ingredientsTrashAmount.Add(salt, 10);
            ingredientsTrashAmount.Add(water, 10);

            var foodsAmount = new Dictionary<Food, int>();
            saltWater = new Food("Salt water", ingredients.ToArray());
            foodsAmount.Add(saltWater, 0);
            Recipes = new List<Food> { saltWater };
            storage = new Storage(Recipes,
                ingredients,
                foodsAmount,
                ingredientsAmount,
                ingredientsTrashAmount,
                new Dictionary<Ingredient, int>(),
                int.MaxValue,
                int.MaxValue,
                int.MaxValue,
                0,
                0,
                0,
                0,
                new Rnd(0));


            kitchen = new Kitchen(storage);

            bill = new Customer("Bill", water);
            den = new Customer("Den", new Ingredient("Chicken"));
            hall = new Hall(new List<Customer> { bill, den }, Recipes);
            hall.Customers.ForEach(c => c.budget = 100);
        }
        [TestMethod]
        public void EndDaySuccess()
        {
            EndDay EndDayCommand = new EndDay("", accounting, kitchen);
            BuyCommand = new Buy(accounting, hall, kitchen, "Buy, Den, Salt water");
            BuyCommand.IsAllowed = true;
            BuyCommand.ExecuteCommand();

            var expectbudget = Math.Round(accounting.Budget - accounting.CalculateEndDayTax(kitchen), 2);

            EndDayCommand.ExecuteCommand();

            Assert.AreEqual(expectbudget, accounting.Budget);
        }
    }
}
