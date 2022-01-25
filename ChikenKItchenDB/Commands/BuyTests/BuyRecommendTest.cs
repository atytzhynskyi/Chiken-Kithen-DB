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
    public class BuyRecommendTest
    {
        Ingredient lemon = new Ingredient("Lemon");
        Ingredient paprika = new Ingredient("Paprika");
        Ingredient salt = new Ingredient("Salt");
        Ingredient water = new Ingredient("Water");
        Ingredient lime = new Ingredient("Lime");
        Ingredient pepper = new Ingredient("Pepper");

        Food saltWater;
        Food saltWaterVip;
        Food saltWaterPremium;
        Food saltWaterDouble;
        Food saltWaterWithPepper;

        List<Ingredient> ingredients;
        List<Food> Recipes;
        Storage storage;
        Accounting accounting;
        Kitchen kitchen;
        Customer bill;
        Customer den;
        Hall hall;
        Dictionary<Ingredient, int> ingredientsPrice;

        Buy command;
        [TestInitialize]
        public void SetupContext()
        {
            ingredientsPrice = new Dictionary<Ingredient, int>();
            ingredientsPrice.Add(lemon, 25);
            ingredientsPrice.Add(paprika, 10);
            ingredientsPrice.Add(salt, 50);
            ingredientsPrice.Add(water, 100);
            ingredientsPrice.Add(lime, 30);
            ingredientsPrice.Add(pepper, 30);

            ingredients = new List<Ingredient> { lemon, paprika, salt, water, lime, pepper };

            saltWater = new Food("Salt water");
            saltWater.RecipeIngredients.Add(water);
            saltWater.RecipeIngredients.Add(salt);

            saltWaterWithPepper = new Food("Salt water with pepper");
            saltWaterWithPepper.RecipeIngredients.Add(water);
            saltWaterWithPepper.RecipeIngredients.Add(salt);
            saltWaterWithPepper.RecipeIngredients.Add(pepper);

            saltWaterVip = new Food("Salt water vip");
            saltWaterVip.RecipeIngredients.Add(lemon);
            saltWaterVip.RecipeFoods.Add(saltWater);

            saltWaterPremium = new Food("Salt water premium");
            saltWaterPremium.RecipeIngredients.Add(lime);
            saltWaterPremium.RecipeIngredients.Add(lime);
            saltWaterPremium.RecipeFoods.Add(saltWaterVip);

            saltWaterDouble = new Food("Salt water double");
            saltWaterDouble.RecipeFoods.Add(saltWater);
            saltWaterDouble.RecipeFoods.Add(saltWater);

            Recipes = new List<Food> { saltWater, saltWaterVip, saltWaterPremium, saltWaterDouble, saltWaterWithPepper };
            storage = new Storage(Recipes, ingredients);

            storage.IngredientsAmount[lemon] = 4;
            storage.IngredientsAmount[paprika] = 2;
            storage.IngredientsAmount[salt] = 10;
            storage.IngredientsAmount[water] = 10;
            storage.IngredientsAmount[lime] = 1;
            storage.IngredientsAmount[pepper] = 3;

            kitchen = new Kitchen(storage);

            bill = new Customer("Bill", pepper);
            den = new Customer("Den", new Ingredient("Chicken"));
            hall = new Hall(new List<Customer> { bill, den }, Recipes);
            hall.Customers.ForEach(c => c.budget = 300);
        }
        [TestMethod]
        public void TestBuyRecommendSuccessWithoutWant()
        {
            var rnd = new ReproducerRnd(new int[] { 80 });
            accounting = new Accounting(500, 0.5, 0.2, 0, 0.1, 0.1, 0, ingredientsPrice, rnd);

            command = new Buy(accounting, hall, kitchen, "Buy, Bill, Recommend, Water");
            command.IsAllowed = true;

            double expectTip = 16.8;                        //210 * (80 / 100) * 0.1 = 16.8
            double expectPrice = 210;                       //175 + 30(margin tax (0.1)) = 210
            double expectTax = 105;                         //210 * 0.5 = 110
            var expectedBudget = 621.8;                     //500 + 210(price) - 105(tax) + 16.8(tip) = 621.8
            var expectedBudgetOfCustomerBill = 73.2;        //300 - 210(price) - 16.8 = 73.2

            string expectResult = $"{bill.Name}, {bill.budget}, Salt water vip, {expectPrice} -> success; money amount: {Math.Round(expectPrice - expectTax + expectTip, 2)}; tax: {expectTax}; tip {expectTip}";

            command.ExecuteCommand();

            Assert.AreEqual(expectResult, command.Result);
            Assert.AreEqual(expectedBudget, accounting.Budget);
            Assert.AreEqual(expectedBudgetOfCustomerBill, hall.Customers.Find(c => c == bill).budget);
        }

        [TestMethod]
        public void TestBuyRecommendSuccessWithWantTwoIngredient()
        {
            //11 => 0.11 => GetWanted()          | want 2 ingredients
            //2 => GetRandomIngredient()
            //3 => GetRandomIngredient()
            //0 => IsTip()
            //80 => 0.8 => GetTipPercent()
            var rnd = new ReproducerRnd(new int[] { 11, 2, 3, 0, 80 });
            accounting = new Accounting(500, 0.5, 0.2, 0, 0.1, 0.1, 0, ingredientsPrice, rnd);

            command = new Buy(accounting, hall, kitchen, "Buy, Bill, Recommend, Water");
            command.IsAllowed = true;

            double expectTip = 67.2;                        //210 * (80 / 100) * 0.1 * 4(want) = 67.2
            double expectPrice = 210;                       //175 + 30(margin tax (0.1)) = 210
            double expectTax = 105;                         //210 * 0.5 = 110
            var expectedBudget = 672.2;                     //500 + 210(price) - 110(tax) + 67.2(tip) = 672.2
            var expectedBudgetOfCustomerBill = 22.8;        //300 - 210(price) - 67.2 = 22.8

            string expectResult = $"{bill.Name}, {bill.budget}, Salt water vip, {expectPrice} -> success; money amount: {Math.Round(expectPrice - expectTax + expectTip, 2)}; tax: {expectTax}; tip {expectTip}";

            command.ExecuteCommand();

            Assert.AreEqual(expectResult, command.Result);
            Assert.AreEqual(expectedBudget, accounting.Budget);
            Assert.AreEqual(expectedBudgetOfCustomerBill, hall.Customers.Find(c => c == bill).budget);
        }

    }
}
