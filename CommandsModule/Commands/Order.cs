using BaseClasses;
using ChikenKithen;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommandsModule
{
    public class Order : Command
    {
        public Ingredient Ingredient;
        public int Amount;
        public Order(Hall hall, Kitchen kitchen, string _FullCommand) : base(hall, kitchen, _FullCommand)
        {
            Ingredient = kitchen.Storage.GetIngredient(_FullCommand.Split(", ")[1]);
            int.TryParse(_FullCommand.Split(", ")[2], out Amount);
        }
        public override void ExecuteCommand()
        {
            if (!IsAllowed)
            {
                Result = "Command not allowed";
                return;
            }

            int price = Kitchen.Storage.IngredientsPrice[Ingredient] * Amount;
            if (Kitchen.Budget <= price)
            {
                Result = "Dont have enought money";
            }
            Kitchen.UseMoney(price);
            Kitchen.Storage.IngredientsAmount[Ingredient] += Amount;
            Result = "success";
        }
    }
}
