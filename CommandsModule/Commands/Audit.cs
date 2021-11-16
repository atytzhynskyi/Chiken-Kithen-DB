using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BaseClasses;
using ChikenKithen;

namespace CommandsModule
{
    public class Audit : Command
    {
        RecordsBase RecordsBase;
        public Audit(Hall hall, Kitchen kitchen, string commandString, RecordsBase _RecordsBase) : base(hall, kitchen, commandString)
        {
            RecordsBase = _RecordsBase;
        }
        public void ExecuteCommand()
        {
            if (!IsAllowed)
            {
                Result = "Command not allowed";
                return;
            }

            string fileName = "";
            for (int i = 0; true; i++)
            {
                if (!File.Exists($@"..\..\..\Audit{i}"))
                {
                    fileName = $@"..\..\..\Audit{i}";
                    break;
                }
            }

            using (StreamWriter fs = new StreamWriter(File.Create(fileName)))
            {
                fs.Write("INIT\n");
                foreach (Record record in RecordsBase.Records)
                {
                    fs.WriteLine($"{record.Command.FullCommand} -> {record.Command.Result}");
                    fs.WriteLine(GetWareHouse(record.IngredientsAmount, record.FoodsAmount));
                    fs.WriteLine($"Budget: {record.Budget}");

                    fs.WriteLine("");
                    if (RecordsBase.Records.First() == record)
                    {
                        fs.WriteLine("START\n");
                    }
                }
            }
        }
        
        private string GetWareHouse(Dictionary<Ingredient, int> ingredientsAmount, Dictionary<Food, int> foodsAmount)
        {
            string WareHouse = "Warehouse: ";
            foreach (Ingredient ingredient in ingredientsAmount.Keys)
            {
                WareHouse += $"{ingredient.Name} {ingredientsAmount[ingredient]}, ";
            }
            foreach (Food food in foodsAmount.Keys)
            {
                WareHouse += $"{food.Name} {foodsAmount[food]}, ";
            }
            return WareHouse;
        }
    }
}
