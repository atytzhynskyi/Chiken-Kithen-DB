using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using BaseClasses;

namespace ChikenKitchenDataBase
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<Food> Recipes { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<RecipeItem> RecipeItems { get; set; }
        public DbSet<Allergy> Allergies { get; set; }

        public ApplicationContext()
        {
            Database.EnsureCreated();
            InitializateRecipeItems();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=ChikenKitchen;Trusted_Connection=True;MultipleActiveResultSets=True;");
        }
        private void InitializateRecipeItems()
        {
            foreach (Food food in Recipes)
            {
                var readRecipeItems = RecipeItems.FromSqlRaw<RecipeItem>("SELECT * FROM RecipeItems WHERE FoodId=@id", new SqlParameter("@id", food.Id)).ToList();
                foreach (Ingredient ingredient in Ingredients)
                {
                    foreach (RecipeItem recipeItem in readRecipeItems)
                    {
                        if (recipeItem.IngredientId == ingredient.Id)
                        {
                            food.Recipe.Add(new RecipeItem(food, ingredient));
                        }
                    }
                }
            }
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
                Food food = new Food(Ingredients.ToList(), fileLine.ToArray());
                AddWithoutDuplicate(food);
                SaveChanges();
                SaveRecipeItems();
            }
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
                AddWithoutDuplicate(ingredient);
                SaveChanges();
            }
        }
        public void FillCustomerList()
        {
            using var streamReader = File.OpenText(@"..\..\..\Customers.csv");
            using var csv = new CsvReader(streamReader, CultureInfo.CurrentCulture);

            while (csv.Read())
            {
                Customer customer = new Customer(csv.GetField(1));
                AddWithoutDuplicate(customer);
                csv.Read();
                for (int i = 1; csv.TryGetField<string>(i, out string ingredientName); i++)
                {
                    if (string.IsNullOrEmpty(ingredientName)) break;
                    customer.Allergies.Add(new Allergy(customer, new Ingredient(ingredientName)));
                }
                SaveChanges();
            }
        }
        public void SaveRecipeItems()
        {
            foreach (Food food in Recipes)
            {
                foreach (RecipeItem recipeItem in food.Recipe)
                {
                    bool isFound = false;
                    foreach (RecipeItem recipeItemDb in RecipeItems)
                    {
                        if (recipeItem.Food.Name == recipeItemDb.Food.Name && recipeItem.Ingredient.Name == recipeItemDb.Ingredient.Name)
                        {
                            isFound = true;
                            break;
                        }
                    }
                    if (!isFound)
                        RecipeItems.Add(new RecipeItem(food, recipeItem.Ingredient));

                }
            }
            SaveChanges();
        }
        public void AddWithoutDuplicate(Ingredient ingredient)
        {
            if (Ingredients.Any(ingredientRead => ingredientRead.Name == ingredient.Name))
            {
                return;
            }
            Ingredients.Add(ingredient);
            SaveChanges();
        }
        public void AddWithoutDuplicate(Food food)
        {
            if (Recipes.Any(foodRead => foodRead.Name == food.Name))
            {
                return;
            }
            Recipes.Add(food);
            SaveChanges();
        }
        public void AddWithoutDuplicate(Customer customer)
        {
            if (Customers.Any(customerRead => customerRead.Name == customer.Name))
            {
                return;
            }
            Customers.Add(customer);
            SaveChanges();
        }
    }
}