using System;
using System.Collections.Generic;
using System.Linq;
using BaseClasses;

namespace AdvanceClasses
{
    public class Kitchen
    {
        public Storage Storage;

        public Kitchen(Storage storage)
        {
            this.Storage = storage;
        }

        public bool Cook(Food order)
        {
            if (!IsEnoughIngredients(order))
            {
                return false;
            }

            //if recipe doesnt exist in recipes return false
            if(!Storage.Recipes.Any(r=>r.Name == order.Name))
            {
                return false;
            }

            //set order from recipes because orders recipe can be empty
            order = Storage.Recipes.Where(r => r.Name == order.Name).First();
            
            foreach (var ingredient in from Ingredient ingredient in Storage.Ingredients
                                       from Ingredient ingredientRecipe in order.RecipeIngredients
                                       where ingredient.Name == ingredientRecipe.Name
                                       select ingredient)
            {
                Storage.IngredientsAmount[ingredient]--;
            }

            foreach(var recipeFood in order.RecipeFoods)
            {
                if (Storage.FoodAmount[recipeFood] > 0)
                {
                    Storage.FoodAmount[recipeFood]--;
                    continue;
                }

                if (Cook(recipeFood)) continue;
                else
                    return false;
            }

            return true;
        }

        public void CheckSpoilIngredient(Food food)
        {
            if (object.Equals(food, null))
            {
                return;
            }

            if (Storage.Recipes.Any(f => f.Name == food.Name))
            {
                food = Storage.Recipes.Find(f => f.Name == food.Name);
            }

            //group ingredient because recipe can contain several ingredients of one type
            var groupIngredients = food.RecipeIngredients.GroupBy(x => x);
            foreach (var item in groupIngredients)
            {
                var spoil = Storage.GetNumberOfSpoil(Storage.IngredientsAmount[item.Key]);
                Storage.IngredientsAmount[item.Key] -= spoil;

                if (spoil != 0)
                {
                    Storage.IngredientsTrashAmount[item.Key] += spoil;
                    Console.WriteLine($"Spoil: {item.Key.Name}, amount: {spoil}");
                }
            }

            //same for food
            var groupFoods = food.RecipeFoods.GroupBy(x => x);
            foreach (var item in groupFoods)
            {
                if (Storage.FoodAmount[item.Key] < item.Count())
                {
                    CheckSpoilIngredient(item.Key);
                }
            }

            return;
        }

        public bool IsEnoughIngredients(Food food)
        {
            if(object.Equals(food, null))
            {
                return false;
            }

            if(Storage.Recipes.Any(f=>f.Name == food.Name)){
                food = Storage.Recipes.Find(f => f.Name == food.Name);
            }

            //group ingredient because recipe can contain several ingredients of one type
            var groupIngredients = food.RecipeIngredients.GroupBy(x => x);
            foreach (var item in groupIngredients){
                if (Storage.IngredientsAmount[item.Key] < item.Count()){
                    return false;
                }
            }

            //same for food
            var groupFoods = food.RecipeFoods.GroupBy(x => x);
            foreach (var item in groupFoods) {
                if (Storage.FoodAmount[item.Key] < item.Count()){
                    //if amout of foods in storage doesnt enough then form new recipe with lower-level food 
                    // "Chicken with seasoning" => Chicken + season => Chicken + salt + peaper
                    //its reason why recipe can contain several ingredients or foods of one type 
                    List<Food> foodsForChecking = food.RecipeFoods.Where(x => true).ToList();
                    List<Ingredient> ingredientsForChecking = food.RecipeIngredients.Where(x => true).ToList();

                    foodsForChecking.Remove(item.Key);
                    foodsForChecking.AddRange(Storage.GetRecipe(item.Key.Name).RecipeFoods);

                    ingredientsForChecking.AddRange(item.Key.RecipeIngredients);

                    return IsEnoughIngredients(new Food("", ingredientsForChecking, foodsForChecking));
                }
            }

            return true;
        }
    }
}
