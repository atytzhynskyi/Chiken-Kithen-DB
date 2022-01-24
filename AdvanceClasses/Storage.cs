using BaseClasses;
using Randomizer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdvanceClasses
{
    public class Storage
    {
        public List<Ingredient> Ingredients { get; set; }
        public Dictionary<Ingredient, int> IngredientsAmount { get; set; } = new Dictionary<Ingredient, int>();
        public Dictionary<Ingredient, int> IngredientsTrashAmount { get; set; } = new Dictionary<Ingredient, int>();
        public Dictionary<Ingredient, int> TrashAmount { get; set; } = new Dictionary<Ingredient, int>();

        public List<Food> Recipes;
        public Dictionary<Food, int> FoodAmount { get; set; } = new Dictionary<Food, int>();

        private readonly int maxIngredientType;
        private readonly int maxFoodType;
        private readonly int totalMax;

        private readonly int _wasteLimit;
        private readonly double _spoilRate;

        private readonly int _maxIngredientVolatility;
        private readonly int _maxDishVolatility;

        public IRnd Randomizer { get; private set; }

        public Storage(List<Food> _Foods,
            List<Ingredient> _Ingredients,
            Dictionary<Food, int> foodsAmount,
            Dictionary<Ingredient, int> _IngredientsAmount,
            Dictionary<Ingredient, int> ingredientsTrashAmount,
            Dictionary<Ingredient, int> totalTrashAmount,
            int _maxIngredientType,
            int _maxFoodType,
            int _totalMax,
            int wasteLimit,
            double spoilRate,
            int maxIngredientVolatility,
            int maxDishVolatility,
            IRnd rnd)
        {
            totalMax = _totalMax;
            maxIngredientType = _maxIngredientType;
            maxFoodType = _maxFoodType;

            Ingredients = _Ingredients;
            IngredientsAmount = _IngredientsAmount;
            Recipes = _Foods;
            FoodAmount = foodsAmount;

            IngredientsTrashAmount = ingredientsTrashAmount;
            TrashAmount = totalTrashAmount;
            _wasteLimit = wasteLimit;
            _spoilRate = spoilRate;

            _maxIngredientVolatility = maxIngredientVolatility;
            _maxDishVolatility = maxDishVolatility;

            Randomizer = rnd;
        }

        public Storage(List<Ingredient> _Ingredients, int _maxIngredientType, int _totalMax)
        {
            totalMax = _totalMax;
            maxIngredientType = _maxIngredientType;
            maxFoodType = int.MaxValue;

            Ingredients = _Ingredients;
            Recipes = new List<Food>();
            FillDictionaryByZero<Ingredient>(Ingredients, IngredientsAmount);
            FillDictionaryByZero<Ingredient>(Ingredients, IngredientsTrashAmount);
        }
        public Storage(List<Food> recipes, List<Ingredient> _Ingredients)
        {
            totalMax = int.MaxValue;
            maxIngredientType = int.MaxValue;
            maxFoodType = int.MaxValue;

            Ingredients = _Ingredients;
            Recipes = recipes;
            FillDictionaryByZero<Ingredient>(Ingredients, IngredientsAmount);
            FillDictionaryByZero<Ingredient>(Ingredients, IngredientsTrashAmount);

            FillDictionaryByZero<Food>(Recipes, FoodAmount);
        }

        private void FillDictionaryByZero<T>(List<T> list, Dictionary<T, int> dict)
        {
            foreach (var item in list)
            {
                dict.Add(item, 0);
            }
        }
        public void AddIngredientAmount(string ingredientName, int amount)
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
                amount -= ingredientAmount + amount - maxIngredientType;
            }
            if (wasted != 0)
            {
                IngredientsTrashAmount[ingredient] += wasted;
                Console.WriteLine($"Wasted: {ingredientName}, amount: {wasted}");
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
            if (wasted != 0)
            {
                FillInTrash(food, wasted);
                Console.WriteLine($"Wasted: {foodName}, amount: {wasted}");
            }

            FoodAmount[food] += amount;
        }

        private void FillInTrash(Food food, int times)
        {
            if (object.Equals(food, null))
            {
                return;
            }

            if (Recipes.Any(f => f.Name == food.Name))
            {
                food = Recipes.Find(f => f.Name == food.Name);
            }

            foreach (var item in food.RecipeIngredients)
            {
                IngredientsTrashAmount[item] += times;
            }

            var groupFoods = food.RecipeFoods.GroupBy(x => x);
            foreach (var item in groupFoods)
            {
                FillInTrash(item.Key, times);
            }
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
            if (_spoilRate <= 0)
            {
                return 0;
            }

            //Using 100, we get the most accurate value
            var koef = 100;
            var maxNumbers = 100 * koef;
            var maxNumberSpoil = (int)Math.Truncate(_spoilRate * koef);

            var spoil = 0;
            for (int i = 1; i <= amount; i++)
            {
                Random rnd = new Random();
                int value = rnd.Next(1, maxNumbers + 1);

                if (value <= maxNumberSpoil)
                {
                    spoil += 1;
                }
            }

            return spoil;
        }

        public void RunSpoiling()
        {
            Dictionary<Ingredient, int> ingredientsAmountCopy = new Dictionary<Ingredient, int>(IngredientsAmount);
            foreach (var item in ingredientsAmountCopy)
            {
                var spoil = GetNumberOfSpoil(item.Value);

                if (spoil != 0)
                {
                    IngredientsAmount[item.Key] -= spoil;
                    IngredientsTrashAmount[item.Key] += spoil;
                    Console.WriteLine($"Spoil: {item.Key.Name}, amount: {spoil}");
                }
            }

        }

        public bool IsRestaurantPoisoned()
        {
            if (_wasteLimit == 0)
            {
                return false;
            }

            return IngredientsTrashAmount.Values.Sum() > _wasteLimit;
        }

        public int GetIngredientVolatility()
        {
            return GetVolatility(_maxIngredientVolatility);
        }

        public int GetDishVolatility()
        {
            return GetVolatility(_maxDishVolatility);
        }

        public int GetVolatility(int maxVolatility)
        {
            if (maxVolatility == 0)
            {
                return 0;
            }

            int volatility = Randomizer.GetRandomInt(0, maxVolatility + 1);
            volatility = Randomizer.GetRandomInt() % 2 == 0 ? volatility : volatility * (-1);

            if (volatility <= -100)
            {
                return -100;
            }

            return volatility;
        }

        public List<Food> GetRecommendedRecipeFoods(List<Ingredient> ingredientsRecommended)
        {
            throw new NotImplementedException();
        }

    }
}
