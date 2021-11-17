using BaseClasses;
using ChikenKithen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandsModule
{
    public class Cook : Command
    {
        readonly Food Food;
        readonly int Amount;
        public Cook(Hall hall, Kitchen kitchen, string _FullCommand) : base(hall, kitchen, _FullCommand)
        {
            Food = kitchen.Storage.GetRecipeByName(_FullCommand.Split(", ")[1]);
            int.TryParse(_FullCommand.Split(", ")[2], out Amount);
        }
        public override void ExecuteCommand()
        {
            int cookedAmount = 0;

            if (!IsAllowed)
            {
                Result = "Command not allowed";
                return;
            }

            SetResultIfIssues();
            if(!object.Equals(Result, null))
            {
                return;
            }

            for (int i = 0; i < Amount; i++)
            {
                if (kitchen.Cook(Food))
                {
                    cookedAmount++;
                    Result = "success";
                }
                else
                {
                    Result = "Failed to cook food";
                    break;
                }
            }
            kitchen.Storage.AddFood(Food.Name, cookedAmount);
        }
        private void SetResultIfIssues()
        {
            if (object.Equals(Food, null))
            {
                Result = "Food 404";
            }
            if (object.Equals(Amount, null))
            {
                Result = "Amount incorrect";
            }
            if (!kitchen.IsEnoughIngredients(Food))
            {
                Result = "Dont have enough ingredients";
            }
        }
    }
}
