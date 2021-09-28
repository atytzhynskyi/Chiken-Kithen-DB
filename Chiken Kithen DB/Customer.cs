using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Chiken_Kithen_DB
{
    [Table("Customers")]
    class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Food Order { get; set; }
        public List<Ingredient> Allergies { get; set; } = new List<Ingredient>();

        public Customer() { }

        public Customer(string _Name) {
            Name = _Name;
        }

        public Customer(string _Name, Food _Order, params Ingredient[] _Allergies){
            Name = _Name;
            Order = _Order;
            Allergies.AddRange(_Allergies);
        }
    }
}
