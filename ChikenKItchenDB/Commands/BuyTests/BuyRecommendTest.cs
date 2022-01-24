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
        Food saltWaterPremiumDouble;
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
            ingredientsPrice.Add(lemon, 75);
            ingredientsPrice.Add(paprika, 10);
            ingredientsPrice.Add(salt, 50);
            ingredientsPrice.Add(water, 100);
            ingredientsPrice.Add(lime, 150);

            ingredients = new List<Ingredient> { lemon, paprika, salt, water, lime };
            
            saltWater = new Food("Salt water");
            saltWater.RecipeIngredients.Add(water);
            saltWater.RecipeIngredients.Add(salt);

            //allergy
            saltWaterWithPepper = new Food("Salt water with pepper");
            saltWaterWithPepper.RecipeIngredients.Add(water);
            saltWaterWithPepper.RecipeIngredients.Add(salt);
            saltWaterWithPepper.RecipeIngredients.Add(pepper);

            saltWaterVip = new Food("Salt water vip");
            saltWaterVip.RecipeIngredients.Add(lemon);
            saltWaterVip.RecipeFoods.Add(saltWater);

            saltWaterPremium = new Food("Salt water premium");
            saltWaterPremium.RecipeIngredients.Add(lime);
            saltWaterPremium.RecipeFoods.Add(saltWaterVip);

            //storage has not enought ingredient
            saltWaterPremiumDouble = new Food("Salt water premium double");
            saltWaterPremium.RecipeFoods.Add(saltWaterPremium);
            saltWaterPremium.RecipeFoods.Add(saltWaterPremium);

            Recipes = new List<Food> { saltWater, saltWaterVip, saltWaterPremium, saltWaterPremiumDouble, saltWaterWithPepper };
            storage = new Storage(Recipes, ingredients);

            storage.IngredientsAmount[lemon] = 1;
            storage.IngredientsAmount[paprika] = 1;
            storage.IngredientsAmount[salt] = 10;
            storage.IngredientsAmount[salt] = 10;
            storage.IngredientsAmount[water] = 10;
            storage.IngredientsAmount[lime] = 5;

            kitchen = new Kitchen(storage);

            bill = new Customer("Bill", pepper);
            den = new Customer("Den", new Ingredient("Chicken"));
            hall = new Hall(new List<Customer> { bill, den }, Recipes);
            hall.Customers.ForEach(c => c.budget = 200);
        }
        [TestMethod]
        public void TestBuyRecommendSuccessWithoutWant()
        {
            var rnd = new ReproducerRnd(new int[] { 80 });
            accounting = new Accounting(500, 0.5, 0, 0, 0.1, 0.1, 0, ingredientsPrice, rnd);

            command = new Buy(accounting, hall, kitchen, "Buy, Den, Recommend, Water");
            command.IsAllowed = true;
            //var expectedBudgetOfCustomerDen = 19;       //50 - 31;
            double expectTip = 12;     //150 * (80 / 100) * 0.1
            double expectPrice = Math.Round(accounting.CalculateFoodMenuPrice(Recipes, saltWater), 2);
            double expectTax = Math.Round(accounting.CalculateTransactionTax(expectPrice), 2);

            string expectResult = $"{den.Name}, {den.budget}, Salt water, {expectPrice} -> success; money amount: {Math.Round(expectPrice - expectTax + expectTip, 2)}; tax: {expectTax}; tip {expectTip}";

            //Result = $"{Customer.Name}, {Math.Round(Customer.budget + price + tip, 2)}, {Customer.Order.Name}, {price} -> success; money amount: {Math.Round(price - tax + tip, 2)}; tax: {tax}; tip {tip}";

            command.ExecuteCommand();

            Assert.AreEqual(expectResult, command.Result);
            //Assert.AreEqual(expectedBudgetOfCustomerDen, hall.Customers.Find(c => c == den).budget);
        }

        [TestMethod]
        public void TestBuyRecommendSuccessWithWantThreeIngredient()
        {
            //4 => 0.04 => GetWanted()          | want 3 ingredients
            //2 => GetRandomIngredient()
            //3 => GetRandomIngredient()
            //2 => GetRandomIngredient()
            //0 => IsTip()
            //80 => 0.8 => GetTipPercent()
            var rnd = new ReproducerRnd(new int[] { 4, 2, 3, 2, 0, 80 });
            accounting = new Accounting(500, 0.5, 0, 0, 0.1, 0.1, 0, ingredientsPrice, rnd);

            command = new Buy(accounting, hall, kitchen, "Buy, Den, Recommend, Water");
            command.IsAllowed = true;
            double expectTip = 50;     //150 * (80 / 100 * 0.1) * 2 * 2 * 2 = 96;   150(price) + 96(tip) = 246(total); 246 > 200(budget); 200 - 150 = 50
            double expectPrice = Math.Round(accounting.CalculateFoodMenuPrice(Recipes, saltWater), 2);
            double expectTax = Math.Round(accounting.CalculateTransactionTax(expectPrice), 2);

            string expectResult = $"{den.Name}, {den.budget}, Salt water, {expectPrice} -> success; money amount: {Math.Round(expectPrice - expectTax + expectTip, 2)}; tax: {expectTax}; tip {expectTip}";

            command.ExecuteCommand();

            Assert.AreEqual(expectResult, command.Result);
        }



    }
}
