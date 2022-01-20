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
    public class TableTest
    {
        Ingredient potatoes = new Ingredient("Potatoes");
        Ingredient tuna = new Ingredient("Tuna");
        Food smashedPotatoes;
        Food fries;
        Food irishFish;
        List<Ingredient> ingredients;
        List<Food> recipes;
        Storage storage;
        Accounting accounting;
        Kitchen kitchen;
        Customer bill;
        Customer den;
        Customer ketty;
        Customer tomas;
        Hall hall;

        Table command;
        [TestInitialize]
        public void SetupContext()
        {
            Dictionary<Ingredient, int> ingredientsPrice = new Dictionary<Ingredient, int>();
            ingredientsPrice.Add(potatoes, 3);
            ingredientsPrice.Add(tuna, 25);

            accounting = new Accounting(500, 0.5, 0, 0, 0, 0, 0, ingredientsPrice, new Rnd(0));

            ingredients = new List<Ingredient> { potatoes, tuna };

            smashedPotatoes = new Food("Smashed Potatoes");
            smashedPotatoes.RecipeIngredients.Add(potatoes);

            fries = new Food("Fries");
            fries.RecipeIngredients.Add(potatoes);

            irishFish = new Food("Irish Fish");
            irishFish.RecipeIngredients.Add(tuna);
            irishFish.RecipeFoods.Add(smashedPotatoes);
            irishFish.RecipeFoods.Add(fries);


            recipes = new List<Food> { irishFish, smashedPotatoes, fries };
            storage = new Storage(recipes, ingredients);

            storage.IngredientsAmount[potatoes] = 10;
            storage.IngredientsAmount[tuna] = 10;

            kitchen = new Kitchen(storage);

            bill = new Customer("Bill");
            den = new Customer("Den");
            tomas = new Customer("Tomas");
            ketty = new Customer("Ketty", tuna);

            hall = new Hall(new List<Customer> { bill, den, tomas, ketty }, recipes);
            hall.Customers.ForEach(c => c.budget = 50);
        }
        [TestMethod]
        public void TestTableNotAllowed()
        {
            command = new Table(accounting, hall, kitchen, "Table, Den, Bill, Tomas, Ketty, Irish Fish, Irish Fish, Irish Fish");
            command.ExecuteCommand();
            Assert.AreEqual("Command not allowed", command.Result, "Executed not allowed command");
        }

        [TestMethod]
        public void TestTableSuccess()
        {
            command = new Table(accounting, hall, kitchen, "Table, Den, Bill, Tomas, Ketty, Irish Fish, Irish Fish, Irish Fish, Irish Fish");
            command.IsAllowed = true;

            //3(potatoes)+3(potatoes)+25(tuna) = 31; 31 * 0.5 (tax) = 15.5
            var expectedBudget = 546.5;                 //500 + 15.5 * 3 + 0(allergy)
            var expectedBudgetOfCustomerDen = 19;       //50 - 31;
            var expectedBudgetOfCustomerBill = 19;      //50 - 31;
            var expectedBudgetOfCustomerTomas = 19;     //50 - 31;
            var expectedBudgetOfCustomerKetty = 50;     //50 - 0;
            var expectedMoneyAmount = 46.5;             //15.5 * 3
            var expectResult = $"success; money amount: {expectedMoneyAmount}; tax:";

            command.ExecuteCommand();

            Assert.IsTrue(command.Result.StartsWith(expectResult));
            Assert.AreEqual(expectedBudget, accounting.Budget);
            Assert.AreEqual(expectedBudgetOfCustomerDen, hall.Customers.Find(c => c == den).budget);
            Assert.AreEqual(expectedBudgetOfCustomerBill, hall.Customers.Find(c => c == bill).budget);
            Assert.AreEqual(expectedBudgetOfCustomerTomas, hall.Customers.Find(c => c == tomas).budget);
            Assert.AreEqual(expectedBudgetOfCustomerKetty, hall.Customers.Find(c => c == ketty).budget);
        }

        [TestMethod]
        public void TestTablePooledOnAndOneCustomerHasNotEnoughMoneySuccess()
        {
            Dictionary<Ingredient, int> ingredientsPrice = new Dictionary<Ingredient, int>();
            ingredientsPrice.Add(potatoes, 3);
            ingredientsPrice.Add(tuna, 25);

            accounting = new Accounting(500, 0.5, 0, 0, 0, 0.4, 0, ingredientsPrice, new Rnd(0));

            //Tomas has not enough money
            hall.Customers.Find(c => c == tomas).budget = 20;

            hall.Customers.Find(c => c == den).budget = 100;
            hall.Customers.Find(c => c == bill).budget = 50;
            hall.Customers.Find(c => c == ketty).budget = 40;

            command = new Table(accounting, hall, kitchen, "Table, Pooled, Den, Bill, Tomas, Ketty, Irish Fish, Irish Fish, Irish Fish, Irish Fish");
            command.IsAllowed = true;

            //3(potatoes)+3(potatoes)+25(tuna) = 31; 31 * 0.5 (tax) = 15.5
            //93 * 0.4 = 37.2 - tip
            var expectedBudget = 583.7;                     //500 + 15.5 * 3 + 0(allergy) + 37.2(tip)
            var expectedBudgetOfCustomerDen = 38.15;        //100 - 36.5(pooled) - 25.35(tip)
            var expectedBudgetOfCustomerBill = 1.65;        //50 - 36.5(pooled) - 11.85(tip)
            var expectedBudgetOfCustomerTomas = 0;          //20 + 11(pooled) - 31(price)
            var expectedBudgetOfCustomerKetty = 40;         //40
            var expectedMoneyAmount = 83.7;                 //500 + 46.5 + 37.2(tips);
            var expectResult = $"success; money amount: {expectedMoneyAmount}; tax:";

            command.ExecuteCommand();

            Assert.IsTrue(command.Result.StartsWith(expectResult));
            Assert.AreEqual(expectedBudget, accounting.Budget);
            Assert.AreEqual(expectedBudgetOfCustomerDen, hall.Customers.Find(c => c == den).budget);
            Assert.AreEqual(expectedBudgetOfCustomerBill, hall.Customers.Find(c => c == bill).budget);
            Assert.AreEqual(expectedBudgetOfCustomerTomas, hall.Customers.Find(c => c == tomas).budget);
            Assert.AreEqual(expectedBudgetOfCustomerKetty, hall.Customers.Find(c => c == ketty).budget);
        }

    }
}
