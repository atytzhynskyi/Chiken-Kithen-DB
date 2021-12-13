using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using BaseClasses;

namespace ChikenKitchenDataBase
{
    public class ApplicationContext : DbContext
    {
        static void Main(string[] args)
        { }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<IngredientProperties> IngredientProperties { get; set; }
        public DbSet<Food> Foods { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<FoodComponent> FoodComponents { get; set; }
        public DbSet<IngredientComponent> IngredientComponents { get; set; }
        public DbSet<Allergy> Allergies { get; set; }
        public ApplicationContext()
        {
            Database.EnsureCreated();

            SetRecipes();
            SetCustomersAllergies();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=ChikenKitchen;Trusted_Connection=True;MultipleActiveResultSets=True;");
        }

        public void InitializeFromFiles()
        {
            if (!Budgets.Any())
            {
                Budgets.Add(new Budget(500));
            }
            if (!Ingredients.Any())
            {
                SaveIngredients(ReadFile.GetIngredients());
            }

            if (!Foods.Any())
            {
                SaveFoods(ReadFile.GetFoods());
            }

            if (!Customers.Any())
            {
                SaveCustomers(ReadFile.GetCustomers());
            }

            if (!IngredientProperties.Any())
            {
                IngredientProperties.AddRange(ReadFile.GetIngredientsProperties(Ingredients.ToList()));
                SetPropertiesIngredientsId();
            }

            if (!Allergies.Any())
            {
                SaveAllergies(ReadFile.GetCustomers());
                SetCustomersAllergies();
            }

            if (!FoodComponents.Any() || !IngredientComponents.Any())
            {
                SaveRecipeItems(ReadFile.GetFoodsWithRecipes(Foods.ToList(), Ingredients.ToList()));
                SetRecipes();
            }

            SaveChanges();
        }

        public Dictionary<Food, int> GetFoodsAmount()
        {
            Dictionary<Food, int> foodsAmount = new Dictionary<Food, int>();
            Foods.ToList().ForEach(f=>foodsAmount.Add(f,0));
            return foodsAmount;
        }

        public void SetPropertiesIngredientsId()
        {
            foreach (IngredientProperties ingredientProperties in IngredientProperties)
            {
                if (object.Equals(ingredientProperties.Ingredient, null)) continue;
                ingredientProperties.IngredientId = ingredientProperties.Ingredient.Id;
            }
            SaveChanges();
        }
        public double GetBudget()
        {
            return Budgets.First().Balance;
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

        public Dictionary<Ingredient, int> GetIngredientsPrice()
        {
            Dictionary<Ingredient, int> IngredientPrice = new Dictionary<Ingredient, int>();
            foreach (IngredientProperties ingredientProperties in IngredientProperties)
            {
                IngredientPrice.Add(Ingredients.Where(i => i.Id == ingredientProperties.IngredientId).FirstOrDefault(), ingredientProperties.Price);
            }
            foreach (var ingredient in Ingredients)
            {
                try
                {
                    _ = IngredientPrice[ingredient];
                }
                catch (System.Collections.Generic.KeyNotFoundException)
                {
                    IngredientPrice.Add(ingredient, 0);
                }
            }
            return IngredientPrice;
        }

        public List<Food> GetFoods()
        {
            return Foods.ToList();
        }
        public void SetRecipes()
        {
            List<FoodComponent> foodComponentsCopy = FoodComponents.ToList();
            var groupFoodsComponents = foodComponentsCopy.GroupBy(x => x.RecipeId);

            foreach (var item in groupFoodsComponents)
            {
                foreach (var value in item)
                {
                    Foods.Where(x => x.Id == item.Key).First().RecipeFoods.AddRange(Foods.Where(f => f.Id == value.FoodId));
                }
            }

            List<IngredientComponent> ingredientComponentsCopy = IngredientComponents.ToList();
            var groupIngredientsComponents = ingredientComponentsCopy.GroupBy(x => x.RecipeId);

            foreach (var item in groupIngredientsComponents)
            {
                foreach (var value in item)
                {
                    Foods.Where(x => x.Id == item.Key).First().RecipeIngredients.AddRange(Ingredients.Where(i => i.Id == value.IngredientId));
                }
            }
        }
        public List<Customer> GetCustomers()
        {
            return Customers.ToList();
        }
        public void SetCustomersAllergies()
        {
            List<Allergy> allergies = Allergies.ToList();
            var groupAllergies = allergies.GroupBy(x => x.CustomerId);

            foreach (var item in groupAllergies)
            {
                foreach (var value in item)
                {
                    Customers.Where(c => c.Id == item.Key).First().Allergies.AddRange(Ingredients.Where(i => i.Id == value.IngredientId));
                }
            }
        }

        public void SaveAll(List<Ingredient> _Ingredients, Dictionary<Ingredient, int> _IngredientsAmount, Dictionary<Ingredient, int> _IngredientsPrice, List<Food> _Recipes, List<Customer> _Customers, double _Budget)
        {
            SaveIngredients(_Ingredients);
            SaveIngredientsProperties(_IngredientsAmount, _IngredientsPrice);
            SaveFoods(_Recipes);
            SaveRecipeItems(_Recipes);
            SaveCustomers(_Customers);
            SaveAllergies(_Customers);
            SaveBudget(_Budget);
            SaveChanges();
        }
        public void SaveIngredients(List<Ingredient> _Ingredients)
        {
            foreach (Ingredient _ingredient in _Ingredients)
            {
                AddWithoutDuplicate(_ingredient);
            }
            SaveChanges();
        }
        public void SaveBudget(double _Budget)
        {
            Budgets.First().Balance = _Budget;
            SaveChanges();
        }
        public void SaveIngredientsProperties(Dictionary<Ingredient, int> _IngredientsAmount, Dictionary<Ingredient, int> _IngredientsPrice)
        {
            foreach (Ingredient _ingredient in Ingredients)
            {
                IngredientProperties ing = new IngredientProperties(_ingredient, _IngredientsAmount[_ingredient], _IngredientsPrice[_ingredient]);
                ing.IngredientId = Ingredients.Where(i => i.Name == _ingredient.Name).FirstOrDefault().Id;
                AddWithoutDuplicate(ing);
            }
            SaveChanges();

            foreach (Ingredient _ingredient in Ingredients)
            {
                try
                {
                    IngredientProperties ing = new IngredientProperties(_ingredient, _IngredientsAmount[_ingredient], _IngredientsPrice[_ingredient]);
                    ing.IngredientId = Ingredients.Where(i => i.Name == _ingredient.Name).FirstOrDefault().Id;
                    IngredientProperties.Where(ip => ip.IngredientId == ing.IngredientId).FirstOrDefault().Count = ing.Count;
                    IngredientProperties.Where(ip => ip.IngredientId == ing.IngredientId).FirstOrDefault().Price = ing.Price;
                }
                catch (System.Collections.Generic.KeyNotFoundException) { }
            }

            SaveChanges();
        }

        public void SaveFoods(List<Food> _Foods)
        {
            foreach (Food _recipe in _Foods)
            {
                if (!Foods.Any(r => r.Name == _recipe.Name))
                {
                    AddWithoutDuplicate(_recipe);
                }
            }
            SaveChanges();
        }

        public void SaveRecipeItems(List<Food> _Recipes)
        {
            foreach (Food food in _Recipes)
            {
                SaveRecipeItems(_Recipes, food);
            }
            SaveChanges();
        }

        private void SaveRecipeItems(List<Food> _Recipes, Food food)
        {
            foreach (Food recipeFood in food.RecipeFoods)
            {
                if (!FoodComponents.Any(fc => fc.RecipeId == food.Id && fc.FoodId == recipeFood.Id))
                {
                    FoodComponents.Add(new FoodComponent(food, _Recipes.Where(r => r.Name == recipeFood.Name).FirstOrDefault()));
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

        public void SaveCustomers(List<Customer> _Customers)
        {
            foreach (Customer _customer in _Customers)
            {
                AddWithoutDuplicate(_customer);
                Customers.Where(c => c.Name == _customer.Name).FirstOrDefault().budget = _customer.budget;
            }
        }

        public void SaveAllergies(List<Customer> _Customers)
        {
            foreach (Customer _customer in _Customers)
            {
                Customer customer = Customers.Where(c => c.Name == _customer.Name).FirstOrDefault();
                foreach (Ingredient ingredient in _customer.Allergies)
                {
                    Ingredient allergyIngredient = Ingredients.Where(i => i.Name == ingredient.Name).FirstOrDefault();
                    if (object.Equals(allergyIngredient, null))
                    {
                        Ingredients.Add(ingredient);
                        SaveChanges();
                        allergyIngredient = ingredient;
                    }
                    AddWithoutDuplicate(new Allergy(customer, allergyIngredient));
                }
            }
        }

        public void AddWithoutDuplicate(IngredientProperties _ingredientProperties)
        {
            if (IngredientProperties.Any(ip => ip.IngredientId == _ingredientProperties.IngredientId))
            {
                return;
            }
            IngredientProperties.Add(_ingredientProperties);
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
            if (Foods.Any(foodRead => foodRead.Name == food.Name))
            {
                return;
            }
            Foods.Add(food);
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

        public void AddWithoutDuplicate(Allergy allergy)
        {
            allergy.IngredientId = Ingredients.Where(i => i.Name == allergy.Ingredient.Name).FirstOrDefault().Id;
            allergy.CustomerId = Customers.Where(c => c.Name == allergy.Customer.Name).FirstOrDefault().Id;

            if (allergy.CustomerId == 0 || allergy.IngredientId == 0)
            {
                return;
            }

            if (!Allergies.Any(a => a.CustomerId == allergy.CustomerId && a.IngredientId == allergy.IngredientId))
            {
                Allergies.Add(allergy);
                SaveChanges();
            }
        }
    }
}