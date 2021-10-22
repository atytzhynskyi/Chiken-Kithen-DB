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
        public RecipeBook RecipeBook;
        public Dictionary<Food, int> FoodAmount { get; set; } = new Dictionary<Food, int>();
        public Kitchen() { }
        public Kitchen(Storage _Storage, RecipeBook _Recipes)
        {
            Storage = _Storage;
            RecipeBook = _Recipes;
            foreach (Food food in RecipeBook.Recipes)
            {
                FoodAmount.Add(food, 0);
            }
        }
        public void Cook(Food order)
        {
            if (!isEnoughIngredients(order))
            {
                return;
            }

            foreach (Food recipe in RecipeBook.Recipes)
            {
                if (recipe.Name == order.Name)
                {
                    order.RecipeIngredients = recipe.RecipeIngredients;
                    order.RecipeFoods = recipe.RecipeFoods;
                }
            }

            foreach (var ingredient in from Ingredient ingredient in Storage.Ingredients
                                       from Ingredient ingredientRecipe in order.RecipeIngredients
                                       where ingredient.Name == ingredientRecipe.Name
                                       select ingredient)
            {
                Storage.IngredientsAmount[ingredient] -= 1;
            }

            foreach (var food in from Food food in RecipeBook.Recipes
                                 from Food RecipeFood in order.RecipeFoods
                                 where food.Name == RecipeFood.Name
                                 select food)
            {
                Cook(food);
                FoodAmount[food]--;
            }

            foreach (Food food in RecipeBook.Recipes)
            {
                if (food.Name == order.Name)
                {
                    FoodAmount[food]++;
                    return;
                }
            }
        }
        public bool isEnoughIngredients(Food food)
        {
            Dictionary<Ingredient, int> ingredientAmountsCopy = new Dictionary<Ingredient, int>();
            foreach(var ingredientAmount in Storage.IngredientsAmount)
            {
                ingredientAmountsCopy.Add(ingredientAmount.Key, ingredientAmount.Value);
            }
            foreach (Ingredient ingredientRecipe in GetBaseIngredientRecipe(food))
            {
                foreach (Ingredient ingredient in Storage.Ingredients)
                {
                    if (ingredient.Name == ingredientRecipe.Name)
                    {
                        ingredientAmountsCopy[ingredient]--;
                        if (ingredientAmountsCopy[ingredient] < 0)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        private List<Ingredient> GetBaseIngredientRecipe(Food food)
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
            foreach (Food food in RecipeBook.Recipes)
            {
                Console.WriteLine(food.Name);
            }
        }
    }
}
