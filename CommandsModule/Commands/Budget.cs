using AdvanceClasses;

namespace CommandsModule
{
    public class Budget : ICommand
    {
        public string Result { get; private set; }
        public string FullCommand { get; private set; }
        public string CommandType { get; private set; }
        public bool IsAllowed { get; set; }
        private Accounting accounting { get; set; }

        readonly double Amount;

        readonly string Sign;

        public Budget(Accounting Accounting, string _FullCommand)
        {
            accounting = Accounting;

            FullCommand = _FullCommand;
            CommandType = FullCommand.Split(", ")[0];

            Sign = _FullCommand.Split(", ")[1];
            double.TryParse(_FullCommand.Split(", ")[2], out Amount);
        }
        public void ExecuteCommand()
        {
            if(Sign == "=")
            {
                accounting.SetMoney(Amount);
                Result = "success";
                return;
            }
            if(Sign == "-")
            {
                accounting.UseMoneyWithoutTax(Amount);
                Result = "success";
                return;
            }
            if(Sign == "+")
            {
                accounting.AddMoneyWithoutTax(Amount);
                Result = "success";
                return;
            }
            Result = "fail";
        }
    }
}
