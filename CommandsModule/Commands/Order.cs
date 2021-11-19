using AdvanceClasses;
using BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandsModule
{
    public class Order : ICommand
    {
        public string FullCommand { get; private set; }
        public string CommandType { get; private set; }
        public string Result { get; private set; }
        public bool IsAllowed { get; set; }

        public Ingredient Ingredient;
        public int Amount;
        private Accounting accounting { get; set; }
        private Kitchen kitchen { get; set; }
        public Order(Accounting Accounting, Kitchen Kitchen, string _FullCommand)
        {
            FullCommand = _FullCommand;
            CommandType = FullCommand.Split(", ")[0];
            IsAllowed = false;

            kitchen = Kitchen;
            accounting = Accounting;

            Ingredient = kitchen.Storage.GetIngredient(FullCommand.Split(", ")[1]);
            int.TryParse(FullCommand.Split(", ")[2], out Amount);
        }
        public void ExecuteCommand()
        {
            if (!IsAllowed)
            {
                Result = "Command not allowed";
                return;
            }

            if(object.Equals(Ingredient, null))
            {
                Result = "Ingredient 404";
                return;
            }

            int price = kitchen.Storage.IngredientsPrice[Ingredient] * Amount;
            if (accounting.Budget <= price)
            {
                Result = "Dont have enought money";
                return;
            }
            accounting.UseMoney(price);
            kitchen.Storage.AddIngredient(Ingredient.Name, Amount);
            Result = $"success; money used:{price + accounting.CalculateTransactionTax(price)}; tax:{accounting.CalculateTransactionTax(price)}";
        }
    }
}
