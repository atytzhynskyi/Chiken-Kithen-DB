using AdvanceClasses;
using BaseClasses;
using System;

namespace CommandsModule
{
    public class Buy : ICommand
    {
        public string Result { get; private set; }
        public string FullCommand { get; private set; }
        public string CommandType { get; private set; }
        public bool IsAllowed { get; set; }

        public string AllergicConfig { get; set; }

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
            Food = kitchen.Storage.GetRecipe(_FullCommand.Split(", ")[2]);
        }
        public void ExecuteCommand()
        {
            SetResultIfIssues();

            if (!object.Equals(Result, null)) return;

            Customer.Order = Food;

            Customer.VisitsCount++;

            GiveCustomersOrder();

            double price = Math.Round(accounting.CalculateFoodMenuPrice(
                                                   kitchen.Storage.Recipes, Customer.Order), 2);
            if (hall.IsDiscountAppliable(Customer))
            {
                price = Math.Round(price * hall.GetDiscountValueFromFile(), 2);
            }

            if (Customer.isAllergic(kitchen.Storage.Recipes, Food).Item1)
            {
                ExecuteAllergicBuy(price);
                return;
            }

            var customerBudgetOld = Customer.budget;

            var tip = accounting.IsTip() ? accounting.GetTip(price) : 0;


            hall.GetPaid(accounting, kitchen.Storage.Recipes, Customer, tip);
            tip = price + tip < customerBudgetOld ? tip : customerBudgetOld - price;
            var tax = accounting.CalculateTransactionTax(price);

            Result = $"{Customer.Name}, {Math.Round(Customer.budget + price + tip, 2)}, {Customer.Order.Name}, {price} -> success; money amount: {Math.Round(price - tax + tip, 2)}; tax: {tax}; tip {tip}";
        }

        private void ExecuteAllergicBuy(double price)
        {
            //If a number is set in the config, then we determine whether to keep or waste the dish
            if (int.TryParse(AllergicConfig, out int keepPrice))
            {
                if (keepPrice >= price)
                {
                    AllergicConfig = "keep";
                }
                else AllergicConfig = "waste";
            }


            string allergicResult;
            switch (AllergicConfig)
            {
                case ("keep"):
                    KeepAlergicOrder();
                    allergicResult = "dish keeped";
                    break;

                case ("waste"): //If in a config something else to consider that it is "waste"
                default:
                    allergicResult = "dish wasted";
                    break;
            }
            Result = $"Can't eat: allergic to: {Customer.isAllergic(kitchen.Storage.Recipes, Customer.Order).Item2.Name}; {allergicResult}";
            return;
        }

        private void KeepAlergicOrder()
        {
            kitchen.Storage.FoodAmount[Food]++;
            double costPrice = accounting.CalculateFoodCostPrice(kitchen.Storage.Recipes, Food);
            accounting.UseMoneyWithoutTax(costPrice / 25);
        }

        private void GiveCustomersOrder()
        {
            if (kitchen.Storage.FoodAmount[Customer.Order] >= 1)
            {
                hall.GiveFoodFromStorage(kitchen, Customer);
            }
            else
            {
                kitchen.Cook(Customer.Order);
                hall.GiveFood(Customer.Name);
            }
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

            if (object.Equals(Customer, null))
            {
                Result = "Customer 404";
                return;
            }
            if (object.Equals(Food, null))
            {
                Result = "Food 404";
                return;
            }

            if (Customer.budget < accounting.CalculateFoodMenuPrice(
                                                kitchen.Storage.Recipes, Food))
            {
                Result = "Can't order: customer dont have enough money";
                return;
            }

            //if it's food and it present in storage it means that we can buy it without checking all ingredients
            if (kitchen.Storage.FoodAmount[Food] >= 1)
            {
                return;
            }

            //check of ingredients on spoiling (point 6.8.2)
            //after it might will not be able enough ingredient
            kitchen.CheckSpoilIngredient(Food);

            //check it again
            if (!kitchen.IsEnoughIngredients(Food))
            {
                Result = "Can't order: dont have enough ingredients";
                return;
            }
        }

    }
}
