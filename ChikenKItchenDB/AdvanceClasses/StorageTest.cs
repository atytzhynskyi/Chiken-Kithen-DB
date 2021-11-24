using Microsoft.VisualStudio.TestTools.UnitTesting;
using BaseClasses;
using System;
using System.Collections.Generic;
using System.Text;
using ChikenKithen;
using AdvanceClasses;
using System.Linq;

namespace BaseClasses.Tests
{
    [TestClass()]
    public class StorageTest
    {
        [TestMethod()]
        public void AddIngredientWhenIngredientsAmountIsMoreThenMaxIngredientTypeAndTotalMax()
        {
            // Given
            List<Ingredient> ingredients = new List<Ingredient>
            {
                new Ingredient("sugar"),
                new Ingredient("salt")
            };
            int maxIngredientType = 5;
            int totalMax = 15;



            Storage storage = new Storage(ingredients, maxIngredientType, totalMax);



            // When
            storage.AddIngredient("salt", 50);
            Dictionary<Ingredient, int> actual = storage.IngredientsAmount;



            int[] actualAmountArray = actual.Select(amount => amount.Value).ToArray();
            int actualAmount = actualAmountArray[1];



            // Expected
            int expectedAmount = 5;



            // Then
            Assert.AreEqual(expectedAmount, actualAmount); // storage.IngredientsAmount = -30
        }
    }
}