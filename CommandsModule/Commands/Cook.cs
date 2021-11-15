﻿using BaseClasses;
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
            if (!IsAllowed)
            {
                Result = "Command not allowed";
                return;
            }

            SetResultIfIssues();
            if(object.Equals(Result, null))
            {
                return;
            }

            for (int i = 0; i < Amount; i++)
            {
                Kitchen.Cook(Food);
            }
            Result = "success";
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
            if (!Kitchen.IsEnoughIngredients(Food))
            {
                Result = "Dont have enough ingredients";
            }
        }
    }
}
