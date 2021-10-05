using System;
using System.Collections.Generic;
using System.Text;

namespace Chiken_Kithen_DB
{
    class Menu
    {
        public List<Food> Foods = new List<Food>();
        public List<Food> GetFoods() => Foods;
        public Menu()
        {
            FillFoodsRecipe();
        }
        public Menu(List<Food> _Foods)
        {
            Foods.AddRange(_Foods);
        }
        private void FillFoodsRecipe()
        {
            Foods.Add(new Food("Emperor Chicken"));
            Foods.Add(new Food("Fat Cat Chiken"));
            Foods.Add(new Food("Princess Chicken"));
            Foods.Add(new Food("Youth Sauce"));
            Foods.Add(new Food("Spicy Sauce"));
            Foods.Add(new Food("Omega Sauce"));
            Foods.Add(new Food("Diamond Salad"));
            Foods.Add(new Food("Ruby Salad"));
            Foods.Add(new Food("Fries"));
            Foods.Add(new Food("Smashed Potatoes"));
            Foods.Add(new Food("Tuna Cake"));
            Foods.Add(new Food("Fish In Water"));
        }
        public void AddNewFood()
        {
            Console.WriteLine("What is name of new food?");
            string foodName = Console.ReadLine();
            Food food = new Food(foodName);
            Console.WriteLine("What are in the recipe? (please use ',' between ingredients)");
            string[] ingredientsRecipeSplit = Console.ReadLine().Split(", ");
            List<Ingredient> ingredientsRecipe = new List<Ingredient>();
            foreach (string ingredientNameStr in ingredientsRecipeSplit)
            {
                ingredientsRecipe.Add(new Ingredient(ingredientNameStr));
            }
            food.Recipe.AddRange(ingredientsRecipe);
            Foods.Add(food);
            Console.WriteLine(food.Name + " added to the menu");
        }
        public void ShowAll()
        {
            foreach (Food food in Foods)
            {
                Console.WriteLine(food.Name);
            }
        }
    }
}
