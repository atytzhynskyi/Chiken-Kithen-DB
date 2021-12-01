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
    public class TestKitchen
    {
        Ingredient coffee;
        Ingredient milk;
        Ingredient water;
        Ingredient sugar;
        List<Ingredient> ingredientsList;
        Dictionary<Ingredient, int> ingredientsAmount;
        Dictionary<Ingredient, int> ingredientsPrice;
        Food americano;
        Ingredient[] americanoIngredients;
        Food cappuccino;
        List<Ingredient> cappuccinoIngredients;
        List<Food> cappuccinoFoods;
        List<Food> foodsList;
        Dictionary<Food, int> foodsAmount;
        Storage storage;
        Kitchen kitchen;


        [SetUp]
        public void SetUp()
        {
            coffee = new Ingredient("coffee");
            milk = new Ingredient("milk");
            water = new Ingredient("water");
            sugar = new Ingredient("sugar");

            ingredientsList = new List<Ingredient> { coffee, milk, water, sugar };

            ingredientsAmount = new Dictionary<Ingredient, int>
            {
                { coffee, 2 },
                { milk, 2 },
                { water, 3 },
                { sugar, 1 }
            };

            ingredientsPrice = new Dictionary<Ingredient, int>
            {
                { coffee, 20 },
                { milk, 10 },
                { water, 5 },
                { sugar, 8 }
            };

            americanoIngredients = new Ingredient[] { coffee, water };
            americano = new Food("americano", americanoIngredients);

            cappuccinoIngredients = new List<Ingredient> { coffee, milk, sugar };
            cappuccinoFoods = new List<Food> { americano };
            cappuccino = new Food("cappuccino");

            foodsList = new List<Food> { cappuccino, americano };
            foodsAmount = new Dictionary<Food, int>
            {
                { cappuccino, 2 },
                { americano, 2 }
            };

            int maxIngredentType = 10;
            int maxFoodType = 5;
            int totalMax = 20;

            storage = new Storage(foodsList, ingredientsList, foodsAmount, ingredientsAmount, ingredientsPrice, maxIngredentType, maxFoodType, totalMax);

            kitchen = new Kitchen(storage);
        }
        
        [Test]
        public void GetIngredientsAmountIfIngredientsIsEnough()                                // public bool IsEnoughIngredients(Food food)
        {
            // Given
            cappuccino.RecipeIngredients = cappuccinoIngredients;

            // When
            bool actual = kitchen.IsEnoughIngredients(cappuccino);

            // Then
            Assert.IsTrue(actual);
        }

        [Test]
        public void GetIngredientsAmountIsEqualToStorageIngredients()                         // public bool IsEnoughIngredients(Food food)
        {
            // Given
            cappuccino.RecipeIngredients = new List<Ingredient> { coffee, milk, milk, sugar };

            // When
            bool actual = kitchen.IsEnoughIngredients(cappuccino);

            // Then
            Assert.IsTrue(actual);
        }

        [Test]
        public void GetIngredientsAmountIfIngredientsIsNotEnough()
        {
            // Given
            cappuccino.RecipeIngredients = new List<Ingredient> { coffee, coffee, coffee, milk, sugar};

            // When
            bool actual = kitchen.IsEnoughIngredients(cappuccino);

            // Then
            Assert.IsFalse(actual);
        }                         // public bool IsEnoughIngredients(Food food)

        [Test]
        public void GetIngredientsAmountIfIFoodsIsEnough()
        {
            // Given
            cappuccino.RecipeFoods = cappuccinoFoods;

            // When
            bool actual = kitchen.IsEnoughIngredients(cappuccino);

            // Then
            Assert.IsTrue(actual);
        }                                 // public bool IsEnoughIngredients(Food food)

        [Test]
        public void GetIngredientsAmountIfIFoodsIsNotEnough()                                 // public bool IsEnoughIngredients(Food food)
        {
            // Given
            cappuccino.RecipeFoods = new List<Food> { americano, americano, americano, americano, americano };

            // When
            bool actual = kitchen.IsEnoughIngredients(cappuccino);

            // Then
            Assert.IsFalse(actual, "test string");
        }

        [Test]
        public void GetListOfIngredientsForFoodThatConsistOfIngredientsOnly()                 // public List<Ingredient> GetBaseIngredientRecipe(Food food)
        {
            // Given
            cappuccino.RecipeIngredients = cappuccinoIngredients;

            // When
            List<Ingredient> actual = kitchen.GetBaseIngredientRecipe(cappuccino);

            // Expected
            List<Ingredient> expected = new List<Ingredient> { coffee, milk, sugar };

            // Then
            string[] actualIngredients = actual.Select(i => i.Name).ToArray();
            string[] expectedIngredients = expected.Select(i => i.Name).ToArray();

            CollectionAssert.AreEqual(expectedIngredients, actualIngredients);
        }

        [Test]
        public void GetListOfIngredientsForFoodThatConsistOfFoodsOnly()                       // public List<Ingredient> GetBaseIngredientRecipe(Food food)
        {
            // Given
            cappuccino.RecipeFoods = cappuccinoFoods;

            // When
            List<Ingredient> actual = kitchen.GetBaseIngredientRecipe(cappuccino);

            // Expected
            List<Ingredient> expected = new List<Ingredient> { coffee, water };

            // Then
            string[] actualIngredients = actual.Select(i => i.Name).ToArray();
            string[] expectedIngredients = expected.Select(i => i.Name).ToArray();

            CollectionAssert.AreEqual(expectedIngredients, actualIngredients);
        }

        [Test]
        public void GetListOfIngredientsForFoodThatConsistOfIngredientsAndFoodsWithIdenticalIngredients() // public List<Ingredient> GetBaseIngredientRecipe(Food food)
        {
            // Given
            cappuccino.RecipeIngredients = cappuccinoIngredients;
            cappuccino.RecipeFoods = cappuccinoFoods;

            // When
            List<Ingredient> actual = kitchen.GetBaseIngredientRecipe(cappuccino);

            // Expected
            List<Ingredient> expected = new List<Ingredient> { coffee,  milk, sugar, water };

            // Then
            string[] actualIngredients = actual.Select(i => i.Name).ToArray();
            string[] expectedIngredients = expected.Select(i => i.Name).ToArray();

            CollectionAssert.AreEqual(expectedIngredients, actualIngredients);               // identical ingredients do not skip
        }

        [Test]
        public void GetListOfIngredientsForFoodWithEmptyStringWithoutName()                  // public List<Ingredient> GetBaseIngredientRecipe(Food food)
        {
            // Given
            cappuccino.Name = ("");
            cappuccino.RecipeIngredients = cappuccinoIngredients;

            // When
            List<Ingredient> actual = kitchen.GetBaseIngredientRecipe(cappuccino);           // уточнити в Толіка як має оброблятися name = пуста стрічка

            // Expected
            List<Ingredient> expected = new List<Ingredient> { coffee, milk, sugar };

            // Then
            string[] actualIngredients = actual.Select(i => i.Name).ToArray();
            string[] expectedIngredients = expected.Select(i => i.Name).ToArray();

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void GetListOfIngredientsForNull()                                            // public List<Ingredient> GetBaseIngredientRecipe(Food food)
        {
            // Given
            cappuccino = null;

            // When
            List<Ingredient> actual = kitchen.GetBaseIngredientRecipe(cappuccino);           // уточнити в Толіка як має оброблятися null, зараз NullReferenceException

            // Expected
            List<Ingredient> expected = null;

            // Then
            string[] actualIngredients = actual.Select(i => i.Name).ToArray();
            string[] expectedIngredients = expected.Select(i => i.Name).ToArray();

            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
