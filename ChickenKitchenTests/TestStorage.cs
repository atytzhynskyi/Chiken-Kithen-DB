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
    public class TestStorage
    {
        [Test]
        public void AddIngredientWhenIngredientsAmountIsMoreThenTotalMax()        // public void AddIngredient(string ingredientName, int amount)
        {
            // Given
            List<Ingredient> ingredients = new List<Ingredient>
            {
                new Ingredient("sugar"),
                new Ingredient("salt")
            };
            int maxIngredientType = 100;
            int totalMax = 8;

            Storage storage = new Storage(ingredients, maxIngredientType, totalMax);

            // When
            storage.AddIngredient("salt", 10);                                   // GetTotalAmount() = 0
            Dictionary<Ingredient, int> actual = storage.IngredientsAmount;

            int[] actualAmountArray = actual.Select(amount => amount.Value).ToArray();
            int actualAmount = actualAmountArray[1];

            // Expected
            int expectedAmount = 8;

            // Then
            Assert.AreEqual(expectedAmount, actualAmount);                       // Console.WriteLine() check
        }

        [Test]
        public void AddIngredientWhenIngredientsAmountIsLessThenTotalMax()        // public void AddIngredient(string ingredientName, int amount)
        {
            // Given
            List<Ingredient> ingredients = new List<Ingredient>
            {
                new Ingredient("sugar"),
                new Ingredient("salt")
            };
            int maxIngredientType = 100;
            int totalMax = 10;

            Storage storage = new Storage(ingredients, maxIngredientType, totalMax);

            // When
            storage.AddIngredient("salt", 6);                                   // GetTotalAmount() = 0
            Dictionary<Ingredient, int> actual = storage.IngredientsAmount;

            int[] actualAmountArray = actual.Select(amount => amount.Value).ToArray();
            int actualAmount = actualAmountArray[1];

            // Expected
            int expectedAmount = 6;

            // Then
            Assert.AreEqual(expectedAmount, actualAmount);
        }

        [Test]
        public void AddIngredientWhenIngredientsAmountIsMoreThenMaxIngredientType()      // public void AddIngredient(string ingredientName, int amount)
        {
            // Given
            List<Ingredient> ingredients = new List<Ingredient>
            {
                new Ingredient("sugar"),
                new Ingredient("salt")
            };
            int maxIngredientType = 8;
            int totalMax = 100;

            Storage storage = new Storage(ingredients, maxIngredientType, totalMax);

            // When
            storage.AddIngredient("salt", 10);                                   // GetTotalAmount() = 0
            Dictionary<Ingredient, int> actual = storage.IngredientsAmount;

            int[] actualAmountArray = actual.Select(amount => amount.Value).ToArray();
            int actualAmount = actualAmountArray[1];

            // Expected
            int expectedAmount = 8;

            // Then
            Assert.AreEqual(expectedAmount, actualAmount);                        // Console.WriteLine() check
        }

        [Test]
        public void AddIngredientWhenIngredientsAmountIsLessThenMaxIngredientType()      // public void AddIngredient(string ingredientName, int amount)
        {
            // Given
            List<Ingredient> ingredients = new List<Ingredient>
            {
                new Ingredient("sugar"),
                new Ingredient("salt")
            };
            int maxIngredientType = 10;
            int totalMax = 100;

            Storage storage = new Storage(ingredients, maxIngredientType, totalMax);

            // When
            storage.AddIngredient("salt", 8);                                    // GetTotalAmount() = 0
            Dictionary<Ingredient, int> actual = storage.IngredientsAmount;

            int[] actualAmountArray = actual.Select(amount => amount.Value).ToArray();
            int actualAmount = actualAmountArray[1];

            // Expected
            int expectedAmount = 8;

            // Then
            Assert.AreEqual(expectedAmount, actualAmount);
        }

        [Test]
        public void AddIngredientWhenIngredientsAmountIsMoreThenMaxIngredientTypeAndTotalMax()      // public void AddIngredient(string ingredientName, int amount)
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
            storage.AddIngredient("salt", 50);                                    // GetTotalAmount() = 0
            Dictionary<Ingredient, int> actual = storage.IngredientsAmount;

            int[] actualAmountArray = actual.Select(amount => amount.Value).ToArray();
            int actualAmount = actualAmountArray[1];

            // Expected
            int expectedAmount = 5;

            // Then
            Assert.AreEqual(expectedAmount, actualAmount);                       // storage.IngredientsAmount = -30
        }

        [Test]
        public void GetTotalAmountOfIngredients()                                 // private int GetTotalAmount()
        {
            // Given
            Ingredient sugar = new Ingredient("sugar");
            Ingredient salt = new Ingredient("salt");

            List<Ingredient> ingredients = new List<Ingredient> { sugar, salt };

            Dictionary<Ingredient, int> ingredientsAmount = new Dictionary<Ingredient, int>
            {
                {sugar, 10},
                {salt, 20 }
            };

            int maxIngredientType = 100;
            int totalMax = 100;

            Storage storage = new Storage(ingredients, ingredientsAmount, maxIngredientType, totalMax);

            // When
            storage.AddIngredient("salt", 0);                                    // how to check all ingredients value amount?
            Dictionary<Ingredient, int> actual = storage.IngredientsAmount;

            int[] actualAmountArray = actual.Select(amount => amount.Value).ToArray();
            int actualAmount = actualAmountArray[1];

            // Expected
            int expectedAmount = 20;

            // Then
            Assert.AreEqual(expectedAmount, actualAmount);
        }

        [Test]
        public void AddFoodWhenIngredientsAmountIsMoreThenTotalMax()              // public void AddFood(string foodName, int amount)
        {
            // Given
            List<Food> foods = new List<Food>
            {
                new Food("candy"),
                new Food("cake")
            };
            
            List<Ingredient> ingredients = new List<Ingredient>
            {
                new Ingredient("sugar"),
                new Ingredient("salt")
            };
            int maxIngredientType = 100;
            int maxFoodType = 100;
            int totalMax = 8;

            Storage storage = new Storage(foods, ingredients, maxIngredientType, maxFoodType, totalMax);

            // When
            storage.AddFood("cake", 10);                                        // GetTotalAmount() = 0
            Dictionary<Food, int> actual = storage.FoodAmount;

            int[] actualAmountArray = actual.Select(amount => amount.Value).ToArray();
            int actualAmount = actualAmountArray[1];

            // Expected
            int expectedAmount = 8;

            // Then
            Assert.AreEqual(expectedAmount, actualAmount);                       // Console.WriteLine() check
        }

        [Test]
        public void AddFoodWhenIngredientsAmountIsLessThenTotalMax()              // public void AddFood(string foodName, int amount)
        {
            // Given
            List<Food> foods = new List<Food>
            {
                new Food("candy"),
                new Food("cake")
            };

            List<Ingredient> ingredients = new List<Ingredient>
            {
                new Ingredient("sugar"),
                new Ingredient("salt")
            };
            int maxIngredientType = 100;
            int maxFoodType = 100;
            int totalMax = 10;

            Storage storage = new Storage(foods, ingredients, maxIngredientType, maxFoodType, totalMax);

            // When
            storage.AddFood("cake", 8);                                         // GetTotalAmount() = 0
            Dictionary<Food, int> actual = storage.FoodAmount;

            int[] actualAmountArray = actual.Select(amount => amount.Value).ToArray();
            int actualAmount = actualAmountArray[1];

            // Expected
            int expectedAmount = 8;

            // Then
            Assert.AreEqual(expectedAmount, actualAmount);                       // Console.WriteLine() check
        }

        [Test]
        public void AddFoodtWhenIngredientsAmountIsMoreThenMaxIngredientType()    // public void AddFood(string foodName, int amount)
        {
            // Given
            List<Food> foods = new List<Food>
            {
                new Food("candy"),
                new Food("cake")
            };

            List<Ingredient> ingredients = new List<Ingredient>
            {
                new Ingredient("sugar"),
                new Ingredient("salt")
            };
            int maxIngredientType = 100;
            int maxFoodType = 8;
            int totalMax = 100;

            Storage storage = new Storage(foods, ingredients, maxIngredientType, maxFoodType, totalMax);

            // When
            storage.AddFood("cake", 10);                                         // GetTotalAmount() = 0
            Dictionary<Food, int> actual = storage.FoodAmount;

            int[] actualAmountArray = actual.Select(amount => amount.Value).ToArray();
            int actualAmount = actualAmountArray[1];

            // Expected
            int expectedAmount = 8;

            // Then
            Assert.AreEqual(expectedAmount, actualAmount);                       // Console.WriteLine() check
        }

        [Test]
        public void AddFoodWhenIngredientsAmountIsLessThenMaxIngredientType()     // public void AddFood(string foodName, int amount)
        {
            // Given
            List<Food> foods = new List<Food>
            {
                new Food("candy"),
                new Food("cake")
            };

            List<Ingredient> ingredients = new List<Ingredient>
            {
                new Ingredient("sugar"),
                new Ingredient("salt")
            };
            int maxIngredientType = 100;
            int maxFoodType = 10;
            int totalMax = 100;

            Storage storage = new Storage(foods, ingredients, maxIngredientType, maxFoodType, totalMax);

            // When
            storage.AddFood("cake", 8);                                         // GetTotalAmount() = 0
            Dictionary<Food, int> actual = storage.FoodAmount;

            int[] actualAmountArray = actual.Select(amount => amount.Value).ToArray();
            int actualAmount = actualAmountArray[1];

            // Expected
            int expectedAmount = 8;

            // Then
            Assert.AreEqual(expectedAmount, actualAmount);                       // Console.WriteLine() check
        }

        [Test]
        public void AddFoodWhenIngredientsAmountIsMoreThenMaxIngredientTypeAndTotalMax()            // public void AddFood(string foodName, int amount)
        {
            // Given
            List<Food> foods = new List<Food>
            {
                new Food("candy"),
                new Food("cake")
            };

            List<Ingredient> ingredients = new List<Ingredient>
            {
                new Ingredient("sugar"),
                new Ingredient("salt")
            };
            int maxIngredientType = 100;
            int maxFoodType = 10;
            int totalMax = 20;

            Storage storage = new Storage(foods, ingredients, maxIngredientType, maxFoodType, totalMax);

            // When
            storage.AddFood("cake", 50);                                         // GetTotalAmount() = 0
            Dictionary<Food, int> actual = storage.FoodAmount;

            int[] actualAmountArray = actual.Select(amount => amount.Value).ToArray();
            int actualAmount = actualAmountArray[1];

            // Expected
            int expectedAmount = 10;

            // Then
            Assert.AreEqual(expectedAmount, actualAmount);                       // actualAmount = -20
        }

        [Test]
        public void GetExistingRecipeByName()                                     // public Food GetRecipeByName(string Name)
        {
            // Given
            Ingredient coffee = new Ingredient("coffee");
            Ingredient milk = new Ingredient("milk");

            List<Ingredient> ingredients = new List<Ingredient> { coffee, milk };

            Storage storage = new Storage(ingredients);

            Ingredient[] recipeIngredients = { coffee, milk };

            storage.Recipes = new List<Food>
            {
                new Food("cappuccino", recipeIngredients)
            };

            // When
            Food actual = storage.GetRecipeByName("cappuccino");

            string actualName = actual.Name;

            // Expected
            string expectedName = "cappuccino";

            // Then
            Assert.AreEqual(expectedName, actualName);
        }

        [Test]
        public void GetNonExistingRecipeByName()                                  // public Food GetRecipeByName(string Name)
        {
            // Given
            Ingredient coffee = new Ingredient("coffee");
            Ingredient milk = new Ingredient("milk");

            List<Ingredient> ingredients = new List<Ingredient> { coffee, milk };

            Storage storage = new Storage(ingredients);

            Ingredient[] recipeIngredients = { coffee, milk };

            storage.Recipes = new List<Food>
            {
                new Food("cappuccino", recipeIngredients)
            };

            // When
            Food actual = storage.GetRecipeByName("coffe");

            // Expected
            Food expected = null;

            // Then
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetRecipeForNull()                                           // public Food GetRecipeByName(string Name)
        {
            // Given
            Ingredient coffee = new Ingredient("coffee");
            Ingredient milk = new Ingredient("milk");

            List<Ingredient> ingredients = new List<Ingredient> { coffee, milk };

            Storage storage = new Storage(ingredients);

            Ingredient[] recipeIngredients = { coffee, milk };

            storage.Recipes = new List<Food>
            {
                new Food("cappuccino", recipeIngredients)
            };

            // When
            Food actual = storage.GetRecipeByName(null);

            // Expected
            Food expected = null;

            // Then
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetRecipeForNameIsEmptyString()                               // public Food GetRecipeByName(string Name)
        {
            // Given
            Ingredient coffee = new Ingredient("coffee");
            Ingredient milk = new Ingredient("milk");

            List<Ingredient> ingredients = new List<Ingredient> { coffee, milk };

            Storage storage = new Storage(ingredients);

            Ingredient[] recipeIngredients = { coffee, milk };

            storage.Recipes = new List<Food>
            {
                new Food("cappuccino", recipeIngredients)
            };

            // When
            Food actual = storage.GetRecipeByName("");

            // Expected
            Food expected = null;

            // Then
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetNameOfExistingIngredient()                                 // public Ingredient GetIngredient(string name)
        {
            // Given
            List<Ingredient> ingredients = new List<Ingredient> {
                new Ingredient("salt"),
                new Ingredient("pepper"),
                new Ingredient("sugar")
            };

            Storage storage = new Storage(ingredients);

            // When
            Ingredient actual = storage.GetIngredient("pepper");

            string actualName = actual.Name;

            // Expected
            string expectedName = "pepper";

            // Then
            Assert.AreEqual(expectedName, actualName);
        }

        [Test]
        public void GetNameOfNonExistingIngredient()                              // public Ingredient GetIngredient(string name)
        {
            // Given
            List<Ingredient> ingredients = new List<Ingredient> {
                new Ingredient("salt"),
                new Ingredient("pepper"),
                new Ingredient("sugar")
            };

            Storage storage = new Storage(ingredients);

            // When
            Ingredient actual = storage.GetIngredient("oregano");

            // Expected
            Ingredient expected = null;

            // Then
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AddNewIngredientName()                                        // public void AddNewIngredient(Ingredient ingredient, int amount)
        {
            // Given
            List<Ingredient> ingredients = new List<Ingredient>
            {
                new Ingredient("sugar"),
                new Ingredient("salt")
            };

            Storage storage = new Storage(ingredients);                           // чи правильно обраний конструктор

            // When
            Ingredient newIngredient = new Ingredient("milk");
            int newAmount = 0;

            storage.AddNewIngredient(newIngredient, newAmount);

            List<Ingredient> actual = storage.Ingredients;

            // Expected
            List<Ingredient> expected = new List<Ingredient>
            {
                new Ingredient("sugar"),
                new Ingredient("salt"),
                new Ingredient("milk"),
            };

            string[] actualNames = actual.Select(i => i.Name).ToArray();
            string[] expectedNames = expected.Select(i => i.Name).ToArray();

            // Then
            CollectionAssert.AreEqual(expectedNames, actualNames);
        }

        [Test]
        public void AddNewIngredientAmount()                                      // public void AddNewIngredient(Ingredient ingredient, int amount)
        {
            // Given
            List<Ingredient> ingredient = new List<Ingredient>
            {
                new Ingredient("sugar"),
                new Ingredient("salt")
            };

            Storage storage = new Storage(ingredient);                            // how to choose constructor

            storage.IngredientsAmount = new Dictionary<Ingredient, int> 
            { 
                { new Ingredient("sugar"), 10 },
                { new Ingredient("salt"), 20 } 
            };

            // When
            Ingredient newIngredient = new Ingredient("milk");
            int newAmount = 15;

            storage.AddNewIngredient(newIngredient, newAmount);

            Dictionary<Ingredient, int> actual = storage.IngredientsAmount;

            // Expected
            Dictionary<Ingredient, int> expected = new Dictionary<Ingredient, int>
            {
                { new Ingredient("sugar"), 10 },
                { new Ingredient("salt"), 20 },
                { new Ingredient("milk"), 15 }

            };

            // Then
            int[] actualAmount = actual.Select(amount => amount.Value).ToArray();
            int[] expectedAmount = expected.Select(amount => amount.Value).ToArray();

            CollectionAssert.AreEqual(expectedAmount, actualAmount);              // check array[] or newAmount only
        }

        [Test]
        public void AddExistingIngredientName()                                   // public void AddNewIngredient(Ingredient ingredient, int amount)
        {
            // Given
            List<Ingredient> ingredient = new List<Ingredient>
            {
                new Ingredient("sugar"),
                new Ingredient("salt")
            };

            Storage storage = new Storage(ingredient);

            // When
            Ingredient newIngredient = new Ingredient("sugar");
            int newAmount = 0;

            storage.AddNewIngredient(newIngredient, newAmount);                     // чи повинна бути перевірка на те, чи існує вже такий інгредієнт

            List<Ingredient> actual = storage.Ingredients;

            // Expected
            List<Ingredient> expected = new List<Ingredient>
            {
                new Ingredient("sugar"),
                new Ingredient("salt"),
            };

            string[] actualNames = actual.Select(i => i.Name).ToArray();
            string[] expectedNames = expected.Select(i => i.Name).ToArray();

            // Then
            CollectionAssert.AreEqual(expectedNames, actualNames);
        }

        [Test]
        public void RewriteCountOfExistingIngredient()                            // public void RewriteIngredientCount(int ingredientAmount, string chengeIngredientName)
        {
            // Given
            Ingredient tea = new Ingredient("tea");
            Ingredient coffee = new Ingredient("coffee");
            Ingredient milk = new Ingredient("milk");

            List<Ingredient> ingredients = new List<Ingredient> {tea, coffee};

            Storage storage = new Storage(ingredients);

            storage.IngredientsAmount = new Dictionary<Ingredient, int> 
            {
                { tea, 10 },
                { coffee, 20 }
            };

            // When
            int amount = 30;
            string name = "tea";

            storage.RewriteIngredientCount(amount, name);

            Dictionary<Ingredient, int> actual = storage.IngredientsAmount;

            // Expected
            var expected = new Dictionary<Ingredient, int>
            {
                { new Ingredient("tea"), 30 },
                { new Ingredient("coffee"), 20 }
            };

            int[] actualAmount = actual.Select(amount => amount.Value).ToArray();
            int[] expectedAmount = expected.Select(amount => amount.Value).ToArray();

            // Then
            CollectionAssert.AreEqual(expectedAmount, actualAmount);
        }

        [Test]
        public void DeleteExistingIngredient()                                    // public void DeleteIngredient(string _ingredientName)
        {
            //Given
            List<Ingredient> ingredients = new List<Ingredient> {
                new Ingredient("chicken"),
                new Ingredient("octopus")
            };

            Storage storage = new Storage(ingredients);

            // When
            storage.DeleteIngredient("chicken");                                  // метод видаляє лише Ingredients.Name, але значення залишаються в IngredientsPrice, IngredientsAmount  

            List<Ingredient> actual = storage.Ingredients;

            // Expected
            List<Ingredient> expected = new List<Ingredient> {
                new Ingredient("octopus")
            };

            string[] actualNames = actual.Select(ingredient => ingredient.Name).ToArray();          // ["octopus"]
            string[] expectedNames = expected.Select(ingredient => ingredient.Name).ToArray();      // ["octopus"]

            // Then
            CollectionAssert.AreEqual(expectedNames, actualNames);
        }

        [Test]
        public void DeleteNonExistingIngredient()                                 // public void DeleteIngredient(string _ingredientName)
        {
            //Given
            List<Ingredient> ingredients = new List<Ingredient> {
                new Ingredient("chicken"),
                new Ingredient("octopus")
            };

            Storage storage = new Storage(ingredients);

            // When
            storage.DeleteIngredient("tuna");                                   // чи повинен метод повертати інфо про те, що такого ingredientName немає

            List<Ingredient> actual = storage.Ingredients;

            // Expected
            List<Ingredient> expected = new List<Ingredient> {
                new Ingredient("chicken"),
                new Ingredient("octopus")
            };

            string[] actualNames = actual.Select(ingredient => ingredient.Name).ToArray();          // ["chicken", "octopus"]
            string[] expectedNames = expected.Select(ingredient => ingredient.Name).ToArray();      // ["chicken", "octopus"]

            // Then
            CollectionAssert.AreEqual(expectedNames, actualNames);
        }
    }
}