using BaseClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AdvanceClasses;
using jsonReadModule;
using CommandsModule.Commands;

namespace CommandsModule
{
    public static class CommandBuilder
    {
        static public ICommand Build(Accounting accounting, Hall hall, Kitchen kitchen, string commandString, RecordsBase RecordsBase)
        {
            ICommand command;

            string commandType = commandString.Split(", ")[0];

         switch (commandType)
            {
                case ("Buy"):
                    command = new Buy(accounting, hall, kitchen, commandString);
                    break;
                case ("Order"):
                    command = new Order(accounting, kitchen, commandString);
                    break;
                case ("Table"):
                    command = new Table(accounting, hall, kitchen, commandString);
                    break;
                case ("Cook"):
                    command = new Cook(kitchen, commandString);
                    break;
                case ("Customers"):
                    command = new Customers(hall, commandString);
                    break;
                case ("ExecuteFileCommands"):
                    command = new ExecuteFileCommands(accounting, hall, kitchen, commandString, RecordsBase);
                    break;
                case ("Budget"):
                    command = new Budget(accounting, commandString);
                    break;
                case ("Audit"):
                    command = new Audit(commandString, RecordsBase);
                    break;
                case ("End Day"):
                    command = new EndDay(commandString, accounting);
                    break;
                default:
                    command = new Default();
                    break;
            }
            return command;
        }
    }
}
