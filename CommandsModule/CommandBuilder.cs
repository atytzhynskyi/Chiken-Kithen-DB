﻿using BaseClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AdvanceClasses;
using ChikenKithen;
using jsonReadModule;
using CommandsModule.Commands;

namespace CommandsModule
{
    public static class CommandBuilder
    {
        static public Command Build(Hall hall, Kitchen kitchen, string commandString, RecordsBase RecordsBase)
        {
            Command command;

            string commandType = commandString.Split(", ")[0];

            switch (commandType)
            {
                case ("Buy"):
                    command = new Buy(hall, kitchen, commandString);
                    break;
                case ("Order"):
                    command = new Order(hall, kitchen, commandString);
                    break;
                case ("Table"):
                    command = new Table(hall, kitchen, commandString);
                    break;
                case ("Cook"):
                    command = new Cook(hall, kitchen, commandString);
                    break;
                case ("Customers"):
                    command = new Customers(hall, kitchen, commandString);
                    break;
                case ("ExecuteFileCommands"):
                    command = new ExecuteFileCommands(hall, kitchen, commandString);
                    break;
                case ("Budget"):
                    command = new Budget(hall, kitchen, commandString);
                    break;
                case ("Audit"):
                    command = new Audit(hall, kitchen, commandString, RecordsBase);
                    break;
                default:
                    command = new Table(hall, kitchen, "Table, ");
                    break;
            }
            return command;
        }
    }
}
