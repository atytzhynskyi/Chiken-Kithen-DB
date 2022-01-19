using System.Linq;
using AdvanceClasses;
using CommandsModule.Commands;

namespace CommandsModule
{
    public static class CommandBuilder
    {
        static public ICommand Build(Accounting accounting, Hall hall, Kitchen kitchen, string commandString, RecordsBase RecordsBase)
        {
            ICommand command;

            string commandType = commandString.Split(", ")[0];


            if (kitchen.Storage.IsRestaurantPoisoned())
            {
                if (commandType == "Throw trash away")
                {
                    return new ThrowTrashAway(kitchen, commandString);
                }

                return new Default("RESTAURANT POISONED");
            }

         switch (commandType)
            {
                case ("Buy"):
                    command = new Buy(accounting, hall, kitchen, commandString);
                    break;
                case ("Order"):
                    command = new Order(accounting, kitchen, commandString, jsonReadModule.JsonRead.ReadFromJson<string>(@"..\..\..\Configs\OrderConfig.json").First().Value);
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
                    command = new EndDay(commandString, accounting, kitchen);
                    break;
                case ("Throw trash away"):
                    command = new ThrowTrashAway(kitchen, commandString);
                    break;
                default:
                    command = new Default("Command not found");
                    break;
            }
            return command;
        }
    }
}
