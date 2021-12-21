using System.Collections.Generic;
using NUnit.Framework;
using BaseClasses;
using AdvanceClasses;

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
        Dictionary<Ingredient, int> ingredientsTrashAmount;
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

            ingredientsTrashAmount = new Dictionary<Ingredient, int>();

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

            double spoilRate = 0;

            storage = new Storage(foodsList, ingredientsList, foodsAmount, ingredientsAmount, ingredientsTrashAmount, 0, maxIngredentType, maxFoodType, totalMax, 0, spoilRate);

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
    }
}
