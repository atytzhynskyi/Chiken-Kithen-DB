using System.Collections.Generic;
using NUnit.Framework;
using BaseClasses;
using AdvanceClasses;

namespace ChickenKitchenTests
{
    class TestHall
    {
        List<Customer> customers;
        List<Food> menu;

        [SetUp]
        public void SetUp()
        {
            Customer JohnDoe = new Customer("John Doe");
            Customer JaneDoe = new Customer("Jane Doe");
            customers = new List<Customer> { JohnDoe, JaneDoe };

            Food cappuccino = new Food("cappuccino");
            Food americano = new Food("americano");

            Hall hall = new Hall(customers, menu);
        }


        [Test]
        public void GetExistingCustomer()                                         // public Customer GetCustomer(string Name)
        {
            // Given
            Hall hall = new Hall();
            hall.Customers = new List<Customer> { new Customer("Jane Doe") };

            // When
            Customer actual = hall.GetCustomer("Jane Doe");

            // Expected
            Customer expected = new Customer("Jane Doe");

            // Then
            Assert.AreEqual(expected.Name, actual.Name);
        }

        [Test]
        public void GetNonExistingCustomer()                                      // public Customer GetCustomer(string Name)
        {
            // Given
            Hall hall = new Hall();
            hall.Customers = new List<Customer> { new Customer("Jane Doe") };

            // When
            Customer actual = hall.GetCustomer("John Doe");

            // Then
            Assert.IsTrue(object.Equals(actual, null));
        }


        [Test]
        public void GiveExistingFoodFromStorage()                                 // public void GiveFoodFromStorage(Kitchen kitchen, Customer customer)
        {
            // Given
            Hall hall = new Hall();

            Ingredient coffee = new Ingredient("coffee");
            Ingredient water = new Ingredient("water");
            Food americano = new Food("americano", new Ingredient[] { coffee, water });
            Food cappuccino = new Food("cappuccino");
            cappuccino.RecipeFoods = new List<Food> { americano };

            List<Ingredient> ingredientList = new List<Ingredient> { coffee, water };
            List<Food> foodList = new List<Food> { americano };

            Storage storage = new Storage(foodList, ingredientList);
            storage.FoodAmount = new Dictionary<Food, int> { { americano, 2 } };

            Kitchen kitchen = new Kitchen(storage);
            Customer customer = new Customer("John Doe");
            customer.Order = new Food("americano");

            // When
            hall.GiveFoodFromStorage(kitchen, customer);
            int actual = kitchen.Storage.FoodAmount[americano];

            // Then
            Assert.AreEqual(1, actual);
        }

        [Test]
        public void GetPaidFoodThatConsistOfIngredientsOnlyWithoutTip()                     // public void GetPaid(Accounting accounting, Dictionary<Ingredient, int> ingredientPrice, List<Food> Recipes, Customer customer)
        {
            // Given
            Hall hall = new Hall();

            Ingredient coffee = new Ingredient("coffee");
            Ingredient water = new Ingredient("water");
            Dictionary<Ingredient, int> ingredientsPrice = new Dictionary<Ingredient, int> { { coffee, 5 }, { water, 2 } };

            Food americano = new Food("americano", new Ingredient[] { coffee, water });
            List<Food> recipes = new List<Food> { americano };

            Accounting accounting = new Accounting(100, 0.1, 0.4, 0.2, 0, ingredientsPrice);

            Customer customer = new Customer("John Doe");
            customer.Order = americano;
            customer.VisitsCount = 1;
            customer.budget = 50;

            var amountOfTip = 0;

            // When
            hall.GetPaid(accounting, recipes, customer, amountOfTip);
            double actual = customer.budget;

            // Then
            Assert.AreEqual(40.2, actual);
        }

        [Test]
        public void GetPaidFoodThatConsistOfFoodsAndIngredientsWithoutTip()
        {
            // Given
            Hall hall = new Hall();

            Ingredient coffee = new Ingredient("coffee");
            Ingredient water = new Ingredient("water");
            Ingredient milk = new Ingredient("milk");
            Ingredient sugar = new Ingredient("sugar");
            Dictionary<Ingredient, int> ingredientsPrice = new Dictionary<Ingredient, int> { { coffee, 5 }, { water, 2 }, { milk, 4 }, { sugar, 3 } };

            Food americano = new Food("americano", new Ingredient[] { coffee, water });
            Food cappuccino = new Food("cappuccino");
            cappuccino.RecipeIngredients = new List<Ingredient> { milk, sugar };
            cappuccino.RecipeFoods = new List<Food> { americano };

            List<Food> recipes = new List<Food> { cappuccino, americano };

            Accounting accounting = new Accounting(100, 0.1, 0.4, 0.2, 0, ingredientsPrice);

            Customer customer = new Customer("John Doe");
            customer.Order = cappuccino;
            customer.VisitsCount = 1;
            customer.budget = 50;

            var amountOfTip = 0;

            // When
            hall.GetPaid(accounting, recipes, customer, amountOfTip);

            // Then
            Assert.AreEqual(30.4, customer.budget);
            //Assert.AreEqual(114, accounting.Budget);
        }             // public void GetPaid(Accounting accounting, Dictionary<Ingredient, int> ingredientPrice, List<Food> Recipes, Customer customer)

        [Test]
        public void GetPaidFoodThatConsistOfFoodsAndIngredientsWithTipAndTipLessThanBudgetOfCustomer()
        {
            // Given
            Hall hall = new Hall();

            Ingredient coffee = new Ingredient("coffee");
            Ingredient water = new Ingredient("water");
            Ingredient milk = new Ingredient("milk");
            Ingredient sugar = new Ingredient("sugar");
            Dictionary<Ingredient, int> ingredientsPrice = new Dictionary<Ingredient, int> { { coffee, 5 }, { water, 2 }, { milk, 4 }, { sugar, 3 } };

            Food americano = new Food("americano", new Ingredient[] { coffee, water });
            Food cappuccino = new Food("cappuccino");
            cappuccino.RecipeIngredients = new List<Ingredient> { milk, sugar };
            cappuccino.RecipeFoods = new List<Food> { americano };

            List<Food> recipes = new List<Food> { cappuccino, americano };

            Accounting accounting = new Accounting(100, 0.1, 0.4, 0.2, 0, ingredientsPrice);

            Customer customer = new Customer("John Doe");
            customer.Order = cappuccino;
            customer.VisitsCount = 1;
            customer.budget = 50;

            var amountOfTip = 20;

            // When
            hall.GetPaid(accounting, recipes, customer, amountOfTip);

            // Then
            Assert.AreEqual(10.4, customer.budget);
        }

        [Test]
        public void GetPaidFoodThatConsistOfFoodsAndIngredientsWithTipAndTipBiggerThanBudgetOfCustomer()
        {
            // Given
            Hall hall = new Hall();

            Ingredient coffee = new Ingredient("coffee");
            Ingredient water = new Ingredient("water");
            Ingredient milk = new Ingredient("milk");
            Ingredient sugar = new Ingredient("sugar");
            Dictionary<Ingredient, int> ingredientsPrice = new Dictionary<Ingredient, int> { { coffee, 5 }, { water, 2 }, { milk, 4 }, { sugar, 3 } };

            Food americano = new Food("americano", new Ingredient[] { coffee, water });
            Food cappuccino = new Food("cappuccino");
            cappuccino.RecipeIngredients = new List<Ingredient> { milk, sugar };
            cappuccino.RecipeFoods = new List<Food> { americano };

            List<Food> recipes = new List<Food> { cappuccino, americano };

            Accounting accounting = new Accounting(100, 0.1, 0.4, 0.2, 0, ingredientsPrice);

            Customer customer = new Customer("John Doe");
            customer.Order = cappuccino;
            customer.VisitsCount = 1;
            customer.budget = 50;

            var amountOfTip = 40;

            // When
            hall.GetPaid(accounting, recipes, customer, amountOfTip);

            // Then
            Assert.AreEqual(0, customer.budget);
        }

    }
}
