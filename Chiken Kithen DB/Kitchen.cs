using System;
using System.Collections.Generic;
using System.Text;

namespace Chiken_Kithen_DB
{
    class Kitchen
    {
        public Storage Storage;
        public RecipeBook RecipeBook;
        public Dictionary<Food, int> FoodAmount { get; set;} = new Dictionary<Food, int>();
        public Kitchen()
        {
        }
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
            foreach (Food recipe in RecipeBook.Recipes)
            {
                if (recipe.Name == order.Name)
                    order.RecipeItems = recipe.RecipeItems;
            }

            if (!isEnoughIngredients(order))
            {
                Console.WriteLine("We dont have enough ingredients");
                return;
            }
            order.RecipeItems = GetBaseIngredientRecipe(order.RecipeItems);
            foreach (Ingredient ingredient in Storage.Ingredients)
            {
                foreach (RecipeItem ingredientRecipe in order.RecipeItems)
                {
                    if (ingredient.Name == ingredientRecipe.Ingredient.Name)
                    {
                        Storage.IngredientsAmount[ingredient] -= 1;
                    }
                }
            }
            foreach (Food food in RecipeBook.Recipes)
            {
                if(food.Name == order.Name)
                    FoodAmount[food]++;
            }
            return;
        }
        public bool isEnoughIngredients(Food food)
        {
            List<RecipeItem>fullRecipe = GetBaseIngredientRecipe(food.RecipeItems);
            Dictionary<Ingredient, int> ingredientAmountsCopy = new Dictionary<Ingredient, int>();

            foreach(Ingredient ingredient in Storage.Ingredients)
            {
                ingredientAmountsCopy.Add(ingredient, Storage.IngredientsAmount[ingredient]);
            }

            foreach (RecipeItem ingredientRecipe in fullRecipe)
            {
                foreach (Ingredient ingredient in Storage.Ingredients)
                {
                    if (ingredient.Name == ingredientRecipe.Ingredient.Name)
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
                        fullRecipe.AddRange(GetBaseIngredientRecipe(food.RecipeItems));
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
