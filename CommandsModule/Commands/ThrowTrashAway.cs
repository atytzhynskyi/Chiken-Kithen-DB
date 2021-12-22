using AdvanceClasses;
using System;

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
            kitchen.Storage.ThrowTrashAway = true;
            Result = "look file \"TrashHistory.txt\"";
        }
    }
}
