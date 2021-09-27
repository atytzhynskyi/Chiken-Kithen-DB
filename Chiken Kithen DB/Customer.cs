using System;
using System.Collections.Generic;
using System.Text;

namespace Chiken_Kithen_DB
{
    class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Food Order { get; set; }
        public List<Ingredient> Allergies { get; set; } = new List<Ingredient>();
        public Customer() { }
        public Customer(string _Name, string OrderName, params string[] Allergies)
        {

        }
    }
}
