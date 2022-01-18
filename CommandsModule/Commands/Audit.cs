using System.Collections.Generic;
using System.IO;
using System.Linq;
using BaseClasses;

namespace CommandsModule
{
    public class Audit : ICommand
    {
        public string FullCommand { get; private set; }
        public string CommandType { get; private set; }
        public string Result { get; private set; }
        public bool IsAllowed { get; set; }
        RecordsBase RecordsBase;
        public Audit(string _FullCommand, RecordsBase _RecordsBase)
        {
            FullCommand = _FullCommand;
            CommandType = FullCommand.Split(", ")[0];

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
                if (!File.Exists($@"..\..\..\Audits\Audit{i}"))
                {
                    fileName = $@"..\..\..\Audits\Audit{i}";
                    break;
                }
            }

            using (StreamWriter fs = new StreamWriter(File.Create(fileName)))
            {
                fs.Write("INIT\n");
                foreach (Record record in RecordsBase.Records)
                {
                    if(RecordsBase.Records.First() != record)
                        fs.WriteLine($"{record.Command.FullCommand} -> {record.Command.Result}");

                    fs.WriteLine(GetWareHouse(record.IngredientsAmount, record.FoodsAmount));
                    fs.WriteLine($"Budget: {record.Budget}");

                    fs.WriteLine("");
                    if (RecordsBase.Records.First() == record)
                    {
                        fs.WriteLine("START\n");
                    }
                }
                fs.WriteLine("\nEND");
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
