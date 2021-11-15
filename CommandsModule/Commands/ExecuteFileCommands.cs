using ChikenKithen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CommandsModule.Commands
{
    public class ExecuteFileCommands : Command
    {
        readonly string FileName;
        List<Command> Commands = new List<Command>();
        public ExecuteFileCommands(Hall Hall, Kitchen Kitchen, string FullCommand) : base(Hall, Kitchen, FullCommand)
        {
            FileName = @"..\..\..\Commands.csv";
        }
        public override void ExecuteCommand()
        {
            ReadCommandFromFile();
            foreach(Command command in Commands)
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
                        Commands.Add(CommandBuilder.Build(Hall, Kitchen, readLine));
                    }
                }
            }
        }
    }
}
