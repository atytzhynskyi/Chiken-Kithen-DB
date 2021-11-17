using BaseClasses;
using ChikenKithen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandsModule.Commands
{
    class Customers : Command
    {
        public Customers(Hall Hall, Kitchen Kitchen, string _FullCommand) : base (Hall, Kitchen, _FullCommand) { }

        public override void ExecuteCommand()
        {
            foreach (Customer customer in hall.Customers)
            {
                Console.WriteLine("Name: {0}", customer.Name);
                Console.WriteLine("Budget: {0}", customer.budget);
                Console.Write("Allergies: ");
                foreach (Ingredient ingredient in customer.Allergies.Distinct())
                {
                    Console.Write("{0}, ", ingredient.Name);
                }
                Console.Write("\n\n");
            }
        }
    }
}
