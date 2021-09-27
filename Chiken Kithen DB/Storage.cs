using CsvHelper;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Chiken_Kithen_DB
{
    class Storage : DbContext
    {
        public DbSet<Ingredient> Ingredients { get; set; }
        public Dictionary<Ingredient, int> IngredientsAmount { get; set; } = new Dictionary<Ingredient, int>();
        public Storage()
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=storage;Trusted_Connection=True;");
        }
        public void AddBaseIngredients()
        {
            using var streamReader = File.OpenText("Ingredients.csv");
            using var csvReader = new CsvReader(streamReader, CultureInfo.CurrentCulture);

            string value;
            int amount;
            while (csvReader.Read())
            {
                Ingredient ingredient = new Ingredient();

                csvReader.TryGetField<string>(1, out value);
                if (!int.TryParse(value, out amount))
                    continue;

                csvReader.TryGetField<string>(0, out value);
                ingredient.Name = value;
                Ingredients.Add(ingredient);
                SaveChanges();

                csvReader.TryGetField<int>(1, out amount);
                IngredientsAmount.Add(ingredient, amount);
            }
        }

        public void AddNewIngredient(Ingredient ingredient, int amount)
        {
            Ingredients.Add(ingredient);
            IngredientsAmount.Add(ingredient, amount);
            SaveChanges();
        }
        public void RewriteIngredientCount(int ingredientAmount, string chengeIngredientName)
        {
            foreach (var ingredient in from Ingredient ingredient in Ingredients
                                       where ingredient.Name == chengeIngredientName
                                       select ingredient)
            {
                IngredientsAmount[ingredient] = ingredientAmount;
                SaveChanges();
                return;
            }
        }
        public void DeleteIngredient(string _ingredientName)
        {
            foreach (Ingredient ingredient in Ingredients)
            {
                if (ingredient.Name == _ingredientName)
                {
                    Ingredients.Remove(ingredient);
                    return;
                }
            }
        }
        public void ShowIngredients()
        {
            var ingredients = Ingredients.ToList();
            Console.WriteLine("Ingredients List:");
            foreach (Ingredient ingredient in ingredients)
            {
                Console.WriteLine(ingredient.Name + " " + IngredientsAmount[ingredient]);
            }
        }
        public void Clear()
        {
            foreach (Ingredient ingredient in Ingredients)
            {
                Ingredients.Remove(ingredient);
            }
            SaveChanges();
        }
    }
}
