using AdvanceClasses;
using BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandsModule
{
    public class RecordsBase
    {
        public List<Record> Records = new List<Record>();
        public RecordsBase(Accounting accounting, Kitchen kitchen)
        {
            Records.Add(new Record(accounting, kitchen));
        }
        public void AddRecordIfSomeChange(ICommand command, Kitchen kitchen, Accounting accounting)
        {
            if (!IsSomethingChanged(kitchen, accounting))
            {
                return;
            }
            Records.Add(new Record(command, kitchen, accounting));
        }
        public bool IsSomethingChanged(Kitchen kitchen, Accounting accounting)
        {
            if (accounting.Budget != Records.Last().Budget)
            {
                return true;
            }

            if (IsFoodsChanged(kitchen.Storage.FoodAmount))
            {
                return true;
            }

            if (IsIngredientsChanged(kitchen.Storage.IngredientsAmount))
            {
                return true;
            }

            return false;
        }
        public bool IsFoodsChanged(Dictionary<Food, int> foodsAmount)
        {
            foreach (Food food in Records.Last().FoodsAmount.Keys)
            {
                if (Records.Last().FoodsAmount[food] != foodsAmount[food])
                {
                    return true;
                }
            }
            return false;
        }
        public bool IsIngredientsChanged(Dictionary<Ingredient, int> ingredientsAmount)
        {
            foreach (Ingredient ingredient in Records.Last().IngredientsAmount.Keys)
            {
                if (Records.Last().IngredientsAmount[ingredient] != ingredientsAmount[ingredient])
                {
                    return true;
                }
            }
            return false;
        }
    }
}
