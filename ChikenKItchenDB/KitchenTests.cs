﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chiken_Kithen_DB;
using System;
using System.Collections.Generic;
using System.Text;
using ChikenKItchenDB;

namespace Chiken_Kithen_DB.Tests
{
    [TestClass()]
    public class KitchenTests
    {
        Ingredient water;
        Storage storage;
        Food ice;
        Food iceWithWater;
        RecipeBook recipeBook;
        Kitchen kitchen;
        [TestInitialize]
        public void SetupContext()
        {
            water = new Ingredient("water");
            storage = new Storage(new List<Ingredient> { water });
            storage.IngredientsAmount.Add(water, 3);
            ice = new Food("Ice", water);
            iceWithWater = new Food("Ice with water", water, new Ingredient("Ice"));
            recipeBook = new RecipeBook(new List<Food> { ice, iceWithWater });
            kitchen = new Kitchen(storage, recipeBook);
            kitchen.Cook(iceWithWater);
            
        }
        [TestMethod()]
        public void CookTestFinalFoodCount()
        {
            Assert.AreEqual(1, kitchen.FoodAmount[iceWithWater], "final food count doesn't right");
        }

        [TestMethod()]
        public void CookTestBaseIngredientCount()
        {
            Assert.AreEqual(1, storage.IngredientsAmount[water], "base ingredient count doesn't right");
        }

        [TestMethod()]
        public void CookTestFoodCount()
        {
            Assert.AreEqual(0, kitchen.FoodAmount[ice], "food as ingredient count doesn't right");
        }

        [TestMethod()]
        public void isEnoughIngredientsTest()
        {
            //Assert.IsTrue(isEnoughIngredients(iceWithWater));
        }
    }
}