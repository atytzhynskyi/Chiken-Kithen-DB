using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using BaseClasses;

namespace ChikenKithen
{
    public class Kitchen
    {
        public Storage Storage;
        public List<Food> Recipes;
        public int Budget;
        public Dictionary<Food, int> FoodAmount { get; set; } = new Dictionary<Food, int>();
        public Kitchen() { }
        public Kitchen(Storage _Storage, List<Food> _Recipes, int _Budget)
        {
            Storage = _Storage;
            Recipes = _Recipes;
            foreach (Food food in Recipes)
            {
                FoodAmount.Add(food, 0);
            }
            Budget = _Budget;
        }
        public Food GetRecipeByName(string Name)
        {
            return Recipes.Where(r=>r.Name == Name).FirstOrDefault();
        }
        public int CalculateFoodCostPrice(Food food)
        {
            int price = 0;
            food = GetRecipeByName(food.Name);
            foreach(Food foodRecipe in food.RecipeFoods)
            {
                price += CalculateFoodCostPrice(foodRecipe);
            }
            foreach(Ingredient ingredient in food.RecipeIngredients)
            {
                price += Storage.IngredientsPrice[Storage.Ingredients.Where(i => i.Name == ingredient.Name).First()];
            }
            return Convert.ToInt32(price*1.3);
        }
        public int CalculateFoodMenuPrice(Food food)
        {
            return Convert.ToInt32(CalculateFoodCostPrice(food));
        }
        public void Cook(Food order)
        {
            if (!IsEnoughIngredients(order))
            {
                return;
            }

            if(!Recipes.Any(r=>r.Name == order.Name))
            {
                return;
            }

            order = Recipes.Where(r => r.Name == order.Name).First();
            
            foreach (var ingredient in from Ingredient ingredient in Storage.Ingredients
                                       from Ingredient ingredientRecipe in order.RecipeIngredients
                                       where ingredient.Name == ingredientRecipe.Name
                                       select ingredient)
            {
                Storage.IngredientsAmount[ingredient] -= 1;
            }

            foreach (var food in from Food food in Recipes
                                 from Food RecipeFood in order.RecipeFoods
                                 where food.Name == RecipeFood.Name
                                 select food)
            {
                Cook(food);
                FoodAmount[food]--;
            }

            FoodAmount[order]++;
        }

        public bool IsEnoughIngredients(Food food)
        {
            var ingredients = GetBaseIngredientRecipe(food);

            var usedIngredientsAmount = ingredients.GroupBy(x => x);

            foreach(var item in usedIngredientsAmount)
            {
                if (Storage.IngredientsAmount[item.Key] >= item.Count())
                    continue;
                else
                    return false;
            }

            return true;
        }

        public List<Ingredient> GetBaseIngredientRecipe(Food food)
        {
            List<Ingredient> fullRecipe = new List<Ingredient>();

            fullRecipe.AddRange(food.RecipeIngredients);
            
            foreach (Food recipeFood in food.RecipeFoods)
                fullRecipe.AddRange(GetBaseIngredientRecipe(recipeFood));

            return fullRecipe;
        }
        public void ShowAll()
        {
            foreach (Ingredient ingredient in Storage.Ingredients)
            {
                Console.WriteLine(ingredient.Name + " " + Storage.IngredientsAmount[ingredient]);
            }
            foreach (Food food in Recipes)
            {
                Console.WriteLine(food.Name);
            }
        }
    }
}
