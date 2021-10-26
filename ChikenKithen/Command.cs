using BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChikenKithen
{
    static class Command
    {
        static public void ExecuteCommand(string command, Kitchen kitchen, Hall hall)
        {
            string commandType = command.Split(',')[0];
            switch (commandType)
            {
                case ("Buy"):
                    string name = command.Split(", ")[1];
                    if (hall.isNewCustomer(name))
                    {
                        return;
                    }
                    Customer customer = hall.GetCustomer(name);
                    Food food = new Food(command.Split(", ")[2]);
                    if (!kitchen.RecipeBook.Recipes.Any(f => f.Name == food.Name))
                    {
                        return;
                    }
                    customer.Order = food;
                    //customer.SetOrder(kitchen.RecipeBook.Recipes, food);

                    if (customer.isAllergic(kitchen.RecipeBook.Recipes, customer.Order).Item1)
                    {
                        Console.WriteLine(command + " -> Can't order: allergic to: " + customer.isAllergic(kitchen.RecipeBook.Recipes, customer.Order).Item2.Name);
                        return;
                    }
                    if (!kitchen.isEnoughIngredients(customer.Order))
                    {
                        Console.WriteLine(command + " -> Can't order: dont have enough ingredients");
                        return;
                    }
                    kitchen.Cook(customer.Order);
                    hall.GiveFood(kitchen, customer);
                    Console.Write(command + " -> success\n");
                    break;
                case ("Order"):
                    Ingredient ingredient = new Ingredient(command.Split(", ")[1]);
                    if(!kitchen.Storage.Ingredients.Any(i=>i.Name == ingredient.Name))
                    {
                        return;
                    }
                    ingredient = kitchen.Storage.Ingredients.Where(i => i.Name == ingredient.Name).First();
                    kitchen.Storage.IngredientsAmount[ingredient]+= Convert.ToInt32(command.Split(", ")[2]);
                    Console.WriteLine(command + " -> success");
                    break;
                case ("Ingredients"):
                    kitchen.Storage.ShowIngredients();
                    break;
                case ("Foods"):
                    kitchen.ShowAll();
                    break;
                default:
                    return;
            }
            Ingredient ingredient1 = new Ingredient("Chicken");
            ingredient1 = kitchen.Storage.Ingredients.Where(i=>i.Name == ingredient1.Name).First();
            kitchen.Storage.IngredientsAmount[ingredient1] = 8;
        }
    }
}
