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
            SetResultIfIssues();

            if (!object.Equals(Result, null)) return;

            Customer.Order = Food;
            

            if (kitchen.Storage.FoodAmount[Customer.Order] >= 1)
            {
                hall.GiveFoodFromStorage(kitchen, Customer);
            }
            else
            {
                kitchen.Cook(Customer.Order);
                hall.GiveFood(Customer.Name);
            }

            Customer.VisitsCount++;
            hall.GetPaid(accounting, kitchen.Storage.IngredientsPrice, kitchen.Storage.Recipes, Customer);

            double price = Math.Round(accounting.CalculateFoodMenuPrice(
                                                   kitchen.Storage.Recipes, kitchen.Storage.IngredientsPrice, Customer.Order), 2);
            if (hall.IsDiscountAppliable(Customer))
            {
                price = Math.Round(price * hall.GetDiscount(), 2);
            }
            double tax = accounting.CalculateTransactionTax(price);
            Result = $"{Customer.Name}, {Customer.budget}, {Customer.Order.Name}, {price} -> success; money amount: {price-tax}; tax: {tax};";
        }

        private void SetResultIfIssues()
        {
            if (!IsAllowed)
            {
                Result = "Command not allowed";
                return;
            }

            if (accounting.Budget < 0)
            {
                Result = "RESTAURANT BANKRUPT";
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
            if (Customer.budget < accounting.CalculateFoodMenuPrice(
                                                kitchen.Storage.Recipes, kitchen.Storage.IngredientsPrice, Food))
            {
                Result = "Can't order: customer dont have enough money";
                return;
            }

            if (Customer.budget < accounting.CalculateFoodMenuPrice(
                                                kitchen.Storage.Recipes, kitchen.Storage.IngredientsPrice, Food))
            {
                Result = "Can't order: customer dont have enough money";
                return;
            }

            if (!kitchen.IsEnoughIngredients(Food))
            {
                Result = "Can't order: dont have enough ingredients";
                return;
            }

            if (Customer.isAllergic(kitchen.Storage.Recipes, Food).Item1)
            {

                Result = $"Can't eat: allergic to: {Customer.isAllergic(kitchen.Storage.Recipes, Customer.Order).Item2.Name}";
                return;
            }
        }
    }
}
