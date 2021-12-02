using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using BaseClasses;
using AdvanceClasses;
using ChikenKitchenDataBase;
using ChikenKithen;
using CommandsModule;

namespace ChickenKitchenTests
{
    public class TestCustomer
    {
        Food food;
        List<Food> recipes;
        List<Food> recipeFood;
        List<Ingredient> recipeIngredients;

        [SetUp]
        public void SetUp()
        {
            Ingredient water = new Ingredient("water");
            Ingredient flour = new Ingredient("flour");
            Ingredient tomato = new Ingredient("tomato");
            Ingredient mozzarella = new Ingredient("mozzarella");

            Food dough = new Food("dough", new Ingredient[] { flour, water });

            recipeFood = new List<Food> { dough };
            recipeIngredients = new List<Ingredient> { tomato, mozzarella };

            food = new Food("pizza", recipeIngredients, recipeFood);

            recipes = new List<Food> { dough, food };
        }
        
        [Test]
        public void CheckAllergyForUsedIngredient()
        {
            // Given
            string name = "Jane Doe";
            Ingredient[] allergies = { new Ingredient("tomato") };

            Customer customer = new Customer(name, allergies);

            // When
            (bool, Ingredient) actual = customer.isAllergic(recipes, food);

            // Then
            Assert.AreEqual(true, actual.Item1);
            Assert.AreEqual("tomato", actual.Item2.Name);
        }

        [Test]
        public void CheckAllergyForUsedIngredientFromFoods()
        {
            // Given
            string name = "Jane Doe";
            Ingredient[] allergies = { new Ingredient("flour") };

            Customer customer = new Customer(name, allergies);

            // When
            (bool, Ingredient) actual = customer.isAllergic(recipes, food);

            // Then
            Assert.AreEqual(true, actual.Item1);
            Assert.AreEqual("flour", actual.Item2.Name);
        }

        [Test]
        public void CheckAllergyForNonUsedIngredient()
        {
            // Given
            string name = "Jane Doe";
            Ingredient[] allergies = { new Ingredient("tuna") };

            Customer customer = new Customer(name, allergies);

            // When
            (bool, Ingredient) actual = customer.isAllergic(recipes, food);

            // Then
            Assert.AreEqual(false, actual.Item1);
            Assert.AreEqual("NULL", actual.Item2.Name);
        }

        [Test]
        public void CheckAllergyForFoodIsEqualToNull()
        {
            // Given
            string name = "Jane Doe";
            Ingredient[] allergies = { new Ingredient("tuna") };

            Customer customer = new Customer(name, allergies);

            // When
            (bool, Ingredient) actual = customer.isAllergic(recipes, null);

            // Then
            Assert.AreEqual(null, actual);
        }

    }
}