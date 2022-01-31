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

            var expectedBudget = 1114.88;               //500 + 216(price) - 108(tax) + 17.28(tip) + 282(price) - 141(tax) + 18(tip) + 210(price) - 105(tax) + 16.8(tip) + 360(price) - 180(tax) + 28.8(tip) = 1114.88
            var expectedBudgetOfCustomerDen = 66.72;    //300 - 216(price) - 17.28(tip) = 66.72;            //Salt water with pepper
            var expectedBudgetOfCustomerBill = 0;       //300 - 282(price) - 18(tip) = 0;                   //Salt water premium
            var expectedBudgetOfCustomerTomas = 73.2;   //300 - 210(price) - 16.8(tip) = 73.2;              //Salt water Vip
            var expectedBudgetOfCustomerKetty = 300;    //Allergy
            var expectedBudgetOfCustomerElon = 11.2;    //400 - 360(price) - 28.8(tip) = 11.2;              //Salt water double
            var expectedMoneyAmount = 614.88;           //1114.88 - 500 = 614.88
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
            //55 => 0.55 =>  GetWanted()        | no want // standart tip
            //0 => IsTip()                      | true
            //80 => 0.8 => GetTipPercent()

            //55 => 0.55 =>  GetWanted()        | no want
            //1, IsTip()                        | false

            //33 => 0.33 =>  GetWanted()        | want 1 ingredients
            //0 => GetRandomIngredient()
            //0 => IsTip()
            //80 => 0.8 => GetTipPercent()

            //11 => 0.11 =>  GetWanted()        | want 2 ingredients
            //2 => GetRandomIngredient()
            //3 => GetRandomIngredient()
            //0 => IsTip()
            //80 => 0.8 => GetTipPercent()

            var rnd = new ReproducerRnd(new int[] { 55, 0, 80, 55, 1, 33, 0, 0, 80, 11, 2, 3, 0, 80 });

            accounting = new Accounting(500, 0.5, 0.2, 0, 0.1, 0.1, 0, ingredientsPrice, rnd);

            hall.Customers.Find(c => c == den).budget = 100;
            hall.Customers.Find(c => c == bill).budget = 200;
            hall.Customers.Find(c => c == tomas).budget = 300;
            hall.Customers.Find(c => c == ketty).budget = 75;
            hall.Customers.Find(c => c == elon).budget = 450;

            storage.IngredientsAmount[lemon] = 1;

            command = new Table(accounting, hall, kitchen, "Table, Pooled, Den, Bill, Tomas, Ketty, Elon, Salt water with pepper, Recommend, Water, Recommend, Water, Lemon, Salt water double, Recommend, Water");
            command.IsAllowed = true;

            var expectedBudget = 1067;                  //500 + 216(price) - 108(tax) + 17.28(tip) + 360(price) - 180(tax) + 0(tip) + 210(price) - 105(tax) + 33.6(tip) + 180(price) - 90(tax) + 33.12(tip) = 1067
            var expectedBudgetOfCustomerDen = 0;        //100   | Salt water with pepper(216 price)     | 100 + 116(pooled) = 0     | 84 - 17.28 = 66.72            |
            var expectedBudgetOfCustomerBill = 0;       //200   | Salt water double(360 price)          | 200 + 160(pooled) = 0     | 66.72 - 0(tip) = 66.72        |
            var expectedBudgetOfCustomerTomas = 0;      //300   | Salt water vip(210 price)             | 300 - 90(pooled)  = 0     | 66.72 - 33.6(tip) = 33.12     |
            var expectedBudgetOfCustomerKetty = 75;     //75
            var expectedBudgetOfCustomerElon = 0;       //450   | Salt water(180 price)                 | 450 - 186(price) = 84     |  33.12 - 33.12(tip) = 0       |84  / 84 * 0 = 0
            var expectedMoneyAmount = 567;              //1067 - 500 = 567;
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

    }
}
