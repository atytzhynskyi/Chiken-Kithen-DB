using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

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

            Customer customer = new Customer("SugarMan", water);

            Food ice = new Food("Ice", water);
            Food saltWater = new Food("Salt water", salt, water);
            Food saltwaterIce = new Food("Salt water ice", new List<Food>{ saltWater, ice}.ToArray());
            Food saltRock = new Food("Salt rock", salt);

            List<Food> recipeBook = new List<Food> { ice, saltWater, saltwaterIce, saltRock };

            Assert.IsTrue(customer.isAllergic(recipeBook, saltwaterIce).Item1);
            Assert.IsTrue(customer.isAllergic(recipeBook, saltWater).Item1);
            Assert.IsFalse(customer.isAllergic(recipeBook, saltRock).Item1);
        }
    }
}