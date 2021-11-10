using BaseClasses;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace ChikenKithen
{
    public class Storage
    {
        public List<Ingredient> Ingredients { get; set; }
        public Dictionary<Ingredient, int> IngredientsAmount { get; set; } = new Dictionary<Ingredient, int>();
        public Dictionary<Ingredient, int> IngredientsPrice { get; set; } = new Dictionary<Ingredient, int>();
        public Storage(List<Ingredient> _Ingredients, Dictionary<Ingredient, int> _IngredientsAmount, Dictionary<Ingredient, int> _IngredientsPrice)
        {
            Ingredients = _Ingredients;
            IngredientsAmount = _IngredientsAmount;
            IngredientsPrice = _IngredientsPrice;
        }
        public Storage(List<Ingredient> _Ingredients, Dictionary<Ingredient, int> _IngredientsAmount)
        {
            Ingredients = _Ingredients;
            IngredientsAmount = _IngredientsAmount;
        }
        public Storage(List<Ingredient> _Ingredients)
        {
            Ingredients = _Ingredients;
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
