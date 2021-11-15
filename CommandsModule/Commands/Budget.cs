using ChikenKithen;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommandsModule
{
    class Budget : Command
    {
        readonly int Amount;
        readonly string Sign;
        public Budget(Hall hall, Kitchen kitchen, string _FullCommand) : base(hall, kitchen, _FullCommand)
        {
            Sign = _FullCommand.Split(", ")[1];
            int.TryParse(_FullCommand.Split(", ")[2], out Amount);
        }
        public override void ExecuteCommand()
        {
            if(Sign == "=")
            {
                Kitchen.SetMoney(Amount);
                Result = "success";
                return;
            }
            if(Sign == "-")
            {
                Kitchen.UseMoney(Amount);
                Result = "success";
                return;
            }
            if(Sign == "+")
            {
                Kitchen.AddMoney(Amount);
                Result = "success";
                return;
            }
            Result = "fail";
        }
    }
}
