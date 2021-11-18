using AdvanceClasses;
using BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandsModule.Commands
{
    class Customers : ICommand
    {
        public string FullCommand { get; private set; }
        public string CommandType { get; private set; }
        public string Result { get; private set; }

        public bool IsAllowed { get; set; }
        private Hall hall { get; set; }
        public Customers(Hall Hall, string _FullCommand) {
            hall = Hall;
            FullCommand = _FullCommand;
            CommandType = FullCommand.Split(", ")[0];
        }

        public void ExecuteCommand()
        {
            foreach (Customer customer in hall.Customers)
            {
                Console.WriteLine("Name: {0}", customer.Name);
                Console.WriteLine("Budget: {0}", customer.budget);
                Console.Write("Allergies: ");
                foreach (Ingredient ingredient in customer.Allergies)
                {
                    Console.Write("{0}, ", ingredient.Name);
                }
                Console.Write("\n\n");

                Result = "success";
            }
        }
    }
}
