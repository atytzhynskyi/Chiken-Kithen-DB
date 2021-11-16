using BaseClasses;
using ChikenKithen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandsModule
{
    public class Record
    {
        public Command Command;

        public Dictionary<Food, int> FoodsAmount = new Dictionary<Food, int>();
        public Dictionary<Ingredient, int> IngredientsAmount = new Dictionary<Ingredient, int>();
        public int Budget;

        public Record(Command command)
        {
            Command = command;
            FoodsAmount = command.Kitchen.Storage.FoodAmount.ToDictionary(x => x.Key, x => x.Value);
            IngredientsAmount = command.Kitchen.Storage.IngredientsAmount.ToDictionary(x => x.Key, x => x.Value);
            Budget = command.Kitchen.Budget;
        }
        public Record(Kitchen kitchen)
        {
            FoodsAmount = kitchen.Storage.FoodAmount.ToDictionary(x => x.Key, x => x.Value);
            IngredientsAmount = kitchen.Storage.IngredientsAmount.ToDictionary(x => x.Key, x => x.Value);
            Budget = kitchen.Budget;
        }
    }
}
