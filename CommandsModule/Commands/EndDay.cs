using AdvanceClasses;

namespace CommandsModule.Commands
{
    public class EndDay : ICommand
    {
        public string FullCommand { get; }

        public string CommandType { get; }

        public bool IsAllowed { get; set; }

        public string Result { get; private set; }
        public Accounting Accounting;
        public Kitchen Kitchen;
        public EndDay(string _FullCommand, Accounting accounting, Kitchen kitchen)
        {
            this.Accounting = accounting;
            this.Kitchen = kitchen;
            FullCommand = _FullCommand;
            CommandType = FullCommand.Split(", ")[0];
        }

        public void ExecuteCommand()
        {
            Result = $"Current budget:{Accounting.Budget}, Start budget:{Accounting.GetStartBudget()}, Tax:{Accounting.CalculateDailyTax(Kitchen)}";
            Accounting.PayDayTax(Kitchen);
        }
    }
}
