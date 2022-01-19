using System.IO;

namespace CommandsModule
{
    public class TrashHistory
    {
        private readonly string _path;
        private string _history;

        private string _currentInfo;
        private string _totalInfo;

        public TrashHistory()
        {
            //var _path = $@"..\..\..\TrashHistory.txt";
            _path = $@"TrashHistory.txt";

            _history = "";
        }

        private string GetCurrentReportInfo()
        {
            return _currentInfo + "\n" + _totalInfo + "\n\n";
        }

        public void AddCurrentTrashInfo(string info)
        {
            _currentInfo = "Current trash: " + info + "\n";
        }

        public void AddOldTrashInfo(string info)
        {
            _totalInfo = "Old trash: " + info + "\n";
        }

        public void Save()
        {
            ReadFile();

            _history += GetCurrentReportInfo();
            WriteFile(_history);
        }

        public void Delete()
        {
            WriteFile("");
        }

        private void ReadFile()
        {
            if (_history != string.Empty)
            {
                return;
            }

            if (!File.Exists(_path))
            {
                WriteFile("");
            }

            _history = File.ReadAllText(_path);
        }

        private void WriteFile(string data)
        {
            File.WriteAllText(_path, data);
        }
    }
}
