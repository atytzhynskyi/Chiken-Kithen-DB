using System;
using System.Collections.Generic;
using System.Text;
using BaseClasses;
using System.Linq;
using jsonReadModule;

namespace ChikenKithen
{
    public class Hall
    {
        public List<Customer> Customers { get; set; }

        public List<Food> Menu = new List<Food>();
        public Hall() { }

        public Hall(List<Customer> customers, List<Food> _Menu)
        {
            Customers = customers;
            Menu = _Menu;
        }

        public bool isNewCustomer(string Name)
        {
            if(Customers.Any(c => c.Name == Name))
                return false;
            else
                return true;
        }
        public void AddNewCustomer(Customer customer)
        {
            Customers.Add(customer);
        }
        public Customer GetCustomer(string Name)
        {
            Customer customer = new Customer("NULL");
            bool isFound = false;
            foreach (Customer customerSearch in Customers)
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
            foreach (Food food in kitchen.Storage.Recipes)
            {
                if (food.Name == customer.Order.Name)
                {
                    if (kitchen.Storage.FoodAmount[food] < 1)
                    {
                        Console.WriteLine("We dont have enough {0} for {1}", customer.Order.Name, customer.Name);
                        return;
                    }
                    kitchen.Storage.FoodAmount[food]--;
                    return;
                }
            }
            Console.WriteLine("Order doesnt exist in Food List");
        }
        public List<Ingredient> AskAllergiesIngredients()
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
        
        private string GetFullNameByNick(string nickName, Customer customer)
        {
            if (customer.Name.Contains(nickName))
            {
                return customer.Name;
            }
            return "NULL";
        }

        public void GetPaid(Kitchen kitchen, Customer customer)
        {
            int price = kitchen.CalculateFoodMenuPrice(customer.Order);
            customer.VisitsCount++;

            if (IsDiscountAppliable(customer))
            {
                price -= Convert.ToInt32(GetDiscount() * price);
            }
            customer.budget -= price;
            kitchen.Budget += price;
        }

        private double GetDiscount()
        {
            return jsonRead.ReadFromJson<int>(@"..\..\..\discount.json").Values.First()/100;
        }

        private bool IsDiscountAppliable(Customer customer)
        {
            if (customer.VisitsCount % 3 == 0)
            {
                return true;
            }
            return false;
        }
    }
}
