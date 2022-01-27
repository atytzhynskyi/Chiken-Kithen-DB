﻿using AdvanceClasses;
using BaseClasses;
using CommandsModule;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randomizer;
using System;
using System.Collections.Generic;

namespace ChikenKItchenDB.CommandsModule
{
    [TestClass()]
    public class TableRecommendTest
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
        List<Food> recipes;

        Storage storage;
        Accounting accounting;
        Kitchen kitchen;
        Hall hall;

        Customer bill;
        Customer den;
        Customer ketty;
        Customer tomas;
        Customer elon;

        Dictionary<Ingredient, int> ingredientsPrice;

        Table command;

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

            recipes = new List<Food> { saltWater, saltWaterVip, saltWaterPremium, saltWaterDouble, saltWaterWithPepper };
            storage = new Storage(recipes, ingredients);

            storage.IngredientsAmount[lemon] = 2;
            storage.IngredientsAmount[paprika] = 2;
            storage.IngredientsAmount[salt] = 10;
            storage.IngredientsAmount[water] = 10;
            storage.IngredientsAmount[lime] = 4;
            storage.IngredientsAmount[pepper] = 3;

            kitchen = new Kitchen(storage);

            
            bill = new Customer("Bill", pepper);
            den = new Customer("Den");
            tomas = new Customer("Tomas", lime);
            ketty = new Customer("Ketty", salt);
            elon = new Customer("Elon", pepper, lime);

            hall = new Hall(new List<Customer> { bill, den, tomas, ketty, elon }, recipes);
            hall.Customers.ForEach(c => c.budget = 300);
        }

        [TestMethod]
        public void TestTableRecommendSuccessWithoutWant()
        {
            var rnd = new ReproducerRnd(new int[] { 80 });
            accounting = new Accounting(500, 0.5, 0.2, 0, 0.1, 0.1, 0, ingredientsPrice, rnd);

            hall.Customers.Find(c => c == elon).budget = 400;

            command = new Table(accounting, hall, kitchen, "Table, Den, Bill, Tomas, Ketty, Elon, Salt water with pepper, Recommend, Water, Recommend, Water, Lemon, Salt water double, Recommend, Water");
            command.IsAllowed = true;

            var expectedBudget = 1077.68;               //500 + 216(price) - 108(tax) + 17.28(tip) + 210(price) - 105(tax) + 16.8(tip) + 210(price) - 105(tax) + 16.8(tip) + 360(price) - 180(tax) + 28.8(tip) = 1077.68
            var expectedBudgetOfCustomerDen = 66.72;    //300 - 216(price) - 17.28(tip) = 66.72;            //Salt water with pepper
            var expectedBudgetOfCustomerBill = 73.2;    //300 - 210(price) - 16.8(tip) = 73.2;              //Salt water Vip
            var expectedBudgetOfCustomerTomas = 73.2;   //300 - 210(price) - 16.8(tip) = 73.2;              //Salt water Vip
            var expectedBudgetOfCustomerKetty = 300;    //Allergy
            var expectedBudgetOfCustomerElon = 11.2;    //400 - 360(price) - 28.8(tip) = 11.2;              //Salt water double
            var expectedMoneyAmount = 577.68;           //1077.68 - 500 = 577.68
            var expectResult = $"success; money amount: {expectedMoneyAmount}; tax:";

            command.ExecuteCommand();

            Assert.IsTrue(command.Result.StartsWith(expectResult));
            Assert.AreEqual(expectedBudget, accounting.Budget);
            Assert.AreEqual(expectedBudgetOfCustomerDen, hall.Customers.Find(c => c == den).budget);
            Assert.AreEqual(expectedBudgetOfCustomerBill, hall.Customers.Find(c => c == bill).budget);
            Assert.AreEqual(expectedBudgetOfCustomerTomas, hall.Customers.Find(c => c == tomas).budget);
            Assert.AreEqual(expectedBudgetOfCustomerKetty, hall.Customers.Find(c => c == ketty).budget);
            Assert.AreEqual(expectedBudgetOfCustomerElon, hall.Customers.Find(c => c == elon).budget);
        }

        [TestMethod]
        public void TestTableRecommendAndPooledOnWithWantSuccess()
        {
            //33 => 0.33 =>  GetWanted()        | want 1 ingredients
            //1 => GetRandomIngredient()
            //0 => IsTip()
            //80 => 0.8 => GetTipPercent()

            //11 => 0.11 =>  GetWanted()        | want 2 ingredients
            //1, GetRandomIngredient()
            //0, GetRandomIngredient()
            //0, IsTip()
            //80 => 0.8 => GetTipPercent()

            //33 => 0.33 =>  GetWanted()        | want 1 ingredients
            //1 => GetRandomIngredient()
            //0 => IsTip()
            //80 => 0.8 => GetTipPercent()

            //33 => 0.33 =>  GetWanted()        | want 1 ingredients
            //1 => GetRandomIngredient()
            //0 => IsTip()
            //80 => 0.8 => GetTipPercent()

            var rnd = new ReproducerRnd(new int[] {33, 1, 0, 80, 11, 1, 0, 0, 80, 33, 1, 0, 80, 33, 1, 0, 80 });

            accounting = new Accounting(500, 0.5, 0, 0, 0, 0.4, 0, ingredientsPrice, rnd);

            hall.Customers.Find(c => c == den).budget = 100;
            hall.Customers.Find(c => c == bill).budget = 200;
            hall.Customers.Find(c => c == tomas).budget = 300;
            hall.Customers.Find(c => c == ketty).budget = 50;
            hall.Customers.Find(c => c == elon).budget = 450;

            command = new Table(accounting, hall, kitchen, "Table, Pooled, Den, Bill, Tomas, Ketty, Elon, Salt water with pepper, Recommend, Water, Recommend, Water, Lemon, Salt water double, Recommend, Water");
            command.IsAllowed = true;

            var expectedBudget = 685.38;                    //500 + 15.5 * 3 + 0(allergy) + 79.36(tip) + 19.84(tip) + 39.68(tip) = 685.38
            var expectedBudgetOfCustomerDen = 131.39;       //300 - 36.5(pooled) = 263.5    |   236.5 + 13.5 = 277  | 277    - 79.36(tip) = 197.64  | 263.5 / 277 * 138.12 = 131.39
            var expectedBudgetOfCustomerBill = 6.73;        //50  - 36.5(pooled) = 13.5     |                       | 197.64 - 19.84(tip) = 177.8   | 13.5  / 277 * 138.12 = 6.73
            var expectedBudgetOfCustomerTomas = 0;          //20  + 11(pooled)   = 0        |                       | 177.8  - 39.68(tip) = 138.12  |
            var expectedBudgetOfCustomerKetty = 40;         //40
            var expectedMoneyAmount = 185.38;                 //685.38 - 500 = 185.38;
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
