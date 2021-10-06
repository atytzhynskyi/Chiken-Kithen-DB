using System;
using System.Collections.Generic;
using System.Text;

namespace Chiken_Kithen_DB
{
    class Hall
    {
        CustomerBase AllCustomers = new CustomerBase();
        public Hall() { }
        public Hall(ApplicationContext applicationContext)
        {
            AllCustomers = new CustomerBase(applicationContext);
        }
        public Hall(List<Customer> customers)
        {
            AllCustomers = new CustomerBase(customers);
        }
        public CustomerBase GetAllCustomers() => AllCustomers;
        public bool isNewCustomer(string Name)
        {
            foreach (Customer customerTemp in AllCustomers.Customers)
            {
                if (customerTemp.Name == Name)
                {
                    return false;
                }
            }
            return true;
        }
        public void AddNewCustomer(Customer customer)
        {
            AllCustomers.Customers.Add(customer);
        }
        public Customer GetCustomer(string Name)
        {
            Customer customer = new Customer("NULL");
            bool isFound = false;
            foreach (Customer customerSearch in AllCustomers.Customers)
            {
                if (customerSearch.Name == Name)
                {
                    customer = customerSearch;
                    break;
                }
                if (GetFullNameByNick(Name, customerSearch) == "NULL")
                {
                    continue;
                }
                else
                {
                    customer = customerSearch;
                    if (isFound)
                    {
                        Console.WriteLine("Sorry, can't do, " + Name + " is unidentified");
                        return new Customer("NULL");
                    }
                    isFound = true;
                }
            }
            return customer;
        }
        public void GiveFood(Kitchen kitchen, Customer customer)
        {
            foreach (Food food in kitchen.RecipeBook.Recipes)
            {
                if (food.Name == customer.Order.Name)
                {
                    if (kitchen.FoodAmount[food] < 1)
                    {
                        Console.WriteLine("We dont have " + customer.Order.Name);
                        return;
                    }
                    kitchen.FoodAmount[food]--;
                    Console.WriteLine(customer.Name + " get " + food.Name);
                    customer.Order = new Food("");
                    return;
                }
            }
            Console.WriteLine("Order doesnt exist in Ingedient List");
        }
        public List<Ingredient> AskAllergies()
        {
            List<Ingredient> allergicIngredients = new List<Ingredient>();
            Console.WriteLine("Do you have any allergies? (please use ',' between allergic food)");
            string[] allergicFoodName = Console.ReadLine().Split(", ");
            foreach (string ingredientName in allergicFoodName)
            {
                allergicIngredients.Add(new Ingredient(ingredientName));
            }
            return allergicIngredients;
        }
        public Food AskOrder()
        {
            Console.WriteLine("What you prefer to order?");
            string _Order = Console.ReadLine();
            Food food = new Food(_Order);
            return food;
        }
        public string AskName()
        {
            Console.WriteLine("Welcome to Chicken Kitchen, what is your name?");
            string Name = Console.ReadLine();
            return Name;
        }
        private string GetFullNameByNick(string nickName, Customer customer)
        {
            if (customer.Name.Contains(nickName))
            {
                return customer.Name;
            }
            return "NULL";
        }
    }
}
