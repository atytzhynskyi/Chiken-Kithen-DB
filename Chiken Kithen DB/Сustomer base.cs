using Microsoft.EntityFrameworkCore;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;
using System.Linq;

namespace Chiken_Kithen_DB
{
    class CustomerBase
    {
        public List<Customer> Customers { get; set; }
        public CustomerBase(ApplicationContext db)
        {
            Customers.AddRange(db.Customers);
        }
        
        public void ShowCustomers()
        {
            foreach(Customer customer in Customers)
            {
                Console.WriteLine(customer.Id + " " + customer.Name + "\n");
                foreach(Ingredient ingredient in customer.Allergies)
                {
                    Console.Write(ingredient.Name + " ");
                }
            }
        }
    }
}
