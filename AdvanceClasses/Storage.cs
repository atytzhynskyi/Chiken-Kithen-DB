using BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdvanceClasses
{
    public class Storage
    {
        public List<Ingredient> Ingredients { get; set; }
        public Dictionary<Ingredient, int> IngredientsAmount { get; set; } = new Dictionary<Ingredient, int>();

        public List<Food> Recipes;
        public Dictionary<Food, int> FoodAmount { get; set; } = new Dictionary<Food, int>();

        private readonly int maxIngredientType;
        private readonly int maxFoodType;
        private readonly int totalMax;

        private readonly double _spoilRate;

        public Storage(List<Food> _Foods, List<Ingredient> _Ingredients, Dictionary<Food, int> _FoodAmount,
            Dictionary<Ingredient, int> _IngredientsAmount, int _maxIngredientType, int _maxFoodType, int _totalMax, double spoilRate)
        {
            totalMax = _totalMax;
            maxIngredientType = _maxIngredientType;
            maxFoodType = _maxFoodType;

            Ingredients = _Ingredients;
            IngredientsAmount = _IngredientsAmount;
            Recipes = _Foods;
            FoodAmount = _FoodAmount;

            _spoilRate = spoilRate;
        }

        public Storage(List<Ingredient> _Ingredients, int _maxIngredientType, int _totalMax)
        {
            totalMax = _totalMax;
            maxIngredientType = _maxIngredientType;
            maxFoodType = int.MaxValue;

            Ingredients = _Ingredients;
            Recipes = new List<Food>();
            FillDictionaryByZero<Ingredient>(Ingredients, IngredientsAmount);
        }
        public Storage(List<Food> recipes, List<Ingredient> _Ingredients)
        {
            totalMax = int.MaxValue;
            maxIngredientType = int.MaxValue;
            maxFoodType = int.MaxValue;

            Ingredients = _Ingredients;
            Recipes = recipes;
            FillDictionaryByZero<Ingredient>(Ingredients, IngredientsAmount);

            FillDictionaryByZero<Food>(Recipes, FoodAmount);
        }

        private void FillDictionaryByZero<T>(List<T> list, Dictionary<T, int> dict)
        {
            foreach(var item in list)
            {
                dict.Add(item,0);
            }
        }
        public void AddIngredientAmount(string ingredientName, int amount)
        {
            var spoil = GetNumberOfSpoil(amount, _spoilRate);
            amount -= spoil;

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
                amount -= ingredientAmount + amount - maxIngredientType;
            }
            if (wasted != 0)
            {
                Console.WriteLine($"Wasted: {ingredientName}, amount: {wasted}");
            }

            if (spoil != 0)
            {
                Console.WriteLine($"Spoil: {ingredientName}, amount: {spoil}");
            }

            IngredientsAmount[ingredient] += amount;
        }

        public void AddFoodAmount(string foodName, int amount)
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
                amount -= foodAmount + amount - maxFoodType;
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

        public Food GetRecipe(string Name)
        {
            return Recipes.Find(r => r.Name == Name);
        }
        public Ingredient GetIngredient(string name)
        {
            return Ingredients.Find(i => i.Name == name);
        }
        public void ShowIngredients()
        {
            Console.WriteLine("Ingredients List:");
            foreach (Ingredient ingredient in IngredientsAmount.Keys)
            {
                if (IngredientsAmount[ingredient] == 0) continue;
                Console.Write(ingredient.Name + " ");
                Console.Write(IngredientsAmount[ingredient] + "\n");
            }
        }

        public int GetNumberOfSpoil(int amount)
        {
            return GetNumberOfSpoil(amount, _spoilRate);
        }

        private int GetNumberOfSpoil(int amount, double spoilRate)
        {
            if (spoilRate <= 0)
            {
                return 0;
            }

            if (spoilRate >= 100)
            {
                return amount;
            }

            //Using 100, we get the most accurate value
            var koef = 100;
            var maxNumbers = 100 * koef;
            var maxNumberSpoil = (int)Math.Truncate(spoilRate * koef);

            var spoil = 0;
            for (int i = 1; i <= amount; i++)
            {
                Random rnd = new Random();
                int value = rnd.Next(1, maxNumbers);

                if (value <= maxNumberSpoil)
                {
                    spoil += 1;
                }
            }

            return spoil;
        }

    }
}
