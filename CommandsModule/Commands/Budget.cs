﻿using AdvanceClasses;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommandsModule
{
    class Budget : ICommand
    {
        public string Result { get; private set; }
        public string FullCommand { get; private set; }
        public string CommandType { get; private set; }
        public bool IsAllowed { get; set; }
        private Accounting accounting { get; set; }

        readonly int Amount;

        readonly string Sign;

        public Budget(Accounting Accounting, string _FullCommand)
        {
            accounting = Accounting;

            FullCommand = _FullCommand;
            CommandType = FullCommand.Split(", ")[0];

            Sign = _FullCommand.Split(", ")[1];
            int.TryParse(_FullCommand.Split(", ")[2], out Amount);
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
                accounting.UseMoney(Amount);
                Result = "success";
                return;
            }
            if(Sign == "+")
            {
                accounting.AddMoney(Amount);
                Result = "success";
                return;
            }
            Result = "fail";
        }
    }
}
