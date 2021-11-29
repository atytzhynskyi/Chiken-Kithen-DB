using AdvanceClasses;
using BaseClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using CommandsModule.Commands;
using CommandsModule;

namespace ChikenKItchenDB.CommandsModule
{
    [TestClass()]
    public class OrderTest
    {
        Ingredient salt = new Ingredient("Salt");
        List<Ingredient> ingredients;
        Storage storage;
        Accounting accounting = new Accounting(500, 0.5, 0, 0);
        Kitchen kitchen;
        string _FullCommand;
        ICommand command;

        [TestInitialize]
        public void SetupContext()
        {
            accounting = new Accounting(500, 0.5, 0, 0);

            ingredients = new List<Ingredient> { salt };
            storage = new Storage(ingredients);
            storage.IngredientsPrice[salt] = 10;
            
            kitchen = new Kitchen(storage);
        }
        [TestMethod]
        public void TestOrderNotAllowed()
        {
            _FullCommand = "Order, Salt, 10";
            command = new Order(accounting, kitchen, _FullCommand);

            command.ExecuteCommand();

            Assert.AreEqual(storage.IngredientsAmount[salt], 0, "Executed not allowed command");
            Assert.AreEqual(accounting.Budget, 500, "Budget reduced");
        }
        [TestMethod]
        public void TestOrderSuccess()
        {
            _FullCommand = "Order, Salt, 10";
            Order order = new Order(accounting, kitchen, _FullCommand);
            order.SetOrderOption("All");
            command = order;

            command.IsAllowed = true;
            

            command.ExecuteCommand();

            Assert.AreEqual(10, storage.IngredientsAmount[salt], "Ingredient amount doesnt right");
            Assert.AreEqual(350, accounting.Budget, "Wrong budget reduce");
        }
        [TestMethod]
        public void TestOrderIngredientDoesntInStorage()
        {
            command = new Order(accounting, kitchen, "Order, Water, 20");
            command.IsAllowed = true;

            command.ExecuteCommand();

            Assert.AreEqual(command.Result, "Ingredient or Food not found");
        }

        [TestMethod]
        public void TestOrderNotNumberCount()
        {
            command = new Order(accounting, kitchen, "Order, Salt, 2ww");
            command.IsAllowed = true;

            command.ExecuteCommand();

            Assert.AreEqual(0, storage.IngredientsAmount[salt], "Ingredient amount doesnt right");
            Assert.AreEqual(500, accounting.Budget, "Wrong budget reduce");
        }
    }
}
