using AdvanceClasses;
using BaseClasses;
using CommandsModule;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace ChikenKItchenDB.CommandsModule
{
    [TestClass()]
    public class BuyTest
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
            Dictionary<Ingredient, int> ingredientsPrice = new Dictionary<Ingredient, int> ();
            ingredientsPrice.Add(salt, 10);
            ingredientsPrice.Add(water, 10);

            accounting = new Accounting(500, 0.5, 0, 0, ingredientsPrice);

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
            hall.Customers.ForEach(c=>c.budget=100);
        }
        [TestMethod]
        public void TestBuyNotAllowed()
        {
            command = new Buy(accounting, hall, kitchen, "Buy, Den, Salt water");
            command.ExecuteCommand();
            Assert.AreEqual("Command not allowed", command.Result, "Executed not allowed command");
        }

        [TestMethod]
        public void TestBuySuccess()
        {
            command = new Buy(accounting, hall, kitchen, "Buy, Den, Salt water");
            command.IsAllowed = true;
            double price = Math.Round(accounting.CalculateFoodMenuPrice(Recipes, saltWater), 2);
            double tax = Math.Round(accounting.CalculateTransactionTax(price), 2);
            string expectResult = $"{den.Name}, {den.budget}, Salt water, {price} -> success; money amount: {price - tax}; tax: {tax};";

            command.ExecuteCommand();

            Assert.AreEqual(expectResult, command.Result, "command execute have some issues");
        }

        [TestMethod]
        public void TestBuyAllergicWaste()
        {
            command = new Buy(accounting, hall, kitchen, "Buy, Bill, Salt water");
            command.IsAllowed = true;

            string expectResult = "Can't eat: allergic to: Water; dish wasted";

            command.ExecuteCommand();

            Assert.AreEqual(expectResult, command.Result);
            Assert.AreEqual(kitchen.Storage.FoodAmount[saltWater], 0, "dish keeped");
            Assert.AreEqual(accounting.Budget, 500, "incorrect budget");
        }
        [TestMethod]
        public void TestBuyAllergicKeep()
        {
            command = new Buy(accounting, hall, kitchen, "Buy, Bill, Salt water");
            command.IsAllowed = true;
            command.AllergicConfig = "keep";

            double expectBudget = Math.Round(accounting.Budget - (accounting.CalculateFoodCostPrice(Recipes, saltWater)/25), 2);

            command.ExecuteCommand();

            string expectResult = "Can't eat: allergic to: Water; dish keeped";

            Assert.AreEqual(expectResult, command.Result);
            Assert.AreEqual(1, kitchen.Storage.FoodAmount[saltWater], "dish wasted");
            Assert.AreEqual(expectBudget, accounting.Budget, "incorrect budget");
        }

        [TestMethod]
        public void TestBuyAllergicConfigNumberKeep()
        {
            command = new Buy(accounting, hall, kitchen, "Buy, Bill, Salt water");
            command.IsAllowed = true;
            command.AllergicConfig = "100";

            double expectBudget = Math.Round(accounting.Budget - (accounting.CalculateFoodCostPrice(Recipes, saltWater) / 25), 2);

            command.ExecuteCommand();

            string expectResult = "Can't eat: allergic to: Water; dish keeped";

            Assert.AreEqual(expectResult, command.Result);
            Assert.AreEqual(1, kitchen.Storage.FoodAmount[saltWater], "dish wasted");
            Assert.AreEqual(expectBudget, accounting.Budget, "incorrect budget");
        }

        [TestMethod]
        public void TestBuyAllergicConfigNumbeWaste()
        {
            command = new Buy(accounting, hall, kitchen, "Buy, Bill, Salt water");
            command.IsAllowed = true;
            command.AllergicConfig = "1";

            string expectResult = "Can't eat: allergic to: Water; dish wasted";

            command.ExecuteCommand();

            Assert.AreEqual(expectResult, command.Result);
            Assert.AreEqual(kitchen.Storage.FoodAmount[saltWater], 0, "dish keeped");
            Assert.AreEqual(accounting.Budget, 500, "incorrect budget");
        }

        [TestMethod]
        public void TestBuyCustomer404()
        {
            command = new Buy(accounting, hall, kitchen, "Buy, Bin, Salt water");
            command.IsAllowed = true;

            string expectResult = "Customer 404";

            command.ExecuteCommand();

            Assert.AreEqual(expectResult, command.Result);
            Assert.AreEqual(kitchen.Storage.FoodAmount[saltWater], 0, "dish cooked");
            Assert.AreEqual(accounting.Budget, 500, "incorrect budget");
        }

        [TestMethod]
        public void TestBuyFood404()
        {
            command = new Buy(accounting, hall, kitchen, "Buy, Bill, ice");
            command.IsAllowed = true;

            string expectResult = "Food 404";

            command.ExecuteCommand();

            Assert.AreEqual(expectResult, command.Result);
            Assert.AreEqual(kitchen.Storage.FoodAmount[saltWater], 0, "dish cooked");
            Assert.AreEqual(accounting.Budget, 500, "incorrect budget");
        }

        [TestMethod]
        public void TestBuyNotEnoughIngredients()
        {
            command = new Buy(accounting, hall, kitchen, "Buy, Bill, Salt water");
            command.IsAllowed = true;
            storage.IngredientsAmount[water] = 0;

            string expectResult = "Can't order: dont have enough ingredients";

            command.ExecuteCommand();

            Assert.AreEqual(expectResult, command.Result);
            Assert.AreEqual(kitchen.Storage.FoodAmount[saltWater], 0, "dish cooked");
            Assert.AreEqual(accounting.Budget, 500, "incorrect budget");
        }
        [TestMethod]
        public void TestBuyFromStorage()
        {
            command = new Buy(accounting, hall, kitchen, "Buy, Den, Salt water");
            command.IsAllowed = true;
            storage.FoodAmount[saltWater] = 2;

            double price = Math.Round(accounting.CalculateFoodMenuPrice(Recipes, saltWater), 2);
            double tax = Math.Round(accounting.CalculateTransactionTax(price), 2);
            string expectResult = $"{den.Name}, {den.budget}, Salt water, {price} -> success; money amount: {price - tax}; tax: {tax};";

            command.ExecuteCommand();

            Assert.AreEqual(expectResult, command.Result);
            Assert.AreEqual(kitchen.Storage.FoodAmount[saltWater], 1, "dish amount incorrect");
            Assert.AreEqual(accounting.Budget, 510, "incorrect budget");
        }
    }
}
