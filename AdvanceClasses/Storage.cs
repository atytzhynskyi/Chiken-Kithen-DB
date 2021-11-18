using BaseClasses;
using jsonReadModule;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace AdvanceClasses
{
    public class Storage
    {
        public List<Ingredient> Ingredients { get; set; }
        public Dictionary<Ingredient, int> IngredientsAmount { get; set; } = new Dictionary<Ingredient, int>();

        public Dictionary<Ingredient, int> IngredientsPrice { get; set; } = new Dictionary<Ingredient, int>();

        public List<Food> Recipes;
        public Dictionary<Food, int> FoodAmount { get; set; } = new Dictionary<Food, int>();

        private readonly int maxIngredientType;
        private readonly int maxFoodType;
        private readonly int totalMax;

        public Storage(List<Food> _Foods, List<Ingredient> _Ingredients, Dictionary<Food, int> _FoodAmount,
            Dictionary<Ingredient, int> _IngredientsAmount, Dictionary<Ingredient, int> _IngredientsPrice,
            int _maxIngredientType, int _maxFoodType, int _totalMax)
        {
            totalMax = _totalMax;
            maxIngredientType = _maxIngredientType;
            maxFoodType = _maxFoodType;

            Ingredients = _Ingredients;
            IngredientsAmount = _IngredientsAmount;
            IngredientsPrice = _IngredientsPrice;
            Recipes = _Foods;
            FoodAmount = _FoodAmount;
        }
        public Storage(List<Food> _Foods, List<Ingredient> _Ingredients, 
            Dictionary<Ingredient, int> _IngredientsAmount, Dictionary<Ingredient, int> _IngredientsPrice, 
            int _maxIngredientType, int _maxFoodType, int _totalMax)
        {
            totalMax = _totalMax;
            maxIngredientType = _maxIngredientType;
            maxFoodType = _maxFoodType;

            Ingredients = _Ingredients;
            IngredientsAmount = _IngredientsAmount;
            IngredientsPrice = _IngredientsPrice;
            Recipes = _Foods;

            FillDictionaryByZero<Food>(Recipes, FoodAmount);
        }

        public Storage(List<Food> _Foods, List<Ingredient> _Ingredients, 
            int _maxIngredientType, int _maxFoodType, int _totalMax)
        {
            totalMax = _totalMax;
            maxIngredientType = _maxIngredientType;
            maxFoodType = _maxFoodType;

            Ingredients = _Ingredients;
            Recipes = _Foods;

            FillDictionaryByZero<Food>(Recipes, FoodAmount);
            FillDictionaryByZero<Ingredient>(Ingredients, IngredientsPrice);
            FillDictionaryByZero<Ingredient>(Ingredients, IngredientsAmount);
        }

        public Storage(List<Ingredient> _Ingredients, Dictionary<Ingredient, int> _IngredientsAmount, int _maxIngredientType, int _totalMax)
        {
            totalMax = _totalMax;
            maxIngredientType = _maxIngredientType;
            maxFoodType = int.MaxValue;

            Ingredients = _Ingredients;
            Recipes = new List<Food>();
            IngredientsAmount = _IngredientsAmount;
            FillDictionaryByZero<Ingredient>(Ingredients, IngredientsPrice);
        }

        public Storage(List<Ingredient> _Ingredients, int _maxIngredientType, int _totalMax)
        {
            totalMax = _totalMax;
            maxIngredientType = _maxIngredientType;
            maxFoodType = int.MaxValue;

            Ingredients = _Ingredients;
            Recipes = new List<Food>();
            FillDictionaryByZero<Ingredient>(Ingredients, IngredientsPrice);
            FillDictionaryByZero<Ingredient>(Ingredients, IngredientsAmount);
        }
        public Storage(List<Food> food, List<Ingredient> _Ingredients)
        {
            totalMax = int.MaxValue;
            maxIngredientType = int.MaxValue;
            maxFoodType = int.MaxValue;

            Ingredients = _Ingredients;
            Recipes = food;
            FillDictionaryByZero<Ingredient>(Ingredients, IngredientsPrice);
            FillDictionaryByZero<Ingredient>(Ingredients, IngredientsAmount);

            FillDictionaryByZero<Food>(Recipes, FoodAmount);
        }

        public Storage(List<Ingredient> _Ingredients)
        {
            totalMax = int.MaxValue;
            maxIngredientType = int.MaxValue;
            maxFoodType = int.MaxValue;

            Ingredients = _Ingredients;
            Recipes = new List<Food>();
            FillDictionaryByZero<Ingredient>(Ingredients, IngredientsPrice);
            FillDictionaryByZero<Ingredient>(Ingredients, IngredientsAmount);
        }



        private void FillDictionaryByZero<T>(List<T> list, Dictionary<T, int> dict)
        {
            foreach(var item in list)
            {
                dict.Add(item,0);
            }
        }
        public void AddIngredient(string ingredientName, int amount)
        {
            var ingredient = Ingredients.Where(x => x.Name == ingredientName).FirstOrDefault();
            int newTotalAmount = GetTotalAmount() + amount;
            int ingredientAmount = IngredientsAmount[ingredient];
            int wasted = 0;

            if (newTotalAmount > totalMax)
            {
                wasted = newTotalAmount - totalMax;
                amount -= wasted;
            }
            if (ingredientAmount + amount > maxIngredientType)
            {
                wasted += ingredientAmount + amount - maxIngredientType;
                amount -= wasted;
            }
            if(wasted != 0)
            {
                Console.WriteLine($"Wasted: {ingredientName}, amount: {wasted}");
            }
            IngredientsAmount[ingredient] += amount;
        }
        public void AddFood(string foodName, int amount)
        {
            var food = Recipes.Where(x => x.Name == foodName).FirstOrDefault();
            int newTotalAmount = GetTotalAmount() + amount;
            int foodAmount = FoodAmount[food];
            int wasted = 0;

            if (newTotalAmount > totalMax)
            {
                wasted = newTotalAmount - totalMax;
                amount -= wasted;
            }
            if (foodAmount + amount > maxFoodType)
            {
                wasted += foodAmount + amount - maxFoodType;
                amount -= wasted;
            }
            if(wasted != 0)
            {
                Console.WriteLine($"Wasted: {foodName}, amount: {wasted}");
            }

            FoodAmount[food] += amount;
        }
        private int GetTotalAmount()
        {
            return IngredientsAmount.Values.Sum() + FoodAmount.Values.Sum();
        }

        public Food GetRecipeByName(string Name)
        {
            return Recipes.Find(r => r.Name == Name);
        }
        public Ingredient GetIngredient(string name)
        {
            return Ingredients.Find(i => i.Name == name);
        }
        public void AddNewIngredient()
        {
            Ingredient ingredient = new Ingredient();
            Console.WriteLine("What is name of this ingredient?");
            ingredient.Name = Console.ReadLine();
            Console.WriteLine("How much do you want?");
            int count = Convert.ToInt32(Console.ReadLine());
            if (!Ingredients.Any(i => i.Name == ingredient.Name))
            {
                IngredientsAmount.Add(ingredient, count);
            }
            else IngredientsAmount[Ingredients.Find(i => i.Name == ingredient.Name)] = count;
            Ingredients.Add(ingredient);
            return;
        }
        public void AddNewIngredient(Ingredient ingredient, int amount)
        {
            Ingredients.Add(ingredient);
            IngredientsAmount.Add(ingredient, amount);
        }
        public void RewriteIngredientCount(int ingredientAmount, string chengeIngredientName)
        {
            foreach (var ingredient in from Ingredient ingredient in Ingredients
                                       where ingredient.Name == chengeIngredientName
                                       select ingredient)
            {
                IngredientsAmount[ingredient] = ingredientAmount;
                return;
            }
        }
        public void DeleteIngredient(string _ingredientName)
        {
            foreach (Ingredient ingredient in Ingredients)
            {
                if (ingredient.Name == _ingredientName)
                {
                    Ingredients.Remove(ingredient);
                    return;
                }
            }
        }
        public void ShowIngredients()
        {
            Console.WriteLine("Ingredients List:");
            foreach (Ingredient ingredient in Ingredients.ToList())
            {
                try
                {
                    _ = IngredientsAmount[ingredient];
                }
                catch (System.Collections.Generic.KeyNotFoundException)
                {
                    continue;
                }
                if (IngredientsAmount[ingredient] == 0) continue;
                Console.Write(ingredient.Name + " ");
                Console.Write(IngredientsAmount[ingredient] + " ");
                Console.Write(IngredientsPrice[ingredient] + "\n");
            }
        }
        public void Clear()
        {
            foreach (Ingredient ingredient in Ingredients)
            {
                Ingredients.Remove(ingredient);
            }
        }
    }
}
