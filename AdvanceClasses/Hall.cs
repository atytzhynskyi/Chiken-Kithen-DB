using System;
using System.Collections.Generic;
using BaseClasses;
using System.Linq;
using jsonReadModule;

namespace AdvanceClasses
{
    public class Hall
    {
        const string DISCOUNT_CONFIG_FILE_PATH = @"..\..\..\Configs\Discount.json";

        public List<Customer> Customers { get; set; }

        public List<Food> Menu = new List<Food>();
        public Hall() { }
        public Hall(List<Customer> customers, List<Food> _Menu)
        {
            Customers = customers;
            Menu = _Menu;
        }

        public Customer GetCustomer(string Name)
        {
            return Customers.Find(c=>c.Name == Name);
        }
        
        public void GiveFoodFromStorage(Kitchen kitchen, Customer customer)
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

        public void GiveFood(string name)
        {
            //method to give just cooked food. Dont need implementation yet.
        }

        public void GetPaid(Accounting accounting, List<Food> Recipes, Customer customer)
        {
            double price = accounting.CalculateFoodMenuPrice(Recipes, customer.Order);

            if (IsDiscountAppliable(customer))
            {
                price -=  GetDiscountValueFromFile() * price;
            }
            customer.budget = Math.Round(customer.budget - price, 2);
            accounting.AddMoney(price);
        }

        public double GetDiscountValueFromFile()
        {
            var parsedValues = JsonRead.ReadFromJson<int>(DISCOUNT_CONFIG_FILE_PATH).Values;

            if(!object.Equals(parsedValues, null))
            {
                var discount = parsedValues.FirstOrDefault();

                return (double)discount / 100;
            }

            return (double)0;
        }

        public bool IsDiscountAppliable(Customer customer)
        {
            if (customer.VisitsCount % 3 == 0)
            {
                return true;
            }
            return false;
        }
    }
}
