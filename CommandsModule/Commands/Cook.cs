using AdvanceClasses;
using BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandsModule
{
    public class Cook : ICommand
    {
        public string FullCommand { get; private set; }
        public string CommandType { get; private set; }
        public string Result { get; private set; }
        public bool IsAllowed { get; set; }
        private Kitchen kitchen { get; set; }
        readonly Food Food;
        readonly int Amount;
        public Cook(Kitchen Kitchen, string _FullCommand)
        {
            kitchen = Kitchen;

            FullCommand = _FullCommand;
            CommandType = FullCommand.Split(", ")[0];

            Food = kitchen.Storage.GetRecipeByName(_FullCommand.Split(", ")[1]);
            int.TryParse(_FullCommand.Split(", ")[2], out Amount);
        }
        public void ExecuteCommand()
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
