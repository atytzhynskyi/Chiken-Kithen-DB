using AdvanceClasses;
using BaseClasses;
using CommandsModule;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randomizer;
using System;
using System.Collections.Generic;

namespace ChikenKItchenDB.CommandsModule
{
    [TestClass()]
    public class BuyTipTest
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

        Buy command;
        [TestInitialize]
        public void SetupContext()
        {
            Dictionary<Ingredient, int> ingredientsPrice = new Dictionary<Ingredient, int>();
            ingredientsPrice.Add(salt, 10);
            ingredientsPrice.Add(water, 10);

            accounting = new Accounting(500, 0.5, 0, 0, 0.1, 0.1, 0, ingredientsPrice, new Rnd(0));

            ingredients = new List<Ingredient> { salt, water };
            saltWater = new Food("Salt water", ingredients.ToArray());
            Recipes = new List<Food> { saltWater };
            storage = new Storage(Recipes, ingredients);

            storage.IngredientsAmount[salt] = 10;
            storage.IngredientsAmount[water] = 10;

            kitchen = new Kitchen(storage);

            bill = new Customer("Bill", water);
            den = new Customer("Den", new Ingredient("Chicken"));
            hall = new Hall(new List<Customer> { bill, den }, Recipes);
            hall.Customers.ForEach(c => c.budget = 100);
        }
        [TestMethod]
        public void TestBuySuccess()
        {
            command = new Buy(accounting, hall, kitchen, "Buy, Den, Salt water");
            command.IsAllowed = true;
            double expectTip = 1.63;
            double expectPrice = Math.Round(accounting.CalculateFoodMenuPrice(Recipes, saltWater), 2);
            double expectTax = Math.Round(accounting.CalculateTransactionTax(expectPrice), 2);

            string expectResult = $"{den.Name}, {den.budget}, Salt water, {expectPrice} -> success; money amount: {Math.Round(expectPrice - expectTax + expectTip, 2)}; tax: {expectTax}; tip {expectTip}";

            command.ExecuteCommand();

            Assert.AreEqual(expectResult, command.Result);
        }
    }
}
