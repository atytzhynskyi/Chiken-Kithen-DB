﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using BaseClasses;
using System;
using System.Collections.Generic;
using System.Text;
using ChikenKithen;

namespace BaseClasses.Tests
{
    [TestClass()]
    public class CustomerTests
    {
        [TestMethod()]
        public void isAllergicTest()
        {
            Ingredient water = new Ingredient("Water");
            Ingredient salt = new Ingredient("Salt");
            Dictionary<Ingredient, int> dict = new Dictionary<Ingredient, int> { };
            dict.Add(water, 10);
            dict.Add(salt, 10);
            Storage storage = new Storage(new List<Ingredient> { water, salt}, dict);
            Customer customer = new Customer("SugarMan", water);
            Food ice = new Food("Ice", water);
            Food saltWater = new Food("Salt water", salt, water);
            Food saltwaterIce = new Food("Salt water ice", new Ingredient(saltWater.Name), new Ingredient(ice.Name));
            Food saltRock = new Food("Salt rock", salt);
            RecipeBook recipeBook = new RecipeBook(new List<Food> { ice, saltWater, saltwaterIce, saltRock });

            Assert.IsTrue(customer.isAllergic(recipeBook.Recipes, saltwaterIce).Item1);
            Assert.IsTrue(customer.isAllergic(recipeBook.Recipes, saltWater).Item1);
            Assert.IsFalse(customer.isAllergic(recipeBook.Recipes, saltRock).Item1);
        }
    }
}