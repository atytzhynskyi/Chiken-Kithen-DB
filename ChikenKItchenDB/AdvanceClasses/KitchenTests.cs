using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using BaseClasses;
using AdvanceClasses;

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
            ice = new Food("Ice", water);
            iceWithWater = new Food("Ice with water", new List<Ingredient> { water }, new List<Food> { ice });

            recipeBook = new List<Food> { ice, iceWithWater };

            storage = new Storage(recipeBook, new List<Ingredient> { water });

            storage.IngredientsAmount[water] = 4;

            kitchen = new Kitchen(storage);
        }
            [TestMethod()]
        public void CookTestTrue()
        {
            Assert.IsTrue(kitchen.Cook(iceWithWater), "cook doesnt done");
        }

        [TestMethod()]
        public void CookTestBaseIngredientCount()
        {
            kitchen.Cook(iceWithWater);
            Assert.AreEqual(2, storage.IngredientsAmount[water], "base ingredient count doesn't right");
        }

        [TestMethod()]
        public void CookTestFoodCount()
        {
            Assert.AreEqual(0, kitchen.Storage.FoodAmount[ice], "food as ingredient count doesn't right");
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
            kitchen.Cook(iceWithWater);
            Assert.IsFalse(kitchen.IsEnoughIngredients(iceWithWater));
        }
        [TestMethod()]
        public void isEnoughIngredientsWithFoodTest()
        {
            storage.FoodAmount[ice] = 1;
            storage.IngredientsAmount[water] = 1;

            Assert.IsTrue(kitchen.IsEnoughIngredients(iceWithWater));
        }
    }
}