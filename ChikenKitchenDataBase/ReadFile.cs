using BaseClasses;
using CsvHelper;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ChikenKitchenDataBase
{
    static class ReadFile
    {
        static public List<Ingredient> GetIngredients()
        {
            using var streamReader = File.OpenText(@"..\..\..\Configs\Ingredients.csv");
            using var csvReader = new CsvReader(streamReader, CultureInfo.CurrentCulture);

            List<Ingredient> ingredients = new List<Ingredient>();
            string name;
            int amount;
            while (csvReader.Read())
            {
                csvReader.TryGetField<string>(1, out name);
                if (!int.TryParse(name, out amount))
                    continue;

                csvReader.TryGetField<string>(0, out name);
                if (string.IsNullOrEmpty(name)) continue;

                ingredients.Add(new Ingredient(name));
            }
            return ingredients;
        }
        static public List<IngredientProperties> GetIngredientsProperties(List<Ingredient> ingredients)
        {
            using var streamReader = File.OpenText(@"..\..\..\Configs\Ingredients.csv");
            using var csvReader = new CsvReader(streamReader, CultureInfo.CurrentCulture);

            List<IngredientProperties> ingredientsProperties = new List<IngredientProperties>();
            Dictionary<Ingredient, int> IngredientAmount = new Dictionary<Ingredient, int>();
            Dictionary<Ingredient, int> IngredientPrice = new Dictionary<Ingredient, int>();

            string name;
            int amount;
            int price;

            while (csvReader.Read())
            {
                csvReader.TryGetField<string>(1, out name);
                if (!int.TryParse(name, out amount))
                    continue;

                csvReader.TryGetField<string>(2, out name);
                if (!int.TryParse(name, out price))
                    continue;

                csvReader.TryGetField<string>(0, out name);
                if (string.IsNullOrEmpty(name)) continue;

                Ingredient ingredient = ingredients.Where(i => i.Name == name).FirstOrDefault();

                ingredientsProperties.Add(new IngredientProperties(ingredient, amount, price, 0));
            }
            return ingredientsProperties;
        }

        static public List<Food> GetFoods()
        {
            List<Food> foods = new List<Food>();

            using var streamReader = File.OpenText(@"..\..\..\Configs\Foods.csv");
            using var csvReader = new CsvReader(streamReader, CultureInfo.CurrentCulture);

            string name;
            while (csvReader.Read())
            {
                List<string> fileLine = new List<string>();
                csvReader.TryGetField<string>(0, out name);
                if (string.IsNullOrEmpty(name)) continue;
                
                foods.Add(new Food(name));
            }

            return foods;
        }
        static public List<Food> GetFoodsWithRecipes(List<Food> foods, List<Ingredient> ingredients)
        {
            using var streamReader = File.OpenText(@"..\..\..\Configs\Foods.csv");
            using var csvReader = new CsvReader(streamReader, CultureInfo.CurrentCulture);

            string name;
            while (csvReader.Read())
            {
                List<string> fileLine = new List<string>();
                csvReader.TryGetField<string>(0, out name);

                if (string.IsNullOrEmpty(name)) continue;

                Food food = foods.Where(f => f.Name == name).FirstOrDefault();
                for (int i = 1; csvReader.TryGetField<string>(i, out name); i++)
                {
                    if (string.IsNullOrEmpty(name)) continue;

                    if (foods.Any(r => r.Name == name))
                    {
                        if (food.RecipeFoods.Any(r => r.Name == name)) continue;
                        food.RecipeFoods.Add(foods.Where(r => r.Name == name).FirstOrDefault());
                        continue;
                    }
                    if (ingredients.Any(i => i.Name == name))
                    {
                        if (food.RecipeIngredients.Any(r => r.Name == name)) continue;
                        food.RecipeIngredients.Add(ingredients.Where(i => i.Name == name).FirstOrDefault());
                        continue;
                    }
                }
            }
            return foods;
        }

        static public List<Customer> GetCustomers()
        {
            List<Customer> customers = new List<Customer>();
            using var streamReader = File.OpenText(@"..\..\..\Configs\Customers.csv");
            using var csv = new CsvReader(streamReader, CultureInfo.CurrentCulture);

            while (csv.Read())
            {
                Customer customer = new Customer(csv.GetField(1));
                csv.Read();
                csv.TryGetField<int>(1, out int budget);
                customer.budget = budget;
                csv.Read();
                for (int i = 1; csv.TryGetField<string>(i, out string ingredientName); i++)
                {
                    if (string.IsNullOrEmpty(ingredientName)) break;
                    customer.Allergies.Add(new Ingredient(ingredientName));
                }
                customers.Add(customer);
            }

            return customers;
        }
    }
}
