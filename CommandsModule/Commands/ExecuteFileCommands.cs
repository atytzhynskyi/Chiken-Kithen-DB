using AdvanceClasses;
using System.Collections.Generic;
using System.IO;

namespace CommandsModule.Commands
{
    public class ExecuteFileCommands : ICommand
    {
        public string FullCommand { get; private set; }
        public string CommandType { get; private set; }
        public string Result { get; private set; }
        public bool IsAllowed { get; set; }

        private Accounting accounting { get; set; }
        private Kitchen kitchen { get; set; }
        private Hall hall { get; set; }

        readonly string FileName;
        RecordsBase records;
        List<ICommand> Commands = new List<ICommand>();
        public ExecuteFileCommands(Accounting Accounting, Hall Hall, Kitchen Kitchen, string _FullCommand, RecordsBase _records)
        {
            accounting = Accounting;
            hall = Hall;
            kitchen = Kitchen;

            FullCommand = _FullCommand;
            CommandType = FullCommand.Split(", ")[0];

            FileName = @"..\..\..\Configs\Commands.csv";
            records = _records;
        }
        public void ExecuteCommand()
        {
            ReadCommandFromFile();
            foreach(ICommand command in Commands)
            {
                command.ExecuteCommand();
            }
        }
        private void ReadCommandFromFile()
        {
            using (StreamReader sr = new StreamReader(FileName))
            {
                string readLine;
                while ((readLine = sr.ReadLine()) != null)
                {
                    if (readLine != "ExecuteFileCommands")
                    {
                        Commands.Add(CommandBuilder.Build(accounting, hall, kitchen, readLine, records));
                    }
                }
            }
        }
    }
}
