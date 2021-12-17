using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace BaseClasses
{
    [Table("Customers")]
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [NotMapped]
        public int VisitsCount { get; set; }
        [NotMapped]
        public Food Order { get; set; }
        public double budget { get; set; }
        [NotMapped]
        public List<Ingredient> Allergies { get; set; } = new List<Ingredient>();
        public Customer() { }

        public Customer(string _Name)
        {
            Name = _Name;
        }
        public Customer(string _Name, params Ingredient[] _Allergies)
        {
            Name = _Name;
            Allergies.AddRange(_Allergies);
        }
        public Customer(string _Name, Food _Order, params Ingredient[] _Allergies)
        {
            Name = _Name;
            Order = _Order;
            Allergies.AddRange(_Allergies);
        }
        public void SetOrder(List<Food> menu, Food _Order)
        {
            foreach (Food food in menu)
            {
                if (_Order.Name == food.Name)
                {
                    Order = food;
                    return;
                }
            }
            Console.WriteLine("Order doesnt exist in menu");
        }
        public (bool, Ingredient) isAllergic(List<Food> recipes, Food food)
        {
            food = recipes.Where(r => r.Name == food.Name).FirstOrDefault();
            foreach (var recipe in from Food recipe in recipes
                                   from Food recipeFood in food.RecipeFoods
                                   where recipe.Name == recipeFood.Name
                                   where isAllergic(recipes, recipe).Item1
                                   select recipe)
            {
                return (true, isAllergic(recipes, recipe).Item2);
            }
            foreach (Ingredient ingredient in food.RecipeIngredients)
            {
                if (Allergies.Any(a => a.Name == ingredient.Name))
                {
                    return (true, ingredient);
                }
            }
            return (false, new Ingredient("NULL"));
        }
    }
}
