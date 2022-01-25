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
        Ingredient lemon = new Ingredient("Lemon");
        Ingredient paprika = new Ingredient("Paprika");
        Ingredient salt = new Ingredient("Salt");
        Ingredient water = new Ingredient("Water");
        
        Food saltWater;

        List<Ingredient> ingredients;
        List<Food> recipes;

        Storage storage;
        Accounting accounting;
        Kitchen kitchen;
        Hall hall;

        Customer bill;
        Customer den;

        Dictionary<Ingredient, int> ingredientsPrice;

        Buy command;

        [TestInitialize]
        public void SetupContext()
        {
            ingredientsPrice = new Dictionary<Ingredient, int>();
            ingredientsPrice.Add(lemon, 5);
            ingredientsPrice.Add(paprika, 10);
            ingredientsPrice.Add(salt, 50);
            ingredientsPrice.Add(water, 100);

            var recipeIngredients = new List<Ingredient> { salt, water };
            ingredients = new List<Ingredient> { lemon, paprika, salt, water };
            saltWater = new Food("Salt water", recipeIngredients.ToArray());
            recipes = new List<Food> { saltWater };
            storage = new Storage(recipes, ingredients);

            storage.IngredientsAmount[lemon] = 1;
            storage.IngredientsAmount[paprika] = 1;
            storage.IngredientsAmount[salt] = 10;
            storage.IngredientsAmount[salt] = 10;
            storage.IngredientsAmount[water] = 10;

            kitchen = new Kitchen(storage);

            bill = new Customer("Bill", water);
            den = new Customer("Den", new Ingredient("Chicken"));
            hall = new Hall(new List<Customer> { bill, den }, recipes);
            hall.Customers.ForEach(c => c.budget = 200);
        }
        [TestMethod]
        public void TestBuySuccessWithoutWant()
        {
            var rnd = new ReproducerRnd(new int[] { 80 });
            accounting = new Accounting(500, 0.5, 0, 0, 0.1, 0.1, 0, ingredientsPrice, rnd);

            command = new Buy(accounting, hall, kitchen, "Buy, Den, Salt water");
            command.IsAllowed = true;
            double expectTip = 12;     //150 * (80 / 100) * 0.1
            double expectPrice = Math.Round(accounting.CalculateFoodMenuPrice(recipes, saltWater), 2);
            double expectTax = Math.Round(accounting.CalculateTransactionTax(expectPrice), 2);

            string expectResult = $"{den.Name}, {den.budget}, Salt water, {expectPrice} -> success; money amount: {Math.Round(expectPrice - expectTax + expectTip, 2)}; tax: {expectTax}; tip {expectTip}";

            command.ExecuteCommand();

            Assert.AreEqual(expectResult, command.Result);
        }

        [TestMethod]
        public void TestBuySuccessWithWantThreeIngredient()
        {
            //4 => 0.04 => GetWanted()          | want 3 ingredients
            //2 => GetRandomIngredient()
            //3 => GetRandomIngredient()
            //2 => GetRandomIngredient()
            //0 => IsTip()
            //80 => 0.8 => GetTipPercent()
            var rnd = new ReproducerRnd(new int[] { 4, 2, 3, 2, 0, 80 });
            accounting = new Accounting(500, 0.5, 0, 0, 0.1, 0.1, 0, ingredientsPrice, rnd);

            command = new Buy(accounting, hall, kitchen, "Buy, Den, Salt water");
            command.IsAllowed = true;
            double expectTip = 50;     //150 * (80 / 100 * 0.1) * 2 * 2 * 2 = 96;   150(price) + 96(tip) = 246(total); 246 > 200(budget); 200 - 150 = 50
            double expectPrice = Math.Round(accounting.CalculateFoodMenuPrice(recipes, saltWater), 2);
            double expectTax = Math.Round(accounting.CalculateTransactionTax(expectPrice), 2);

            string expectResult = $"{den.Name}, {den.budget}, Salt water, {expectPrice} -> success; money amount: {Math.Round(expectPrice - expectTax + expectTip, 2)}; tax: {expectTax}; tip {expectTip}";

            command.ExecuteCommand();

            Assert.AreEqual(expectResult, command.Result);
        }

        [TestMethod]
        public void TestBuySuccessWithWantTwoIngredient()
        {
            //11 => 0.11 =>  GetWanted()        | want 2 ingredients
            //2, GetRandomIngredient()
            //3, GetRandomIngredient()
            //0, IsTip()
            //80 => 0.8 => GetTipPercent()
            var rnd = new ReproducerRnd(new int[] { 11, 2, 3, 0, 80 });
            accounting = new Accounting(500, 0.5, 0, 0, 0.1, 0.1, 0, ingredientsPrice, rnd);

            command = new Buy(accounting, hall, kitchen, "Buy, Den, Salt water");
            command.IsAllowed = true;
            double expectTip = 48;     //150 * (80 / 100 * 0.1) * 2 * 2 
            double expectPrice = Math.Round(accounting.CalculateFoodMenuPrice(recipes, saltWater), 2);
            double expectTax = Math.Round(accounting.CalculateTransactionTax(expectPrice), 2);

            string expectResult = $"{den.Name}, {den.budget}, Salt water, {expectPrice} -> success; money amount: {Math.Round(expectPrice - expectTax + expectTip, 2)}; tax: {expectTax}; tip {expectTip}";

            command.ExecuteCommand();

            Assert.AreEqual(expectResult, command.Result);
        }

        [TestMethod]
        public void TestBuySuccessWithWantOneIngredient()
        {
            //33 => 0.33 =>  GetWanted()        | want 1 ingredients
            //2 => GetRandomIngredient()
            //0 => IsTip()
            //80 => 0.8 => GetTipPercent()
            var rnd = new ReproducerRnd(new int[] { 33, 2, 0, 80 });
            accounting = new Accounting(500, 0.5, 0, 0, 0.1, 0.1, 0, ingredientsPrice, rnd);

            command = new Buy(accounting, hall, kitchen, "Buy, Den, Salt water");
            command.IsAllowed = true;
            double expectTip = 24;     //150 * (80 / 100 * 0.1) * 2
            double expectPrice = Math.Round(accounting.CalculateFoodMenuPrice(recipes, saltWater), 2);
            double expectTax = Math.Round(accounting.CalculateTransactionTax(expectPrice), 2);

            string expectResult = $"{den.Name}, {den.budget}, Salt water, {expectPrice} -> success; money amount: {Math.Round(expectPrice - expectTax + expectTip, 2)}; tax: {expectTax}; tip {expectTip}";

            command.ExecuteCommand();

            Assert.AreEqual(expectResult, command.Result);
        }


    }
}
