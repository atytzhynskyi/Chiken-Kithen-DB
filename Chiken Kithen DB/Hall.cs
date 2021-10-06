using System;
using System.Collections.Generic;
using System.Text;

namespace Chiken_Kithen_DB
{
    class Hall
    {
        List<Customer> AllCustomers = new List<Customer>();
        public Hall() { }
        public Hall(List<Customer> customers)
        {
            AllCustomers.AddRange(customers);
        }
        public List<Customer> GetAllCustomers() => AllCustomers;
        public bool isNewCustomer(string Name)
        {
            foreach (Customer customerTemp in AllCustomers)
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
            AllCustomers.Add(customer);
        }
        public Customer GetCustomer(string Name)
        {
            foreach (Customer customer in AllCustomers)
            {
                if (customer.Name == Name)
                {
                    return customer;
                }
                if (GetFullNameByNick(Name, customer) == "NULL")
                {
                    continue;
                }
                else return customer;
            }
            return new Customer("NULL");
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
            string[] splitName = customer.Name.Split(" ");
            foreach (string splitNamePart in splitName)
            {
                if (splitNamePart == nickName)
                {
                    return customer.Name;
                }
            }
            return "NULL";
        }
    }
}
