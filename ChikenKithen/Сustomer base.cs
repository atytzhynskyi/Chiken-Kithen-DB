using Microsoft.EntityFrameworkCore;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;
using System.Linq;
using BaseClasses;
using ChikenKitchenDataBase;

namespace ChikenKithen
{
    class CustomerBase
    {
        public List<Customer> Customers { get; set; } = new List<Customer>();
        public CustomerBase() { }
        public CustomerBase(List<Customer> _Customers)
        {
            Customers = _Customers;
        }
        public CustomerBase(ApplicationContext db)
        {
            Customers.AddRange(db.Customers);
        }
        
        public void ShowCustomers()
        {
            foreach(Customer customer in Customers)
            {
                Console.WriteLine(customer.Id + " " + customer.Name + "\n");
                foreach(Allergy allergy in customer.Allergies)
                {
                    Console.Write(allergy.Ingredient.Name + " ");
                }
            }
        }
    }
}
