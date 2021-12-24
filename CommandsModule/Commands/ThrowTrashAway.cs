using AdvanceClasses;
using BaseClasses;
using System.Collections.Generic;
using System.Linq;

namespace CommandsModule
{
    class ThrowTrashAway : ICommand
    {
        public string Result { get; private set; }
        public string FullCommand { get; private set; }
        public string CommandType { get; private set; }
        public bool IsAllowed { get; set; }

        private Kitchen kitchen { get; set; }

        public ThrowTrashAway(Kitchen Kitchen, string _FullCommand)
        {
            kitchen = Kitchen;

            FullCommand = _FullCommand;
            CommandType = FullCommand.Split(", ")[0];
        }

        public void ExecuteCommand()
        {
            Result = "look file \"TrashHistory.txt\"";

            var ingredientsTrashAmount = kitchen.Storage.IngredientsTrashAmount;
            var totalTrashAmount = kitchen.Storage.TotalTrashAmount;

            totalTrashAmount = AddCurrentTrashToTotalTrash(ingredientsTrashAmount, totalTrashAmount);
            SaveHistory(totalTrashAmount, ingredientsTrashAmount);
            ClearCurrentTrash(ingredientsTrashAmount);
        }

        private Dictionary<Ingredient, int> ClearCurrentTrash(Dictionary<Ingredient, int> ingredientsTrashAmount)
        {
            Dictionary<Ingredient, int> ingredientsTrashAmountCopy = new Dictionary<Ingredient, int>(ingredientsTrashAmount.Where(d => d.Value > 0));
            foreach (var item in ingredientsTrashAmountCopy)
            {
                ingredientsTrashAmount[item.Key] = 0;
            }
            return ingredientsTrashAmount;
        }

        private Dictionary<Ingredient, int> AddCurrentTrashToTotalTrash(Dictionary<Ingredient, int> ingredientsTrashAmount, Dictionary<Ingredient, int> totalTrashAmount)
        {
            foreach (var item in ingredientsTrashAmount.Where(d => d.Value > 0))
            {
                if (totalTrashAmount.ContainsKey(item.Key))
                {
                    totalTrashAmount[item.Key] += item.Value;
                    continue;
                }

                totalTrashAmount.Add(item.Key, item.Value);
            }

            return totalTrashAmount;
        }

        public void SaveHistory(Dictionary<Ingredient, int> trash, Dictionary<Ingredient, int> ingredientsTrashAmount)
        {
            var trashHistory = new TrashHistory();

            trashHistory.AddCurrentTrashInfo(GetReportCurrentTrash(ingredientsTrashAmount));
            trashHistory.AddTotalTrashInfo(GetReportTotalTrash(trash));

            trashHistory.Save();
        }

        private string GetReportCurrentTrash(Dictionary<Ingredient, int> ingredientsTrashAmount)
        {
            var currentTrash = "";

            foreach (var trash in ingredientsTrashAmount.Where(d => d.Value > 0))
            {
                currentTrash += $"{trash.Key.Name}: {trash.Value}, ";
            }
            return currentTrash;
        }

        private string GetReportTotalTrash(Dictionary<Ingredient, int> trash)
        {
            var totalTrash = "";
            foreach (var item in trash.Where(d => d.Value > 0))
            {
                totalTrash += $"{item.Key.Name}: {item.Value}, ";
            }
            return totalTrash;
        }

    }
}
