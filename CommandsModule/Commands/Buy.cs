using AdvanceClasses;
using BaseClasses;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommandsModule
{
    public class Buy : ICommand
    {
        public string Result { get; private set; }
        public string FullCommand { get; private set; }
        public string CommandType { get; private set; }
        public bool IsAllowed { get; set; }


        private Accounting accounting { get; set; }
        private Kitchen kitchen { get; set; }
        private Hall hall { get; set; }

        public Customer Customer = new Customer();
        public Food Food = new Food();
        public Buy(Accounting Accounting, Hall Hall, Kitchen Kitchen, string _FullCommand)
        {
            accounting = Accounting;
            hall = Hall;
            kitchen = Kitchen;

            FullCommand = _FullCommand;
            CommandType = FullCommand.Split(", ")[0];

            Customer = hall.GetCustomer(_FullCommand.Split(", ")[1]);
            Food = kitchen.Storage.GetRecipeByName(_FullCommand.Split(", ")[2]);
        }
        public void ExecuteCommand()
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
            if (Customer.budget < accounting.CalculateFoodMenuPrice(
                                                kitchen.Storage.Recipes, kitchen.Storage.IngredientsPrice, Customer.Order))
            {
                Result = "Can't order: customer dont have enough money";
                return;
            }

            if (!kitchen.IsEnoughIngredients(Customer.Order))
            {
                Result = "Can't order: dont have enough ingredients";
                return;
            }

            if (kitchen.Storage.FoodAmount[Customer.Order] >= 1)
            {
                hall.GiveFoodFromStorage(kitchen, Customer);
            }
            else
            {
                kitchen.Cook(Customer.Order);
                hall.GiveFood();
            }

            if (Customer.isAllergic(kitchen.Storage.Recipes, Customer.Order).Item1)
            {

                Result = $"Can't eat: allergic to: {Customer.isAllergic(kitchen.Storage.Recipes, Customer.Order).Item2.Name}";
                return;
            }

            Customer.VisitsCount++;
            hall.GetPaid(accounting, kitchen.Storage.IngredientsPrice, kitchen.Storage.Recipes, Customer);
            Result = "success";
        }
    }
}
