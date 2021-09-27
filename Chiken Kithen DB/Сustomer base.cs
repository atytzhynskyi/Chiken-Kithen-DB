using Microsoft.EntityFrameworkCore;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;

namespace Chiken_Kithen_DB
{
    class СustomerBase : DbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public СustomerBase()
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=storage;Trusted_Connection=True;");
        }
        public void FillCustomerList()
        {
            using var streamReader = File.OpenText("Customers.csv");
            using var csvReader = new CsvReader(streamReader, CultureInfo.CurrentCulture);


        }
    }
}
