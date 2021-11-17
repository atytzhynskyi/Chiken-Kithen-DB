﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using BaseClasses;

namespace ChikenKithen
{
    public class Kitchen
    {
        public Storage Storage;
        public int Budget { get; private set; }
        public Kitchen() { }
        public Kitchen(Storage _Storage, int _Budget)
        {
            Storage = _Storage;
            Budget = _Budget;
        }
        public int CalculateFoodCostPrice(Food food)
        {
            int price = 0;
            food = Storage.GetRecipeByName(food.Name);
            foreach(Food foodRecipe in food.RecipeFoods)
            {
                price += CalculateFoodCostPrice(foodRecipe);
            }
            foreach(Ingredient ingredient in food.RecipeIngredients)
            {
                price += Storage.IngredientsPrice[Storage.Ingredients.Where(i => i.Name == ingredient.Name).First()];
            }
            return price;
        }
        public int CalculateFoodMenuPrice(Food food)
        {
            return Convert.ToInt32(CalculateFoodCostPrice(food)*1.3);
        }
        public bool Cook(Food order)
        {
            if (!IsEnoughIngredients(order))
            {
                return false;
            }

            if(!Storage.Recipes.Any(r=>r.Name == order.Name))
            {
                return false;
            }

            order = Storage.Recipes.Where(r => r.Name == order.Name).First();
            
            foreach (var ingredient in from Ingredient ingredient in Storage.Ingredients
                                       from Ingredient ingredientRecipe in order.RecipeIngredients
                                       where ingredient.Name == ingredientRecipe.Name
                                       select ingredient)
            {
                Storage.IngredientsAmount[ingredient]--;
            }

            foreach (var food in from Food food in Storage.Recipes
                                 from Food RecipeFood in order.RecipeFoods
                                 where food.Name == RecipeFood.Name
                                 select food)
            {
                Cook(food);
            }

            return true;
        }

        public bool IsEnoughIngredients(Food food)
        {
            var ingredients = GetBaseIngredientRecipe(food);

            foreach(Food foodRecipe in food.RecipeFoods)
            {
                if (Storage.FoodAmount[Storage.GetRecipeByName(foodRecipe.Name)] > 0)
                {
                    foreach(Ingredient ingredient in foodRecipe.RecipeIngredients)
                    {
                        ingredients.Remove(ingredient);
                    }
                }
            }

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
            foreach (Food food in Storage.Recipes)
            {
                Console.WriteLine(food.Name);
            }
        }
        public void UseMoney(int amount)
        {
            Budget -= amount;
        }
        public void AddMoney(int amount)
        {
            Budget += amount;
        }
        public void SetMoney(int amount)
        {
            Budget = amount;
        }
    }
}
