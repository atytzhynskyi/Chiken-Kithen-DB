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
    class CustomerBase : DbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public CustomerBase()
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=customers;Trusted_Connection=True;");
        }
        public void FillCustomerList()
        {
            using var streamReader = File.OpenText("Customers.csv");
            using var csv = new CsvReader(streamReader, CultureInfo.CurrentCulture);

            int lastId = 0;
            while (csv.Read())
            {
                switch (csv.GetField(0))
                {
                    case "Name":
                        Customer customer = new Customer(csv.GetField(1));
                        Customers.Add(customer);
                        SaveChanges();
                        lastId = customer.Id;
                        break;
                    case "Allergies":
                        Customer customer1 = Customers.SingleOrDefault(e => e.Id == lastId);
                        for (int i=0; csv.TryGetField<string>(i, out string value); i++)
                        {
                            Ingredient ingredient = new Ingredient(value);
                            customer1.Allergies.Add(ingredient);
                        }
                        break;
                }
            }
            SaveChanges();
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
