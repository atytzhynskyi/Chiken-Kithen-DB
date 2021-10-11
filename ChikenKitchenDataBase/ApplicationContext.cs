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
        static void Main(string[] args)
        { }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<IngredientProperties> IngredientProperties { get; set; }
        public DbSet<Food> Recipes { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<RecipeItem> RecipeItems { get; set; }
        public DbSet<Allergy> Allergies { get; set; }
        public ApplicationContext()
        {
            Database.EnsureCreated();
            InitializateRecipeItems();
            SetCustomersAllergies();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=ChikenKitchen;Trusted_Connection=True;MultipleActiveResultSets=True;");
        }
        private void SetCustomersAllergies()
        {
            foreach(Allergy allergy in Allergies)
            {
                Customers.Where(c => c.Id == allergy.CustomerId).FirstOrDefault().Allergies.Add(allergy);
            }
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
        public void AddIngredientCount()
        {
            using var streamReader = File.OpenText(@"..\..\..\Ingredients.csv");
            using var csvReader = new CsvReader(streamReader, CultureInfo.CurrentCulture);

            string name;
            int amount;
            while (csvReader.Read())
            {
                csvReader.TryGetField<string>(1, out name);
                if (!int.TryParse(name, out amount))
                    continue;

                csvReader.TryGetField<string>(0, out name);
                if (string.IsNullOrEmpty(name)) continue;

                if (!IngredientProperties.Any(ing => ing.IngredientId == Ingredients.Where(i => i.Name == name).FirstOrDefault().Id))
                {
                    AddWithoutDuplicate(new IngredientProperties(Ingredients.Where(ingredient => ingredient.Name == name).FirstOrDefault(), amount));
                }
            }
            SaveIngredientsCount();
        }

        public void SetNullOrder()
        {
            foreach(Customer customer in Customers)
            {
                customer.Order = null;
            }
            SaveChanges();
        }

        public void SaveAll(List<Ingredient> _Ingredients, Dictionary<Ingredient, int> _IngredientsAmount, List<Food> _Recipes, List<Customer> _Customers)
        {
            foreach (Ingredient _ingredient in _Ingredients)
            {
                if (!Ingredients.Any(i => i.Name == _ingredient.Name))
                {
                    Ingredients.Add(_ingredient);
                }
            }
            SaveChanges();
            foreach (Ingredient _ingredient in _Ingredients)
            {
                foreach (var ingredientAmount in _IngredientsAmount)
                {
                    if (ingredientAmount.Key.Name == _ingredient.Name)
                    {
                        IngredientProperties ing = new IngredientProperties(_ingredient, ingredientAmount.Value);
                        ing.IngredientId = Ingredients.Where(i => i.Name == _ingredient.Name).FirstOrDefault().Id;
                        AddWithoutDuplicate(ing);
                    }
                }
            }
            SaveIngredientsCount();
            DeleteNullIdIngredientProperties();
            SaveChanges();
            foreach (Food _recipe in _Recipes)
            {
                if (!Recipes.Any(r => r.Name == _recipe.Name))
                {
                    Recipes.Add(_recipe);
                }
            }
            SaveChanges();
            SaveRecipeItems();
            foreach (Customer _customer in _Customers)
            {
                if (!Customers.Any(c => c.Name == _customer.Name))
                {
                    Customers.Add(_customer);
                }
            }
            SaveChanges();
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
        public Dictionary<Ingredient, int> GetIngredientsAmount()
        {
            Dictionary<Ingredient, int> IngredientAmount = new Dictionary<Ingredient, int>();
            foreach (IngredientProperties ingredientProperties in IngredientProperties)
            {
                IngredientAmount.Add(Ingredients.Where(i => i.Id == ingredientProperties.IngredientId).FirstOrDefault(), ingredientProperties.Count);
            }
            foreach (var ingredient in Ingredients)
            {
                try
                {
                    _ = IngredientAmount[ingredient];
                }
                catch (System.Collections.Generic.KeyNotFoundException)
                {
                    IngredientAmount.Add(ingredient, 0);
                }
            }
            return IngredientAmount;
        }
        public void SaveIngredientsCount()
        {
            foreach (IngredientProperties ingredientProperties in IngredientProperties)
            {
                foreach (Ingredient ingredient in Ingredients)
                {
                    if (ingredientProperties.Ingredient == ingredient)
                    {
                        ingredientProperties.IngredientId = ingredient.Id;
                    }
                }
            }

            SaveChanges();
        }
        public void DeleteNullIdIngredientProperties()
        {
            foreach (var count in IngredientProperties)
            {
                if (count.IngredientId == 0)
                {
                    IngredientProperties.Remove(count);
                }
            }
        }
        public void SaveRecipeItems()
        {
            foreach (Food food in Recipes)
            {
                foreach (RecipeItem recipeItem in food.Recipe)
                {
                    if (!RecipeItems.Any(recipeItemDb => recipeItem.Food.Name == recipeItemDb.Food.Name
                                                      && recipeItem.Ingredient.Name == recipeItemDb.Ingredient.Name))
                    {
                        RecipeItems.Add(new RecipeItem(food, recipeItem.Ingredient));
                    }
                }
            }
            SaveChanges();
        }
        public void AddWithoutDuplicate(IngredientProperties _ingredientProperties)
        {
            foreach (IngredientProperties ingredientProp in IngredientProperties)
            {
                foreach (Ingredient ingredient in Ingredients)
                {
                    if (ingredient.Name == _ingredientProperties.Ingredient.Name)
                    {
                        _ingredientProperties.IngredientId = ingredient.Id;
                        if (_ingredientProperties.IngredientId == ingredientProp.IngredientId)
                        {
                            return;
                        }
                    }
                }
            }
            IngredientProperties.Add(_ingredientProperties);
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