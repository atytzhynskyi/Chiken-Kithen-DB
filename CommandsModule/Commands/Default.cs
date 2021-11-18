using System;
using System.Collections.Generic;
using System.Text;

namespace CommandsModule
{
    class Default : ICommand
    {
        public string FullCommand { get; private set; }

        public string CommandType { get; private set; }

        public bool IsAllowed { get; private set; }

        public string Result { get; private set; }
        public Default() { }
        public void ExecuteCommand()
        {
            Result = "Command not found";
        }
    }
}
