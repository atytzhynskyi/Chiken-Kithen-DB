using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Chiken_Kithen_DB
{
    class DataBase : DbContext
    {
        public DbSet<Food> Recipes { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<Customer> Customers { get; set; }

        public DataBase()
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=ChikenKitchen;Trusted_Connection=True;MultipleActiveResultSets=True;");
        }
        public void AddBaseRecipe()
        {
            using var streamReader = File.OpenText(@"..\..\..\Foods.csv");
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
            };
            using var csvReader = new CsvReader(streamReader, config);
            string name;
            while (csvReader.Read())
            {
                List<string> fileLine = new List<string>();
                for (int i = 0; csvReader.TryGetField<string>(i, out name); i++)
                {
                    fileLine.Add(name);
                }
                Recipes.Add(new Food(fileLine.ToArray()));
            }
            SaveChanges();
        }
        public void AddBaseIngredients()
        {
            using var streamReader = File.OpenText(@"..\..\..\Ingredients.csv");
            using var csvReader = new CsvReader(streamReader, CultureInfo.CurrentCulture);

            string name;
            int amount;
            while (csvReader.Read())
            {
                Ingredient ingredient = new Ingredient();

                csvReader.TryGetField<string>(1, out name);
                if (!int.TryParse(name, out amount))
                    continue;

                csvReader.TryGetField<string>(0, out name);
                if (string.IsNullOrEmpty(name)) continue;

                ingredient.Name = name;
                Ingredients.Add(ingredient);
                SaveChanges();
            }
        }
        public void FillCustomerList()
        {
            using var streamReader = File.OpenText(@"..\..\..\Customers.csv");
            using var csv = new CsvReader(streamReader, CultureInfo.CurrentCulture);

            int lastId = 0;
            while (csv.Read())
            {
                switch (csv.GetField(0))
                {
                    case "Name":
                        Customer customer = new Customer(csv.GetField(1));
                        Customers.Add(customer);
                        lastId = customer.Id;
                        break;
                    case "Allergies":
                        Customer customer1 = Customers.SingleOrDefault(e => e.Id == lastId);
                        for (int i = 0; csv.TryGetField<string>(i, out string name); i++)
                        {
                            Ingredient ingredient = new Ingredient(name);
                            customer1.Allergies.Add(ingredient);
                        }
                        break;
                }
            }
            SaveChanges();
        }
    }
}
