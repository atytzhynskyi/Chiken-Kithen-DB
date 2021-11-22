using AdvanceClasses;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommandsModule.Commands
{
    class EndDay : ICommand
    {
        public string FullCommand { get; }

        public string CommandType { get; }

        public bool IsAllowed { get; set; }

        public string Result { get; private set; }
        public Accounting accounting;

        public EndDay(string _FullCommand, Accounting accounting)
        {
            this.accounting = accounting;
            FullCommand = _FullCommand;
            CommandType = FullCommand.Split(", ")[0];
        }

        public void ExecuteCommand()
        {
            Result = $"Current budget:{accounting.Budget}, Start budget:{accounting.GetStartBudget()}, Tax:{accounting.CalculateDailyTax()}";
            accounting.PayDayTax();
        }
    }
}
