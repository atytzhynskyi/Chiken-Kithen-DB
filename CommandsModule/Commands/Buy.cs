using BaseClasses;
using ChikenKithen;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommandsModule
{
    public class Buy : Command
    {
        public Customer Customer = new Customer();
        public Food Food = new Food();
        public Buy(Hall hall, Kitchen kitchen, string commandString) : base(hall, kitchen, commandString)
        {
            Customer = hall.GetCustomer(commandString.Split(", ")[1]);
            Food = kitchen.Storage.GetRecipeByName(commandString.Split(", ")[2]);
        }
        public override void ExecuteCommand()
        {
            if (!IsAllowed)
            {
                Result = "Command not allowed";
                return;
            }

            if (Customer.Name == "NULL")
            {
                Result = "Customer 404";
                return;
            }
            if (Food.Name == "NULL")
            {
                Result = "Food 404";
                return;
            }
            Customer.Order = Food;
            if (Customer.budget < Kitchen.CalculateFoodMenuPrice(Customer.Order))
            {
                Result = "Can't order: customer dont have enough money";
                return;
            }

            if (!Kitchen.IsEnoughIngredients(Customer.Order))
            {
                Result = "Can't order: dont have enough ingredients";
                return;
            }

            if (Kitchen.Storage.FoodAmount[Customer.Order] <= 0)
            {
                Kitchen.Cook(Customer.Order);
            }
            Hall.GiveFood(Kitchen, Customer);

            if (Customer.isAllergic(Kitchen.Storage.Recipes, Customer.Order).Item1)
            {

                Result = $"Can't eat: allergic to: {Customer.isAllergic(Kitchen.Storage.Recipes, Customer.Order).Item2.Name}";
                return;
            }
            Customer.VisitsCount++;
            Hall.GetPaid(Kitchen, Customer);
            Result = "success";
        }
    }
}
