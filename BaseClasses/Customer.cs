using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BaseClasses
{
    [Table("Customers")]
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Food Order { get; set; }
        public List<Allergy> Allergies { get; set; } = new List<Allergy>();

        public Customer() { }

        public Customer(string _Name) {
            Name = _Name;
        } 

        public Customer(string _Name, Food _Order, params Allergy[] _Allergies){
            Name = _Name;
            Order = _Order;
            Allergies.AddRange(_Allergies);
        }
        public void SetOrder(List<Food> menu, Ingredient _Order)
        {
            foreach (Food food in menu)
            {
                if (_Order.Name == food.Name)
                {
                    Order.Name = food.Name;
                    return;
                }
            }
            Console.WriteLine("Order doesnt exist in menu");
        }
        public bool isAllergic(List<Food> recipes, Food food)
        {
            foreach (Food _food in recipes)
            {
                if (_food.Name == food.Name)
                {
                    food = _food;
                }
            }
            foreach (RecipeItem recipeItem in food.Recipe)
            {
                foreach (Allergy allergy in Allergies)
                {
                    if (allergy.Ingredient.Name == recipeItem.Ingredient.Name)
                    {
                        return true;
                    }
                }
            }
            foreach (Food _food in recipes)
            {
                foreach (RecipeItem recipeItem in food.Recipe)
                {
                    if (_food.Name == recipeItem.Ingredient.Name)
                    {
                        if (isAllergic(recipes, _food))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
