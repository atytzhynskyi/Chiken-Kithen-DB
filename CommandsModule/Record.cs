using AdvanceClasses;
using BaseClasses;
using System.Collections.Generic;
using System.Linq;

namespace CommandsModule
{
    public class Record
    {
        public ICommand Command;

        public Dictionary<Food, int> FoodsAmount = new Dictionary<Food, int>();
        public Dictionary<Ingredient, int> IngredientsAmount = new Dictionary<Ingredient, int>();
        public double Budget;
        public double Tip;

        public Record(ICommand command, Kitchen kitchen, Accounting accounting)
        {
            Command = command;
            FoodsAmount = kitchen.Storage.FoodAmount.ToDictionary(x => x.Key, x => x.Value);
            IngredientsAmount = kitchen.Storage.IngredientsAmount.ToDictionary(x => x.Key, x => x.Value);
            Budget = accounting.Budget;
            Tip = accounting.CollectedTip;
        }
        public Record(Accounting accounting, Kitchen kitchen)
        {
            FoodsAmount = kitchen.Storage.FoodAmount.ToDictionary(x => x.Key, x => x.Value);
            IngredientsAmount = kitchen.Storage.IngredientsAmount.ToDictionary(x => x.Key, x => x.Value);
            Budget = accounting.Budget;
            Tip = accounting.CollectedTip;
        }
    }
}
