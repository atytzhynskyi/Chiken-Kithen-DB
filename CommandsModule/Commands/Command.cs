using ChikenKithen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandsModule
{
    public abstract class Command
    {
        public readonly string FullCommand;
        public readonly string CommandType;

        protected Kitchen Kitchen;
        protected Hall Hall;

        public string Result { get; protected set; }
        public bool IsAllowed { get; protected set; }
        public Command(Hall hall, Kitchen kitchen, string commandString)
        {
            FullCommand = commandString;
            CommandType = commandString.Split(", ")[0];
            IsAllowed = false;

            Kitchen = kitchen;
            Hall = hall;
        }
        public Command(string commandString)
        {
            FullCommand = commandString;
            CommandType = commandString.Split(", ")[0];
            IsAllowed = false;
        }
        public Command()
        {
            IsAllowed = false;
        }
        public virtual void SetPermision(Dictionary<string, bool> permisions)
        {
            if (!permisions.Keys.Any(x => x == CommandType))
            {
                IsAllowed = false;
                return;
            }
            IsAllowed = permisions[CommandType];
        }
        public virtual void ExecuteCommand(Hall hall, Kitchen kitchen) { Result = "Command dont have implementation"; }
        public virtual void ExecuteCommand() { Result = "Command dont have implementation"; }
    }
}
