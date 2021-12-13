using Microsoft.VisualStudio.TestTools.UnitTesting;
using CommandsModule;
using System.Collections.Generic;
using AdvanceClasses;
using BaseClasses;

namespace ChikenKItchenDB.CommandsModule
{
    [TestClass()]
    public class CookTests
    {
        Ingredient salt = new Ingredient("Salt");
        Ingredient water = new Ingredient("Water");
        Food saltWater;
        List<Ingredient> ingredients;
        List<Food> Recipes;
        Storage storage;
        Accounting accounting;
        Kitchen kitchen;

        Cook command;
        [TestInitialize]
        public void SetupContext()
        {
            Dictionary<Ingredient, int> ingredientsPrice = new Dictionary<Ingredient, int>();
            ingredientsPrice.Add(salt, 10);
            ingredientsPrice.Add(water, 10);

            accounting = new Accounting(500, 0.5, 0, 0, ingredientsPrice);

            ingredients = new List<Ingredient> { salt, water };
            saltWater = new Food("Salt water", ingredients.ToArray());
            Recipes = new List<Food> { saltWater };
            storage = new Storage(Recipes, ingredients, new Dictionary<Food, int>(), new Dictionary<Ingredient, int>(), 10, 10, 25, 0);

            storage.IngredientsAmount.Add(salt, 10);
            storage.IngredientsAmount.Add(water, 10);
            storage.FoodAmount.Add(saltWater, 0);

            kitchen = new Kitchen(storage);
        }
        [TestMethod()]
        public void TestSingleCookSuccess()
        {
            command = new Cook(kitchen, "Cook, Salt water, 1");
            command.IsAllowed = true;

            command.ExecuteCommand();

            Assert.AreEqual(command.Result, "success");
            Assert.AreEqual(storage.FoodAmount[saltWater], 1, "dish count incorrect");
        }

        [TestMethod()]
        public void TestMultipleCookSuccess()
        {
            command = new Cook(kitchen, "Cook, Salt water, 3");
            command.IsAllowed = true;

            command.ExecuteCommand();

            Assert.AreEqual(command.Result, "success");
            Assert.AreEqual(storage.FoodAmount[saltWater], 3, "dish count incorrect");
        }
        [TestMethod()]
        public void TestCookLimitWaste()
        {
            command = new Cook(kitchen, "Cook, Salt water, 15");
            command.IsAllowed = true;

            command.ExecuteCommand();

            Assert.AreEqual("Failed to cook food 5 times", command.Result);
            Assert.AreEqual(10, storage.FoodAmount[saltWater], "dish count incorrect");
        }
        [TestMethod()]
        public void TestCookNotExistFood()
        {
            command = new Cook(kitchen, "Cook, Rocksugar, 15");
            command.IsAllowed = true;

            command.ExecuteCommand();

            Assert.AreEqual("Food 404", command.Result);
            Assert.AreEqual(0, storage.FoodAmount[saltWater], "dish count incorrect");
        }
    }
}