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
                Console.WriteLine("aaa");
                return;
            }

            foreach (Food recipe in RecipeBook.Recipes)
            {
                if (recipe.Name == order.Name)
                    order.Recipe = recipe.Recipe;
            }

            foreach (var ingredient in from Ingredient ingredient in Storage.Ingredients
                                       from RecipeItem ingredientRecipe in order.Recipe
                                       where ingredient.Name == ingredientRecipe.Ingredient.Name
                                       where !RecipeBook.Recipes.Any(f => f.Name==ingredient.Name)
                                       select ingredient)
            {
                Storage.IngredientsAmount[ingredient] -= 1;
            }

            foreach (var food in from Food food in RecipeBook.Recipes
                                 from RecipeItem ingredient in order.Recipe
                                 where food.Name == ingredient.Ingredient.Name
                                 select food)
            {
                Cook(food);
                FoodAmount[food]--;
            }

            foreach (Food food in RecipeBook.Recipes)
            {
                if (food.Name == order.Name)
                    FoodAmount[food]++;
            }
            return;
        }
        public bool isEnoughIngredients(Food food)
        {
            List<RecipeItem> fullRecipe = new List<RecipeItem>(GetBaseIngredientRecipe(food.Recipe));
            Dictionary<Ingredient, int> ingredientAmountsCopy = new Dictionary<Ingredient, int>();
            foreach(var ingredientAmount in Storage.IngredientsAmount)
            {
                ingredientAmountsCopy.Add(ingredientAmount.Key, ingredientAmount.Value);
            }
            foreach (RecipeItem recipeItem in GetBaseIngredientRecipe(food.Recipe))
            {
                foreach (Ingredient ingredient in Storage.Ingredients)
                {
                    if (ingredient.Name == recipeItem.Ingredient.Name && !RecipeBook.Recipes.Any(f => f.Name==ingredient.Name))
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
        private List<RecipeItem> GetBaseIngredientRecipe(List<RecipeItem> Recipe)
        {
            List<RecipeItem> fullRecipe = new List<RecipeItem>();
            foreach (RecipeItem recipeItem in Recipe)
            {
                foreach (Food food in RecipeBook.Recipes)
                {
                    if (food.Name == recipeItem.Ingredient.Name)
                    {
                        fullRecipe.AddRange(GetBaseIngredientRecipe(food.Recipe));
                        break;
                    }
                }
                foreach (Ingredient baseIngredient in Storage.Ingredients)
                {
                    if (baseIngredient.Name == recipeItem.Ingredient.Name)
                    {
                        fullRecipe.Add(recipeItem);
                    }
                }
            }
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
