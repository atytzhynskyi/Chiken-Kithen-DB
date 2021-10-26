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
        public DbSet<FoodComponent> FoodComponents { get; set; }
        public DbSet<IngredientComponent> IngredientComponents { get; set; }
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
            foreach (Allergy allergy in Allergies)
            {
                foreach (Customer customer in Customers.Where(c => c.Id == allergy.CustomerId).ToList())
                {
                    customer.Allergies.Add(allergy);
                }
            }
        }
        private void InitializateRecipeItems()
        {
            foreach (FoodComponent foodComponent in FoodComponents)
            {
                foreach (Food food in Recipes)
                {
                    if (food.Id == foodComponent.RecipeId)
                    {
                        food.RecipeFoods.Add(Recipes.Where(r => r.Id == foodComponent.Id).FirstOrDefault());
                    }
                }
            }
            foreach (IngredientComponent ingredientComponent in IngredientComponents)
            {
                foreach (Food food in Recipes)
                {
                    if (food.Id == ingredientComponent.RecipeId)
                    {
                        food.RecipeIngredients.Add(Ingredients.Where(i => i.Id == ingredientComponent.IngredientId).FirstOrDefault());
                    }
                }
            }
        }
        public void AddBaseFoods()
        {
            using var streamReader = File.OpenText(@"..\..\..\Foods.csv");
            using var csvReader = new CsvReader(streamReader, CultureInfo.CurrentCulture);

            string name;
            while (csvReader.Read())
            {
                List<string> fileLine = new List<string>();
                csvReader.TryGetField<string>(0, out name);
                if (string.IsNullOrEmpty(name)) continue;
                if (Recipes.Any(f => f.Name == name)) continue;

                Food food = new Food(name);
                AddWithoutDuplicate(food);
            }
        }
        public void AddBaseRecipe()
        {
            using var streamReader = File.OpenText(@"..\..\..\Foods.csv");
            using var csvReader = new CsvReader(streamReader, CultureInfo.CurrentCulture);

            string name;
            while (csvReader.Read())
            {
                List<string> fileLine = new List<string>();
                csvReader.TryGetField<string>(0, out name);

                if (string.IsNullOrEmpty(name)) continue;

                Food food = Recipes.Where(f=>f.Name==name).FirstOrDefault();
                for (int i = 1; csvReader.TryGetField<string>(i, out name); i++)
                {
                    if (string.IsNullOrEmpty(name)) continue;

                    if (Recipes.Any(r => r.Name == name))
                    {
                        if (food.RecipeFoods.Any(r => r.Name == name)) continue;
                        food.RecipeFoods.Add(Recipes.Where(r => r.Name == name).FirstOrDefault());
                        continue;
                    }
                    if (Ingredients.Any(i => i.Name == name))
                    {
                        if (food.RecipeIngredients.Any(r => r.Name == name)) continue;
                        food.RecipeIngredients.Add(Ingredients.Where(i => i.Name == name).FirstOrDefault());
                        continue;
                    }
                }
                SaveRecipeItems(food);
            }
        }

        private void SaveRecipeItems(Food food)
        {
            foreach (Food recipeFood in food.RecipeFoods)
            {
                if (!FoodComponents.Any(fc => fc.RecipeId == food.Id && fc.FoodId == recipeFood.Id))
                {
                    FoodComponents.Add(new FoodComponent(food, Recipes.Where(r => r.Name == recipeFood.Name).FirstOrDefault()));
                }
            }
            foreach (Ingredient ingredient in food.RecipeIngredients)
            {
                if (!IngredientComponents.Any(ic => ic.Ingredient.Name == ingredient.Name && ic.Recipe.Name == food.Name))
                {
                    IngredientComponents.Add(new IngredientComponent(food, ingredient));
                }
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
            SaveIngredientsProperties();
        }

        public void SetNullOrder()
        {
            foreach (Customer customer in Customers)
            {
                customer.Order = null;
            }
            SaveChanges();
        }

        public void SaveAll(List<Ingredient> _Ingredients, Dictionary<Ingredient, int> _IngredientsAmount, Dictionary<Ingredient, int> _IngredientsPrice, List<Food> _Recipes, List<Customer> _Customers)
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
                try
                {
                    IngredientProperties ing = new IngredientProperties(_ingredient, _IngredientsAmount[_ingredient], _IngredientsPrice[_ingredient]);
                    ing.IngredientId = Ingredients.Where(i => i.Name == _ingredient.Name).FirstOrDefault().Id;
                    AddWithoutDuplicate(ing);
                }
                catch (System.Collections.Generic.KeyNotFoundException) { }
            }
            SaveIngredientsProperties();
            DeleteNullIdIngredientProperties();
            RemoveZeroIngredientProperties();
            SaveChanges();

            foreach (Food _recipe in _Recipes)
            {
                if (!Recipes.Any(r => r.Name == _recipe.Name))
                {
                    AddWithoutDuplicate(_recipe);
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
                csv.TryGetField<int>(1, out int budget);
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
        public void SaveIngredientsProperties()
        {
            foreach (IngredientProperties ingredientProperties in IngredientProperties)
            {
                foreach (Ingredient ingredient in Ingredients)
                {
                    if (ingredientProperties.IngredientId == ingredient.Id)
                    {
                        ingredientProperties.IngredientId = ingredient.Id;
                    }
                }
            }

            SaveChanges();
        }
        public void RemoveZeroIngredientProperties()
        {
            foreach (IngredientProperties ingredientProperties in IngredientProperties)
            {
                if (ingredientProperties.IngredientId == 0)
                {
                    IngredientProperties.Remove(ingredientProperties);
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
                foreach (Food recipeFood in food.RecipeFoods)
                {
                    if (!FoodComponents.Any(fc => fc.RecipeId == food.Id && fc.FoodId == recipeFood.Id))
                    {
                        FoodComponents.Add(new FoodComponent(food, Recipes.Where(r => r.Name == recipeFood.Name).FirstOrDefault()));
                    }
                }
                foreach (Ingredient ingredient in food.RecipeIngredients)
                {
                    if (!IngredientComponents.Any(ic => ic.Ingredient.Name == ingredient.Name && ic.Recipe.Name == food.Name))
                    {
                        IngredientComponents.Add(new IngredientComponent(food, ingredient));
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