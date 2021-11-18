using AdvanceClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandsModule
{
    public interface ICommand
    {
        string FullCommand { get;}
        string CommandType { get;}
        bool IsAllowed { get; }
        string Result { get; }
        void ExecuteCommand();
    }

    public abstract class Command
    {
        public readonly string FullCommand;
        public readonly string CommandType;

        public readonly Kitchen kitchen;
        public readonly Hall hall;
        public readonly Accounting accounting;
        public string Result { get; protected set; }
        public bool IsAllowed { get; protected set; }

        public Command(Accounting accounting, Hall hall, Kitchen kitchen, string commandString, Dictionary<string, bool> permisions)
        {
            FullCommand = commandString;
            CommandType = commandString.Split(", ")[0];

            this.kitchen = kitchen;
            this.hall = hall;
            this.accounting = accounting;

            SetPermision(permisions);
        }

        public Command(Hall hall, Kitchen kitchen, string commandString, Dictionary<string, bool> permisions)
        {
            FullCommand = commandString;
            CommandType = commandString.Split(", ")[0];

            this.kitchen = kitchen;
            this.hall = hall;

            SetPermision(permisions);
        }

        public Command(Accounting accounting, Hall hall, Kitchen kitchen, string commandString)
        {
            FullCommand = commandString;
            CommandType = commandString.Split(", ")[0];
            IsAllowed = false;

            this.kitchen = kitchen;
            this.hall = hall;
            this.accounting = accounting;
        }
        public Command(Accounting accounting, Kitchen kitchen, string commandString)
        {
            FullCommand = commandString;
            CommandType = commandString.Split(", ")[0];
            IsAllowed = false;

            this.kitchen = kitchen;
            this.accounting = accounting;
        }
        public Command(Accounting accounting, string commandString)
        {
            FullCommand = commandString;
            CommandType = commandString.Split(", ")[0];
            IsAllowed = false;

            this.accounting = accounting;
        }
        public Command(Kitchen kitchen, string commandString)
        {
            FullCommand = commandString;
            CommandType = commandString.Split(", ")[0];
            IsAllowed = false;
        }
        public Command(Hall hall, string commandString)
        {
            FullCommand = commandString;
            CommandType = commandString.Split(", ")[0];
            IsAllowed = false;

            this.hall = hall;
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
        public virtual void ExecuteCommand() { Result = "Command dont have implementation"; }
    }
}
