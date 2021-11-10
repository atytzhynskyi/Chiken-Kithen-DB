using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChikenKithen;
using System;
using System.Collections.Generic;
using System.Text;
using ChikenKItchenDB;
using BaseClasses;
using ChikenKithen;

namespace ChikenKithen.Tests
{
    [TestClass()]
    public class KitchenTests
    {
        Ingredient water;
        Storage storage;
        Food ice;
        Food iceWithWater;
        List<Food> recipeBook;
        Kitchen kitchen;
        [TestInitialize]
        public void SetupContext()
        {
            water = new Ingredient("water");
            storage = new Storage(new List<Ingredient> { water });
            storage.IngredientsAmount.Add(water, 4);
            ice = new Food("Ice", water);
            iceWithWater = new Food("Ice with water", new List<Ingredient> { water }, new List<Food> { ice });
            recipeBook = new List<Food>(new List<Food> { ice, iceWithWater });
            kitchen = new Kitchen(storage, recipeBook, 500);
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
            Assert.AreEqual(2, storage.IngredientsAmount[water], "base ingredient count doesn't right");
        }

        [TestMethod()]
        public void CookTestFoodCount()
        {
            Assert.AreEqual(0, kitchen.FoodAmount[ice], "food as ingredient count doesn't right");
        }

        [TestMethod()]
        public void isEnoughIngredientsTestTrue()
        {
            Assert.IsTrue(kitchen.IsEnoughIngredients(iceWithWater));
        }

        [TestMethod()]
        public void isEnoughIngredientsTestFalse()
        {
            kitchen.Cook(iceWithWater);
            Assert.IsFalse(kitchen.IsEnoughIngredients(iceWithWater));
        }
    }
}