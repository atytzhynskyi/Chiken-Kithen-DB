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
    class TestHall
    {
        /*
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
        */

        [Test]
        public void IsCustomerNewTest()                                            // public bool isNewCustomer(string Name)
        {
            // Given
            string name_given = "John Doe";
            string name_expected = "Jane Doe";

            Hall hall = new Hall();

            hall.Customers = new List<Customer> { new Customer(name_given) };

            // When
            bool actual = hall.isNewCustomer(name_expected);

            // Then
            Assert.IsTrue(actual);
        }

        [Test]
        public void IsCustomerExistingTest()                                       // public bool isNewCustomer(string Name)
        {
            // Given
            string name_given = "John Doe";
            string name_expected = "John Doe";

            Hall hall = new Hall();

            hall.Customers = new List<Customer> { new Customer(name_given) };

            // When
            bool actual = hall.isNewCustomer(name_expected);

            // Then
            Assert.IsFalse(actual);
        }

        [Test]
        public void IsEmptyStringTest()                                            // public bool isNewCustomer(string Name)
        {
            // Given
            string name_given = "John Doe";
            string name_expected = "";

            Hall hall = new Hall();

            hall.Customers = new List<Customer> { new Customer(name_given) };

            // When
            bool actual = hall.isNewCustomer(name_expected);

            // Then
            Assert.IsFalse(actual);
        }

        [Test]
        public void IsNullTest()                                                  // public bool isNewCustomer(string Name)
        {
            // Given
            string name_given = "John Doe";
            string name_expected = null;

            Hall hall = new Hall();

            hall.Customers = new List<Customer> { new Customer(name_given) };

            // When
            bool actual = hall.isNewCustomer(name_expected);

            // Then
            Assert.IsFalse(actual);
        }

        [Test]
        public void AddNewCustomerTest()                                // public void AddNewCustomer(Customer customer)
        {
            // Given
            Hall hall = new Hall();
            hall.Customers = new List<Customer> { new Customer("John Doe") };

            Customer JaneDoe = new Customer("Jane Doe");

            // When
            hall.AddNewCustomer(JaneDoe);
            var actual = hall.Customers;

            // Expected
            List<Customer> expected = new List<Customer> { new Customer("John Doe"), new Customer("Jane Doe") };
            string[] actualCustomers = actual.Select(i => i.Name).ToArray();
            string[] expectedCustomers = expected.Select(i => i.Name).ToArray();

            // Then
            CollectionAssert.AreEqual(actualCustomers, expectedCustomers);
        }

        [Test]
        public void AddNewCustomerNullTest()                                // public void AddNewCustomer(Customer customer)
        {
            // Given
            Hall hall = new Hall();
            hall.Customers = new List<Customer> { new Customer("John Doe") };

            Customer JaneDoe = null;

            // When
            hall.AddNewCustomer(JaneDoe);
            var actual = hall.Customers;                 // тут в список попадає null. Як має оброблятися null

            // Expected
            List<Customer> expected = new List<Customer> { new Customer("John Doe") };
            string[] actualCustomers = actual.Select(i => i.Name).ToArray();
            string[] expectedCustomers = expected.Select(i => i.Name).ToArray();

            // Then
            CollectionAssert.AreEqual(actualCustomers, expectedCustomers);
        }

        [Test]
        public void GetExistingCustomerTest()                               // public Customer GetCustomer(string Name)
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
        public void GetNonExistingCustomerTest()                            // public Customer GetCustomer(string Name)
        {
            // Given
            Hall hall = new Hall();
            hall.Customers = new List<Customer> { new Customer("Jane Doe") };

            // When
            Customer actual = hall.GetCustomer("John Doe");

            // Expected
            Customer expected = new Customer("NULL");

            // Then
            Assert.AreEqual(expected.Name, actual.Name);
        }

        [Test]
        public void GetExistingCustomerByNickNameTest()                 // public Customer GetCustomer(string Name)
        {
            // Given
            Hall hall = new Hall();
            hall.Customers = new List<Customer> { new Customer("Jane Doe") };

            // When
            Customer actual = hall.GetCustomer("Jane");

            // Expected
            Customer expected = new Customer("Jane Doe");

            // Then
            Assert.AreEqual(expected.Name, actual.Name);
        }


        [Test]
        public void GiveFoodFromStorageTest()                            // public void GiveFoodFromStorage(Kitchen kitchen, Customer customer)
        {
            // Given
            Hall hall = new Hall();

            Ingredient coffee = new Ingredient("coffee");
            Ingredient water = new Ingredient("water");
            Food americano = new Food("americano", new Ingredient[] { coffee, water });
            Food cappuccino = new Food("cappuccino");
            cappuccino.RecipeFoods = new List<Food> {americano};

            List <Ingredient> ingredientList = new List<Ingredient> { coffee, water };
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
        public void GetPaidTest()
        {
            // Given
            Hall hall = new Hall();

            Ingredient coffee = new Ingredient("coffee");
            Ingredient water = new Ingredient("water");
            Dictionary<Ingredient, int> ingredientsPrice = new Dictionary<Ingredient, int> { { coffee, 5 }, { water, 2 } };

            Food americano = new Food("americano", new Ingredient[] { coffee, water });
            List<Food> recipes = new List<Food> { americano };

            Accounting accounting = new Accounting(100);

            Customer customer = new Customer("John Doe");
            customer.Order = americano;
            customer.VisitsCount = 1;
            customer.budget = 50;

            // When
            hall.GetPaid(accounting, ingredientsPrice, recipes, customer);
            double actual = customer.budget;

            // Then
            Assert.AreEqual(43, actual);
        }

        [Test]
        public void GetPaidTest1()
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

            Accounting accounting = new Accounting(100);

            Customer customer = new Customer("John Doe");
            customer.Order = cappuccino;
            customer.VisitsCount = 1;
            customer.budget = 50;

            // When
            hall.GetPaid(accounting, ingredientsPrice, recipes, customer);

            // Then
            Assert.AreEqual(36, customer.budget);
            //Assert.AreEqual(114, accounting.Budget);

        }

        [Test]
        public void GetPaidTest2()
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

            Accounting accounting = new Accounting(100);

            Customer customer = new Customer("John Doe");
            customer.Order = cappuccino;
            customer.VisitsCount = 1;
            customer.budget = 10;

            // When
            hall.GetPaid(accounting, ingredientsPrice, recipes, customer);
            double actual = customer.budget;

            // Then
            Assert.AreEqual(36, actual);
        }
    }
}
