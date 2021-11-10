using BaseClasses;
using ChikenKithen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandModule
{
    class Record
    {
        public string CommandAndResult;

        public Dictionary<Food, int> FoodsAmount = new Dictionary<Food, int>();
        public Dictionary<Ingredient, int> IngredientsAmount = new Dictionary<Ingredient, int>();
        public int Budget;

        public Record(Kitchen kitchen)
        {
            CommandAndResult = "";
            FoodsAmount = kitchen.FoodAmount.ToDictionary(x => x.Key, x => x.Value);
            IngredientsAmount = kitchen.Storage.IngredientsAmount.ToDictionary(x => x.Key, x => x.Value);
            Budget = kitchen.Budget;
        }
        public Record(Kitchen kitchen, string _CommandAndResult)
        {
            CommandAndResult = _CommandAndResult;
            FoodsAmount = kitchen.FoodAmount.ToDictionary(x => x.Key, x => x.Value);
            IngredientsAmount = kitchen.Storage.IngredientsAmount.ToDictionary(x => x.Key, x => x.Value);
            Budget = kitchen.Budget;
        }
    }
}
