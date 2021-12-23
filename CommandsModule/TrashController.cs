using BaseClasses;
using System.Collections.Generic;
using System.Linq;

namespace CommandsModule
{
    public class TrashController
    {
        private TrashHistory _trashHistory;

        public TrashController()
        {
            _trashHistory = new TrashHistory();
        }

        public void SaveHistory(Dictionary<Ingredient, int> trashes, Dictionary<Ingredient, int> ingredientsTrashAmount)
        {
            _trashHistory.AddCurrentTrashInfo(GetReportCurrentTrash(ingredientsTrashAmount));
            _trashHistory.AddTotalTrashInfo(GetReportTotalTrash(trashes));

            _trashHistory.Save();
        }

        private string GetReportCurrentTrash(Dictionary<Ingredient, int> ingredientsTrashAmount)
        {
            var currentTrash = "";

            foreach (var trash in ingredientsTrashAmount)
            {
                if (trash.Value > 0)
                {
                    currentTrash += $"{trash.Key.Name}: {trash.Value}, ";
                }
            }

            return currentTrash;
        }

        private string GetReportTotalTrash(Dictionary<Ingredient, int> trashes)
        {
            var totalTrash = "";
            foreach (var trash in trashes)
            {
                if (trash.Value > 0)
                {
                    totalTrash += $"{trash.Key.Name}: {trash.Value}, ";
                }
            }

            return totalTrash;
        }
    }
}
