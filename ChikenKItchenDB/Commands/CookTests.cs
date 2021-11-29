using Microsoft.VisualStudio.TestTools.UnitTesting;
using CommandsModule;
using System;
using System.Collections.Generic;
using System.Text;
using AdvanceClasses;
using BaseClasses;

namespace CommandsModule.Tests
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
        Accounting accounting = new Accounting(500, 0.5, 0, 0);
        Kitchen kitchen;

        Cook command;
        [TestInitialize]
        public void SetupContext()
        {
            accounting = new Accounting(500, 0.5, 0, 0);

            ingredients = new List<Ingredient> { salt, water };
            saltWater = new Food("Salt water", ingredients.ToArray());
            Recipes = new List<Food> { saltWater };
            storage = new Storage(Recipes, ingredients, 10, 10, 25);

            storage.IngredientsPrice[salt] = 10;
            storage.IngredientsPrice[water] = 10;

            storage.IngredientsAmount[salt] = 10;
            storage.IngredientsAmount[water] = 10;

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
        /*[TestMethod()]
        public void TestCookLimitWaste()
        {
            command = new Cook(kitchen, "Cook, Salt water, 15");
            command.IsAllowed = true;

            command.ExecuteCommand();

            Assert.AreEqual("Failed to cook food", command.Result);
            Assert.AreEqual(10, storage.FoodAmount[saltWater], "dish count incorrect");
        }
        */
    }
}