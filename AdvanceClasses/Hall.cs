using System;
using System.Collections.Generic;
using System.Text;
using BaseClasses;
using System.Linq;
using jsonReadModule;

namespace AdvanceClasses
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

        private string GetFullNameByNick(string nickName, Customer customer)
        {
            if (customer.Name.Contains(nickName))
            {
                return customer.Name;
            }
            return "NULL";
        }

        public void GetPaid(Accounting accounting, List<Food> Recipes, Customer customer)
        {
            double price = accounting.CalculateFoodMenuPrice(Recipes, customer.Order);

            if (IsDiscountAppliable(customer))
            {
                price -=  GetDiscount() * price;
            }
            customer.budget = Math.Round(customer.budget - price, 2);
            accounting.AddMoney(price);
        }

        public double GetDiscount()
        {
            return (float)JsonRead.ReadFromJson<int>(@"..\..\..\Configs\Discount.json").Values.First()/100;
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
